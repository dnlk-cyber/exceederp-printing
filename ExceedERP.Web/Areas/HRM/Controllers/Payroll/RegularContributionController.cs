using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class RegularContributionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close);
            var lists = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = lists.Select(x => x.g).ToList(); ;
            var lookups = db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.REGULAR); 
            ViewData["Contribution"] = lookups;
         
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
   
        public ActionResult RegularContributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<RegularContribution> regularcontributions = db.RegularContributions.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = regularcontributions.ToDataSourceResult(request, regularContribution => new {
                RegContributionId = regularContribution.RegContributionId,
                EmployeeId = regularContribution.EmployeeId,
                ContributionTypeId = regularContribution.ContributionTypeId,
                Amount = regularContribution.Amount,
                AdditionalAmount = regularContribution.AdditionalAmount,
                IsActive = regularContribution.IsActive,
                GLPeriodId = regularContribution.GLPeriodId,
                NetGross = regularContribution.NetGross,
                Remark = regularContribution.Remark,
                ApplyToAll = regularContribution.ApplyToAll,
                DateCreated = regularContribution.DateCreated,
                LastModified = regularContribution.LastModified,
                CreatedBy = regularContribution.CreatedBy,
                ModifiedBy = regularContribution.ModifiedBy,
                OrgBranchName = regularContribution.OrgBranchName 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegularContributions_Create([DataSourceRequest]DataSourceRequest request, RegularContribution regularContribution)
        {
            ModelState.Clear();
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            if (ModelState.IsValid)
            {
                regularContribution.OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var checkexistancep = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == regularContribution.GLPeriodId && regularContribution.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistancep != null)
                {
                    

                    ModelState.AddModelError("", "You cannot add this contribution. This period is already processed!!");
                }
                else
                {
                    var checkexistance = db.RegularContributions.Where(x => x.EmployeeId == regularContribution.EmployeeId  && x.ContributionTypeId == regularContribution.ContributionTypeId).FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "This Regular contribution type  on this period is already registered!!");
                    }
                    else
                    {
                        if (regularContribution.ApplyToAll)
                        {
                            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                            IQueryable<Person_Employee> employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                            //var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
                            //List<Person_Employee> employees = new List<Person_Employee>();
                            //foreach (var emp in person_employee)
                            //{
                            //    var check = placments.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                            //    if (check != null)
                            //    {
                            //        employees.Add(emp);
                            //    }
                            //}
                            foreach (var e in employees)
                            {
                                db = new ExceedDbContext();
                                var entity = new RegularContribution
                                {
                                    EmployeeId = e.EmployeeId,
                                    ContributionTypeId = regularContribution.ContributionTypeId,
                                    Amount = regularContribution.Amount,
                                    AdditionalAmount = regularContribution.AdditionalAmount,
                                    IsActive = true,
                                    GLPeriodId = regularContribution.GLPeriodId,
                                    NetGross = regularContribution.NetGross,
                                    Remark = regularContribution.Remark,
                                    ApplyToAll = regularContribution.ApplyToAll,
                                    DateCreated = DateTime.Now,
                                    LastModified = regularContribution.LastModified,
                                    CreatedBy = User.Identity.Name,
                                    ModifiedBy = regularContribution.ModifiedBy,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)

                                };
                                db.RegularContributions.Add(entity);
                                db.SaveChanges();
                                regularContribution.RegContributionId = entity.RegContributionId;
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"added contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                #endregion
                            }
                        }
                        else
                        {

                            var entity = new RegularContribution
                            {
                                EmployeeId = regularContribution.EmployeeId,
                                ContributionTypeId = regularContribution.ContributionTypeId,
                                Amount = regularContribution.Amount,
                                AdditionalAmount = regularContribution.AdditionalAmount,
                                IsActive = true,
                                GLPeriodId = regularContribution.GLPeriodId,
                                NetGross = regularContribution.NetGross,
                                Remark = regularContribution.Remark,
                                ApplyToAll = regularContribution.ApplyToAll,
                                DateCreated = DateTime.Now,
                                LastModified = regularContribution.LastModified,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = regularContribution.ModifiedBy,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                            };

                            db.RegularContributions.Add(entity);
                            db.SaveChanges();
                            regularContribution.RegContributionId = entity.RegContributionId;
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"added contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                            #endregion
                        }
                    }
                }
            }
            return Json(new[] { regularContribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegularContributions_Update([DataSourceRequest]DataSourceRequest request, RegularContribution regularContribution)
        {
            ModelState.Clear();
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == regularContribution.EmployeeId && x.GLPeriodId == regularContribution.GLPeriodId && regularContribution.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    var checkLoan = db.PayrollProcessRegularContribution.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.RegContributionId == regularContribution.RegContributionId).FirstOrDefault();
                    if (checkLoan != null)
                    {
                        ModelState.AddModelError("", "You cannot update this data. It already processed!!");
                    }
                    else
                    { 
                        if (regularContribution.ApplyToAll)
                {
                    List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                    IQueryable<Person_Employee> employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                    //var placments = db.Placements.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.isCurrent);
                    //List<Person_Employee> employees = new List<Person_Employee>();
                    //foreach (var emp in person_employee)
                    //{
                    //    var check = placments.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                    //    if (check != null)
                    //    {
                    //        employees.Add(emp);
                    //    }
                    //}
                    var allcontribution = db.RegularContributions.Where(x => x.GLPeriodId == regularContribution.GLPeriodId && x.ContributionTypeId == regularContribution.ContributionTypeId).ToList();
                    if (allcontribution.Any())
                    {
                        //  db.RegularContributions.Attach(allcontribution);
                        db.RegularContributions.RemoveRange(allcontribution);
                        db.SaveChanges();
                    }
                    foreach (var e in employees)
                    {
                        db = new ExceedDbContext();
                        ExceedDbContext dbR = new ExceedDbContext();

                        var entity = new RegularContribution
                        {
                            EmployeeId = e.EmployeeId,
                            ContributionTypeId = regularContribution.ContributionTypeId,
                            Amount = regularContribution.Amount,
                            AdditionalAmount = regularContribution.AdditionalAmount,
                            IsActive = true,
                            GLPeriodId = regularContribution.GLPeriodId,
                            NetGross = regularContribution.NetGross,
                            Remark = regularContribution.Remark,
                            ApplyToAll = regularContribution.ApplyToAll,
                            DateCreated = DateTime.Now,
                            LastModified = regularContribution.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = regularContribution.ModifiedBy,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)

                        };

                        db.RegularContributions.Add(entity);
                        db.SaveChanges();
                        regularContribution.RegContributionId = entity.RegContributionId;
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                #endregion

                            }
                        }
                        else{

                        var entity = new RegularContribution
                        {
                            RegContributionId = regularContribution.RegContributionId,
                            EmployeeId = regularContribution.EmployeeId,
                            ContributionTypeId = regularContribution.ContributionTypeId,
                            Amount = regularContribution.Amount,
                            AdditionalAmount = regularContribution.AdditionalAmount,
                            IsActive = regularContribution.IsActive,
                            GLPeriodId = regularContribution.GLPeriodId,
                            NetGross = regularContribution.NetGross,
                            Remark = regularContribution.Remark,
                            ApplyToAll = regularContribution.ApplyToAll,
                            DateCreated = regularContribution.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = regularContribution.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = regularContribution.OrgBranchName
                        };

                        db.RegularContributions.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                            #endregion
                        }


                    }
                    
                }
                else
                {

                    if (regularContribution.ApplyToAll)
                    {
                        List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                        IQueryable<Person_Employee> employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                        //List<Person_Employee> employees = new List<Person_Employee>();
                        //foreach (var emp in person_employee)
                        //{
                        //    var check = placments.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                        //    if (check != null)
                        //    {
                        //        employees.Add(emp);
                        //    }
                        //}
                        var allcontribution = db.RegularContributions.Where(x => x.GLPeriodId == regularContribution.GLPeriodId && x.ContributionTypeId == regularContribution.ContributionTypeId).ToList();
                        if (allcontribution.Any())
                        {
                          //  db.RegularContributions.Attach(allcontribution);
                            db.RegularContributions.RemoveRange(allcontribution);
                            db.SaveChanges();
                        }
                        foreach (var e in employees)
                        {
                            db = new ExceedDbContext();
                            ExceedDbContext dbR = new ExceedDbContext();
                           
                            var entity = new RegularContribution
                            {
                                EmployeeId = e.EmployeeId,
                                ContributionTypeId = regularContribution.ContributionTypeId,
                                Amount = regularContribution.Amount,
                                AdditionalAmount = regularContribution.AdditionalAmount,
                                IsActive = true,
                                GLPeriodId = regularContribution.GLPeriodId,
                                NetGross = regularContribution.NetGross,
                                Remark = regularContribution.Remark,
                                ApplyToAll = regularContribution.ApplyToAll,
                                DateCreated = DateTime.Now,
                                LastModified = regularContribution.LastModified,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = regularContribution.ModifiedBy,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)

                            };
                            
                                db.RegularContributions.Add(entity);
                                db.SaveChanges();
                                regularContribution.RegContributionId = entity.RegContributionId;
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                            #endregion

                        }
                    }
                    else
                    {
                        var entity = new RegularContribution
                        {
                            RegContributionId = regularContribution.RegContributionId,
                            EmployeeId = regularContribution.EmployeeId,
                            ContributionTypeId = regularContribution.ContributionTypeId,
                            Amount = regularContribution.Amount,
                            AdditionalAmount = regularContribution.AdditionalAmount,
                            IsActive = regularContribution.IsActive,
                            GLPeriodId = regularContribution.GLPeriodId,
                            NetGross = regularContribution.NetGross,
                            Remark = regularContribution.Remark,
                            ApplyToAll = regularContribution.ApplyToAll,
                            DateCreated = regularContribution.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = regularContribution.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            OrgBranchName = regularContribution.OrgBranchName
                        };

                        db.RegularContributions.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll regular contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.Employee.EmployeeCode} amount {entity.Amount} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                        #endregion
                    }
                }
            }

            return Json(new[] { regularContribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegularContributions_Destroy([DataSourceRequest]DataSourceRequest request, RegularContribution regularContribution)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == regularContribution.EmployeeId && x.GLPeriodId == regularContribution.GLPeriodId && regularContribution.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    //var checkLoan = db.PayrollProcessRegularContribution.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.RegContributionId == regularContribution.RegContributionId).FirstOrDefault();
                    //if (checkLoan != null)
                    //{
                        ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                    }
                    else
                    {
                var entity = new RegularContribution
                {
                    RegContributionId = regularContribution.RegContributionId,
                    EmployeeId = regularContribution.EmployeeId,
                    ContributionTypeId = regularContribution.ContributionTypeId,
                    Amount = regularContribution.Amount,
                    AdditionalAmount = regularContribution.AdditionalAmount,
                    IsActive = regularContribution.IsActive,
                    GLPeriodId = regularContribution.GLPeriodId,
                    NetGross = regularContribution.NetGross,
                    Remark = regularContribution.Remark,
                    ApplyToAll = regularContribution.ApplyToAll,
                    DateCreated = regularContribution.DateCreated,
                    LastModified = regularContribution.LastModified,
                    CreatedBy = regularContribution.CreatedBy,
                    ModifiedBy = regularContribution.ModifiedBy,
                      OrgBranchName = regularContribution.OrgBranchName 
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee regular contribution information of employee " + regularContribution.EmployeeId,Modules.Payroll);
                #endregion
                db.RegularContributions.Attach(entity);
                db.RegularContributions.Remove(entity);
                db.SaveChanges();
            }
                }
                //else
                //{
                //    var entity = new RegularContribution
                //    {
                //        RegContributionId = regularContribution.RegContributionId,
                //        EmployeeId = regularContribution.EmployeeId,
                //        ContributionTypeId = regularContribution.ContributionTypeId,
                //        Amount = regularContribution.Amount,
                //        AdditionalAmount = regularContribution.AdditionalAmount,
                //        IsActive = regularContribution.IsActive,
                //        GLPeriodId = regularContribution.GLPeriodId,
                //        Remark = regularContribution.Remark,
                //        ApplyToAll = regularContribution.ApplyToAll,
                //        DateCreated = regularContribution.DateCreated,
                //        LastModified = regularContribution.LastModified,
                //        CreatedBy = regularContribution.CreatedBy,
                //        ModifiedBy = regularContribution.ModifiedBy,
                //          OrgBranchName = regularContribution.OrgBranchName
                //    };

                //    db.RegularContributions.Attach(entity);
                //    db.RegularContributions.Remove(entity);
                //    db.SaveChanges();
                //}
           // }

            return Json(new[] { regularContribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
