using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceRequestApprover)] 
    public class EquipmentDemandController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        IVehicleDemandService _VehicleDemandService;
        ExceedDbContext db = new ExceedDbContext();
        //private readonly CacheData CacheData = new CacheData();

        public EquipmentDemandController()
        {
            _VehicleDemandService = new VehicleDemandService();
        }
        //
        // GET: /FleetManagement/VehicleDemand/
        public ActionResult Index()
        {
            //ViewData["Fleets"] = db.Fleets.ToList().Select(s => new
            //{
            //    Id = s.FleetsID,
            //    s.OrgBranchName,
            //    Name = s.EquipmentName + "-" + s.PlateNumber + "-" + s.ChasisNumber
            //}).Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));


            

            
            ViewData["Fleets"] = CacheData.GetFleetsCache().Select(s => new
            {
                Id = s.Id,
                Name = s.Name
            }).AsQueryable();


            
            ViewData["Employee"] = CacheData.GetEmployeeCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
       i => new
       {
           Name = i.Name,
           Id = i.Id
       }).AsQueryable();
            ViewData["EquipmentCategories"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Name = i.Name.Trim(),
          Id = i.Id.ToString()
      }).AsQueryable();
            ViewData["EquipmentTypes"] = CacheData.GetEquipmentTypeCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
            ViewData["EquipmentNames"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();

            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName + "-" + s.MobilePhone
            });
            
            return View();
        }


        //public ActionResult VehicleDemand_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    //List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
        //    List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

        //    var VehicleDemand = _VehicleDemandService.GetAll().Where(x => mybraches.Contains(x.BranchId) || x.BranchId == 0);
        //    if (GlobalHelper.IsMultipleBusinessUnit())
        //    {
        //        List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
        //        VehicleDemand = VehicleDemand.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
        //    }
        //    return Json(VehicleDemand.ToDataSourceResult(request));

        //}
        public ActionResult VehicleDemand_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            //List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            //var VehicleDemand = _VehicleDemandService.GetAll().Where(x => mybraches.Contains(x.BranchId) || x.BranchId == 0);
            //List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            //IQueryable<VehicleDemand> VehicleDemand = _VehicleDemandService.GetAll().AsQueryable();

            IQueryable<VehicleDemand> VehicleDemand = db.VehicleDemand.Where(x => mybraches.Contains(x.BranchId) || x.BranchId == 0).AsQueryable();
            if(id!=null)
            {
                VehicleDemand = VehicleDemand.Where(x => x.ServiceCenterJobId == id);
            }
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                VehicleDemand = VehicleDemand.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            //        var result = VehicleDemand.Join(db.Person_Employee,
            //      vd => vd.EmployeeId,
            //      emp => emp.EmployeeId,
            //      (vd, emp) => new { vd, emp })
            //.ToDataSourceResult(request, x => new
            //{
            //    BranchId = x.vd.BranchId,
            //    BusinessUnitId = x.vd.BusinessUnitId,
            //    IsOnlineApproved = x.vd.IsOnlineApproved,
            //    IsSendForApproval = x.vd.IsSendForApproval,
            //    IsSecondSendForApproval = x.vd.IsSecondSendForApproval,
            //    IsSecondOnlineApproved = x.vd.IsSecondOnlineApproved,
            //    IsVoid = x.vd.IsVoid,
            //    Num = x.vd.Num,
            //    FromStructureName = x.vd.FromStructureName,
            //    FromBranchName = x.vd.FromBranchName,
            //    phoneNumber = x.vd.phoneNumber,
            //    PlateNumber = x.vd.PlateNumber,
            //    ReasonForTravel = x.vd.ReasonForTravel,
            //    Remark = x.vd.Remark,
            //    RequestDate = x.vd.RequestDate,
            //    RequestingBranchName = x.vd.RequestingBranchName,
            //    RequestingStructureName = x.vd.RequestingStructureName,
            //    RequestNumber = x.vd.RequestNumber,
            //    RequestQuantity = x.vd.RequestQuantity,
            //    StartingDate = x.vd.StartingDate,
            //    Status = x.vd.Status,
            //    ToDate = x.vd.ToDate,
            //    VehicleDemandId = x.vd.VehicleDemandId,
            //    CurrentApprovalStep = x.vd.CurrentApprovalStep,
            //    IsDepartmentOnlineApproved = x.vd.IsDepartmentOnlineApproved,
            //    DepartmentOnlineApprovedBy = x.vd.DepartmentOnlineApprovedBy,
            //    IsDepartmentSendForApproval = x.vd.IsDepartmentSendForApproval,
            //    EmployeeName =
            //        (x.emp.FirstNameEnglish ?? "") + " " +
            //        (x.emp.MiddleNameEnglish ?? "") + " " +
            //        (x.emp.LastNameEnglish ?? ""),
            //    IsAssigned = x.vd.IsAssigned,
            //    OrgStructureId = x.vd.OrgStructureId
            //});
            var result = VehicleDemand
            .ToDataSourceResult(request, x => new
            {
                BranchId = x.BranchId,
                BusinessUnitId = x.BusinessUnitId,
                IsOnlineApproved = x.IsOnlineApproved,
                IsSendForApproval = x.IsSendForApproval,
                IsSecondSendForApproval = x.IsSecondSendForApproval,
                IsSecondOnlineApproved = x.IsSecondOnlineApproved,
                IsVoid = x.IsVoid,
                Num = x.Num,
                FromStructureName = x.FromStructureName,
                FromBranchName = x.FromBranchName,
                phoneNumber = x.phoneNumber,
                PlateNumber = x.PlateNumber,
                ReasonForTravel = x.ReasonForTravel,
                Remark = x.Remark,
                RequestDate = x.RequestDate,
                RequestingBranchName = x.RequestingBranchName,
                RequestingStructureName = x.RequestingStructureName,
                RequestNumber = x.RequestNumber,
                RequestQuantity = x.RequestQuantity,
                StartingDate = x.StartingDate,
                Status = x.Status,
                ToDate = x.ToDate,
                VehicleDemandId = x.VehicleDemandId,
                EmployeeId = x.EmployeeId,
                CurrentApprovalStep = x.CurrentApprovalStep,
                IsDepartmentOnlineApproved = x.IsDepartmentOnlineApproved,
                DepartmentOnlineApprovedBy = x.DepartmentOnlineApprovedBy,
                IsDepartmentSendForApproval = x.IsDepartmentSendForApproval,
                
                IsAssigned = x.IsAssigned,
                OrgStructureId = x.OrgStructureId
            });
            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Create([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand, string id)
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
           
            if (VehicleDemand.ReasonForTravel == null)
            {
                ModelState.AddModelError(string.Empty, "Place Added Reason for Travel");
            }
            if (VehicleDemand.StartingDate > VehicleDemand.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {
                int RequestNumber = 1;
                VehicleDemand vehicleDemand =db.VehicleDemand.OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                if (vehicleDemand != null)
                {
                    RequestNumber = int.Parse(vehicleDemand.RequestNumber.ToString()) + 1;
                }
                VehicleDemand.Status = MaintenanceConst.Pending;
                VehicleDemand.VehicleDemandId = Guid.NewGuid();
                VehicleDemand.RequestNumber = RequestNumber;
                if(id!=null)
                {
                    VehicleDemand.ServiceCenterJobId = id;
                }
                VehicleDemand.DateCreated = DateTime.Now;
                VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                VehicleDemand.CreatedBy = User.Identity.Name;
                //VehicleDemand.BranchId = 3;
                VehicleDemand.BranchId = GetBranch(VehicleDemand.EmployeeId).BranchId;
                //VehicleDemand.BusinessUnitId = 2;
                VehicleDemand.BusinessUnitId = GetCompStruct(VehicleDemand.EmployeeId).OrgStructureId;
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
        public ActionResult VehicleDemand_Update([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
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
                    VehicleDemand.ModifiedBy = User.Identity.Name;
                    VehicleDemand.LastModified = DateTime.Now;
                    VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                    _VehicleDemandService.Update(VehicleDemand);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Vehicle demand registration", "Success", User.Identity.Name, $"updated vehicle demand {VehicleDemand.VehicleDemandId}", Modules.FleetManagement);
                    #endregion
                }
            }
            return Json(new[] { VehicleDemand }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Destroy([DataSourceRequest] DataSourceRequest request, VehicleDemand vehicleDemand)
        {
            var VehicleDemand = _VehicleDemandService.GetById(vehicleDemand.VehicleDemandId);
            if (VehicleDemand != null)
            {
                _VehicleDemandService.Delete(VehicleDemand);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Demand " + vehicleDemand.VehicleDemandId);
            }

            return Json(new[] { VehicleDemand }.ToDataSourceResult(request));
        }


        public Branch GetBranch(int empId)
        {
           
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId);
            
            var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
            
            return branch;
        }
        public MainCompanyStructure GetCompStruct(int empId)
        {

            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId);

            var orgStruct = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);

            return orgStruct;
        }

        [AuthorizeRoles(FleetManagementRoles.RentRequestor)]
        public JsonResult VehicleDemandSendForOnlineApproval(string vehicleDemandId)
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
                vehicleDemand.IsSendForApproval = true;
                vehicleDemand.Status = MaintenanceConst.WaitingApproval;
               // db.VehicleDemand.Attach(vehicleDemand);
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

        public JsonResult EquipmentDemandReject(Guid id)
        {
            var equipmentDemand = db.VehicleDemand.FirstOrDefault(x => x.VehicleDemandId == id);
            equipmentDemand.IsOnlineApproved = false;
            equipmentDemand.IsDepartmentOnlineApproved = false;
            equipmentDemand.OnlineApprovedBy = " ";
            equipmentDemand.DepartmentOnlineApprovedBy = " ";
            equipmentDemand.Status = "Distribution Rejected";
            db.Entry(equipmentDemand).State = EntityState.Modified;
            db.SaveChanges();
            var approval = db.VehicleDemandApproval.FirstOrDefault(x => x.VehicleDemandId == id);
            db.VehicleDemandApproval.Remove(approval);
            db.SaveChanges();
            var formattedData = new
            {
                Message = "Vehicle Demand Rejected",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}