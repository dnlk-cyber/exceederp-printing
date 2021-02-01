using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    [AuthorizeRoles(ProcurementConstants.ProcurementUser, ProcurementConstants.ProcurementTenderApprover)]
    public class FinancialOfferController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LotFinancialOffers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<LotFinancialOffer> lotfinancialoffers = _db.LotFinancialOffers.Where(l => l.LotID == id);
            DataSourceResult result = lotfinancialoffers.ToDataSourceResult(request, lotFinancialOffer => new
            {
                LotFinancialOfferID = lotFinancialOffer.LotFinancialOfferID,
                SupplierId = lotFinancialOffer.SupplierId,
                IsNewSupplier = lotFinancialOffer.IsNewSupplier,
                SupplierName = lotFinancialOffer.SupplierName,
                LotID = lotFinancialOffer.LotID,
                Price = lotFinancialOffer.Price,
                Description = lotFinancialOffer.Description,
                OrgBranchName = lotFinancialOffer.OrgBranchName,
                DateCreated = lotFinancialOffer.DateCreated,
                LastModified = lotFinancialOffer.LastModified,
                CreatedBy = lotFinancialOffer.CreatedBy,
                ModifiedBy = lotFinancialOffer.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotFinancialOffers_Create([DataSourceRequest]DataSourceRequest request, LotFinancialOffer lotFinancialOffer, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotFinancialOffer
                {
                    LotID = id,
                    SupplierId = lotFinancialOffer.SupplierId,
                    IsNewSupplier = lotFinancialOffer.IsNewSupplier,
                    SupplierName = lotFinancialOffer.SupplierName,
                    Price = lotFinancialOffer.Price,
                    Description = lotFinancialOffer.Description,
                    OrgBranchName = lotFinancialOffer.OrgBranchName,
                    DateCreated = DateTime.Now.Date,
                    LastModified = lotFinancialOffer.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = lotFinancialOffer.ModifiedBy
                };

                _db.LotFinancialOffers.Add(entity);
                _db.SaveChanges();
                lotFinancialOffer.LotFinancialOfferID = entity.LotFinancialOfferID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot financial offer", "Success", User.Identity.Name, $"created financial offer {entity.Description} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotFinancialOffer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotFinancialOffers_Update([DataSourceRequest]DataSourceRequest request, LotFinancialOffer lotFinancialOffer)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotFinancialOffer
                {
                    LotFinancialOfferID = lotFinancialOffer.LotFinancialOfferID,
                    LotID = lotFinancialOffer.LotID,
                    SupplierId = lotFinancialOffer.SupplierId,
                    IsNewSupplier = lotFinancialOffer.IsNewSupplier,
                    SupplierName = lotFinancialOffer.SupplierName,
                    Price = lotFinancialOffer.Price,
                    Description = lotFinancialOffer.Description,
                    OrgBranchName = lotFinancialOffer.OrgBranchName,
                    DateCreated = lotFinancialOffer.DateCreated,
                    LastModified = lotFinancialOffer.LastModified,
                    CreatedBy = lotFinancialOffer.CreatedBy,
                    ModifiedBy = lotFinancialOffer.ModifiedBy
                };

                _db.LotFinancialOffers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot financial offer", "Success", User.Identity.Name, $"updated financial offer {entity.Description} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { lotFinancialOffer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LotFinancialOffers_Destroy([DataSourceRequest]DataSourceRequest request, LotFinancialOffer lotFinancialOffer)
        {
            if (ModelState.IsValid)
            {
                var entity = new LotFinancialOffer
                {
                    LotFinancialOfferID = lotFinancialOffer.LotFinancialOfferID,
                    SupplierId = lotFinancialOffer.SupplierId,
                    IsNewSupplier = lotFinancialOffer.IsNewSupplier,
                    SupplierName = lotFinancialOffer.SupplierName,
                    Price = lotFinancialOffer.Price,
                    Description = lotFinancialOffer.Description,
                    OrgBranchName = lotFinancialOffer.OrgBranchName,
                    DateCreated = lotFinancialOffer.DateCreated,
                    LastModified = lotFinancialOffer.LastModified,
                    CreatedBy = lotFinancialOffer.CreatedBy,
                    ModifiedBy = lotFinancialOffer.ModifiedBy
                };

                _db.LotFinancialOffers.Attach(entity);
                _db.LotFinancialOffers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { lotFinancialOffer }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetFinancialOfferedSupplier(string id)
        {
            int lotid = int.Parse(id);
            var persons = _db.ParticipantsItems.Where(l => l.LotID == lotid).AsQueryable();
            List<BidPurchasingSuppliers> supplierList = new List<BidPurchasingSuppliers>();
            foreach (ParticipantsItem participantsItem in persons)
            {

                var suppliers = _db.BidPurchasingSupplierses.FirstOrDefault(x=>x.SupplierName==participantsItem.SupplierName);
                supplierList.Add(suppliers);
            }
            return Json(supplierList.Select(i => new { Supplier = i.SupplierName, SupplierCode = i.BidPurchasingSuppliersID }), JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
