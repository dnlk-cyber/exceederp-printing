using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ProjectUserAssignmentController : Controller
    {
        ApplicationDbContext userDb = new ApplicationDbContext();
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ProjectUserAssignment
        public ActionResult Index()
        {
            ViewData["Projects"] = db.ProjectInformation.ToList();

            //var users = userDb.Users;
            //List<ApplicationUser> list = new List<ApplicationUser>();
            //foreach (ApplicationUser user in users)
            //{
            //    user.FirstName = user.FirstName + " " + user.MiddleName + " " + user.LastName;
            //    list.Add(user);
            //}
            //ViewData["Users"] = users.ToList();
            return View();
        }
    }
}