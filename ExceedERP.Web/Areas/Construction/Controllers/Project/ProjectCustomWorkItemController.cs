using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectCustomWorkItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index(string name)
        //{

        //    return View();
        //}

        public ActionResult CustomWorkItem_Read([DataSourceRequest]DataSourceRequest request)
        {
            
            IQueryable<ProjectCustomWorkItem> customWorkItems = db.ProjectCustomWorkItems;
            DataSourceResult result = customWorkItems.ToDataSourceResult(request, customWorkItem => new
            {
                ProjectCustomWorkItemId = customWorkItem.ProjectCustomWorkItemId,
                ProjectInformationId = customWorkItem.ProjectInformationId,
                BlockId = customWorkItem.BlockId,
                ProjectType = customWorkItem.ProjectType,
                WorkCategoryId = customWorkItem.WorkCategoryId,
                WorkItemName = customWorkItem.WorkItemName,
                TotalUnitRate = customWorkItem.TotalUnitRate,
                Quantity = customWorkItem.Quantity,
                TotalCost = customWorkItem.TotalCost,
                Remark = customWorkItem.Remark,
                ItemCode = customWorkItem.ItemCode,

                OrgBranchName = customWorkItem.OrgBranchName,
                DateCreated = customWorkItem.DateCreated,
                LastModified = customWorkItem.LastModified,
                CreatedBy = customWorkItem.CreatedBy,
                ModifiedBy = customWorkItem.ModifiedBy,
            });

            return Json(result);

        }
        public ActionResult CustomWorkItemPerStructure_Read([DataSourceRequest]DataSourceRequest request, int strId, int blockId)
        {
            IQueryable<WorkCategory> workcategorys = db.WorkCategory.Where(wc => wc.ConfigurationId == strId);
            List<ProjectCustomWorkItem> workItems = new List<ProjectCustomWorkItem>();
            BlockMapping currentBlock = new BlockMapping();

            BlockMapping otherOne = new BlockMapping();
            var temp2 = db.BlockMappings.Find(blockId);
            if (temp2 != null)
            {
                otherOne = temp2;
            }

            foreach (WorkCategory category in workcategorys)
            {
                var workItemList = db.ProjectCustomWorkItems.Where(wi => wi.WorkCategoryId == category.WorkCategoryId && wi.BlockId == temp2.BlockId);
                if (workItemList.Any())
                {
                    foreach (ProjectCustomWorkItem item in workItemList)
                    {
                        workItems.Add(item);
                    }
                }
            }
            IQueryable<ProjectCustomWorkItem> workItemsToReturn = workItems.AsQueryable();
            
            DataSourceResult result = workItemsToReturn.ToDataSourceResult(request, customWorkItem => new
            {
                ProjectCustomWorkItemId = customWorkItem.ProjectCustomWorkItemId,
                ProjectInformationId = customWorkItem.ProjectInformationId,
                BlockId = customWorkItem.BlockId,
                ProjectType = customWorkItem.ProjectType,
                WorkCategoryId = customWorkItem.WorkCategoryId,
                WorkItemName = customWorkItem.WorkItemName,
                TotalUnitRate = customWorkItem.TotalUnitRate,
                Quantity = customWorkItem.Quantity,
                TotalCost = customWorkItem.TotalCost,
                Remark = customWorkItem.Remark,
                ItemCode = customWorkItem.ItemCode,

                OrgBranchName = customWorkItem.OrgBranchName,
                DateCreated = customWorkItem.DateCreated,
                LastModified = customWorkItem.LastModified,
                CreatedBy = customWorkItem.CreatedBy,
                ModifiedBy = customWorkItem.ModifiedBy
            });

            return Json(result);

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Create([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCustomWorkItem
                {
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    BlockId = customWorkItem.BlockId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Quantity = customWorkItem.Quantity,
                    TotalCost = customWorkItem.TotalCost,
                    Remark = customWorkItem.Remark,

                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = customWorkItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = customWorkItem.ModifiedBy,
                };

                db.ProjectCustomWorkItems.Add(entity);
                db.SaveChanges();
                customWorkItem.ProjectCustomWorkItemId = entity.ProjectCustomWorkItemId;

            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Update([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCustomWorkItem
                {
                    ProjectCustomWorkItemId = customWorkItem.ProjectCustomWorkItemId,
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    BlockId = customWorkItem.BlockId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Quantity = customWorkItem.Quantity,
                    TotalCost = customWorkItem.Quantity * customWorkItem.TotalUnitRate,
                    Remark = customWorkItem.Remark,


                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = customWorkItem.DateCreated,
                    LastModified = DateTime.Now.Date,
                    CreatedBy = customWorkItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ProjectCustomWorkItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCustomWorkItem customWorkItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCustomWorkItem
                {
                    ProjectCustomWorkItemId = customWorkItem.ProjectCustomWorkItemId,
                    ProjectInformationId = customWorkItem.ProjectInformationId,
                    BlockId = customWorkItem.BlockId,
                    WorkCategoryId = customWorkItem.WorkCategoryId,
                    ProjectType = customWorkItem.ProjectType,
                    WorkItemName = customWorkItem.WorkItemName,
                    TotalUnitRate = customWorkItem.TotalUnitRate,
                    Quantity = customWorkItem.Quantity,
                    TotalCost = customWorkItem.TotalCost,
                    Remark = customWorkItem.Remark,

                    OrgBranchName = customWorkItem.OrgBranchName,
                    DateCreated = customWorkItem.DateCreated,
                    LastModified = customWorkItem.LastModified,
                    CreatedBy = customWorkItem.CreatedBy,
                    ModifiedBy = customWorkItem.ModifiedBy,
                };

                db.ProjectCustomWorkItems.Attach(entity);
                db.ProjectCustomWorkItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { customWorkItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}