using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class SupplementaryWorksController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        // GET: Construction/SupplementaryWorks
        public ActionResult Index()
        {
       
            db.Configuration.ProxyCreationEnabled = false;

            //var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            //ViewData["PeriodList"] = periods;

            ////ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            //var branches = db.Branch.ToList();
            //ViewData["Branch"] = branches;

            var types = db.Configurations.Where(x => x.Reference == "STRUCTURE_TYPE").ToList();
            ViewData["StrTypes"] = types;

            //var lis = db.Configurations.Where(x => x.Reference == ConstructionSettingCategories.STRUCTURE_TYPE.ToString()).ToList();
            //ViewData["StructureType"] = lis;

            //var workItemCategories = from w in db.WorkCategory
            //                         join p in db.ProjectTypeMappings
            //                             on w.ConfigurationId equals p.ConfigurationId
            //                         select (new { w.WorkCategoryId, w.ConfigurationId, w.Description, w.Name, w.Index });

            ViewData["Categories"] = db.WorkCategory.Select(c => new { c.WorkCategoryId, c.Name, c.ConfigurationId }).ToList();

            var workItems = db.ProjectEstimationWorkItem.Select(es => new { es.ItemName, es.ProjectEstimationWorkItemId }).ToList();
            ViewData["WorkItems"] = workItems;

            ViewData["uom"] = db.UnitOfMeasurement.Select(x => new { x.UnitOfMeasurementId, x.Abbreviation }).ToList();

            ViewData["ItemCategorys"] = db.ItemCategorys.Select(p => new { p.ItemCategoryCode, p.Name }).ToList();

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
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projects = db.ProjectInformation.Where(x => x.ProjectStatus != ProjectStatus.CLOSED && !x.IsDeleted && ids.Contains(x.ProjectInformationId));            
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
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
        /// <summary>
        /// add work item as a variation work item
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="subProjTypeMappingId"></param>
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
                            OverHeadCost = currentWorkItem.OverHeadCost,
                            ProfitCost = currentWorkItem.ProfitCost,
                            ProjectType = currentWorkItem.ProjectType,
                            Remark = currentWorkItem.Remark,
                            LabourHourlyOutPut = currentWorkItem.LabourHourlyOutPut,
                            UnitOfMeasurementId = currentWorkItem.UnitOfMeasurementId,
                            OverheadRate = currentWorkItem.OverheadRate,
                            ProfitMargin = currentWorkItem.ProfitMargin,

                            ParentId = currentWorkItem.ParentId,
                            IsSupplementaryWorkItem = true,


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
                            var childWorkItems = db.ProjectEstimationWorkItem
                                .Where(wi => wi.ParentId == currentWorkItem.ProjectEstimationWorkItemId).ToList();
                            if (childWorkItems.Any())
                            {
                                foreach (ProjectEstimationWorkItem childWorkItem in childWorkItems)
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


        private string GetTitle(int id)
        {
            var pJTitle = db.ProjectJobTitles.Where(x => x.ProjectJobTitleId == id).FirstOrDefault();
            if (pJTitle != null)
            {
                return pJTitle.JobTitle;
            }
            return "";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}