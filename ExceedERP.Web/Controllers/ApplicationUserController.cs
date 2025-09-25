﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Common.HRM;
 using ExceedERP.Helpers.Common;
using System.Runtime.Caching;

namespace ExceedERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Admin_Regional,Admin_Module")]
    public class ApplicationUserController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ExceedDbContext db1 = new ExceedDbContext();

        public ActionResult Index()
        {
            var employee = db1.Person_Employee;
            var structure = db1.MainCompanyStructures;
            //var selectedEmployee = employee.Select(
            //  i => new
            //  {
            //      FullName = i.EmployeeCode + "_" + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
            //      PersonId = i.EmployeeId
            //  }).AsQueryable();
            //ViewData["EmployeeList"] = selectedEmployee;

            //var selectedstructure = structure.Select(
            //    i => new
            //    {
            //        Name = i.Name,
            //        Code = i.OrgStructureId
            //    }).AsQueryable();
            //ViewData["WorkUnitList"] = selectedstructure;

            //var selectedEmployee = employee.Select(
            //    i => new
            //    {
            //        FullName = (i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish).Replace("/","-"),//.Replace("\\", " - "),
            //        PersonId = i.EmployeeId
            //    }).AsQueryable();
            var selectedEmployee = from e in db1.Person_Employee
                                   join p in db1.Placements
                                       on e.EmployeeId equals p.EmployeeId
                                   where e.IsActive && p.isCurrent
                                   select (new
                                   {
                                       e.BranchId,
                                       PersonId = e.EmployeeId,
                                       FullName = (e.EmployeeCode + " " + e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish).Replace("/", "-"),//.Replace("\\", " - "),
                                   });

            if (IsRegionalAdmin())
            {
                List<int> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name).Select(int.Parse).ToList();
                string useid = UserInformationHelper.GetUserId(User.Identity.Name);
                selectedEmployee = selectedEmployee.Where(x => mybraches.Contains(x.BranchId));
            }

            ViewData["EmployeeList"] = selectedEmployee;

            var selectedstructure = structure.Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();
            ViewData["WorkUnitList"] = selectedstructure;

            var bank = db1.Branch.ToList();

            ViewData["Branches"] = bank
                .Select(e => new Branch
                {
                    BranchId = e.BranchId,
                    Name = e.Name
                })
                .OrderBy(e => e.Name);

            return View();
        }

        private AccountController account;

        public ActionResult ApplicationUsers_Read([DataSourceRequest]DataSourceRequest request, string branchid)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            List<ApplicationUser> applicationUsers = account.GetUsers();
            List<RegisterViewModel> userList = new List<RegisterViewModel>();
            #region filter by selected branch

            if (!String.IsNullOrEmpty(branchid))
            {
                var employeesassignnedinbranch = db1.Placements.Where(x => x.BranchId.ToString() == branchid)
                    .Where(x => x.isCurrent).Select(x => x.EmployeeId).ToList();
                applicationUsers = applicationUsers.Where(x => employeesassignnedinbranch.Contains(x.EmployeeId))
                    .ToList();
            }

            #endregion
            foreach (ApplicationUser user in applicationUsers)
            {
                //filter users in allowed branches only
                if (IsRegionalAdmin())
                {
                    var userBranch = db.UserBranches.Where(x => x.ApplicationUserId == user.Id)
                        .Where(x => mybraches.Contains(x.UserBranchId)).ToList();
                    if (!userBranch.Any())
                        continue;
                }
                RegisterViewModel registerViewModel = new RegisterViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    PhoneNo = user.PhoneNo,
                    EmployeeId = user.EmployeeId,
                    OrgStructureId = user.OrgStructureId,
                    IsEnabled = user.IsEnabled
                };
                userList.Add(registerViewModel);
            }
            IQueryable<RegisterViewModel> erpUsers = userList.AsQueryable();
            DataSourceResult result = erpUsers.ToDataSourceResult(request, user => new
            {
                user.Id,
                user.Email,
                user.UserName,
                user.FirstName,
                user.MiddleName,
                user.LastName,
                user.PhoneNo,
                user.EmployeeId,
                user.OrgStructureId,
                user.IsEnabled

            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicationUsers_Create([DataSourceRequest]DataSourceRequest request, RegisterViewModel applicationUser)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);

            var checkExistance = db.Users.Where(x => x.EmployeeId == applicationUser.EmployeeId).FirstOrDefault();
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "The Employee already exists!!");
            }
            if (ModelState.IsValid)
            {
                account.Register(applicationUser);

            }

            return Json(new[] { applicationUser }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicationUsers_Update([DataSourceRequest]DataSourceRequest request, RegisterViewModel applicationUser)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {

                account.Edit(applicationUser);
            }

            return Json(new[] { applicationUser }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicationUsers_Destroy([DataSourceRequest]DataSourceRequest request, RegisterViewModel applicationUser)
        {
            account = new AccountController(ApplicationUserManager.Usermgr, ApplicationSignInManager.SignInManager);
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                account.DeleteConfirmed(applicationUser.Id);
            }

            return Json(new[] { applicationUser }.ToDataSourceResult(request, ModelState));
        }
        private ApplicationGroupManager _groupManager;
        public ApplicationGroupManager GroupManager
        {
            get
            {
                return _groupManager ?? new ApplicationGroupManager();
            }
            private set
            {
                _groupManager = value;
            }
        }
        private ApplicationBranchManager _branchManager;
        public ApplicationBranchManager BranchManager
        {
            get
            {
                return _branchManager ?? new ApplicationBranchManager();
            }
            private set
            {
                _branchManager = value;
            }
        }
        public ActionResult Groups(GroupViewModel[] result)
        {
            var model = result[result.Length - 1];
            result = result.Where(x => x.Id != model.Id && x.Name != "masterGridParameter").ToArray();

            string[] groupIds = result.Select(x => x.Id).ToArray();

            if (model.Id != null)
            {
                GroupManager.SetUserGroups(model.Id, User.Identity.Name, groupIds);//added User.Identity.Name
                //log
                var user = db.Users.FirstOrDefault(p => p.Id == model.Id);
                UserHelper.AccessLog(Request.UserHostAddress, "Edit User", "Success", User.Identity.Name, "Edited groups of user  " +user?.UserName);
            }

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingBranches_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<BranchVm> branches, string id)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            db = new ApplicationDbContext();
            var results = new List<BranchVm>();
            BranchVm branchVm = new BranchVm();
            ExceedDbContext dbs = new ExceedDbContext();

            UserBranchGroup userBranches = new UserBranchGroup();

            int Count = 0;

            if (branches != null && ModelState.IsValid)
            {
                foreach (var branch in branches)
                {
                    //filter only allowed branches
                    if (IsRegionalAdmin())
                    {
                        if (!mybraches.Contains(branch.BranchId))
                        {
                            branch.IsAllowed = false;
                            branch.IsDefault = false;
                        }

                    }
                    dbs = new ExceedDbContext();
                    var userBranch = db.UserBranches.FirstOrDefault(x => x.UserBranchId == branch.BranchId.ToString() && x.ApplicationUserId == id);
                    userBranches = new UserBranchGroup();
                    if (userBranch != null)
                    {
                        db.UserBranches.Remove(userBranch);
                        db.SaveChanges();
                    }

                    userBranches.ApplicationUserId = id;
                    userBranches.UserBranchId = branch.BranchId;
                    if (branch.IsDefault && Count == 0)
                    {
                        Count = 1;
                        userBranches.IsDefault = branch.IsDefault;
                        db.UserBranches.Add(userBranches);
                        db.SaveChanges();
                        continue;
                    }

                    if (branch.IsAllowed)
                    {

                        db.UserBranches.Add(userBranches);
                        db.SaveChanges();
                        //log
                        var user = db.Users.FirstOrDefault(p => p.Id == id);
                        int did = int.Parse(branch.BranchId);
                        var HRbranch = dbs.Branch.FirstOrDefault(p => p.BranchId ==did);
                        UserHelper.AccessLog(Request.UserHostAddress, "User branch", "Success", User.Identity.Name, $"Updated user branch for user  {user?.UserName} given branch {HRbranch.Name} ");

                    }
                }
            }


            return Json(results.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branches(BranchVm result, string id)
        {
            //var model = result[result.Length - 1];
            //result = result.Where(x => x.BranchId != model.BranchId && x.BranchName != "masterGridParameter").ToArray();

            //string[] branchId = result.Select(x => x.BranchId).Distinct().ToArray();

            //if (model.BranchId != null)
            //{
            // BranchManager.SetUsersBranch(id,result); 
            //}

            return Json(result);
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Users");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Pdf-Export", "Success", User.Identity.Name, "Exported User roles");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        public string GetFullName(int empId)
        {

            return UserInformationHelper.GetEmployeeFullNameById(empId);

        }
        public ActionResult Branch_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<BranchVm> modelList = new List<BranchVm>();
            var usersBranch = db.UserBranches;
            ExceedDbContext dbExceed = new ExceedDbContext();
            var branchList = dbExceed.Branch.ToList();
            //filter allowed branches only
            if (IsRegionalAdmin())
            {
                branchList = branchList.Where(x => mybraches.Contains(x.BranchId.ToString())).ToList();
            }

            foreach (var b in branchList)
            {
                var branchVm = new BranchVm
                {
                    BranchName = b.Name
                };
                var check = usersBranch.FirstOrDefault(x => x.UserBranchId == b.BranchId.ToString() && x.ApplicationUserId == Id.ToString());
                if (check != null)
                {
                    branchVm.IsAllowed = true;
                    branchVm.IsDefault = check.IsDefault;
                }
                else
                {
                    branchVm.IsAllowed = false;
                }
                modelList.Add(branchVm);

            }
            IQueryable<BranchVm> branchs = modelList.AsQueryable();
            DataSourceResult result = branchs.ToDataSourceResult(request, branch => new
            {

                BranchName = branch.BranchName,
                Remark = branch.Remark,
                IsAllowed = branch.IsAllowed,
                IsDeafult = branch.IsDefault,
            });

            return Json(result);
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var user = db.Users.FirstOrDefault(p => p.Id == id);
                if (user != null && user.IsEnabled)
                {
                    user.IsEnabled = false;
                    #region access log
                    UserHelper.AccessLog(Request.UserHostAddress, "Disable User", "Success", User.Identity.Name, "Disabled user  " +user.UserName);
                    #endregion
                }
                else
                {
                    if (user != null)
                    {
                        user.IsEnabled = true;
                        #region access log

                        UserHelper.AccessLog(Request.UserHostAddress, "Enable User", "Success", User.Identity.Name,"Enabled user  " + user.UserName);

                        #endregion
                    }
                }
                db.Users.Attach(user);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");

        }

        public JsonResult RefreshUserWorkUnit()
        {

            var users = db.Users.ToList();
            foreach (var user in users)
            {
                var placement = db1.Placements.Where(x=>x.EmployeeId==user.EmployeeId).FirstOrDefault(x => x.isCurrent);
                if (placement != null) user.OrgStructureId = placement.OrgStructureId;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }

            var formattedData = new
            {
                Message = "Refresh Successful.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //helper

        public bool IsRegionalAdmin()
        {
            return (!System.Web.HttpContext.Current.User.IsInRole("Admin")
                    && System.Web.HttpContext.Current.User.IsInRole("Admin_Regional"));


        }

        //provider

        public JsonResult GetEmployeeMap1(string text)
        {
            List<EmployeeCacheVm> list = new List<EmployeeCacheVm>();
            if (!String.IsNullOrWhiteSpace(text))
            {
                var allData = UserInformationHelper.GetEmployeeDetailFromCache();
                list = allData.Where(x => x.FullName.ToUpper().Contains(text.ToUpper())).ToList();
            }
            return Json(list.Select(p => new
            {
                PersonId = p.PersonId,
                FullName = p.EmployeeCode+"-"+p.FullName
            }), JsonRequestBehavior.AllowGet);

        }

        //only placed employees
        public JsonResult GetEmployeeMap_8(string text)
        {
            ExceedDbContext _db = new ExceedDbContext();
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = from e in _db.Person_Employee
                                join p in _db.Placements
                                    on e.EmployeeId equals p.EmployeeId
                                where e.IsActive && p.isCurrent

                        select( new EmployeeVm
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            FirstName = e.FirstName,
                            MiddleName = e.MiddleName,
                            LastName = e.LastName,
                            FirstNameEnglish = e.FirstNameEnglish,
                            MiddleNameEnglish = e.MiddleNameEnglish,
                            LastNameEnglish = e.LastNameEnglish
                        });

                if(!string.IsNullOrEmpty(text))
                {
                    employees = employees.Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper()));
                }
                    
                list = employees.ToList();
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }

        //active
        public JsonResult GetEmployeeMap(string text)
        {
            ExceedDbContext _db = new ExceedDbContext();
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = _db.Person_Employee
                    .Where(x=>x.IsActive)
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
