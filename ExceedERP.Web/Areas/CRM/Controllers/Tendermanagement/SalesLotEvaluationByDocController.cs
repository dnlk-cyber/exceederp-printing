using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
    public class SalesLotEvaluationByDocController : Controller
    {
        // GET: CRM/LotEvaluationByDoc
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult doc_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var docRes = db.SalesLotEvaluationByDocuments.Where(sli => sli.SalesLotId == id).ToList();
            var result = docRes.ToDataSourceResult(request, docresult => new
            {
                SalesLotId=docresult.SalesLotId,
                TinNumber = docresult.TinNumber,
                CreatedBy = docresult.CreatedBy,
                ModifiedBy = docresult.ModifiedBy,
                LastModified = docresult.LastModified,
                Document = docresult.Document,
                Remark= docresult.Remark,
                IsNotAvailable = docresult.IsNotAvailable,
                SalesLotEvaluationByDocumentId = docresult.SalesLotEvaluationByDocumentId

            });
            return Json(result);
        }
        public ActionResult doc_create([DataSourceRequest]DataSourceRequest request, SalesLotEvaluationByDocument loteval,int id)
        {
            loteval.SalesLotId = id;
            if(ModelState.IsValid)
            {

                var entity = (SalesLotEvaluationByDocument)this.GetMappedDoc(loteval);
                db.SalesLotEvaluationByDocuments.Add(entity);
                db.SaveChanges();
                if(loteval.IsNotAvailable)
                {
                    var fetchParticipant = db.SalesLotParticipants.SingleOrDefault(slp => slp.PurchaseId.ToString() == loteval.TinNumber && slp.SalesLotId == id);
                    if(fetchParticipant!=null)
                    {
                        db.SalesLotParticipants.Remove(fetchParticipant);
                        db.SaveChanges();
                    }
                }
                if (loteval.IsNotAvailable == false)
                {
                    var fetchParticipant = db.SalesLotParticipants.SingleOrDefault(slp => slp.PurchaseId.ToString() == loteval.TinNumber && slp.SalesLotId == id);
                    if (fetchParticipant == null)
                    {
                        var fetchPurchase = db.SalesLotPurchases.SingleOrDefault(slp => slp.SalesLotPurchaseId.ToString() == loteval.TinNumber);
                        if (fetchPurchase != null)
                        {
                            var newParticipant = new SalesLotParticipant();
                            newParticipant.SalesLotParticipantId = newParticipant.SalesLotParticipantId;
                            newParticipant.TinNumber = fetchPurchase.TinNumber;
                            newParticipant.CustomerName = fetchPurchase.CustomerName;
                            newParticipant.Address = fetchPurchase.Address;
                            newParticipant.Representative = fetchPurchase.Representative;
                            newParticipant.ReceiptNo = fetchPurchase.ReceiptNo;
                            newParticipant.ReceiptNotRequired = fetchPurchase.ReceiptNotRequired;
                            newParticipant.PurchaseId = fetchPurchase.SalesLotPurchaseId;
                            newParticipant.Telephone = fetchPurchase.Telephone;
                            newParticipant.PurchaseDate = fetchPurchase.PurchaseDate;
                            newParticipant.SalesLotId = fetchPurchase.SalesLotId;
                            db.SalesLotParticipants.Add(newParticipant);
                            db.SaveChanges();
                        }
                    }
                }
            }

            return Json(new[] { loteval}.ToDataSourceResult(request, ModelState));
        }
        public ActionResult doc_Update([DataSourceRequest]DataSourceRequest request, SalesLotEvaluationByDocument loteval)
        {
            var fetchDoc = db.SalesLotEvaluationByDocuments.SingleOrDefault(ledoc=>ledoc.SalesLotEvaluationByDocumentId == loteval.SalesLotEvaluationByDocumentId);
            fetchDoc.TinNumber = loteval.TinNumber;
            fetchDoc.ModifiedBy = loteval.ModifiedBy;
            fetchDoc.LastModified = loteval.LastModified;
            fetchDoc.Document = loteval.Document;
            fetchDoc.IsNotAvailable = loteval.IsNotAvailable;
            fetchDoc.Remark = loteval.Remark;
            db.Entry(fetchDoc).State = EntityState.Modified;
            db.SaveChanges();
            if (loteval.IsNotAvailable)
            {
                var fetchParticipant = db.SalesLotParticipants.SingleOrDefault(slp => slp.PurchaseId.ToString() == loteval.TinNumber && slp.SalesLotId == loteval.SalesLotId);
                if (fetchParticipant != null)
                {
                    db.SalesLotParticipants.Remove(fetchParticipant);
                    db.SaveChanges();
                }
            }
            if (!loteval.IsNotAvailable)
            {
                var fetchParticipant = db.SalesLotParticipants.SingleOrDefault(slp => slp.PurchaseId.ToString() == loteval.TinNumber && slp.SalesLotId == loteval.SalesLotId);
                if (fetchParticipant == null)
                {
                    var fetchPurchase = db.SalesLotPurchases.SingleOrDefault(slp => slp.SalesLotPurchaseId.ToString() == loteval.TinNumber);
                    if (fetchPurchase != null)
                    {
                        var newParticipant = new SalesLotParticipant();
                        newParticipant.SalesLotParticipantId = newParticipant.SalesLotParticipantId;
                        newParticipant.TinNumber = fetchPurchase.TinNumber;
                        newParticipant.CustomerName = fetchPurchase.CustomerName;
                        newParticipant.Address = fetchPurchase.Address;
                        newParticipant.Representative = fetchPurchase.Representative;
                        newParticipant.ReceiptNo = fetchPurchase.ReceiptNo;
                        newParticipant.ReceiptNotRequired = fetchPurchase.ReceiptNotRequired;
                        newParticipant.PurchaseId = fetchPurchase.SalesLotPurchaseId;
                        newParticipant.Telephone = fetchPurchase.Telephone;
                        newParticipant.PurchaseDate = fetchPurchase.PurchaseDate;
                        newParticipant.SalesLotId = fetchPurchase.SalesLotId;
                        db.SalesLotParticipants.Add(newParticipant);
                        db.SaveChanges();
                    }
                }
            }
            
            return Json(new[] { loteval }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult doc_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotEvaluationByDocument loteval)
        {
            var fetchDoc = db.SalesLotEvaluationByDocuments.SingleOrDefault(ledoc=>ledoc.SalesLotEvaluationByDocumentId==loteval.SalesLotEvaluationByDocumentId);
            if (fetchDoc != null)
            {
                db.SalesLotEvaluationByDocuments.Remove(fetchDoc);
                db.SaveChanges();

            }
            return Json(new[] { loteval }.ToDataSourceResult(request, ModelState));
        }
        private object GetMappedDoc(SalesLotEvaluationByDocument docresult)
        {
            var entity = new SalesLotEvaluationByDocument
            {
                SalesLotId = docresult.SalesLotId,
                TinNumber = docresult.TinNumber,
                CreatedBy = docresult.CreatedBy,
                ModifiedBy = docresult.ModifiedBy,
                LastModified = docresult.LastModified,
                Document = docresult.Document,
                IsNotAvailable = docresult.IsNotAvailable,
                SalesLotEvaluationByDocumentId = docresult.SalesLotEvaluationByDocumentId

            };
            return entity;
        }
    }
}