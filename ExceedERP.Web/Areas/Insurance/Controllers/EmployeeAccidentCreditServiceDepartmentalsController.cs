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
    public class EmployeeAccidentCreditServiceDepartmentalsController : BaseController
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
        
        public ActionResult EmployeeAccidentCreditServiceDepartmentals_Read([DataSourceRequest] DataSourceRequest request)
        {
            var creditServices = db.EmployeeAccidentCreditServices/*.Where(x=>x.IsDepartmentOnlineApproved)*/;
            
            DataSourceResult result = creditServices.ToDataSourceResult(request, creditService => new
            {

                EmployeeAccidentCreditServiceId = creditService.EmployeeAccidentCreditServiceId,
                EmployeeId = creditService.EmployeeId,
                BranchId = creditService.BranchId,
                BusinessUnitId = creditService.BusinessUnitId,
                OrgStructureId = creditService.OrgStructureId,
                EmployementType = creditService.EmployementType,
                ResponsibilityLevel = creditService.ResponsibilityLevel,
                Date = creditService.Date,
                DateAm = creditService.DateAm,
                Description = creditService.Description,
                IsVerified = creditService.IsVerified,
                Credit = creditService.Credit,
                IsPaid = creditService.IsPaid,
                IsTheDocumentComplete = creditService.IsTheDocumentComplete,
                MedicalCenter = creditService.MedicalCenter,
                IsTheDocumentCompleteByEIC = creditService.IsTheDocumentCompleteByEIC,
                Status = creditService.Status,
                Remark = creditService.Remark,
                CurrentApprovalStep = creditService.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = creditService.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = creditService.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = creditService.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = creditService.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = creditService.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = creditService.IsDepartmentSendForApproval,
                ForwardedFrom = creditService.ForwardedFrom,
                ForwardedTo = creditService.ForwardedTo,
                IsFinalApproval = creditService.IsFinalApproval,
                IsSendForApproval = creditService.IsSendForApproval,
                SendForApprovalTime = creditService.SendForApprovalTime,
                IsOnlineApproved = creditService.IsOnlineApproved,
                IsOnlineTransferred = creditService.IsOnlineTransferred,
                IsOnlineTransferredBy = creditService.IsOnlineTransferredBy,
                IsVoid = creditService.IsVoid,
                OnlineApprovedBy = creditService.OnlineApprovedBy,
                OnlineApprovedTime = creditService.OnlineApprovedTime,
                OnlineTransferredTime = creditService.OnlineTransferredTime,
                SendForApprovalBy = creditService.SendForApprovalBy,
                VoidTime = creditService.VoidTime,
                OrgBranchName = creditService.OrgBranchName,
                VoidBy = creditService.VoidBy,
                DateCreated = creditService.DateCreated,
                CreatedBy = creditService.CreatedBy,
                ModifiedBy = creditService.ModifiedBy,
                LastModified = creditService.LastModified
            });
            return Json(result);
        }
        public ActionResult EmployeeAccidentCreditServiceDepartmentals_Create([DataSourceRequest] DataSourceRequest request, EmployeeAccidentCreditService creditService)
        {


            #region data validation
            if (!string.IsNullOrEmpty(creditService.DateAm))
            {
           
                var splitDate = creditService.DateAm.Split('/');
                creditService.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (creditService.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (creditService.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == creditService.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    creditService.OrgStructureId = placement.OrgStructureId;
                    creditService.BranchId = placement.BranchId;
                    creditService.BusinessUnitId = placement.BusinessUnitId;
                    creditService.ResponsibilityLevel = placement.ResponsibilityLevel;
                    creditService.EmployementType = placement.EmployementType;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
               
            #endregion
            if (ModelState.IsValid)
            {
                creditService.CreatedBy = User.Identity.Name;
                creditService.DateCreated = DateTime.Now;
                creditService.IsSendForApproval = true;
                var entity = GetMappedObject(creditService);
                db.EmployeeAccidentCreditServices.Add(entity);
                db.SaveChanges();
                creditService.EmployeeAccidentCreditServiceId = entity.EmployeeAccidentCreditServiceId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Accident Credit Service ", "Success", User.Identity.Name, $"created Employee Accident Credit Service {entity.EmployeeAccidentCreditServiceId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { creditService }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAccidentCreditServiceDepartmentals_Update([DataSourceRequest] DataSourceRequest request, EmployeeAccidentCreditService creditService)
        {

            #region data Validation
            if (!string.IsNullOrEmpty(creditService.DateAm))
            {

                var splitDate = creditService.DateAm.Split('/');
                creditService.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (creditService.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (creditService.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == creditService.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    creditService.OrgStructureId = placement.OrgStructureId;
                    creditService.BranchId = placement.BranchId;
                    creditService.BusinessUnitId = placement.BusinessUnitId;
                    creditService.ResponsibilityLevel = placement.ResponsibilityLevel;
                    creditService.EmployementType = placement.EmployementType;


                }
                else
                {
                        ModelState.AddModelError("placement", "Employee has not current placemnt");

                }

            }

            #endregion
            if (ModelState.IsValid)
            {
                creditService.ModifiedBy = User.Identity.Name;
                creditService.LastModified = DateTime.Now;
                var entity = GetMappedObject(creditService);
                db.EmployeeAccidentCreditServices.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Accident Insurance Claim update", "Success", User.Identity.Name, $"updated Employee Accident Insurance Claim {entity.EmployeeAccidentCreditServiceId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { creditService }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EmployeeAccidentCreditServiceDepartmentals_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeAccidentCreditService creditService)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(creditService);
                db.EmployeeAccidentCreditServices.Attach(entity);
                db.EmployeeAccidentCreditServices.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Employee Accident Credit Services {entity.EmployeeAccidentCreditServiceId }");
                #endregion
            }

            return Json(new[] { creditService }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeAccidentCreditService GetMappedObject(EmployeeAccidentCreditService creditService)
        {
            var entity = new EmployeeAccidentCreditService
            {
                EmployeeAccidentCreditServiceId = creditService.EmployeeAccidentCreditServiceId,
                EmployeeId = creditService.EmployeeId,
                BranchId = creditService.BranchId,
                BusinessUnitId = creditService.BusinessUnitId,
                OrgStructureId = creditService.OrgStructureId,
                EmployementType = creditService.EmployementType,
                ResponsibilityLevel = creditService.ResponsibilityLevel,
                Date = creditService.Date,
                DateAm = creditService.DateAm,
                Description = creditService.Description,
                IsVerified = creditService.IsVerified,
                IsPaid = creditService.IsPaid,
                IsTheDocumentComplete = creditService.IsTheDocumentComplete,
                MedicalCenter = creditService.MedicalCenter,
                Credit = creditService.Credit,
                IsTheDocumentCompleteByEIC = creditService.IsTheDocumentCompleteByEIC,
                Status = creditService.Status,
                Remark = creditService.Remark,
                CurrentApprovalStep = creditService.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = creditService.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = creditService.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = creditService.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = creditService.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = creditService.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = creditService.IsDepartmentSendForApproval,
                ForwardedFrom = creditService.ForwardedFrom,
                ForwardedTo = creditService.ForwardedTo,
                IsFinalApproval = creditService.IsFinalApproval,
                IsSendForApproval = creditService.IsSendForApproval,
                SendForApprovalTime = creditService.SendForApprovalTime,
                IsOnlineApproved = creditService.IsOnlineApproved,
                IsOnlineTransferred = creditService.IsOnlineTransferred,
                IsOnlineTransferredBy = creditService.IsOnlineTransferredBy,
                IsVoid = creditService.IsVoid,
                OnlineApprovedBy = creditService.OnlineApprovedBy,
                OnlineApprovedTime = creditService.OnlineApprovedTime,
                OnlineTransferredTime = creditService.OnlineTransferredTime,
                SendForApprovalBy = creditService.SendForApprovalBy,
                VoidTime = creditService.VoidTime,
                OrgBranchName = creditService.OrgBranchName,
                VoidBy = creditService.VoidBy,
                DateCreated = creditService.DateCreated,
                CreatedBy = creditService.CreatedBy,
                ModifiedBy = creditService.ModifiedBy,
                LastModified = creditService.LastModified

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