using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DDS.Models {
    [Table("Usuarios")]
    public class Usuario : Contextual {
        [Key]
        public int UsuarioID { get; set; }
        [Required, Index(IsUnique = true), StringLength(255, ErrorMessage = "El nombre de un usuario no puede superar los 255 caracteres. ")]
        public String Nombre { get; set; }
        [Required, StringLength(10, ErrorMessage = "La contraseña de un usuario no puede superar los 10 caracteres. ")]
        public String Password { get; set; }

        internal Usuario(String nomb, String pass) {
            Nombre = nomb;
            Password = pass;
            db.Usuarios.Add(this);
            db.SaveChanges();
        }

        public Usuario() { }

        internal static Usuario Get(String nomb, String pass) {
            return (from u in db.Usuarios
                    where u.Nombre == nomb
                    && u.Password == pass
                    select u).SingleOrDefault();
        }

        internal static bool NameTaken(String nomb) {
            return (from u in db.Usuarios
                    where u.Nombre == nomb
                    select u.UsuarioID).Count() > 0;
        }

        internal List<string> GetMetodologías() {
            return (from m in db.Metodologías
                    where m.UsuarioID == UsuarioID
                    select m.Nombre).ToList();
        }

        internal List<string> GetIndicadores() {
            return (from i in db.Indicadores
                    where i.UsuarioID == UsuarioID
                    select i.Nombre).ToList();
        }
    }
}