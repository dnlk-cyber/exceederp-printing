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
    public class TenderTechnicalController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult TenderTechnicals_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TenderTechnical> tendertechnical = _db.TenderTechnicals.Where(x => x.LotID == id);
            DataSourceResult result = tendertechnical.ToDataSourceResult(request, tenderTechnical => new
            {
                TenderTechnicalId = tenderTechnical.TenderTechnicalId,
                LotItemID = tenderTechnical.LotItemID,
                SupplierId = tenderTechnical.SupplierId,
                IsNewSupplier = tenderTechnical.IsNewSupplier,
                SupplierName = tenderTechnical.SupplierName,
                CatCode = tenderTechnical.CatCode,
                CategoryName = tenderTechnical.CategoryName,
                ItmCode = tenderTechnical.ItmCode,
                ItemName = tenderTechnical.ItemName,
                ItemSpecification = tenderTechnical.ItemSpecification,
                Name = tenderTechnical.Name,
                ModelNo = tenderTechnical.ModelNo,
                LotID = tenderTechnical.LotID,
                Result = tenderTechnical.Result,
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTechnicals_Create([DataSourceRequest]DataSourceRequest request, TenderTechnical tenderTechnical, int id)
        {

            //var bidPParticipatingSuppliers = db.BidPParticipatingSupplierses.Find(int.Parse(tenderTechnical.SupplierName));//name has participant id value
            //if (bidPParticipatingSuppliers != null)
            //{
            //    tenderTechnical.SupplierId = bidPParticipatingSuppliers.SupplierId;
            //    tenderTechnical.IsNewSupplier = bidPParticipatingSuppliers.IsNewSupplier;
            //    tenderTechnical.SupplierName = bidPParticipatingSuppliers.SupplierName;
            //}

            if (tenderTechnical.LotItemID==0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get lot item info

            var lotItem = _db.LotItems.Find(tenderTechnical.LotItemID);
            if (lotItem != null)
            {
                tenderTechnical.CatCode = lotItem.CatCode;
                tenderTechnical.CategoryName = lotItem.CategoryName;
                tenderTechnical.ItmCode = lotItem.ItmCode;
                tenderTechnical.ItemName = lotItem.ItemName;
                tenderTechnical.Name = lotItem.Name;
                tenderTechnical.ModelNo = lotItem.ModelNo;
                tenderTechnical.ItemSpecification = lotItem.ItemSpecification;
            }

            #endregion
            #region validation

            var type = _db.TenderEvaluationCriterias.Where(x => x.LotID == id).Where(x => x.Type == CriteriaType.Technical).ToList();

            if (!type.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill technical result since there is no technical criteria.");
            }
            var criteria = _db.TenderEvaluationCriterias.FirstOrDefault(x => x.LotID == id && x.Type == CriteriaType.Technical);
            if (tenderTechnical.Result > criteria?.Weight)
            {
                ModelState.AddModelError("aboveWeight", "The result cannot be above the criteria weight " + criteria.Weight);
            }

            if (_db.TenderTechnicals.Any(x => x.ItmCode == tenderTechnical.ItmCode && x.SupplierName == tenderTechnical.SupplierName && x.LotID == id))
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }



            #endregion
            var existing = _db.TenderTechnicals.Where(x => x.LotID == id).ToList();
            var participatingSuppliers = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).ToList();
            var lotitems = _db.LotItems.Where(x => x.LotID == id).ToList();

            if (ModelState.IsValid)
            {
                _db.TenderTechnicals.RemoveRange(existing);
                _db.SaveChanges();
                foreach (var participating in participatingSuppliers)
                {
                    foreach (var items in lotitems)
                    {
                        var entity = new TenderTechnical
                        {
                            LotID = id,
                            LotItemID = items.LotItemsID,
                            SupplierId = participating.SupplierId,
                            IsNewSupplier = participating.IsNewSupplier,
                            SupplierName = participating.SupplierName,
                            CatCode = items.CatCode,
                            CategoryName = items.CategoryName,
                            ItmCode = items.ItmCode,
                            ItemName = items.ItemName,
                            ItemSpecification=items.ItemSpecification,
                            Name = tenderTechnical.Name,
                            ModelNo = tenderTechnical.ModelNo,
                            Result = tenderTechnical.Result,
                        };

                        _db.TenderTechnicals.Add(entity);
                        _db.SaveChanges();
                        tenderTechnical.TenderTechnicalId = entity.TenderTechnicalId;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "tender lot technical", "Success", User.Identity.Name, $"created lot technical for {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                        #endregion
                    }
                }
            }

            return Json(new[] { tenderTechnical }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTechnicals_Update([DataSourceRequest]DataSourceRequest request, TenderTechnical tenderTechnical)
        {
            //var participatingSuppliers = db.BidPParticipatingSupplierses.Find(int.Parse(tenderTechnical.SupplierName));//name has participant id value
            //if (participatingSuppliers != null)
            //{
            //    tenderTechnical.SupplierId = participatingSuppliers.SupplierId;
            //    tenderTechnical.IsNewSupplier = participatingSuppliers.IsNewSupplier;
            //    tenderTechnical.SupplierName = participatingSuppliers.SupplierName;
            //}

            if (tenderTechnical.LotItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get lot item info

            var lotItem = _db.LotItems.Find(tenderTechnical.LotItemID);
            if (lotItem != null)
            {
                tenderTechnical.CatCode = lotItem.CatCode;
                tenderTechnical.CategoryName = lotItem.CategoryName;
                tenderTechnical.ItmCode = lotItem.ItmCode;
                tenderTechnical.ItemName = lotItem.ItemName;
                tenderTechnical.Name = lotItem.Name;
                tenderTechnical.ModelNo = lotItem.ModelNo;
                tenderTechnical.ItemSpecification = lotItem.ItemSpecification;
            }

            #endregion

            #region validation

            var type = _db.TenderEvaluationCriterias.Where(x => x.LotID == tenderTechnical.LotID).Where(x => x.Type == CriteriaType.Technical).ToList();

            if (!type.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill technical result since there is no technical criteria.");
            }
            var criteria = _db.TenderEvaluationCriterias.FirstOrDefault(x => x.LotID == tenderTechnical.LotID && x.Type == CriteriaType.Technical);
            if (tenderTechnical.Result > criteria?.Weight)
            {
                ModelState.AddModelError("aboveWeight", "The result cannot be above the criteria weight " + criteria.Weight);
            }

            var assigned = _db.TenderTechnicals
                .Where(x =>x.LotItemID == tenderTechnical.LotItemID)
                .Where(x=>x.SupplierName == tenderTechnical.SupplierName )
                .Where(x=>x.LotID == tenderTechnical.LotID)
                .Where(x=>x.TenderTechnicalId != tenderTechnical.TenderTechnicalId)
                .Where(x => x.LotItemID != 0)
                .ToList();

            if (assigned.Any())
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }


            #endregion

            if (ModelState.IsValid)
            {
                var entity = new TenderTechnical
                {
                    TenderTechnicalId = tenderTechnical.TenderTechnicalId,
                    LotID = tenderTechnical.LotID,
                    LotItemID = tenderTechnical.LotItemID,
                    SupplierId = tenderTechnical.SupplierId,
                    IsNewSupplier = tenderTechnical.IsNewSupplier,
                    SupplierName = tenderTechnical.SupplierName,
                    
                    CatCode = tenderTechnical.CatCode,
                    CategoryName = tenderTechnical.CategoryName,
                    ItmCode = tenderTechnical.ItmCode,
                    ItemName = tenderTechnical.ItemName,
                    Name = tenderTechnical.Name,
                    ItemSpecification = tenderTechnical.ItemSpecification,
                    ModelNo = tenderTechnical.ModelNo,
                    Result = tenderTechnical.Result,
                };

                _db.TenderTechnicals.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot technical", "Success", User.Identity.Name, $"updated lot technical for {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { tenderTechnical }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderTechnicals_Destroy([DataSourceRequest]DataSourceRequest request, TenderTechnical tenderTechnical)
        {
            if (ModelState.IsValid)
            {
                var entity = new TenderTechnical
                {
                    TenderTechnicalId = tenderTechnical.TenderTechnicalId,
                    LotItemID = tenderTechnical.LotItemID,
                    SupplierId = tenderTechnical.SupplierId,
                    IsNewSupplier = tenderTechnical.IsNewSupplier,
                    SupplierName = tenderTechnical.SupplierName,
                    LotID = tenderTechnical.LotID,
                    ItmCode = tenderTechnical.ItmCode,
                    Result = tenderTechnical.Result,
                };

                _db.TenderTechnicals.Attach(entity);
                _db.TenderTechnicals.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { tenderTechnical }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
