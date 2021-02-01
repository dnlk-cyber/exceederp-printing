using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class EquipmentArrivalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentArrival_Read([DataSourceRequest] DataSourceRequest request)
        {
            var EquipmentArrival = db.EquipmentArrival.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(s => new
                                    {
                                        EquipmentArrivalId = s.EquipmentArrivalId,
                                        AssignmentId = s.AssignmentId,
                                        Date = s.Date,
                                        DCENo = s.DCENo,
                                        EquipmentType = s.EquipmentType,
                                        FleetsId = s.FleetsId,
                                        KMHRReading = s.KMHRReading,
                                        MeansOfTravelling = s.MeansOfTravelling,
                                        Model = s.Model,
                                        OperatorName = s.OperatorName,
                                        OwnedBy = s.OwnedBy,
                                        Status = s.Status,
                                        DateCreated = s.DateCreated,
                                        CreatedBy = s.CreatedBy,
                                        LastModified = s.LastModified,
                                        ModifiedBy = s.ModifiedBy,
                                        IsSendForApproval = s.IsSendForApproval,
                                        IsOnlineApproved = s.IsOnlineApproved

                                    }).ToList();

            return Json(EquipmentArrival.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentArrival_Create([DataSourceRequest] DataSourceRequest request, EquipmentArrival EquipmentArrival)
        {
            ModelState.Remove("EquipmentArrivalId");
            ModelState.Remove("AssignmentId");
            IAssignedEquipmentService _AssignedEquipmentService = new AssignedEquipmentService();
           
                var Assignment = db.AssignedEquipment.Where(x => x.AssignedEquipmentID == EquipmentArrival.FleetsId).FirstOrDefault();
                if (Assignment != null)
                {
                    var Equipment = db.Fleets.Where(x => x.FleetsID == Assignment.EquipmentID).Select(s => new  { 
                        UnitNum = s.UnitNum,
                        CurentOdoMeter = s.CurentOdoMeter,
                        Model = s.Model,
                        OperatorName = s.OperatorName,
                        IsExternal = s.IsExternal,
                        FleetsID = s.FleetsID,
                        FleetPhoto = s.EquipmentType.CategoryName


                    }).FirstOrDefault();
                    if (Equipment != null)
                    {
                        var Arrival = new EquipmentArrival
                        {
                            EquipmentArrivalId = Guid.NewGuid(),
                            AssignmentId = Assignment.AssignedEquipmentID,
                            Date = EquipmentArrival.Date,
                            DCENo = Equipment.UnitNum,
                            EquipmentType = Equipment.FleetPhoto,
                            FleetsId = Equipment.FleetsID,
                            KMHRReading = Equipment.CurentOdoMeter,
                            MeansOfTravelling = EquipmentArrival.MeansOfTravelling,
                            Model = Equipment.Model,
                            OperatorName = Equipment.OperatorName,
                            OwnedBy = Equipment.IsExternal ? "External" : "Company",
                            Status = MaintenanceConst.Pending,
                            DateCreated = DateTime.Now,    
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                
                            CreatedBy = User.Identity.Name

                        };
                        db.EquipmentArrival.Add(Arrival);
                        db.SaveChanges();
                        EquipmentArrival = Arrival;




                        EquipmentArrival = Arrival;
                    }
                }
           
            return Json(new[] { EquipmentArrival }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentArrival_Update([DataSourceRequest] DataSourceRequest request, EquipmentArrival EquipmentArrival)
        {
            if (EquipmentArrival != null)
            {
                EquipmentArrival.ModifiedBy = User.Identity.Name;
                EquipmentArrival.LastModified = DateTime.Now;
                EquipmentArrival.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db.EquipmentArrival.Attach(EquipmentArrival);
                db.Entry(EquipmentArrival).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { EquipmentArrival }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentArrival_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentArrival EquipmentArrival)
        {
            var EquipmentArrivals = db.EquipmentArrival.Where(x => x.EquipmentArrivalId == EquipmentArrival.EquipmentArrivalId).FirstOrDefault();
            if (EquipmentArrivals != null)
            {

                db.EquipmentArrival.Attach(EquipmentArrivals);
                db.EquipmentArrival.Remove(EquipmentArrivals);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted EquipmentArrival number " + EquipmentArrival.EquipmentArrivalId);

            }

            return Json(new[] { EquipmentArrival }.ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public ActionResult DistrubutedEquipmentDDBind()
        {

            IVehicleDistributionService _DistrubtionService = new VehicleDistributionService();
            IAssignedEquipmentService _AssignedEquipmentService = new AssignedEquipmentService();
            IFleetService _FleetService = new FleetService();

            var AssignedEquipments = new List<AssignedEquipment>();
            var Fleet = _FleetService.GetAll();

            List<VehicleDistribution> Distrubtions = _DistrubtionService.GetAll().Where(x => x.Status == "Approved").ToList();
           foreach(var Distrubtion in Distrubtions)
           {
                var Equipments = new List<AssignedEquipment>();
               var AssignedEquipment = _AssignedEquipmentService.GetAll().Where(x => x.RequestNumber == Distrubtion.RequestID && x.IsArrived == false);
             
               foreach(var Assigned in AssignedEquipment)
           {
               var FleetExist = Fleet.Where(x => x.FleetsID == Assigned.EquipmentID).FirstOrDefault();
                  if(FleetExist != null)
                  {
                      Equipments.Add(Assigned);
                  }
                  AssignedEquipments.AddRange(Equipments);
              }
           }
           return Json(AssignedEquipments.Select(s => new SelectListItem { Text = s.EquipmentName , Value =  s.AssignedEquipmentID.ToString() }), JsonRequestBehavior.AllowGet);
        }



    }
}
