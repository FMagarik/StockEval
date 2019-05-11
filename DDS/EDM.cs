namespace DDS {
    using Models;
    using System.Data.Entity;

    public class EDM : DbContext {
        public EDM() : base("name=EDM") {}

        public virtual DbSet<Cuenta> Cuentas { get; set; }
        public virtual DbSet<Indicador> Indicadores { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Metodolog�a> Metodolog�as { get; set; }
        public virtual DbSet<Condici�n> Condiciones { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<ValorIndicador> ValoresIndicadores { get; set; }
    }
}