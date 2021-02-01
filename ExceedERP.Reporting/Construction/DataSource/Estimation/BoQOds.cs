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

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class BoQOds : List<BoQSubProjectViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BoQSubProjectViewModel> GetAllBoQComponents(string projectId, string subProjectTypeId)
        {
            var list = new List<BoQSubProjectViewModel>();
            if (projectId == null)
            {
                return list;
            }
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var workItemsInProject = db.GenericWorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                var pieceRatesInProject = db.GenericAreaWorkItem.Where(pr => pr.ProjectInformationId == project.ProjectInformationId).ToList();
                var subContractWorkItemsInProject = db.GenericSubContractWorkItem.Where(sc => sc.ProjectInformationId == project.ProjectInformationId).ToList();

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
                        var subTypeWorkItems = workItemsInProject.Where(w => w.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                        var subTypePieceRates = pieceRatesInProject.Where(pr => pr.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                        var subTypeSubContractWorkItems = subContractWorkItemsInProject.Where(sc => sc.SubProjectTypeId == subType.SubProjectTypeId).ToList();

                        #region sub-type cost calculation
                        var subTypeCost = new decimal();
                        if (subTypeWorkItems.Any())
                        {
                            subTypeCost = subTypeWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                        }
                        if (subTypePieceRates.Any())
                        {
                            subTypeCost += subTypePieceRates.Sum(pr => pr.OverHeadProfitCost);
                        }
                        if (subTypeSubContractWorkItems.Any())
                        {
                            subTypeCost += subTypeSubContractWorkItems.Sum(sc => (sc.Quntity * sc.TotalCost));
                        }
                        #endregion

                        var subProjectTypeRecord = new BoQSubProjectViewModel
                        {
                            Id = subType.SubProjectTypeId,
                            Name = subType.Name,
                            TotalPrice = subTypeCost
                        };

                        #region structures
                        #region  LINQ for structure types under this very block
                        
                        #endregion
                        var mappings = db.SubProjectStructureTypeMapping.Where(sb => sb.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                        var idsHash = new HashSet<int>(mappings.Select(st => st.ConfigurationId));
                        var structureTypes = db.Configurations.Where(c => c.Reference == "STRUCTURE_TYPE" && idsHash.Contains(c.ConfigurationId)).ToList();
                        if (structureTypes.Any())
                        {
                            foreach (var structure in structureTypes)
                            {
                                var structureRecord = new BoQStructureTypeViewModel
                                {
                                    Name = structure.CurrentValue,
                                    Code = ConsSettingHelper.GetStructureTypeCode(structure.ConfigurationId, project.ProjectType),
                                };
                                structureRecord.Name = structureRecord.Name.Replace('_', ' ');

                                #region work categories under this very structure

                                var categoryIdsInWorkItems = new HashSet<int>(subTypeWorkItems.Select(itm => itm.WorkCategoryId));
                                var categoryIdsInPieceRates = new HashSet<int>(subTypePieceRates.Select(pr => pr.WorkCategoryId));
                                var categoryIdsInSubContractItems = new HashSet<int>(subTypeSubContractWorkItems.Select(sc => sc.WorkCategoryId));

                                var workCategoriesUnderStructure = db.WorkCategory.Where(wc =>
                                                wc.ConfigurationId == structure.ConfigurationId &&
                                                (categoryIdsInWorkItems.Contains(wc.WorkCategoryId) ||
                                                categoryIdsInSubContractItems.Contains(wc.WorkCategoryId) ||
                                                categoryIdsInPieceRates.Contains(wc.WorkCategoryId))).ToList();
                                if (workCategoriesUnderStructure.Any())
                                {
                                    foreach (WorkCategory category in workCategoriesUnderStructure)
                                    {
                                        #region total cost of work category calculation

                                        var categoryCost = new decimal();
                                        var categoryWorkItems = workItemsInProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                                        var categoryPieceRates = pieceRatesInProject.Where(pr => pr.WorkCategoryId == category.WorkCategoryId && pr.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                                        var categorySubContractWorkItmes = subContractWorkItemsInProject.Where(sc => sc.WorkCategoryId == category.WorkCategoryId && sc.SubProjectTypeId == subType.SubProjectTypeId).ToList();
                                        if (categoryWorkItems.Any())
                                        {
                                            categoryCost = categoryWorkItems.Sum(wi => (wi.TotalUnitCost * wi.TotalQuantityOfWork));
                                        }
                                        if (categoryPieceRates.Any())
                                        {
                                            categoryCost += categoryPieceRates.Sum(sc => sc.OverHeadProfitCost);
                                        }
                                        if (categorySubContractWorkItmes.Any())
                                        {
                                            categoryCost += categorySubContractWorkItmes.Sum(sc => (sc.Quntity * sc.TotalCost));
                                        }
                                        #endregion
                                        var categoryRecord = new BoQWorkCategoryViewModel
                                        {
                                            Name = category.Name,
                                            Code = category.Index,
                                            TotalPrice = categoryCost
                                        };
                                        #region add work items, piece rates and subcontract to boq
                                        foreach (GenericWorkItemPerProject workItem in categoryWorkItems)
                                        {
                                            var workItemRecord = new BoQWorkItemViewModel
                                            {
                                                Description = workItem.ItemName,
                                                Code = workItem.ItemCode,
                                                HasChildWorkItems = workItem.IsSubCategory,
                                                Quantity = workItem.TotalQuantityOfWork,
                                                Unit = ConsSettingHelper.GetUnitOfMearurnment(workItem.UnitOfMeasurementId ?? 0),
                                                UnitRate = workItem.TotalUnitCost,

                                            };
                                            workItemRecord.TotalAmount = workItemRecord.Quantity * workItemRecord.UnitRate;
                                            categoryRecord.WorkItemList.Add(workItemRecord);

                                        }
                                        foreach (GenericAreaWorkItem areaWorkItem in categoryPieceRates)
                                        {
                                            var pieceRateRecord = new BoQWorkItemViewModel
                                            {
                                                Description = areaWorkItem.WorkItemName,
                                                Code = areaWorkItem.ItemCode,
                                                //Unit = ConsSettingHelper.GetUnitOfMearurnment(areaWorkItem.Uni)
                                                Quantity = 1,
                                                UnitRate = areaWorkItem.OverHeadProfitCost,
                                                TotalAmount = areaWorkItem.OverHeadProfitCost
                                            };
                                            categoryRecord.WorkItemList.Add(pieceRateRecord);
                                        }
                                        foreach (GenericSubContractWorkItem subWorkItem in categorySubContractWorkItmes)
                                        {
                                            var subContractRecord = new BoQWorkItemViewModel
                                            {
                                                Description = subWorkItem.WorkItemName,
                                                Code = subWorkItem.ItemCode,
                                                Unit = ConsSettingHelper.GetUnitOfMearurnment(subWorkItem.UnitOfMeasurementId),
                                                Quantity = subWorkItem.Quntity,
                                                UnitRate = subWorkItem.TotalCost,
                                                TotalAmount = subWorkItem.TotalCost * subWorkItem.Quntity
                                            };
                                            categoryRecord.WorkItemList.Add(subContractRecord);
                                        } 
                                        #endregion
                                        //if (categoryRecord.TotalPrice > 0)
                                        //{
                                        structureRecord.CategoryList.Add(categoryRecord);
                                        //}
                                    }
                                }
                                #endregion
                                if (structureRecord.CategoryList.Any())
                                {
                                    subProjectTypeRecord.StructureTypes.Add(structureRecord);
                                }
                            }
                        }
                        #endregion
                        // add the block to the list
                        list.Add(subProjectTypeRecord);
                    }
                }
                #endregion

                //list.Add(record);
                //}
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
                headerDs.Consultant = item.Consultant;
                headerDs.Client = item.Client;
                headerDs.Location = this.GetBranchName(item.BranchId);

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

        private string GetBranchName(int id)
        {
            var name = "";
            var currentBranch = db.Branch.Find(id);
            if (currentBranch != null)
            {
                name = currentBranch.Name;
            }
            return name;
        }
    }
}
