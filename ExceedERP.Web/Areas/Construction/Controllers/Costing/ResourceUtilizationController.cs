

using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    [AuthorizeRoles(ConstructionConstants.CostingUser)]
    public class ResourceUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ResourceUtilization
        public ActionResult Index()
        {
            ConstructionInputs inputs = new ConstructionInputs();
            var lookups = db.Lookups.ToList();

            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.Name, c.WorkCategoryId}).ToList();

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId,x.Abbreviation}).ToList();

            #region concatnated work items
            //var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemCode).ToList();
            //var workItemsWithConcatnatedValue = new List<WorkItemPerProject>();
            //var workItem = new WorkItemPerProject();
            //foreach (var item in workItems)
            //{
            //    var tempWorkItem = new WorkItemPerProject();
            //    if (item.BlockId != null)
            //    {
            //        tempWorkItem = item;
            //        var workItemBlock = db.Blocks.Find(item.BlockId);
            //        if (workItemBlock != null)
            //        {
            //            tempWorkItem.ItemName = item.ItemName + " of " + workItemBlock.Name;
            //        }
            //    }
            //    workItemsWithConcatnatedValue.Add(tempWorkItem);
            //}
            #endregion
            //ViewData["WorkItems"] = workItems;

            ViewData["ConstructionPeriods"] = db.ConstructionPeriod.Select(p => new { p.Name, p.ConstructionPeriodId}).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId}).ToList();

            var methodsofWork = db.MethodOfWork.ToList();
            ViewData["Methods"] = methodsofWork;

            //ViewData["ProjectWorkItems"] = db.WorkItemPerProject.Select(w => new { w.ItemName, w.WorkItemPerProjectId});

            ViewData["ItemCategorys"] = db.ConstructionItemCategories.Select(p => new { p.ItemCategoryCode,  p.Name });

            ViewData["Items"] = db.InventoryItems.Select(it => new { it.Name, it.ItemCode}).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;

            return View();
        }
        public ActionResult Project(int p) {
            #region contents
            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.Name, c.WorkCategoryId }).ToList();

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            #region concatnated work items
            //var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemCode).ToList();
            
            #endregion
            //ViewData["WorkItems"] = workItems;

            ViewData["ConstructionPeriods"] = db.ConstructionPeriod.Select(c => new { c.Name, c.ConstructionPeriodId }).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();

            var methodsofWork = db.MethodOfWork.ToList();
            ViewData["Methods"] = methodsofWork;

            //ViewData["ProjectWorkItems"] = db.WorkItemPerProject.Select(w => new { w.ItemName, w.WorkItemPerProjectId });

            ViewData["ItemCategorys"] = db.ConstructionItemCategories.Select(c => new { c.ItemCategoryCode, c.Name });

            ViewData["Items"] = db.InventoryItems.Select(it => new { it.Name, it.ItemCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;
            #endregion

            ViewBag.Project = "";
            ViewBag.ProjectId = p;
            var currentProject = db.ProjectInformation.Find(p);
            if (currentProject != null)
            {
                ViewBag.Project = currentProject.Name;
            }

            return View();
        }
        public ActionResult DailyUtl(int d) {

            #region
            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.Name, c.WorkCategoryId }).ToList();

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            #region concatnated work items
            //var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemCode).ToList();

            #endregion
            //ViewData["WorkItems"] = workItems;

            ViewData["ConstructionPeriods"] = db.ConstructionPeriod.Select(c => new { c.Name, c.ConstructionPeriodId }).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();

            var methodsofWork = db.MethodOfWork.ToList();
            ViewData["Methods"] = methodsofWork;

            //ViewData["ProjectWorkItems"] = db.WorkItemPerProject.Select(w => new { w.ItemName, w.WorkItemPerProjectId });

            ViewData["ItemCategorys"] = db.ConstructionItemCategories.Select(c => new { c.ItemCategoryCode, c.Name });

            ViewData["Items"] = db.InventoryItems.Select(it => new { it.Name, it.ItemCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(
                    x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            var internalIds = new HashSet<Guid>(db.InternalMachinaryRent.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(db.MachinaryRent.Select(ee => ee.FleetsID));
            var combinedEquipments =
                db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)))
                .Select(eq => new { eq.FleetsID, eq.EquipmentName })
                .ToList();
            ViewData["Equipment"] = combinedEquipments;
            #endregion

            ViewBag.Project = "";
            ViewBag.ProjectId = "";
            ViewBag.Period = "";
            ViewBag.Day = "";
            ViewBag.DayId = "";
            //var currentProject = db.ProjectInformation.Find(p);
            //var currentPeriod = db.ResourceUtilizationPeriods.Find(m);
            var currentDay = db.ResourceUtilizationDays.Find(d);
            //if (currentProject != null)
            //{
            //    ViewBag.Project = currentProject.Name;
            //}
            //if (currentPeriod != null)
            //{
            //    ViewBag.Period = currentPeriod.Period.Name;
            //}
            if (currentDay != null)
            {
                ViewBag.Day = currentDay.Name;
                ViewBag.Project = currentDay.ResourceUtilizationPeriod.Project.Name;
                ViewBag.ProjectId = currentDay.ResourceUtilizationPeriod.ProjectInformationId;
                ViewBag.Period = currentDay.ResourceUtilizationPeriod.Period.Name;
                ViewBag.DayId = currentDay.ResourceUtilizationDayId;
            }
            return View();
        }
        #region methods
        private WorkCategory GetWorkItemCategory(int workItemId)
        {
            WorkCategory category = new WorkCategory();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                var tempCategory = db.WorkCategory.Find(workItem.WorkCategoryId);
                if (tempCategory != null)
                {
                    category = tempCategory;
                }
            }

            return category;
        }
        private UnitOfMeasurement GetWorkItemUnitOfMeasurnment(int workItemId)
        {
            UnitOfMeasurement unit = new UnitOfMeasurement();
            var workItem = db.WorkItemPerProject.Find(workItemId);
            if (workItem != null)
            {
                var tempUnit = db.UnitOfMeasurement.Find(workItem.UnitOfMeasurementId);
                if (tempUnit != null)
                {
                    unit = tempUnit;
                }
            }

            return unit;
        }
        private WorkItemPerProject GetWorkItem(int id)
        {
            WorkItemPerProject workItem = new WorkItemPerProject();
            var tempWorkItem = db.WorkItemPerProject.Find(id);
            if (tempWorkItem != null)
            {
                workItem = tempWorkItem;
            }
            return workItem;

        }
        private Block GetBlock(int? id)
        {
            Block block = new Block();
            if (id != null)
            {
                var temp = db.Blocks.Find(id);
                if (temp != null)
                {
                    block = temp;
                }
            }
            return block;

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
        private bool PeriodHasUtilizationData(ResourceUtilizationWorkItem model)
        {
            bool hasUtilizationData = false;
            var temp = db.ResourceUtilizationWorkItem.Where(ruwi => ruwi.ResourceUtilizationDayId == model.ResourceUtilizationDayId
                        && ruwi.WorkItemPerProjectId == model.WorkItemPerProjectId).FirstOrDefault();
            if (temp != null)
            {
                hasUtilizationData = true;
            }
            return hasUtilizationData;
        }
        #endregion
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
            IQueryable<ResourceUtilizationWorkItem> workItems = db.ResourceUtilizationWorkItem.Where(ru => ru.ResourceUtilizationDayId == dayId);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItem => new
            {
                ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
                MethodOfWorkId = workItem.MethodOfWorkId,
                WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                IsSubCategory = workItem.IsSubCategory,
                ParentId = workItem.ParentId,
                ResourceUtilizationDayId = workItem.ResourceUtilizationDayId,
                Category = workItem.WorkCategory,
                ItemName = workItem.ItemName,
                UnitOfMeasurnment = workItem.UnitOfMeasurnment,
                ExecutedWork = workItem.ExecutedWork,
                WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
                OutPutPerDay = workItem.OutPutPerDay,
                DirectContractUnitPrice = workItem.DirectContractUnitPrice,
                Income = workItem.Income,
                MaterialCost = workItem.MaterialCost,
                EquipmentCost = workItem.EquipmentCost,
                LaborCost = workItem.LaborCost,
                TotalDirectCost = workItem.TotalDirectCost,
                TotalCost = workItem.TotalCost,
                ProfitCost = workItem.ProfitCost,
                OverheadCost = workItem.OverheadCost,

                SubProjectTypeName = workItem.SubProjectTypeName,
                BlockName = workItem.BlockName,
                StructureyType = workItem.StructureyType,
                WorkCategory = workItem.WorkCategory,

                OrgBranchName = workItem.OrgBranchName,
                DateCreated = workItem.DateCreated,
                LastModified = workItem.LastModified,
                CreatedBy = workItem.CreatedBy,
                ModifiedBy = workItem.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult ResourceUtilzationChildWorkItems_Read([DataSourceRequest]DataSourceRequest request, int parentId)
        {
            IQueryable<ResourceUtilizationWorkItem> workItems = db.ResourceUtilizationWorkItem.Where(ru => ru.ParentId == parentId);
            DataSourceResult result = workItems.ToDataSourceResult(request, workItem => new
            {
                ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
                MethodOfWorkId = workItem.MethodOfWorkId,
                WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                IsSubCategory = workItem.IsSubCategory,
                ParentId = workItem.ParentId,
                ResourceUtilizationDayId = workItem.ResourceUtilizationDayId,
                Category = workItem.WorkCategory,
                ItemName = workItem.ItemName,
                UnitOfMeasurnment = workItem.UnitOfMeasurnment,
                ExecutedWork = workItem.ExecutedWork,
                WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
                OutPutPerDay = workItem.OutPutPerDay,
                DirectContractUnitPrice = workItem.DirectContractUnitPrice,
                Income = workItem.Income,
                MaterialCost = workItem.MaterialCost,
                EquipmentCost = workItem.EquipmentCost,
                LaborCost = workItem.LaborCost,
                TotalDirectCost = workItem.TotalDirectCost,
                TotalCost = workItem.TotalCost,
                ProfitCost = workItem.ProfitCost,
                OverheadCost = workItem.OverheadCost,

                SubProjectTypeName = workItem.SubProjectTypeName,
                BlockName = workItem.BlockName,
                StructureyType = workItem.StructureyType,
                WorkCategory = workItem.WorkCategory,

                OrgBranchName = workItem.OrgBranchName,
                DateCreated = workItem.DateCreated,
                LastModified = workItem.LastModified,
                CreatedBy = workItem.CreatedBy,
                ModifiedBy = workItem.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult SpecificWorkItemDetail_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            var currentWorkItem = db.ResourceUtilizationWorkItem.Find(workItemId);
            List<WorkItemDetailViewModel> list = new List<WorkItemDetailViewModel>();
            if (currentWorkItem != null)
            {
                #region add components
                var nameRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Item Name",
                    Value = currentWorkItem.ItemName
                };
                var qtyRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Excuted amount of work",
                    Value = currentWorkItem.ExecutedWork.ToString("N4")
                };
                var materialCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Material Cost",
                    Value = currentWorkItem.MaterialCost.ToString("N4")
                };
                var equipmentCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Equipment Cost",
                    Value = currentWorkItem.EquipmentCost.ToString("N4")
                };
                var laborCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Manpower Cost",
                    Value = currentWorkItem.LaborCost.ToString("N4")
                };
                var directCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Direct cost",
                    Value = currentWorkItem.TotalDirectCost.ToString("N4")
                };
                var profitCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Profit Cost (" + currentWorkItem.WorkItemPerProject.ProfitMargin + "%)",
                    Value = currentWorkItem.ProfitCost.ToString("N4")
                };
                var overheadCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Overhead Cost (" + currentWorkItem.WorkItemPerProject.OverheadRate + "%)",
                    Value = currentWorkItem.OverheadCost.ToString("N4")
                };
                var totalCostRecord = new WorkItemDetailViewModel
                {
                    Attribute = "Total Cost",
                    Value = currentWorkItem.TotalCost.ToString("N4")
                };
                #endregion
                //list.Add(nameRecord);
                //list.Add(qtyRecord);

                list.Add(materialCostRecord);
                list.Add(equipmentCostRecord);
                list.Add(laborCostRecord);
                list.Add(directCostRecord);
                //list.Add(profitCostRecord);
                //list.Add(overheadCostRecord);
                list.Add(totalCostRecord);

            }
            //IQueryable<TemplateWorkItemDetailViewModel> result = list.AsQueryable();
            return Json(list.AsQueryable().ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceUtilizationWorkItem_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationWorkItem workItem, int dayId)
        {
            if (ModelState.IsValid)
            {
                if (PeriodHasUtilizationData(workItem))
                {
                    ModelState.AddModelError("", "This work item has resource utilizatin data for this period");
                }
                else
                {
                    var entity = new ResourceUtilizationWorkItem
                    {
                        //ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
                        MethodOfWorkId = workItem.MethodOfWorkId,
                        WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                        ResourceUtilizationDayId = dayId,
                        ItemName = workItem.ItemName,
                        IsSubCategory = workItem.IsSubCategory,
                        ParentId = workItem.ParentId,
                        UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(workItem.WorkItemPerProjectId).Abbreviation,
                        ExecutedWork = workItem.ExecutedWork,
                        WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
                        OutPutPerDay = workItem.OutPutPerDay,
                        DirectContractUnitPrice = this.GetWorkItem(workItem.WorkItemPerProjectId).TotalUnitCost,
                        Income = workItem.Income,
                        TotalDirectCost = workItem.TotalDirectCost,
                        MaterialCost = this.GetMaterialCost(workItem.ResourceUtilizationWorkItemId),
                        EquipmentCost = this.GetEquipmentCost(workItem.ResourceUtilizationWorkItemId),
                        LaborCost = this.GetLaborCost(workItem.ResourceUtilizationWorkItemId),
                        TotalCost = workItem.TotalCost,
                        ProfitCost = workItem.ProfitCost,
                        OverheadCost = workItem.OverheadCost,


                        SubProjectTypeName = workItem.SubProjectTypeName,
                        BlockName = workItem.BlockName,
                        StructureyType = workItem.StructureyType,
                        WorkCategory = workItem.WorkCategory,

                        OrgBranchName = workItem.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = workItem.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = workItem.ModifiedBy
                    };

                    entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
                    entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
                    entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;
                    entity.TotalCost = entity.TotalDirectCost + entity.ProfitCost + entity.OverheadCost;

                    db.ResourceUtilizationWorkItem.Add(entity);
                    db.SaveChanges();

                    workItem.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;
                    workItem.WorkCategory = entity.WorkCategory;
                    workItem.UnitOfMeasurnment = entity.UnitOfMeasurnment;
                    workItem.Income = entity.Income;
                    workItem.DirectContractUnitPrice = entity.DirectContractUnitPrice;
                    workItem.OutPutPerDay = entity.OutPutPerDay;
                    workItem.TotalDirectCost = entity.TotalDirectCost;

                    workItem.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;

                }

            }

            return Json(new[] { workItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceUtilizationWorkItem_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationWorkItem workItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationWorkItem
                {
                    ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
                    MethodOfWorkId = workItem.MethodOfWorkId,
                    WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                    ResourceUtilizationDayId = workItem.ResourceUtilizationDayId,
                    ItemName = workItem.ItemName,
                    IsSubCategory = workItem.IsSubCategory,
                    ParentId = workItem.ParentId,
                    //UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(workItem.WorkItemPerProjectId).Abbreviation,
                    UnitOfMeasurnment = workItem.UnitOfMeasurnment,
                    ExecutedWork = workItem.ExecutedWork,
                    WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
                    OutPutPerDay = workItem.OutPutPerDay,
                    //DirectContractUnitPrice = this.GetWorkItem(workItem.WorkItemPerProjectId).TotalUnitCost,
                    DirectContractUnitPrice = workItem.DirectContractUnitPrice,
                    Income = workItem.Income,
                    TotalDirectCost = workItem.TotalDirectCost,
                    //MaterialCost = this.GetMaterialCost(workItem.ResourceUtilizationWorkItemId),
                    //EquipmentCost = this.GetEquipmentCost(workItem.ResourceUtilizationWorkItemId),
                    //LaborCost = this.GetLaborCost(workItem.ResourceUtilizationWorkItemId),
                    MaterialCost = workItem.MaterialCost,
                    EquipmentCost = workItem.EquipmentCost,
                    LaborCost = workItem.LaborCost,
                    TotalCost = workItem.TotalCost,
                    ProfitCost = workItem.ProfitCost,
                    OverheadCost = workItem.OverheadCost,

                    SubProjectTypeName = workItem.SubProjectTypeName,
                    BlockName = workItem.BlockName,
                    StructureyType = workItem.StructureyType,
                    WorkCategory = workItem.WorkCategory,

                    OrgBranchName = workItem.OrgBranchName,
                    DateCreated = workItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = workItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
                entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
                entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;

                db.ResourceUtilizationWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //workItem.ResourceUtilizationWorkItemId = entity.ResourceUtilizationWorkItemId;
                workItem.WorkCategory = entity.WorkCategory;
                workItem.UnitOfMeasurnment = entity.UnitOfMeasurnment;
                workItem.Income = entity.Income;
                workItem.DirectContractUnitPrice = entity.DirectContractUnitPrice;
                workItem.OutPutPerDay = entity.OutPutPerDay;
                workItem.TotalDirectCost = entity.TotalDirectCost;
            }

            return Json(new[] { workItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceUtilizationWorkItem_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationWorkItem workItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationWorkItem
                {
                    ResourceUtilizationWorkItemId = workItem.ResourceUtilizationWorkItemId,
                    MethodOfWorkId = workItem.MethodOfWorkId,
                    WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                    ResourceUtilizationDayId = workItem.ResourceUtilizationDayId,
                    ItemName = workItem.ItemName,
                    IsSubCategory = workItem.IsSubCategory,
                    ParentId = workItem.ParentId,
                    UnitOfMeasurnment = workItem.UnitOfMeasurnment,
                    ExecutedWork = workItem.ExecutedWork,
                    WorkDurationInHoursPerDay = workItem.WorkDurationInHoursPerDay,
                    OutPutPerDay = workItem.OutPutPerDay,
                    DirectContractUnitPrice = workItem.DirectContractUnitPrice,
                    Income = workItem.Income,
                    MaterialCost = workItem.MaterialCost,
                    EquipmentCost = workItem.EquipmentCost,
                    LaborCost = workItem.LaborCost,
                    TotalDirectCost = workItem.TotalDirectCost,
                    TotalCost = workItem.TotalCost,
                    ProfitCost = workItem.ProfitCost,
                    OverheadCost = workItem.OverheadCost,

                    SubProjectTypeName = workItem.SubProjectTypeName,
                    BlockName = workItem.BlockName,
                    StructureyType = workItem.StructureyType,
                    WorkCategory = workItem.WorkCategory,

                    OrgBranchName = workItem.OrgBranchName,
                    DateCreated = workItem.DateCreated,
                    LastModified = workItem.LastModified,
                    CreatedBy = workItem.CreatedBy,
                    ModifiedBy = workItem.ModifiedBy
                };

                db.ResourceUtilizationWorkItem.Attach(entity);
                db.ResourceUtilizationWorkItem.Remove(entity);
                db.SaveChanges();

                this.DeleteUtilizationResources(workItem.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { workItem }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public JsonResult CopyResourceFromEstimationToUtilization(int id, int destinationItemId) {
            var materials = db.ProjectMaterial.Where(m => m.WorkItemPerProjectId == id).ToList();
            var equipments = db.ProjectEquipment.Where(e => e.WorkItemPerProjectId == id).ToList();
            var labors = db.ProjectDirectLabour.Where(l => l.WorkItemPerProjectId == id).ToList();

            if (materials.Any()) {
                foreach(ProjectMaterial material in materials)
                {
                    var existingMa = db.ResourceUtilizationMaterials.FirstOrDefault(m => m.ItemCode == material.ItemCode && m.ResourceUtilizationWorkItemId == destinationItemId);
                    if(existingMa == null)
                    {
                        var mtUtl = new ResourceUtilizationMaterial
                        {
                            ItemCategoryCode = material.ItemCategoryCode,
                            ItemCode = material.ItemCode,
                            ResourceUtilizationWorkItemId = destinationItemId,
                            Unit = material.Unit,
                            UnitPrice = material.UnitPrice,
                            Quantity = 0,
                            TotalCost = 0,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            
                        };
                        db.ResourceUtilizationMaterials.Add(mtUtl);
                        db.SaveChanges();
                    }
                    
                }
            }
            if (equipments.Any()) {
                foreach(ProjectEquipment equipment in equipments)
                {
                    var existingEq = db.ResourceUtilizationEquipment.FirstOrDefault(m => m.FleetsID == equipment.FleetsID && m.ResourceUtilizationWorkItemId == destinationItemId);
                    if(existingEq == null)
                    {
                        var eqUtl = new ResourceUtilizationEquipment
                        {
                            FleetsID = equipment.FleetsID,
                            Quantity = 0,
                            TotalCost = 0,
                            Unit = equipment.Unit,
                            UnitFactory = equipment.UnitFactory,
                            EquipmentType = equipment.EquipmentType,
                            EquipmentUnitCost = equipment.EquipmentUnitCost,
                            RentalCost = equipment.RentalCost,
                            ResourceUtilizationWorkItemId = destinationItemId,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                        };
                        db.ResourceUtilizationEquipment.Add(eqUtl);
                        db.SaveChanges();
                    }
                    
                }
            }
            if (labors.Any()) {
                foreach(ProjectDirectLabour labor in labors)
                {
                    var existingLab = db.ResourceUtilizationDirectLabours.FirstOrDefault(m => m.ProjectEstimationIndexLabourId == labor.ProjectEstimationIndexLabourId && m.ResourceUtilizationWorkItemId == destinationItemId);
                    if(existingLab == null)
                    {
                        var lbrUtl = new ResourceUtilizationDirectLabour
                        {
                            IndexedHourlySalary = labor.IndexedHourlySalary,
                            ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                            Quantity = 0,
                            ResourceUtilizationWorkItemId = destinationItemId,
                            UnitFactory = labor.UnitFactory,
                            TotalCost = 0,
                            Unit = labor.Unit,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                        };
                        db.ResourceUtilizationDirectLabours.Add(lbrUtl);
                        db.SaveChanges();
                    }
                }
            }
            var data = new
            {
                Message = "Copy Successful!"
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        #region inner methods

        private bool ValueExists(MonthlyFuelCost model)
        {
            bool monthFuelCostExists = false;
            var list = db.MonthlyFuelCost.ToList();
            foreach (var item in list)
            {
                if (item.Month == model.Month && item.FuelType == model.FuelType && item.Year == model.Year)
                {
                    monthFuelCostExists = true;
                }
            }
            return monthFuelCostExists;
        }
        private void DeleteUtilizationResources(int uWorkItemId)
        {
            var database = new ExceedDbContext();
            var materialResource = database.ResourceUtilizationMaterials.Where(m => m.ResourceUtilizationWorkItemId == uWorkItemId);
            if (materialResource.Any())
            {
                database.ResourceUtilizationMaterials.RemoveRange(materialResource);
                database.SaveChanges();
            }
            var equipmentResource = database.ResourceUtilizationEquipment.Where(e => e.ResourceUtilizationWorkItemId == uWorkItemId);
            if (equipmentResource.Any())
            {
                database.ResourceUtilizationEquipment.RemoveRange(equipmentResource);
                database.SaveChanges();
            }
            var laborResource = database.ResourceUtilizationDirectLabours.Where(l => l.ResourceUtilizationWorkItemId == uWorkItemId);
            if (laborResource.Any())
            {
                database.ResourceUtilizationDirectLabours.RemoveRange(laborResource);
                database.SaveChanges();
            }

        }

        /// <summary>
        /// this is used for updating the resource utilization total direct cost 
        /// after any resource is added, modified or removed 
        /// </summary>
        /// <param name="id"></param>
        public void UpdateResourceUtilizationTotalDirectCostOfWorkItem(int id)
        {
            var database = new ExceedDbContext();
            var currentWorkItem = database.ResourceUtilizationWorkItem.Find(id);
            if (currentWorkItem != null)
            {
                var entity = new ResourceUtilizationWorkItem
                {
                    ResourceUtilizationWorkItemId = currentWorkItem.ResourceUtilizationWorkItemId,
                    MethodOfWorkId = currentWorkItem.MethodOfWorkId,
                    WorkItemPerProjectId = currentWorkItem.WorkItemPerProjectId,
                    ResourceUtilizationDayId = currentWorkItem.ResourceUtilizationDayId,
                    ItemName = currentWorkItem.ItemName,
                    UnitOfMeasurnment = this.GetWorkItemUnitOfMeasurnment(currentWorkItem.WorkItemPerProjectId).Abbreviation,
                    ExecutedWork = currentWorkItem.ExecutedWork,
                    WorkDurationInHoursPerDay = currentWorkItem.WorkDurationInHoursPerDay,
                    OutPutPerDay = currentWorkItem.OutPutPerDay,
                    DirectContractUnitPrice = this.GetWorkItem(currentWorkItem.WorkItemPerProjectId).TotalUnitCost,
                    Income = currentWorkItem.Income,
                    MaterialCost = this.GetMaterialCost(currentWorkItem.ResourceUtilizationWorkItemId),
                    EquipmentCost = this.GetEquipmentCost(currentWorkItem.ResourceUtilizationWorkItemId),
                    LaborCost = this.GetLaborCost(currentWorkItem.ResourceUtilizationWorkItemId),
                    TotalCost = currentWorkItem.TotalCost,
                    ProfitCost = currentWorkItem.ProfitCost,
                    OverheadCost = currentWorkItem.OverheadCost,

                    IsSubCategory = currentWorkItem.IsSubCategory,
                    ParentId = currentWorkItem.ParentId,

                    SubProjectTypeName = currentWorkItem.SubProjectTypeName,
                    BlockName = currentWorkItem.BlockName,
                    StructureyType = currentWorkItem.StructureyType,
                    WorkCategory = currentWorkItem.WorkCategory,

                    OrgBranchName = currentWorkItem.OrgBranchName,
                    DateCreated = currentWorkItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = currentWorkItem.CreatedBy,
                    ModifiedBy = User == null ? currentWorkItem.ModifiedBy : User.Identity.Name
                };

                entity.Income = entity.ExecutedWork * entity.DirectContractUnitPrice;
                entity.OutPutPerDay = entity.ExecutedWork / (entity.WorkDurationInHoursPerDay / 8);
                entity.TotalDirectCost = entity.MaterialCost + entity.EquipmentCost + entity.LaborCost;
                entity.TotalCost = entity.TotalDirectCost + entity.ProfitCost + entity.OverheadCost;

                db.ResourceUtilizationWorkItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private decimal GetMaterialCost(int id)
        {
            decimal matCost = new decimal();
            var materials = db.ResourceUtilizationMaterials.Where(m => m.ResourceUtilizationWorkItemId == id).ToList();
            if (materials.Any())
            {
                matCost = materials.Sum(m => m.TotalCost);
            }
            return matCost;
        }
        private decimal GetEquipmentCost(int id)
        {
            decimal equCost = new decimal();
            var equipments = db.ResourceUtilizationEquipment.Where(m => m.ResourceUtilizationWorkItemId == id).ToList();
            if (equipments.Any())
            {
                equCost = equipments.Sum(e => e.TotalCost);
            }
            return equCost;
        }
        private decimal GetLaborCost(int id)
        {
            decimal laborCost = new decimal();
            var labors = db.ResourceUtilizationDirectLabours.Where(dl => dl.ResourceUtilizationWorkItemId == id).ToList();
            if (labors.Any())
            {
                laborCost = labors.Sum(m => m.TotalCost);
            }
            return laborCost;
        }
        #endregion
    }
}