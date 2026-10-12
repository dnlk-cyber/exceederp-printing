using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    // Edit rules shared by the leave approval pages; mirrors LeaveRequestPersonal_Update.
    public sealed class LeaveRequestEditService
    {
        private readonly ExceedDbContext db;

        public LeaveRequestEditService(ExceedDbContext db)
        {
            this.db = db;
        }

        // Returns the saved entity, or null when a rule failed (errors are added to modelState).
        public LeaveRequest Update(LeaveRequest leaveRequest, string userName, ModelStateDictionary modelState)
        {
            var entity = db.LeaveRequest.Find(leaveRequest.LeaveRequestId);
            if (entity == null)
                return Fail(modelState, "Leave request not found.");

            if (entity.IsOnlineApproved || entity.IsDepartmentOnlineApproved)
                return Fail(modelState, "This leave request is already approved and can no longer be edited.");

            // The editor is the approver, so the employee comes from the request itself.
            leaveRequest.EmployeeId = leaveRequest.EmployeeId != 0 ? leaveRequest.EmployeeId : entity.EmployeeId;
            if (leaveRequest.EmployeeId == 0)
                return Fail(modelState, "Please select an employee first!! Or refresh the page to clear the cache");

            var totalBalance = db.LeavesEntitle
                .Where(x => !x.IsExpired && x.EmployeeId == leaveRequest.EmployeeId)
                .Select(x => x.Balance)
                .DefaultIfEmpty(0)
                .Sum(x => x);

            if (!ApplyDates(leaveRequest, modelState))
                return null;

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= leaveRequest.From && x.DateTo >= leaveRequest.From);
            if (fiscalYear == null)
                return Fail(modelState, "No fiscal year is defined for the selected leave start date. Please define the fiscal period or select a valid date.");
            leaveRequest.GlFiscalYearId = fiscalYear.GlFiscalYearId;

            if (!ApplyLeaveDays(leaveRequest, entity, fiscalYear.GlFiscalYearId, modelState))
                return null;

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.IsActive);
            if (!ValidatePolicies(leaveRequest, entity, employee, totalBalance, modelState))
                return null;

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveRequest.EmployeeId && x.isCurrent);
            if (placement == null)
                return Fail(modelState, "Employee does not have a current placement", "placement");

            leaveRequest.BranchId = placement.BranchId;
            leaveRequest.BusinessUnitId = placement.BusinessUnitId;
            leaveRequest.LookupId = placement.LookupId;
            leaveRequest.LeaveActionTypes = leaveRequest.Type != LeaveType.ANNUAL ? LeaveActionTypes.Without_Plan : leaveRequest.LeaveActionTypes;

            Copy(leaveRequest, entity, employee, totalBalance, userName);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            return entity;
        }

        // Returns "balance-exactDeserve-leaveDays" for the popup.
        public string CalculateAmounts(int empId, DateTime fromDate, DateTime toDate, MorningOrAfternoon morningOrAfternoon)
        {
            if (empId == 0)
                return "0-0-0";

            var balanceQuery = db.LeavesEntitle.Where(x => x.EmployeeId == empId && !x.IsExpired);
            decimal totalBalance = balanceQuery.Any() ? balanceQuery.Sum(x => x.Balance) : 0;
            decimal exactDeserve = 0;
            decimal leaveDays = 0;

            var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= fromDate && x.DateTo >= fromDate);
            if (fiscalYear != null)
            {
                var leaveTypeSetting = db.LeaveTypeSettings.FirstOrDefault(x => x.LeaveType == LeaveType.ANNUAL);
                var entitleCurrentYear = balanceQuery.FirstOrDefault(x => x.GlFiscalYearId == fiscalYear.GlFiscalYearId);
                if (entitleCurrentYear != null && leaveTypeSetting != null && !leaveTypeSetting.AllowFullAnnualLeave)
                {
                    exactDeserve = GetEmployeeDeserveLeaveDays(fiscalYear.DateFrom, toDate, entitleCurrentYear.Amount, entitleCurrentYear.Balance, totalBalance);
                }

                if (exactDeserve == 0 && totalBalance > 0)
                    exactDeserve = totalBalance;
                if (exactDeserve > totalBalance)
                    exactDeserve = totalBalance;

                leaveDays = HrHelper.GetLeaveDays(fromDate, toDate, fiscalYear.GlFiscalYearId, LeaveDay.Full_Day, LeaveType.ANNUAL, empId);

                decimal afternoonOffDays = morningOrAfternoon == MorningOrAfternoon.Afternoon ? GetHalfDayWeekendDays(fromDate, toDate) : 0;
                if (afternoonOffDays > 0 && leaveDays > 0)
                    leaveDays -= afternoonOffDays;
            }

            return $"{totalBalance}-{exactDeserve}-{leaveDays}";
        }

        public static decimal GetEmployeeDeserveLeaveDays(DateTime from, DateTime to, decimal amount, decimal balance, decimal totalBalance)
        {
            var workedDays = (to.Date - from.Date).Days;
            var deserveAmountCurrentYear = (amount * workedDays) / 365;
            var previousEntitle = totalBalance - balance;
            var exactDeserve = previousEntitle + deserveAmountCurrentYear;

            var fraction = exactDeserve % 1;
            decimal rounding = fraction >= 0.7m ? 1m : fraction <= 0.3m ? 0m : 0.5m;

            return (int)exactDeserve + rounding;
        }

        // Half-day weekends (non-holiday) in the range; an afternoon start does not consume them.
        private decimal GetHalfDayWeekendDays(DateTime startDate, DateTime endDate)
        {
            decimal half = 0;
            var sundaySetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SUNDAY);
            var saturdaySetting = db.Weekends.FirstOrDefault(x => x.Name == WeekendName.SATURDAY);
            int days = (endDate - startDate).Days;

            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                var setting = testDate.DayOfWeek == DayOfWeek.Saturday ? saturdaySetting
                            : testDate.DayOfWeek == DayOfWeek.Sunday ? sundaySetting
                            : null;

                if (setting != null && setting.IsHalfDay && !db.Holidays.Any(x => x.Date == testDate))
                    half += 0.5m;
            }
            return half;
        }

        private bool ApplyDates(LeaveRequest leaveRequest, ModelStateDictionary modelState)
        {
            bool useEthiopian = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? false;

            if (useEthiopian)
            {
                if (string.IsNullOrWhiteSpace(leaveRequest.FromAmharic) || string.IsNullOrWhiteSpace(leaveRequest.ToAmharic))
                    return Invalid(modelState, "Both From and To Date fields are mandatory.");

                leaveRequest.From = HrHelper.ParseEthiopicDate(leaveRequest.FromAmharic);
                leaveRequest.To = HrHelper.ParseEthiopicDate(leaveRequest.ToAmharic);
            }
            else
            {
                if (leaveRequest.From == default(DateTime) || leaveRequest.To == default(DateTime))
                    return Invalid(modelState, "Both From and To Date fields are mandatory.");

                leaveRequest.FromAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.From.Day, leaveRequest.From.Month, leaveRequest.From.Year);
                leaveRequest.ToAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(leaveRequest.To.Day, leaveRequest.To.Month, leaveRequest.To.Year);
            }

            if (leaveRequest.From.Date > leaveRequest.To.Date)
                return Invalid(modelState, "From Date field should be less or equal to To date field!!");

            return true;
        }

        private bool ApplyLeaveDays(LeaveRequest leaveRequest, LeaveRequest entity, int fiscalYearId, ModelStateDictionary modelState)
        {
            if (leaveRequest.Day == LeaveDay.Mixed)
            {
                if (leaveRequest.LeaveDays <= 0)
                    return Invalid(modelState, "Please enter a valid amount of leave days for Mixed day type.");

                if (leaveRequest.Type == LeaveType.ANNUAL)
                {
                    decimal maxCalculatedDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                    if (leaveRequest.LeaveDays > maxCalculatedDays)
                        return Invalid(modelState, "The entered leave amount exceeds the maximum calculated days for the selected date range.");
                }
            }
            else if (leaveRequest.From.Date == leaveRequest.To.Date
                     && leaveRequest.From.Date == entity.From.Date
                     && leaveRequest.To.Date == entity.To.Date
                     && leaveRequest.Day == entity.Day)
            {
                leaveRequest.LeaveDays = entity.LeaveDays;
            }
            else if (leaveRequest.Type == LeaveType.ANNUAL)
            {
                if (leaveRequest.From.Date < leaveRequest.To.Date)
                    leaveRequest.LeaveDays = HrHelper.GetLeaveDays(leaveRequest.From, leaveRequest.To, fiscalYearId, leaveRequest.Day, leaveRequest.Type, leaveRequest.EmployeeId);
                else if (leaveRequest.ToTime > leaveRequest.FromTime)
                    leaveRequest.LeaveDays = (decimal)(leaveRequest.ToTime.Value - leaveRequest.FromTime.Value).TotalHours / 8;
                else
                    leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? 1 : 0.5m;
            }
            else
            {
                var totalDays = (decimal)(leaveRequest.To - leaveRequest.From).TotalDays + 1;
                leaveRequest.LeaveDays = leaveRequest.Day == LeaveDay.Full_Day ? totalDays : totalDays / 2;
            }

            leaveRequest.LeaveDays = Math.Abs(leaveRequest.LeaveDays);

            if (leaveRequest.MorningOrAfternoon == MorningOrAfternoon.Afternoon)
            {
                leaveRequest.LeaveDays -= GetHalfDayWeekendDays(leaveRequest.From, leaveRequest.To);
            }

            if (leaveRequest.LeaveDays <= 0)
                return Invalid(modelState, "Leave days must be greater than 0!!");

            return true;
        }

        private bool ValidatePolicies(LeaveRequest leaveRequest, LeaveRequest entity, Person_Employee employee, decimal totalBalance, ModelStateDictionary modelState)
        {
            if (leaveRequest.Type == LeaveType.Leave_With_Out_Pay && totalBalance > 0)
                return Invalid(modelState, $"You can't request Leave without pay having annual leave Amount = {totalBalance}, use your leave first");

            if (leaveRequest.LeaveDays > totalBalance && leaveRequest.Type == LeaveType.ANNUAL)
                return Invalid(modelState, $"The requested leave days ({leaveRequest.LeaveDays}) cannot exceed the available leave balance ({totalBalance}).");

            bool overlaps = db.LeaveRequest.Any(x => !x.IsVoid
                && x.EmployeeId == leaveRequest.EmployeeId
                && x.LeaveRequestId != entity.LeaveRequestId
                && ((x.From <= leaveRequest.From && x.To >= leaveRequest.From) || (x.From <= leaveRequest.To && x.To >= leaveRequest.To)));
            if (overlaps)
                return Invalid(modelState, "Your request overlaps with another existing leave request.");

            if (employee != null)
            {
                if (leaveRequest.Type == LeaveType.MATERNITY && employee.Gender == Gender.MALE)
                    return Invalid(modelState, "Males can't take Maternity Leave");

                if (leaveRequest.Type == LeaveType.PATERNITY && employee.Gender == Gender.FEMALE)
                    return Invalid(modelState, "Females can't take Paternity Leave");
            }

            var forcedAllowed = new[] { LeaveType.ANNUAL, LeaveType.OTHER, LeaveType.Leave_With_Out_Pay };
            if (leaveRequest.LeaveActionTypes == LeaveActionTypes.Forced && !forcedAllowed.Contains(leaveRequest.Type))
                return Invalid(modelState, $"Forced Leave is only allowed for: {string.Join(", ", forcedAllowed)}. You selected: {leaveRequest.Type}.", "Type");

            return true;
        }

        private static void Copy(LeaveRequest source, LeaveRequest target, Person_Employee employee, decimal totalBalance, string userName)
        {
            target.EmployeeId = source.EmployeeId;
            target.BusinessUnitId = source.BusinessUnitId;
            target.BranchId = source.BranchId;
            target.LookupId = source.LookupId;
            target.From = source.From;
            target.To = source.To;
            target.FromTime = source.FromTime;
            target.ToTime = source.ToTime;
            target.LeaveDays = source.LeaveDays;
            target.ExactDeserveLeaveDays = source.ExactDeserveLeaveDays;
            target.Type = source.Type;
            target.Reason = source.Reason;
            target.GlFiscalYearId = source.GlFiscalYearId;
            target.Remark = source.Remark;
            target.LastModified = DateTime.Now;
            target.ModifiedBy = userName;
            target.LeaveBalance = totalBalance;
            target.Day = source.Day;
            target.MorningOrAfternoon = source.MorningOrAfternoon;
            target.Hours = source.Hours;
            target.FromAmharic = source.FromAmharic;
            target.ToAmharic = source.ToAmharic;
            target.IsForOther = source.IsForOther;
            target.LeaveActionTypes = source.LeaveActionTypes;
            target.EmployeeCode = employee?.EmployeeCode;
        }

        private static LeaveRequest Fail(ModelStateDictionary modelState, string message, string key = "")
        {
            modelState.AddModelError(key, message);
            return null;
        }

        private static bool Invalid(ModelStateDictionary modelState, string message, string key = "")
        {
            modelState.AddModelError(key, message);
            return false;
        }
    }
}

