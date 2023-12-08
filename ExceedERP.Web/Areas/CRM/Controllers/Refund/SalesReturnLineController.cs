using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.CRM;
using System;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesReturnUser, CRMRoles.SalesReturnApprover)]
    public class SalesReturnLineController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        public ActionResult SalesReturnLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SalesReturnLine> crmdepositline = db.SalesReturnLines.Where(l => l.SalesReturnId == id);
            DataSourceResult result = crmdepositline.ToDataSourceResult(request, salesReturnLine => new {
                SalesReturnLineId = salesReturnLine.SalesReturnLineId,
                SalesReturnId = salesReturnLine.SalesReturnId,
                PaymentType = salesReturnLine.PaymentType,
                SalesReference = salesReturnLine.SalesReference,
                ShipmentReference = salesReturnLine.ShipmentReference,
                ProductId = salesReturnLine.ProductId,
                UnitPrice = salesReturnLine.UnitPrice,
                Quantity = salesReturnLine.Quantity,
                SubTotal = salesReturnLine.SubTotal,
                TaxTypeId = salesReturnLine.TaxTypeId,
                TaxAmount = salesReturnLine.TaxAmount,
                TotalAmount = salesReturnLine.TotalAmount,
                Remark = salesReturnLine.Remark,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturnLine_Create([DataSourceRequest]DataSourceRequest request, SalesReturnLine salesReturnLine, int id)
        {
            //if(DuplicateLineItemExists(salesReturnLine, id))
            //{
            //    ModelState.AddModelError("", "Duplicate line item not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new SalesReturnLine
                {
                    SalesReturnLineId = salesReturnLine.SalesReturnLineId,
                    SalesReturnId = id,
                    PaymentType = salesReturnLine.PaymentType,
                    SalesReference = salesReturnLine.SalesReference,
                    ShipmentReference = salesReturnLine.ShipmentReference,
                    ProductId = salesReturnLine.ProductId,
                    UnitPrice = salesReturnLine.UnitPrice,
                    Quantity = salesReturnLine.Quantity,
                    SubTotal = salesReturnLine.SubTotal,
                    TaxTypeId = salesReturnLine.TaxTypeId,
                    TaxAmount = salesReturnLine.TaxAmount,
                    TotalAmount = salesReturnLine.TotalAmount,
                    Remark = salesReturnLine.Remark,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };
                entity.SubTotal = entity.UnitPrice * entity.Quantity;
                entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
                entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

                db.SalesReturnLines.Add(entity);
                db.SaveChanges();
                salesReturnLine.SalesReturnLineId = entity.SalesReturnLineId;
				UpdateHeader(entity.SalesReturnId);
			}

			return Json(new[] { salesReturnLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturnLine_Update([DataSourceRequest]DataSourceRequest request, SalesReturnLine salesReturnLine)
        {
            //if (DuplicateLineItemExists(salesReturnLine, salesReturnLine.SalesReturnId))
            //{
            //    ModelState.AddModelError("", "Duplicate line item not allowed");
            //}
            if (ModelState.IsValid)
            {
                var entity = new SalesReturnLine
                {
                    SalesReturnLineId = salesReturnLine.SalesReturnLineId,
                    SalesReturnId = salesReturnLine.SalesReturnId,
                    PaymentType = salesReturnLine.PaymentType,
                    SalesReference = salesReturnLine.SalesReference,
                    ShipmentReference = salesReturnLine.ShipmentReference,
                    ProductId = salesReturnLine.ProductId,
                    UnitPrice = salesReturnLine.UnitPrice,
                    Quantity = salesReturnLine.Quantity,
                    SubTotal = salesReturnLine.SubTotal,
                    TaxTypeId = salesReturnLine.TaxTypeId,
                    TaxAmount = salesReturnLine.TaxAmount,
                    TotalAmount = salesReturnLine.TotalAmount,
                    Remark = salesReturnLine.Remark,
                    
                    DateCreated = salesReturnLine.DateCreated,
                    LastModified = salesReturnLine.LastModified,
                    CreatedBy = salesReturnLine.CreatedBy,
                    ModifiedBy = salesReturnLine.ModifiedBy
                };
                entity.SubTotal = entity.UnitPrice * entity.Quantity;
                entity.TaxAmount = CRMSalesHelper.CalculateTaxAmount(entity.TaxTypeId, entity.SubTotal);
                entity.TotalAmount = entity.SubTotal + entity.TaxAmount;

                db.SalesReturnLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

				UpdateHeader(entity.SalesReturnId);
			}

			return Json(new[] { salesReturnLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesReturnLine_Destroy([DataSourceRequest]DataSourceRequest request, SalesReturnLine salesReturnLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new SalesReturnLine
                {
                    SalesReturnLineId = salesReturnLine.SalesReturnLineId,
                    SalesReturnId = salesReturnLine.SalesReturnId,
                    PaymentType = salesReturnLine.PaymentType,
                    SalesReference = salesReturnLine.SalesReference,
                    ShipmentReference = salesReturnLine.ShipmentReference,
                    ProductId = salesReturnLine.ProductId,
                    UnitPrice = salesReturnLine.UnitPrice,
                    Quantity = salesReturnLine.Quantity,
                    SubTotal = salesReturnLine.SubTotal,
                    TaxTypeId = salesReturnLine.TaxTypeId,
                    TaxAmount = salesReturnLine.TaxAmount,
                    TotalAmount = salesReturnLine.TotalAmount,
                    Remark = salesReturnLine.Remark,

                    DateCreated = salesReturnLine.DateCreated,
                    LastModified = salesReturnLine.LastModified,
                    CreatedBy = salesReturnLine.CreatedBy,
                    ModifiedBy = salesReturnLine.ModifiedBy
                };

                db.SalesReturnLines.Attach(entity);
                db.SalesReturnLines.Remove(entity);
                db.SaveChanges();

				UpdateHeader(entity.SalesReturnId);
            }

            return Json(new[] { salesReturnLine }.ToDataSourceResult(request, ModelState));
        }
        //private bool DuplicateLineItemExists(SalesReturnLine line, int headerId)
        //{
        //    return 
        //        db.SalesReturnLines
        //        .Any(l => l.SalesReturnLineId != line.SalesReturnLineId && 
        //            l.SalesReturnId == headerId && 
        //            l.ReceiveMethodId == line.ReceiveMethodId &&
        //            l.Reference == line.Reference);
        //}
        //private CRMDeposit GetDepositHeader(int id)
        //{
        //    return db.CRMDeposit.Find(id);
        //}
		private void UpdateHeader(int id)
		{
			var salesReturn = db.SalesReturn.Find(id);
			if(salesReturn != null)
			{
				var lines =
					db.SalesReturnLines
					.Where(l => l.SalesReturnId == salesReturn.SalesReturnId)
					.ToList();

				decimal amount = lines.Any()
					? lines.Sum(l => l.TotalAmount)
					: 0;
				salesReturn.TotalAmount = amount;

				db.Entry(salesReturn).State = EntityState.Modified;
				db.SaveChanges();
			}
		}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
