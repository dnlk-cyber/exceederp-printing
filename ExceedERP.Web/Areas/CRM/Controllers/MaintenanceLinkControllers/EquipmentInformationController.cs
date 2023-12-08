using ExceedERP.Core.Domain.CRM;

using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    //[AuthorizeRoles(FleetManagementRoles.MaintenanceEquipmentRegistrationUser, FleetManagementRoles.MileageImportAndExport)]
    public class EquipmentInformationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers.ToList();
            return View();
        }

        public ActionResult Equipments_Read([DataSourceRequest]DataSourceRequest request, int customerId)
        {
            IQueryable<EquipmentInformation> equipments = db.EquipmentInformation.Where(c => c.CustomerId == customerId);
            DataSourceResult result = equipments.ToDataSourceResult(request, equipment => new
            {
                EquipmentInformationId = equipment.EquipmentInformationId,
                CustomerId = equipment.CustomerId,
                GroupName = equipment.GroupName,
                LicensePlate = equipment.LicensePlate,
                DeviceNumber = equipment.DeviceNumber,
                InitialOdometer = equipment.InitialOdometer,
                OdometerOnLastMaintenance = equipment.OdometerOnLastMaintenance,
                CurrentOdometer = equipment.CurrentOdometer,
                FutureMaintenanceAlert = equipment.FutureMaintenanceAlert,

                RegularMaintenanceInterval = equipment.RegularMaintenanceInterval,
                NotifyUpComingMaintenanceBefore = equipment.NotifyUpComingMaintenanceBefore,
                OdometerOnNextRegularMaintenance = equipment.OdometerOnNextRegularMaintenance,
                RemainingKmForUpComingMaintenance = equipment.RemainingKmForUpComingMaintenance,

                LastMaintained = equipment.LastMaintained,
                Status = equipment.Status,

                DateCreated = equipment.DateCreated,
                LastModified = equipment.LastModified,
                CreatedBy = equipment.CreatedBy,
                ModifiedBy = equipment.ModifiedBy
            });

            return Json(result);
        }
        //public ActionResult AlertedEquipments_Read([DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<EquipmentInformation> equipments = db.EquipmentInformation.Where(e => e.FutureMaintenanceAlert == FutureMaintenanceAlert.On);
        //    DataSourceResult result = equipments.ToDataSourceResult(request, equipment => new
        //    {
        //        EquipmentInformationId = equipment.EquipmentInformationId,
        //        CustomerId = equipment.CustomerId,
        //        GroupName = equipment.GroupName,
        //        LicensePlate = equipment.LicensePlate,
        //        DeviceNumber = equipment.DeviceNumber,
        //        InitialOdometer = equipment.InitialOdometer,
        //        OdometerOnLastMaintenance = equipment.OdometerOnLastMaintenance,
        //        CurrentOdometer = equipment.CurrentOdometer,
        //        FutureMaintenanceAlert = equipment.FutureMaintenanceAlert,

        //        RegularMaintenanceInterval = equipment.RegularMaintenanceInterval,
        //        NotifyUpComingMaintenanceBefore = equipment.NotifyUpComingMaintenanceBefore,
        //        OdometerOnNextRegularMaintenance = equipment.OdometerOnNextRegularMaintenance,
        //        RemainingKmForUpComingMaintenance = equipment.RemainingKmForUpComingMaintenance,

        //        LastMaintained = equipment.LastMaintained,
        //        Status = equipment.Status,

        //        DateCreated = equipment.DateCreated,
        //        LastModified = equipment.LastModified,
        //        CreatedBy = equipment.CreatedBy,
        //        ModifiedBy = equipment.ModifiedBy
        //    });

        //    return Json(result);
        //}
        public ActionResult EquipmentAlert_Read([DataSourceRequest]DataSourceRequest request, string licensePlate)
        {
            List<EquipmentMaintenanceAlertViewModel> listToReturn = new List<EquipmentMaintenanceAlertViewModel>();
            var currentEquipment = db.EquipmentInformation.FirstOrDefault(e => e.LicensePlate == licensePlate);
            if (currentEquipment != null)
            {
                //var licensePlateRecord = new EquipmentMaintenanceAlertViewModel
                //{
                //    Attribute = "License Plate",
                //    Value = currentEquipment.LicensePlate
                //};
                var initialOdometerRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Initial Odometer ",
                    Value = currentEquipment.InitialOdometer.ToString("N2") + " km"
                };
                var currentOdometerRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Current Odometer ",
                    Value = currentEquipment.CurrentOdometer.ToString("N2") + " km"
                };
                var lastMaintenanceOdometerRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Odometer on Last Maintenance ",
                    Value = currentEquipment.OdometerOnLastMaintenance.ToString("N2") + " km"
                };
                var maintenanceIntervalRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Maintenance Interval ",
                    Value = currentEquipment.RegularMaintenanceInterval.ToString("N2") + " km"
                };
                var notifyMaintenanceRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Notify UpComing Maintenance Before",
                    Value = currentEquipment.NotifyUpComingMaintenanceBefore.ToString("N2") + " km"
                };
                var remainingForNextMaintenanceRecord = new EquipmentMaintenanceAlertViewModel
                {
                    Attribute = "Remaining For UpComing Maintenance",
                    Value = currentEquipment.RemainingKmForUpComingMaintenance <= 0 ? "0 km" : currentEquipment.RemainingKmForUpComingMaintenance.ToString("N2") + " km"
                };
                listToReturn.Add(initialOdometerRecord);
                listToReturn.Add(currentOdometerRecord);
                listToReturn.Add(lastMaintenanceOdometerRecord);
                listToReturn.Add(maintenanceIntervalRecord);
                listToReturn.Add(notifyMaintenanceRecord);
                listToReturn.Add(remainingForNextMaintenanceRecord);
            }
            return Json(listToReturn.AsQueryable().ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Create([DataSourceRequest]DataSourceRequest request, EquipmentInformation equipment, int cstId)
        {
            ModelState.Remove("EquipmentInformationId");
            if (ModelState.IsValid)
            {
                var entity = new EquipmentInformation
                {
                    EquipmentInformationId = Guid.NewGuid(),
                    CustomerId = cstId,
                    GroupName = equipment.GroupName,
                    LicensePlate = equipment.LicensePlate,
                    DeviceNumber = equipment.DeviceNumber,
                    InitialOdometer = equipment.InitialOdometer,
                    OdometerOnLastMaintenance = equipment.OdometerOnLastMaintenance,
                    CurrentOdometer = equipment.CurrentOdometer,
                    FutureMaintenanceAlert = equipment.FutureMaintenanceAlert,

                    RegularMaintenanceInterval = equipment.RegularMaintenanceInterval,
                    NotifyUpComingMaintenanceBefore = equipment.NotifyUpComingMaintenanceBefore,
                    OdometerOnNextRegularMaintenance = equipment.OdometerOnNextRegularMaintenance,
                    RemainingKmForUpComingMaintenance = equipment.RemainingKmForUpComingMaintenance,

                    LastMaintained = equipment.LastMaintained,
                    Status = equipment.Status,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };
                var intervalSetting = db.MaintenanceIntervals.FirstOrDefault(f => f.Type == MaintenanceIntervalType.RegularMaintenanceInterval);
                var notificationSetting = db.MaintenanceIntervals.FirstOrDefault(f => f.Type == MaintenanceIntervalType.NotifyUpComingMaintenanceBefore);

                if (intervalSetting != null && notificationSetting != null)
                {
                    entity.RegularMaintenanceInterval = intervalSetting.Value;
                    entity.OdometerOnNextRegularMaintenance = entity.OdometerOnLastMaintenance + entity.RegularMaintenanceInterval;

                    entity.NotifyUpComingMaintenanceBefore = notificationSetting.Value;

                    entity.RemainingKmForUpComingMaintenance = entity.OdometerOnNextRegularMaintenance - entity.CurrentOdometer;
                }
                db.EquipmentInformation.Add(entity);
                db.SaveChanges();
                equipment.EquipmentInformationId = entity.EquipmentInformationId;
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Update([DataSourceRequest]DataSourceRequest request, EquipmentInformation equipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EquipmentInformation
                {
                    EquipmentInformationId = equipment.EquipmentInformationId,
                    CustomerId = equipment.CustomerId,
                    GroupName = equipment.GroupName,
                    LicensePlate = equipment.LicensePlate,
                    DeviceNumber = equipment.DeviceNumber,
                    InitialOdometer = equipment.InitialOdometer,
                    OdometerOnLastMaintenance = equipment.OdometerOnLastMaintenance,
                    CurrentOdometer = equipment.CurrentOdometer,
                    FutureMaintenanceAlert = equipment.FutureMaintenanceAlert,

                    RegularMaintenanceInterval = equipment.RegularMaintenanceInterval,
                    NotifyUpComingMaintenanceBefore = equipment.NotifyUpComingMaintenanceBefore,
                    OdometerOnNextRegularMaintenance = equipment.OdometerOnNextRegularMaintenance,
                    RemainingKmForUpComingMaintenance = equipment.RemainingKmForUpComingMaintenance,

                    LastMaintained = equipment.LastMaintained,
                    Status = equipment.Status,

                    DateCreated = equipment.DateCreated,
                    CreatedBy = equipment.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.EquipmentInformation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Destroy([DataSourceRequest]DataSourceRequest request, EquipmentInformation equipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EquipmentInformation
                {
                    EquipmentInformationId = equipment.EquipmentInformationId,
                    CustomerId = equipment.CustomerId,
                    GroupName = equipment.GroupName,
                    LicensePlate = equipment.LicensePlate,
                    DeviceNumber = equipment.DeviceNumber,
                    InitialOdometer = equipment.InitialOdometer,
                    OdometerOnLastMaintenance = equipment.OdometerOnLastMaintenance,
                    CurrentOdometer = equipment.CurrentOdometer,
                    FutureMaintenanceAlert = equipment.FutureMaintenanceAlert,

                    RegularMaintenanceInterval = equipment.RegularMaintenanceInterval,
                    NotifyUpComingMaintenanceBefore = equipment.NotifyUpComingMaintenanceBefore,
                    OdometerOnNextRegularMaintenance = equipment.OdometerOnNextRegularMaintenance,
                    RemainingKmForUpComingMaintenance = equipment.RemainingKmForUpComingMaintenance,

                    LastMaintained = equipment.LastMaintained,
                    Status = equipment.Status,

                    DateCreated = equipment.DateCreated,
                    LastModified = equipment.LastModified,
                    CreatedBy = equipment.CreatedBy,
                    ModifiedBy = equipment.ModifiedBy
                };

                db.EquipmentInformation.Attach(entity);
                db.EquipmentInformation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GenerateJobRequest(string equipmentId)
        {
            Guid id = Guid.Parse(equipmentId);
            var currentEquipment = db.EquipmentInformation.Find(id);
            if (currentEquipment != null)
            {
                var reqs = db.JobRequest.Where(r => !r.IsOnlineApproved && r.LicensePlate == currentEquipment.LicensePlate).ToList();
                if (reqs.Any())
                {
                    return Json(new { Message = "There is active job request for this Equipment. Operation Canceled!", Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var activeOrders = db.JobOrder.Where(j => j.LicensePlate == currentEquipment.LicensePlate && j.Status != "Closed").ToList();
                    if (activeOrders.Any())
                    {
                        return Json(new { Message = "There is active job Order for this Equipment. Operation Canceled!", Status = "Error" }, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        var requestToAdd = new JobRequest
                        {
                            JobRequestId = Guid.NewGuid(),
                            LicensePlate = currentEquipment.LicensePlate,
                            EquipmentInformationId = currentEquipment.EquipmentInformationId,
                            Requester = User.Identity.Name,
                            Source = JobRequestSource.CompanyVehicle,
                            Odometer = currentEquipment.CurrentOdometer,
                            Status = "Pending",
                            Remark = "System Generated Job Request",

                            DateCreated = DateTime.Now.Date,
                            CreatedBy = User.Identity.Name,
                            IsPM = true,

                        };
                        db.JobRequest.Add(requestToAdd);
                        db.SaveChanges();

                        //var addedRequest = db.JobRequest.Find(requestToAdd.JobRequestId);
                        //if(addedRequest != null)
                        //{
                        //    requestToAdd.RequestNumber = addedRequest.RequestNumber;
                        //}

                        JobCauses cause = new JobCauses
                        {
                            Name = "Service",
                            Description = "Service",
                            JobCausesId = Guid.NewGuid(),
                            OrderId = requestToAdd.JobRequestId,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,

                        };
                        db.JobCauses.Add(cause);
                        db.SaveChanges();

                        return Json(new { Message = "Job Request Created Successfully. Request Number = " + requestToAdd.Num, Status = "Success" }, JsonRequestBehavior.AllowGet);

                    }
                }
            }
            else
                return Json(new { Message = "Not found!", Status = "Error" }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetEquipmentInformation(int? id)
        {

            //var ActiveFleet = db.Fleets.Where(x => x.Status != MaintenanceConst.OnMaintenance && x.IsOnlineApproved == true && x.IsReturned == false).Select(s => new FleetViewModel
            //{
            //    FleetsID = s.FleetsID,
            //    UnitNum = s.UnitNum,
            //    EquipmentName = s.PlateNumber + " - " + s.UnitNum,

            //    PlateNumber = s.PlateNumber + " - " + s.UnitNum
            //}).ToList();
            var list = new List<EquipmentInformation> ();
            if(id != null)
            {
                 list = db.EquipmentInformation.Where(q => q.CustomerId == id).ToList();

            }
            else{
                list = db.EquipmentInformation.ToList();
                }
            return Json(list.Select(e => new { e.EquipmentInformationId, e.LicensePlate }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
