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

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class LeavePayableDetailsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/LeavePayableDetails/leavPId
       
        public ActionResult LeavePayableDetails_Read([DataSourceRequest]DataSourceRequest request, int leavPId)
        {
            IQueryable<LeavePayableDetail> leavePayables = db.LeavePayableDetails.Where(x=>x.LeavePayableId== leavPId);
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
        public ActionResult LeavePayableDetails_Update([DataSourceRequest]DataSourceRequest request, LeavePayableDetail leave)
        {

            if (ModelState.IsValid)
            {
                leave.LastModified = DateTime.Now.Date;
                leave.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(leave);
                db.LeavePayableDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
              
                 UserHelper.OperationLog(Request.UserHostAddress, "Leave payable detail", "Success", User.Identity.Name, "Leave payable Detail is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { leave }.ToDataSourceResult(request, ModelState));
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
                Remark = leavePayable.Remark,
                OrgBranchName = leavePayable.OrgBranchName,
                DateCreated = leavePayable.DateCreated,
                CreatedBy = leavePayable.CreatedBy,
                LastModified = leavePayable.LastModified,
                ModifiedBy = leavePayable.ModifiedBy
            };
            return entity;
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
