using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceLinesController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARInvoiceLines_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArInvoiceLines> arinvoicelines = _db.ARInvoiceLines.Where(x=>x.ArInvoiceId==id);
            DataSourceResult result = arinvoicelines.ToDataSourceResult(request, aRInvoiceLines => new {
                ArInvoiceId = aRInvoiceLines.ArInvoiceId,
                ArInvoiceLinesId = aRInvoiceLines.ArInvoiceLinesId,
                Items = aRInvoiceLines.Items,
                Description = aRInvoiceLines.Description,
                AccountCode = aRInvoiceLines.AccountCode,
                AccountDesc = aRInvoiceLines.AccountDesc,
                Quantity = aRInvoiceLines.Quantity,
                UnitPrice = aRInvoiceLines.UnitPrice,
                SubTotal = aRInvoiceLines.SubTotal,
                GLTaxRateID = aRInvoiceLines.GLTaxRateID,
                TaxAmount = aRInvoiceLines.TaxAmount,
                Discount = aRInvoiceLines.Discount,
                TotalAmount = aRInvoiceLines.TotalAmount,
                ArSalesOrderLineId = aRInvoiceLines.ArSalesOrderLineId,
                OrgBranchName = aRInvoiceLines.OrgBranchName,
                DateCreated = aRInvoiceLines.DateCreated,
                LastModified = aRInvoiceLines.LastModified,
                CreatedBy = aRInvoiceLines.CreatedBy,
                ModifiedBy = aRInvoiceLines.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ArInvoiceLines aRInvoiceLines)
        {
            #region calculate

            aRInvoiceLines.SubTotal = (aRInvoiceLines.Quantity * aRInvoiceLines.UnitPrice)-aRInvoiceLines.Discount;
            aRInvoiceLines.SubTotal = Math.Round(aRInvoiceLines.SubTotal, 2);
            var tax = _db.GLTaxRates.Find(aRInvoiceLines.GLTaxRateID);
            if (tax != null)
            {
                aRInvoiceLines.TaxAmount = aRInvoiceLines.SubTotal * tax.CalculatedRate;
                aRInvoiceLines.TaxAmount = Math.Round(aRInvoiceLines.TaxAmount, 2);
            }
            aRInvoiceLines.TotalAmount = aRInvoiceLines.TaxAmount + aRInvoiceLines.SubTotal;
            aRInvoiceLines.TotalAmount = Math.Round(aRInvoiceLines.TotalAmount, 2);
            #endregion
            #region validated record

            ArInvoice parent =
                _db.ARInvoices.FirstOrDefault(
                    x => x.ArInvoiceId == aRInvoiceLines.ArInvoiceId);
            if (parent != null && parent.IsOnlineApproved)
            {
               
                ModelState.AddModelError("approved", "You cannot manipulate this record"); 
            }

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ArInvoiceLines
                    {
                        Items = aRInvoiceLines.Items,
                        ArInvoiceId = id,
                        Description = aRInvoiceLines.Description,
                        AccountCode = aRInvoiceLines.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aRInvoiceLines.AccountCode),
                        Quantity = aRInvoiceLines.Quantity,
                        UnitPrice = aRInvoiceLines.UnitPrice,
                        SubTotal = aRInvoiceLines.Quantity * aRInvoiceLines.UnitPrice,
                        Discount = aRInvoiceLines.Discount,
                        GLTaxRateID = aRInvoiceLines.GLTaxRateID,
                        TaxAmount = aRInvoiceLines.TaxAmount,
                        TotalAmount = aRInvoiceLines.TotalAmount,
                        ArSalesOrderLineId = aRInvoiceLines.ArSalesOrderLineId,
                        OrgBranchName = aRInvoiceLines.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRInvoiceLines.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRInvoiceLines.ModifiedBy
                    };

                    _db.ARInvoiceLines.Add(entity);
                    _db.SaveChanges();
                    aRInvoiceLines.ArInvoiceLinesId = entity.ArInvoiceLinesId;

                    #region update parent and default distribution

                    UpdateInvoiceTotals(id);
                    _distributionHelper.UpdateArInvoiceDistribution(id);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice item", "Success", User.Identity.Name, $"created invoice item {entity.AccountCode} under record {entity.ArInvoiceId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { aRInvoiceLines }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceLines_Update([DataSourceRequest]DataSourceRequest request, ArInvoiceLines aRInvoiceLines,int id)
        {

            #region validated record

            ArInvoice parent =
                _db.ARInvoices.FirstOrDefault(
                    x => x.ArInvoiceId == aRInvoiceLines.ArInvoiceId);
            if (parent != null && parent.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record");
            }

            #endregion

            #region calculate

            aRInvoiceLines.SubTotal = (aRInvoiceLines.Quantity * aRInvoiceLines.UnitPrice) - aRInvoiceLines.Discount;
            aRInvoiceLines.SubTotal = Math.Round(aRInvoiceLines.SubTotal, 2);
            var tax = _db.GLTaxRates.Find(aRInvoiceLines.GLTaxRateID);
            if (tax != null)
            {
                aRInvoiceLines.TaxAmount = aRInvoiceLines.SubTotal * tax.CalculatedRate;
                aRInvoiceLines.TaxAmount = Math.Round(aRInvoiceLines.TaxAmount, 2);
            }
            aRInvoiceLines.TotalAmount = aRInvoiceLines.TaxAmount + aRInvoiceLines.SubTotal;
            aRInvoiceLines.TotalAmount = Math.Round(aRInvoiceLines.TotalAmount, 2);

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ArInvoiceLines
                    {
                        ArInvoiceLinesId = aRInvoiceLines.ArInvoiceLinesId,
                        ArInvoiceId = id,
                        Items = aRInvoiceLines.Items,
                        Description = aRInvoiceLines.Description,
                        AccountCode = aRInvoiceLines.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aRInvoiceLines.AccountCode),
                        Quantity = aRInvoiceLines.Quantity,
                        UnitPrice = aRInvoiceLines.UnitPrice,
                        Discount = aRInvoiceLines.Discount,
                        SubTotal = aRInvoiceLines.SubTotal,
                        GLTaxRateID = aRInvoiceLines.GLTaxRateID,
                        TaxAmount = aRInvoiceLines.TaxAmount,
                        TotalAmount = aRInvoiceLines.TotalAmount,
                        ArSalesOrderLineId = aRInvoiceLines.ArSalesOrderLineId,
                        OrgBranchName = aRInvoiceLines.OrgBranchName,
                        DateCreated = aRInvoiceLines.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoiceLines.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoiceLines.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region update parent and default distribution

                    UpdateInvoiceTotals(id);
                    _distributionHelper.UpdateArInvoiceDistribution(id);

                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice item", "Success", User.Identity.Name, $"updated invoice item {entity.AccountCode} under record {entity.ArInvoiceId} ", Modules.Finance);
                #endregion

            }
            return Json(new[] { aRInvoiceLines }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceLines_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoiceLines aRInvoiceLines)
        {
            #region validated record

            ArInvoice parent =
                _db.ARInvoices.FirstOrDefault(
                    x => x.ArInvoiceId == aRInvoiceLines.ArInvoiceId);
            if (parent != null && parent.IsOnlineApproved)
            {
                ModelState.AddModelError("approved", "You cannot manipulate this record");
            }

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ArInvoiceLines
                    {
                        ArInvoiceLinesId = aRInvoiceLines.ArInvoiceLinesId,
                        Items = aRInvoiceLines.Items,
                        Description = aRInvoiceLines.Description,
                        AccountCode = aRInvoiceLines.AccountCode,
                        AccountDesc = aRInvoiceLines.AccountDesc,
                        Quantity = aRInvoiceLines.Quantity,
                        UnitPrice = aRInvoiceLines.UnitPrice,
                        SubTotal = aRInvoiceLines.SubTotal,
                        GLTaxRateID = aRInvoiceLines.GLTaxRateID,
                        TaxAmount = aRInvoiceLines.TaxAmount,
                        TotalAmount = aRInvoiceLines.TotalAmount,
                        ArSalesOrderLineId = aRInvoiceLines.ArSalesOrderLineId,
                        Discount = aRInvoiceLines.Discount,
                        OrgBranchName = aRInvoiceLines.OrgBranchName,
                        DateCreated = aRInvoiceLines.DateCreated,
                        LastModified = aRInvoiceLines.LastModified,
                        CreatedBy = aRInvoiceLines.CreatedBy,
                        ModifiedBy = aRInvoiceLines.ModifiedBy
                    };

                    _db.ARInvoiceLines.Attach(entity);
                    _db.ARInvoiceLines.Remove(entity);
                    _db.SaveChanges();


                    #region update parent and default distribution
                    
                    UpdateInvoiceTotals(aRInvoiceLines.ArInvoiceId);
                    _distributionHelper.UpdateArInvoiceDistribution(aRInvoiceLines.ArInvoiceId);

                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AR invoice item" + aRInvoiceLines.ArInvoiceId + " " + aRInvoiceLines.Description, Modules.Finance);
                #endregion

            }
            return Json(new[] { aRInvoiceLines }.ToDataSourceResult(request, ModelState));
        }
        
        private void UpdateInvoiceTotals(int id)
        {
            var aRInvoice = _db.ARInvoices.Find(id);
            
            if (aRInvoice != null)
            {
                var lines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == id).ToList();

                if (lines.Any())
                {
                    decimal lineamount = Math.Round(lines.Sum(x => x.SubTotal), 2);
                    decimal taxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    decimal totalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);

                    #region update parent total

                    aRInvoice.InvoiceStatus = InvoiceStatus.Open;
                    aRInvoice.LineAmount = lineamount;
                    aRInvoice.TaxAmount = taxAmount;
                    aRInvoice.TotalAmount = totalAmount;
                    aRInvoice.AmountDue = totalAmount;
                    _db.Entry(aRInvoice).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    
                }

            }


        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
