using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web; 
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProjectActivity
{
    public class ProductionActivityController : Controller
    {
        ExceedDbContext db;
        public ProductionActivityController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["ActivityTypes"]  = db.ProductionSettings.Select(s => new { s.ProductionSettingId, s.Value}).ToList();
            return View();
        }
        public ActionResult Activities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionActivity> activities = db.ProductionActivities;
            DataSourceResult result = activities.ToDataSourceResult(request, activity => new
            {
                ProductionActivityId = activity.ProductionActivityId,
                Name = activity.Name,
                ProductionSettingId = activity.ProductionSettingId,
                OperationType = activity.OperationType,

                //MaterialCost = activity.MaterialCost,
                //LaborCost = activity.LaborCost,
                //Equipmentcost = activity.Equipmentcost,
                //OverheadCost = activity.OverheadCost,
                //ProfitCost = activity.ProfitCost,
                //TotalCost = activity.TotalCost,

                OrgBranchName = activity.OrgBranchName,
                DateCreated = activity.DateCreated,
                LastModified = activity.LastModified,
                CreatedBy = activity.CreatedBy,
                ModifiedBy = activity.ModifiedBy,
            });

            return Json(result);
        }
        public ActionResult MachineryActivities_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionActivity> activities = db.ProductionActivities.Where(a => a.OperationType == OperationType.MachineryOperation);
            DataSourceResult result = activities.ToDataSourceResult(request, activity => new
            {
                ProductionActivityId = activity.ProductionActivityId,
                Name = activity.Name,
                ProductionSettingId = activity.ProductionSettingId,
                OperationType = activity.OperationType,

                OrgBranchName = activity.OrgBranchName,
                DateCreated = activity.DateCreated,
                LastModified = activity.LastModified,
                CreatedBy = activity.CreatedBy,
                ModifiedBy = activity.ModifiedBy,
            });

            return Json(result);
        }
        public ActionResult LaborActivities_Read([DataSourceRequest]DataSourceRequest request)
        {
            this.AddDefaultActivities(ProductionSettingType.SeedProductionStage);
            IQueryable<ProductionActivity> activities = db.ProductionActivities.Where(a => a.OperationType == OperationType.LaborOperation);
            DataSourceResult result = activities.ToDataSourceResult(request, activity => new
            {
                ProductionActivityId = activity.ProductionActivityId,
                Name = activity.Name,
                ProductionSettingId = activity.ProductionSettingId,
                OperationType = activity.OperationType,

                OrgBranchName = activity.OrgBranchName,
                DateCreated = activity.DateCreated,
                LastModified = activity.LastModified,
                CreatedBy = activity.CreatedBy,
                ModifiedBy = activity.ModifiedBy,
            });

            return Json(result);
        }

        private void AddDefaultActivities(ProductionSettingType seedProductionStage)
        {
            var defaultSetting = ProductionSettingHelper.GetDefaultSetting(seedProductionStage);
            var addedDefaultOperations = db.ProductionActivities.Where(ac => ac.ProductionSettingId == defaultSetting.ProductionSettingId).ToList();
            if (!addedDefaultOperations.Any())
            {
                var laborOperations = Enum.GetNames(typeof(ProductionDefaultLaborOperations)).ToList();
                var machineryOperations = Enum.GetNames(typeof(ProductionDefaultMachineryOperations)).ToList();
                var formatedLaborOperations = new List<string>();
                var formatedMachineryOperations = new List<string>();
                #region format string
                foreach (var name in laborOperations)
                {
                    if (name.IndexOf('_') > -1)
                    {
                        var formatedName = name.Replace('_', ' ');
                        formatedLaborOperations.Add(formatedName);
                    }
                    else
                    {
                        formatedLaborOperations.Add(name);
                    }
                }
                foreach (var name in machineryOperations)
                {
                    if (name.IndexOf('_') > -1)
                    {
                        var formatedName = name.Replace('_', ' ');
                        formatedMachineryOperations.Add(formatedName);
                    }
                    else
                    {
                        formatedMachineryOperations.Add(name);
                    }
                }
                #endregion

                #region add the default operations
                foreach (var name in formatedLaborOperations)
                {
                    var record = new ProductionActivity
                    {
                        Name = name,
                        ProductionSettingId = defaultSetting.ProductionSettingId,
                        OperationType = OperationType.LaborOperation,
                    };
                    db.ProductionActivities.Add(record);
                    db.SaveChanges();
                }
                foreach (var name in formatedMachineryOperations)
                {
                    var record = new ProductionActivity
                    {
                        Name = name,
                        ProductionSettingId = defaultSetting.ProductionSettingId,
                        OperationType = OperationType.MachineryOperation,
                    };
                    db.ProductionActivities.Add(record);
                    db.SaveChanges();
                } 
                #endregion
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Create([DataSourceRequest]DataSourceRequest request, ProductionActivity activity, OperationType type)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionActivity
                {
                    ProductionActivityId = activity.ProductionActivityId,
                    Name = activity.Name,
                    ProductionSettingId = activity.ProductionSettingId,
                    OperationType = type,

                    //MaterialCost = activity.MaterialCost,
                    //LaborCost = activity.LaborCost,
                    //Equipmentcost = activity.Equipmentcost,
                    //OverheadCost = activity.OverheadCost,
                    //ProfitCost = activity.ProfitCost,
                    //TotalCost = activity.TotalCost,

                    OrgBranchName = activity.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = activity.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = activity.ModifiedBy,

                };
                //entity.TotalCost = entity.MaterialCost + entity.LaborCost + entity.Equipmentcost;

                db.ProductionActivities.Add(entity);
                db.SaveChanges();

                activity.ProductionActivityId = entity.ProductionActivityId;
                activity.ProductionSettingId = entity.ProductionSettingId;

            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Update([DataSourceRequest]DataSourceRequest request, ProductionActivity activity)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionActivity
                {
                    ProductionActivityId = activity.ProductionActivityId,
                    Name = activity.Name,
                    ProductionSettingId = activity.ProductionSettingId,
                    OperationType = activity.OperationType,

                    //MaterialCost = activity.MaterialCost,
                    //LaborCost = activity.LaborCost,
                    //Equipmentcost = activity.Equipmentcost,
                    //OverheadCost = activity.OverheadCost,
                    //ProfitCost = activity.ProfitCost,
                    //TotalCost = activity.TotalCost,

                    OrgBranchName = activity.OrgBranchName,
                    DateCreated = activity.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = activity.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                //entity.TotalCost = entity.MaterialCost + entity.LaborCost + entity.Equipmentcost;

                db.ProductionActivities.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Activity_Destroy([DataSourceRequest]DataSourceRequest request, ProductionActivity activity)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionActivity
                {
                    ProductionActivityId = activity.ProductionActivityId,
                    Name = activity.Name,
                    ProductionSettingId = activity.ProductionSettingId,
                    OperationType = activity.OperationType,

                    //MaterialCost = activity.MaterialCost,
                    //LaborCost = activity.LaborCost,
                    //Equipmentcost = activity.Equipmentcost,
                    //OverheadCost = activity.OverheadCost,
                    //ProfitCost = activity.ProfitCost,
                    //TotalCost = activity.TotalCost,

                    OrgBranchName = activity.OrgBranchName,
                    DateCreated = activity.DateCreated,
                    LastModified = activity.LastModified,
                    CreatedBy = activity.CreatedBy,
                    ModifiedBy = activity.ModifiedBy,

                };

                db.ProductionActivities.Attach(entity);
                db.ProductionActivities.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { activity }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}