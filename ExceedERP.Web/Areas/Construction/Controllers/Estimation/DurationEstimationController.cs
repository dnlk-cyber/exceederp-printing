using ExceedERP.Core.Domain.Common;
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

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class DurationEstimationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/DurationEstimation
        public ActionResult Index()
        {
            
            var workItems = db.ProjectEstimationWorkItem.Select(es => new { es.ItemName, es.ProjectEstimationWorkItemId}).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x =>new{x.UnitOfMeasurementId,x.Abbreviation}).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode,p.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode}).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            

            //var internalEquipment = db.InternalMachinaryRent.ToList();
            //var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments = 
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;


            return View();
        }
        public ActionResult Project(int p)
        {
            ViewBag.Project = "";
            ViewBag.ProjectId = p;
            var currentProject = db.ProjectInformation.Find(p);
            if (currentProject != null)
            {
                ViewBag.Project = currentProject.Name;
            }


            var workItems = db.ProjectEstimationWorkItem.Select(es => new { es.ItemName, es.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.Name, i.ItemCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();



            //var internalEquipment = db.InternalMachinaryRent.ToList();
            //var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;

            return View();
        }
        
        public ActionResult WorkItem_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.ParentId == 0);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeOrderIndex(workItemPerProject.SubProjectTypeId) + " " + ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork + workItemPerProject.AddedQuantityOfWork - workItemPerProject.OmittedQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    ProductivityOfCrewPerDay = workItemPerProject.ProductivityOfCrewPerDay ?? 0,
                    DurationPerCrew = workItemPerProject.DurationPerCrew ?? 0,
                    NumberOfCrew = workItemPerProject.NumberOfCrew ?? 0,
                    EstimatedDuration = workItemPerProject.EstimatedDuration ?? 0
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult WorkItemUnderWorkItem_Read([DataSourceRequest] DataSourceRequest request, int parentWorkItemId)
        {
            var workItems =
                db.GenericWorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            var workItemsToReturn = new List<GenericWorkItemPerProjectViewModel>();
            foreach (GenericWorkItemPerProject workItemPerProject in workItems)
            {
                var record = new GenericWorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItemPerProject.SubProjectTypeId),
                    StructureyType = workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Name,
                    WorkCategoryId = workItemPerProject.WorkCategoryId,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.GenericWorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,

                    ProductivityOfCrewPerDay = workItemPerProject.ProductivityOfCrewPerDay ?? 0,
                    DurationPerCrew = workItemPerProject.DurationPerCrew ?? 0,
                    NumberOfCrew = workItemPerProject.NumberOfCrew ?? 0,
                    EstimatedDuration = workItemPerProject.EstimatedDuration ?? 0

                };
                workItemsToReturn.Add(record);
            }
            IQueryable<GenericWorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItem_Update([DataSourceRequest] DataSourceRequest request,
            GenericWorkItemPerProjectViewModel workItemModel)
        {
            if (ModelState.IsValid)
            {
                var entity = db.GenericWorkItemPerProject.Find(workItemModel.Id);
                if (entity != null)
                {
                    var netQty = entity.TotalQuantityOfWork + entity.AddedQuantityOfWork - entity.OmittedQuantityOfWork;

                    entity.ProductivityOfCrewPerDay = workItemModel.ProductivityOfCrewPerDay;
                    entity.NumberOfCrew = workItemModel.NumberOfCrew;

                    entity.DurationPerCrew = ProjectEstimationHelper.CalculateDurationPerCrew(netQty, entity.ProductivityOfCrewPerDay ?? 0);
                    entity.EstimatedDuration = ProjectEstimationHelper.CalculateWorkDuration(entity.DurationPerCrew ?? 0, entity.NumberOfCrew ?? 0);


                    db.GenericWorkItemPerProject.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { workItemModel }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult UpdateDurationDataFromEstimation(int targetProjectId)
        {
            var workItems = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == targetProjectId).ToList();
            foreach (GenericWorkItemPerProject workItem in workItems)
            {
                if (workItem.IsSubCategory)
                {
                    workItem.ProductivityOfCrewPerDay = workItem.ProductivityOfCrewPerDay == null ? 0 : workItem.ProductivityOfCrewPerDay;
                    workItem.DurationPerCrew = workItem.DurationPerCrew == null ? 0 : workItem.DurationPerCrew;
                    workItem.NumberOfCrew = workItem.NumberOfCrew == null ? 0 : workItem.NumberOfCrew;
                    workItem.EstimatedDuration = workItem.EstimatedDuration == null ? 0 : workItem.EstimatedDuration;
                }
                else
                {
                    var netQty = workItem.TotalQuantityOfWork + workItem.AddedQuantityOfWork - workItem.OmittedQuantityOfWork;

                    workItem.ProductivityOfCrewPerDay = workItem.ProductivityOfCrewPerDay == 0 || workItem.ProductivityOfCrewPerDay == null ? workItem.LabourHourlyOutPut * 8 : workItem.ProductivityOfCrewPerDay;
                    workItem.NumberOfCrew = workItem.NumberOfCrew == 0 || workItem.NumberOfCrew == null ? 1 : workItem.NumberOfCrew;

                    workItem.DurationPerCrew = ProjectEstimationHelper.CalculateDurationPerCrew(netQty, workItem.ProductivityOfCrewPerDay ?? 0);
                    workItem.EstimatedDuration = ProjectEstimationHelper.CalculateWorkDuration(workItem.DurationPerCrew ?? 0, workItem.NumberOfCrew ?? 0);
                }
                db.GenericWorkItemPerProject.Attach(workItem);
                db.Entry(workItem).State = EntityState.Modified;
                db.SaveChanges();
            }
            var formatedData = new
            {
                Message = "Operation Completed Successfully!",
                Status = "Ok"
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CopyEstimationDataToSchedule(int targetProjectId)
        {
            var projectWorkItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == targetProjectId).ToList();

            var project = db.ProjectInformation.Find(targetProjectId);
            if (project == null)
            {
                var d = new
                {
                    Message = "Project not found!",
                    Status = "Error"
                };
                return Json(d, JsonRequestBehavior.AllowGet);
            }
            else if (project != null && !projectWorkItems.Any())
            {

                // something is wrong with this .....
                //ProjectEstimationHelper.SplitProjectAndAddWorkItemsToSpecificBlocks(targetProjectId);
                //new ProjectEstimationSplitController().SplitProjectAndAddWorkItemsToSpecificBlocks(targetProjectId);
                var dt = new
                {
                    Message = project.ProjectType == AvailableProjectTypes.Building ? "Sync Estimation data to all Blocks before u proceed to schedule!" : "Split Estimation data",
                    Status = "Error"
                };
                return Json(dt, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var mainContract = ContractMgmtHelper.GetProjectMainContractIfExists(targetProjectId);
                if (mainContract == null)
                {
                    var formattedData = new
                    {
                        Message = "Contract information is not set for this project!",
                        Status = "Error"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var projectMobilizationDate = mainContract.CommencementDate;

                    #region Add schedule version 1.0, if its the first time
                    var firstVersionSchedule = db.ScheduleRevisions.FirstOrDefault(r => r.ProjectInformationId == targetProjectId);
                    if (firstVersionSchedule == null)
                    {
                        // add  shedule version 1.0
                        var firstVersion = new ScheduleRevision
                        {
                            ProjectInformationId = project.ProjectInformationId,
                            Version = 1,
                            ScheduleStatus = ProjectScheduleStatus.Editing,
                            Status = OperationStatuses.New.ToString(),
                            OrgBranchName = null,
                            DateOfApproval = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            DateCreated = DateTime.Now,
                            LastModified = null,

                        };
                        db.ScheduleRevisions.Add(firstVersion);
                        db.SaveChanges();
                    }
                    #endregion

                    #region sub-category work items
                    var allSubCategoryWorkItems = projectWorkItems
                        .Where(w => w.ProjectInformationId == project.ProjectInformationId &&
                        w.IsSubCategory).ToList();


                    if (allSubCategoryWorkItems.Any())
                    {
                        foreach (WorkItemPerProject workItem in allSubCategoryWorkItems)
                        {
                            //
                            //  check if the work item is added to schedule
                            //var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemPerProjectId == workItem.WorkItemPerProjectId).FirstOrDefault();
                            //if (existingWorkItemInSchedule == null)
                            //{
                            //    var entity = this.AddWorkItemToSchedule(workItem, projectMobilizationDate, null);
                            //}
                        }
                    }
                    #endregion

                    #region work duration of non-sub category

                    var allNonSubCategoryWorkItems = projectWorkItems
                        .Where(w => !(w.IsSubCategory)).ToList();

                    if (allNonSubCategoryWorkItems.Any())
                    {
                        foreach (WorkItemPerProject workItem in allNonSubCategoryWorkItems)
                        {
                            ////
                            ////  check if the work item is added to schedule
                            //var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemPerProjectId == workItem.WorkItemPerProjectId).FirstOrDefault();
                            //if (existingWorkItemInSchedule == null)
                            //{
                            //    this.AddWorkItemToSchedule(workItem, projectMobilizationDate, null);
                            //}
                        }
                    }
                    #endregion

                    var formatedData = new
                    {
                        Message = "Estimation Data Copied to Schedule Successfully!",
                        Status = "Ok"
                    };
                    return Json(formatedData, JsonRequestBehavior.AllowGet);
                }

            }


        }

        private Task AddWorkItemToSchedule(WorkItemPerProject workItem, DateTime projectMobilizationDate, int? parentId)
        {
            var id = SchedulingHelper.GetProjectSpecificId(workItem.ProjectInformationId);

            var duration = workItem.IsSubCategory ? 0 : this.GetEstimatedDurationOfWorkItem(workItem);
            var cualculatedEndDate = workItem.IsSubCategory ? projectMobilizationDate : BusinessDaysCalculator.GetCalculatedEndDate(projectMobilizationDate, false, duration, workItem.ProjectInformationId);


            var taskToInsert = new TaskViewModel
            {

                Start = projectMobilizationDate,
                End = cualculatedEndDate,

                ParentID = parentId,
                Title = workItem.ItemName,
                PercentComplete = 0,

                Summary = workItem.IsSubCategory ? true : false,
                Expanded = true,

                //WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                WorkCategoryId = workItem.WorkCategoryId,
                ConfigurationId = workItem.WorkCategory.ConfigurationId,
                BlockId = workItem.BlockId ?? 0,
                SubProjectTypeId = ProjectEstimationHelper.GetWorkItemSubProjectType(workItem).SubProjectTypeId,
                ProjectInformationId = workItem.ProjectInformationId,

                SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString(),
                WorkCategory = workItem.WorkCategory.Name,

                EstimatedDuration = duration,
                TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(workItem.WorkItemPerProjectId),
                DirectCost = workItem.DirectCost,
                MaterialCost = workItem.MaterialCostPerUnit,
                EquipmentCost = workItem.EquipmentCostPerUnit,
                LabourCost = workItem.LaborCostPerUnit,
                OrderId = this.SetWorkItemOrderInSchedule(workItem),
                Version = 1,

                ActualDuration = duration,
                ProjectSpecificId = id,
            };

            var entity = taskToInsert.ToEntity();
            db.Tasks.Add(entity);
            db.SaveChanges();
            SchedulingHelper.PrepareScheduleBreakDown(entity.ID);


            if (workItem.IsSubCategory)
            {
                List<Task> childTasks = new List<Task>();

                var childWorkItems = db.WorkItemPerProject.Where(w => w.ParentId == workItem.WorkItemPerProjectId && w.ProjectInformationId == workItem.ProjectInformationId).ToList();
                foreach (WorkItemPerProject childWorkItem in childWorkItems)
                {
                    var addedChildTask = this.AddWorkItemToSchedule(childWorkItem, projectMobilizationDate, entity.ID);
                    if (addedChildTask != null) {
                        SchedulingHelper.PrepareScheduleBreakDown(addedChildTask.ID);
                    }
                    childTasks.Add(addedChildTask);
                }
                if (childTasks.Any())
                {
                    entity.EstimatedDuration = childTasks.Max(t => t.EstimatedDuration);
                    entity.End = childTasks.Max(t => t.End);
                    entity.ActualDuration = entity.EstimatedDuration;
                    entity.BudgetCost = childTasks.Sum(c => c.BudgetCost);
                    //entity.TotalQuantityOfWork = listOfChildTasks.Sum(c => c.TotalQuantityOfWork);

                    db.Tasks.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    SchedulingHelper.PrepareScheduleBreakDown(entity.ID);
                    childTasks.Clear();
                }
            }
            return entity;
        }
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }

        private decimal GetEstimatedDurationOfWorkItem(WorkItemPerProject record)
        {
            var duration = new decimal();
            var rootWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(wi => wi.GenericWorkItemPerProjectId == record.GenericWorkItemPerProjectId);

            if (rootWorkItem != null)
            {

                duration = rootWorkItem.EstimatedDuration ?? 0;
            }
            return duration;
        }
        private int SetWorkItemOrderInSchedule(WorkItemPerProject model)
        {
            int order = 0;
            var currentProject = db.ProjectInformation.Find(model.ProjectInformationId);
            if (currentProject != null)
            {
                //var availableTasks = db.Tasks.Where(t => t.WorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId);
                //if (availableTasks.Any())
                //{
                //    order = availableTasks.Max(task => task.OrderID) + 1;
                //}
            }
            return order;
        }
    }
}