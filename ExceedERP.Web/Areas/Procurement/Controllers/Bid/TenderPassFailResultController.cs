using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Bid
{
    public class TenderPassFailResultController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PassFailFinancial([DataSourceRequest]DataSourceRequest request ,string id)
        {
            int lotId = int.Parse(id);
            Core.Domain.Procurement.Tender.Lots.Lot lot = _db.Lots.Find(lotId);
            int items = lot.LotItems.Count; 
            int result = lot.TenderPassFailResults.GroupBy(x => x.ItmCode).Select(x => x.FirstOrDefault()).Count();
            if (items != result)
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Please Fill The Result For All Bid Items !');</script>");            
                
            }
            else
            {
                //foreach (PassFailResult lotTenderPassFailResult in lot.TenderPassFailResults)
                //{
                //    TenderEvaluationCriteria criteria =
                //        db.TenderEvaluationCriterias.FirstOrDefault(
                //            x => x.LotID == lotId && x.Type == CriteriaType.Financial);
                //    //lotTenderPassFailResult.FinancialResult = ComputeFinancial(lotTenderPassFailResult.ItmCode,
                //        criteria.TenderEvaluationCriteriaID, lot.ParticipantsItems.ToList());
                //    db.Entry(lotTenderPassFailResult).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
               
            }
            return RedirectToAction("Index", "Tender");
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult TenderPassFailResult_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            
            IQueryable<PassFailResult> tenderpassfailresult = _db.TenderPassFailResult.Where(x => x.LotID == id);
            DataSourceResult result = tenderpassfailresult.ToDataSourceResult(request, passFailResult => new {
                LotID = passFailResult.LotID,
                PassFailResultId = passFailResult.PassFailResultId,
                LotItemID = passFailResult.LotItemID,
                CatCode = passFailResult.CatCode,
                CategoryName = passFailResult.CategoryName,
                ItmCode = passFailResult.ItmCode,
                ItemName = passFailResult.ItemName,
                ItemSpecification = passFailResult.ItemSpecification,
                Name = passFailResult.Name,
                ModelNo = passFailResult.ModelNo,
                SupplierId = passFailResult.SupplierId,
                IsNewSupplier = passFailResult.IsNewSupplier,
                SupplierName = passFailResult.SupplierName,
                Result = passFailResult.Result,
                Remark = passFailResult.Remark
            });

            return Json(result);
        }
        //public float ComputeFinancial(string itemCode, int criteriaId, List<ParticipantsItem> participantsItems)
        //{
        //    TenderEvaluationCriteria criteria = _db.TenderEvaluationCriterias.Find(criteriaId);
        //    ParticipantsItem participantsItem = participantsItems.FirstOrDefault(x => x.ItmCode  == itemCode);
        //    float lowestPrice = (float)participantsItems.Where(x => x.ItmCode == itemCode).Min(x => x.UnitPrice);
        //    float unitPrice = (float)participantsItem.UnitPrice;
        //    float result = 0;
        //    result = lowestPrice * criteria.Weight / unitPrice;
        //    return result;

        //}

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderPassFailResult_Create([DataSourceRequest]DataSourceRequest request, PassFailResult passFailResult, int id)
        {
            //var evaluationByDocument = db.LotEvaluationByDocuments.Find(int.Parse(passFailResult.SupplierName));//name has purchase id value
            //if (evaluationByDocument != null)
            //{
            //    passFailResult.SupplierId = evaluationByDocument.SupplierId;
            //    passFailResult.IsNewSupplier = evaluationByDocument.IsNewSupplier;
            //    passFailResult.SupplierName = evaluationByDocument.SupplierName;
            //}

            if (passFailResult.LotItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get lot item info

            var lotItem = _db.LotItems.Find(passFailResult.LotItemID);
            if (lotItem != null)
            {
                passFailResult.CatCode = lotItem.CatCode;
                passFailResult.CategoryName = lotItem.CategoryName;
                passFailResult.ItmCode = lotItem.ItmCode;
                passFailResult.ItemName = lotItem.ItemName;
                passFailResult.Name = lotItem.Name;
                passFailResult.ModelNo = lotItem.ModelNo;
                passFailResult.ItemSpecification = lotItem.ItemSpecification;
            }

            #endregion

            #region validation
            var criteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == id)
                .Where(x => x.Type == CriteriaType.Pass_Fail).ToList();
            if (!criteria.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill pass fail result since there is no pass fail criteria.");
            }
            var assigned = _db.TenderPassFailResult
                .Where(x => x.ItmCode == passFailResult.ItmCode)
                .Where(x => x.SupplierName == passFailResult.SupplierName)
                .Where(x => x.LotID == id).ToList();
            if (assigned.Any())
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }

            #endregion

            var existing = _db.TenderPassFailResult.Where(x => x.LotID == id).ToList();

            var participatingSuppliers = _db.BidPParticipatingSupplierses.Where(x => x.LotID == id).ToList();
            var lotitems = _db.LotItems.Where(x => x.LotID == id).ToList();

            if (ModelState.IsValid)
            {
                _db.TenderPassFailResult.RemoveRange(existing);
                _db.SaveChanges();
                foreach (var participating in participatingSuppliers)
                {
                    #region failed suppliers

                    var exist = _db.LotEvaluationByDocuments
                        .Where(x => x.LotID == id)
                        .Where(x => x.SupplierName == participating.SupplierName)
                        .Where(x => x.IsAvailable).ToList();//has missing doc
                    if (exist.Any())
                        continue;

                    #endregion

                    foreach (var items in lotitems)
                    {
                        var entity = new PassFailResult
                        {
                            LotID = id,
                            LotItemID = items.LotItemsID,
                            CatCode = items.CatCode,
                            CategoryName = items.CategoryName,
                            ItmCode = items.ItmCode,
                            ItemName = items.ItemName,
                            Name = passFailResult.Name,
                            ModelNo = passFailResult.ModelNo,
                            ItemSpecification = passFailResult.ItemSpecification,

                            SupplierId = participating.SupplierId,
                            IsNewSupplier = participating.IsNewSupplier,
                            SupplierName = participating.SupplierName,
                            Result = passFailResult.Result,
                            Remark = passFailResult.Remark
                        };

                        _db.TenderPassFailResult.Add(entity);
                        _db.SaveChanges();
                        passFailResult.PassFailResultId = entity.PassFailResultId;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "tender lot pass fail result", "Success", User.Identity.Name, $"created lot pass fail result for {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                        #endregion
                    }
                }
            }

            return Json(new[] { passFailResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderPassFailResult_Update([DataSourceRequest]DataSourceRequest request, PassFailResult passFailResult)
        {
            //var participatingSuppliers = db.BidPParticipatingSupplierses.Find(int.Parse(passFailResult.SupplierName));//name has participant id value
            //if (participatingSuppliers != null)
            //{
            //    passFailResult.SupplierId = participatingSuppliers.SupplierId;
            //    passFailResult.IsNewSupplier = participatingSuppliers.IsNewSupplier;
            //    passFailResult.SupplierName = participatingSuppliers.SupplierName;
            //}

            if (passFailResult.LotItemID == 0)
                ModelState.AddModelError("selectitem", "Please Select an Item.");

            #region get lot item info

            var lotItem = _db.LotItems.Find(passFailResult.LotItemID);
            if (lotItem != null)
            {
                passFailResult.CatCode = lotItem.CatCode;
                passFailResult.CategoryName = lotItem.CategoryName;
                passFailResult.ItmCode = lotItem.ItmCode;
                passFailResult.ItemName = lotItem.ItemName;
                passFailResult.Name = lotItem.Name;
                passFailResult.ModelNo = lotItem.ModelNo;
                passFailResult.ItemSpecification = lotItem.ItemSpecification;
            }
            #endregion
            #region validation
            var criteria = _db.TenderEvaluationCriterias.Where(x => x.LotID == passFailResult.LotID)
                .Where(x => x.Type == CriteriaType.Pass_Fail).ToList();
            if (!criteria.Any())
            {
                ModelState.AddModelError("invalidaCriteria", "You cannot fill pass fail result since there is no pass fail criteria.");
            }
            var assigned = _db.TenderPassFailResult
                .Where(x => x.PassFailResultId != passFailResult.PassFailResultId)
                .Where(x => x.LotItemID == passFailResult.LotItemID)
                .Where(x => x.SupplierName == passFailResult.SupplierName)
                .Where(x => x.LotID == passFailResult.LotID)
                .Where(x => x.LotItemID != 0)
                .ToList();
            if (assigned.Any())
            {
                ModelState.AddModelError("alreadyExist", "You have already assigned this Item for the selected Supplier");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new PassFailResult
                {
                    PassFailResultId = passFailResult.PassFailResultId,
                    LotItemID = passFailResult.LotItemID,
                    CatCode = passFailResult.CatCode,
                    CategoryName = passFailResult.CategoryName,
                    ItmCode = passFailResult.ItmCode,
                    ItemName = passFailResult.ItemName,
                    Name = passFailResult.Name,
                    ModelNo = passFailResult.ModelNo,
                    ItemSpecification = passFailResult.ItemSpecification,
                    LotID = passFailResult.LotID,
                    SupplierId = passFailResult.SupplierId,
                    IsNewSupplier = passFailResult.IsNewSupplier,
                    SupplierName = passFailResult.SupplierName,
                    Result = passFailResult.Result,
                    Remark = passFailResult.Remark
                };

                _db.TenderPassFailResult.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "tender lot pass fail result", "Success", User.Identity.Name, $"updated lot pass fail result for {entity.SupplierName} under record {entity.LotID} ", Modules.Procurement);
                #endregion
            }

            return Json(new[] { passFailResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderPassFailResult_Destroy([DataSourceRequest]DataSourceRequest request, PassFailResult passFailResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new PassFailResult
                {
                    PassFailResultId = passFailResult.PassFailResultId,
                    LotItemID = passFailResult.LotItemID,
                    ItmCode = passFailResult.ItmCode,
                    LotID = passFailResult.LotID,
                    SupplierId = passFailResult.SupplierId,
                    IsNewSupplier = passFailResult.IsNewSupplier,
                    SupplierName = passFailResult.SupplierName,
                    Result = passFailResult.Result,
                    Remark = passFailResult.Remark
                };

                _db.TenderPassFailResult.Attach(entity);
                _db.TenderPassFailResult.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { passFailResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
