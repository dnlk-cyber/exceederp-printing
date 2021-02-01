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

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.StandardCollection
{
    public class ArStandardCollectionLinesController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARStandardCollectionLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArStandardCollectionLines> arstandardcollectionlines = _db.ARStandardCollectionLines.Where(x=>x.ArStandardCollectionId==id);
            DataSourceResult result = arstandardcollectionlines.ToDataSourceResult(request, aRStandardCollectionLines => new
            {
                ArStandardCollectionLinesId = aRStandardCollectionLines.ArStandardCollectionLinesId,
                ArStandardCollectionId = aRStandardCollectionLines.ArStandardCollectionId,
                IsPrepayment = aRStandardCollectionLines.IsPrepayment,
                InvoiceNumber = aRStandardCollectionLines.InvoiceNumber,
                InstallmentNo = aRStandardCollectionLines.InstallmentNo,
                ApplyAmount = aRStandardCollectionLines.ApplyAmount,
                BalanceDue = aRStandardCollectionLines.BalanceDue,
                ApplyDate = aRStandardCollectionLines.ApplyDate,
                TaxType = aRStandardCollectionLines.TaxType,
                TaxAmount = aRStandardCollectionLines.TaxAmount,
                WithholdingType = aRStandardCollectionLines.WithholdingType,
                WithholdingAmount = aRStandardCollectionLines.WithholdingAmount,
                WithholdTax = aRStandardCollectionLines.WithholdTax,
                WithholdTaxAmount = aRStandardCollectionLines.WithholdTaxAmount,
                NetAmount = aRStandardCollectionLines.NetAmount,
                HasPrepayment = aRStandardCollectionLines.HasPrepayment,
                PrepaymentAmount = aRStandardCollectionLines.PrepaymentAmount,
                Source = aRStandardCollectionLines.Source,
                BranchId = aRStandardCollectionLines.BranchId,
                OrgBranchName = aRStandardCollectionLines.OrgBranchName,
                DateCreated = aRStandardCollectionLines.DateCreated,
                LastModified = aRStandardCollectionLines.LastModified,
                CreatedBy = aRStandardCollectionLines.CreatedBy,
                ModifiedBy = aRStandardCollectionLines.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ArStandardCollectionLines aRStandardCollectionLines)
        {
            if (aRStandardCollectionLines.IsPrepayment && aRStandardCollectionLines.PrepaymentAmount != 0)
            {
                ModelState.AddModelError("prep","Since the payment is prepayment, prepayment amount should not be filled.");
            }
            #region tax witholding tax calc//old
            //if (!String.IsNullOrEmpty(aRStandardCollectionLines.TaxType))
            //{
            //    var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aRStandardCollectionLines.TaxType);
            //    if (tax != null && tax.CalculatedRate > 0)
            //        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount / (1 + tax.CalculatedRate)) * tax.CalculatedRate;
            //    else
            //    {
            //        aRStandardCollectionLines.WithholdTaxAmount = 0;
            //    }
            //}
            #endregion
            #region  tax calc
            var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aRStandardCollectionLines.TaxType);
            string invoicetype = aRStandardCollectionLines.InvoiceNumber.Split('_')[0];
            string recordid = aRStandardCollectionLines.InvoiceNumber.Split('_')[1];
            int did = int.Parse(recordid);
            if (invoicetype == "INVOICE")
            {
                var invoice = _db.ARInvoices.Find(did);
                aRStandardCollectionLines.BranchId = invoice.BranchId;

                aRStandardCollectionLines.Source = InvoiceSource.Invoice;
                var invoiceLines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == did).AsQueryable();
                if (invoiceLines.Any())
                {
                    if (aRStandardCollectionLines.WithholdTax)
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount - invoiceLines.Sum(x => x.TaxAmount)) * (decimal)0.15; //0.15 will be by setting later
                        aRStandardCollectionLines.WithholdTaxAmount = Math.Round(aRStandardCollectionLines.WithholdTaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = 0;//reset
                    }

                    if (tax != null && tax.CalculatedRate > 0)
                    {
                        aRStandardCollectionLines.TaxAmount = invoiceLines.Sum(x => x.TaxAmount);
                        aRStandardCollectionLines.TaxAmount = Math.Round(aRStandardCollectionLines.TaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.TaxAmount = 0;
                    }

                   
                }

               

            }
            else if (invoicetype == "SALESORDER")
            {
                var salesorder = _db.ArSalesOrders.Find(did);
                aRStandardCollectionLines.BranchId = salesorder.BranchId;

                aRStandardCollectionLines.Source = InvoiceSource.SalesOrder;
                var orderLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == did).AsQueryable();
                if (orderLines.Any())
                {
                    if (aRStandardCollectionLines.WithholdTax)
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount- orderLines.Sum(x => x.TaxAmount)) * (decimal)0.15;//0.15 will be by setting later
                        aRStandardCollectionLines.WithholdTaxAmount = Math.Round(aRStandardCollectionLines.WithholdTaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = 0;
                    }

                    aRStandardCollectionLines.TaxAmount = orderLines.Sum(x => x.TaxAmount);
                    aRStandardCollectionLines.TaxAmount = Math.Round(aRStandardCollectionLines.TaxAmount, 2);

                }
            }

            #region withholding calc
            if (!String.IsNullOrEmpty(aRStandardCollectionLines.WithholdingType))
            {
                var withholding = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == aRStandardCollectionLines.WithholdingType);
                if (withholding != null)
                {
                    aRStandardCollectionLines.WithholdingAmount = (aRStandardCollectionLines.ApplyAmount - aRStandardCollectionLines.TaxAmount) * withholding.CalculatedRate;
                    aRStandardCollectionLines.WithholdingAmount = Math.Round(aRStandardCollectionLines.WithholdingAmount, 2);
                }
            }
            #endregion

            #endregion

            #region net

            aRStandardCollectionLines.NetAmount = aRStandardCollectionLines.ApplyAmount
                                                  - aRStandardCollectionLines.WithholdingAmount
                                                  - aRStandardCollectionLines.WithholdTaxAmount
                                                  - aRStandardCollectionLines.PrepaymentAmount;
            #endregion
            #region validated record

            ArStandardCollection parent =
                _db.ARStandardCollections.Find(aRStandardCollectionLines.ArStandardCollectionId);
            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("Error", "You cannot manipulate this record");

            #endregion

            if (ModelState.IsValid)
            {
               
                    var entity = new ArStandardCollectionLines
                    {
                        ArStandardCollectionId = id,
                        IsPrepayment = aRStandardCollectionLines.IsPrepayment,
                        InvoiceNumber = aRStandardCollectionLines.InvoiceNumber,
                        InstallmentNo = aRStandardCollectionLines.InstallmentNo,
                        ApplyAmount = aRStandardCollectionLines.ApplyAmount,
                        BalanceDue = aRStandardCollectionLines.BalanceDue,
                        ApplyDate = aRStandardCollectionLines.ApplyDate,
                        TaxType = aRStandardCollectionLines.TaxType,
                        TaxAmount = aRStandardCollectionLines.TaxAmount,
                        WithholdingType = aRStandardCollectionLines.WithholdingType,
                        WithholdingAmount = aRStandardCollectionLines.WithholdingAmount,
                        WithholdTax = aRStandardCollectionLines.WithholdTax,
                        WithholdTaxAmount = aRStandardCollectionLines.WithholdTaxAmount,
                        NetAmount = aRStandardCollectionLines.NetAmount,
                        HasPrepayment = aRStandardCollectionLines.HasPrepayment,
                        PrepaymentAmount = aRStandardCollectionLines.PrepaymentAmount,
                        Source = aRStandardCollectionLines.Source,
                        BranchId = aRStandardCollectionLines.BranchId,
                        OrgBranchName = aRStandardCollectionLines.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRStandardCollectionLines.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRStandardCollectionLines.ModifiedBy
                    };

                    _db.ARStandardCollectionLines.Add(entity);
                    _db.SaveChanges();
                    aRStandardCollectionLines.ArStandardCollectionLinesId = entity.ArStandardCollectionLinesId;

                    #region updatebalance and distri

                    UpdateCollectionBalances(id);
                    _distributionHelper.UpdateArStandardCollectionDistribution(id);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection item", "Success", User.Identity.Name, $"created standard collection item {entity.ApplyAmount} under record {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion

            }
            return Json(new[] { aRStandardCollectionLines }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionLines_Update(int id,[DataSourceRequest]DataSourceRequest request, ArStandardCollectionLines aRStandardCollectionLines)
        {
            if (aRStandardCollectionLines.IsPrepayment && aRStandardCollectionLines.PrepaymentAmount != 0)
            {
                ModelState.AddModelError("prep", @"Since the payment is prepayment, prepayment amount should not be filled.");
            }

            #region tax witholding tax calc--old
            //if (!String.IsNullOrEmpty(aRStandardCollectionLines.TaxType))
            //{
            //    var tax = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == aRStandardCollectionLines.TaxType);
            //    if (tax != null)
            //        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount/(1+ tax.CalculatedRate))*tax.CalculatedRate;
            //}
            #endregion
            #region witholding tax calc
            var tax = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == aRStandardCollectionLines.TaxType);
            string invoicetype = aRStandardCollectionLines.InvoiceNumber.Split('_')[0];
            string recordid = aRStandardCollectionLines.InvoiceNumber.Split('_')[1];
            int did = int.Parse(recordid);
            if (invoicetype == "INVOICE")
            {
                var invoice = _db.ARInvoices.Find(did);
                aRStandardCollectionLines.BranchId = invoice.BranchId;

                aRStandardCollectionLines.Source = InvoiceSource.Invoice;
                var invoiceLines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == did).AsQueryable();
                if (invoiceLines.Any())
                {
                    
                    if (aRStandardCollectionLines.WithholdTax)
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount- invoiceLines.Sum(x => x.TaxAmount)) * (decimal)0.15;//0.15 will be by setting later
                        aRStandardCollectionLines.WithholdTaxAmount = Math.Round(aRStandardCollectionLines.WithholdTaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = 0;
                    }

                    if (tax != null && tax.CalculatedRate > 0)
                    {
                        aRStandardCollectionLines.TaxAmount = invoiceLines.Sum(x => x.TaxAmount); 
                        aRStandardCollectionLines.TaxAmount = Math.Round(aRStandardCollectionLines.TaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.TaxAmount = 0;
                    }
                   
                    
                }



            }
            else if (invoicetype == "SALESORDER")
            {
                var salesorder = _db.ArSalesOrders.Find(did);
                aRStandardCollectionLines.BranchId = salesorder.BranchId;

                aRStandardCollectionLines.Source = InvoiceSource.SalesOrder;
                var orderLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == did).AsQueryable();
                if (orderLines.Any())
                {
                   
                    if (tax != null && tax.CalculatedRate > 0 && aRStandardCollectionLines.WithholdTax)
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = (aRStandardCollectionLines.ApplyAmount- orderLines.Sum(x => x.TaxAmount)) * (decimal)0.15;//0.15 will be by setting later
                        aRStandardCollectionLines.WithholdTaxAmount = Math.Round(aRStandardCollectionLines.WithholdTaxAmount, 2);
                    }
                    else
                    {
                        aRStandardCollectionLines.WithholdTaxAmount = 0;
                    }

                    aRStandardCollectionLines.TaxAmount = orderLines.Sum(x => x.TaxAmount);
                    aRStandardCollectionLines.TaxAmount = Math.Round(aRStandardCollectionLines.TaxAmount, 2);

                }
            }

            #endregion
            #region withholding calc
            if (!String.IsNullOrEmpty(aRStandardCollectionLines.WithholdingType))
            {
                var withholding = _db.GLTaxRates.SingleOrDefault(x => x.TaxName == aRStandardCollectionLines.WithholdingType);
                if (withholding != null)
                {
                    aRStandardCollectionLines.WithholdingAmount = (aRStandardCollectionLines.ApplyAmount - aRStandardCollectionLines.TaxAmount) * withholding.CalculatedRate;
                    aRStandardCollectionLines.WithholdingAmount = Math.Round(aRStandardCollectionLines.WithholdingAmount, 2);
                }
            }
            #endregion
            #region net

            aRStandardCollectionLines.NetAmount = aRStandardCollectionLines.ApplyAmount
                                                  - aRStandardCollectionLines.WithholdingAmount
                                                  - aRStandardCollectionLines.WithholdTaxAmount
                                                  - aRStandardCollectionLines.PrepaymentAmount;
            #endregion
            #region validated record

            var parent =
                _db.ARStandardCollections.Find(aRStandardCollectionLines.ArStandardCollectionId);
            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("Error", "You cannot manipulate this record");

            #endregion
            if (ModelState.IsValid)
            {
                    #region get previous installment

                    if (!String.IsNullOrEmpty(aRStandardCollectionLines.InvoiceNumber))
                    {
                        var previousPayments =
                            _db.ARStandardCollectionLines.Where(
                                x => x.InvoiceNumber == aRStandardCollectionLines.InvoiceNumber).AsNoTracking().ToList();
                        aRStandardCollectionLines.InstallmentNo = "Installment-" + (previousPayments.Count() + 1);
                    }

                    #endregion

                    var entity = new ArStandardCollectionLines
                    {
                        ArStandardCollectionId = id,
                        ArStandardCollectionLinesId = aRStandardCollectionLines.ArStandardCollectionLinesId,
                        IsPrepayment = aRStandardCollectionLines.IsPrepayment,
                        InvoiceNumber = aRStandardCollectionLines.InvoiceNumber,
                        InstallmentNo = aRStandardCollectionLines.InstallmentNo,
                        ApplyAmount = aRStandardCollectionLines.ApplyAmount,
                        BalanceDue = aRStandardCollectionLines.BalanceDue,
                        ApplyDate = aRStandardCollectionLines.ApplyDate,
                        TaxType = aRStandardCollectionLines.TaxType,
                        TaxAmount = aRStandardCollectionLines.TaxAmount,
                        WithholdingType = aRStandardCollectionLines.WithholdingType,
                        WithholdingAmount = aRStandardCollectionLines.WithholdingAmount,
                        WithholdTax = aRStandardCollectionLines.WithholdTax,
                        WithholdTaxAmount = aRStandardCollectionLines.WithholdTaxAmount,
                        NetAmount = aRStandardCollectionLines.NetAmount,
                        HasPrepayment = aRStandardCollectionLines.HasPrepayment,
                        PrepaymentAmount = aRStandardCollectionLines.PrepaymentAmount,
                        Source = aRStandardCollectionLines.Source,
                        BranchId = aRStandardCollectionLines.BranchId,
                        OrgBranchName = aRStandardCollectionLines.OrgBranchName,
                        DateCreated = aRStandardCollectionLines.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRStandardCollectionLines.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARStandardCollectionLines.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region updatebalance and distri

                    UpdateCollectionBalances(id);
                    _distributionHelper.UpdateArStandardCollectionDistribution(id);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection item", "Success", User.Identity.Name, $"updated standard collection item {entity.ApplyAmount} under record {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion

            }
            return Json(new[] { aRStandardCollectionLines }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionLines_Destroy([DataSourceRequest]DataSourceRequest request, ArStandardCollectionLines aRStandardCollectionLines)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ArStandardCollection parent =
                    _db.ARStandardCollections.Find(aRStandardCollectionLines.ArStandardCollectionId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ArStandardCollectionLines
                    {
                        ArStandardCollectionLinesId = aRStandardCollectionLines.ArStandardCollectionLinesId,
                        IsPrepayment = aRStandardCollectionLines.IsPrepayment,
                        InvoiceNumber = aRStandardCollectionLines.InvoiceNumber,
                        InstallmentNo = aRStandardCollectionLines.InstallmentNo,
                        ApplyAmount = aRStandardCollectionLines.ApplyAmount,
                        BalanceDue = aRStandardCollectionLines.BalanceDue,
                        ApplyDate = aRStandardCollectionLines.ApplyDate,
                        TaxType = aRStandardCollectionLines.TaxType,
                        TaxAmount = aRStandardCollectionLines.TaxAmount,
                        WithholdingType = aRStandardCollectionLines.WithholdingType,
                        WithholdingAmount = aRStandardCollectionLines.WithholdingAmount,
                        WithholdTax = aRStandardCollectionLines.WithholdTax,
                        WithholdTaxAmount = aRStandardCollectionLines.WithholdTaxAmount,
                        NetAmount = aRStandardCollectionLines.NetAmount,
                        HasPrepayment = aRStandardCollectionLines.HasPrepayment,
                        PrepaymentAmount = aRStandardCollectionLines.PrepaymentAmount,
                        Source = aRStandardCollectionLines.Source,
                        BranchId = aRStandardCollectionLines.BranchId,
                        OrgBranchName = aRStandardCollectionLines.OrgBranchName,
                        DateCreated = aRStandardCollectionLines.DateCreated,
                        LastModified = aRStandardCollectionLines.LastModified,
                        CreatedBy = aRStandardCollectionLines.CreatedBy,
                        ModifiedBy = aRStandardCollectionLines.ModifiedBy
                    };

                    _db.ARStandardCollectionLines.Attach(entity);
                    _db.ARStandardCollectionLines.Remove(entity);
                    _db.SaveChanges();

                    #region updatebalance and distri

                    UpdateCollectionBalances(aRStandardCollectionLines.ArStandardCollectionId);
                    _distributionHelper.UpdateArStandardCollectionDistribution(aRStandardCollectionLines.ArStandardCollectionId);

                    #endregion

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted standard collection item " + aRStandardCollectionLines.ArStandardCollectionId + " " + aRStandardCollectionLines.InvoiceNumber, Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aRStandardCollectionLines }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView ,FinanceConstants.AccountReceivableApprover + "," +FinanceConstants.AccountReceivableCollection)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        private void UpdateCollectionBalances(int? id)
        {
            var collection = _db.ARStandardCollections.Find(id);
            if (collection != null)
            {
                #region update parent balances
                var lines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id).ToList();
                collection.PrepaymentBalance = lines.Where(x => x.IsPrepayment).Sum(x => x.ApplyAmount);
                
                collection.TaxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                collection.TotalAmount = Math.Round(lines.Sum(x => x.ApplyAmount), 2);
                collection.LineAmount = collection.TotalAmount- collection.TaxAmount;
                decimal totalWithInvoice = lines.Where(x => x.IsPrepayment == false).Where(x => x.InvoiceNumber != null).Where(x => x.InvoiceNumber != "").Sum(x => x.ApplyAmount);
                decimal totalWithoutInvoice = lines.Where(x => x.IsPrepayment == false).Where(x => string.IsNullOrEmpty(x.InvoiceNumber)).Sum(x => x.ApplyAmount);

                if (String.IsNullOrEmpty(collection.CustomerName))
                {
                    collection.UnidentifiedBalance = totalWithoutInvoice;
                }
                else
                {
                    collection.UnappliedBalance = totalWithoutInvoice;
                    collection.AppliedBalance = totalWithInvoice;
                }
                _db.Entry(collection).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion

            }

           

        }

    

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
