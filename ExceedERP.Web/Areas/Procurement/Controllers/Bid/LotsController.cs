using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{

    public class LotsController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Lots_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<Core.Domain.Procurement.Tender.Lots.Lot> lots = _db.Lots.Where(t => t.TenderID == id);
            DataSourceResult result = lots.ToDataSourceResult(request, lot => new
            {
                LotID = lot.LotID,
                TenderID = lot.TenderID,
                LotNumber = lot.LotNumber,
                Category = lot.Category,
                Description = lot.Description,
                EvaluateTotalFinance = lot.EvaluateTotalFinance,
                OrgBranchName = lot.OrgBranchName,
                DateCreated = lot.DateCreated,
                LastModified = lot.LastModified,
                CreatedBy = lot.CreatedBy,
                ModifiedBy = lot.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lots_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Tender.Lots.Lot lot, int id)
        {
            #region validation

            if (lot.LotNumber<=0)
            {
                ModelState.AddModelError("", lot.LotNumber + " is not valid number.");
            }
            var exists = _db.Lots.Where(x => x.LotNumber == lot.LotNumber)
                .Where(x => x.Category == lot.Category)// OCC--- cat and lot number refer the same lot
                .Where(x => x.TenderID == id).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", lot.LotNumber + " already exists in the tender system.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new Core.Domain.Procurement.Tender.Lots.Lot
                    {
                        LotNumber = lot.LotNumber,
                        TenderID = id,
                        Category = lot.Category,
                        Description = lot.Description,
                        OrgBranchName = lot.OrgBranchName,
                        EvaluateTotalFinance = lot.EvaluateTotalFinance,
                        DateCreated = DateTime.Now.Date,
                        LastModified = lot.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = lot.ModifiedBy
                    };

                    _db.Lots.Add(entity);
                    _db.SaveChanges();
                    lot.LotID = entity.LotID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot", "Success", User.Identity.Name, $"created lot {entity.LotID} under record {entity.TenderID} ", Modules.Procurement);
                #endregion


            }

            return Json(new[] { lot }.ToDataSourceResult(request, ModelState));
        }

       

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lots_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Tender.Lots.Lot lot)
        {
            #region validation

            if (lot.LotNumber <= 0)
            {
                ModelState.AddModelError("", lot.LotNumber + " is not valid number.");
            }

            var exists = _db.Lots.Where(x => x.LotID != lot.LotID)
                .Where(x => x.LotNumber == lot.LotNumber)
                .Where(x => x.Category == lot.Category)// OCC--- cat and lot number refer the same lot
                .Where(x => x.TenderID == lot.TenderID).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("", lot.LotNumber + " already exists in the tender system.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new Core.Domain.Procurement.Tender.Lots.Lot
                    {
                        LotID = lot.LotID,
                        TenderID = lot.TenderID,
                        Category = lot.Category,
                        LotNumber = lot.LotNumber,
                        Description = lot.Description,
                        EvaluateTotalFinance = lot.EvaluateTotalFinance,
                        OrgBranchName = lot.OrgBranchName,
                        DateCreated = lot.DateCreated,
                        LastModified = lot.LastModified,
                        CreatedBy = lot.CreatedBy,
                        ModifiedBy = lot.ModifiedBy
                    };

                    _db.Lots.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot", "Success", User.Identity.Name, $"updated lot {entity.LotID} under record {entity.TenderID} ", Modules.Procurement);
                #endregion


            }

            return Json(new[] { lot }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lots_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.Tender.Lots.Lot lot)
        {
            #region validation
            var exists = _db.LotItems.Where(x => x.LotID == lot.LotID).ToList();
            var exists1 = _db.BidPurchasingSupplierses.Where(x => x.LotID == lot.LotID).ToList();
            var exists2 = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lot.LotID).ToList();
            var exists3 = _db.LotEvaluationByDocuments.Where(x => x.LotID == lot.LotID).ToList();
            var exists4 = _db.TenderEvaluationCriterias.Where(x => x.LotID == lot.LotID).ToList();
            var exists5 = _db.TenderTechnicals.Where(x => x.LotID == lot.LotID).ToList();
            var exists6 = _db.TenderPassFailResult.Where(x => x.LotID == lot.LotID).ToList();
            var exists7 = _db.BidParticipatingSupplierItems.Where(x => x.BidPParticipatingSupplier.LotID == lot.LotID).ToList();
            var exists8 = _db.TenderEvaluationResults.Where(x => x.LotID == lot.LotID).ToList();
            var exists9 = _db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();

            if (exists9.Any() || exists7.Any() || exists8.Any() || exists5.Any() || exists6.Any() || exists3.Any() || exists4.Any() || exists1.Any() || exists2.Any() || exists.Any())
            {
                ModelState.AddModelError("", $" Lot {lot.LotNumber } can not be deleted ,it has items.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.Tender.Lots.Lot
                {
                    LotID = lot.LotID,
                    LotNumber = lot.LotNumber,
                    Category = lot.Category,
                    Description = lot.Description,
                    OrgBranchName = lot.OrgBranchName,
                    DateCreated = lot.DateCreated,
                    LastModified = lot.LastModified,
                    CreatedBy = lot.CreatedBy,
                    ModifiedBy = lot.ModifiedBy
                };

                _db.Lots.Attach(entity);
                _db.Lots.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { lot }.ToDataSourceResult(request, ModelState));
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
