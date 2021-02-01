using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstmnPriceRateWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal OverHeadCost { get; set; }
        public static decimal ProfitMargin { get; set; }
        public static string CurrentUSer { get; set; }

        public ActionResult Index(string name)
        {

            SetDefaultValue();
            return View();
        }

        public JsonResult GetWorkItemsItems(int WorkCategoryId)
        {
            var items = db.ProjectEstimationAreaWorkItem.ToList().Where(x => x.WorkCategoryId == WorkCategoryId).ToList();

            ViewData["WorkItems"] = items.Select(p => new { ItemName = p.WorkItemName, ProjectEstimationAreaWorkItemId = p.ProjectEstimationAreaWorkItemId });
            return Json(items.Select(p => new { ItemName = p.WorkItemName, ProjectEstimationAreaWorkItemId = p.ProjectEstimationAreaWorkItemId }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemCategory()
        {
            var items = db.ConstructionItemCategories.ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProjectEstimationAreaWorkItem_Read([DataSourceRequest]DataSourceRequest request, int type)
        {
            var projectType = (AvailableProjectTypes)type;
            SetDefaultValue();
            IQueryable<ProjectEstimationAreaWorkItem> ProjectEstimationAreaWorkItems =
                    db.ProjectEstimationAreaWorkItem.Where(x => x.ProjectType == projectType);
            DataSourceResult result = ProjectEstimationAreaWorkItems.ToDataSourceResult(request,
                    projectEstimationAreaWorkItem => new
                    {
                        ProjectEstimationAreaWorkItemId = projectEstimationAreaWorkItem.ProjectEstimationAreaWorkItemId,
                        WorkCategoryId = projectEstimationAreaWorkItem.WorkCategoryId,
                        Price = projectEstimationAreaWorkItem.Price,
                        WorkItemName = projectEstimationAreaWorkItem.WorkItemName,
                        ItemCode = projectEstimationAreaWorkItem.ItemCode,
                        DirectCost = projectEstimationAreaWorkItem.DirectCost,
                        Width = projectEstimationAreaWorkItem.Width,
                        Height = projectEstimationAreaWorkItem.Height,
                        Area = projectEstimationAreaWorkItem.Area,
                        CylinderLock = projectEstimationAreaWorkItem.CylinderLock,
                        OverHeadProfitCost = projectEstimationAreaWorkItem.OverHeadProfitCost,
                        ProjectType = projectEstimationAreaWorkItem.ProjectType,
                        MaterialCost = projectEstimationAreaWorkItem.MaterialCost,

                        OrgBranchName = projectEstimationAreaWorkItem.OrgBranchName,
                        DateCreated = projectEstimationAreaWorkItem.DateCreated,
                        LastModified = projectEstimationAreaWorkItem.LastModified,
                        CreatedBy = projectEstimationAreaWorkItem.CreatedBy,
                        ModifiedBy = projectEstimationAreaWorkItem.ModifiedBy,
                        TransportCost = projectEstimationAreaWorkItem.TransportCost,
                        BlockId = projectEstimationAreaWorkItem.BlockId,
                    });

            return Json(result);
            //}

        }
        public ActionResult TemplateWorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjTypeMappingId)
        {
            List<CopyTemplateAreaWorkItemViewModel> listToReturn = new List<CopyTemplateAreaWorkItemViewModel>();

            var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
            if (mapping != null)
            {
                var currentSubProjectType = db.SubProjectType.Find(mapping.SubProjectTypeId);
                if (currentSubProjectType != null)
                {
                    var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    if (project != null)
                    {
                        var templateWorkItems = db.ProjectEstimationAreaWorkItem
                                        .Where(tw => tw.ProjectType == project.ProjectType &&
                                        tw.WorkCategory.ConfigurationId == strId
                                        ).ToList();

                        if (templateWorkItems.Any())
                        {
                            foreach (ProjectEstimationAreaWorkItem workItem in templateWorkItems)
                            {
                                // check if the workItem is copied already 
                                var workItemInProject =
                                    db.GenericAreaWorkItem.FirstOrDefault(
                                        wpp => wpp.ItemCode == workItem.ItemCode &&
                                        wpp.WorkCategory.ConfigurationId == strId &&
                                        wpp.WorkItemName == workItem.WorkItemName &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                                var templateToReturn = new CopyTemplateAreaWorkItemViewModel
                                {
                                    Id = workItem.ProjectEstimationAreaWorkItemId,
                                    WorkItemName = workItem.WorkItemName,
                                    WorkCategory = workItem.WorkCategory.Name,
                                    ItemCode = workItem.ItemCode,
                                    //UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                    OverHeadProfitCost = workItem.OverHeadProfitCost,
                                };
                                if (workItemInProject != null)
                                {
                                    templateToReturn.IsCopied = true;
                                }
                                listToReturn.Add(templateToReturn);
                            }
                        }
                    }
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationAreaWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationAreaWorkItem projectEstimationAreaWorkItem, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            if (ModelState.IsValid)
            {
                //if (ProjectEstimationWorkItemController.ProjectId == 0)
                //{

                //    return new JavaScriptResult() { Script = "alert('" + "Please select the project first!!" + "');" };
                //}
                var entity = new ProjectEstimationAreaWorkItem
                {
                    ProjectEstimationAreaWorkItemId = projectEstimationAreaWorkItem.ProjectEstimationAreaWorkItemId,
                    Price = projectEstimationAreaWorkItem.Price,
                    WorkCategoryId = projectEstimationAreaWorkItem.WorkCategoryId,
                    WorkItemName = projectEstimationAreaWorkItem.WorkItemName,
                    ItemCode = projectEstimationAreaWorkItem.ItemCode,
                    DirectCost = projectEstimationAreaWorkItem.DirectCost,
                    Width = projectEstimationAreaWorkItem.Width,
                    Height = projectEstimationAreaWorkItem.Height,
                    Area = projectEstimationAreaWorkItem.Area,
                    CylinderLock = projectEstimationAreaWorkItem.CylinderLock,
                    OverHeadProfitCost = projectEstimationAreaWorkItem.OverHeadProfitCost,
                    TransportCost = projectEstimationAreaWorkItem.TransportCost,
                    ProjectType = projectType,
                    MaterialCost = projectEstimationAreaWorkItem.MaterialCost,

                    OrgBranchName = projectEstimationAreaWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = projectEstimationAreaWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectEstimationAreaWorkItem.ModifiedBy,
                    //BlockId = ProjectEstimationWorkItemController.BlockId,
                };
                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCost / 100)) +
                                         (entity.DirectCost * (ProfitMargin / 100));
                db.ProjectEstimationAreaWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                projectEstimationAreaWorkItem.ProjectEstimationAreaWorkItemId = entity.ProjectEstimationAreaWorkItemId;

            }

            return Json(new[] { projectEstimationAreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationAreaWorkItem_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationAreaWorkItem projectEstimationAreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationAreaWorkItem
                {
                    ProjectEstimationAreaWorkItemId = projectEstimationAreaWorkItem.ProjectEstimationAreaWorkItemId,
                    WorkCategoryId = projectEstimationAreaWorkItem.WorkCategoryId,
                    Price = projectEstimationAreaWorkItem.Price,
                    WorkItemName = projectEstimationAreaWorkItem.WorkItemName,
                    ItemCode = projectEstimationAreaWorkItem.ItemCode,
                    DirectCost = projectEstimationAreaWorkItem.DirectCost,
                    Width = projectEstimationAreaWorkItem.Width,
                    Height = projectEstimationAreaWorkItem.Height,
                    Area = projectEstimationAreaWorkItem.Area,
                    TransportCost = projectEstimationAreaWorkItem.TransportCost,
                    CylinderLock = projectEstimationAreaWorkItem.CylinderLock,
                    OverHeadProfitCost = projectEstimationAreaWorkItem.OverHeadProfitCost,

                    ProjectType = projectEstimationAreaWorkItem.ProjectType,
                    MaterialCost = projectEstimationAreaWorkItem.MaterialCost,

                    OrgBranchName = projectEstimationAreaWorkItem.OrgBranchName,
                    DateCreated = projectEstimationAreaWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = projectEstimationAreaWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    BlockId = projectEstimationAreaWorkItem.BlockId,
                };

                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCost / 100)) +
                                            (entity.DirectCost * (ProfitMargin / 100));
                db = new ExceedDbContext();
                db.ProjectEstimationAreaWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { projectEstimationAreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationAreaWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationAreaWorkItem projectEstimationAreaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationAreaWorkItem
                {
                    ProjectEstimationAreaWorkItemId = projectEstimationAreaWorkItem.ProjectEstimationAreaWorkItemId,
                    WorkCategoryId = projectEstimationAreaWorkItem.WorkCategoryId,
                    WorkItemName = projectEstimationAreaWorkItem.WorkItemName,
                    ItemCode = projectEstimationAreaWorkItem.ItemCode,
                    DirectCost = projectEstimationAreaWorkItem.DirectCost,
                    Width = projectEstimationAreaWorkItem.Width,
                    Height = projectEstimationAreaWorkItem.Height,
                    Price = projectEstimationAreaWorkItem.Price,
                    Area = projectEstimationAreaWorkItem.Area,
                    CylinderLock = projectEstimationAreaWorkItem.CylinderLock,
                    OverHeadProfitCost = projectEstimationAreaWorkItem.OverHeadProfitCost,
                    TransportCost = projectEstimationAreaWorkItem.TransportCost,
                    ProjectType = projectEstimationAreaWorkItem.ProjectType,
                    MaterialCost = projectEstimationAreaWorkItem.MaterialCost,

                    OrgBranchName = projectEstimationAreaWorkItem.OrgBranchName,
                    DateCreated = projectEstimationAreaWorkItem.DateCreated,
                    LastModified = projectEstimationAreaWorkItem.LastModified,
                    CreatedBy = projectEstimationAreaWorkItem.CreatedBy,
                    ModifiedBy = projectEstimationAreaWorkItem.ModifiedBy,
                    BlockId = projectEstimationAreaWorkItem.BlockId,
                };

                db.ProjectEstimationAreaWorkItem.Attach(entity);
                db.ProjectEstimationAreaWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationAreaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        private void SetDefaultValue()
        {
            var configartion = db.Configurations.ToList();
            if (configartion.Any())
            {
                var ovH = configartion.Where(x => x.Attribute == "OVER_HEAD_COST_RATE").FirstOrDefault();
                if (ovH != null)
                {
                    OverHeadCost = Convert.ToDecimal(ovH.CurrentValue);
                }
                var profMa = configartion.Where(x => x.Attribute == "PROFIT_COST_RATE").FirstOrDefault();
                if (profMa != null)
                {
                    ProfitMargin = Convert.ToDecimal(profMa.CurrentValue);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
