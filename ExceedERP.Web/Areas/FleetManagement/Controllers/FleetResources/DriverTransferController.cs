using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
    public class DriverTransferController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IDriverVehicleTransferService _DriverVehicleTransferService;
        IFleetService _FleetService;
        public DriverTransferController()
        {
            _FleetService = new FleetService();
            _DriverVehicleTransferService = new DriverVehicleTransferService();
        }
        //
        // GET: /FleetManagement/DriverVehicleTransfer/
        public ActionResult Index()
        {            
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.EmployeeId+"_"+s.FullName
            });
            return View();
        }

        public JsonResult Seed()
        {
            var dvt = db.DriverVehicleTransfer.ToList();
            foreach (var item in dvt)
            {
                var fleet = db.Fleets.Find(item.FleetId);
                if(fleet != null)
                {
                    item.EquipmentCategoryId = fleet?.EquipmentIdentityID ?? Guid.Empty;
                    item.EquipmentTypeId = fleet?.EquipmentTypeID ?? Guid.Empty;
                    item.EquipmentNameId = fleet?.EquipmentNameID ?? Guid.Empty;
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DriverVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var DriverVehicleTransfer = _DriverVehicleTransferService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            return Json(DriverVehicleTransfer.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer DriverVehicleTransfer)
        {

            ExceedDbContext db = new ExceedDbContext();
            ModelState.Remove("DriverVehicleTransferID");
            ModelState.Remove("FromEquipment");
            ModelState.Remove("ToEquipment");
            if (DriverVehicleTransfer.FromOperatorId == DriverVehicleTransfer.ToOperatorId)
            {
                ModelState.Clear();
                ModelState.AddModelError("", " Driver must be Diffrent ! ");
            }
            else if (DriverVehicleTransfer.StartingDate > DriverVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date !");
            }
            else
            {
                var FetchFromOperator=
                    _FleetService.GetAll()
                        .Where(x => x.OperatorId.ToString() == DriverVehicleTransfer.FromOperatorId)
                        .FirstOrDefault();
                var FromFleet = db.DriverAssignments.FirstOrDefault(x => x.FleetId == DriverVehicleTransfer.FleetId);
                if(FetchFromOperator!=null)
                {

                }

                

                var ToFleet = db.DriverAssignments.SingleOrDefault(foid => foid.OperatorId.ToString() == DriverVehicleTransfer.ToOperatorId);
              

                if (FromFleet == null)

                {
                    ModelState.AddModelError("From fleet is null", "Transfer from Operator is not Registered");
                }
                if (ToFleet == null)
                {
                    ModelState.AddModelError("To fleet is null", "Transfer to Operator is not Registered");
                }
                if ((FromFleet != null) && (ToFleet != null))
                {
                    if (DriverVehicleTransfer != null)
                    {
                        DriverVehicleTransfer.DriverVehicleTransferID = Guid.NewGuid();
                        if (FromFleet != null)
                        {
                            DriverVehicleTransfer.FromEquipment = FromFleet.FleetId;
                        }

                        if (ToFleet!=null)
                        {
                            DriverVehicleTransfer.ToEquipment = ToFleet.FleetId;
                        }
                        
                        DriverVehicleTransfer.FromOperatorId =FromFleet.OperatorId.ToString();
                        //DriverVehicleTransfer.FromEquipment = FromFleet.FleetId;
                        DriverVehicleTransfer.FromEquipment = FromFleet.FleetId;
                        DriverVehicleTransfer.ToEquipment = ToFleet.FleetId;
                        DriverVehicleTransfer.DateCreated = DateTime.Now;
                        DriverVehicleTransfer.CreatedBy = User.Identity.Name;
                        DriverVehicleTransfer.Status = MaintenanceConst.Pending;
                        DriverVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        _DriverVehicleTransferService.Insert(DriverVehicleTransfer);
                        var EquipmentTyres = db.EquipmentTyres.Where(x => (x.fleetId == DriverVehicleTransfer.FromEquipment || x.fleetId == DriverVehicleTransfer.ToEquipment) && x.Isattached).ToList();

                        foreach (var EquipmentTyre in EquipmentTyres)
                        {

                            var TyreDetail = new SiteVechicleTransferTyreDetail()
                            {
                                SiteVechicleTransferId = DriverVehicleTransfer.DriverVehicleTransferID,
                                SerialNo = EquipmentTyre.SerialNo,
                                Type = EquipmentTyre.Type,
                                Description = EquipmentTyre.Make + " " + EquipmentTyre.Condition,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now

                            };
                            db.SiteVechicleTransferTyreDetailes.Add(TyreDetail);
                            db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Operator transfer", "Success", User.Identity.Name, $"created driver transfer   {TyreDetail.SiteVechicleTransferTyreDetailId}", Modules.FleetManagement);
                            #endregion

                        }
                    }
                }
            }

                return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request, ModelState));
            
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer DriverVehicleTransfer)
        {
            if (DriverVehicleTransfer.StartingDate > DriverVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else
            {

                if (DriverVehicleTransfer != null)
                {
                    DriverVehicleTransfer.DateCreated = DateTime.Now;
                    DriverVehicleTransfer.CreatedBy = User.Identity.Name;
                    DriverVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                    _DriverVehicleTransferService.Update(DriverVehicleTransfer);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Operator transfer", "Success", User.Identity.Name, $"updated driver transfer   {DriverVehicleTransfer.DriverVehicleTransferID}", Modules.FleetManagement);
                    #endregion
                }

            }
            return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Destroy([DataSourceRequest] DataSourceRequest request, DriverVehicleTransfer driverVehicleTransfer)
        {
            var DriverVehicleTransfer = _DriverVehicleTransferService.GetById(driverVehicleTransfer.DriverVehicleTransferID);
            
            if (DriverVehicleTransfer != null)
            {
                _DriverVehicleTransferService.Delete(DriverVehicleTransfer);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Diver Transfer" + DriverVehicleTransfer.DriverVehicleTransferID);

            }

            return Json(new[] { DriverVehicleTransfer }.ToDataSourceResult(request));
        }



    }
}