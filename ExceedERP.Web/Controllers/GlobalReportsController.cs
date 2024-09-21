using System.Web.Mvc;

namespace ExceedERP.Web.Controllers
{
    public class GlobalReportsController : Controller
    {
        // GET: GlobalRepors
        public ActionResult UserGroupsRoles()
        {
            return View();
        }
        public ActionResult UserInGroupsRoles()
        {
            return View();
        }
    }
}