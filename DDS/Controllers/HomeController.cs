using System.Web.Mvc;

namespace DDS.Controllers {
    [Authorize]
    public class HomeController : Controller {
        public ActionResult Index() {
            return View();
        }
    }
}