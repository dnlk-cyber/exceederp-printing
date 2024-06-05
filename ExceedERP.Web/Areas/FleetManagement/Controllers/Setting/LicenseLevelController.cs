using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class LicenseLevelController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public LicenseLevelController()
        {
        }
        //
        // GET: /FleetManagement/LicenseLevel/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult LicenseLevel_Read([DataSourceRequest] DataSourceRequest request)
        {

            var oper = db.LicenseLevels.ToList();
            var Opers = oper.Select(o => new LicenseLevel
            {
                LicenseLevelId = o.LicenseLevelId,
                Name = o.Name,
                Description = o.Description


            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseLevel_Create([DataSourceRequest] DataSourceRequest request, LicenseLevel LicenseLevel)
        {
            ModelState.Remove("LicenseLevelId");
            List<LicenseLevel> LicenseLevels = db.LicenseLevels.Where(f => f.Name == LicenseLevel.Name).ToList();
            if (LicenseLevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " License Level Already Exists ");
                return this.Json(new[] { LicenseLevel }.ToDataSourceResult(request, ModelState));
            }

            if (LicenseLevel != null)
            {

                LicenseLevel.CreatedBy = User.Identity.Name;
                LicenseLevel.DateCreated = DateTime.Now;


                db.LicenseLevels.Add(LicenseLevel);
                db.SaveChanges();
            }

            return this.Json(new[] { LicenseLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseLevel_Update([DataSourceRequest] DataSourceRequest request, LicenseLevel LicenseLevel)
        {

            List<LicenseLevel> LicenseLevels = db.LicenseLevels.Where(f => f.Name == LicenseLevel.Name && f.LicenseLevelId != LicenseLevel.LicenseLevelId).ToList();
            if (LicenseLevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Operator Position Already Exists ");
                return this.Json(new[] { LicenseLevel }.ToDataSourceResult(request, ModelState));
            }

            if (LicenseLevel != null)
            {
                LicenseLevel.ModifiedBy = User.Identity.Name;
                LicenseLevel.LastModified = DateTime.Now;


                db.LicenseLevels.Attach(LicenseLevel);
                db.Entry(LicenseLevel).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
         
            }


            return this.Json(new[] { LicenseLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseLevel_Destroy([DataSourceRequest] DataSourceRequest request, LicenseLevel LicenseLevel)
        {
            var LicenseLevels = db.LicenseLevels.Where(x=>x.LicenseLevelId == LicenseLevel.LicenseLevelId).FirstOrDefault();
            if (LicenseLevels != null)
            {
                db.LicenseLevels.Attach(LicenseLevels);
                db.LicenseLevels.Remove(LicenseLevels);
                db.SaveChanges();
            }

            return this.Json(new[] { LicenseLevel }.ToDataSourceResult(request));
        }


    }
}