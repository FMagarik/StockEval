namespace DDS {
    using Models;
    using System.Data.Entity;

    public class EDM : DbContext {
        public EDM() : base("name=EDM") {}

        public virtual DbSet<Cuenta> Cuentas { get; set; }
        public virtual DbSet<Indicador> Indicadores { get; set; }
        public virtual DbSet<Empresa> Empresas { get; set; }
        public virtual DbSet<Metodología> Metodologías { get; set; }
        public virtual DbSet<Condición> Condiciones { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<ValorIndicador> ValoresIndicadores { get; set; }
    }
}