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
	public class EquipmentOperationPlanController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            PopulateData();
            return View();
        }
        public ActionResult FuelIndex()
        {
            PopulateData();
            return View();
        }
        private void PopulateData()
        {
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["GLPeriod"] = db.GLPeriods
                .Select(x => new {
                    Value=x.GLPeriodId,
                    Text=x.Name
                }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
     ViewData["PlateNunber"] = CacheData.GetFleetsSecondCache().Select(
    i => new
    {
        Text = i.PlateNumber,
        Value = i.FleetsID
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

            
		}

		public ActionResult _Read([DataSourceRequest] DataSourceRequest request)
		{
            var categoryRate = db.EquipmentOperationPlan.Where(x=>x.IsFuel==false).ToList();
            DataSourceResult result = categoryRate.ToDataSourceResult(request, e => new

            {
                CategoryId = e.CategoryId,
                EquipmentOperationPlanId = e.EquipmentOperationPlanId,
                GLPeriodId = e.GLPeriodId,
                EquipmentTypeId = e.EquipmentTypeId,
                EquipmentOperationHR = e.EquipmentOperationHR,
                OperationHR = e.OperationHR,
                DownHR = e.DownHR,
                IdleHR = e.IdleHR,
                GLFiscalYearId = e.GLFiscalYearId,
                CreatedBy = e.CreatedBy,
                FleetId = e.FleetId,
                EquipmentNameId = e.EquipmentNameId,
                SerialNumber = e.SerialNumber,
                DateCreated=e.DateCreated,
                FuelConsumption = e.FuelConsumption,
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
		}public ActionResult Fuel_Read([DataSourceRequest] DataSourceRequest request)
		{
            var categoryRate = db.EquipmentOperationPlan.Where(x=>x.IsFuel==true).ToList();
            DataSourceResult result = categoryRate.ToDataSourceResult(request, e => new

            {
                CategoryId = e.CategoryId,
                EquipmentOperationPlanId = e.EquipmentOperationPlanId,
                GLPeriodId = e.GLPeriodId,
                EquipmentTypeId = e.EquipmentTypeId,
                EquipmentOperationHR = e.EquipmentOperationHR,
                OperationHR = e.OperationHR,
                DownHR = e.DownHR,
                IdleHR = e.IdleHR,
                GLFiscalYearId = e.GLFiscalYearId,
                CreatedBy = e.CreatedBy,
                FleetId = e.FleetId,
                EquipmentNameId = e.EquipmentNameId,
                SerialNumber = e.SerialNumber,
                DateCreated=e.DateCreated,
                FuelConsumption = e.FuelConsumption,
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _Create([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlan model)
        {
            if (model != null)
            {
                // 🔴 VALIDATION
                var existing = db.EquipmentOperationPlan
                    .FirstOrDefault(x => x.EquipmentTypeId == model.EquipmentTypeId &&
                                x.GLFiscalYearId == model.GLFiscalYearId&&x.GLPeriodId==model.GLPeriodId&&x.FleetId==model.FleetId&&x.IsFuel==false);

                // Rule 3: Prevent mixing Yearly & Monthly
                if (existing!=null)
                {
                      ModelState.AddModelError("", "Monthly plan already exists for this Equipment Type in this Fiscal Year.");
                    
                }

                if(model.GLPeriodId==null)
                {
                    ModelState.AddModelError("", "Please select Period");
                }

                if(model.FleetId==null)
                {
                    ModelState.AddModelError("", "Please select Plate Number");
                }
                if (ModelState.IsValid)
                {
                    model.DateCreated = DateTime.Now;
                    model.CreatedBy = User.Identity.Name;

                    db.EquipmentOperationPlan.Add(model);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Fuel_Create([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlan model)
        {
            if (model != null)
            {
                // 🔴 VALIDATION
                
                // Rule 3: Prevent mixing Yearly & Monthly
                var existing = db.EquipmentOperationPlan
                    .FirstOrDefault(x => x.EquipmentTypeId == model.EquipmentTypeId &&
                                x.GLFiscalYearId == model.GLFiscalYearId && x.GLPeriodId == model.GLPeriodId && x.FleetId == model.FleetId && x.IsFuel == true);

                // Rule 3: Prevent mixing Yearly & Monthly
                if (existing != null)
                {
                    ModelState.AddModelError("", "Monthly plan already exists for this Equipment Type in this Fiscal Year.");

                }

                if (model.GLPeriodId == null)
                {
                    ModelState.AddModelError("", "Please select Period");
                }

                if (model.FleetId == null)
                {
                    ModelState.AddModelError("", "Please select Plate Number");
                }
                // ✅ SAVE
                if (ModelState.IsValid)
                {
                    model.DateCreated = DateTime.Now;
                    model.CreatedBy = User.Identity.Name;
                    model.IsFuel = true;
                    db.EquipmentOperationPlan.Add(model);
                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _Update([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlan model)
        {
            if (model != null)
            {
                var existing = db.EquipmentOperationPlan
                    .FirstOrDefault(x => x.EquipmentTypeId == model.EquipmentTypeId &&
                                x.GLFiscalYearId == model.GLFiscalYearId && x.GLPeriodId == model.GLPeriodId && x.FleetId == model.FleetId &&x.EquipmentOperationPlanId!=model.EquipmentOperationPlanId&& x.IsFuel == false);
                // Rule 3: Prevent mixing Yearly & Monthly
                if (existing != null)
                {
                    ModelState.AddModelError("", "Monthly plan already exists for this Equipment Type in this Fiscal Year.");

                }

                if (model.GLPeriodId == null)
                {
                    ModelState.AddModelError("", "Please select Period");
                }

                if (model.FleetId == null)
                {
                    ModelState.AddModelError("", "Please select Plate Number");
                }
                // ✅ UPDATE
                if (ModelState.IsValid)
                {
                    var entity = db.EquipmentOperationPlan.Find(model.EquipmentOperationPlanId);

                    entity.CategoryId = model.CategoryId;
                    entity.EquipmentTypeId = model.EquipmentTypeId;
                    entity.OperationHR = model.OperationHR;
                    entity.IdleHR = model.IdleHR;
                    entity.FuelConsumption = model.FuelConsumption;
                    entity.DownHR = model.DownHR;
                    entity.GLFiscalYearId = model.GLFiscalYearId;
                    entity.GLPeriodId = model.GLPeriodId;
                    entity.EquipmentOperationHR = model.EquipmentOperationHR;

                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fuel_Update([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlan model)
        {
            if (model != null)
            {
                var existing = db.EquipmentOperationPlan
                    .FirstOrDefault(x => x.EquipmentTypeId == model.EquipmentTypeId &&
                                x.GLFiscalYearId == model.GLFiscalYearId && x.GLPeriodId == model.GLPeriodId && x.FleetId == model.FleetId &&x.EquipmentOperationPlanId!=model.EquipmentOperationPlanId&& x.IsFuel == true);
                // Rule 3: Prevent mixing Yearly & Monthly
                if (existing != null)
                {
                    ModelState.AddModelError("", "Monthly plan already exists for this Equipment Type in this Fiscal Year.");

                }

                if (model.GLPeriodId == null)
                {
                    ModelState.AddModelError("", "Please select Period");
                }

                if (model.FleetId == null)
                {
                    ModelState.AddModelError("", "Please select Plate Number");
                }
                // ✅ UPDATE
                if (ModelState.IsValid)
                {
                    var entity = db.EquipmentOperationPlan.Find(model.EquipmentOperationPlanId);

                    entity.CategoryId = model.CategoryId;
                    entity.EquipmentTypeId = model.EquipmentTypeId;
                    entity.OperationHR = model.OperationHR;
                    entity.IdleHR = model.IdleHR;
                    entity.FuelConsumption = model.FuelConsumption;
                    entity.DownHR = model.DownHR;
                    entity.GLFiscalYearId = model.GLFiscalYearId;
                    entity.GLPeriodId = model.GLPeriodId;
                    entity.EquipmentOperationHR = model.EquipmentOperationHR;

                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.SaveChanges();
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
           
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult _Destroy([DataSourceRequest] DataSourceRequest request, EquipmentOperationPlan model)
        {
            if (model != null)
            {
                var entity = db.EquipmentOperationPlan.Find(model.EquipmentOperationPlanId);

                if (entity != null)
                {
                    db.EquipmentOperationPlan.Remove(entity);
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

