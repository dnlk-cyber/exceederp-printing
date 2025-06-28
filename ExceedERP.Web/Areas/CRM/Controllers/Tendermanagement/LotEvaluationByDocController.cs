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
    public class LotEvaluationByDocController : Controller
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
                LotEvaluationByDocumentId = docresult.SalesLotEvaluationByDocumentId

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
                
            }

            return Json(new[] { loteval}.ToDataSourceResult(request, ModelState));
        }
        public ActionResult doc_Update([DataSourceRequest]DataSourceRequest request, SalesLotEvaluationByDocument loteval)
        {
            var fetchDoc = db.SalesLotEvaluationByDocuments.SingleOrDefault(ledoc=>ledoc.SalesLotId==loteval.SalesLotEvaluationByDocumentId);
            fetchDoc.TinNumber = loteval.TinNumber;
            fetchDoc.ModifiedBy = loteval.ModifiedBy;
            fetchDoc.LastModified = loteval.LastModified;
            fetchDoc.Document = loteval.Document;
            fetchDoc.IsNotAvailable = loteval.IsNotAvailable;
            fetchDoc.Remark = loteval.Remark;
            db.Entry(fetchDoc).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new[] { loteval }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult doc_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotEvaluationByDocument loteval)
        {
            var fetchDoc = db.SalesLotEvaluationByDocuments.SingleOrDefault(ledoc=>ledoc.SalesLotId==loteval.SalesLotEvaluationByDocumentId);
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