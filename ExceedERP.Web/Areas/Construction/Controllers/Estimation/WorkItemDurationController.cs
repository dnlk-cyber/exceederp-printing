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
    public class WorkItemDurationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/WorkItemDuration
        public ActionResult Index()
        {
            //var availableTakeOff = (from t in db.TakeOffPerProject
            //                        join wi in db.WorkItemPerProject
            //                        on t.WorkItemPerProjectId equals wi.WorkItemPerProjectId
            //                        //where wi.ProjectInformationId == currentProjectIdR
            //                        select new
            //                        {
            //                            ID = t.TakeOffPerProjectId
            //                        }).ToList();
            //var ids = new HashSet<int>(availableTakeOff.Select(ts => ts.ID));

            //var takeoffSheets = db.TakeOffPerProject.Where(takeoff => (ids.Contains(takeoff.TakeOffPerProjectId)));
            //ViewData["TakeOffs"] = takeoffSheets;


            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name, i.ItemCategoryCode }).ToList();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });


            //this.CopyDataFromWorkItemToDurationEstimation();
            return View();
        }
        public ActionResult Project(int p)
        {
            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;

            ViewBag.Project = "";
            ViewBag.ProjectId = p;
            var currentProject = db.ProjectInformation.Find(p);
            if (currentProject != null)
            {
                ViewBag.Project = currentProject.Name;
            }

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(pp => new { pp.ItemCategoryCode, pp.Name }).ToList();
            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name, i.ItemCategoryCode }).ToList();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });



            return View();
        }
        public ActionResult WorkItemDuration_Read([DataSourceRequest]DataSourceRequest request, int currentProjectId)
        {
            // find all the work-item-durations under a given project
            //var wdUnderTheProject = (from wd in db.WorkItemDuration
            //                         join wi in db.WorkItemPerProject
            //                         on wd.WorkItemId equals wi.WorkItemPerProjectId
            //                         where wi.ProjectInformationId == currentProjectIdR
            //                         select new
            //                         {
            //                             ID = wd.WorkItemDurationId
            //                         }).ToList();
            //var ids = new HashSet<int>(wdUnderTheProject.Select(wd => wd.ID));

            //IQueryable<WorkItemDuration> workItemDurations = db.WorkItemDuration.Where(wd => (ids.Contains(wd.WorkItemDurationId)));
            IQueryable<WorkItemDuration> workItemDurations = db.WorkItemDuration.Where(wd => wd.ProjectInformationId == currentProjectId && wd.Parent == 0);
            DataSourceResult result = workItemDurations.ToDataSourceResult(request, workDuration => new
            {
                WorkItemDurationId = workDuration.WorkItemDurationId,
                WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                ItemCode = workDuration.ItemCode,
                WorkDescription = workDuration.WorkDescription,
                Unit = workDuration.Unit,
                HasSubWorkItems = workDuration.HasSubWorkItems,
                Quantity = workDuration.Quantity,
                ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                DurationPerCrew = workDuration.DurationPerCrew,
                NumberOfCrew = workDuration.NumberOfCrew,
                EstimatedDuration = workDuration.EstimatedDuration,

                Parent = workDuration.Parent,
                Source = workDuration.Source,
                ProjectInformationId = workDuration.ProjectInformationId,

                BudgetCost = workDuration.BudgetCost,
                MaterialCost = workDuration.MaterialCost,
                EquipmentCost = workDuration.EquipmentCost,
                LabourCost = workDuration.LabourCost,
                DirectCost = workDuration.DirectCost,
                ProfitCost = workDuration.ProfitCost,
                OverHeadCost = workDuration.OverHeadCost,
                TotalUnitRate = workDuration.TotalUnitRate,

                WorkCategoryId = workDuration.WorkCategoryId,
                ConfigurationId = workDuration.ConfigurationId,
                BlockId = workDuration.BlockId,
                SubProjectTypeId = workDuration.SubProjectTypeId,

                SubProjectTypeName = workDuration.SubProjectTypeName,
                BlockName = workDuration.BlockName,
                StructureyType = workDuration.StructureyType,
                WorkCategory = workDuration.WorkCategory,
            });

            return Json(result);
        }

        public ActionResult WorkItemDuration_Child_Read([DataSourceRequest]DataSourceRequest request, int parentId)
        {
            IQueryable<WorkItemDuration> workItemDurations = db.WorkItemDuration.Where(wd => wd.Parent == parentId);
            DataSourceResult result = workItemDurations.ToDataSourceResult(request, workDuration => new
            {
                WorkItemDurationId = workDuration.WorkItemDurationId,
                ProjectInformationId = workDuration.ProjectInformationId,
                WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                ItemCode = workDuration.ItemCode,
                WorkDescription = workDuration.WorkDescription,
                Unit = workDuration.Unit,
                Parent = workDuration.Parent,
                HasSubWorkItems = workDuration.HasSubWorkItems,
                Quantity = workDuration.Quantity,
                ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                DurationPerCrew = workDuration.DurationPerCrew,
                NumberOfCrew = workDuration.NumberOfCrew,
                EstimatedDuration = workDuration.EstimatedDuration,

                Source = workDuration.Source,

                BudgetCost = workDuration.BudgetCost,
                MaterialCost = workDuration.MaterialCost,
                EquipmentCost = workDuration.EquipmentCost,
                LabourCost = workDuration.LabourCost,
                DirectCost = workDuration.DirectCost,
                ProfitCost = workDuration.ProfitCost,
                OverHeadCost = workDuration.OverHeadCost,
                TotalUnitRate = workDuration.TotalUnitRate,

                WorkCategoryId = workDuration.WorkCategoryId,
                ConfigurationId = workDuration.ConfigurationId,
                BlockId = workDuration.BlockId,
                SubProjectTypeId = workDuration.SubProjectTypeId,

                SubProjectTypeName = workDuration.SubProjectTypeName,
                BlockName = workDuration.BlockName,
                StructureyType = workDuration.StructureyType,
                WorkCategory = workDuration.WorkCategory,
            });

            return Json(result);
        }
        public ActionResult WorkItemToCopyToDuration_Read([DataSourceRequest]DataSourceRequest request, int projectId = 0, int blkTypeId = 0, int blockId = 0, int configId = 0, int catId = 0)
        {
            List<CopyEstimationDataToDurationViewModel> list = new List<CopyEstimationDataToDurationViewModel>();
            var items = db.WorkItemPerProject.Where(w => w.ProjectInformationId == projectId && w.ParentId == 0).ToList();


            if (blkTypeId > 0)
            {
                var blockIds = new HashSet<int>(db.Blocks.Where(b => b.SubProjectTypeId == blkTypeId && b.ProjectInformationId == projectId).ToList().Select(bl => bl.BlockId));
                items = items.Where(i => blockIds.Contains(i.BlockId.Value)).ToList();

                if (blockId > 0)
                {
                    items = items.Where(i => i.BlockId == blockId).ToList();

                    if (configId > 0)
                    {
                        items = items.Where(i => i.WorkCategory.ConfigurationId == configId).ToList();

                        if (catId > 0)
                        {
                            items = items.Where(i => i.WorkCategoryId == catId).ToList();
                        }
                    }
                }
            }
            foreach (WorkItemPerProject workItem in items)
            {
                var record = new CopyEstimationDataToDurationViewModel
                {
                    Id = workItem.WorkItemPerProjectId,
                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                    BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                    StructureType = ConsSettingHelper.GetStructureTypeCode(workItem.WorkCategory.ConfigurationId, workItem.ProjectType) + " " + workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItem.WorkCategory.Index + " " + workItem.WorkCategory.Name,

                    ItemName = workItem.ItemName,
                    TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                    HasSubItems = workItem.IsSubCategory,
                    TotalUnitRate = workItem.TotalUnitCost,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                };
                var existingItemInDuration = db.WorkItemDuration.FirstOrDefault(w => w.ProjectInformationId == projectId && w.Source == DurationItemSource.Estimation && w.WorkItemPerProjectId == workItem.WorkItemPerProjectId);
                if (existingItemInDuration != null)
                {
                    record.IsCopied = true;
                }
                list.Add(record);
            }

            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemDuration_Create([DataSourceRequest]DataSourceRequest request, WorkItemDuration workDuration, int currentProjectId)
        {
            if (ModelState.IsValid)
            {

                var entity = new WorkItemDuration
                {
                    //WorkItemDurationId = workDuration.WorkItemDurationId,
                    WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                    WorkDescription = workDuration.WorkDescription,
                    //WorkDescription = GetConcatnatedWorkDescription(workDuration.WorkItemPerProjectId),
                    //ItemCode = GetCurrentWorkItem(workDuration.GenericWorkItemPerProjectId).ItemCode,
                    //Unit = GetUnitOfMeasurement(GetCurrentWorkItem(workDuration.GenericWorkItemPerProjectId).UnitOfMeasurementId ?? 0),
                    ItemCode = workDuration.ItemCode,
                    Unit = workDuration.Unit,
                    HasSubWorkItems = workDuration.HasSubWorkItems,
                    ProjectInformationId = currentProjectId,
                    //Quantity = GetWorkItemQuantityOfWork(workDuration.GenericWorkItemPerProjectId),
                    Quantity = workDuration.Quantity,
                    ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                    NumberOfCrew = workDuration.NumberOfCrew,
                    Source = DurationItemSource.Manual,

                    DurationPerCrew = workDuration.DurationPerCrew,
                    EstimatedDuration = workDuration.EstimatedDuration,

                    SubProjectTypeName = "",
                    BlockName = "",
                    StructureyType = "",
                    WorkCategory = "",

                };
                entity.DurationPerCrew = CalculateDurationPerCrew(entity.Quantity, entity.ProductivityOfCrewPerDay);
                entity.EstimatedDuration = entity.EstimatedDuration != 0 ? entity.EstimatedDuration : CalculateWorkDuration(entity.DurationPerCrew, entity.NumberOfCrew);

                db.WorkItemDuration.Add(entity);
                db.SaveChanges();

                workDuration.WorkItemDurationId = entity.WorkItemDurationId;
                workDuration.WorkDescription = entity.WorkDescription;
                workDuration.Unit = entity.Unit;
                workDuration.Quantity = entity.Quantity;
                workDuration.DurationPerCrew = entity.DurationPerCrew;
                workDuration.EstimatedDuration = entity.EstimatedDuration;

                //this.AddWorkItemToSchedule(entity);
                //}
            }

            return Json(new[] { workDuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemDuration_Child_Create([DataSourceRequest]DataSourceRequest request, WorkItemDuration workDuration, int parentId)
        {
            if (ModelState.IsValid)
            {

                var entity = new WorkItemDuration
                {
                    //WorkItemDurationId = workDuration.WorkItemDurationId,
                    WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                    WorkDescription = workDuration.WorkDescription,
                    //WorkDescription = GetConcatnatedWorkDescription(workDuration.WorkItemPerProjectId),
                    //ItemCode = GetCurrentWorkItem(workDuration.GenericWorkItemPerProjectId).ItemCode,
                    //Unit = GetUnitOfMeasurement(GetCurrentWorkItem(workDuration.GenericWorkItemPerProjectId).UnitOfMeasurementId ?? 0),
                    ItemCode = workDuration.ItemCode,
                    Unit = workDuration.Unit,
                    HasSubWorkItems = workDuration.HasSubWorkItems,
                    ProjectInformationId = GetProjectIdOfDurationItem(parentId),
                    Parent = parentId,
                    //Quantity = GetWorkItemQuantityOfWork(workDuration.GenericWorkItemPerProjectId),
                    Quantity = workDuration.Quantity,
                    ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                    NumberOfCrew = workDuration.NumberOfCrew,
                    Source = DurationItemSource.Manual,

                    DurationPerCrew = workDuration.DurationPerCrew,
                    EstimatedDuration = workDuration.EstimatedDuration,

                    SubProjectTypeName = "",
                    BlockName = "",
                    StructureyType = "",
                    WorkCategory = "",

                };
                entity.DurationPerCrew = CalculateDurationPerCrew(entity.Quantity, entity.ProductivityOfCrewPerDay);
                entity.EstimatedDuration = entity.EstimatedDuration != 0 ? entity.EstimatedDuration : CalculateWorkDuration(entity.DurationPerCrew, entity.NumberOfCrew);

                db.WorkItemDuration.Add(entity);
                db.SaveChanges();

                workDuration.WorkItemDurationId = entity.WorkItemDurationId;
                workDuration.WorkDescription = entity.WorkDescription;
                workDuration.Unit = entity.Unit;
                workDuration.Quantity = entity.Quantity;
                workDuration.DurationPerCrew = entity.DurationPerCrew;
                workDuration.EstimatedDuration = entity.EstimatedDuration;

                //this.AddWorkItemToSchedule(entity);
                //}
            }

            return Json(new[] { workDuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemDuration_Update([DataSourceRequest]DataSourceRequest request, WorkItemDuration workDuration)
        {
            if (ModelState.IsValid)
            {
                var entityFromcontext = db.WorkItemDuration.Find(workDuration.WorkItemDurationId);
                if (entityFromcontext != null)
                {
                    //entityFromcontext.WorkItemDurationId = workDuration.WorkItemDurationId;
                    //entityFromcontext.WorkItemPerProjectId = workDuration.WorkItemPerProjectId;
                    entityFromcontext.ItemCode = workDuration.ItemCode;
                    entityFromcontext.WorkDescription = workDuration.WorkDescription;
                    entityFromcontext.Unit = workDuration.Unit;
                    entityFromcontext.HasSubWorkItems = workDuration.HasSubWorkItems;
                    entityFromcontext.Quantity = workDuration.Quantity;
                    entityFromcontext.ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay;
                    entityFromcontext.DurationPerCrew = workDuration.DurationPerCrew;
                    entityFromcontext.NumberOfCrew = workDuration.NumberOfCrew;
                    entityFromcontext.EstimatedDuration = workDuration.EstimatedDuration;
                    entityFromcontext.Parent = workDuration.Parent;
                    entityFromcontext.Source = workDuration.Source;

                    //entityFromcontext.ProjectInformationId = workDuration.ProjectInformationId;
                    //entityFromcontext.SubProjectTypeName = workDuration.SubProjectTypeName;
                    //entityFromcontext.BlockName = workDuration.BlockName;
                    //entityFromcontext.StructureyType = workDuration.StructureyType;
                    //entityFromcontext.WorkCategory = workDuration.WorkCategory;

                    entityFromcontext.DurationPerCrew = entityFromcontext.Source == DurationItemSource.Manual ? entityFromcontext.DurationPerCrew : CalculateDurationPerCrew(entityFromcontext.Quantity, entityFromcontext.ProductivityOfCrewPerDay);
                    entityFromcontext.EstimatedDuration =
                        workDuration.Source == DurationItemSource.Manual ?
                        workDuration.EstimatedDuration :
                        CalculateWorkDuration(entityFromcontext.DurationPerCrew, entityFromcontext.NumberOfCrew);

                    db.WorkItemDuration.Attach(entityFromcontext);
                    db.Entry(entityFromcontext).State = EntityState.Modified;
                    db.SaveChanges();

                    workDuration.WorkItemDurationId = entityFromcontext.WorkItemDurationId;
                    workDuration.WorkDescription = entityFromcontext.WorkDescription;
                    workDuration.Unit = entityFromcontext.Unit;
                    workDuration.Quantity = entityFromcontext.Quantity;
                    workDuration.DurationPerCrew = entityFromcontext.DurationPerCrew;
                    workDuration.EstimatedDuration = entityFromcontext.EstimatedDuration;
                }
                //var entity = new WorkItemDuration
                //{
                //    WorkItemDurationId = workDuration.WorkItemDurationId,
                //    WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                //    ItemCode = workDuration.ItemCode,
                //    WorkDescription = workDuration.WorkDescription,
                //    Unit = workDuration.Unit,
                //    HasSubWorkItems = workDuration.HasSubWorkItems,
                //    Quantity = workDuration.Quantity,
                //    ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                //    DurationPerCrew = workDuration.DurationPerCrew,
                //    NumberOfCrew = workDuration.NumberOfCrew,
                //    EstimatedDuration = workDuration.EstimatedDuration,

                //    Parent = workDuration.Parent,
                //    Source = workDuration.Source,
                //    ProjectInformationId = workDuration.ProjectInformationId,

                //    SubProjectTypeName = workDuration.SubProjectTypeName,
                //    BlockName = workDuration.BlockName,
                //    StructureyType = workDuration.StructureyType,
                //    WorkCategory = workDuration.WorkCategory,
                //};
                //entity.DurationPerCrew = entity.Source == DurationItemSource.Manual ? entity.DurationPerCrew : CalculateDurationPerCrew(entity.Quantity, entity.ProductivityOfCrewPerDay);
                //entity.EstimatedDuration =
                //    workDuration.Source == DurationItemSource.Manual ?
                //    workDuration.EstimatedDuration :
                //    CalculateWorkDuration(entity.DurationPerCrew, entity.NumberOfCrew);

                //db.WorkItemDuration.Attach(entity);
                //db.Entry(entity).State = EntityState.Modified;
                //db.SaveChanges();

                //workDuration.WorkItemDurationId = entity.WorkItemDurationId;
                //workDuration.WorkDescription = entity.WorkDescription;
                //workDuration.Unit = entity.Unit;
                //workDuration.Quantity = entity.Quantity;
                //workDuration.DurationPerCrew = entity.DurationPerCrew;
                //workDuration.EstimatedDuration = entity.EstimatedDuration;

                SchedulingHelper.UpdateResourcesOnDurationItemUpdate(entityFromcontext);
            }

            return Json(new[] { workDuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkItemDuration_Destroy([DataSourceRequest]DataSourceRequest request, WorkItemDuration workDuration)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkItemDuration
                {
                    WorkItemDurationId = workDuration.WorkItemDurationId,
                    WorkItemPerProjectId = workDuration.WorkItemPerProjectId,
                    ItemCode = workDuration.ItemCode,
                    WorkDescription = workDuration.WorkDescription,
                    Unit = workDuration.Unit,
                    HasSubWorkItems = workDuration.HasSubWorkItems,
                    Quantity = workDuration.Quantity,
                    ProductivityOfCrewPerDay = workDuration.ProductivityOfCrewPerDay,
                    DurationPerCrew = workDuration.DurationPerCrew,
                    NumberOfCrew = workDuration.NumberOfCrew,
                    EstimatedDuration = workDuration.EstimatedDuration,

                    Parent = workDuration.Parent,
                    Source = workDuration.Source,
                    ProjectInformationId = workDuration.ProjectInformationId,

                    BudgetCost = workDuration.BudgetCost,
                    MaterialCost = workDuration.MaterialCost,
                    EquipmentCost = workDuration.EquipmentCost,
                    LabourCost = workDuration.LabourCost,
                    DirectCost = workDuration.DirectCost,
                    ProfitCost = workDuration.ProfitCost,
                    OverHeadCost = workDuration.OverHeadCost,
                    TotalUnitRate = workDuration.TotalUnitRate,

                    WorkCategoryId = workDuration.WorkCategoryId,
                    ConfigurationId = workDuration.ConfigurationId,
                    BlockId = workDuration.BlockId,
                    SubProjectTypeId = workDuration.SubProjectTypeId,

                    SubProjectTypeName = workDuration.SubProjectTypeName,
                    BlockName = workDuration.BlockName,
                    StructureyType = workDuration.StructureyType,
                    WorkCategory = workDuration.WorkCategory,
                };

                db.WorkItemDuration.Attach(entity);
                db.WorkItemDuration.Remove(entity);
                db.SaveChanges();

                if (entity.HasSubWorkItems)
                {
                    this.DeleteSubItemsOnItemDelete(entity);
                }
            }

            return Json(new[] { workDuration }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projects = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION && ids.Contains(x.ProjectInformationId));
            DataSourceResult result = projects.ToDataSourceResult(request, projectInformation => new
            {
                ProjectType = projectInformation.ProjectType,
                ProjectInformationId = projectInformation.ProjectInformationId,
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


        #region  methods
        private void DeleteSubItemsOnItemDelete(WorkItemDuration deletedItem)
        {
            var subItems = db.WorkItemDuration.Where(w => w.Parent == deletedItem.WorkItemDurationId).ToList();
            if (subItems.Any())
            {
                foreach (WorkItemDuration childItem in subItems)
                {
                    db.WorkItemDuration.Remove(childItem);
                    db.SaveChanges();

                    if (childItem.HasSubWorkItems)
                    {
                        DeleteSubItemsOnItemDelete(childItem);
                    }
                }
            }

        }
        //Summary:
        //      Copy Estimation data to project schedule
        public JsonResult CopyEstimationDataToSchedule(int targetProjectId)
        {
            var projectWorkItems = db.WorkItemDuration.Where(w => w.ProjectInformationId == targetProjectId).ToList();
            if (!projectWorkItems.Any())
            {
                //ProjectEstimationSplitController
            }
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
                            CreatedBy = null,
                            ModifiedBy = null,
                            DateCreated = null,
                            LastModified = null,

                        };
                        db.ScheduleRevisions.Add(firstVersion);
                        db.SaveChanges();
                    }
                    #endregion

                    #region sub-category work items
                    //var allSubCategoryWorkItems = db.WorkItemPerProject
                    //    .Where(w => w.ProjectInformationId == project.ProjectInformationId &&
                    //    w.IsSubCategory).ToList();

                    var allSubCategoryWorkItems = db.WorkItemDuration
                        .Where(wd => wd.ProjectInformationId == targetProjectId &&
                        wd.HasSubWorkItems
                        ).ToList();
                    if (allSubCategoryWorkItems.Any())
                    {
                        foreach (WorkItemDuration workItem in allSubCategoryWorkItems)
                        {
                            AddSelectedDurationItemToSchedule(project, workItem, projectMobilizationDate, null);
                        }
                    }
                    #endregion

                    #region work duration of non-sub category

                    var allNonSubCategoryWorkItems = db.WorkItemDuration
                        .Where(w => w.ProjectInformationId == project.ProjectInformationId &&
                        !(w.HasSubWorkItems)).ToList();

                    if (allNonSubCategoryWorkItems.Any())
                    {
                        foreach (WorkItemDuration workItem in allNonSubCategoryWorkItems)
                        {
                            //var idOfThisWorkItemInSchedule = SchedulingHelper.GetProjectSpecificId(targetProjectId);
                            ////
                            ////  check if the work item is added to schedule
                            //var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == workItem.WorkItemDurationId).FirstOrDefault();
                            //if (existingWorkItemInSchedule == null)
                            //{
                            //    //var tskDb = new ExceedDbContext();
                            //    var taskToInsert = new TaskViewModel();
                            //    //var targetWBS = new ProjectWorkBreakDown();
                            //    var duration = workItem.EstimatedDuration;

                            //    var cualculatedEndDate = BusinessDaysCalculator.GetCalculatedEndDate(projectMobilizationDate, false, duration, project.ProjectInformationId);

                            //    taskToInsert.Start = projectMobilizationDate;
                            //    //taskToInsert.End = projectMobilizationDate.AddDays((int)duration);
                            //    taskToInsert.End = cualculatedEndDate;

                            //    taskToInsert.Title = workItem.WorkDescription;
                            //    taskToInsert.PercentComplete = 0;

                            //    taskToInsert.Summary = workItem.HasSubWorkItems ? true : false;
                            //    taskToInsert.Expanded = true;

                            //    taskToInsert.WorkItemDurationId = workItem.WorkItemDurationId;
                            //    taskToInsert.WorkCategoryId = workItem.WorkCategoryId;
                            //    taskToInsert.ConfigurationId = workItem.ConfigurationId;
                            //    taskToInsert.BlockId = workItem.BlockId;
                            //    taskToInsert.SubProjectTypeId = workItem.SubProjectTypeId;

                            //    taskToInsert.SubProjectTypeName = workItem.SubProjectTypeName;
                            //    taskToInsert.BlockName = workItem.BlockName;
                            //    taskToInsert.StructureyType = workItem.StructureyType;
                            //    taskToInsert.WorkCategory = workItem.WorkCategory;

                            //    taskToInsert.EstimatedDuration = duration;
                            //    taskToInsert.TotalQuantityOfWork = workItem.Quantity;
                            //    taskToInsert.BudgetCost = workItem.BudgetCost;
                            //    taskToInsert.DirectCost = workItem.DirectCost;
                            //    taskToInsert.MaterialCost = workItem.MaterialCost;
                            //    taskToInsert.EquipmentCost = workItem.EquipmentCost;
                            //    taskToInsert.LabourCost = workItem.LabourCost;
                            //    taskToInsert.OrderId = this.SetWorkItemOrderInSchedule(workItem);
                            //    taskToInsert.Version = 1;

                            //    taskToInsert.ActualDuration = taskToInsert.EstimatedDuration;
                            //    taskToInsert.ProjectSpecificId = idOfThisWorkItemInSchedule;

                            //    var record = taskToInsert.ToEntity();
                            //    db.Tasks.Add(record);
                            //    db.SaveChanges();

                            //    SchedulingHelper.PrepareScheduleBreakDown(record.ID);
                            //}
                            AddSelectedDurationItemToSchedule(project, workItem, projectMobilizationDate, null);
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

            //if (source == "Project")
            //{
            //    return RedirectToAction("Project", "WorkItemDuration", new { p = targetProjectId });
            //}
            //return RedirectToAction("Index");
        }
        private Task AddSelectedDurationItemToSchedule(ProjectInformation project, WorkItemDuration workItem, DateTime mobilizationDate, int? parentTaskId)
        {
            var idOfCurrentWorkItemInSchedule = SchedulingHelper.GetProjectSpecificId(project.ProjectInformationId);
            // child work items
            var childWorkItems =
                db.WorkItemDuration.Where(w => w.ProjectInformationId == project.ProjectInformationId &&
                                    w.Parent == workItem.WorkItemDurationId
                                    ).ToList();


            //
            //  check if the work item is added to schedule
            var existingWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == workItem.WorkItemDurationId).FirstOrDefault();
            if (existingWorkItemInSchedule == null)
            {
                var taskToInsert = new TaskViewModel();

                var listOfChildTasks = new List<Task>();

                var cualculatedEndDate = workItem.HasSubWorkItems ? mobilizationDate : 
                    BusinessDaysCalculator.GetCalculatedEndDate(mobilizationDate, false, workItem.EstimatedDuration, project.ProjectInformationId);

                taskToInsert.Start = mobilizationDate;
                taskToInsert.End = cualculatedEndDate;

                taskToInsert.Title = workItem.WorkDescription;
                taskToInsert.PercentComplete = 0;

                taskToInsert.ParentID = parentTaskId;
                taskToInsert.Summary = workItem.HasSubWorkItems ? true : false;
                taskToInsert.Expanded = true;

                taskToInsert.WorkItemDurationId = workItem.WorkItemDurationId;
                taskToInsert.WorkCategoryId = workItem.WorkCategoryId;
                taskToInsert.ConfigurationId = workItem.ConfigurationId;
                taskToInsert.BlockId = workItem.BlockId;
                taskToInsert.SubProjectTypeId = workItem.SubProjectTypeId;

                //taskToInsert.SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId);
                //taskToInsert.BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId);
                //taskToInsert.StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString();
                //taskToInsert.WorkCategory = workItem.WorkCategory.Name;
                taskToInsert.SubProjectTypeName = workItem.SubProjectTypeName;
                taskToInsert.BlockName = workItem.BlockName;
                taskToInsert.StructureyType = workItem.StructureyType;
                taskToInsert.WorkCategory = workItem.WorkCategory;

                taskToInsert.EstimatedDuration = workItem.HasSubWorkItems ? 1 : workItem.EstimatedDuration;
                taskToInsert.TotalQuantityOfWork = workItem.Quantity;
                taskToInsert.BudgetCost = workItem.BudgetCost;
                taskToInsert.DirectCost = workItem.DirectCost;
                taskToInsert.MaterialCost = workItem.MaterialCost;
                taskToInsert.EquipmentCost = workItem.EquipmentCost;
                taskToInsert.LabourCost = workItem.LabourCost;
                taskToInsert.OrderId = this.SetWorkItemOrderInSchedule(workItem);
                taskToInsert.Version = 1;

                taskToInsert.ActualDuration = taskToInsert.EstimatedDuration;
                taskToInsert.ProjectSpecificId = idOfCurrentWorkItemInSchedule;

                var entity = taskToInsert.ToEntity();
                //var tempDb = new ExceedDbContext();
                db.Tasks.Add(entity);
                db.SaveChanges();
                taskToInsert.TaskID = entity.ID;
                //}

                #region add child tasks

                if (childWorkItems.Any())
                {
                    //var currentParentTask = db.Tasks.FirstOrDefault(t => t.WorkItemDurationId == workItemDuration.WorkItemDurationId);
                    //if (currentParentTask != null)
                    //{
                    foreach (WorkItemDuration childWI in childWorkItems)
                    {
                        var addedTask = AddSelectedDurationItemToSchedule(project, childWI, mobilizationDate, entity.ID);
                        if(addedTask != null)
                        {
                            listOfChildTasks.Add(addedTask);
                        }

                        //var idOfChildTaskInSchedule = SchedulingHelper.GetProjectSpecificId(project.ProjectInformationId);

                        //var existingChildWorkItemInSchedule = db.Tasks.Where(t => t.WorkItemDurationId == childWI.WorkItemDurationId).FirstOrDefault();
                        //if (existingChildWorkItemInSchedule == null)
                        //{
                        //    var childTaskToAdd = new TaskViewModel();
                        //    //var todaysDate = DateTime.Now;
                        //    //var duration = this.GetEstimatedDurationOfWorkItem(childWI);
                        //    var duration = childWI.EstimatedDuration;
                        //    var cualculatedEndDate = BusinessDaysCalculator.GetCalculatedEndDate(mobilizationDate, false, duration, project.ProjectInformationId);

                        //    childTaskToAdd.ParentID = entity.ID;
                        //    childTaskToAdd.Start = mobilizationDate;
                        //    //childTaskToAdd.End = projectMobilizationDate.AddDays((int)duration);
                        //    childTaskToAdd.End = cualculatedEndDate;

                        //    childTaskToAdd.Title = childWI.WorkDescription;
                        //    childTaskToAdd.PercentComplete = 0;

                        //    childTaskToAdd.Summary = childWI.HasSubWorkItems ? true : false;
                        //    childTaskToAdd.Expanded = true;

                        //    childTaskToAdd.WorkItemDurationId = childWI.WorkItemDurationId;
                        //    childTaskToAdd.WorkCategoryId = childWI.WorkCategoryId;
                        //    childTaskToAdd.ConfigurationId = childWI.ConfigurationId;
                        //    childTaskToAdd.BlockId = childWI.BlockId;
                        //    childTaskToAdd.SubProjectTypeId = taskToInsert.SubProjectTypeId;

                        //    childTaskToAdd.SubProjectTypeName = taskToInsert.SubProjectTypeName;
                        //    childTaskToAdd.BlockName = taskToInsert.BlockName;
                        //    childTaskToAdd.StructureyType = childWI.StructureyType;
                        //    childTaskToAdd.WorkCategory = childWI.WorkCategory;

                        //    childTaskToAdd.EstimatedDuration = duration;
                        //    childTaskToAdd.TotalQuantityOfWork = childWI.Quantity;
                        //    childTaskToAdd.BudgetCost = childWI.BudgetCost;
                        //    childTaskToAdd.DirectCost = childWI.DirectCost;
                        //    childTaskToAdd.MaterialCost = childWI.MaterialCost;
                        //    childTaskToAdd.EquipmentCost = childWI.EquipmentCost;
                        //    childTaskToAdd.LabourCost = childWI.LabourCost;
                        //    childTaskToAdd.OrderId = this.SetWorkItemOrderInSchedule(childWI);
                        //    childTaskToAdd.Version = 1;

                        //    childTaskToAdd.ActualDuration = childTaskToAdd.EstimatedDuration;
                        //    //taskToInsert.OriginalDuration = 1;
                        //    //taskToInsert.RemainingDuration = 1;
                        //    childTaskToAdd.ProjectSpecificId = idOfChildTaskInSchedule;
                        //    var record = childTaskToAdd.ToEntity();
                        //    db.Tasks.Add(record);
                        //    db.SaveChanges();

                        //    listOfChildTasks.Add(record);

                        //    SchedulingHelper.PrepareScheduleBreakDown(record.ID);

                        //}
                    }

                    //}
                }
                #endregion

                if (listOfChildTasks.Any())
                {
                    entity.EstimatedDuration = listOfChildTasks.Max(t => t.EstimatedDuration);
                    //entity.End = entity.Start.AddDays((int)entity.EstimatedDuration);
                    entity.End = listOfChildTasks.Max(t => t.End);
                    entity.ActualDuration = entity.EstimatedDuration;
                    entity.BudgetCost = listOfChildTasks.Sum(c => c.BudgetCost);
                    //entity.TotalQuantityOfWork = listOfChildTasks.Sum(c => c.TotalQuantityOfWork);

                    db.Tasks.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    listOfChildTasks.Clear();
                }
                // summary:
                //  parent work items dont need cost break down.
                //SchedulingHelper.PrepareScheduleBreakDown(entity.ID);
                if (!workItem.HasSubWorkItems)
                {
                    SchedulingHelper.PrepareScheduleBreakDown(entity.ID);
                }
                return entity;
            }
            else
            {
                return null;
            }
        }
        //public ActionResult UpdateDurationDataFromEstimation(int targetProjectId)
        //{
        //    var currentProject = db.ProjectInformation.Find(targetProjectId);
        //    if (currentProject == null)
        //    {
        //        var jData = new
        //        {
        //            Message = "Project not found!",
        //            Status = "Error"
        //        };
        //        return Json(jData, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var workItems = db.GenericWorkItemPerProject.Where(wi => wi.ProjectInformationId == targetProjectId).ToList();
        //        var estmnDuration = db.WorkItemDuration.Where(d => d.GenericWorkItemPerProject.ProjectInformationId == targetProjectId).ToList();
        //        if (estmnDuration.Any() && workItems.Any())
        //        {
        //            foreach (WorkItemDuration itemDuration in estmnDuration)
        //            {
        //                var rootWorkItem = workItems.FirstOrDefault(w => w.GenericWorkItemPerProjectId == itemDuration.GenericWorkItemPerProjectId);
        //                if (rootWorkItem != null)
        //                {
        //                    itemDuration.HasSubWorkItems = rootWorkItem.IsSubCategory;
        //                    itemDuration.ProductivityOfCrewPerDay = itemDuration.GenericWorkItemPerProject.LabourHourlyOutPut > 0 && itemDuration.ProductivityOfCrewPerDay == 8 ? itemDuration.GenericWorkItemPerProject.LabourHourlyOutPut * 8 : itemDuration.ProductivityOfCrewPerDay;
        //                    itemDuration.Quantity = GetWorkItemQuantityOfWork(itemDuration.GenericWorkItemPerProjectId);
        //                    itemDuration.DurationPerCrew = this.CalculateDurationPerCrew(itemDuration.Quantity, itemDuration.ProductivityOfCrewPerDay);
        //                    itemDuration.EstimatedDuration = this.CalculateWorkDuration(itemDuration.DurationPerCrew, itemDuration.NumberOfCrew);

        //                    db.WorkItemDuration.Attach(itemDuration);
        //                    db.Entry(itemDuration).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }
        //            }
        //            var formatedData = new
        //            {
        //                Message = "Operation completed Successfully!",
        //                Status = "Ok"
        //            };
        //        }
        //        else
        //        {
        //            var d = new
        //            {
        //                Message = "No Items found!",
        //                Status = "Error"
        //            };
        //            return Json(d, JsonRequestBehavior.AllowGet);

        //        }
        //    }

        //    var data = new
        //    {
        //        Message = "Operation Completed Successfully",
        //        Status = "Ok"
        //    };
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public GenericWorkItemPerProject GetCurrentWorkItem(int workItemId)
        {
            GenericWorkItemPerProject wi = new GenericWorkItemPerProject();
            var temp = db.GenericWorkItemPerProject.Find(workItemId);
            if (temp != null)
            {
                wi = temp;
            }
            return wi;
        }
        private string GetConcatnatedWorkDescription(int workItemId)
        {
            //ProjectEstimationWorkItem estWItem = new ProjectEstimationWorkItem();
            WorkItemPerProject wi = new WorkItemPerProject();
            var concatnatedDescription = "";
            //var takeOff = db.TakeOffPerProject.Find(id);
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            if (currentWorkItem != null)
            {
                //var item = db.WorkItemPerProject.Find(takeOff.WorkItemPerProjectId);
                //if (item != null)
                //{
                wi = currentWorkItem;
                //}
                if (wi != null)
                {
                    if (wi.ItemName != null)
                    {
                        concatnatedDescription = wi.ItemName;
                        var block = db.Blocks.Find(wi.BlockId);
                        if (block != null)
                        {
                            concatnatedDescription = concatnatedDescription + " of " + block.Name;
                        }
                    }
                    // work item has no name, so get the name from the estimation work item
                    else
                    {
                        var estItem = db.ProjectEstimationWorkItem.Find(wi.ProjectEstimationWorkItemId);
                        if (estItem != null)
                        {
                            concatnatedDescription = estItem.ItemName;
                            var block = db.Blocks.Find(wi.BlockId);
                            if (block != null)
                            {
                                concatnatedDescription = concatnatedDescription + " of " + block.Name;
                            }
                        }
                    }

                }
            }
            return concatnatedDescription;
        }
        private WorkItemPerProject GetWorkItem(int id)
        {
            WorkItemPerProject wi = new WorkItemPerProject();
            var takeOff = db.TakeOffPerProject.Find(id);
            if (takeOff != null)
            {
                var item = db.WorkItemPerProject.Find(takeOff.WorkItemPerProjectId);
                if (item != null)
                {
                    wi = item;
                }

            }
            return wi;
        }
        //private decimal GetUpdatedWorkItemQuantity(int id)
        //{
        //    var quantity = new decimal();
        //    var item = db.WorkItemPerProject.Find(id);
        //    if (item != null)
        //    {
        //        quantity = item.TotalQuantityOfWork;
        //    }
        //    return quantity;
        //}
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
        public decimal CalculateDurationPerCrew(decimal quantity, decimal productivity)
        {
            decimal durationPerCrew = new decimal();
            if (productivity != 0)
            {
                durationPerCrew = quantity / productivity;
                durationPerCrew = Math.Round(durationPerCrew, 4);
            }
            return durationPerCrew;
        }
        private decimal CalculateWorkDuration(decimal durationPerCrew, int noOfCrew)
        {
            decimal result = new decimal();
            if (noOfCrew > 0)
            {
                result = durationPerCrew / noOfCrew;
                result = Math.Round(result, 4);
            }
            return result;
        }

        public JsonResult CopyDataFromWorkItemToDurationEstimation(int targetProjectId)
        {
            var dirtyData = db.WorkItemDuration.Where(w => w.ProjectInformationId == 0).ToList();
            if (dirtyData.Any())
            {
                db.WorkItemDuration.RemoveRange(dirtyData);
                db.SaveChanges();
            }

            var wrongData = db.WorkItemDuration.Where(w => w.ProjectInformationId == targetProjectId).ToList();
            if (wrongData.Any())
            {
                db.WorkItemDuration.RemoveRange(wrongData);
                db.SaveChanges();
            }
            #region sub category work items -- at the top of the heirarchy
            //var subCategoryWorkItems = db.GenericWorkItemPerProject.Where(w => w.IsSubCategory && w.ParentId == 0).ToList();
            var subCategoryWorkItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == targetProjectId).ToList();
            if (subCategoryWorkItems.Any())
            {
                foreach (WorkItemPerProject workItem in subCategoryWorkItems)
                {
                    AddSingleWorkItemToDurationEstimation(workItem, 0);
                }
            }

            //var childAndSubCategoryWorkItems = db.GenericWorkItemPerProject.Where(w => w.IsSubCategory && w.ParentId != 0).ToList();
            //if (childAndSubCategoryWorkItems.Any())
            //{
            //foreach (GenericWorkItemPerProject workItem in childAndSubCategoryWorkItems)
            //{
            //    var existingItem = db.WorkItemDuration.FirstOrDefault(wd => wd.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId);

            //    if (existingItem == null)
            //    {
            //        var durationRecord = new WorkItemDuration
            //        {
            //            ItemCode = workItem.ItemCode,
            //            GenericWorkItemPerProjectId = workItem.GenericWorkItemPerProjectId,
            //            WorkDescription = workItem.ItemName,
            //            Unit = GetUnitOfMeasurement(workItem.UnitOfMeasurementId ?? 0),
            //            Quantity = 0,
            //            NumberOfCrew = 0,
            //            ProductivityOfCrewPerDay = 0,

            //            SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId),
            //            StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString(),
            //            WorkCategory = workItem.WorkCategory.Name,

            //        };
            //        //durationRecord.DurationPerCrew = durationRecord.Quantity / durationRecord.ProductivityOfCrewPerDay;
            //        //durationRecord.EstimatedDuration = durationRecord.Quantity / durationRecord.NumberOfCrew;

            //        db.WorkItemDuration.Add(durationRecord);
            //        db.SaveChanges();
            //    }
            //}
            //}
            #endregion

            #region work items that are not sub category
            //var workItems = db.GenericWorkItemPerProject.Where(w => !(w.IsSubCategory)).ToList();
            //if (workItems.Any())
            //{
            //foreach (GenericWorkItemPerProject workItem in workItems)
            //{
            //    var existingItem = db.WorkItemDuration.FirstOrDefault(wd => wd.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId);
            //    if (existingItem == null)
            //    {
            //        var durationRecord = new WorkItemDuration
            //        {
            //            ItemCode = workItem.ItemCode,
            //            GenericWorkItemPerProjectId = workItem.GenericWorkItemPerProjectId,
            //            WorkDescription = workItem.ItemName,
            //            Unit = GetUnitOfMeasurement(workItem.UnitOfMeasurementId ?? 0),
            //            Quantity = this.GetWorkItemQuantityOfWork(workItem.GenericWorkItemPerProjectId),
            //            NumberOfCrew = 1,
            //            ProductivityOfCrewPerDay = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut * 8 : 0,

            //            SubProjectTypeName = ProjectEstimationHelper.GetSubProjectTypeName(workItem.SubProjectTypeId),
            //            StructureyType = workItem.WorkCategory.Configuration.CurrentValue.ToString(),
            //            WorkCategory = workItem.WorkCategory.Name,

            //        };
            //        durationRecord.DurationPerCrew = this.CalculateDurationPerCrew(durationRecord.Quantity, durationRecord.ProductivityOfCrewPerDay);
            //        durationRecord.EstimatedDuration = this.CalculateWorkDuration(durationRecord.DurationPerCrew, durationRecord.NumberOfCrew);

            //        db.WorkItemDuration.Add(durationRecord);
            //        db.SaveChanges();
            //    }

            //}
            //}
            #endregion

            var msg = new
            {
                Status = "Ok",
                Message = "Operation Completed!"
            };
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CopyManPowerResourceFromEstimation(int sourceId, int destinationId)
        {
            var sourceItem = db.WorkItemPerProject.Find(sourceId);
            if (sourceItem == null)
            {
                var m = new { Message = "Source item not found!", Status = "Error" };
                return Json(m, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var destinationItem = db.WorkItemDuration.Find(destinationId);
                if (destinationItem != null)
                {
                    var labors = db.ProjectDirectLabour.Where(l => l.WorkItemPerProjectId == sourceId).ToList();

                    if (labors.Any())
                    {
                        foreach (ProjectDirectLabour labor in labors)
                        {
                            var existingResource = db.ResourceEstimationLabour.FirstOrDefault(re => re.WorkItemDurationId == destinationId && re.ProjectEstimationIndexLabourId == labor.ProjectEstimationIndexLabourId);
                            if (existingResource == null)
                            {
                                var record = new ResourceEstimationLabour
                                {
                                    WorkItemDurationId = destinationItem.WorkItemDurationId,
                                    ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                                    Number = labor.Quantity,
                                    UnitFactor = labor.UnitFactory,
                                    UnitFactorAndNumberPerCrew = labor.UnitFactory * labor.Quantity,
                                    //NumberPerDay = labor.NumberPerDay,
                                    //TotalCostPerDay = labor.TotalCostPerDay,
                                    //IndexedHourlySalary = ProjectEstimationHelper.GetIndexedHourlyPayPerJobTitle(labor.ProjectEstimationIndexLabourId),
                                    IndexedHourlySalary = labor.IndexedHourlySalary,

                                    OrgBranchName = labor.OrgBranchName,
                                    DateCreated = labor.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = labor.CreatedBy,
                                    ModifiedBy = User.Identity.Name
                                };
                                record.NumberPerDay = destinationItem.NumberOfCrew > 0 ? record.UnitFactorAndNumberPerCrew * destinationItem.NumberOfCrew : record.UnitFactorAndNumberPerCrew;
                                record.TotalCostPerDay = 8 * record.IndexedHourlySalary * record.NumberPerDay;

                                db.ResourceEstimationLabour.Add(record);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new { Message = "Manpower resources copied!", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CopyEquipmentResourceFromEstimation(int sourceId, int destinationId)
        {
            var sourceItem = db.WorkItemPerProject.Find(sourceId);
            if (sourceItem == null)
            {
                var m = new { Message = "Source item not found!", Status = "Error" };
                return Json(m, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var destinationItem = db.WorkItemDuration.Find(destinationId);
                if (destinationItem != null)
                {
                    var equipments = db.ProjectEquipment.Where(l => l.WorkItemPerProjectId == sourceId).ToList();

                    if (equipments.Any())
                    {
                        foreach (ProjectEquipment equipment in equipments)
                        {
                            var existingResource = db.ResourceEstimationEquipments.FirstOrDefault(re => re.WorkItemDurationId == destinationId && re.FleetsID == equipment.FleetsID);
                            if (existingResource == null)
                            {
                                var record = new ResourceEstimationEquipment
                                {
                                    WorkItemDurationId = destinationItem.WorkItemDurationId,
                                    FleetsID = equipment.FleetsID,
                                    Number = equipment.Quantity,
                                    UnitFactor = equipment.UnitFactory,
                                    UnitFactorAndNumberPerCrew = equipment.UnitFactory * equipment.Quantity,
                                    //NumberPerDay = labor.NumberPerDay,
                                    //TotalCostPerDay = labor.TotalCostPerDay,
                                    //HourlyCost = ProjectEstimationHelper.GetEquipmentRentalRate(equipment.FleetsID),
                                    HourlyCost = equipment.RentalCost,

                                    OrgBranchName = equipment.OrgBranchName,
                                    DateCreated = equipment.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = equipment.CreatedBy,
                                    ModifiedBy = User.Identity.Name
                                };
                                record.NumberPerDay = destinationItem.NumberOfCrew > 0 ? record.UnitFactorAndNumberPerCrew * destinationItem.NumberOfCrew : record.UnitFactorAndNumberPerCrew;
                                record.TotalCostPerDay = 8 * record.HourlyCost * record.NumberPerDay;

                                db.ResourceEstimationEquipments.Add(record);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new { Message = "Machinery resources copied!", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult CopyMaterialResourceFromEstimation(int sourceId, int destinationId)
        {
            var sourceItem = db.WorkItemPerProject.Find(sourceId);
            if (sourceItem == null)
            {
                var m = new { Message = "Source item not found!", Status = "Error" };
                return Json(m, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var destinationItem = db.WorkItemDuration.Find(destinationId);
                if (destinationItem != null)
                {
                    var materials = db.ProjectMaterial.Where(l => l.WorkItemPerProjectId == sourceId).ToList();

                    if (materials.Any())
                    {
                        foreach (ProjectMaterial material in materials)
                        {
                            var existingResource = db.ResourceEstimationMaterials.FirstOrDefault(re => re.WorkItemDurationId == destinationId && re.ItemCode == material.ItemCode);
                            if (existingResource == null)
                            {
                                var record = new ResourceEstimationMaterial
                                {
                                    WorkItemDurationId = destinationItem.WorkItemDurationId,
                                    ItemCategoryCode = material.ItemCategoryCode,
                                    ItemCode = material.ItemCode,
                                    QuantityPerUnit = material.Quantity,
                                    TotalCost = material.TotalCost,
                                    Unit = material.Unit,
                                    UnitPrice = material.UnitPrice,

                                    //NumberPerDay = material.NumberPerDay,
                                    //TotalCostPerDay = material.TotalCostPerDay,
                                    //IndexedHourlySalary = ProjectEstimationHelper.GetIndexedHourlyPayPerJobTitle(material.ProjectEstimationIndexLabourId),

                                    OrgBranchName = material.OrgBranchName,
                                    DateCreated = material.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = material.CreatedBy,
                                    ModifiedBy = User.Identity.Name
                                };
                                record.TotalQuantity = record.QuantityPerUnit * destinationItem.Quantity;
                                record.TotalCost = record.TotalQuantity * record.UnitPrice;

                                db.ResourceEstimationMaterials.Add(record);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Json(new { Message = "Material resources copied!", Status = "Ok" }, JsonRequestBehavior.AllowGet);
            }
        }
        public void CopyItemFromEstimationToDuration(int itemId)
        {
            var existingItem = db.WorkItemDuration.FirstOrDefault(w => w.Source == DurationItemSource.Estimation && w.WorkItemPerProjectId == itemId);
            if (existingItem != null)
            {
                db.WorkItemDuration.Remove(existingItem);
                db.SaveChanges();
            }
            var item = db.WorkItemPerProject.Find(itemId);
            if (item != null)
            {
                AddSingleWorkItemToDurationEstimation(item, 0);
            }
        }
        private void AddSingleWorkItemToDurationEstimation(WorkItemPerProject workItem, int parentDurationItemId)
        {
            var existingItem = db.WorkItemDuration.FirstOrDefault(wd => wd.Source == DurationItemSource.Estimation && wd.WorkItemPerProjectId == workItem.WorkItemPerProjectId);
            if (existingItem == null)
            {
                var durationRecord = new WorkItemDuration
                {
                    ItemCode = workItem.ItemCode,
                    ProjectInformationId = workItem.ProjectInformationId,
                    WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                    WorkDescription = workItem.ItemName,
                    Unit = GetUnitOfMeasurement(workItem.UnitOfMeasurementId ?? 0),
                    //Quantity = this.GetWorkItemQuantityOfWork(workItem.WorkItemPerProjectId),
                    Quantity = workItem.TotalQuantityOfWork + workItem.AddedQuantityOfWork - workItem.OmittedQuantityOfWork,
                    NumberOfCrew = 1,
                    ProductivityOfCrewPerDay = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut * 8 : 0,

                    Source = DurationItemSource.Estimation,
                    HasSubWorkItems = workItem.IsSubCategory,

                    Parent = parentDurationItemId,

                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                    BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItem.WorkCategory.ConfigurationId, workItem.ProjectType) + " " + workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItem.WorkCategory.Index + " " + workItem.WorkCategory.Name,

                    BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(workItem.WorkItemPerProjectId),
                    MaterialCost = workItem.MaterialCostPerUnit,
                    EquipmentCost = workItem.EquipmentCostPerUnit,
                    LabourCost = workItem.LaborCostPerUnit,
                    DirectCost = workItem.DirectCost,
                    ProfitCost = workItem.ProfitCost,
                    OverHeadCost = workItem.OverHeadCost,
                    TotalUnitRate = workItem.TotalUnitCost,

                    WorkCategoryId = workItem.WorkCategoryId,
                    ConfigurationId = workItem.WorkCategory.ConfigurationId,
                    BlockId = workItem.BlockId.Value,
                    SubProjectTypeId = ProjectEstimationHelper.GetWorkItemSubProjectType(workItem).SubProjectTypeId

                };
                durationRecord.DurationPerCrew = this.CalculateDurationPerCrew(durationRecord.Quantity, durationRecord.ProductivityOfCrewPerDay);
                durationRecord.EstimatedDuration = this.CalculateWorkDuration(durationRecord.DurationPerCrew, durationRecord.NumberOfCrew);

                db.WorkItemDuration.Add(durationRecord);
                db.SaveChanges();

                #region copy resources
                var materials = db.ProjectMaterial.Where(m => m.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                var equipments = db.ProjectEquipment.Where(m => m.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                var labors = db.ProjectDirectLabour.Where(m => m.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                #region material
                if (materials.Any())
                {
                    foreach (ProjectMaterial material in materials)
                    {
                        var record = new ResourceEstimationMaterial
                        {
                            WorkItemDurationId = durationRecord.WorkItemDurationId,
                            ItemCategoryCode = material.ItemCategoryCode,
                            ItemCode = material.ItemCode,
                            QuantityPerUnit = material.Quantity,
                            TotalCost = material.TotalCost,
                            Unit = material.Unit,
                            UnitPrice = material.UnitPrice,

                            OrgBranchName = material.OrgBranchName,
                            DateCreated = material.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = material.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };
                        record.TotalQuantity = record.QuantityPerUnit * durationRecord.Quantity;
                        record.TotalCost = record.TotalQuantity * record.UnitPrice;

                        db.ResourceEstimationMaterials.Add(record);
                        db.SaveChanges();
                    }
                }
                #endregion
                #region equipments
                if (equipments.Any())
                {
                    foreach (ProjectEquipment equipment in equipments)
                    {
                        var record = new ResourceEstimationEquipment
                        {
                            WorkItemDurationId = durationRecord.WorkItemDurationId,
                            FleetsID = equipment.FleetsID,
                            Number = equipment.Quantity,
                            UnitFactor = equipment.UnitFactory,
                            UnitFactorAndNumberPerCrew = equipment.UnitFactory * equipment.Quantity,
                            HourlyCost = equipment.RentalCost,

                            OrgBranchName = equipment.OrgBranchName,
                            DateCreated = equipment.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = equipment.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };
                        record.NumberPerDay = durationRecord.NumberOfCrew > 0 ? record.UnitFactorAndNumberPerCrew * durationRecord.NumberOfCrew : record.UnitFactorAndNumberPerCrew;
                        record.TotalCostPerDay = 8 * record.HourlyCost * record.NumberPerDay;

                        db.ResourceEstimationEquipments.Add(record);
                        db.SaveChanges();
                    }
                }
                #endregion
                #region labors
                if (labors.Any())
                {
                    foreach (ProjectDirectLabour labor in labors)
                    {
                        var record = new ResourceEstimationLabour
                        {
                            WorkItemDurationId = durationRecord.WorkItemDurationId,
                            ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                            Number = labor.Quantity,
                            UnitFactor = labor.UnitFactory,
                            UnitFactorAndNumberPerCrew = labor.UnitFactory * labor.Quantity,
                            IndexedHourlySalary = labor.IndexedHourlySalary,

                            OrgBranchName = labor.OrgBranchName,
                            DateCreated = labor.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = labor.CreatedBy,
                            ModifiedBy = User.Identity.Name
                        };
                        record.NumberPerDay = durationRecord.NumberOfCrew > 0 ? record.UnitFactorAndNumberPerCrew * durationRecord.NumberOfCrew : record.UnitFactorAndNumberPerCrew;
                        record.TotalCostPerDay = 8 * record.IndexedHourlySalary * record.NumberPerDay;

                        db.ResourceEstimationLabour.Add(record);
                        db.SaveChanges();
                    }
                }
                #endregion
                #endregion

                if (workItem.IsSubCategory)
                {
                    var childItems = db.WorkItemPerProject.Where(itm => itm.ParentId == workItem.WorkItemPerProjectId).ToList();
                    if (childItems.Any())
                    {
                        foreach (WorkItemPerProject childItem in childItems)
                        {
                            AddSingleWorkItemToDurationEstimation(childItem, durationRecord.WorkItemDurationId);
                        }
                    }
                }
            }
        }
        //private void AddStructureTypeToWBSMapping(int projectId)
        //{
        //    var blockMappings = db.BlockMappings.Where(bm => bm.Block.ProjectInformationId == projectId).ToList();
        //    var strId = new HashSet<int>(blockMappings.Select(bm => bm.ConfigurationId));
        //    var structretypesUnderProject = db.Configurations.Where(cn => (strId.Contains(cn.ConfigurationId))).ToList();
        //    if (structretypesUnderProject.Any())
        //    {
        //        foreach (Configuration structureToMapp in structretypesUnderProject)
        //        {
        //            var existingWbs =
        //            db.ProjectWorkBreakDown
        //            .FirstOrDefault(wbs => wbs.ConfigurationId == structureToMapp.ConfigurationId && wbs.ProjectInformationId == projectId);
        //            if (existingWbs == null)
        //            {
        //                var entity = new ProjectWorkBreakDown
        //                {
        //                    ConfigurationId = structureToMapp.ConfigurationId,
        //                    Description = structureToMapp.CurrentValue,
        //                    //WBSCode = structureToMapp.BlockMappingId.ToString("D6"),
        //                    ProjectInformationId = projectId,
        //                };
        //                db.ProjectWorkBreakDown.Add(entity);
        //                db.SaveChanges();
        //            }
        //        }
        //    }
        //}
        //private void AddWorkItemToSchedule(WorkItemDuration model)
        //{
        //    var tskDb = new ExceedDbContext();
        //    var taskToInsert = new TaskViewModel();
        //    var targetWBS = new ProjectWorkBreakDown();

        //    var currentWorkItem = db.WorkItemPerProject.Find(model.WorkItemId);
        //    var projInformation = new ProjectInformation();
        //    if (currentWorkItem != null)
        //    {
        //        var tempProject = db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
        //        if (tempProject != null)
        //        {
        //            projInformation = tempProject;
        //        }
        //    }

        //    // find existing wbs under the given project information and take one of them
        //    var currentWBS = db.ProjectWorkBreakDown.Where(wbs => wbs.ProjectInformationId == projInformation.ProjectInformationId);
        //    if (currentWBS.Any())
        //    {
        //        var tempWBS = currentWBS.FirstOrDefault();
        //        if (tempWBS != null)
        //        {
        //            targetWBS = tempWBS;
        //        }
        //    }
        //    else
        //    {
        //        // if there is no wbs in the given project,
        //        // create a new one and create the task and assign this newly created
        //        // wbs to its ProjectWorkBreakdownId property.

        //        var temp = new ProjectWorkBreakDown();
        //        targetWBS = temp;
        //        targetWBS.ProjectInformationId = projInformation.ProjectInformationId;
        //        targetWBS.Description = projInformation.Name + "-wbs" + temp.ProjectWorkBreakDownId;

        //        var wbsDb = new ExceedDbContext();
        //        db.ProjectWorkBreakDown.Add(targetWBS);
        //        db.SaveChanges();
        //    }


        //    //if (targetWBS == null)
        //    //{

        //    //}

        //    //taskToInsert.ParentID = null;
        //    //taskToInsert.TaskCode = null;
        //    var todaysDate = DateTime.Now;
        //    taskToInsert.Start = todaysDate;
        //    taskToInsert.End = todaysDate.AddDays((int)model.EstimatedDuration);

        //    taskToInsert.Title = model.WorkDescription;
        //    //taskToInsert.PercentComplete = 0;
        //    //taskToInsert.OrderId = 0;
        //    taskToInsert.Summary = false;
        //    taskToInsert.Expanded = true;
        //    taskToInsert.ProjectWorkBreakDownId = targetWBS.ProjectWorkBreakDownId;
        //    //taskToInsert.ProjectWorkBreakDown = targetWBS;

        //    taskToInsert.WorkItemDurationId = model.WorkItemDurationId;

        //    taskToInsert.EstimatedDuration = model.EstimatedDuration;
        //    taskToInsert.BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(currentWorkItem.WorkItemPerProjectId);
        //    taskToInsert.OrderId = this.SetWorkItemOrderInSchedule(currentWorkItem);
        //    taskToInsert.Version = 1;
        //    //taskToInsert.OriginalDuration = 1;
        //    //taskToInsert.RemainingDuration = 1;

        //    var entity = taskToInsert.ToEntity();
        //    var tempDb = new ExceedDbContext();
        //    tempDb.Tasks.Add(entity);
        //    tempDb.SaveChanges();

        //}

        // :
        // the content below is added to make the work item duration calculations
        // as detail of projects

        //private int GetBlockId(int id)
        //{
        //    var idToReturn = 0;
        //    var durationWorkItem = db.WorkItemDuration.FirstOrDefault(wd => wd.WorkItemDurationId == id);
        //    if (durationWorkItem != null)
        //    {
        //        var workItem =
        //                db.WorkItemPerProject
        //                .FirstOrDefault(wi => wi.WorkItemPerProjectId == durationWorkItem.WorkItemPerProjectId);
        //        if (workItem != null)
        //        {
        //            if (workItem.IsSubCategory)
        //            {
        //                idToReturn = workItem.BlockId ?? 0;
        //            }
        //            else
        //            {
        //                if (workItem.BlockId == null)
        //                {
        //                    // find the parent and use that block id
        //                    var parent = db.WorkItemPerProject.FirstOrDefault(w => w.WorkItemPerProjectId == workItem.ParentId);
        //                    if (parent != null)
        //                    {
        //                        idToReturn = parent.BlockId ?? 0;
        //                    }
        //                }
        //                else
        //                {
        //                    idToReturn = (int)workItem.BlockId;
        //                }
        //            }
        //        }
        //    }
        //    return idToReturn;
        //}
        private int SetWorkItemOrderInSchedule(WorkItemDuration model)
        {
            int order = 0;
            var currentProject = db.ProjectInformation.Find(model.ProjectInformationId);
            if (currentProject != null)
            {
                var availableTasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProject.ProjectInformationId);
                if (availableTasks.Any())
                {
                    order = availableTasks.Max(task => task.OrderID) + 1;
                }
            }
            return order;
        }
        private decimal GetWorkItemQuantityOfWork(int workItemId)
        {
            var qnty = new decimal();
            var currentWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(w => w.GenericWorkItemPerProjectId == workItemId);
            if (currentWorkItem != null)
            {
                qnty = currentWorkItem.TotalQuantityOfWork + currentWorkItem.AddedQuantityOfWork - currentWorkItem.OmittedQuantityOfWork;
                // sub project - block, station or other kind
                var subProject = db.SubProjectType.Find(currentWorkItem.SubProjectTypeId);
                if (subProject != null)
                {
                    if (subProject.NumberOfBlocks > 0)
                    {
                        qnty = (currentWorkItem.TotalQuantityOfWork + currentWorkItem.AddedQuantityOfWork - currentWorkItem.OmittedQuantityOfWork) * subProject.NumberOfBlocks;
                    }
                }
                else
                {
                    var parentWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(wi => wi.GenericWorkItemPerProjectId == currentWorkItem.ParentId);
                    if (parentWorkItem != null)
                    {
                        var subP = db.SubProjectType.Find(parentWorkItem.SubProjectTypeId);
                        if (subP != null)
                        {
                            if (subP.NumberOfBlocks > 0)
                            {
                                qnty = (currentWorkItem.TotalQuantityOfWork + currentWorkItem.AddedQuantityOfWork - currentWorkItem.OmittedQuantityOfWork) * subP.NumberOfBlocks;
                            }
                        }
                    }
                }
            }
            return qnty;
        }
        private decimal GetEstimatedDurationOfWorkItem(WorkItemPerProject record)
        {
            var duration = new decimal();
            //var rootWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(wi => wi.GenericWorkItemPerProjectId == record.GenericWorkItemPerProjectId);
            //var durationEstimatinModel = db.WorkItemDuration.FirstOrDefault(d => d.GenericWorkItemPerProjectId == record.GenericWorkItemPerProjectId);

            //if (durationEstimatinModel != null && rootWorkItem != null)
            //{
            //    var subProjectType = db.SubProjectType.Find(rootWorkItem.SubProjectTypeId);
            //    if (subProjectType != null)
            //    {
            //        duration = subProjectType.NumberOfBlocks > 0 ? durationEstimatinModel.EstimatedDuration / subProjectType.NumberOfBlocks
            //            : durationEstimatinModel.EstimatedDuration;
            //    }
            //}
            return duration;
        }
        //private void GetEstimatedDurationOf
        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        private int GetProjectIdOfDurationItem(int durationItemId)
        {
            var id = 0;
            var parentItem = db.WorkItemDuration.Find(durationItemId);
            if (parentItem != null && parentItem.Parent != 0)
            {
                return GetProjectIdOfDurationItem(parentItem.Parent);
            }
            if (parentItem != null && parentItem.Parent == 0)
            {
                id = parentItem.ProjectInformationId;
            }
            return id;
        }

        #region methods to run on browser, just for specific purpose.
        /// <summary>
        /// This is used to clean existing (old) data from WorkItemDuration table. "id" is ProjectId.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult CleanOldData(int id)
        {
            var dirtyData = db.WorkItemDuration.Where(w => w.ProjectInformationId == 0).ToList();
            if (dirtyData.Any())
            {
                db.WorkItemDuration.RemoveRange(dirtyData);
                db.SaveChanges();
            }

            var wrongData = db.WorkItemDuration.Where(w => w.ProjectInformationId == id).ToList();
            if (wrongData.Any())
            {
                db.WorkItemDuration.RemoveRange(wrongData);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        /// <summary>
        /// This is used to update WorkItemDurationTable. this targets the newly added columns:
        /// WorkCategoryId, ConfigrationId, BlockId, SubProjectTypeId  and
        /// BudgetCost, MaterialCost, EquipmentCost, LaborCost, ProfitCost, OverheadCost, DirectCost, TotalUnitRate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult UpdateOldDataToAccomodateNewlyAddedColumns(int id)
        {
            var dataToUpdate = db.WorkItemDuration.Where(w => w.Source == DurationItemSource.Estimation && w.ProjectInformationId == id).ToList();
            if (dataToUpdate.Any())
            {
                foreach (WorkItemDuration item in dataToUpdate)
                {
                    if (item.WorkCategoryId <= 0 || item.ConfigurationId <= 0 || item.BlockId <= 0 || item.SubProjectTypeId <= 0)
                    {
                        var sourceItem = db.WorkItemPerProject.Find(item.WorkItemPerProjectId);
                        if (sourceItem != null)
                        {
                            item.BudgetCost = ProjectEstimationHelper.GetWorkItemBudgetedCost(sourceItem.WorkItemPerProjectId);
                            item.MaterialCost = sourceItem.MaterialCostPerUnit;
                            item.EquipmentCost = sourceItem.EquipmentCostPerUnit;
                            item.LabourCost = sourceItem.LaborCostPerUnit;
                            item.DirectCost = sourceItem.DirectCost;
                            item.ProfitCost = sourceItem.ProfitCost;
                            item.OverHeadCost = sourceItem.OverHeadCost;
                            item.TotalUnitRate = sourceItem.TotalUnitCost;

                            item.WorkCategoryId = sourceItem.WorkCategoryId;
                            item.ConfigurationId = sourceItem.WorkCategory.ConfigurationId;
                            item.BlockId = sourceItem.BlockId.Value;
                            item.SubProjectTypeId = ProjectEstimationHelper.GetWorkItemSubProjectType(sourceItem).SubProjectTypeId;

                            db.WorkItemDuration.Attach(item);
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
        #endregion
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}