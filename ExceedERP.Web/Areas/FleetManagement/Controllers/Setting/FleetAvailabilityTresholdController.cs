using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Finance.GL;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
	public class FleetAvailabilityTresholdController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
		{

            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
    
     ViewData["EquipmentName"]=CacheData.GetEquipmentNameCaches().Select(i => new
     {
         Text = i.Name,
         Value = i.Id
     }).AsQueryable();
     ViewData["PlateNumber"]=CacheData.GetFleetsSecondCache().Select(i => new
     {
         Text = i.PlateNumber,
         Value = i.FleetsID
     }).AsQueryable();
            ViewData["FiscalYear"] = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open)
                .Select(x => new
                {
                    Value=x.GlFiscalYearId,
                    Text=x.Name
                }).AsQueryable();

            return View();
		}

		public ActionResult _Read([DataSourceRequest] DataSourceRequest request)
		{
            var categoryRate = db.FleetAvailabilityTreshold.ToList();
            DataSourceResult result=categoryRate.ToDataSourceResult(request,e=>new

            {
                CategoryId = e.CategoryId,
                FleetAvailabilityTresholdId = e.FleetAvailabilityTresholdId,
                FleetId = e.FleetId,
                NameId = e.NameId,
                TypeId = e.TypeId,
                AvailabilityTreshold = e.AvailabilityTreshold,
                GLFiscalYearId = e.GLFiscalYearId,
                CreatedBy=e.CreatedBy,
                DateCreated=e.DateCreated,
                ModifiedBy=e.ModifiedBy,
                LastModified = e.LastModified,
                IsOnlineApproved=e.IsOnlineApproved,
                IsOnlineTransferred=e.IsOnlineTransferred,
                IsOnlineTransferredBy=e.IsOnlineTransferredBy,
                IsSendForApproval=e.IsSendForApproval,
                IsVoid=e.IsVoid,
                SendForApprovalBy=e.SendForApprovalBy,
                SendForApprovalTime=e.SendForApprovalTime,
                OnlineApprovedBy=e.OnlineApprovedBy,
                OnlineApprovedTime= e.OnlineApprovedTime,
                Remark = e.Remark
				}
		);

			return Json(categoryRate.ToDataSourceResult(request));
		}
        [HttpPost]
        public ActionResult _Create([DataSourceRequest] DataSourceRequest request, FleetAvailabilityTreshold model)
        {
            if (model != null)
            {
                bool exists = db.FleetAvailabilityTreshold.Any(x =>
                    x.GLFiscalYearId == model.GLFiscalYearId);

                if (exists)
                {
                    ModelState.AddModelError("", "Already planned for the selected fiscal year.");
                }

                if (ModelState.IsValid)
                {
                    model.CreatedBy = User.Identity.Name;
                    model.DateCreated = DateTime.Now;

                    db.FleetAvailabilityTreshold.Add(model);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult _Update([DataSourceRequest] DataSourceRequest request, FleetAvailabilityTreshold model)
        {
            if (model != null)
            {
                bool exists = db.FleetAvailabilityTreshold.Any(x =>
                    x.GLFiscalYearId == model.GLFiscalYearId &&
                    x.FleetAvailabilityTresholdId != model.FleetAvailabilityTresholdId);

                if (exists)
                {
                    ModelState.AddModelError("", "Already planned for the selected fiscal year.");
                }

                if (ModelState.IsValid)
                {
                    var entity = db.FleetAvailabilityTreshold
                                   .FirstOrDefault(x => x.FleetAvailabilityTresholdId == model.FleetAvailabilityTresholdId);

                    if (entity != null)
                    {
                        //entity.CategoryId = model.CategoryId;
                        //entity.FleetId = model.FleetId;
                        //entity.NameId = model.NameId;
                        //entity.TypeId = model.TypeId;
                        entity.AvailabilityTreshold = model.AvailabilityTreshold;
                        entity.GLFiscalYearId = model.GLFiscalYearId;
                        entity.Remark = model.Remark;

                        entity.ModifiedBy = User.Identity.Name;
                        entity.LastModified = DateTime.Now;

                        db.SaveChanges();
                    }
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult _Destroy([DataSourceRequest] DataSourceRequest request, FleetAvailabilityTreshold model)
        {
            if (model != null)
            {
                var entity = db.FleetAvailabilityTreshold
                               .FirstOrDefault(x => x.FleetAvailabilityTresholdId == model.FleetAvailabilityTresholdId);

                if (entity != null)
                {
                    db.FleetAvailabilityTreshold.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

