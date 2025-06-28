using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class SalesLotPurchasesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["saleslot"] = db.SalesLots.ToList();

            return View();
        }
        
        public ActionResult SalesLotPurchases_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<SalesLotPurchase> salesLots= db.SalesLotPurchases.Where(x=>x.SalesLotId==id);
            DataSourceResult result = salesLots.ToDataSourceResult(request, salesLot => new {
                SalesLotPurchaseId = salesLot.SalesLotPurchaseId,
                SalesLotId = salesLot.SalesLotId,
                LotName=salesLot.LotName,
                PurchaseDate = salesLot.PurchaseDate,
                ReceiptNo = salesLot.ReceiptNo,
                ReceiptNotRequired = salesLot.ReceiptNotRequired,               
                CustomerName = salesLot.CustomerName,
                TinNumber=salesLot.TinNumber,
                Representative = salesLot.Representative,
                Telephone = salesLot.Telephone,
                Address = salesLot.Address,
                Remark = salesLot.Remark,
                OrgBranchName = salesLot.OrgBranchName,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified
            });

            return Json(result);
        }
        public ActionResult SalesLotPurchases_Create([DataSourceRequest]DataSourceRequest request, SalesLotPurchase salesLot,int id)
        {

            var lotIdCheck = db.SalesLots.SingleOrDefault(p => p.SalesLotId == id);
            var dateCheck = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == lotIdCheck.SaleTenderId);
            var checkRepetion = db.SalesLotPurchases.SingleOrDefault(p => p.SalesLotId == salesLot.SalesLotId && p.TinNumber == salesLot.TinNumber);
            //if ((dateCheck.EvaluationDate > salesLot.PurchaseDate && dateCheck.FloatDate <= salesLot.PurchaseDate)&&checkRepetion==null)
            //{
            if ((dateCheck.EvaluationDate < salesLot.PurchaseDate  && dateCheck.FloatDate > salesLot.PurchaseDate) && checkRepetion != null)
                ModelState.AddModelError("PurchaseDate", "Purchase Date should be greater than or equal to Evaluation Date and less than or equal to Float Date");

            if (!salesLot.ReceiptNotRequired && string.IsNullOrEmpty(salesLot.ReceiptNo))
                ModelState.AddModelError("receipt","Receipt is required");

            if (ModelState.IsValid)
                {
                    salesLot.DateCreated = DateTime.Now;
                    salesLot.CreatedBy = User.Identity.Name;
                    salesLot.LotName = lotIdCheck.LotName;
                    var entity = GetMappedObject(salesLot,id);
                    db.SalesLotPurchases.Add(entity);
                    db.SaveChanges();
                    salesLot.SalesLotPurchaseId = entity.SalesLotPurchaseId;
                }
            

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotPurchases_Update([DataSourceRequest]DataSourceRequest request, SalesLotPurchase salesLot,int id)
        {
            var lotIdCheck = db.SalesLots.SingleOrDefault(p => p.SalesLotId == id);
            var dateCheck = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == lotIdCheck.SaleTenderId);
            var checkRepetion = db.SalesLotPurchases.SingleOrDefault(p => p.SalesLotId == salesLot.SalesLotId && p.TinNumber == salesLot.TinNumber);

            if ((dateCheck.EvaluationDate < salesLot.PurchaseDate && dateCheck.FloatDate > salesLot.PurchaseDate) && checkRepetion != null)
                ModelState.AddModelError("PurchaseDate", "Purchase Date should be greater than or equal to Evaluation Date and less than or equal to Float Date");

            if (!salesLot.ReceiptNotRequired && string.IsNullOrEmpty(salesLot.ReceiptNo))
                ModelState.AddModelError("receipt", "Receipt is required");
            if (ModelState.IsValid)
            {
                salesLot.LastModified = DateTime.Now;
                salesLot.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(salesLot,id);
                db.SalesLotPurchases.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotPurchases_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotPurchase salesLot,int id)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(salesLot,id);
                db.SalesLotPurchases.Attach(entity);
                db.SalesLotPurchases.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { salesLot }.ToDataSourceResult(request, ModelState));
        }
        private SalesLotPurchase GetMappedObject(SalesLotPurchase salesLot,int id)
        {
            var entity = new SalesLotPurchase
            {
                SalesLotPurchaseId = salesLot.SalesLotPurchaseId,
                SalesLotId = id,
                LotName=salesLot.LotName,
                PurchaseDate = salesLot.PurchaseDate,
                ReceiptNo = salesLot.ReceiptNo,
                ReceiptNotRequired = salesLot.ReceiptNotRequired,
                Representative = salesLot.Representative,
                CustomerName = salesLot.CustomerName,
                TinNumber = salesLot.TinNumber,
                Telephone = salesLot.Telephone,
                Address = salesLot.Address,
                Remark = salesLot.Remark,
                OrgBranchName = salesLot.OrgBranchName,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified,

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
