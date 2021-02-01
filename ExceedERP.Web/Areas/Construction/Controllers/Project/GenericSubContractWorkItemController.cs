using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class GenericSubContractWorkItemController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private static decimal OverHeadCostRate { get; set; }
        private static decimal ProfitMarginRate { get; set; }
        ConstructionInputs inputs;

        public GenericSubContractWorkItemController()
        {
            db = new ExceedDbContext();
            OverHeadCostRate = ConsSettingHelper.GetCurrentOverHeadRate();
            ProfitMarginRate = ConsSettingHelper.GetCurrentProfitRate();
        }

        public ActionResult SubContractWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GenericSubContractWorkItem> SubContractWorkItems = db.GenericSubContractWorkItem.AsQueryable();
            
            //SubContractWorkItems =
            //        db.GenericSubContractWorkItem.Where(x => x.ProjectInformationId == ids);
            DataSourceResult result = SubContractWorkItems.ToDataSourceResult(request,
                    SubContractWorkItem => new
                    {
                        GenericSubContractWorkItemId = SubContractWorkItem.GenericSubContractWorkItemId,
                        WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = SubContractWorkItem.WorkItemName,
                        ItemCode = SubContractWorkItem.ItemCode,
                        Quntity = SubContractWorkItem.Quntity,
                        SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                        SelectedCost = SubContractWorkItem.SelectedCost,
                        TotalCost = SubContractWorkItem.TotalCost,
                        ProjectInformationId = SubContractWorkItem.ProjectInformationId,
                        SubProjectTypeId = SubContractWorkItem.SubProjectTypeId,
                        OverHeadProfit = SubContractWorkItem.OverHeadProfit,

                        OrgBranchName = SubContractWorkItem.OrgBranchName,
                        DateCreated = SubContractWorkItem.DateCreated,
                        LastModified = SubContractWorkItem.LastModified,
                        CreatedBy = SubContractWorkItem.CreatedBy,
                        ModifiedBy = SubContractWorkItem.ModifiedBy,

                    });

            return Json(result);
            //}

        }

        public ActionResult SubContractWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int strIdFRead, int subProjectMappingId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strIdFRead);
            List<GenericSubContractWorkItem> workItems = new List<GenericSubContractWorkItem>();
            //SubProjectStructureTypeMapping subProjectTypeMapping = new SubProjectStructureTypeMapping();

            SubProjectStructureTypeMapping mapping = new SubProjectStructureTypeMapping();
            var temp = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);
            if (temp != null)
            {
                mapping = temp;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.GenericSubContractWorkItem.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.SubProjectTypeId == mapping.SubProjectTypeId);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (GenericSubContractWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<GenericSubContractWorkItem> workItemsToReturn = workItems.AsQueryable();

            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request,
                    SubContractWorkItem => new
                    {
                        GenericSubContractWorkItemId = SubContractWorkItem.GenericSubContractWorkItemId,
                        WorkCategoryId = SubContractWorkItem.WorkCategoryId,
                        UnitOfMeasurementId = SubContractWorkItem.UnitOfMeasurementId,
                        WorkItemName = SubContractWorkItem.WorkItemName,
                        ItemCode = SubContractWorkItem.ItemCode,
                        Quntity = SubContractWorkItem.Quntity,
                        SubContractCostOne = SubContractWorkItem.SubContractCostOne,
                        SubContractCostTwo = SubContractWorkItem.SubContractCostTwo,
                        SubContractCostThree = SubContractWorkItem.SubContractCostThree,
                        SelectedCost = SubContractWorkItem.SelectedCost,
                        TotalCost = SubContractWorkItem.TotalCost,
                        ProjectInformationId = SubContractWorkItem.ProjectInformationId,
                        SubProjectTypeId = SubContractWorkItem.SubProjectTypeId,
                        OverHeadProfit = SubContractWorkItem.OverHeadProfit,

                        OrgBranchName = SubContractWorkItem.OrgBranchName,
                        DateCreated = SubContractWorkItem.DateCreated,
                        LastModified = SubContractWorkItem.LastModified,
                        CreatedBy = SubContractWorkItem.CreatedBy,
                        ModifiedBy = SubContractWorkItem.ModifiedBy,

                    });

            return Json(result);

        }

        public ActionResult WorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int destinationSubProjectTypeMappingId, int sourceSubProjTypeMappingId)
        {
            List<GenericSubContractWorkItemCopyViewModel> listToReturn = new List<GenericSubContractWorkItemCopyViewModel>();

            var sourceMapping = db.SubProjectStructureTypeMapping.Find(sourceSubProjTypeMappingId);
            var destinationMapping = db.SubProjectStructureTypeMapping.Find(destinationSubProjectTypeMappingId);
            if (sourceMapping != null && destinationMapping != null)
            {
                //var workCategories 
                var sourceSubProjectType = db.SubProjectType.Find(sourceMapping.SubProjectTypeId);
                var destinationSubProjectType = db.SubProjectType.Find(destinationMapping.SubProjectTypeId);
                if (sourceSubProjectType != null && destinationSubProjectType != null)
                {
                    var workItemsToCopy = db.GenericSubContractWorkItem
                        .Where(w => w.SubProjectTypeId == sourceMapping.SubProjectTypeId &&
                        w.WorkCategory.ConfigurationId == sourceMapping.ConfigurationId
                        ).ToList();

                    foreach (GenericSubContractWorkItem workItemToCopy in workItemsToCopy)
                    {
                        var existingWorkItem = 
                            db.GenericSubContractWorkItem
                            .FirstOrDefault(w => w.WorkItemName == workItemToCopy.WorkItemName &&
                                w.ItemCode == workItemToCopy.ItemCode &&
                                w.SubProjectTypeId == destinationSubProjectType.SubProjectTypeId && 
                                w.WorkCategory.ConfigurationId == destinationMapping.ConfigurationId);

                        var item = new GenericSubContractWorkItemCopyViewModel
                        {
                            Id = workItemToCopy.GenericSubContractWorkItemId,
                            WorkCategory = workItemToCopy.WorkCategory.Name,
                            WorkItemName = workItemToCopy.WorkItemName,
                            TotalCost = workItemToCopy.TotalCost,
                            ItemCode = workItemToCopy.ItemCode,
                            UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemToCopy.UnitOfMeasurementId),
                            SelectedCost = workItemToCopy.SelectedCost,
                            OverHeadProfit = workItemToCopy.OverHeadProfit
                        };
                        if (existingWorkItem != null)
                        {
                            item.IsCopied = true;
                        }
                        listToReturn.Add(item);
                    }
                }
            }

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Create([DataSourceRequest]DataSourceRequest request, GenericSubContractWorkItem subContractWorkItem, int subProjectMappingId)
        {
            if (ModelState.IsValid)
            {

                var entity = new GenericSubContractWorkItem
                {
                    GenericSubContractWorkItemId = subContractWorkItem.GenericSubContractWorkItemId,
                    WorkCategoryId = subContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = subContractWorkItem.WorkItemName,
                    ItemCode = subContractWorkItem.ItemCode,
                    Quntity = subContractWorkItem.Quntity,
                    SubContractCostOne = subContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = subContractWorkItem.SubContractCostThree,
                    SelectedCost = subContractWorkItem.SelectedCost,
                    TotalCost = subContractWorkItem.TotalCost,
                    SubProjectTypeId = subProjectMappingId,
                    OverHeadProfit = subContractWorkItem.OverHeadProfit,
                    ProjectInformationId = subContractWorkItem.ProjectInformationId,

                    OrgBranchName = subContractWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = subContractWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = subContractWorkItem.ModifiedBy,
                };
                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCostRate / 100)) +
                                          (entity.SelectedCost * (ProfitMarginRate / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;

                db.GenericSubContractWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                subContractWorkItem.GenericSubContractWorkItemId = entity.GenericSubContractWorkItemId;

            }

            return Json(new[] { subContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Update([DataSourceRequest]DataSourceRequest request, GenericSubContractWorkItem subContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericSubContractWorkItem
                {
                    GenericSubContractWorkItemId = subContractWorkItem.GenericSubContractWorkItemId,
                    WorkCategoryId = subContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = subContractWorkItem.WorkItemName,
                    ItemCode = subContractWorkItem.ItemCode,
                    Quntity = subContractWorkItem.Quntity,
                    SubContractCostOne = subContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = subContractWorkItem.SubContractCostThree,
                    SelectedCost = subContractWorkItem.SelectedCost,
                    TotalCost = subContractWorkItem.TotalCost,
                    ProjectInformationId = subContractWorkItem.ProjectInformationId,
                    SubProjectTypeId = subContractWorkItem.SubProjectTypeId,
                    OverHeadProfit = subContractWorkItem.OverHeadProfit,

                    OrgBranchName = subContractWorkItem.OrgBranchName,
                    DateCreated = subContractWorkItem.DateCreated,
                    LastModified = subContractWorkItem.LastModified,
                    CreatedBy = subContractWorkItem.CreatedBy,
                    ModifiedBy = subContractWorkItem.ModifiedBy
                };

                entity.SelectedCost = (entity.SubContractCostOne + entity.SubContractCostTwo + entity.SubContractCostThree) / 3;
                entity.OverHeadProfit = (entity.SelectedCost * (OverHeadCostRate / 100)) +
                                          (entity.SelectedCost * (ProfitMarginRate / 100));
                entity.TotalCost = entity.SelectedCost + entity.OverHeadProfit;

                db.GenericSubContractWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { subContractWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubContractWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, GenericSubContractWorkItem subContractWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericSubContractWorkItem
                {
                    GenericSubContractWorkItemId = subContractWorkItem.GenericSubContractWorkItemId,
                    WorkCategoryId = subContractWorkItem.WorkCategoryId,
                    UnitOfMeasurementId = subContractWorkItem.UnitOfMeasurementId,
                    WorkItemName = subContractWorkItem.WorkItemName,
                    ItemCode = subContractWorkItem.ItemCode,
                    Quntity = subContractWorkItem.Quntity,
                    SubContractCostOne = subContractWorkItem.SubContractCostOne,
                    SubContractCostTwo = subContractWorkItem.SubContractCostTwo,
                    SubContractCostThree = subContractWorkItem.SubContractCostThree,
                    SelectedCost = subContractWorkItem.SelectedCost,
                    TotalCost = subContractWorkItem.TotalCost,
                    ProjectInformationId = subContractWorkItem.ProjectInformationId,
                    SubProjectTypeId = subContractWorkItem.SubProjectTypeId,
                    OverHeadProfit = subContractWorkItem.OverHeadProfit,
                     
                    OrgBranchName = subContractWorkItem.OrgBranchName,
                    DateCreated = subContractWorkItem.DateCreated,
                    LastModified = subContractWorkItem.LastModified,
                    CreatedBy = subContractWorkItem.CreatedBy,
                    ModifiedBy = subContractWorkItem.ModifiedBy
                };

                db.GenericSubContractWorkItem.Attach(entity);
                db.GenericSubContractWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subContractWorkItem }.ToDataSourceResult(request, ModelState));
        }
    }
}