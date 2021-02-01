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
    public class EarnedValueAnalysisPerItemOds
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EVMPeriodViewModel> GetEVMPerItem(string projectId, string year, string periodId)
        {
            var list = new List<EVMPeriodViewModel>();

            int id = Convert.ToInt32(projectId);
            int pId = Convert.ToInt32(periodId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();
            if (project != null)
            {

                var evms = db.EarnedValuePerItem.Where(evm => evm.Period.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();
                if (evms.Any())
                {
                    if (year != null)
                    {
                        evms = evms.Where(e => e.GlFiscalYear == year).ToList();

                    }
                    if (periodId != null)
                    {
                        evms = evms.Where(e => e.EarnedValueManagementPeriodId == pId).ToList();
                    }

                    var ids = new HashSet<int>(evms.Select(ev => ev.EarnedValueManagementPeriodId));
                    var periods = db.EarnedValueManagementPeriods.Where(p => (ids.Contains(p.EarnedValueManagementPeriodId))).ToList();
                    if (periods.Any())
                    {
                        foreach (EarnedValueManagementPeriod period in periods)
                        {
                            var record = new EVMPeriodViewModel
                            {
                                Name = ConstructionInputsHelper.GetConstructionPeriod(period.ConstructionPeriodId)
                            };
                            var evmsUnderPeriod = evms.Where(e => e.EarnedValueManagementPeriodId == period.EarnedValueManagementPeriodId).ToList();
                            if (evmsUnderPeriod.Any())
                            {
                                foreach (EarnedValuePerItem evPerItem in evmsUnderPeriod)
                                {
                                    var itemRecord = new EVMPerItemViewModel
                                    {
                                        WorkDescription = ProjectEstimationHelper.GetConcatnatedWorkItemDescription(evPerItem.WorkItemPerProjectId),
                                        Unit = evPerItem.Unit,
                                        BudgetedQuantity = evPerItem.BudgetedQuantity,
                                        PlannedQuanity = evPerItem.PlannedQuanity,
                                        ExecutedQuantity = evPerItem.ExecutedQuantity,
                                        EstimatedUnitPrice = evPerItem.EstimatedUnitPrice,
                                        ActualUnitPrice = evPerItem.ActualUnitPrice,

                                        BAC = evPerItem.BAC,
                                        BCWS = evPerItem.BCWS,
                                        BCWP = evPerItem.BCWP,
                                        ACWP = evPerItem.ACWP,
                                        PCP = evPerItem.PCP,
                                        PCA = evPerItem.PCA,
                                        CV = evPerItem.CV,
                                        CVPercentage = evPerItem.CVPercentage,
                                        CPI = evPerItem.CPI,
                                        SV = evPerItem.SV,
                                        SVPercentage = evPerItem.SVPercentage,
                                        SPI = evPerItem.SPI,
                                        CR = evPerItem.CR,
                                        TCPI = evPerItem.TCPI,
                                        TSPI = evPerItem.TSPI,
                                        EAC = evPerItem.EAC,
                                        VAC = evPerItem.VAC,
                                        ETC = evPerItem.ETC,
                                    };
                                    record.EVMList.Add(itemRecord);
                                }
                            }
                            list.Add(record);
                        }
                    }
                }
            }

            return list;
        }
        

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReportYearViewModel> GetAvailableYearsPerItemAnalysis(string projectId)
        {
            var list = new List<ReportYearViewModel>();
            int id = Convert.ToInt32(projectId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var yearlyEarnedValueAnalysisList = db.EarnedValuePerItem.Where(evm => evm.Period.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();
                if (yearlyEarnedValueAnalysisList.Any())
                {
                    var yearsRawList = new HashSet<string>(yearlyEarnedValueAnalysisList.Select(yv => yv.GlFiscalYear));
                    foreach (var year in yearsRawList)
                    {
                        var record = new ReportYearViewModel
                        {
                            Title = year
                        };
                        list.Add(record);
                    }
                }
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EVMPerItemPeriodViewModel> GetPeriodsPerItemAnalysis(string projectId, string year)
        {
            var list = new List<EVMPerItemPeriodViewModel>();
            int id = Convert.ToInt32(projectId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var evms = db.EarnedValuePerItem.Where(evm => evm.Period.ProjectInformationId == project.ProjectInformationId && evm.GlFiscalYear == year).OrderBy(ev => ev.GlFiscalYear).ToList();
                if (evms.Any())
                {
                    var ids = new HashSet<int>(evms.Select(ev => ev.EarnedValueManagementPeriodId));
                    var periods = db.EarnedValueManagementPeriods.Where(p => (ids.Contains(p.EarnedValueManagementPeriodId))).ToList();
                    if (periods.Any())
                    {
                        foreach (EarnedValueManagementPeriod period in periods)
                        {
                            var record = new EVMPerItemPeriodViewModel
                            {
                                Id = period.EarnedValueManagementPeriodId,
                                Name = ConstructionInputsHelper.GetConstructionPeriod(period.ConstructionPeriodId)
                            };
                            list.Add(record);
                        }
                    }
                }
            }
            return list;
        }
        //public List<GLFiscalYearViewModel> GetEVMYears(string projectId)
        //{
        //    List<GLFiscalYearViewModel> list = new List<GLFiscalYearViewModel>();
        //    int id = Convert.ToInt32(projectId);

        //    var currentProject = db.ProjectInformation.Find(id);
        //    if (currentProject != null)
        //    {
        //        var joinedVal = (from ep in db.EarnedValueManagementPeriods
        //                         join cp in db.ConstructionPeriod
        //                         on ep.ConstructionPeriodId equals cp.ConstructionPeriodId
        //                         join gp in db.GLPeriods
        //                         on cp.Name equals gp.Name
        //                         where ep.ProjectInformationId == id
        //                         select new
        //                         {
        //                             GlFiscalYearId = gp.GlFiscalYearId
        //                         }).ToList();
        //        var ids = new HashSet<int>(joinedVal.Select(y => y.GlFiscalYearId));
        //        var yearsWithVales = db.GLFiscalYears.Where(y => (ids.Contains(y.GlFiscalYearId))).ToList();
        //        if (yearsWithVales.Any())
        //        {
        //            foreach (var year in yearsWithVales)
        //            {
        //                var yearRecord = new GLFiscalYearViewModel
        //                {
        //                    Id = year.GlFiscalYearId,
        //                    Name = year.Name
        //                };
        //                list.Add(yearRecord);
        //            }
        //        }
        //    }
        //    return list;
        //}
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
