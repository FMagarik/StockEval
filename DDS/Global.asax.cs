using DDS.Models;
using System.IO;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DDS {
    public class MvcApplication : System.Web.HttpApplication {
        public static string TempPath;

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            if (!Usuario.NameTaken("Facundo")) new Usuario("Facundo", "123");
            if (!Usuario.NameTaken("Luciano")) new Usuario("Luciano", "456");

            TempPath = Path.GetTempPath() + "ComoInvierto";
            Directory.CreateDirectory(TempPath + "\\Procesados");
            Directory.CreateDirectory(TempPath + "\\Descartados");
        }
    }
}
