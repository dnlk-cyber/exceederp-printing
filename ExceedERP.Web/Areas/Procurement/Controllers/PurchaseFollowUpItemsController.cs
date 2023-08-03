﻿
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class PurchaseFollowUpItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        public ActionResult PurchaseFollowUpItems_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PurchaseFollowUpItems> purchasefollowupitems = db.PurchaseFollowUpItems.Where(x=>x.PurchaseFollowUpID==id);
            DataSourceResult result = purchasefollowupitems.ToDataSourceResult(request, purchaseFollowUpItems => new
            {
                PurchaseFollowUpID = purchaseFollowUpItems.PurchaseFollowUpID,
                PurchaseFollowUpItemsID = purchaseFollowUpItems.PurchaseFollowUpItemsID,
                CatCode = purchaseFollowUpItems.CatCode+"-"+purchaseFollowUpItems.CategoryName,
                CategoryName = purchaseFollowUpItems.CategoryName,
                ItmCode = purchaseFollowUpItems.ItmCode + "-" + purchaseFollowUpItems.ItemName,
                ItemName = purchaseFollowUpItems.ItemName,
                PartNo = purchaseFollowUpItems.PartNo,
                Quantity = purchaseFollowUpItems.Quantity,
                ItemSpecification = purchaseFollowUpItems.ItemSpecification,
                UoM = purchaseFollowUpItems.UoM,
                ItemType = purchaseFollowUpItems.ItemType,
                Supplier = purchaseFollowUpItems.Supplier,
                PurchaserOrder = purchaseFollowUpItems.PurchaserOrder,
                InsuranceAppliedDate = purchaseFollowUpItems.InsuranceAppliedDate,
                InsuranceNo = purchaseFollowUpItems.InsuranceNo,
                InsuranceCertificateDate = purchaseFollowUpItems.InsuranceCertificateDate,
                FCAppliedDate = purchaseFollowUpItems.FCAppliedDate,
                FCPermitDate = purchaseFollowUpItems.FCPermitDate,
                FCPermitNo = purchaseFollowUpItems.FCPermitNo,
                FCPermitExpiryDate = purchaseFollowUpItems.FCPermitExpiryDate,
                LCAppliedDate = purchaseFollowUpItems.LCAppliedDate,
                LCNo = purchaseFollowUpItems.LCNo,
                LCOpenedDate = purchaseFollowUpItems.LCOpenedDate,
                Vessel = purchaseFollowUpItems.Vessel,
                Port = purchaseFollowUpItems.Port,
                ShippedDate = purchaseFollowUpItems.ShippedDate,
                VesselArriveDate = purchaseFollowUpItems.VesselArriveDate,
                BillOfLoading = purchaseFollowUpItems.BillOfLoading,
                DocumentCollectionDate = purchaseFollowUpItems.DocumentCollectionDate,
                DocumentSubmitDate = purchaseFollowUpItems.DocumentSubmitDate,
                Maritimeno = purchaseFollowUpItems.Maritimeno,
                ArriveDate = purchaseFollowUpItems.ArriveDate,
                Condition = purchaseFollowUpItems.Condition,
                Damaged = purchaseFollowUpItems.Damaged,
                Missing = purchaseFollowUpItems.Missing,
                ClaimDate = purchaseFollowUpItems.ClaimDate,
                RefNo = purchaseFollowUpItems.RefNo
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUser, ProcurementRoles.ProcurementApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseFollowUpItems_Update([DataSourceRequest]DataSourceRequest request, PurchaseFollowUpItems purchaseFollowUpItems)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseFollowUpItems
                {
                    PurchaseFollowUpID = purchaseFollowUpItems.PurchaseFollowUpID,
                    PurchaseFollowUpItemsID = purchaseFollowUpItems.PurchaseFollowUpItemsID,
                    CatCode = purchaseFollowUpItems.CatCode,
                    CategoryName = purchaseFollowUpItems.CategoryName,
                    ItmCode = purchaseFollowUpItems.ItmCode,
                    ItemName = purchaseFollowUpItems.ItemName,
                    PartNo = purchaseFollowUpItems.PartNo,
                    Supplier = purchaseFollowUpItems.Supplier,
                    PurchaserOrder = purchaseFollowUpItems.PurchaserOrder,
                    InsuranceAppliedDate = purchaseFollowUpItems.InsuranceAppliedDate,
                    InsuranceNo = purchaseFollowUpItems.InsuranceNo,
                    InsuranceCertificateDate = purchaseFollowUpItems.InsuranceCertificateDate,
                    FCAppliedDate = purchaseFollowUpItems.FCAppliedDate,
                    FCPermitDate = purchaseFollowUpItems.FCPermitDate,
                    FCPermitNo = purchaseFollowUpItems.FCPermitNo,
                    FCPermitExpiryDate = purchaseFollowUpItems.FCPermitExpiryDate,
                    LCAppliedDate = purchaseFollowUpItems.LCAppliedDate,
                    LCNo = purchaseFollowUpItems.LCNo,
                    LCOpenedDate = purchaseFollowUpItems.LCOpenedDate,
                    Vessel = purchaseFollowUpItems.Vessel,
                    Port = purchaseFollowUpItems.Port,
                    ShippedDate = purchaseFollowUpItems.ShippedDate,
                    VesselArriveDate = purchaseFollowUpItems.VesselArriveDate,
                    BillOfLoading = purchaseFollowUpItems.BillOfLoading,
                    DocumentCollectionDate = purchaseFollowUpItems.DocumentCollectionDate,
                    DocumentSubmitDate = purchaseFollowUpItems.DocumentSubmitDate,
                    Maritimeno = purchaseFollowUpItems.Maritimeno,
                    ArriveDate = purchaseFollowUpItems.ArriveDate,
                    Condition = purchaseFollowUpItems.Condition,
                    Damaged = purchaseFollowUpItems.Damaged,
                    Missing = purchaseFollowUpItems.Missing,
                    ClaimDate = purchaseFollowUpItems.ClaimDate,
                    RefNo = purchaseFollowUpItems.RefNo
                };

                db.PurchaseFollowUpItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseFollowUpItems }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
