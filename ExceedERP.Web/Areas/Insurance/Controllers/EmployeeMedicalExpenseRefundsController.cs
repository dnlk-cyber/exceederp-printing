using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Insurance.InsuranceNew;
using ExceedERP.Web.Areas.Insurance.Models;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [Authorize]
    public class EmployeeMedicalExpenseRefundsController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var selectedEmployees = db.Person_Employee.Select(i => new EmployeeViewModel
            {
                EmployeeId = i.EmployeeId,
                FullName = i.EmployeeCode + "-" + i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish
            }).ToList();

            var branches = db.Branch.Select(x => new BranchViewModel
            {
                BranchId = x.BranchId,
                Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ")
            }).ToList();

            var businessUnits = db.BusinessUnits.Select(x => new BusinessUnitViewModel
            {
                BusinessUnitId = x.BusinessUnitId,
                Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ")
            }).ToList();

            var workUnits = db.MainCompanyStructures.Select(x => new OrgStructureViewModel
            {
                OrgStructureId = x.OrgStructureId,
                Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ")
            }).ToList();
            var fiscalYears = db.GLFiscalYears.Select(x => new FiscalYearViewModel
            {
                GlFiscalYearId = x.GlFiscalYearId,
                Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ")
            }).ToList();
            var glPeriods = db.GLPeriods.Select(x => new GLPeriodViewModel
            {
                GLPeriodId = x.GLPeriodId,
                Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ")
            }).ToList();
            ViewData["personEmployees"] = selectedEmployees;
            ViewData["branch"] = branches;
            ViewData["businessUnit"] = businessUnits;
            ViewData["workUnit"] = workUnits;
            ViewData["FiscalYear"] = fiscalYears;
            ViewData["Period"] = glPeriods;
            return View();
        }
        
        public ActionResult EmployeeMedicalExpenseRefunds_Read([DataSourceRequest] DataSourceRequest request)
        {
            var medicalExpenseRefunds = db.EmployeeMedicalExpenseRefunds.Where(x=>x.IsDepartmentOnlineApproved);
            
            DataSourceResult result = medicalExpenseRefunds.ToDataSourceResult(request, medicalExpenseRefund => new
            {

                EmployeeMedicalExpenseRefundId = medicalExpenseRefund.EmployeeMedicalExpenseRefundId,
                EmployeeId = medicalExpenseRefund.EmployeeId,
                BranchId = medicalExpenseRefund.BranchId,
                BusinessUnitId = medicalExpenseRefund.BusinessUnitId,
                OrgStructureId = medicalExpenseRefund.OrgStructureId,
                GlFiscalYearId = medicalExpenseRefund.GlFiscalYearId,
                GLPeriodId = medicalExpenseRefund.GLPeriodId,
                BenefitType = medicalExpenseRefund.BenefitType,
                EmployementType = medicalExpenseRefund.EmployementType,
                ResponsibilityLevel = medicalExpenseRefund.ResponsibilityLevel,
                AmountInBirr = medicalExpenseRefund.AmountInBirr,
                AllowedAmount = medicalExpenseRefund.AllowedAmount,
                UsedAmount = medicalExpenseRefund.UsedAmount,
                RemainingAmount = medicalExpenseRefund.RemainingAmount,
                DepartmentAproverId = medicalExpenseRefund.DepartmentAproverId,
                NumberOfDocument = medicalExpenseRefund.NumberOfDocument,
                TotalExpense = medicalExpenseRefund.TotalExpense,
                Deduction = medicalExpenseRefund.Deduction,
                ReasonForDeduction = medicalExpenseRefund.ReasonForDeduction,
                NetPay = medicalExpenseRefund.NetPay,
                Date = medicalExpenseRefund.Date,
                DateAm = medicalExpenseRefund.DateAm,
                IsVerifyied = medicalExpenseRefund.IsVerifyied,
                IsTheDocumentComplete = medicalExpenseRefund.IsTheDocumentComplete,
                Status = medicalExpenseRefund.Status,
                Remark = medicalExpenseRefund.Remark,
                CurrentApprovalStep = medicalExpenseRefund.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = medicalExpenseRefund.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = medicalExpenseRefund.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = medicalExpenseRefund.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = medicalExpenseRefund.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = medicalExpenseRefund.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = medicalExpenseRefund.IsDepartmentSendForApproval,
                ForwardedFrom = medicalExpenseRefund.ForwardedFrom,
                ForwardedTo = medicalExpenseRefund.ForwardedTo,
                IsFinalApproval = medicalExpenseRefund.IsFinalApproval,
                IsSendForApproval = medicalExpenseRefund.IsSendForApproval,
                SendForApprovalTime = medicalExpenseRefund.SendForApprovalTime,
                IsOnlineApproved = medicalExpenseRefund.IsOnlineApproved,
                IsOnlineTransferred = medicalExpenseRefund.IsOnlineTransferred,
                IsOnlineTransferredBy = medicalExpenseRefund.IsOnlineTransferredBy,
                IsVoid = medicalExpenseRefund.IsVoid,
                OnlineApprovedBy = medicalExpenseRefund.OnlineApprovedBy,
                OnlineApprovedTime = medicalExpenseRefund.OnlineApprovedTime,
                OnlineTransferredTime = medicalExpenseRefund.OnlineTransferredTime,
                SendForApprovalBy = medicalExpenseRefund.SendForApprovalBy,
                VoidTime = medicalExpenseRefund.VoidTime,
                OrgBranchName = medicalExpenseRefund.OrgBranchName,
                VoidBy = medicalExpenseRefund.VoidBy,
                DateCreated = medicalExpenseRefund.DateCreated,
                CreatedBy = medicalExpenseRefund.CreatedBy,
                ModifiedBy = medicalExpenseRefund.ModifiedBy,
                LastModified = medicalExpenseRefund.LastModified
            });
            return Json(result);
        }
        public ActionResult EmployeeMedicalExpenseRefunds_Create([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {

          
            #region data validation
            if (medicalExpenseRefund.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (medicalExpenseRefund.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == medicalExpenseRefund.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    medicalExpenseRefund.OrgStructureId = placement.OrgStructureId;
                    medicalExpenseRefund.BranchId = placement.BranchId;
                    medicalExpenseRefund.BusinessUnitId = placement.BusinessUnitId;
                    medicalExpenseRefund.ResponsibilityLevel = placement.ResponsibilityLevel;
                    medicalExpenseRefund.EmployementType = placement.EmployementType;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
               
            #endregion
            if (ModelState.IsValid)
            {
                medicalExpenseRefund.CreatedBy = User.Identity.Name;
                medicalExpenseRefund.DateCreated = DateTime.Now;
                var entity = GetMappedObject(medicalExpenseRefund);
                db.EmployeeMedicalExpenseRefunds.Add(entity);
                db.SaveChanges();

                medicalExpenseRefund.EmployeeMedicalExpenseRefundId = entity.EmployeeMedicalExpenseRefundId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Medical Expense Refund ", "Success", User.Identity.Name, $"created Employee Medical Expense Refund {entity.EmployeeMedicalExpenseRefundId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeMedicalExpenseRefunds_Update([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {
           
            #region data Validation
            if (medicalExpenseRefund.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (medicalExpenseRefund.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == medicalExpenseRefund.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    medicalExpenseRefund.OrgStructureId = placement.OrgStructureId;
                    medicalExpenseRefund.BranchId = placement.BranchId;
                    medicalExpenseRefund.BusinessUnitId = placement.BusinessUnitId;
                    medicalExpenseRefund.ResponsibilityLevel = placement.ResponsibilityLevel;
                    medicalExpenseRefund.EmployementType = placement.EmployementType;


                }
                else
                {
                        ModelState.AddModelError("placement", "Employee has not current placemnt");

                }

            }

            #endregion
            if (ModelState.IsValid)
            {
                medicalExpenseRefund.ModifiedBy = User.Identity.Name;
                medicalExpenseRefund.LastModified = DateTime.Now;
                var entity = GetMappedObject(medicalExpenseRefund);
                db.EmployeeMedicalExpenseRefunds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Medical Expense Refund update", "Success", User.Identity.Name, $"updated Employee Medical Expense Refund {entity.EmployeeMedicalExpenseRefundId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EmployeeMedicalExpenseRefunds_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(medicalExpenseRefund);
                db.EmployeeMedicalExpenseRefunds.Attach(entity);
                db.EmployeeMedicalExpenseRefunds.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted air ticket request {entity.EmployeeMedicalExpenseRefundId }");
                #endregion
            }

            return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeMedicalExpenseRefund GetMappedObject(EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {
            var entity = new EmployeeMedicalExpenseRefund
            {
                EmployeeMedicalExpenseRefundId = medicalExpenseRefund.EmployeeMedicalExpenseRefundId,
                EmployeeId = medicalExpenseRefund.EmployeeId,
                BranchId = medicalExpenseRefund.BranchId,
                BusinessUnitId = medicalExpenseRefund.BusinessUnitId,
                OrgStructureId = medicalExpenseRefund.OrgStructureId,
                GlFiscalYearId = medicalExpenseRefund.GlFiscalYearId,
                GLPeriodId = medicalExpenseRefund.GLPeriodId,
                BenefitType = medicalExpenseRefund.BenefitType,
                EmployementType = medicalExpenseRefund.EmployementType,
                ResponsibilityLevel = medicalExpenseRefund.ResponsibilityLevel,
                AmountInBirr = medicalExpenseRefund.AmountInBirr,
                AllowedAmount = medicalExpenseRefund.AllowedAmount,
                UsedAmount = medicalExpenseRefund.UsedAmount,
                RemainingAmount = medicalExpenseRefund.RemainingAmount,
                DepartmentAproverId = medicalExpenseRefund.DepartmentAproverId,
                NumberOfDocument = medicalExpenseRefund.NumberOfDocument,
                TotalExpense = medicalExpenseRefund.TotalExpense,
                Deduction = medicalExpenseRefund.Deduction,
                ReasonForDeduction = medicalExpenseRefund.ReasonForDeduction,
                NetPay = medicalExpenseRefund.NetPay,
                Date = medicalExpenseRefund.Date,
                DateAm = medicalExpenseRefund.DateAm,
                IsVerifyied = medicalExpenseRefund.IsVerifyied,
                IsTheDocumentComplete = medicalExpenseRefund.IsTheDocumentComplete,
                Status = medicalExpenseRefund.Status,
                Remark = medicalExpenseRefund.Remark,
                CurrentApprovalStep = medicalExpenseRefund.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = medicalExpenseRefund.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = medicalExpenseRefund.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = medicalExpenseRefund.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = medicalExpenseRefund.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = medicalExpenseRefund.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = medicalExpenseRefund.IsDepartmentSendForApproval,
                ForwardedFrom = medicalExpenseRefund.ForwardedFrom,
                ForwardedTo = medicalExpenseRefund.ForwardedTo,
                IsFinalApproval = medicalExpenseRefund.IsFinalApproval,
                IsSendForApproval = medicalExpenseRefund.IsSendForApproval,
                SendForApprovalTime = medicalExpenseRefund.SendForApprovalTime,
                IsOnlineApproved = medicalExpenseRefund.IsOnlineApproved,
                IsOnlineTransferred = medicalExpenseRefund.IsOnlineTransferred,
                IsOnlineTransferredBy = medicalExpenseRefund.IsOnlineTransferredBy,
                IsVoid = medicalExpenseRefund.IsVoid,
                OnlineApprovedBy = medicalExpenseRefund.OnlineApprovedBy,
                OnlineApprovedTime = medicalExpenseRefund.OnlineApprovedTime,
                OnlineTransferredTime = medicalExpenseRefund.OnlineTransferredTime,
                SendForApprovalBy = medicalExpenseRefund.SendForApprovalBy,
                VoidTime = medicalExpenseRefund.VoidTime,
                OrgBranchName = medicalExpenseRefund.OrgBranchName,
                VoidBy = medicalExpenseRefund.VoidBy,
                DateCreated = medicalExpenseRefund.DateCreated,
                CreatedBy = medicalExpenseRefund.CreatedBy,
                ModifiedBy = medicalExpenseRefund.ModifiedBy,
                LastModified = medicalExpenseRefund.LastModified

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
   

}