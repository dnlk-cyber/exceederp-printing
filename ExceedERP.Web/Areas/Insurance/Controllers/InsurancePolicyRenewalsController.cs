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
    public class InsurancePolicyRenewalsController : BaseController
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
        
        public ActionResult InsurancePolicyRenewals_Read([DataSourceRequest] DataSourceRequest request)
        {
            var insurancePolicies = db.InsurancePolicyRenewals.Where(x => x.IsDepartmentOnlineApproved);
            
            DataSourceResult result = insurancePolicies.ToDataSourceResult(request, insurancePolicy => new
            {

                InsurancePolicyRenewalId = insurancePolicy.InsurancePolicyRenewalId,
                EmployeeId = insurancePolicy.EmployeeId,
                BranchId = insurancePolicy.BranchId,
                BusinessUnitId = insurancePolicy.BusinessUnitId,
                OrgStructureId = insurancePolicy.OrgStructureId,
                EmployementType = insurancePolicy.EmployementType,
                ResponsibilityLevel = insurancePolicy.ResponsibilityLevel,
                Date = insurancePolicy.Date,
                DateAm = insurancePolicy.DateAm,
                Description = insurancePolicy.Description,
                IsVerified = insurancePolicy.IsVerified,
                IsPaid = insurancePolicy.IsPaid,
                IsTheDocumentComplete = insurancePolicy.IsTheDocumentComplete,
                Machine = insurancePolicy.Machine,
                Vehicle = insurancePolicy.Vehicle,
                PaymentAmount = insurancePolicy.PaymentAmount,
                IsTheDocumentCompleteByEIC = insurancePolicy.IsTheDocumentCompleteByEIC,
                Status = insurancePolicy.Status,
                Remark = insurancePolicy.Remark,
                CurrentApprovalStep = insurancePolicy.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = insurancePolicy.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = insurancePolicy.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = insurancePolicy.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = insurancePolicy.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = insurancePolicy.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = insurancePolicy.IsDepartmentSendForApproval,
                ForwardedFrom = insurancePolicy.ForwardedFrom,
                ForwardedTo = insurancePolicy.ForwardedTo,
                IsFinalApproval = insurancePolicy.IsFinalApproval,
                IsSendForApproval = insurancePolicy.IsSendForApproval,
                SendForApprovalTime = insurancePolicy.SendForApprovalTime,
                IsOnlineApproved = insurancePolicy.IsOnlineApproved,
                IsOnlineTransferred = insurancePolicy.IsOnlineTransferred,
                IsOnlineTransferredBy = insurancePolicy.IsOnlineTransferredBy,
                IsVoid = insurancePolicy.IsVoid,
                OnlineApprovedBy = insurancePolicy.OnlineApprovedBy,
                OnlineApprovedTime = insurancePolicy.OnlineApprovedTime,
                OnlineTransferredTime = insurancePolicy.OnlineTransferredTime,
                SendForApprovalBy = insurancePolicy.SendForApprovalBy,
                VoidTime = insurancePolicy.VoidTime,
                OrgBranchName = insurancePolicy.OrgBranchName,
                VoidBy = insurancePolicy.VoidBy,
                DateCreated = insurancePolicy.DateCreated,
                CreatedBy = insurancePolicy.CreatedBy,
                ModifiedBy = insurancePolicy.ModifiedBy,
                LastModified = insurancePolicy.LastModified
            });
            return Json(result);
        }
        public ActionResult InsurancePolicyRenewals_Create([DataSourceRequest] DataSourceRequest request, InsurancePolicyRenewal insurancePolicy)
        {


            #region data validation
            if (!string.IsNullOrEmpty(insurancePolicy.DateAm))
            {
           
                var splitDate = insurancePolicy.DateAm.Split('/');
                insurancePolicy.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (insurancePolicy.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (insurancePolicy.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == insurancePolicy.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    insurancePolicy.OrgStructureId = placement.OrgStructureId;
                    insurancePolicy.BranchId = placement.BranchId;
                    insurancePolicy.BusinessUnitId = placement.BusinessUnitId;
                    insurancePolicy.ResponsibilityLevel = placement.ResponsibilityLevel;
                    insurancePolicy.EmployementType = placement.EmployementType;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }
               
            #endregion
            if (ModelState.IsValid)
            {
                insurancePolicy.CreatedBy = User.Identity.Name;
                insurancePolicy.DateCreated = DateTime.Now;
                var entity = GetMappedObject(insurancePolicy);
                db.InsurancePolicyRenewals.Add(entity);
                db.SaveChanges();
                insurancePolicy.InsurancePolicyRenewalId = entity.InsurancePolicyRenewalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Policy Renewal ", "Success", User.Identity.Name, $"created Insurance Policy Renewal {entity.InsurancePolicyRenewalId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { insurancePolicy }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsurancePolicyRenewals_Update([DataSourceRequest] DataSourceRequest request, InsurancePolicyRenewal insurancePolicy)
        {

            #region data Validation
            if (!string.IsNullOrEmpty(insurancePolicy.DateAm))
            {

                var splitDate = insurancePolicy.DateAm.Split('/');
                insurancePolicy.Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (insurancePolicy.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            if (insurancePolicy.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == insurancePolicy.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    insurancePolicy.OrgStructureId = placement.OrgStructureId;
                    insurancePolicy.BranchId = placement.BranchId;
                    insurancePolicy.BusinessUnitId = placement.BusinessUnitId;
                    insurancePolicy.ResponsibilityLevel = placement.ResponsibilityLevel;
                    insurancePolicy.EmployementType = placement.EmployementType;


                }
                else
                {
                        ModelState.AddModelError("placement", "Employee has not current placemnt");

                }

            }

            #endregion
            if (ModelState.IsValid)
            {
                insurancePolicy.ModifiedBy = User.Identity.Name;
                insurancePolicy.LastModified = DateTime.Now;
                var entity = GetMappedObject(insurancePolicy);
                db.InsurancePolicyRenewals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Policy Renewal update", "Success", User.Identity.Name, $"updated Insurance Policy Renewal {entity.InsurancePolicyRenewalId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { insurancePolicy }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult InsurancePolicyRenewals_Destroy([DataSourceRequest] DataSourceRequest request, InsurancePolicyRenewal insurancePolicy)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(insurancePolicy);
                db.InsurancePolicyRenewals.Attach(entity);
                db.InsurancePolicyRenewals.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Insurance Policy Renewal {entity.InsurancePolicyRenewalId }");
                #endregion
            }

            return Json(new[] { insurancePolicy }.ToDataSourceResult(request, ModelState));
        }
        private InsurancePolicyRenewal GetMappedObject(InsurancePolicyRenewal insurancePolicy)
        {
            var entity = new InsurancePolicyRenewal
            {
                InsurancePolicyRenewalId = insurancePolicy.InsurancePolicyRenewalId,
                EmployeeId = insurancePolicy.EmployeeId,
                BranchId = insurancePolicy.BranchId,
                BusinessUnitId = insurancePolicy.BusinessUnitId,
                OrgStructureId = insurancePolicy.OrgStructureId,
                EmployementType = insurancePolicy.EmployementType,
                ResponsibilityLevel = insurancePolicy.ResponsibilityLevel,
                Date = insurancePolicy.Date,
                DateAm = insurancePolicy.DateAm,
                Description = insurancePolicy.Description,
                IsVerified = insurancePolicy.IsVerified,
                IsPaid = insurancePolicy.IsPaid,
                IsTheDocumentComplete = insurancePolicy.IsTheDocumentComplete,
                Machine = insurancePolicy.Machine,
                Vehicle = insurancePolicy.Vehicle,
                PaymentAmount = insurancePolicy.PaymentAmount,
                IsTheDocumentCompleteByEIC = insurancePolicy.IsTheDocumentCompleteByEIC,
                Status = insurancePolicy.Status,
                Remark = insurancePolicy.Remark,
                CurrentApprovalStep = insurancePolicy.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = insurancePolicy.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = insurancePolicy.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = insurancePolicy.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = insurancePolicy.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = insurancePolicy.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = insurancePolicy.IsDepartmentSendForApproval,
                ForwardedFrom = insurancePolicy.ForwardedFrom,
                ForwardedTo = insurancePolicy.ForwardedTo,
                IsFinalApproval = insurancePolicy.IsFinalApproval,
                IsSendForApproval = insurancePolicy.IsSendForApproval,
                SendForApprovalTime = insurancePolicy.SendForApprovalTime,
                IsOnlineApproved = insurancePolicy.IsOnlineApproved,
                IsOnlineTransferred = insurancePolicy.IsOnlineTransferred,
                IsOnlineTransferredBy = insurancePolicy.IsOnlineTransferredBy,
                IsVoid = insurancePolicy.IsVoid,
                OnlineApprovedBy = insurancePolicy.OnlineApprovedBy,
                OnlineApprovedTime = insurancePolicy.OnlineApprovedTime,
                OnlineTransferredTime = insurancePolicy.OnlineTransferredTime,
                SendForApprovalBy = insurancePolicy.SendForApprovalBy,
                VoidTime = insurancePolicy.VoidTime,
                OrgBranchName = insurancePolicy.OrgBranchName,
                VoidBy = insurancePolicy.VoidBy,
                DateCreated = insurancePolicy.DateCreated,
                CreatedBy = insurancePolicy.CreatedBy,
                ModifiedBy = insurancePolicy.ModifiedBy,
                LastModified = insurancePolicy.LastModified

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