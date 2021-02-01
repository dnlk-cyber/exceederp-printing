using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    [AuthorizeRoles(ConstructionConstants.CostingUser)]
    public class DetailedWorkProgressController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted && ids.Contains(p.ProjectInformationId));
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }
        public ActionResult ResourceUtilizationWorkItem_Read([DataSourceRequest]DataSourceRequest request, int dayId)
        {
            List<DetailedWorkProgressViewModel> list = new List<DetailedWorkProgressViewModel>();
            var items = db.DetailedWorkProgress.Where(dwp => dwp.ResourceUtilizationDayId == dayId && dwp.WorkItemPerProject.ParentId == 0).ToList();
            foreach (DetailedWorkProgress workProgress in items)
            {
                var record = new DetailedWorkProgressViewModel
                {
                    Id = workProgress.DetailedWorkProgressId,
                    WorkItemPerProjectId = workProgress.WorkItemPerProjectId,
                    ItemCode = workProgress.WorkItemPerProject.ItemCode,
                    UoM = GetUnitOfMeasurement(workProgress.WorkItemPerProject.UnitOfMeasurementId??0),
                    ItemName = workProgress.WorkItemPerProject.ItemName,
                    ContractQuantity = workProgress.WorkItemPerProject.TotalQuantityOfWork,
                    TakeOffQuantity = workProgress.WorkItemPerProject.ActualQuantityOfWork,
                    UnitPrice = workProgress.WorkItemPerProject.TotalUnitCost,

                    IsSubCategory = workProgress.WorkItemPerProject.IsSubCategory,
                    ParentId = workProgress.WorkItemPerProject.ParentId,

                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workProgress.WorkItemPerProject.BlockId),
                    BlockName = ProjectEstimationHelper.GetBlockName(workProgress.WorkItemPerProject.BlockId),
                    StructureType = ConsSettingHelper.GetStructureTypeCode(workProgress.WorkItemPerProject.WorkCategory.ConfigurationId, workProgress.WorkItemPerProject.ProjectType) + " " + workProgress.WorkItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workProgress.WorkItemPerProject.WorkCategory.Index + " " + workProgress.WorkItemPerProject.WorkCategory.Name,

                    ActualQuantity = workProgress.ActualQuantity,
                    ActualAmount = workProgress.ActualAmount,
                    PlannedQuantity = workProgress.PlannedQuantity,
                    PlannedAmount = workProgress.PlannedAmount,

                    PreviousActualQuantity = workProgress.PreviousActualQuantity,
                    PreviousActualAmount = workProgress.PreviousActualAmount,
                    PreviousPlannedQuantity = workProgress.PreviousPlannedQuantity,
                    PreviousPlannedAmount = workProgress.PreviousPlannedAmount,

                    TodateActualQuantity = workProgress.TodateActualQuantity,
                    TodateActualAmount = workProgress.TodateActualAmount,
                    TodatePlannedQuantity = workProgress.TodatePlannedQuantity,
                    TodatePlannedAmount = workProgress.TodatePlannedAmount,
                };
                list.Add(record);
            }
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult ResourceUtilizationWorkItem_Child_Read([DataSourceRequest]DataSourceRequest request, int actualParentId, int sourceItemParentId)
        {
            List<DetailedWorkProgressViewModel> list = new List<DetailedWorkProgressViewModel>();
            var parentItem = db.DetailedWorkProgress.Find(actualParentId);
            if(parentItem != null)
            {
                var childItems = db.DetailedWorkProgress.Where(dwp => dwp.ResourceUtilizationDayId == parentItem.ResourceUtilizationDayId && dwp.WorkItemPerProject.ParentId == sourceItemParentId).ToList();

                foreach (DetailedWorkProgress workProgress in childItems)
                {
                    var record = new DetailedWorkProgressViewModel
                    {
                        Id = workProgress.DetailedWorkProgressId,
                        WorkItemPerProjectId = workProgress.WorkItemPerProjectId,
                        ItemCode = workProgress.WorkItemPerProject.ItemCode,
                        UoM = GetUnitOfMeasurement(workProgress.WorkItemPerProject.UnitOfMeasurementId ?? 0),
                        ItemName = workProgress.WorkItemPerProject.ItemName,
                        ContractQuantity = workProgress.WorkItemPerProject.TotalQuantityOfWork,
                        TakeOffQuantity = workProgress.WorkItemPerProject.ActualQuantityOfWork,
                        UnitPrice = workProgress.WorkItemPerProject.TotalUnitCost,

                        IsSubCategory = workProgress.WorkItemPerProject.IsSubCategory,
                        ParentId = workProgress.WorkItemPerProject.ParentId,

                        //SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workProgress.WorkItemPerProject.BlockId),
                        //BlockName = ProjectEstimationHelper.GetBlockName(workProgress.WorkItemPerProject.BlockId),
                        //StructureType = ConsSettingHelper.GetStructureTypeCode(workProgress.WorkItemPerProject.WorkCategory.ConfigurationId, workProgress.WorkItemPerProject.ProjectType) + " " + workProgress.WorkItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                        //WorkCategory = workProgress.WorkItemPerProject.WorkCategory.Index + " " + workProgress.WorkItemPerProject.WorkCategory.Name,

                        ActualQuantity = workProgress.ActualQuantity,
                        ActualAmount = workProgress.ActualAmount,
                        PlannedQuantity = workProgress.PlannedQuantity,
                        PlannedAmount = workProgress.PlannedAmount,

                        PreviousActualQuantity = workProgress.PreviousActualQuantity,
                        PreviousActualAmount = workProgress.PreviousActualAmount,
                        PreviousPlannedQuantity = workProgress.PreviousPlannedQuantity,
                        PreviousPlannedAmount = workProgress.PreviousPlannedAmount,

                        TodateActualQuantity = workProgress.TodateActualQuantity,
                        TodateActualAmount = workProgress.TodateActualAmount,
                        TodatePlannedQuantity = workProgress.TodatePlannedQuantity,
                        TodatePlannedAmount = workProgress.TodatePlannedAmount,
                    };
                    list.Add(record);
                }
            }
            
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ResourceUtilizationWorkItem_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationWorkItem workItem, int dayId)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (PeriodHasUtilizationData(workItem))
        //        {
        //            ModelState.AddModelError("", "This work item has resource utilizatin data for this period");
        //        }
        //        else
        //        {
        //            var entity = new ResourceUtilizationWorkItem
        //            {
        //                //ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
        //                MethodOfWorkId = workItem.MethodOfWorkId,
        //                WorkItemPerProjectId = workItem.WorkItemPerProjectId,
        //                ResourceUtilizationDayId = dayId,
        //                ItemName = workItem.ItemName,
        //                IsSubCategory = workItem.IsSubCategory,
        //                ParentId = workItem.ParentId,
        //                UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(workItem.WorkItemPerProjectId).Abbreviation,
        //                ExecutedWork = workItem.ExecutedWork,
        //                WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
        //                OutPutPerDay = workItem.OutPutPerDay,
        //                DirectContractUnitPrice = this.GetWorkItem(workItem.WorkItemPerProjectId).TotalUnitCost,
        //                Income = workItem.Income,
        //                TotalDirectCost = workItem.TotalDirectCost,
        //                MaterialCost = this.GetMaterialCost(workItem.ResourceUtilizationWorkItemId),
        //                EquipmentCost = this.GetEquipmentCost(workItem.ResourceUtilizationWorkItemId),
        //                LaborCost = this.GetLaborCost(workItem.ResourceUtilizationWorkItemId),
        //                TotalCost = workItem.TotalCost,
        //                ProfitCost = workItem.ProfitCost,
        //                OverheadCost = workItem.OverheadCost,


        //                SubProjectTypeName = workItem.SubProjectTypeName,
        //                BlockName = workItem.BlockName,
        //                StructureyType = workItem.StructureyType,
        //                WorkCategory = workItem.WorkCategory,

        //                OrgBranchName = workItem.OrgBranchName,
        //                DateCreated = DateTime.Now,
        //                LastModified = workItem.LastModified,
        //                CreatedBy = User.Identity.Name,
        //                ModifiedBy = workItem.ModifiedBy
        //            };

        //            entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
        //            entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
        //            entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;
        //            entity.TotalCost = entity.TotalDirectCost + entity.ProfitCost + entity.OverheadCost;

        //            db.ResourceUtilizationWorkItem.Add(entity);
        //            db.SaveChanges();

        //            workItem.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;
        //            workItem.WorkCategory = entity.WorkCategory;
        //            workItem.UnitOfMeasurnment = entity.UnitOfMeasurnment;
        //            workItem.Income = entity.Income;
        //            workItem.DirectContractUnitPrice = entity.DirectContractUnitPrice;
        //            workItem.OutPutPerDay = entity.OutPutPerDay;
        //            workItem.TotalDirectCost = entity.TotalDirectCost;

        //            workItem.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;

        //        }

        //    }

        //    return Json(new[] { workItem }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceUtilizationWorkItem_Update([DataSourceRequest]DataSourceRequest request, DetailedWorkProgressViewModel workProgress)
        {
            if (ModelState.IsValid)
            {
                var entityFromContext = db.DetailedWorkProgress.Find(workProgress.Id);

                entityFromContext.ActualQuantity = workProgress.ActualQuantity;
                entityFromContext.ActualAmount = workProgress.ActualAmount;
                entityFromContext.PlannedQuantity = workProgress.PlannedQuantity;
                entityFromContext.PlannedAmount = workProgress.PlannedAmount;

                entityFromContext.PreviousActualQuantity = workProgress.PreviousActualQuantity;
                entityFromContext.PreviousActualAmount = workProgress.PreviousActualAmount;
                entityFromContext.PreviousPlannedQuantity = workProgress.PreviousPlannedQuantity;
                entityFromContext.PreviousPlannedAmount = workProgress.PreviousPlannedAmount;

                entityFromContext.TodateActualQuantity = workProgress.TodateActualQuantity;
                entityFromContext.TodateActualAmount = workProgress.TodateActualAmount;
                entityFromContext.TodatePlannedQuantity = workProgress.TodatePlannedQuantity;
                entityFromContext.TodatePlannedAmount = workProgress.TodatePlannedAmount;


                #region update cualculated fileds
                entityFromContext.TodatePlannedQuantity = workProgress.PreviousPlannedQuantity + workProgress.PlannedQuantity;

                entityFromContext.TodateActualQuantity = workProgress.PreviousActualQuantity + workProgress.ActualQuantity;

                entityFromContext.PreviousPlannedAmount = entityFromContext.PreviousPlannedQuantity * workProgress.UnitPrice;
                entityFromContext.PreviousActualAmount = entityFromContext.PreviousActualQuantity * workProgress.UnitPrice;

                entityFromContext.PlannedAmount = entityFromContext.PlannedQuantity * workProgress.UnitPrice;
                entityFromContext.ActualAmount = entityFromContext.ActualQuantity * workProgress.UnitPrice;

                entityFromContext.TodatePlannedAmount = entityFromContext.TodatePlannedQuantity * workProgress.UnitPrice;
                entityFromContext.TodateActualAmount = entityFromContext.TodateActualQuantity * workProgress.UnitPrice;
                #endregion


                db.DetailedWorkProgress.Attach(entityFromContext);
                db.Entry(entityFromContext).State = EntityState.Modified;
                db.SaveChanges();

               
            }

            return Json(new[] { workProgress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceUtilizationWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, DetailedWorkProgress workProgress)
        {
            if (ModelState.IsValid)
            {
                var entity = db.DetailedWorkProgress.Find(workProgress.DetailedWorkProgressId);

                db.DetailedWorkProgress.Attach(entity);
                db.DetailedWorkProgress.Remove(entity);
                db.SaveChanges();

                //this.DeleteUtilizationResources(workItem.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { workProgress }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult CopyWorkItemsFromEstimationToResourceUtilization(int dayId)
        {
            var day = db.ResourceUtilizationDays.Find(dayId);
            if(day == null)
            {
                return Json(new { Message = "Day not found!", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            else
            { 
                var workItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == day.ResourceUtilizationPeriod.ProjectInformationId).ToList();
                if (workItems.Any())
                {
                    foreach(WorkItemPerProject workItem in workItems)
                    {
                        var existingItem = db.DetailedWorkProgress.FirstOrDefault(w => w.WorkItemPerProjectId == workItem.WorkItemPerProjectId && w.ResourceUtilizationDayId == dayId);
                        if(existingItem == null)
                        {
                            var record = new DetailedWorkProgress
                            {
                                WorkItemPerProjectId = workItem.WorkItemPerProjectId,

                                ResourceUtilizationDayId = dayId
                            };
                            db.DetailedWorkProgress.Add(record);
                            db.SaveChanges();
                        }
                    }
                }
                return Json(new { Message = "Operation Was Successful!", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public string GetUnitOfMeasurement(int id)
        {
            Core.Domain.Construction.UnitOfMeasurement unit = new Core.Domain.Construction.UnitOfMeasurement();
            var temp = db.UnitOfMeasurement.Find(id);
            if (temp != null)
            {
                unit = temp;
            }
            return unit.Abbreviation;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}