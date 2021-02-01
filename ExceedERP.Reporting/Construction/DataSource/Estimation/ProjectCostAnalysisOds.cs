using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class ProjectCostAnalysisOds : List<ProjectCostAnalysisSubProjectTypeVM>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectCostAnalysisSubProjectTypeVM> GetProjectCostAnalysis(string projectId, string subProjectTypeId)
        {
            List<ProjectCostAnalysisSubProjectTypeVM> list = new List<ViewModel.ProjectCostAnalysisSubProjectTypeVM>();
            if (projectId == null)
            {
                return null;
            }
            var id = Convert.ToInt32(projectId);
            var currentProject = db.ProjectInformation.Find(id);
            if (currentProject != null)
            {
                var allWorkItemsUnderProject = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == id).ToList();
                var allMaterialsUnderProject = db.GenericProjectMaterial.Where(m => m.WorkItem.ProjectInformationId == id).ToList();
                var allLaborsUnderProject = db.GenericProjectDirectLabour.Where(l => l.WorkItem.ProjectInformationId == id).ToList();
                var allEquipmentsUnderProject = db.GenericProjectEquipment.Where(e => e.WorkItem.ProjectInformationId == id).ToList();

                #region Sub-Project types or ..
                int subId = Convert.ToInt32(subProjectTypeId);
                var subtypes = db.SubProjectType.Where(b => b.ProjectInformationId == id).ToList();
                if (subProjectTypeId != null)
                {
                    subtypes = subtypes.Where(s => s.SubProjectTypeId == subId).ToList();
                }
                if (subtypes.Any())
                {
                    foreach (SubProjectType subType in subtypes)
                    {
                       

                        var subProjectTypeRecord = new ProjectCostAnalysisSubProjectTypeVM
                        {
                            Name = subType.Name,
                        };

                        #region structures
                        #region  LINQ for structure types under this very sub-project type
                        var structureTypes = (from c in db.Configurations
                                              join w in db.ProjectTypeMappings
                                                  on c.ConfigurationId equals w.ConfigurationId

                                              join p in db.ProjectInformation
                                                  on w.ProjectType equals p.ProjectType
                                              join st in db.SubProjectType
                                             on p.ProjectInformationId equals st.ProjectInformationId

                                              where
                                             p.ProjectInformationId == id &&
                                              c.Reference == "STRUCTURE_TYPE" &&
                                              st.SubProjectTypeId == subType.SubProjectTypeId
                                              select (new
                                              {
                                                  c.CurrentValue,
                                                  c.ConfigurationId
                                              })).ToList();
                        #endregion
                        if (structureTypes.Any())
                        {
                            foreach (var structure in structureTypes)
                            {
                                var structureRecord = new ProjectCostAnalysisStructureTypeVM
                                {
                                    Name = structure.CurrentValue
                                };
                                structureRecord.Name = structureRecord.Name.Replace('_', ' ');
                                #region work categories under this very structure
                                var workCategoriesUnderStructure = db.WorkCategory.Where(wc => wc.ConfigurationId == structure.ConfigurationId).ToList();
                                if (workCategoriesUnderStructure.Any())
                                {
                                    foreach (WorkCategory category in workCategoriesUnderStructure)
                                    {
                                        
                                        //#endregion
                                        var categoryRecord = new ProjectCostAnalysisWorkCategoryVM
                                        {
                                            Name = category.Name,
                                        };
                                        //var categoryWorkItems = db.GenericWorkItemPerProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                                        var categoryWorkItems = allWorkItemsUnderProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                                        foreach (GenericWorkItemPerProject workItem in categoryWorkItems)
                                        {
                                            var workItemRecord = new ProjectCostAnalysisWorkItemVM
                                            {
                                                Name = workItem.ItemName,
                                                HourlyOutput = workItem.LabourHourlyOutPut,
                                                TotalQuantity = workItem.TotalQuantityOfWork,
                                                Unit = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                                MaterialCostPerUnit = workItem.MaterialCostPerUnit,
                                                LaborCostPerUnit = workItem.LaborCostPerUnit,
                                                EquipmentCostPerUnit = workItem.EquipmentCostPerUnit,
                                                ProfitCost = workItem.ProfitCost,
                                                OverHeadCost = workItem.OverHeadCost,
                                                DirectCost = workItem.DirectCost,
                                                TotalUnitCost = workItem.TotalUnitCost,
                                            };
                                            #region resources ... material, labor and equipment
                                            var materials = allMaterialsUnderProject.Where(m => m.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                            var equipments = allEquipmentsUnderProject.Where(e => e.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                            var labors = allLaborsUnderProject.Where(l => l.GenericWorkItemPerProjectId == workItem.GenericWorkItemPerProjectId).ToList();
                                            foreach (GenericProjectMaterial material in materials)
                                            {
                                                var matRecord = new ProjectCostAnalysisMaterialVM
                                                {
                                                    Name = material.Item.Name,
                                                    Quantity = material.Quantity,
                                                    Unit = material.Unit,
                                                    UnitPrice = material.UnitPrice,
                                                    TotalCost = material.TotalCost,
                                                };
                                                workItemRecord.MaterialList.Add(matRecord);
                                            }
                                            foreach (GenericProjectEquipment equipment in equipments)
                                            {
                                                var eqpRecord = new ProjectCostAnalysisEquipmentVM
                                                {
                                                    Name = equipment.Fleet.EquipmentName,
                                                    Quantity = equipment.Quantity,
                                                    UnitFactor = equipment.UnitFactory,
                                                    HourlyRentalCost = equipment.RentalCost,
                                                    HourlyCost = equipment.TotalCost,
                                                };
                                                workItemRecord.EquipmentList.Add(eqpRecord);
                                            }
                                            foreach (GenericProjectDirectLabour labor in labors)
                                            {
                                                var lbrRecord = new ProjectCostAnalysisLaborVM
                                                {
                                                    Title = ConstructionInputsHelper.GetJobTitleName(labor.ProjectEstimationIndexLabour.ProjectJobTitleId),
                                                    HourlySalary = labor.IndexedHourlySalary,
                                                    Quantity = Math.Round(labor.Quantity, 0),
                                                    UnitFactor = labor.UnitFactory,
                                                    TotalHourlyCost = labor.TotalCost
                                                };
                                                workItemRecord.LaborList.Add(lbrRecord);
                                            }
                                            #endregion
                                            workItemRecord.TotalMaterialCost = workItemRecord.MaterialList.Sum(m => m.TotalCost);
                                            workItemRecord.TotalLaborCost = workItemRecord.LaborList.Sum(l => l.TotalHourlyCost);
                                            workItemRecord.TotalEquipmentCost = workItemRecord.EquipmentList.Sum(e => e.HourlyCost);
                                            categoryRecord.WorkItemList.Add(workItemRecord);

                                        }
                                        if (categoryRecord.WorkItemList.Any())
                                        {
                                            structureRecord.WorkCategoryList.Add(categoryRecord);
                                        }
                                    }
                                }
                                #endregion
                                if (structureRecord.WorkCategoryList.Any())
                                {
                                    subProjectTypeRecord.StructureList.Add(structureRecord);
                                }
                            }
                        }
                        #endregion
                        // add the block to the list
                        list.Add(subProjectTypeRecord);
                    }
                }
                #endregion
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return headerDs;
        }
    }
}
