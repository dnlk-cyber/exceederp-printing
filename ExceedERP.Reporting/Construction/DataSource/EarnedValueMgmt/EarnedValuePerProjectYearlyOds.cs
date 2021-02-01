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
    public class EarnedValuePerProjectYearlyOds : List<EVMPerProjectYearlyAnalysisViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        public string ConsructionInputsHelper { get; private set; }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EVMPerProjectYearlyAnalysisViewModel> GetYearlyEarnedValueAnalysis(string projectId, string yearOfEvaluation)
        {
            var list = new List<EVMPerProjectYearlyAnalysisViewModel>();
            int id = Convert.ToInt32(projectId);
            var project = db.ProjectInformation.Where(p => p.ProjectInformationId == id).FirstOrDefault();

            if (project != null)
            {
                if (project.ProjectType != AvailableProjectTypes.Road)
                {
                    var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();
                    if (yearOfEvaluation != null)
                    {
                        yearlyEarnedValueAnalysisList = yearlyEarnedValueAnalysisList.Where(yev => yev.GlFiscalYear == yearOfEvaluation).ToList();
                    }
                    if (yearlyEarnedValueAnalysisList.Any())
                    {
                        var yearsRawList = new HashSet<string>(yearlyEarnedValueAnalysisList.Select(yv => yv.GlFiscalYear));
                        var yearsList = new List<string>();
                        #region MyRegion
                        //foreach (var temp in yearsRawList)
                        //{
                        //    //var match = yearsList.FirstOrDefault(stringToCheck => stringToCheck.Contains(temp));
                        //    string match = yearsList.FirstOrDefault(x => x == temp);
                        //    if (match == null)
                        //    {
                        //        yearsList.Add(temp);

                        //    }

                        //}
                        //yearsList.AddRange(yearsRawList.Distinct()); 
                        #endregion
                        foreach (var year in yearsRawList)
                        {
                            var yearlyValues = yearlyEarnedValueAnalysisList.Where(yrv => yrv.GlFiscalYear == year).OrderBy(v => v.EarnedValuePerProjectId);
                            if (yearlyValues.Any())
                            {
                                //foreach(EarnedValuePerProject earnedValuePerProject in yearlyValues)
                                //{
                                var yearlyRecord = new EVMPerProjectYearlyAnalysisViewModel
                                {
                                    ProjectName = project.Name,
                                    Year = year
                                };

                                //var monthsList = new HashSet<Months>(yearlyValues.Select(yv => yv.Month).Distinct());
                                //if (monthsList.Any())
                                //{
                                var sortedRecords = EarnedValueManagementHelper.GetSortedValue(yearlyValues.ToList());
                                var arrayOfRecords = sortedRecords.ToArray();
                                for (int i = 0; i < sortedRecords.Count; i++)
                                {
                                    var ev = arrayOfRecords[i];

                                    var monthlyRecord = new EVMMonthlyValuesViewModel
                                    {
                                        Month = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                        RecordOrder = i,
                                        BAC = ev.BAC,
                                        BCWS = ev.BCWS,
                                        BCWSCummulative = ev.BCWS,
                                        BCWP = ev.BCWP,
                                        BCWPCummulative = ev.BCWP,
                                        ACWP = ev.ACWP,
                                        ACWPCummulative = ev.ACWP,
                                        PCP = ev.PCP,
                                        PCA = ev.PCA,
                                        CV = ev.CV,
                                        CVPercentage = ev.CVPercentage,
                                        CPI = ev.CPI,
                                        SV = ev.SV,
                                        SVPercentage = ev.SVPercentage,
                                        SPI = ev.SPI,
                                        CR = ev.CR,
                                        TCPI = ev.TCPI,
                                        TSPI = ev.TSPI,
                                        EAC = ev.EAC,
                                        VAC = ev.VAC,
                                        ETC = ev.ETC,
                                    };
                                    yearlyRecord.MonthlyValueList.Add(monthlyRecord);
                                }
                                list.Add(yearlyRecord);
                                //}
                            }

                        }
                    }
                }
                else
                {
                    var yearlyEarnedValueAnalysisList = db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == project.ProjectInformationId).OrderBy(ev => ev.GlFiscalYear).ToList();
                    if (yearOfEvaluation != null)
                    {
                        yearlyEarnedValueAnalysisList = yearlyEarnedValueAnalysisList.Where(yev => yev.GlFiscalYear == yearOfEvaluation).ToList();
                    }
                    if (yearlyEarnedValueAnalysisList.Any())
                    {
                        var yearsRawList = new HashSet<string>(yearlyEarnedValueAnalysisList.Select(yv => yv.GlFiscalYear));
                        var yearsList = new List<string>();

                        foreach (var year in yearsRawList)
                        {
                            var yearlyValues = yearlyEarnedValueAnalysisList.Where(yrv => yrv.GlFiscalYear == year).OrderBy(v => v.EarnedValuePerProjectId);
                            if (yearlyValues.Any())
                            {
                                var yearlyRecord = new EVMPerProjectYearlyAnalysisViewModel
                                {
                                    ProjectName = project.Name,
                                    Year = year
                                };

                                var sortedRecords = EarnedValueManagementHelper.GetSortedValue(yearlyValues.ToList());
                                var arrayOfRecords = sortedRecords.ToArray();
                                for (int i = 0; i < sortedRecords.Count; i++)
                                {
                                    var ev = arrayOfRecords[i];
                                    var prevRecArray = arrayOfRecords.Slice(0, i);
                                    if (prevRecArray.Length > 0)
                                    {
                                        var prevRecList = prevRecArray.ToList();

                                        var monthlyRecord = new EVMMonthlyValuesViewModel
                                        {
                                            Month = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                            RecordOrder = i,
                                            BAC = ev.BAC,
                                            BCWS = ev.BCWS,
                                            BCWSCummulative = prevRecList.Sum(v => v.BCWS) + ev.BCWS,
                                            BCWP = ev.BCWP,
                                            BCWPCummulative = prevRecList.Sum(v => v.BCWP) + ev.BCWP,
                                            ACWP = ev.ACWP,
                                            ACWPCummulative = prevRecList.Sum(v => v.ACWP) + ev.ACWP,
                                            PCP = ev.PCP,
                                            PCA = ev.PCA,
                                            CV = ev.CV,
                                            CVPercentage = ev.CVPercentage,
                                            CPI = ev.CPI,
                                            SV = ev.SV,
                                            SVPercentage = ev.SVPercentage,
                                            SPI = ev.SPI,
                                            CR = ev.CR,
                                            TCPI = ev.TCPI,
                                            TSPI = ev.TSPI,
                                            EAC = ev.EAC,
                                            VAC = ev.VAC,
                                            ETC = ev.ETC,
                                        };
                                        yearlyRecord.MonthlyValueList.Add(monthlyRecord);
                                    }
                                    else
                                    {
                                        var monthlyRecord = new EVMMonthlyValuesViewModel
                                        {
                                            Month = ConstructionInputsHelper.GetConstructionPeriod(ev.ConstructionPeriodId),
                                            RecordOrder = i,
                                            BAC = ev.BAC,
                                            BCWS = ev.BCWS,
                                            BCWSCummulative = ev.BCWS,
                                            BCWP = ev.BCWP,
                                            BCWPCummulative = ev.BCWP,
                                            ACWP = ev.ACWP,
                                            ACWPCummulative = ev.ACWP,
                                            PCP = ev.PCP,
                                            PCA = ev.PCA,
                                            CV = ev.CV,
                                            CVPercentage = ev.CVPercentage,
                                            CPI = ev.CPI,
                                            SV = ev.SV,
                                            SVPercentage = ev.SVPercentage,
                                            SPI = ev.SPI,
                                            CR = ev.CR,
                                            TCPI = ev.TCPI,
                                            TSPI = ev.TSPI,
                                            EAC = ev.EAC,
                                            VAC = ev.VAC,
                                            ETC = ev.ETC,
                                        };
                                        yearlyRecord.MonthlyValueList.Add(monthlyRecord);
                                    }
                                }
                                list.Add(yearlyRecord);
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
