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
    public class EmployeeAccidentInsuranceClaimDepartmentsController : BaseController
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
        
        public ActionResult EEmployeeAccidentInsuranceClaimDepartments_Read([DataSourceRequest] DataSourceRequest request)
        {
            var insuranceClaims = db.EmployeeAccidentInsuranceClaims.Where(x=>x.IsDepartmentOnlineApproved);
            
            DataSourceResult result = insuranceClaims.ToDataSourceResult(request, insuranceClaim => new
            {

                EmployeeAccidentInsuranceClaimId = insuranceClaim.EmployeeAccidentInsuranceClaimId,
                EmployeeId = insuranceClaim.EmployeeId,
                BranchId = insuranceClaim.BranchId,
                BusinessUnitId = insuranceClaim.BusinessUnitId,
                OrgStructureId = insuranceClaim.OrgStructureId,
                EmployementType = insuranceClaim.EmployementType,
                ResponsibilityLevel = insuranceClaim.ResponsibilityLevel,
                Date = insuranceClaim.Date,
                DateAm = insuranceClaim.DateAm,
                CauseOfTheAccident = insuranceClaim.CauseOfTheAccident,
                InsuranceType = insuranceClaim.InsuranceType,
                TypeOfDamageCaussedByAccident = insuranceClaim.TypeOfDamageCaussedByAccident,
                OpinionOfImmediateSuperior = insuranceClaim.OpinionOfImmediateSuperior,
                IsPaid = insuranceClaim.IsPaid,
                IsTheDocumentCompleteByEIC = insuranceClaim.IsTheDocumentCompleteByEIC,
                SalaryDailyOrmonthlyBeforeTtheAccident = insuranceClaim.SalaryDailyOrmonthlyBeforeTtheAccident,
                LocationOfAccident = insuranceClaim.LocationOfAccident,
                Description = insuranceClaim.Description,
                IsVerifyied = insuranceClaim.IsVerifyied,
                IsTheDocumentComplete = insuranceClaim.IsTheDocumentComplete,
                Status = insuranceClaim.Status,
                Remark = insuranceClaim.Remark,
                CurrentApprovalStep = insuranceClaim.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = insuranceClaim.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = insuranceClaim.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = insuranceClaim.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = insuranceClaim.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = insuranceClaim.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = insuranceClaim.IsDepartmentSendForApproval,
                ForwardedFrom = insuranceClaim.ForwardedFrom,
                ForwardedTo = insuranceClaim.ForwardedTo,
                IsFinalApproval = insuranceClaim.IsFinalApproval,
                IsSendForApproval = insuranceClaim.IsSendForApproval,
                SendForApprovalTime = insuranceClaim.SendForApprovalTime,
                IsOnlineApproved = insuranceClaim.IsOnlineApproved,
                IsOnlineTransferred = insuranceClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = insuranceClaim.IsOnlineTransferredBy,
                IsVoid = insuranceClaim.IsVoid,
                OnlineApprovedBy = insuranceClaim.OnlineApprovedBy,
                OnlineApprovedTime = insuranceClaim.OnlineApprovedTime,
                OnlineTransferredTime = insuranceClaim.OnlineTransferredTime,
                SendForApprovalBy = insuranceClaim.SendForApprovalBy,
                VoidTime = insuranceClaim.VoidTime,
                OrgBranchName = insuranceClaim.OrgBranchName,
                VoidBy = insuranceClaim.VoidBy,
                DateCreated = insuranceClaim.DateCreated,
                CreatedBy = insuranceClaim.CreatedBy,
                ModifiedBy = insuranceClaim.ModifiedBy,
                LastModified = insuranceClaim.LastModified
            });
            return Json(result);
        }
        public ActionResult EmployeeAccidentInsuranceClaimDepartments_Create([DataSourceRequest] DataSourceRequest request, EmployeeAccidentInsuranceClaim insuranceClaim)
        {

          
            #region data validation
            if (insuranceClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (insuranceClaim.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == insuranceClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    insuranceClaim.OrgStructureId = placement.OrgStructureId;
                    insuranceClaim.BranchId = placement.BranchId;
                    insuranceClaim.BusinessUnitId = placement.BusinessUnitId;
                    insuranceClaim.ResponsibilityLevel = placement.ResponsibilityLevel;
                    insuranceClaim.EmployementType = placement.EmployementType;
                    insuranceClaim.SalaryDailyOrmonthlyBeforeTtheAccident = placement.Salary;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
               
            #endregion
            if (ModelState.IsValid)
            {
                insuranceClaim.CreatedBy = User.Identity.Name;
                insuranceClaim.DateCreated = DateTime.Now;
                var entity = GetMappedObject(insuranceClaim);
                db.EmployeeAccidentInsuranceClaims.Add(entity);
                db.SaveChanges();
                insuranceClaim.EmployeeAccidentInsuranceClaimId = entity.EmployeeAccidentInsuranceClaimId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Accident Insurance Claim ", "Success", User.Identity.Name, $"created Employee Accident Insurance ClaimId {entity.EmployeeAccidentInsuranceClaimId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { insuranceClaim }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeAccidentInsuranceClaimDepartments_Update([DataSourceRequest] DataSourceRequest request, EmployeeAccidentInsuranceClaim insuranceClaim)
        {
           
            #region data Validation
            if (insuranceClaim.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (insuranceClaim.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == insuranceClaim.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    insuranceClaim.OrgStructureId = placement.OrgStructureId;
                    insuranceClaim.BranchId = placement.BranchId;
                    insuranceClaim.BusinessUnitId = placement.BusinessUnitId;
                    insuranceClaim.ResponsibilityLevel = placement.ResponsibilityLevel;
                    insuranceClaim.EmployementType = placement.EmployementType;
                    insuranceClaim.SalaryDailyOrmonthlyBeforeTtheAccident = placement.Salary;

                }
                else
                {
                        ModelState.AddModelError("placement", "Employee has not current placemnt");


                }

            }

            #endregion
            if (ModelState.IsValid)
            {
                insuranceClaim.ModifiedBy = User.Identity.Name;
                insuranceClaim.LastModified = DateTime.Now;
                var entity = GetMappedObject(insuranceClaim);
                db.EmployeeAccidentInsuranceClaims.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Accident Insurance Claim update", "Success", User.Identity.Name, $"updated Employee Accident Insurance Claim {entity.EmployeeAccidentInsuranceClaimId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { insuranceClaim }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EmployeeAccidentInsuranceClaims_Destroy([DataSourceRequest] DataSourceRequest request, EmployeeAccidentInsuranceClaim insuranceClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insuranceClaim);
                db.EmployeeAccidentInsuranceClaims.Attach(entity);
                db.EmployeeAccidentInsuranceClaims.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Employee Accident Insurance Claim {entity.EmployeeAccidentInsuranceClaimId }");
                #endregion
            }

            return Json(new[] { insuranceClaim }.ToDataSourceResult(request, ModelState));
        }
        private EmployeeAccidentInsuranceClaim GetMappedObject(EmployeeAccidentInsuranceClaim insuranceClaim)
        {
            var entity = new EmployeeAccidentInsuranceClaim
            {
                EmployeeAccidentInsuranceClaimId = insuranceClaim.EmployeeAccidentInsuranceClaimId,
                EmployeeId = insuranceClaim.EmployeeId,
                BranchId = insuranceClaim.BranchId,
                BusinessUnitId = insuranceClaim.BusinessUnitId,
                OrgStructureId = insuranceClaim.OrgStructureId,
                EmployementType = insuranceClaim.EmployementType,
                ResponsibilityLevel = insuranceClaim.ResponsibilityLevel,
                Date = insuranceClaim.Date,
                DateAm = insuranceClaim.DateAm,
                CauseOfTheAccident = insuranceClaim.CauseOfTheAccident,
                InsuranceType = insuranceClaim.InsuranceType,
                TypeOfDamageCaussedByAccident = insuranceClaim.TypeOfDamageCaussedByAccident,
                OpinionOfImmediateSuperior = insuranceClaim.OpinionOfImmediateSuperior,
                IsPaid = insuranceClaim.IsPaid,
                IsTheDocumentCompleteByEIC = insuranceClaim.IsTheDocumentCompleteByEIC,
                SalaryDailyOrmonthlyBeforeTtheAccident = insuranceClaim.SalaryDailyOrmonthlyBeforeTtheAccident,
                LocationOfAccident = insuranceClaim.LocationOfAccident,
                Description = insuranceClaim.Description,
                IsVerifyied = insuranceClaim.IsVerifyied,
                IsTheDocumentComplete = insuranceClaim.IsTheDocumentComplete,
                Status = insuranceClaim.Status,
                Remark = insuranceClaim.Remark,
                CurrentApprovalStep = insuranceClaim.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = insuranceClaim.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = insuranceClaim.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = insuranceClaim.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = insuranceClaim.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = insuranceClaim.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = insuranceClaim.IsDepartmentSendForApproval,
                ForwardedFrom = insuranceClaim.ForwardedFrom,
                ForwardedTo = insuranceClaim.ForwardedTo,
                IsFinalApproval = insuranceClaim.IsFinalApproval,
                IsSendForApproval = insuranceClaim.IsSendForApproval,
                SendForApprovalTime = insuranceClaim.SendForApprovalTime,
                IsOnlineApproved = insuranceClaim.IsOnlineApproved,
                IsOnlineTransferred = insuranceClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = insuranceClaim.IsOnlineTransferredBy,
                IsVoid = insuranceClaim.IsVoid,
                OnlineApprovedBy = insuranceClaim.OnlineApprovedBy,
                OnlineApprovedTime = insuranceClaim.OnlineApprovedTime,
                OnlineTransferredTime = insuranceClaim.OnlineTransferredTime,
                SendForApprovalBy = insuranceClaim.SendForApprovalBy,
                VoidTime = insuranceClaim.VoidTime,
                OrgBranchName = insuranceClaim.OrgBranchName,
                VoidBy = insuranceClaim.VoidBy,
                DateCreated = insuranceClaim.DateCreated,
                CreatedBy = insuranceClaim.CreatedBy,
                ModifiedBy = insuranceClaim.ModifiedBy,
                LastModified = insuranceClaim.LastModified

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