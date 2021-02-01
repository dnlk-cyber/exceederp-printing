using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();



        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }

        private void PopulateViewData()
        {
            var terms = _db.PaymentTerms.ToList();

            ViewData["terms"] = terms.Select(p => new PaymentTerms
            {
                PaymentTermsDays = p.PaymentTermsDays
            });

            var customers = _db.ARCustomerProfiles.Where(x => x.IsActive).ToList();

            ViewData["customers"] = customers.Select(c => new ArCustomerProfile
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CompanyName
            });


            var tax = _db.GLTaxRates.Where(x => x.ApplyToAr).ToList();

            ViewData["taxes"] = tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            });


            var productorService = _db.ARSetupProductServiceAccounts.ToList();

            ViewData["productorServices"] = productorService
                .Select(e => new ARSetupProductServiceAccount
                {
                    Product = e.Product
                })
                .OrderBy(e => e.Product);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARInvoices_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            IQueryable<ArInvoice> arinvoices = _db.ARInvoices.Where(x=>x.IsManual==false);
            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetArInvoiceTranferableList();
                arinvoices =
                    arinvoices.Where(x => transfferableList.Contains(x.ArInvoiceId));
            }
            DataSourceResult result = arinvoices.ToDataSourceResult(request, aRInvoice => new
            {
                ArInvoiceId = aRInvoice.ArInvoiceId,
                CustomerName = aRInvoice.CustomerName,
                CustomerId = aRInvoice.CustomerId,
                TransactionClass = aRInvoice.TransactionClass,
                Date = aRInvoice.Date,
                PaymentTerm = aRInvoice.PaymentTerm,
                DueDate = aRInvoice.DueDate,
                LineAmount = Math.Round(aRInvoice.LineAmount, 2),
                TaxAmount = Math.Round(aRInvoice.TaxAmount, 2),
                TotalAmount = Math.Round(aRInvoice.TotalAmount, 2),
                AmountDue = Math.Round(aRInvoice.AmountDue, 2),
                InvoiceStatus = aRInvoice.InvoiceStatus,
                IsManual = aRInvoice.IsManual,
                Reference = aRInvoice.Reference,
                BranchId = aRInvoice.BranchId,
                ArSalesOrderId = aRInvoice.ArSalesOrderId,
                ArSalesOrderPartialPrintId = aRInvoice.ArSalesOrderPartialPrintId,
                SourceInvoiceId = aRInvoice.SourceInvoiceId,
                IsSendForApproval = aRInvoice.IsSendForApproval,
                IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                Status = aRInvoice.Status,
                IsOnlineApproved = aRInvoice.IsOnlineApproved,
                OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                IsVoid = aRInvoice.IsVoid,
                VoidBy = aRInvoice.VoidBy,
                VoidTime = aRInvoice.VoidTime,
                OrgBranchName = aRInvoice.OrgBranchName,
                DateCreated = aRInvoice.DateCreated,
                LastModified = aRInvoice.LastModified,
                CreatedBy = aRInvoice.CreatedBy,
                ModifiedBy = aRInvoice.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Create([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            #region validation
            string customername = "";

            //if (aRInvoice.ArSalesOrderPartialPrintId==0)
            //    ModelState.AddModelError("salesorder", "Please select sales order.");

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit","Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            //var arSalesOrderPartialPrint = _db.ArSalesOrderPartialPrints.Find(aRInvoice.ArSalesOrderPartialPrintId);
            //if (arSalesOrderPartialPrint != null) aRInvoice.ArSalesOrderId = arSalesOrderPartialPrint.ArSalesOrderId;

            //allow partial invoices
            //var exist = _db.ARInvoices.Where(x => x.ArSalesOrderId == aRInvoice.ArSalesOrderId).ToList();
            //if (exist.Any())
            //    ModelState.AddModelError("exist", "The sales order number " + aRInvoice.ArSalesOrderId + " already exists.");

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                var exists = _db.ARInvoices.Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }


            #region validate sales order remaining quantity

            var orderLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == aRInvoice.ArSalesOrderId).ToList();
            bool qtyExist = false;
            foreach (var orderLine in orderLines)
            {
                var orderitem = _db.ArProductService.Find(int.Parse(orderLine.SubItem));
                if (orderitem != null)
                {
                    #region get prev invoiced
                    decimal prevQty = 0;
                    decimal prevDiscount = 0;
                    var invoicedPrevs = _db.ARInvoices
                        .Where(x => x.ArSalesOrderId == aRInvoice.ArSalesOrderId)
                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                        .ToList();
                    if (invoicedPrevs.Any())
                    {
                        foreach (var prev in invoicedPrevs)
                        {
                            var invoiceLines = _db.ARInvoiceLines
                                .Where(x => x.ArInvoiceId == prev.ArInvoiceId)
                                .Where(x => x.ArSalesOrderLineId == orderLine.ArSalesOrderLineId).ToList();

                            prevQty = prevQty + invoiceLines.Sum(x => x.Quantity);
                            prevDiscount = prevDiscount + invoiceLines.Sum(x => x.Discount);
                        }

                    }
                    #endregion

                    if (prevQty < orderLine.Quantity)
                    {
                        qtyExist = true;
                        continue;
                    }
                }
            }
            if(!qtyExist)
                ModelState.AddModelError("noqty","There is no remaining uninvoiced quantity in this invoice.");
            #endregion
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ArInvoice
                {
                    CustomerName = customername,
                    CustomerId = aRInvoice.CustomerId,
                    TransactionClass = aRInvoice.TransactionClass,
                    Date = aRInvoice.Date.Date,
                    PaymentTerm = aRInvoice.PaymentTerm,
                    DueDate = aRInvoice.DueDate.Date,
                    LineAmount = aRInvoice.LineAmount,
                    TaxAmount = aRInvoice.TaxAmount,
                    TotalAmount = aRInvoice.TotalAmount,
                    AmountDue = aRInvoice.AmountDue,
                    InvoiceStatus = aRInvoice.InvoiceStatus,
                    IsManual = aRInvoice.IsManual,
                    Reference = aRInvoice.Reference,
                    BranchId = aRInvoice.BranchId,
                    IsPostedToSecondLedger = aRInvoice.IsPostedToSecondLedger,
                    ArSalesOrderId = aRInvoice.ArSalesOrderId,
                    SourceInvoiceId = aRInvoice.SourceInvoiceId,
                    ArSalesOrderPartialPrintId = 0,
                    IsSendForApproval = aRInvoice.IsSendForApproval,
                    IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                    IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                    OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                    Status = Enum.GetName(typeof(OperationStatuses), 5),
                    IsOnlineApproved = aRInvoice.IsOnlineApproved,
                    OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                    OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                    IsVoid = aRInvoice.IsVoid,
                    VoidBy = aRInvoice.VoidBy,
                    VoidTime = aRInvoice.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aRInvoice.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRInvoice.ModifiedBy
                };

                _db.ARInvoices.Add(entity);
                _db.SaveChanges();
                aRInvoice.ArInvoiceId = entity.ArInvoiceId;

                SalesOrderInvoiceItems(entity.ArInvoiceId);
                UpdateInvoiceTotals(entity.ArInvoiceId);
                _distributionHelper.UpdateArInvoiceDistribution(entity.ArInvoiceId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice", "Success", User.Identity.Name, $"created invoice {entity.ArInvoiceId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Update([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            #region validation
            string customername = "";

            //if (aRInvoice.ArSalesOrderPartialPrintId == 0)
            //    ModelState.AddModelError("salesorder", "Please select sales order.");

            if (String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                ModelState.AddModelError("exisit", "Customer ID can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.CustomerId))
                customername = _db.ARCustomerProfiles.Where(c => c.CustomerId == aRInvoice.CustomerId)
                    .Select(x => x.CompanyName).FirstOrDefault();
            if (String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                ModelState.AddModelError("exisit", "Payment Term can't be empty.");

            if (!String.IsNullOrWhiteSpace(aRInvoice.PaymentTerm))
                aRInvoice.DueDate = aRInvoice.Date.AddDays(int.Parse(aRInvoice.PaymentTerm));

            //var arSalesOrderPartialPrint = _db.ArSalesOrderPartialPrints.Find(aRInvoice.ArSalesOrderPartialPrintId);
            //if (arSalesOrderPartialPrint != null) aRInvoice.ArSalesOrderId = arSalesOrderPartialPrint.ArSalesOrderId;


            //partial invoice
            //var exist = _db.ARInvoices.Where(x => x.ArInvoiceId != aRInvoice.ArInvoiceId).Where(x => x.ArSalesOrderId == aRInvoice.ArSalesOrderId).ToList();
            //if (exist.Any())
            //    ModelState.AddModelError("exist", "The sales order number " + aRInvoice.ArSalesOrderId + " already exists.");

            if (!String.IsNullOrEmpty(aRInvoice.Reference))
            {
                var exists = _db.ARInvoices
                    .Where(x=>x.ArInvoiceId!=aRInvoice.ArInvoiceId)
                    .Where(x => x.Reference == aRInvoice.Reference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Reference", $"{aRInvoice.Reference}  already exists in the system.");
            }

            var parent = _db.ARInvoices.Find(aRInvoice.ArInvoiceId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                {
                    ModelState.AddModelError("approved", "You cannot manipulate this record");
                }

                #region detach local version
                var local = _db.Set<ArInvoice>()
                    .Local
                    .FirstOrDefault(f => f.ArInvoiceId == parent.ArInvoiceId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }

            #endregion

            if (ModelState.IsValid)
            {
                    var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        CustomerName = aRInvoice.CustomerName,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                        IsManual = aRInvoice.IsManual,
                        Reference = aRInvoice.Reference,
                        BranchId = aRInvoice.BranchId,
                        IsPostedToSecondLedger = aRInvoice.IsPostedToSecondLedger,
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        ArSalesOrderPartialPrintId = aRInvoice.ArSalesOrderPartialPrintId,
                        SourceInvoiceId = aRInvoice.SourceInvoiceId,
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        Status = aRInvoice.Status,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR invoice", "Success", User.Identity.Name, $"updated invoice {entity.ArInvoiceId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoices_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoice aRInvoice)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.ARInvoices.Find(aRInvoice.ArInvoiceId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ArInvoice>()
                        .Local
                        .FirstOrDefault(f => f.ArInvoiceId == parent.ArInvoiceId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ArInvoice
                    {
                        ArInvoiceId = aRInvoice.ArInvoiceId,
                        CustomerName = aRInvoice.CustomerName,
                        CustomerId = aRInvoice.CustomerId,
                        TransactionClass = aRInvoice.TransactionClass,
                        Date = aRInvoice.Date,
                        PaymentTerm = aRInvoice.PaymentTerm,
                        DueDate = aRInvoice.DueDate,
                        LineAmount = aRInvoice.LineAmount,
                        TaxAmount = aRInvoice.TaxAmount,
                        TotalAmount = aRInvoice.TotalAmount,
                        AmountDue = aRInvoice.AmountDue,
                        InvoiceStatus = aRInvoice.InvoiceStatus,
                        IsSendForApproval = aRInvoice.IsSendForApproval,
                        IsOnlineTransferred = aRInvoice.IsOnlineTransferred,
                        IsOnlineTransferredBy = aRInvoice.IsOnlineTransferredBy,
                        OnlineTransferredTime = aRInvoice.OnlineTransferredTime,
                        BranchId = aRInvoice.BranchId,
                        Status = aRInvoice.Status,
                        IsManual = aRInvoice.IsManual,
                        IsPostedToSecondLedger = aRInvoice.IsPostedToSecondLedger,
                        ArSalesOrderId = aRInvoice.ArSalesOrderId,
                        ArSalesOrderPartialPrintId = aRInvoice.ArSalesOrderPartialPrintId,
                        IsOnlineApproved = aRInvoice.IsOnlineApproved,
                        OnlineApprovedBy = aRInvoice.OnlineApprovedBy,
                        OnlineApprovedTime = aRInvoice.OnlineApprovedTime,
                        IsVoid = aRInvoice.IsVoid,
                        VoidBy = aRInvoice.VoidBy,
                        VoidTime = aRInvoice.VoidTime,
                        OrgBranchName = aRInvoice.OrgBranchName,
                        DateCreated = aRInvoice.DateCreated,
                        LastModified = aRInvoice.LastModified,
                        CreatedBy = aRInvoice.CreatedBy,
                        ModifiedBy = aRInvoice.ModifiedBy
                    };

                    _db.ARInvoices.Attach(entity);
                    _db.ARInvoices.Remove(entity);
                    _db.SaveChanges();

                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AR Invoice" + aRInvoice.ArInvoiceId + " " + aRInvoice.Reference, Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRInvoice }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        public JsonResult SendforApproval(string id)
        {
            int myid = int.Parse(id);
            var record = _db.ARInvoices.Find(myid);
            #region check segment length validation

            var distributions = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
            {
                string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #region debit credit validation
                if (dist.Debit != 0 && dist.Credit != 0)
                {
                    message = $"{dist.AccountCode} Account has both debit and credit value.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit == 0 && dist.Credit == 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit > 0 && dist.Credit > 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit < 0 || dist.Credit < 0)
                {
                    message = $"{dist.AccountCode} Negative values are not allowed.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }


            #endregion
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "You cannot manipulate this record.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "This record is already sent for approval.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            decimal sumTotalinvoiceLines = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumTotalinvoiceLines = record.ArInvoiceLines.Sum(x => x.TotalAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            try
            {
                sumTotalDisDebit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Debit);
                sumTotalDisCredit = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == myid).Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //if (sumTotalDisDebit != sumTotalinvoiceLines)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Total of distribution and total of invoice lines is not equal.Please correct it and try again",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (!record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ARInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableInvoice)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.ARInvoices.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AR_Invoice.ToString())
                    .Where(x => x.SourceId == record.ArInvoiceId.ToString())
                    .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ArInvoiceDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ArInvoiceDistribution.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL
                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.Date.Date)
                    .Where(x => x.DateTo >= record.Date.Date).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                   
                    else 
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else 
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Invoice date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
               
                if (record.ArInvoiceDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AR Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "AR Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AR Invoice",
                        SourcesName = "AR",
                        JournalNumber = "ARINV." + record.ArInvoiceId.ToString(),
                        SourceId = record.ArInvoiceId.ToString(),
                        Source = JournalSources.AR_Invoice.ToString(),
                        EffectiveDate = record.Date.Date,
                        OrgBranchName = record.OrgBranchName,
                        BranchId=record.BranchId,
                        Status = "Approved",
                        Remark = record.Reference,
                        PaymentReference = "",
                        ReferenceDoc = record.Reference,
                        InvoiceReferenceDoc = "",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ArInvoiceDistribution)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion
                  
            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ARInvoices.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.ArInvoiceLines.Count > 0)
            {
                #region validate invoice

                var payments = _db.ARStandardCollectionLines.Where(x => x.InvoiceNumber == record.ArInvoiceId.ToString())
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The invoice number {record.ArInvoiceId} is already applied in payment \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region update originial invoice of credit memo

                var lines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == record.ArInvoiceId).ToList();

                if (lines.Any())
                {
                    decimal lineamount = Math.Round(lines.Sum(x => x.SubTotal), 2);
                    decimal taxAmount = Math.Round(lines.Sum(x => x.TaxAmount), 2);
                    decimal totalAmount = Math.Round(lines.Sum(x => x.TotalAmount), 2);

                    if (record.TransactionClass == TransactionClass.CreditMemo)
                    {
                        if (record.SourceInvoiceId != null)
                        {
                            #region update related invoice
                            var orignalInvoice = _db.ARInvoices.Find(record.SourceInvoiceId);
                            if (orignalInvoice != null)
                            {
                                orignalInvoice.InvoiceStatus = InvoiceStatus.Open;
                                orignalInvoice.LineAmount = orignalInvoice.LineAmount +lineamount;
                                orignalInvoice.TaxAmount = orignalInvoice.TaxAmount + taxAmount;
                                orignalInvoice.TotalAmount = orignalInvoice.TotalAmount + totalAmount;
                                orignalInvoice.AmountDue = orignalInvoice.AmountDue + totalAmount;
                                _db.Entry(orignalInvoice).State = EntityState.Modified;
                                _db.SaveChanges();
                            }

                            #endregion
                        }
                    }
                }

                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ARInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "receivable invoice", "Success", User.Identity.Name, $"void receivable invoice {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.ArInvoiceLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        private void SalesOrderInvoiceItems(int id)
        {
            var invoice = _db.ARInvoices.Find(id);
            var orderLines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == invoice.ArSalesOrderId).ToList();
            foreach (var orderLine in orderLines)
            {
                var orderitem = _db.ArProductService.Find(int.Parse(orderLine.SubItem));
               
               
                if (orderitem != null)
                {
                    #region get prev invoiced
                    decimal prevQty = 0;
                    decimal prevDiscount = 0;
                    var invoicedPrevs = _db.ARInvoices
                        .Where(x => x.ArSalesOrderId == invoice.ArSalesOrderId)
                        .Where(x => x.IsOnlineApproved&&x.IsVoid==false)
                        .ToList();
                    if (invoicedPrevs.Any())
                    {
                        foreach (var prev in invoicedPrevs)
                        {
                            var invoiceLines = _db.ARInvoiceLines
                                .Where(x=>x.ArInvoiceId==prev.ArInvoiceId)
                                .Where(x => x.ArSalesOrderLineId == orderLine.ArSalesOrderLineId).ToList();

                            prevQty = prevQty+invoiceLines.Sum(x => x.Quantity);
                            prevDiscount = prevDiscount+invoiceLines.Sum(x => x.Discount);
                        }
                       
                    }
                    #endregion

                    if (prevQty < orderLine.Quantity)
                    {
                        #region calculate
                        decimal qty = orderLine.Quantity - prevQty;
                        decimal discount = prevDiscount<orderLine.Discount? (orderLine.Discount - prevDiscount):0;
                        decimal salesPrice = qty * orderLine.UnitPrice;
                        decimal subTotal = salesPrice - discount;
                        decimal taxAmount=0;
                        var tax = _db.GLTaxRates.Find(orderLine.GLTaxRateID);
                        if (tax != null)
                        {
                            taxAmount = subTotal * tax.CalculatedRate;
                            taxAmount = Math.Round(taxAmount, 2);
                        }
                       decimal  totalAmount = taxAmount + subTotal;
                        #endregion
                       
                        var entity = new ArInvoiceLines
                        {
                            Items = orderitem.ArSetupProductService.Product,
                            ArInvoiceId = id,
                            Description = orderitem.ItemName,
                            AccountCode = orderLine.AccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(orderLine.AccountCode),
                            Quantity = qty,
                            UnitPrice = orderLine.UnitPrice,
                            Discount = discount,
                            SubTotal = subTotal,
                            GLTaxRateID = orderLine.GLTaxRateID,
                            TaxAmount = taxAmount,
                            TotalAmount = totalAmount,
                            ArSalesOrderLineId = orderLine.ArSalesOrderLineId,
                            OrgBranchName = orderLine.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = orderLine.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = orderLine.ModifiedBy
                        };

                        _db.ARInvoiceLines.Add(entity);
                    }
                }

                _db.SaveChanges();
            }

        }
        private void UpdateInvoiceTotals(int id)
        {
            var aRInvoice = _db.ARInvoices.Find(id);
            var lines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId == id).ToList();
            if (aRInvoice != null)
            {
                #region update parent total
                aRInvoice.InvoiceStatus = InvoiceStatus.Open;
                aRInvoice.LineAmount = lines.Sum(x => x.SubTotal);
                aRInvoice.TaxAmount = lines.Sum(x => x.TaxAmount);
                aRInvoice.TotalAmount = lines.Sum(x => x.TotalAmount);
                aRInvoice.AmountDue = aRInvoice.TotalAmount;
                _db.Entry(aRInvoice).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion
            }

           


        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
