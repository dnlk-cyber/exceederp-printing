using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectTakeOffController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            //var lookups = db.Lookups.ToList();

            ViewData["Category"] = db.WorkCategory.Select(c => new { c .Name, c.WorkCategoryId}).ToList();
            //var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            //ViewData["Unit"] = units;

            var deformedBars = db.DeformedBars.ToList();
            ViewData["bars"] = deformedBars;

            //var unitWegight = db.UnitRate.ToList();
            //ViewData["Diameters"] = unitWegight.ToList();
            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId}).ToList();
            ViewData["WorkItems"] = workItems.ToList();
            //var projectTypes = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            //ViewData["ProjectTypeList"] = projectTypes;
            ViewData["uom"] = db.UnitOfMeasurement.Select(s => new { s.Abbreviation,  s.UnitOfMeasurementId }).ToList();
            return View();
        }
        public ActionResult Project(int p) {
            db.Configuration.ProxyCreationEnabled = false;
            //var lookups = db.Lookups.ToList();

            ViewData["Category"] = db.WorkCategory.Select(c => new { c.Name, c.WorkCategoryId }).ToList();
            //var units = lookups.Where(x => x.LookupType == LookUpTypes.UOM).ToList();
            //ViewData["Unit"] = units;

            var deformedBars = db.DeformedBars.ToList();
            ViewData["bars"] = deformedBars;

            //var unitWegight = db.UnitRate.ToList();
            //ViewData["Diameters"] = unitWegight.ToList();
            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ItemName, w.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems.ToList();
            //var projectTypes = lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();
            //ViewData["ProjectTypeList"] = projectTypes;
            ViewData["uom"] = db.UnitOfMeasurement.Select(s => new { s.Abbreviation, s.UnitOfMeasurementId }).ToList();

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
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation
                .Where(x => x.ProjectStatus != ProjectStatus.ESTIMATION &&  x.ProjectStatus != ProjectStatus.CLOSED && !x.IsDeleted && ids.Contains(x.ProjectInformationId));
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
                    StructureyType = ConsSettingHelper.GetStructureTypeCode(workItemPerProject.WorkCategory.ConfigurationId, workItemPerProject.ProjectType) + " " + workItemPerProject.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                    WorkCategory = workItemPerProject.WorkCategory.Index + " " + workItemPerProject.WorkCategory.Name,
                    IsSubCategory = workItemPerProject.IsSubCategory,
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
        public ActionResult WorkItemUnderWorkItem_Read([DataSourceRequest] DataSourceRequest request, int parentWorkItemId)
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

        public ActionResult ProjectTakeOff_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<ProjectTakeOff> projecttakeoff = db.ProjectTakeOff.Where(t => t.WorkItemPerProjectId == workItemId).AsQueryable();
            DataSourceResult result = projecttakeoff.ToDataSourceResult(request, projectTakeOff => new
            {
                ProjectTakeOffId = projectTakeOff.ProjectTakeOffId,
                TakeOffNo = projectTakeOff.TakeOffNo,
                WorkItemPerProjectId = projectTakeOff.WorkItemPerProjectId,
                ProjectInformationId = projectTakeOff.ProjectInformationId,
                Remark = projectTakeOff.Remark,
                IsReinforcementbarTakeOff = projectTakeOff.IsReinforcementbarTakeOff,

                OrgBranchName = projectTakeOff.OrgBranchName,
                DateCreated = projectTakeOff.DateCreated,
                LastModified = projectTakeOff.LastModified,
                CreatedBy = projectTakeOff.CreatedBy,
                ModifiedBy = projectTakeOff.ModifiedBy
            });

            return Json(result);
        }
        private bool WorkItemHasTakeOffSheet(int workItemId)
        {
            bool hasTakeOffSheet = false;
            var takeOff = db.ProjectTakeOff.Where(t => t.WorkItemPerProjectId == workItemId).ToList();
            if (takeOff.Any())
            {
                hasTakeOffSheet = true;
            }
            return hasTakeOffSheet;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOff_Create([DataSourceRequest]DataSourceRequest request, ProjectTakeOff projectTakeOff, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (WorkItemHasTakeOffSheet(workItemId))
                {
                    ModelState.AddModelError("", "Take off sheet exists for this work item");
                }
                else
                {
                    var entity = new ProjectTakeOff
                    {
                        TakeOffNo = projectTakeOff.TakeOffNo,
                        WorkItemPerProjectId = workItemId,
                        ProjectInformationId = this.GetProjectInformationId(workItemId),
                        Remark = projectTakeOff.Remark,
                        IsReinforcementbarTakeOff = projectTakeOff.IsReinforcementbarTakeOff,

                        OrgBranchName = projectTakeOff.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectTakeOff.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectTakeOff.ModifiedBy
                    };

                    db.ProjectTakeOff.Add(entity);
                    db.SaveChanges();

                    this.UpdateTakeOffNumber(entity);
                    projectTakeOff.ProjectTakeOffId = entity.ProjectTakeOffId;
                }

            }

            return Json(new[] { projectTakeOff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOff_Update([DataSourceRequest]DataSourceRequest request, ProjectTakeOff projectTakeOff)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOff
                {
                    ProjectTakeOffId = projectTakeOff.ProjectTakeOffId,
                    TakeOffNo = projectTakeOff.TakeOffNo,
                    WorkItemPerProjectId = projectTakeOff.WorkItemPerProjectId,
                    ProjectInformationId = projectTakeOff.ProjectInformationId,
                    Remark = projectTakeOff.Remark,
                    IsReinforcementbarTakeOff = projectTakeOff.IsReinforcementbarTakeOff,

                    OrgBranchName = projectTakeOff.OrgBranchName,
                    DateCreated = projectTakeOff.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectTakeOff.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectTakeOff.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectTakeOff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOff_Destroy([DataSourceRequest]DataSourceRequest request, ProjectTakeOff projectTakeOff)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOff
                {

                    ProjectTakeOffId = projectTakeOff.ProjectTakeOffId,
                    TakeOffNo = projectTakeOff.TakeOffNo,
                    WorkItemPerProjectId = projectTakeOff.WorkItemPerProjectId,
                    ProjectInformationId = projectTakeOff.ProjectInformationId,
                    Remark = projectTakeOff.Remark,
                    IsReinforcementbarTakeOff = projectTakeOff.IsReinforcementbarTakeOff,

                    OrgBranchName = projectTakeOff.OrgBranchName,
                    DateCreated = projectTakeOff.DateCreated,
                    LastModified = projectTakeOff.LastModified,
                    CreatedBy = projectTakeOff.CreatedBy,
                    ModifiedBy = projectTakeOff.ModifiedBy
                };

                db.ProjectTakeOff.Attach(entity);
                db.ProjectTakeOff.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectTakeOff }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateTakeOffNumber(ProjectTakeOff takeOff)
        {
            takeOff.TakeOffNo = takeOff.ProjectTakeOffId.ToString("D10");

            db.ProjectTakeOff.Attach(takeOff);
            db.Entry(takeOff).State = EntityState.Modified;
            db.SaveChanges();
        }
        private int GetProjectInformationId(int workItemId)
        {
            var currentWorkItem = db.WorkItemPerProject.Find(workItemId);
            var project = new ProjectInformation();
            if (currentWorkItem != null)
            {
                var temp = db.ProjectInformation.Find(currentWorkItem.ProjectInformationId);
                if (temp != null)
                {
                    project = temp;
                }
            }
            return project.ProjectInformationId;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
