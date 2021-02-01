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
    public class ArSalesOrderPartialController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderPartials_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderPartial> arSalesOrderPartials = _db.ArSalesOrderPartials.Where(x=>x.ArSalesOrderPartialPrintId==id);
            DataSourceResult result = arSalesOrderPartials.ToDataSourceResult(request, arSalesOrderPartial => new {
                ArSalesOrderPartialPrintId = arSalesOrderPartial.ArSalesOrderPartialPrintId,
                ArSalesOrderPartialId = arSalesOrderPartial.ArSalesOrderPartialId,
                ItemName = arSalesOrderPartial.ItemName,
                SubItem = arSalesOrderPartial.SubItem,
                AccountCode = arSalesOrderPartial.AccountCode,
                AccountDesc = arSalesOrderPartial.AccountDesc,
                UoM = arSalesOrderPartial.UoM,
                Quantity = arSalesOrderPartial.Quantity,
                UnitPrice = arSalesOrderPartial.UnitPrice,
                SalesPrice = arSalesOrderPartial.SalesPrice,
                Discount = arSalesOrderPartial.Discount,
                Commission = arSalesOrderPartial.Commission,
                SubTotal = arSalesOrderPartial.SubTotal,
                GLTaxRateID = arSalesOrderPartial.GLTaxRateID,
                TaxAmount = arSalesOrderPartial.TaxAmount,
                TotalAmount = arSalesOrderPartial.TotalAmount,
                OrgBranchName = arSalesOrderPartial.OrgBranchName,
                DateCreated = arSalesOrderPartial.DateCreated,
                LastModified = arSalesOrderPartial.LastModified,
                CreatedBy = arSalesOrderPartial.CreatedBy,
                ModifiedBy = arSalesOrderPartial.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartials_Create(int id,[DataSourceRequest]DataSourceRequest request, ArSalesOrderPartial arSalesOrderPartial)
        {
            #region get data

            if (!String.IsNullOrWhiteSpace(arSalesOrderPartial.SubItem))
            {
                int lineid = int.Parse(arSalesOrderPartial.SubItem);
                var salesorderpartialprint = _db.ArSalesOrderPartialPrints.Find(id);
                var salesorderline = _db.ArSalesOrderLines.Where(x=>x.ArSalesOrderId==salesorderpartialprint.ArSalesOrderId).FirstOrDefault(x => x.ArSalesOrderLineId==lineid);//has line id
                var partials = _db.ArSalesOrderPartials.Where(x=>x.SubItem==salesorderline.SubItem).Where(x => x.ArSalesOrderPartialPrint.ArSalesOrderId == salesorderpartialprint.ArSalesOrderId);

               
                if (salesorderline != null)
                {
                    decimal orignalQty = salesorderline.Quantity;
                    decimal totalPartial = partials.Sum(x => x.Quantity) + arSalesOrderPartial.Quantity;
                    if(orignalQty<totalPartial)
                        ModelState.AddModelError("extra","Total quantity will be extra by "+ (totalPartial-orignalQty));

                    arSalesOrderPartial.ItemName = salesorderline.ItemName;
                    arSalesOrderPartial.SubItem = salesorderline.SubItem;
                    arSalesOrderPartial.UoM = salesorderline.UoM;
                    arSalesOrderPartial.AccountCode = salesorderline.AccountCode;
                    arSalesOrderPartial.AccountDesc = salesorderline.AccountDesc;
                    arSalesOrderPartial.UnitPrice = salesorderline.UnitPrice;
                    arSalesOrderPartial.GLTaxRateID = salesorderline.GLTaxRateID;
                }
            }
            else
            {
                ModelState.AddModelError("product","Please select product.");
            }
            #endregion
            #region calculate

            arSalesOrderPartial.SalesPrice = arSalesOrderPartial.Quantity * arSalesOrderPartial.UnitPrice;
            arSalesOrderPartial.SubTotal = arSalesOrderPartial.SalesPrice- arSalesOrderPartial.Discount - arSalesOrderPartial.Commission;
            var tax = _db.GLTaxRates.Find(arSalesOrderPartial.GLTaxRateID);
            if (tax != null)
            {
                arSalesOrderPartial.TaxAmount = arSalesOrderPartial.SubTotal * tax.CalculatedRate;
                arSalesOrderPartial.TaxAmount = Math.Round(arSalesOrderPartial.TaxAmount, 2);
            }
            arSalesOrderPartial.TotalAmount = arSalesOrderPartial.TaxAmount + arSalesOrderPartial.SubTotal;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartial
                {
                    ArSalesOrderPartialPrintId = id,
                    ItemName = arSalesOrderPartial.ItemName,
                    SubItem = arSalesOrderPartial.SubItem,
                    AccountCode = arSalesOrderPartial.AccountCode,
                    AccountDesc = arSalesOrderPartial.AccountDesc,
                    UoM = arSalesOrderPartial.UoM,
                    Quantity = arSalesOrderPartial.Quantity,
                    UnitPrice = arSalesOrderPartial.UnitPrice,
                    SalesPrice = arSalesOrderPartial.SalesPrice,
                    Discount = arSalesOrderPartial.Discount,
                    Commission = arSalesOrderPartial.Commission,
                    SubTotal = arSalesOrderPartial.SubTotal,
                    GLTaxRateID = arSalesOrderPartial.GLTaxRateID,
                    TaxAmount = arSalesOrderPartial.TaxAmount,
                    TotalAmount = arSalesOrderPartial.TotalAmount,
                    OrgBranchName = arSalesOrderPartial.OrgBranchName,
                    DateCreated = arSalesOrderPartial.DateCreated,
                    LastModified = arSalesOrderPartial.LastModified,
                    CreatedBy = arSalesOrderPartial.CreatedBy,
                    ModifiedBy = arSalesOrderPartial.ModifiedBy
                };

                _db.ArSalesOrderPartials.Add(entity);
                _db.SaveChanges();
                arSalesOrderPartial.ArSalesOrderPartialId = entity.ArSalesOrderPartialId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order partial print item", "Success", User.Identity.Name, $"created sales order partial print item {entity.ArSalesOrderPartialId} under record {entity.ArSalesOrderPartialPrintId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPartial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartials_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartial arSalesOrderPartial)
        {
            #region get data

            if (!String.IsNullOrWhiteSpace(arSalesOrderPartial.SubItem))
            {
                int lineid = int.Parse(arSalesOrderPartial.SubItem);
                var salesorderpartialprint = _db.ArSalesOrderPartialPrints.Find(arSalesOrderPartial.ArSalesOrderPartialPrintId);
                var salesorderline = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == salesorderpartialprint.ArSalesOrderId).FirstOrDefault(x => x.ArSalesOrderLineId == lineid);//has line id
                var partials = _db.ArSalesOrderPartials.Where(x => x.SubItem == salesorderline.SubItem).Where(x => x.ArSalesOrderPartialPrint.ArSalesOrderId == salesorderpartialprint.ArSalesOrderId);


                if (salesorderline != null)
                {
                    decimal orignalQty = salesorderline.Quantity;
                    decimal totalPartial = partials.Sum(x => x.Quantity) + arSalesOrderPartial.Quantity;
                    if (orignalQty < totalPartial)
                        ModelState.AddModelError("extra", "Total quantity will be extra by " + (totalPartial - orignalQty));

                    arSalesOrderPartial.ItemName = salesorderline.ItemName;
                    arSalesOrderPartial.SubItem = salesorderline.SubItem;
                    arSalesOrderPartial.UoM = salesorderline.UoM;
                    arSalesOrderPartial.AccountCode = salesorderline.AccountCode;
                    arSalesOrderPartial.AccountDesc = salesorderline.AccountDesc;
                    arSalesOrderPartial.UnitPrice = salesorderline.UnitPrice;
                    arSalesOrderPartial.GLTaxRateID = salesorderline.GLTaxRateID;
                }
            }
            else
            {
                ModelState.AddModelError("product", "Please select product.");
            }
            #endregion
            #region calculate

            arSalesOrderPartial.SalesPrice = arSalesOrderPartial.Quantity * arSalesOrderPartial.UnitPrice;
            arSalesOrderPartial.SubTotal = arSalesOrderPartial.SalesPrice - arSalesOrderPartial.Discount - arSalesOrderPartial.Commission;
            var tax = _db.GLTaxRates.Find(arSalesOrderPartial.GLTaxRateID);
            if (tax != null)
            {
                arSalesOrderPartial.TaxAmount = arSalesOrderPartial.SubTotal * tax.CalculatedRate;
                arSalesOrderPartial.TaxAmount = Math.Round(arSalesOrderPartial.TaxAmount, 2);
            }
            arSalesOrderPartial.TotalAmount = arSalesOrderPartial.TaxAmount + arSalesOrderPartial.SubTotal;
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartial
                {
                    ArSalesOrderPartialPrintId = arSalesOrderPartial.ArSalesOrderPartialPrintId,
                    ArSalesOrderPartialId = arSalesOrderPartial.ArSalesOrderPartialId,
                    ItemName = arSalesOrderPartial.ItemName,
                    SubItem = arSalesOrderPartial.SubItem,
                    AccountCode = arSalesOrderPartial.AccountCode,
                    AccountDesc = arSalesOrderPartial.AccountDesc,
                    UoM = arSalesOrderPartial.UoM,
                    Quantity = arSalesOrderPartial.Quantity,
                    UnitPrice = arSalesOrderPartial.UnitPrice,
                    SalesPrice = arSalesOrderPartial.SalesPrice,
                    Discount = arSalesOrderPartial.Discount,
                    Commission = arSalesOrderPartial.Commission,
                    SubTotal = arSalesOrderPartial.SubTotal,
                    GLTaxRateID = arSalesOrderPartial.GLTaxRateID,
                    TaxAmount = arSalesOrderPartial.TaxAmount,
                    TotalAmount = arSalesOrderPartial.TotalAmount,
                    OrgBranchName = arSalesOrderPartial.OrgBranchName,
                    DateCreated = arSalesOrderPartial.DateCreated,
                    LastModified = arSalesOrderPartial.LastModified,
                    CreatedBy = arSalesOrderPartial.CreatedBy,
                    ModifiedBy = arSalesOrderPartial.ModifiedBy
                };

                _db.ArSalesOrderPartials.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order partial print item", "Success", User.Identity.Name, $"updated sales order partial print item {entity.ArSalesOrderPartialId} under record {entity.ArSalesOrderPartialPrintId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderPartial }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderPartials_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderPartial arSalesOrderPartial)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderPartial
                {
                    ArSalesOrderPartialPrintId = arSalesOrderPartial.ArSalesOrderPartialPrintId,
                    ArSalesOrderPartialId = arSalesOrderPartial.ArSalesOrderPartialId,
                    ItemName = arSalesOrderPartial.ItemName,
                    AccountCode = arSalesOrderPartial.AccountCode,
                    AccountDesc = arSalesOrderPartial.AccountDesc,
                    UoM = arSalesOrderPartial.UoM,
                    Quantity = arSalesOrderPartial.Quantity,
                    UnitPrice = arSalesOrderPartial.UnitPrice,
                    SalesPrice = arSalesOrderPartial.SalesPrice,
                    Discount = arSalesOrderPartial.Discount,
                    Commission = arSalesOrderPartial.Commission,
                    SubTotal = arSalesOrderPartial.SubTotal,
                    GLTaxRateID = arSalesOrderPartial.GLTaxRateID,
                    TaxAmount = arSalesOrderPartial.TaxAmount,
                    TotalAmount = arSalesOrderPartial.TotalAmount,
                    OrgBranchName = arSalesOrderPartial.OrgBranchName,
                    DateCreated = arSalesOrderPartial.DateCreated,
                    LastModified = arSalesOrderPartial.LastModified,
                    CreatedBy = arSalesOrderPartial.CreatedBy,
                    ModifiedBy = arSalesOrderPartial.ModifiedBy
                };

                _db.ArSalesOrderPartials.Attach(entity);
                _db.ArSalesOrderPartials.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderPartial }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
