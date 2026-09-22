using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    public class ExternalPaymentRequestJobController : Controller
    {
        // GET: Maintenance/ExternalPaymentRequestJob
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalPaymentRequestJob_Read([DataSourceRequest]DataSourceRequest request,int Id)
        {
            IQueryable<ExternalPaymentRequestJob> FetchJobs=db.ExternalPaymentRequestJobs.Where(x => x.ExternalMaintenancePaymenntrequestId == Id);
            DataSourceResult result = FetchJobs.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet); // Use AllowGet for GET requests if needed
        }
        public ActionResult ExternalPaymentRequestJob_Create([DataSourceRequest] DataSourceRequest request, ExternalPaymentRequestJob PaymentRequestJob, int Id)
        {
            var fetchJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == PaymentRequestJob.ServiceCenterJobId.ToString());
            if(fetchJob!=null)
            {
                var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == fetchJob.JobOrderID);
                if(fetchJobOrder!=null)
                {
                    PaymentRequestJob.EnteranceDate = fetchJobOrder.DateCreated;
                    PaymentRequestJob.ExitDate = fetchJobOrder.DateCompleted;
                }
                PaymentRequestJob.FleetId = fetchJob.FleetsID;
                PaymentRequestJob.EquipmentName = fetchJob.EquipmentName;
              
            }
            if(ModelState.IsValid)
            {
                var entity = new ExternalPaymentRequestJob
                {
                    ExternalMaintenancePaymenntrequestId = Id,
                    ServiceCenterJobId = PaymentRequestJob.ServiceCenterJobId,
                    Desctiption = PaymentRequestJob.Desctiption,
                    EnteranceDate = PaymentRequestJob.EnteranceDate,
                    ExitDate=PaymentRequestJob.ExitDate,
                    FleetId=PaymentRequestJob.FleetId,
                    EquipmentName=PaymentRequestJob.EquipmentName,

                };
                db.ExternalPaymentRequestJobs.Add(entity);
                db.SaveChanges();
                decimal TotalOnMaintenance = 0;
                var fetchHeader = db.ExternalMaintenancePaymentRequests.FirstOrDefault(x => x.ExternalMaintenancePaymenntrequestId == entity.ExternalMaintenancePaymenntrequestId);
                   
                var fetchExternalPaymentRequest = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId.ToString() == entity.ServiceCenterJobId);
                    if(fetchExternalPaymentRequest!=null)
                {
                    var fetchExternalPayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == fetchExternalPaymentRequest.ServiceCenterJobId).ToList();
                       foreach(var ExternalPayment in fetchExternalPayment)
                    {
                        TotalOnMaintenance += ExternalPayment.Total;
                        var externalEntity = new ExternalMaintenancePaymentRequestDetail
                        {
                            UnitPrice = ExternalPayment.UnitPrice,
                            CostType = ExternalPayment.CostType,
                            Date = ExternalPayment.Date,
                            Quantity = ExternalPayment.Quantity,
                            SubTotal = ExternalPayment.SubTotal,
                            GlTaxRateId = ExternalPayment.GlTaxRateId,
                            Total = ExternalPayment.Total,
                            ExternalPaymentRequestJobId = entity.ExternalPaymentRequestJobId,
                        };
                        db.ExternalMaintenancePaymentRequestDetails.Add(externalEntity);                        
                    }
                    fetchHeader.TotalAmount += TotalOnMaintenance;
                    fetchHeader.AmountDue += TotalOnMaintenance;
                    db.Entry(fetchHeader).State = System.Data.Entity.EntityState.Modified;
                 db.SaveChanges();
                }
            }
            return Json(new[] { PaymentRequestJob }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult ExternalPaymentRequestJob_Update([DataSourceRequest]DataSourceRequest request, ExternalPaymentRequestJob PaymentRequestJob)
        {
            var FetchJob = db.ExternalPaymentRequestJobs.FirstOrDefault(x => x.ExternalPaymentRequestJobId == PaymentRequestJob.ExternalPaymentRequestJobId);
            if(FetchJob==null)
            {
                ModelState.AddModelError("No Record", "Record Not Found");
            }
            if(ModelState.IsValid)
            {
                FetchJob.ServiceCenterJobId = PaymentRequestJob.ServiceCenterJobId;
                FetchJob.Desctiption = PaymentRequestJob.Desctiption;
                db.Entry(FetchJob).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { PaymentRequestJob }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ExternalPaymentRequestJob_Destroy([DataSourceRequest]DataSourceRequest request, ExternalPaymentRequestJob PaymentRequestJob)
        {
            var FetchJob = db.ExternalPaymentRequestJobs.FirstOrDefault(x => x.ExternalPaymentRequestJobId == PaymentRequestJob.ExternalPaymentRequestJobId);

            if (FetchJob == null)
            {
                ModelState.AddModelError("No Record", "Record Not Found");
            }
            if (ModelState.IsValid)
            {
                db.ExternalPaymentRequestJobs.Remove(FetchJob);
                db.SaveChanges();
            }
            return Json(new[] { PaymentRequestJob }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}