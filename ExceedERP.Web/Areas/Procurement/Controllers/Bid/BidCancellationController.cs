using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class BidCancellationController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }
       

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderCancellations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
           

           IQueryable <TenderCancellation> tenderCancellations = _db.TenderCancellations.Where(x => x.TenderID == id);
            DataSourceResult result = tenderCancellations.ToDataSourceResult(request, tenderCancellation => new
            {
                TenderCancellationId = tenderCancellation.TenderCancellationId,
                TenderID = tenderCancellation.TenderID,
                Date = tenderCancellation.Date,
                Description = tenderCancellation.Description,
                CreatedBy = tenderCancellation.CreatedBy,
                DateCreated = tenderCancellation.DateCreated,
                ModifiedBy = tenderCancellation.ModifiedBy,
                LastModified = tenderCancellation.LastModified,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellations_Create([DataSourceRequest]DataSourceRequest request, TenderCancellation tenderCancellation, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderCancellation
                {
                    TenderID = id,
                    TenderCancellationId = tenderCancellation.TenderCancellationId,
                    Description = tenderCancellation.Description,
                    Date = tenderCancellation.Date,
                    CreatedBy = User.Identity.Name,
                    DateCreated=DateTime.Now,
                };

                _db.TenderCancellations.Add(entity);
                _db.SaveChanges();
                tenderCancellation.TenderCancellationId = entity.TenderCancellationId;

                #region update tender

                var tender = _db.Tenders.Find(id);
                if (tender != null)
                {
                    tender.Status = "Cancelled";
                    tender.IsCancelled = true;
                    _db.TenderCancellations.Attach(entity);
                    _db.Entry(tender).State = EntityState.Modified;
                }

                _db.SaveChanges();

                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender cancellation", "Success", User.Identity.Name, $"created bid cancellation {entity.Description} under record {entity.TenderID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderCancellation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellations_Update([DataSourceRequest]DataSourceRequest request, TenderCancellation tenderCancellation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderCancellation
                {
                    TenderID=tenderCancellation.TenderID,
                    TenderCancellationId = tenderCancellation.TenderCancellationId,
                    Description=tenderCancellation.Description,
                    Date = tenderCancellation.Date,
                    CreatedBy =tenderCancellation.CreatedBy,
                    DateCreated = tenderCancellation.DateCreated,
                    ModifiedBy=User.Identity.Name,
                    LastModified=DateTime.Now
                };

                _db.TenderCancellations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender cancellation", "Success", User.Identity.Name, $"updated bid cancellation {entity.Description} under record {entity.TenderID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderCancellation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCancellations_Destroy([DataSourceRequest]DataSourceRequest request, TenderCancellation tenderCancellation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderCancellation
                {
                    TenderCancellationId = tenderCancellation.TenderCancellationId,
                    TenderID = tenderCancellation.TenderID,
                    Date = tenderCancellation.Date
                };
               
                _db.TenderCancellations.Attach(entity);
                _db.TenderCancellations.Remove(entity);
                _db.SaveChanges();
                #region update tender

                var tender = _db.Tenders.Find(tenderCancellation.TenderID);
                if (tender != null)
                {
                    tender.IsCancelled = false;
                    _db.TenderCancellations.Attach(entity);
                    _db.Entry(tender).State = EntityState.Modified;
                }

                _db.SaveChanges();

                #endregion
            }

            return Json(new[] { tenderCancellation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
