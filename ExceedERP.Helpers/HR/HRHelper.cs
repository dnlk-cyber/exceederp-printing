using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Helpers.HR
{
    public static class HrHelper
    {
       

        public static string ReportDateOneAm { get; set; }
        public static string ReportDateTwoAm { get; set; }
        public static DateTime ReportDateOne { get; set; }
        public static DateTime ReportDateTwo { get; set; }
        public static string GetMartialStatus(Person_Employee e)
        {
            switch (e.MartialStatus)
            {
                case MartialStatus.DIVORCED:
                    return "የተለያየ";
                case MartialStatus.MARRIED:
                    {
                        return "ያገባ";
                    }
                case MartialStatus.SEPARATED:
                    {
                        return "የተለያየ";
                    }
                default:
                    {
                        return "ያላገባ";
                    }
            }
        }
        public static string GetEmployeeStatus(EmployeeStatus status)
        {
            switch (status)
            {

                case EmployeeStatus.CONTRACT:
                    return "ጊዜያዊ";
                case EmployeeStatus.PERMANENT:
                    return "ቋሚ";
                case EmployeeStatus.PROBATION:
                    return "የሙከራ ጊዜ";
                case EmployeeStatus.NO_STATUS:
                    return "አይታወቅም";
                case EmployeeStatus.FREELANCER:
                    return "ፍሪላንሰር";
            }
            return "";


        }
        public static string GetResponsibilityLevel(ResponsibilityLevel level)
        {
            switch (level)
            {
                case ResponsibilityLevel.STAFF:
                    return "ባለሙያ";

                case ResponsibilityLevel.LOW_MANAGMENT:

                    return "ዝቅተኛ አመራር";
                case ResponsibilityLevel.MEDIUM_MANAGMENT:

                    return "መካከለኛ አመራር";

                case ResponsibilityLevel.TOP_MANAGMENT:

                    return "ከፍተኛ አመራር";


            }
            return "";
        }
        public static string GetAssignmentType(AssignmentType type)
        {
            switch (type)
            {
                case AssignmentType.NEW:
                    return "አዲስ";
                case AssignmentType.ASSIGNMENT:

                    return "በምደባ";

                case AssignmentType.DEMOTION:

                    return "በደረጃ ዝቅ በማድረግ";

                case AssignmentType.PROMOTION:

                    return "በዕድገት";
                case AssignmentType.RECOMENDATION:

                    return "በጥቆማ";
                case AssignmentType.STRUCTURE_CHANGE:

                    return "በመዋቅር ለውጥ";
                case AssignmentType.TRANSFER:

                    return "በዝውውር";
                case AssignmentType.YEARLY_PROMOTION:

                    return "በዓመታዊ ዕድገት";

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
                    return "የዓመት";
                case LeaveType.MARRIAGE:
                    return "የጋብቻ";
                case LeaveType.MATERNITY:
                    return "የእናትነት";
                case LeaveType.OTHER:
                    return "የህመም";
                case LeaveType.PATERNITY:
                    return "የአባትነት";
                case LeaveType.SICK_LEAVE:
                    return "ሌላ";

            }

            return "";
        }
        public static string GetGender(Gender gender)
        {
            if (gender == Gender.FEMALE)
            {
                return "ሴት";
            }

            return "ወንድ";


        }
        public static string GetVacancyType(VacancyType vacancyType)
        {
            if (vacancyType == VacancyType.PERMANENT)
            {
                return "ቋሚ";
            }

            return "ጊዜያዊ";


        }

        public static string GetReligion(ReligionList religion)
        {
            switch (religion)
            {

                case ReligionList.CATHOLIC:
                    return "ካቶሊክ";
                case ReligionList.MUSLIM:
                    return "ሙስሊም";
                case ReligionList.ORTHODOX:
                    return "ኦርቶዶክስ";
                case ReligionList.OTHER:
                    return "ሌላ";
                case ReligionList.PROTESTANT:
                    return "ፐሮቴስታንት";
                case ReligionList.WAAQEFFATAA:
                    return "ዋቄፈታ";

            }

            return "";


        }
        public static string GetRegionName(Region region)
        {
            switch (region)
            {

                case Region.AMHARA:
                    return "አማራ";
                case Region.OROMO:
                    return "ኦሮሞ";
                case Region.AFAR:
                    return "አፋር";
                case Region.BENSHANGUL_GUMUZ:
                    return "ቤንሻንጉል ጉምዝ";
                case Region.GAMBELLA:
                    return "ጋምቤላ";
                case Region.HARRARI:
                    return "ሃረሪ";
                case Region.SOMALIA:
                    return "ሶማሊያ";
                case Region.SOUTH_NATION_AND_NATIONALITIES:
                    return "ደ / ብ / ብ / ህ / ክ";
                case Region.TIGRAY:
                    return "ትግራይ";
            }

            return "";


        }

        public static string GetEmployeeFullName(int employeeId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var employee = db.Person_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if (employee != null)
            {
                return employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
            }

            return "";
        }
        public static double GetEmployeeAge(int employeeId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var employee = db.Person_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
            if (employee != null)
            {
                double age = DateTime.Now.Date.Subtract(new DateTime(employee.DateOfBirth.Year, employee.DateOfBirth.Month,
                       employee.DateOfBirth.Day)).TotalDays / 365.25;
                return age;
            }

            return 0;
        }

        public static string GetWorkUnitCostCenter(int orgId,int branchid)
        {
            ExceedDbContext db = new ExceedDbContext();
            string costCenter = "";
            var record = db.BranchCostCenter
                .Where(x=>x.BranchId==branchid)
                .FirstOrDefault(x => x.OrgStructureId == orgId);
            if (record != null) costCenter = record.Values;
            return costCenter;
        }
        public static string GetOrganizatioName(int orgId)
        {
            ExceedDbContext db = new ExceedDbContext();

            var record = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgId);
            if (record != null) return record.Name;
            return "";
        }
        public static string GetNationality(int id)
        {
            ExceedDbContext db = new ExceedDbContext();

            var record = db.Countries.FirstOrDefault(x => x.CountryId == id);
            if (record != null) return record.Nationality;
            return "";
        }
        public static string GetRankName(int id)
        {
            ExceedDbContext db = new ExceedDbContext();

            var record = db.Ranks.FirstOrDefault(x => x.RankId == id);
            if (record != null) return record.RankName;
            return "";
        }
        public static string GetLookupDescription(int titleId)
        {
            ExceedDbContext db = new ExceedDbContext();

            var record = db.Lookups.FirstOrDefault(x => x.LookupId == titleId);
            if (record != null) return record.Description;
            return "";
        }
        public static string GetEmployeeEducationField(int employeeId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var education = db.Education.Where(x => x.EmployeeId == employeeId && x.IsHighLevel).FirstOrDefault();
            if (education != null)
            {
                var record = db.Lookups.FirstOrDefault(x => x.LookupId == education.EduField);
                if (record != null) return record.Description;
            }

            return "";
        }
        public static string GetBranchSubAccount(int branchId)
        {
            ExceedDbContext db = new ExceedDbContext();
            string subAccount = "";
            var record = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
            if (record != null) subAccount = record.SubAccount;
            return subAccount;
        }

        public static string GetBranchLocationAccount(int branchId)
        {
            ExceedDbContext db = new ExceedDbContext();

            string branchLocationAccount = "";
            var record = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
            if (record != null) branchLocationAccount = record.Code;
            return branchLocationAccount;
        }

        public static List<Person_Employee> PersonEmployeesList(object[] branchId, string orgStructureId, string employeeType)
        {
            ExceedDbContext db = new ExceedDbContext();
            List<Person_Employee> emplist;

            List<string> allBranches = new List<string>();
            if (branchId != null && orgStructureId != null)
            {
                allBranches = branchId.Select(x => x.ToString()).ToList();
                emplist =
                    EmployeeHelper.GetEmployeesBySelectedBranchAndWorkUnit(allBranches, Convert.ToInt32(orgStructureId)).Where(x => x.IsActive).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                allBranches = branchId.Select(x => x.ToString()).ToList();
                emplist =
                    EmployeeHelper.GetEmployeesBySelectedBranch(allBranches)
                        .Where(x => x.IsActive)
                        .ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                emplist =
                    EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId))
                        .Where(x => x.IsActive)
                        .ToList();
            }
            else
            {
                emplist = db.Person_Employee.Where(x => x.IsActive).ToList();
            }
            return emplist;
        }

        public static List<Person_Employee> PersonEmployeesOneBranch(string branchId, string orgStructureId, string employeeType)
        {
            ExceedDbContext db = new ExceedDbContext();
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

        public static List<Person_Employee> AllPersonEmployeesOneBranch(string branchId, string orgStructureId, string employeeType)
        {
            ExceedDbContext db = new ExceedDbContext();
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

        public static decimal GetWorkingDays(DateTime from, DateTime to)
        {
            decimal days = 0;
            double weekDays = GetWeekDays(from, to);
            decimal count = GetHolidays(from, to);

            days = to.Subtract(from).Days + 1 - count - Convert.ToDecimal(weekDays);

            return days;
        }
        public static double GetWeekDays(DateTime startDate, DateTime endDate)
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

                        if (saturdayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (saturdayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
                        {
                            var holiday = db.Holidays.FirstOrDefault(x => x.Date == testDate);
                            if (holiday == null)
                            {
                                if (sundayweekendSetting.IsHalfDay)
                                {
                                    noOfWeek += 0.5;
                                }
                                else
                                {
                                    noOfWeek += 1;

                                }
                            }
                        }
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }
        public static int GetHolidays(DateTime from, DateTime to)
        {
            ExceedDbContext db = new ExceedDbContext();
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
    }
}
