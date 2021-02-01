using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using XAPI;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBSCPerforObjectDataSource : List<EmployeeBscPerfromance>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeBscPerfromance> GetEmployeeBSCPerformance(string branchId, string orgStructureId, string employeeType, string year, string status)
        {
            List<EmployeeBscPerfromance> EmployeeInfoList = new List<EmployeeBscPerfromance>();
            EmployeeBscPerfromance EmployeeInfo = new EmployeeBscPerfromance();

            List<Behaviours> behaviourList = new List<Behaviours>();
            Behaviours behaviours = new Behaviours();
            var lookups = db.Lookups.ToList();
            //var definations = db.MeasurementDefinitions.ToList();
            //var detail = db.Measurements.ToList();
            List<Person_Employee> emplist;

            emplist = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, employeeType);

            List<Quarter> quarters = new List<Quarter>();
            if (year != null)
            {
                var quarterLength = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year);
                if (quarterLength != null)
                {
                    quarters = db.Quarters.Where(x => x.QuarterLengthId == quarterLength.QuarterLengthId).ToList();
                }
            }

            //var periods = from p in db.QuarterLengths
            //              join f in db.GLFiscalYears.Where(x => x.GlFiscalYearId.ToString() == year)
            //                  on p.GlFiscalYearId equals f.GlFiscalYearId
            //              join q in db.Quarters
            //              on p.QuarterLengthId equals q.QuarterLengthId
            //              where f.Status == FiscalYearStatus.Open
            //              select (new { q });

            //var period = periods.Select(x => x.q).ToList();

            // var placements = db.Placements.ToList();


            var performances = db.EmployeePerformanceScores.ToList();
            if (status == "የተሞላላቸው")
            {
                performances = performances.Where(x => x.IsApproved).ToList();
            }
            if (status == "ያልተሞላላቸው")
            {
                performances = performances.Where(x => !x.IsApproved).ToList();
            }

            if (emplist.Any())
            {
                foreach (var e in emplist)
                {
                    decimal result = 0;
                    decimal checkNullTotalResult = 0;
                    behaviourList = new List<Behaviours>();
                    EmployeeInfo = new EmployeeBscPerfromance();
                    EmployeeInfo.EvaluationPeiordList = new List<EvaluationPeiord>();
                    EmployeeInfo.EmployeeId = e.EmployeeCode;

                    EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);
                    EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId);
                    EmployeeInfo.Total = result;
                    var palcmentOne = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    if (palcmentOne != null)
                    {
                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == palcmentOne.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                        var title = lookups.FirstOrDefault(x => x.LookupId == palcmentOne.LookupId);
                        if (title != null)
                        {
                            EmployeeInfo.JobTitle = title.Description;
                        }

                    }
                    foreach (var r in quarters)
                    {
                        EvaluationPeiord evaluationPeiordList = new EvaluationPeiord();

                        decimal checkQuarter = 0;

                        evaluationPeiordList.Period = r.AmharicName;
                        evaluationPeiordList.Behaviours = new List<Behaviours>();

                        var measurmentDefinitions = db.MeasurementDefinitions.ToList();
                        var place = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == e.EmployeeId);
                        if (place != null)
                        {
                            measurmentDefinitions = measurmentDefinitions.Where(x => x.Role == place.ResponsibilityLevel).ToList();
                        }

                        foreach (var m in measurmentDefinitions)
                        {
                            behaviours = new Behaviours();
                            behaviours.Detailses = new List<Details>();
                            Details details;

                            behaviours.Behaviour = m.EvaluationCriteria;

                            var measurments = db.Measurements.Where(x => x.MeasurementDefinitionId == m.MeasurementDefinitionId);

                            foreach (var n in measurments)
                            {
                                details = new Details();

                                var performanceScore = performances.Where(x => x.MeasurementId == n.MeasurementId && x.EmployeeId == e.EmployeeId && x.QuarterId == r.QuarterId).ToList();
                                details.Performance = performanceScore.Sum(x => x.Result);
                                result = result + details.Performance;
                                checkQuarter = checkQuarter + details.Performance;
                                details.Measurment = n.MeasurementName;

                                behaviours.Detailses.Add(details);

                            }

                            // behaviourList.Add(behaviours);

                            evaluationPeiordList.Behaviours.Add(behaviours);
                        }


                        if (status == "የተሞላላቸው" && checkQuarter > 0)
                        {
                            EmployeeInfo.EvaluationPeiordList.Add(evaluationPeiordList);
                        }
                        else if (status == "ያልተሞላላቸው" && checkQuarter == 0)
                        {
                            var singleScore = db.EmployeePerformanceScores.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.QuarterId == r.QuarterId);
                            if (singleScore != null && singleScore.Result == 0)
                            {
                                EmployeeInfo.EvaluationPeiordList.Add(evaluationPeiordList);
                                checkNullTotalResult = checkNullTotalResult + 1;
                            }
                            else if (singleScore == null)
                            {
                                EmployeeInfo.EvaluationPeiordList.Add(evaluationPeiordList);
                                checkNullTotalResult = checkNullTotalResult + 1;
                            }

                        }

                        //result = behaviours.Detailses.Sum(x => x.Performance);                       
                    }

                    EmployeeInfo.Total = result;

                    var rank = db.PerformanceRank.FirstOrDefault(x => x.Minimum <= result && x.Maximum >= result);
                    if (rank != null)
                    {
                        EmployeeInfo.Rank = rank.Description;
                    }

                    if (status == "የተሞላላቸው" && result > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                    else if (status == "ያልተሞላላቸው" && result == 0 && checkNullTotalResult > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }
            }

            return EmployeeInfoList;
        }


    }
}