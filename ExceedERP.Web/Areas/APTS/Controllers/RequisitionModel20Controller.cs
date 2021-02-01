using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class RequisitionModel20Controller : Controller
    {
        // GET: APTS/RequisitionModel20
        ExceedDbContext db = new ExceedDbContext();
        public static string RequestDateAm { get; set; }
        public static DateTime RequestDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var drug = db.DrugLists.Where(x => x.IsActive);
            ViewData["druglists"] = drug;
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["Period"] = db.GLPeriods.ToList();
            ViewData["Unit"] = db.UoMs.ToList();
        }
        public void SetDate(string name, string value)
        {
            if (name == "RequestDateAm")
            {
                RequestDateAm = value;
                var splitDate = RequestDateAm.Split('/');
                RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
           


        }
        public ActionResult GetDrugLists()
        {
            var drugLists = db.DrugLists.Where(x => x.IsActive && x.IsOur);
            return Json(drugLists.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUnits()
        {
            var drugUnits = db.UoMs.ToList();
            return Json(drugUnits.Select(p => new
            {
                UoMName = p.UoMName,
                Description = p.UoMName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFisicalYear()
        {

            var periodList = db.GLFiscalYears;

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriod()
        {

            var periodList = db.GLPeriods;
            //var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(RequestStatus))
                .Cast<RequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public ActionResult SendForApproval()
        {
            var status =
                     db.RequestesStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == RequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }
                    if (s.Status == RequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
            RequestesStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.RequestesStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == RequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new RequestesStatus
                {
                    RequestStatusId = requestesStatus.RequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    Remark = requestesStatus.Remark,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                };

                db.RequestesStatus.Attach(entity);
                db.RequestesStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new RequestesStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = RequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.RequestesStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult RequisitionModel20_Read([DataSourceRequest]DataSourceRequest request)
        {
            var requests = db.RequisionModel20s.ToList();
            DataSourceResult result = requests.ToDataSourceResult(request, medice => new
            {
                RequisitionModel20Id = medice.RequisitionModel20Id,
                DrugListsId = medice.DrugListsId,
                BegStock = medice.BegStock,
                ReceivedQty = medice.ReceivedQty,
                Description = medice.Description,
                InternalCode = medice.InternalCode,
                Loss = medice.Loss,
                EndStock = medice.EndStock,
                CalculatedConsumption = medice.CalculatedConsumption,
                Max = medice.Max,
                QtyToMax = medice.QtyToMax,
                QtyToBeApproved = medice.QtyToBeApproved,
                RequestDate = medice.RequestDate,
                RequestDateAm = medice.RequestDateAm,
                RequestedBy = medice.RequestedBy,
                ApproverName = medice.ApproverName,
                ApprovedDate = medice.ApprovedDate,
                DateCreated = medice.DateCreated,
                CreatedBy = medice.CreatedBy,
                IsApproved = medice.IsApproved,
                IsNew = medice.IsNew,
                IsClosed = medice.IsClosed,
                IsOut = medice.IsOut,
                IsCompleted = medice.IsCompleted,
                GlFiscalYearId = medice.GlFiscalYearId,
                GLPeriodId = medice.GLPeriodId,
                Requester = medice.Requester,
                Unit = medice.Unit,
                
                Remark = medice.Remark

            });

            return Json(result);
        }
        public ActionResult RequisitionModel20_Create([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {
                if(drugRequest.RequestDateAm != null)
                {
                    var splitDate = drugRequest.RequestDateAm.Split('/');
                    var RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                }

                var entity = new RequisitionModel20
                {
                    DrugListsId = drugRequest.DrugListsId,
                    BegStock = drugRequest.BegStock,
                    ReceivedQty = drugRequest.ReceivedQty,
                    Loss = drugRequest.Loss,
                    EndStock = drugRequest.EndStock,
                    CalculatedConsumption = drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock,
                    Max = (drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock) * 2,
                    QtyToMax = ((drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock) * 2) - 2,
                    QtyToBeApproved = drugRequest.QtyToBeApproved,
                    RequestDate = drugRequest.RequestDate,//DateTime.Now,// RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestedBy = drugRequest.RequestedBy,
                    Description = db.DrugLists.FirstOrDefault(x=>x.DrugListsId == drugRequest.DrugListsId)?.Description,
                    InternalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugRequest.DrugListsId)?.InternalDrugCode,
                    ApproverName = drugRequest.ApproverName,
                    ApprovedDate = drugRequest.ApprovedDate,
                    DateCreated = drugRequest.DateCreated,
                    IsApproved =drugRequest.IsApproved,
                    IsNew = true,
                    CreatedBy = drugRequest.CreatedBy,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    Unit = drugRequest.Unit,
                    Remark = drugRequest.Remark,
                    IsClosed = false,
                    IsOut = false,
                    IsCompleted = false,
                };
                db.RequisionModel20s.Add(entity);
                db.SaveChanges();
                drugRequest.RequisitionModel20Id = entity.RequisitionModel20Id;

                var requestedDrug = db.RequisionModel20s.FirstOrDefault(x=>x.DrugListsId == drugRequest.DrugListsId && x.IsApproved && x.IsOut && x.IsCompleted);
                if(requestedDrug != null)
                {
                    var despensaryStore = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == requestedDrug.DrugListsId);
                    if(despensaryStore != null)
                    {
                        despensaryStore.BegStock = despensaryStore.BegStock + despensaryStore.NewReceived;
                        despensaryStore.NewReceived = requestedDrug.ReceivedQty;
                        despensaryStore.Balance = despensaryStore.BegStock + despensaryStore.NewReceived;

                        db.Entry(despensaryStore).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
        
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequisitionModel20_Update([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new RequisitionModel20
                {
                    RequisitionModel20Id=drugRequest.RequisitionModel20Id,
                    DrugListsId = drugRequest.DrugListsId,
                    Description = drugRequest.Description,
                    InternalCode = drugRequest.InternalCode,
                    BegStock = drugRequest.BegStock,
                    ReceivedQty = drugRequest.ReceivedQty,
                    Loss = drugRequest.Loss,                  
                    EndStock = drugRequest.EndStock,
                    CalculatedConsumption = drugRequest.CalculatedConsumption,
                    Max = drugRequest.Max,
                    QtyToMax = drugRequest.QtyToMax,
                    QtyToBeApproved = drugRequest.QtyToBeApproved,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestedBy = drugRequest.RequestedBy,
                    ApproverName = drugRequest.ApproverName,
                    IsApproved = drugRequest.IsApproved,
                    IsNew = drugRequest.IsNew,
                    ApprovedDate = drugRequest.ApprovedDate,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    Unit = drugRequest.Unit,
                    DateCreated = drugRequest.DateCreated,
                    CreatedBy = drugRequest.CreatedBy,
                    Remark = drugRequest.Remark
                };

                db.RequisionModel20s.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequisitionModel20_Destroy([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new RequisitionModel20
                {
                    RequisitionModel20Id = drugRequest.RequisitionModel20Id,
                    DrugListsId = drugRequest.DrugListsId,
                    Description = drugRequest.Description,
                    InternalCode = drugRequest.InternalCode,
                    BegStock = drugRequest.BegStock,
                    ReceivedQty = drugRequest.ReceivedQty,
                    Loss = drugRequest.Loss,
                    EndStock = drugRequest.EndStock,
                    CalculatedConsumption = drugRequest.CalculatedConsumption,
                    Max = drugRequest.Max,
                    QtyToMax = drugRequest.QtyToMax,
                    QtyToBeApproved = drugRequest.QtyToBeApproved,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestedBy = drugRequest.RequestedBy,
                    ApproverName = drugRequest.ApproverName,
                    IsApproved = drugRequest.IsApproved,
                    IsNew = drugRequest.IsNew,
                    ApprovedDate = drugRequest.ApprovedDate,
                    DateCreated = DateTime.Now,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    Unit = drugRequest.Unit,
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    CreatedBy = drugRequest.CreatedBy,
                    Remark = drugRequest.Remark
                };
                db.RequisionModel20s.Attach(entity);
                db.RequisionModel20s.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public string Approve(int id)
        {
            var status = db.RequisionModel20s.FirstOrDefault(x => x.RequisitionModel20Id == id);
            if (status != null)
            {
                status.IsApproved = true;
               
                db.RequisionModel20s.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return status?.ToString();
        }
        public string GetBalance(int drugList)
        {
            var stockBalance = "";
            var stock = db.DespensaryStores.FirstOrDefault(x=>x.DrugListsId == drugList);
            var begBalance = stock.BegStock;
            var receivedQuantity = stock.NewReceived;

            stockBalance = begBalance + "-" + receivedQuantity;
            //  var endStock = begBalance + receivedQuantity + stock
            return stockBalance;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}