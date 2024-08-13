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
    public class EquipmentAccidentClaimsController : BaseController
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
        
        public ActionResult EquipmentAccidentClaims_Read([DataSourceRequest] DataSourceRequest request)
        {
            var insurancePolicies = db.EquipmentAccidentClaims.Where(x => x.IsDepartmentOnlineApproved);
            
            DataSourceResult result = insurancePolicies.ToDataSourceResult(request, accidentClaim => new
            {

                EquipmentAccidentClaimId = accidentClaim.EquipmentAccidentClaimId,
                RequestRecorderEmployeeId = accidentClaim.RequestRecorderEmployeeId,
                //BranchId = accidentClaim.BranchId,
                //BusinessUnitId = accidentClaim.BusinessUnitId,
                //OrgStructureId = accidentClaim.OrgStructureId,
                //EmployementType = accidentClaim.EmployementType,
                //ResponsibilityLevel = accidentClaim.ResponsibilityLevel,
                CollidedAddress = accidentClaim.CollidedAddress,
                AccidentID = accidentClaim.AccidentID,
                FleetsID = accidentClaim.FleetsID,
                AccidentDetail = accidentClaim.AccidentDetail,
                AccidentDate = accidentClaim.AccidentDate,
                AccidentDateAm = accidentClaim.AccidentDateAm,
                AccidentTime = accidentClaim.AccidentTime,
                CollidedOwnerName = accidentClaim.CollidedOwnerName,
                CollidedPhone = accidentClaim.CollidedPhone,
                CollidedPlateNo = accidentClaim.CollidedPlateNo,
                DriverName = accidentClaim.DriverName,
                PlateNumber = accidentClaim.PlateNumber,
                InsuranceBranch = accidentClaim.InsuranceBranch,
                MachineOperatorName = accidentClaim.MachineOperatorName,
                IsCollissionWithAnotherVehicle = accidentClaim.IsCollissionWithAnotherVehicle,
                Description = accidentClaim.Description,
                IsVerified = accidentClaim.IsVerified,
                IsPaid = accidentClaim.IsPaid,
                IsTheDocumentComplete = accidentClaim.IsTheDocumentComplete,
                Machine = accidentClaim.Machine,
                Vehicle = accidentClaim.Vehicle,
                PaymentAmount = accidentClaim.PaymentAmount,
                IsTheDocumentCompleteByEIC = accidentClaim.IsTheDocumentCompleteByEIC,
                Status = accidentClaim.Status,
                Remark = accidentClaim.Remark,
                CurrentApprovalStep = accidentClaim.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = accidentClaim.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = accidentClaim.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = accidentClaim.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = accidentClaim.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = accidentClaim.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = accidentClaim.IsDepartmentSendForApproval,
                ForwardedFrom = accidentClaim.ForwardedFrom,
                ForwardedTo = accidentClaim.ForwardedTo,
                IsFinalApproval = accidentClaim.IsFinalApproval,
                IsSendForApproval = accidentClaim.IsSendForApproval,
                SendForApprovalTime = accidentClaim.SendForApprovalTime,
                IsOnlineApproved = accidentClaim.IsOnlineApproved,
                IsOnlineTransferred = accidentClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = accidentClaim.IsOnlineTransferredBy,
                IsVoid = accidentClaim.IsVoid,
                OnlineApprovedBy = accidentClaim.OnlineApprovedBy,
                OnlineApprovedTime = accidentClaim.OnlineApprovedTime,
                OnlineTransferredTime = accidentClaim.OnlineTransferredTime,
                SendForApprovalBy = accidentClaim.SendForApprovalBy,
                VoidTime = accidentClaim.VoidTime,
                OrgBranchName = accidentClaim.OrgBranchName,
                VoidBy = accidentClaim.VoidBy,
                DateCreated = accidentClaim.DateCreated,
                CreatedBy = accidentClaim.CreatedBy,
                ModifiedBy = accidentClaim.ModifiedBy,
                LastModified = accidentClaim.LastModified
            });
            return Json(result);
        }
        public ActionResult EquipmentAccidentClaims_Create([DataSourceRequest] DataSourceRequest request, EquipmentAccidentClaim accidentClaim)
        {


            #region data validation
            if (!string.IsNullOrEmpty(accidentClaim.AccidentDateAm))
            {
           
                var splitDate = accidentClaim.AccidentDateAm.Split('/');
                accidentClaim.AccidentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (accidentClaim.RequestRecorderEmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }

            //if (accidentClaim.EmployeeId != 0)
            //{
            //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == accidentClaim.EmployeeId && x.isCurrent);
            //    if (placement != null)
            //    {
            //        accidentClaim.OrgStructureId = placement.OrgStructureId;
            //        accidentClaim.BranchId = placement.BranchId;
            //        accidentClaim.BusinessUnitId = placement.BusinessUnitId;
            //        accidentClaim.ResponsibilityLevel = placement.ResponsibilityLevel;
            //        accidentClaim.EmployementType = placement.EmployementType;
            //    }
            //    else
            //    {
            //        ModelState.AddModelError("placement", "Employee has not current placemnt");
            //    }
            //}
               
            #endregion
            if (ModelState.IsValid)
            {
                accidentClaim.CreatedBy = User.Identity.Name;
                accidentClaim.DateCreated = DateTime.Now;
                var entity = GetMappedObject(accidentClaim);
                db.EquipmentAccidentClaims.Add(entity);
                db.SaveChanges();
                accidentClaim.EquipmentAccidentClaimId = entity.EquipmentAccidentClaimId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Equipment Accident Claim  ", "Success", User.Identity.Name, $"created Equipment Accident Claim {entity.EquipmentAccidentClaimId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { accidentClaim }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentAccidentClaims_Update([DataSourceRequest] DataSourceRequest request, EquipmentAccidentClaim accidentClaim)
        {

            #region data Validation
            if (!string.IsNullOrEmpty(accidentClaim.AccidentDateAm))
            {

                var splitDate = accidentClaim.AccidentDateAm.Split('/');
                accidentClaim.AccidentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }
            else
            {
                ModelState.AddModelError("", "Date is required");
            }
            if (accidentClaim.RequestRecorderEmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select Employee");
            }
            //if (accidentClaim.EmployeeId != 0)
            //{
            //    var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == accidentClaim.EmployeeId && x.isCurrent);
            //    if (placement != null)
            //    {
            //        accidentClaim.OrgStructureId = placement.OrgStructureId;
            //        accidentClaim.BranchId = placement.BranchId;
            //        accidentClaim.BusinessUnitId = placement.BusinessUnitId;
            //        accidentClaim.ResponsibilityLevel = placement.ResponsibilityLevel;
            //        accidentClaim.EmployementType = placement.EmployementType;


            //    }
            //    else
            //    {
            //            ModelState.AddModelError("placement", "Employee has not current placemnt");

            //    }

            //}

            #endregion
            if (ModelState.IsValid)
            {
                accidentClaim.ModifiedBy = User.Identity.Name;
                accidentClaim.LastModified = DateTime.Now;
                var entity = GetMappedObject(accidentClaim);
                db.EquipmentAccidentClaims.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Equipment Accident Claim update", "Success", User.Identity.Name, $"updated Equipment Accident Claim {entity.EquipmentAccidentClaimId} ", Modules.Insurance);
                #endregion
            }
            return Json(new[] { accidentClaim }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EquipmentAccidentClaims_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentAccidentClaim accidentClaim)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(accidentClaim);
                db.EquipmentAccidentClaims.Attach(entity);
                db.EquipmentAccidentClaims.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Equipment Accident Claim {entity.EquipmentAccidentClaimId }");
                #endregion
            }

            return Json(new[] { accidentClaim }.ToDataSourceResult(request, ModelState));
        }
        private EquipmentAccidentClaim GetMappedObject(EquipmentAccidentClaim accidentClaim)
        {
            var entity = new EquipmentAccidentClaim
            {
                EquipmentAccidentClaimId = accidentClaim.EquipmentAccidentClaimId,
                RequestRecorderEmployeeId = accidentClaim.RequestRecorderEmployeeId,
                //BranchId = accidentClaim.BranchId,
                //BusinessUnitId = accidentClaim.BusinessUnitId,
                //OrgStructureId = accidentClaim.OrgStructureId,
                //EmployementType = accidentClaim.EmployementType,
                //ResponsibilityLevel = accidentClaim.ResponsibilityLevel,
                CollidedAddress = accidentClaim.CollidedAddress,
                AccidentID = accidentClaim.AccidentID,
                FleetsID = accidentClaim.FleetsID,
                AccidentDetail = accidentClaim.AccidentDetail,
                AccidentDate = accidentClaim.AccidentDate,
                AccidentDateAm = accidentClaim.AccidentDateAm,
                AccidentTime = accidentClaim.AccidentTime,
                CollidedOwnerName = accidentClaim.CollidedOwnerName,
                CollidedPhone = accidentClaim.CollidedPhone,
                CollidedPlateNo = accidentClaim.CollidedPlateNo,
                DriverName = accidentClaim.DriverName,
                PlateNumber = accidentClaim.PlateNumber,
                InsuranceBranch = accidentClaim.InsuranceBranch,
                MachineOperatorName = accidentClaim.MachineOperatorName,
                IsCollissionWithAnotherVehicle = accidentClaim.IsCollissionWithAnotherVehicle,
                Description = accidentClaim.Description,
                IsVerified = accidentClaim.IsVerified,
                IsPaid = accidentClaim.IsPaid,
                IsTheDocumentComplete = accidentClaim.IsTheDocumentComplete,
                Machine = accidentClaim.Machine,
                Vehicle = accidentClaim.Vehicle,
                PaymentAmount = accidentClaim.PaymentAmount,
                IsTheDocumentCompleteByEIC = accidentClaim.IsTheDocumentCompleteByEIC,
                Status = accidentClaim.Status,
                Remark = accidentClaim.Remark,
                CurrentApprovalStep = accidentClaim.CurrentApprovalStep,
                DepartmentOnlineApprovedBy = accidentClaim.DepartmentOnlineApprovedBy,
                DepartmentOnlineApprovedTime = accidentClaim.DepartmentOnlineApprovedTime,
                IsDepartmentOnlineApproved = accidentClaim.IsDepartmentOnlineApproved,
                DepartmentSendForApprovalBy = accidentClaim.DepartmentSendForApprovalBy,
                DepartmentSendForApprovalTime = accidentClaim.DepartmentSendForApprovalTime,
                IsDepartmentSendForApproval = accidentClaim.IsDepartmentSendForApproval,
                ForwardedFrom = accidentClaim.ForwardedFrom,
                ForwardedTo = accidentClaim.ForwardedTo,
                IsFinalApproval = accidentClaim.IsFinalApproval,
                IsSendForApproval = accidentClaim.IsSendForApproval,
                SendForApprovalTime = accidentClaim.SendForApprovalTime,
                IsOnlineApproved = accidentClaim.IsOnlineApproved,
                IsOnlineTransferred = accidentClaim.IsOnlineTransferred,
                IsOnlineTransferredBy = accidentClaim.IsOnlineTransferredBy,
                IsVoid = accidentClaim.IsVoid,
                OnlineApprovedBy = accidentClaim.OnlineApprovedBy,
                OnlineApprovedTime = accidentClaim.OnlineApprovedTime,
                OnlineTransferredTime = accidentClaim.OnlineTransferredTime,
                SendForApprovalBy = accidentClaim.SendForApprovalBy,
                VoidTime = accidentClaim.VoidTime,
                OrgBranchName = accidentClaim.OrgBranchName,
                VoidBy = accidentClaim.VoidBy,
                DateCreated = accidentClaim.DateCreated,
                CreatedBy = accidentClaim.CreatedBy,
                ModifiedBy = accidentClaim.ModifiedBy,
                LastModified = accidentClaim.LastModified

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