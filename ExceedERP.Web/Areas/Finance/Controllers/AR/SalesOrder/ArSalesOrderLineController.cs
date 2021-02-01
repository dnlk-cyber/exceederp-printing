using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderLineController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderLine> arsalesorderlines = _db.ArSalesOrderLines.Where(x=>x.ArSalesOrderId==id);
            DataSourceResult result = arsalesorderlines.ToDataSourceResult(request, arSalesOrderLine => new {
                ArSalesOrderId = arSalesOrderLine.ArSalesOrderId,
                ArSalesOrderLineId = arSalesOrderLine.ArSalesOrderLineId,
                ItemName = arSalesOrderLine.ItemName,
                SubItem = arSalesOrderLine.SubItem,
                AccountCode = arSalesOrderLine.AccountCode,
                AccountDesc = arSalesOrderLine.AccountDesc,
                UoM = arSalesOrderLine.UoM,
                Quantity = arSalesOrderLine.Quantity,
                UnitPrice = arSalesOrderLine.UnitPrice,
                SalesPrice = arSalesOrderLine.SalesPrice,
                Discount = arSalesOrderLine.Discount,
                Commission = arSalesOrderLine.Commission,
                SubTotal = arSalesOrderLine.SubTotal,
                GLTaxRateID = arSalesOrderLine.GLTaxRateID,
                TaxAmount = arSalesOrderLine.TaxAmount,
                TotalAmount = arSalesOrderLine.TotalAmount,
                Remark = arSalesOrderLine.Remark,
                OrgBranchName = arSalesOrderLine.OrgBranchName,
                DateCreated = arSalesOrderLine.DateCreated,
                LastModified = arSalesOrderLine.LastModified,
                CreatedBy = arSalesOrderLine.CreatedBy,
                ModifiedBy = arSalesOrderLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderLine arSalesOrderLine)
        {
            #region calculate

            arSalesOrderLine.SalesPrice = arSalesOrderLine.Quantity * arSalesOrderLine.UnitPrice;
            arSalesOrderLine.SubTotal = arSalesOrderLine.SalesPrice- arSalesOrderLine.Discount - arSalesOrderLine.Commission;
            var tax = _db.GLTaxRates.Find(arSalesOrderLine.GLTaxRateID);
            if (tax != null)
            {
                arSalesOrderLine.TaxAmount = arSalesOrderLine.SubTotal * tax.CalculatedRate;
                arSalesOrderLine.TaxAmount = Math.Round(arSalesOrderLine.TaxAmount, 2);
            }
            arSalesOrderLine.TotalAmount = arSalesOrderLine.TaxAmount + arSalesOrderLine.SubTotal;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderLine
                {
                    ArSalesOrderId = id,
                    ItemName = arSalesOrderLine.ItemName,
                    SubItem = arSalesOrderLine.SubItem,
                    AccountCode = arSalesOrderLine.AccountCode,
                    AccountDesc = arSalesOrderLine.AccountDesc,
                    UoM = arSalesOrderLine.UoM,
                    Quantity = arSalesOrderLine.Quantity,
                    UnitPrice = arSalesOrderLine.UnitPrice,
                    SalesPrice = arSalesOrderLine.SalesPrice,
                    Discount = arSalesOrderLine.Discount,
                    Commission = arSalesOrderLine.Commission,
                    SubTotal = arSalesOrderLine.SubTotal,
                    GLTaxRateID = arSalesOrderLine.GLTaxRateID,
                    TaxAmount = arSalesOrderLine.TaxAmount,
                    TotalAmount = arSalesOrderLine.TotalAmount,
                    Remark = arSalesOrderLine.Remark,
                    OrgBranchName = arSalesOrderLine.OrgBranchName,
                    DateCreated = arSalesOrderLine.DateCreated,
                    LastModified = arSalesOrderLine.LastModified,
                    CreatedBy = arSalesOrderLine.CreatedBy,
                    ModifiedBy = arSalesOrderLine.ModifiedBy
                };

                _db.ArSalesOrderLines.Add(entity);
                _db.SaveChanges();
                arSalesOrderLine.ArSalesOrderLineId = entity.ArSalesOrderLineId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order item", "Success", User.Identity.Name, $"created sales order item {entity.ItemName} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderLines_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderLine arSalesOrderLine)
        {
            #region calculate

            arSalesOrderLine.SalesPrice = arSalesOrderLine.Quantity * arSalesOrderLine.UnitPrice;
            arSalesOrderLine.SubTotal = arSalesOrderLine.SalesPrice - arSalesOrderLine.Discount - arSalesOrderLine.Commission;
            var tax = _db.GLTaxRates.Find(arSalesOrderLine.GLTaxRateID);
            if (tax != null)
            {
                arSalesOrderLine.TaxAmount = arSalesOrderLine.SubTotal * tax.CalculatedRate;
                arSalesOrderLine.TaxAmount = Math.Round(arSalesOrderLine.TaxAmount, 2);
            }
            arSalesOrderLine.TotalAmount = arSalesOrderLine.TaxAmount + arSalesOrderLine.SubTotal;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderLine
                {
                    ArSalesOrderId = arSalesOrderLine.ArSalesOrderId,
                    ArSalesOrderLineId = arSalesOrderLine.ArSalesOrderLineId,
                    ItemName = arSalesOrderLine.ItemName,
                    SubItem = arSalesOrderLine.SubItem,
                    AccountCode = arSalesOrderLine.AccountCode,
                    AccountDesc = arSalesOrderLine.AccountDesc,
                    UoM = arSalesOrderLine.UoM,
                    Quantity = arSalesOrderLine.Quantity,
                    UnitPrice = arSalesOrderLine.UnitPrice,
                    SalesPrice = arSalesOrderLine.SalesPrice,
                    Discount = arSalesOrderLine.Discount,
                    Commission = arSalesOrderLine.Commission,
                    SubTotal = arSalesOrderLine.SubTotal,
                    GLTaxRateID = arSalesOrderLine.GLTaxRateID,
                    TaxAmount = arSalesOrderLine.TaxAmount,
                    TotalAmount = arSalesOrderLine.TotalAmount,
                    Remark = arSalesOrderLine.Remark,
                    OrgBranchName = arSalesOrderLine.OrgBranchName,
                    DateCreated = arSalesOrderLine.DateCreated,
                    LastModified = arSalesOrderLine.LastModified,
                    CreatedBy = arSalesOrderLine.CreatedBy,
                    ModifiedBy = arSalesOrderLine.ModifiedBy
                };

                _db.ArSalesOrderLines.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order item", "Success", User.Identity.Name, $"updated sales order item {entity.ItemName} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderLines_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderLine arSalesOrderLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderLine
                {
                    ArSalesOrderId = arSalesOrderLine.ArSalesOrderId,
                    ArSalesOrderLineId = arSalesOrderLine.ArSalesOrderLineId,
                    ItemName = arSalesOrderLine.ItemName,
                    AccountCode = arSalesOrderLine.AccountCode,
                    AccountDesc = arSalesOrderLine.AccountDesc,
                    UoM = arSalesOrderLine.UoM,
                    Quantity = arSalesOrderLine.Quantity,
                    UnitPrice = arSalesOrderLine.UnitPrice,
                    SalesPrice = arSalesOrderLine.SalesPrice,
                    Discount = arSalesOrderLine.Discount,
                    Commission = arSalesOrderLine.Commission,
                    SubTotal = arSalesOrderLine.SubTotal,
                    GLTaxRateID = arSalesOrderLine.GLTaxRateID,
                    TaxAmount = arSalesOrderLine.TaxAmount,
                    TotalAmount = arSalesOrderLine.TotalAmount,
                    Remark = arSalesOrderLine.Remark,
                    OrgBranchName = arSalesOrderLine.OrgBranchName,
                    DateCreated = arSalesOrderLine.DateCreated,
                    LastModified = arSalesOrderLine.LastModified,
                    CreatedBy = arSalesOrderLine.CreatedBy,
                    ModifiedBy = arSalesOrderLine.ModifiedBy
                };

                _db.ArSalesOrderLines.Attach(entity);
                _db.ArSalesOrderLines.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
