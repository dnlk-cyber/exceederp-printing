using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Hubs;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers
{
    public class HRMInputs : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int EmployeeId
        {
            get;
            set;
        }
        public static string Owner
        {
            get;
            set;
        }
        public static int SelectedFile { get; set; }
        public static int RequestId { get; set; }
        public static string ReferenceType { get; set; }
        public static string LetterId { get; set; }
        public static int CosignId { get; set; }
        public static bool GetSalarySetting()
        {
            ExceedDbContext _db = new ExceedDbContext();
            var setting = _db.SalarySettings.FirstOrDefault();
            if (setting != null)
            {
                return setting.IsJobtitleIncludedInSalary;
            }

            return false;
        }
        public List<EmployeeIdView> GetEmployees(string user)
        {
            return GetPlacmentEmployees(user);
            #region commented

            //List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            //List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            //EmployeeIdView EmployeeIdView = new EmployeeIdView();
            //var employeeList = db.Person_Employee.Where(x => mybraches.Contains(x.OrgBranchName));
            ////var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            ////if (employeeList.Any())
            ////{
            ////    employeeList = employeeList.AsQueryable();
            ////}
            //foreach (var emp in employeeList)
            //{
            //    //var check = placments.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
            //    //if (check != null)
            //    //{
            //        EmployeeIdView = new EmployeeIdView();
            //        EmployeeIdView.PersonId = emp.EmployeeId;
            //        EmployeeIdView.EmployeeCode = emp.EmployeeCode;
            //        EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
            //        EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
            //        EmployeeIdView.IsActive = emp.IsActive;
            //        EmployeeIdViews.Add(EmployeeIdView);
            //    //}
            //}
            //var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            //return list;
            #endregion
        }
        public List<EmployeeIdView> GetEmployeesForPlacment(string user)
        {

            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => mybraches.Contains(x.OrgBranchName));

            foreach (var emp in employeeList)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.FullNameEng = emp.FullNameaEnglish;
                EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);


            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<EmployeeIdView> GetPayrollPlacmentEmployees(string user)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(user);
            List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements.Where(x => x.isCurrent).
                                        Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                placments = placments.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }

                var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {
                EmployeeIdView = new EmployeeIdView
                {
                    PersonId = emp.EmployeeId,
                    EmployeeCode = emp.EmployeeCode,
                    FullNameWithOutCode = emp.FullName,
                    FullName = emp.FullNameCode,
                    IsActive = emp.IsActive
                };
                EmployeeIdViews.Add(EmployeeIdView);
            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<EmployeeIdView> GetPayrollPlacmentEmployees_ori(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements;//.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {
                EmployeeIdView = new EmployeeIdView();

                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);
            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        //replacement for the below one
        public List<int> GetUserBranchAndBusinessUnitEmployees(string user)
        {
            db.Database.CommandTimeout = 7200;//added
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(user);

            List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(user);
         
            var placments = db.Placements.Where(x => x.isCurrent).
                                        Where(x => mybraches.Contains(x.BranchId));

            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                placments = placments.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            var list = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where  p.isCurrent && e.IsActive
                            select (new
                            {
                                EmployeeId = p.EmployeeId
                            });
            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId).ToList();
                return listOfEmployees;
            }
            return new List<int>();
        }
        public List<string> GetUserBranchEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            var placments = from p in db.Placements
                            join
                                e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where mybraches.Contains(p.BranchId.ToString()) && p.isCurrent && e.IsActive
                            select (new
                            {
                                EmployeeId = p.EmployeeId
                            });
            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }
        public List<int> GetUserBranchAllEmployees(string user)
        {
            List<int> employeeIdLists = new List<int>();

            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            var employees = db.Person_Employee.ToList();
            foreach (var emp in employees)
            {
                if (emp != null && emp.IsActive)
                {
                    var empPlacement = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
                    if (empPlacement != null)
                    {
                        employeeIdLists.Add(empPlacement.EmployeeId);
                    }
                }
                else if (emp != null)
                {
                    var empPlacement = db.Placements.Where(x => x.EmployeeId == emp.EmployeeId && mybraches.Contains(x.BranchId.ToString())).OrderByDescending(x => x.AssignmentDate).ToList().FirstOrDefault();
                    if (empPlacement != null)
                    {
                        employeeIdLists.Add(empPlacement.EmployeeId);
                    }
                }
            }

            return employeeIdLists;
        }       

        public List<string> GetAllUserBranchEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            var placments = from p in db.Placements
                            join e in db.Person_Employee
                                on p.EmployeeId equals e.EmployeeId
                            where mybraches.Contains(p.BranchId.ToString())
                            select (new
                            {
                                EmployeeId = p.EmployeeId
                            });
            if (placments.Any())
            {
                var listOfEmployees = placments.Select(x => x.EmployeeId.ToString()).ToList();
                return listOfEmployees;
            }
            return new List<string>();
        }
        public List<EmployeeIdView> GetPlacmentEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public List<EmployeeIdView> GetAllPlacmentEmployees(string user)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(user);
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();
            var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);

            var lists = from e in employeeList
                        join p in placments
                            on e.EmployeeId equals p.EmployeeId
                        select (new
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            FullNameCode = e.EmployeeCode + "- " + e.FirstName + " " + e.MiddleName + " " + e.LastName,
                            IsActive = e.IsActive,
                        });
            lists = lists.Distinct();
            foreach (var emp in lists)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FullName;
                EmployeeIdView.FullName = emp.FullNameCode;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }
            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public JsonResult GetFiscalYear()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public List<EmployeeIdView> GetEmployees()
        {

            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employeeList = db.Person_Employee.AsQueryable();

            if (employeeList.Any())
            {
                employeeList = employeeList.AsQueryable();
            }
            foreach (var emp in employeeList)
            {

                EmployeeIdView = new EmployeeIdView();
                EmployeeIdView.PersonId = emp.EmployeeId;
                EmployeeIdView.EmployeeCode = emp.EmployeeCode;
                EmployeeIdView.FullNameWithOutCode = emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.FullName = emp.EmployeeCode + "- " + emp.FirstName + " " + emp.MiddleName + " " + emp.LastName;
                EmployeeIdView.IsActive = emp.IsActive;
                EmployeeIdViews.Add(EmployeeIdView);

            }

            var list = EmployeeIdViews != null ? EmployeeIdViews.ToList() : new List<EmployeeIdView>();
            return list;
        }
        public string GetEmployeeCode(int empId)
        {
            var empCode = db.Person_Employee.Find(empId)?.EmployeeCode;
            return empCode;
        }
        //public void HRNotifications()
        //{
        //    List<UserVM> contractuser = new List<UserVM>();
        //    List<UserVM> promotionuser = new List<UserVM>();
        //    List<UserVM> terminationtuser = new List<UserVM>();
        //    List<UserVM> probationuser = new List<UserVM>();
        //    List<UserVM> expiryNotify = new List<UserVM>();


        //    #region APTS Drug Expiry

        //    var expiries = db.DrugReceivalDetails.OrderByDescending(x => x.ExpiryDate);

        //    foreach (var expiry in expiries)
        //    {
        //        #region notification


        //        int leftDateToExpire = expiry.ExpiryDate.Subtract(DateTime.Now).Days;
        //        while (leftDateToExpire <= 30 && leftDateToExpire > 0)
        //        {
        //            expiryNotify.AddRange(UserInformationHelper.GetUsersInRole(APTSConstants.APTSadmin));
        //            expiryNotify.AddRange(UserInformationHelper.GetUsersInRole(APTSConstants.APTSDespensary));
        //            expiryNotify.AddRange(UserInformationHelper.GetUsersInRole(APTSConstants.APTSDrugStore));
        //            foreach (var userVm in expiryNotify)
        //            {

        //                string drug = "";
        //                var drugList = db.DrugLists.FirstOrDefault(x => x.DrugListsId == expiry.DrugListsId);
        //                if (drugList != null)
        //                {
        //                    drug = drugList.Description;
        //                }

        //                NotifModels notif = new NotifModels
        //                {

        //                    UserId = "System",
        //                    Title = "Drug Expiry Notification",
        //                    Message = $"{drug} has {leftDateToExpire} Days lefts To Expier",
        //                    URL = "~/APTS/DrugReceivalModel19",
        //                    DeadLine = expiry.ExpiryDate

        //                };

        //                Notify.NotifyUser(notif);
        //            }
        //        }

        //        #endregion
        //        #endregion




        //        #region Contract
        //        var contract = db.Contract.OrderByDescending(x => x.To);

        //        foreach (var c in contract)
        //        {

        //            // var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
        //            // NotificationHelper.AddNotofication(Request.UserHostAddress, "HR_Profile_Admin,HR_Admin", User.Identity.Name, c.To, "", fullname + " -Employee Promotion");

        //            #region notification


        //            int leftDate = c.To.Subtract(DateTime.Now).Days;
        //            while (leftDate <= 15 && leftDate > 0)
        //            {
        //                contractuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRProfileAdmin));
        //                contractuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin));
        //                foreach (var userVm in contractuser)
        //                {
        //                    string person = "";
        //                    var employeeId = UserInformationHelper.GetEmployeeId(userVm.UserName);
        //                    var contractEmp = db.Contract.FirstOrDefault(x => x.EmployeeId == employeeId);
        //                    if (contractEmp != null)
        //                    {
        //                        person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == contractEmp.EmployeeId)?.FullNameAmharic;
        //                    }

        //                    NotifModels notif = new NotifModels
        //                    {

        //                        UserId = UserInformationHelper.GetEmployeeUserId(employeeId),
        //                        Title = "Contract Expiry Notification",
        //                        Message = $"Contract Employee {person} has {leftDate} Days lefts To Expier Contract",
        //                        URL = "~/HRM/EmployeeContract",
        //                        DeadLine = c.To

        //                    };

        //                    Notify.NotifyUser(notif);
        //                }
        //            }

        //            #endregion
        //        }
        //        #endregion
        //        #region Promotion
        //        int PromotionPerYear = 0;
        //        int checkDiffrence = 0;
        //        var configration = db.Configurations.FirstOrDefault(x => x.Reference == "YEARLY_PROMOTION_PER_YEAR" && x.Attribute == "Promotion");
        //        if (configration != null)
        //        {
        //            PromotionPerYear = Convert.ToInt32(configration.CurrentValue);
        //        }
        //        var emplist = db.Person_Employee.Where(x => x.IsActive).ToList();


        //        if (emplist.Any())
        //        {
        //            foreach (var e in emplist)
        //            {
        //                db = new ExceedDbContext();
        //                var placements = db.Placements.FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);

        //                if (placements != null)
        //                {
        //                    checkDiffrence = DateTime.Now.Year - placements.AssignmentDate.Year;
        //                    if (checkDiffrence >= PromotionPerYear)
        //                    {
        //                        var fullname = e.FirstName + " " + e.MiddleName + " " + e.LastName;
        //                        //NotificationHelper.AddNotofication(Request.UserHostAddress, "HR_Placement_Admin,HR_Admin,HR_Placement_User", User.Identity.Name, DateTime.Now, "", fullname + " -Employee Contract");
        //                        //NotificationHelper.AddNotofication(Request.UserHostAddress, "HR_Placement_Admin,HR_Admin,HR_Placement_User", User.Identity.Name, DateTime.Now, "", fullname + " -Employee Contract");
        //                        #region notification


        //                        // int leftDate = e.d.Subtract(DateTime.Now).Days;

        //                        promotionuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRPlacementAdmin));
        //                        promotionuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRPlacementUser));
        //                        promotionuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin));
        //                        foreach (var userVm in promotionuser)
        //                        {
        //                            string person = "";
        //                            var employeeId = UserInformationHelper.GetEmployeeId(userVm.UserName);
        //                            var contractEmp = db.Contract.FirstOrDefault(x => x.EmployeeId == employeeId);
        //                            if (contractEmp != null)
        //                            {
        //                                person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == contractEmp.EmployeeId)?.FullNameAmharic;
        //                            }

        //                            NotifModels notif = new NotifModels
        //                            {

        //                                UserId = UserInformationHelper.GetEmployeeUserId(employeeId),
        //                                Title = "Promotion Notification",
        //                                Message = $"{person} is Promoted",
        //                                URL = "~/HRM/ApplicationForm",
        //                                //DeadLine = c.EndDate

        //                            };

        //                            Notify.NotifyUser(notif);

        //                        }


        //                        #endregion

        //                    }
        //                }
        //            }
        //        }

        //        #endregion
        //        #region Termination
        //        var Terminations = db.Terminations.OrderByDescending(x => x.TerminationDate);
        //        foreach (var c in Terminations)
        //        {

        //            //var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
        //            // NotificationHelper.AddNotofication(Request.UserHostAddress, "HR_Termination_User,HR_Admin", User.Identity.Name, c.TerminationDate, "", fullname + " -Employee Termination");
        //            terminationtuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRTerminationUser));
        //            terminationtuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin));

        //            int left = 0;
        //            foreach (var userVm in terminationtuser)
        //            {
        //                string person = "";
        //                var employeeId = UserInformationHelper.GetEmployeeId(userVm.UserName);
        //                var termEmp = db.Terminations.Where(x => x.EmployeeId == employeeId).ToList();
        //                foreach (var termination in termEmp)
        //                {
        //                    person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == termination.EmployeeId)
        //                        ?.FullNameAmharic;
        //                    left = termination.TerminationDate.Subtract(DateTime.Now).Days;



        //                    NotifModels notif = new NotifModels
        //                    {

        //                        UserId = UserInformationHelper.GetEmployeeUserId(employeeId),
        //                        Title = "Termination Notification",
        //                        Message = $" {person} has {left} Days lefts To Terminate",
        //                        URL = "~/HRM/TerminationRequest",
        //                        DeadLine = c.TerminationDate

        //                    };
        //                    Notify.NotifyUser(notif);
        //                }


        //            }



        //        }
        //        #endregion
        //        #region Probation
        //        var Probations = db.Prohibitions.Where(x => x.LookupId == EmployeeStatus.PROBATION).OrderByDescending(x => x.EndDate);
        //        foreach (var c in Probations)
        //        {

        //            //var fullname = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
        //            //NotificationHelper.AddNotofication(Request.UserHostAddress, "HR_Recruitment_Admin,HR_Recruitment_User,HR_Admin", User.Identity.Name, c.EndDate, "", fullname + " -Employee Termination");


        //            #region notification


        //            int leftDate = c.EndDate.Subtract(DateTime.Now).Days;

        //            probationuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRRecruitmentAdmin));
        //            probationuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRRecruitmentUser));
        //            probationuser.AddRange(UserInformationHelper.GetUsersInRole(HrmConstants.HRAdmin));
        //            foreach (var userVm in probationuser)
        //            {
        //                string person = "";
        //                var employeeId = UserInformationHelper.GetEmployeeId(userVm.UserName);
        //                var contractEmp = db.Contract.FirstOrDefault(x => x.EmployeeId == employeeId);
        //                if (contractEmp != null)
        //                {
        //                    person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == contractEmp.EmployeeId)?.FullNameAmharic;
        //                }

        //                NotifModels notif = new NotifModels
        //                {

        //                    UserId = UserInformationHelper.GetEmployeeUserId(employeeId),
        //                    Title = "Probation Notification",
        //                    Message = $"{person} Probation End date lefts only {leftDate}",
        //                    URL = "~/HRM/EmployeeProbation",
        //                    DeadLine = c.EndDate

        //                };

        //                Notify.NotifyUser(notif);

        //            }


        //            #endregion
        //        }
        //        #endregion


        //    }


        //}
    }
}