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
    public class LicenseTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public LicenseTypeController()
        {
        }
        //
        // GET: /FleetManagement/LicenseType/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult LicenseType_Read([DataSourceRequest] DataSourceRequest request)
        {

            var oper = db.LicenseTypes.ToList();
            var Opers = oper.Select(o => new LicenseType
            {
                LicenseTypeId = o.LicenseTypeId,
                Name = o.Name,
                Description = o.Description


            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseType_Create([DataSourceRequest] DataSourceRequest request, LicenseType LicenseType)
        {
            ModelState.Remove("LicenseTypeId");
            List<LicenseType> LicenseTypes = db.LicenseTypes.Where(f => f.Name == LicenseType.Name).ToList();
            if (LicenseTypes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " License Level Already Exists ");
                return this.Json(new[] { LicenseType }.ToDataSourceResult(request, ModelState));
            }

            if (LicenseType != null)
            {

                LicenseType.CreatedBy = User.Identity.Name;
                LicenseType.DateCreated = DateTime.Now;


                db.LicenseTypes.Add(LicenseType);
                db.SaveChanges();
            }

            return this.Json(new[] { LicenseType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseType_Update([DataSourceRequest] DataSourceRequest request, LicenseType LicenseType)
        {

            List<LicenseType> LicenseTypes = db.LicenseTypes.Where(f => f.Name == LicenseType.Name && f.LicenseTypeId != LicenseType.LicenseTypeId).ToList();
            if (LicenseTypes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " License Type Already Exists ");
                return this.Json(new[] { LicenseType }.ToDataSourceResult(request, ModelState));
            }

            if (LicenseType != null)
            {
                LicenseType.ModifiedBy = User.Identity.Name;
                LicenseType.LastModified = DateTime.Now;


                db.LicenseTypes.Attach(LicenseType);
                db.Entry(LicenseType).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
         
            }


            return this.Json(new[] { LicenseType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult LicenseType_Destroy([DataSourceRequest] DataSourceRequest request, LicenseType LicenseType)
        {
            var LicenseTypes = db.LicenseTypes.Where(x=>x.LicenseTypeId == LicenseType.LicenseTypeId).FirstOrDefault();
            if (LicenseTypes != null)
            {
                db.LicenseTypes.Attach(LicenseTypes);
                db.LicenseTypes.Remove(LicenseTypes);
                db.SaveChanges();
            }

            return this.Json(new[] { LicenseType }.ToDataSourceResult(request));
        }
       


    }
}