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
    public class RequestFromDrugStoreController : Controller
    {
        // GET: APTS/RequestFromDrugStore
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

        public ActionResult RequestFromDrugStore_Read([DataSourceRequest]DataSourceRequest request)
        {
            var requests = db.DrugRequests.Where(x=>x.IsSendForApproval).ToList();
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
                IsSendForApproval = drugRequest.IsSendForApproval,
                IsOnlineApproved = drugRequest.IsOnlineApproved,
                IsRejected = drugRequest.IsRejected,
                IsPending = drugRequest.IsPending,
                IsDepartmentApproved = drugRequest.IsDepartmentApproved,

            });

            return Json(result);
        }
        public ActionResult RequestFromDrugStore_Create([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
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
                    Remark = drugRequest.Remark
                };
                db.DrugRequests.Add(entity);
                db.SaveChanges();
                drugRequest.DrugRequestId = entity.DrugRequestId;
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequestFromDrugStore_Update([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            //var total = model22.UnitCost * model22.QuantityIssued;
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
        public ActionResult RequestFromDrugStore_Destroy([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
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
                    Remark = drugRequest.Remark
                };
                db.DrugRequests.Attach(entity);
                db.DrugRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
       

       
        public string Confirmed(int id)
        {
            var status = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == id);
          
            if (status != null)
            {
                status.IsConfirmed = true;

                db.DrugRequests.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return status?.ToString();
        }

      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}