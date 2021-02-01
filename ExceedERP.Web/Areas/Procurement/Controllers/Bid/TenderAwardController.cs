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


    public class TenderAwardController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }




        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult BidAwardedSuppliers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BidAwardedSupplier> bidawardedsuppliers = _db.BidAwardedSuppliers.Where(l => l.LotID == id);
            DataSourceResult result = bidawardedsuppliers.ToDataSourceResult(request, bidAwardedSupplier => new
            {
                BidAwardedSupplierID = bidAwardedSupplier.BidAwardedSupplierID,
                LotID = bidAwardedSupplier.LotID,
                TenderReference = bidAwardedSupplier.TenderReference,
                SupplierId = bidAwardedSupplier.SupplierId,
                IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                SupplierName = bidAwardedSupplier.SupplierName,
                DeliveryDate = bidAwardedSupplier.DeliveryDate,
                AwardDate = bidAwardedSupplier.AwardDate,
                AwardGiven = bidAwardedSupplier.AwardGiven,
                Remark = bidAwardedSupplier.Remark,
                IsTransferredToContract = bidAwardedSupplier.IsTransferredToContract,
                OrgBranchName = bidAwardedSupplier.OrgBranchName,
                DateCreated = bidAwardedSupplier.DateCreated,
                LastModified = bidAwardedSupplier.LastModified,
                CreatedBy = bidAwardedSupplier.CreatedBy,
                ModifiedBy = bidAwardedSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSuppliers_Create([DataSourceRequest]DataSourceRequest request, BidAwardedSupplier bidAwardedSupplier, int id)
        {
            var participatingSuppliers = _db.BidPParticipatingSupplierses.FirstOrDefault(x => x.SupplierName == bidAwardedSupplier.SupplierName);//name has participant id value
            if (participatingSuppliers != null)
            {
                bidAwardedSupplier.SupplierId = participatingSuppliers.SupplierId;
                bidAwardedSupplier.IsNewSupplier = participatingSuppliers.IsNewSupplier;
                bidAwardedSupplier.SupplierName = participatingSuppliers.SupplierName;
            }
            //if (db.BidAwardedSuppliers.Any(x => x.SupplierName == bidAwardedSupplier.SupplierName && x.LotID == id))
            //{
            //    ModelState.AddModelError("already", "You have already assigned this winning supplier.");
            //}

            var lot = _db.Lots.Find(id);
            bidAwardedSupplier.TenderReference = lot?.Tender.TenderReference;
            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplier
                {
                    LotID = id,
                    TenderReference = bidAwardedSupplier.TenderReference,
                    SupplierId = bidAwardedSupplier.SupplierId,
                    IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                    SupplierName = bidAwardedSupplier.SupplierName,
                    DeliveryDate = bidAwardedSupplier.DeliveryDate,
                    AwardDate = bidAwardedSupplier.AwardDate,
                    AwardGiven = bidAwardedSupplier.AwardGiven,
                    Remark = bidAwardedSupplier.Remark,
                    IsTransferredToContract = bidAwardedSupplier.IsTransferredToContract,
                    OrgBranchName = bidAwardedSupplier.OrgBranchName,
                    LastModified = bidAwardedSupplier.LastModified,
                    CreatedBy = User.Identity.Name,
                    DateCreated=DateTime.Now,
                    ModifiedBy = bidAwardedSupplier.ModifiedBy
                };

                _db.BidAwardedSuppliers.Add(entity);
                _db.SaveChanges();
                bidAwardedSupplier.BidAwardedSupplierID = entity.BidAwardedSupplierID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot award", "Success", User.Identity.Name, $"created lot award for {entity.SupplierName} under record {entity.TenderReference} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] { bidAwardedSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSuppliers_Update([DataSourceRequest]DataSourceRequest request, BidAwardedSupplier bidAwardedSupplier)
        {
            var participatingSuppliers = _db.BidPParticipatingSupplierses.FirstOrDefault(x => x.SupplierName==bidAwardedSupplier.SupplierName);//name has participant id value
            if (participatingSuppliers != null)
            {
                bidAwardedSupplier.SupplierId = participatingSuppliers.SupplierId;
                bidAwardedSupplier.IsNewSupplier = participatingSuppliers.IsNewSupplier;
                bidAwardedSupplier.SupplierName = participatingSuppliers.SupplierName;
            }
            //if (db.BidAwardedSuppliers.Any(x => x.BidAwardedSupplierID != bidAwardedSupplier.BidAwardedSupplierID && x.LotID == bidAwardedSupplier.LotID && x.SupplierName == bidAwardedSupplier.SupplierName))
            //{
            //    ModelState.AddModelError("already", "You have already assigned this winning supplier.");

            //}
            var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
            bidAwardedSupplier.TenderReference = lot?.Tender.TenderReference;
            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplier
                {
                    BidAwardedSupplierID = bidAwardedSupplier.BidAwardedSupplierID,
                    SupplierId = bidAwardedSupplier.SupplierId,
                    IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                    SupplierName = bidAwardedSupplier.SupplierName,
                    LotID = bidAwardedSupplier.LotID,
                    TenderReference = bidAwardedSupplier.TenderReference,
                    DeliveryDate = bidAwardedSupplier.DeliveryDate,
                    AwardDate = bidAwardedSupplier.AwardDate,
                    AwardGiven = bidAwardedSupplier.AwardGiven,
                    Remark = bidAwardedSupplier.Remark,
                    IsTransferredToContract = bidAwardedSupplier.IsTransferredToContract,
                    OrgBranchName = bidAwardedSupplier.OrgBranchName,
                    DateCreated = bidAwardedSupplier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = bidAwardedSupplier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.BidAwardedSuppliers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot award", "Success", User.Identity.Name, $"updated lot award for {entity.SupplierName} under record {entity.TenderReference} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { bidAwardedSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BidAwardedSuppliers_Destroy([DataSourceRequest]DataSourceRequest request, BidAwardedSupplier bidAwardedSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new BidAwardedSupplier
                {
                    BidAwardedSupplierID = bidAwardedSupplier.BidAwardedSupplierID,
                    SupplierId = bidAwardedSupplier.SupplierId,
                    IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                    SupplierName = bidAwardedSupplier.SupplierName,
                    AwardDate = bidAwardedSupplier.AwardDate,
                    AwardGiven = bidAwardedSupplier.AwardGiven,
                    Remark = bidAwardedSupplier.Remark,
                    IsTransferredToContract = bidAwardedSupplier.IsTransferredToContract,
                    OrgBranchName = bidAwardedSupplier.OrgBranchName,
                    DateCreated = bidAwardedSupplier.DateCreated,
                    LastModified = bidAwardedSupplier.LastModified,
                    CreatedBy = bidAwardedSupplier.CreatedBy,
                    ModifiedBy = bidAwardedSupplier.ModifiedBy
                };

                _db.BidAwardedSuppliers.Attach(entity);
                _db.BidAwardedSuppliers.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { bidAwardedSupplier }.ToDataSourceResult(request, ModelState));
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
