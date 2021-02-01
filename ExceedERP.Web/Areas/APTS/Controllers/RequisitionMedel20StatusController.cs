using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class RequisitionMedel20StatusController : Controller
    {
        // GET: APTS/RequisitionMedel20Status
        ExceedDbContext db = new ExceedDbContext();
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
        public ActionResult GetDrugLists()
        {
            var drugLists = db.DrugLists.Where(x => x.IsActive && x.IsOur);
            return Json(drugLists.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
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
        public string CheckStatus(int masterId)
        {
            var reques = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest");
            if (reques != null)
            {
                var checkApprove = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest" && x.Status == RequestStatus.APPROVE);
                if (checkApprove != null)
                {
                    return "approved";
                }
                var deptApproval = db.RequestesStatus.FirstOrDefault(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest" && x.Status == RequestStatus.DEPARTMENT_APPROVAL);
                if (deptApproval != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        public ActionResult RequisitionMedel20Status_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<RequisitionModel20> requests = new List<RequisitionModel20>();

            var drugRequests = db.RequisionModel20s.ToList();
          
       
            foreach (var m in drugRequests)
            {
                db = new ExceedDbContext();
                var status = db.RequestesStatus.Where(x => x.Status == RequestStatus.WAITING_FOR_APPROVAL && x.RequestId == m.RequisitionModel20Id && x.ReferenceType == "DrugRequest").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }
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
                GlFiscalYearId = medice.GlFiscalYearId,
                GLPeriodId = medice.GLPeriodId,
                Requester = medice.Requester,
                Unit = medice.Unit,
                Remark = medice.Remark

            });

            return Json(result);
        }
        public ActionResult RequisitionMedel20Status_Create([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {
                //var splitDate = drugRequest.RequestDateAm.Split('/');
                //var RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

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
                    Description = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugRequest.DrugListsId)?.Description,
                    InternalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugRequest.DrugListsId)?.InternalDrugCode,
                    ApproverName = drugRequest.ApproverName,
                    ApprovedDate = drugRequest.ApprovedDate,
                    DateCreated = drugRequest.DateCreated,
                    IsApproved = drugRequest.IsApproved,
                    IsNew = true,
                    CreatedBy = drugRequest.CreatedBy,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    Unit = drugRequest.Unit,
                    Remark = drugRequest.Remark
                };
                db.RequisionModel20s.Add(entity);
                db.SaveChanges();
                drugRequest.RequisitionModel20Id = entity.RequisitionModel20Id;

                //var requestedDrug = db.RequisionModel20s.FirstOrDefault(x => x.DrugListsId == drugRequest.DrugListsId && x.IsApproved && x.IsOut && x.IsCompleted);
                //if (requestedDrug != null)
                //{
                //    var despensaryStore = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == requestedDrug.DrugListsId);
                //    if (despensaryStore != null)
                //    {
                //        despensaryStore.CurrentAmount = despensaryStore.CurrentAmount + despensaryStore.NewReceived;
                //        despensaryStore.NewReceived = requestedDrug.ReceivedQty;
                //        despensaryStore.TotalAmount = despensaryStore.CurrentAmount + despensaryStore.NewReceived;

                //        db.Entry(despensaryStore).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}

            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequisitionMedel20Status_Update([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
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
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    DateCreated = drugRequest.DateCreated,
                    CreatedBy = drugRequest.CreatedBy,
                    Unit = drugRequest.Unit,
                    Remark = drugRequest.Remark
                };

                db.RequisionModel20s.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequisitionMedel20Status_Destroy([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
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
                    Requester = drugRequest.Requester,
                    GLPeriodId = drugRequest.GLPeriodId,
                    CreatedBy = drugRequest.CreatedBy,
                    Unit = drugRequest.Unit,
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
            var stock = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == drugList);
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