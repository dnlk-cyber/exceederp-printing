using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class AlertLevelController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FuelConsumptionAlertLevel_Read([DataSourceRequest] DataSourceRequest request)
        {
            var FuelConsumptionAlertLevel = db.FuelConsumptionAlertLevel.ToList();

            return Json(FuelConsumptionAlertLevel.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelConsumptionAlertLevel_Create([DataSourceRequest] DataSourceRequest request, FuelConsumptionAlertLevel model)
        {
            ModelState.Remove("FuelConsumptionAlertLevelId");
            List<FuelConsumptionAlertLevel> AlertLevels = db.FuelConsumptionAlertLevel.Where(f => f.Name == model.Name).ToList();
            if (AlertLevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Alert Level Already Exists ");
                return this.Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            if (model.FromValue > model.ToValue)
            {
                ModelState.Clear();
                ModelState.AddModelError("IncorrectValue", " To Value Must be Greater Than From Value ");
                return this.Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            var Level = new FuelConsumptionAlertLevel
            {
                FuelConsumptionAlertLevelId = Guid.NewGuid(),
                Name = model.Name,
                ToValue = model.ToValue,
                FromValue = model.FromValue,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name

            };
            db.FuelConsumptionAlertLevel.Add(Level);
            db.SaveChanges();
            model.FuelConsumptionAlertLevelId = Level.FuelConsumptionAlertLevelId;

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelConsumptionAlertLevel_Update([DataSourceRequest] DataSourceRequest request, FuelConsumptionAlertLevel FuelConsumptionAlertLevel)
        {
            List<FuelConsumptionAlertLevel> AlertLevels = db.FuelConsumptionAlertLevel.Where(f => f.Name == FuelConsumptionAlertLevel.Name && f.FuelConsumptionAlertLevelId != FuelConsumptionAlertLevel.FuelConsumptionAlertLevelId).ToList();
            if (AlertLevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Alert Level Already Exists ");
                return this.Json(new[] { FuelConsumptionAlertLevel }.ToDataSourceResult(request, ModelState));
            }
            if (FuelConsumptionAlertLevel.FromValue > FuelConsumptionAlertLevel.ToValue)
            {
                ModelState.Clear();
                ModelState.AddModelError("IncorrectValue", " To Value Must be Greater Than From Value ");
                return this.Json(new[] { FuelConsumptionAlertLevel }.ToDataSourceResult(request, ModelState));
            }
            if (FuelConsumptionAlertLevel != null)
            {
                FuelConsumptionAlertLevel.ModifiedBy = User.Identity.Name;
                FuelConsumptionAlertLevel.LastModified = DateTime.Now;

                db.FuelConsumptionAlertLevel.Attach(FuelConsumptionAlertLevel);
                db.Entry(FuelConsumptionAlertLevel).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { FuelConsumptionAlertLevel }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelConsumptionAlertLevel_Destroy([DataSourceRequest] DataSourceRequest request, FuelConsumptionAlertLevel FuelConsumptionAlertLevel)
        {
            var FuelConsumptionAlertLevels = db.FuelConsumptionAlertLevel.Where(x => x.FuelConsumptionAlertLevelId == FuelConsumptionAlertLevel.FuelConsumptionAlertLevelId).FirstOrDefault();
            if (FuelConsumptionAlertLevels != null)
            {

                db.FuelConsumptionAlertLevel.Attach(FuelConsumptionAlertLevels);
                db.FuelConsumptionAlertLevel.Remove(FuelConsumptionAlertLevels);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted FuelConsumptionAlertLevel number " + FuelConsumptionAlertLevel.FuelConsumptionAlertLevelId);

            }

            return Json(new[] { FuelConsumptionAlertLevel }.ToDataSourceResult(request));
        }



    }
}
