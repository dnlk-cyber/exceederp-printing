using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class BidPurchaseController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult BidPurchasingSupplierses_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BidPurchasingSuppliers> bidpurchasingsupplierses = _db.BidPurchasingSupplierses.Where(l => l.LotID == id);
            DataSourceResult result = bidpurchasingsupplierses.ToDataSourceResult(request, bidPurchasingSuppliers => new
            {
                BidPurchasingSuppliersID = bidPurchasingSuppliers.BidPurchasingSuppliersID,
                SupplierId = bidPurchasingSuppliers.SupplierId,
                IsNewSupplier = bidPurchasingSuppliers.IsNewSupplier,
                SupplierName = bidPurchasingSuppliers.SupplierName,
                LotID = bidPurchasingSuppliers.LotID,
                Representative = bidPurchasingSuppliers.Representative,
                PurchaseDate = bidPurchasingSuppliers.PurchaseDate,
                ReceiptNo = bidPurchasingSuppliers.ReceiptNo,
                ReceiptNotRequired = bidPurchasingSuppliers.ReceiptNotRequired,
                Address = bidPurchasingSuppliers.Address,
                Telephone = bidPurchasingSuppliers.Telephone,
                Remark = bidPurchasingSuppliers.Remark,
                OrgBranchName = bidPurchasingSuppliers.OrgBranchName,
                DateCreated = bidPurchasingSuppliers.DateCreated,
                LastModified = bidPurchasingSuppliers.LastModified,
                CreatedBy = bidPurchasingSuppliers.CreatedBy,
                ModifiedBy = bidPurchasingSuppliers.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPurchasingSupplierses_Create([DataSourceRequest]DataSourceRequest request, BidPurchasingSuppliers bidPurchasingSuppliers, int id)
        {
            #region validation
            //reset wrong data
            if (bidPurchasingSuppliers.IsNewSupplier)
            {
                bidPurchasingSuppliers.SupplierId = "";
            }

            if (!bidPurchasingSuppliers.IsNewSupplier)
            {
                bidPurchasingSuppliers.SupplierName = "";
                string supplierid = bidPurchasingSuppliers.SupplierId;
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == supplierid);
                if (supplier != null) bidPurchasingSuppliers.SupplierName = supplier.Name;
            }

            if (!bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierId))
                ModelState.AddModelError("invalid", "Please select a supplier.");

            if (bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierName))
                ModelState.AddModelError("invalid", "Please enter supplier name.");

            var lot = _db.Lots.Find(id);
            if (lot != null)
            {
                var tender = lot.Tender;
               

                if (tender.TenderType == TenderType.Selected)
                {
                    string supplierid = bidPurchasingSuppliers.SupplierId;
                    var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == supplierid);
                    if (supplier != null && !supplier.IsForShortList)
                            ModelState.AddModelError("invalid", "Supplier is not found in short list.");

                }
               

                if (tender.TenderType == TenderType.Open)
                {
                    if (!String.IsNullOrWhiteSpace(bidPurchasingSuppliers.ReceiptNo))
                    {
                        var receiptexists = _db.BidPurchasingSupplierses
                            .Where(x=>x.LotID==id)
                            .Where(x => x.ReceiptNo.ToUpper() == bidPurchasingSuppliers.ReceiptNo.ToUpper()).ToList();

                        if (receiptexists.Any())
                            ModelState.AddModelError("Receipt No.",
                                bidPurchasingSuppliers.ReceiptNo + " is already exists in the system.");

                        if (!bidPurchasingSuppliers.ReceiptNotRequired)
                        {
                            var receiptexistsinAr = _db.ARMiscelaneousCollections.Where(x =>
                                x.ReceiptNumber.ToUpper() == bidPurchasingSuppliers.ReceiptNo.ToUpper()).ToList();

                            if (!receiptexistsinAr.Any())
                                ModelState.AddModelError("Receipt No. not valid",
                                    bidPurchasingSuppliers.ReceiptNo + " is not found in the system.");
                        }
                    }
                    if (string.IsNullOrWhiteSpace(bidPurchasingSuppliers.ReceiptNo) && ! bidPurchasingSuppliers.ReceiptNotRequired)
                    {
                        ModelState.AddModelError("Receipt","Please fill Receipt No or letter reference.");
                    }
                }
            }


            var supplierExistisInLot = _db.BidPurchasingSupplierses.Any(x => x.LotID == id && x.SupplierName == bidPurchasingSuppliers.SupplierName);
            if (supplierExistisInLot)
                ModelState.AddModelError("Supplier", "This Supplier has already purchased the Bid!");




            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new BidPurchasingSuppliers
                    {
                        LotID = id,
                        SupplierId = bidPurchasingSuppliers.SupplierId,
                        IsNewSupplier = bidPurchasingSuppliers.IsNewSupplier,
                        SupplierName = bidPurchasingSuppliers.SupplierName,
                        Representative = bidPurchasingSuppliers.Representative,
                        PurchaseDate = bidPurchasingSuppliers.PurchaseDate,
                        ReceiptNo = bidPurchasingSuppliers.ReceiptNo,
                        ReceiptNotRequired = bidPurchasingSuppliers.ReceiptNotRequired,
                        Address = bidPurchasingSuppliers.Address,
                        Telephone = bidPurchasingSuppliers.Telephone,
                        Remark = bidPurchasingSuppliers.Remark,
                        OrgBranchName = bidPurchasingSuppliers.OrgBranchName,
                        LastModified = bidPurchasingSuppliers.LastModified,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now.Date,
                        ModifiedBy = bidPurchasingSuppliers.ModifiedBy
                    };

                    _db.BidPurchasingSupplierses.Add(entity);
                    _db.SaveChanges();
                    bidPurchasingSuppliers.BidPurchasingSuppliersID = entity.BidPurchasingSuppliersID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot bid document purchase", "Success", User.Identity.Name, $"created bid purchase by {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { bidPurchasingSuppliers }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPurchasingSupplierses_Update([DataSourceRequest]DataSourceRequest request, BidPurchasingSuppliers bidPurchasingSuppliers)
        {
            #region validation
            //reset wrong data
            if (bidPurchasingSuppliers.IsNewSupplier)
            {
                bidPurchasingSuppliers.SupplierId = "";
            }

            if (!bidPurchasingSuppliers.IsNewSupplier)
            {
                bidPurchasingSuppliers.SupplierName = "";
                string supplierid = bidPurchasingSuppliers.SupplierId;
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == supplierid);
                if (supplier != null) bidPurchasingSuppliers.SupplierName = supplier.Name;
            }

            if (!bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierId))
                ModelState.AddModelError("invalid", "Please select a supplier.");

            if (bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierName))
                ModelState.AddModelError("invalid", "Please enter supplier name.");

            var lot = _db.Lots.Find(bidPurchasingSuppliers.LotID);
            if (lot != null)
            {
                var tender = lot.Tender;
                string supplierid = bidPurchasingSuppliers.SupplierId;
                var supplier = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == supplierid);
                if (supplier != null) bidPurchasingSuppliers.SupplierName = supplier.Name;

                if (tender.TenderType == TenderType.Selected)
                {
                    if (supplier != null && !supplier.IsForShortList)
                        ModelState.AddModelError("invalid", "Supplier is not found in short list.");

                }
                else
                {
                    if (!bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierId))
                        ModelState.AddModelError("invalid", "Please select a supplier.");
                    else
                    {
                        if (bidPurchasingSuppliers.IsNewSupplier && String.IsNullOrEmpty(bidPurchasingSuppliers.SupplierName))
                            ModelState.AddModelError("invalid", "Please enter supplier name.");

                    }
                }

                if (tender.TenderType == TenderType.Open)
                {
                    if (!String.IsNullOrWhiteSpace(bidPurchasingSuppliers.ReceiptNo))
                    {
                        var receiptexists = _db.BidPurchasingSupplierses
                            .Where(x => x.LotID == bidPurchasingSuppliers.LotID)
                            .Where(x=>x.BidPurchasingSuppliersID!=bidPurchasingSuppliers.BidPurchasingSuppliersID)
                            .Where(x => x.ReceiptNo.ToUpper() == bidPurchasingSuppliers.ReceiptNo.ToUpper()).ToList();

                        if (receiptexists.Any())
                            ModelState.AddModelError("Receipt No.",
                                bidPurchasingSuppliers.ReceiptNo + " is already exists in the system.");

                        if (!bidPurchasingSuppliers.ReceiptNotRequired)
                        {
                            var receiptexistsinAr = _db.ARMiscelaneousCollections.Where(x =>
                                x.ReceiptNumber.ToUpper() == bidPurchasingSuppliers.ReceiptNo.ToUpper()).ToList();

                            if (!receiptexistsinAr.Any())
                                ModelState.AddModelError("Receipt No. not valid",
                                    bidPurchasingSuppliers.ReceiptNo + " is not found in the system.");
                        }
                    }
                    if (string.IsNullOrWhiteSpace(bidPurchasingSuppliers.ReceiptNo) && !bidPurchasingSuppliers.ReceiptNotRequired)
                    {
                        ModelState.AddModelError("Receipt", "Please fill Receipt No or letter reference.");
                    }
                }
            }


            var supplierExistisInLot = _db.BidPurchasingSupplierses
                .Where(x=>x.BidPurchasingSuppliersID!= bidPurchasingSuppliers.BidPurchasingSuppliersID)
                .Any(x => x.LotID == bidPurchasingSuppliers.LotID && x.SupplierName == bidPurchasingSuppliers.SupplierName);
            if (supplierExistisInLot)
                ModelState.AddModelError("Supplier", "This Supplier has already purchased the Bid!");




            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new BidPurchasingSuppliers
                    {
                        LotID = bidPurchasingSuppliers.LotID,
                        BidPurchasingSuppliersID = bidPurchasingSuppliers.BidPurchasingSuppliersID,
                        SupplierId = bidPurchasingSuppliers.SupplierId,
                        IsNewSupplier = bidPurchasingSuppliers.IsNewSupplier,
                        SupplierName = bidPurchasingSuppliers.SupplierName,
                        Representative = bidPurchasingSuppliers.Representative,
                        PurchaseDate = bidPurchasingSuppliers.PurchaseDate,
                        ReceiptNo = bidPurchasingSuppliers.ReceiptNo,
                        ReceiptNotRequired = bidPurchasingSuppliers.ReceiptNotRequired,
                        Address = bidPurchasingSuppliers.Address,
                        Telephone = bidPurchasingSuppliers.Telephone,
                        Remark = bidPurchasingSuppliers.Remark,
                        OrgBranchName = bidPurchasingSuppliers.OrgBranchName,
                        DateCreated = bidPurchasingSuppliers.DateCreated,
                        LastModified = bidPurchasingSuppliers.LastModified,
                        CreatedBy = bidPurchasingSuppliers.CreatedBy,
                        ModifiedBy = bidPurchasingSuppliers.ModifiedBy
                    };

                    _db.BidPurchasingSupplierses.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot bid document purchase", "Success", User.Identity.Name, $"updated bid purchase by {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { bidPurchasingSuppliers }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPurchasingSupplierses_Destroy([DataSourceRequest]DataSourceRequest request, BidPurchasingSuppliers bidPurchasingSuppliers)
        {
            if (ModelState.IsValid)
            {
                var entity = new BidPurchasingSuppliers
                {
                    BidPurchasingSuppliersID = bidPurchasingSuppliers.BidPurchasingSuppliersID,
                    SupplierId = bidPurchasingSuppliers.SupplierId,
                    IsNewSupplier = bidPurchasingSuppliers.IsNewSupplier,
                    SupplierName = bidPurchasingSuppliers.SupplierName,
                    Representative = bidPurchasingSuppliers.Representative,
                    PurchaseDate = bidPurchasingSuppliers.PurchaseDate,
                    ReceiptNo = bidPurchasingSuppliers.ReceiptNo,
                    Address = bidPurchasingSuppliers.Address,
                    Telephone = bidPurchasingSuppliers.Telephone,
                    Remark = bidPurchasingSuppliers.Remark,
                    OrgBranchName = bidPurchasingSuppliers.OrgBranchName,
                    DateCreated = bidPurchasingSuppliers.DateCreated,
                    LastModified = bidPurchasingSuppliers.LastModified,
                    CreatedBy = bidPurchasingSuppliers.CreatedBy,
                    ModifiedBy = bidPurchasingSuppliers.ModifiedBy
                };

                _db.BidPurchasingSupplierses.Attach(entity);
                _db.BidPurchasingSupplierses.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { bidPurchasingSuppliers }.ToDataSourceResult(request, ModelState));
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

        public JsonResult GetBidPurchase(string id)
        {
            int lotid = int.Parse(id);
            var failedPrequalification=_db.LotEvaluationByDocuments
                .Where(x => x.LotID == lotid)
                .Where(x=>x.IsAvailable)//IsAvailable is not available
                .Select(x=>x.SupplierName);

            var persons = _db.BidPurchasingSupplierses
                .Where(x => x.LotID == lotid)
                .Where(x => failedPrequalification.Contains(x.SupplierName)==false)
                .AsQueryable();

            return Json(persons.Select(i => new
                {
                    Supplier = i.SupplierName,
                    SupplierValue = i.SupplierName
                }), 
                JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
