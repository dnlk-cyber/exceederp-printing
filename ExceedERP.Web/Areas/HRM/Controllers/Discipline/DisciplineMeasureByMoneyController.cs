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
using ExceedERP.Core.Domain.HRM.Payroll;
using System.Data.Entity;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.HR;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{
    [AuthorizeRoles(HrmConstants.HRDisciplineUser)]
    public class DisciplineMeasureByMoneyController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            return View();
        }

        private bool ApplyDisciplineMoneyLogic(DisciplineMeasuerByMoney model, out string error)
        {
            error = null;

            var disciplineMeasure = db.EmpDisciplineMeasure
                .SingleOrDefault(x => x.DisciplinaryCaseId == model.DisciplinaryCaseId);

            if (disciplineMeasure == null)
            {
                error = "Please add employee discipline measures first.";
                return false;
            }

            var placement = db.Placements
                .FirstOrDefault(x => x.EmployeeId == model.EmployeeId && x.isCurrent);

            var detailMeasure = db.DisciplineMeasuresDetail.FirstOrDefault(x =>
                x.MeasuresId == disciplineMeasure.MeasuresId &&
                x.DetailMeasuresId == disciplineMeasure.DetailMeasuresId);

            if (detailMeasure == null)
            {
                error = "Invalid or deleted discipline measure.";
                return false;
            }

            var measureType = db.DisciplineMeasuresType.FirstOrDefault(x =>
                x.DetailMeasuresId == detailMeasure.DetailMeasuresId && x.IsByMoney);

            if (measureType == null)
            {
                error = "Measure type is not configured or is not monetary.";
                return false;
            }

            model.IsDeductedFromSalary = model.IsDeductedFromSalary;

            model.SalaryDeductionPercentage = model.SalaryDeductionPercentage
                                              ?? measureType.SalaryDeductionPercentage
                                              ?? 0;

            if (model.IsDeductedFromSalary)
            {
                bool hasPercentage =model.SalaryDeductionPercentage.HasValue 
                                        && model.SalaryDeductionPercentage > 0;

                bool hasDays =
                    measureType.Day > 0 || (model.Days ?? 0) > 0;

                bool hasManualAmount =model.Amount > 0;

                if (!hasPercentage && !hasDays && !hasManualAmount)
                {
                    error = "Please provide salary percentage, number of days, or amount.";
                    return false;
                }

                if (placement == null || placement.Salary == 0)
                {
                    error = "The employee must have a valid current placement and salary.";
                    return false;
                }
                decimal netSalary = HrHelper.CalculateNetSalary(placement.Salary);
                if (hasPercentage)
                {
                    var percentage = model.SalaryDeductionPercentage ?? 0;

                    //model.MonthlyAmount = (placement.Salary * percentage) / 100;
                    model.MonthlyAmount = (netSalary * percentage) / 100;

                    if (model.Months <= 0)
                    {
                        error = "Number of months must be greater than zero.";
                        return false;
                    }

                    model.Amount = model.MonthlyAmount * model.Months;
                    model.Balance = model.Amount;
                }
                else if (hasDays)
                {
                    var days = (decimal)(model.Days ?? measureType.Day);
                    //var dailyRate = placement.Salary / 30m;
                    var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == model.GLPeriodId);

                    decimal daysInMonth = period != null? (decimal)(period.DateTo - period.DateFrom).TotalDays + 1: 30m;


                    var dailyRate = netSalary / daysInMonth;

                    model.Amount = days * dailyRate;
                    model.Balance = model.Amount;

                    if (model.Months > 0)
                        model.MonthlyAmount = model.Amount / model.Months;
                    else if (measureType.Month > 0)
                        model.MonthlyAmount = model.Amount / measureType.Month;
                    else
                        model.MonthlyAmount = model.Amount;
                }
                else if (hasManualAmount)
                {
                    model.Balance = model.Amount;

                    if (model.Months > 0)
                        model.MonthlyAmount = model.Amount / model.Months;
                    else
                        model.MonthlyAmount = model.Amount;
                }
            }
            else if (model.Months > 0)
            {
                model.MonthlyAmount = model.Amount / model.Months;
                model.Balance = model.Amount;
                model.IsDeductedFromSalary = model.IsDeductedFromSalary;
            }

            return true;
        }

        public ActionResult DisciplineMeasuerByMoney_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<DisciplineMeasuerByMoney> disciplinemeasuerbymoney = db.DisciplineMeasuerByMoney.Where(x => x.DisciplinaryCaseId == id);
            DataSourceResult result = disciplinemeasuerbymoney.ToDataSourceResult(request, disciplineMeasuerByMoney => new
            {
                DisciplineMeasuerByMoneyId = disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId,
                DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
                GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
                EmployeeId = disciplineMeasuerByMoney.EmployeeId,
                IsActive = disciplineMeasuerByMoney.IsActive,
                IsDeductedFromSalary = disciplineMeasuerByMoney.IsDeductedFromSalary,
                SalaryDeductionPercentage = disciplineMeasuerByMoney.SalaryDeductionPercentage,
                Amount = disciplineMeasuerByMoney.Amount,
                Months = disciplineMeasuerByMoney.Months,
                Days = disciplineMeasuerByMoney.Days,
                MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
                Balance = disciplineMeasuerByMoney.Balance,
                Remark = disciplineMeasuerByMoney.Remark,
                OrgBranchName = disciplineMeasuerByMoney.OrgBranchName,
                IsOnlineApproved = disciplineMeasuerByMoney.IsOnlineApproved,
                OnlineApprovedBy = disciplineMeasuerByMoney.OnlineApprovedBy,
                OnlineApprovedTime = disciplineMeasuerByMoney.OnlineApprovedTime,
                DateCreated = disciplineMeasuerByMoney.DateCreated,
                LastModified = disciplineMeasuerByMoney.LastModified,
                CreatedBy = disciplineMeasuerByMoney.CreatedBy,
                ModifiedBy = disciplineMeasuerByMoney.ModifiedBy
            });
            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuerByMoney_Create_Exist([DataSourceRequest]DataSourceRequest request, DisciplineMeasuerByMoney disciplineMeasuerByMoney, int idTwo, int empIdTwo)
        {
           
            ModelState.Clear();
            disciplineMeasuerByMoney.EmployeeId = empIdTwo;
            disciplineMeasuerByMoney.DisciplinaryCaseId = idTwo;
            // Check for existing record
            bool alreadyExists = db.DisciplineMeasuerByMoney.Any(x =>
                x.DisciplinaryCaseId == disciplineMeasuerByMoney.DisciplinaryCaseId 
                && x.GLPeriodId == disciplineMeasuerByMoney.GLPeriodId
                &&x.EmployeeId == disciplineMeasuerByMoney.EmployeeId);

            if (alreadyExists)
            {
                ModelState.AddModelError("", "This measure for this period is already registered.");
                return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            }

            // Validate main measure
            var disciplineMeasure = db.EmpDisciplineMeasure.SingleOrDefault(x => x.DisciplinaryCaseId == idTwo);
            if (disciplineMeasure == null)
            {
                ModelState.AddModelError("", "Please Added Employee Discipline Measures first.");
                return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            }
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empIdTwo && x.isCurrent);
            // Validate detail measure
            var detailMeasure = db.DisciplineMeasuresDetail.FirstOrDefault(x =>
                                                            x.MeasuresId == disciplineMeasure.MeasuresId &&
                                                            x.DetailMeasuresId == disciplineMeasure.DetailMeasuresId);

            if (detailMeasure == null)
            {
                ModelState.AddModelError("", "Invalid or deleted discipline measure.");
                return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            }

            // Validate measure type
            var measureType = db.DisciplineMeasuresType.SingleOrDefault(x =>
                                                    x.DetailMeasuresId == detailMeasure.DetailMeasuresId
                                                    &&x.IsByMoney);

            if (measureType == null)
            {
                ModelState.AddModelError("", "Measure type setting is not set or the discipline measure is not by money.");
                return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            }
            disciplineMeasuerByMoney.IsDeductedFromSalary = measureType.IsDeductedFromSalary;
            disciplineMeasuerByMoney.SalaryDeductionPercentage = measureType.SalaryDeductionPercentage?? disciplineMeasuerByMoney.SalaryDeductionPercentage??0;
            if (disciplineMeasuerByMoney.IsDeductedFromSalary)
            {
                // Validate percentage
                if ((!disciplineMeasuerByMoney.SalaryDeductionPercentage.HasValue ||
                    disciplineMeasuerByMoney.SalaryDeductionPercentage == 0) && measureType.Day == 0 && disciplineMeasuerByMoney.Days==0)
                {
                    ModelState.AddModelError("", "This measure for this period must have a valid Salary Deduction Percentage.");
                    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
                }

                if (placement == null || placement.Salary == 0)
                {
                    ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
                    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
                }

                decimal netSalary = HrHelper.CalculateNetSalary(placement.Salary);
                //  Monthly Deduction ---
                var percentage = disciplineMeasuerByMoney.SalaryDeductionPercentage ?? 0;
                //disciplineMeasuerByMoney.MonthlyAmount = (placement.Salary * percentage) / 100;
                disciplineMeasuerByMoney.MonthlyAmount = (netSalary * percentage) / 100;

                //  Compute Total Amount (for all months) 
                if (disciplineMeasuerByMoney.Months > 0 && disciplineMeasuerByMoney.MonthlyAmount > 0)
                {
                    disciplineMeasuerByMoney.Amount = disciplineMeasuerByMoney.MonthlyAmount * disciplineMeasuerByMoney.Months;
                    disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
                }
                else if ((measureType.Day > 0 || (disciplineMeasuerByMoney.Days.HasValue && disciplineMeasuerByMoney.Days > 0))
                       && (measureType.IsDeductedFromSalary || disciplineMeasuerByMoney.IsDeductedFromSalary))
                {
                    if (placement == null || placement.Salary == 0)
                    {
                        ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
                        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
                    }

                    var days = disciplineMeasuerByMoney?.Days ?? measureType.Day;
                    //var dailyRate = placement.Salary / 30m;
                    var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == disciplineMeasuerByMoney.GLPeriodId);

                    decimal daysInMonth = period != null ? (decimal)(period.DateTo - period.DateFrom).TotalDays + 1 : 30m;

                    var dailyRate = netSalary / daysInMonth;
                    measureType.Amount = days * dailyRate;
                    disciplineMeasuerByMoney.Amount = days * dailyRate;
                    disciplineMeasuerByMoney.Balance = days * dailyRate;
                    disciplineMeasuerByMoney.MonthlyAmount = days * dailyRate;
                   
                     if (disciplineMeasuerByMoney.Months > 0 && disciplineMeasuerByMoney.Amount > 0)
                        disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
                     else if (measureType.Amount > 0 && measureType.Month > 0)
                        disciplineMeasuerByMoney.MonthlyAmount = measureType.Amount / measureType.Month;

                }

                else
                {
                    ModelState.AddModelError("", "Number of months must be greater than zero.");
                    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
                }
            }

            if (disciplineMeasuerByMoney.Months > 0 && !disciplineMeasuerByMoney.IsDeductedFromSalary)
            {
                disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
                disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
            }

            //if (disciplineMeasuerByMoney.IsDeductedFromSalary)
            //{
            //    // Validate percentage
            //    if ((!disciplineMeasuerByMoney.SalaryDeductionPercentage.HasValue ||
            //        disciplineMeasuerByMoney.SalaryDeductionPercentage == 0)&&!disciplineMeasuerByMoney.Days.HasValue)
            //    {
            //        ModelState.AddModelError("", "This measure for this period must have a valid Salary Deduction Percentage.");
            //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            //    }

            //    // Get employee placement (for salary)

            //    if (placement == null || placement.Salary == 0)
            //    {
            //        ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
            //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            //    }

            //    //  Monthly Deduction ---
            //    var percentage = disciplineMeasuerByMoney.SalaryDeductionPercentage ?? 0;
            //    disciplineMeasuerByMoney.MonthlyAmount = (placement.Salary * percentage) / 100;

            //    //  Compute Total Amount (for all months) 
            //    if (disciplineMeasuerByMoney.Months > 0&& measureType.Month > 0)
            //    {
            //        disciplineMeasuerByMoney.Amount = disciplineMeasuerByMoney.MonthlyAmount * disciplineMeasuerByMoney.Months;
            //        disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
            //    }
            //    else if ((measureType.Day > 0||(disciplineMeasuerByMoney.Days.HasValue&&disciplineMeasuerByMoney.Days>0)) 
            //             && (measureType.IsDeductedFromSalary||disciplineMeasuerByMoney.IsDeductedFromSalary))
            //    {
            //        if (placement == null || placement.Salary == 0)
            //        {
            //            ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
            //            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            //        }
            //        var days = disciplineMeasuerByMoney?.Days ?? measureType.Day;
            //        var dailyRate = placement.Salary / 30m;
            //        measureType.Amount = days * dailyRate;
            //        if (measureType.Amount > 0 && measureType.Month > 0)
            //            disciplineMeasuerByMoney.MonthlyAmount = measureType.Amount / measureType.Month;
            //        else if (disciplineMeasuerByMoney.Months > 0 && disciplineMeasuerByMoney.Amount > 0)
            //            disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;

            //    }
            //    else
            //    {
            //        ModelState.AddModelError("", "Number of months must be greater than zero.");
            //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            //    }
            //}
            //else if ((disciplineMeasuerByMoney.Months > 0|| measureType.Month >0) && !measureType.IsDeductedFromSalary)
            //{
            //    if(measureType.Amount >0&& measureType.Month > 0)
            //        disciplineMeasuerByMoney.MonthlyAmount = measureType.Amount / measureType.Month;
            //    else if(disciplineMeasuerByMoney.Months>0&& disciplineMeasuerByMoney.Amount>0)
            //        disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;

            //}
            //else if (measureType.Day >0&&measureType.IsDeductedFromSalary)
            //{
            //    if (placement == null || placement.Salary == 0)
            //    {
            //        ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
            //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
            //    }

            //    disciplineMeasuerByMoney.Amount = measureType.Day * (placement.Salary / 30m);
            //    disciplineMeasuerByMoney.MonthlyAmount = measureType.Day * (placement.Salary / 30m);
            //    if (measureType.Amount >0&& measureType.Month > 0)
            //        disciplineMeasuerByMoney.MonthlyAmount = measureType.Amount / measureType.Month;
            //    else if(disciplineMeasuerByMoney.Months>0&& disciplineMeasuerByMoney.Amount>0)
            //        disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
            //}
            if (ModelState.IsValid)
            {
                var entity = new DisciplineMeasuerByMoney
                {
                    DisciplinaryCaseId = disciplineMeasuerByMoney.DisciplinaryCaseId,
                    Amount = disciplineMeasuerByMoney.Amount,
                    Days = disciplineMeasuerByMoney.Days,
                    GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
                    EmployeeId = disciplineMeasuerByMoney.EmployeeId,
                    Months = disciplineMeasuerByMoney.Months,
                    MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
                    Balance = disciplineMeasuerByMoney.Balance,
                    IsDeductedFromSalary = disciplineMeasuerByMoney.IsDeductedFromSalary,
                    SalaryDeductionPercentage = disciplineMeasuerByMoney.SalaryDeductionPercentage,
                    Remark = disciplineMeasuerByMoney.Remark,
                    IsActive = disciplineMeasuerByMoney.IsActive,
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

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Success", User.Identity.Name, User.Identity.Name + " add displine Measure By Money at Date " + DateTime.Today, Modules.HRM);

                #endregion
            }


            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DisciplineMeasuerByMoney_Create([DataSourceRequest] DataSourceRequest request,DisciplineMeasuerByMoney model,int idTwo,int empIdTwo)
        {
            model.EmployeeId = empIdTwo;
            model.DisciplinaryCaseId = idTwo;

            // Duplicate check
            if (db.DisciplineMeasuerByMoney.Any(x =>
                x.DisciplinaryCaseId == model.DisciplinaryCaseId &&
                x.GLPeriodId == model.GLPeriodId &&
                x.EmployeeId == model.EmployeeId))
            {
                ModelState.AddModelError("", "This measure for this period is already registered.");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            //  Apply shared logic
            if (!ApplyDisciplineMoneyLogic(model, out string error))
            {
                ModelState.AddModelError("", error);
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                model.DateCreated = DateTime.Now;
                model.CreatedBy = User.Identity.Name;

                db.DisciplineMeasuerByMoney.Add(model);
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress,
                    "DisciplinaryMeasureByMoney",
                    "Create",
                    User.Identity.Name,
                    $"{User.Identity.Name} added a discipline measure by money at {DateTime.Now}",
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuerByMoney_Update( [DataSourceRequest] DataSourceRequest request,DisciplineMeasuerByMoney model)
        {
            var entity = db.DisciplineMeasuerByMoney
                .FirstOrDefault(x => x.DisciplineMeasuerByMoneyId == model.DisciplineMeasuerByMoneyId);

            if (entity == null)
            {
                ModelState.AddModelError("", "The record you are trying to update no longer exists.");

                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            // Payroll processed validation
            var checkExistance = db.PayrollProcessViewModel
                .FirstOrDefault(x => x.EmployeeId == model.EmployeeId);

            if (checkExistance != null)
            {
                var checkLoan = db.PayrollProcessDeductions.FirstOrDefault(x =>
                    x.PayrollProcessViewModelId == checkExistance.PayrollProcessViewModelId &&
                    x.Name == model.DisciplinaryCaseId.ToString() &&
                    x.Remark == "Discipline");

                if (checkLoan != null)
                {
                    ModelState.AddModelError("",
                        "You cannot update this information, it is already payroll processed.");

                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
            }

            // Duplicate validation excluding current record
            bool duplicateExists = db.DisciplineMeasuerByMoney.Any(x =>
                x.DisciplinaryCaseId == model.DisciplinaryCaseId &&
                x.GLPeriodId == model.GLPeriodId &&
                x.EmployeeId == model.EmployeeId &&
                x.DisciplineMeasuerByMoneyId != model.DisciplineMeasuerByMoneyId);

            if (duplicateExists)
            {
                ModelState.AddModelError("",
                    "This measure for this period is already registered.");

                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            // Shared calculation + validation logic
            if (!ApplyDisciplineMoneyLogic(model, out string error))
            {
                ModelState.AddModelError("", error);

                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                entity.GLPeriodId = model.GLPeriodId;
                entity.Amount = model.Amount;
                entity.Days = model.Days;
                entity.Months = model.Months;
                entity.MonthlyAmount = model.MonthlyAmount;
                entity.Balance = model.Balance;
                entity.IsDeductedFromSalary = model.IsDeductedFromSalary;
                entity.SalaryDeductionPercentage = model.SalaryDeductionPercentage;
                entity.Remark = model.Remark;
                entity.IsActive = model.IsActive;
                entity.OrgBranchName = model.OrgBranchName;
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "DisciplinaryMeasureByMoney",
                    "Update",
                    User.Identity.Name,
                    $"{User.Identity.Name} updated a discipline measure by money at {DateTime.Now}",
                    Modules.HRM);
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        //public ActionResult DisciplineMeasuerByMoney_Update([DataSourceRequest] DataSourceRequest request, DisciplineMeasuerByMoney disciplineMeasuerByMoney)
        //{

        //    var entity = db.DisciplineMeasuerByMoney.FirstOrDefault(x=>x.DisciplineMeasuerByMoneyId==disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId);
        //    if (entity == null)
        //    {
        //        ModelState.AddModelError("", "The record you are trying to update no longer exists.");
        //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //    }
        //    var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == disciplineMeasuerByMoney.EmployeeId).FirstOrDefault();
        //    if (checkexistance != null)
        //    {
        //        var checkLoan = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId 
        //                                    && x.Name == disciplineMeasuerByMoney.DisciplinaryCaseId.ToString() 
        //                                    && x.Remark == "Discipline").FirstOrDefault();
        //        if (checkLoan != null)
        //        {
        //            ModelState.AddModelError("", "You cannot  update this information, it already payroll  processed!!");
        //            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));

        //        }
        //    }
        //    // Validate main measure
        //    var disciplineMeasure = db.EmpDisciplineMeasure.SingleOrDefault(x => x.DisciplinaryCaseId == disciplineMeasuerByMoney.DisciplinaryCaseId);
        //    if (disciplineMeasure == null)
        //    {
        //        ModelState.AddModelError("", "Please Added Employee Discipline Measures first.");
        //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //    }

        //    // Get employee placement (for salary)
        //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == disciplineMeasuerByMoney.EmployeeId && x.isCurrent);
        //    // Validate detail measure
        //    var detailMeasure = db.DisciplineMeasuresDetail.FirstOrDefault(x =>
        //        x.MeasuresId == disciplineMeasure.MeasuresId &&
        //        x.DetailMeasuresId == disciplineMeasure.DetailMeasuresId);

        //    if (detailMeasure == null)
        //    {
        //        ModelState.AddModelError("", "Invalid or deleted discipline measure.");
        //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //    }

        //    // Validate measure type
        //    var measureType = db.DisciplineMeasuresType.SingleOrDefault(x =>
        //        x.DetailMeasuresId == detailMeasure.DetailMeasuresId && x.IsByMoney);

        //    if (measureType == null)
        //    {
        //        ModelState.AddModelError("", "Measure type setting is not set or the discipline measure is not by money.");
        //        return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //    }
        //    if (disciplineMeasuerByMoney.IsDeductedFromSalary)
        //    {
        //        // Validate percentage
        //        if ((!disciplineMeasuerByMoney.SalaryDeductionPercentage.HasValue ||
        //            disciplineMeasuerByMoney.SalaryDeductionPercentage == 0)&& measureType.Day==0&&disciplineMeasuerByMoney.Days==0)
        //        {
        //            ModelState.AddModelError("", "This measure for this period must have a valid Salary Deduction Percentage.");
        //            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //        }

        //        if (placement == null || placement.Salary == 0)
        //        {
        //            ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
        //            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //        }

        //        //  Monthly Deduction ---
        //        var percentage = disciplineMeasuerByMoney.SalaryDeductionPercentage ?? 0;
        //        disciplineMeasuerByMoney.MonthlyAmount = (placement.Salary * percentage) / 100;

        //        //  Compute Total Amount (for all months) 
        //        if (disciplineMeasuerByMoney.Months > 0 && disciplineMeasuerByMoney.MonthlyAmount>0)
        //        {
        //            disciplineMeasuerByMoney.Amount = disciplineMeasuerByMoney.MonthlyAmount * disciplineMeasuerByMoney.Months;
        //            disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
        //        }
        //        else if ((measureType.Day > 0 || (disciplineMeasuerByMoney.Days.HasValue && disciplineMeasuerByMoney.Days > 0))
        //               && (measureType.IsDeductedFromSalary || disciplineMeasuerByMoney.IsDeductedFromSalary))
        //        {
        //            if (placement == null || placement.Salary == 0)
        //            {
        //                ModelState.AddModelError("", "This employee must have a valid current placement and salary.");
        //                return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //            }

        //            var days = disciplineMeasuerByMoney?.Days ?? measureType.Day;
        //            var dailyRate = placement.Salary / 30m;
        //            measureType.Amount = days * dailyRate;
        //            disciplineMeasuerByMoney.Amount = days * dailyRate;
        //            disciplineMeasuerByMoney.Balance = days * dailyRate;
        //            disciplineMeasuerByMoney.MonthlyAmount = days * dailyRate;

        //            if (measureType.Amount > 0 && measureType.Month > 0)
        //                disciplineMeasuerByMoney.MonthlyAmount = measureType.Amount / measureType.Month;

        //            else if (disciplineMeasuerByMoney.Months > 0 && disciplineMeasuerByMoney.Amount > 0)
        //                disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;


        //        }

        //        else
        //        {
        //            ModelState.AddModelError("", "Number of months must be greater than zero.");
        //            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //        }
        //    }

        //    if (disciplineMeasuerByMoney.Months > 0 && !disciplineMeasuerByMoney.IsDeductedFromSalary)
        //    {
        //        disciplineMeasuerByMoney.MonthlyAmount = disciplineMeasuerByMoney.Amount / disciplineMeasuerByMoney.Months;
        //        disciplineMeasuerByMoney.Balance = disciplineMeasuerByMoney.Amount;
        //    }

        //    if (disciplineMeasuerByMoney != null && ModelState.IsValid)
        //    {

        //        entity.GLPeriodId = disciplineMeasuerByMoney.GLPeriodId;
        //        entity.Amount = disciplineMeasuerByMoney.Amount;
        //        entity.Days = disciplineMeasuerByMoney.Days;
        //        entity.Months = disciplineMeasuerByMoney.Months;
        //        entity.MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount;
        //        entity.Balance = disciplineMeasuerByMoney.Balance;
        //        entity.IsDeductedFromSalary = disciplineMeasuerByMoney.IsDeductedFromSalary;
        //        entity.SalaryDeductionPercentage = disciplineMeasuerByMoney.SalaryDeductionPercentage;
        //        entity.Remark = disciplineMeasuerByMoney.Remark;
        //        entity.IsActive = disciplineMeasuerByMoney.IsActive;
        //        entity.OrgBranchName = disciplineMeasuerByMoney.OrgBranchName;
        //        entity.LastModified = DateTime.Now;
        //        entity.ModifiedBy = User.Identity.Name;
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();

        //        UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Update",
        //            User.Identity.Name, $"{User.Identity.Name} updated a discipline measure by money at {DateTime.Now}", Modules.HRM);
        //    }

        //    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        //}
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DisciplineMeasuerByMoney_Destroy([DataSourceRequest] DataSourceRequest request,DisciplineMeasuerByMoney disciplineMeasuerByMoney)
        {
            if (disciplineMeasuerByMoney != null)
            {
                var entity = db.DisciplineMeasuerByMoney.Find(disciplineMeasuerByMoney.DisciplineMeasuerByMoneyId);
                if (entity == null)
                {
                    ModelState.AddModelError("", "The record you are trying to delete no longer exists.");
                    return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
                }

                db.DisciplineMeasuerByMoney.Remove(entity);
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "DisciplinaryMeasureByMoney", "Delete",
                    User.Identity.Name, $"{User.Identity.Name} deleted a discipline measure by money at {DateTime.Now}", Modules.HRM);
            }

            return Json(new[] { disciplineMeasuerByMoney }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult SendToFixedContribution(int id)
        {
            var disciplineMeasuerByMoney = db.DisciplineMeasuerByMoney.Find(id);
            if (disciplineMeasuerByMoney == null)
            {
                return Json(new
                {
                    Message = "The record you are trying to update no longer exists.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            var alreadyExists = db.FixedContributions.Any(x =>
                            x.EmployeeId == disciplineMeasuerByMoney.EmployeeId &&
                            x.GLPeriodId == disciplineMeasuerByMoney.GLPeriodId &&
                            x.Remark == "Deduction from Disciplinary Measure");

            if (alreadyExists)
            {
                return Json(new
                {
                    Message = "This employee already has a fixed contribution for this period.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }
            var disciplinaryCase = db.DisciplinaryCases.Find(disciplineMeasuerByMoney.DisciplinaryCaseId);
            if (disciplinaryCase == null)
            {
                return Json(new
                {
                    Message = "The disciplinary case record associated with this transaction no longer exists.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            //  Ensure the measure is not already sent/approved
            if (disciplineMeasuerByMoney.IsOnlineApproved)
            {
                return Json(new
                {
                    Message = "This disciplinary measure has already been sent to Fixed Contribution.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            //Get the fixed contribution type
            var hrmContributionSetting = db.HRMContributionSettings.FirstOrDefault();


            if (hrmContributionSetting == null)
            {
                return Json(new
                {
                    Message = "No Fixed Contribution type found. Please configure it first.",
                    Status = "Not-Ok"
                }, JsonRequestBehavior.AllowGet);
            }

            // Create the fixed contribution entry
            var contributionEntity = new FixedContribution
            {
                EmployeeId = disciplineMeasuerByMoney.EmployeeId,
                ContributionTypeId = hrmContributionSetting.ContributionTypeId,
                Amount = disciplineMeasuerByMoney.Amount,
                GLPeriodId = disciplineMeasuerByMoney.GLPeriodId,
                Balance = disciplineMeasuerByMoney.Balance,
                MonthlyAmount = disciplineMeasuerByMoney.MonthlyAmount,
                Months = disciplineMeasuerByMoney.Months,
                Remark = "Deduction from Disciplinary Measure",
                DateCreated = DateTime.Now,
                LastModified = null,
                CreatedBy = disciplineMeasuerByMoney.CreatedBy ?? User.Identity.Name,
                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                ModifiedBy = null,
                ApplyToAll = false,
                NetGross = 0,
                AmtPercnet = 0,
                AmountPercent = 0,
                IsActive = true
            };
            //Mark the disciplinary measure as approved
            disciplineMeasuerByMoney.IsOnlineApproved = true;
            disciplineMeasuerByMoney.OnlineApprovedBy = User.Identity.Name;
            disciplineMeasuerByMoney.OnlineApprovedTime = DateTime.Now;
            disciplinaryCase.IsOnlineApproved = true;
            disciplinaryCase.OnlineApprovedBy = User.Identity.Name;
            disciplinaryCase.OnlineApprovedTime = DateTime.Now;
            db.Entry(disciplineMeasuerByMoney).State = EntityState.Modified;
            db.Entry(disciplinaryCase).State = EntityState.Modified;
            db.FixedContributions.Add(contributionEntity);
            db.SaveChanges();

            return Json(new
            {
                Message = "You successfully sent this record to Fixed Contribution.",
                Status = "Ok"
            }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
