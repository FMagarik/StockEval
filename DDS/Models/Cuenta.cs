using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("Cuentas")]
    public class Cuenta : Contextual {
        [Key]
        public int CuentaID { get; set; }
        [Required]
        public int EmpresaID { get; set; }
        [Required]
        public int Período { get; set; }
        [Required, StringLength(255, ErrorMessage = "El nombre de una cuenta no puede superar los 255 caracteres. ")]
        public String Nombre { get; set; }
        [Required]
        public double Valor { get; set; }
        public virtual Empresa Empresa { get; set; }

        internal static List<string> GetNombres() {
            return (from c in db.Cuentas
                    select c.Nombre).ToList();
        }

        internal static void Save() {
            db.SaveChanges();
        }

        internal void SyncValoresIndicadores() {
            Empresa e = Empresa.Get(EmpresaID);
            if (e != null) ValorIndicador.RecalcularValoresIndicadores(e, Período);
        }

        internal Cuenta(int empresaID, int período, String nombre, double valor) {
            Período = período;
            Nombre = nombre;
            Valor = valor;
            EmpresaID = empresaID;
            db.Cuentas.Add(this);
        }

        private Cuenta() { }
    }
}