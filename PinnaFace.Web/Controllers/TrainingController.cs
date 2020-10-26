using System.Web.Mvc;

namespace PinnaFace.Web.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

    }
}