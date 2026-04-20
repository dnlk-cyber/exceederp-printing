using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin + "," + Payroll_Process + "," + Payroll_View + "," + Payroll_Distribute + "," + Payroll_Close + "," + Payroll_Transfer)]
    public class BackPaymentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private PayrollHelper payrollHelper = new PayrollHelper();
        private const string Payroll_Process = "Payroll_Process";
        private const string Payroll_Distribute = "Payroll_Distribute";
        private const string Payroll_Close = "Payroll_Close";
        private const string Payroll_Admin = "Payroll_Admin";
        private const string Payroll_View = "Payroll_View";
        private const string Payroll_Transfer = "Payroll_Transfer";
        HRMInputs inputs = new HRMInputs();

        public decimal EmployeeContribution { get; private set; }
        public decimal CompanyContribution { get; private set; }

        public ActionResult Index()
        {
            var periodList = db.PayrollPeriods;
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = listt.Select(x => x.g).ToList();

            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            GetBranch();
            if (GlobalHelper.IsPayrollModebranchAndBusinessUnit())
            {
                GetBusinessUnits();
            }
            return View();
        }
        public ActionResult GetBranch()
        {
           var mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId));
            ViewData["branches"] = branches.Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.BranchId
              }).AsQueryable(); ;
            return Json(branches.Select(p => new { BranchId = p.BranchId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBusinessUnits()
        {
            List<int> myBusinessUnit = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
            var bizUnits = db.BusinessUnits.Where(x => myBusinessUnit.Contains(x.BusinessUnitId)).ToList();
            var bizUnitdisplay = bizUnits.ToList();

            bizUnitdisplay.Add(new BusinessUnit
            {
                BusinessUnitId = 0,
                Name = "Not set"
            });
            ViewData["businesUnits"] = bizUnitdisplay.Select(
              i => new
              {
                  Name = i.Name,
                  BusinessUnitId = i.BusinessUnitId
              }).AsQueryable(); ;
            return Json(bizUnits.Select(p => new { BusinessUnitId = p.BusinessUnitId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriod()
        {
            FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();
            int curentYearId = fiscalYearHelper.GetCurrentFYearId();
            int prevYearId = fiscalYearHelper.GetPreviousFYearId(curentYearId);
            var list = db.GLPeriods.Where(x => x.GlFiscalYearId == curentYearId || x.GlFiscalYearId == prevYearId).OrderByDescending(x => x.DateFrom);

            return Json(list.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult BackPays_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> employees = inputs.GetUserBranchAndBusinessUnitEmployees(User.Identity.Name); //inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<BackPay> BackPays = db.BackPays;
            DataSourceResult result = BackPays.ToDataSourceResult(request, backPay => new
            {
                BackPayId = backPay.BackPayId,
                GLPeriodId = backPay.GLPeriodId,
                ToGLPeriodId = backPay.ToGLPeriodId,
                ProcessedGLPeriodId = backPay.ProcessedGLPeriodId,
                BusinessUnitId = backPay.BusinessUnitId,
                BranchId = backPay.BranchId,
                Remark = backPay.Remark,
                Date = backPay.Date,
                IsClosed = backPay.IsClosed,
                DateCreated = backPay.DateCreated,
                LastModified = backPay.LastModified,
                CreatedBy = backPay.CreatedBy,
                OrgBranchName = backPay.OrgBranchName,
                ModifiedBy = backPay.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPays_Create([DataSourceRequest] DataSourceRequest request, BackPay backPay)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPay
                {
                    GLPeriodId = backPay.GLPeriodId,
                    ToGLPeriodId = backPay.ToGLPeriodId,
                    BusinessUnitId = backPay.BusinessUnitId,
                    BranchId = backPay.BranchId,
                    Date = backPay.Date,
                    Remark = backPay.Remark,
                    IsClosed = backPay.IsClosed,
                    DateCreated = DateTime.Now,
                    LastModified = backPay.LastModified,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = backPay.ModifiedBy
                };

                db.BackPays.Add(entity);
                db.SaveChanges();
                backPay.BackPayId = entity.BackPayId;
            }

            return Json(new[] { backPay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPays_Update([DataSourceRequest] DataSourceRequest request, BackPay backPay)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPay
                {
                    BackPayId = backPay.BackPayId,
                    GLPeriodId = backPay.GLPeriodId,
                    ToGLPeriodId = backPay.ToGLPeriodId,
                    BusinessUnitId = backPay.BusinessUnitId,
                    BranchId = backPay.BranchId,
                    Remark = backPay.Remark,
                    Date = backPay.Date,
                    IsClosed = backPay.IsClosed,
                    DateCreated = backPay.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = backPay.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = backPay.OrgBranchName,
                };

                db.BackPays.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { backPay }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPays_Destroy([DataSourceRequest] DataSourceRequest request, BackPay backPay)
        {
            if (backPay.IsClosed)
                ModelState.AddModelError("delete", "You can't delete a closed transaction.");

            if (ModelState.IsValid)
            {
                var entity = new BackPay
                {
                    BackPayId = backPay.BackPayId,
                    GLPeriodId = backPay.GLPeriodId,
                    ToGLPeriodId = backPay.ToGLPeriodId,
                    BusinessUnitId = backPay.BusinessUnitId,
                    BranchId = backPay.BranchId,
                    Date = backPay.Date,
                    Remark = backPay.Remark,
                    DateCreated = backPay.DateCreated,
                    LastModified = backPay.LastModified,
                    CreatedBy = backPay.CreatedBy,
                    OrgBranchName = backPay.OrgBranchName,
                    ModifiedBy = backPay.ModifiedBy
                };
                #region delete log
                // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee loan information of employee " + loan.EmployeeId + " and period " + loan.BackPayId);
                #endregion
                db.BackPays.Attach(entity);
                db.BackPays.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { backPay }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult ProcessBackPay(int period, int toperiod, int processperiod, int branchid, int businessUnitid)
        {

            if (period != processperiod && toperiod != processperiod)
            {
                // All values are different — valid case
            }
            else
            {
                var formattedData2 = new
                {
                    Message = "The processed period should be different from both.",
                    Status = "NOT_OK"
                };
                return Json(formattedData2, JsonRequestBehavior.AllowGet);
            }


            var periodObj = db.GLPeriods.Find(period);
            var toPeriodObj = db.GLPeriods.Find(toperiod);
            var processPeriodObj = db.GLPeriods.Find(processperiod);

            if (periodObj == null || toPeriodObj == null || processPeriodObj == null)
            {
                var formattedData2 = new
                {
                    Message = "The periods are not found",
                    Status = "NOT_OK"
                };
                return Json(formattedData2, JsonRequestBehavior.AllowGet);
            }
            if (periodObj.DateFrom > toPeriodObj.DateFrom)
            {
                var formattedData1 = new
                {
                    Message = "The 'period to' should be after 'period from'",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            if (toPeriodObj.DateFrom >= processPeriodObj.DateFrom)
            {
                var formattedData1 = new
                {
                    Message = "The 'Processed period' should be after 'period to' ",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            #region allowance rule check
            // Get all AllowanceType enum values
            var allEnumValues = Enum.GetValues(typeof(AllowanceType))
                                    .Cast<AllowanceType>()
                                    .ToList();

            // Get existing AllowanceTypes from DB
            var existingTypes = db.AllowanceTypeRule
                                  .Select(r => r.AllowanceType)
                                  .ToList();

            // Find missing enum values not in DB
            var missingTypes = allEnumValues.Except(existingTypes).ToList();

            if (missingTypes.Any())
            {

                var formattedData1 = new
                {
                    Message = "Please Set Allowance Rule For " + missingTypes.FirstOrDefault().ToString(),
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }


            #endregion

            GetPensionSetting();
            decimal salaryDiff = 0;
            decimal incomeTaxDiff = 0;
            decimal pensionDiff = 0;
            decimal pensionCompDiff = 0;
            decimal totalSalaryDifference = 0;
            decimal totalIncomeTax = 0;
            decimal totalEmpPension = 0;
            decimal totalCompPension = 0;

            var activeEmployees = payrollHelper.GetEmployees(branchid, businessUnitid).ToList();

            int workDays = 30;


            var coveredPeriods = db.GLPeriods
                   .Where(x => x.DateFrom >= periodObj.DateFrom && x.DateFrom <= toPeriodObj.DateFrom)
                   .OrderBy(x => x.DateFrom).ToList();

            //remove exisiting
            var back = db.BackPays
                .Where(x => x.GLPeriodId == period && x.ToGLPeriodId == toperiod && x.BranchId == branchid && x.BusinessUnitId == businessUnitid).FirstOrDefault();

            if (back != null)
            {
                db.BackPays.Remove(back);
                db.SaveChanges();
            }

            #region create header
            var backpay = new BackPay
            {
                GLPeriodId = period,
                ToGLPeriodId = toperiod,
                ProcessedGLPeriodId=processperiod,
                BranchId = branchid,
                BusinessUnitId = businessUnitid,
                Date = DateTime.Today,
                IsClosed = false,
            };
            db.BackPays.Add(backpay);
            db.SaveChanges();
            #endregion

            foreach (var e in activeEmployees)
            {
                salaryDiff = 0;
                incomeTaxDiff = 0;
                pensionDiff = 0;
                pensionCompDiff = 0;
                totalSalaryDifference = 0;
                totalIncomeTax = 0;
                totalEmpPension = 0;
                totalCompPension = 0;

                //var line = new BackPaymentLine
                //{
                //    BackPay = backpay,
                //    EmployeeId = e.PersonId,
                //    BasicSalary = totalSalaryDifference,
                //    Pension = totalEmpPension,
                //    IncomeTax = totalIncomeTax,
                //    Net = totalEmpPension,

                //};

                //db.BackPaymentLines.Add(line);
                //db.SaveChanges();

                var lineVm = new BackPaymentLineVm
                {
                    EmployeeId = e.PersonId,
                    BackPaymentDetailVms = new List<BackPaymentDetailVm>()
                };



                foreach (var p in coveredPeriods)
                {
                    workDays = 30; //default work days in a month

                    var processed = db.PayrollProcessViewModel
                        .Where(x => x.EmployeeId == e.PersonId)
                        .Where(x => x.GLPeriodId == p.GLPeriodId).FirstOrDefault();

                    if (processed != null)
                    {
                        if (e.PlacmentDate <= p.DateTo && e.PlacmentDate >= p.DateFrom)
                            workDays = (e.PlacmentDate - p.DateFrom).Days;

                        else if (e.PlacmentDate > p.DateTo)
                            continue;

                        salaryDiff = (e.BasicSalary - processed.OriginalBasicSalary); // * (workDays / 30);
                        totalSalaryDifference = totalSalaryDifference + salaryDiff;
                        incomeTaxDiff = (decimal)XAPI.IncomeTax.GetIncomeTax((float)(e.BasicSalary))- (decimal)XAPI.IncomeTax.GetIncomeTax((float)(processed.OriginalBasicSalary));
                        totalIncomeTax = totalIncomeTax + incomeTaxDiff;
                        pensionDiff = (e.BasicSalary * Convert.ToDecimal((EmployeeContribution / 100)))- (processed.OriginalBasicSalary * Convert.ToDecimal((EmployeeContribution / 100)));
                        totalEmpPension = totalEmpPension + pensionDiff;
                        pensionCompDiff = (e.BasicSalary * Convert.ToDecimal((CompanyContribution / 100))) - (processed.OriginalBasicSalary * Convert.ToDecimal((CompanyContribution / 100))); ;
                        totalCompPension = totalCompPension + pensionCompDiff;

                        if (salaryDiff > 0)
                        {
                            #region salary difference
                            var detail = new BackPaymentDetailVm
                            {
                                GLPeriodId = p.GLPeriodId,
                                Description = $"Salary Arrears {p.Name}",
                                Type = DetailType.Addition,
                                Amount = salaryDiff,

                            };

                            lineVm.BackPaymentDetailVms.Add(detail);
                            #endregion
                            #region tax difference
                            detail = new BackPaymentDetailVm
                            {
                                GLPeriodId = p.GLPeriodId,
                                Description = $"Income Tax Arrears {p.Name}",
                                Type = DetailType.Deduction,
                                Amount = incomeTaxDiff,

                            };
                            lineVm.BackPaymentDetailVms.Add(detail);
                            #endregion
                            #region emp pension difference
                            detail = new BackPaymentDetailVm
                            {
                                GLPeriodId = p.GLPeriodId,
                                Description = $"Pension from Employee Arrears {p.Name}",
                                Type = DetailType.Deduction,
                                Amount = pensionDiff,

                            };
                            lineVm.BackPaymentDetailVms.Add(detail);
                            #endregion
                            #region comp pension difference
                            detail = new BackPaymentDetailVm
                            {
                                GLPeriodId = p.GLPeriodId,
                                Description = $"Pension from Company Arrears {p.Name}",
                                Type = DetailType.NA,
                                Amount = pensionCompDiff,

                            };
                            lineVm.BackPaymentDetailVms.Add(detail);

                            #endregion
                        }

                    }
                }

                if (totalSalaryDifference > 0)
                {
                    var line = new BackPaymentLine
                    {
                        BackPay = backpay,
                        EmployeeId = e.PersonId,
                        BasicSalary = totalSalaryDifference,
                        Pension = totalEmpPension,
                        PensionComp = totalCompPension,
                        IncomeTax = totalIncomeTax,
                        Net = totalSalaryDifference - (totalIncomeTax + totalEmpPension),

                    };

                    db.BackPaymentLines.Add(line);
                    db.SaveChanges();

                    foreach (var d in lineVm.BackPaymentDetailVms)
                    {
                        var detail = new BackPaymentDetail
                        {
                            BackPaymentLine = line,
                            Description = d.Description,
                            Type = d.Type,
                            Amount = d.Amount,
                            GLPeriodId = d.GLPeriodId

                        };

                        db.BackPaymentDetails.Add(detail);
                        
                    }
                    db.SaveChanges();
                }

            }

            var formattedData = new
            {
                Message = "Process complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
           
        }
        private bool GetPensionSetting()
        {
            var Pensions = db.Configurations.Where(x => x.Reference == "Pension").ToList();
            try
            {
                if (Pensions.Any())
                {
                    var Employ = Pensions.Where(o => o.Attribute == PensionAttribute.EMPLOYEE_CONTRIBUTION.ToString())
                        .FirstOrDefault();
                    if (Employ != null)
                    {
                        EmployeeContribution =
                            Convert.ToDecimal(Employ.CurrentValue);
                    }
                    var Comp = Pensions.Where(o => o.Attribute == PensionAttribute.COMPANY_CONTRIBUTION.ToString())
                      .FirstOrDefault();
                    if (Comp != null)
                    {
                        CompanyContribution =
                            Convert.ToDecimal(Comp.CurrentValue);
                    }
                    var Retire = Pensions.Where(o => o.Attribute == PensionAttribute.RETIREMENT_AGE.ToString())
                        .FirstOrDefault();



                    return true;
                }

            }
            catch (Exception)
            {

                return false;
            }
            return false;
        }



        public string CheckStatus(int fromPeriod, int toPeriod, int branchid, int businessUnitid)
        {
            var backPays = db.BackPays.Where(x => x.GLPeriodId == fromPeriod && x.ToGLPeriodId==toPeriod && x.BranchId == branchid);
            if (GlobalHelper.IsPayrollModebranchAndBusinessUnit() && businessUnitid != 0)
            {
                backPays = backPays.Where(x => x.BusinessUnitId == businessUnitid);
            }

            var backpay = backPays.FirstOrDefault();
            if (backpay != null)
            {
                if (backpay.IsClosed)
                {
                    return "c";
                }
                else
                {
                    return "n";
                }
            }
            else
            {
                return "r";
            }
        }
        public JsonResult Close(int id)
        {
            var backPay = db.BackPays.Find(id);

            if (backPay != null)
            {
                backPay.IsClosed = true;
                db.BackPays.Attach(backPay);
                db.Entry(backPay).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "Closed.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "Record not found.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class BackPaymentLineVm
    {
        public int EmployeeId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal Pension { get; set; }
        public decimal Net { get; set; }
        public List<BackPaymentDetailVm> BackPaymentDetailVms { get; set; }
    }

    public class BackPaymentDetailVm
    {
        public int BackPaymentDetailId { get; set; }
        public int GLPeriodId { get; set; }
        public string Description { get; set; }
        public DetailType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
