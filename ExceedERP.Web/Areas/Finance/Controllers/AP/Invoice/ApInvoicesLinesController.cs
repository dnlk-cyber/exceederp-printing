using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApInvoicesLinesController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult APPaybleInvoicesLines_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaybleInvoicesLines> appaybleinvoiceslines = _db.APPaybleInvoicesLines.Where(x=>x.ApPaybleInvoicesId==id);
            DataSourceResult result = appaybleinvoiceslines.ToDataSourceResult(request, aPPaybleInvoicesLines => new {
                ApPaybleInvoicesLinesId = aPPaybleInvoicesLines.ApPaybleInvoicesLinesId,
                ApPaybleInvoicesId=id,
                Name = aPPaybleInvoicesLines.Name,
                Description = aPPaybleInvoicesLines.Description,
                AccountCode = aPPaybleInvoicesLines.AccountCode,
                AccountDesc = aPPaybleInvoicesLines.AccountDesc,
                InvoiceAmount = aPPaybleInvoicesLines.InvoiceAmount,
                AdvanceAmount = aPPaybleInvoicesLines.AdvanceAmount,
                NetPayable = aPPaybleInvoicesLines.NetPayable,
                ReceiptNo = aPPaybleInvoicesLines.ReceiptNo,
                TaxType = aPPaybleInvoicesLines.TaxType,
                TaxAmount = aPPaybleInvoicesLines.TaxAmount,
                WithholdingType = aPPaybleInvoicesLines.WithholdingType,
                WithholdingAmount = aPPaybleInvoicesLines.WithholdingAmount,
                WithholdTax = aPPaybleInvoicesLines.WithholdTax,
                WithholdTaxAmount = aPPaybleInvoicesLines.WithholdTaxAmount,
                PoNo = aPPaybleInvoicesLines.PoNo,
                OrgBranchName = aPPaybleInvoicesLines.OrgBranchName,
                DateCreated = aPPaybleInvoicesLines.DateCreated,
                LastModified = aPPaybleInvoicesLines.LastModified,
                CreatedBy = aPPaybleInvoicesLines.CreatedBy,
                ModifiedBy = aPPaybleInvoicesLines.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoicesLines_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoicesLines aPPaybleInvoicesLines)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoicesLines.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    #region net payable calculate tax and withholding

                    if (!String.IsNullOrEmpty(aPPaybleInvoicesLines.TaxType))
                    {
                        // tax def
                        var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aPPaybleInvoicesLines.TaxType);
                        if (tax != null)
                        {
                            aPPaybleInvoicesLines.TaxAmount = aPPaybleInvoicesLines.InvoiceAmount * tax.CalculatedRate;
                        }
                    }
                    if (!String.IsNullOrEmpty(aPPaybleInvoicesLines.WithholdingType))
                    {
                        // withholding def
                        var withholding =
                            _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aPPaybleInvoicesLines.WithholdingType);
                        if (withholding != null)
                        {
                            aPPaybleInvoicesLines.WithholdingAmount = aPPaybleInvoicesLines.InvoiceAmount *
                                                                      withholding.CalculatedRate;
                        }
                    }
                    if (aPPaybleInvoicesLines.WithholdTax)
                    {
                        aPPaybleInvoicesLines.WithholdTaxAmount = aPPaybleInvoicesLines.TaxAmount;
                    }
                    if (!aPPaybleInvoicesLines.WithholdTax)
                    {
                        aPPaybleInvoicesLines.WithholdTaxAmount = 0;
                    }
                    aPPaybleInvoicesLines.NetPayable = aPPaybleInvoicesLines.InvoiceAmount
                                                       - aPPaybleInvoicesLines.WithholdingAmount
                                                       + aPPaybleInvoicesLines.TaxAmount
                                                       - aPPaybleInvoicesLines.WithholdTaxAmount;

                    #endregion

                    var entity = new ApPaybleInvoicesLines
                    {
                        ApPaybleInvoicesId = id,
                        Name = aPPaybleInvoicesLines.Name,
                        Description = aPPaybleInvoicesLines.Description,
                        AccountCode = aPPaybleInvoicesLines.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoicesLines.AccountCode),
                        InvoiceAmount = aPPaybleInvoicesLines.InvoiceAmount,
                        AdvanceAmount = aPPaybleInvoicesLines.AdvanceAmount,
                        NetPayable = aPPaybleInvoicesLines.NetPayable,
                        ReceiptNo = aPPaybleInvoicesLines.ReceiptNo,
                        TaxType = aPPaybleInvoicesLines.TaxType,
                        TaxAmount = aPPaybleInvoicesLines.TaxAmount,
                        WithholdingType = aPPaybleInvoicesLines.WithholdingType,
                        WithholdingAmount = aPPaybleInvoicesLines.WithholdingAmount,
                        WithholdTax = aPPaybleInvoicesLines.WithholdTax,
                        WithholdTaxAmount = aPPaybleInvoicesLines.WithholdTaxAmount,
                        PoNo = aPPaybleInvoicesLines.PoNo,
                        OrgBranchName = aPPaybleInvoicesLines.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPPaybleInvoicesLines.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPPaybleInvoicesLines.ModifiedBy
                    };

                    _db.APPaybleInvoicesLines.Add(entity);
                    _db.SaveChanges();
                    aPPaybleInvoicesLines.ApPaybleInvoicesLinesId = entity.ApPaybleInvoicesLinesId;
                    
                    #region update and distribution

                    UpdateInvoiceTotals(id);
                    _distributionHelper.UpdateApAutoInvoiceDistribution(id);

                    #endregion
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP invoice item", "Success", User.Identity.Name, $"created invoice item {entity.AccountCode} under record {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aPPaybleInvoicesLines }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoicesLines_Update(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoicesLines aPPaybleInvoicesLines)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoicesLines.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    #region net payable calculate tax and withholding

                    if (!String.IsNullOrEmpty(aPPaybleInvoicesLines.TaxType))
                    {
                        // tax def
                        var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aPPaybleInvoicesLines.TaxType);
                        if (tax != null)
                        {
                            aPPaybleInvoicesLines.TaxAmount = aPPaybleInvoicesLines.InvoiceAmount * tax.CalculatedRate;
                        }
                    }
                    if (!String.IsNullOrEmpty(aPPaybleInvoicesLines.WithholdingType))
                    {
                        // withholding def
                        var withholding =
                            _db.GLTaxRates.FirstOrDefault(x => x.TaxName == aPPaybleInvoicesLines.WithholdingType);
                        if (withholding != null)
                        {
                            aPPaybleInvoicesLines.WithholdingAmount = aPPaybleInvoicesLines.InvoiceAmount *
                                                                      withholding.CalculatedRate;
                        }
                    }
                    if (aPPaybleInvoicesLines.WithholdTax)
                    {
                        aPPaybleInvoicesLines.WithholdTaxAmount = aPPaybleInvoicesLines.TaxAmount;
                    }
                    if (!aPPaybleInvoicesLines.WithholdTax)
                    {
                        aPPaybleInvoicesLines.WithholdTaxAmount = 0;
                    }
                    aPPaybleInvoicesLines.NetPayable = aPPaybleInvoicesLines.InvoiceAmount
                                                       - aPPaybleInvoicesLines.WithholdingAmount
                                                       + aPPaybleInvoicesLines.TaxAmount
                                                       - aPPaybleInvoicesLines.WithholdTaxAmount;

                    #endregion

                    var entity = new ApPaybleInvoicesLines
                    {
                        ApPaybleInvoicesId = id,
                        ApPaybleInvoicesLinesId = aPPaybleInvoicesLines.ApPaybleInvoicesLinesId,
                        Name = aPPaybleInvoicesLines.Name,
                        Description = aPPaybleInvoicesLines.Description,
                        AccountCode = aPPaybleInvoicesLines.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoicesLines.AccountCode),
                        InvoiceAmount = aPPaybleInvoicesLines.InvoiceAmount,
                        AdvanceAmount = aPPaybleInvoicesLines.AdvanceAmount,
                        NetPayable = aPPaybleInvoicesLines.NetPayable,
                        ReceiptNo = aPPaybleInvoicesLines.ReceiptNo,
                        TaxType = aPPaybleInvoicesLines.TaxType,
                        TaxAmount = aPPaybleInvoicesLines.TaxAmount,
                        WithholdingType = aPPaybleInvoicesLines.WithholdingType,
                        WithholdingAmount = aPPaybleInvoicesLines.WithholdingAmount,
                        WithholdTax = aPPaybleInvoicesLines.WithholdTax,
                        WithholdTaxAmount = aPPaybleInvoicesLines.WithholdTaxAmount,
                        PoNo = aPPaybleInvoicesLines.PoNo,
                        OrgBranchName = aPPaybleInvoicesLines.OrgBranchName,
                        DateCreated = aPPaybleInvoicesLines.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPPaybleInvoicesLines.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APPaybleInvoicesLines.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    
                    #region update and distribution

                    UpdateInvoiceTotals(id);
                    _distributionHelper.UpdateApAutoInvoiceDistribution(id);

                    #endregion
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP invoice item", "Success", User.Identity.Name, $"updated invoice item {entity.AccountCode} under record {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aPPaybleInvoicesLines }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoicesLines_Destroy([DataSourceRequest]DataSourceRequest request, ApPaybleInvoicesLines aPPaybleInvoicesLines)
        { 
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoicesLines.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoicesLines
                    {
                        ApPaybleInvoicesLinesId = aPPaybleInvoicesLines.ApPaybleInvoicesLinesId,
                        Name = aPPaybleInvoicesLines.Name,
                        Description = aPPaybleInvoicesLines.Description,
                        AccountCode = aPPaybleInvoicesLines.AccountCode,
                        AccountDesc = aPPaybleInvoicesLines.AccountDesc,
                        InvoiceAmount = aPPaybleInvoicesLines.InvoiceAmount,
                        AdvanceAmount = aPPaybleInvoicesLines.AdvanceAmount,
                        NetPayable = aPPaybleInvoicesLines.NetPayable,
                        ReceiptNo = aPPaybleInvoicesLines.ReceiptNo,
                        TaxType = aPPaybleInvoicesLines.TaxType,
                        TaxAmount = aPPaybleInvoicesLines.TaxAmount,
                        WithholdingType = aPPaybleInvoicesLines.WithholdingType,
                        WithholdingAmount = aPPaybleInvoicesLines.WithholdingAmount,
                        WithholdTax = aPPaybleInvoicesLines.WithholdTax,
                        WithholdTaxAmount = aPPaybleInvoicesLines.WithholdTaxAmount,
                        PoNo = aPPaybleInvoicesLines.PoNo,
                        OrgBranchName = aPPaybleInvoicesLines.OrgBranchName,
                        DateCreated = aPPaybleInvoicesLines.DateCreated,
                        LastModified = aPPaybleInvoicesLines.LastModified,
                        CreatedBy = aPPaybleInvoicesLines.CreatedBy,
                        ModifiedBy = aPPaybleInvoicesLines.ModifiedBy
                    };

                    _db.APPaybleInvoicesLines.Attach(entity);
                    _db.APPaybleInvoicesLines.Remove(entity);
                    _db.SaveChanges();


                    #region update and distribution

                    UpdateInvoiceTotals(aPPaybleInvoicesLines.ApPaybleInvoicesId);
                    _distributionHelper.UpdateApAutoInvoiceDistribution(aPPaybleInvoicesLines.ApPaybleInvoicesId);

                    #endregion
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payable invoice item " + aPPaybleInvoicesLines.ApPaybleInvoicesId + " " + aPPaybleInvoicesLines.AccountCode, Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aPPaybleInvoicesLines }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

       

        private void UpdateInvoiceTotals(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApPaybleInvoices record = _db.APPaybleInvoices.Find(id);
            if (record == null)
            {
                //return HttpNotFound();
            }

            else
            {
                #region update parent total
                record.InvoiceStatus = InvoiceStatus.Open;
                record.InvoiceAmount = record.APPaybleInvoicesLines.Sum(x => x.InvoiceAmount);
                record.AdvanceAmount = record.APPaybleInvoicesLines.Sum(x => x.AdvanceAmount);
                record.TaxAmount = record.APPaybleInvoicesLines.Sum(x => x.TaxAmount);
                record.WithholdingAmount = record.APPaybleInvoicesLines.Sum(x => x.WithholdingAmount);
                record.Subtotal = record.InvoiceAmount + record.TaxAmount - record.WithholdingAmount;
                record.WithholdTaxAmount = record.APPaybleInvoicesLines.Sum(x => x.WithholdTaxAmount);
                record.NetPayable = record.APPaybleInvoicesLines.Sum(x => x.NetPayable);
                record.AmountDue = record.NetPayable;
                _db.Entry(record).State = EntityState.Modified;
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
