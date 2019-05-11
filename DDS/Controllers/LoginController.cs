using DDS.Models;
using System.Web.Mvc;

namespace DDS.Controllers {
    public class LoginController : Controller {
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult Validate(Usuario u) {
            u = Usuario.Get(u.Nombre, u.Password);
            if (u != null) {
                System.Web.Security.FormsAuthentication.SetAuthCookie(u.UsuarioID.ToString(), false);
                TempData["Info"] = "Inicio de sesión exitoso";
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Credenciales erróneas. Por favor intente nuevamente";
            return RedirectToAction("Index");
        }
        
        [Authorize]
        public ActionResult Logoff() {
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }
    }
}