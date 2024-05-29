using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTrainingObjectDataSource : List<EmployeeTraining>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTraining> GetEmployeeTraining(string type, string status, DateTime from, DateTime to, string branchId, string orgStructureId)
        {
            List<EmployeeTraining> EmployeeInfoList = new List<EmployeeTraining>();
            EmployeeTraining EmployeeInfo = new EmployeeTraining();
            int number = 0;

            var branches = db.Branch.ToList();

            var jobs = db.Lookups.ToList();

            var providers = db.Companies.Where(x => x.Category == "1").ToList();

            DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
            DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);

            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }

            bool idCompleted = false;

            if (status == "COMPLETED")
            {
                idCompleted = true;
            }

            else if (status == "NOT_COMPLETED")
            {
                idCompleted = false;
            }

            var courses = db.CourseTrackings.Where(x => x.TrainingType.ToString().ToUpper() == type.ToUpper()).ToList();

            if (type != null)
            {
                courses = db.CourseTrackings.Where(x => x.TrainingType.ToString().ToUpper() == type.ToUpper()).ToList();
            }

            if (status == null)
            {
                foreach (var l in courses)
                {
                    db = new ExceedDbContext();

                    var trainees = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId);

                    foreach (var t in trainees)
                    {
                        db = new ExceedDbContext();

                        var emplist = db.Person_Employee.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

                        foreach (var e in emplist)
                        {
                            DateTime courceTrackForm = new DateTime(l.From.Year, l.From.Month,
                                l.From.Day);
                            DateTime courceTrackTo = new DateTime(l.To.Year, l.To.Month,
                                l.To.Day);
                            if (courceTrackForm >= dateOne && courceTrackTo <= dateTow)
                            {

                                EmployeeInfo = new EmployeeTraining();
                                EmployeeInfo.EmployeeId = e.EmployeeCode;

                                if (e.Gender == 0)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                var dateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackForm.Day, courceTrackForm.Month, courceTrackForm.Year);
                                var dateTo = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackTo.Day, courceTrackTo.Month, courceTrackTo.Year);
                                EmployeeInfo.From = dateFrom;// courceTrackForm.ToShortDateString();
                                EmployeeInfo.To = dateTo;// courceTrackTo.ToShortDateString();

                                var trainee = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId).ToList();
                                number = trainee.Count();
                                EmployeeInfo.Cost = l.Cost / number;

                                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == l.GlFiscalYearId);
                                if (year != null)
                                {
                                    EmployeeInfo.Period = year.Name;
                                }

                                var place = placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();

                                if (place != null)
                                {

                                    var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                                           
                                    if (deptt != null)
                                    {
                                        EmployeeInfo.Departement = deptt.Name;
                                    }
                                }
                               
                                EmployeeInfo.Course = jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;
                                EmployeeInfo.Provider =
                                    providers.Where(x => x.MainCompanyId == l.MainCompanyId)
                                        .FirstOrDefault()
                                        .TradeName;
                                //EmployeeInfo.TrainingType = l.TrainingType.ToString();
                                if (l.TrainingType == TrainingType.TRAINING)
                                {
                                    EmployeeInfo.TrainingType = "ስልጠና";
                                }
                                else
                                {
                                    EmployeeInfo.TrainingType = "ትምህርት";
                                }

                                EmployeeInfoList.Add(EmployeeInfo);
                            }
                        }
                    }
                }

            }
            else if (status != null)
            {
                foreach (var l in courses)
                {
                    db = new ExceedDbContext();

                    var trainees = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId && x.IsCompleteTraining == idCompleted).ToList();

                    foreach (var t in trainees)
                    {
                        db = new ExceedDbContext();

                        var emplist = db.Person_Employee.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                DateTime courceTrackForm = new DateTime(l.From.Year, l.From.Month,
                                    l.From.Day);
                                DateTime courceTrackTo = new DateTime(l.To.Year, l.To.Month,
                                    l.To.Day);
                                if (courceTrackForm >= dateOne && courceTrackTo <= dateTow)
                                {
                                    EmployeeInfo = new EmployeeTraining();
                                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                                    if (e.Gender == 0)
                                    {
                                        EmployeeInfo.Sex = "ወንድ";
                                    }
                                    else
                                    {
                                        EmployeeInfo.Sex = "ሴት";
                                    }
                                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                    var dateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackForm.Day, courceTrackForm.Month, courceTrackForm.Year);
                                    var dateTo = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackTo.Day, courceTrackTo.Month, courceTrackTo.Year);

                                    EmployeeInfo.From = dateFrom;// courceTrackForm.ToShortDateString();
                                    EmployeeInfo.To = dateTo;// courceTrackTo.ToShortDateString();
                                    var trainee = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId).ToList();
                                    number = trainee.Count();
                                    EmployeeInfo.Cost = l.Cost / number;
                                    var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == l.GlFiscalYearId);
                                    if (year != null)
                                    {
                                        EmployeeInfo.Period = year.Name;
                                    }

                                    var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                                    if (place != null)
                                    {
                                        var deptt =
                                            db.MainCompanyStructures.Where(
                                                    x => x.OrgStructureId == place.OrgStructureId)
                                                .FirstOrDefault();
                                        if (deptt != null)
                                        {
                                            EmployeeInfo.Departement = deptt.Name;
                                        }
                                    }

                                    EmployeeInfo.Course =
                                        jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;
                                    EmployeeInfo.Provider =
                                        providers.Where(x => x.MainCompanyId == l.MainCompanyId)
                                            .FirstOrDefault()
                                            .TradeName;
                                    // EmployeeInfo.TrainingType = l.TrainingType.ToString();
                                    if (l.TrainingType == TrainingType.TRAINING)
                                    {
                                        EmployeeInfo.TrainingType = "ስልጠና";
                                    }
                                    else
                                    {
                                        EmployeeInfo.TrainingType = "ትምህርት";
                                    }

                                    EmployeeInfoList.Add(EmployeeInfo);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var l in courses)
                {

                    var trainees = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId);

                    if (status == "COMPLETED")
                    {
                        trainees = trainees.Where(x => x.IsCompleteTraining == true);
                    }
                    else if (status == "NOT_COMPLETED")
                    {
                        trainees = trainees.Where(x => x.IsCompleteTraining == false);
                    }

                    foreach (var t in trainees)
                    {
                        db = new ExceedDbContext();
                        var emplist = db.Person_Employee.Where(x => x.EmployeeId == t.EmployeeId && x.IsActive).ToList();

                        if (emplist.Any())
                        {
                            foreach (var e in emplist)
                            {
                                DateTime courceTrackForm = new DateTime(l.From.Year, l.From.Month,
                                    l.From.Day);
                                DateTime courceTrackTo = new DateTime(l.To.Year, l.To.Month,
                                    l.To.Day);

                                EmployeeInfo = new EmployeeTraining();
                                EmployeeInfo.EmployeeId = e.EmployeeCode;
                                if (e.Gender == 0)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;

                                var dateFrom = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackForm.Day, courceTrackForm.Month, courceTrackForm.Year);
                                var dateTo = XAPI.EthiopicDateTime.GetEthiopicDate(courceTrackTo.Day, courceTrackTo.Month, courceTrackTo.Year);

                                EmployeeInfo.From = dateFrom;// courceTrackForm.ToShortDateString();
                                EmployeeInfo.To = dateTo;// courceTrackTo.ToShortDateString();
                                var trainee = db.Trainee.Where(x => x.CourseTrackingId == l.CourseTrackingId).ToList();
                                number = trainee.Count();
                                EmployeeInfo.Cost = l.Cost / number;
                                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == l.GlFiscalYearId);
                                if (year != null)
                                {
                                    EmployeeInfo.Period = year.Name;
                                }

                                var place = placements.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                                if (place != null)
                                {

                                    var deptt =
                                        db.MainCompanyStructures.Where(
                                                x => x.OrgStructureId == place.OrgStructureId)
                                            .FirstOrDefault();
                                    if (deptt != null)
                                    {
                                        EmployeeInfo.Departement = deptt.Name;
                                    }
                                }

                                EmployeeInfo.Course =
                                    jobs.Where(x => x.LookupId == l.LookupId).FirstOrDefault().Description;
                                EmployeeInfo.Provider =
                                    providers.Where(x => x.MainCompanyId == l.MainCompanyId)
                                        .FirstOrDefault()
                                        .TradeName;
                                // EmployeeInfo.TrainingType = l.TrainingType.ToString();
                                if (l.TrainingType == TrainingType.TRAINING)
                                {
                                    EmployeeInfo.TrainingType = "ስልጠና";
                                }
                                else
                                {
                                    EmployeeInfo.TrainingType = "ትምህርት";
                                }

                                EmployeeInfoList.Add(EmployeeInfo);
                            }

                        }

                    }

                }
            }
            return EmployeeInfoList;
        }
    }
}