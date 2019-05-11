using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDS.Models {
    [Table("Condiciones")]
    public class Condición : Contextual {
        [Key]
        public int CondiciónID { get; set; }
        [Required]
        public int MetodologíaID { get; set; }
        [Required]
        public bool EsCreciente { get; set; }
        [Required]
        public bool EsConsistente { get; set; }
        [Required]
        public string Fórmula { get; set; }
        public virtual Metodología Metodología { get; set; }

        public Condición (string fórmula, int metodologíaID) {
            EsCreciente = fórmula.Contains("$");
            Fórmula = fórmula.Replace("$", string.Empty);
            EsConsistente = fórmula.Contains("#");
            Fórmula = fórmula.Replace("#", string.Empty);
            MetodologíaID = metodologíaID;
            db.Condiciones.Add(this);
            db.SaveChanges();
        }

        private Condición() {}
    }
}