using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("Metodologias")]
    public class Metodología : Contextual {
        internal bool parseosVálidos;

        [Key]
        public int MetodologíaID { get; set; }
        [Required]
        public int UsuarioID { get; set; }
        [Required, Index(IsUnique = true), StringLength(255, ErrorMessage = "El nombre de una metodología no puede superar los 255 caracteres. ")]
        public string Nombre { get; set; }
        public virtual ICollection<Condición> Condiciones { get; set; }

        internal static Metodología Get(string nombre) {
            return (from m in db.Metodologías
                   where m.Nombre == nombre
                   select m).SingleOrDefault();
        }

        internal static List<string> GetNombres(int usuarioID) {
            return (from m in db.Metodologías
                    where m.UsuarioID == usuarioID
                    select m.Nombre).ToList();
        }

        internal static bool NameTaken(String nomb) {
            return (from m in db.Metodologías
                    where m.Nombre == nomb
                    select m.UsuarioID).Count() > 0;
        }

        internal void BorrarConds() {
            db.Condiciones.RemoveRange(
                db.Condiciones.Where(c => c.MetodologíaID == MetodologíaID)
            );
        }

        internal void Borrar() {
            BorrarConds();
            db.Metodologías.Remove(this);
            Save();
        }

        internal void Save() {
            db.SaveChanges();
        }

        internal Metodología(int usuarioID, string nombre, string fórmula) {
            UsuarioID = usuarioID;
            Nombre = nombre;
            db.Metodologías.Add(this);
            db.SaveChanges();
            NuevaFórmula(fórmula);
        }

        internal void NuevaFórmula(string fórmula) {
            var formulasCondiciones = new List<string>(fórmula.Split('&'));
            foreach (var form in formulasCondiciones)
                new Condición(form, MetodologíaID);
        }

        private Metodología() { }

        internal string Fórmula() {
            var condiciones = from c in db.Condiciones
                              where c.MetodologíaID == MetodologíaID
                              select c.Fórmula;
            return string.Join("&", condiciones.ToArray());
        }

        internal bool CumpleCondiciones(Dictionary<string, Cuenta> cuentas) {
            bool cumple = true;
            parseosVálidos = true;
            var condiciones = from c in db.Condiciones
                              where c.MetodologíaID == MetodologíaID
                              select c;

            foreach (var cond in condiciones) {
                Parser parser = new Parser(cond.Fórmula);
                if (cond.EsCreciente) {
                    Empresa e = cuentas.First().Value.Empresa;
                    double últimoValor = double.MinValue;
                    foreach (int period in e.GetPeríodosDeEmpresa()) {
                        parser = new Parser(cond.Fórmula);
                        var cuentasEmpresa = e.DiccionarioCuentasDelPeríodo(period);
                        double nuevoValor = parser.CalcularValor(cuentasEmpresa);
                        cumple = nuevoValor > últimoValor;
                        últimoValor = nuevoValor;
                        parseosVálidos &= parser.EsVálido();
                        if (!cumple || !parseosVálidos) break;
                    }
                } else if (cond.EsConsistente) {
                    Empresa e = cuentas.First().Value.Empresa;
                    List<int> períodos = e.GetPeríodosDeEmpresa();
                    List<double> valores = new List<double>();
                    foreach (int period in períodos) {
                        parser = new Parser(cond.Fórmula);
                        var cuentasEmpresa = e.DiccionarioCuentasDelPeríodo(period);
                        valores.Add(parser.CalcularValor(cuentasEmpresa));
                        parseosVálidos &= parser.EsVálido();
                        if (!parseosVálidos) break;
                    }
                    double promedio = valores.Average();
                    double desviaciónTotal = 0;
                    foreach (double v in valores) desviaciónTotal += Math.Abs(v - promedio);
                    desviaciónTotal = desviaciónTotal / valores.Count();
                    cumple = (desviaciónTotal / promedio) < 0.2;
                } else {
                    cumple &= (parser.CalcularValor(cuentas) == 1);
                    parseosVálidos &= parser.EsVálido();
                }
                if (!cumple || !parseosVálidos) break;
            }
            return cumple & parseosVálidos;
        }
    }
}