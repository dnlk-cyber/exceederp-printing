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
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeMasterObjectDataSource : List<EmployeeMasterViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo(/*string branchId,*/ object[] branchId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo)
        {

            //   EmployeeMasterViewModel EmployeeMasterViewModel = new EmployeeMasterViewModel();
            List<EmployeeMasterViewModel> EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();

            List<string> allBranches = new List<string>();

            var placements = db.Placements.ToList();

            if (status != null)
            {
                if (status.ToLower() == "active")
                {
                    placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).Where(x => x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
                    // employees = employees.Where(x => x.IsActive).ToList();
                }

                if (status.ToLower() == "inactive")
                {
                    placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements).OrderByDescending(x => x.AssignmentDate).Where(x => !x.isCurrent).DistinctByField(x => x.EmployeeId).ToList();
                    //employees = employees.Where(x => !x.IsActive).ToList();
                }
            }


            var branches = branchId.ToList();
            if (branches.Any())
            {
                List<Placement> filteredPlacements = new List<Placement>();
                foreach (var b in branches)
                {
                    var filt = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, Convert.ToInt32(b)).ToList();

                    filteredPlacements.AddRange(filt);
                }

                placements = filteredPlacements;
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = placements.Where(x => x.OrgStructureId == orgId).ToList();
            }
            if (employee != null)
            {
                int.TryParse(employee, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId).ToList();
            }

            if (hireFrom != null && hireTo == null)
            {
                int.TryParse(hireFrom, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }

            }
            else if (hireTo != null && hireFrom == null)
            {
                int.TryParse(hireTo, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }
            }
            else if (hireFrom != null && hireTo != null)
            {
                int.TryParse(hireFrom, out int glFiscalId1);
                int.TryParse(hireTo, out int glFiscalId2);

                var glYearF = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId1);
                var glYearT = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId2);
                if (glYearF != null && glYearT != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    // employees = employees.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).ToList();
                }
            }

            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            if (from != null || to != null)
            {
                foreach (var pl in placements)
                {
                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                    if (emp != null)
                    {
                        var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                        foreach (var ex in exps)
                        {
                            checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                        }
                        if (emp.IsActive)
                        {
                            checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);
                        }
                        else
                        {
                            var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (termination != null)
                            {
                                checkTotalExp = checkTotalExp + termination.TerminationDate.Subtract(emp.HiredDate).Days;
                            }
                        }

                        int check = (int)checkTotalExp / 365;

                        //// this is to check if the year entered by user is character or digit 

                        //int fr = -1;
                        //int tt = -1;

                        //if (from != null)
                        //{
                        //    foreach (char c in from)
                        //    {
                        //        if (char.IsDigit(c))
                        //        {
                        //            fr = Convert.ToInt32(from);
                        //        }                                
                        //    }
                        //}
                        //if (to != null)
                        //{
                        //    foreach (char c in to)
                        //    {
                        //        if (char.IsDigit(c))
                        //            tt = Convert.ToInt32(to);
                        //    }
                        //}

                        if (from != null && to != null)
                        {
                            int.TryParse(from, out int fr);
                            int.TryParse(to, out int tt);

                            if (check >= fr && check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from != null && to == null)
                        {
                            int.TryParse(from, out int fr);

                            if (check >= fr)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from == null && to != null)
                        {
                            int.TryParse(to, out int tt);

                            if (check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }

                        checkTotalExp = 0;
                    }

                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId)).ToList();
            }



            foreach (var pl in placements)
            {
                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    var retireGC = e.DateOfBirth.AddYears(60);
                    var retire = XAPI.EthiopicDateTime.GetEthiopicDate(retireGC.Day, retireGC.Month, retireGC.Year);

                    var EmployeeMasterViewModel = new EmployeeMasterViewModel();

                    EmployeeMasterViewModel.DateOfBirth = e.DateOfBirthAm;
                    EmployeeMasterViewModel.RetirementDate = retire;// e.DateOfBirth.AddYears(60).ToShortDateString();
                    EmployeeMasterViewModel.EmployeeCode = e.EmployeeCode;
                    EmployeeMasterViewModel.EmployeeId = e.EmployeeId;
                    EmployeeMasterViewModel.FullName = e.FullNameAmharic; //HrHelper.GetEmployeeFullName(e.EmployeeId);
                    EmployeeMasterViewModel.Gender = HrHelper.GetGender(e.Gender);
                    EmployeeMasterViewModel.Religion = HrHelper.GetReligion(e.Religion);
                    EmployeeMasterViewModel.Age = DateTime.Now.Year - e.DateOfBirth.Year;

                    EmployeeMasterViewModel.Education = new List<EmployeeEductaionList>();
                    EmployeeMasterViewModel.Training = new List<Training>();
                    EmployeeMasterViewModel.TrainingExt = new List<TrainingExt>();
                    EmployeeMasterViewModel.Languages = new List<ExceedERP.Reporting.HRM.HRM.HRMViewModel.Languages>();
                    EmployeeMasterViewModel.EmployeePerformances = new List<EmployeePerformances>();
                    EmployeeMasterViewModel.Discipline = new List<Discipline>();

                    if (!string.IsNullOrEmpty(e.Nation))
                    {
                        int.TryParse(e.Nation, out int lookId);
                        var nation = db.Lookups.FirstOrDefault(x => x.LookupId == lookId);
                        if (nation != null)
                        {
                            EmployeeMasterViewModel.Nation = nation.Description;
                        }
                    }

                    var education = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();

                    foreach (var educ in education)
                    {
                        var filed = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduField);
                        var level = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduLevel);
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = filed != null ? filed.Description : "አልተሞላም";

                        edu.EduLevel = level != null ? level.Description : "አልተሞላም";
                        edu.From = educ.FromEAmharic;
                        edu.To = educ.ToEAmharic;
                        edu.Institute = educ.OrganizationName;

                        EmployeeMasterViewModel.Education.Add(edu);
                    }
                    //// i do this to connect the broke lines when the list is empty
                    if (!EmployeeMasterViewModel.Education.Any())
                    {
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = " ";

                        edu.EduLevel = " ";
                        edu.From = " ";
                        edu.To = " ";
                        edu.Institute = " ";

                        EmployeeMasterViewModel.Education.Add(edu);
                    }

                    var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

                    if (value != null)
                    {
                        EmployeeMasterViewModel.PensionNo = value.IdNumber;
                    }

                    var empExtExps = db.Experiences.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    double expSum = 0;
                    foreach (var ex in empExtExps)
                    {
                        expSum = expSum + (ex.To - ex.From).Days;
                    }
                    EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWordsAmharic(expSum);
                    EmployeeMasterViewModel.MartialStatus = HrHelper.GetMartialStatus(e);

                    double totalInternalDays = DateTime.Now.Subtract(e.HiredDate).TotalDays;

                    EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWordsAmharic(totalInternalDays);

                    var ExpTotalSum = totalInternalDays + expSum;

                    EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(ExpTotalSum);

                    var languageProficiency = db.LanguageOfProficiencies.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var lang in languageProficiency)
                    {
                        var language = db.Languages.FirstOrDefault(x => x.LanguageId == lang.LanguageId);
                        if (language != null)
                        {
                            HRMViewModel.Languages lan = new HRMViewModel.Languages();

                            lan.Name = language.Description;
                            EmployeeMasterViewModel.Languages.Add(lan);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Languages.Any())
                    {
                        HRMViewModel.Languages lan = new HRMViewModel.Languages();

                        lan.Name = " ";

                        EmployeeMasterViewModel.Languages.Add(lan);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Training.Any())
                    {
                        Training train = new Training();

                        train.Name = " ";

                        EmployeeMasterViewModel.Training.Add(train);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.TrainingExt.Any())
                    {
                        TrainingExt trainExt = new TrainingExt();

                        trainExt.Name = " ";

                        EmployeeMasterViewModel.TrainingExt.Add(trainExt);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.EmployeePerformances.Any())
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();

                        empPerform.Type = " ";

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    var disciplineCases = db.DisciplinaryCases.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var disc in disciplineCases)
                    {
                        var discipine = new Discipline();

                        var empDisc = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                        if (empDisc != null)
                        {
                            var discMeasure = db.DisciplineMeasures.FirstOrDefault(x => x.MeasuresId == empDisc.MeasuresId);
                            if (discMeasure != null)
                            {
                                discipine.MeasureRank = discMeasure.MeasuresType;
                            }
                            var discMeasureDetail = db.DisciplineMeasuresDetail.FirstOrDefault(x => x.DetailMeasuresId == empDisc.DetailMeasuresId);
                            if (discMeasureDetail != null)
                            {
                                discipine.MeasureTaken = discMeasureDetail.Description;
                            }

                            discipine.Month = empDisc.Month;
                            var disMeasureByMoney = db.DisciplineMeasuerByMoney.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                            if (disMeasureByMoney != null)
                            {
                                discipine.Amount = disMeasureByMoney.Amount;
                            }

                            EmployeeMasterViewModel.Discipline.Add(discipine);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Discipline.Any())
                    {
                        var discipine = new Discipline();

                        discipine.MeasureRank = " ";
                        discipine.MeasureTaken = " ";
                        discipine.Month = 0;
                        discipine.Amount = 0;

                        EmployeeMasterViewModel.Discipline.Add(discipine);
                    }

                    //EmployeeMasterViewModel.InternalExpriance = DateTime.Now.Date.Subtract(new DateTime(e.HiredDate.Year, e.HiredDate.Month,
                    //    e.HiredDate.Day)).TotalDays / 365.25;

                    EmployeeMasterViewModel.HireDate = e.HiredDateAmharic;//placement.AssignmentDateAmharic;
                    EmployeeMasterViewModel.Salary = pl.Salary;
                    EmployeeMasterViewModel.ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel);
                    EmployeeMasterViewModel.EmployementType = HrHelper.GetAssignmentType(pl.AssignmentType);

                    if (e.Status == EmployeeStatus.CONTRACT)
                    {
                        EmployeeMasterViewModel.Employement = "ጊዜያዊ";
                    }
                    else if (e.Status == EmployeeStatus.PERMANENT)
                    {
                        EmployeeMasterViewModel.Employement = "ቋሚ";
                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == pl.BranchId);
                    if (branch != null)
                    {
                        EmployeeMasterViewModel.BranchName = branch.AmharicName;
                    }
                    var structure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                    if (structure != null)
                    {
                        EmployeeMasterViewModel.StructureName = structure.Name; ;
                    }


                    var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == pl.LookupId);

                    if (jobTitle != null)
                    {
                        EmployeeMasterViewModel.JobTitle = jobTitle.Description;
                    }
                    EmployeeMasterViewModel.Salary = pl.Salary;

                    var step = db.Steps.FirstOrDefault(x => x.StepId == pl.StepId);
                    if (step != null)
                    {
                        EmployeeMasterViewModel.Step = step.StepName;
                    }
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == pl.RankId);
                    if (rank != null)
                    {
                        EmployeeMasterViewModel.Rank = rank.RankName;
                    }
                    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);
                    if (currentPeriod != null)
                    {
                        var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired);

                        if (entitle.Any())
                        {
                            EmployeeMasterViewModel.TotalLeave = entitle.Sum(x => x.Balance);
                        }
                    }

                    EmployeeMasterViewModelList.Add(EmployeeMasterViewModel);

                }

            }

            return EmployeeMasterViewModelList;
        }


        public List<EmployeeMasterViewModel> GetEmployeeMasterInfo_Old(/*string branchId,*/ object[] branchId, string orgStructureId, string employeeType, string status, string employee, string from, string to, string hireFrom, string hireTo)
        {

            EmployeeMasterViewModel EmployeeMasterViewModel = new EmployeeMasterViewModel();
            List<EmployeeMasterViewModel> EmployeeMasterViewModelList = new List<EmployeeMasterViewModel>();

            List<Placement> placements;
            List<string> allBranches = new List<string>();

            placements = FilterEmployees(branchId, orgStructureId, employeeType);

            if (status != null)
            {
                if (status.ToLower() == "active")
                {
                    placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements).Where(x => x.isCurrent).ToList();
                    // employees = employees.Where(x => x.IsActive).ToList();
                }

                if (status.ToLower() == "inactive")
                {
                    placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
                    //employees = employees.Where(x => !x.IsActive).ToList();
                }
            }

            if (hireFrom != null && hireTo == null)
            {
                int.TryParse(hireFrom, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }

            }
            else if (hireTo != null && hireFrom == null)
            {
                int.TryParse(hireTo, out int glFiscalId);
                var glYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId);
                if (glYear != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    //  employees = employees.Where(x => x.HiredDate >= glYear.DateFrom && x.HiredDate <= glYear.DateTo).ToList();
                }
            }
            else if (hireFrom != null && hireTo != null)
            {
                int.TryParse(hireFrom, out int glFiscalId1);
                int.TryParse(hireTo, out int glFiscalId2);

                var glYearF = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId1);
                var glYearT = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == glFiscalId2);
                if (glYearF != null && glYearT != null)
                {
                    var empIds = db.Person_Employee.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).Select(x => x.EmployeeId).ToList();
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                    // employees = employees.Where(x => x.HiredDate >= glYearF.DateFrom && x.HiredDate <= glYearT.DateTo).ToList();
                }
            }

            var expiriances = db.Experiences.ToList();
            DateTime today = DateTime.Today;
            var currentPeriod = db.GLFiscalYears.Where(x => x.DateFrom <= today).FirstOrDefault(x => x.DateTo >= today);



            if (employee != null)
            {
                int.TryParse(employee, out int empId);
                placements = placements.Where(x => x.EmployeeId == empId).ToList();
            }

            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            if (from != null || to != null)
            {
                foreach (var pl in placements)
                {
                    var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                    if (emp != null)
                    {
                        var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                        foreach (var ex in exps)
                        {
                            checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                        }
                        if (emp.IsActive)
                        {
                            checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);
                        }
                        else
                        {
                            var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (termination != null)
                            {
                                checkTotalExp = checkTotalExp + termination.TerminationDate.Subtract(emp.HiredDate).Days;
                            }
                        }

                        int check = (int)checkTotalExp / 365;

                        //// this is to check if the year entered by user is character or digit 

                        //int fr = -1;
                        //int tt = -1;

                        //if (from != null)
                        //{
                        //    foreach (char c in from)
                        //    {
                        //        if (char.IsDigit(c))
                        //        {
                        //            fr = Convert.ToInt32(from);
                        //        }                                
                        //    }
                        //}
                        //if (to != null)
                        //{
                        //    foreach (char c in to)
                        //    {
                        //        if (char.IsDigit(c))
                        //            tt = Convert.ToInt32(to);
                        //    }
                        //}

                        if (from != null && to != null)
                        {
                            int.TryParse(from, out int fr);
                            int.TryParse(to, out int tt);

                            if (check >= fr && check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from != null && to == null)
                        {
                            int.TryParse(from, out int fr);

                            if (check >= fr)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (from == null && to != null)
                        {
                            int.TryParse(to, out int tt);

                            if (check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }

                        checkTotalExp = 0;
                    }

                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId)).ToList();
            }

            placements = placements.DistinctByField(x => x.EmployeeId).ToList();

            foreach (var pl in placements)
            {
                var e = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId);

                if (e != null)
                {
                    var retireGC = e.DateOfBirth.AddYears(60);
                    var retire = XAPI.EthiopicDateTime.GetEthiopicDate(retireGC.Day, retireGC.Month, retireGC.Year);

                    EmployeeMasterViewModel = new EmployeeMasterViewModel();

                    EmployeeMasterViewModel.DateOfBirth = e.DateOfBirthAm;
                    EmployeeMasterViewModel.RetirementDate = retire;// e.DateOfBirth.AddYears(60).ToShortDateString();
                    EmployeeMasterViewModel.EmployeeCode = e.EmployeeCode;
                    EmployeeMasterViewModel.EmployeeId = e.EmployeeId;
                    EmployeeMasterViewModel.FullName = e.FullNameAmharic; //HrHelper.GetEmployeeFullName(e.EmployeeId);
                    EmployeeMasterViewModel.Gender = HrHelper.GetGender(e.Gender);
                    EmployeeMasterViewModel.Religion = HrHelper.GetReligion(e.Religion);
                    EmployeeMasterViewModel.Age = DateTime.Now.Year - e.DateOfBirth.Year;

                    EmployeeMasterViewModel.Education = new List<EmployeeEductaionList>();
                    EmployeeMasterViewModel.Training = new List<Training>();
                    EmployeeMasterViewModel.TrainingExt = new List<TrainingExt>();
                    EmployeeMasterViewModel.Languages = new List<ExceedERP.Reporting.HRM.HRM.HRMViewModel.Languages>();
                    EmployeeMasterViewModel.EmployeePerformances = new List<EmployeePerformances>();
                    EmployeeMasterViewModel.Discipline = new List<Discipline>();

                    if (!string.IsNullOrEmpty(e.Nation))
                    {
                        int.TryParse(e.Nation, out int lookId);
                        var nation = db.Lookups.FirstOrDefault(x => x.LookupId == lookId);
                        if (nation != null)
                        {
                            EmployeeMasterViewModel.Nation = nation.Description;
                        }
                    }

                    var education = db.Education.Where(x => x.EmployeeId == e.EmployeeId).ToList();

                    foreach (var educ in education)
                    {
                        var filed = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduField);
                        var level = db.Lookups.FirstOrDefault(x => x.LookupId == educ.EduLevel);
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = filed != null ? filed.Description : "አልተሞላም";

                        edu.EduLevel = level != null ? level.Description : "አልተሞላም";
                        edu.From = educ.FromEAmharic;
                        edu.To = educ.ToEAmharic;
                        edu.Institute = educ.OrganizationName;

                        EmployeeMasterViewModel.Education.Add(edu);
                    }
                    //// i do this to connect the broke lines when the list is empty
                    if (!EmployeeMasterViewModel.Education.Any())
                    {
                        EmployeeEductaionList edu = new EmployeeEductaionList();

                        edu.EduField = " ";

                        edu.EduLevel = " ";
                        edu.From = " ";
                        edu.To = " ";
                        edu.Institute = " ";

                        EmployeeMasterViewModel.Education.Add(edu);
                    }

                    var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

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
                    EmployeeMasterViewModel.Expiriance = EmployeeHelper.GetDaystoWordsAmharic(expSum);
                    EmployeeMasterViewModel.MartialStatus = HrHelper.GetMartialStatus(e);

                    double totalInternalDays = DateTime.Now.Subtract(e.HiredDate).TotalDays;

                    EmployeeMasterViewModel.InternalExpriance = EmployeeHelper.GetDaystoWordsAmharic(totalInternalDays);

                    var ExpTotalSum = totalInternalDays + expSum;

                    EmployeeMasterViewModel.ExpTotalSum = EmployeeHelper.GetDaystoWordsAmharic(ExpTotalSum);

                    var languageProficiency = db.LanguageOfProficiencies.Where(x => x.EmployeeId == e.EmployeeId);
                    foreach (var lang in languageProficiency)
                    {
                        var language = db.Languages.FirstOrDefault(x => x.LanguageId == lang.LanguageId);
                        if (language != null)
                        {
                            HRMViewModel.Languages lan = new HRMViewModel.Languages();

                            lan.Name = language.Description;
                            EmployeeMasterViewModel.Languages.Add(lan);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Languages.Any())
                    {
                        HRMViewModel.Languages lan = new HRMViewModel.Languages();

                        lan.Name = " ";

                        EmployeeMasterViewModel.Languages.Add(lan);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Training.Any())
                    {
                        Training train = new Training();

                        train.Name = " ";

                        EmployeeMasterViewModel.Training.Add(train);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.TrainingExt.Any())
                    {
                        TrainingExt trainExt = new TrainingExt();

                        trainExt.Name = " ";

                        EmployeeMasterViewModel.TrainingExt.Add(trainExt);
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

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.EmployeePerformances.Any())
                    {
                        EmployeePerformances empPerform = new EmployeePerformances();

                        empPerform.Type = " ";

                        EmployeeMasterViewModel.EmployeePerformances.Add(empPerform);
                    }

                    var disciplineCases = db.DisciplinaryCases.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    foreach (var disc in disciplineCases)
                    {
                        var discipine = new Discipline();

                        var empDisc = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                        if (empDisc != null)
                        {
                            var discMeasure = db.DisciplineMeasures.FirstOrDefault(x => x.MeasuresId == empDisc.MeasuresId);
                            if (discMeasure != null)
                            {
                                discipine.MeasureRank = discMeasure.MeasuresType;
                            }
                            var discMeasureDetail = db.DisciplineMeasuresDetail.FirstOrDefault(x => x.DetailMeasuresId == empDisc.DetailMeasuresId);
                            if (discMeasureDetail != null)
                            {
                                discipine.MeasureTaken = discMeasureDetail.Description;
                            }

                            discipine.Month = empDisc.Month;
                            var disMeasureByMoney = db.DisciplineMeasuerByMoney.FirstOrDefault(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId);
                            if (disMeasureByMoney != null)
                            {
                                discipine.Amount = disMeasureByMoney.Amount;
                            }

                            EmployeeMasterViewModel.Discipline.Add(discipine);
                        }
                    }

                    ////// i did this to fix the broken line when the list become empty
                    if (!EmployeeMasterViewModel.Discipline.Any())
                    {
                        var discipine = new Discipline();

                        discipine.MeasureRank = " ";
                        discipine.MeasureTaken = " ";
                        discipine.Month = 0;
                        discipine.Amount = 0;

                        EmployeeMasterViewModel.Discipline.Add(discipine);
                    }

                    //EmployeeMasterViewModel.InternalExpriance = DateTime.Now.Date.Subtract(new DateTime(e.HiredDate.Year, e.HiredDate.Month,
                    //    e.HiredDate.Day)).TotalDays / 365.25;

                    EmployeeMasterViewModel.HireDate = e.HiredDateAmharic;//placement.AssignmentDateAmharic;
                    EmployeeMasterViewModel.Salary = pl.Salary;
                    EmployeeMasterViewModel.ResponsibilityLevel = HrHelper.GetResponsibilityLevel(pl.ResponsibilityLevel);
                    EmployeeMasterViewModel.EmployementType = HrHelper.GetAssignmentType(pl.AssignmentType);

                    if (e.Status == EmployeeStatus.CONTRACT)
                    {
                        EmployeeMasterViewModel.Employement = "ጊዜያዊ";
                    }
                    else if (e.Status == EmployeeStatus.PERMANENT)
                    {
                        EmployeeMasterViewModel.Employement = "ቋሚ";
                    }

                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == pl.BranchId);
                    if (branch != null)
                    {
                        EmployeeMasterViewModel.BranchName = branch.AmharicName;
                    }
                    var structure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                    if (structure != null)
                    {
                        EmployeeMasterViewModel.StructureName = structure.Name; ;
                    }


                    var jobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == pl.LookupId);

                    if (jobTitle != null)
                    {
                        EmployeeMasterViewModel.JobTitle = jobTitle.Description;
                    }
                    EmployeeMasterViewModel.Salary = pl.Salary;

                    var step = db.Steps.FirstOrDefault(x => x.StepId == pl.StepId);
                    if (step != null)
                    {
                        EmployeeMasterViewModel.Step = step.StepName;
                    }
                    var rank = db.Ranks.FirstOrDefault(x => x.RankId == pl.RankId);
                    if (rank != null)
                    {
                        EmployeeMasterViewModel.Rank = rank.RankName;
                    }

                    if (currentPeriod != null)
                    {
                        var entitle = db.LeavesEntitle.Where(x => x.EmployeeId == e.EmployeeId && !x.IsExpired);

                        if (entitle.Any())
                        {

                            EmployeeMasterViewModel.TotalLeave = entitle.Sum(x => x.Balance);
                        }
                    }

                    EmployeeMasterViewModelList.Add(EmployeeMasterViewModel);

                }

            }

            return EmployeeMasterViewModelList;
        }

        //private decimal GetTakenAmount(int periodId, int empId)
        //{
        //    var taken = db.Leaves.Where(x => x.GlFiscalYearId == periodId && x.EmployeeId == empId).ToList();
        //    if (taken.Any())
        //    {
        //        return taken.Sum(x => x.Amount);
        //    }
        //    return 0;
        //}

        /////////////////////////////////////////////////////////////////////////
        public List<Placement> FilterEmployees(object[] branchId, string orgStructureId, string employeeType)
        {

            List<Placement> emplist = new List<Placement>();

            var checkBr = branchId.ToList();
            //bool checkBra = object.ReferenceEquals(branchId, null);
            var placements = db.Placements.ToList();

            if (checkBr.Any())
            {
                foreach (var b in checkBr)
                {
                    var filt = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, Convert.ToInt32(b)).ToList();

                    emplist.AddRange(filt);
                }
            }
            if (orgStructureId != null)
            {
                emplist = EmployeeHelper.GetEmployees_ByDepartment_FromPlacement(placements, Convert.ToInt32(orgStructureId)).ToList();
            }
            if (!checkBr.Any() && orgStructureId == null)
            {
                emplist = placements;
            }
            return emplist;
        }

        ///////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee.ToList();
        //    var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId).ToList();
        //    if (placements.Any())
        //    {
        //        var filterd = from f in placements
        //                      join e in allEmployee
        //                      on f.EmployeeId equals e.EmployeeId
        //                      where f.EmployeeId == e.EmployeeId
        //                      select (new { e });

        //        foreach (var p in filterd)
        //        {

        //            list.Add(p.e);

        //        }
        //    }
        //    return list;

        //}

        //////////////////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByBranch(int branchId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee.ToList();
        //    var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
        //    if (placements.Any())
        //    {
        //        foreach (var p in placements)
        //        {
        //            var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
        //            if (check != null)
        //            {
        //                list.Add(check);
        //            }
        //        }
        //    }
        //    return list;

        //}

        /////////////////////////////////////////////////////////////////

        //public List<Person_Employee> GetEmployeesByWorkUnit(int orgStructureId)
        //{

        //    var list = new List<Person_Employee>();
        //    var allEmployee = db.Person_Employee;
        //    var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
        //    if (placements.Any())
        //    {
        //        foreach (var p in placements)
        //        {
        //            var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
        //            if (check != null)
        //            {
        //                list.Add(check);
        //            }
        //        }
        //    }
        //    return list;
        //}

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