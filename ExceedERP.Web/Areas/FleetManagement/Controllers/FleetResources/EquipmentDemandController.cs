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
        IVehicleDemandService _VehicleDemandService;
        ExceedDbContext db = new ExceedDbContext();

        public EquipmentDemandController()
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
                Name = s.FirstNameEnglish+" "+s.MiddleNameEnglish+" "+s.LastNameEnglish
            });

            ViewData["EquipmentName"] = db.EquipmentName.Select(s => new
            {
                Id = s.EquipmentNameID,
                Name = s.Name
            });
            return View();
        }


        public ActionResult VehicleDemand_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> userworkunit = UserInformationHelper.GetUserHeadWorkUnitAndChildWorkUnits(User.Identity.Name);
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
                
            var VehicleDemand = _VehicleDemandService.GetAll().Where(x => mybraches.Contains(x.BranchId) || x.BranchId==0);
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                List<int> myBusinessUnits = UserInformationHelper.GetUserBusinessUnits(User.Identity.Name);
                VehicleDemand = VehicleDemand.Where(x => myBusinessUnits.Contains(x.BusinessUnitId));
            }
            return Json(VehicleDemand.ToDataSourceResult(request));
            
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDemand_Create([DataSourceRequest] DataSourceRequest request, VehicleDemand VehicleDemand)
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
                VehicleDemand vehicleDemand = _VehicleDemandService.GetAll().OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                if (vehicleDemand != null)
                {
                    RequestNumber = int.Parse(vehicleDemand.RequestNumber.ToString()) + 1;
                }
                VehicleDemand.Status = MaintenanceConst.Pending;
                VehicleDemand.VehicleDemandId = Guid.NewGuid();
                VehicleDemand.RequestNumber = RequestNumber;

                VehicleDemand.DateCreated = DateTime.Now;
                VehicleDemand.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                VehicleDemand.CreatedBy = User.Identity.Name;
                VehicleDemand.BranchId = GetBranch(VehicleDemand.EmployeeId).BranchId;
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
    }
}