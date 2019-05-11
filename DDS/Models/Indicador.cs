using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("Indicadores")]
    public class Indicador : Contextual {
        internal Parser parser;

        [Key]
        public int IndicadorID { get; set; }
        [Required]
        public int UsuarioID { get; set; }
        [Required, Index(IsUnique = true), StringLength(255, ErrorMessage = "El nombre de un indicador no puede superar los 255 caracteres. ")]
        public string Nombre { get; set; }
        [Required, StringLength(255, ErrorMessage = "La fórmula de un indicador no puede superar los 255 caracteres. ")]
        public string Fórmula { get; set; }
        
        internal static Indicador Get(string nombre) {
            return (from i in db.Indicadores
                   where i.Nombre == nombre
                   select i).SingleOrDefault();
        }

        internal static Indicador Get(int ID) {
            return (from i in db.Indicadores
                    where i.IndicadorID == ID
                    select i).SingleOrDefault();
        }

        internal static List<string> GetNombres(int usuarioID) {
            return (from i in db.Indicadores
                    where i.UsuarioID == usuarioID
                    select i.Nombre).ToList();
        }

        internal static bool NameTaken(String nomb) {
            return (from i in db.Indicadores
                    where i.Nombre == nomb
                    select i.UsuarioID).Count() > 0;
        }

        internal void Borrar() {
            ValorIndicador.BorrarPorIndicador(this);
            db.Indicadores.Remove(this);
            Save();
        }

        internal void Save() {
            db.SaveChanges();
        }

        internal void CrearValoresIndicadores() {
            List<Empresa> empresas = Empresa.GetEmpresas();
            foreach (var e in empresas) {
                List<int> períodos = e.GetPeríodosDeEmpresa();
                foreach (var p in períodos) {
                    ValorIndicador vi = new ValorIndicador(e.EmpresaID, IndicadorID, p);
                }
            }
            GC.Collect();
        }

        internal Indicador(int usuarioID, string nombre, string fórmula) {
            UsuarioID = usuarioID;
            Nombre = nombre;
            Fórmula = fórmula;
            db.Indicadores.Add(this);
            db.SaveChanges();
            CrearValoresIndicadores();
        }

        private Indicador() { }

        internal double CalcularValor(Empresa e, int período) {
            ValorIndicador vi = ValorIndicador.GetValorIndicador(e.EmpresaID, this, período);
            if (vi != null) {
                bool válido = vi.Recalcular();
                return (vi != null && válido ? vi.Valor : 0);
            }
            vi = new ValorIndicador(e.EmpresaID, IndicadorID, período);
            return vi.Valor;
        }

        internal double CalcularValor(Dictionary<string, Cuenta> cuentas) {
            parser = new Parser(Fórmula);
            return parser.CalcularValor(cuentas);
        }
    }
}