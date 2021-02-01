using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{

    [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
    public class LeaveEntitleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var periodList = db.GLFiscalYears.ToList();
            ViewData["PeriodList"] = periodList;// != null ? periodList.ToList() : new List<GlFiscalYear>();
            //ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);

            ViewData["EmployeeList"] = db.Person_Employee.ToList();

            var list = inputs.GetEmployees(User.Identity.Name);

            ViewData["Period"] = db.GLFiscalYears.ToList();

            ViewData["branch"] = db.Branch.ToList();

        }

        #region Entitlement By Old Awaj ... add 1 day per year
        public void GetEmployeeLeaveEntitlemnt(bool check, int period)
        {
            //var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now);
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);

            var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            int maximumValue = 0;
            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null)
            {
                maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
            }
            LeaveEntitle entitle = new LeaveEntitle();

            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            var employeeList = db.Person_Employee.Where(x => employees.Contains(x.EmployeeId.ToString()) && x.IsActive).AsQueryable();

            foreach (var emp in employeeList)
            {
                var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == emp.EmployeeId);
                if (leaveEntitle != null)
                {
                    DateTime hireDate = emp.HiredDate;
                    if (hireDate.Year <= currentPeriod.DateTo.Year)
                    {

                        var add = 0;
                        double currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);

                        double month = (currentPeriod.DateTo - emp.HiredDate).Days / 30;

                        double y = (int)month / 12;

                        double ym = month % 12;

                        if (leaveThrashold != null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue) && (hireDate > currentPeriod.DateTo))
                        {
                            currentExpriance = 1;
                        }
                        else if (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
                        {
                            currentExpriance = 0;

                        }
                        else if (leaveThrashold == null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue))
                        {
                            currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);
                        }
                        else
                        {
                            currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);
                        }


                        //if (currentPeriod.DateTo.Year > DateTime.Now.Year)
                        //{
                        //    add = currentPeriod.DateTo.Year - DateTime.Now.Year;
                        //    currentExpriance = currentExpriance + add;
                        //}
                        //else if (currentPeriod.DateTo.Year < DateTime.Now.Year)
                        //{
                        //    add = DateTime.Now.Year - currentPeriod.DateTo.Year;
                        //    currentExpriance = currentExpriance - add;
                        //}
                        double totalDuration = 0;
                        if (check)
                        {
                            var expriance = db.Experiences.Where(e => e.EmployeeId == emp.EmployeeId).ToList();
                            if (expriance.Any())
                            {
                                totalDuration = Convert.ToDouble(expriance.Sum(s => s.TotalDuration));
                            }
                        }
                        double totalExpriance = currentExpriance + totalDuration;
                        var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);
                        double LeaveBaseLine = 0;

                        if (role != null)
                        {
                            var baseLine = 0;
                            if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                            {
                                var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                if (firstOrDefault != null)
                                    LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                            }
                            else
                            {
                                var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                if (firstOrDefault != null)
                                    LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                            }

                            if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
                            {
                                currentExpriance = 0;
                                // currentExpriance = ym/12;
                                totalExpriance = currentExpriance + totalDuration;
                                LeaveBaseLine = ym / 12 * baseLine;

                            }
                            if ((hireDate > currentPeriod.DateTo) && (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue)))
                            {
                                LeaveBaseLine = baseLine;
                            }
                        }
                        decimal amount = Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                        //if (currentPeriod.DateTo.Year == hireDate.Year)
                        //{
                        //    var dateWorked = (currentPeriod.DateTo - hireDate).Days;

                        //    double baseLine = (LeaveBaseLine * dateWorked) / 365;
                        //    amount = Convert.ToDecimal(baseLine + totalExpriance);
                        //}


                        leaveEntitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                        //leaveEntitle.Balance = Convert.ToDecimal(totalExpriance + LeaveBaseLine);

                        if (maximumValue < (double)leaveEntitle.Amount)
                        {
                            leaveEntitle.Amount = maximumValue;
                            //leaveEntitle.Balance = maximumValue;
                        }
                        if (leaveEntitle.Amount < 0)
                        {
                            leaveEntitle.Amount = 0;
                            //leaveEntitle.Balance = 0;
                        }

                        db.Entry(leaveEntitle).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (leaveEntitle == null)
                {
                    if (currentPeriod != null)
                    {
                        DateTime hireDate = emp.HiredDate;
                        if (hireDate <= currentPeriod.DateTo)
                        {
                            EmployeeIdView = new EmployeeIdView();
                            EmployeeIdView.PersonId = emp.EmployeeId;
                            EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                            EmployeeIdView.IsActive = emp.IsActive;
                            EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

                            var add = 0;
                            double currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);


                            double month = (currentPeriod.DateTo - emp.HiredDate).Days / 30;

                            double y = (int)month / 12;

                            double ym = (month % 12);

                            if (leaveThrashold != null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue) && (hireDate > currentPeriod.DateTo))
                            {
                                currentExpriance = 1;
                            }
                            else if (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
                            {
                                currentExpriance = 0;
                            }
                            else if (leaveThrashold == null && y == 0)
                            {
                                currentExpriance = 0;// (currentPeriod.DateTo.Year - emp.HiredDate.Year);
                            }
                            else
                            {
                                double expMon = (DateTime.Today - emp.HiredDate).Days / 30;

                                double expY = (int)expMon / 12;

                                double expM = (expMon % 12);

                                currentExpriance = expY;// (currentPeriod.DateTo.Year - emp.HiredDate.Year);
                            }

                            //if (currentPeriod.DateTo.Year > DateTime.Now.Year)
                            //{
                            //    add = currentPeriod.DateTo.Year - DateTime.Now.Year;
                            //    currentExpriance = currentExpriance + add;
                            //}
                            //else if (currentPeriod.DateTo.Year < DateTime.Now.Year)
                            //{
                            //    add = DateTime.Now.Year - currentPeriod.DateTo.Year;
                            //    currentExpriance = currentExpriance - add;
                            //}
                            double totalDuration = 0;
                            if (check)
                            {
                                var expriance = db.Experiences.Where(e => e.EmployeeId == emp.EmployeeId).ToList();
                                if (expriance.Any())
                                {
                                    totalDuration = Convert.ToDouble(expriance.Sum(s => s.TotalDuration));
                                }
                            }
                            double totalExpriance = currentExpriance + totalDuration;
                            var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);

                            entitle = new LeaveEntitle();
                            entitle.EmployeeId = emp.EmployeeId;
                            entitle.EmployeeCode = emp.EmployeeCode;
                            db = new ExceedDbContext();

                            double LeaveBaseLine = 0;

                            if (role != null)
                            {
                                var baseLine = 0;

                                if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                {
                                    var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                    if (firstOrDefault != null)
                                        LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                    baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                }
                                else
                                {
                                    var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                    if (firstOrDefault != null)
                                        LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                    baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
                                }
                                if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && y == 0)
                                {
                                    //currentExpriance = 0;
                                    // currentExpriance = ym/12;
                                    totalExpriance = currentExpriance + totalDuration;
                                    LeaveBaseLine = ym / 12 * baseLine;

                                }
                                var hdate = hireDate.AddYears(1);
                                if ((hdate >= currentPeriod.DateFrom && hdate <= currentPeriod.DateTo))
                                {
                                    var din = currentPeriod.DateFrom.AddDays(-1);

                                    double mmm = (din - emp.HiredDate).Days / 30;

                                    double yy = (int)mmm / 12;

                                    double ymm = (mmm % 12);

                                    if ((leaveThrashold != null && yy == 0 && (int)ymm < Convert.ToInt32(leaveThrashold.CurrentValue)))
                                    {
                                        LeaveBaseLine = baseLine;
                                    }
                                    if ((leaveThrashold != null && yy == 0 && (int)ymm >= Convert.ToInt32(leaveThrashold.CurrentValue)))
                                    {
                                        LeaveBaseLine = baseLine;
                                        totalExpriance = totalExpriance + 1;
                                    }

                                }
                            }
                            decimal amount = Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                            //if (currentPeriod.DateTo.Year == hireDate.Year)
                            //{
                            //    var dateWorked = (currentPeriod.DateTo - hireDate).Days;

                            //    double baseLine = (LeaveBaseLine * dateWorked) / 365;
                            //    amount = Convert.ToDecimal(baseLine + totalExpriance);
                            //}



                            entitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                            entitle.Balance = amount; //Convert.ToDecimal(totalExpriance + LeaveBaseLine);

                            entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                            entitle.DateCreated = DateTime.Now;
                            entitle.LastModified = null;
                            entitle.CreatedBy = User.Identity.Name;
                            entitle.ModifiedBy = null;
                            entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                            if (Convert.ToDecimal(maximumValue) < entitle.Amount)
                            {
                                entitle.Amount = maximumValue;
                                entitle.Balance = maximumValue;
                            }
                            if (entitle.Amount < 0)
                            {
                                entitle.Amount = 0;
                                entitle.Balance = 0;
                            }
                            db.LeavesEntitle.Add(entitle);
                            db.SaveChanges();
                            //}



                            EmployeeIdViews.Add(EmployeeIdView);
                        }
                    }
                }
            }
        }
        #endregion

        #region Entitlement By New Awaj ... add 1 day per two years and intial 16
        public JavaScriptResult GetEmployeeLeaveEntitlemntNewAwaj(bool check, int period)
        {
            //var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now);
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);

            var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            int maximumValue = 0;
            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null)
            {
                maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
            }
            

            EmployeeIdView EmployeeIdView = new EmployeeIdView();

            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);

            var employeeList = db.Person_Employee.Where(x => employees.Contains(x.EmployeeId.ToString()) && x.IsActive).AsQueryable();
            db = new ExceedDbContext();

            #region Validation

            var forleaders0ld = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
            if(forleaders0ld == null || forleaders0ld.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders old initial value";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }

            var forperformerold = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
            if (forperformerold == null || forperformerold.CurrentValue == null)
            {
                var message = "you have to add Leave for performers old initial value";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }

            var forleadersnew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
            if (forleadersnew == null || forleadersnew.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders new initial value";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }

            var forperformernew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
            if (forperformernew == null || forperformernew.CurrentValue == null)
            {
                var message = "you have to add Leave for performers new initial value";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }

            var oldendyear = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString());
            if (oldendyear == null || oldendyear.CurrentValue == null)
            {
                var message = "you have to add end of old intial value year";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }

            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
            if (frequency == null)
            {
                var message = "you have to add frequency in HRM Setting";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }
            else if (String.IsNullOrEmpty(frequency.CurrentValue))
            {
                var message = "add current value in frequency first";

                return new JavaScriptResult() { Script = "alert('" + message + "');" };

            }

            #endregion

            foreach (var emp in employeeList)
            {               

                var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == emp.EmployeeId);
                if (leaveEntitle != null)
                {

                    var initialSetting = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.InitialYear.ToString());
                    if (initialSetting != null)
                    {
                        int yearInitial = Convert.ToInt32(initialSetting.CurrentValue);

                        var intialFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == yearInitial /*&& x.DateTo.Year >= year*/);
                        if (intialFiscalYear != null && emp.HiredDate < intialFiscalYear.DateTo)
                        {
                            double currentExpriance = GetCurrentExperienceLeaveDays(emp.EmployeeId, currentPeriod, intialFiscalYear);

                            DateTime hireDate = emp.HiredDate;

                            double totalDays = (currentPeriod.DateTo - emp.HiredDate).TotalDays;
                            var totalYears = Math.Truncate(totalDays / 365);
                            var totalMonths = Math.Truncate((totalDays % 365) / 30);
                            var remainingDays = Math.Truncate((totalDays % 365) % 30);

                            double externalExperience = 0;
                            if (check)
                            {
                                externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
                            }

                            double totalExpriance = currentExpriance + externalExperience;

                            var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);


                            leaveEntitle.EmployeeId = emp.EmployeeId;
                            leaveEntitle.EmployeeCode = emp.EmployeeCode;

                            // var hiredYear = db.GLFiscalYears.FirstOrDefault(x=>x.DateFrom <= emp.HiredDate && x.DateTo >= emp.HiredDate);

                            double LeaveBaseLine = 0;

                            var oldEndYear = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString());

                            if (oldEndYear != null)
                            {
                                var oldEndBudgetYear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == oldEndYear.CurrentValue.Trim());

                                if (oldEndBudgetYear != null)
                                {
                                    if (role != null && emp.HiredDate < oldEndBudgetYear.DateFrom)
                                    {
                                        var baseLine = 0;

                                        if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                        {
                                            var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                            if (leaders != null)
                                            {
                                                LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                baseLine = Convert.ToInt32(leaders.CurrentValue);
                                            }
                                        }
                                        else
                                        {
                                            var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                            if (performers != null)
                                            {
                                                LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                baseLine = Convert.ToInt32(performers.CurrentValue);
                                            }
                                        }
                                        if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && totalYears == 0)
                                        {
                                            //currentExpriance = 0;
                                            // currentExpriance = ym/12;
                                            // totalExpriance = currentExpriance + externalExperience;
                                            LeaveBaseLine = totalMonths / 12 * baseLine;

                                        }

                                    }
                                    else if (role != null && emp.HiredDate >= oldEndBudgetYear.DateFrom)
                                    {
                                        var baseLine = 0;

                                        if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                        {
                                            var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
                                            if (leaders != null)
                                            {
                                                LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                baseLine = Convert.ToInt32(leaders.CurrentValue);
                                            }
                                        }
                                        else
                                        {
                                            var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
                                            if (performers != null)
                                            {
                                                LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                baseLine = Convert.ToInt32(performers.CurrentValue);
                                            }
                                        }
                                        if (/*(hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null &&*/ totalYears == 0)
                                        {
                                            //currentExpriance = 0;
                                            // currentExpriance = ym/12;
                                            // totalExpriance = currentExpriance + externalExperience;
                                            LeaveBaseLine = totalMonths / 12 * baseLine;

                                        }

                                    }
                                }
                            }

                            decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

                            //if (leaveEntitlePrev != null && Convert.ToInt32(frequency.CurrentValue) == 1)
                            //{
                            //    amount = leaveEntitlePrev.Amount + 1;
                            //}

                            leaveEntitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                            //leaveEntitle.Balance = amount; //Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                            leaveEntitle.IsAdded = true;
                            leaveEntitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                            leaveEntitle.DateCreated = DateTime.Now;
                            leaveEntitle.LastModified = null;
                            leaveEntitle.CreatedBy = User.Identity.Name;
                            leaveEntitle.ModifiedBy = null;
                            leaveEntitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                            if (Convert.ToDecimal(maximumValue) < leaveEntitle.Amount)
                            {
                                leaveEntitle.Amount = maximumValue;
                                leaveEntitle.Balance = maximumValue;
                            }
                            if (leaveEntitle.Amount < 0)
                            {
                                leaveEntitle.Amount = 0;
                                leaveEntitle.Balance = 0;
                            }

                            db.Entry(leaveEntitle).State = EntityState.Modified;
                            db.SaveChanges();
                            //}



                            EmployeeIdViews.Add(EmployeeIdView);
                        }
                    }
                }
                else if (leaveEntitle == null)
                {
                   var entitle = new LeaveEntitle();

                    if (currentPeriod != null)
                    {

                        if (emp.HiredDate <= currentPeriod.DateFrom || (emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo))
                        {
                            var preveDateFrom = currentPeriod.DateFrom.AddYears(-1);
                            var prevDateTo = currentPeriod.DateTo.AddYears(-1);

                            var prevYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == preveDateFrom && x.DateTo == prevDateTo);
                            if (prevYear != null)
                            {
                                var leaveEntitlePrev = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlFiscalYearId == prevYear.GlFiscalYearId);
                                if (leaveEntitlePrev == null || (leaveEntitlePrev != null && Convert.ToInt32(frequency.CurrentValue) == 1))
                                {
                                    db = new ExceedDbContext();
                                    EmployeeIdView = new EmployeeIdView();
                                    EmployeeIdView.PersonId = emp.EmployeeId;
                                    EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                                    EmployeeIdView.IsActive = emp.IsActive;
                                    EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

                                    var initialSetting = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.InitialYear.ToString());
                                    if (initialSetting != null)
                                    {
                                        int yearInitial = Convert.ToInt32(initialSetting.CurrentValue);

                                        var intialFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == yearInitial /*&& x.DateTo.Year >= year*/);
                                        if (intialFiscalYear != null && emp.HiredDate < intialFiscalYear.DateTo)
                                        {
                                            double currentExpriance = GetCurrentExperienceLeaveDays(emp.EmployeeId, currentPeriod, intialFiscalYear);

                                            DateTime hireDate = emp.HiredDate;

                                            double totalDays = (currentPeriod.DateTo - emp.HiredDate).TotalDays;
                                            var totalYears = Math.Truncate(totalDays / 365);
                                            var totalMonths = Math.Truncate((totalDays % 365) / 30);
                                            var remainingDays = Math.Truncate((totalDays % 365) % 30);

                                            double externalExperience = 0;
                                            if (check)
                                            {
                                                externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
                                            }

                                            double totalExpriance = currentExpriance + externalExperience;

                                            var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);


                                            entitle.EmployeeId = emp.EmployeeId;
                                            entitle.EmployeeCode = emp.EmployeeCode;

                                            // var hiredYear = db.GLFiscalYears.FirstOrDefault(x=>x.DateFrom <= emp.HiredDate && x.DateTo >= emp.HiredDate);

                                            double LeaveBaseLine = 0;

                                            var oldEndYear = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString());

                                            if (oldEndYear != null)
                                            {
                                               
                                                var oldEndBudgetYear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == oldEndYear.CurrentValue.Trim());

                                                if (oldEndBudgetYear != null)
                                                {
                                                    if (role != null && emp.HiredDate < oldEndBudgetYear.DateFrom)
                                                    {
                                                        var baseLine = 0;

                                                        if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                        {
                                                            var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                                            if (leaders != null)
                                                            {
                                                                LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                baseLine = Convert.ToInt32(leaders.CurrentValue);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                                            if (performers != null)
                                                            {
                                                                LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                                baseLine = Convert.ToInt32(performers.CurrentValue);
                                                            }
                                                        }
                                                        if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && totalYears == 0)
                                                        {
                                                            //currentExpriance = 0;
                                                            // currentExpriance = ym/12;
                                                            // totalExpriance = currentExpriance + externalExperience;
                                                            LeaveBaseLine = totalMonths / 12 * baseLine;

                                                        }

                                                    }
                                                    else if (role != null && emp.HiredDate >= oldEndBudgetYear.DateFrom)
                                                    {
                                                        var baseLine = 0;

                                                        if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                        {
                                                            var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
                                                            if (leaders != null)
                                                            {
                                                                LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                baseLine = Convert.ToInt32(leaders.CurrentValue);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
                                                            if (performers != null)
                                                            {
                                                                LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                                baseLine = Convert.ToInt32(performers.CurrentValue);
                                                            }
                                                        }
                                                        if (/*(hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null &&*/ totalYears == 0)
                                                        {
                                                            //currentExpriance = 0;
                                                            // currentExpriance = ym/12;
                                                            // totalExpriance = currentExpriance + externalExperience;
                                                            LeaveBaseLine = totalMonths / 12 * baseLine;

                                                        }

                                                    }
                                                }
                                            }

                                            decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

                                            //if (leaveEntitlePrev != null && Convert.ToInt32(frequency.CurrentValue) == 1)
                                            //{
                                            //    amount = leaveEntitlePrev.Amount + 1;
                                            //}

                                            entitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                                            entitle.Balance = amount; //Convert.ToDecimal(totalExpriance + LeaveBaseLine);
                                            entitle.IsAdded = true;
                                            entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                                            entitle.DateCreated = DateTime.Now;
                                            entitle.LastModified = null;
                                            entitle.CreatedBy = User.Identity.Name;
                                            entitle.ModifiedBy = null;
                                            entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                                            if (Convert.ToDecimal(maximumValue) < entitle.Amount)
                                            {
                                                entitle.Amount = maximumValue;
                                                entitle.Balance = maximumValue;
                                            }
                                            if (entitle.Amount < 0)
                                            {
                                                entitle.Amount = 0;
                                                entitle.Balance = 0;
                                            }
                                            db.LeavesEntitle.Add(entitle);
                                            db.SaveChanges();
                                            //}



                                            EmployeeIdViews.Add(EmployeeIdView);
                                        }
                                    }

                                }
                                else
                                {
                                    var leaveEntitlePrevAdded = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlFiscalYearId == prevYear.GlFiscalYearId && x.IsAdded);
                                    if (leaveEntitlePrevAdded != null)
                                    {
                                        db = new ExceedDbContext();
                                        EmployeeIdView = new EmployeeIdView();
                                        EmployeeIdView.PersonId = leaveEntitlePrevAdded.EmployeeId;
                                        EmployeeIdView.EmployeeCode = leaveEntitlePrevAdded.EmployeeCode;
                                        EmployeeIdView.IsActive = emp.IsActive;
                                        EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

                                        var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);

                                        decimal defaultLeave = 0;

                                        var oldEndYear = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString());

                                        if (oldEndYear != null)
                                        {
                                           
                                            var oldEndBudgetYear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == oldEndYear.CurrentValue.Trim());

                                            if (oldEndBudgetYear != null)
                                            {

                                                if (role != null && emp.HiredDate < oldEndBudgetYear.DateFrom)
                                                {
                                                    if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                    {
                                                        var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                                        if (leaders != null)
                                                        {
                                                            defaultLeave = Convert.ToInt32(leaders.CurrentValue);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                                        if (performers != null)
                                                        {
                                                            defaultLeave = Convert.ToInt32(performers.CurrentValue);
                                                        }
                                                    }
                                                }
                                                else if (role != null && emp.HiredDate >= oldEndBudgetYear.DateFrom)
                                                {
                                                    if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                    {
                                                        var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
                                                        if (leaders != null)
                                                        {
                                                            defaultLeave = Convert.ToInt32(leaders.CurrentValue);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
                                                        if (performers != null)
                                                        {
                                                            defaultLeave = Convert.ToInt32(performers.CurrentValue);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (leaveEntitlePrevAdded.Amount < defaultLeave)
                                        {
                                            entitle.Amount = defaultLeave;
                                            entitle.Balance = defaultLeave;
                                        }
                                        else
                                        {
                                            entitle.Amount = leaveEntitlePrevAdded.Amount;
                                            entitle.Balance = leaveEntitlePrevAdded.Amount;
                                        }

                                        entitle.EmployeeId = emp.EmployeeId;
                                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                                        entitle.DateCreated = DateTime.Now;
                                        entitle.LastModified = null;
                                        entitle.CreatedBy = User.Identity.Name;
                                        entitle.ModifiedBy = null;
                                        entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                                        db.LeavesEntitle.Add(entitle);
                                        db.SaveChanges();
                                    }
                                    var leaveEntitlePrevNotAdded = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlFiscalYearId == prevYear.GlFiscalYearId && !x.IsAdded);
                                    if (leaveEntitlePrevNotAdded != null)
                                    {
                                        db = new ExceedDbContext();
                                        EmployeeIdView = new EmployeeIdView();
                                        EmployeeIdView.PersonId = leaveEntitlePrevNotAdded.EmployeeId;
                                        EmployeeIdView.EmployeeCode = leaveEntitlePrevNotAdded.EmployeeCode;
                                        EmployeeIdView.IsActive = emp.IsActive;
                                        EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

                                        //entitle.EmployeeId = emp.EmployeeId;
                                        //entitle.Amount = leaveEntitlePrevNotAdded.Amount + 1;
                                        //entitle.Balance = leaveEntitlePrevNotAdded.Amount + 1;
                                        //entitle.IsAdded = true;
                                        //entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                                        //entitle.DateCreated = DateTime.Now;
                                        //entitle.LastModified = null;
                                        //entitle.CreatedBy = User.Identity.Name;
                                        //entitle.ModifiedBy = null;
                                        //entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                                        var initialSetting = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == LeaveHrmSetting.InitialYear.ToString());
                                        if (initialSetting != null)
                                        {
                                            int yearInitial = Convert.ToInt32(initialSetting.CurrentValue);

                                            var intialFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == yearInitial /*&& x.DateTo.Year >= year*/);
                                            if (intialFiscalYear != null && emp.HiredDate < intialFiscalYear.DateTo)
                                            {
                                                double currentExpriance = GetCurrentExperienceLeaveDays(emp.EmployeeId, currentPeriod, intialFiscalYear);

                                                DateTime hireDate = emp.HiredDate;

                                                double totalDays = (currentPeriod.DateTo - emp.HiredDate).TotalDays;
                                                var totalYears = Math.Truncate(totalDays / 365);
                                                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                                                var remainingDays = Math.Truncate((totalDays % 365) % 30);

                                                double externalExperience = 0;
                                                if (check)
                                                {
                                                    externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
                                                }

                                                entitle = new LeaveEntitle();

                                                entitle.EmployeeId = emp.EmployeeId;
                                                entitle.EmployeeCode = emp.EmployeeCode;

                                                double totalExpriance = currentExpriance + externalExperience;

                                                var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);

                                               
                                                

                                                double LeaveBaseLine = 0;

                                                var oldEndYear = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.Old_Initial_Leave_End_Year.ToString());

                                                if (oldEndYear != null)
                                                {
                                                    var oldEndBudgetYear = db.GLFiscalYears.FirstOrDefault(x => x.Name.Trim() == oldEndYear.CurrentValue.Trim());

                                                    if (oldEndBudgetYear != null)
                                                    {

                                                        if (role != null && emp.HiredDate < oldEndBudgetYear.DateFrom)
                                                        {
                                                            var baseLine = 0;

                                                            if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                            {
                                                                var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                                                                if (leaders != null)
                                                                {
                                                                    LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                    baseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                                                                if (performers != null)
                                                                {
                                                                    LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                                    baseLine = Convert.ToInt32(performers.CurrentValue);
                                                                }
                                                            }
                                                            if (totalYears == 0)
                                                            {
                                                                //currentExpriance = 0;
                                                                // currentExpriance = ym/12;
                                                                //  totalExpriance = currentExpriance + externalExperience;
                                                                LeaveBaseLine = totalMonths / 12 * baseLine;

                                                            }

                                                        }
                                                        else if (role != null && emp.HiredDate >= oldEndBudgetYear.DateFrom)
                                                        {
                                                            var baseLine = 0;

                                                            if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                                                            {
                                                                var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
                                                                if (leaders != null)
                                                                {
                                                                    LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                    baseLine = Convert.ToInt32(leaders.CurrentValue);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
                                                                if (performers != null)
                                                                {
                                                                    LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                                                                    baseLine = Convert.ToInt32(performers.CurrentValue);
                                                                }
                                                            }
                                                            if (totalYears == 0)
                                                            {
                                                                //currentExpriance = 0;
                                                                // currentExpriance = ym/12;
                                                                //   totalExpriance = currentExpriance + externalExperience;
                                                                LeaveBaseLine = totalMonths / 12 * baseLine;

                                                            }

                                                        }
                                                    }
                                                }
                                                // this line is commented because of it calculate the leave from the beggining but now we need ..
                                                // to add the one to the previouse entitle

                                                decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

                                                // we add 1 to the previous entitle amount
                                                //decimal amount = leaveEntitlePrevNotAdded.Amount + 1;

                                                entitle.Amount = amount;
                                                entitle.Balance = amount;
                                                entitle.IsAdded = true;
                                                entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                                                entitle.DateCreated = DateTime.Now;
                                                entitle.LastModified = null;
                                                entitle.CreatedBy = User.Identity.Name;
                                                entitle.ModifiedBy = null;
                                                entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


                                                if (Convert.ToDecimal(maximumValue) < entitle.Amount)
                                                {
                                                    entitle.Amount = maximumValue;
                                                    entitle.Balance = maximumValue;
                                                }
                                                if (entitle.Amount < 0)
                                                {
                                                    entitle.Amount = 0;
                                                    entitle.Balance = 0;
                                                }
                                                db.LeavesEntitle.Add(entitle);
                                                db.SaveChanges();
                                                //}


                                                EmployeeIdViews.Add(EmployeeIdView);
                                            }
                                        }

                                    }
                                }
                            }

                        }
                    }

                }

            }
            var messages = "Leave Successfully Generated";

            return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        }
        #endregion
        public double GetExternalExperienceLeaveDays(int id)
        {
            double durationAfter = 0;
            double durationBefore = 0;
            double totalDuration = 0;
            double totalDays = 0;
            var configuration = db.Configurations.FirstOrDefault(x => x.Reference == "HRM" && x.Attribute == LeaveHrmSetting.InitialYear.ToString());
            if (configuration != null)
            {
                var year = Convert.ToInt32(configuration.CurrentValue);
                var expriances = db.Experiences.Where(e => e.EmployeeId == id).ToList();
                foreach (var exp in expriances)
                {
                    if (exp.From.Year <= year && exp.To.Year <= year)
                    {
                        //double month = exp.To.Subtract(exp.From).Days / 30;

                        //double y = (int)month / 12;

                        //durationBefore = durationBefore + y;
                        totalDays = (exp.To - exp.From).TotalDays;
                    }
                    if (exp.From.Year <= year && exp.To.Year >= year)
                    {
                        //double month = exp.To.Subtract(exp.From).Days / 30;

                        //double y = (int)month / 12;

                        //durationBefore = durationBefore + y;
                        var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                        if (frequency != null)
                        {

                            var intialFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == year);
                            double first = (intialFiscalYear.DateFrom.AddDays(-1) - exp.From).TotalDays;
                            double second = ((exp.To - intialFiscalYear.DateFrom).TotalDays) / Convert.ToInt32(frequency.CurrentValue);

                            totalDays = first + second;
                        }

                    }
                    else if (exp.From.Year >= year)
                    {
                        var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                        if (frequency != null)
                        {
                            totalDays = ((exp.To - exp.From).TotalDays) / Convert.ToInt32(frequency.CurrentValue);
                        }
                    }
                }
            }

            var totalYears = Math.Truncate(totalDays / 365);
            var totalMonths = Math.Truncate((totalDays % 365) / 30);
            var remainingDays = Math.Truncate((totalDays % 365) % 30);


            return totalYears;
        }
        public double GetCurrentExperienceLeaveDays(int id, GlFiscalYear currentPeriod, GlFiscalYear intialYear)
        {
            double currentExpriance = 0;
            double totalDays = 0;
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                double first = 0;
                double second = 0;

                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

                DateTime hireDate = person.HiredDate;
                if (hireDate <= currentPeriod.DateTo && currentPeriod.DateTo <= intialYear.DateFrom)
                {
                    // experience before the initial year 
                    totalDays = (currentPeriod.DateTo - person.HiredDate).TotalDays;
                    //var totalYears = Math.Truncate(totalDays / 365);
                    //var totalMonths = Math.Truncate((totalDays % 365) / 30);
                    //var remainingDays = Math.Truncate((totalDays % 365) % 30);

                }
                if (currentPeriod.DateFrom >= intialYear.DateFrom && hireDate <= intialYear.DateFrom)
                {

                    // first means experience before the initial year of awaj
                    first = (intialYear.DateFrom - hireDate).TotalDays;

                    // second means experience after the initial year of awaj
                    second = (currentPeriod.DateTo - intialYear.DateFrom).TotalDays;

                    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                    if (frequency != null)
                    {
                        second = second / Convert.ToInt32(frequency.CurrentValue);
                        totalDays = first + second;
                    }
                }
                if (currentPeriod.DateFrom >= intialYear.DateFrom && hireDate >= intialYear.DateFrom)
                {
                    // experience after the initial year 
                    var expYear = (currentPeriod.DateTo - hireDate).TotalDays;

                    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                    if (frequency != null)
                    {
                        totalDays = expYear / Convert.ToInt32(frequency.CurrentValue);
                    }
                }

                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);

                //double month = (currentPeriod.DateTo - person.HiredDate).Days / 30;

                //double y = (int)month / 12;

                //double ym = (month % 12);

                if (leaveThrashold != null && totalYears == 0 && totalMonths >= Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 1;
                }
                else if (leaveThrashold != null && totalYears == 0 && totalMonths < Convert.ToInt32(leaveThrashold.CurrentValue))
                {
                    currentExpriance = 0;
                }
                else
                {
                    currentExpriance = totalYears;
                }

            }

            return currentExpriance;
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
        public int GetBranch(int id)
        {
            int branch = 0;
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                branch = placement.BranchId;
            }
            else
            {
                var place = db.Placements.Where(x => x.EmployeeId == id).OrderByDescending(x => x.AssignmentDate).ToList().FirstOrDefault();
                if (place != null)
                {
                    branch = place.BranchId;
                }
            }

            return branch;
        }
        public string GetEmployeeCode(int id)
        {
            string code = "";
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                code = person.EmployeeCode;
            }
            return code;
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        public ActionResult LeavesEntitle_Read([DataSourceRequest]DataSourceRequest request)
        {
            //GetUserBranchAllEmployees
            //   List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<int> employees = inputs.GetUserBranchAllEmployees(User.Identity.Name);
            IQueryable<LeaveEntitle> leavesentitle = db.LeavesEntitle.Where(x => employees.Contains(x.EmployeeId) && !x.IsExpired).OrderBy(o => o.GlFiscalYearId);
            DataSourceResult result = leavesentitle.ToDataSourceResult(request, leaveEntitle => new
            {
                LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                EmployeeId = leaveEntitle.EmployeeId,
                Balance = leaveEntitle.Balance,
                BranchId = GetBranch(leaveEntitle.EmployeeId),
                GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                Amount = leaveEntitle.Amount,
                Remark = leaveEntitle.Remark,
                DateCreated = leaveEntitle.DateCreated,
                LastModified = leaveEntitle.LastModified,
                CreatedBy = leaveEntitle.CreatedBy,
                ModifiedBy = leaveEntitle.ModifiedBy,
                EmployeeCode = GetEmployeeCode(leaveEntitle.EmployeeId),
                TotlalAmount = leaveEntitle.TotlalAmount,
                OrgBranchName = leaveEntitle.OrgBranchName,
                ExpiredBy = leaveEntitle.ExpiredBy,
                ExpiredDate = leaveEntitle.ExpiredDate,
                ReversedBy = leaveEntitle.ReversedBy,
                Void = leaveEntitle.Void,
                ReversedDate = leaveEntitle.ReversedDate

            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Create([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();

            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed entitle amount");
            }
            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && (leaveEntitle.Balance > Convert.ToDecimal(maximumLeave.CurrentValue) || leaveEntitle.Amount > Convert.ToDecimal(maximumLeave.CurrentValue)))
            {
                ModelState.AddModelError("", "Entitle amount can't exceed leave maximum setting");
            }
            if (ModelState.IsValid)
            {

                var fisicalYear = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                var year = db.LeavesEntitle.Where(x => x.GlFiscalYearId == fisicalYear.GlFiscalYearId && x.EmployeeId == leaveEntitle.EmployeeId).FirstOrDefault();
                if (year != null)
                {
                    ModelState.AddModelError("", "Cannot Register more than once per ayear");
                }
                else
                {
                    var branchId = 0;

                    var branch = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
                    if (branch != null)
                    {
                        branchId = branch.BranchId;
                    }

                    var entity = new LeaveEntitle
                    {
                        EmployeeId = leaveEntitle.EmployeeId,
                        BranchId = branchId,
                        TotlalAmount = leaveEntitle.TotlalAmount,
                        GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                        Amount = leaveEntitle.Amount,
                        Balance = leaveEntitle.Balance,
                        Remark = leaveEntitle.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = leaveEntitle.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = leaveEntitle.ModifiedBy,
                        EmployeeCode = inputs.GetEmployees().Where(x => x.PersonId == leaveEntitle.EmployeeId).FirstOrDefault().EmployeeCode,

                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };
                    entity.TotlalAmount = entity.Amount + GetTransfreedAmmount(leaveEntitle.GlFiscalYearId, leaveEntitle.EmployeeId);
                    db.LeavesEntitle.Add(entity);
                    db.SaveChanges();
                    leaveEntitle.LeaveEntitleId = entity.LeaveEntitleId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + person.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                    }
                    #endregion

                }
            }
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));

        }
        private decimal GetTransfreedAmmount(int periodId, int empId)
        {
            var transfrred = db.LeaveTransfer.Where(x => x.To == periodId && x.EmployeeId == empId).FirstOrDefault();
            if (transfrred != null)
            {

                return transfrred.Amount;
            }
            return 0;                               //
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Update([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();
            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed entitle amount");
            }
            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && (leaveEntitle.Balance > Convert.ToDecimal(maximumLeave.CurrentValue) || leaveEntitle.Amount > Convert.ToDecimal(maximumLeave.CurrentValue)))
            {
                ModelState.AddModelError("", "Entitle amount can't exceed leave maximum setting");
            }
            if (ModelState.IsValid)
            {
                var branchId = 0;

                var branch = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
                if (branch != null)
                {
                    branchId = branch.BranchId;
                }

                var entity = new LeaveEntitle
                {
                    LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                    EmployeeId = leaveEntitle.EmployeeId,
                    BranchId = branchId,
                    Balance = leaveEntitle.Balance,
                    TotlalAmount = leaveEntitle.TotlalAmount,
                    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                    Amount = leaveEntitle.Amount,
                    Remark = leaveEntitle.Remark,
                    DateCreated = leaveEntitle.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = leaveEntitle.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == leaveEntitle.EmployeeId).EmployeeCode,
                    OrgBranchName = leaveEntitle.OrgBranchName
                };
                //
                entity.TotlalAmount = entity.Amount + GetTransfreedAmmount(leaveEntitle.GlFiscalYearId, leaveEntitle.EmployeeId);
                db.LeavesEntitle.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion
            }

            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Destroy([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {

                var year = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                int selectedToYear = year.DateTo.Year;
                int selectedFromYear = year.DateFrom.Year;
                int currentYear = DateTime.Now.Year;

                if (selectedFromYear <= currentYear && selectedToYear >= currentYear)
                {
                    ModelState.AddModelError("", "You cannot Delete current year Entitlement");
                }
                else
                {

                    var entity = new LeaveEntitle
                    {
                        LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                        EmployeeId = leaveEntitle.EmployeeId,
                        BranchId = leaveEntitle.BranchId,
                        Balance = leaveEntitle.Balance,
                        TotlalAmount = leaveEntitle.TotlalAmount,
                        GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                        Amount = leaveEntitle.Amount,
                        Remark = leaveEntitle.Remark,
                        DateCreated = leaveEntitle.DateCreated,
                        LastModified = leaveEntitle.LastModified,
                        CreatedBy = leaveEntitle.CreatedBy,
                        ModifiedBy = leaveEntitle.ModifiedBy,
                        EmployeeCode = leaveEntitle.EmployeeCode,
                        OrgBranchName = leaveEntitle.OrgBranchName,


                    };
                    #region delete log
                    var yearDelete = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                    var delete = yearDelete.Name;
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee leave entitlement of employee" + leaveEntitle.EmployeeCode + " and fiscal year " + delete);
                    #endregion
                    db.LeavesEntitle.Attach(entity);
                    db.LeavesEntitle.Remove(entity);
                    db.SaveChanges();
                }
            }
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        public void ExpireEntitle(string period)
        {
            var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();
            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = true;
                leave.ExpiredBy = User.Identity.Name;
                leave.ExpiredDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public void ReverseExpireEntitle(string period)
        {
            var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();
            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = false;
                leave.ReversedBy = User.Identity.Name;
                leave.ReversedDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave entitlemnt");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave entitlemnt");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
