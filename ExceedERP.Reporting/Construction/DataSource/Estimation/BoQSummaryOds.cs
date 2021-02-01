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
    public class BoQSummaryOds : List<BoQSummarySubProjectViewModel>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BoQSummarySubProjectViewModel> GetAllSubProjects(string projectId, string subProjectTypeId)
        {
            var list = new List<BoQSummarySubProjectViewModel>();
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

                #region Sub-Projects block or ..
                int subTypeId = Convert.ToInt32(subProjectTypeId);
                var subprojectTypes = db.SubProjectType.Where(b => b.ProjectInformationId == id).ToList();
                if (subProjectTypeId != null)
                {
                    subprojectTypes = subprojectTypes.Where(s => s.SubProjectTypeId == subTypeId).ToList();
                }
                if (subprojectTypes.Any())
                {
                    foreach (SubProjectType subProjectType in subprojectTypes)
                    {
                        #region  cost calculation
                        var subTypeCost = new decimal();
                        var subTypeWorkItems = workItemsInProject.Where(w => w.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        var subTypePieceRates = pieceRatesInProject.Where(pr => pr.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        var subTypeSubContractWorkItems = subContractWorkItemsInProject.Where(sc => sc.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();

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

                        var subProjectTypeRecord = new BoQSummarySubProjectViewModel
                        {
                            Id = subProjectType.SubProjectTypeId,
                            Name = subProjectType.Name,
                            TotalPrice = subTypeCost
                        };

                        #region structures
                        #region  LINQ for structure types under this very block
                        //var structureTypes = (from c in db.Configurations
                        //                      join w in db.ProjectTypeMappings
                        //                          on c.ConfigurationId equals w.ConfigurationId

                        //                      join p in db.ProjectInformation
                        //                          on w.ProjectType equals p.ProjectType
                        //                      join st in db.SubProjectType
                        //                     on p.ProjectInformationId equals st.ProjectInformationId

                        //                      where
                        //                     p.ProjectInformationId == id &&
                        //                      c.Reference == "STRUCTURE_TYPE" &&
                        //                      st.SubProjectTypeId == subProjectTypeRecord.Id
                        //                      select (new
                        //                      {
                        //                          c.CurrentValue,
                        //                          c.ConfigurationId
                        //                      })).ToList();
                        #endregion
                        var mappings = db.SubProjectStructureTypeMapping.Where(sb => sb.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                        var idsHash = new HashSet<int>(mappings.Select(st => st.ConfigurationId));
                        var structureTypes = db.Configurations.Where(c => c.Reference == "STRUCTURE_TYPE" && idsHash.Contains(c.ConfigurationId)).ToList();
                        if (structureTypes.Any())
                        {
                            foreach (var structure in structureTypes)
                            {
                                var structureRecord = new BoQSummaryStructureTypeViewModel
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
                                        var categoryWorkItems = workItemsInProject.Where(w => w.WorkCategoryId == category.WorkCategoryId && w.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                                        var categoryPieceRates = pieceRatesInProject.Where(pr => pr.WorkCategoryId == category.WorkCategoryId && pr.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();
                                        var categorySubContractWorkItmes = subContractWorkItemsInProject.Where(sc => sc.WorkCategoryId == category.WorkCategoryId && sc.SubProjectTypeId == subProjectType.SubProjectTypeId).ToList();

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
                                        var categoryRecord = new BoQSummaryWorkCategoryViewModel
                                        {
                                            Name = category.Name,
                                            Code = category.Index,
                                            TotalPrice = categoryCost
                                        };
                                        structureRecord.CategoryList.Add(categoryRecord);
                                    }
                                }
                                #endregion

                                subProjectTypeRecord.StructureTypes.Add(structureRecord);
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
    [DataObject]
    public class ProjectBlocksOds : List<ProjectBlockVM>
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProjectBlockVM> GetAllBlocks(string projectId)
        {
            var list = new List<ProjectBlockVM>();
            int id = Convert.ToInt32(projectId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var blocks = db.Blocks.Where(b => b.ProjectInformationId == project.ProjectInformationId).ToList();
                if (blocks.Any())
                {
                    //var yearsRawList = new HashSet<string>(yearlyEarnedValueAnalysisList.Select(yv => yv.Year));
                    foreach (Block block in blocks)
                    {
                        var record = new ProjectBlockVM
                        {
                            Id = block.BlockId,
                            Name = block.Name
                        };
                        list.Add(record);
                    }
                }
            }
            return list;
        }
    }
}
