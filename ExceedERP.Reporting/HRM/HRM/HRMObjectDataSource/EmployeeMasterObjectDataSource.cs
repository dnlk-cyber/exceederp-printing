using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeMasterObjectDataSource : List<EmployeeMasterViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo(string branchId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo)
        {

            EmployeeMasterViewModel EmployeeMasterViewModel = new EmployeeMasterViewModel();
            List<EmployeeMasterViewModel> EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();
            var placmenets = db.Placements/*.Where(x => x.isCurrent)*/.ToList();
            var educations = db.Education;
            var lookups = db.Lookups;
            var identifications = db.Identifications;
            List<Person_Employee> employees;
            List<string> allBranches = new List<string>();

            employees = PersonEmployeeBranch(branchId, orgStructureId, "");
            if(employeeType != null)
            {
                int.TryParse(employeeType, out int eId);
                employees = employees.Where(e => e.EmployeeTypeId == eId).ToList();
            }

            //DateTime dateFrom = new DateTime(hireFrom.Year, hireFrom.Month, hireFrom.Day);
            //DateTime dateTo = new DateTime(hireTo.Year, hireTo.Month, hireTo.Day);

            if (hireFrom != null && hireTo == null)
            {
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == hireFrom);
                if (glYear != null)
                {
                    employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }

            }
            if (hireTo != null && hireFrom == null)
            {
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == hireTo);
                if (glYear != null)
                {
                    employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }
            }

            if (hireFrom != null && hireTo != null)
            {
                var glYearF = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == hireFrom);
                var glYearT = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == hireTo);
                if (glYearF != null && glYearT != null)
                {
                    employees = employees.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).ToList();
                }
            }

            var expiriances = db.Experiences.ToList();
            DateTime today = DateTime.Today;
            var currentPeriod = db.GLFiscalYears
                                        .Where(x => x.DateFrom <= today).FirstOrDefault(x => x.DateTo >= today);

            if (status != null)
            {
                if (status.ToLower() == "active")
                    employees = employees.Where(x => x.IsActive).ToList();

                if (status.ToLower() == "inactive")
                    employees = employees.Where(x => !x.IsActive).ToList();
            }

            if (employee != null)
            {
                employees = employees.Where(x => x.EmployeeId.ToString() == employee).ToList();
            }

            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            if (from != null || to != null)
            {
                foreach (var emp in employees)
                {
                    var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                    foreach (var ex in exps)
                    {
                        checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                    }
                    checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);

                    int check = (int)checkTotalExp / 365;

                    int fr = -1, tt = -1;
                    //if (int.Parse(fromE.ToString()).GetType().Equals(typeof(int)))
                    //{
                    //     fr = Convert.ToInt32(fromE);
                    //     tt = Convert.ToInt32(toE);
                    //}
                    if (from != null)
                    {
                        foreach (char c in from)
                        {
                            if (char.IsDigit(c))
                                fr = Convert.ToInt32(from);
                        }
                    }
                    if (to != null)
                    {
                        foreach (char c in to)
                        {
                            if (char.IsDigit(c))
                                tt = Convert.ToInt32(to);
                        }
                    }

                    if (from != null && to != null)
                    {
                        if (check >= fr && check <= tt)
                        {

                            empColList.Add(emp.EmployeeId);
                        }
                    }
                    if (from != null && to == null)
                    {
                        if (check >= fr)
                        {

                            empColList.Add(emp.EmployeeId);
                        }
                    }
                    if (from == null && to != null)
                    {
                        if (check <= tt)
                        {
                            empColList.Add(emp.EmployeeId);
                        }
                    }



                    checkTotalExp = 0;
                }

                employees = employees.Where(x => empColList.Contains(x.EmployeeId)).ToList();
            }

            //placmenets = placmenets.DistinctByFiled(x => x.EmployeeId).ToList();
            var kk = 0;
            foreach (var e in employees)
            {

                //var place = placmenets.FirstOrDefault(x=>x.EmployeeId == em.EmployeeId);
                //if (place != null)
                //{

                // em = EmployeeMasterViewModelList.FirstOrDefault(x=>x.EmployeeId == place.EmployeeId);
                //if(em != null)
                //{
                //    continue;
                //}
                //if (employees.Any())
                //{
                //    var e = employees.FirstOrDefault(x => x.EmployeeId == place.EmployeeId);
                //foreach (var e in employees.Where(x => x.IsActive && x))
                //{
                if (e != null)
                {
                    var retireGC = e.DateOfBirth.AddYears(60);
                    var retire = XAPI.EthiopicDateTime.GetEthiopicDate(retireGC.Day, retireGC.Month, retireGC.Year);
                    EmployeeMasterViewModel = new EmployeeMasterViewModel();
                    EmployeeMasterViewModel.DateOfBirth = e.DateOfBirthAm;
                    EmployeeMasterViewModel.RetirementDate = retire;// e.DateOfBirth.AddYears(60).ToShortDateString();
                    EmployeeMasterViewModel.EmployeeCode = e.EmployeeCode;
                    EmployeeMasterViewModel.EmployeeId = e.EmployeeId;
                    EmployeeMasterViewModel.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId);
                    EmployeeMasterViewModel.Gender = HrHelper.GetGender(e.Gender);
                    EmployeeMasterViewModel.Religion = HrHelper.GetReligion(e.Religion);
                    EmployeeMasterViewModel.Age = DateTime.Now.Year - e.DateOfBirth.Year;
                    EmployeeMasterViewModel.Education = new List<EmployeeEductaionList>();
                    EmployeeMasterViewModel.Training = new List<Training>();
                    EmployeeMasterViewModel.TrainingExt = new List<TrainingExt>();
                    EmployeeMasterViewModel.Languages = new List<Languages>();
                    EmployeeMasterViewModel.EmployeePerformances = new List<EmployeePerformances>();
                    var nation = lookups.FirstOrDefault(x => x.LookupId.ToString() == e.Nation);
                    if (nation != null)
                    {
                        EmployeeMasterViewModel.Nation = nation.Description;
                    }
                    //var education =
                    //    educations.Where(x => x.EmployeeId == e.EmployeeId && x.IsHighLevel)
                    //        .OrderByDescending(x => x.To)
                    //        .FirstOrDefault();
                    var education =
                        educations.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    if (education.Any())
                    {
                        //var level = lookups.FirstOrDefault(x => x.LookupId == education.EduLevel);
                        //if (level != null)
                        //{
                        //    EmployeeMasterViewModel.EduLevel = level.Description;

                        //}

                        foreach (var educ in education)
                        {
                            var filed = lookups.FirstOrDefault(x => x.LookupId == educ.EduField);
                            var level = lookups.FirstOrDefault(x => x.LookupId == educ.EduLevel);
                            EmployeeEductaionList edu = new EmployeeEductaionList();

                            edu.EduField = filed != null ? filed.Description : "አልተሞላም";

                            edu.EduLevel = level != null ? level.Description : "አልተሞላም";
                            edu.From = educ.FromEAmharic;
                            edu.To = educ.ToEAmharic;
                            edu.Institute = educ.OrganizationName;

                            EmployeeMasterViewModel.Education.Add(edu);
                        }


                    }


                    var value = identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                            x.Type == IdentificationType.PENSION_NO);
                    if (value != null)
                    {

                        EmployeeMasterViewModel.PensionNo = value.IdNumber;
                    }

                    var empExtExps = expiriances.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    double expSum = 0;
                    foreach (var ex in empExtExps)
                    {
                        expSum = expSum + (ex.To - ex.From).Days;
                    }
                    EmployeeMasterViewModel.Expiriance =
                        EmployeeHelper.GetDaystoWordsAmharic(expSum);
                    EmployeeMasterViewModel.MartialStatus = HrHelper.GetMartialStatus(e);
                    double totalDay = DateTime.Now.Subtract(e.HiredDate).TotalDays;


                    EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWordsAmharic(totalDay);

                    var ExpTotalSum = totalDay + expSum;

                    EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(ExpTotalSum);

                    var languageProficiency = db.LanguageOfProficiencies.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var lang in languageProficiency)
                    {
                        var language = db.Languages.FirstOrDefault(x => x.LanguageId == lang.LanguageId);
                        if (language != null)
                        {
                            Languages lan = new Languages();

                            lan.Name = language.Description;
                            EmployeeMasterViewModel.Languages.Add(lan);
                        }
                    }

                    var courseTrankings = db.Trainee.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var course in courseTrankings)
                    {
                        Training train = new Training();
                        var tracking = db.CourseTrackings.FirstOrDefault(x => x.CourseTrackingId == course.CourseTrackingId);
                        if (tracking != null)
                        {
                            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == tracking.LookupId);
                            if (lookup != null)
                            {
                                train.Name = lookup.Description;

                                EmployeeMasterViewModel.Training.Add(train);

                            }

                        }

                    }

                    var trainings = db.EmpTrainingEducations.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var train in trainings)
                    {
                        TrainingExt trainExt = new TrainingExt();
                        if (train != null)
                        {
                            try
                            {
                                trainExt.Name = train.TrainingName;

                                EmployeeMasterViewModel.TrainingExt.Add(trainExt);
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }

                    var empPerformances = db.MeasurementDefinitions.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var empPerformance in empPerformances)
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();
                        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == empPerformance.LookupId);
                        if (lookup != null)
                        {
                            empPerform.Type = lookup.Description;
                        }
                        empPerform.EvaluationCriteria = empPerformance.EvaluationCriteria;
                        empPerform.Result = empPerformance.Result.ToString();

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    //EmployeeMasterViewModel.InternalExpriance = DateTime.Now.Date.Subtract(new DateTime(e.HiredDate.Year, e.HiredDate.Month,
                    //    e.HiredDate.Day)).TotalDays / 365.25;
                    var placement = placmenets.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                    var empHiredate = db.Person_Employee.FirstOrDefault();
                    if (placement != null)
                    {
                        EmployeeMasterViewModel.HireDate = e.HiredDateAmharic;//placement.AssignmentDateAmharic;
                        EmployeeMasterViewModel.Salary = placement.Salary;
                        EmployeeMasterViewModel.ResponsibilityLevel =
                            HrHelper.GetResponsibilityLevel(placement.ResponsibilityLevel);
                        EmployeeMasterViewModel.EmployementType =
                          HrHelper.GetAssignmentType(placement.AssignmentType);

                        var empType = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                        if (empType != null)
                        {
                            if (empType.Status == EmployeeStatus.CONTRACT)
                            {
                                EmployeeMasterViewModel.Employement = "ጊዜያዊ";
                            }
                            else if (empType.Status == EmployeeStatus.PERMANENT)
                            {
                                EmployeeMasterViewModel.Employement = "ቋሚ";
                            }

                        }

                        EmployeeMasterViewModel.BranchName = placement.Branch.AmharicName;
                        EmployeeMasterViewModel.StructureName = placement.MainCompanyStructure.Name;

                        var jobTitle = lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (jobTitle != null)
                        {
                            EmployeeMasterViewModel.JobTitle = jobTitle.Description;
                        }
                        EmployeeMasterViewModel.Salary = placement.Salary;

                        var step = db.Steps.FirstOrDefault(x => x.StepId == placement.StepId);
                        if (step != null)
                        {
                            EmployeeMasterViewModel.Step = step.StepName;
                        }
                        var rank = db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        if (rank != null)
                        {
                            EmployeeMasterViewModel.Rank = rank.RankName;
                        }

                    }
                    if (currentPeriod != null)
                    {
                        //var entitle =
                        //    db.LeavesEntitle.FirstOrDefault(
                        //        x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == e.EmployeeId);
                        var entitle =
                            db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired);
                        if (entitle.Any())
                        {
                            //EmployeeMasterViewModel.TotalLeave = (entitle.Amount + GetTransfreedAmmount(currentPeriod.GlFiscalYearId,
                            //                                          e.EmployeeId)) -
                            //                                     GetTakenAmount(currentPeriod.GlFiscalYearId,
                            //                                         e.EmployeeId);
                            EmployeeMasterViewModel.TotalLeave = entitle.Sum(x => x.Balance);
                        }
                    }
                    EmployeeMasterViewModelList.Add(EmployeeMasterViewModel);
                }
                //}
                //    }
                //}
            }

            return EmployeeMasterViewModelList;
        }
        private decimal GetTransfreedAmmount(int periodId, int empId)
        {
            var transfrred = db.LeaveTransfer.FirstOrDefault(x => x.To == periodId && x.EmployeeId == empId && x.Check);
            if (transfrred != null)
            {
                if (transfrred.Check)
                {
                    return transfrred.Amount;
                }
            }
            return 0;
        }
        private decimal GetTakenAmount(int periodId, int empId)
        {
            var taken = db.Leaves.Where(x => x.GlFiscalYearId == periodId && x.EmployeeId == empId).ToList();
            if (taken.Any())
            {
                return taken.Sum(x => x.Amount);
            }
            return 0;
        }

        /////////////////////////////////////////////////////////////////////////
        public List<Person_Employee> PersonEmployeeBranch(string branchId, string orgStructureId, string employeeType)
        {


            List<Person_Employee> emplist;

            if (branchId != null && orgStructureId != null)
            {
                emplist =
                    EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId))

                        .ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                emplist = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                emplist = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {
                emplist = db.Person_Employee.ToList();
            }
            return emplist;
        }

        ///////////////////////////////////////////

        public List<Person_Employee> GetEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
        {

            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId).ToList();
            if (placements.Any())
            {
                var filterd = from f in placements
                              join e in allEmployee
                              on f.EmployeeId equals e.EmployeeId
                              where f.EmployeeId == e.EmployeeId
                              select (new { e });

                foreach (var p in filterd)
                {

                    list.Add(p.e);

                }
            }
            return list;

        }

        //////////////////////////////////////////////////////

        public List<Person_Employee> GetEmployeesByBranch(int branchId)
        {

            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;

        }

        /////////////////////////////////////////////////////////////////

        public List<Person_Employee> GetEmployeesByWorkUnit(int orgStructureId)
        {

            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee;
            var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;
        }

        public List<int> GetNumbersList()
        {
            List<int> numbers = new List<int>();
            for (var i = 0; i <= 100; i++)
            {
                numbers.Add(i);
            }
            return numbers;
        }
    }





}