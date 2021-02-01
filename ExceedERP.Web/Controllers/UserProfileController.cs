using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.Finance.Controllers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNet.Identity;

namespace ExceedERP.Web.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ExceedDbContext _db = new ExceedDbContext();

        // GET: UserProfile
        public ActionResult Index()
        {

            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var branches = _db.Branch;

            ViewData["Branches"] = branches;

           

           

        }

        public ActionResult MyBranches_Read([DataSourceRequest]DataSourceRequest request, string type)
        {
            string userId = User.Identity.GetUserId();

            IQueryable<UserBranchGroup> accountChekerVms =db.UserBranches.Where(x=>x.ApplicationUserId== userId).AsQueryable();
            DataSourceResult result = accountChekerVms.ToDataSourceResult(request, accountChekerVm => new
            {
                Id = accountChekerVm.Id,
                UserBranchId = accountChekerVm.UserBranchId,
                IsDefault = accountChekerVm.IsDefault,
                
            });

            return Json(result);
        }

        public JsonResult MakeDefault(int id)
        {
            string userId = User.Identity.GetUserId();
           
            UserBranchGroup mapping = db.UserBranches.Find(id);
            if (mapping != null)
            {
                //make all non-default
                var userBranches = db.UserBranches.Where(x => x.ApplicationUserId == userId).ToList();
                foreach (var map in userBranches)
                {
                    map.IsDefault = false;
                    db.Entry(map).State = EntityState.Modified;
                    db.SaveChanges();
                }

                mapping.IsDefault = true;
                db.Entry(mapping).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "Default updated successfully.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData = new
                {
                    Message = "Mapping not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: UserProfile/Details/5

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
