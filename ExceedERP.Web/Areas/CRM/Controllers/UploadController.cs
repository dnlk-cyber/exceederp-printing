using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class UploadController : Controller
    {
        // GET: Common/Upload
        public ActionResult Index()
        {
            return View();
        }
    }
}