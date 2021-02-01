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
    public class DrutRequestStatusController : Controller
    {
        // GET: APTS/DrutRequestStatus
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["UnitsList"] = db.DrugUnits.ToList();
            var drug = db.DrugLists.Where(x => x.IsActive);
            ViewData["druglists"] = drug;
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["Period"] = db.GLPeriods.ToList();
            ViewData["Branches"] = db.Branch.ToList();
            ViewData["Stores"] = db.InventoryStores.ToList();
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
        public ActionResult DrutRequestStatus_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<DrugRequest> requests = new List<DrugRequest>();

            var drugRequests = db.DrugRequests.ToList();


            foreach (var m in drugRequests)
            {
                db = new ExceedDbContext();
                var status = db.APTSRequestStatus.Where(x => x.Status == DrugRequestStatus.WAITING_FOR_APPROVAL && x.RequestId == m.DrugRequestId && x.ReferenceType == "DrugRequest").FirstOrDefault();
                if (status != null)
                {
                    requests.Add(m);
                }

            }
            DataSourceResult result = requests.ToDataSourceResult(request, drugRequest => new
            {
                DrugRequestId = drugRequest.DrugRequestId,
                GlFiscalYearId = drugRequest.GlFiscalYearId,
                BranchId = drugRequest.BranchId,
                RequestDate = drugRequest.RequestDate,
                RequestDateAm = drugRequest.RequestDateAm,
                DateCreated = drugRequest.DateCreated,
                Remark = drugRequest.Remark,
                IsClosed = drugRequest.IsClosed,
                IsConfirmed = drugRequest.IsConfirmed,
                StoreCode = drugRequest.StoreCode,
                IsSendForApproval = drugRequest.IsSendForApproval,
                IsOnlineApproved = drugRequest.IsOnlineApproved,
                IsRejected = drugRequest.IsRejected,
                IsPending = drugRequest.IsPending,
                IsDepartmentApproved = drugRequest.IsDepartmentApproved,

            });

            return Json(result);
        }
        public ActionResult DrutRequestStatus_Create([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    DateCreated = drugRequest.DateCreated,
                    StoreCode = drugRequest.StoreCode,
                    IsClosed = drugRequest.IsClosed,
                    IsConfirmed = drugRequest.IsConfirmed,
                    IsOnlineApproved = drugRequest.IsOnlineApproved,
                    IsSendForApproval = drugRequest.IsSendForApproval,
                    Remark = drugRequest.Remark
                };
                db.DrugRequests.Add(entity);
                db.SaveChanges();
                drugRequest.DrugRequestId = entity.DrugRequestId;



            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrutRequestStatus_Update([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (drugRequest.RequestDateAm != null)
                {
                    var splitDate = drugRequest.RequestDateAm.Split('/');
                    var RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                    var entity = new DrugRequest
                    {
                        DrugRequestId = drugRequest.DrugRequestId,
                        GlFiscalYearId = drugRequest.GlFiscalYearId,
                        BranchId = drugRequest.BranchId,
                        RequestDate = RequestDate,
                        RequestDateAm = drugRequest.RequestDateAm,
                        DateCreated = DateTime.Today,
                        StoreCode = drugRequest.StoreCode,
                        IsClosed = drugRequest.IsClosed,
                        IsConfirmed = drugRequest.IsConfirmed,
                        IsOnlineApproved = drugRequest.IsOnlineApproved,
                        IsSendForApproval = drugRequest.IsSendForApproval,
                        IsRejected = drugRequest.IsRejected,
                        IsPending = drugRequest.IsPending,
                        IsDepartmentApproved = drugRequest.IsDepartmentApproved,
                        Remark = drugRequest.Remark
                    };

                    db.DrugRequests.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrutRequestStatus_Destroy([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    DateCreated = drugRequest.DateCreated,
                    StoreCode = drugRequest.StoreCode,
                    Remark = drugRequest.Remark
                };
                db.DrugRequests.Attach(entity);
                db.DrugRequests.Remove(entity);
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