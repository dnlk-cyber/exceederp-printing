using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Services.Description;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstmnSubContractWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static decimal OverHeadCost { get; set; }
        public static decimal ProfitMargin { get; set; }
      
        public JsonResult GetItemCategory()
        {
            var items = db.ConstructionItemCategories.ToList();
            return Json(items.Select(p => new { ItemCategoryCode = p.ItemCategoryCode, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult EstimationSubContractWorkItem_Read([DataSourceRequest]DataSourceRequest request, int type)
        {
            var projectType = (AvailableProjectTypes)type;
            SetDefaultValue();
            IQueryable<EstimationSubContractWorkItem> EstimationSubContractWorkItems =
                    db.EstimationSubContractWorkItem.Where(x => x.ProjectType == projectType);
            DataSourceResult result = EstimationSubContractWorkItems.ToDataSourceResult(request,
                    EstimationSubContractWorkItem => new
                    {
                        EstimationSubContractWorkItemId = EstimationSubContractWorkItem.EstimationSubContractWorkItemId,
                        WorkCategoryId = EstimationSubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = EstimationSubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = EstimationSubContractWorkItem.WorkItemName,
                        ItemCode = EstimationSubContractWorkItem.ItemCode,
                        Quntity = EstimationSubContractWorkItem.Quntity,
                        SubContractCostOne = EstimationSubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = EstimationSubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = EstimationSubContractWorkItem.SubContractCostThree,
                        SelectedCost = EstimationSubContractWorkItem.SelectedCost,
                        TotalCost = EstimationSubContractWorkItem.TotalCost,
                        ProjectType = EstimationSubContractWorkItem.ProjectType,
                        OverHeadProfit = EstimationSubContractWorkItem.OverHeadProfit,
                        OrgBranchName = EstimationSubContractWorkItem.OrgBranchName,
                        DateCreated = EstimationSubContractWorkItem.DateCreated,
                        LastModified = EstimationSubContractWorkItem.LastModified,
                        CreatedBy = EstimationSubContractWorkItem.CreatedBy,
                        ModifiedBy = EstimationSubContractWorkItem.ModifiedBy,
                        BlockId = EstimationSubContractWorkItem.BlockId,

                    });

            return Json(result);
            //}

        }

        public ActionResult TemplateWorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int strId, int subProjTypeMappingId)
        {
            List<CopyTemplateSubContractWorkItemViewModel> listToReturn = new List<CopyTemplateSubContractWorkItemViewModel>();

            var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
            if (mapping != null)
            {
                var currentSubProjectType = db.SubProjectType.Find(mapping.SubProjectTypeId);
                if (currentSubProjectType != null)
                {
                    var project = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                    if (project != null)
                    {
                        var templateWorkItems = db.EstimationSubContractWorkItem
                                        .Where(tw => tw.ProjectType == project.ProjectType &&
                                        tw.WorkCategory.ConfigurationId == strId
                                        ).ToList();

                        if (templateWorkItems.Any())
                        {
                            foreach (EstimationSubContractWorkItem workItem in templateWorkItems)
                            {
                                // check if the workItem is copied already 
                                var workItemInProject =
                                    db.GenericSubContractWorkItem.FirstOrDefault(
                                        wpp => wpp.ItemCode == workItem.ItemCode &&
                                        wpp.WorkCategory.ConfigurationId == strId &&
                                        wpp.WorkItemName == workItem.WorkItemName &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);

                                var templateToReturn = new CopyTemplateSubContractWorkItemViewModel
                                {
                                    Id = workItem.EstimationSubContractWorkItemId,
                                    WorkItemName = workItem.WorkItemName,
                                    WorkCategory = workItem.WorkCategory.Name,
                                    //TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                    //UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                    TotalCost = workItem.TotalCost,
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
        public ActionResult EstimationSubContractWorkItem_Create([DataSourceRequest]DataSourceRequest request, EstimationSubContractWorkItem EstimationSubContractWorkItem, int type)
        {
            var projectType = (AvailableProjectTypes)type;

            if (ModelState.IsValid)
            {
                //if (ProjectEstimationWorkItemController.ProjectId == 0)
                //{

                //    return new JavaScriptResult() { Script = "alert('" + "Please select the project first!!" + "');" };
                //}
                var entity = new EstimationSubContractWorkItem
                {
                    EstimationSubContractWorkItemId = EstimationSubContractWorkItem.EstimationSubContractWorkItemId,
                    WorkCategoryId = EstimationSubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = EstimationSubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = EstimationSubContractWorkItem.WorkItemName,
                    ItemCode = EstimationSubContractWorkItem.ItemCode,
                    Quntity = EstimationSubContractWorkItem.Quntity,
                    SubContractCostOne = EstimationSubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = EstimationSubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = EstimationSubContractWorkItem.SubContractCostThree,
                    SelectedCost = EstimationSubContractWorkItem.SelectedCost,
                    TotalCost = EstimationSubContractWorkItem.TotalCost,
                    ProjectType = projectType,

                    OrgBranchName = EstimationSubContractWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = EstimationSubContractWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = EstimationSubContractWorkItem.ModifiedBy,
                    BlockId = ProjectEstimationWorkItemController.BlockId,
                };
                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCost / 100)) +
                                          (entity.SelectedCost * (ProfitMargin / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;

                db.EstimationSubContractWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                EstimationSubContractWorkItem.EstimationSubContractWorkItemId = entity.EstimationSubContractWorkItemId;

            }

            return Json(new[] { EstimationSubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationSubContractWorkItem_Update([DataSourceRequest]DataSourceRequest request, EstimationSubContractWorkItem EstimationSubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationSubContractWorkItem
                {
                    EstimationSubContractWorkItemId = EstimationSubContractWorkItem.EstimationSubContractWorkItemId,
                    WorkCategoryId = EstimationSubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = EstimationSubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = EstimationSubContractWorkItem.WorkItemName,
                    ItemCode = EstimationSubContractWorkItem.ItemCode,
                    Quntity = EstimationSubContractWorkItem.Quntity,
                    SubContractCostOne = EstimationSubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = EstimationSubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = EstimationSubContractWorkItem.SubContractCostThree,
                    SelectedCost = EstimationSubContractWorkItem.SelectedCost,
                    TotalCost = EstimationSubContractWorkItem.TotalCost,
                    ProjectType = EstimationSubContractWorkItem.ProjectType,

                    OrgBranchName = EstimationSubContractWorkItem.OrgBranchName,
                    DateCreated = EstimationSubContractWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = EstimationSubContractWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    BlockId = EstimationSubContractWorkItem.BlockId,
                };

                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCost / 100)) +
                                          (entity.SelectedCost * (ProfitMargin / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;
                db = new ExceedDbContext();
                db.EstimationSubContractWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { EstimationSubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EstimationSubContractWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, EstimationSubContractWorkItem EstimationSubContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationSubContractWorkItem
                {
                    EstimationSubContractWorkItemId = EstimationSubContractWorkItem.EstimationSubContractWorkItemId,
                    WorkCategoryId = EstimationSubContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = EstimationSubContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = EstimationSubContractWorkItem.WorkItemName,
                    ItemCode = EstimationSubContractWorkItem.ItemCode,
                    Quntity = EstimationSubContractWorkItem.Quntity,
                    SubContractCostOne = EstimationSubContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = EstimationSubContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = EstimationSubContractWorkItem.SubContractCostThree,
                    SelectedCost = EstimationSubContractWorkItem.SelectedCost,
                    TotalCost = EstimationSubContractWorkItem.TotalCost,
                    ProjectType = EstimationSubContractWorkItem.ProjectType,
                    OrgBranchName = EstimationSubContractWorkItem.OrgBranchName,
                    DateCreated = EstimationSubContractWorkItem.DateCreated,
                    LastModified = EstimationSubContractWorkItem.LastModified,
                    CreatedBy = EstimationSubContractWorkItem.CreatedBy,
                    ModifiedBy = EstimationSubContractWorkItem.ModifiedBy,
                    BlockId = EstimationSubContractWorkItem.BlockId,
                };

                db.EstimationSubContractWorkItem.Attach(entity);
                db.EstimationSubContractWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { EstimationSubContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        private void SetDefaultValue()
        {
            var dataB = new ExceedDbContext();
            var configartion = dataB.Configurations.ToList();
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
