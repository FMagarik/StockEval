using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("Empresas")]
    public class Empresa : Contextual {
        [Key]
        public int EmpresaID { get; set; }
        [Required, Index(IsUnique = true), StringLength(255, ErrorMessage = "El nombre de una empresa no puede superar los 255 caracteres. ")]
        public String Nombre { get; set; }
        public virtual ICollection<Cuenta> Cuentas { get; set; }

        internal Empresa(String nombre) {
            Nombre = nombre;
            db.Empresas.Add(this);
            db.SaveChanges();
        }

        private Empresa() { }

        internal static List<string> GetNombres() {
            return (from e in db.Empresas select e.Nombre).ToList();
        }

        internal static Empresa Get(string nombre) {
            return (from e in db.Empresas
                    where e.Nombre == nombre
                    select e).SingleOrDefault();
        }

        internal static Empresa Get(int ID) {
            return (from e in db.Empresas
                    where e.EmpresaID == ID
                    select e).SingleOrDefault();
        }

        internal static List<Empresa> GetEmpresas() {
            return (from e in db.Empresas
                   select e).ToList();
        }

        internal static List<int> GetPeríodos() {
            return (from c in db.Cuentas
                    select c.Período).Distinct().ToList();
        }

        internal List<int> GetPeríodosDeEmpresa() {
            return (from c in db.Cuentas
                    where c.EmpresaID == EmpresaID
                    select c.Período).Distinct().ToList();
        }

        internal List<Cuenta> TodasCuentas() {
            return (from c in db.Cuentas
                    where c.EmpresaID == EmpresaID
                    orderby c.Período, c.Nombre
                    select c).ToList();
        }

        internal List<Cuenta> CuentasDelPeríodo(int period) {
            return (from c in db.Cuentas
                    where c.EmpresaID == EmpresaID && c.Período == period
                    select c).ToList();
        }

        internal Dictionary<string, Cuenta> DiccionarioCuentasDelPeríodo(int period) {
            return CuentasDelPeríodo(period).ToDictionary(k => k.Nombre, v => v);
        }

        internal Cuenta Cuenta(string nombre, int period) {
            return (from c in db.Cuentas
                    where c.EmpresaID == EmpresaID && c.Nombre == nombre && c.Período == period
                    select c).FirstOrDefault();
        }
    }
}