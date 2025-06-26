using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Maintenance;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class EquipmentDemandSelfCareController : Controller
    {
        IVehicleDemandService _VehicleDemandService;
        ExceedDbContext db = new ExceedDbContext();
        public EquipmentDemandSelfCareController()
        {
            _VehicleDemandService = new VehicleDemandService();
        }
        //
        // GET: /FleetManagement/VehicleDemand/
        public ActionResult Index()
        {
            ViewData["Fleets"] = db.Fleets.ToList().Select(s => new {
                Id = s.FleetsID,
                s.OrgBranchName,
                Name = s.EquipmentName + "-" + s.PlateNumber + "-" + s.ChasisNumber
            }).Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

            ViewData["EquipmentCategories"] = db.EquipmentIdentity.ToList().Select(s => new {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            });
            ViewData["EquipmentTypes"] = db.EquipmentType.ToList().Select(s => new {
                Id = s.EquipmentTypeID,
                Name = s.EquipmentTypeName
            });
            ViewData["EquipmentNames"] = db.EquipmentName.ToList().Select(s => new {
                Id = s.EquipmentNameID,
                Name = s.Name
            });
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName + "-" + s.MobilePhone
            });
            ViewData["FromBranches"] = db.Branch.ToList().Select(s => new {
                Id = s.BranchId,
                Name = s.Name
            });
            ViewData["FromStructure"] = db.MainCompanyStructures.ToList().Select(s => new {
                Id = s.OrgStructureId,
                Name = s.Name
            });
            ViewData["Employee"] = db.Person_Employee.ToList().Select(s => new {
                Id = s.EmployeeId,
                Name = s.FirstNameEnglish + " " + s.MiddleNameEnglish + " " + s.LastNameEnglish
            });
            ViewData["EquipmentName"] = db.EquipmentName.Select(s => new
            {
                Id = s.EquipmentNameID,
                Name = s.Name
            });
            return View();
        }
        public ActionResult VehicleDemandSelfCare_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            IQueryable<VehicleDemand> VehicleDemand = _VehicleDemandService.GetAll().AsQueryable();
            if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService)
                 && System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HRDepartmentHead))
            {

                VehicleDemand = VehicleDemand.Where(x => userworkunit.Contains(x.OrgStructureId)
                || x.ForwardedTo == User.Identity.Name
                || x.OrgStructureId == 0
                || x.ForwardedFrom == User.Identity.Name
                || x.CreatedBy == User.Identity.Name);

            }
            else if (System.Web.HttpContext.Current.User.IsInRole(HrmConstants.HREmployeeSelfService))
            {
                VehicleDemand = VehicleDemand.Where(x => x.CreatedBy == User.Identity.Name);
            }
            return Json(VehicleDemand.ToDataSourceResult(request));


        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemandSelfCare_Create([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
        {
            
            #region data 
            if (VehicleDemand.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (VehicleDemand.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == VehicleDemand.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    VehicleDemand.OrgStructureId = placement.OrgStructureId;
                    VehicleDemand.BranchId = placement.BranchId;
                    VehicleDemand.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            //VehicleDemand.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            //VehicleDemand.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion
            ModelState.Remove("VehicleDemandId");
            if (VehicleDemand.StartingDate > VehicleDemand.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {
                int RequestNumber = 1;
                VehicleDemand vehicleDemand = _VehicleDemandService.GetAll().OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                if (vehicleDemand != null)
                {
                    RequestNumber = int.Parse(vehicleDemand.RequestNumber.ToString()) + 1;
                }
                VehicleDemand.Status = MaintenanceConst.Pending;
                VehicleDemand.VehicleDemandId = Guid.NewGuid();
                VehicleDemand.RequestNumber = RequestNumber;
                VehicleDemand.DateCreated = DateTime.Now;
                VehicleDemand.ForwardedFrom = User.Identity.Name;
                VehicleDemand.ForwardedTo = User.Identity.Name;
                VehicleDemand.IsFinalApproval = VehicleDemand.IsFinalApproval;
                VehicleDemand.CurrentApprovalStep = VehicleDemand.CurrentApprovalStep;
                VehicleDemand.IsDepartmentOnlineApproved = VehicleDemand.IsDepartmentOnlineApproved;
                VehicleDemand.DepartmentOnlineApprovedBy = VehicleDemand.DepartmentOnlineApprovedBy;
                VehicleDemand.DepartmentOnlineApprovedTime = VehicleDemand.DepartmentOnlineApprovedTime;
                VehicleDemand.IsDepartmentSendForApproval = VehicleDemand.IsDepartmentSendForApproval;
                VehicleDemand.DepartmentSendForApprovalBy = VehicleDemand.DepartmentSendForApprovalBy;
                VehicleDemand.DepartmentSendForApprovalTime = VehicleDemand.DepartmentSendForApprovalTime;
                VehicleDemand.IsOnlineApproved = VehicleDemand.IsOnlineApproved;
                VehicleDemand.OnlineApprovedBy = VehicleDemand.OnlineApprovedBy;
                VehicleDemand.OnlineApprovedTime = VehicleDemand.OnlineApprovedTime;
                VehicleDemand.IsVoid = VehicleDemand.IsVoid;
                VehicleDemand.VoidBy = VehicleDemand.VoidBy;
                VehicleDemand.VoidTime = VehicleDemand.VoidTime;
                VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                VehicleDemand.CreatedBy = User.Identity.Name;
                
                if (VehicleDemand != null)
                {
                    _VehicleDemandService.Insert(VehicleDemand);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Equipment demand registration", "Success", User.Identity.Name, $"created equipment demand {VehicleDemand.VehicleDemandId}", Modules.FleetManagement);
                    #endregion
                }
            }
            return Json(new[] { VehicleDemand }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemandSelfCare_Update([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
        {
            #region data 
            if (VehicleDemand.EmployeeId == 0)
            {
                ModelState.AddModelError("requester", "Please select requestor");
            }
            if (VehicleDemand.EmployeeId != 0)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == VehicleDemand.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    VehicleDemand.OrgStructureId = placement.OrgStructureId;
                    VehicleDemand.BranchId = placement.BranchId;
                    VehicleDemand.BusinessUnitId = placement.BusinessUnitId;
                }
                else
                {
                    ModelState.AddModelError("placement", "Employee has not current placemnt");
                }
            }

            //VehicleDemand.CreatedAtBranchId = UserInformationHelper.GetUserDefaultBranchId(User.Identity.Name);
            //VehicleDemand.CreatedAtBusinessUnitId = UserInformationHelper.GetUserDefaultBusinessUnitId(User.Identity.Name);
            #endregion
            if (VehicleDemand.StartingDate > VehicleDemand.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {
                if (VehicleDemand != null)
                {
                    VehicleDemand.ForwardedFrom = User.Identity.Name;
                    VehicleDemand.ForwardedTo = User.Identity.Name;
                    VehicleDemand.IsFinalApproval = VehicleDemand.IsFinalApproval;
                    VehicleDemand.CurrentApprovalStep = VehicleDemand.CurrentApprovalStep;
                    VehicleDemand.IsDepartmentOnlineApproved = VehicleDemand.IsDepartmentOnlineApproved;
                    VehicleDemand.DepartmentOnlineApprovedBy = VehicleDemand.DepartmentOnlineApprovedBy;
                    VehicleDemand.DepartmentOnlineApprovedTime = VehicleDemand.DepartmentOnlineApprovedTime;
                    VehicleDemand.IsDepartmentSendForApproval = VehicleDemand.IsDepartmentSendForApproval;
                    VehicleDemand.DepartmentSendForApprovalBy = VehicleDemand.DepartmentSendForApprovalBy;
                    VehicleDemand.DepartmentSendForApprovalTime = VehicleDemand.DepartmentSendForApprovalTime;
                    VehicleDemand.IsOnlineApproved = VehicleDemand.IsOnlineApproved;
                    VehicleDemand.OnlineApprovedBy = VehicleDemand.OnlineApprovedBy;
                    VehicleDemand.OnlineApprovedTime = VehicleDemand.OnlineApprovedTime;
                    VehicleDemand.IsVoid = VehicleDemand.IsVoid;
                    VehicleDemand.VoidBy = VehicleDemand.VoidBy;
                    VehicleDemand.ModifiedBy = User.Identity.Name;
                    VehicleDemand.LastModified = DateTime.Now;
                    VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    VehicleDemand.BusinessUnitId = db.MainCompanyStructures.FirstOrDefault(x => x.OrgBranchName == VehicleDemand.FromStructureName).OrgStructureId;
                    _VehicleDemandService.Update(VehicleDemand);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Vehicle demand registration", "Success", User.Identity.Name, $"updated vehicle demand {VehicleDemand.VehicleDemandId}", Modules.FleetManagement);
                    #endregion
                }
            }
            return Json(new[] { VehicleDemand }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemandSelfCare_Destroy([DataSourceRequest] DataSourceRequest request, VehicleDemand vehicleDemand)
        {
            var VehicleDemand = _VehicleDemandService.GetById(vehicleDemand.VehicleDemandId);
            if (VehicleDemand != null)
            {
                _VehicleDemandService.Delete(VehicleDemand);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Demand " + vehicleDemand.VehicleDemandId);
            }

            return Json(new[] { VehicleDemand }.ToDataSourceResult(request));
        }


        public JsonResult VehicleDemand_SendForOnlineApproval(string vehicleDemandId)
        {

            Guid.TryParse(vehicleDemandId, out Guid id);
            var vehicleDemand = db.VehicleDemand.Where(x => x.VehicleDemandId == id).FirstOrDefault();
            var requestEquipments = db.RequestedEquipments.Where(x => x.RequestNumber == vehicleDemand.VehicleDemandId);
            if (!requestEquipments.Any())
            {
                var returnJsonData = new
                {
                    Message = "Please Add Request",
                };
                return Json(returnJsonData.Message, JsonRequestBehavior.AllowGet);
            }
            if (vehicleDemand != null)
            {
                vehicleDemand.IsDepartmentSendForApproval = true;
                vehicleDemand.Status = MaintenanceConst.WaitingApproval;
                db.VehicleDemand.Attach(vehicleDemand);
                db.Entry(vehicleDemand).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);
            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }

        public ActionResult VehicleDemandApprovalSelfCare_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            IQueryable<VehicleDemandApproval> fleetApproval = db.VehicleDemandApproval.Where(x => x.VehicleDemandId == Id);

            var dsResult = fleetApproval.ToDataSourceResult(request, model => new VehicleDemandApproval
            {
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                OrgBranchName = model.OrgBranchName,
                Remark = model.Remark,
                Status = model.Status,
                VehicleDemand = model.VehicleDemand,
                VehicleDemandApprovalID = model.VehicleDemandApprovalID,
                VehicleDemandId = model.VehicleDemandId
            });
            return Json(dsResult);
                
        }


        [AuthorizeRoles(HrmConstants.HRDepartmentHead)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemandSelfCareApproval([DataSourceRequest]DataSourceRequest request, VehicleDemandApproval VehicleDemandApproval, string EId)
        {

            ModelState.Remove("VehicleDemandId");
            Guid Id = Guid.Parse(EId);

            var record = db.VehicleDemand.Find(Id);
            if (record == null)
                ModelState.AddModelError("No record", "No Record Found");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            if (record.IsDepartmentOnlineApproved)
                ModelState.AddModelError("already Approved", "record already Approved");

            if (ModelState.IsValid)
            {
                if (VehicleDemandApproval.Status == MaintenanceConst.Approved)
                {
                   // var entity = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
                     
                    record.IsDepartmentOnlineApproved = true;
                    record.DepartmentOnlineApprovedBy = User.Identity.Name;
                    record.Status = MaintenanceConst.Approved;
                    record.DepartmentOnlineApprovedTime = DateTime.Now;
                    record.Status = MaintenanceConst.Approved;

                    db.VehicleDemand.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                if (VehicleDemandApproval.Status == MaintenanceConst.Rejected)
                {
                    //var entity = db.VehicleDemand.Where(x => x.VehicleDemandId == Id).FirstOrDefault();
                    record.IsDepartmentSendForApproval = false;
                    record.Status = MaintenanceConst.Rejected;
                    db.VehicleDemand.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();

                }
                var approval = new VehicleDemandApproval()
                {

                    Status = VehicleDemandApproval.Status,
                    VehicleDemandId = Id,
                    Remark = VehicleDemandApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.VehicleDemandApproval.Add(approval);
                db.SaveChanges();
                VehicleDemandApproval.VehicleDemandApprovalID = approval.VehicleDemandApprovalID;
            }


            return Json(new[] { VehicleDemandApproval }.ToDataSourceResult(request));
        }
}
        }
    