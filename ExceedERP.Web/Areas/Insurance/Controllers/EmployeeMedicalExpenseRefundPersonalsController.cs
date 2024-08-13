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
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Areas.Insurance.Models;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [Authorize]
    public class EmployeeMedicalExpenseRefundPersonalsController : BaseController
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

            ViewData["personEmployees"] = selectedEmployees;
            ViewData["branch"] = branches;
            ViewData["businessUnit"] = businessUnits;
            ViewData["workUnit"] = workUnits;
            return View();

        }
        
        public ActionResult EmployeeMedicalExpenseRefundPersonals_Read([DataSourceRequest] DataSourceRequest request)
        {

            var empId  = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var medicalExpenseRefunds = db.EmployeeMedicalExpenseRefunds.Where(x=>x.EmployeeId==empId);
            
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
        public ActionResult EmployeeMedicalExpenseRefundPersonals_Create([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {

            #region data validation
            if (!string.IsNullOrEmpty(medicalExpenseRefund.DateAm))
            {
                var splitDate = medicalExpenseRefund.DateAm.Split('/');
                medicalExpenseRefund.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
                return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));

            }
            medicalExpenseRefund.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
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
            var setting = db.InsurancePaymentSettings
                .FirstOrDefault(x => x.EmployeeType == medicalExpenseRefund.EmployementType
                                  && x.Level == medicalExpenseRefund.ResponsibilityLevel
                                  && x.BenefitType == medicalExpenseRefund.BenefitType);
            if (setting == null)
            {
                ModelState.AddModelError("exist", "No matching insurance payment settings were found. Please ensure the correct settings are configured.");
                return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));
            }
            // Continue with processing if the setting is found

            if (setting != null && setting.CurrentValue != null)
            {
                if (setting.CurrentValue > medicalExpenseRefund.AmountInBirr)
                {
                    medicalExpenseRefund.AllowedAmount = (decimal)setting.CurrentValue;
                    medicalExpenseRefund.UsedAmount = medicalExpenseRefund.AmountInBirr;
                    medicalExpenseRefund.RemainingAmount = (decimal)setting.CurrentValue - medicalExpenseRefund.AmountInBirr;


                }
                else
                {
                    medicalExpenseRefund.AllowedAmount = (decimal)setting.CurrentValue;
                    medicalExpenseRefund.UsedAmount = (decimal)setting.CurrentValue;
                    medicalExpenseRefund.RemainingAmount = 0;
                }

            }
            else if (setting != null && setting.CurrentValue == null)
            {

                medicalExpenseRefund.AllowedAmount = null;
                medicalExpenseRefund.UsedAmount = null;
                medicalExpenseRefund.RemainingAmount = null;




            }
            medicalExpenseRefund.TotalExpense = 0;
            medicalExpenseRefund.Deduction = 0;
            //  medicalExpenseRefund.ReasonForDeduction = medicalExpenseRefund.ReasonForDeduction,
            medicalExpenseRefund.NetPay = 0;
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
        public ActionResult EmployeeMedicalExpenseRefundPersonals_Update([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
        {

            #region data Validation

            if (!string.IsNullOrEmpty(medicalExpenseRefund.DateAm))
            {
                var splitDate = medicalExpenseRefund.DateAm.Split('/');
                medicalExpenseRefund.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
                return Json(new[] { medicalExpenseRefund }.ToDataSourceResult(request, ModelState));

            }
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
        public ActionResult EmployeeMedicalExpenseRefundPersonals_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeMedicalExpenseRefund medicalExpenseRefund)
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

        public JsonResult SendForApprovalRequest(int id)
        {
            var record = db.EmployeeMedicalExpenseRefunds.Find(id);

            if (record == null)
            {
                return Json(new
                {
                    Message = "Record not found.",
                    Status = "Error"
                }, JsonRequestBehavior.AllowGet);
            }

            var attachmentCount = db.MedicalExpenseRefundAttachments
                                   .Count(x => x.EmployeeMedicalExpenseRefundId == record.EmployeeMedicalExpenseRefundId);

            if (attachmentCount <= record.NumberOfDocument && attachmentCount > 0)
            {
                record.IsSendForApproval = true;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = "You have successfully sent for approval.",
                    Status = "OK"
                }, JsonRequestBehavior.AllowGet);
            }

            if (attachmentCount == 0)
            {
                return Json(new
                {
                    Message = "You have to attach all files before sending for approval.",
                    Status = "Error"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Message = "It has already been sent for approval.",
                Status = "Error"
            }, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}