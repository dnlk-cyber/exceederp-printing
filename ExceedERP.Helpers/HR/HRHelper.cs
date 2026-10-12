using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Localization;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Helpers.HR
{
    public static class HrHelper
    {
        public struct OTSetting
        {
            public decimal RestDayRate { get; set; }
            public decimal NormalRate { get; set; }
            public decimal NightRate { get; set; }
            public decimal HolidayRate { get; set; }
            public decimal OTCoefficient { get; set; }
        }

        public static OTSetting GetOTSetting()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var otrates = db.Configurations
                .Where(x => x.Reference == "OverTime")
                .ToList();

                return new OTSetting
                {
                    RestDayRate = GetValue(otrates, OverTimeAttribute.RESTDAY_RATE),
                    NormalRate = GetValue(otrates, OverTimeAttribute.NORMAL_RATE),
                    NightRate = GetValue(otrates, OverTimeAttribute.NIGHT_RATE),
                    HolidayRate = GetValue(otrates, OverTimeAttribute.HOLIDAY_RATE),
                    OTCoefficient = GetValue(otrates, OverTimeAttribute.COEFFICIENT)
                };
            }
        }

        private static decimal GetValue(List<Configuration> list, OverTimeAttribute attr)
        {
            return list
                .FirstOrDefault(x => x.Attribute == attr.ToString())
                ?.CurrentValue is string val
                ? Convert.ToDecimal(val)
                : 0;
        }
    
        public static decimal CalculateEmployeePension(decimal salary)
        {
            decimal pensionRate = 0.07m; // example 7%
            return salary * pensionRate;
        }
        public static decimal CalculateEmployerPension(decimal salary)
        {
            decimal pensionRate = 0.11m; // example 11%
            return salary * pensionRate;
        }
       
        public static int GetLeaveIncrementPerYear(ResponsibilityLevel responsibilityLevel)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                bool isLeader = responsibilityLevel != ResponsibilityLevel.STAFF;

                string attribute = isLeader
                    ? LeaveSetting.Management_Leave_Entitlement_Increase_Period.ToString()
                    : LeaveSetting.Staff_Leave_Entitlement_Increase_Period.ToString();

                var config = db.Configurations
                               .FirstOrDefault(x => x.Attribute == attribute)
                             ?? db.Configurations
                                  .FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString());

                if (config == null)
                    return 0;

                return int.Parse(config.CurrentValue);
            }

        }
        public static int GetAge(this DateTime birthDate)
        {
            var today = DateTime.Today;
            if (birthDate.Date > today)
                return 0;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
        public static decimal GetFieldDays(int empId, int glperiodId, DateTime? fromDate, DateTime? toDate)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal days = 0;
                var periodObj = db.GLPeriods.Find(glperiodId);
                DateTime from = fromDate ?? periodObj.DateFrom;
                DateTime to = toDate ?? periodObj.DateTo;

                // Step 1: get all approved travel orders for the employee
                var fieldDsys = db.ApTravelOrderDepartures
                    .Where(d => d.TravelOrder.IsOnlineApproved
                             && !d.TravelOrder.IsVoid
                             && d.TravelOrder.EmployeeId == empId)
                    .GroupBy(d => d.ApTravelOrderId)
                    .Select(g => new
                    {
                        StartDate = g.Where(d => d.Type == DepartureType.Departure)
                                     .OrderBy(d => d.Date)
                                     .Select(d => d.Date)
                                     .FirstOrDefault(),
                        EndDate = g.Where(d => d.Type == DepartureType.Return)
                                   .OrderByDescending(d => d.ArrivalDate)
                                   .Select(d => d.ArrivalDate)
                                   .FirstOrDefault()
                    })
                    .AsNoTracking()
                    .ToList();

                // Step 2: Adjust ranges to overlap with [from, to]
                var adjusted = fieldDsys
                    .Where(g => g.StartDate != DateTime.MinValue && g.EndDate != DateTime.MinValue) // skip incomplete trips
                    .Select(g => new
                    {
                        StartDate = g.StartDate < from ? from : g.StartDate,
                        EndDate = g.EndDate > to ? to : g.EndDate
                    })
                    .Where(g => g.StartDate <= g.EndDate) // must overlap
                    .ToList();

                // Step 3: Sum up overlapping days
                if (adjusted.Any())
                    days = adjusted.Sum(g => (g.EndDate - g.StartDate).Days + 1);

                return days;

            }
        }
        public static decimal GetFieldDays1(int empId, int glperiodId, DateTime? fromDate, DateTime? toDate)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal days = 0;
                var periodObj = db.GLPeriods.Find(glperiodId);
                DateTime from = fromDate ?? periodObj.DateFrom;
                DateTime to = toDate ?? periodObj.DateTo;
                var fieldDsys = db.ApTravelOrderDepartures
                .Where(d => d.TravelOrder.IsOnlineApproved
                        && !d.TravelOrder.IsVoid &&
                        d.TravelOrder.EmployeeId == empId &&
                       (d.Type == DepartureType.Departure || d.Type == DepartureType.Return) &&
                       d.ArrivalDate >= from &&
                       d.Date <= to)
                .GroupBy(d => d.ApTravelOrderId) // Group by travel order to process each travel separately
                .Select(g => new
                {
                    StartDate = g.Where(d => d.Type == DepartureType.Departure)
                                 .OrderBy(d => d.Date)
                                 .Select(d => d.Date)
                                 .FirstOrDefault(),
                    EndDate = g.Where(d => d.Type == DepartureType.Return)
                               .OrderByDescending(d => d.ArrivalDate)
                               .Select(d => d.ArrivalDate)
                               .FirstOrDefault()
                }).AsNoTracking()
                .ToList()
                .Select(g => new
                {
                    StartDate = (g.StartDate == DateTime.MinValue || g.StartDate < periodObj.DateFrom) ? periodObj.DateFrom : g.StartDate,
                    EndDate = (g.EndDate == DateTime.MinValue || g.EndDate > periodObj.DateTo) ? periodObj.DateTo : g.EndDate
                }).ToList();
                //.Select(g => new
                //{
                //    StartDate = g.StartDate < periodObj.DateFrom ? periodObj.DateFrom : g.StartDate,
                //    EndDate = g.EndDate > periodObj.DateTo ? periodObj.DateTo : g.EndDate
                //}).ToList();

                if (fieldDsys.Any())
                    return fieldDsys.Sum(g => (g.EndDate - g.StartDate).Days + 1);

                return days;
            }
        }
        public static decimal GetLeaveDays(int empId, int glperiodId, DateTime? fromDate, DateTime? toDate)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal days = 0;
                var period = db.GLPeriods.Find(glperiodId);
                DateTime from = fromDate ?? period.DateFrom;
                DateTime to = toDate ?? period.DateTo;
                var leaveDays = db.LeaveRequest
                .Where(l => l.EmployeeId == empId &&
                       l.To >= from &&
                       l.From <= to &&
                       l.IsOnlineApproved &&
                       !l.IsVoid)
                .Select(l => new
                {
                    StartDate = l.From < period.DateFrom ? period.DateFrom : l.From,
                    EndDate = l.To > period.DateTo ? period.DateTo : l.To
                })
                .ToList();

                if (leaveDays.Any())
                    return leaveDays.Sum(l => (l.EndDate - l.StartDate).Days + 1);
                return days;
            }
        }
        //public static string ReportDateOneAm { get; set; }
        //public static string ReportDateTwoAm { get; set; }
        //public static DateTime ReportDateOne { get; set; }
        //public static DateTime ReportDateTwo { get; set; }

        public static double GetHalf(DateTime startDate, DateTime endDate)
        {
            ExceedDbContext db = new ExceedDbContext();
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }

                        break;
                }
            }
            return noOfWeek;
        }
        public static double GetNotHalf(DateTime startDate, DateTime endDate)
        {
            ExceedDbContext db = new ExceedDbContext();
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && !saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }

                        }

                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && !sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                noOfWeek = noOfWeek + 1;
                            }
                        }


                        break;
                }
            }
            return noOfWeek;
        }
        public static bool IsHalf(DateTime startDate, DateTime endDate)
        {
            ExceedDbContext db = new ExceedDbContext();
            bool isHalf = false;
            var sundayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdayweekendSetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {

                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null && saturdayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                isHalf = true;
                            }

                        }

                        break;
                    case DayOfWeek.Sunday:


                        if (sundayweekendSetting != null && sundayweekendSetting.IsHalfDay)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                isHalf = true;
                            }
                        }


                        break;
                }
            }
            return isHalf;
        }
        public static decimal GetHolidays(DateTime from, DateTime to, int glFiscalYearId)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal count = 0;

            var holidayList = db.Holidays.Where(x => x.GlFiscalYearId == glFiscalYearId).ToList();
            if (holidayList.Any())
            {
                foreach (var h in holidayList)
                {
                    if (h.Date >= from && h.Date <= to)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        public static decimal GetLeaveDays(DateTime from, DateTime to, int glFiscalYearId, LeaveDay day, LeaveType leaveType, int empId)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal leaveDays = 0;
            //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            decimal holiday = GetHolidays(from, to, glFiscalYearId);
            var weekDays = GetWeekDays(from, to, empId);
            var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == leaveType && x.LeaveType != LeaveType.ANNUAL);
            if (leaveType == LeaveType.ANNUAL || (leaveTypeSetting != null && leaveTypeSetting.IsWorkingDays))
            {
                leaveDays = to.Subtract(from).Days + 1 - holiday;
                leaveDays = leaveDays - Convert.ToDecimal(weekDays);
                if (day == LeaveDay.Half_Day)
                {
                    decimal checkWeekEnd = to.Subtract(from).Days + 1 - holiday;
                    double amt = GetHalf(from, to);
                    double notHalf = GetNotHalf(from, to);
                    bool isHalf = IsHalf(from, to);
                    if (isHalf)
                    {
                        leaveDays = (checkWeekEnd - Convert.ToDecimal(notHalf)) / 2;
                    }
                    else
                    {
                        leaveDays = leaveDays / 2;
                    }
                }
            }
            else
            {
                leaveDays = to.Subtract(from).Days + 1;
                if (day == LeaveDay.Half_Day)
                {
                    leaveDays = leaveDays / 2;
                }
            }
            return leaveDays;
        }
        public static DateTime ParseEthiopicDate(string ethiopicDate)
        {
            var parts = ethiopicDate.Split('/');
            return XAPI.EthiopicDateTime.GetGregorianDate(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
        public static string GetECDate(DateTime gcDate)
        {
            return XAPI.EthiopicDateTime.GetEthiopicDate(gcDate.Day, gcDate.Month, gcDate.Year);
        }
        public static string GetMartialStatus(Person_Employee e)
        {
            switch (e.MartialStatus)
            {
                case MartialStatus.DIVORCED:
                    return Resources.Enum_MartialStatus_DIVORCED;
                case MartialStatus.MARRIED:
                    {
                        return Resources.Enum_MartialStatus_MARRIED;
                    }
                case MartialStatus.SEPARATED:
                    {
                        return Resources.Enum_MartialStatus_SEPARATED;
                    }
                default:
                    {
                        return Resources.Enum_MartialStatus_SINGLE;
                    }
            }
        }
        public static string GetEmployeeStatus(EmployeeStatus status)
        {
            switch (status)
            {

                case EmployeeStatus.CONTRACT:
                    return Resources.Enum_EmployeeStatus_CONTRACT.ToLower();
                case EmployeeStatus.PERMANENT:
                    return Resources.Enum_EmployeeStatus_PERMANENT.ToLower();
                case EmployeeStatus.PROBATION:
                    return Resources.Enum_EmployeeStatus_PROBATION.ToLower();
                case EmployeeStatus.NO_STATUS:
                    return Resources.Enum_EmployeeStatus_NO_STATUS.ToLower().Replace("_", " ");
                case EmployeeStatus.FREELANCER:
                    return Resources.Enum_EmployeeStatus_FREELANCER.ToLower();
            }
            return "";


        }
        public static string GetResponsibilityLevel(ResponsibilityLevel level)
        {
            switch (level)
            {
                case ResponsibilityLevel.STAFF:
                    return Resources.Enum_ResponsibilityLevel_STAFF;

                case ResponsibilityLevel.LOW_MANAGMENT:

                    return Resources.Enum_ResponsibilityLevel_LOW_MANAGMENT;
                case ResponsibilityLevel.MEDIUM_MANAGMENT:

                    return Resources.Enum_ResponsibilityLevel_MEDIUM_MANAGMENT;

                case ResponsibilityLevel.TOP_MANAGMENT:

                    return Resources.Enum_ResponsibilityLevel_TOP_MANAGMENT;
            }
            return "";
        }
        public static string GetAssignmentType(AssignmentType type)
        {
            switch (type)
            {
                case AssignmentType.NEW:
                    return Resources.Enum_AssignmentType_NEW;
                case AssignmentType.ASSIGNMENT:

                    return Resources.Enum_AssignmentType_ASSIGNMENT;

                case AssignmentType.DEMOTION:

                    return Resources.Enum_AssignmentType_DEMOTION;

                case AssignmentType.PROMOTION:

                    return Resources.Enum_AssignmentType_PROMOTION;
                case AssignmentType.RECOMENDATION:

                    return Resources.Enum_AssignmentType_RECOMENDATION;
                case AssignmentType.STRUCTURE_CHANGE:

                    return Resources.Enum_AssignmentType_STRUCTURE_CHANGE;
                case AssignmentType.TRANSFER:

                    return Resources.Enum_AssignmentType_TRANSFER;
                case AssignmentType.YEARLY_PROMOTION:

                    return Resources.Enum_AssignmentType_YEARLY_PROMOTION;

            }
            return "";
        }
        //public static string GetIndustryType(IndustryType industryType)
        //{
        //    switch (industryType)
        //    {

        //        case IndustryType.GOVERNMENTAL:
        //            return "መንግስታዊ አገልግሎት";
        //        case IndustryType.CIVIL:
        //            return "የሲቪል አገልግሎት";
        //        case IndustryType.MILITARY:
        //            return "ወታደራዊ አገልግሎት";

        //    }

        //    return "";
        //}
        public static string GetLeaveType(LeaveType leaveType)
        {
            switch (leaveType)
            {

                case LeaveType.ANNUAL:
                    return Resources.Enum_LeaveType_ANNUAL;
                case LeaveType.MARRIAGE:
                    return Resources.Enum_LeaveType_MARRIAGE;
                case LeaveType.MATERNITY:
                    return Resources.Enum_LeaveType_MATERNITY;
                case LeaveType.OTHER:
                    return Resources.Enum_LeaveType_OTHER;
                case LeaveType.PATERNITY:
                    return Resources.Enum_LeaveType_PATERNITY;
                case LeaveType.SICK_LEAVE:
                    return Resources.Enum_LeaveType_SICK_LEAVE;

            }

            return "";
        }
        public static string GetGender(Gender gender)
        {

            if (gender == Gender.FEMALE)
            {
                return "Female";
            }

            return "Male";


        }
        public static string GetLocalizedGender(Gender gender)
        {
            string cultureName = GlobalHelper.GetCurrentCulture();

            if (!string.IsNullOrEmpty(cultureName))
            {


                if ((cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et"))
                {
                    if (gender == Gender.FEMALE)
                    {
                        return "ሴት";
                    }

                    return "ወንድ";
                }
                if (cultureName.ToLower() == "om")
                {
                    if (gender == Gender.FEMALE)
                    {
                        return "Dhalaa";
                    }

                    return "Dhiira";
                }
                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                {
                    if (gender == Gender.FEMALE)
                    {
                        return "Female";
                    }

                    return "Male";
                }
            }
            if (gender == Gender.FEMALE)
            {
                return "Female";
            }

            return "Male";


        }
        public static string GetVacancyType(VacancyType vacancyType)
        {
            if (vacancyType == VacancyType.PERMANENT)
            {
                return Resources.Enum_VacancyType_PERMANENT;
            }

            return Resources.Enum_VacancyType_CONTRACT;


        }
        public static string GetReligion(ReligionList religion)
        {
            switch (religion)
            {

                case ReligionList.CATHOLIC:
                    return Resources.Enum_ReligionList_CATHOLIC;
                case ReligionList.MUSLIM:
                    return Resources.Enum_ReligionList_MUSLIM;
                case ReligionList.ORTHODOX:
                    return Resources.Enum_ReligionList_ORTHODOX;
                case ReligionList.OTHER:
                    return Resources.Enum_ReligionList_OTHER;
                case ReligionList.PROTESTANT:
                    return Resources.Enum_ReligionList_PROTESTANT;
                case ReligionList.WAAQEFFATAA:
                    return Resources.Enum_ReligionList_WAAQEFFATAA;

            }

            return "";


        }
        public static string GetRegionName(Region region)
        {
            switch (region)
            {

                case Region.AMHARA:
                    return Resources.Enum_Region_AMHARA;
                case Region.OROMIA:
                    return Resources.Enum_Region_OROMO;
                case Region.AFAR:
                    return Resources.Enum_Region_AFAR;
                case Region.BENSHANGUL_GUMUZ:
                    return Resources.Enum_Region_BENSHANGUL_GUMUZ;
                case Region.GAMBELLA:
                    return Resources.Enum_Region_GAMBELLA;
                case Region.HARRARI:
                    return Resources.Enum_Region_HARRARI;
                case Region.SOMALIA:
                    return Resources.Enum_Region_SOMALIA;
                case Region.SOUTH_NATION_AND_NATIONALITIES:
                    return Resources.Enum_Region_SOUTH_NATION_AND_NATIONALITIES;
                case Region.TIGRAY:
                    return Resources.Enum_Region_TIGRAY;
            }

            return "";


        }
        public static string GetEmployeeFullName(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var employee = db.Person_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                if (employee != null)
                {
                    return employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                }

                return "";
            }
        }
        public static double GetEmployeeAge(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var employee = db.Person_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                if (employee != null)
                {
                    double age = DateTime.Now.Date.Subtract(new DateTime(employee.DateOfBirth.Year, employee.DateOfBirth.Month,
                           employee.DateOfBirth.Day)).TotalDays / 365.25;
                    age = Math.Round(age, 2);
                    return age;
                }

                return 0;
            }
        }
        public static double GetEmployeeExactAge(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);

                if (employee != null)
                {
                    DateTime currentDate = DateTime.Now;
                    DateTime birthDate = employee.DateOfBirth;

                    int years = currentDate.Year - birthDate.Year;
                    int months = currentDate.Month - birthDate.Month;
                    int days = currentDate.Day - birthDate.Day;

                    // Adjust for negative values
                    if (days < 0)
                    {
                        // Borrow from the month
                        months--;
                        int daysInLastMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month - 1);
                        days += daysInLastMonth;
                    }

                    // Adjust for negative or zero months
                    if (months < 0)
                    {
                        // Borrow from the year
                        years--;
                        months += 12;
                    }

                    double age = years + (months / 12.0) + (days / 365.25);
                    return age;
                }

                return 0;
            }
        }
        public static string GetWorkUnitCostCenter(int orgId, int branchid)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                string costCenter = "";
                var record = db.BranchCostCenter
                    .FirstOrDefault(x => x.OrgStructureId == orgId);
                if (record != null) costCenter = record.Values;
                return costCenter;
            }
        }
        public static string GetOrganizatioName(int orgId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                var record = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgId);
                if (record != null) return record.Name;
                return "";
            }
        }
        public static string GetNationality(int id)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                var record = db.Countries.FirstOrDefault(x => x.CountryId == id);
                if (record != null) return record.Nationality;
                return "";
            }
        }
        public static string GetRankName(int id)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                var record = db.Ranks.FirstOrDefault(x => x.RankId == id);
                if (record != null) return record.RankName;
                return "";
            }
        }
        public static string GetLookupDescription(int titleId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                var record = db.Lookups.FirstOrDefault(x => x.LookupId == titleId);
                if (record != null) return record.Description;
                return "";
            }
        }
        public static string GetEmployeeEducationField(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var education = db.Education.Where(x => x.EmployeeId == employeeId && x.IsHighLevel).FirstOrDefault();
                if (education != null)
                {
                    var record = db.Lookups.FirstOrDefault(x => x.LookupId == education.EduField);
                    if (record != null) return record.Description;
                }

                return "";
            }
        }

        //public static List<Person_Employee> PersonEmployeesList(object[] branchId, string orgStructureId, string employeeType)
        //{
        //    using (ExceedDbContext db = new ExceedDbContext())
        //    {
        //        List<Person_Employee> emplist;

        //        List<string> allBranches = new List<string>();
        //        if (branchId != null && orgStructureId != null)
        //        {
        //            allBranches = branchId.Select(x => x.ToString()).ToList();
        //            emplist =
        //                EmployeeHelper.GetEmployeesBySelectedBranchAndWorkUnit(allBranches, Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
        //        }
        //        else if (branchId != null && orgStructureId == null)
        //        {
        //            allBranches = branchId.Select(x => x.ToString()).ToList();
        //            emplist =
        //                EmployeeHelper.GetEmployeesBySelectedBranch(allBranches)
        //                    .Where(x => x.IsActive)
        //                    .ToList();
        //        }
        //        else if (branchId == null && orgStructureId != null)
        //        {
        //            emplist =
        //                EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId))
        //                    .Where(x => x.IsActive)
        //                    .ToList();
        //        }
        //        else
        //        {
        //            emplist = db.Person_Employee.Where(x => x.IsActive).ToList();
        //        }
        //        return emplist;
        //    }
        //}
        public static List<Person_Employee> PersonEmployeesOneBranch(string branchId, string orgStructureId, string employeeType)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                List<Person_Employee> emplist;

                if (branchId != null && orgStructureId != null)
                {
                    emplist =
                        EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId))
                            .Where(x => x.IsActive)
                            .ToList();
                }
                else if (branchId != null && orgStructureId == null)
                {
                    emplist = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).Where(x => x.IsActive).ToList();
                }
                else if (branchId == null && orgStructureId != null)
                {
                    emplist = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
                }
                else
                {
                    emplist = db.Person_Employee.Where(x => x.IsActive).ToList();
                }
                return emplist;
            }
        }
        public static List<Person_Employee> AllPersonEmployeesOneBranch(string branchId, string orgStructureId, string employeeType)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                List<Person_Employee> emplist;

                if (branchId != null && orgStructureId != null)
                {
                    emplist =
                        EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
                }
                else if (branchId != null && orgStructureId == null)
                {
                    emplist = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
                }
                else if (branchId == null && orgStructureId != null)
                {
                    emplist = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
                }
                else
                {
                    emplist = db.Person_Employee.ToList();
                }
                return emplist;
            }
        }
        public static decimal GetWorkingDays(DateTime from, DateTime to)
        {
            decimal days = 0;
            double weekDays = GetWeekDays(from, to,0);
            decimal count = GetHolidays(from, to);

            days = to.Subtract(from).Days + 1 - count - Convert.ToDecimal(weekDays);

            return days;
        }
        public static double GetWeekDays(DateTime startDate, DateTime endDate, int empId)
        {
            using (var db = new ExceedDbContext())
            {
                double noOfWeek = 0;

                var placement = db.Placements
                    .FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);

                var saturdayWeekend = db.Weekends
                    .FirstOrDefault(x => x.Name == WeekendName.SATURDAY);

                var sundayWeekend = db.Weekends
                    .FirstOrDefault(x => x.Name == WeekendName.SUNDAY);

                // Load holidays once
                HashSet<DateTime> holidays = new HashSet<DateTime>(
                    db.Holidays
                      .Where(x => x.Date >= startDate && x.Date <= endDate)
                      .Select(x => x.Date));

                // Load all weekend exceptions
                List<WeekendException> weekendExceptions = db.WeekendExceptions
                    .Where(x => x.EmployeeId == empId)
                    .ToList();

                if (!weekendExceptions.Any() && placement != null)
                {
                    weekendExceptions = db.WeekendExceptions
                        .Where(x => x.OrgStructureId == placement.OrgStructureId)
                        .ToList();
                }

                if (!weekendExceptions.Any() && placement != null)
                {
                    weekendExceptions = db.WeekendExceptions
                        .Where(x => x.BranchId == placement.BranchId)
                        .ToList();
                }

                int totalDays = (endDate.Date - startDate.Date).Days;

                for (int i = 0; i <= totalDays; i++)
                {
                    var currentDate = startDate.Date.AddDays(i);

                    if (holidays.Contains(currentDate))
                        continue;

                    var weekendSetting = currentDate.DayOfWeek == DayOfWeek.Saturday
                        ? saturdayWeekend
                        : currentDate.DayOfWeek == DayOfWeek.Sunday
                            ? sundayWeekend
                            : null;

                    if (weekendSetting == null)
                        continue;

                    // Find the exception for THIS weekend only
                    var weekendException = weekendExceptions
                        .FirstOrDefault(x => x.WeekendId == weekendSetting.WeekendId);

                    if (weekendException != null)
                    {
                        noOfWeek += weekendException.IsHalfDay
                            ? 0.5
                            : weekendException.IsFullDay
                                ? 0
                                : 1;
                    }
                    else
                    {
                        noOfWeek += weekendSetting.IsHalfDay
                            ? 0.5
                            : weekendSetting.IsFullDay
                                ? 0
                                : 1;
                    }
                }

                return noOfWeek;
            }
        }
        public static int GetHolidays(DateTime from, DateTime to)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                int count = 0;
                var holidayList = db.Holidays.ToList();
                if (holidayList.Any())
                {
                    foreach (var h in holidayList)
                    {
                        if (h.Date >= from && h.Date <= to)
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
        }

        public static string GetAmharicMonthOfGergorian(int month)
        {
            var mon = "";
            switch (month)
            {
                case 1:
                    mon = "????";
                    break;
                case 2:
                    mon = "?????";
                    break;
                case 3:
                    mon = "???";
                    break;
                case 4:
                    mon = "????";
                    break;
                case 5:
                    mon = "??";
                    break;
                case 6:
                    mon = "??";
                    break;
                case 7:
                    mon = "???";
                    break;
                case 8:
                    mon = "????";
                    break;
                case 9:
                    mon = "??????";
                    break;
                case 10:
                    mon = "?????";
                    break;
                case 11:
                    mon = "?????";
                    break;
                case 12:
                    mon = "?????";
                    break;
            }
            return mon;
        }

        public static string CapitalizeEacgWordFirstLetter(string word)
        {
            string capitalizedWord = "";
            var spliteWords = word.ToLower().Split(' ');

            for (int i = 0; i < spliteWords.Count(); i++)
            {
                var wordArray = spliteWords[i].ToCharArray();
                var firstLetter = wordArray[0];
                var capital = firstLetter.ToString().ToUpper();
                for (int j = 1; j < wordArray.Count(); j++)
                {
                    capital = capital + wordArray[j];
                }

                capitalizedWord = capitalizedWord + " " + capital;
            }
            return capitalizedWord;
        }

        /// New

        public static string GenerateVacancyCode(Vacancy vaccancyPlan)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                string id = "";
                int nextID;

                var alllist = db.VaccancyPlans.ToList();

                var ExternalOrInternal = "";

                if (vaccancyPlan.RecruSource == RecruitmentSource.EXTERNAL)
                {
                    ExternalOrInternal = "Ext";
                }
                else if (vaccancyPlan.RecruSource == RecruitmentSource.INTERNAL)
                {
                    ExternalOrInternal = "Int";
                }

                if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
                {
                    if (alllist.Any())
                    {
                        var cont = alllist.Where(x => x.VacancyType == VacancyType.CONTRACT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                        if (cont != null)
                        {
                            if (cont.VaccancyCode.Contains('-'))
                            {
                                nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[2]) + 1;

                                id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                            }
                            else
                            {
                                // id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                            }
                        }
                        else
                        {
                            id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                        }
                    }
                    else
                    {
                        id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                    }

                }
                else
                {
                    if (alllist.Any())
                    {
                        var cont = alllist.Where(x => x.VacancyType == VacancyType.PERMANENT).OrderByDescending(x => x.VacancyId).FirstOrDefault();

                        if (cont != null)
                        {
                            if (cont.VaccancyCode.Contains('-'))
                            {
                                nextID = Convert.ToInt32(cont.VaccancyCode.Split('-')[2]) + 1;
                                id = ExternalOrInternal + "-PE-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                            }
                            else
                            {
                                // id = ExternalOrInternal + "-CO-" + nextID.ToString() + "-" + DateTime.Now.Year.ToString();
                            }

                        }
                        else
                        {
                            id = ExternalOrInternal + "-PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                        }
                    }
                    else
                    {
                        if (vaccancyPlan.VacancyType == VacancyType.CONTRACT)
                        {
                            id = ExternalOrInternal + "-CO-" + "1" + "-" + DateTime.Now.Year.ToString();
                        }
                        else
                        {
                            id = ExternalOrInternal + "-PE-" + "1" + "-" + DateTime.Now.Year.ToString();
                        }
                    }
                }

                return id;
            }
        }

        public static decimal GetSalaryFromSalarySetting(int stepId, int rankId, int lookUpId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                decimal salaryAmount = 0;
                var salarySetting = db.SalarySettings.FirstOrDefault();
                if (salarySetting != null && salarySetting.IsJobtitleIncludedInSalary)
                {
                    var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rankId && s.StepId == stepId && s.LookupId == lookUpId);

                    if (salary != null)
                    {
                        salaryAmount = salary.Salary;
                    }
                }
                else
                {
                    var salary = db.SalariesJobTitles.FirstOrDefault(s => s.RankId == rankId && s.StepId == stepId);

                    if (salary != null)
                    {
                        salaryAmount = salary.Salary;
                    }
                }

                return salaryAmount;
            }
        }

        public static string GetHireDate(int empId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var person = db.Person_Employee.Find(empId);
                if (person != null)
                {
                    return person.HiredDateAmharic;
                }

                return "";
            }
        }

        public static string GetBranchLocationAccount(int branchId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                string branchLocationAccount = "";
                var record = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                if (record != null) branchLocationAccount = record.LocationAccount;
                return branchLocationAccount;
            }
        }
        public static string GetBranchSubAccount(int branchId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                string branchSubAccount = "";
                var record = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                if (record != null) branchSubAccount = record.SubAccount;
                return branchSubAccount;
            }
        }
        public static string GetBusinessUnitSectorAccount(int businessUnitId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                string businessUnitAccount = "";
                var record = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == businessUnitId);
                if (record != null) businessUnitAccount = record.Accountcode;
                return businessUnitAccount;
            }
        }

        public static string GetWorkUnitCostCenter(int orgId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                string costCenter = "";
                var record = db.BranchCostCenter
                    // .Where(x => x.BranchId == branchid)
                    .FirstOrDefault(x => x.OrgStructureId == orgId);
                if (record != null) costCenter = record.Values;
                return costCenter;
            }
        }
        public static List<int> GetEmployeesWithExpiringContracts(List<ContactVM> employees)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                if (employees == null || !employees.Any())
                    return new List<int>();

                var employeeIds = employees.Select(e => e.EmployeeId).ToList();
                var today = DateTime.Now.Date;

                var contracts = db.Contract
                    .Where(c => employeeIds.Contains(c.EmployeeId)
                                && c.To >= today)
                    .GroupBy(c => c.EmployeeId)
                    .Select(g => g
                        .OrderByDescending(c => c.To)
                        .FirstOrDefault())
                    .ToList();

                var result = contracts
                    .Where(c => (c.To - today).TotalDays <= 30)
                    .Select(c => c.EmployeeId)
                    .ToList();

                return result;
            }
        }
        public static List<int> GetEmployeeInProbaition(List<Person_Employee> employees)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var today = DateTime.Now.Date;
                var employeeProbationSetting = db.EmployeeProbationSettings.FirstOrDefault();
                var probationSetting = db.ProbationsSettings.FirstOrDefault();
                int probationDays = probationSetting?.ProbationDays ?? 60;
                List<DateTime> holidays = db.Holidays.Select(h => h.Date).ToList();
                var employeeIds = new List<int>();
                foreach (var employee in employees)
                {
                    var hireDate = employee.HiredDate.Date;
                    var daysElapsed = (today - hireDate).Days;
                    // If more than the configured probation days have passed, skip
                    if (daysElapsed > probationDays)
                        continue;
                    int countedDays = 0;
                    var currentDate = hireDate;
                    while (countedDays < probationDays && currentDate < today)
                    {
                        currentDate = currentDate.AddDays(1);
                        bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday
                            || currentDate.DayOfWeek == DayOfWeek.Sunday;
                        bool isHoliday = holidays.Contains(currentDate);
                        if (employeeProbationSetting != null)
                        {
                            if (!employeeProbationSetting.IncludeWeekend && isWeekend)
                                continue;
                            if (!employeeProbationSetting.IncludeHoliday && isHoliday)
                                continue;
                        }
                        countedDays++;
                    }
                    // If still within probation period, include the employee
                    if (countedDays < probationDays)
                    {
                        employeeIds.Add(employee.EmployeeId);
                    }
                }
                return employeeIds;
            }
        }
        public static int GetRemainingContractDays(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var today = DateTime.Now.Date;

                var contract = db.Contract
                    .Where(c => c.EmployeeId == employeeId && c.To >= today)
                    .OrderByDescending(c => c.To)
                    .FirstOrDefault();

                if (contract == null)
                    return 0;

                return (int)(contract.To.Date - today).TotalDays;
            }
        }
        public static int CheckProhibition(int empId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                // Load employee once
                var employee = db.Person_Employee
                             .Where(x => x.EmployeeId == empId)
                             .Select(x => new { x.HiredDate, x.Status })
                             .FirstOrDefault();

                if (employee == null || employee.Status != EmployeeStatus.PROBATION)
                    return 1;

                // Load prohibition once
                var prohibition = db.Prohibitions
                                    .Where(x => x.EmployeeId == empId)
                                    .Select(x => new { x.EndDate })
                                    .FirstOrDefault();

                if (prohibition == null)
                    return 1;

                // Load holidays once (HashSet for O(1) lookup)
                HashSet<DateTime> holidays = new HashSet<DateTime>(
                    db.Holidays.Select(x => x.Date)
                );

                // Calculate working days efficiently
                int workingDays = 0;
                DateTime current = employee.HiredDate.Date;
                DateTime end = prohibition.EndDate.Date;

                while (current <= end && workingDays < 60)
                {
                    // Skip weekends & holidays
                    if (current.DayOfWeek != DayOfWeek.Saturday &&
                        current.DayOfWeek != DayOfWeek.Sunday &&
                        !holidays.Contains(current))
                    {
                        workingDays++;
                    }

                    // Early exit if 60 working days reached
                    if (workingDays >= 60)
                        break;

                    current = current.AddDays(1);
                }

                return workingDays < 60 ? 2 : 0;
            }
        }
        public static bool IsProbationCompleted(int employeeId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                // Load employee and placement once
                var employee = db.Person_Employee
                             .Where(x => x.EmployeeId == employeeId && x.IsActive)
                             .Select(x => new { x.HiredDate })
                             .FirstOrDefault();

                if (employee == null)
                    return false;

                var placement = db.Placements
                                  .Where(x => x.EmployeeId == employeeId && x.isCurrent)
                                  .Select(x => new { x.ResponsibilityLevel })
                                  .FirstOrDefault();

                // Determine probation days
                var probationSetting = db.ProbationsSettings.FirstOrDefault();
                int probationDays = probationSetting?.ProbationDays ?? 60; // default
                if (placement != null)
                {
                    probationSetting = db.ProbationsSettings
                                            .FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);

                    if (probationSetting != null && probationSetting.ProbationDays > 0)
                        probationDays = probationSetting.ProbationDays;
                }

                // Load holidays once (HashSet for O(1) lookup)
                var holidays = new HashSet<DateTime>(db.Holidays.Select(x => x.Date));

                // Count working days
                DateTime current = employee.HiredDate.Date;
                DateTime today = DateTime.Today;
                int workingDays = 0;

                while (current <= today && workingDays < probationDays)
                {
                    if (current.DayOfWeek != DayOfWeek.Saturday &&
                        current.DayOfWeek != DayOfWeek.Sunday &&
                        !holidays.Contains(current))
                    {
                        workingDays++;
                    }

                    current = current.AddDays(1);
                }

                return workingDays >= probationDays;
            }
        }
        public static bool GetDatePreference()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                return db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            }
        }
        public static decimal CalculateNetSalary(decimal baseSalary)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                if (baseSalary <= 0)
                    return 0;

                // Employee Pension (7%)
                decimal pension = CalculateEmployeePension(baseSalary);
                // Taxable Income
                //decimal taxableIncome = baseSalary - pension;

                // PAYE Tax
                //decimal tax = (decimal)XAPI.IncomeTax.GetIncomeTax( (float)taxableIncome);
                decimal tax = (decimal)XAPI.IncomeTax.GetIncomeTax( (float)baseSalary);

                // Net Salary
                return Math.Round(baseSalary - pension - tax, 2);
            }
      
            
        }
    }
}
