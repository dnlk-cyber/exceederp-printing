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

    [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin)]
    public class LeaveEntitleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        //HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["PeriodList"] = CacheData.GetFiscalYearCaches().Select(
             i => new
             {
                 Name = i.Name,
                 GlFiscalYearId = i.Id,
             }).AsQueryable();

            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
              i => new
              {
                  FullNameAmharic = i.AmharicName,
                  FullNameaEnglish = i.Name,
                  EmployeeId = i.Id,
              }).AsQueryable();
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
               i => new
               {
                   Name = i.Name,
                   BranchId = i.Id,
               }).AsQueryable(); ;
            
        }

        #region Entitlement By Old Awaj ... add 1 day per year
        //public void GetEmployeeLeaveEntitlemnt(bool check, int period)
        //{
        //    //var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Now && x.DateTo >= DateTime.Now);
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);

        //    var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //    List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
        //    int maximumValue = 0;
        //    var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
        //    if (maximumLeave != null)
        //    {
        //        maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
        //    }
        //    LeaveEntitle entitle = new LeaveEntitle();

        //    EmployeeIdView EmployeeIdView = new EmployeeIdView();
        //    List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
        //    var employeeList = db.Person_Employee.Where(x => employees.Contains(x.EmployeeId) && x.IsActive).AsQueryable();

        //    foreach (var emp in employeeList)
        //    {
        //        var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == emp.EmployeeId);
        //        if (leaveEntitle != null)
        //        {
        //            DateTime hireDate = emp.HiredDate;
        //            if (hireDate.Year <= currentPeriod.DateTo.Year)
        //            {

        //                var add = 0;
        //                double currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);

        //                double month = (currentPeriod.DateTo - emp.HiredDate).Days / 30;

        //                double y = (int)month / 12;

        //                double ym = month % 12;

        //                if (leaveThrashold != null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue) && (hireDate > currentPeriod.DateTo))
        //                {
        //                    currentExpriance = 1;
        //                }
        //                else if (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
        //                {
        //                    currentExpriance = 0;

        //                }
        //                else if (leaveThrashold == null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue))
        //                {
        //                    currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);
        //                }
        //                else
        //                {
        //                    currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);
        //                }


        //                //if (currentPeriod.DateTo.Year > DateTime.Now.Year)
        //                //{
        //                //    add = currentPeriod.DateTo.Year - DateTime.Now.Year;
        //                //    currentExpriance = currentExpriance + add;
        //                //}
        //                //else if (currentPeriod.DateTo.Year < DateTime.Now.Year)
        //                //{
        //                //    add = DateTime.Now.Year - currentPeriod.DateTo.Year;
        //                //    currentExpriance = currentExpriance - add;
        //                //}
        //                double totalDuration = 0;
        //                if (check)
        //                {
        //                    var expriance = db.Experiences.Where(e => e.EmployeeId == emp.EmployeeId).ToList();
        //                    if (expriance.Any())
        //                    {
        //                        totalDuration = Convert.ToDouble(expriance.Sum(s => s.TotalDuration));
        //                    }
        //                }
        //                double totalExpriance = currentExpriance + totalDuration;
        //                var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);
        //                double LeaveBaseLine = 0;

        //                if (role != null)
        //                {
        //                    var baseLine = 0;
        //                    if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
        //                    {
        //                        var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
        //                        if (firstOrDefault != null)
        //                            LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                        baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                    }
        //                    else
        //                    {
        //                        var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
        //                        if (firstOrDefault != null)
        //                            LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                        baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                    }

        //                    if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
        //                    {
        //                        currentExpriance = 0;
        //                        // currentExpriance = ym/12;
        //                        totalExpriance = currentExpriance + totalDuration;
        //                        LeaveBaseLine = ym / 12 * baseLine;

        //                    }
        //                    if ((hireDate > currentPeriod.DateTo) && (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue)))
        //                    {
        //                        LeaveBaseLine = baseLine;
        //                    }
        //                }
        //                decimal amount = Convert.ToDecimal(totalExpriance + LeaveBaseLine);
        //                //if (currentPeriod.DateTo.Year == hireDate.Year)
        //                //{
        //                //    var dateWorked = (currentPeriod.DateTo - hireDate).Days;

        //                //    double baseLine = (LeaveBaseLine * dateWorked) / 365;
        //                //    amount = Convert.ToDecimal(baseLine + totalExpriance);
        //                //}


        //                leaveEntitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
        //                //leaveEntitle.Balance = Convert.ToDecimal(totalExpriance + LeaveBaseLine);

        //                if (maximumValue < (double)leaveEntitle.Amount)
        //                {
        //                    leaveEntitle.Amount = maximumValue;
        //                    //leaveEntitle.Balance = maximumValue;
        //                }
        //                if (leaveEntitle.Amount < 0)
        //                {
        //                    leaveEntitle.Amount = 0;
        //                    //leaveEntitle.Balance = 0;
        //                }

        //                db.Entry(leaveEntitle).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //        if (leaveEntitle == null)
        //        {
        //            if (currentPeriod != null)
        //            {
        //                DateTime hireDate = emp.HiredDate;
        //                if (hireDate <= currentPeriod.DateTo)
        //                {
        //                    EmployeeIdView = new EmployeeIdView();
        //                    EmployeeIdView.PersonId = emp.EmployeeId;
        //                    EmployeeIdView.EmployeeCode = emp.EmployeeCode;
        //                    EmployeeIdView.IsActive = emp.IsActive;
        //                    EmployeeIdView.FullName = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;

        //                    var add = 0;
        //                    double currentExpriance = (currentPeriod.DateTo.Year - emp.HiredDate.Year);


        //                    double month = (currentPeriod.DateTo - emp.HiredDate).Days / 30;

        //                    double y = (int)month / 12;

        //                    double ym = (month % 12);

        //                    if (leaveThrashold != null && y == 0 && (int)ym >= Convert.ToInt32(leaveThrashold.CurrentValue) && (hireDate > currentPeriod.DateTo))
        //                    {
        //                        currentExpriance = 1;
        //                    }
        //                    else if (leaveThrashold != null && y == 0 && (int)ym < Convert.ToInt32(leaveThrashold.CurrentValue))
        //                    {
        //                        currentExpriance = 0;
        //                    }
        //                    else if (leaveThrashold == null && y == 0)
        //                    {
        //                        currentExpriance = 0;// (currentPeriod.DateTo.Year - emp.HiredDate.Year);
        //                    }
        //                    else
        //                    {
        //                        double expMon = (DateTime.Today - emp.HiredDate).Days / 30;

        //                        double expY = (int)expMon / 12;

        //                        double expM = (expMon % 12);

        //                        currentExpriance = expY;// (currentPeriod.DateTo.Year - emp.HiredDate.Year);
        //                    }

        //                    //if (currentPeriod.DateTo.Year > DateTime.Now.Year)
        //                    //{
        //                    //    add = currentPeriod.DateTo.Year - DateTime.Now.Year;
        //                    //    currentExpriance = currentExpriance + add;
        //                    //}
        //                    //else if (currentPeriod.DateTo.Year < DateTime.Now.Year)
        //                    //{
        //                    //    add = DateTime.Now.Year - currentPeriod.DateTo.Year;
        //                    //    currentExpriance = currentExpriance - add;
        //                    //}
        //                    double totalDuration = 0;
        //                    if (check)
        //                    {
        //                        var expriance = db.Experiences.Where(e => e.EmployeeId == emp.EmployeeId).ToList();
        //                        if (expriance.Any())
        //                        {
        //                            totalDuration = Convert.ToDouble(expriance.Sum(s => s.TotalDuration));
        //                        }
        //                    }
        //                    double totalExpriance = currentExpriance + totalDuration;
        //                    var role = db.Placements.FirstOrDefault(pl => pl.EmployeeId == emp.EmployeeId && pl.isCurrent);

        //                    entitle = new LeaveEntitle();
        //                    entitle.EmployeeId = emp.EmployeeId;
        //                    entitle.EmployeeCode = emp.EmployeeCode;
        //                    db = new ExceedDbContext();

        //                    double LeaveBaseLine = 0;

        //                    if (role != null)
        //                    {
        //                        var baseLine = 0;

        //                        if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
        //                        {
        //                            var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
        //                            if (firstOrDefault != null)
        //                                LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                            baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                        }
        //                        else
        //                        {
        //                            var firstOrDefault = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
        //                            if (firstOrDefault != null)
        //                                LeaveBaseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                            baseLine = Convert.ToInt32(firstOrDefault.CurrentValue);
        //                        }
        //                        if ((hireDate >= currentPeriod.DateFrom && hireDate <= currentPeriod.DateTo) && leaveThrashold != null && y == 0)
        //                        {
        //                            //currentExpriance = 0;
        //                            // currentExpriance = ym/12;
        //                            totalExpriance = currentExpriance + totalDuration;
        //                            LeaveBaseLine = ym / 12 * baseLine;

        //                        }
        //                        var hdate = hireDate.AddYears(1);
        //                        if ((hdate >= currentPeriod.DateFrom && hdate <= currentPeriod.DateTo))
        //                        {
        //                            var din = currentPeriod.DateFrom.AddDays(-1);

        //                            double mmm = (din - emp.HiredDate).Days / 30;

        //                            double yy = (int)mmm / 12;

        //                            double ymm = (mmm % 12);

        //                            if ((leaveThrashold != null && yy == 0 && (int)ymm < Convert.ToInt32(leaveThrashold.CurrentValue)))
        //                            {
        //                                LeaveBaseLine = baseLine;
        //                            }
        //                            if ((leaveThrashold != null && yy == 0 && (int)ymm >= Convert.ToInt32(leaveThrashold.CurrentValue)))
        //                            {
        //                                LeaveBaseLine = baseLine;
        //                                totalExpriance = totalExpriance + 1;
        //                            }

        //                        }
        //                    }
        //                    decimal amount = Convert.ToDecimal(totalExpriance + LeaveBaseLine);
        //                    //if (currentPeriod.DateTo.Year == hireDate.Year)
        //                    //{
        //                    //    var dateWorked = (currentPeriod.DateTo - hireDate).Days;

        //                    //    double baseLine = (LeaveBaseLine * dateWorked) / 365;
        //                    //    amount = Convert.ToDecimal(baseLine + totalExpriance);
        //                    //}



        //                    entitle.Amount = amount;//Convert.ToDecimal(totalExpriance + LeaveBaseLine);
        //                    entitle.Balance = amount; //Convert.ToDecimal(totalExpriance + LeaveBaseLine);

        //                    entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                    entitle.DateCreated = DateTime.Now;
        //                    entitle.LastModified = null;
        //                    entitle.CreatedBy = User.Identity.Name;
        //                    entitle.ModifiedBy = null;
        //                    entitle.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);


        //                    if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                    {
        //                        entitle.Amount = maximumValue;
        //                        entitle.Balance = maximumValue;
        //                    }
        //                    if (entitle.Amount < 0)
        //                    {
        //                        entitle.Amount = 0;
        //                        entitle.Balance = 0;
        //                    }
        //                    db.LeavesEntitle.Add(entitle);
        //                    db.SaveChanges();
        //                    //}



        //                    EmployeeIdViews.Add(EmployeeIdView);
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion

        [AuthorizeRoles( HrmConstants.HRLeaveAdmin)]
        #region Entitlement By New Awaj ... add 1 day per two years and intial 16
        public JsonResult GetEmployeeLeaveEntitlemntNewAwajExisted(int period, string employeeId)
        {
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
            if (currentPeriod == null)
            {
                return Json(new
                {
                    Message = "You are trying to generate for undefined fiscal Year",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            var message = "";
            #region Validation
            if (newAwajInitialYear == null)
            {
                 message = "Please Define Setting for Initial year first";
                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
            if (initialGlFiscalYear ==null)
            {
                message = "You have to  set  initial year first or Increment will Happen";
                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            
            var validation = Validation_For_Entitlement_Generation();
            if (!string.IsNullOrEmpty(validation))
            {
                return Json(new
                {
                    Message = validation,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
            int leaveThresholdValue = 0;
            if(leaveThrashold != null)
            {

                leaveThresholdValue = Convert.ToInt32(leaveThrashold.CurrentValue);

            }
            int maximumValue = 0;
            int maximumForLeader = 0;
            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null)
            {
                maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
            }
            else
            {

                message = "You have to set MAXIMUM_LEAVE_ENTITLEMENT first";
                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var maxLeader = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
            if (maxLeader != null)
            {
                maximumForLeader = Convert.ToInt32(maxLeader.CurrentValue);
            }
            else
            {
                message = "You have to set MAXIMUM_Leave_For_Leaders setting first";
                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            #endregion


            decimal amount = 0;
            //DateTime PrevYearDateFrom = currentPeriod.DateFrom.AddYears(-1);
            //DateTime PrevYearDateTo = currentPeriod.DateTo.AddYears(-1);
            var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom< currentPeriod.DateFrom).Max(x=>x.DateFrom);
            var previousYear = db.GLFiscalYears.Where(x => x.DateFrom==maxPrevYearFrom).FirstOrDefault();
            if (previousYear == null)
            {
                return Json(new
                {
                    Message = "You can only generate leave entitlement if it has already been generated for the previous fiscal year.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == previousYear.GlFiscalYearId).AsNoTracking().ToList();
            var activeplacements = db.Placements.Where(x=>x.isCurrent).ToList();
            if(!string.IsNullOrEmpty(employeeId)&&int.TryParse(employeeId,out int empId) && empId != 0)
            {
                activeplacements = activeplacements.Where(x => x.EmployeeId == empId).ToList();
                prevEntitlements = prevEntitlements.Where(x => x.EmployeeId == empId).ToList();
            }
            foreach (var empPlacement in activeplacements)
            {
                var profile = db.Person_Employee.Find(empPlacement.EmployeeId); 
                var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                var currentEntitle = db.LeavesEntitle
                                                .FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId 
                                                                    && x.EmployeeId == empPlacement.EmployeeId
                                                                    &&!x.IsExpired);
                var icreementSetting = db.Configurations
                    .Where(x => x.Attribute == LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString())
                    .FirstOrDefault()?.CurrentValue; ;
                int.TryParse(icreementSetting,out int perYear);
                var yearIds = db.GLFiscalYears.OrderByDescending(x => x.GlFiscalYearId).Take(3).Select(x => x.GlFiscalYearId);
                var allLeaveEntitlements = db.LeavesEntitle.Where(x => yearIds.Contains(x.GlFiscalYearId));
                var isLeader = empPlacement.ResponsibilityLevel != ResponsibilityLevel.STAFF;
                var entitle = new LeaveEntitle();
                if (currentEntitle == null)
                {

                    if (prevEntitlement != null)
                    {
                        //because the increment can be per 2 year or three year
                        amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId) ? prevEntitlement.Amount + 1 : prevEntitlement.Amount;
                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                        entitle.EmployeeId = empPlacement.EmployeeId;
                        entitle.EmployeeCode = empPlacement.EmployeeCode;
                        entitle.BranchId = empPlacement.BranchId;
                        entitle.OrgBranchName = empPlacement.BranchId.ToString();
                        entitle.Amount = isLeader? Math.Min(amount, maximumForLeader)
                                                : Math.Min(amount, maximumValue);

                        entitle.Balance = isLeader ? Math.Min(amount, maximumForLeader)
                                                : Math.Min(amount, maximumValue);
                      
                        entitle.IsAdded = true;
                        entitle.IsExpired = false;
                        entitle.DateCreated = DateTime.Now;
                        entitle.LastModified = null;
                        entitle.CreatedBy = User.Identity.Name;
                        entitle.ModifiedBy = null;
                        db.LeavesEntitle.Add(entitle);
                        db.SaveChanges();
                    }
                }
                else if (currentEntitle != null && prevEntitlement != null)
                {
                    //because the increment can be per 2 year or three year
                    amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId) ? prevEntitlement.Amount + 1 : prevEntitlement.Amount;
                    //initial changes
                    currentEntitle.Amount = isLeader ? Math.Min(amount, maximumForLeader)
                                                : Math.Min(amount, maximumValue);
                    currentEntitle.Balance = isLeader ? Math.Min(amount, maximumForLeader)
                                                : Math.Min(amount, maximumValue);
                    db.Entry(currentEntitle).State = EntityState.Modified;

                    db.SaveChanges();
                }

            }
                
               
            
            #region operation log

            UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for fiscal Year of " + currentPeriod?.Name + "  is Generated By  " + User.Identity.Name, Modules.HRM);

            #endregion
            message = "Leave Successfully Generated";
            return Json(new
            {
                Message = message,
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeLeaveEntitlemntNewAwajCopy(bool check, int period)
        {
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
            var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();

            #region Validation
            if (newAwajInitialYear == null)
                return Json(new
                {
                    Message = "Please Select Period First!!",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
            if (initialGlFiscalYear == null)
                return Json(new
                {
                    Message = "You set an invalid initial year",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            var validationMessage = Validation_For_Entitlement_Generation();
            if (!string.IsNullOrEmpty(validationMessage))
                return Json(new
                {
                    Message = validationMessage,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            var leaveThreshold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

            // Fetch Maximum Leave Entitlement and Maximum for Leaders
            var maxLeaveEntitlementConfig = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maxLeaveEntitlementConfig == null)
                return Json(new
                {
                    Message = "You have to set MAXIMUM_LEAVE_ENTITLEMENT first",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            int maximumValue = Convert.ToInt32(maxLeaveEntitlementConfig.CurrentValue);

            var maxLeaveForLeaderConfig = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
            if (maxLeaveForLeaderConfig == null)
                return Json(new
                {
                    Message = "You have to set MAXIMUM_Leave_For_Leaders setting first",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            int maximumForLeader = Convert.ToInt32(maxLeaveForLeaderConfig.CurrentValue);

            var employeeList = db.Person_Employee.Where(x => x.IsActive).AsNoTracking().ToList();
            #endregion

            if (currentPeriod == null)
                return Json(new
                {
                    Message = "You are trying to generate for undefined Period",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);

            var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom < currentPeriod.DateFrom).Max(x => x.DateFrom);
            var prevousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);

            if (prevousYear != null)
            {
                var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == prevousYear.GlFiscalYearId).AsNoTracking().ToList();
                var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

                foreach (var empPlacement in activePlacements)
                {
                    var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                    var currentEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == empPlacement.EmployeeId);

                    var entitle = currentEntitle ?? new LeaveEntitle()
                    {
                        GlFiscalYearId = currentPeriod.GlFiscalYearId,
                        EmployeeId = empPlacement.EmployeeId,
                        EmployeeCode = empPlacement.EmployeeCode,
                        BranchId = empPlacement.BranchId,
                        OrgBranchName = empPlacement.BranchId.ToString(),
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        IsAdded = true,
                        LastModified = null,
                        ModifiedBy = null
                    };

                    if (prevEntitlement != null)
                    {
                        entitle.Amount = prevEntitlement.Amount + 1;
                        entitle.Balance = prevEntitlement.Amount + 1;
                    }
                    else
                    {
                        double currentExperience = GetCurrentExperienceLeaveDaysNew(empPlacement.EmployeeId, currentPeriod, initialGlFiscalYear);
                        double externalExperience = check ? GetExternalExperienceLeaveDays(empPlacement.EmployeeId) : 0;
                        double leaveBaseLine = employeeList.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId) is var emp && emp != null
                            ? GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear)
                            : empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue;

                        decimal amount = Convert.ToDecimal(currentExperience + externalExperience + leaveBaseLine);

                        entitle.Amount = Math.Max(0, Math.Min(empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue, amount));
                        entitle.Balance = entitle.Amount;
                    }

                    if (currentEntitle == null)
                    {
                        db.LeavesEntitle.Add(entitle);
                    }
                    else
                    {
                        db.Entry(currentEntitle).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, $"Leave Entitle for {entitle.EmployeeCode} is Created By {User.Identity.Name}", Modules.HRM);
                    #endregion
                }
            }

            return Json(new
            {
                Message = "Leave Successfully Generated",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeLeaveEntitlementNewAwaj(int period, string employeeId)
        {

            // 1. Validate current fiscal year
            var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
            if (currentPeriod == null)
            {
                return Json(new
                {
                    Message = "You are trying to generate entitlement for an undefined fiscal year.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 2. Validate initial Awaj settings
            var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (newAwajInitialYear == null)
            {
                return Json(new
                {
                    Message = "Please define the initial year settings first.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var initialGlFiscalYear = db.GLFiscalYears
                .FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
            if (initialGlFiscalYear == null)
            {
                return Json(new
                {
                    Message = "You must set the initial year before entitlement generation can proceed.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 3. Custom validation hook
            //var validation = Validation_For_Entitlement_Generation();
            //if (!string.IsNullOrEmpty(validation))
            //{
            //    return Json(new
            //    {
            //        Message = validation,
            //        Status = "NOT_OK"
            //    }, JsonRequestBehavior.AllowGet);
            //}
            #region validation leave Entitlement
            var forleaders0ld = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
            if (forleaders0ld == null || forleaders0ld.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders old initial value";
                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var forperformerold = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
            if (forperformerold == null || forperformerold.CurrentValue == null)
            {
                var message = "you have to add Leave for performers old initial value";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var forleadersnew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
            if (forleadersnew == null || forleadersnew.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders new initial value";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var forperformernew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
            if (forperformernew == null || forperformernew.CurrentValue == null)
            {
                var message = "you have to add Leave for performers new initial value";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            var thresholdLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());
            if (thresholdLeave == null || thresholdLeave.CurrentValue == null)
            {
                var message = $"you have to add Leave {LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString().ToLower()} new initial value";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
            if (frequency == null)
            {
                var message = "you have to add frequency in HRM Setting";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
           
            else if (string.IsNullOrEmpty(frequency.CurrentValue))
            {
                var message = "add current value in frequency first";

                return Json(new
                {
                    Message = message,
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            #endregion
            // 4. Load configuration values
            // Get MAXIMUM_LEAVE_ENTITLEMENT
            var maxLeaveConfig = db.Configurations
                .FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());

            int maximumValue = 0;
            int LeaveTrusholdValue = 0;
            int leaveTresholdForLeaderdrsNew = 0;
            if (maxLeaveConfig == null)
            {
                return Json(new
                {
                    Message = "You must set MAXIMUM_LEAVE_ENTITLEMENT first..",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int.TryParse(maxLeaveConfig.CurrentValue, out maximumValue);
                int.TryParse(thresholdLeave.CurrentValue, out LeaveTrusholdValue);
                int.TryParse(forleadersnew.CurrentValue, out leaveTresholdForLeaderdrsNew);
            }

            // Get MAXIMUM_Leave_For_Leaders
            var maxLeaderConfig = db.Configurations
                .FirstOrDefault(x => x.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());

            int maximumForLeader = 0;
            if (maxLeaderConfig == null)
            {
                return Json(new
                {
                    Message = "You must set MAXIMUM_Leave_For_Leaders first..",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                int.TryParse(maxLeaderConfig.CurrentValue, out maximumForLeader);
            }
            int perYearIncrement = Convert.ToInt32(
                db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLEMENT_INCREEMENT_YEAR.ToString())
                ?.CurrentValue ?? "0");

            // 5. Find previous fiscal year
            var maxPrevYearFrom = db.GLFiscalYears
                .Where(x => x.DateFrom < currentPeriod.DateFrom)
                .Max(x => (DateTime?)x.DateFrom);

            var previousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);
            if (previousYear == null)
            {
                return Json(new
                {
                    Message = "You can only generate leave entitlement if it has already been generated for the previous fiscal year.",
                    Status = "NOT_OK"
                }, JsonRequestBehavior.AllowGet);
            }

            // 6. Load data
            var AllEntitlements = db.LeavesEntitle
                .AsNoTracking()
                .ToList();
             var prevEntitlements = AllEntitlements
                .Where(x => x.GlFiscalYearId == previousYear.GlFiscalYearId)
                .ToList();

            var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

            if (!string.IsNullOrEmpty(employeeId) && int.TryParse(employeeId, out int empId) && empId != 0)
            {
                activePlacements = activePlacements.Where(x => x.EmployeeId == empId).ToList();
                prevEntitlements = prevEntitlements.Where(x => x.EmployeeId == empId).ToList();
            }

            // 7. Process entitlements
            foreach (var empPlacement in activePlacements)
            {
                var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                var currentEntitlement = AllEntitlements
                                           .FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId
                                                             && x.EmployeeId == empPlacement.EmployeeId
                                                             /*&& !x.IsExpired*/);

                bool isLeader = empPlacement.ResponsibilityLevel != ResponsibilityLevel.STAFF;

                //if (prevEntitlement == null) continue;

           
                if (prevEntitlement != null)
                {
                    // Calculate entitlement amount
                    decimal amount = (currentPeriod.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId)
                        ? prevEntitlement.Amount + 1
                        : prevEntitlement.Amount;

                    decimal cappedAmount = isLeader
                        ? Math.Min(amount, maximumForLeader)
                        : Math.Min(amount, maximumValue);
                    if (currentEntitlement == null)
                    {
                        
                        // Insert new entitlement
                        var entitle = new LeaveEntitle
                        {
                            GlFiscalYearId = currentPeriod.GlFiscalYearId,
                            EmployeeId = empPlacement.EmployeeId,
                            EmployeeCode = empPlacement.EmployeeCode,
                            BranchId = empPlacement.BranchId,
                            OrgBranchName = empPlacement.BranchId.ToString(),
                            Amount = cappedAmount,
                            Balance = cappedAmount,
                            IsAdded = true,
                            IsExpired = false,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };
                        db.LeavesEntitle.Add(entitle);
                    }
                    else
                    {
                        currentEntitlement.IsAdded = true;
                        currentEntitlement.IsExpired = false;
                        currentEntitlement.Amount = cappedAmount;
                        currentEntitlement.Balance = cappedAmount;
                        currentEntitlement.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                        currentEntitlement.GlFiscalYear = null; // 👈 prevent double attach
                        currentEntitlement.Employee = null; // 👈 prevent double attach
                        currentEntitlement.LastModified = DateTime.Now;
                        currentEntitlement.ModifiedBy = User.Identity.Name;

                        db.Entry(currentEntitlement).State = EntityState.Modified;
                    }
                    db.SaveChanges();


                }
                else
                {
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
                    if (person == null)
                        continue;
                    var hireDate = person.HiredDate;
                    decimal newEntitle = isLeader ? leaveTresholdForLeaderdrsNew : LeaveTrusholdValue;
                    decimal newAmount = newEntitle; // default full entitlement
                    if (hireDate.Date > currentPeriod.DateTo.Date)
                        continue;
                    if (hireDate.Date > currentPeriod.DateFrom.Date && hireDate.Date < currentPeriod.DateTo)
                    {
                        // total days in the current period
                        var totalPeriodDays = (currentPeriod.DateTo.Date - currentPeriod.DateFrom.Date).TotalDays;

                        // remaining days from hire date until period end
                        var eligibleDays = (currentPeriod.DateTo.Date - hireDate.Date).TotalDays;

                        // pro-rate leave based on remaining fraction of the period
                        var proratedFraction = (decimal)(eligibleDays / totalPeriodDays);

                        newAmount = Math.Round(newEntitle * proratedFraction, 2);
                    }
                    if (currentEntitlement != null)
                    {
                        // Update existing entitlement
                        currentEntitlement.IsAdded = true;
                        currentEntitlement.IsExpired = false;
                        currentEntitlement.Amount = newEntitle;
                        currentEntitlement.Balance = newAmount;
                        currentEntitlement.GlFiscalYearId = currentPeriod.GlFiscalYearId;
                        currentEntitlement.GlFiscalYear = null; // 👈 prevent double attach
                        currentEntitlement.Employee = null; // 👈 prevent double attach
                        currentEntitlement.LastModified = DateTime.Now;
                        currentEntitlement.ModifiedBy = User.Identity.Name;
                        db.Entry(currentEntitlement).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else
                    {
                       
                        // else if hired before the period → keep full entitlement

                        var entitle = new LeaveEntitle
                        {
                            GlFiscalYearId = currentPeriod.GlFiscalYearId,
                            EmployeeId = empPlacement.EmployeeId,
                            EmployeeCode = empPlacement.EmployeeCode,
                            BranchId = empPlacement.BranchId,
                            OrgBranchName = empPlacement.BranchId.ToString(),
                            Amount = newEntitle,
                            Balance = newAmount,
                            IsAdded = true,
                            IsExpired = false,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name
                        };

                        db.LeavesEntitle.Add(entitle);
                        db.SaveChanges();

                    }

                }




            }
            //db.SaveChanges();

            // 8. Log operation
            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Leave Entitlement",
                "Success",
                User.Identity.Name,
                $"Leave entitlement for fiscal year {currentPeriod?.Name} was generated by {User.Identity.Name}",
                Modules.HRM);

            return Json(new
            {
                Message = "Leave entitlement successfully generated.",
                Status = "OK"
            }, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEmployeeLeaveEntitlemntNewAwajCopy(bool check, int period)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);
        //    var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();

        //    #region Validation
        //    if (newAwajInitialYear == null)
        //        return Json(new
        //        {
        //            Message = "Please Select Period First!!",
        //            Status = "NOT_OK"
        //        },JsonRequestBehavior.AllowGet);
        //        //return Json(Script = "alert('Please Define Setting for Initial year first');");

        //    var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //    if (initialGlFiscalYear == null)
        //        return new JavaScriptResult() { Script = "alert('You set an invalid initial year');" };

        //    var validationMessage = Validation_For_Entitlement_Generation();
        //    if (!string.IsNullOrEmpty(validationMessage))
        //        return new JavaScriptResult() { Script = $"alert('{validationMessage}');" };

        //    var leaveThreshold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //    // Fetch Maximum Leave Entitlement and Maximum for Leaders
        //    int maximumValue = Convert.ToInt32(db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString())?.CurrentValue
        //        ?? throw new Exception("You have to set MAXIMUM_LEAVE_ENTITLEMENT first"));

        //    int maximumForLeader = Convert.ToInt32(db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString())?.CurrentValue
        //        ?? throw new Exception("You have to set MAXIMUM_Leave_For_Leaders setting first"));

        //    var employeeList = db.Person_Employee.Where(x => x.IsActive).AsNoTracking().ToList();
        //    #endregion

        //    if (currentPeriod == null)
        //        return new JavaScriptResult() { Script = "alert('You are trying to generate for undefined Period');" };

        //    var maxPrevYearFrom = db.GLFiscalYears.Where(x => x.DateFrom < currentPeriod.DateFrom).Max(x => x.DateFrom);
        //    var prevousYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == maxPrevYearFrom);

        //    if (prevousYear != null)
        //    {
        //        var prevEntitlements = db.LeavesEntitle.Where(x => x.GlFiscalYearId == prevousYear.GlFiscalYearId).AsNoTracking().ToList();
        //        var activePlacements = db.Placements.Where(x => x.isCurrent).ToList();

        //        foreach (var empPlacement in activePlacements)
        //        {
        //            var prevEntitlement = prevEntitlements.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId);
        //            var currentEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == empPlacement.EmployeeId);

        //            var entitle = currentEntitle ?? new LeaveEntitle()
        //            {
        //                GlFiscalYearId = currentPeriod.GlFiscalYearId,
        //                EmployeeId = empPlacement.EmployeeId,
        //                EmployeeCode = empPlacement.EmployeeCode,
        //                BranchId = empPlacement.BranchId,
        //                OrgBranchName = empPlacement.BranchId.ToString(),
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name,
        //                IsAdded = true,
        //                LastModified = null,
        //                ModifiedBy = null
        //            };

        //            if (prevEntitlement != null)
        //            {
        //                entitle.Amount = prevEntitlement.Amount + 1;
        //                entitle.Balance = prevEntitlement.Amount + 1;
        //            }
        //            else
        //            {
        //                double currentExperience = GetCurrentExperienceLeaveDaysNew(empPlacement.EmployeeId, currentPeriod, initialGlFiscalYear);
        //                double externalExperience = check ? GetExternalExperienceLeaveDays(empPlacement.EmployeeId) : 0;
        //                double leaveBaseLine = employeeList.FirstOrDefault(x => x.EmployeeId == empPlacement.EmployeeId) is var emp && emp != null
        //                    ? GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear)
        //                    : empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue;

        //                decimal amount = Convert.ToDecimal(currentExperience + externalExperience + leaveBaseLine);

        //                entitle.Amount = Math.Max(0, Math.Min(empPlacement.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT ? maximumForLeader : maximumValue, amount));
        //                entitle.Balance = entitle.Amount;
        //            }

        //            if (currentEntitle == null)
        //            {
        //                db.LeavesEntitle.Add(entitle);
        //            }
        //            else
        //            {
        //                db.Entry(currentEntitle).State = EntityState.Modified;
        //            }
        //            db.SaveChanges();

        //            #region operation log
        //            UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, $"Leave Entitle for {entitle.EmployeeCode} is Created By {User.Identity.Name}", Modules.HRM);
        //            #endregion
        //        }
        //    }

        //    return new JavaScriptResult() { Script = "alert('Leave Successfully Generated');" };
        //}

        //public JavaScriptResult GetEmployeeLeaveEntitlemntNewAwaj(bool check, int period)
        //{
        //    var currentPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == period);

        //    if (currentPeriod != null)
        //    {
        //        var newAwajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
        //        if (newAwajInitialYear != null)
        //        {
        //            var initialGlFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == newAwajInitialYear.GlFisicalYearId);
        //            if (initialGlFiscalYear != null)
        //            {
        //                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

        //                int maximumValue = 0;
        //                int maximumForLeader = 0;
        //                var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
        //                if (maximumLeave != null)
        //                {
        //                    maximumValue = Convert.ToInt32(maximumLeave.CurrentValue);
        //                }

        //                var maxLeader = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_Leave_For_Leaders.ToString());
        //                if (maxLeader != null)
        //                {
        //                    maximumForLeader = Convert.ToInt32(maxLeader.CurrentValue);
        //                }

        //                var employeeList = db.Person_Employee.Where(x => x.IsActive).ToList();

        //                #region Validation

        //                var validation = Validation_For_Entitlement_Generation();

        //                if (!string.IsNullOrEmpty(validation))
        //                {
        //                    return new JavaScriptResult() { Script = "alert('" + validation + "');" };
        //                }

        //                #endregion

        //                foreach (var emp in employeeList)
        //                {
        //                    var leaveEntitle = db.LeavesEntitle.FirstOrDefault(x => x.GlFiscalYearId == currentPeriod.GlFiscalYearId && x.EmployeeId == emp.EmployeeId);

        //                    if (leaveEntitle != null)
        //                    {
        //                        decimal amount = 0;

        //                        if (!leaveEntitle.IsAdded)
        //                        {
        //                            double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                            double externalExperience = 0;
        //                            if (check)
        //                            {
        //                                externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                            }

        //                            double totalExpriance = currentExpriance + externalExperience;

        //                            double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                            amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);
        //                        }
        //                        else if (leaveEntitle.IsAdded)
        //                        {
        //                            amount = leaveEntitle.Amount;
        //                        }

        //                        leaveEntitle.EmployeeId = emp.EmployeeId;
        //                        leaveEntitle.EmployeeCode = emp.EmployeeCode;

        //                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
        //                        if (placement != null)
        //                        {
        //                            leaveEntitle.BranchId = placement.BranchId;
        //                            leaveEntitle.OrgBranchName = placement.BranchId.ToString();
        //                        }

        //                        decimal leaveTakenAndSubtractedFromThisBudgetYear = GetTakenLeaveInOneBudgetYear(emp.EmployeeId, currentPeriod.GlFiscalYearId);

        //                        leaveEntitle.Amount = amount;
        //                        leaveEntitle.Balance = leaveEntitle.Amount - leaveTakenAndSubtractedFromThisBudgetYear;
        //                        leaveEntitle.IsAdded = true;
        //                        leaveEntitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                        leaveEntitle.LastModified = DateTime.Now;
        //                        leaveEntitle.ModifiedBy = User.Identity.Name;

        //                        if (Convert.ToDecimal(maximumValue) < leaveEntitle.Amount)
        //                        {
        //                            leaveEntitle.Amount = maximumValue;
        //                            leaveEntitle.Balance = leaveEntitle.Amount - leaveTakenAndSubtractedFromThisBudgetYear;
        //                        }
        //                        if (leaveEntitle.Amount < 0)
        //                        {
        //                            leaveEntitle.Amount = 0;
        //                            leaveEntitle.Balance = 0;
        //                        }

        //                        db.Entry(leaveEntitle).State = EntityState.Modified;
        //                        db.SaveChanges();
        //                        #region operation log

        //                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + leaveEntitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                        #endregion

        //                    }

        //                    else if (leaveEntitle == null)
        //                    {
        //                        var entitle = new LeaveEntitle();

        //                        bool IsHireDateBetweenCurrentPeriod = false;
        //                        if (emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
        //                        {
        //                            IsHireDateBetweenCurrentPeriod = true;
        //                        }

        //                        if (emp.HiredDate <= currentPeriod.DateFrom || IsHireDateBetweenCurrentPeriod)
        //                        {
        //                            var preveDateFrom = currentPeriod.DateFrom.AddYears(-1);
        //                            var prevDateTo = currentPeriod.DateTo.AddYears(-1);

        //                            var prevYear = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom == preveDateFrom && x.DateTo == prevDateTo);
        //                            if (prevYear != null)
        //                            {
        //                                var leaveEntitlePrev = db.LeavesEntitle.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.GlFiscalYearId == prevYear.GlFiscalYearId);

        //                                var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());

        //                                bool CheckPreviosLeaveAndFrequencyValueIsOne = false;
        //                                if (leaveEntitlePrev != null)
        //                                {
        //                                    if (frequency != null && Convert.ToInt32(frequency.CurrentValue) == 1)
        //                                    {
        //                                        CheckPreviosLeaveAndFrequencyValueIsOne = true;
        //                                    }
        //                                }

        //                                if (leaveEntitlePrev == null || CheckPreviosLeaveAndFrequencyValueIsOne)
        //                                {
        //                                    double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                    double externalExperience = 0;
        //                                    if (check)
        //                                    {
        //                                        externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                    }

        //                                    double totalExpriance = currentExpriance + externalExperience;

        //                                    entitle.EmployeeId = emp.EmployeeId;
        //                                    entitle.EmployeeCode = emp.EmployeeCode;

        //                                    double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                    decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                    ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                    if (placement != null)
        //                                    {
        //                                        entitle.BranchId = placement.BranchId;
        //                                        entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        res = placement.ResponsibilityLevel;
        //                                    }
        //                                    entitle.Amount = amount;
        //                                    entitle.Balance = amount;

        //                                    entitle.IsAdded = true;
        //                                    entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                    entitle.DateCreated = DateTime.Now;
        //                                    entitle.LastModified = null;
        //                                    entitle.CreatedBy = User.Identity.Name;
        //                                    entitle.ModifiedBy = null;

        //                                    if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                    {
        //                                        entitle.Amount = maximumForLeader;
        //                                        entitle.Balance = maximumForLeader;
        //                                    }
        //                                    else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                    {
        //                                        entitle.Amount = maximumValue;
        //                                        entitle.Balance = maximumValue;
        //                                    }

        //                                    if (entitle.Amount < 0)
        //                                    {
        //                                        entitle.Amount = 0;
        //                                        entitle.Balance = 0;
        //                                    }
        //                                    db.LeavesEntitle.Add(entitle);
        //                                    db.SaveChanges();
        //                                    #region operation log

        //                                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                    #endregion

        //                                }
        //                                else
        //                                {
        //                                    if (leaveEntitlePrev.IsAdded)
        //                                    {
        //                                        decimal defaultLeave = Convert.ToDecimal(GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear));

        //                                        if (leaveEntitlePrev.Amount < defaultLeave)
        //                                        {
        //                                            entitle.Amount = defaultLeave;
        //                                            entitle.Balance = defaultLeave;
        //                                        }
        //                                        else
        //                                        {
        //                                            entitle.Amount = leaveEntitlePrev.Amount;
        //                                            entitle.Balance = leaveEntitlePrev.Amount;
        //                                        }

        //                                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                        if (placement != null)
        //                                        {
        //                                            entitle.BranchId = placement.BranchId;
        //                                            entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        }

        //                                        entitle.EmployeeId = emp.EmployeeId;
        //                                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                        entitle.IsAdded = false;
        //                                        entitle.DateCreated = DateTime.Now;
        //                                        entitle.LastModified = null;
        //                                        entitle.CreatedBy = User.Identity.Name;
        //                                        entitle.ModifiedBy = null;

        //                                        db.LeavesEntitle.Add(entitle);
        //                                        db.SaveChanges();
        //                                        #region operation log

        //                                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                        #endregion
        //                                    }
        //                                    else if (!leaveEntitlePrev.IsAdded)
        //                                    {
        //                                        double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                        double externalExperience = 0;
        //                                        if (check)
        //                                        {
        //                                            externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                        }

        //                                        entitle = new LeaveEntitle();

        //                                        entitle.EmployeeId = emp.EmployeeId;
        //                                        entitle.EmployeeCode = emp.EmployeeCode;

        //                                        ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                        var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                        if (placement != null)
        //                                        {
        //                                            entitle.BranchId = placement.BranchId;
        //                                            entitle.OrgBranchName = placement.BranchId.ToString();
        //                                            res = placement.ResponsibilityLevel;
        //                                        }

        //                                        double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                        decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                        // we add 1 to the previous entitle amount
        //                                        //decimal amount = leaveEntitlePrevNotAdded.Amount + 1;

        //                                        // the reason i substrac 1 from amount is because the when we add the experience the year adds 2 day
        //                                        entitle.Amount = amount - 1;
        //                                        entitle.Balance = amount - 1;
        //                                        entitle.IsAdded = true;
        //                                        entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                        entitle.DateCreated = DateTime.Now;
        //                                        entitle.LastModified = null;
        //                                        entitle.CreatedBy = User.Identity.Name;
        //                                        entitle.ModifiedBy = null;

        //                                        if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                        {
        //                                            entitle.Amount = maximumForLeader;
        //                                            entitle.Balance = maximumForLeader;
        //                                        }
        //                                        else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                        {
        //                                            entitle.Amount = maximumValue;
        //                                            entitle.Balance = maximumValue;
        //                                        }
        //                                        if (entitle.Amount < 0)
        //                                        {
        //                                            entitle.Amount = 0;
        //                                            entitle.Balance = 0;
        //                                        }

        //                                        db.LeavesEntitle.Add(entitle);
        //                                        db.SaveChanges();
        //                                        #region operation log

        //                                        UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                        #endregion
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                // check if there is only one budget year defined 
        //                                // this happens the system starts for the first time for the company 
        //                                var noOfPeriodDefined = db.GLFiscalYears.ToList();
        //                                var mindate = noOfPeriodDefined.Select(x => x.DateFrom).Min();

        //                                //if (noOfPeriodDefined.Count() > 1 && currentPeriod.DateFrom > mindate)
        //                                //{
        //                                //    string Message = "Check you budget year from and to value";
        //                                //    return new JavaScriptResult() { Script = "alert('" + Message + "');" };
        //                                //}
        //                                //else
        //                                //{
        //                                    double currentExpriance = GetCurrentExperienceLeaveDaysNew(emp.EmployeeId, currentPeriod, initialGlFiscalYear);

        //                                    double externalExperience = 0;
        //                                    if (check)
        //                                    {
        //                                        externalExperience = GetExternalExperienceLeaveDays(emp.EmployeeId);
        //                                    }

        //                                    entitle = new LeaveEntitle();

        //                                    entitle.EmployeeId = emp.EmployeeId;
        //                                    entitle.EmployeeCode = emp.EmployeeCode;

        //                                    ResponsibilityLevel res = ResponsibilityLevel.STAFF;
        //                                    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == entitle.EmployeeId && x.isCurrent);
        //                                    if (placement != null)
        //                                    {
        //                                        entitle.BranchId = placement.BranchId;
        //                                        entitle.OrgBranchName = placement.BranchId.ToString();
        //                                        res = placement.ResponsibilityLevel;
        //                                    }

        //                                    double LeaveBaseLine = GetInitialLeaveValue(emp, currentPeriod, initialGlFiscalYear);

        //                                    decimal amount = Convert.ToDecimal(currentExpriance + externalExperience + LeaveBaseLine);

        //                                    // we add 1 to the previous entitle amount
        //                                    //decimal amount = leaveEntitlePrevNotAdded.Amount + 1;

        //                                    entitle.Amount = amount - 1;
        //                                    entitle.Balance = amount - 1;
        //                                    entitle.IsAdded = true;
        //                                    entitle.GlFiscalYearId = currentPeriod.GlFiscalYearId;
        //                                    entitle.DateCreated = DateTime.Now;
        //                                    entitle.LastModified = null;
        //                                    entitle.CreatedBy = User.Identity.Name;
        //                                    entitle.ModifiedBy = null;

        //                                    if (Convert.ToDecimal(maximumValue) < entitle.Amount && res == ResponsibilityLevel.TOP_MANAGMENT)
        //                                    {
        //                                        entitle.Amount = maximumForLeader;
        //                                        entitle.Balance = maximumForLeader;
        //                                    }
        //                                    else if (Convert.ToDecimal(maximumValue) < entitle.Amount)
        //                                    {
        //                                        entitle.Amount = maximumValue;
        //                                        entitle.Balance = maximumValue;
        //                                    }
        //                                    if (entitle.Amount < 0)
        //                                    {
        //                                        entitle.Amount = 0;
        //                                        entitle.Balance = 0;
        //                                    }

        //                                    db.LeavesEntitle.Add(entitle);
        //                                    db.SaveChanges();
        //                                    #region operation log

        //                                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entitle.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

        //                                    #endregion
        //                               // }
        //                            }
        //                        }

        //                    }

        //                }
        //            }
        //            else
        //            {
        //                var messagesInitial = "You set invalid initial year";

        //                return new JavaScriptResult() { Script = "alert('" + messagesInitial + "');" };
        //            }
        //        }
        //        else
        //        {
        //            var messagesInitialYear = "Please define setting for initial year first";

        //            return new JavaScriptResult() { Script = "alert('" + messagesInitialYear + "');" };
        //        }
        //    }
        //    else
        //    {
        //        var messagesPeriod = "you are trying to generate for undefined Period";

        //        return new JavaScriptResult() { Script = "alert('" + messagesPeriod + "');" };
        //    }

        //    var messages = "Leave Successfully Generated";

        //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
        //}
        #endregion
        #region Annual Leave Generating Functions
        public decimal GetTakenLeaveInOneBudgetYear(int empId, int yearId)
        {
            var leaveUsages = db.LeaveUsageTrackingForVoids.Where(x => x.EmployeeId == empId && x.GlFisicalYearId == yearId).ToList();
            return leaveUsages.Sum(x => x.Amount);
        }
        public double GetInitialLeaveValue(Person_Employee emp, GlFiscalYear currentPeriod, GlFiscalYear initialGlFiscalYear)
        {
            double LeaveBaseLine = 0;

            double days = (currentPeriod.DateTo - emp.HiredDate).Days;
            var totalYears = Math.Truncate(days / 365);
            var totalMonths = Math.Truncate((days % 365) / 30);
            var remainingDays = Math.Truncate((days % 365) % 30);

            var role = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
            if (role != null)
            {
                if (emp.HiredDate < initialGlFiscalYear.DateFrom)
                {
                   // if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                    if (role.IsHead)
                    {
                        var leaders = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
                        if (leaders != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(leaders.CurrentValue);
                        }
                    }
                    else
                    {
                        var performers = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
                        if (performers != null)
                        {
                            LeaveBaseLine = Convert.ToInt32(performers.CurrentValue);
                        }
                    }
                    if (totalYears == 0 && emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
                    {
                        LeaveBaseLine = totalMonths / 12 * LeaveBaseLine;
                    }

                }
                else if (emp.HiredDate >= initialGlFiscalYear.DateFrom)
                {
                    var baseLine = 0;

                   // if (role.ResponsibilityLevel == ResponsibilityLevel.TOP_MANAGMENT)
                    if (role.IsHead)
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
                    if (totalYears == 0 && emp.HiredDate >= currentPeriod.DateFrom && emp.HiredDate <= currentPeriod.DateTo)
                    {
                        LeaveBaseLine = totalMonths / 12 * baseLine;
                    }
                }
            }

            return LeaveBaseLine;
        }
        public string Validation_For_Entitlement_Generation()
        {
            var forleaders0ld = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_OLD.ToString());
            if (forleaders0ld == null || forleaders0ld.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders old initial value";

                return message;
            }

            var forperformerold = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_OLD.ToString());
            if (forperformerold == null || forperformerold.CurrentValue == null)
            {
                var message = "you have to add Leave for performers old initial value";

                return message;
            }

            var forleadersnew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_LEADRES_NEW.ToString());
            if (forleadersnew == null || forleadersnew.CurrentValue == null)
            {
                var message = "you have to add Leave for leaders new initial value";

                return message;
            }

            var forperformernew = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.LEAVE_FOR_PERFORMERS_NEW.ToString());
            if (forperformernew == null || forperformernew.CurrentValue == null)
            {
                var message = "you have to add Leave for performers new initial value";

                return message;
            }

            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
            if (frequency == null)
            {
                var message = "you have to add frequency in HRM Setting";

                return message;
            }
            else if (string.IsNullOrEmpty(frequency.CurrentValue))
            {
                var message = "add current value in frequency first";

                return message;
            }

            return null;
        }
        #endregion
        public double GetExternalExperienceLeaveDays(int id)
        {
            double totalDays = 0;
            var awajInitialYear = db.LeaveNewAwajStartYears.FirstOrDefault();
            if (awajInitialYear != null)
            {
                var initialPeriod = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == awajInitialYear.GlFisicalYearId);
                if (initialPeriod != null)
                {
                    var expriances = db.Experiences.Where(e => e.EmployeeId == id).ToList();
                    foreach (var exp in expriances)
                    {
                        if (exp.From <= initialPeriod.DateFrom && exp.To <= initialPeriod.DateFrom)
                        {
                            totalDays += (exp.To - exp.From).Days;
                        }
                        else if (exp.From <= initialPeriod.DateFrom && exp.To >= initialPeriod.DateFrom)
                        {
                            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                            if (frequency != null)
                            {
                                double first = (initialPeriod.DateFrom - exp.From).Days;
                                double second = ((exp.To - initialPeriod.DateFrom).Days) / Convert.ToInt32(frequency.CurrentValue);

                                totalDays += first + second;
                            }

                        }
                        else if (exp.From >= initialPeriod.DateFrom)
                        {
                            var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                            if (frequency != null)
                            {
                                totalDays += ((exp.To - exp.From).Days) / Convert.ToInt32(frequency.CurrentValue);
                            }
                        }
                    }
                }
            }

            var totalYears = Math.Truncate(totalDays / 365);
            var totalMonths = Math.Truncate((totalDays % 365) / 30);
            var remainingDays = Math.Truncate((totalDays % 365) % 30);

            return totalYears;
        }
        public double GetCurrentExperienceLeaveDaysOld(int id, GlFiscalYear currentPeriod, GlFiscalYear intialYear)
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
                    totalDays = (currentPeriod.DateTo - person.HiredDate).Days;
                }
                if (currentPeriod.DateFrom >= intialYear.DateFrom && hireDate <= intialYear.DateFrom)
                {
                    // first means experience before the initial year of awaj
                    first = (intialYear.DateFrom - hireDate).Days;

                    // second means experience after the initial year of awaj
                    second = (currentPeriod.DateTo - intialYear.DateFrom).Days;

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
                    var expYear = (currentPeriod.DateTo - hireDate).Days;

                    var frequency = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Reference == "Leave" && x.Attribute == LeaveHrmSetting.Frequency.ToString());
                    if (frequency != null)
                    {
                        totalDays = expYear / Convert.ToInt32(frequency.CurrentValue);
                    }
                }

                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);

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
        // i wrote this function at 29/07/2021
        public double GetCurrentExperienceLeaveDaysNew(int id, GlFiscalYear currentPeriod, GlFiscalYear intialYear)
        {
            double currentExpriance = 0;
            double totalDays = 0;
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (person != null)
            {
                double first = 0;
                double second = 0;

                var leaveThrashold = db.Configurations.FirstOrDefault(x => x.Attribute == LeaveSetting.LEAVE_ENTITLE_THRESHOLD.ToString());

                var startDate = db.LeaveEntitleStartingDates.FirstOrDefault();
                if (startDate != null)
                {
                    if (startDate.Start == LeaveEntitleStarting.Fiscal_Year_Initial_Date)
                    {
                        if (person.HiredDate <= currentPeriod.DateFrom)
                        {
                            totalDays = (currentPeriod.DateFrom - person.HiredDate).Days;
                        }
                    }
                    else if (startDate.Start == LeaveEntitleStarting.Fiscal_Year_End_Date)
                    {
                        if (person.HiredDate <= currentPeriod.DateTo)
                        {
                            totalDays = (currentPeriod.DateTo - person.HiredDate).Days;
                        }
                    }

                }


                var totalYears = Math.Truncate(totalDays / 365);
                var totalMonths = Math.Truncate((totalDays % 365) / 30);
                var remainingDays = Math.Truncate((totalDays % 365) % 30);

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
        public string GetHiredDate(int id)
        {
            var person =  db.Person_Employee.Find(id);
            if (person == null)
            {
                // Log or handle the case where the employee is not found
                return "Employee not found.";
            }

            return person.HiredDateAmharic ?? "Hired date not available.";
        }
        public decimal GetTotalEntitlement(int id)
        {
            var entitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            if (entitlements.Any())
            {
                return entitlements.Sum(x => x.Amount);
            }
            return 0;
        }
        public decimal GetTotalBalance(int id)
        {
            var entitlements = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired);
            if (entitlements.Any())
            {
                return entitlements.Sum(x => x.Balance);
            }
            return 0;
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser, HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        public ActionResult LeavesEntitle_Read([DataSourceRequest] DataSourceRequest request)
        {
            var employeeIds = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);
            var query = db.LeavesEntitle
                .Where(x => employeeIds.Contains(x.EmployeeId) && !x.IsExpired)
                .OrderBy(o => o.GlFiscalYearId)
                .Select(leaveEntitle => new
                {
                    LeaveEntitleId = leaveEntitle.LeaveEntitleId,
                    EmployeeId = leaveEntitle.EmployeeId,
                    Balance = leaveEntitle.Balance,
                    BranchId = leaveEntitle.Employee.BranchId, // Assuming BranchId is accessible from Employee
                    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                    Amount = leaveEntitle.Amount,
                    Remark = leaveEntitle.Remark,
                    DateCreated = leaveEntitle.DateCreated,
                    LastModified = leaveEntitle.LastModified,
                    CreatedBy = leaveEntitle.CreatedBy,
                    ModifiedBy = leaveEntitle.ModifiedBy,
                    EmployeeCode = leaveEntitle.Employee.EmployeeCode, // Avoid calling helper method
                    TotlalAmount = leaveEntitle.TotlalAmount,
                    OrgBranchName = leaveEntitle.OrgBranchName,
                    ExpiredBy = leaveEntitle.ExpiredBy,
                    ExpiredDate = leaveEntitle.ExpiredDate,
                    ReversedBy = leaveEntitle.ReversedBy,
                    Void = leaveEntitle.Void,
                    ReversedDate = leaveEntitle.ReversedDate,
                    SumOfBalances = db.LeavesEntitle
                        .Where(le => le.EmployeeId == leaveEntitle.EmployeeId && !le.IsExpired)
                        .Sum(le => (decimal?)le.Balance) ?? 0,
                    HireDate = leaveEntitle.Employee.HiredDate
                });

            var result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult LeavesEntitle_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    List<int> employees = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);

        //    var leavesentitle = db.LeavesEntitle.Where(x => employees.Contains(x.EmployeeId) && !x.IsExpired).OrderBy(o => o.GlFiscalYearId);

        //    DataSourceResult result = leavesentitle.ToDataSourceResult(request, leaveEntitle => new
        //    {
        //        LeaveEntitleId = leaveEntitle.LeaveEntitleId,
        //        EmployeeId = leaveEntitle.EmployeeId,
        //        Balance = leaveEntitle.Balance,
        //        BranchId = GetBranch(leaveEntitle.EmployeeId),
        //        GlFiscalYearId = leaveEntitle.GlFiscalYearId,
        //        Amount = leaveEntitle.Amount,
        //        Remark = leaveEntitle.Remark,
        //        DateCreated = leaveEntitle.DateCreated,
        //        LastModified = leaveEntitle.LastModified,
        //        CreatedBy = leaveEntitle.CreatedBy,
        //        ModifiedBy = leaveEntitle.ModifiedBy,
        //        EmployeeCode = GetEmployeeCode(leaveEntitle.EmployeeId),
        //        TotlalAmount = leaveEntitle.TotlalAmount,
        //        OrgBranchName = leaveEntitle.OrgBranchName,
        //        ExpiredBy = leaveEntitle.ExpiredBy,
        //        ExpiredDate = leaveEntitle.ExpiredDate,
        //        ReversedBy = leaveEntitle.ReversedBy,
        //        Void = leaveEntitle.Void,
        //        ReversedDate = leaveEntitle.ReversedDate,
        //        SumOfBalances = GetTotalBalance(leaveEntitle.EmployeeId),
        //        HireDate = leaveEntitle.Employee.HiredDate.ToShortDateString()

        //    });

        //    return Json(result);
        //}
        public ActionResult LeavEntitleEmployee_Read([DataSourceRequest] DataSourceRequest request, string field)
        {
            int.TryParse(field, out int employeeId);
            var result = db.LeavesEntitle.Where(x=>x.EmployeeId==employeeId).Distinct().ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeavesEntitle_New_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            //var employeeIds = GetActiveEmployeeFromCurrentUserBranch(User.Identity.Name);

            var query = db.LeavesEntitle.Where(x => myBranches.Contains(x.BranchId)
                                                    && !x.IsExpired)
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    Balance = g.Sum(x => x.Balance),
                    Amount = g.Sum(x => x.Amount),
                    Latest = g.OrderByDescending(x => x.GlFiscalYearId).FirstOrDefault(),
                    Count = g.Count()
                })
                .Select(x => new
                {
                    EmployeeId=x.EmployeeId,
                    Amount=x.Amount,
                    Balance =x.Balance,
                    //x.TotalAmount,
                    //x.TotalBalance,
                    x.Count,
                    BranchId = x.Latest.Employee.BranchId,
                    GlFiscalYearId = x.Latest.GlFiscalYearId,
                    Remark = x.Latest.Remark,
                    EmployeeCode = x.Latest.Employee.EmployeeCode,
                    DateCreated = x.Latest.DateCreated,
                    LastModified = x.Latest.LastModified,
                    CreatedBy = x.Latest.CreatedBy,
                    ModifiedBy = x.Latest.ModifiedBy,
                    OrgBranchName = x.Latest.OrgBranchName,
                    ExpiredBy = x.Latest.ExpiredBy,
                    ExpiredDate = x.Latest.ExpiredDate,
                    ReversedBy = x.Latest.ReversedBy,
                    Void = x.Latest.Void,
                    ReversedDate = x.Latest.ReversedDate,
                    HireDate = x.Latest.Employee.HiredDateAmharic

                });

            var result = query.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);

            
        }
        public ActionResult LeavesEntitle_OnePerson_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var leavesEntitleQuery = db.LeavesEntitle
        .Where(x => x.EmployeeId == id && x.IsExpired==false)
        .OrderByDescending(x => x.GlFiscalYearId)
        .Select(x => new
        {
            x.LeaveEntitleId,
            x.EmployeeId,
            x.Balance,
            x.GlFiscalYearId,
            x.Amount,
            x.Remark,
            x.DateCreated,
            x.LastModified,
            x.CreatedBy,
            x.ModifiedBy,
            x.TotlalAmount,
            x.OrgBranchName,
            x.ExpiredBy,
            x.ExpiredDate,
            x.ReversedBy,
            x.Void,
            x.ReversedDate,
            HireDate = x.Employee.HiredDateAmharic,
            BranchId = x.Employee.BranchId,
            EmployeeCode = x.Employee.EmployeeCode
        });

            var result = leavesEntitleQuery.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
            //var leavesentitle = db.LeavesEntitle.Where(x => x.EmployeeId == id && !x.IsExpired)/*.Select(x => new { x.LeaveEntitleId, x.EmployeeId, x.Amount, x.Balance, x.GlFiscalYearId, x.Remark })*/.OrderByDescending(x=>x.GlFiscalYearId).ToList();
            //DataSourceResult result = leavesentitle.ToDataSourceResult(request, leaveEntitle => new
            //{
            //    LeaveEntitleId = leaveEntitle.LeaveEntitleId,
            //    EmployeeId = leaveEntitle.EmployeeId,
            //    Balance = leaveEntitle.Balance,
            //    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
            //    Amount = leaveEntitle.Amount,
            //    Remark = leaveEntitle.Remark,
            //    BranchId = GetBranch(leaveEntitle.EmployeeId),
            //    DateCreated = leaveEntitle.DateCreated,
            //    LastModified = leaveEntitle.LastModified,
            //    CreatedBy = leaveEntitle.CreatedBy,
            //    ModifiedBy = leaveEntitle.ModifiedBy,
            //    EmployeeCode = GetEmployeeCode(leaveEntitle.EmployeeId),
            //    TotlalAmount = leaveEntitle.TotlalAmount,
            //    OrgBranchName = leaveEntitle.OrgBranchName,
            //    ExpiredBy = leaveEntitle.ExpiredBy,
            //    ExpiredDate = leaveEntitle.ExpiredDate,
            //    ReversedBy = leaveEntitle.ReversedBy,
            //    Void = leaveEntitle.Void,
            //    ReversedDate = leaveEntitle.ReversedDate,
            //    SumOfBalances = GetTotalBalance(leaveEntitle.EmployeeId),
            //    HireDate = leaveEntitle.Employee.HiredDate.ToShortDateString()
            //});

            //return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Create([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            var year = db.LeavesEntitle.FirstOrDefault(x =>
                    x.GlFiscalYearId == leaveEntitle.GlFiscalYearId &&
                    x.EmployeeId == leaveEntitle.EmployeeId);
            if (year != null)
            {
                if (year.IsExpired)
                {
                    ModelState.AddModelError("",
                        "Cannot register entitlement. The entitlement exists but it is expired.");
                }
                else
                {
                    ModelState.AddModelError("",
                        "Cannot register entitlement more than once per year.");
                }

                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && decimal.TryParse(maximumLeave.CurrentValue, out var maxLeaveValue))
            {
                if (leaveEntitle.Balance > maxLeaveValue || leaveEntitle.Amount > maxLeaveValue)
                {
                    ModelState.AddModelError("",
                        $"Entitlement amount can't exceed the maximum allowed ({maxLeaveValue}).");
                    return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
                }
            }

            // First Validation: Balance should not exceed the entitled amount

            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed the entitled amount.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            // Second Validation: Both Balance and Amount should not be less than zero
            if (leaveEntitle.Balance < 0 || leaveEntitle.Amount < 0)
            {
                ModelState.AddModelError("", "Both Balance and entitled amount cannot be less than zero.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            
            if (ModelState.IsValid)
            {
                var branch = db.Placements.FirstOrDefault(x => x.EmployeeId == leaveEntitle.EmployeeId && x.isCurrent);
                if (branch != null)
                {
                    leaveEntitle.BranchId = branch.BranchId;
                }
                var employee = db.Person_Employee.Find(leaveEntitle.EmployeeId);
                if (employee != null)
                {
                    leaveEntitle.EmployeeCode = employee.EmployeeCode;
                }

                var entity = new LeaveEntitle
                {
                    EmployeeId = leaveEntitle.EmployeeId,
                    BranchId = leaveEntitle.BranchId,
                    TotlalAmount = leaveEntitle.TotlalAmount,
                    GlFiscalYearId = leaveEntitle.GlFiscalYearId,
                    Amount = leaveEntitle.Amount,
                    Balance = leaveEntitle.Balance,
                    Remark = leaveEntitle.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = leaveEntitle.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = leaveEntitle.ModifiedBy,
                    EmployeeCode = leaveEntitle.EmployeeCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                entity.TotlalAmount = entity.Amount + GetTransfreedAmmount(leaveEntitle.GlFiscalYearId, leaveEntitle.EmployeeId);

                db.LeavesEntitle.Add(entity);
                db.SaveChanges();

                leaveEntitle.LeaveEntitleId = entity.LeaveEntitleId;

                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entity.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                #endregion
            }
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));

        }
        private decimal GetTransfreedAmmount(int periodId, int empId)
        {
            //var transfrred = db.LeaveTransfer.Where(x => x.To == periodId && x.EmployeeId == empId).FirstOrDefault();
            //if (transfrred != null)
            //{

            //    return transfrred.Amount;
            //}
            return 0;                               //
        }
        [AuthorizeRoles(HrmConstants.HRLeaveUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Update([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            ModelState.Clear();
            // First Validation: Balance should not exceed the entitled amount
            if (leaveEntitle.Balance > leaveEntitle.Amount)
            {
                ModelState.AddModelError("", "Balance can't exceed the entitled amount.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            // Second Validation: Both Balance and Amount should not be less than zero
            if (leaveEntitle.Balance < 0 || leaveEntitle.Amount < 0)
            {
                ModelState.AddModelError("", "Both Balance and entitled amount cannot be less than zero.");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            var maximumLeave = db.Configurations.FirstOrDefault(c => c.Attribute == LeaveSetting.MAXIMUM_LEAVE_ENTITLEMENT.ToString());
            if (maximumLeave != null && (leaveEntitle.Balance > Convert.ToDecimal(maximumLeave.CurrentValue) || leaveEntitle.Amount > Convert.ToDecimal(maximumLeave.CurrentValue)))
            {
                ModelState.AddModelError("", "Entitle amount can't exceed leave maximum setting");
            }
            if (ModelState.IsValid)
            {
                var entitle = db.LeavesEntitle.FirstOrDefault(x => x.LeaveEntitleId == leaveEntitle.LeaveEntitleId);
                if (entitle != null)
                {
                    entitle.Amount = leaveEntitle.Amount;
                    entitle.Balance = leaveEntitle.Balance;
                    entitle.Remark = leaveEntitle.Remark;
                    db.Entry(entitle).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle of " + entitle.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                    #endregion
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
                    db.LeavesEntitle.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Leave Entitle", "Success", User.Identity.Name, "Leave Entitle for " + entity.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRLeaveAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavesEntitle_Destroy([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            var year = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;
            var currentEntitlement = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitle.EmployeeId
                                                             && x.GlFiscalYearId == leaveEntitle.GlFiscalYearId).ToList();

            if (selectedFromYear <= currentYear && selectedToYear >= currentYear && currentEntitlement.Count <= 1)
            {
                ModelState.AddModelError("", "You cannot delete current year entitlement");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                // Find the existing LeaveEntitle entity by primary key
                var entityToDelete = db.LeavesEntitle.Find(leaveEntitle.LeaveEntitleId);
                if (entityToDelete != null)
                {
                    // Log deletion
                    var yearDelete = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
                    var deleteYearName = yearDelete.Name;
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                        "Deleted employee leave entitlement of employee " + leaveEntitle.EmployeeCode + " and fiscal year " + deleteYearName);

                    // Remove the entity from the database
                    db.LeavesEntitle.Remove(entityToDelete);
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "The entitlement could not be found.");
                }
            }

            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult LeavesEntitle_DestroyCopy([DataSourceRequest]DataSourceRequest request, LeaveEntitle leaveEntitle)
        {
            var year = db.GLFiscalYears.Find(leaveEntitle.GlFiscalYearId);
            int selectedToYear = year.DateTo.Year;
            int selectedFromYear = year.DateFrom.Year;
            int currentYear = DateTime.Now.Year;
            var currententitele = db.LeavesEntitle.Where(x => x.EmployeeId == leaveEntitle.EmployeeId
                                                          && x.GlFiscalYearId == leaveEntitle.GlFiscalYearId).ToList();
          
            if (selectedFromYear <= currentYear && selectedToYear >= currentYear&& currententitele.Count() <= 1)
            {
                ModelState.AddModelError("", "You cannot Delete current year Entitlement");
                return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
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
            return Json(new[] { leaveEntitle }.ToDataSourceResult(request, ModelState));
        }
        private List<int> GetActiveEmployeeFromCurrentUserBranch(string user)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(user);

            return db.Placements
                .Where(x => myBranches.Contains(x.BranchId) && x.isCurrent && x.Employee.IsActive)
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();
        }
        [HttpPost]
        public JsonResult ExpireEntitle(string period, int? employeeId)
        {
            if (string.IsNullOrWhiteSpace(period))
                return Json(new { Message = "Select valid Fiscal year or Period." });
            int.TryParse(period, out int fiscalYearId);
            var leaveEntitles = db.LeavesEntitle
                .Where(x => x.GlFiscalYearId == fiscalYearId && (!employeeId.HasValue || x.EmployeeId == employeeId))
                .ToList();

            if (!leaveEntitles.Any())
                return Json(new { Message = "No matching entitlements found." });
            if (leaveEntitles.All(x=>x.IsExpired))
                return Json(new { Message = "Already Expired." });

            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = true;
                leave.ExpiredBy = User.Identity.Name;
                leave.ExpiredDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new { Message = "Entitlements successfully expired." });
        }

        [HttpPost]
        public JsonResult ReverseExpireEntitle(string period, int? employeeId)
        {
            if (string.IsNullOrWhiteSpace(period))
                return Json(new { Message = "Select valid Fiscal year or Period." });
            int.TryParse(period, out int fiscalYearId);
            var leaveEntitles = db.LeavesEntitle
                .Where(x => x.GlFiscalYearId == fiscalYearId && (!employeeId.HasValue || x.EmployeeId == employeeId))
                .ToList();

            if (!leaveEntitles.Any())
                return Json(new { Message = "No matching entitlements found." });
            if (leaveEntitles.All(x => !x.IsExpired))
                return Json(new { Message = "Already Active." });
            foreach (var leave in leaveEntitles)
            {
                leave.IsExpired = false;
                leave.ReversedBy = User.Identity.Name;
                leave.ReversedDate = DateTime.Today;

                db.Entry(leave).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new { Message = "Entitlements successfully reversed." });
        }
        //public JsonResult ExpireEntitle(string period, int? employeeId)
        //{
        //    var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();
        //    if(leaveEntitles.Any()&& employeeId.HasValue()&& employeeId != null)
        //    {
        //        leaveEntitles = leaveEntitles.Where(x => x.EmployeeId == employeeId).ToList();
        //    }
        //    foreach (var leave in leaveEntitles)
        //    {
        //        leave.IsExpired = true;
        //        leave.ExpiredBy = User.Identity.Name;
        //        leave.ExpiredDate = DateTime.Today;

        //        db.Entry(leave).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        //public JsonResult ReverseExpireEntitle(string period ,int? employeeId)
        //{
        //    var leaveEntitles = db.LeavesEntitle.Where(x => x.GlFiscalYearId.ToString() == period).ToList();

        //    foreach (var leave in leaveEntitles)
        //    {
        //        leave.IsExpired = false;
        //        leave.ReversedBy = User.Identity.Name;
        //        leave.ReversedDate = DateTime.Today;

        //        db.Entry(leave).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
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
