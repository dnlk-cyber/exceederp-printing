using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Severance;

namespace ExceedERP.Web.Areas.HRM.Controllers.Severance
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin, HrmConstants.HRAttendanceUser)]
    public class SeverancePayableDetailsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/SeverancePayableDetails/sPId
       
        public ActionResult SeverancePayableDetails_Read([DataSourceRequest]DataSourceRequest request, int sPId)
        {
            IQueryable<SeverancePayableDetail> severancePayableDetail = db.SeverancePayableDetails.Where(x=>x.SeverancePayableId==sPId);
            DataSourceResult result = severancePayableDetail.ToDataSourceResult(request, payableDetail => new
            {
                SeverancePayableDetailId = payableDetail.SeverancePayableDetailId,
                SeverancePayableId = payableDetail.SeverancePayableId,
                EmployeeCode = payableDetail.EmployeeCode,
                EmployeeId = payableDetail.EmployeeId,
                BranchId = payableDetail.BranchId,
                BusinessUnitId = payableDetail.BusinessUnitId,
                OrgStructureId = payableDetail.OrgStructureId,
                Balance = payableDetail.Balance,
                BasicSalary = payableDetail.BasicSalary,
                GrossComponsation = payableDetail.GrossComponsation,
                NetPay = payableDetail.NetPay,
                NoneTaxableTotal = payableDetail.NoneTaxableTotal,
                NumberOFYears = payableDetail.NumberOFYears,
                SpecialSalaryDeduction = payableDetail.SpecialSalaryDeduction,
                TaxWithheldComTax = payableDetail.TaxWithheldComTax,
                TaxWithheldIncomeTax = payableDetail.TaxWithheldIncomeTax,
                TotalDeductions = payableDetail.TotalDeductions,
                DateCreated = payableDetail.DateCreated,
                CreatedBy = payableDetail.CreatedBy,
                LastModified = payableDetail.LastModified,
                ModifiedBy = payableDetail.ModifiedBy
            });

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableDetails_Create([DataSourceRequest]DataSourceRequest request, SeverancePayableDetail severance,int sPId)
        {
            if (ModelState.IsValid)
            {
                severance.SeverancePayableId = sPId;
                severance.DateCreated = DateTime.Now.Date;
                severance.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(severance);
                db.SeverancePayableDetails.Add(entity);
                db.SaveChanges();
                #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable Detail ", "Success", User.Identity.Name, "Severance Payable Detail is Created By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }
            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableDetails_Update([DataSourceRequest]DataSourceRequest request, SeverancePayableDetail severance)
        {

            if (ModelState.IsValid)
            {
                severance.LastModified = DateTime.Now.Date;
                severance.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(severance);
                db.SeverancePayableDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
              
                 UserHelper.OperationLog(Request.UserHostAddress, "Severance Payable Detail", "Success", User.Identity.Name, "Severance Payable Detail is Updated By  " + User.Identity.Name,Modules.HRM);
                
                #endregion
            }


            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SeverancePayableDetails_Destroy([DataSourceRequest]DataSourceRequest request, SeverancePayableDetail severance)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(severance);
                #region delete log
                var leaves = db.Person_Employee.Find(severance.EmployeeId);
                var delete = leaves.FullNameaEnglish;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   employee"  + delete);
                #endregion
                db.SeverancePayableDetails.Attach(entity);
                db.SeverancePayableDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { severance }.ToDataSourceResult(request, ModelState));
        }
        private SeverancePayableDetail GetMappedObject(SeverancePayableDetail payableDetail)
        {
            var entity = new SeverancePayableDetail
            {
                SeverancePayableDetailId = payableDetail.SeverancePayableDetailId,
                SeverancePayableId = payableDetail.SeverancePayableId,
                EmployeeCode = payableDetail.EmployeeCode,
                EmployeeId = payableDetail.EmployeeId,
                BranchId = payableDetail.BranchId,
                BusinessUnitId = payableDetail.BusinessUnitId,
                OrgStructureId = payableDetail.OrgStructureId,
                Balance = payableDetail.Balance,
                BasicSalary = payableDetail.BasicSalary,
                GrossComponsation = payableDetail.GrossComponsation,
                NetPay = payableDetail.NetPay,
                NoneTaxableTotal = payableDetail.NoneTaxableTotal,
                NumberOFYears = payableDetail.NumberOFYears,
                SpecialSalaryDeduction = payableDetail.SpecialSalaryDeduction,
                TaxWithheldComTax = payableDetail.TaxWithheldComTax,
                TaxWithheldIncomeTax = payableDetail.TaxWithheldIncomeTax,
                TotalDeductions = payableDetail.TotalDeductions,
                DateCreated = payableDetail.DateCreated,
                CreatedBy = payableDetail.CreatedBy,
                LastModified = payableDetail.LastModified,
                ModifiedBy = payableDetail.ModifiedBy
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
