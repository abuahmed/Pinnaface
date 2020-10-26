using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PinnaFace.Web.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index()
        {
            return View("Error");
        }

        public ActionResult NotFound()
        {
            //Response.StatusCode = 404;
            return View("NotFound");
        }
    }
}
