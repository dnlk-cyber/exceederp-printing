using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class AssociationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var assocationTypes = Enum.GetValues(typeof(AssocationType))
                      .Cast<AssocationType>()
                      .ToList();
            ViewData["AssociationTypeList"] = assocationTypes;
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close);
            var listt = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });
            ViewData["PeriodList"] = listt.Select(x => x.g).ToList(); ;
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public JsonResult GetEmployees()
        {
            db = new ExceedDbContext();
            var employees = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = employees;

            return Json(employees.Select(p => new { PersonId = p.PersonId, FullName = p.FullName }), JsonRequestBehavior.AllowGet);


        }
        public ActionResult Associations_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Association> associations = db.Associations.Where(x => employees.Contains(x.EmployeeId.ToString())).OrderBy(o => o.Type);
            DataSourceResult result = associations.ToDataSourceResult(request, association => new {
                AssociationId = association.AssociationId,
                EmployeeId = association.EmployeeId,
                Type = association.Type,
                MonthlyAmount = association.MonthlyAmount,
                AdditionalAmount = association.AdditionalAmount,
                IsMemeber = association.IsMemeber,
                GLPeriodId = association.GLPeriodId,
                Remark = association.Remark,
                DateCreated = association.DateCreated,
                LastModified = association.LastModified,
                CreatedBy = association.CreatedBy,
                ModifiedBy = association.ModifiedBy,
                ApplyToAll = association.ApplyToAll ,
                AmtPercnet = association.AmtPercnet,
                NetGross = association.NetGross ,
                Amount = association.Amount 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Associations_Create([DataSourceRequest]DataSourceRequest request, Association association)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                association.OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == association.GLPeriodId && association.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You cannot add this data. This period is already processed!!");
                }
                else
                {
                    if (association.AmtPercnet == FixedContPecntType.PERCENT && association.MonthlyAmount == 0)
                    {
                        ModelState.AddModelError("", "Percent Value should be greater than 0!!");
                    }
                    else if (association.AmtPercnet == FixedContPecntType.AMOUNT && association.Amount == 0)
                    {
                        ModelState.AddModelError("", "Amount Value should be greater than 0!!");
                    }
                    else
                    {
                        if (association.ApplyToAll)
                        
                        {
                            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                            IQueryable<Person_Employee> employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                           
                            foreach (var e in employees)
                            {
                                db = new ExceedDbContext();
                                var entity = new Association
                                {
                                    EmployeeId =e.EmployeeId,
                                    Type = association.Type,
                                    MonthlyAmount = association.MonthlyAmount,
                                    AdditionalAmount = association.AdditionalAmount,
                                    IsMemeber = association.IsMemeber,
                                    GLPeriodId = association.GLPeriodId,
                                    Remark = association.Remark,
                                    DateCreated = DateTime.Now,
                                    LastModified = association.LastModified,
                                    CreatedBy = User.Identity.Name,
                                    ModifiedBy = association.ModifiedBy,
                                    ApplyToAll = association.ApplyToAll,
                                    AmtPercnet = association.AmtPercnet,
                                    NetGross = association.NetGross ,
                                    Amount = association.Amount ,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                                };
                                if (association.AmtPercnet == FixedContPecntType.PERCENT)
                                {
                                    var basicsalary = db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                                    if (basicsalary != null)
                                    {
                                        decimal salary = basicsalary.Salary;
                                        if (association.NetGross == FixedContAmtType.FROM_NET)
                                        {

                                            decimal Pension = salary * Convert.ToDecimal(0.07);
                                            decimal beforeIncome = (decimal)XAPI.IncomeTax.GetIncomeTax((float)salary);
                                            entity.Amount = (salary - Pension - beforeIncome) * (entity.MonthlyAmount / 100);
                                        }
                                        else
                                        {
                                            entity.Amount = salary * (entity.MonthlyAmount / 100);
                                        }
                                        //if (association.Months != 0)
                                        //{
                                        //    entity.MonthlyAmount = entity.Amount / fixedContribution.Months;
                                        //    entity.Balance = entity.Amount;
                                        //}
                                    }
                                }
                                var check = db.Associations.Where(x => x.EmployeeId == entity.EmployeeId&& x.Type == entity .Type ).FirstOrDefault( );
                                if (check == null)
                                {
                                    db.Associations.Add(entity);
                                    db.SaveChanges();
                                    association.AssociationId = entity.AssociationId;

                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "payroll association", "Success", User.Identity.Name, $"added association type  {entity.Type} for employee {entity.EmployeeId} amount type {entity.AmtPercnet.ToString()} amount {entity.Amount} monthly {entity.MonthlyAmount} additional amount {entity.AdditionalAmount}", Modules.Payroll);
                                    #endregion
                                }
                                else

                                {
                                    ModelState.AddModelError("",
                                    "This Assocation type is already registered!!");
                                }
                            }
                        }
                        else
                        {
                            var checkexistancee = db.Associations.Where(x => x.EmployeeId == association.EmployeeId &&  x.Type == association.Type).FirstOrDefault();
                            if (checkexistancee != null)
                            {
                                ModelState.AddModelError("",
                                    "This Assocation type is already registered!!");
                            }
                            else

                            {
                                var entity = new Association
                                {
                                    EmployeeId = association.EmployeeId,
                                    Type = association.Type,
                                    MonthlyAmount = association.MonthlyAmount,
                                    AdditionalAmount = association.AdditionalAmount,
                                    IsMemeber = association.IsMemeber,
                                    GLPeriodId = association.GLPeriodId,
                                    Remark = association.Remark,
                                    DateCreated = DateTime.Now,
                                    LastModified = association.LastModified,
                                    CreatedBy = User.Identity.Name,
                                    ModifiedBy = association.ModifiedBy,
                                    ApplyToAll = association.ApplyToAll,
                                    AmtPercnet = association.AmtPercnet,
                                    NetGross = association.NetGross,
                                    Amount = association.Amount,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                };
                                if (association.AmtPercnet == FixedContPecntType.PERCENT)
                                {
                                    var basicsalary =
                                        db.Placements.Where(x => x.EmployeeId == association.EmployeeId && x.isCurrent)
                                            .FirstOrDefault();
                                    if (basicsalary != null)
                                    {
                                        decimal salary = basicsalary.Salary;
                                        if (association.NetGross == FixedContAmtType.FROM_NET)
                                        {

                                            decimal Pension = salary*Convert.ToDecimal(0.07);
                                            decimal beforeIncome = (decimal) XAPI.IncomeTax.GetIncomeTax((float) salary);
                                            entity.Amount = (salary - Pension - beforeIncome)*(entity.MonthlyAmount/100);
                                        }
                                        else
                                        {
                                            entity.Amount = salary*(entity.MonthlyAmount/100);
                                        }
                                        //if (fixedContribution.Months != 0)
                                        //{
                                        //    entity.MonthlyAmount = entity.Amount / fixedContribution.Months;
                                        //    entity.Balance = entity.Amount;
                                        //}
                                    }
                                }
                                db.Associations.Add(entity);
                                db.SaveChanges();
                                association.AssociationId = entity.AssociationId;
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll association", "Success", User.Identity.Name, $"added association type  {entity.Type} for employee {entity.EmployeeId} amount type {entity.AmtPercnet.ToString()} amount {entity.Amount} monthly {entity.MonthlyAmount} additional amount {entity.AdditionalAmount}", Modules.Payroll);
                                #endregion
                            }
                        }
                    }
                }

            }

            return Json(new[] { association }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Associations_Update([DataSourceRequest]DataSourceRequest request, Association association)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == association.EmployeeId && x.GLPeriodId == association.GLPeriodId && association.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    //var checkLoan = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.Name == association.Type.ToString()).FirstOrDefault();
                    //if (checkLoan != null)
                    //{
                        ModelState.AddModelError("", "You cannot update this data. It already processed!!");
                    }
                    else
                    {
                        if (association.MonthlyAmount <= 0 && association.AmtPercnet == FixedContPecntType.PERCENT)
                        {
                            ModelState.AddModelError("", "Monthly amount should be greater than 0!!");
                        }
                        else if (association.Amount <= 0 && association.AmtPercnet == FixedContPecntType.AMOUNT)
                        {
                            ModelState.AddModelError("", "Amount should be greater than 0!!");
                        }
                        else
                        {
                            if (association.ApplyToAll)
                            {
                                List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                                IQueryable<Person_Employee> employees =
                                    db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));

                                var allcontribution =
                                    db.Associations.Where(
                                        x =>
                                            x.GLPeriodId == association.GLPeriodId &&
                                            x.Type == association.Type).ToList();
                                if (allcontribution.Any())
                                {
                                    //  db.RegularContributions.Attach(allcontribution);
                                    db.Associations.RemoveRange(allcontribution);
                                    db.SaveChanges();
                                }
                                foreach (var e in employees)
                                {
                                    db = new ExceedDbContext();
                                    ExceedDbContext dbR = new ExceedDbContext();

                                    var entity = new Association
                                    {
                                        EmployeeId = e.EmployeeId,
                                        Type = association.Type,
                                        MonthlyAmount = association.MonthlyAmount,
                                        AdditionalAmount = association.AdditionalAmount,
                                        IsMemeber = association.IsMemeber,
                                        GLPeriodId = association.GLPeriodId,
                                        Remark = association.Remark,
                                        DateCreated = DateTime.Now,
                                        LastModified = association.LastModified,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = association.ModifiedBy,
                                        ApplyToAll = association.ApplyToAll,
                                        AmtPercnet = association.AmtPercnet,
                                        NetGross = association.NetGross ,
                                        Amount = association.Amount ,
                                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                                    };
                                    if (association.AmtPercnet == FixedContPecntType.PERCENT)
                                    {
                                        var basicsalary = db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                                        if (basicsalary != null)
                                        {
                                            decimal salary = basicsalary.Salary;
                                            if (association.NetGross == FixedContAmtType.FROM_NET)
                                            {

                                                decimal Pension = salary * Convert.ToDecimal(0.07);
                                                decimal beforeIncome = (decimal)XAPI.IncomeTax.GetIncomeTax((float)salary);
                                                entity.Amount = (salary - Pension - beforeIncome) * (entity.MonthlyAmount / 100);
                                            }
                                            else
                                            {
                                                entity.Amount = salary * (entity.MonthlyAmount / 100);
                                            }
                                            //if (association.Months != 0)
                                            //{
                                            //    entity.MonthlyAmount = entity.Amount / fixedContribution.Months;
                                            //    entity.Balance = entity.Amount;
                                            //}
                                        }
                                    }
                                    var check = db.Associations.Where(x => x.EmployeeId == entity.EmployeeId && x.GLPeriodId == entity.GLPeriodId && x.Type == entity.Type).FirstOrDefault();
                                    if (check == null)
                                    {
                                        db.Associations.Add(entity);
                                        db.SaveChanges();
                                        association.AssociationId = entity.AssociationId;
                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "payroll association", "Success", User.Identity.Name, $"updated association type  {entity.Type} for employee {entity.EmployeeId} amount type {entity.AmtPercnet.ToString()} amount {entity.Amount} monthly {entity.MonthlyAmount} additional amount {entity.AdditionalAmount}", Modules.Payroll);
                                    #endregion
                                }

                            }
                            }
                            else
                            {


                                var entity = new Association
                                {
                                    AssociationId = association.AssociationId,
                                    EmployeeId = association.EmployeeId,
                                    Type = association.Type,
                                    MonthlyAmount = association.MonthlyAmount,
                                    AdditionalAmount = association.AdditionalAmount,
                                    IsMemeber = association.IsMemeber,
                                    GLPeriodId = association.GLPeriodId,
                                    Remark = association.Remark,
                                    DateCreated = association.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = association.CreatedBy,
                                    ModifiedBy = User.Identity.Name,
                                    ApplyToAll = association.ApplyToAll,
                                    AmtPercnet = association.AmtPercnet,
                                    NetGross = association.NetGross ,
                                    Amount = association.Amount ,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                };

                                db.Associations.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll association", "Success", User.Identity.Name, $"updated association type  {entity.Type} for employee {entity.EmployeeId} amount type {entity.AmtPercnet.ToString()} amount {entity.Amount} monthly {entity.MonthlyAmount} additional amount {entity.AdditionalAmount}", Modules.Payroll);
                            #endregion
                        }
                    }
                    }
                }
                
            return Json(new[] { association }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Associations_Destroy([DataSourceRequest]DataSourceRequest request, Association association)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == association.EmployeeId && x.GLPeriodId == association.GLPeriodId && association.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    //var checkLoan = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.Name == association.Type.ToString ()).FirstOrDefault();
                    //if (checkLoan != null)
                    //{
                        ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                    }
                    else
                    {
                        var entity = new Association
                        {
                            AssociationId = association.AssociationId,
                            EmployeeId = association.EmployeeId,
                            Type = association.Type,
                            MonthlyAmount = association.MonthlyAmount,
                            AdditionalAmount = association.AdditionalAmount,
                            IsMemeber = association.IsMemeber,
                            GLPeriodId = association.GLPeriodId,
                            Remark = association.Remark,
                            DateCreated = association.DateCreated,
                            LastModified = association.LastModified,
                            CreatedBy = association.CreatedBy,
                            ModifiedBy = association.ModifiedBy,
                            ApplyToAll = association.ApplyToAll,
                            AmtPercnet = association.AmtPercnet,
                            NetGross = association.NetGross ,
                            Amount = association.Amount ,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };
                        #region delete log
                        UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted association information of employee " + association.EmployeeId,Modules.Payroll );
                        #endregion
                        db.Associations.Attach(entity);
                        db.Associations.Remove(entity);
                        db.SaveChanges();
                    }
                }
            //    else
            //    {
            //        var entity = new Association
            //        {
            //            AssociationId = association.AssociationId,
            //            EmployeeId = association.EmployeeId,
            //            Type = association.Type,
            //            MonthlyAmount = association.MonthlyAmount,
            //            AdditionalAmount = association.AdditionalAmount,
            //            IsMemeber = association.IsMemeber,
            //            GLPeriodId = association.GLPeriodId,
            //            Remark = association.Remark,
            //            DateCreated = association.DateCreated,
            //            LastModified = association.LastModified,
            //            CreatedBy = association.CreatedBy,
            //            ModifiedBy = association.ModifiedBy
            //        };

            //        db.Associations.Attach(entity);
            //        db.Associations.Remove(entity);
            //        db.SaveChanges();
            //    }

            //}

            return Json(new[] { association }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
