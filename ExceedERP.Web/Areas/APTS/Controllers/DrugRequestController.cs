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
    public class DrugRequestController : Controller
    {
        // GET: APTS/DrugRequest
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
        }
        public string CheckStatus(int masterId)
        {
            var reques = db.APTSRequestStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest").FirstOrDefault();
            if (reques != null)
            {
                var checkApprove = db.APTSRequestStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest" && x.Status == DrugRequestStatus.APPROVE).FirstOrDefault();
                if (checkApprove != null)
                {
                    return "approved";
                }
                var reject = db.APTSRequestStatus.Where(x => x.RequestId == masterId && x.ReferenceType == "DrugRequest" && x.Status == DrugRequestStatus.REJECT).FirstOrDefault();
                if (reject != null)
                {
                    return "Dapproved";
                }
                return "true";
            }
            return "false";
        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(DrugRequestStatus))
                .Cast<DrugRequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }
        public ActionResult SendForApproval()
        {
            var status =
                     db.APTSRequestStatus.Where(x => x.RequestId == HRMInputs.RequestId && x.ReferenceType == HRMInputs.ReferenceType).ToList();
            string messages = "";
            if (status.Any())
            {
                foreach (var s in status)
                {
                    if (s.Status == DrugRequestStatus.APPROVE)
                    {
                        messages = "The Request is already approved!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }
                    if (s.Status == DrugRequestStatus.WAITING_FOR_APPROVAL)
                    {
                        messages = "The Request is already send for approval!!";
                        return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                    }

                }
            }
            APTSRequestStatus requestesStatus;
            db = new ExceedDbContext();
            requestesStatus =
                db.APTSRequestStatus.Where(
                    x =>
                        x.RequestId == HRMInputs.RequestId &&
                        x.ReferenceType == HRMInputs.ReferenceType && x.Status == DrugRequestStatus.WAITING_FOR_APPROVAL).FirstOrDefault();
            db = new ExceedDbContext();
            if (requestesStatus == null)
            {
                var entityy = new APTSRequestStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = DrugRequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.APTSRequestStatus.Add(entityy);
                db.SaveChanges();
            }
            else
            {
                var entity = new APTSRequestStatus
                {
                    APTSRequestStatusId = requestesStatus.APTSRequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    Remark = requestesStatus.Remark,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(requestesStatus.Date.Day, requestesStatus.Date.Month, requestesStatus.Date.Year),
                };

                db.APTSRequestStatus.Attach(entity);
                db.APTSRequestStatus.Remove(entity);
                db.SaveChanges();
                db = new ExceedDbContext();

                var entityy = new APTSRequestStatus
                {
                    RequestId = HRMInputs.RequestId,
                    ReferenceType = HRMInputs.ReferenceType,
                    Status = DrugRequestStatus.WAITING_FOR_APPROVAL,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year)
                };

                db.APTSRequestStatus.Add(entityy);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult DrugRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            var requests = db.DrugRequests.ToList();
            DataSourceResult result = requests.ToDataSourceResult(request, drugRequest => new
            {
                BranchId = drugRequest.BranchId,
                DrugRequestId = drugRequest.DrugRequestId,
                GlFiscalYearId = drugRequest.GlFiscalYearId,
                RequestDate = drugRequest.RequestDate,
                RequestDateAm = drugRequest.RequestDateAm,
                DateCreated = drugRequest.DateCreated,
                Remark = drugRequest.Remark,
                IsClosed = drugRequest.IsClosed,
                IsConfirmed = drugRequest.IsConfirmed,
                IsSendForApproval = drugRequest.IsSendForApproval,
                IsOnlineApproved = drugRequest.IsOnlineApproved,
                IsRejected = drugRequest.IsRejected,
                IsPending = drugRequest.IsPending,
                IsDepartmentApproved = drugRequest.IsDepartmentApproved,
                StoreCode = drugRequest.StoreCode

            });

            return Json(result);
        }
        public ActionResult DrugRequest_Create([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
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
                        GlFiscalYearId = drugRequest.GlFiscalYearId,
                        BranchId = drugRequest.BranchId,
                        RequestDateAm = drugRequest.RequestDateAm,
                        RequestDate = RequestDate,
                        DateCreated = DateTime.Today,
                        StoreCode = drugRequest.StoreCode,
                        Remark = drugRequest.Remark

                    };
                    db.DrugRequests.Add(entity);
                    db.SaveChanges();
                    drugRequest.DrugRequestId = entity.DrugRequestId;

                }

            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugRequest_Update([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            ModelState.Clear();
            if (drugRequest.RequestDateAm != null)
            {
                var splitDate = drugRequest.RequestDateAm.Split('/');
                drugRequest.RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Reqeust Date is required");
            }
            if (ModelState.IsValid)
            {
                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestDate = drugRequest.RequestDate,
                    DateCreated = drugRequest.DateCreated,
                    StoreCode = drugRequest.StoreCode,
                    IsSendForApproval = drugRequest.IsSendForApproval,
                    IsOnlineApproved = drugRequest.IsOnlineApproved,
                    IsRejected = drugRequest.IsRejected,
                    IsPending = drugRequest.IsPending,
                    IsDepartmentApproved = drugRequest.IsDepartmentApproved,
                    Remark = drugRequest.Remark

                };

                db.DrugRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DrugRequest_Destroy([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestDate = drugRequest.RequestDate,
                    DateCreated = drugRequest.DateCreated,
                    Remark = drugRequest.Remark

                };
                db.DrugRequests.Attach(entity);
                db.DrugRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
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