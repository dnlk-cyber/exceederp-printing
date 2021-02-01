using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class RoadGrandSummaryOds : List<RoadGrandSummaryProjectViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RoadGrandSummaryProjectViewModel> GetRoadGrandSummary(string projectId)
        {
            List<RoadGrandSummaryProjectViewModel> list = new List<RoadGrandSummaryProjectViewModel>();
            var projects = _db.ProjectInformation.Where(p => p.ProjectType == AvailableProjectTypes.Road).ToList();
            int id = Convert.ToInt32(projectId);

            if (projectId != null)
            {
                projects = projects.Where(p => p.ProjectInformationId == id).ToList();
            }
           
            foreach(var project in  projects)
            {
                var workItemsInProject = _db.GenericWorkItemPerProject.Where(p => p.ProjectInformationId == project.ProjectInformationId).ToList();
                var pieceRatesInProject = _db.GenericAreaWorkItem.Where(pr => pr.ProjectInformationId == project.ProjectInformationId).ToList();
                var subContractWorkItemsInProject = _db.GenericSubContractWorkItem.Where(sc => sc.ProjectInformationId == project.ProjectInformationId).ToList();

                var categoryIdsInWorkItems = new HashSet<int>(workItemsInProject.Select(w => w.WorkCategoryId));
                var categoryIdsInPiecRates = new HashSet<int>(pieceRatesInProject.Select(w => w.WorkCategoryId));
                var categoryIdsInSubContractWorkItems = new HashSet<int>(subContractWorkItemsInProject.Select(w => w.WorkCategoryId));
                var categoriesUnderThisProject = 
                    _db.WorkCategory.Where(c => categoryIdsInWorkItems.Contains(c.WorkCategoryId) ||
                                                categoryIdsInPiecRates.Contains(c.WorkCategoryId) ||
                                                categoryIdsInSubContractWorkItems.Contains(c.WorkCategoryId)
                                                ).ToList();
                var record = new RoadGrandSummaryProjectViewModel
                {
                    Name = project.Name
                };
                var subProjectTypes = _db.SubProjectType.Where(b => b.ProjectInformationId == project.ProjectInformationId).ToList();
                var subProjectTypeIds = new HashSet<int>(subProjectTypes.Select(s => s.SubProjectTypeId));
                #region  LINQ for structure types under this very block
                var structureTypes = (from c in _db.Configurations
                                      join w in _db.ProjectTypeMappings
                                          on c.ConfigurationId equals w.ConfigurationId

                                      join p in _db.ProjectInformation
                                          on w.ProjectType equals p.ProjectType
                                      join st in _db.SubProjectType
                                     on p.ProjectInformationId equals st.ProjectInformationId

                                      where
                                     p.ProjectInformationId == project.ProjectInformationId &&
                                      c.Reference == "STRUCTURE_TYPE" &&
                                      subProjectTypeIds.Contains(st.SubProjectTypeId)
                                      //st.SubProjectTypeId == subProjectTypeRecord.Id
                                      select (new
                                      {
                                          c.CurrentValue,
                                          c.ConfigurationId
                                      })).ToList();
                #endregion
                if (structureTypes.Any())
                {
                    foreach(var str in structureTypes)
                    {
                        var categoriesInThisStructre = categoriesUnderThisProject.Where(c => c.ConfigurationId == str.ConfigurationId).ToList();
                        var catIds = new HashSet<int>(categoriesInThisStructre.Select(c => c.WorkCategoryId)).ToList();

                        var workItemsUnderGivenStructreAndProject = workItemsInProject.Where(w => catIds.Contains(w.WorkCategoryId)).ToList();
                        var pieceRatesUnderGivenStructreAndProject = pieceRatesInProject.Where(w => catIds.Contains(w.WorkCategoryId)).ToList();
                        var subContractWorkItemsUnderGivenStructreAndProject = subContractWorkItemsInProject.Where(w => catIds.Contains(w.WorkCategoryId)).ToList();

                        var totalCostOfStructure = new decimal();
                        if (workItemsUnderGivenStructreAndProject.Any())
                        {
                            totalCostOfStructure = workItemsUnderGivenStructreAndProject.Sum(w => (w.TotalQuantityOfWork + w.AddedQuantityOfWork - w.OmittedQuantityOfWork) * w.TotalUnitCost);
                        }
                        if (pieceRatesUnderGivenStructreAndProject.Any())
                        {
                            totalCostOfStructure += pieceRatesUnderGivenStructreAndProject.Sum(p => p.OverHeadProfitCost);
                        }
                        if (subContractWorkItemsUnderGivenStructreAndProject.Any())
                        {
                            totalCostOfStructure += subContractWorkItemsUnderGivenStructreAndProject.Sum(sc => sc.Quntity * sc.TotalCost);
                        }
                        var strRecord = new RoadGrandSummaryStructureTypeViewModel
                        {
                            Name = str.CurrentValue,
                            Code = ConsSettingHelper.GetStructureTypeCode(str.ConfigurationId, AvailableProjectTypes.Road),
                            TotalCost = totalCostOfStructure
                        };
                        record.StructuresList.Add(strRecord);
                    }
                   
                }
                if (record.StructuresList.Any())
                {
                    list.Add(record);
                }
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProjectHeaderVM GetProjectHeader(string projectId)
        {
            int id = Convert.ToInt32(projectId);
            var item = _db.ProjectInformation.Find(id);
            ProjectHeaderVM headerDs = new ProjectHeaderVM();
            var organization = _db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
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
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
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
