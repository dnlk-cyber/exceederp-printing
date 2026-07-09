using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.Global;
using XAPI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class LeavePayableDetailsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/LeavePayableDetails/leavPId
       
        public ActionResult LeavePayableDetails_Read([DataSourceRequest]DataSourceRequest request, int leavPId)
        {
            IQueryable<LeavePayableDetail> leavePayables = db.LeavePayableDetails.Where(x=>x.LeavePayableId == leavPId);
            DataSourceResult result = leavePayables.ToDataSourceResult(request, leavePayable => new
            {
                LeavePayableDetailId = leavePayable.LeavePayableDetailId,
                LeavePayableId = leavePayable.LeavePayableId,
                EmployeeCode = leavePayable.EmployeeCode,
                EmployeeId = leavePayable.EmployeeId,
                BranchId = leavePayable.BranchId,
                BusinessUnitId = leavePayable.BusinessUnitId,
                OrgStructureId = leavePayable.OrgStructureId,
                BasicSalary = leavePayable.BasicSalary,
                Year1 = leavePayable.Year1,
                Year2 = leavePayable.Year2,
                PayableDays = leavePayable.PayableDays,
                Balance = leavePayable.Balance,
                AgregateTaxableAmount = leavePayable.AgregateTaxableAmount,
                AgregateIncomeTax = leavePayable.AgregateIncomeTax,
                AnnualLeavePerMonth = leavePayable.AnnualLeavePerMonth,
                BasicSalaryIncomeTax = leavePayable.BasicSalaryIncomeTax,
                TaxDifference = leavePayable.TaxDifference,
                NetPayment = leavePayable.NetPayment,
                IsSendForPayment = leavePayable.IsSendForPayment,
                SendForPaymentBy = leavePayable.SendForPaymentBy,
                SendForPaymentTime = leavePayable.SendForPaymentTime,
                Remark = leavePayable.Remark,
                OrgBranchName = leavePayable.OrgBranchName,
                DateCreated = leavePayable.DateCreated,
                CreatedBy = leavePayable.CreatedBy,
                LastModified = leavePayable.LastModified,
                ModifiedBy = leavePayable.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayableDetails_Create([DataSourceRequest]DataSourceRequest request, LeavePayableDetail leave,int lPId)
        {
            if (ModelState.IsValid)
            {
                leave.LeavePayableId = lPId;
                leave.DateCreated = DateTime.Now.Date;
                leave.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(leave);
                db.LeavePayableDetails.Add(entity);
                db.SaveChanges();
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Leave payable ", "Success", User.Identity.Name, "Leave payable  is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayableDetails_Update([DataSourceRequest] DataSourceRequest request,LeavePayableDetail payableDetail)
        {
            //  Get required data early
            var placement = db.Placements
                .FirstOrDefault(x => x.isCurrent && x.EmployeeId == payableDetail.EmployeeId);

            if (placement == null || placement.Salary <= 0)
            {
                ModelState.AddModelError("", "Employee has no valid current placement or salary is not set.");
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
            }

            var leavePayable = db.LeavePayables
                .FirstOrDefault(x => x.LeavePayableId == payableDetail.LeavePayableId);

            if (leavePayable == null)
            {
                ModelState.AddModelError("", "Leave payable record not found.");
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
            }

            //var leavePayableDetail = db.LeavePayableDetails
            //    .FirstOrDefault(x => x.LeavePayableDetailId == payableDetail.LeavePayableDetailId);

            if (payableDetail == null)
            {
                ModelState.AddModelError("", "Leave payable detail record not found.");
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
            }

            var fiscalYear = db.GLFiscalYears.Find(leavePayable.GlFiscalYearId);
            if (fiscalYear == null)
            {
                ModelState.AddModelError("", "Fiscal year is not configured properly.");
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
            }

            var setting = db.LeavePayableSettings.FirstOrDefault();
            if (setting == null || setting.CurrentValue <= 0)
            {
                ModelState.AddModelError("", "Leave payable settings are missing or invalid.");
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
            }

            if (!ModelState.IsValid)
                return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));

            //  Calculations
            decimal totalDays = payableDetail.Year1 + payableDetail.Year2;
            decimal salary = placement.Salary;

            decimal totalPayable = (salary * totalDays) / setting.CurrentValue;
            decimal monthlyLeave = totalPayable / 12;
            decimal aggregateTaxable = monthlyLeave + salary;

            decimal aggregateTax = (decimal)IncomeTax.GetIncomeTax((float)aggregateTaxable);
            //decimal aggregateTax = CalculateTax(aggregateTaxable);
           // decimal basicTax = CalculateBasicSalaryIncomeTax(salary);
            decimal basicTax = (decimal)IncomeTax.GetIncomeTax((float)salary);

            decimal taxDifference = (aggregateTax - basicTax) * 12;
            decimal netPayment = totalPayable - taxDifference;

            //  Update entity
            payableDetail.Year1 = payableDetail.Year1;
            payableDetail.Year2 = payableDetail.Year2;
            payableDetail.PayableDays = totalDays;

            payableDetail.BasicSalary = salary;
            payableDetail.Balance = totalPayable;
            payableDetail.AnnualLeavePerMonth = monthlyLeave;

            payableDetail.AgregateTaxableAmount = aggregateTaxable;
            payableDetail.AgregateIncomeTax = aggregateTax;
            payableDetail.BasicSalaryIncomeTax = basicTax;

            payableDetail.TaxDifference = taxDifference;
            payableDetail.NetPayment = netPayment;

            payableDetail.Remark = "Leave payable updated";
            payableDetail.LastModified = DateTime.Now;
            payableDetail.ModifiedBy = User.Identity.Name;
            var entity = GetMappedObject(payableDetail);
            db.LeavePayableDetails.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            // Logging
            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Leave Payable Detail",
                "Success",
                User.Identity.Name,
                $"Leave payable detail updated by {User.Identity.Name}",
                Modules.HRM
            );

            return Json(new[] { payableDetail }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeavePayables_Destroy([DataSourceRequest]DataSourceRequest request, LeavePayableDetail leave)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(leave);
                #region delete log
                var leaves = db.Person_Employee.Find(leave.EmployeeId);
                var delete = leaves.FullNameaEnglish;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   employee"  + delete);
                #endregion
                db.LeavePayableDetails.Attach(entity);
                db.LeavePayableDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
        }
        private LeavePayableDetail GetMappedObject(LeavePayableDetail leavePayable)
        {
            var entity = new LeavePayableDetail
            {
                LeavePayableDetailId = leavePayable.LeavePayableDetailId,
                LeavePayableId = leavePayable.LeavePayableId,
                EmployeeCode = leavePayable.EmployeeCode,
                EmployeeId = leavePayable.EmployeeId,
                BranchId = leavePayable.BranchId,
                BusinessUnitId = leavePayable.BusinessUnitId,
                OrgStructureId = leavePayable.OrgStructureId,
                Year1 = leavePayable.Year1,
                Year2 = leavePayable.Year2,
                PayableDays = leavePayable.PayableDays,
                Balance = leavePayable.Balance,
                BasicSalary = leavePayable.BasicSalary,
                AgregateTaxableAmount = leavePayable.AgregateTaxableAmount,
                AgregateIncomeTax = leavePayable.AgregateIncomeTax,
                AnnualLeavePerMonth = leavePayable.AnnualLeavePerMonth,
                BasicSalaryIncomeTax = leavePayable.BasicSalaryIncomeTax,
                TaxDifference = leavePayable.TaxDifference,
                NetPayment = leavePayable.NetPayment,
                IsSendForPayment = leavePayable.IsSendForPayment,
                SendForPaymentBy = leavePayable.SendForPaymentBy,
                SendForPaymentTime = leavePayable.SendForPaymentTime,
                PaymentRequestId = leavePayable.PaymentRequestId,
                Remark = leavePayable.Remark,
                OrgBranchName = leavePayable.OrgBranchName,
                DateCreated = leavePayable.DateCreated,
                CreatedBy = leavePayable.CreatedBy,
                LastModified = leavePayable.LastModified,
                ModifiedBy = leavePayable.ModifiedBy
            };
            return entity;
        }

        public decimal CalculateTax(decimal income)
        {
            if (income <= 600)
                return income * 0;
            else if (income <= 1650)
                return income * 0.1m - 60;
            else if (income <= 3200)
                return income * 0.15m - 142.5m;
            else if (income <= 5250)
                return income * 0.2m - 302.5m;
            else if (income <= 7800)
                return income * 0.25m - 565;
            else if (income <= 10900)
                return income * 0.3m - 955;
            else // if income > 10900
                return income * 0.35m - 1500; // Adjust as needed for income > 10900
        }
        public decimal CalculateBasicSalaryIncomeTax(decimal income)
        {
            if (income <= 600)
                return income * 0;
            else if (income <= 1650)
                return income * 0.1m - 60;
            else if (income <= 3200)
                return income * 0.15m - 142.5m;
            else if (income <= 5250)
                return income * 0.2m - 302.5m;
            else if (income <= 7800)
                return income * 0.25m - 565;
            else if (income <= 10900)
                return income * 0.3m - 955;
            else // income > 10900
                return income * 0.35m - 1500;
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported employee leave plan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported employee leave plan");
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
