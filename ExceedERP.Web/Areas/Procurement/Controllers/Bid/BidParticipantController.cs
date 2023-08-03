using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{


    public class BidParticipantController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult BidPParticipatingSupplierses_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BidPParticipatingSuppliers> bidpparticipatingsupplierses = _db.BidPParticipatingSupplierses.Where(l => l.LotID == id);
            DataSourceResult result = bidpparticipatingsupplierses.ToDataSourceResult(request, bidPParticipatingSuppliers => new
            {
                BidPParticipatingSuppliersID = bidPParticipatingSuppliers.BidPParticipatingSuppliersID,
                SupplierId = bidPParticipatingSuppliers.SupplierId,
                IsNewSupplier = bidPParticipatingSuppliers.IsNewSupplier,
                SupplierName = bidPParticipatingSuppliers.SupplierName,
                LotID = bidPParticipatingSuppliers.LotID,
                Representative = bidPParticipatingSuppliers.Representative,
                Address = bidPParticipatingSuppliers.Address,
                Telephone = bidPParticipatingSuppliers.Telephone,
                BidSubmissionDate = bidPParticipatingSuppliers.BidSubmissionDate,
                BidSubmissionTime = bidPParticipatingSuppliers.BidSubmissionTime,
                NumberOfDocument = bidPParticipatingSuppliers.NumberOfDocument,
                Remark = bidPParticipatingSuppliers.Remark,
                OrgBranchName = bidPParticipatingSuppliers.OrgBranchName,
                DateCreated = bidPParticipatingSuppliers.DateCreated,
                LastModified = bidPParticipatingSuppliers.LastModified,
                CreatedBy = bidPParticipatingSuppliers.CreatedBy,
                ModifiedBy = bidPParticipatingSuppliers.ModifiedBy,
                IsQualified = bidPParticipatingSuppliers.IsQualified
            });

            return Json(result);
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPParticipatingSupplierses_Create([DataSourceRequest]DataSourceRequest request, BidPParticipatingSuppliers bidPParticipatingSuppliers, int id)
        {

            var bidpurchase = _db.BidPurchasingSupplierses.FirstOrDefault(x=>x.SupplierName==bidPParticipatingSuppliers.SupplierName);
            if (bidpurchase != null)
            {
                bidPParticipatingSuppliers.SupplierId = bidpurchase.SupplierId;
                bidPParticipatingSuppliers.IsNewSupplier = bidpurchase.IsNewSupplier;
            }

            var isExist =_db.BidPParticipatingSupplierses.Any(x =>x.LotID == id &&x.SupplierName == bidPParticipatingSuppliers.SupplierName);
            if (isExist)
            {
                ModelState.AddModelError("alreadyExit", "You has already added this supplier to the participant list!");
            }

            var existing = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).ToList();
            var bidPurchasingSuppliers = _db.BidPurchasingSupplierses.Where(x => x.LotID == id).ToList();

            if (ModelState.IsValid)
            {
                _db.BidPParticipatingSupplierses.RemoveRange(existing);
                _db.SaveChanges();
                foreach (var purchasingSuppliers in bidPurchasingSuppliers)
                {
                    var entity = new BidPParticipatingSuppliers
                    {
                        LotID = id,
                        SupplierId = purchasingSuppliers.SupplierId,
                        IsNewSupplier = purchasingSuppliers.IsNewSupplier,
                        SupplierName = purchasingSuppliers.SupplierName,
                        Representative = purchasingSuppliers.Representative,
                        Address = purchasingSuppliers.Address,
                        Telephone = purchasingSuppliers.Telephone,
                        BidSubmissionDate = bidPParticipatingSuppliers.BidSubmissionDate,
                        BidSubmissionTime = bidPParticipatingSuppliers.BidSubmissionTime,
                        NumberOfDocument = bidPParticipatingSuppliers.NumberOfDocument,
                        Remark = bidPParticipatingSuppliers.Remark,
                        OrgBranchName = bidPParticipatingSuppliers.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = bidPParticipatingSuppliers.LastModified,
                        IsQualified = bidPParticipatingSuppliers.IsQualified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = bidPParticipatingSuppliers.ModifiedBy
                    };

                    _db.BidPParticipatingSupplierses.Add(entity);
                    _db.SaveChanges();
                    bidPParticipatingSuppliers.BidPParticipatingSuppliersID = entity.BidPParticipatingSuppliersID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "lot bid participant", "Success", User.Identity.Name, $"created bid partifipant {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { bidPParticipatingSuppliers }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPParticipatingSupplierses_Update([DataSourceRequest]DataSourceRequest request, BidPParticipatingSuppliers bidPParticipatingSuppliers)
        {
            var bidpurchase = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName == bidPParticipatingSuppliers.SupplierName);
            if (bidpurchase != null)
            {
                bidPParticipatingSuppliers.SupplierId = bidpurchase.SupplierId;
                bidPParticipatingSuppliers.IsNewSupplier = bidpurchase.IsNewSupplier;
            }

            var isExist =
                _db.BidPParticipatingSupplierses.Any(
                    x =>
                        x.LotID == bidPParticipatingSuppliers.LotID &&
                        x.SupplierName == bidPParticipatingSuppliers.SupplierName && x.BidPParticipatingSuppliersID != bidPParticipatingSuppliers.BidPParticipatingSuppliersID);
            if (isExist)
            {
                ModelState.AddModelError("alreadyExit", "You has already added this supplier to the participant list!");
            }
            if (ModelState.IsValid)
            {
                var entity = new BidPParticipatingSuppliers
                {
                    LotID = bidPParticipatingSuppliers.LotID,
                    BidPParticipatingSuppliersID = bidPParticipatingSuppliers.BidPParticipatingSuppliersID,
                    SupplierId = bidPParticipatingSuppliers.SupplierId,
                    IsNewSupplier = bidPParticipatingSuppliers.IsNewSupplier,
                    SupplierName = bidPParticipatingSuppliers.SupplierName,
                    Representative = bidPParticipatingSuppliers.Representative,
                    Address = bidPParticipatingSuppliers.Address,
                    Telephone = bidPParticipatingSuppliers.Telephone,
                    BidSubmissionDate = bidPParticipatingSuppliers.BidSubmissionDate,
                    BidSubmissionTime = bidPParticipatingSuppliers.BidSubmissionTime,
                    NumberOfDocument = bidPParticipatingSuppliers.NumberOfDocument,
                    Remark = bidPParticipatingSuppliers.Remark,
                    IsQualified = bidPParticipatingSuppliers.IsQualified,
                    OrgBranchName = bidPParticipatingSuppliers.OrgBranchName,
                    DateCreated = bidPParticipatingSuppliers.DateCreated,
                    LastModified = bidPParticipatingSuppliers.LastModified,
                    CreatedBy = bidPParticipatingSuppliers.CreatedBy,
                    ModifiedBy = bidPParticipatingSuppliers.ModifiedBy
                };

                _db.BidPParticipatingSupplierses.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "lot bid participant", "Success", User.Identity.Name, $"updated bid partifipant {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidPParticipatingSuppliers }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidPParticipatingSupplierses_Destroy([DataSourceRequest]DataSourceRequest request, BidPParticipatingSuppliers bidPParticipatingSuppliers)
        {
            if (ModelState.IsValid)
            {
                var entity = new BidPParticipatingSuppliers
                {
                    BidPParticipatingSuppliersID = bidPParticipatingSuppliers.BidPParticipatingSuppliersID,
                    SupplierId = bidPParticipatingSuppliers.SupplierId,
                    IsNewSupplier = bidPParticipatingSuppliers.IsNewSupplier,
                    SupplierName = bidPParticipatingSuppliers.SupplierName,
                    Representative = bidPParticipatingSuppliers.Representative,
                    Address = bidPParticipatingSuppliers.Address,
                    Telephone = bidPParticipatingSuppliers.Telephone,
                    BidSubmissionDate = bidPParticipatingSuppliers.BidSubmissionDate,
                    BidSubmissionTime = bidPParticipatingSuppliers.BidSubmissionTime,
                    NumberOfDocument = bidPParticipatingSuppliers.NumberOfDocument,
                    IsQualified = bidPParticipatingSuppliers.IsQualified,
                    Remark = bidPParticipatingSuppliers.Remark,
                    OrgBranchName = bidPParticipatingSuppliers.OrgBranchName,
                    DateCreated = bidPParticipatingSuppliers.DateCreated,
                    LastModified = bidPParticipatingSuppliers.LastModified,
                    CreatedBy = bidPParticipatingSuppliers.CreatedBy,
                    ModifiedBy = bidPParticipatingSuppliers.ModifiedBy
                };

                _db.BidPParticipatingSupplierses.Attach(entity);
                _db.BidPParticipatingSupplierses.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { bidPParticipatingSuppliers }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetBidParticipant(int id)
        {
            var participatingSupplier = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).AsQueryable();
           
            return Json(participatingSupplier.Select(i => new
            {
                Supplier = i.SupplierName,
                SupplierCode = i.BidPParticipatingSuppliersID
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBidPrequalified(int id)
        {
            List<BidPParticipatingSuppliers> supplierlist = new List<BidPParticipatingSuppliers>();

            var participating = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).ToList();

            supplierlist.AddRange(participating);

            foreach (var supplier in participating)
            {
                var exist = _db.LotEvaluationByDocuments
                    .Where(x => x.LotID == id)
                    .Where(x => x.SupplierName == supplier.SupplierName)
                    .Where(x => x.IsAvailable).ToList();//has missing doc
                if (exist.Any())
                    supplierlist.Remove(supplier);
                else
                {
                    var existFailed = _db.TenderPassFailResult
                        .Where(x => x.LotID == id)
                        .Where(x => x.SupplierName == supplier.SupplierName)
                        .Where(x => x.Result == PassFailStatus.Fail);
                    if (existFailed.Any())
                        supplierlist.Remove(supplier);
                }
            }
           

            return Json(supplierlist.Select(i => new
            {
                Supplier = i.SupplierName,
                SupplierCode = i.BidPParticipatingSuppliersID
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetBidResultSuppliers(int id)
        {
            List<BidPParticipatingSuppliers> supplierlist = new List<BidPParticipatingSuppliers>();

            var participating = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).ToList();

            supplierlist.AddRange(participating);

            foreach (var supplier in participating)
            {
                var exist = _db.LotEvaluationByDocuments
                    .Where(x => x.LotID == id)
                    .Where(x => x.SupplierName == supplier.SupplierName)
                    .Where(x => x.IsAvailable).ToList();//has missing doc
                if (exist.Any())
                    supplierlist.Remove(supplier);
                else
                {
                    //if not available in result remove
                    var existInResult = _db.TenderEvaluationResults
                        .Where(x => x.LotID == id)
                        .FirstOrDefault(x => x.SupplierName == supplier.SupplierName);

                    if(existInResult==null)
                        supplierlist.Remove(supplier);
                }
            }


            return Json(supplierlist.Select(i => new
            {
                Supplier = i.SupplierName,
                SupplierCode = i.SupplierName
            }), JsonRequestBehavior.AllowGet);

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
