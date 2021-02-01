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
using System;
using System.Xml.Schema;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
      [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class FixedContributionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public decimal EmployeeContribution { get; set; }
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
            var ConrtibutionTypes = db.ContributionTypes.Where (x=> x.ContributionCategory ==   ContributionCategory.FIXED );
            ViewData["Contribution"] = ConrtibutionTypes;
         
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            var Pensions = db.Configurations.Where(x => x.Reference == "Pension").ToList();
            if (Pensions.Any())
            {
                var Employ = Pensions.Where(o => o.Attribute == PensionAttribute.EMPLOYEE_CONTRIBUTION.ToString())
                            .FirstOrDefault();

                if (Employ != null)
                {
                    EmployeeContribution= Convert.ToDecimal(Employ.CurrentValue);
                }
            }
            return View();
        }
   
        public ActionResult FixedContributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<FixedContribution> fixedcontributions = db.FixedContributions.Where(x => employees.Contains(x.EmployeeId.ToString())); 
            DataSourceResult result = fixedcontributions.ToDataSourceResult(request, fixedContribution => new {
                FixedContributionId = fixedContribution.FixedContributionId,
                EmployeeId = fixedContribution.EmployeeId,
                ContributionTypeId = fixedContribution.ContributionTypeId,
                Amount = fixedContribution.Amount,
                Balance = fixedContribution.Balance,
                Months = fixedContribution.Months,
                MonthlyAmount = fixedContribution.MonthlyAmount,
                GLPeriodId = fixedContribution.GLPeriodId,
                Remark = fixedContribution.Remark,
                DateCreated = fixedContribution.DateCreated,
                LastModified = fixedContribution.LastModified,
                CreatedBy = fixedContribution.CreatedBy,
                ModifiedBy = fixedContribution.ModifiedBy,
                ApplyToAll = fixedContribution.ApplyToAll,
                OrgBranchName = fixedContribution.OrgBranchName ,
                NetGross = fixedContribution.NetGross,
                AmtPercnet = fixedContribution.AmtPercnet,
                AmountPercent = fixedContribution.AmountPercent,
                IsActive = fixedContribution.IsActive
            });

            return Json(result);
        }
        private decimal GetSportFee(int employeeId, decimal BasicSalary)
        {
            var sports =
                                 db.Associations.Where(x => x.EmployeeId ==employeeId)
                                     .Where(x => x.IsMemeber == true)
                                     .Where(x => x.Type == AssocationType.SPORT_FEE)
                                     .ToList();
            decimal Totalsport = 0;
            decimal OneSport = 0;
            foreach (Association credit in sports)
            {
                OneSport = 0;
                if (credit.AmtPercnet == FixedContPecntType.PERCENT)
                {
                    if (credit.NetGross == FixedContAmtType.FROM_GROSS)
                    {
                        Totalsport = Totalsport + credit.AdditionalAmount +
                                       (BasicSalary * credit.MonthlyAmount / 100);
                        OneSport = credit.AdditionalAmount +
                                       (BasicSalary * credit.MonthlyAmount / 100);
                    }
                    if (credit.NetGross == FixedContAmtType.FROM_NET)
                    {
                        decimal Pension = BasicSalary * Convert.ToDecimal((EmployeeContribution / 100));
                        decimal beforeIncome =
                            (decimal)XAPI.IncomeTax.GetIncomeTax((float)(BasicSalary));
                        Totalsport = Totalsport + credit.AdditionalAmount +
                                       ((BasicSalary - Pension - beforeIncome) *
                                        (credit.MonthlyAmount / 100));
                        OneSport = credit.AdditionalAmount +
                                       ((BasicSalary - Pension - beforeIncome) *
                                        (credit.MonthlyAmount / 100));
                    }
                }
                else
                {
                    Totalsport = Totalsport + credit.Amount;
                    OneSport = credit.Amount;
                }


                
            }
            return Totalsport;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedContributions_Create([DataSourceRequest]DataSourceRequest request, FixedContribution fixedContribution)
        {
          //  List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
           
            if (fixedContribution.Months != 0)
            {
                fixedContribution.MonthlyAmount = fixedContribution.Amount / fixedContribution.Months;
                fixedContribution.Balance = fixedContribution.Amount;
            }
           
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                fixedContribution.OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name) ;
                var checkexistancep = db.PayrollProcessViewModel
                    .FirstOrDefault(x => x.GLPeriodId == fixedContribution.GLPeriodId && fixedContribution.OrgBranchName == x.BranchId.ToString());
                if (checkexistancep != null)
                {


                    ModelState.AddModelError("", "You cannot add this data. This period is already processed!!");
                }
                else
                {
                    if (fixedContribution.Months == 0)
                    {
                        ModelState.AddModelError("", "Months Value should be greater than 0!!");
                    }



                    else if (fixedContribution.AmtPercnet == FixedContPecntType.PERCENT && fixedContribution.AmountPercent == 0)
                    {
                        ModelState.AddModelError("", "Percent Value should be greater than 0!!");
                    }
                    else if (fixedContribution.AmtPercnet == FixedContPecntType.AMOUNT && fixedContribution.Amount == 0)
                    {
                        ModelState.AddModelError("", "Percent Value should be greater than 0!!");
                    }
                    else
                    {
                        if (fixedContribution.ApplyToAll)
                        {
                            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                            IQueryable<Person_Employee> employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                          

                            var allcontribution = db.FixedContributions.Where(x => x.GLPeriodId == fixedContribution.GLPeriodId && x.ContributionTypeId == fixedContribution.ContributionTypeId).ToList();
                            if (allcontribution.Any())
                            {
                                //  db.RegularContributions.Attach(allcontribution);
                                db.FixedContributions.RemoveRange(allcontribution);
                                db.SaveChanges();
                            }
                            foreach (var e in employees)
                            {
                                db = new ExceedDbContext();
                                var entity = new FixedContribution
                                {
                                    EmployeeId = e.EmployeeId,
                                    ContributionTypeId = fixedContribution.ContributionTypeId,
                                    Amount = fixedContribution.Amount,
                                    GLPeriodId = fixedContribution.GLPeriodId,
                                    Balance = fixedContribution.Balance,
                                    MonthlyAmount = fixedContribution.MonthlyAmount,
                                    Months = fixedContribution.Months,
                                    Remark = fixedContribution.Remark,
                                    DateCreated = DateTime.Now,
                                    LastModified = fixedContribution.LastModified,
                                    CreatedBy = User.Identity.Name,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                    ModifiedBy = fixedContribution.ModifiedBy,
                                    ApplyToAll = fixedContribution.ApplyToAll,
                                    NetGross = fixedContribution.NetGross,
                                    AmtPercnet = fixedContribution.AmtPercnet,
                                    AmountPercent = fixedContribution.AmountPercent,
                                    IsActive = fixedContribution.IsActive
                                };
                                if (fixedContribution.AmtPercnet == FixedContPecntType.PERCENT)
                                {
                                    var basicsalary = db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                                    if (basicsalary != null)
                                    {
                                        decimal salary = basicsalary.Salary;
                                        decimal redCross = 0;
                                        decimal sportFee = GetSportFee(entity.EmployeeId, basicsalary.Salary);
                                        var contribution = db.ContributionTypes.Where(x => x.Description == "Red Cross").FirstOrDefault();
                                        if (contribution!=null)
                                        {
                                            var regural = db.RegularContributions.Where(x => x.ContributionTypeId == contribution.ContributionTypeId && x.EmployeeId == entity.EmployeeId && x.IsActive).FirstOrDefault();
                                            if (regural!=null)
                                            {
                                                redCross = regural.AdditionalAmount +
                                                                         (basicsalary.Salary * regural.Amount / 100);
                                            }
                                        }
                                        if (fixedContribution.NetGross == FixedContAmtType.FROM_NET)
                                        {

                                            decimal Pension = salary * Convert.ToDecimal(EmployeeContribution/100);
                                            decimal beforeIncome = (decimal)XAPI.IncomeTax.GetIncomeTax((float)salary);
                                            entity.Amount = (salary - Pension - beforeIncome - redCross-sportFee ) * (entity.AmountPercent / 100);
                                        }
                                        else
                                        {
                                            entity.Amount = salary * (entity.AmountPercent / 100);
                                        }
                                        if (fixedContribution.Months != 0)
                                        {
                                            entity.MonthlyAmount = entity.Amount / fixedContribution.Months;
                                            entity.Balance = entity.Amount;
                                        }
                                    }
                                }
                                db.FixedContributions.Add(entity);
                                db.SaveChanges();
                                fixedContribution.FixedContributionId = entity.FixedContributionId;
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"added contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                #endregion
                            }
                        }

                        else
                        {
                            var checkexistance = db.FixedContributions.Where(x => x.EmployeeId == fixedContribution.EmployeeId && x.GLPeriodId == fixedContribution.GLPeriodId && x.ContributionTypeId == fixedContribution.ContributionTypeId).FirstOrDefault();
                            if (checkexistance != null)
                            {
                                ModelState.AddModelError("", "This Fixed contribution type  on this period is already registered!!");
                            }
                            else
                            {
                                var entity = new FixedContribution
                                {
                                    EmployeeId = fixedContribution.EmployeeId,
                                    ContributionTypeId = fixedContribution.ContributionTypeId,
                                    Amount = fixedContribution.Amount,
                                    GLPeriodId = fixedContribution.GLPeriodId,
                                    Balance = fixedContribution.Balance,
                                    MonthlyAmount = fixedContribution.MonthlyAmount,
                                    Months = fixedContribution.Months,
                                    Remark = fixedContribution.Remark,
                                    DateCreated = DateTime.Now,
                                    LastModified = fixedContribution.LastModified,
                                    CreatedBy = User.Identity.Name,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                    ModifiedBy = fixedContribution.ModifiedBy,
                                    ApplyToAll = fixedContribution.ApplyToAll,
                                    NetGross = fixedContribution.NetGross,
                                    AmtPercnet = fixedContribution.AmtPercnet,
                                    AmountPercent = fixedContribution.AmountPercent,
                                    IsActive = fixedContribution.IsActive
                                };
                                if (fixedContribution.AmtPercnet == FixedContPecntType.PERCENT)
                                {
                                    var basicsalary = db.Placements.Where(x => x.EmployeeId == fixedContribution.EmployeeId && x.isCurrent).FirstOrDefault();
                                    if (basicsalary != null)
                                    {
                                        decimal salary = basicsalary.Salary;
                                        decimal redCross = 0;
                                        decimal sportFee = GetSportFee(entity.EmployeeId, basicsalary.Salary);
                                        var contribution = db.ContributionTypes.Where(x => x.Description == "Red Cross").FirstOrDefault();
                                        if (contribution != null)
                                        {
                                            var regural = db.RegularContributions.Where(x => x.ContributionTypeId == contribution.ContributionTypeId && x.EmployeeId == entity.EmployeeId && x.IsActive).FirstOrDefault();
                                            if (regural != null)
                                            {
                                              redCross=  regural.AdditionalAmount +
                                                                         (basicsalary.Salary * regural.Amount / 100);
                                              
                                            }
                                        }
                                        if (fixedContribution.NetGross == FixedContAmtType.FROM_NET)
                                        {

                                            decimal Pension = salary * Convert.ToDecimal(EmployeeContribution /100);
                                            decimal beforeIncome = (decimal)XAPI.IncomeTax.GetIncomeTax((float)salary);
                                            entity.Amount = (salary - Pension - beforeIncome - redCross-sportFee ) * (entity.AmountPercent / 100);
                                        }
                                        else
                                        {
                                            entity.Amount = salary * (entity.AmountPercent / 100);
                                        }
                                        if (fixedContribution.Months != 0)
                                        {
                                            entity.MonthlyAmount = entity.Amount / fixedContribution.Months;
                                            entity.Balance = entity.Amount;
                                        }
                                    }
                                }
                                db.FixedContributions.Add(entity);
                                db.SaveChanges();
                                fixedContribution.FixedContributionId = entity.FixedContributionId;
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"added contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                #endregion
                            }
                        }
                    }
                }
            }

            return Json(new[] { fixedContribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedContributions_Update([DataSourceRequest]DataSourceRequest request, FixedContribution fixedContribution)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                if (fixedContribution.Months != 0)
                {
                    fixedContribution.MonthlyAmount = fixedContribution.Amount/fixedContribution.Months;
                    fixedContribution.Balance = fixedContribution.Amount;
                }
                if (fixedContribution.Months == 0)
                {
                    ModelState.AddModelError("", "Months Value should be greater than 0!!");
                }
                else
                {
                    var checkLoanS =
                        db.FixedContributions.Where(x => x.FixedContributionId == fixedContribution.FixedContributionId)
                            .FirstOrDefault();
                    if (checkLoanS != null)
                    {
                        if (checkLoanS.IsActive != fixedContribution.IsActive)
                        {
                            if (fixedContribution.Months != 0)
                            {
                                fixedContribution.MonthlyAmount = fixedContribution.Amount/fixedContribution.Months;
                                fixedContribution.Balance = fixedContribution.Amount;
                            }
                            if (fixedContribution.ApplyToAll)
                            {
                                List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                                IQueryable<Person_Employee> employees =
                                    db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                                ;
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
                                var allcontribution =
                                    db.FixedContributions.Where(
                                        x =>
                                            x.GLPeriodId == fixedContribution.GLPeriodId &&
                                            x.ContributionTypeId == fixedContribution.ContributionTypeId).ToList();
                                if (allcontribution.Any())
                                {
                                    //  db.RegularContributions.Attach(allcontribution);
                                    db.FixedContributions.RemoveRange(allcontribution);
                                    db.SaveChanges();
                                }
                                foreach (var e in employees)
                                {
                                    db = new ExceedDbContext();
                                    ExceedDbContext dbR = new ExceedDbContext();

                                    var entity = new FixedContribution
                                    {
                                        FixedContributionId = fixedContribution.FixedContributionId,
                                        EmployeeId = e.EmployeeId,
                                        ContributionTypeId = fixedContribution.ContributionTypeId,
                                        Amount = fixedContribution.Amount,
                                        GLPeriodId = fixedContribution.GLPeriodId,
                                        Months = fixedContribution.Months,
                                        Balance = fixedContribution.Balance,
                                        MonthlyAmount = fixedContribution.MonthlyAmount,
                                        Remark = fixedContribution.Remark,
                                        DateCreated = fixedContribution.DateCreated,
                                        LastModified = DateTime.Now,
                                        CreatedBy = fixedContribution.CreatedBy,
                                        ModifiedBy = User.Identity.Name,
                                        ApplyToAll = fixedContribution.ApplyToAll,
                                        OrgBranchName =
                                            UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                        NetGross = fixedContribution.NetGross,
                                        AmtPercnet = fixedContribution.AmtPercnet,
                                        AmountPercent = fixedContribution.AmountPercent,
                                        IsActive = fixedContribution.IsActive

                                    };
                                    if (fixedContribution.AmtPercnet == FixedContPecntType.PERCENT)
                                    {
                                        var basicsalary =
                                            db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent)
                                                .FirstOrDefault();
                                        if (basicsalary != null)
                                        {
                                            decimal salary = basicsalary.Salary;
                                            decimal redCross = 0;
                                            decimal sportFee = GetSportFee(entity.EmployeeId, basicsalary.Salary);
                                            var contribution = db.ContributionTypes.Where(x => x.Description == "Red Cross").FirstOrDefault();
                                            if (contribution != null)
                                            {
                                                var regural = db.RegularContributions.Where(x => x.ContributionTypeId == contribution.ContributionTypeId && x.EmployeeId == entity.EmployeeId && x.IsActive).FirstOrDefault();
                                                if (regural != null)
                                                {
                                                    redCross = regural.AdditionalAmount +
                                                                        (basicsalary.Salary * regural.Amount / 100);
                                                }
                                            }
                                            if (fixedContribution.NetGross == FixedContAmtType.FROM_NET)
                                            {

                                                decimal Pension = salary*Convert.ToDecimal(EmployeeContribution /100);
                                                decimal beforeIncome =
                                                    (decimal) XAPI.IncomeTax.GetIncomeTax((float) salary);
                                                entity.Amount = (salary - Pension - beforeIncome-redCross -sportFee )*
                                                                (entity.AmountPercent/100);
                                            }
                                            else
                                            {
                                                entity.Amount = salary*(entity.AmountPercent/100);
                                            }
                                            if (fixedContribution.Months != 0)
                                            {
                                                entity.MonthlyAmount = entity.Amount/fixedContribution.Months;
                                                entity.Balance = entity.Amount;
                                            }
                                        }
                                    }
                                    db.FixedContributions.Add(entity);
                                    db.SaveChanges();
                                    fixedContribution.FixedContributionId = entity.FixedContributionId;
                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                    #endregion

                                }
                            }
                            else
                            {
                                var entity = new FixedContribution
                                {
                                    FixedContributionId = fixedContribution.FixedContributionId,
                                    EmployeeId = fixedContribution.EmployeeId,
                                    ContributionTypeId = fixedContribution.ContributionTypeId,
                                    Amount = fixedContribution.Amount,
                                    GLPeriodId = fixedContribution.GLPeriodId,
                                    Months = fixedContribution.Months,
                                    Balance = fixedContribution.Balance,
                                    MonthlyAmount = fixedContribution.MonthlyAmount,
                                    Remark = fixedContribution.Remark,
                                    DateCreated = fixedContribution.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = fixedContribution.CreatedBy,
                                    ModifiedBy = User.Identity.Name,
                                    ApplyToAll = fixedContribution.ApplyToAll,
                                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                    NetGross = fixedContribution.NetGross,
                                    AmtPercnet = fixedContribution.AmtPercnet,
                                    AmountPercent = fixedContribution.AmountPercent,
                                    IsActive = fixedContribution.IsActive,
                                    
                                };
                                db = new ExceedDbContext();
                                db.FixedContributions.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                db.SaveChanges();
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                #endregion
                            }
                        }
                        else

                        {


                            var checkexistance =
                                db.PayrollProcessViewModel.Where(
                                    x =>
                                        x.EmployeeId == fixedContribution.EmployeeId &&
                                        x.GLPeriodId == fixedContribution.GLPeriodId &&
                                        fixedContribution.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                            if (checkexistance != null)
                            {

                                ModelState.AddModelError("", "You cannot update this data. It already processed!!");
                            }


                            else
                            {

                                if (fixedContribution.Months != 0)
                                {
                                    fixedContribution.MonthlyAmount = fixedContribution.Amount/fixedContribution.Months;
                                    fixedContribution.Balance = fixedContribution.Amount;
                                }
                                if (fixedContribution.ApplyToAll)
                                {
                                    List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
                                    IQueryable<Person_Employee> employees =
                                        db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString()));
                                    ;
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
                                    var allcontribution =
                                        db.FixedContributions.Where(
                                            x =>
                                                x.GLPeriodId == fixedContribution.GLPeriodId &&
                                                x.ContributionTypeId == fixedContribution.ContributionTypeId).ToList();
                                    if (allcontribution.Any())
                                    {
                                        //  db.RegularContributions.Attach(allcontribution);
                                        db.FixedContributions.RemoveRange(allcontribution);
                                        db.SaveChanges();
                                    }
                                    foreach (var e in employees)
                                    {
                                        db = new ExceedDbContext();
                                        ExceedDbContext dbR = new ExceedDbContext();

                                        var entity = new FixedContribution
                                        {
                                            FixedContributionId = fixedContribution.FixedContributionId,
                                            EmployeeId = e.EmployeeId,
                                            ContributionTypeId = fixedContribution.ContributionTypeId,
                                            Amount = fixedContribution.Amount,
                                            GLPeriodId = fixedContribution.GLPeriodId,
                                            Months = fixedContribution.Months,
                                            Balance = fixedContribution.Balance,
                                            MonthlyAmount = fixedContribution.MonthlyAmount,
                                            Remark = fixedContribution.Remark,
                                            DateCreated = fixedContribution.DateCreated,
                                            LastModified = DateTime.Now,
                                            CreatedBy = fixedContribution.CreatedBy,
                                            ModifiedBy = User.Identity.Name,
                                            ApplyToAll = fixedContribution.ApplyToAll,
                                            OrgBranchName =
                                                UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                            NetGross = fixedContribution.NetGross,
                                            AmtPercnet = fixedContribution.AmtPercnet,
                                            AmountPercent = fixedContribution.AmountPercent,
                                            IsActive = fixedContribution.IsActive

                                        };
                                        if (fixedContribution.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            var basicsalary =
                                                db.Placements.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent)
                                                    .FirstOrDefault();
                                            if (basicsalary != null)
                                            {
                                                decimal salary = basicsalary.Salary;
                                                decimal redCross = 0;
                                                decimal sportFee = GetSportFee(entity.EmployeeId, basicsalary.Salary);
                                                var contribution = db.ContributionTypes.Where(x => x.Description == "Red Cross").FirstOrDefault();
                                                if (contribution != null)
                                                {
                                                    var regural = db.RegularContributions.Where(x => x.ContributionTypeId == contribution.ContributionTypeId && x.EmployeeId == entity.EmployeeId && x.IsActive).FirstOrDefault();
                                                    if (regural != null)
                                                    {
                                                        redCross = regural.AdditionalAmount +
                                                                         (basicsalary.Salary * regural.Amount / 100);
                                                    }
                                                }
                                                if (fixedContribution.NetGross == FixedContAmtType.FROM_NET)
                                                {

                                                    decimal Pension = salary*Convert.ToDecimal(EmployeeContribution /100);
                                                    decimal beforeIncome =
                                                        (decimal) XAPI.IncomeTax.GetIncomeTax((float) salary);
                                                    entity.Amount = (salary - Pension - beforeIncome-redCross -sportFee)*
                                                                    (entity.AmountPercent/100);
                                                }
                                                else
                                                {
                                                    entity.Amount = salary*(entity.AmountPercent/100);
                                                }
                                                if (fixedContribution.Months != 0)
                                                {
                                                    entity.MonthlyAmount = entity.Amount/fixedContribution.Months;
                                                    entity.Balance = entity.Amount;
                                                }
                                            }
                                        }
                                        db.FixedContributions.Add(entity);
                                        db.SaveChanges();
                                        fixedContribution.FixedContributionId = entity.FixedContributionId;
                                        #region operation log
                                        UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                        #endregion

                                    }
                                }
                                else
                                {
                                    var entity = new FixedContribution
                                    {
                                        FixedContributionId = fixedContribution.FixedContributionId,
                                        EmployeeId = fixedContribution.EmployeeId,
                                        ContributionTypeId = fixedContribution.ContributionTypeId,
                                        Amount = fixedContribution.Amount,
                                        GLPeriodId = fixedContribution.GLPeriodId,
                                        Months = fixedContribution.Months,
                                        Balance = fixedContribution.Balance,
                                        MonthlyAmount = fixedContribution.MonthlyAmount,
                                        Remark = fixedContribution.Remark,
                                        DateCreated = fixedContribution.DateCreated,
                                        LastModified = DateTime.Now,
                                        CreatedBy = fixedContribution.CreatedBy,
                                        ModifiedBy = User.Identity.Name,
                                        ApplyToAll = fixedContribution.ApplyToAll,
                                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                        NetGross = fixedContribution.NetGross,
                                        AmtPercnet = fixedContribution.AmtPercnet,
                                        AmountPercent = fixedContribution.AmountPercent,
                                        IsActive = fixedContribution.IsActive
                                    };
                                    db = new ExceedDbContext();
                                    db.FixedContributions.Attach(entity);
                                    db.Entry(entity).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "payroll fixed contribution", "Success", User.Identity.Name, $"updated contribution  {entity.ContributionType} for employee {entity.EmployeeId} amount {entity.Amount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} type {entity.NetGross}", Modules.Payroll);
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }

            return Json(new[] { fixedContribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedContributions_Destroy([DataSourceRequest]DataSourceRequest request, FixedContribution fixedContribution)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == fixedContribution.EmployeeId && x.GLPeriodId == fixedContribution.GLPeriodId && fixedContribution.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    //var checkLoan = db.PayrollProcessFixedContribution.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.FixedContributionId == fixedContribution.FixedContributionId).FirstOrDefault();
                    //if (checkLoan != null)
                    //{
                        ModelState.AddModelError("", "You cannot delete this data. It already processed!!");
                    }
                    else
                    {
                var entity = new FixedContribution
                {
                    FixedContributionId = fixedContribution.FixedContributionId,
                    EmployeeId = fixedContribution.EmployeeId,
                    ContributionTypeId = fixedContribution.ContributionTypeId,
                    Amount = fixedContribution.Amount,
                    GLPeriodId = fixedContribution.GLPeriodId,
                    Balance = fixedContribution.Balance,
                    Months = fixedContribution.Months,
                    MonthlyAmount = fixedContribution.MonthlyAmount,
                    Remark = fixedContribution.Remark,
                    DateCreated = fixedContribution.DateCreated,
                    LastModified = fixedContribution.LastModified,
                    CreatedBy = fixedContribution.CreatedBy,
                    ModifiedBy = fixedContribution.ModifiedBy,
                    ApplyToAll = fixedContribution.ApplyToAll,
                    OrgBranchName = fixedContribution.OrgBranchName ,
                    NetGross = fixedContribution.NetGross,
                    AmtPercnet = fixedContribution.AmtPercnet,
                    AmountPercent = fixedContribution.AmountPercent,
                    IsActive = fixedContribution.IsActive
                };
                #region delete log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee fixed contribution information of employee " + fixedContribution.EmployeeId + " and period " + fixedContribution.GLPeriodId,Modules.Payroll);
                #endregion
                db.FixedContributions.Attach(entity);
                db.FixedContributions.Remove(entity);
                db.SaveChanges();
            }
                }
            //    else
            //    {
            //        var entity = new FixedContribution
            //        {
            //            FixedContributionId = fixedContribution.FixedContributionId,
            //            EmployeeId = fixedContribution.EmployeeId,
            //            ContributionTypeId = fixedContribution.ContributionTypeId,
            //            Amount = fixedContribution.Amount,
            //            GLPeriodId = fixedContribution.GLPeriodId,
            //            Balance = fixedContribution.Balance,
            //            Months = fixedContribution.Months,
            //            MonthlyAmount = fixedContribution.MonthlyAmount,
            //            Remark = fixedContribution.Remark,
            //            DateCreated = fixedContribution.DateCreated,
            //            LastModified = fixedContribution.LastModified,
            //            CreatedBy = fixedContribution.CreatedBy,
            //            ModifiedBy = fixedContribution.ModifiedBy,
            //            ApplyToAll = fixedContribution.ApplyToAll,
            //            OrgBranchName = fixedContribution.OrgBranchName ,
            //            NetGross = fixedContribution.NetGross,
            //            AmtPercnet = fixedContribution.AmtPercnet
            //        };

            //        db.FixedContributions.Attach(entity);
            //        db.FixedContributions.Remove(entity);
            //        db.SaveChanges();
            //    }
            //}

            return Json(new[] { fixedContribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
