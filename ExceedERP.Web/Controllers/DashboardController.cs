using System;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Drawing.Charts;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Helpers.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Insurance;

namespace ExceedERP.Web.Controllers
{ 

    public class DashboardController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
        private readonly DashboardHelper _dashboardHelper = new DashboardHelper();

        [AuthorizeRoles(CommonConstants.DashboardHr)]
        public ActionResult HrDashboard()
        {
            return View();
        }
        //public ActionResult HrDashboard()
        //{
        //    var model = new HrDashboardViewModel();
        //    #region hr group
        //    #region Employee Count  Employment Type
        //    var contart = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.CONTRACT).ToList();
        //    var permanent = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PERMANENT).ToList();
        //    var probation = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PROBATION).ToList();
        //    model.PieSeries.Add(new PieSeriesData()
        //    {
        //        category = "Contract",
        //        value = contart.Count(),
        //        Color = "#068c35"
        //    });

        //    model.PieSeries.Add(new PieSeriesData()
        //    {
        //        category = "Permanent",
        //        value = permanent.Count(),
        //        Color = "#90cc38"
        //    });

        //    model.PieSeries.Add(new PieSeriesData()
        //    {
        //        category = "Probation",
        //        value = probation.Count(),
        //        Color = "#9de219"
        //    });


        //    #endregion
        //    #region Employee Count by Gender and Employment Type
        //    var MaleContratemployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.CONTRACT).ToList();
        //    var femaleContEmployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.CONTRACT).ToList();
        //    var MalePermaemployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PERMANENT).ToList();
        //    var femaleEmployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PERMANENT).ToList();
        //    var MaleProbationemployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PROBATION).ToList();
        //    var femaleProbationEmployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PROBATION).ToList();
        //    model.HrSeries.Add(new HrSeriesData()
        //    {
        //        Name = "Male",
        //        Data = new decimal[] { MaleContratemployees.Count(), MalePermaemployees.Count(), MaleProbationemployees.Count(), MaleContratemployees.Count() + MalePermaemployees.Count() + MaleProbationemployees.Count() }
        //    });

        //    model.HrSeries.Add(new HrSeriesData()
        //    {
        //        Name = "Female",
        //        Data = new decimal[] { femaleContEmployees.Count(), femaleEmployees.Count(), femaleProbationEmployees.Count(), femaleContEmployees.Count() + femaleEmployees.Count() + femaleProbationEmployees.Count() }
        //    });

        //    model.HrSeries.Add(new HrSeriesData()
        //    {
        //        Name = "Total",
        //        Data = new decimal[] { MaleContratemployees.Count() + femaleContEmployees.Count(), MalePermaemployees.Count() + femaleEmployees.Count(), MaleProbationemployees.Count() + femaleProbationEmployees.Count(), MalePermaemployees.Count() + femaleEmployees.Count() + femaleContEmployees.Count() + MaleContratemployees.Count() + MaleProbationemployees.Count() + femaleProbationEmployees.Count() }
        //    });
        //    #endregion
        //    #region Employee By Assignment Type

        //    model.HrAssignmentCategories.AddRange(new string[] { "Assignemnt Type" });

        //    model.HrAssignmentSeriesData = new List<HrAssignmentSeriesData>();

        //    var Newemployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.NEW).ToList();
        //    var promotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.PROMOTION).ToList();
        //    var demotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.DEMOTION).ToList();
        //    var recomendedEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.RECOMENDATION).ToList();
        //    var TransfredEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.TRANSFER).ToList();
        //    var structureChange = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.STRUCTURE_CHANGE).ToList();
        //    var assignment = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.ASSIGNMENT).ToList();
        //    var yearlyPromoted = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.YEARLY_PROMOTION).ToList();
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "NEW",
        //        Data = new decimal[] { Newemployees.Count() }
        //    });

        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "PROMOTION",
        //        Data = new decimal[] { promotedEmployees.Count() }
        //    });

        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "DEMOTION",
        //        Data = new decimal[] { demotedEmployees.Count() }
        //    });
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "RECOMENDATION",
        //        Data = new decimal[] { recomendedEmployee.Count() }
        //    });
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "TRANSFER",
        //        Data = new decimal[] { TransfredEmployee.Count() }
        //    });
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "STRUCTURE_CHANGE",
        //        Data = new decimal[] { structureChange.Count() }
        //    });
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "ASSIGNMENT",
        //        Data = new decimal[] { assignment.Count() }
        //    });
        //    model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
        //    {
        //        Name = "YEARLY_PROMOTION",
        //        Data = new decimal[] { yearlyPromoted.Count() }
        //    });
        //    #endregion
        //    #region Employee By Responsiblity Level

        //    model.HrResponsiblityCategories.AddRange(new string[] { "Responsibility Level" });

        //    model.HrResponsiblitySeriesData = new List<HrResponsiblitySeriesData>();

        //    var staff = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.STAFF).ToList();
        //    var low = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.LOW_MANAGMENT).ToList();
        //    var medium = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.MEDIUM_MANAGMENT).ToList();
        //    var top = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT).ToList();

        //    model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
        //    {
        //        Name = "STAFF",
        //        Data = new decimal[] { staff.Count() }
        //    });

        //    model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
        //    {
        //        Name = "LOW_MANAGMENT",
        //        Data = new decimal[] { low.Count() }
        //    });

        //    model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
        //    {
        //        Name = "MEDIUM_MANAGMENT",
        //        Data = new decimal[] { medium.Count() }
        //    });
        //    model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
        //    {
        //        Name = "TOP_MANAGMENT",
        //        Data = new decimal[] { top.Count() }
        //    });

        //    #endregion
        //    #region Leave
        //    var todayD = DateTime.Now.Date;
        //    var today = new DateTime(todayD.Year, todayD.Month, 1);
        //    var prevD = todayD.AddMonths(-1);
        //    var secondMonthD = todayD.AddMonths(-2);

        //    var prevMonth = new DateTime(prevD.Year, prevD.Month, 1);
        //    var secondMonth = new DateTime(secondMonthD.Year, secondMonthD.Month, 1);

        //    var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
        //    var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
        //    var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

        //    model.Categories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.Series = new List<MySeriesData>();

        //    #region Curremt Month
        //    var annualleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= today && x.To <= todayEnd).Count();
        //    var merrageleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= today && x.To <= todayEnd).Count();
        //    var maternityleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= today && x.To <= todayEnd).Count();
        //    var paternityleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= today && x.To <= todayEnd).Count();
        //    var sickleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= today && x.To <= todayEnd).Count();
        //    var otherleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER && x.From >= today && x.To <= todayEnd).Count();
        //    #endregion
        //    #region second Month
        //    var annualleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    var merrageleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    var maternityleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    var paternityleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    var sickleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    var otherleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
        //    #endregion
        //    #region third Month
        //    var annualleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    var merrageleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    var maternityleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    var paternityleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    var sickleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    var otherleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
        //    #endregion
        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "ANNUAL",
        //        //Stack = secondMonth.ToString("MMMM"),
        //        Data = new decimal[] { annualleavethird, annualleavesecond, annualleaveCurrent }
        //    });

        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "MARRIAGE",
        //        //Stack =  secondMonth.ToString("MMMM"),,
        //        Data = new decimal[] { merrageleavethird, merrageleavesecond, merrageleaveCurrent }
        //    });

        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "MATERNITY",
        //        // Stack = "B",
        //        Data = new decimal[] { maternityleavethird, maternityleavesecond, maternityleaveCurrent }
        //    });
        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "PATERNITY",
        //        // Stack = "B",
        //        Data = new decimal[] { paternityleavethird, paternityleavesecond, paternityleaveCurrent }
        //    });
        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "SICK_LEAVE",
        //        // Stack = "B",
        //        Data = new decimal[] { sickleavethird, sickleavesecond, sickleaveCurrent }
        //    });
        //    model.Series.Add(new MySeriesData()
        //    {
        //        Name = "OTHER",
        //        // Stack = "B",
        //        Data = new decimal[] { otherleavethird, otherleavesecond, otherleaveCurrent }
        //    });
        //    #endregion
        //    #region Training
        //    //var today = DateTime.Now.Date;
        //    //var prevMonth = new DateTime(today.Year, today.Month - 1, today.Day);
        //    //var secondMonth = new DateTime(today.Year, today.Month - 2, today.Day);

        //    model.TrainingCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.HrTrainingData = new List<HrTrainingData>();

        //    #region Curremt Month
        //    var trainings = from c in _db.CourseTrackings
        //                    join
        //                        t in _db.Trainee
        //                        on c.CourseTrackingId equals t.CourseTrackingId

        //                    select new { c.From, c.To };

        //    var trainingCurrent = trainings.Where(x => x.From >= today && x.To <= todayEnd).Count();



        //    #endregion
        //    #region second Month
        //    var trainingsecond = trainings.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd).Count();

        //    #endregion
        //    #region third Month
        //    var trainingthird = trainings.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd).Count();

        //    #endregion
        //    model.HrTrainingData.Add(new HrTrainingData()
        //    {
        //        Name = "No Of Trainee",
        //        Data = new decimal[] { trainingthird, trainingsecond, trainingCurrent }
        //    });


        //    #endregion
        //    #region Letter
        //    //var today = DateTime.Now.Date;
        //    //var prevMonth = new DateTime(today.Year, today.Month - 1, today.Day);
        //    //var secondMonth = new DateTime(today.Year, today.Month - 2, today.Day);

        //    model.LetterCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.HrLetterData = new List<HrLetterData>();

        //    #region Curremt Month
        //    var incommings = _db.Incomming.ToList();
        //    var outgoings = _db.Outgoing.ToList();

        //    var incommingCurrent = incommings.Where(x => x.RecivedDate >= today && x.RecivedDate <= todayEnd).Count();
        //    var outgoingCurrent = outgoings.Where(x => x.Date >= today && x.Date <= todayEnd).Count();


        //    #endregion
        //    #region second Month
        //    var incommingsecond = incommings.Where(x => x.RecivedDate >= prevMonth && x.RecivedDate <= prevMonthEnd).Count();
        //    var outgoingsecond = outgoings.Where(x => x.Date >= prevMonth && x.Date <= prevMonthEnd).Count();
        //    #endregion
        //    #region third Month
        //    var incommingthird = incommings.Where(x => x.RecivedDate >= secondMonth && x.RecivedDate <= secondMonthEnd).Count();
        //    var outgoingthird = outgoings.Where(x => x.Date >= secondMonth && x.Date <= secondMonthEnd).Count();
        //    #endregion
        //    model.HrLetterData.Add(new HrLetterData()
        //    {
        //        Name = "Incoming",
        //        Data = new decimal[] { incommingthird, incommingsecond, incommingCurrent }
        //    });

        //    model.HrLetterData.Add(new HrLetterData()
        //    {
        //        Name = "Outgoing",
        //        Data = new decimal[] { outgoingthird, outgoingsecond, outgoingCurrent }
        //    });

        //    #endregion
        //    #region Absent

        //    int index = 0;
        //    GLPeriod periodOne = new GLPeriod();
        //    GLPeriod periodTwo = new GLPeriod();
        //    string namecurrent = "";
        //    string nameOne = "";
        //    string nameTwo = "";
        //    List<string> allPeriodsname = new List<string>();
        //    var currentPeriod = _db.GLPeriods.Where(x => today >= x.DateFrom && today <= x.DateTo).FirstOrDefault();
        //    if (currentPeriod != null)
        //    {
        //        namecurrent = currentPeriod.Name;
        //        var allPeriods = _db.GLPeriods.Where(x => x.DateTo <= currentPeriod.DateTo && x.GLPeriodId != currentPeriod.GLPeriodId).OrderBy(x => x.DateTo).ToList();
        //        foreach (var a in allPeriods)
        //        {
        //            if (index == 0)
        //            {
        //                periodOne = new GLPeriod();
        //                periodOne = a;
        //                nameOne = a.Name;
        //            }
        //            else if (index == 1)
        //            {
        //                periodTwo = new GLPeriod();
        //                periodTwo = a;
        //                nameTwo = a.Name;
        //            }

        //            else
        //            {
        //                break;
        //            }
        //            index++;
        //        }
        //    }


        //    if (nameOne != "")
        //    {
        //        allPeriodsname.Add(nameOne);

        //    }
        //    if (nameTwo != "")
        //    {
        //        allPeriodsname.Add(nameTwo);

        //    }
        //    if (namecurrent != "")
        //    {
        //        allPeriodsname.Add(namecurrent);

        //    }
        //    model.AbsentCategories.AddRange(allPeriodsname);
        //    model.HrAbsentData = new List<HrAbsentData>();

        //    #region Curremt Month
        //    var lateComming = _db.LateCommings.ToList();
        //    var abcets = _db.EmpPayrollNoDaysWorkeds.ToList();

        //    var absentCurrent = abcets.Where(x => x.GLPeriodId >= currentPeriod.GLPeriodId).ToList().Sum(x => x.Days);
        //    var lateCurrent = lateComming.Where(x => x.Date >= currentPeriod.DateFrom && x.Date <= currentPeriod.DateTo).ToList().Sum(x => (int)x.Time);


        //    #endregion
        //    #region second Month
        //    var absentsecond = abcets.Where(x => x.GLPeriodId >= periodTwo.GLPeriodId).ToList().Sum(x => x.Days);
        //    var latesecond = lateComming.Where(x => x.Date >= periodTwo.DateFrom && x.Date <= periodTwo.DateTo).ToList().Sum(x => (int)x.Time);
        //    #endregion
        //    #region third Month
        //    var absentthird = abcets.Where(x => x.GLPeriodId >= periodOne.GLPeriodId).ToList().Sum(x => x.Days);
        //    var latethird = lateComming.Where(x => x.Date >= periodOne.DateFrom && x.Date <= periodOne.DateTo).ToList().Sum(x => (int)x.Time);
        //    #endregion
        //    model.HrAbsentData.Add(new HrAbsentData()
        //    {
        //        Name = "Absent",
        //        Data = new decimal[] { absentthird, absentsecond, absentCurrent }
        //    });

        //    model.HrAbsentData.Add(new HrAbsentData()
        //    {
        //        Name = "Late Coming",
        //        Data = new decimal[] { latethird, latesecond, lateCurrent }
        //    });

        //    #endregion
        //    #region  Vacant Postion By Branch
        //    var listOfBranchs = _db.Branch.OrderBy(x => x.Name).ToList();

        //    model.VacantPostionCategories.AddRange(new string[] { "Vacant Postion", "Total" });

        //    model.HrVacantPostionData = new List<HrVacantPostionData>();

        //    decimal branchQuntity = 0;
        //    decimal assignedQuant = 0;
        //    //foreach (var b in listOfBranchs)
        //    //{
        //    //    decimal branchQuntity = 0;
        //    //    var mapping = _db.JobTitleMapping.Where(x => x.BranchId == b.BranchId).ToList();
        //    //    if (mapping.Any())
        //    //    {
        //    //         branchQuntity = mapping.Where(x => x.BranchId == b.BranchId).Sum(x => x.Quntity);
        //    //    }
        //    //    var placments= _db.Placements .Where (x=> x.isCurrent && x.BranchId == b.BranchId ).Count ();




        //    //}
        //    var mapping = _db.JobTitleMapping.ToList();
        //    if (mapping.Any())
        //    {
        //        branchQuntity = mapping.Sum(x => x.Quntity);
        //    }
        //    var placments = _db.Placements.Where(x => x.isCurrent).ToList();
        //    if (placments.Any())
        //    {
        //        assignedQuant = placments.Count();
        //    }
        //    model.HrVacantPostionData.Add(new HrVacantPostionData()
        //    {
        //        Name = "Vacant Postion",
        //        Data = new decimal[] { branchQuntity - assignedQuant, branchQuntity }
        //    });
        //    #endregion
        //    #region Termination
        //    var terminationTypes = _db.Lookups.Where(x => x.LookupType == LookUpTypes.TERMINATIONTYPE).ToList();

        //    model.TerminationCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.HrTerminationData = new List<HrTerminationData>();
        //    foreach (var t in terminationTypes)
        //    {
        //        #region Curremt Month
        //        var terminations = _db.Terminations.Where(x => x.LookupId == t.LookupId).ToList();

        //        var terminationCurrent = terminations.Where(x => x.TerminationDate >= today && x.TerminationDate <= todayEnd).Count();



        //        #endregion
        //        #region second Month
        //        var terminationsecond = terminations.Where(x => x.TerminationDate >= prevMonth && x.TerminationDate <= prevMonthEnd).Count();

        //        #endregion
        //        #region third Month
        //        var terminationthird = terminations.Where(x => x.TerminationDate >= secondMonth && x.TerminationDate <= secondMonthEnd).Count();

        //        #endregion
        //        model.HrTerminationData.Add(new HrTerminationData()
        //        {
        //            Name = t.Description,
        //            Data = new decimal[] { terminationthird, terminationsecond, terminationCurrent }
        //        });
        //    }

        //    #endregion
        //    #region Accident Record
        //    string[] types = { "On Work Place", "Not On Work Place" };

        //    model.AccidentRecordCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.HrAccidentRecordData = new List<HrAccidentRecordData>();
        //    foreach (var t in types)
        //    {
        //        var Accidents = new List<AccidentRecord>();

        //        if (t == "On Work Place")
        //        {
        //            Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace == true).ToList();
        //        }
        //        else
        //        {
        //            Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace == false).ToList();
        //        }
        //        #region Curremt Month


        //        var accidentCurrent = Accidents.Where(x => x.Date >= today && x.Date <= todayEnd).Count();



        //        #endregion
        //        #region second Month
        //        var accidentsecond = Accidents.Where(x => x.Date >= prevMonth && x.Date <= prevMonthEnd).Count();

        //        #endregion
        //        #region third Month
        //        var accidentthird = Accidents.Where(x => x.Date >= secondMonth && x.Date <= secondMonthEnd).Count();

        //        #endregion
        //        model.HrAccidentRecordData.Add(new HrAccidentRecordData()
        //        {
        //            Name = t,
        //            Data = new decimal[] { accidentthird, accidentsecond, accidentCurrent }
        //        });
        //    }

        //    #endregion
        //    #region Vacancy
        //    string[] vacancytypes = { "INTERNAL", "EXTERNAL" };
        //    int vacancyCurrent = 0;
        //    int vacancysecond = 0;
        //    int vacancythird = 0;
        //    model.VacancyCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
        //    model.HrVacancyData = new List<HrVacancyData>();
        //    foreach (var t in vacancytypes)
        //    {
        //        var vacancies = new List<Vacancy>();
        //        var allVacancy = from v in _db.VaccancyPlans
        //                         join r in _db.RecruitmentVacancy
        //                         on v.VaccancyCode equals r.VacancyId.ToString()
        //                         select new { v.From, v.To, v.RecruSource };

        //        #region Curremt Month

        //        if (t == "INTERNAL")
        //        {
        //            // vacancies = allVacancy.Where(v => ).ToList();

        //            vacancyCurrent = allVacancy.Where(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
        //            vacancysecond = vacancies.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
        //            vacancythird = vacancies.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
        //        }
        //        else
        //        {
        //            vacancyCurrent = allVacancy.Where(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.EXTERNAL).Count();
        //            vacancysecond = vacancies.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL).Count();
        //            vacancythird = vacancies.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL).Count();
        //        }




        //        #endregion


        //        model.HrVacancyData.Add(new HrVacancyData()
        //        {
        //            Name = t,
        //            Data = new decimal[] { vacancythird, vacancythird, vacancyCurrent }
        //        });
        //    }

        //    #endregion
        //    #endregion

        //    return View(model);
        //}

        [AuthorizeRoles(CommonConstants.DashboardInventory)]
        public ActionResult InventoryDashboard()
        {
     

            return View();
        }


        //[AuthorizeRoles(CommonConstants.DashboardFixedAsset)]
        public ActionResult FixedAssetDashboard()
        {
            return View();
        }

        [AuthorizeRoles(CommonConstants.DashboardProcurement)]
        public ActionResult ProcurementDashboard()
        {

            return View();
        }

        [AuthorizeRoles(CommonConstants.DashboardFinance)]
        public ActionResult FinanceDashboard()
        {
            return View();
        }
        public ActionResult ConstructionDashboard()
        {
           

            return View();
        }
        public class ChartData
        {
            public decimal Value { get; set; }
            public string Label { get; set; }
        }

        #region finance

        private decimal nth = 1000000;//to change to millions

        #region inventory transfer status

        public JsonResult GetFinanceIssueStatus()
        {
            var list = GetchartData("FINANCE_ISSUE_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFinanceTransferSendStatus()
        {
            var list = GetchartData("FINANCE_TRANSFERSEND_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFinanceTransferReceiveStatus()
        {
            var list = GetchartData("FINANCE_TRANSFERRECEIVE_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFinanceGrnStatus()
        {
            var list = GetchartData("FINANCE_GRN_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFinanceStoreReturnStatus()
        {
            var list = GetchartData("FINANCE_STORERETURN_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFinanceStockAdjustmentStatus()
        {
            var list = GetchartData("FINANCE_STOCKADJUSTMENT_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult OtherAccount()
        {
            var list = GetBankAndCashChartData("OTHER_ACCOUNT");
            var labels = GetBankAndCashChartMonths("OTHER_ACCOUNT");
            var formattedData = new
            {
                Labels = labels,
                DataSet = list.ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCashAndBank()
        {
            var list = GetBankAndCashChartData("CASH_BANK");
            var labels = GetBankAndCashChartMonths("CASH_BANK");
            var formattedData = new
            {
                Labels = labels.ToArray(),
                DataSet = list.ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIncomeExpense()
        {
            var list = GetIncomeExpensechartData("INCOME_EXPENSE");
            var formattedData = new
            {
                DataProfit = list.Select(x => x.Profit).ToArray(),
                DataExpense = list.Select(x => x.Expense).ToArray(),
                DataIncome = list.Select(x => x.Income).ToArray(),
                Labels = list.Select(x => x.Month).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApInvoiceAndPayment()
        {
            var list = GetchartData("AP_INVOICE_PAYMENT");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArInvoiceAndCollection()
        {
            var list = GetchartData("AR_INVOICE_COLLECTION");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplierStatus()
        {
            var list = GetchartData("AP_SUPPLIER");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApInvoiceStatus()
        {
            var list = GetchartData("AP_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x=>x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApPaymentStatus()
        {
            var list = GetchartData("AP_PAYMENT_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArInvoiceStatus()
        {
            var list = GetchartData("AR_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArCollectionStatus()
        {
            var list = GetchartData("AR_COLLECTION_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlStatus()
        {
            var list = GetchartData("GL_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        private List<ChartData> GetchartData(string type)
        {
            List<ChartData> list = new List<ChartData>();
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList().DistinctByField(x => x.Name);
            foreach (var dic in dictionaries)
            {
                var chart = new ChartData
                {
                    Label = dic.Name,
                    Value = dic.Value
                };

                list.Add(chart);
            }

            return list;
        }

        public class IncomeExpenseChartData
        {
            public string Month { get; set; }
            public decimal Profit { get; set; }
            public decimal Expense { get; set; }
            public decimal Income { get; set; }

        }
        private List<IncomeExpenseChartData> GetIncomeExpensechartData(string type)
        {
            var periodList = _fiscalYearHelper.GetRecentNPeriodsInYear(12).Select(x => x.Name);
            List<IncomeExpenseChartData> list = new List<IncomeExpenseChartData>();
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
            var uniquedictionaries = dictionaries.Where(x => periodList.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

            var data = dictionaries.ToList();

            foreach (var dic in uniquedictionaries)
            {
                var chart = new IncomeExpenseChartData
                {
                    Month = dic.Dimesion
                };

                var profit = data.Where(x=>x.Dimesion==dic.Dimesion).FirstOrDefault(x => x.Name == "Profit");
                if (profit != null) chart.Profit = Math.Round((profit.Value/nth),2);

                var expense = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Expense");
                if (expense != null) chart.Expense = Math.Round((expense.Value/nth),2);

                var income = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Income");
                if (income != null) chart.Income = Math.Round((income.Value/nth),2);


                list.Add(chart);
            }

            return list;
        }

        public class AccountWatchChartData
        {
            public string label { get; set; }
            public List<decimal> data { get; set; }

            //public string backgroundColor ="window.chartColors.red";
            //    "window.chartColors.orange",
            //    "window.chartColors.blue",
            //    "window.chartColors.green",
            //    "window.chartColors.yellow"
            //};

            public bool fill = false;
        }

        public class AccountWatchListData
        {
            public decimal data { get; set; }
        }
        private List<AccountWatchChartData> GetBankAndCashChartData(string type)
        {
            
            var periodList = _fiscalYearHelper.GetRecentNPeriodsInYear(12).Select(x => x.Name);

            List<AccountWatchChartData> list = new List<AccountWatchChartData>();
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
            var dimenstions = dictionaries.Where(x => periodList.Contains(x.Dimesion)).ToList().Select(x => x.Dimesion).Distinct();
            var names = dictionaries.ToList().Select(x => x.Name).Distinct();

            var data = dictionaries.ToList();

            foreach (var name in names)
            {
                var chart = new AccountWatchChartData();

                chart.label = name;
                chart.data = new List<decimal>();
                List<AccountWatchListData> dataList = new List<AccountWatchListData>();

                foreach (var dim in dimenstions)
                {
                    var dataitem = new decimal();
                    var value = data.Where(x => x.Dimesion == dim).FirstOrDefault(x => x.Name == name);
                    if (value != null) 
                    chart.data.Add(Math.Round((value.Value/nth),2));
                }

                list.Add(chart);
            }

            return list;
        }

        private string[] GetBankAndCashChartMonths(string type)
        {
            var periodList = _fiscalYearHelper.GetRecentNPeriodsInYear(12).Select(x=>x.Name);

            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();
            string[] months = dictionaries.Where(x=>periodList.Contains(x.Dimesion)).Select(x => x.Dimesion).Distinct().ToArray();

            return months;
        }
        #endregion

        #region construction

        public JsonResult GetActiveProjectTypeData()
        {
            var data = GetActiveProjects();

            var formatedData = new
            {
                Types = data.Select(l => l.ProjectType).ToArray(),
                PCounts = data.Select(l => l.NoOfProject).ToArray()
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetClosedProjectTypeData()
        {
            var data = GetClosedProjects();

            var formatedData = new
            {
                Types = data.Select(l => l.ProjectType).ToArray(),
                PCounts = data.Select(l => l.NoOfProject).ToArray()
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProjectPlanVsAchivementStatus()
        {
            var data = GetPlanVsAchivementStatusData(ConstructionConstants.ProjectPlanAndAchivementStatusCategory);
            var dataToReturn = new
            {
                PlannedCost = data.Select(p => p.PlannedCost).ToArray(),
                ActualCost = data.Select(p => p.ActualCost).ToArray(),
                VariationCost = data.Select(p => p.VariationCost).ToArray(),
                Labels = data.Select(p => p.ProjectName).ToArray()
            };
            return Json(dataToReturn, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProjectCostStatusData()
        {
            var data = GetCostStatusData(ConstructionConstants.ProjectCostStatusCategory);
            var dataToReturn = new
            {
                ContractAmountData = data.Select(d => d.TotalContractAmount).ToArray(),
                ToDateAmountData = data.Select(d => d.ActualCost).ToArray(),
                RemainingAmountData = data.Select(d => d.RemainingAmount).ToArray(),
                Labels = data.Select(d => d.ProjectName).ToArray()
            };
            return Json(dataToReturn, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectTimeStatusData()
        {
            var data = GetTimeStatusData(ConstructionConstants.ProjectTimeStatusCategory);
            var dataToReturn = new
            {
                TotalContractTimeData = data.Select(d => d.TotalContractTime).ToArray(),
                TimeElapsedData = data.Select(d => d.TimeElapsed).ToArray(),
                RemainingTimeData = data.Select(d => d.RemainingTime).ToArray(),
                Labels = data.Select(d => d.ProjectName).ToArray()
            };
            return Json(dataToReturn, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProjectPaymentStatusData()
        {
            var data = GetPaymentStatusData(ConstructionConstants.ProjectPaymentAmountCategory);
            var dataToReturn = new
            {
                TotalContractAmountData = data.Select(d => d.TotalContractAmount).ToArray(),
                PaymentAmountTodateData = data.Select(d => d.PaymentAmountTodate).ToArray(),
                RemainingAmountData = data.Select(d => d.RemainingAmount).ToArray(),
                Labels = data.Select(d => d.ProjectName).ToArray()
            };
            return Json(dataToReturn, JsonRequestBehavior.AllowGet);
        }

        private List<ConstructionProjectTypeData> GetActiveProjects()
        {
            List<ConstructionProjectTypeData> list = new List<ConstructionProjectTypeData>();

            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED).ToList();
            var buildings = projects.Where(p => p.ProjectType == AvailableProjectTypes.Building).ToList();
            if (buildings.Any())
            {
                var bRecord = new ConstructionProjectTypeData
                {
                    ProjectType = "Building",
                    NoOfProject = buildings.Count()
                };
                list.Add(bRecord);
            }
            var roads = projects.Where(p => p.ProjectType == AvailableProjectTypes.Road).ToList();
            if (roads.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Road",
                    NoOfProject = roads.Count()
                };
                list.Add(record);
            }
            var dams = projects.Where(p => p.ProjectType == AvailableProjectTypes.Dam).ToList();
            if (dams.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Dam",
                    NoOfProject = dams.Count()
                };
                list.Add(record);
            }
            var irrigatios = projects.Where(p => p.ProjectType == AvailableProjectTypes.Irrigation).ToList();
            if (irrigatios.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Irrigation",
                    NoOfProject = irrigatios.Count()
                };
                list.Add(record);
            }
            return list;
        }

        private List<ConstructionProjectTypeData> GetClosedProjects()
        {
            List<ConstructionProjectTypeData> list = new List<ConstructionProjectTypeData>();

            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus == ProjectStatus.CLOSED).ToList();
            var buildings = projects.Where(p => p.ProjectType == AvailableProjectTypes.Building).ToList();
            if (buildings.Any())
            {
                var bRecord = new ConstructionProjectTypeData
                {
                    ProjectType = "Building",
                    NoOfProject = buildings.Count()
                };
                list.Add(bRecord);
            }
            var roads = projects.Where(p => p.ProjectType == AvailableProjectTypes.Road).ToList();
            if (roads.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Road",
                    NoOfProject = roads.Count()
                };
                list.Add(record);
            }
            var dams = projects.Where(p => p.ProjectType == AvailableProjectTypes.Dam).ToList();
            if (dams.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Dam",
                    NoOfProject = dams.Count()
                };
                list.Add(record);
            }
            var irrigatios = projects.Where(p => p.ProjectType == AvailableProjectTypes.Irrigation).ToList();
            if (irrigatios.Any())
            {
                var record = new ConstructionProjectTypeData
                {
                    ProjectType = "Irrigation",
                    NoOfProject = irrigatios.Count()
                };
                list.Add(record);
            }
            return list;
        }
        private List<ProjectPlanVsAchivementStatus> GetPlanVsAchivementStatusData(string category)
        {
            List<ProjectPlanVsAchivementStatus> list = new List<ProjectPlanVsAchivementStatus>();
            var dictionaries = _db.DashboardDictionaries.Where(d => d.Category == category).ToList();
            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).OrderBy(p => p.Name).ToList();
            if (projects.Any() && dictionaries.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var chartData = new ProjectPlanVsAchivementStatus
                    {
                        ProjectName = project.Name
                    };
                    var plannedCostDict = dictionaries.FirstOrDefault(d => d.Dimesion == project.Name && d.Name == ConstructionConstants.ProjectPlan);
                    if (plannedCostDict != null)
                    {
                        chartData.PlannedCost = plannedCostDict.Value;
                    }

                    var actualCostDict = dictionaries.FirstOrDefault(d => d.Dimesion == project.Name && d.Name == ConstructionConstants.ProjectAchivement);
                    if (actualCostDict != null)
                    {
                        chartData.ActualCost = actualCostDict.Value;
                    }

                    chartData.VariationCost = Math.Abs(chartData.PlannedCost - chartData.ActualCost);

                    list.Add(chartData);
                }
            }
            return list;
        }
        private List<ProjectCostStatus> GetCostStatusData(string category)
        {
            List<ProjectCostStatus> list = new List<ProjectCostStatus>();
            var dictionaries = _db.DashboardDictionaries.Where(d => d.Category == category).ToList();
            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).OrderBy(p => p.Name).ToList();
            if (projects.Any() && dictionaries.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var chartData = new ProjectCostStatus
                    {
                        ProjectName = project.Name,
                        TotalContractAmount = project.ContractAmount
                    };

                    var actualCostDict = dictionaries.FirstOrDefault(d => d.Dimesion == project.Name && d.Name == ConstructionConstants.ProjectCostTodate);
                    if (actualCostDict != null)
                    {
                        chartData.ActualCost = actualCostDict.Value;
                    }

                    chartData.RemainingAmount = Math.Round(chartData.TotalContractAmount - chartData.ActualCost, 2);

                    list.Add(chartData);
                }
            }
            return list;
        }
        private List<ProjectTimeStatus> GetTimeStatusData(string category)
        {
            List<ProjectTimeStatus> list = new List<ProjectTimeStatus>();
            var dictionaries = _db.DashboardDictionaries.Where(d => d.Category == category).ToList();
            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).OrderBy(p => p.Name).ToList();
            if (projects.Any() && dictionaries.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var ctrTime = ContractMgmtHelper.GetProjectMainContract(project.ProjectInformationId).TotalProjectPeriod;
                    var chartData = new ProjectTimeStatus
                    {
                        ProjectName = project.Name,
                        TotalContractTime = ctrTime ?? 0,
                    };

                    var timeElapeseDict = dictionaries.FirstOrDefault(d => d.Dimesion == project.Name && d.Name == ConstructionConstants.ProjectTimeElapsedToDate);
                    if (timeElapeseDict != null)
                    {
                        chartData.TimeElapsed = timeElapeseDict.Value;
                    }

                    chartData.RemainingTime = Math.Round(chartData.TotalContractTime - chartData.TimeElapsed, 2);
                    if (chartData.RemainingTime < 0)
                    {
                        chartData.RemainingTime = 0;
                    }
                    list.Add(chartData);
                }
            }
            return list;
        }

        private List<ProjectPaymentAmount> GetPaymentStatusData(string category)
        {
            List<ProjectPaymentAmount> list = new List<ProjectPaymentAmount>();
            var dictionaries = _db.DashboardDictionaries.Where(d => d.Category == category).ToList();
            var projects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).OrderBy(p => p.Name).ToList();
            if (projects.Any() && dictionaries.Any())
            {
                foreach (ProjectInformation project in projects)
                {
                    var ctractAmnt = ContractMgmtHelper.GetProjectMainContract(project.ProjectInformationId).TotalProjectAmount;
                    var chartData = new ProjectPaymentAmount
                    {
                        ProjectName = project.Name,
                        TotalContractAmount = ctractAmnt ?? 0,
                    };

                    var paymentToDateDic = dictionaries.FirstOrDefault(d => d.Dimesion == project.Name && d.Name == ConstructionConstants.ProjectPaymentAmountTodate);
                    if (paymentToDateDic != null)
                    {
                        chartData.PaymentAmountTodate = paymentToDateDic.Value;
                    }

                    chartData.RemainingAmount = Math.Round(chartData.TotalContractAmount - chartData.PaymentAmountTodate, 2);
                    if (chartData.RemainingAmount < 0)
                    {
                        chartData.RemainingAmount = 0;
                    }
                    list.Add(chartData);
                }
            }
            return list;
        }
        #endregion

        #region inventory 

        public JsonResult GetItemBalances(string store)
        {
            List<ChartData> list = new List<ChartData>();

            var itembalance = _db.InventoryItembalances.Where(x=>x.StoreCode!=null).ToList();

            if (!String.IsNullOrWhiteSpace(store))
                itembalance = itembalance.Where(x => x.StoreCode == store).ToList();

           
            var dictionaries = itembalance.ToList().DistinctByField(x => x.ItemCode);
            foreach (var dic in dictionaries)
            {
                var chart = new ChartData
                {
                    Label = dic.InventoryItem.Name,
                    Value = itembalance.Where(x=>x.ItemCode==dic.ItemCode).Sum(x=>x.Balance)
                };

                list.Add(chart);
            }

            

            var formattedData = new
            {
                Data = list.Select(x=>x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryValue()
        {
            var storeItemAssignments = _db.StoreItemAssignments.ToList();
            decimal value = 0;
            foreach (var assignment in storeItemAssignments)
            {
                value = value + assignment.AvgPrice * assignment.Balance;
            }
            
            var formattedData = new
            {
                Data = new decimal[] {value},
                Labels = new string[] { "Value" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInventoryStatus()
        {
            var stores = _db.InventoryStores.Count();
            var category = _db.ItemCategorys.Count();
            var items = _db.InventoryItems.Count();
            var formattedData = new
            {
                Data = new decimal[] { stores, category, items },
                Labels = new string[] { "Stores", "Categories", "Items" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSrStatus()
        {
            var list = GetchartData("SR_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetIssueStatus()
        {
            var list = GetchartData("ISSUE_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransferSendStatus()
        {
            var list = GetchartData("TRANSFERSEND_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTransferReceiveStatus()
        {
            var list = GetchartData("TRANSFERRECEIVE_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGrnStatus()
        {
            var list = GetchartData("GRN_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStoreReturnStatus()
        {
            var list = GetchartData("STORERETURN_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStockAdjustmentStatus()
        {
            var list = GetchartData("STOCKADJUSTMENT_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region fixed asset

        public JsonResult GetFixedAssetItemCount()
        {
            List<ChartData> list = new List<ChartData>();
            var assetRegistrations = _db.AssetRegistrations.AsQueryable();
            var categories = assetRegistrations.Select(x => x.FixedAssetCategoryID).ToList().Distinct();
            foreach (var cat in categories)
            {
                var data = new ChartData
                {
                    Label = _db.FixedAssetCategories.Find(cat)?.CategoryName,
                    Value = assetRegistrations.Count(x => x.FixedAssetCategoryID == cat)
                };

                list.Add(data);
            }

            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetValue()
        {
            var assetRegistrations = _db.AssetRegistrations.Where(x=>x.Status==RegistrationStatus.Active).AsQueryable();
            var formattedData = new
            {
                Data = new decimal[] { assetRegistrations.Sum(x=>x.BookValue) },
                Labels = new string[] { "Value" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetStatus()
        {
            var subcategories = _db.FixedAssetSubCategories.Count();
            var categories = _db.FixedAssetCategories.Count();
            var items = _db.AssetRegistrations.Count(x => x.Status == RegistrationStatus.Active);
            var formattedData = new
            {
                Data = new decimal[] { categories, subcategories, items },
                Labels = new string[] { "Categories", "Sub Categories", "Items" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetSrStatus()
        {
            var list = GetchartData("SR_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetIssueStatus()
        {
            var list = GetchartData("ISSUE_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetTransferSendStatus()
        {
            var list = GetchartData("MACHINETRANSFER_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetGrnStatus()
        {
            var list = GetchartData("GRN_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetStoreReturnStatus()
        {
            var list = GetchartData("STORERETURN_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFixedAssetStockAdjustmentStatus()
        {
            var list = GetchartData("STOCKADJUSTMENT_FIXEDASSET_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region procurement
        public JsonResult GetProcurementStatus()
        {
            var list = GetchartData("PROCUREMENT");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPrStatus()
        {
            var list = GetchartData("PR_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPoStatus()
        {
            var list = GetchartData("PO_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformaStatus()
        {
            var list = GetchartData("PROFORMA_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTenderStatus()
        {
            var list = GetchartData("TENDER_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContractStatus()
        {
            var list = GetchartData("CONTRACT_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceGrntStatus()
        {
            var list = GetchartData("SERVICEGRN_DOC_STATUS");
            var formattedData = new
            {
                Data = list.Select(x => x.Value).ToArray(),
                Labels = list.Select(x => x.Label).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region hrm
        public JsonResult GetEmployeeCount()
        {
            var contrat = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.CONTRACT).ToList();
            var permanent = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PERMANENT).ToList();
            var probation = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PROBATION).ToList();


            var formattedData = new
            {
                Data = new int[] { contrat.Count(), permanent.Count(), probation.Count() },
                Labels = new string[] { "Contract", "Permanent", "Probation" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetContratEmployee()
        {
            var MaleContratemployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.CONTRACT).ToList();
            var femaleContEmployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.CONTRACT).ToList();

            var formattedData = new
            {
                Data = new int[]
                {

                    MaleContratemployees.Count(), femaleContEmployees.Count(),

                    MaleContratemployees.Count()+femaleContEmployees.Count()

                },
                Labels = new string[] { "Male", "Female", "Total" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPermanentEmployee()
        {
            var MalePermaemployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PERMANENT).ToList();
            var femaleEmployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PERMANENT).ToList();


            var formattedData = new
            {
                Data = new int[]
                {

                    MalePermaemployees.Count(), femaleEmployees.Count(),

                    MalePermaemployees.Count()+femaleEmployees.Count()

                },
                Labels = new string[] { "Male", "Female", "Total" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProbationEmployee()
        {
            var MaleProbationemployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PROBATION).ToList();
            var femaleProbationEmployees = _db.Person_Employee
                .Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PROBATION).ToList();

            var formattedData = new
            {
                Data = new int[]
                {

                    MaleProbationemployees.Count(), femaleProbationEmployees.Count(),

                    MaleProbationemployees.Count()+femaleProbationEmployees.Count()

                },
                Labels = new string[] { "Male ", "Female", "Total" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeByAssignmentType()
        {

            var Newemployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.NEW).ToList();
            var promotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.PROMOTION).ToList();
            var demotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.DEMOTION).ToList();
            var recomendedEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.RECOMENDATION).ToList();
            var TransfredEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.TRANSFER).ToList();
            var structureChange = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.STRUCTURE_CHANGE).ToList();
            var assignment = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.ASSIGNMENT).ToList();
            var yearlyPromoted = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.YEARLY_PROMOTION).ToList();


            var formattedData = new
            {
                Data = new int[]
                {
                    Newemployees.Count(), promotedEmployees.Count(), demotedEmployees.Count(),
                    recomendedEmployee.Count(), TransfredEmployee.Count(), structureChange.Count(), assignment.Count(),
                    yearlyPromoted.Count()
                },
                Labels = new string[]
                {
                    "NEW", "PROMOTION", "DEMOTION", "RECOMENDATION", "TRANSFER", "STRUCTURE_CHANGE", "ASSIGNMENT",
                    "YEARLY_PROMOTION"
                }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEmployeeByResponsibilityLevel()
        {
            var staff = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.STAFF).ToList();
            var low = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.LOW_MANAGMENT).ToList();
            var medium = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.MEDIUM_MANAGMENT).ToList();
            var top = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT).ToList();

            var formattedData = new
            {
                Data = new int[]
                {
                    staff.Count() ,low.Count(), medium.Count(), top.Count()
                },
                Labels = new string[] { "Staff", "Low Management", "Medium Management", "Top Management" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetlLeave()
        {

            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));



            #region Curremt Month
            var annualleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= today && x.To <= todayEnd);
            var merrageleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= today && x.To <= todayEnd);
            var maternityleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= today && x.To <= todayEnd);
            var paternityleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= today && x.To <= todayEnd);
            var sickleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= today && x.To <= todayEnd);
            var otherleaveCurrent = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= today && x.To <= todayEnd);
            #endregion
            #region second Month
            var annualleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= prevMonth && x.To <= prevMonthEnd);
            var merrageleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= prevMonth && x.To <= prevMonthEnd);
            var maternityleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd);
            var paternityleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd);
            var sickleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= prevMonth && x.To <= prevMonthEnd);
            var otherleavesecond = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= prevMonth && x.To <= prevMonthEnd);
            #endregion
            #region third Month
            var annualleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.ANNUAL && x.From >= secondMonth && x.To <= secondMonthEnd);
            var merrageleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= secondMonth && x.To <= secondMonthEnd);
            var maternityleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.MATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd);
            var paternityleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.PATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd);
            var sickleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= secondMonth && x.To <= secondMonthEnd);
            var otherleavethird = _db.Leaves.Count(x => x.LeaveType == LeaveType.OTHER && x.From >= secondMonth && x.To <= secondMonthEnd);
            #endregion


            var formattedData = new
            {
                Data = new int[]
                {
                     annualleavethird, annualleavesecond, annualleaveCurrent ,

                },
                Labels = new string[] { "Before Prev", "Prev ", "Current " }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTraining()
        {
            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));


            #region Curremt Month
            var trainings = from c in _db.CourseTrackings
                            join
                                t in _db.Trainee
                                on c.CourseTrackingId equals t.CourseTrackingId

                            select new { c.From, c.To, c.TrainingType };

            var trainingCurrent = trainings.Count(x => x.From >= today && x.To <= todayEnd && x.TrainingType == TrainingType.TRAINING);



            #endregion
            #region second Month
            var trainingsecond = trainings.Count(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.TrainingType == TrainingType.TRAINING);

            #endregion
            #region third Month
            var trainingthird = trainings.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.TrainingType == TrainingType.TRAINING);

            #endregion


            var formattedData = new
            {
                Data = new int[]
                {

                    trainingthird, trainingsecond,trainingCurrent

                },
                Labels = new string[] { "before prev", "Prev  ", "Current " }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducation()
        {
            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));


            #region Curremt Month
            var education = from c in _db.CourseTrackings
                            join
                                t in _db.Trainee
                                on c.CourseTrackingId equals t.CourseTrackingId

                            select new { c.From, c.To, c.TrainingType };

            var educationCurrent = education.Count(x => x.From >= today && x.To <= todayEnd && x.TrainingType == TrainingType.EDUCATION);



            #endregion
            #region second Month
            var educationsecond = education.Count(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.TrainingType == TrainingType.EDUCATION);

            #endregion
            #region third Month
            var educationthird = education.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.TrainingType == TrainingType.EDUCATION);

            #endregion


            var formattedData = new
            {
                Data = new int[]
                {

                    educationthird, educationsecond,educationCurrent

                },
                Labels = new string[] { "before prev", "Prev  ", "Current " }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAbsentEmployee()
        {

            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

            #region Absent

            int index = 0;
            GLPeriod periodOne = new GLPeriod();
            GLPeriod periodTwo = new GLPeriod();
            string namecurrent = "";
            string nameOne = "";
            string nameTwo = "";
            List<string> allPeriodsname = new List<string>();
            var currentPeriod = _db.GLPeriods.FirstOrDefault(x => today >= x.DateFrom && today <= x.DateTo);
            if (currentPeriod != null)
            {
                namecurrent = currentPeriod.Name;
                var allPeriods = _db.GLPeriods.Where(x => x.DateTo <= currentPeriod.DateTo && x.GLPeriodId != currentPeriod.GLPeriodId).OrderBy(x => x.DateTo).ToList();
                foreach (var a in allPeriods)
                {
                    if (index == 0)
                    {
                        periodOne = new GLPeriod();
                        periodOne = a;
                        nameOne = a.Name;
                    }
                    else if (index == 1)
                    {
                        periodTwo = new GLPeriod();
                        periodTwo = a;
                        nameTwo = a.Name;
                    }

                    else
                    {
                        break;
                    }
                    index++;
                }
            }


            if (nameOne != "")
            {
                allPeriodsname.Add(nameOne);

            }
            if (nameTwo != "")
            {
                allPeriodsname.Add(nameTwo);

            }
            if (namecurrent != "")
            {
                allPeriodsname.Add(namecurrent);

            }

            #region Curremt Month
            var lateComming = _db.LateCommings.ToList();
            var abcets = _db.EmpPayrollNoDaysWorkeds.ToList();

            var absentCurrent = abcets.Where(x => currentPeriod != null && x.GLPeriodId >= currentPeriod.GLPeriodId).ToList().Sum(x => x.Days);
            var lateCurrent = lateComming.Where(x => currentPeriod != null && (x.Date >= currentPeriod.DateFrom && x.Date <= currentPeriod.DateTo)).ToList().Sum(x => (int)x.Time);


            #endregion
            #region second Month
            var absentsecond = abcets.Where(x => x.GLPeriodId >= periodTwo.GLPeriodId).ToList().Sum(x => x.Days);
            var latesecond = lateComming.Where(x => x.Date >= periodTwo.DateFrom && x.Date <= periodTwo.DateTo).ToList().Sum(x => (int)x.Time);
            #endregion
            #region third Month
            var absentthird = abcets.Where(x => x.GLPeriodId >= periodOne.GLPeriodId).ToList().Sum(x => x.Days);
            var latethird = lateComming.Where(x => x.Date >= periodOne.DateFrom && x.Date <= periodOne.DateTo).ToList().Sum(x => (int)x.Time);
            #endregion

            #endregion


            var formattedData = new
            {
                Data = new Decimal[,]
                {

                    { absentthird, absentsecond, absentCurrent },
                    { latethird, latesecond, lateCurrent }

                },
                Labels = new string[,] { { "Absent" }, { "Late Coming" } }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacantByBranch()
        {
            #region  Vacant Postion By Branch
            var listOfBranchs = _db.Branch.OrderBy(x => x.Name).ToList();
            var userBranch = UserInformationHelper.GetUserBranchs(User.Identity.Name).FirstOrDefault();
            decimal branchQuntity = 0;
            decimal assignedQuant = 0;
            decimal vacantQuantity = 0;

            var mapping = _db.JobTitleMapping.ToList();
            if (mapping.Any())
            {
                branchQuntity = mapping.Sum(x => x.Quntity);
            }
            var placments = _db.Placements.Where(x => x.isCurrent).ToList();
            if (placments.Any())
            {
                assignedQuant = placments.Count();
            }

            #endregion

            var formattedData = new
            {
                Data = new decimal[]
                {

                    branchQuntity - assignedQuant,assignedQuant

                },
                Labels = new string[] { "Vacant Position", "Assigned Quantity" }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTermination()
        {

            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));




            var allTermination = _db.Terminations.ToList();

            #region Curremt Month

            var terminationCurrent =
                allTermination.Count(x => x.TerminationDate >= today && x.TerminationDate <= todayEnd);

            #endregion

            #region second Month

            var terminationsecond =
                allTermination.Count(x => x.TerminationDate >= prevMonth && x.TerminationDate <= prevMonthEnd);

            #endregion


            #region third Month

            var terminationthird = allTermination.Count(x =>
                x.TerminationDate >= secondMonth && x.TerminationDate <= secondMonthEnd);

            #endregion

            #region commented
            //#region Termination
            //var terminationTypes = _db.Lookups.Where(x => x.LookupType == LookUpTypes.TERMINATIONTYPE).ToList();


            //var formattedData = new
            //{
            //    Data = new int[]{},
            //    Labels = new string[] { }
            //};
            //foreach (var t in terminationTypes)
            //{
            //    #region Curremt Month

            //    //var terminations = _db.Terminations.Where(x => x.LookupId == t.LookupId).ToList();

            //    //var terminationCurrent =
            //    //    terminations.Count(x => x.TerminationDate >= today && x.TerminationDate <= todayEnd);



            //    #endregion

            //    #region second Month

            //    //var terminationsecond =
            //    //    terminations.Count(x => x.TerminationDate >= prevMonth && x.TerminationDate <= prevMonthEnd);

            //    #endregion

            //    #region third Month

            //    //var terminationthird = terminations.Count(x =>
            //    //    x.TerminationDate >= secondMonth && x.TerminationDate <= secondMonthEnd);

            //    #endregion



            //    #endregion

            //     formattedData = new
            //    {
            //        Data = new int[]
            //        {

            //            terminationthird,terminationsecond, terminationCurrent

            //        },
            //        Labels = new string[] {"Month Before Prev","Prev Month","Current Month"}
            //    };
            //}

            #endregion

            var formattedData = new
            {
                Data = new int[]
                    {

                        terminationthird,terminationsecond, terminationCurrent

                    },
                Labels = new string[] { "Before Prev", "Prev ", "Current " }
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccidentRecored()
        {
            #region Accident Record
            string[] types = { "On Work Place", "Not On Work Place" };
            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));
            var formattedData = new
            {
                Data = new int[]
                { },
                Labels = new string[] { }
            };
            foreach (var t in types)
            {
                var Accidents = new List<AccidentRecord>();

                if (t == "On Work Place")
                {
                    Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace == true).ToList();
                }
                else
                {
                    Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace == false).ToList();
                }
                #region Curremt Month


                var accidentCurrent = Accidents.Count(x => x.Date >= today && x.Date <= todayEnd);



                #endregion
                #region second Month
                var accidentsecond = Accidents.Count(x => x.Date >= prevMonth && x.Date <= prevMonthEnd);

                #endregion
                #region third Month
                var accidentthird = Accidents.Count(x => x.Date >= secondMonth && x.Date <= secondMonthEnd);

                #endregion

                formattedData = new
                {
                    Data = new int[]
                    {

                        accidentthird,
                        accidentsecond, accidentCurrent

                    },
                    Labels = new string[] { "On Work Place", "Not On Work Place" }
                };
            }

            #endregion


            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancy()
        {
            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month, 1);
            var prevMonth = today.AddMonths(-1);
            var secondMonth = today.AddMonths(-2);

            var todayEnd = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));

            #region Vacancy
            string[] vacancytypes = { "INTERNAL", "EXTERNAL" };
            int vacancyCurrent = 0;
            int vacancysecond = 0;
            int vacancythird = 0;

            var formattedData = new
            {
                Data = new int[]
                { },
                Labels = new string[] { }
            };

            foreach (var t in vacancytypes)
            {
                var vacancies = new List<Vacancy>();
                var allVacancy = from v in _db.VaccancyPlans
                                 join r in _db.RecruitmentVacancy
                                 on v.VacancyId equals r.VacancyId
                                 select new { v.From, v.To, v.RecruSource };

                #region Curremt Month

                if (t == "INTERNAL")
                {
                    // vacancies = allVacancy.Where(v => ).ToList();

                    vacancyCurrent = allVacancy.Count(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.INTERNAL);
                    vacancysecond = vacancies.Count(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL);
                    vacancythird = vacancies.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL);
                }
                else
                {
                    vacancyCurrent = allVacancy.Count(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.EXTERNAL);
                    vacancysecond = vacancies.Count(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL);
                    vacancythird = vacancies.Count(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL);
                }


                #endregion

                formattedData = new
                {
                    Data = new int[]
                     {

                        vacancythird,
                        vacancythird, vacancyCurrent

                     },
                    Labels = new string[] { "Before Prev", "Prev ", "Current " }
                };
            }

            #endregion

            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayrollPerMonth()
        {
            List<PayrollDashboardViewModel> payrollProcess = new List<PayrollDashboardViewModel>();

            var fiscalyearname = _fiscalYearHelper.GetCurrentFYearName();

            var fiscalYear = _db.GLFiscalYears.FirstOrDefault(x => x.Name == fiscalyearname);

            decimal totalAllowance = 0;
            decimal totalDeduction = 0;
            decimal totalAddition = 0;
            decimal totalEarn = 0;
            decimal totalLoan = 0;


            var payrolList = _db.PayrollProcessViewModel.ToList();
            if (fiscalYear != null)
            {

                var periods =
                    _db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId).OrderBy(x => x.Index).ToList();

                for (int i = 0; i < periods.Count(); i++)
                {
                    PayrollDashboardViewModel payroll = new PayrollDashboardViewModel();
                    payroll.PayrollPeriod = new String[1] { periods[i].Name };
                    var payrollPeriod = payrolList.Where(x => x.GLPeriodId == periods[i].GLPeriodId).ToList();

                    if (payrollPeriod.Any())
                    {

                        totalEarn = payrollPeriod.Sum(x => x.GrossEarning);
                        totalLoan = payrollPeriod.Sum(x => x.TotalLoan);
                        totalAllowance = payrollPeriod.Sum(x => x.TotalAllowance);
                        totalDeduction = payrollPeriod.Sum(x => x.TotalDeductions);
                        totalAddition = payrollPeriod.Sum(x => x.TotalAdditions);

                    }

                    payroll.PayrollData = new decimal[5] { totalEarn, totalLoan, totalAllowance, totalDeduction, totalAddition };
                    payrollProcess.Add(payroll);

                }


            }
            var formattedData = new
            {
                Data = payrollProcess.Select(x => x.PayrollData).ToArray(),
                Labels = payrollProcess.Select(x => x.PayrollPeriod).ToArray()
            };

            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public class PayrollDashboardViewModel
        {
            public string[] PayrollPeriod { get; set; }
            public decimal[] PayrollData { get; set; }
        }
        #endregion

        public ActionResult FinanceRatioDashBoard()
        {
            return View();
        }






        #region Ratio DataSource
       

        public async Task<JsonResult> GetProfitabilityRatio()
        {


            var list = await GetProfitabilityRatiochartData();

            //List<ProfitabilityRatioViewModel> list = new List<ProfitabilityRatioViewModel>();
            //list.Add(new ProfitabilityRatioViewModel { Operating_Profit_Margin = 100, Return_On_Assets = 100, Return_On_Equity = 100, Month = "January" });
            //list.Add(new ProfitabilityRatioViewModel { Gross_Profit_Margin = 800, Return_On_Equity = 500, Cost_Of_Goods_Sold = 0, Month = "April" });


            var formattedData = new
            {
                Cost_Of_Goods_Sold = list.Select(x => x.Cost_Of_Goods_Sold).ToArray(),
                Gross_Profit_Margin = list.Select(x => x.Gross_Profit_Margin).ToArray(),
                Net_Profit_Margin = list.Select(x => x.Net_Profit_Margin).ToArray(),
                Return_On_Assets = list.Select(x => x.Return_On_Assets).ToArray(),
                Return_On_Equity = list.Select(x => x.Return_On_Equity).ToArray(),
                Operating_Profit_Margin = list.Select(x => x.Operating_Profit_Margin).ToArray(),
                Labels = list.Select(x => x.Month).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetLiquidityRatio()
        {


            var list = await GetLiquidityRatiochartData();


            //List<LiquidityRatioViewModel> list = new List<LiquidityRatioViewModel>();
            //list.Add(new LiquidityRatioViewModel { Net_Working_Capital_Ratio = 100, Quick_Ratio = 100, Current_Ratio = 100, Month = "January" });
            //list.Add(new LiquidityRatioViewModel { Net_Working_Capital_Ratio = 800, Quick_Ratio = 500, Current_Ratio = 0, Month = "April" });


            var formattedData = new
            {
                Cash_Ratio = list.Select(x => x.Cash_Ratio).ToArray(),
                Current_Ratio = list.Select(x => x.Current_Ratio).ToArray(),
                Net_Working_Capital_Ratio = list.Select(x => x.Net_Working_Capital_Ratio).ToArray(),
                Quick_Ratio = list.Select(x => x.Quick_Ratio).ToArray(),
                Labels = list.Select(x => x.Month).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetActivityRatio()
        {


            var list = await GetActivityRatiochartData();

            //List<ActivityRatioViewModel> list = new List<ActivityRatioViewModel>();
            //list.Add(new ActivityRatioViewModel { Average_Collection_Period = 100, Fixed_Asset_Turnover = 100, Inventory_TurnOver = 100, Month = "January" });
            //list.Add(new ActivityRatioViewModel { Inventory_TurnOver = 800, Receivables_TurnOver = 500, Fixed_Asset_Turnover = 0, Month = "April" });

            var formattedData = new
            {
                Asset_TurnOver = list.Select(x => x.Asset_TurnOver).ToArray(),
                Average_Collection_Period = list.Select(x => x.Average_Collection_Period).ToArray(),
                Fixed_Asset_Turnover = list.Select(x => x.Fixed_Asset_Turnover).ToArray(),
                Inventory_TurnOver = list.Select(x => x.Inventory_TurnOver).ToArray(),
                Receivables_TurnOver = list.Select(x => x.Receivables_TurnOver).ToArray(),
                Labels = list.Select(x => x.Month).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        public async Task<JsonResult> GetCapitalStructureRatio()
        {


            var list = await GetCapitalStructureRatiochartData();

            //List<CapitalStructureRatioViewModel> list = new List<CapitalStructureRatioViewModel>();
            //list.Add(new CapitalStructureRatioViewModel { Debt_To_Assets_Ratio = 200, Debt_To_Equity_Ratio = 800,  Month = "January" });
            //list.Add(new CapitalStructureRatioViewModel { Debt_To_Assets_Ratio = 800, Debt_To_Equity_Ratio = 500 , Month = "April" });

            var formattedData = new
            {
                Debt_To_Assets_Ratio = list.Select(x => x.Debt_To_Assets_Ratio).ToArray(),
                Debt_To_Equity_Ratio = list.Select(x => x.Debt_To_Equity_Ratio).ToArray(),
                Labels = list.Select(x => x.Month).ToArray()
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Ratio DataSource Util

        //private async Task<List<IncomeExpenseChartData>> GetIncomeExpensechartData(string type)
        //{
        //        var periodList = await FiscalYearHelper.GetRecentNPeriodsInYearAsync(6);
        //    var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();

        //    List<IncomeExpenseChartData> list = new List<IncomeExpenseChartData>();
        //    var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == type).ToList();

        //    var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

        //    var data = dictionaries.ToList();

        //    foreach (var dic in uniquedictionaries)
        //    {
        //        var chart = new IncomeExpenseChartData
        //        {
        //            Month = dic.Dimesion
        //        };

        //        var profit = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Profit");
        //        if (profit != null) chart.Profit = Math.Round((profit.Value / nth), 2);

        //        var expense = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Expense");
        //        if (expense != null) chart.Expense = Math.Round((expense.Value / nth), 2);

        //        var income = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Income");
        //        if (income != null) chart.Income = Math.Round((income.Value / nth), 2);


        //        list.Add(chart);
        //    }

        //    return list;
        //}

        private async Task<List<ActivityRatioViewModel>> GetActivityRatiochartData()
        {
            nth = 1;
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
            var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();


            List<ActivityRatioViewModel> list = new List<ActivityRatioViewModel>();
            string Type = "ACTIVITY_RATIO";


            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
            var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

            var data = dictionaries.ToList();

            foreach (var dic in uniquedictionaries)
            {
                var chart = new ActivityRatioViewModel
                {
                    Month = dic.Dimesion
                };

                var Asset_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Asset_TurnOver");
                if (Asset_TurnOver != null) chart.Asset_TurnOver = Math.Round((Asset_TurnOver.Value / nth), 2);

                var Average_Collection_Period = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Average_Collection_Period");
                if (Average_Collection_Period != null) chart.Average_Collection_Period = Math.Round((Average_Collection_Period.Value / nth), 2);

                var Fixed_Asset_Turnover = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Fixed_Asset_Turnover");
                if (Fixed_Asset_Turnover != null) chart.Fixed_Asset_Turnover = Math.Round((Fixed_Asset_Turnover.Value / nth), 2);

                var Inventory_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Inventory_TurnOver");
                if (Inventory_TurnOver != null) chart.Inventory_TurnOver = Math.Round((Asset_TurnOver.Value / nth), 2);

                var Receivables_TurnOver = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Receivables_TurnOver");
                if (Receivables_TurnOver != null) chart.Receivables_TurnOver = Math.Round((Receivables_TurnOver.Value / nth), 2);



                list.Add(chart);
            }

            return list;
        }
        private async Task<List<LiquidityRatioViewModel>> GetLiquidityRatiochartData()
        {
            nth = 1;
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
            var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
            List<LiquidityRatioViewModel> list = new List<LiquidityRatioViewModel>();
            string Type = "LIQUIDITY_RATIO";
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
            var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

            var data = dictionaries.ToList();

            foreach (var dic in uniquedictionaries)
            {
                var chart = new LiquidityRatioViewModel
                {
                    Month = dic.Dimesion
                };

                var Cash_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Cash_Ratio");
                if (Cash_Ratio != null) chart.Cash_Ratio = Math.Round((Cash_Ratio.Value / nth), 2);

                var Net_Working_Capital_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Net_Working_Capital_Ratio");
                if (Net_Working_Capital_Ratio != null) chart.Net_Working_Capital_Ratio = Math.Round((Net_Working_Capital_Ratio.Value / nth), 2);

                var Quick_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Quick_Ratio");
                if (Quick_Ratio != null) chart.Quick_Ratio = Math.Round((Quick_Ratio.Value / nth), 2);



                var Current_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Current_Ratio");
                if (Current_Ratio != null) chart.Current_Ratio = Math.Round((Cash_Ratio.Value / nth), 2);


                list.Add(chart);
            }

            return list;
        }
        private async Task<List<CapitalStructureRatioViewModel>> GetCapitalStructureRatiochartData()
        {
            nth = 1;
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(12);
            var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
            List<CapitalStructureRatioViewModel> list = new List<CapitalStructureRatioViewModel>();
            string Type = "CAPITAL_STRUCTURE_RATIO";
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
            var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

            var data = dictionaries.ToList();

            foreach (var dic in uniquedictionaries)
            {
                var chart = new CapitalStructureRatioViewModel
                {
                    Month = dic.Dimesion
                };

                var Debt_To_Assets_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Debt_To_Assets_Ratio");
                if (Debt_To_Assets_Ratio != null) chart.Debt_To_Assets_Ratio = Math.Round((Debt_To_Assets_Ratio.Value / nth), 2);

                var Debt_To_Equity_Ratio = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Debt_To_Equity_Ratio");
                if (Debt_To_Equity_Ratio != null) chart.Debt_To_Equity_Ratio = Math.Round((Debt_To_Equity_Ratio.Value / nth), 2);



                list.Add(chart);
            }

            return list;
        }
        private async Task<List<ProfitabilityRatioViewModel>> GetProfitabilityRatiochartData()
        {
            nth = 1;
            FiscalYearHelper _FiscalYearHelper = new FiscalYearHelper();
            var periodList = await _FiscalYearHelper.GetRecentNPeriodsInYearAsync(6);
            var Periods = periodList.OrderBy(x => x.DateTo).Select(x => x.Name).ToList();
            List<ProfitabilityRatioViewModel> list = new List<ProfitabilityRatioViewModel>();
            string Type = "PROFITABILITY_RATIO";
            var dictionaries = _db.DashboardDictionaries.Where(x => x.Category == Type).ToList();
            var uniquedictionaries = dictionaries.Where(x => Periods.Contains(x.Dimesion)).ToList().DistinctByField(x => x.Dimesion);

            var data = dictionaries.ToList();

            foreach (var dic in uniquedictionaries)
            {
                var chart = new ProfitabilityRatioViewModel
                {
                    Month = dic.Dimesion
                };

                var Cost_Of_Goods_Sold = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Cost_Of_Goods_Sold");
                if (Cost_Of_Goods_Sold != null) chart.Cost_Of_Goods_Sold = Math.Round((Cost_Of_Goods_Sold.Value / nth), 2);

                var Gross_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Gross_Profit_Margin");
                if (Gross_Profit_Margin != null) chart.Gross_Profit_Margin = Math.Round((Gross_Profit_Margin.Value / nth), 2);

                var Net_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Net_Profit_Margin");
                if (Net_Profit_Margin != null) chart.Net_Profit_Margin = Math.Round((Net_Profit_Margin.Value / nth), 2);


                var Operating_Profit_Margin = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Operating_Profit_Margin");
                if (Operating_Profit_Margin != null) chart.Operating_Profit_Margin = Math.Round((Operating_Profit_Margin.Value / nth), 2);

                var Return_On_Assets = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Return_On_Assets");
                if (Return_On_Assets != null) chart.Return_On_Assets = Math.Round((Return_On_Assets.Value / nth), 2);

                var Return_On_Equity = data.Where(x => x.Dimesion == dic.Dimesion).FirstOrDefault(x => x.Name == "Return_On_Equity");
                if (Return_On_Equity != null) chart.Return_On_Equity = Math.Round((Return_On_Equity.Value / nth), 2);


                list.Add(chart);
            }

            return list;
        }

        #endregion

        public JsonResult RefreshInventoryDashboard()
        {
            //_dashboardHelper.UpdateInventoryDocStatusDict();
            _dashboardHelper.UpdateDashboardItemBalance();
            _dashboardHelper.UpdateInventoryDocStatusDict();
            var formattedData = new
            {
                Message = "Update Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshFinanceDashboard()
        {
            _dashboardHelper.UpdateFinanceDocStatusDict();
            var formattedData = new
            {
                Message = "Update Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshFinanceRatioDashboard()
        {
            _dashboardHelper.UpdateDashboardFinanceRatio();
            var formattedData = new
            {
                Message = "Update Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshProcurementDashboard()
        {
            _dashboardHelper.UpdateProcurementDocStatusDict();
            var formattedData = new
            {
                Message = "Update Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RefreshFixedAssetDashboard()
        {
            _dashboardHelper.UpdateFixedAssetDocStatusDict();
            var formattedData = new
            {
                Message = "Update Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        
    }
}