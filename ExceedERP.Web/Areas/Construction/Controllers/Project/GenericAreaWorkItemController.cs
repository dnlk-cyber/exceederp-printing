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
    public class GenericAreaWorkItemController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private static decimal OverHeadCostRate { get; set; }
        private static decimal ProfitMarginRate { get; set; }
        ConstructionInputs inputs;

        public GenericAreaWorkItemController()
        {
            db = new ExceedDbContext();
            OverHeadCostRate = ConsSettingHelper.GetCurrentOverHeadRate();
            ProfitMarginRate = ConsSettingHelper.GetCurrentProfitRate();
        }
        public ActionResult AreaWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GenericAreaWorkItem> AreaWorkItems = db.GenericAreaWorkItem.AsQueryable();
            //AreaWorkItems =
            //        db.GenericAreaWorkItem.Where(x => x.ProjectInformationId == ida);
            DataSourceResult result = AreaWorkItems.ToDataSourceResult(request,
                    AreaWorkItem => new
                    {
                        GenericAreaWorkItemId = AreaWorkItem.GenericAreaWorkItemId,
                        WorkCategoryId = AreaWorkItem.WorkCategoryId,
                        Price = AreaWorkItem.Price,
                        WorkItemName = AreaWorkItem.WorkItemName,
                        ItemCode = AreaWorkItem.ItemCode,
                        DirectCost = AreaWorkItem.DirectCost,
                        Width = AreaWorkItem.Width,
                        Height = AreaWorkItem.Height,
                        Area = AreaWorkItem.Area,
                        CylinderLock = AreaWorkItem.CylinderLock,
                        OverHeadProfitCost = AreaWorkItem.OverHeadProfitCost,
                        ProjectInformationId = AreaWorkItem.ProjectInformationId,
                        MaterialCost = AreaWorkItem.MaterialCost,
                        OrgBranchName = AreaWorkItem.OrgBranchName,
                        DateCreated = AreaWorkItem.DateCreated,
                        LastModified = AreaWorkItem.LastModified,
                        CreatedBy = AreaWorkItem.CreatedBy,
                        ModifiedBy = AreaWorkItem.ModifiedBy,
                        TransportCost = AreaWorkItem.TransportCost,
                    });

            return Json(result);
        }

        public ActionResult AreaWorkItemPerStructure([DataSourceRequest]DataSourceRequest request, int sId, int subProjectMappingId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == sId);
            List<GenericAreaWorkItem> workItems = new List<GenericAreaWorkItem>();

            SubProjectStructureTypeMapping mapping = new SubProjectStructureTypeMapping();
            var temp = db.SubProjectStructureTypeMapping.Find(subProjectMappingId);
            if (temp != null)
            {
                mapping = temp;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.GenericAreaWorkItem.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.SubProjectTypeId == mapping.SubProjectTypeId);
                if (workItemList != null && workItemList.Count() > 0)
                {
                    foreach (GenericAreaWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<GenericAreaWorkItem> workItemsToReturn = workItems.AsQueryable();
            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request,
                    areaWorkItem => new
                    {
                        GenericAreaWorkItemId = areaWorkItem.GenericAreaWorkItemId,
                        WorkCategoryId = areaWorkItem.WorkCategoryId,
                        Price = areaWorkItem.Price,
                        WorkItemName = areaWorkItem.WorkItemName,
                        ItemCode = areaWorkItem.ItemCode,
                        DirectCost = areaWorkItem.DirectCost,
                        Width = areaWorkItem.Width,
                        Height = areaWorkItem.Height,
                        Area = areaWorkItem.Area,
                        CylinderLock = areaWorkItem.CylinderLock,
                        OverHeadProfitCost = areaWorkItem.OverHeadProfitCost,
                        ProjectInformationId = areaWorkItem.ProjectInformationId,
                        MaterialCost = areaWorkItem.MaterialCost,
                        OrgBranchName = areaWorkItem.OrgBranchName,
                        DateCreated = areaWorkItem.DateCreated,
                        LastModified = areaWorkItem.LastModified,
                        CreatedBy = areaWorkItem.CreatedBy,
                        ModifiedBy = areaWorkItem.ModifiedBy,
                        TransportCost = areaWorkItem.TransportCost,
                        SubProjectTypeId = areaWorkItem.SubProjectTypeId
                    });

            return Json(result);
        }

        public ActionResult WorkItemToCopyToStructre_Read([DataSourceRequest]DataSourceRequest request, int destinationSubProjectTypeMappingId, int sourceSubProjTypeMappingId)
        {
            List<GenericPieceRateCopyViewModel> listToReturn = new List<GenericPieceRateCopyViewModel>();

            var sourceMapping = db.SubProjectStructureTypeMapping.Find(sourceSubProjTypeMappingId);
            var destinationMapping = db.SubProjectStructureTypeMapping.Find(destinationSubProjectTypeMappingId);
            if (sourceMapping != null && destinationMapping != null)
            {
                //var workCategories 
                var sourceSubProjectType = db.SubProjectType.Find(sourceMapping.SubProjectTypeId);
                var destinationSubProjectType = db.SubProjectType.Find(destinationMapping.SubProjectTypeId);
                if (sourceSubProjectType != null && destinationSubProjectType != null)
                {
                    var workItemsToCopy = db.GenericAreaWorkItem
                        .Where(w => w.SubProjectTypeId == sourceMapping.SubProjectTypeId &&
                        w.WorkCategory.ConfigurationId == sourceMapping.ConfigurationId
                        ).ToList();

                    foreach (GenericAreaWorkItem workItemToCopy in workItemsToCopy)
                    {
                        var existingWorkItem =
                            db.GenericAreaWorkItem
                            .FirstOrDefault(w => w.WorkItemName == workItemToCopy.WorkItemName &&
                                w.ItemCode == workItemToCopy.ItemCode &&
                                w.SubProjectTypeId == destinationSubProjectType.SubProjectTypeId &&
                                w.WorkCategory.ConfigurationId == destinationMapping.ConfigurationId);

                        var item = new GenericPieceRateCopyViewModel
                        {
                            Id = workItemToCopy.GenericAreaWorkItemId,
                            CategoryName = workItemToCopy.WorkCategory.Name,
                            WorkItemName = workItemToCopy.WorkItemName,
                            Price = workItemToCopy.Price,
                            ItemCode = workItemToCopy.ItemCode,
                            Area = workItemToCopy.Area,
                            OverHeadProfitCost = workItemToCopy.OverHeadProfitCost,
                            DirectCost = workItemToCopy.DirectCost

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
        public ActionResult AreaWorkItem_Create([DataSourceRequest]DataSourceRequest request, GenericAreaWorkItem areaWorkItem, int idaTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericAreaWorkItem
                {
                    GenericAreaWorkItemId = areaWorkItem.GenericAreaWorkItemId,
                    Price = areaWorkItem.Price,
                    WorkCategoryId = areaWorkItem.WorkCategoryId,
                    WorkItemName = areaWorkItem.WorkItemName,
                    ItemCode = areaWorkItem.ItemCode,
                    DirectCost = areaWorkItem.DirectCost,
                    Width = areaWorkItem.Width,
                    Height = areaWorkItem.Height,
                    Area = areaWorkItem.Area,
                    CylinderLock = areaWorkItem.CylinderLock,
                    OverHeadProfitCost = areaWorkItem.OverHeadProfitCost,
                    TransportCost = areaWorkItem.TransportCost,
                    ProjectInformationId = idaTwo,
                    MaterialCost = areaWorkItem.MaterialCost,
                    SubProjectTypeId = areaWorkItem.SubProjectTypeId,

                    OrgBranchName = areaWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = areaWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = areaWorkItem.ModifiedBy
                };
                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCostRate / 100)) +
                                         (entity.DirectCost * (ProfitMarginRate / 100));
                db.GenericAreaWorkItem.Add(entity);
                db.SaveChanges();
                //ProjectEstimationWorkItemController.ProjectId = 0;
                areaWorkItem.GenericAreaWorkItemId = entity.GenericAreaWorkItemId;

            }

            return Json(new[] { areaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Update([DataSourceRequest]DataSourceRequest request, GenericAreaWorkItem areaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericAreaWorkItem
                {
                    GenericAreaWorkItemId = areaWorkItem.GenericAreaWorkItemId,
                    WorkCategoryId = areaWorkItem.WorkCategoryId,
                    Price = areaWorkItem.Price,
                    WorkItemName = areaWorkItem.WorkItemName,
                    ItemCode = areaWorkItem.ItemCode,
                    DirectCost = areaWorkItem.DirectCost,
                    Width = areaWorkItem.Width,
                    Height = areaWorkItem.Height,
                    Area = areaWorkItem.Area,
                    TransportCost = areaWorkItem.TransportCost,
                    CylinderLock = areaWorkItem.CylinderLock,
                    OverHeadProfitCost = areaWorkItem.OverHeadProfitCost,

                    ProjectInformationId = areaWorkItem.ProjectInformationId,
                    MaterialCost = areaWorkItem.MaterialCost,

                    SubProjectTypeId = areaWorkItem.SubProjectTypeId,

                    OrgBranchName = areaWorkItem.OrgBranchName,
                    DateCreated = areaWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = areaWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.Area = entity.Width * entity.Height;
                entity.MaterialCost = entity.Area * entity.Price;
                entity.DirectCost = entity.MaterialCost + entity.TransportCost + entity.CylinderLock;
                entity.OverHeadProfitCost = entity.DirectCost + (entity.DirectCost * (OverHeadCostRate / 100)) +
                                            (entity.DirectCost * (ProfitMarginRate / 100));

                db.GenericAreaWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { areaWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AreaWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, GenericAreaWorkItem areaWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericAreaWorkItem
                {
                    GenericAreaWorkItemId = areaWorkItem.GenericAreaWorkItemId,
                    WorkCategoryId = areaWorkItem.WorkCategoryId,
                    WorkItemName = areaWorkItem.WorkItemName,
                    ItemCode = areaWorkItem.ItemCode,
                    DirectCost = areaWorkItem.DirectCost,
                    Width = areaWorkItem.Width,
                    Height = areaWorkItem.Height,
                    Price = areaWorkItem.Price,
                    Area = areaWorkItem.Area,
                    CylinderLock = areaWorkItem.CylinderLock,
                    OverHeadProfitCost = areaWorkItem.OverHeadProfitCost,
                    TransportCost = areaWorkItem.TransportCost,
                    ProjectInformationId = areaWorkItem.ProjectInformationId,
                    MaterialCost = areaWorkItem.MaterialCost,
                    SubProjectTypeId = areaWorkItem.SubProjectTypeId,

                    OrgBranchName = areaWorkItem.OrgBranchName,
                    DateCreated = areaWorkItem.DateCreated,
                    LastModified = areaWorkItem.LastModified,
                    CreatedBy = areaWorkItem.CreatedBy,
                    ModifiedBy = areaWorkItem.ModifiedBy
                };

                db.GenericAreaWorkItem.Attach(entity);
                db.GenericAreaWorkItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { areaWorkItem }.ToDataSourceResult(request, ModelState));
        }
    }
}