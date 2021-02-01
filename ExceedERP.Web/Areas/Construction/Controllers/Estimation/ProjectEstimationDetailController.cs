using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Areas.Construction.Controllers.Project;
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
    public class ProjectEstimationDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly ConstructionInputs inputs = new ConstructionInputs();
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            //var workItemCategories = from w in db.WorkCategory
            //                         join p in db.ProjectTypeMappings
            //                             on w.ConfigurationId equals p.ConfigurationId
            //                         select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });
            //ViewData["Categories"] = workItemCategories.ToList();
            ViewData["Categories"] = db.WorkCategory.Select(w => new { w.Name, w.ConfigurationId, w.WorkCategoryId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ProjectEstimationWorkItemId, w.WorkCategoryId, w.ItemName }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name, i.ItemCategoryCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(x =>
                        new
                        {
                            ProjectEstimationIndexLabourId = x.ProjectEstimationIndexLabourId,
                            Title = GetTitle(x.ProjectJobTitleId)
                        }).ToList();

            //var categories = db.EquipmentIdentity.ToList();
            //ViewData["EquipmentIdentity"] = categories.Select(
            //          x =>
            //              new
            //              {
            //                  EquipmentCategoryName = x.EquipmentCategoryName,
            //                  EquipmentIdentityID = x.EquipmentIdentityID,
            //              }).ToList();

            //var equipmentTypes = db.EquipmentType.ToList();
            //ViewData["EquipmentType"] = equipmentTypes.Select(
            //            x =>
            //                new
            //                {
            //                    EquipmentTypeName = x.EquipmentTypeName,
            //                    EquipmentTypeID = x.EquipmentTypeID,
            //                }).ToList();

            var internalEquipment = db.InternalMachinaryRent.ToList();
            var externalEquipment = db.MachinaryRent.ToList();
            var internalIds = new HashSet<Guid>(internalEquipment.Select(ic => ic.FleetsID));
            var externalIds = new HashSet<Guid>(externalEquipment.Select(ee => ee.FleetsID));
            var combinedEquipments = db.Fleets.Where(flt => (internalIds.Contains(flt.FleetsID)) || (externalIds.Contains(flt.FleetsID)));
            ViewData["Equipment"] = combinedEquipments.Select(eq => new { FleetsID = eq.FleetsID, EquipmentName = eq.EquipmentName });

            // temporry script
            //this.UpdateNullWorkItemName();

            RemoveDirtyData();
            return View();
        }
        private void RemoveDirtyData()
        {
            var subProjectTypes = db.SubProjectType.ToList();
            //var workItems = db.GenericWorkItemPerProject.ToList();
            //var rawIds = new HashSet<int?>(workItems.Select(w => w.SubProjectTypeId));
            var ids = new HashSet<int?>();
            foreach (var subProject in subProjectTypes)
            {
                ids.Add(subProject.SubProjectTypeId);
            }

            var weirdWorkItems = db.GenericWorkItemPerProject.Where(w => !(ids.Contains(w.SubProjectTypeId))).ToList();
            if (weirdWorkItems.Any())
            {
                db.GenericWorkItemPerProject.RemoveRange(weirdWorkItems);
                db.SaveChanges();
            }
            // var weirdDuration =db.WorkItemDuration.Where(w => w.GenericWorkItemPerProjectId)
        }
        public ActionResult Project(int p)
        {

            #region
            db.Configuration.ProxyCreationEnabled = false;

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            ViewData["Categories"] = db.WorkCategory.Select(w => new { w.Name, w.ConfigurationId, w.WorkCategoryId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ProjectEstimationWorkItemId, w.WorkCategoryId, w.ItemName }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(c => new { c.ItemCategoryCode, c.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name, i.ItemCategoryCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(x =>
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
        public ActionResult Sp(int sp)
        {
            #region
            db.Configuration.ProxyCreationEnabled = false;

            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;

            ViewData["Branch"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            ViewData["StructureType"] = lis;

            ViewData["Categories"] = db.WorkCategory.Select(w => new { w.Name, w.ConfigurationId, w.WorkCategoryId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(w => new { w.ProjectEstimationWorkItemId, w.WorkCategoryId, w.ItemName }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

            ViewData["Items"] = db.InventoryItems.Select(i => new { i.ItemCode, i.Name, i.ItemCategoryCode }).ToList();

            var jobTitle = db.ProjectEstimationIndexLabour.ToList();
            ViewData["titles"] =
                jobTitle.Select(x =>
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

            #endregion

            ViewBag.Project = "";
            ViewBag.SubProject = "";
            ViewBag.SubProjectId = sp;
            var currentSubProjectType = db.SubProjectType.Find(sp);
            if (currentSubProjectType != null)
            {
                ViewBag.SubProject = currentSubProjectType.Name;
                var currentProject = db.ProjectInformation.Find(currentSubProjectType.ProjectInformationId);
                if (currentProject != null)
                {
                    ViewBag.Project = currentProject.Name;
                }
            }
            return View();
        }
        //public ActionResult Test()
        //{
        //    return View();
        //}
        public ActionResult GenerateDefaultOrderIndex()
        {
            var projects = db.ProjectInformation.ToList();
            foreach (ProjectInformation project in projects)
            {
                var subs = db.SubProjectType.Where(sp => sp.ProjectInformationId == project.ProjectInformationId).OrderBy(s => s.SubProjectTypeId).ToList();
                if (subs.Any())
                {
                    var subsArray = subs.ToArray();
                    for (int i = 1; i <= subsArray.Length; i++)
                    {
                        var sb = subsArray[i - 1];
                        sb.OrderIndex = i;

                        db.SubProjectType.Attach(sb);
                        db.Entry(sb).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                //foreach (SubProjectType subProjectType in subs)
                //{
                //    subProjectType.OrderIndex
                //}
            }
            return Redirect("Index");
        }
        #region temporary methods
        public ActionResult MigrateWorkItemsToNewStructure()
        {
            GenericWorkItemPerProjectController.MigrateWorkItemsToNewStructure();

            return Redirect("Index");
        }
        public ActionResult UpdateLatestChanges()
        {
            this.UpdateChangedInformation();
            return Redirect("Index");
        }
        public ActionResult UpdateTransportCostAndThicknessFactor()
        {
            var items = db.GenericWorkItemPerProject.Where(i => !i.IsSubCategory && !i.HasAdditionalTransportationCost).ToList();
            foreach(GenericWorkItemPerProject item in items)
            {
                item.TotalUnitCostIncludingTransport  =  item.TotalUnitCostIncludingTransport ==0 ? item.TotalUnitCost : item.TotalUnitCostIncludingTransport;
                item.ThicknessFactor = item.ThicknessFactor == 0 ? 1 : item.ThicknessFactor;

                db.GenericWorkItemPerProject.Attach(item);
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect("Index");
        }
        #endregion

        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION);
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted);
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

        public ActionResult ProjectInformation_Read_Deleted([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => p.IsDeleted);
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
                ProjectStatus = projectInformation.ProjectStatus,

                IsDeleted = projectInformation.IsDeleted,
                DeletedBy = projectInformation.DeletedBy,
                DeletedOn = projectInformation.DeletedOn,

            });

            return Json(result);
        }
        public ActionResult ProjectGeneralInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.ESTIMATION && !x.IsDeleted);
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new ProjectGeneralInformationViewModel
            {
                ProjectType = projectInformation.ProjectType,
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,

                Code = projectInformation.Code,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,

                DateCreated = projectInformation.DateCreated != null ? projectInformation.DateCreated.ToString() : " ",
                LastModified = projectInformation.LastModified != null ? projectInformation.LastModified.ToString() : " ",
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus.ToString()

            });

            return Json(result);
        }
        public ActionResult Read_ProjectsWithDeletedSubProjectTypes([DataSourceRequest]DataSourceRequest request)
        {
            HashSet<int> ids = new HashSet<int>(db.SubProjectType.Where(x => x.IsDeleted).ToList().Select(x => x.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => ids.Contains(p.ProjectInformationId));
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
                ProjectStatus = projectInformation.ProjectStatus,

                IsDeleted = projectInformation.IsDeleted,
                DeletedBy = projectInformation.DeletedBy,
                DeletedOn = projectInformation.DeletedOn,

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

                this.CreateUserProjectAssignmentOnProjectAdded(entity.ProjectInformationId, User.Identity.Name);
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
                //ProjectInformationHelper.CleanRemainingDataWhenProjectIsDeleted(projectInformation.ProjectInformationId);

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
                    ,
                    DeletedOn = DateTime.Now,
                    DeletedBy = User.Identity.Name,
                    IsDeleted = true

                };

                db.ProjectInformation.Attach(entity);
                //db.ProjectInformation.Remove(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectInformation }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectInformation_Destroy_Permanently([DataSourceRequest]DataSourceRequest request, ProjectInformation projectInformation)
        {
            if (ModelState.IsValid)
            {
                ProjectInformationHelper.CleanRemainingDataWhenProjectIsDeleted(projectInformation.ProjectInformationId);

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

        public ActionResult SyncEstimationDataToBlocks(int id)
        {
            new ProjectEstimationSplitController().SplitProjectAndAddWorkItemsToSpecificBlocks(id);
            return Redirect("Index");
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
        public void CopyCurrentTemplateSubContractWorkItem(int workItemId, int subProjTypeMappingId)
        {
            var currentWorkItem = db.EstimationSubContractWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        var existingSubContractWorkItem =
                            db.GenericSubContractWorkItem.FirstOrDefault(
                                        wpp => wpp.ItemCode == currentWorkItem.ItemCode &&
                                        wpp.WorkCategory.ConfigurationId == mapping.ConfigurationId &&
                                        wpp.WorkItemName == currentWorkItem.WorkItemName &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);
                        if (existingSubContractWorkItem != null)
                        {
                            ProjectEstimationHelper.DeleteGenericSubContractWorkItemAndCleanReferencesFromProject(existingSubContractWorkItem.GenericSubContractWorkItemId);
                        }

                        var workItemToAdd = new GenericSubContractWorkItem
                        {
                            WorkItemName = currentWorkItem.WorkItemName,
                            ItemCode = currentWorkItem.ItemCode,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,
                            Quntity = currentWorkItem.Quntity,
                            SelectedCost = currentWorkItem.SelectedCost,
                            SubContractCostOne = currentWorkItem.SubContractCostOne,
                            SubContractCostTwo = currentWorkItem.SubContractCostTwo,
                            SubContractCostThree = currentWorkItem.SubContractCostThree,
                            SubProjectTypeId = currentSubProject.SubProjectTypeId,
                            TotalCost = currentWorkItem.TotalCost,
                            UnitOfMeasurementId = currentWorkItem.UnitOfMeasurementId,
                            OverHeadProfit = currentWorkItem.OverHeadProfit,


                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                        };
                        db.GenericSubContractWorkItem.Add(workItemToAdd);
                        db.SaveChanges();
                    }
                }
            }
        }
        public void CopyCurrentTemplatePieceRate(int workItemId, int subProjTypeMappingId)
        {
            var currentWorkItem = db.ProjectEstimationAreaWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        var existingPieceRate =
                                    db.GenericAreaWorkItem.FirstOrDefault(
                                        wpp => wpp.ItemCode == currentWorkItem.ItemCode &&
                                        wpp.WorkCategory.ConfigurationId == mapping.ConfigurationId &&
                                        wpp.WorkItemName == currentWorkItem.WorkItemName &&
                                        wpp.SubProjectTypeId == mapping.SubProjectTypeId);
                        if (existingPieceRate != null)
                        {
                            ProjectEstimationHelper.DeleteGenericPieceRateAndCleanReferencesFromProject(existingPieceRate.GenericAreaWorkItemId);
                        }
                        var workItemToAdd = new GenericAreaWorkItem
                        {
                            WorkItemName = currentWorkItem.WorkItemName,
                            ItemCode = currentWorkItem.ItemCode,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,
                            Width = currentWorkItem.Width,
                            Area = currentWorkItem.Area,
                            CylinderLock = currentWorkItem.CylinderLock,
                            DirectCost = currentWorkItem.DirectCost,
                            Height = currentWorkItem.DirectCost,
                            MaterialCost = currentWorkItem.MaterialCost,
                            Price = currentWorkItem.Price,
                            TransportCost = currentWorkItem.TransportCost,
                            OverHeadProfitCost = currentWorkItem.OverHeadProfitCost,
                            SubProjectTypeId = currentSubProject.SubProjectTypeId,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                        };
                        db.GenericAreaWorkItem.Add(workItemToAdd);
                        db.SaveChanges();
                    }
                }
            }
        }
        public void CopyCurrentTemplateWorkItem(int workItemId, int subProjTypeMappingId)
        {
            var currentWorkItem = db.ProjectEstimationWorkItem.Find(workItemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        // check if the workItem is copied already 
                        var workItemInProject =
                            db.GenericWorkItemPerProject.FirstOrDefault(
                                wpp => wpp.ProjectEstimationWorkItemId == currentWorkItem.ProjectEstimationWorkItemId &&
                                wpp.WorkCategory.ConfigurationId == mapping.ConfigurationId &&
                                wpp.SubProjectTypeId == mapping.SubProjectTypeId);
                        // clean if it exists
                        if (workItemInProject != null)
                        {
                            db.GenericWorkItemPerProject.Attach(workItemInProject);
                            db.GenericWorkItemPerProject.Remove(workItemInProject);
                            db.SaveChanges();
                            if (workItemInProject.IsSubCategory || workItemInProject.ContainsWorkItemAsInput)
                            {
                                ProjectEstimationHelper.CleanReferencesFromProjectOnGenericWorkItemDelete(workItemInProject);
                            }
                        }
                        //var workItemToAdd = new GenericWorkItemPerProject
                        //{
                        //    SubProjectTypeId = currentSubProject.SubProjectTypeId,
                        //    ItemCode = currentWorkItem.ItemCode,
                        //    ContainsWorkItemAsInput = currentWorkItem.ContainsWorkItemAsInput,
                        //    IsSubCategory = currentWorkItem.IsSubCategory,

                        //    ItemName = currentWorkItem.ItemName,
                        //    WorkCategoryId = currentWorkItem.WorkCategoryId,

                        //    ProjectEstimationWorkItemId = currentWorkItem.ProjectEstimationWorkItemId,
                        //    ProjectInformationId = currentSubProject.ProjectInformationId,

                        //    TotalQuantityOfWork = currentWorkItem.TotalQuantityOfWork,
                        //    DirectCost = currentWorkItem.DirectCost,
                        //    MaterialCost = currentWorkItem.MaterialCost,
                        //    LabourOutPut = currentWorkItem.LabourOutPut,
                        //    EquipmentOutput = currentWorkItem.EquipmentOutput,
                        //    MaterialCostPerUnit = currentWorkItem.MaterialCostPerUnit,
                        //    LaborCostPerUnit = currentWorkItem.LaborCostPerUnit,
                        //    EquipmentCostPerUnit = currentWorkItem.EquipmentCostPerUnit,
                        //    TotalUnitCost = currentWorkItem.TotalUnitCost,
                        //    OverHeadCost = currentWorkItem.OverHeadCost,
                        //    ProfitCost = currentWorkItem.ProfitCost,
                        //    ProjectType = currentWorkItem.ProjectType,
                        //    Remark = currentWorkItem.Remark,
                        //    LabourHourlyOutPut = currentWorkItem.LabourHourlyOutPut,
                        //    UnitOfMeasurementId = currentWorkItem.UnitOfMeasurementId,
                        //    OverheadRate = currentWorkItem.OverheadRate,
                        //    ProfitMargin = currentWorkItem.ProfitMargin,

                        //    ParentId = currentWorkItem.ParentId,

                        //    OrgBranchName = currentWorkItem.OrgBranchName,
                        //    DateCreated = DateTime.Now,
                        //    LastModified = currentWorkItem.LastModified,
                        //    CreatedBy = User.Identity.Name,
                        //    ModifiedBy = currentWorkItem.ModifiedBy,
                        //};
                        //db.GenericWorkItemPerProject.Add(workItemToAdd);
                        //db.SaveChanges();

                        this.CopyWorkItemAndItsSubWorkItems(currentWorkItem, currentSubProject, 0);

                        //var id = workItemToAdd.GenericWorkItemPerProjectId;

                        //if (currentWorkItem.IsSubCategory)
                        //{
                        //    var childWorkItems = db.ProjectEstimationWorkItem
                        //        .Where(wi => wi.ParentId == currentWorkItem.ProjectEstimationWorkItemId).ToList();
                        //    if (childWorkItems.Any())
                        //    {
                        //        foreach (ProjectEstimationWorkItem childWorkItem in childWorkItems)
                        //        {
                        //            var childWorkItemToAdd = new GenericWorkItemPerProject
                        //            {
                        //                SubProjectTypeId = currentSubProject.SubProjectTypeId,
                        //                ItemCode = childWorkItem.ItemCode,
                        //                ContainsWorkItemAsInput = childWorkItem.ContainsWorkItemAsInput,
                        //                IsSubCategory = childWorkItem.IsSubCategory,
                        //                ItemName = childWorkItem.ItemName,
                        //                WorkCategoryId = childWorkItem.WorkCategoryId,

                        //                ProjectEstimationWorkItemId = childWorkItem.ProjectEstimationWorkItemId,
                        //                ProjectInformationId = currentSubProject.ProjectInformationId,

                        //                TotalQuantityOfWork = childWorkItem.TotalQuantityOfWork,
                        //                DirectCost = childWorkItem.DirectCost,
                        //                MaterialCost = childWorkItem.MaterialCost,
                        //                LabourOutPut = childWorkItem.LabourOutPut,
                        //                EquipmentOutput = childWorkItem.EquipmentOutput,
                        //                MaterialCostPerUnit = childWorkItem.MaterialCostPerUnit,
                        //                LaborCostPerUnit = childWorkItem.LaborCostPerUnit,
                        //                EquipmentCostPerUnit = childWorkItem.EquipmentCostPerUnit,
                        //                TotalUnitCost = childWorkItem.TotalUnitCost,
                        //                OverHeadCost = childWorkItem.OverHeadCost,
                        //                ProfitCost = childWorkItem.ProfitCost,
                        //                ProjectType = childWorkItem.ProjectType,
                        //                Remark = childWorkItem.Remark,
                        //                LabourHourlyOutPut = childWorkItem.LabourHourlyOutPut,
                        //                UnitOfMeasurementId = childWorkItem.UnitOfMeasurementId,
                        //                OverheadRate = childWorkItem.OverheadRate,
                        //                ProfitMargin = childWorkItem.ProfitMargin,

                        //                ParentId = id,

                        //                OrgBranchName = childWorkItem.OrgBranchName,
                        //                DateCreated = DateTime.Now,
                        //                LastModified = childWorkItem.LastModified,
                        //                CreatedBy = User.Identity.Name,
                        //                ModifiedBy = childWorkItem.ModifiedBy,
                        //            };
                        //            db.GenericWorkItemPerProject.Add(childWorkItemToAdd);
                        //            db.SaveChanges();

                        //            var childId = childWorkItemToAdd.GenericWorkItemPerProjectId;
                        //            // copy material, equipment and labor from templae work item
                        //            CopyWorkItemResourceFromTemplate(childWorkItem.ProjectEstimationWorkItemId, childId, childWorkItem.ContainsWorkItemAsInput);

                        //            #region child of child work items
                        //            if (childWorkItem.IsSubCategory)
                        //            {
                        //                var childOfChildWorkItems = db.ProjectEstimationWorkItem.Where(w => w.ParentId == childWorkItem.ProjectEstimationWorkItemId).ToList();
                        //                if (childOfChildWorkItems.Any())
                        //                {
                        //                    foreach (ProjectEstimationWorkItem childOfChildWorkItem in childOfChildWorkItems)
                        //                    {
                        //                        var childOfChildWorkItemToAdd = new GenericWorkItemPerProject
                        //                        {
                        //                            SubProjectTypeId = currentSubProject.SubProjectTypeId,
                        //                            ItemCode = childOfChildWorkItem.ItemCode,
                        //                            ContainsWorkItemAsInput = childOfChildWorkItem.ContainsWorkItemAsInput,
                        //                            IsSubCategory = childOfChildWorkItem.IsSubCategory,
                        //                            ItemName = childOfChildWorkItem.ItemName,
                        //                            WorkCategoryId = childOfChildWorkItem.WorkCategoryId,

                        //                            ProjectEstimationWorkItemId = childOfChildWorkItem.ProjectEstimationWorkItemId,
                        //                            ProjectInformationId = currentSubProject.ProjectInformationId,

                        //                            TotalQuantityOfWork = childOfChildWorkItem.TotalQuantityOfWork,
                        //                            DirectCost = childOfChildWorkItem.DirectCost,
                        //                            MaterialCost = childOfChildWorkItem.MaterialCost,
                        //                            LabourOutPut = childOfChildWorkItem.LabourOutPut,
                        //                            EquipmentOutput = childOfChildWorkItem.EquipmentOutput,
                        //                            MaterialCostPerUnit = childOfChildWorkItem.MaterialCostPerUnit,
                        //                            LaborCostPerUnit = childOfChildWorkItem.LaborCostPerUnit,
                        //                            EquipmentCostPerUnit = childOfChildWorkItem.EquipmentCostPerUnit,
                        //                            TotalUnitCost = childOfChildWorkItem.TotalUnitCost,
                        //                            OverHeadCost = childOfChildWorkItem.OverHeadCost,
                        //                            ProfitCost = childOfChildWorkItem.ProfitCost,
                        //                            ProjectType = childOfChildWorkItem.ProjectType,
                        //                            Remark = childOfChildWorkItem.Remark,
                        //                            LabourHourlyOutPut = childOfChildWorkItem.LabourHourlyOutPut,
                        //                            UnitOfMeasurementId = childOfChildWorkItem.UnitOfMeasurementId,
                        //                            OverheadRate = childOfChildWorkItem.OverheadRate,
                        //                            ProfitMargin = childOfChildWorkItem.ProfitMargin,

                        //                            ParentId = childWorkItemToAdd.GenericWorkItemPerProjectId,

                        //                            OrgBranchName = childOfChildWorkItem.OrgBranchName,
                        //                            DateCreated = DateTime.Now,
                        //                            LastModified = childOfChildWorkItem.LastModified,
                        //                            CreatedBy = User.Identity.Name,
                        //                            ModifiedBy = childOfChildWorkItem.ModifiedBy,
                        //                        };
                        //                        db.GenericWorkItemPerProject.Add(childOfChildWorkItemToAdd);
                        //                        db.SaveChanges();
                        //                        CopyWorkItemResourceFromTemplate(childOfChildWorkItem.ProjectEstimationWorkItemId, childOfChildWorkItemToAdd.GenericWorkItemPerProjectId, childOfChildWorkItem.ContainsWorkItemAsInput);
                        //                    }
                        //                }
                        //            }
                        //            #endregion
                        //        }
                        //    }
                        //}
                        //// current work item is not a sub-category ...
                        //// so it probably has material, labor and equipment
                        //else
                        //{
                        //    CopyWorkItemResourceFromTemplate(currentWorkItem.ProjectEstimationWorkItemId, id, currentWorkItem.ContainsWorkItemAsInput);
                        //}
                    }
                }
            }
        }
        private void CopyWorkItemResourceFromTemplate(int templateId, int destinationId, bool templateContainsCustomItems)
        {
            var materials = db.ProjectEstimationMaterial
                .Where(m => m.ProjectEstimationWorkItemId == templateId).ToList();
            var equipments = db.ProjectEstimationEquipment
                .Where(eq => eq.ProjectEstimationWorkItemId == templateId).ToList();
            var labors = db.ProjectEstimationDirectLabour
                .Where(l => l.ProjectEstimationWorkItemId == templateId).ToList();
            List<EstimationTemplateCustomWorkItemDetail> customDetails = new List<EstimationTemplateCustomWorkItemDetail>();
            if (templateContainsCustomItems)
            {
                var details = db.EstimationTemplateCustomWorkItemDetails.Where(d => d.ParentId == templateId).ToList();
                if (details.Any())
                {
                    foreach (EstimationTemplateCustomWorkItemDetail itemDetail in details)
                    {
                        var customItemRecord = new GenericProjectCustomWorkItemDetail
                        {
                            QuantityPerUnit = itemDetail.QuantityPerUnit,
                            UnitCost = itemDetail.UnitCost,
                            TotalCost = itemDetail.TotalCost,
                            ProjectEstimationWorkItemId = itemDetail.ProjectEstimationWorkItemId,
                            GenericWorkItemPerProjectId = destinationId,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = itemDetail.OrgBranchName,
                        };
                        db.GenericProjectCustomWorkItemDetails.Add(customItemRecord);
                        db.SaveChanges();
                    }
                }
            }
            #region copy material
            if (materials.Any())
            {
                foreach (ProjectEstimationMaterial material in materials)
                {
                    var materialToAdd = new GenericProjectMaterial
                    {
                        ItemCode = material.ItemCode,
                        ItemCategoryCode = material.ItemCategoryCode,
                        Unit = material.Unit,
                        Quantity = material.Quantity,
                        UnitPrice = material.UnitPrice,
                        TotalCost = material.TotalCost,
                        MaterialUnitCost = material.MaterialUnitCost,
                        GenericWorkItemPerProjectId = destinationId,

                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy,
                    };
                    db.GenericProjectMaterial.Add(materialToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy labor
            if (labors.Any())
            {
                foreach (ProjectEstimationDirectLabour labor in labors)
                {
                    var laborToAdd = new GenericProjectDirectLabour
                    {
                        Unit = labor.Unit,
                        Quantity = labor.Quantity,
                        UnitFactory = labor.UnitFactory,
                        IndexedHourlySalary = labor.IndexedHourlySalary,
                        DailyPay = labor.DailyPay,
                        TotalCost = labor.TotalCost,
                        ManpowerUnitCost = labor.ManpowerUnitCost,
                        GenericWorkItemPerProjectId = destinationId,
                        ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,

                        OrgBranchName = labor.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = labor.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = labor.ModifiedBy,

                    };
                    db.GenericProjectDirectLabour.Add(laborToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy equipment
            if (equipments.Any())
            {
                foreach (ProjectEstimationEquipment equipment in equipments)
                {
                    var equipmentToAdd = new GenericProjectEquipment
                    {
                        FleetsID = equipment.FleetsID,
                        Unit = equipment.Unit,
                        Quantity = equipment.Quantity,
                        UnitFactory = equipment.UnitFactory,
                        RentalCost = equipment.RentalCost,
                        TotalCost = equipment.TotalCost,
                        EquipmentUnitCost = equipment.EquipmentUnitCost,
                        EquipmentType = equipment.EquipmentType,
                        GenericWorkItemPerProjectId = destinationId,

                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = equipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = equipment.ModifiedBy,
                    };
                    db.GenericProjectEquipment.Add(equipmentToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
        }

        public void CopyCurrentWorkItemFromProject(int itemId, int subProjTypeMappingId)
        {
            var currentWorkItem = db.GenericWorkItemPerProject.Find(itemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        #region Remove if work item exists
                        var existingWorkItem = db.GenericWorkItemPerProject.FirstOrDefault(w => w.ProjectEstimationWorkItemId == currentWorkItem.ProjectEstimationWorkItemId &&
                                                                                                w.SubProjectTypeId == currentSubProject.SubProjectTypeId && w.WorkCategory.ConfigurationId == mapping.ConfigurationId);

                        if (existingWorkItem != null)
                        {
                            RemoveExistingWorkItemBeforeCopyNew(existingWorkItem.GenericWorkItemPerProjectId);
                        }
                        #endregion
                        var workItemToAdd = new GenericWorkItemPerProject
                        {
                            SubProjectTypeId = currentSubProject.SubProjectTypeId,
                            ItemCode = currentWorkItem.ItemCode,
                            ContainsWorkItemAsInput = currentWorkItem.ContainsWorkItemAsInput,
                            IsSubCategory = currentWorkItem.IsSubCategory,
                            ItemName = currentWorkItem.ItemName,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,

                            ProjectEstimationWorkItemId = currentWorkItem.ProjectEstimationWorkItemId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,

                            TotalQuantityOfWork = currentWorkItem.TotalQuantityOfWork,
                            DirectCost = currentWorkItem.DirectCost,
                            MaterialCost = currentWorkItem.MaterialCost,
                            LabourOutPut = currentWorkItem.LabourOutPut,
                            EquipmentOutput = currentWorkItem.EquipmentOutput,
                            MaterialCostPerUnit = currentWorkItem.MaterialCostPerUnit,
                            LaborCostPerUnit = currentWorkItem.LaborCostPerUnit,
                            EquipmentCostPerUnit = currentWorkItem.EquipmentCostPerUnit,
                            TotalUnitCost = currentWorkItem.TotalUnitCost,
                            TransportCostRate = currentWorkItem.TransportCostRate,
                            HasAdditionalTransportationCost = currentWorkItem.HasAdditionalTransportationCost,
                            CalculatedTransportCost = currentWorkItem.CalculatedTransportCost,
                            TotalUnitCostIncludingTransport = currentWorkItem.TotalUnitCostIncludingTransport,
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
                        db.GenericWorkItemPerProject.Add(workItemToAdd);
                        db.SaveChanges();

                        var id = workItemToAdd.GenericWorkItemPerProjectId;

                        if (currentWorkItem.IsSubCategory)
                        {
                            var childWorkItems = db.GenericWorkItemPerProject
                                .Where(wi => wi.ParentId == currentWorkItem.GenericWorkItemPerProjectId).ToList();
                            if (childWorkItems.Any())
                            {
                                foreach (GenericWorkItemPerProject childWorkItem in childWorkItems)
                                {
                                    var childWorkItemToAdd = new GenericWorkItemPerProject
                                    {
                                        SubProjectTypeId = currentSubProject.SubProjectTypeId,
                                        ItemCode = childWorkItem.ItemCode,
                                        ContainsWorkItemAsInput = childWorkItem.ContainsWorkItemAsInput,
                                        IsSubCategory = childWorkItem.IsSubCategory,
                                        ItemName = childWorkItem.ItemName,
                                        WorkCategoryId = childWorkItem.WorkCategoryId,

                                        ProjectEstimationWorkItemId = childWorkItem.ProjectEstimationWorkItemId,
                                        ProjectInformationId = currentSubProject.ProjectInformationId,

                                        TotalQuantityOfWork = childWorkItem.TotalQuantityOfWork,
                                        DirectCost = childWorkItem.DirectCost,
                                        MaterialCost = childWorkItem.MaterialCost,
                                        LabourOutPut = childWorkItem.LabourOutPut,
                                        EquipmentOutput = childWorkItem.EquipmentOutput,
                                        MaterialCostPerUnit = childWorkItem.MaterialCostPerUnit,
                                        LaborCostPerUnit = childWorkItem.LaborCostPerUnit,
                                        EquipmentCostPerUnit = childWorkItem.EquipmentCostPerUnit,
                                        TotalUnitCost = childWorkItem.TotalUnitCost,
                                        TransportCostRate = currentWorkItem.TransportCostRate,
                                        HasAdditionalTransportationCost = currentWorkItem.HasAdditionalTransportationCost,
                                        CalculatedTransportCost = currentWorkItem.CalculatedTransportCost,
                                        TotalUnitCostIncludingTransport = currentWorkItem.TotalUnitCostIncludingTransport,
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
                                    db.GenericWorkItemPerProject.Add(childWorkItemToAdd);
                                    db.SaveChanges();

                                    var childId = childWorkItemToAdd.GenericWorkItemPerProjectId;
                                    // copy material, equipment and labor from templae work item
                                    CopyWorkItemResourceFromProject(childWorkItem.GenericWorkItemPerProjectId, childId, childWorkItem.ContainsWorkItemAsInput);

                                    #region child of child work items
                                    if (childWorkItem.IsSubCategory)
                                    {
                                        var childOfChildWorkItems = db.GenericWorkItemPerProject.Where(w => w.ParentId == childWorkItem.GenericWorkItemPerProjectId).ToList();
                                        if (childOfChildWorkItems.Any())
                                        {
                                            foreach (GenericWorkItemPerProject childOfChildWorkItem in childOfChildWorkItems)
                                            {
                                                var childOfChildWorkItemToAdd = new GenericWorkItemPerProject
                                                {
                                                    SubProjectTypeId = currentSubProject.SubProjectTypeId,
                                                    ItemCode = childOfChildWorkItem.ItemCode,
                                                    ContainsWorkItemAsInput = childOfChildWorkItem.ContainsWorkItemAsInput,
                                                    IsSubCategory = childOfChildWorkItem.IsSubCategory,
                                                    ItemName = childOfChildWorkItem.ItemName,
                                                    WorkCategoryId = childOfChildWorkItem.WorkCategoryId,

                                                    ProjectEstimationWorkItemId = childOfChildWorkItem.ProjectEstimationWorkItemId,
                                                    ProjectInformationId = currentSubProject.ProjectInformationId,

                                                    TotalQuantityOfWork = childOfChildWorkItem.TotalQuantityOfWork,
                                                    DirectCost = childOfChildWorkItem.DirectCost,
                                                    MaterialCost = childOfChildWorkItem.MaterialCost,
                                                    LabourOutPut = childOfChildWorkItem.LabourOutPut,
                                                    EquipmentOutput = childOfChildWorkItem.EquipmentOutput,
                                                    MaterialCostPerUnit = childOfChildWorkItem.MaterialCostPerUnit,
                                                    LaborCostPerUnit = childOfChildWorkItem.LaborCostPerUnit,
                                                    EquipmentCostPerUnit = childOfChildWorkItem.EquipmentCostPerUnit,
                                                    TotalUnitCost = childOfChildWorkItem.TotalUnitCost,
                                                    TransportCostRate = currentWorkItem.TransportCostRate,
                                                    HasAdditionalTransportationCost = currentWorkItem.HasAdditionalTransportationCost,
                                                    CalculatedTransportCost = currentWorkItem.CalculatedTransportCost,
                                                    TotalUnitCostIncludingTransport = currentWorkItem.TotalUnitCostIncludingTransport,
                                                    OverHeadCost = childOfChildWorkItem.OverHeadCost,
                                                    ProfitCost = childOfChildWorkItem.ProfitCost,
                                                    ProjectType = childOfChildWorkItem.ProjectType,
                                                    Remark = childOfChildWorkItem.Remark,
                                                    LabourHourlyOutPut = childOfChildWorkItem.LabourHourlyOutPut,
                                                    UnitOfMeasurementId = childOfChildWorkItem.UnitOfMeasurementId,
                                                    OverheadRate = childOfChildWorkItem.OverheadRate,
                                                    ProfitMargin = childOfChildWorkItem.ProfitMargin,

                                                    ParentId = childWorkItemToAdd.GenericWorkItemPerProjectId,

                                                    OrgBranchName = childOfChildWorkItem.OrgBranchName,
                                                    DateCreated = DateTime.Now,
                                                    LastModified = childOfChildWorkItem.LastModified,
                                                    CreatedBy = User.Identity.Name,
                                                    ModifiedBy = childOfChildWorkItem.ModifiedBy,
                                                };
                                                db.GenericWorkItemPerProject.Add(childOfChildWorkItemToAdd);
                                                db.SaveChanges();
                                                CopyWorkItemResourceFromProject(childOfChildWorkItem.ProjectEstimationWorkItemId, childOfChildWorkItemToAdd.GenericWorkItemPerProjectId, childOfChildWorkItem.ContainsWorkItemAsInput);
                                            }
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        // current work item is not a sub-category ...
                        // so it probably has material, labor and equipment
                        else
                        {
                            CopyWorkItemResourceFromProject(currentWorkItem.GenericWorkItemPerProjectId, id, currentWorkItem.ContainsWorkItemAsInput);
                        }
                    }
                }
            }
        }
        public void CopyCurrentPieceRateFromProject(int itemId, int subProjTypeMappingId)
        {

            var currentWorkItem = db.GenericAreaWorkItem.Find(itemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        #region Remove if work item exists
                        var existingWorkItem = db.GenericAreaWorkItem
                                                .FirstOrDefault(w => w.ItemCode == currentWorkItem.ItemCode &&
                                                w.WorkItemName == currentWorkItem.WorkItemName &&
                                                w.SubProjectTypeId == currentSubProject.SubProjectTypeId &&
                                                w.WorkCategory.ConfigurationId == mapping.ConfigurationId);

                        if (existingWorkItem != null)
                        {
                            db.GenericAreaWorkItem.Remove(existingWorkItem);
                            db.SaveChanges();
                        }
                        #endregion

                        var workItemToAdd = new GenericAreaWorkItem
                        {
                            SubProjectTypeId = currentSubProject.SubProjectTypeId,
                            ItemCode = currentWorkItem.ItemCode,

                            WorkItemName = currentWorkItem.WorkItemName,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,

                            DirectCost = currentWorkItem.DirectCost,
                            MaterialCost = currentWorkItem.MaterialCost,
                            Area = currentWorkItem.Area,
                            CylinderLock = currentWorkItem.CylinderLock,
                            Height = currentWorkItem.Height,
                            OverHeadProfitCost = currentWorkItem.OverHeadProfitCost,
                            Price = currentWorkItem.Price,
                            TransportCost = currentWorkItem.TransportCost,
                            Width = currentWorkItem.Width,


                            OrgBranchName = currentWorkItem.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = currentWorkItem.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = currentWorkItem.ModifiedBy,
                        };
                        db.GenericAreaWorkItem.Add(workItemToAdd);
                        db.SaveChanges();

                    }
                }
            }
        }
        public void CopyCurrentSubContractWorkItemFromProject(int itemId, int subProjTypeMappingId)
        {
            var currentWorkItem = db.GenericSubContractWorkItem.Find(itemId);
            if (currentWorkItem != null)
            {
                var mapping = db.SubProjectStructureTypeMapping.Find(subProjTypeMappingId);
                if (mapping != null)
                {
                    // find the subProject ... block, station etc
                    var currentSubProject = db.SubProjectType.Find(mapping.SubProjectTypeId);
                    if (currentSubProject != null)
                    {
                        #region Remove if work item exists
                        var existingWorkItem = db.GenericSubContractWorkItem
                                                .FirstOrDefault(w => w.ItemCode == currentWorkItem.ItemCode &&
                                                w.WorkItemName == currentWorkItem.WorkItemName &&
                                                w.SubProjectTypeId == currentSubProject.SubProjectTypeId &&
                                                w.WorkCategory.ConfigurationId == mapping.ConfigurationId);

                        if (existingWorkItem != null)
                        {
                            db.GenericSubContractWorkItem.Remove(existingWorkItem);
                            db.SaveChanges();
                        }
                        #endregion

                        var workItemToAdd = new GenericSubContractWorkItem
                        {
                            SubProjectTypeId = currentSubProject.SubProjectTypeId,
                            ItemCode = currentWorkItem.ItemCode,

                            WorkItemName = currentWorkItem.WorkItemName,
                            WorkCategoryId = currentWorkItem.WorkCategoryId,
                            ProjectInformationId = currentSubProject.ProjectInformationId,

                            SubContractCostOne = currentWorkItem.SubContractCostOne,
                            SubContractCostTwo = currentWorkItem.SubContractCostTwo,
                            SubContractCostThree = currentWorkItem.SubContractCostThree,
                            SelectedCost = currentWorkItem.SelectedCost,
                            OverHeadProfit = currentWorkItem.OverHeadProfit,
                            TotalCost = currentWorkItem.TotalCost,
                            UnitOfMeasurementId = currentWorkItem.UnitOfMeasurementId,
                            Quntity = currentWorkItem.Quntity,


                            OrgBranchName = currentWorkItem.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = currentWorkItem.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = currentWorkItem.ModifiedBy,
                        };
                        db.GenericSubContractWorkItem.Add(workItemToAdd);
                        db.SaveChanges();

                    }
                }
            }
        }
        private void RemoveExistingWorkItemBeforeCopyNew(int workItemId)
        {
            var workItemToRemove = db.GenericWorkItemPerProject.Find(workItemId);
            if (workItemToRemove != null)
            {
                if (workItemToRemove.IsSubCategory)
                {
                    var childWorkItems = db.GenericWorkItemPerProject.Where(w => w.ParentId == workItemToRemove.GenericWorkItemPerProjectId).ToList();
                    if (childWorkItems.Any())
                    {
                        foreach (GenericWorkItemPerProject childWorkItemToRemove in childWorkItems)
                        {
                            RemoveExistingWorkItemBeforeCopyNew(childWorkItemToRemove.GenericWorkItemPerProjectId);
                        }
                    }

                }
                db.GenericWorkItemPerProject.Attach(workItemToRemove);
                db.GenericWorkItemPerProject.Remove(workItemToRemove);
                db.SaveChanges();
            }
        }
        private void CopyWorkItemResourceFromProject(int sourceId, int destinationId, bool sourceWorkItemContainsCustomItems)
        {
            var materials = db.GenericProjectMaterial
                .Where(m => m.GenericWorkItemPerProjectId == sourceId).ToList();
            var equipments = db.GenericProjectEquipment
                .Where(eq => eq.GenericWorkItemPerProjectId == sourceId).ToList();
            var labors = db.GenericProjectDirectLabour
                .Where(l => l.GenericWorkItemPerProjectId == sourceId).ToList();

            List<GenericProjectCustomWorkItemDetail> customDetails = new List<GenericProjectCustomWorkItemDetail>();
            if (sourceWorkItemContainsCustomItems)
            {
                var details = db.GenericProjectCustomWorkItemDetails.Where(d => d.GenericWorkItemPerProjectId == sourceId).ToList();
                if (details.Any())
                {
                    foreach (GenericProjectCustomWorkItemDetail itemDetail in details)
                    {
                        var customItemRecord = new GenericProjectCustomWorkItemDetail
                        {
                            QuantityPerUnit = itemDetail.QuantityPerUnit,
                            UnitCost = itemDetail.UnitCost,
                            TotalCost = itemDetail.TotalCost,
                            ProjectEstimationWorkItemId = itemDetail.ProjectEstimationWorkItemId,
                            GenericWorkItemPerProjectId = destinationId,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = itemDetail.OrgBranchName,
                        };
                        db.GenericProjectCustomWorkItemDetails.Add(customItemRecord);
                        db.SaveChanges();
                    }
                }
            }

            #region copy material
            if (materials.Any())
            {
                foreach (GenericProjectMaterial material in materials)
                {
                    var materialToAdd = new GenericProjectMaterial
                    {
                        ItemCode = material.ItemCode,
                        ItemCategoryCode = material.ItemCategoryCode,
                        Unit = material.Unit,
                        Quantity = material.Quantity,
                        UnitPrice = material.UnitPrice,
                        TotalCost = material.TotalCost,
                        MaterialUnitCost = material.MaterialUnitCost,
                        GenericWorkItemPerProjectId = destinationId,

                        OrgBranchName = material.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = material.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = material.ModifiedBy,
                    };
                    db.GenericProjectMaterial.Add(materialToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy labor
            if (labors.Any())
            {
                foreach (GenericProjectDirectLabour labor in labors)
                {
                    var laborToAdd = new GenericProjectDirectLabour
                    {
                        Unit = labor.Unit,
                        Quantity = labor.Quantity,
                        UnitFactory = labor.UnitFactory,
                        IndexedHourlySalary = labor.IndexedHourlySalary,
                        DailyPay = labor.DailyPay,
                        TotalCost = labor.TotalCost,
                        ManpowerUnitCost = labor.ManpowerUnitCost,
                        GenericWorkItemPerProjectId = destinationId,
                        ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,

                        OrgBranchName = labor.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = labor.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = labor.ModifiedBy,

                    };
                    db.GenericProjectDirectLabour.Add(laborToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
            #region copy equipment
            if (equipments.Any())
            {
                foreach (GenericProjectEquipment equipment in equipments)
                {
                    var equipmentToAdd = new GenericProjectEquipment
                    {
                        FleetsID = equipment.FleetsID,
                        Unit = equipment.Unit,
                        Quantity = equipment.Quantity,
                        UnitFactory = equipment.UnitFactory,
                        RentalCost = equipment.RentalCost,
                        TotalCost = equipment.TotalCost,
                        EquipmentUnitCost = equipment.EquipmentUnitCost,
                        EquipmentType = equipment.EquipmentType,
                        GenericWorkItemPerProjectId = destinationId,

                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = equipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = equipment.ModifiedBy,
                    };
                    db.GenericProjectEquipment.Add(equipmentToAdd);
                    db.SaveChanges();
                }
            }
            #endregion
        }

        private void UpdateChangedInformation()
        {
            var workItems = db.GenericWorkItemPerProject.Where(w => !w.IsSubCategory).ToList();
            if (workItems.Any())
            {
                foreach (GenericWorkItemPerProject workItem in workItems)
                {
                    var hourlyOutPut = workItem.LabourHourlyOutPut > 0 ? workItem.LabourHourlyOutPut : 1;
                    var totalMaterialCost = new decimal();
                    var totalEquipmentCost = new decimal();
                    var totalLaborCost = new decimal();
                    var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                    var equipments = db.GenericProjectEquipment.Where(e => e.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                    var labors = db.GenericProjectDirectLabour.Where(l => l.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
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

                    db.GenericWorkItemPerProject.Attach(workItem);
                    db.Entry(workItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        private void CopyWorkItemAndItsSubWorkItems(ProjectEstimationWorkItem templateWorkItem, SubProjectType destinationSubProjectType, int parentId)
        {
            var workItemToAdd = new GenericWorkItemPerProject
            {
                SubProjectTypeId = destinationSubProjectType.SubProjectTypeId,
                ItemCode = templateWorkItem.ItemCode,
                ContainsWorkItemAsInput = templateWorkItem.ContainsWorkItemAsInput,
                IsSubCategory = templateWorkItem.IsSubCategory,

                ItemName = templateWorkItem.ItemName,
                WorkCategoryId = templateWorkItem.WorkCategoryId,

                ProjectEstimationWorkItemId = templateWorkItem.ProjectEstimationWorkItemId,
                ProjectInformationId = destinationSubProjectType.ProjectInformationId,

                TotalQuantityOfWork = templateWorkItem.TotalQuantityOfWork,
                DirectCost = templateWorkItem.DirectCost,
                MaterialCost = templateWorkItem.MaterialCost,
                LabourOutPut = templateWorkItem.LabourOutPut,
                EquipmentOutput = templateWorkItem.EquipmentOutput,
                MaterialCostPerUnit = templateWorkItem.MaterialCostPerUnit,
                LaborCostPerUnit = templateWorkItem.LaborCostPerUnit,
                EquipmentCostPerUnit = templateWorkItem.EquipmentCostPerUnit,
                TotalUnitCost = templateWorkItem.TotalUnitCost,
                OverHeadCost = templateWorkItem.OverHeadCost,
                ProfitCost = templateWorkItem.ProfitCost,
                ProjectType = templateWorkItem.ProjectType,
                Remark = templateWorkItem.Remark,
                LabourHourlyOutPut = templateWorkItem.LabourHourlyOutPut,
                UnitOfMeasurementId = templateWorkItem.UnitOfMeasurementId,
                OverheadRate = templateWorkItem.OverheadRate,
                ProfitMargin = templateWorkItem.ProfitMargin,

                ParentId = parentId,

                OrgBranchName = templateWorkItem.OrgBranchName,
                DateCreated = DateTime.Now,
                LastModified = templateWorkItem.LastModified,
                CreatedBy = User.Identity.Name,
                ModifiedBy = templateWorkItem.ModifiedBy,
            };
            db.GenericWorkItemPerProject.Add(workItemToAdd);
            db.SaveChanges();

            if (templateWorkItem.IsSubCategory)
            {
                var childWorkItems = db.ProjectEstimationWorkItem
                                    .Where(wi => wi.ParentId == templateWorkItem.ProjectEstimationWorkItemId).ToList();
                if (childWorkItems.Any())
                {
                    foreach (ProjectEstimationWorkItem childWorkItem in childWorkItems)
                    {
                        this.CopyWorkItemAndItsSubWorkItems(childWorkItem, destinationSubProjectType, workItemToAdd.GenericWorkItemPerProjectId);
                    }
                }
            }
            // current work item is not a sub-category ...
            // so it probably has material, labor and equipment
            else
            {
                CopyWorkItemResourceFromTemplate(templateWorkItem.ProjectEstimationWorkItemId, workItemToAdd.GenericWorkItemPerProjectId, templateWorkItem.ContainsWorkItemAsInput);
            }
        }

        private void CreateUserProjectAssignmentOnProjectAdded(int projectId, string userName)
        {
            var entity = new ConstructionProjectUserAssignment
            {
                ProjectInformationId = projectId,
                UserName = userName,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now,
                ModifiedBy = User.Identity.Name,
                LastModified = DateTime.Now,
            };
            db.ConstructionProjectUserAssignments.Add(entity);
            db.SaveChanges();
        }
        public JsonResult DeleteAllResourcesUnderWorkItem(int id)
        {
            var currentWorkItem = db.GenericWorkItemPerProject.Find(id);
            if (currentWorkItem != null)
            {
                var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == id).ToList();
                if (materials.Any())
                {
                    db.GenericProjectMaterial.RemoveRange(materials);
                    db.SaveChanges();
                }
                var equipments = db.GenericProjectEquipment.Where(eq => eq.GenericWorkItemPerProjectId == id).ToList();
                if (equipments.Any())
                {
                    db.GenericProjectEquipment.RemoveRange(equipments);
                    db.SaveChanges();
                }
                var labors = db.GenericProjectDirectLabour.Where(eq => eq.GenericWorkItemPerProjectId == id).ToList();
                if (labors.Any())
                {
                    db.GenericProjectDirectLabour.RemoveRange(labors);
                    db.SaveChanges();
                }
                if (currentWorkItem.ContainsWorkItemAsInput)
                {
                    var customDetails = db.GenericProjectCustomWorkItemDetails.Where(gc => gc.GenericWorkItemPerProjectId == id).ToList();
                    if (customDetails.Any())
                    {
                        db.GenericProjectCustomWorkItemDetails.RemoveRange(customDetails);
                        db.SaveChanges();
                    }
                }
                var data = new
                {
                    Message = "All materials, labors and equipments under this item were cleaned successfully!",
                    Status = "Ok"
                };
                ProjectEstimationHelper.UpdateGenericWorkItem(id);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            {
                var data = new
                {
                    Message = "Item wasn't found!",
                    Status = "Error"
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult copyResourcesFromSourceItemToDestinationItem(string sourceId, string destinationId)
        {
            var idFrom = Convert.ToInt32(sourceId);
            var idTo = Convert.ToInt32(destinationId);
            if (idFrom == 0 || idTo == 0)
            {
                var data = new
                {
                    Message = "Nothing copied!",
                    Status = "Error"
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (idFrom == idTo)
            {
                var data = new
                {
                    Message = "Cannot copy to the source item itself!",
                    Status = "Error"
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var sourceWorkItem = db.GenericWorkItemPerProject.Find(idFrom);
                if (sourceWorkItem != null)
                {
                    var materials = db.GenericProjectMaterial.Where(m => m.GenericWorkItemPerProjectId == idFrom).ToList();
                    var equipments = db.GenericProjectEquipment.Where(eq => eq.GenericWorkItemPerProjectId == idFrom).ToList();
                    var labors = db.GenericProjectDirectLabour.Where(eq => eq.GenericWorkItemPerProjectId == idFrom).ToList();
                    if (sourceWorkItem.ContainsWorkItemAsInput)
                    {
                        var customDetails = db.GenericProjectCustomWorkItemDetails.Where(gc => gc.GenericWorkItemPerProjectId == idFrom).ToList();
                        foreach (GenericProjectCustomWorkItemDetail customWorkItemDetail in customDetails)
                        {
                            var existingEntity =
                                db.GenericProjectCustomWorkItemDetails.FirstOrDefault(gc => gc.GenericWorkItemPerProjectId == idTo &&
                                gc.ProjectEstimationWorkItemId == customWorkItemDetail.ProjectEstimationWorkItemId);
                            if (existingEntity == null)
                            {
                                var entity = new GenericProjectCustomWorkItemDetail
                                {
                                    GenericWorkItemPerProjectId = idTo,
                                    ProjectEstimationWorkItemId = customWorkItemDetail.ProjectEstimationWorkItemId,
                                    QuantityPerUnit = customWorkItemDetail.QuantityPerUnit,
                                    UnitCost = customWorkItemDetail.UnitCost,
                                    TotalCost = customWorkItemDetail.TotalCost,

                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,
                                };
                                db.GenericProjectCustomWorkItemDetails.Add(entity);
                                db.SaveChanges();
                            }
                        }
                    }
                    foreach (GenericProjectMaterial material in materials)
                    {
                        var existingEntity = db.GenericProjectMaterial.FirstOrDefault(m => m.GenericWorkItemPerProjectId == idTo &&
                        m.ItemCode == material.ItemCode);
                        if (existingEntity == null)
                        {
                            var entity = new GenericProjectMaterial
                            {
                                GenericWorkItemPerProjectId = idTo,
                                ItemCategoryCode = material.ItemCategoryCode,
                                ItemCode = material.ItemCode,
                                Unit = material.Unit,
                                Quantity = material.Quantity,
                                UnitPrice = material.UnitPrice,
                                MaterialUnitCost = material.MaterialUnitCost,
                                TotalCost = material.TotalCost,

                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                            };
                            db.GenericProjectMaterial.Add(entity);
                            db.SaveChanges();
                        }
                    }
                    foreach (GenericProjectEquipment equipment in equipments)
                    {
                        var existingEntity = db.GenericProjectEquipment.FirstOrDefault(m => m.GenericWorkItemPerProjectId == idTo &&
                        m.FleetsID == equipment.FleetsID);
                        if (existingEntity == null)
                        {
                            var entity = new GenericProjectEquipment
                            {
                                GenericWorkItemPerProjectId = idTo,
                                FleetsID = equipment.FleetsID,
                                Unit = equipment.Unit,
                                UnitFactory = equipment.UnitFactory,
                                Quantity = equipment.Quantity,
                                RentalCost = equipment.RentalCost,
                                EquipmentUnitCost = equipment.EquipmentUnitCost,
                                EquipmentType = equipment.EquipmentType,
                                TotalCost = equipment.TotalCost,

                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now,
                            };
                            db.GenericProjectEquipment.Add(entity);
                            db.SaveChanges();
                        }
                    }
                    foreach (GenericProjectDirectLabour labor in labors)
                    {
                        var existingEntity = db.GenericProjectDirectLabour.FirstOrDefault(m => m.GenericWorkItemPerProjectId == idTo &&
                        m.ProjectEstimationIndexLabourId == labor.ProjectEstimationIndexLabourId);
                        var entity = new GenericProjectDirectLabour
                        {
                            GenericWorkItemPerProjectId = idTo,
                            ProjectEstimationIndexLabourId = labor.ProjectEstimationIndexLabourId,
                            IndexedHourlySalary = labor.IndexedHourlySalary,
                            DailyPay = labor.DailyPay,
                            Quantity = labor.Quantity,
                            Unit = labor.Unit,
                            TotalCost = labor.TotalCost,
                            UnitFactory = labor.UnitFactory,
                            ManpowerUnitCost = labor.ManpowerUnitCost,

                            DateCreated = labor.DateCreated,
                            CreatedBy = User.Identity.Name,
                        };
                        db.GenericProjectDirectLabour.Add(entity);
                        db.SaveChanges();
                    }

                    ProjectEstimationHelper.UpdateGenericWorkItem(idTo);
                }
                var data = new
                {
                    Message = "Resources copied successfully!",
                    Status = "Ok"
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}
