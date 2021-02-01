using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
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
    public class EarnedValueAnalysisGraphOds : List<EVMMonthlyValuesViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EVMMonthlyValuesViewModel> GetMonthlyEarnedValueAnalysis(string projectId, string yearName)
        {
            var list = new List<EVMMonthlyValuesViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                //var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).ToList();
                //if (yearlyEarnedValueAnalysisList.Any())
                //{
                //    var yearlyValues = yearlyEarnedValueAnalysisList.Where(yrv => yrv.Year == yearName);
                //    if (yearlyValues.Any())
                //    {
                //        var monthsList = new HashSet<Months>(yearlyValues.Select(yv => yv.Month));
                //        if (monthsList.Any())
                //        {
                //            foreach (var month in monthsList)
                //            {
                //                var monthName = month.ToString();
                //                var monthIntValue = (int)month;
                //                var monthlyValue = yearlyValues.Where(yv => yv.Month == month).FirstOrDefault();
                //                var monthlyRecord = new EVMMonthlyValuesViewModel
                //                {
                //                    Month = monthName,
                //                    MonthValue = monthIntValue,
                //                    BAC = monthlyValue.BAC,
                //                    BCWS = monthlyValue.BCWS,
                //                    BCWP = monthlyValue.BCWP,
                //                    ACWP = monthlyValue.ACWP,
                //                    PCP = monthlyValue.PCP,
                //                    PCA = monthlyValue.PCA,
                //                    CV = monthlyValue.CV,
                //                    CVPercentage = monthlyValue.CVPercentage,
                //                    CPI = monthlyValue.CPI,
                //                    SV = monthlyValue.SV,
                //                    SVPercentage = monthlyValue.SVPercentage,
                //                    SPI = monthlyValue.SPI,
                //                    CR = monthlyValue.CR,
                //                    TCPI = monthlyValue.TCPI,
                //                    TSPI = monthlyValue.TSPI,
                //                    EAC = monthlyValue.EAC,
                //                    VAC = monthlyValue.VAC,
                //                    ETC = monthlyValue.ETC,
                //                };
                //                list.Add(monthlyRecord);
                //            }
                //        }
                //    }
                //}
            }
            //var orderedList = list.OrderBy(v => v.MonthValue).ToList();
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public YearlyGraphicalReportHederVM GetProjecAndYeartHeader(string projectId, string yearName)
        {
            int id = Convert.ToInt32(projectId);
            var item = db.ProjectInformation.Find(id);
            YearlyGraphicalReportHederVM headerDs = new YearlyGraphicalReportHederVM();
            var organization = db.Companies.FirstOrDefault();
            if (item != null)
            {
                headerDs.ProjectName = item.Name;
                headerDs.Year = yearName;

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

    /// <summary>
    /// data source of all the years under a given earned value anaysis per project
    /// </summary>
    [DataObject]
    public class YearsPerProjectAnalysis : List<ReportYearViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReportYearViewModel> GetAvailableYearsPerProjectAnalysis(string projectId)
        {
            var list = new List<ReportYearViewModel>();
            int id = Convert.ToInt32(projectId);

            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();
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
    }
}
