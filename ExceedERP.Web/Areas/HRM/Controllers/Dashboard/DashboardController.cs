//using ExceedERP.Core.Domain.Dashboard;
//using ExceedERP.Core.Domain.Finance.GL;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
//using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System;
using System.Windows.Forms;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;

namespace ExceedERP.Web.Areas.HRM.Controllers.Dashboard
{

    public class MySeriesData
    {
        public string Name { get; set; }
        public string Stack { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class HrSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrTrainingData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrAccidentRecordData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
    public class HrVacancyData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
     public class HrAssignmentSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
     public class HrResponsiblitySeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

     public class HrVacantPostionData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
     public class HrLetterData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
     public class HrAbsentData
     {
         public string Name { get; set; }
         public IEnumerable<decimal> Data { get; set; }
     }
     public class HrTerminationData
     {
         public string Name { get; set; }
         public IEnumerable<decimal> Data { get; set; }
     }
    public class PieSeriesData
    {
        public string category { get; set; }
        public decimal value { get; set; }
        public string Color { get; set; }
    }

  

   

    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
          
            Series = new List<MySeriesData>();
            PieSeries = new List<PieSeriesData>();
            HrSeries = new List<HrSeriesData>();
            HrAssignmentSeriesData = new List<HrAssignmentSeriesData>();
            HrResponsiblitySeriesData = new List<HrResponsiblitySeriesData>();
            HrTrainingData = new List<HrTrainingData>();
            HrVacantPostionData = new List<HrVacantPostionData>();
            HrLetterData = new List<HrLetterData>();
            HrAbsentData = new List<HrAbsentData>();
            HrTerminationData = new List<HrTerminationData>();
            HrAccidentRecordData = new List<HrAccidentRecordData>();
            HrVacancyData = new List<HrVacancyData>();
            Categories = new List<string>();
            HrCategories = new List<string>();
            PieHrCategories = new List<string>();

            HrAssignmentCategories = new List<string>();

            HrResponsiblityCategories = new List<string>();
            TrainingCategories = new List<string>();
            VacantPostionCategories = new List<string>();
            LetterCategories = new List<string>();
            AbsentCategories = new List<string>();
            TerminationCategories = new List<string>();
            AccidentRecordCategories = new List<string>();
            VacancyCategories = new List<string>();

        }

        public List<MySeriesData> Series { get; set; }
        public List<PieSeriesData> PieSeries { get; set; }
        public List<HrSeriesData> HrSeries { get; set; }
        public List<HrAssignmentSeriesData> HrAssignmentSeriesData { get; set; }
        public List<HrResponsiblitySeriesData> HrResponsiblitySeriesData { get; set; }
        public List<HrTrainingData> HrTrainingData { get; set; }
        public List<HrVacantPostionData> HrVacantPostionData { get; set; }
        public List<HrLetterData> HrLetterData { get; set; }
        public List<HrAbsentData> HrAbsentData { get; set; }
        public List<HrTerminationData> HrTerminationData { get; set; }
        public List<HrAccidentRecordData> HrAccidentRecordData { get; set; }
        public List<HrVacancyData> HrVacancyData { get; set; }
        public List<string> Categories { get; private set; }       
        public List<string> HrCategories { get; private set; }
        public List<string> PieHrCategories { get; private set; }
        public List<string> HrAssignmentCategories { get; private set; }
        public List<string> HrResponsiblityCategories { get; private set; }
        public List<string> TrainingCategories { get; private set; }
        public List<string> VacantPostionCategories { get; private set; }
        public List<string> LetterCategories { get; private set; }
        public List<string> AbsentCategories { get; private set; }

        public List<string> TerminationCategories { get; private set; }
        public List<string> AccidentRecordCategories { get; private set; }
        public List<string> VacancyCategories { get; private set; }   
    }

    public class DashboardController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            var model = new DashboardViewModel();
         
            model.HrCategories.AddRange(new string[] { "Contract", "Permanent", "Probation", "Total" });
           

            model.PieHrCategories.AddRange(new string[] { "category", "value", "color" });
            model.PieSeries = new List<PieSeriesData>();

            model.HrSeries = new List<HrSeriesData>();
            #region Employee Count  Employment Type
            var contart = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.CONTRACT).ToList();
            var permanent  = _db.Person_Employee.Where(x => x.IsActive &&  x.Status ==EmployeeStatus.PERMANENT).ToList();
            var probation = _db.Person_Employee.Where(x => x.IsActive && x.Status == EmployeeStatus.PROBATION).ToList();
            model.PieSeries.Add(new PieSeriesData()
            {
                category  = "Contract",
              
                value =   contart.Count() ,
                 Color = "#068c35"
            });

            model.PieSeries.Add(new PieSeriesData()
            {
                category = "Permanent",
                value =  permanent.Count() ,
                Color = "#90cc38"
            });

            model.PieSeries.Add(new PieSeriesData()
            {
                category = "Probation",
                value = probation.Count() ,
                Color="#9de219"
            });
            #endregion
            #region Employee Count by Gender and Employment Type 
            var MaleContratemployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.CONTRACT).ToList();
            var femaleContEmployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.CONTRACT).ToList();
            var MalePermaemployees = _db.Person_Employee.Where(x => x.IsActive &&  x.Gender == Gender.MALE && x.Status == EmployeeStatus.PERMANENT).ToList();
            var femaleEmployees = _db.Person_Employee.Where(x => x.IsActive &&  x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PERMANENT).ToList();
            var MaleProbationemployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.MALE && x.Status == EmployeeStatus.PROBATION).ToList();
            var femaleProbationEmployees = _db.Person_Employee.Where(x => x.IsActive && x.Gender == Gender.FEMALE && x.Status == EmployeeStatus.PROBATION).ToList();
            model.HrSeries.Add(new HrSeriesData()
            {
                Name = "Male",
                Data = new decimal[] { MaleContratemployees.Count(), MalePermaemployees.Count(), MaleProbationemployees.Count(), MaleContratemployees.Count() + MalePermaemployees.Count() + MaleProbationemployees.Count () }
            });

            model.HrSeries.Add(new HrSeriesData()
            {
                Name = "Female",
                Data = new decimal[] { femaleContEmployees.Count(), femaleEmployees.Count(), femaleProbationEmployees.Count(), femaleContEmployees.Count() + femaleEmployees.Count() + femaleProbationEmployees.Count () }
            });

            model.HrSeries.Add(new HrSeriesData()
            {
                Name = "Total",
                Data = new decimal[] { MaleContratemployees.Count() + femaleContEmployees.Count(), MalePermaemployees.Count() + femaleEmployees.Count(), MaleProbationemployees.Count() + femaleProbationEmployees.Count(), MalePermaemployees.Count() + femaleEmployees.Count() + femaleContEmployees.Count() + MaleContratemployees.Count() + MaleProbationemployees.Count() + femaleProbationEmployees.Count() }
            });
            #endregion
            #region Employee By Assignment Type

            model.HrAssignmentCategories.AddRange(new string[] { "Assignemnt Type" });

            model.HrAssignmentSeriesData = new List<HrAssignmentSeriesData>();

            var Newemployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.NEW).ToList();
            var promotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.PROMOTION).ToList();
            var demotedEmployees = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.DEMOTION).ToList();
            var recomendedEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.RECOMENDATION).ToList();
            var TransfredEmployee = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.TRANSFER).ToList();
            var structureChange = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.STRUCTURE_CHANGE).ToList();
            var assignment = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.ASSIGNMENT).ToList();
            var yearlyPromoted = _db.Placements.Where(x => x.isCurrent && x.AssignmentType == AssignmentType.YEARLY_PROMOTION).ToList();
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "NEW",
                Data = new decimal[] { Newemployees.Count () }
            });

            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "PROMOTION",
                Data = new decimal[] { promotedEmployees.Count () }
            });

            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "DEMOTION",
                Data = new decimal[] { demotedEmployees .Count () }
            });
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "RECOMENDATION",
                Data = new decimal[] { recomendedEmployee.Count() }
            });
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "TRANSFER",
                Data = new decimal[] { TransfredEmployee.Count() }
            });
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "STRUCTURE_CHANGE",
                Data = new decimal[] { structureChange.Count() }
            });
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "ASSIGNMENT",
                Data = new decimal[] { assignment.Count() }
            });
            model.HrAssignmentSeriesData.Add(new HrAssignmentSeriesData()
            {
                Name = "YEARLY_PROMOTION",
                Data = new decimal[] { yearlyPromoted.Count() }
            });
            #endregion
            #region Employee By Responsiblity Level

            model.HrResponsiblityCategories.AddRange(new string[] { "Responsibility Level" });

            model.HrResponsiblitySeriesData = new List<HrResponsiblitySeriesData>();

            var staff = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.STAFF).ToList();
            var low = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.LOW_MANAGMENT).ToList();
            var medium = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.MEDIUM_MANAGMENT).ToList();
            var top = _db.Placements.Where(x => x.isCurrent && x.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT).ToList();

            model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
            {
                Name = "STAFF",
                Data = new decimal[] { staff.Count() }
            });

            model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
            {
                Name = "LOW_MANAGMENT",
                Data = new decimal[] { low.Count() }
            });

            model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
            {
                Name = "MEDIUM_MANAGMENT",
                Data = new decimal[] { medium.Count() }
            });
            model.HrResponsiblitySeriesData.Add(new HrResponsiblitySeriesData()
            {
                Name = "TOP_MANAGMENT",
                Data = new decimal[] { top.Count() }
            });
            
            #endregion
            #region Leave
            var todayD = DateTime.Now.Date;
            var today = new DateTime(todayD.Year, todayD.Month , 1);
            var prevMonth = today.AddMonths(-1); //new DateTime(today.Year, today.Month-1, 1);
            var secondMonth = today.AddMonths(-2);//new DateTime(today.Year, today.Month - 2, 1);
          
            var todayEnd = new DateTime(today.Year, today.Month , DateTime.DaysInMonth(today.Year, today.Month));
            var prevMonthEnd = new DateTime(prevMonth.Year, prevMonth.Month, DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month));
            var secondMonthEnd = new DateTime(secondMonth.Year, secondMonth.Month, DateTime.DaysInMonth(secondMonth.Year, secondMonth.Month));
            
            model.Categories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.Series = new List<MySeriesData>();

#region Curremt Month
            var annualleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= today && x.To <= todayEnd).Count();
            var merrageleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= today && x.To <= todayEnd).Count();
            var maternityleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= today && x.To <= todayEnd).Count();
            var paternityleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= today && x.To <= todayEnd).Count();
            var sickleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE &&  x.From >= today && x.To <= todayEnd).Count();
            var otherleaveCurrent = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER &&  x.From >= today && x.To <=todayEnd).Count();
#endregion
            #region second Month
            var annualleavesecond= _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
            var merrageleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
            var maternityleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
            var paternityleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
            var sickleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
            var otherleavesecond = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER && x.From >= prevMonth && x.To <= prevMonthEnd).Count();
#endregion
            #region third Month
            var annualleavethird= _db.Leaves.Where(x => x.LeaveType == LeaveType.ANNUAL && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
            var merrageleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.MARRIAGE && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
            var maternityleavethird= _db.Leaves.Where(x => x.LeaveType == LeaveType.MATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
            var paternityleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.PATERNITY && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
            var sickleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.SICK_LEAVE && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
            var otherleavethird = _db.Leaves.Where(x => x.LeaveType == LeaveType.OTHER && x.From >= secondMonth && x.To <= secondMonthEnd).Count();
#endregion
            model.Series.Add(new MySeriesData()
            {
                Name = "ANNUAL",
               //Stack = secondMonth.ToString("MMMM"),
                Data = new decimal[] { annualleavethird,annualleavesecond,annualleaveCurrent}
            });

            model.Series.Add(new MySeriesData()
            {
                Name = "MARRIAGE",
                //Stack =  secondMonth.ToString("MMMM"),,
                Data = new decimal[] { merrageleavethird, merrageleavesecond, merrageleaveCurrent }
            });

            model.Series.Add(new MySeriesData()
            {
                Name = "MATERNITY",
               // Stack = "B",
                Data = new decimal[] { maternityleavethird, maternityleavesecond, maternityleaveCurrent }
            });
            model.Series.Add(new MySeriesData()
            {
                Name = "PATERNITY",
                // Stack = "B",
                Data = new decimal[] { paternityleavethird, paternityleavesecond, paternityleaveCurrent }
            });
            model.Series.Add(new MySeriesData()
            {
                Name = "SICK_LEAVE",
                // Stack = "B",
                Data = new decimal[] { sickleavethird, sickleavesecond, sickleaveCurrent }
            });
            model.Series.Add(new MySeriesData()
            {
                Name = "OTHER",
                // Stack = "B",
                Data = new decimal[] { otherleavethird, otherleavesecond, otherleaveCurrent }
            });
            #endregion
            #region Training
            //var today = DateTime.Now.Date;
            //var prevMonth = new DateTime(today.Year, today.Month - 1, today.Day);
            //var secondMonth = new DateTime(today.Year, today.Month - 2, today.Day);

            model.TrainingCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.HrTrainingData = new List<HrTrainingData>();

            #region Curremt Month
            var trainings = from c in _db.CourseTrackings
                                  join
                                      t in _db.Trainee
                                      on c.CourseTrackingId equals t.CourseTrackingId

                                  select new { c.From, c.To };

            var trainingCurrent = trainings.Where(x => x.From >= today && x.To <= todayEnd).Count();

           
           
            #endregion
            #region second Month
            var trainingsecond = trainings.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd).Count();
           
            #endregion
            #region third Month
            var trainingthird = trainings.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd).Count();
           
            #endregion
            model.HrTrainingData.Add(new HrTrainingData()
            {
                Name = "No Of Trainee",
               Data = new decimal[] {  trainingthird, trainingsecond,trainingCurrent}
            });

           
            #endregion
            #region Letter
            //var today = DateTime.Now.Date;
            //var prevMonth = new DateTime(today.Year, today.Month - 1, today.Day);
            //var secondMonth = new DateTime(today.Year, today.Month - 2, today.Day);

            model.LetterCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.HrLetterData = new List<HrLetterData>();

            #region Curremt Month
            var incommings = _db.Incomming.ToList();
            var outgoings = _db.Outgoing.ToList();

            var incommingCurrent = incommings.Where(x => x.RecivedDate >= today && x.RecivedDate <= todayEnd).Count();
            var outgoingCurrent = outgoings.Where(x => x.Date >= today && x.Date <= todayEnd).Count();


            #endregion
            #region second Month
            var incommingsecond = incommings.Where(x => x.RecivedDate >= prevMonth && x.RecivedDate <=prevMonthEnd).Count();
            var outgoingsecond = outgoings.Where(x => x.Date >= prevMonth && x.Date <= prevMonthEnd).Count();
            #endregion
            #region third Month
            var incommingthird = incommings.Where(x => x.RecivedDate >= secondMonth && x.RecivedDate <= secondMonthEnd).Count();
            var outgoingthird = outgoings.Where(x => x.Date >= secondMonth && x.Date <= secondMonthEnd).Count();
            #endregion
            model.HrLetterData.Add(new HrLetterData()
            {
                Name = "Incoming",
                Data = new decimal[] { incommingthird, incommingsecond, incommingCurrent }
            });

            model.HrLetterData.Add(new HrLetterData()
            {
                Name = "Outgoing",
                Data = new decimal[] { outgoingthird, outgoingsecond, outgoingCurrent }
            });

            #endregion

            #region Absent
           
            int index = 0;
            GLPeriod periodOne = new GLPeriod();
            GLPeriod periodTwo = new GLPeriod();
            string namecurrent = "";
            string nameOne = "";
            string nameTwo = "";
            List<string> allPeriodsname = new List<string>();
            var currentPeriod = _db.GLPeriods.Where(x => today >= x.DateFrom && today <= x.DateTo).FirstOrDefault();
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
           
           
            if (nameOne!= "")
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
            model.AbsentCategories.AddRange(allPeriodsname);
            model.HrAbsentData = new List<HrAbsentData>();

            #region Curremt Month
           var lateComming = _db.LateCommings.ToList();
            var abcets = _db.EmpPayrollNoDaysWorkeds.ToList();

           var absentCurrent = abcets.Where(x => currentPeriod != null && x.GLPeriodId >= currentPeriod.GLPeriodId).ToList().Sum(x=> x.Days );
           var lateCurrent = lateComming.Where(x => currentPeriod != null && (x.Date>=currentPeriod.DateFrom && x.Date <=currentPeriod.DateTo) ).ToList().Sum(x=> (int)x.Time );


            #endregion
            #region second Month
           var absentsecond = abcets.Where(x => x.GLPeriodId >= periodTwo.GLPeriodId).ToList().Sum(x => x.Days);
           var latesecond = lateComming.Where(x => x.Date >= periodTwo.DateFrom && x.Date <= periodTwo.DateTo).ToList().Sum(x => (int)x.Time);
            #endregion
            #region third Month
           var absentthird = abcets.Where(x => x.GLPeriodId >= periodOne.GLPeriodId).ToList().Sum(x => x.Days);
           var latethird = lateComming.Where(x => x.Date >= periodOne.DateFrom && x.Date <= periodOne.DateTo).ToList().Sum(x => (int)x.Time);
            #endregion
            model.HrAbsentData.Add(new HrAbsentData()
            {
                Name = "Absent",
                Data = new decimal[] { absentthird, absentsecond, absentCurrent }
            });

            model.HrAbsentData.Add(new HrAbsentData()
            {
                Name = "Late Coming",
                Data = new decimal[] { latethird, latesecond, lateCurrent }
            });

            #endregion

            #region  Vacant Postion By Branch
            var listOfBranchs= _db.Branch.OrderBy (x=>x.Name ).ToList ();
           
            model.VacantPostionCategories.AddRange(new string []{"Vacant Postion", "Total"});

            model.HrVacantPostionData = new List<HrVacantPostionData>();

            decimal branchQuntity = 0;
            decimal assignedQuant = 0;
            //foreach (var b in listOfBranchs)
            //{
            //    decimal branchQuntity = 0;
            //    var mapping = _db.JobTitleMapping.Where(x => x.BranchId == b.BranchId).ToList();
            //    if (mapping.Any())
            //    {
            //         branchQuntity = mapping.Where(x => x.BranchId == b.BranchId).Sum(x => x.Quntity);
            //    }
            //    var placments= _db.Placements .Where (x=> x.isCurrent && x.BranchId == b.BranchId ).Count ();


               

            //}
            var mapping = _db.JobTitleMapping.ToList();
            if (mapping.Any())
            {
                branchQuntity = mapping.Sum(x => x.Quntity);
            }
            var placments = _db.Placements.Where(x => x.isCurrent).ToList();
            if (placments.Any())
             {
                 assignedQuant = placments.Count ();
            }
            model.HrVacantPostionData.Add(new HrVacantPostionData()
            {
                Name = "Vacant Postion",
                Data = new decimal[] { branchQuntity - assignedQuant, branchQuntity }
            });
            #endregion
            #region Termination
            var terminationTypes = _db .Lookups .Where (x=> x.LookupType ==   LookUpTypes.TERMINATIONTYPE ).ToList ();
            
            model.TerminationCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.HrTerminationData = new List<HrTerminationData>();
            foreach (var t in terminationTypes)
            {
                #region Curremt Month
                var terminations = _db.Terminations.Where (x=> x.LookupId == t.LookupId ).ToList();

                var terminationCurrent = terminations.Where(x => x.TerminationDate >= today && x.TerminationDate <= todayEnd).Count();



                #endregion
                #region second Month
                var terminationsecond = terminations.Where(x => x.TerminationDate >= prevMonth && x.TerminationDate <= prevMonthEnd).Count();

                #endregion
                #region third Month
                var terminationthird = terminations.Where(x => x.TerminationDate >= secondMonth && x.TerminationDate <= secondMonthEnd).Count();

                #endregion
                model.HrTerminationData.Add(new HrTerminationData()
                {
                    Name = t.Description,
                    Data = new decimal[] { terminationthird, terminationsecond, terminationCurrent }
                });
            }

            #endregion

            #region Accident Record
           string[] types= {"On Work Place","Not On Work Place"};

            model.AccidentRecordCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.HrAccidentRecordData = new List<HrAccidentRecordData>();
            foreach (var t in types)
            {
                var Accidents= new List<AccidentRecord>();

                if (t == "On Work Place")
                {
                      Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace ==true).ToList();
                }
                else
                {
                     Accidents = _db.AccidentRecords.Where(x => x.IsInWorkPlace ==false).ToList();
                }
                #region Curremt Month
               

                var accidentCurrent = Accidents.Where(x => x.Date >= today && x.Date <= todayEnd).Count();



                #endregion
                #region second Month
                var accidentsecond = Accidents.Where(x => x.Date >= prevMonth && x.Date <= prevMonthEnd).Count();

                #endregion
                #region third Month
                var accidentthird = Accidents.Where(x => x.Date >= secondMonth && x.Date <= secondMonthEnd).Count();

                #endregion
                model.HrAccidentRecordData.Add(new HrAccidentRecordData()
                {
                    Name = t,
                    Data = new decimal[] { accidentthird, accidentsecond, accidentCurrent }
                });
            }

            #endregion

            #region Vacancy
            string[] vacancytypes = { "INTERNAL", "EXTERNAL" };
            int vacancyCurrent = 0;
            int vacancysecond = 0;
            int vacancythird = 0;
            model.VacancyCategories.AddRange(new string[] { secondMonth.ToString("MMMM"), prevMonth.ToString("MMMM"), today.ToString("MMMM") });
            model.HrVacancyData = new List<HrVacancyData>();
            foreach (var t in vacancytypes)
            {
                var vacancies = new List<Vacancy>();
                var allVacancy=from v in _db.VaccancyPlans
                                join r in _db.RecruitmentVacancy
                                on v.VacancyId equals r.VacancyId
                               select new { v.From, v.To, v.RecruSource };
                
                #region Curremt Month

                if (t == "INTERNAL")
                {
                   // vacancies = allVacancy.Where(v => ).ToList();

                     vacancyCurrent = allVacancy.Where(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
                     vacancysecond = vacancies.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
                     vacancythird = vacancies.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.INTERNAL).Count();
                }
                else
                {
                     vacancyCurrent = allVacancy.Where(x => x.From >= today && x.To <= todayEnd && x.RecruSource == RecruitmentSource.EXTERNAL).Count();
                    vacancysecond = vacancies.Where(x => x.From >= prevMonth && x.To <= prevMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL ).Count();
                     vacancythird = vacancies.Where(x => x.From >= secondMonth && x.To <= secondMonthEnd && x.RecruSource == RecruitmentSource.EXTERNAL).Count();
                }
               



                #endregion
              
                
                model.HrVacancyData.Add(new HrVacancyData()
                {
                    Name = t,
                    Data = new decimal[] { vacancythird, vacancythird, vacancyCurrent }
                });
            }

            #endregion
            return View(model);
        }


    }
}