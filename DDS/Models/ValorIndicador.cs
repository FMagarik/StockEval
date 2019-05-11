using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("ValoresIndicadores")]
    public class ValorIndicador : Contextual {
        [Key]
        public int ValorIndicadorID { get; set; }
        [Required]
        public int EmpresaID { get; set; }
        [Required]
        public int IndicadorID { get; set; }
        [Required]
        public int Período { get; set; }
        [Required]
        public double Valor { get; set; }

        internal static List<ValorIndicador> GetValoresIndicadores(int indicadorID) {
            return (from vi in db.ValoresIndicadores
                    where vi.IndicadorID == indicadorID
                    select vi).ToList();
        }

        internal static ValorIndicador GetValorIndicador(int empresaID, Indicador i, int período) {
            return (from vi in db.ValoresIndicadores
                    where vi.IndicadorID == i.IndicadorID
                    && vi.EmpresaID == empresaID
                    && vi.Período == período
                    select vi).SingleOrDefault();
        }

        internal static void RecalcularValoresIndicadores(Empresa e, int período) {
            var vis = (
                from vi in db.ValoresIndicadores
                where vi.EmpresaID == e.EmpresaID
                && vi.Período == período
                select vi
            ).ToList();
            foreach (var vi in vis)
                vi.Recalcular();
            GC.Collect();
        }

        internal static void BorrarPorIndicador(Indicador i) {
            db.ValoresIndicadores.RemoveRange(db.ValoresIndicadores.Where(vi => vi.IndicadorID == i.IndicadorID));
            db.SaveChanges();
        }

        internal void BorrarSinGrabar() {
            db.ValoresIndicadores.Remove(this);
        }

        internal void Borrar() {
            BorrarSinGrabar();
            Save();
        }

        internal void Save() {
            db.SaveChanges();
        }

        private bool RecalcularSinGrabar() {
            Empresa e = Empresa.Get(EmpresaID);
            Indicador i = Indicador.Get(IndicadorID);
            bool esVálido = false;
            if (e != null && i != null) {
                Valor = i.CalcularValor(e.DiccionarioCuentasDelPeríodo(Período));
                esVálido |= i.parser.EsVálido();
            }
            return esVálido;
        }

        internal ValorIndicador(int empresaID, int indicadorID, int período) {
            EmpresaID = empresaID;
            IndicadorID = indicadorID;
            Período = período;
            if (RecalcularSinGrabar()) {
                db.ValoresIndicadores.Add(this);
                Save();
            }
        }

        private ValorIndicador() { }

        internal bool Recalcular() {
            bool esVálido = RecalcularSinGrabar();
            if (!esVálido) BorrarSinGrabar();
            Save();
            return esVálido;
        }
    }
}