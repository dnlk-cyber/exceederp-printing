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
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    [AuthorizeRoles(ConstructionConstants.EstimationUser)]
    public class ProjectEstimationSplitController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        ConstructionInputs inputs = new ConstructionInputs();
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            ViewData["SubProjectTypes"] = db.SubProjectType.Select(c => new { c.SubProjectTypeId, c.Name}).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.BranchId, b.Name}).ToList();

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            ViewData["Categories"] = db.WorkCategory.Select(w => new { w.Name, w.ConfigurationId, w.WorkCategoryId}).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId}).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x =>new{x.UnitOfMeasurementId,x.Abbreviation}).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new {  p.ItemCategoryCode,  p.Name });

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

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });

            // temporry script
            this.UpdateNullWorkItemName();

            return View();
        }
        public ActionResult UpdateLatestChanges()
        {
            this.UpdateChangedInformation();
            return Redirect("Index");
        }
        public ActionResult Project(int p)
        {

            db.Configuration.ProxyCreationEnabled = false;

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            ViewData["SubProjectTypes"] = db.SubProjectType.Select(c => new { c.SubProjectTypeId, c.Name }).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            ViewData["Categories"] = db.WorkCategory.Select(w => new { w.Name, w.ConfigurationId, w.WorkCategoryId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name });

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

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });


            ViewBag.Project = "";
            ViewBag.ProjectId = p;
            var currentProject = db.ProjectInformation.Find(p);
            if (currentProject != null)
            {
                ViewBag.Project = currentProject.Name;
            }

            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => !x.IsDeleted);
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Create([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
                {
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
                    DateCreated = DateTime.Now,
                    LastModified = projectInformation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectInformation.ModifiedBy,
                    ProjectStatus = ProjectStatus.ESTIMATION

                };

                db.ProjectInformation.Add(entity);
                db.SaveChanges();
                projectInformation.ProjectInformationId = entity.ProjectInformationId;
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Update([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
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
                    LastModified = DateTime.Now,
                    CreatedBy = projectInformation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ProjectStatus = projectInformation.ProjectStatus
                };

                db.ProjectInformation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Destroy([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectInformation
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
                };

                db.ProjectInformation.Attach(entity);
                db.ProjectInformation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult WorkItem_Read([DataSourceRequest] DataSourceRequest request, int currentProjectId)
        {
            var workItems =
                db.WorkItemPerProject.Where(x => x.ProjectInformationId == currentProjectId && x.ParentId == 0);
            var workItemsToReturn = new List<WorkItemPerProjectViewModel>();
            foreach (WorkItemPerProject workItemPerProject in workItems)
            {
                var record = new WorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItemPerProject.BlockId),
                    BlockName = ProjectEstimationHelper.GetBlockName(workItemPerProject.BlockId),
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " "+ workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.WorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    ActualQuantityOfWork = workItemPerProject.ActualQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                    ProjectType = workItemPerProject.ProjectType,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<WorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ChildWorkItem_Read([DataSourceRequest] DataSourceRequest request, int parentWorkItemId)
        {
            var workItems =
                db.WorkItemPerProject.Where(x => x.ParentId == parentWorkItemId);
            var workItemsToReturn = new List<WorkItemPerProjectViewModel>();
            foreach (WorkItemPerProject workItemPerProject in workItems)
            {
                var record = new WorkItemPerProjectViewModel
                {
                    SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItemPerProject.BlockId),
                    BlockName = ProjectEstimationHelper.GetBlockName(workItemPerProject.BlockId),
                    StructureyType = workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Name,
                    IsSubCategory = workItemPerProject.IsSubCategory,
                    ContainsWorkItemAsInput = workItemPerProject.ContainsWorkItemAsInput,
                    ItemCode = workItemPerProject.ItemCode,
                    ItemName = workItemPerProject.ItemName,
                    Id = workItemPerProject.WorkItemPerProjectId,
                    TotalQuantityOfWork = workItemPerProject.TotalQuantityOfWork,
                    ActualQuantityOfWork = workItemPerProject.ActualQuantityOfWork,
                    DirectCost = workItemPerProject.DirectCost,
                    MaterialCost = workItemPerProject.MaterialCost,
                    LabourOutPut = workItemPerProject.LabourOutPut,
                    EquipmentOutput = workItemPerProject.EquipmentOutput,
                    MaterialCostPerUnit = workItemPerProject.MaterialCostPerUnit,
                    EquipmentCostPerUnit = workItemPerProject.EquipmentCostPerUnit,
                    LaborCostPerUnit = workItemPerProject.LaborCostPerUnit,
                    TotalUnitCost = workItemPerProject.TotalUnitCost,
                    OverHeadCost = workItemPerProject.OverHeadCost,
                    ProfitCost = workItemPerProject.ProfitCost,
                    Remark = workItemPerProject.Remark,
                    LabourHourlyOutPut = workItemPerProject.LabourHourlyOutPut,
                    UnitOfMeasurement = ConsSettingHelper.GetUnitOfMearurnment(workItemPerProject.UnitOfMeasurementId ?? 0),
                    OverheadRate = workItemPerProject.OverheadRate,
                    ProfitMargin = workItemPerProject.ProfitMargin,
                    ProjectType = workItemPerProject.ProjectType,
                };
                workItemsToReturn.Add(record);
            }
            IQueryable<WorkItemPerProjectViewModel> workItemQueryable = workItemsToReturn.AsQueryable();

            return Json(workItemQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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
        private void UpdateNullWorkItemName()
        {
            var workItemWithNullName = db.WorkItemPerProject.Where(w => w.ItemName == null).ToList();
            if (workItemWithNullName.Any())
            {
                foreach (var workItem in workItemWithNullName)
                {
                    workItem.ItemName = ProjectEstimationHelper.GetItemNameFromEstimationWorkItem(workItem.ProjectEstimationWorkItemId);

                    db.WorkItemPerProject.Attach(workItem);
                    db.Entry(workItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            //}
        }
        public void CopyCurrentTemplateWorkItem(int workItemId, int blockMappingId)
        {
            var currentWorkItem = db.ProjectEstimationWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                var bmapping = db.BlockMappings.Find(blockMappingId);
                if (bmapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.Blocks.Find(bmapping.BlockId);
                    if (currentSubProject != null)
                    {
                        var workItemToAdd = new WorkItemPerProject
                        {
                            BlockId = currentSubProject.BlockId,
                            ItemCode = currentWorkItem.ItemCode,
                            IsSubCategory = currentWorkItem.IsSubCategory,
                            ContainsWorkItemAsInput = currentWorkItem.ContainsWorkItemAsInput,
                            ItemName = currentWorkItem.ItemName,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,

                            ProjectEstimationWorkItemId = currentWorkItem.ProjectEstimationWorkItemId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,

                            TotalQuantityOfWork = currentWorkItem.TotalQuantityOfWork,
                            DirectCost = currentWorkItem.DirectCost,
                            MaterialCost = currentWorkItem.MaterialCost,
                            LabourOutPut = currentWorkItem.LabourOutPut,
                            EquipmentOutput = currentWorkItem.EquipmentOutput,
                            TotalUnitCost = currentWorkItem.TotalUnitCost,

                            OverHeadCost = currentWorkItem.OverHeadCost,
                            ProfitCost = currentWorkItem.ProfitCost,
                            ProjectType = currentWorkItem.ProjectType,
                            Remark = currentWorkItem.Remark,
                            LabourHourlyOutPut = currentWorkItem.LabourHourlyOutPut,
                            UnitOfMeasurementId = currentWorkItem.UnitOfMeasurementId,
                            OverheadRate = currentWorkItem.OverheadRate,
                            ProfitMargin = currentWorkItem.ProfitMargin,

                            ParentId = currentWorkItem.ParentId,

                            OrgBranchName = currentWorkItem.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = currentWorkItem.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = currentWorkItem.ModifiedBy,
                        };
                        db.WorkItemPerProject.Add(workItemToAdd);
                        db.SaveChanges();

                        var id = workItemToAdd.WorkItemPerProjectId;

                        if (currentWorkItem.IsSubCategory)
                        {
                            var childWorkItems = db.ProjectEstimationWorkItem
                                .Where(wi => wi.ParentId == currentWorkItem.ProjectEstimationWorkItemId).ToList();
                            if (childWorkItems.Any())
                            {
                                foreach (ProjectEstimationWorkItem childWorkItem in childWorkItems)
                                {
                                    var childWorkItemToAdd = new WorkItemPerProject
                                    {
                                        BlockId = currentSubProject.BlockId,
                                        ItemCode = childWorkItem.ItemCode,
                                        IsSubCategory = childWorkItem.IsSubCategory,
                                        ContainsWorkItemAsInput = childWorkItem.ContainsWorkItemAsInput,
                                        ItemName = childWorkItem.ItemName,
                                        WorkCategoryId = childWorkItem.WorkCategoryId,

                                        ProjectEstimationWorkItemId = childWorkItem.ProjectEstimationWorkItemId,
                                        ProjectInformationId = currentSubProject.ProjectInformationId,

                                        TotalQuantityOfWork = childWorkItem.TotalQuantityOfWork,
                                        DirectCost = childWorkItem.DirectCost,
                                        MaterialCost = childWorkItem.MaterialCost,
                                        LabourOutPut = childWorkItem.LabourOutPut,
                                        EquipmentOutput = childWorkItem.EquipmentOutput,
                                        TotalUnitCost = childWorkItem.TotalUnitCost,
                                        OverHeadCost = childWorkItem.OverHeadCost,
                                        ProfitCost = childWorkItem.ProfitCost,
                                        ProjectType = childWorkItem.ProjectType,
                                        Remark = childWorkItem.Remark,
                                        LabourHourlyOutPut = childWorkItem.LabourHourlyOutPut,
                                        UnitOfMeasurementId = childWorkItem.UnitOfMeasurementId,
                                        OverheadRate = childWorkItem.OverheadRate,
                                        ProfitMargin = childWorkItem.ProfitMargin,

                                        ParentId = id,

                                        OrgBranchName = childWorkItem.OrgBranchName,
                                        DateCreated = DateTime.Now,
                                        LastModified = childWorkItem.LastModified,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = childWorkItem.ModifiedBy,
                                    };
                                    db.WorkItemPerProject.Add(childWorkItemToAdd);
                                    db.SaveChanges();

                                    var childId = childWorkItemToAdd.WorkItemPerProjectId;
                                    // copy material, equipment and labor from templae work item
                                    CopyWorkItemResourceFromTemplate(childWorkItem.ProjectEstimationWorkItemId, childId);
                                }
                            }
                        }
                        // current work item is not a sub-category ...
                        // so it probably has material, labor and equipment
                        else
                        {
                            CopyWorkItemResourceFromTemplate(currentWorkItem.ProjectEstimationWorkItemId, id);
                        }
                    }
                }
            }
        }
        private void CopyWorkItemResourceFromTemplate(int templateId, int destinationId)
        {
            var materials = db.ProjectEstimationMaterial
                .Where(m => m.ProjectEstimationWorkItemId == templateId).ToList();
            var equipments = db.ProjectEstimationEquipment
                .Where(eq => eq.ProjectEstimationWorkItemId == templateId).ToList();
            var labors = db.ProjectEstimationDirectLabour
                .Where(l => l.ProjectEstimationWorkItemId == templateId).ToList();

            #region copy material
            if (materials.Any())
            {
                foreach (ProjectEstimationMaterial material in materials)
                {
                    var materialToAdd = new ProjectMaterial
                    {
                        ItemCode = material.ItemCode,
                        ItemCategoryCode = material.ItemCategoryCode,
                        Unit = material.Unit,
                        Quantity = material.Quantity,
                        UnitPrice = material.UnitPrice,
                        TotalCost = material.TotalCost,
                        MaterialUnitCost = material.MaterialUnitCost,
                        WorkItemPerProjectId = destinationId,

                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy,
                    };
                    db.ProjectMaterial.Add(materialToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy labor
            if (labors.Any())
            {
                foreach (ProjectEstimationDirectLabour labor in labors)
                {
                    var laborToAdd = new ProjectDirectLabour
                    {
                        Unit = labor.Unit,
                        Quantity = labor.Quantity,
                        UnitFactory = labor.UnitFactory,
                        IndexedHourlySalary = labor.IndexedHourlySalary,
                        DailyPay = labor.DailyPay,
                        TotalCost = labor.TotalCost,
                        ManpowerUnitCost = labor.ManpowerUnitCost,
                        WorkItemPerProjectId = destinationId,
                        ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,

                        OrgBranchName = labor.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = labor.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = labor.ModifiedBy,

                    };
                    db.ProjectDirectLabour.Add(laborToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy equipment
            if (equipments.Any())
            {
                foreach (ProjectEstimationEquipment equipment in equipments)
                {
                    var equipmentToAdd = new ProjectEquipment
                    {
                        FleetsID = equipment.FleetsID,
                        Unit = equipment.Unit,
                        Quantity = equipment.Quantity,
                        UnitFactory = equipment.UnitFactory,
                        RentalCost = equipment.RentalCost,
                        TotalCost = equipment.TotalCost,
                        EquipmentUnitCost = equipment.EquipmentUnitCost,
                        EquipmentType = equipment.EquipmentType,
                        WorkItemPerProjectId = destinationId,

                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = equipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = equipment.ModifiedBy,
                    };
                    db.ProjectEquipment.Add(equipmentToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
        }
        public ActionResult CleanAndRearangeBlocks()
        {
            var db = new ExceedDbContext();

            var tasks = db.Tasks.ToList();
            var taskDetail = db.TaskDetail.ToList();

            var workItems = db.WorkItemPerProject.ToList();
            var pcRates = db.AreaWorkItemes.ToList();
            var scworkItems = db.SubContractWorkItem.ToList();
            var customWorkItems = db.ProjectCustomWorkItems.ToList();

            var utlWorkItems = db.ResourceUtilizationWorkItem.ToList();

            var blkMapping = db.BlockMappings.ToList();
            var bloks = db.Blocks.ToList();

            var durationWorkItem = db.WorkItemDuration.ToList();


            db.TaskDetail.RemoveRange(taskDetail);
            db.SaveChanges();

            db.WorkItemDuration.RemoveRange(durationWorkItem);
            db.SaveChanges();

            db.Tasks.RemoveRange(tasks);
            db.SaveChanges();

            db.ResourceUtilizationWorkItem.RemoveRange(utlWorkItems);
            db.SaveChanges();

            db.WorkItemPerProject.RemoveRange(workItems);
            db.AreaWorkItemes.RemoveRange(pcRates);
            db.SubContractWorkItem.RemoveRange(scworkItems);
            db.ProjectCustomWorkItems.RemoveRange(customWorkItems);
            db.SaveChanges();

            db.BlockMappings.RemoveRange(blkMapping);
            db.SaveChanges();

            db.Blocks.RemoveRange(bloks);
            db.SaveChanges();

            SplitAndAddData();
            return Redirect("Index");
        }
        private void SplitAndAddData()
        {
            var projects = db.ProjectInformation.ToList();
            if (projects.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var counter = 1;
                    var subProjects = db.SubProjectType.Where(sb => sb.ProjectInformationId == project.ProjectInformationId).ToList();
                    if (subProjects.Any())
                    {
                        var totalBlocksInProject = subProjects.Sum(s => s.NumberOfBlocks);
                        foreach (SubProjectType subProject in subProjects)
                        {
                            if (project.ProjectType == AvailableProjectTypes.Building)
                            {
                                for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                                {
                                    var blockToAdd = new Block
                                    {
                                        Name = "B-" + counter,
                                        ProjectInformationId = project.ProjectInformationId,
                                        SubProjectTypeId = subProject.SubProjectTypeId,

                                    };
                                    db.Blocks.Add(blockToAdd);
                                    db.SaveChanges();
                                    counter = counter + 1;

                                    var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                    if (subProjectMappings.Any())
                                    {
                                        foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                        {
                                            var blockMappingToAdd = new BlockMapping
                                            {
                                                BlockId = blockToAdd.BlockId,
                                                ConfigurationId = mapping.ConfigurationId,
                                                Remark = mapping.Remark,
                                            };
                                            db.BlockMappings.Add(blockMappingToAdd);
                                            db.SaveChanges();

                                        }
                                    }
                                    AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                                }
                            }
                            else if (project.ProjectType == AvailableProjectTypes.Road)
                            {
                                for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                                {
                                    var blockToAdd = new Block
                                    {
                                        Name = "R-" + counter,
                                        ProjectInformationId = project.ProjectInformationId,
                                        SubProjectTypeId = subProject.SubProjectTypeId,
                                    };
                                    db.Blocks.Add(blockToAdd);
                                    db.SaveChanges();
                                    counter = counter + 1;

                                    var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                    if (subProjectMappings.Any())
                                    {
                                        foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                        {
                                            var blockMappingToAdd = new BlockMapping
                                            {
                                                BlockId = blockToAdd.BlockId,
                                                ConfigurationId = mapping.ConfigurationId,
                                                Remark = mapping.Remark,
                                            };
                                            db.BlockMappings.Add(blockMappingToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                                }
                            }
                        }
                    }

                    this.UpdateWorkItemsHeirarchy(project.ProjectInformationId);
                }
            }
        }

        public JsonResult SplitProjectAndAddWorkItemsToSpecificBlocks(int projectId)
        {
            //var projects = db.ProjectInformation.ToList();
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                //foreach (ProjectInformation project in projects)
                //{
                #region remove existing data
                var tasks = db.Tasks.Where(t => t.WorkItemDuration.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                //var taskDetail = db.TaskDetail.ToList();

                var workItems = db.WorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var pcRates = db.AreaWorkItemes.Where(p => p.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var scworkItems = db.SubContractWorkItem.Where(sc => sc.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var customWorkItems = db.ProjectCustomWorkItems.Where(c => c.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var ids = new HashSet<int>(workItems.Select(w => w.WorkItemPerProjectId));
                var customContents = db.ProjectCustomWorkItemDetails.Where(c => ids.Contains(c.WorkItemPerProjectId)).ToList();

                var utlWorkItems = db.ResourceUtilizationWorkItem.Where(ru => ru.WorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                var blkMapping = db.BlockMappings.Where(bm => bm.Block.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var bloks = db.Blocks.Where(b => b.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                //var durationWorkItem = db.WorkItemDuration.Where(wd => wd.GenericWorkItemPerProject.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                var durationWorkItem = db.WorkItemDuration.Where(wd => wd.ProjectInformationId == currentProject.ProjectInformationId).ToList();


                //db.TaskDetail.RemoveRange(taskDetail);
                //db.SaveChanges();

                db.WorkItemDuration.RemoveRange(durationWorkItem);
                db.SaveChanges();

                db.Tasks.RemoveRange(tasks);
                db.SaveChanges();

                db.ResourceUtilizationWorkItem.RemoveRange(utlWorkItems);
                db.SaveChanges();

                db.WorkItemPerProject.RemoveRange(workItems);
                db.ProjectCustomWorkItemDetails.RemoveRange(customContents);
                db.AreaWorkItemes.RemoveRange(pcRates);
                db.SubContractWorkItem.RemoveRange(scworkItems);
                db.ProjectCustomWorkItems.RemoveRange(customWorkItems);
                db.SaveChanges();

                db.BlockMappings.RemoveRange(blkMapping);
                db.SaveChanges();

                db.Blocks.RemoveRange(bloks);
                db.SaveChanges();

                #endregion
                var counter = 1;
                var subProjects = db.SubProjectType.Where(sb => sb.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (subProjects.Any())
                {
                    var totalBlocksInProject = subProjects.Sum(s => s.NumberOfBlocks);
                    foreach (SubProjectType subProject in subProjects)
                    {
                        if (currentProject.ProjectType == AvailableProjectTypes.Building)
                        {
                            for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                            {
                                var blockToAdd = new Block
                                {
                                    Name = "B-" + counter,
                                    ProjectInformationId = currentProject.ProjectInformationId,
                                    SubProjectTypeId = subProject.SubProjectTypeId,

                                };
                                db.Blocks.Add(blockToAdd);
                                db.SaveChanges();
                                counter = counter + 1;

                                var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                if (subProjectMappings.Any())
                                {
                                    foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                    {
                                        var blockMappingToAdd = new BlockMapping
                                        {
                                            BlockId = blockToAdd.BlockId,
                                            ConfigurationId = mapping.ConfigurationId,
                                            Remark = mapping.Remark,
                                        };
                                        db.BlockMappings.Add(blockMappingToAdd);
                                        db.SaveChanges();

                                    }
                                }
                                AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                            }
                        }
                        else if (currentProject.ProjectType == AvailableProjectTypes.Road)
                        {
                            for (int i = 1; i <= subProject.NumberOfBlocks; i++)
                            {
                                var blockToAdd = new Block
                                {
                                    Name = "R-" + counter,
                                    ProjectInformationId = currentProject.ProjectInformationId,
                                    SubProjectTypeId = subProject.SubProjectTypeId,
                                };
                                db.Blocks.Add(blockToAdd);
                                db.SaveChanges();
                                counter = counter + 1;

                                var subProjectMappings = db.SubProjectStructureTypeMapping.Where(m => m.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                                if (subProjectMappings.Any())
                                {
                                    foreach (SubProjectStructureTypeMapping mapping in subProjectMappings)
                                    {
                                        var blockMappingToAdd = new BlockMapping
                                        {
                                            BlockId = blockToAdd.BlockId,
                                            ConfigurationId = mapping.ConfigurationId,
                                            Remark = mapping.Remark,
                                        };
                                        db.BlockMappings.Add(blockMappingToAdd);
                                        db.SaveChanges();
                                    }
                                }
                                AddWorkItemFromGenericEstimation(subProject.SubProjectTypeId, blockToAdd.BlockId);
                            }
                        }
                    }
                    var formattedData = new
                    {
                        Message = "Sync Successful!",
                        Status = "OK"
                    };
                    UpdateWorkItemsHeirarchy(currentProject.ProjectInformationId);

                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }

                //}
            }
            return null;
        }

        private void AddWorkItemFromGenericEstimation(int subProjectId, int blockId)
        {
            #region work items
            var genericWorkItems = db.GenericWorkItemPerProject.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericWorkItems.Any())
            {
                foreach (GenericWorkItemPerProject gWorkItem in genericWorkItems)
                {
                    var workItemToAdd = new WorkItemPerProject
                    {
                        ProjectEstimationWorkItemId = gWorkItem.ProjectEstimationWorkItemId,
                        IsSubCategory = gWorkItem.IsSubCategory,
                        ItemCode = gWorkItem.ItemCode,
                        ContainsWorkItemAsInput = gWorkItem.ContainsWorkItemAsInput,
                        GenericWorkItemPerProjectId = gWorkItem.GenericWorkItemPerProjectId,
                        ProjectInformationId = gWorkItem.ProjectInformationId,
                        BlockId = blockId,

                        ItemName = gWorkItem.ItemName,
                        TotalQuantityOfWork = gWorkItem.TotalQuantityOfWork,
                        DirectCost = gWorkItem.DirectCost,
                        MaterialCost = gWorkItem.MaterialCost,
                        LabourOutPut = gWorkItem.LabourOutPut,
                        EquipmentOutput = gWorkItem.EquipmentOutput,
                        MaterialCostPerUnit = gWorkItem.MaterialCostPerUnit,
                        LaborCostPerUnit = gWorkItem.LaborCostPerUnit,
                        EquipmentCostPerUnit = gWorkItem.EquipmentCostPerUnit,
                        TotalUnitCost = gWorkItem.TotalUnitCost,
                        HasAdditionalTransportationCost = gWorkItem.HasAdditionalTransportationCost,
                        CalculatedTransportCost = gWorkItem.CalculatedTransportCost,
                        TransportCostRate = gWorkItem.TransportCostRate,
                        TotalUnitCostIncludingTransport = gWorkItem.TotalUnitCostIncludingTransport,
                        OverHeadCost = gWorkItem.OverHeadCost,
                        ProfitCost = gWorkItem.ProfitCost,
                        Remark = gWorkItem.Remark,
                        LabourHourlyOutPut = gWorkItem.LabourHourlyOutPut,
                        UnitOfMeasurementId = gWorkItem.UnitOfMeasurementId,
                        OverheadRate = gWorkItem.OverheadRate,
                        ProfitMargin = gWorkItem.ProfitMargin,

                        OrgBranchName = gWorkItem.OrgBranchName,
                        DateCreated = gWorkItem.DateCreated,
                        LastModified = gWorkItem.LastModified,
                        CreatedBy = gWorkItem.CreatedBy,
                        ModifiedBy = gWorkItem.ModifiedBy,

                        ParentId = gWorkItem.ParentId,
                        ProjectType = gWorkItem.ProjectType,
                        WorkCategoryId = gWorkItem.WorkCategoryId
                    };
                    db.WorkItemPerProject.Add(workItemToAdd);
                    db.SaveChanges();

                    #region copy resources

                    if (gWorkItem.ContainsWorkItemAsInput)
                    {
                        var customDetails = db.GenericProjectCustomWorkItemDetails.Where(w => w.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                        if (customDetails.Any())
                        {
                            foreach (GenericProjectCustomWorkItemDetail sourceDetail in customDetails)
                            {
                                var recordDetail = new ProjectCustomWorkItemDetail
                                {
                                    ProjectEstimationWorkItemId = sourceDetail.ProjectEstimationWorkItemId,
                                    WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId,
                                    UnitCost = sourceDetail.UnitCost,
                                    QuantityPerUnit = sourceDetail.QuantityPerUnit,
                                    TotalCost = sourceDetail.TotalCost,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,
                                    LastModified = DateTime.Now,
                                    ModifiedBy = User.Identity.Name,
                                    OrgBranchName = sourceDetail.OrgBranchName,

                                };
                                db.ProjectCustomWorkItemDetails.Add(recordDetail);
                                db.SaveChanges();
                            }
                        }
                    }


                    #region materials
                    var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (materials.Any())
                    {
                        foreach (GenericProjectMaterial material in materials)
                        {
                            var materialToAdd = new ProjectMaterial
                            {
                                ItemCode = material.ItemCode,
                                ItemCategoryCode = material.ItemCategoryCode,
                                Unit = material.Unit,
                                Quantity = material.Quantity,
                                UnitPrice = material.UnitPrice,
                                TotalCost = material.TotalCost,
                                MaterialUnitCost = material.MaterialUnitCost,
                                OrgBranchName = material.OrgBranchName,
                                DateCreated = material.DateCreated,
                                LastModified = material.LastModified,
                                CreatedBy = material.CreatedBy,
                                ModifiedBy = material.ModifiedBy,
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId
                            };
                            db.ProjectMaterial.Add(materialToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #region direct labor
                    var labors = db.GenericProjectDirectLabour.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (labors.Any())
                    {
                        foreach (GenericProjectDirectLabour labor in labors)
                        {
                            var laborToAdd = new ProjectDirectLabour
                            {
                                Unit = labor.Unit,
                                Quantity = labor.Quantity,
                                UnitFactory = labor.UnitFactory,
                                IndexedHourlySalary = labor.IndexedHourlySalary,
                                DailyPay = labor.DailyPay,
                                TotalCost = labor.TotalCost,
                                ManpowerUnitCost = labor.ManpowerUnitCost,
                                OrgBranchName = labor.OrgBranchName,
                                DateCreated = labor.DateCreated,
                                LastModified = labor.LastModified,
                                CreatedBy = labor.CreatedBy,
                                ModifiedBy = labor.ModifiedBy,
                                ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId
                            };
                            db.ProjectDirectLabour.Add(laborToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #region equipment
                    var equipments = db.GenericProjectEquipment.Where(m => m.GenericWorkItemPerProjectId == gWorkItem.GenericWorkItemPerProjectId).ToList();
                    if (equipments.Any())
                    {
                        foreach (GenericProjectEquipment projectEquipment in equipments)
                        {
                            var equipmentToAdd = new ProjectEquipment
                            {
                                WorkItemPerProjectId = workItemToAdd.WorkItemPerProjectId,
                                FleetsID = projectEquipment.FleetsID,
                                Unit = projectEquipment.Unit,
                                Quantity = projectEquipment.Quantity,
                                UnitFactory = projectEquipment.UnitFactory,
                                RentalCost = projectEquipment.RentalCost,
                                TotalCost = projectEquipment.TotalCost,
                                EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                                EquipmentType = projectEquipment.EquipmentType,

                                OrgBranchName = projectEquipment.OrgBranchName,
                                DateCreated = projectEquipment.DateCreated,
                                LastModified = projectEquipment.LastModified,
                                CreatedBy = projectEquipment.CreatedBy,
                                ModifiedBy = projectEquipment.ModifiedBy,
                            };
                            db.ProjectEquipment.Add(equipmentToAdd);
                            db.SaveChanges();
                        }
                    }
                    #endregion
                    #endregion
                }
            }
            #endregion
            #region piece rates
            var genericPieceRates = db.GenericAreaWorkItem.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericPieceRates.Any())
            {
                foreach (GenericAreaWorkItem pieceRate in genericPieceRates)
                {
                    var pieceRateToAdd = new AreaWorkItem
                    {
                        WorkCategoryId = pieceRate.WorkCategoryId,
                        Price = pieceRate.Price,
                        WorkItemName = pieceRate.WorkItemName,
                        ItemCode = pieceRate.ItemCode,
                        DirectCost = pieceRate.DirectCost,
                        Width = pieceRate.Width,
                        Height = pieceRate.Height,
                        Area = pieceRate.Area,
                        CylinderLock = pieceRate.CylinderLock,
                        OverHeadProfitCost = pieceRate.OverHeadProfitCost,
                        ProjectInformationId = pieceRate.ProjectInformationId,
                        MaterialCost = pieceRate.MaterialCost,
                        TransportCost = pieceRate.TransportCost,
                        GenericAreaWorkItemId = pieceRate.GenericAreaWorkItemId,
                        BlockId = blockId,

                        OrgBranchName = pieceRate.OrgBranchName,
                        DateCreated = pieceRate.DateCreated,
                        LastModified = pieceRate.LastModified,
                        CreatedBy = pieceRate.CreatedBy,
                        ModifiedBy = pieceRate.ModifiedBy,
                    };
                    db.AreaWorkItemes.Add(pieceRateToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region subcontract workItem
            var genericSubContractWorkItems = db.GenericSubContractWorkItem.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericSubContractWorkItems.Any())
            {
                foreach (GenericSubContractWorkItem subContractWorkItem in genericSubContractWorkItems)
                {
                    var scToAdd = new SubContractWorkItem
                    {
                        BlockId = blockId,
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
                        GenericSubContractWorkItemId = subContractWorkItem.GenericSubContractWorkItemId,

                        OrgBranchName = subContractWorkItem.OrgBranchName,
                        DateCreated = subContractWorkItem.DateCreated,
                        LastModified = subContractWorkItem.LastModified,
                        CreatedBy = subContractWorkItem.CreatedBy,
                        ModifiedBy = subContractWorkItem.ModifiedBy
                    };
                    db.SubContractWorkItem.Add(scToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region custom workItem
            var genericCustomWorkItems = db.GenericProjectCustomWorkItem.Where(wi => wi.SubProjectTypeId == subProjectId).ToList();
            if (genericCustomWorkItems.Any())
            {
                foreach (GenericProjectCustomWorkItem customWorkItem in genericCustomWorkItems)
                {
                    var customworkItemToAdd = new ProjectCustomWorkItem
                    {
                        BlockId = blockId,
                        ProjectInformationId = customWorkItem.ProjectInformationId,
                        GenericProjectCustomWorkItemId = customWorkItem.GenericProjectCustomWorkItemId,
                        WorkCategoryId = customWorkItem.WorkCategoryId,
                        ProjectType = customWorkItem.ProjectType,
                        WorkItemName = customWorkItem.WorkItemName,
                        TotalUnitRate = customWorkItem.TotalUnitRate,
                        Quantity = customWorkItem.TotalQuantityOfWork,
                        TotalCost = customWorkItem.TotalCost,
                        Remark = customWorkItem.Remark,
                        ItemCode = customWorkItem.ItemCode,

                        OrgBranchName = customWorkItem.OrgBranchName,
                        DateCreated = customWorkItem.DateCreated,
                        LastModified = customWorkItem.LastModified,
                        CreatedBy = customWorkItem.CreatedBy,
                        ModifiedBy = customWorkItem.ModifiedBy,
                    };
                    db.ProjectCustomWorkItems.Add(customworkItemToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
        }


        public JsonResult SyncVariationAndSupplementaryWorksToAllBlocks(int projectId)
        {
            //var projects = db.ProjectInformation.ToList();
            var currentProject = db.ProjectInformation.Find(projectId);
            if (currentProject != null)
            {
                
                var subProjects = db.SubProjectType.Where(sb => sb.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                if (subProjects.Any())
                {
                    // var totalBlocksInProject = subProjects.Sum(s => s.NumberOfBlocks);
                    var variationWorks = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId &&
                                            w.IsVariationWorkItem).ToList();
                    var supplementaryWorks = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId &&
                                            w.IsSupplementaryWorkItem).ToList();
                    var omittedOrAddedWorks = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == currentProject.ProjectInformationId &&
                                            (w.OmittedQuantityOfWork > 0 || w.AddedQuantityOfWork > 0)).ToList();
                    var allWorkItemsToAdd = new List<GenericWorkItemPerProject>();
                    allWorkItemsToAdd.AddRange(variationWorks);
                    allWorkItemsToAdd.AddRange(supplementaryWorks);
                    allWorkItemsToAdd.AddRange(omittedOrAddedWorks);

                    var ids = new HashSet<int>();

                    var allBlocks = db.Blocks.Where(b => b.ProjectInformationId == currentProject.ProjectInformationId).ToList();
                    foreach (SubProjectType subProject in subProjects)
                    {
                        var workItemsOfThisType = allWorkItemsToAdd.Where(w => w.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                        var blocksOfThisType = allBlocks.Where(b => b.SubProjectTypeId == subProject.SubProjectTypeId).ToList();
                        foreach(GenericWorkItemPerProject workItem in workItemsOfThisType)
                        {
                            foreach(Block block in blocksOfThisType)
                            {
                                var existingItem = db.WorkItemPerProject.FirstOrDefault(w => w.BlockId == block.BlockId && w.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId);
                                if(existingItem == null)
                                {
                                    var record = new WorkItemPerProject
                                    {
                                        ProjectEstimationWorkItemId = workItem.ProjectEstimationWorkItemId,
                                        IsSubCategory = workItem.IsSubCategory,
                                        ItemCode = workItem.ItemCode,
                                        ContainsWorkItemAsInput = workItem.ContainsWorkItemAsInput,
                                        GenericWorkItemPerProjectId = workItem.GenericWorkItemPerProjectId,
                                        ProjectInformationId = workItem.ProjectInformationId,
                                        BlockId = block.BlockId,

                                        ItemName = workItem.ItemName,
                                        TotalQuantityOfWork = workItem.TotalQuantityOfWork,
                                        DirectCost = workItem.DirectCost,
                                        MaterialCost = workItem.MaterialCost,
                                        LabourOutPut = workItem.LabourOutPut,
                                        EquipmentOutput = workItem.EquipmentOutput,
                                        MaterialCostPerUnit = workItem.MaterialCostPerUnit,
                                        LaborCostPerUnit = workItem.LaborCostPerUnit,
                                        EquipmentCostPerUnit = workItem.EquipmentCostPerUnit,
                                        TotalUnitCost = workItem.TotalUnitCost,
                                        OverHeadCost = workItem.OverHeadCost,
                                        ProfitCost = workItem.ProfitCost,
                                        Remark = workItem.Remark,
                                        LabourHourlyOutPut = workItem.LabourHourlyOutPut,
                                        UnitOfMeasurementId = workItem.UnitOfMeasurementId,
                                        OverheadRate = workItem.OverheadRate,
                                        ProfitMargin = workItem.ProfitMargin,

                                        IsSupplementaryWorkItem = workItem.IsSupplementaryWorkItem,
                                        IsVariationWorkItem = workItem.IsVariationWorkItem,
                                        AddedQuantityOfWork = workItem.AddedQuantityOfWork,
                                        OmittedQuantityOfWork = workItem.OmittedQuantityOfWork,

                                        OrgBranchName = workItem.OrgBranchName,
                                        DateCreated = DateTime.Now,
                                        LastModified = workItem.LastModified,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = User.Identity.Name,

                                        ParentId = workItem.ParentId,
                                        ProjectType = workItem.ProjectType,
                                        WorkCategoryId = workItem.WorkCategoryId
                                    };
                                    db.WorkItemPerProject.Add(record);
                                    db.SaveChanges();
                                    ids.Add(record.WorkItemPerProjectId);
                                    #region copy resources
                                    if (workItem.ContainsWorkItemAsInput)
                                    {
                                        var customDetails = db.GenericProjectCustomWorkItemDetails.Where(w => w.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                        if (customDetails.Any())
                                        {
                                            foreach(GenericProjectCustomWorkItemDetail sourceDetail in customDetails)
                                            {
                                                var recordDetail = new ProjectCustomWorkItemDetail
                                                {
                                                    ProjectEstimationWorkItemId = sourceDetail.ProjectEstimationWorkItemId,
                                                    WorkItemPerProjectId = record.WorkItemPerProjectId,
                                                    UnitCost = sourceDetail.UnitCost,
                                                    QuantityPerUnit = sourceDetail.QuantityPerUnit,
                                                    TotalCost = sourceDetail.TotalCost,
                                                    CreatedBy = User.Identity.Name,
                                                    DateCreated = DateTime.Now,
                                                    LastModified = DateTime.Now,
                                                    ModifiedBy = User.Identity.Name,
                                                    OrgBranchName = sourceDetail.OrgBranchName,
                                                    
                                                };
                                                db.ProjectCustomWorkItemDetails.Add(recordDetail);
                                                db.SaveChanges();
                                            }
                                        }
                                    }
                                    #region materials
                                    var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                    if (materials.Any())
                                    {
                                        foreach (GenericProjectMaterial material in materials)
                                        {
                                            var materialToAdd = new ProjectMaterial
                                            {
                                                ItemCode = material.ItemCode,
                                                ItemCategoryCode = material.ItemCategoryCode,
                                                Unit = material.Unit,
                                                Quantity = material.Quantity,
                                                UnitPrice = material.UnitPrice,
                                                TotalCost = material.TotalCost,
                                                MaterialUnitCost = material.MaterialUnitCost,
                                                OrgBranchName = material.OrgBranchName,
                                                DateCreated = material.DateCreated,
                                                LastModified = material.LastModified,
                                                CreatedBy = material.CreatedBy,
                                                ModifiedBy = material.ModifiedBy,
                                                WorkItemPerProjectId = record.WorkItemPerProjectId
                                            };
                                            db.ProjectMaterial.Add(materialToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    #region direct labor
                                    var labors = db.GenericProjectDirectLabour.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                    if (labors.Any())
                                    {
                                        foreach (GenericProjectDirectLabour labor in labors)
                                        {
                                            var laborToAdd = new ProjectDirectLabour
                                            {
                                                Unit = labor.Unit,
                                                Quantity = labor.Quantity,
                                                UnitFactory = labor.UnitFactory,
                                                IndexedHourlySalary = labor.IndexedHourlySalary,
                                                DailyPay = labor.DailyPay,
                                                TotalCost = labor.TotalCost,
                                                ManpowerUnitCost = labor.ManpowerUnitCost,
                                                OrgBranchName = labor.OrgBranchName,
                                                DateCreated = labor.DateCreated,
                                                LastModified = labor.LastModified,
                                                CreatedBy = labor.CreatedBy,
                                                ModifiedBy = labor.ModifiedBy,
                                                ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                                                WorkItemPerProjectId = record.WorkItemPerProjectId
                                            };
                                            db.ProjectDirectLabour.Add(laborToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    #region equipment
                                    var equipments = db.GenericProjectEquipment.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                    if (equipments.Any())
                                    {
                                        foreach (GenericProjectEquipment projectEquipment in equipments)
                                        {
                                            var equipmentToAdd = new ProjectEquipment
                                            {
                                                WorkItemPerProjectId = record.WorkItemPerProjectId,
                                                FleetsID = projectEquipment.FleetsID,
                                                Unit = projectEquipment.Unit,
                                                Quantity = projectEquipment.Quantity,
                                                UnitFactory = projectEquipment.UnitFactory,
                                                RentalCost = projectEquipment.RentalCost,
                                                TotalCost = projectEquipment.TotalCost,
                                                EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                                                EquipmentType = projectEquipment.EquipmentType,

                                                OrgBranchName = projectEquipment.OrgBranchName,
                                                DateCreated = projectEquipment.DateCreated,
                                                LastModified = projectEquipment.LastModified,
                                                CreatedBy = projectEquipment.CreatedBy,
                                                ModifiedBy = projectEquipment.ModifiedBy,
                                            };
                                            db.ProjectEquipment.Add(equipmentToAdd);
                                            db.SaveChanges();
                                        }
                                    }
                                    #endregion
                                    #endregion
                                }
                            }
                        }
                    }
                    var formattedData = new
                    {
                        Message = "Sync Successful!",
                        Status = "OK"
                    };
                    UpdateWorkItemsHeirarchy(ids, currentProject.ProjectInformationId);

                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }

                //}
            }
            var errorData = new
            {
                Message = "Some Errors were found and operation wasn't successful",
                Status = "Error"
            };
            return Json(errorData, JsonRequestBehavior.AllowGet);
        }

        // update the entire work items heirarchy .... 
        private void UpdateWorkItemsHeirarchy(int projectId)
        {
            var workItemsInProject = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == projectId && wi.ParentId != 0).ToList();
            if (workItemsInProject.Any())
            {
                foreach (var workItem in workItemsInProject)
                {
                    var workItemParent = db.WorkItemPerProject
                        .FirstOrDefault(w => w.ProjectInformationId == projectId && 
                                            w.BlockId == workItem.BlockId && 
                                            w.GenericWorkItemPerProjectId == workItem.ParentId);

                    if (workItemParent != null)
                    {
                        workItem.ParentId = workItemParent.WorkItemPerProjectId;
                        db.WorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        /// <summary>
        /// update the heirarchy of specific work items
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="projectId"></param>
        private void UpdateWorkItemsHeirarchy(HashSet<int> ids, int projectId)
        {
            var workItemsInProject = db.WorkItemPerProject.Where(wi => ids.Contains(wi.WorkItemPerProjectId)).ToList();
            if (workItemsInProject.Any())
            {
                foreach (var workItem in workItemsInProject)
                {
                    var workItemParent = db.WorkItemPerProject
                        .FirstOrDefault(w => w.ProjectInformationId == projectId &&
                                            w.BlockId == workItem.BlockId &&
                                            w.GenericWorkItemPerProjectId == workItem.ParentId);

                    if (workItemParent != null)
                    {
                        workItem.ParentId = workItemParent.WorkItemPerProjectId;
                        db.WorkItemPerProject.Attach(workItem);
                        db.Entry(workItem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
        private void UpdateChangedInformation()
        {
            var workItems = db.WorkItemPerProject.Where(w => !w.IsSubCategory).ToList();
            if (workItems.Any())
            {
                foreach (WorkItemPerProject workItem in workItems)
                {
                    var hourlyOutPut = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut : 1;
                    var totalMaterialCost = new decimal();
                    var totalEquipmentCost = new decimal();
                    var totalLaborCost = new decimal();
                    var materials = db.ProjectMaterial.Where(m => m.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                    var equipments = db.ProjectEquipment.Where(e => e.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                    var labors = db.ProjectDirectLabour.Where(l => l.WorkItemPerProjectId == workItem.WorkItemPerProjectId).ToList();
                    if (materials.Any())
                    {
                        totalMaterialCost = materials.Sum(m => m.TotalCost);
                    }
                    if (labors.Any())
                    {
                        totalLaborCost = labors.Sum(l => l.TotalCost);
                    }
                    if (equipments.Any())
                    {
                        totalEquipmentCost = equipments.Sum(e => e.TotalCost);
                    }

                    //workItem.MaterialCostPerUnit = totalMaterialCost / hourlyOutPut;
                    workItem.MaterialCostPerUnit = totalMaterialCost;
                    workItem.EquipmentCostPerUnit = totalEquipmentCost / hourlyOutPut;
                    workItem.LaborCostPerUnit = totalLaborCost / hourlyOutPut;

                    workItem.DirectCost = workItem.MaterialCostPerUnit + workItem.LaborCostPerUnit + workItem.EquipmentCostPerUnit;
                    workItem.OverHeadCost = workItem.OverheadRate / 100 * workItem.DirectCost;
                    workItem.ProfitCost = workItem.ProfitMargin / 100 * workItem.DirectCost;

                    workItem.TotalUnitCost = workItem.DirectCost + workItem.ProfitCost + workItem.OverHeadCost;

                    db.WorkItemPerProject.Attach(workItem);
                    db.Entry(workItem).State = EntityState.Modified;
                    db.SaveChanges();
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