using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Construction.Extensions;
using ExceedERP.Reporting.Construction.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Construction.DataSource
{
    [DataObject]
    public class EVMPerProjectGraphicalSummaryOds
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EVMProjectGraphicalSummaryVM> GetProjectSummary(string projectId)
        {
            List<EVMProjectGraphicalSummaryVM> list = new List<EVMProjectGraphicalSummaryVM>();

            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            if (project != null)
            {
                if (project.ProjectType != AvailableProjectTypes.Road)
                {
                    var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();

                    if (yearlyEarnedValueAnalysisList.Any())
                    {
                        var sortedRecords = EarnedValueManagementHelper.GetSortedValue(yearlyEarnedValueAnalysisList.ToList());
                        var arrayOfRecords = sortedRecords.ToArray();
                        for (int i = 0; i < sortedRecords.Count; i++)
                        {
                            var ev = arrayOfRecords[i];
                            var recordToAdd = new EVMProjectGraphicalSummaryVM
                            {
                                ConcatnatedMonth = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                Order = i,
                                BCWS = ev.BCWS,
                                BCWSCummulative = ev.BCWS,
                                BCWP = ev.BCWP,
                                BCWPCummulative = ev.BCWP,
                                ACWP = ev.ACWP,
                                ACWPCummulative = ev.ACWP
                            };
                            list.Add(recordToAdd);
                        }

                    }
                }
                else
                {
                    var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();

                    if (yearlyEarnedValueAnalysisList.Any())
                    {
                        var sortedRecords = EarnedValueManagementHelper.GetSortedValue(yearlyEarnedValueAnalysisList.ToList());
                        var arrayOfRecords = sortedRecords.ToArray();
                        for (int i = 0; i < sortedRecords.Count; i++)
                        {
                            var ev = arrayOfRecords[i];
                            var prevRecArray = arrayOfRecords.Slice(0, i);
                            if (prevRecArray.Length > 0)
                            {
                                var prevRecList = prevRecArray.ToList();
                                var recordToAdd = new EVMProjectGraphicalSummaryVM
                                {
                                    ConcatnatedMonth = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                    Order = i,
                                    BCWS = ev.BCWS,
                                    BCWSCummulative = prevRecList.Sum(v => v.BCWS) + ev.BCWS,
                                    BCWP = ev.BCWP,
                                    BCWPCummulative = prevRecList.Sum(v => v.BCWP) + ev.BCWP,
                                    ACWP = ev.ACWP,
                                    ACWPCummulative = prevRecList.Sum(v => v.ACWP) + ev.ACWP
                                };
                                list.Add(recordToAdd);
                            }
                            else
                            {
                                var recordToAdd = new EVMProjectGraphicalSummaryVM
                                {
                                    ConcatnatedMonth = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                    Order = i,
                                    BCWS = ev.BCWS,
                                    BCWSCummulative = ev.BCWS,
                                    BCWP = ev.BCWP,
                                    BCWPCummulative = ev.BCWP,
                                    ACWP = ev.ACWP,
                                    ACWPCummulative = ev.ACWP
                                };
                                list.Add(recordToAdd);
                            }
                        }

                    }
                }
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
