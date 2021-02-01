using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class MyProfileController : Controller
    {
        // GET: HRM/MyProfile
        
        public ActionResult MyProfile()
        {
            
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            ViewBag.Id = id;
            return View();
        }
    }
}