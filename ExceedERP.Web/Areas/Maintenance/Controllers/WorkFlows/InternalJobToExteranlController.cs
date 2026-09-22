using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    public class InternalJobToExteranlController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/InternalJobToExteranl
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InternalToExternal_Read([DataSourceRequest]DataSourceRequest request,string id)
        {
            var FetchRecord = db.InternalJobToExternal.Where(x => x.ServiceCenterJobId.ToString() == id);
                var Result = FetchRecord.ToDataSourceResult(request,x=>new {
                    ServiceCenterJobId = x.ServiceCenterJobId,
                    IsOnlineApproved = x.IsOnlineApproved,
                    IsSendForApproval=x.IsSendForApproval,
                    SendForApprovalBy=x.SendForApprovalBy,
                    SendForApprovalTime=x.SendForApprovalTime,
                    IsVoid=x.IsVoid,
                    VoidBy=x.VoidBy,
                    VoidTime=x.VoidTime,
                    CreatedBy=x.CreatedBy,
                    DateCreated=x.DateCreated,
                    LastModified=x.LastModified,
                    ModifiedBy=x.ModifiedBy,
                    Description=x.Description,
                    InternalJobToExternalId=x.InternalJobToExternalId,

                });
            return Json(Result);
        }

        public ActionResult InternalToExternal_Create([DataSourceRequest]DataSourceRequest request, InternalJobToExternal InternalToExternal,string id)
        {
            InternalToExternal.ServiceCenterJobId = id;
            InternalToExternal.CreatedBy = User.Identity.Name;
            InternalToExternal.DateCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.InternalJobToExternal.Add(InternalToExternal);
                string Odometer = string.Empty;
                string EquipmentType = string.Empty;
                string AssetNum = string.Empty;
                string PlateNum = string.Empty;
                Guid EquipmentTypeId = Guid.Empty;
                var FetchJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == id);
                if(FetchJob!=null)
                {
                    var FetchOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID ==FetchJob.JobOrderID);
                    FetchJob.IsSentToExternal = true;
                    db.Entry(FetchJob).State = System.Data.Entity.EntityState.Modified;
                    if(FetchOrder!=null)
                    {
                        var Request = db.JobRequest.Where(x => x.Num==FetchOrder.Referance).FirstOrDefault();
                        if (Request != null)
                        {
                               
                                    var JobOrder = new JobOrder();
                                    JobOrder.JobOrderID = Guid.NewGuid();
                                    JobOrder.ServiceCenterJobId = FetchJob.ServiceCenterJobId.ToString();
                                    JobOrder.ToExternalJobOrderId = FetchOrder.JobOrderID.ToString();
                                    JobOrder.EquipmentInformationId = Request.EquipmentInformationId == null ? Guid.Empty : Request.EquipmentInformationId.Value;
                                    JobOrder.CurrentFuel = Request.FuelAmount;
                                    JobOrder.LicensePlate = Request.LicensePlate;
                                    JobOrder.FleetsID = db.Fleets.FirstOrDefault(x => x.PlateNumber == Request.LicensePlate || x.SerialNo == Request.LicensePlate).FleetsID;
                                    JobOrder.EquipmentName = Request.EquipmentName;
                                    JobOrder.OperatorName = Request.OperatorName;
                                    JobOrder.AssetNum = AssetNum;
                                    JobOrder.Referance = Request.Num;
                                    JobOrder.IsPM = Request.IsPM;
                                    JobOrder.Odometer = Request.Odometer;
                                    JobOrder.EquipmentType = EquipmentType;
                                    JobOrder.MaintenanceCenter = FetchOrder.MaintenanceCenter; // added
                                    JobOrder.MaintenanceCenterId = FetchOrder.MaintenanceCenterId; // added
                                    JobOrder.PercentCompleted = string.Empty;
                                    JobOrder.Odometer = Request.Odometer;
                                    JobOrder.InspectorComments = string.Empty;
                                    JobOrder.DateCompleted = DateTime.Now;
                                    JobOrder.DateCreated = DateTime.Now;
                                    JobOrder.LabourCost = 0;
                                    JobOrder.SpareCost = 0;
                                    JobOrder.IsExternal = Request.Source == JobRequestSource.ExternalVehicle ? true : false;
                                    JobOrder.DateIssued = DateTime.Now;
                                    JobOrder.Type = string.Empty;
                                    JobOrder.EquipmentNo = PlateNum;
                                    JobOrder.InspectedBy = Request.Requester;
                                    JobOrder.Status = MaintenanceConst.Pending;
                                    JobOrder.SendForApprovalTime = DateTime.Now;
                                    JobOrder.OnlineApprovedTime = DateTime.Now;
                                    JobOrder.IsSentToWorkShop = false;
                                    JobOrder.IsSubmittedToServiceCenter = false;
                                    JobOrder.CreatedBy = User.Identity.Name;
                                    JobOrder.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                                    JobOrder.BranchId = Request.BranchId;
                                    JobOrder.BusinessUnitId = Request.BusinessUnitId;
                                    JobOrder.OrgStructureId = Request.OrgStructureId;
                                    JobOrder.CreatedAtBranchId = Request.CreatedAtBranchId;
                                    JobOrder.CreatedAtBusinessUnitId = Request.CreatedAtBusinessUnitId;
                                    JobOrder.RequesterId = Request.RequesterId;
                                    JobOrder.EquipmentName = Request.EquipmentName;
                                    JobOrder.IsInsured = Request.HasInsurance;
                                    db.JobOrder.Add(JobOrder);
                                    db.SaveChanges();
                        }
                    }
                }
              
                    }

            return Json(new[] { InternalToExternal }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult InternalToExternal_Update([DataSourceRequest]DataSourceRequest request, InternalJobToExternal InternalToExternal)
        {
           var FetchRecord = db.InternalJobToExternal.FirstOrDefault(x => x.ServiceCenterJobId == InternalToExternal.ServiceCenterJobId);
            if(ModelState.IsValid)
            {
                FetchRecord.Description = InternalToExternal.Description;
                db.Entry(FetchRecord).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { InternalToExternal }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult InternalToExternal_Destroy([DataSourceRequest]DataSourceRequest request, InternalJobToExternal InternalToExternal)
        {
           var FetchRecord = db.InternalJobToExternal.FirstOrDefault(x => x.ServiceCenterJobId == InternalToExternal.ServiceCenterJobId);
            if(ModelState.IsValid)
            {
                db.InternalJobToExternal.Remove(FetchRecord);
                db.SaveChanges();
            }
            return Json(new[] { InternalToExternal }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}