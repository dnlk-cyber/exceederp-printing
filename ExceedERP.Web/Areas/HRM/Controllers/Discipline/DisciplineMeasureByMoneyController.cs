using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplineMeasureByMoneyController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DisciplineMeasuerByMoney_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<DisciplineMeasuerByMoney> disciplinemeasuerbymoney = db.DisciplineMeasuerByMoney.Where(x => x.DisciplinaryCaseId == id);
            DataSourceResult result = disciplinemeasuerbymoney.ToDataSourceResult(request, disciplineMeasuerByMoney => new
            {
                DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
                DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
                EmployeeId = disciplineMeasuerByMoney.EmployeeId,
                IsActive = disciplineMeasuerByMoney.IsActive,
                Amount = disciplineMeasuerByMoney.Amount,
                Months = disciplineMeasuerByMoney.Months,
                MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
                Balance = disciplineMeasuerByMoney.Balance,
                Remark = disciplineMeasuerByMoney.Remark,
                OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
                DateCreated = disciplineMeasuerByMoney.DateCreated,
                LastModified = disciplineMeasuerByMoney.LastModified,
                CreatedBy = disciplineMeasuerByMoney.CreatedBy,
                ModifiedBy = disciplineMeasuerByMoney.ModifiedBy,
                GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuerByMoney_Create([DataSourceRequest]DataSourceRequest request, DisciplineMeasuerByMoney disciplineMeasuerByMoney, int idTwo, int empIdTwo)
        {
            ModelState.Clear();

            if (disciplineMeasuerByMoney.Months != 0)
            {
                disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
                disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
            }
            var checkexistance = db.DisciplineMeasuerByMoney.Where(x => x.DisciplinaryCaseId == disciplineMeasuerByMoney.DisciplinaryCaseId && x.EmployeeId == disciplineMeasuerByMoney.EmployeeId).FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This measure on this period is already registered!!");
            }
            var disciplineMeasure = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == idTwo);
            if (disciplineMeasure != null)
            {
                var detailMeasure = db.DisciplineMeasuresDetail.FirstOrDefault(x => x.MeasuresId == disciplineMeasure.MeasuresId && x.DetailMeasuresId == disciplineMeasure.DetailMeasuresId);
                if (detailMeasure != null)
                {
                    var measureType = db.DisciplineMeasuresType.FirstOrDefault(x => x.DetailMeasuresId == detailMeasure.DetailMeasuresId);
                    if (measureType != null)
                    {
                        if (measureType.IsByMoney)
                        {
                            disciplineMeasuerByMoney.Amount = measureType.Amount;
                        }
                        else
                        {
                            ModelState.AddModelError("", "this discipline measure is not by money");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "measure type setting is not set");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid or deleted descipline measure");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid or deleted descipline setting");
            }

            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasuerByMoney
                {
                    DisciplinaryCaseId = idTwo,
                    Amount = disciplineMeasuerByMoney.Amount,
                    GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
                    EmployeeId = empIdTwo,
                    Months = disciplineMeasuerByMoney.Months,
                    MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
                    Balance = disciplineMeasuerByMoney.Balance,
                    Remark = disciplineMeasuerByMoney.Remark,
                    OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = disciplineMeasuerByMoney.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = disciplineMeasuerByMoney.ModifiedBy
                };

                db.DisciplineMeasuerByMoney.Add(entity);
                db.SaveChanges();
                disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId = entity.DisciplineMeasuerByMoneyId;

                #region operation log

                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Success", User.Identity.Name, User.Identity.Name + " add displine Measure By Money at Date " + DateTime.Today, Modules.HRM);

                #endregion
            }


            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DisciplineMeasuerByMoney_Update([DataSourceRequest]DataSourceRequest request, DisciplineMeasuerByMoney disciplineMeasuerByMoney)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        if (disciplineMeasuerByMoney.Months != 0)
        //        {
        //            disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
        //            disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
        //        }
        //        var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == disciplineMeasuerByMoney.EmployeeId ).FirstOrDefault();
        //        if (checkexistance != null)
        //        {
        //            var checkLoan = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.Name == disciplineMeasuerByMoney.DisciplinaryCaseId .ToString () && x.Remark == "Discipline").FirstOrDefault();
        //            if (checkLoan != null)
        //            {
        //                ModelState.AddModelError("", "You cannot  update this information, it already processed!!");
        //            }
        //            else
        //            {
        //                var entity = new DisciplineMeasuerByMoney
        //                {
        //                    DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
        //                    DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
        //                    IsActive = disciplineMeasuerByMoney.IsActive,
        //                    Amount = disciplineMeasuerByMoney.Amount,
        //                    EmployeeId = disciplineMeasuerByMoney.EmployeeId,
        //                    Months = disciplineMeasuerByMoney.Months,
        //                    MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
        //                    Balance = disciplineMeasuerByMoney.Balance,
        //                    Remark = disciplineMeasuerByMoney.Remark,
        //                    OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
        //                    DateCreated = disciplineMeasuerByMoney.DateCreated,
        //                    LastModified = DateTime.Now,
        //                    CreatedBy = disciplineMeasuerByMoney.CreatedBy,
        //                    ModifiedBy = User.Identity.Name,
        //                    GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
        //                };

        //                db.DisciplineMeasuerByMoney.Attach(entity);
        //                db.Entry(entity).State = EntityState.Modified;
        //                db.SaveChanges();

        //                #region operation log

        //                //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //                UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Success", User.Identity.Name, User.Identity.Name + " update displine Measure By Money at Date " + DateTime.Today, Modules.HRM);

        //                #endregion
        //            }
        //        }
        //        else
        //        {
        //            var entity = new DisciplineMeasuerByMoney
        //        {
        //            DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
        //            DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
        //            IsActive = disciplineMeasuerByMoney.IsActive,
        //            Amount = disciplineMeasuerByMoney.Amount,
        //            EmployeeId = disciplineMeasuerByMoney.EmployeeId,
        //            Months = disciplineMeasuerByMoney.Months,
        //            MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
        //            Balance = disciplineMeasuerByMoney.Balance,
        //            Remark = disciplineMeasuerByMoney.Remark,
        //            OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
        //            DateCreated = disciplineMeasuerByMoney.DateCreated,
        //            LastModified = DateTime.Now,
        //            CreatedBy = disciplineMeasuerByMoney.CreatedBy,
        //            ModifiedBy = User.Identity.Name,
        //            GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
        //        };

        //            db.DisciplineMeasuerByMoney.Attach(entity);
        //            db.Entry(entity).State = EntityState.Modified;
        //            db.SaveChanges();

        //            #region operation log

        //            //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

        //            UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Success", User.Identity.Name, User.Identity.Name + " update displine Measure By Money at Date " + DateTime.Today, Modules.HRM);

        //            #endregion
        //        }
        //    }

        //    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult DisciplineMeasuerByMoney_Destroy([DataSourceRequest]DataSourceRequest request, DisciplineMeasuerByMoney disciplineMeasuerByMoney)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == disciplineMeasuerByMoney.EmployeeId && x.GLPeriodId == disciplineMeasuerByMoney .GLPeriodId  ).FirstOrDefault();
        //        if (checkexistance != null)
        //        {
        //            var checkLoan = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.Name == disciplineMeasuerByMoney.DisciplinaryCaseId .ToString () && x.Remark == "Discipline").FirstOrDefault();
        //            if (checkLoan != null)
        //            {
        //                ModelState.AddModelError("", "You cannot  delete this information, it already processed!!");
        //            }
        //            else
        //            {
        //                var entity = new DisciplineMeasuerByMoney
        //                {
        //                    DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
        //                    DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
        //                    IsActive = disciplineMeasuerByMoney.IsActive,
        //                    Amount = disciplineMeasuerByMoney.Amount,
        //                    EmployeeId = disciplineMeasuerByMoney.EmployeeId,
        //                    Months = disciplineMeasuerByMoney.Months,
        //                    MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
        //                    Balance = disciplineMeasuerByMoney.Balance,
        //                    Remark = disciplineMeasuerByMoney.Remark,
        //                    OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
        //                    DateCreated = disciplineMeasuerByMoney.DateCreated,
        //                    LastModified = disciplineMeasuerByMoney.LastModified,
        //                    CreatedBy = disciplineMeasuerByMoney.CreatedBy,
        //                    ModifiedBy = disciplineMeasuerByMoney.ModifiedBy,
        //                    GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
        //                };
        //                #region delete log
        //                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline Measuer By Money information of employee" + disciplineMeasuerByMoney.EmployeeId);
        //                #endregion
        //                db.DisciplineMeasuerByMoney.Attach(entity);
        //                db.DisciplineMeasuerByMoney.Remove(entity);
        //                db.SaveChanges();
        //            }
        //        }
        //        else 
        //        {
        //        var entity = new DisciplineMeasuerByMoney
        //        {
        //            DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
        //            DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
        //            IsActive = disciplineMeasuerByMoney.IsActive,
        //            Amount = disciplineMeasuerByMoney.Amount,
        //            EmployeeId = disciplineMeasuerByMoney.EmployeeId,
        //            Months = disciplineMeasuerByMoney.Months,
        //            MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
        //            Balance = disciplineMeasuerByMoney.Balance,
        //            Remark = disciplineMeasuerByMoney.Remark,
        //            OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
        //            DateCreated = disciplineMeasuerByMoney.DateCreated,
        //            LastModified = disciplineMeasuerByMoney.LastModified,
        //            CreatedBy = disciplineMeasuerByMoney.CreatedBy,
        //            ModifiedBy = disciplineMeasuerByMoney.ModifiedBy,
        //            GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted discipline Measuer By Money information of employee" + disciplineMeasuerByMoney.Employee.EmployeeCode);
        //        #endregion
        //        db.DisciplineMeasuerByMoney.Attach(entity);
        //        db.DisciplineMeasuerByMoney.Remove(entity);
        //        db.SaveChanges();
        //    }
        //        }

        //    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
