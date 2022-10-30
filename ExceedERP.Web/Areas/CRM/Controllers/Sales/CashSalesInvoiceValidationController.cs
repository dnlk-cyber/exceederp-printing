using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesApprover, CRMRoles.CashSalesUser)]
    public class CashSalesInvoiceValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Validations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CashSalesInvoiceValidation> validations = db.CashSalesInvoiceValidation.Where(v => v.CashSalesInvoiceId == id);
            DataSourceResult result = validations.ToDataSourceResult(request, validation => new
            {
                validation.CashSalesInvoiceValidationId,
                validation.CashSalesInvoiceId,

                validation.OrgBranchName,
                validation.DateCreated,
                validation.LastModified,
                validation.CreatedBy,
                validation.ModifiedBy,

                validation.Status,
                validation.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Create([DataSourceRequest]DataSourceRequest request, CashSalesInvoiceValidation validation, int id)
        {
            var record = db.CashSalesInvoices.Find(id);

            #region Checking the  record

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }
                if (!record.IsSendForApproval)
                {
                    string message = "Record is not Waiting Approval. Operation Canceled";
                    ModelState.AddModelError("not_sent_for_approval", message);
                }

                if (validation.Status == ApprovalStatuses.Reject)
                {
                    record.IsSendForApproval = false;
                    record.LastObjectState = CRMConstants.Rejected;
                    record.Status = "Rejected";
                }

            }
            else
            {
                ModelState.AddModelError("Not Found", "Sales Invoice  record not found");
            }
            #endregion

            if (ModelState.IsValid)
            {
                if (record != null)
                {
                    if (validation.Status == ApprovalStatuses.Approve)
                    {
                        var success = UpdateChangesOnSalesInvoiceApprove(record, record.PaymentMethod);
                        if (success)
                        {
                            #region update
                            record.LastObjectState = CRMVoucherStatusConstatns.Approved;
                            record.IsOnlineApproved = true;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            #endregion

                            // trigger shipment modifications  if invoice is from maintenance
                            //CRMMaintenanceIntegrationHelper.UpdateMaintenanceShipmentOnInvoiceApprove(invoice: record, userName: User.Identity.Name);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Something Went Wrong in Processing the Transaction!");
                        }
                    }
                    if (validation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        record.LastObjectState = CRMConstants.Rejected;

                    }
                    db.CashSalesInvoices.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    var res = db.SaveChanges();
                    if (res > 0 && ModelState.IsValid)
                    {
                        var entity = new CashSalesInvoiceValidation
                        {
                            //ReceivableInvoiceId = validation.ReceivableInvoiceId,
                            CashSalesInvoiceId = id,
                            Status = validation.Status,
                            Remark = validation.Remark,

                            OrgBranchName = validation.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = validation.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = validation.ModifiedBy,
                        };

                        db.CashSalesInvoiceValidation.Add(entity);
                        db.SaveChanges();

                        validation.CashSalesInvoiceValidationId = entity.CashSalesInvoiceValidationId;
                    }
                }


            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Update([DataSourceRequest]DataSourceRequest request, CashSalesInvoiceValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashSalesInvoiceValidation
                {
                    CashSalesInvoiceValidationId = validation.CashSalesInvoiceValidationId,
                    CashSalesInvoiceId = validation.CashSalesInvoiceId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.CashSalesInvoiceValidation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Validation_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesInvoiceValidation validation)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashSalesInvoiceValidation
                {
                    CashSalesInvoiceValidationId = validation.CashSalesInvoiceValidationId,
                    CashSalesInvoiceId = validation.CashSalesInvoiceId,
                    Status = validation.Status,
                    Remark = validation.Remark,

                    OrgBranchName = validation.OrgBranchName,
                    DateCreated = validation.DateCreated,
                    LastModified = validation.LastModified,
                    CreatedBy = validation.CreatedBy,
                    ModifiedBy = validation.ModifiedBy,
                };

                db.CashSalesInvoiceValidation.Attach(entity);
                db.CashSalesInvoiceValidation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { validation }.ToDataSourceResult(request, ModelState));
        }

        private bool UpdateChangesOnSalesInvoiceApprove(CashSalesInvoice invoice, string type)
        {


            bool transactionCompletedSuccessfully = false;

            if (type == CRMConstants.CreditPaymentMethod)
            {
                transactionCompletedSuccessfully = ProcessCreditInvoiceApproval(invoice);
            }

            else if (type == CRMConstants.CashPaymentMethod)
            {
                transactionCompletedSuccessfully = ProcessCashInvoiceApproval(invoice);
            }
            return transactionCompletedSuccessfully;
        }

        private bool ProcessCashInvoiceApproval(CashSalesInvoice invoice)
        {
            var successful = false;
            CRMSalesHelper helper = new CRMSalesHelper();

            //var AdvancePaymentLines = db.AdvancePaymentLines.ToList();
            //var AdvancePayments = db.AdvancePayments.ToList();

            // create shipment for Manual-Purchase Invoices with auto-generate shipment
            if (invoice.IsManual && invoice.ShipmentGenerationMethod == ShipmentGenerationMethod.AutoGenerate)
            {
                //CRMSalesHelper.SendSalesToBankReconciliation(invoice, User.Identity.Name);

                CRMSalesHelper.CreateShipmentRecordFromInvoice(invoice, User.Identity.Name);
            }

            List<LineItem> InvoiceItems =
                                           db.LineItems.Where(
                                               l => l.Voucher == invoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2")
                                               .ToList();
            if (InvoiceItems.Any())
            {
                foreach (var InvoiceItem in InvoiceItems)
                {
                    InvoiceItem.Status = CRMConstants.Paid;

                    db.LineItems.Attach(InvoiceItem);
                    db.Entry(InvoiceItem).State = EntityState.Modified;
                    db.SaveChanges();

                }

                //var AdvanceResult = helper.ProcessAdvancePayment(invoice, InvoiceItems, AdvancePaymentLines, AdvancePayments);

                //if (AdvanceResult != null && AdvanceResult.Item1 != null && AdvanceResult.Item1.Any())
                //{
                //    ExceedDbContext exceedDb = new ExceedDbContext();
                //    foreach (var item in AdvanceResult.Item1)
                //    {
                //        var Adlines = exceedDb.AdvancePaymentLines.Where(x => x.AdvancePaymentLineId == item.AdvancePaymentLineId).FirstOrDefault();
                //        if (Adlines != null)
                //        {
                //            Adlines.Value = item.Amount;
                //            exceedDb.AdvancePaymentLines.Attach(Adlines);
                //            exceedDb.Entry(Adlines).State = System.Data.Entity.EntityState.Modified;
                //            exceedDb.SaveChanges();
                //        }
                //    }
                //    // db.AdvancePaymentLogs.AddRange(AdvanceResult.Item1);

                //    db.SaveChanges();
                //}

                invoice.LastObjectState = CRMConstants.Forwarded;

                //invoice.OnHandPayment = InvoiceItems.Sum(x => x.TotalAmount) - AdvanceResult.Item2;
                //invoice.AdvancPayment = AdvanceResult.Item2;
                invoice.IsOnlineApproved = true;
                invoice.OnlineApprovedBy = User.Identity.Name;
                invoice.OnlineApprovedTime = DateTime.Now;

                db.CashSalesInvoices.Attach(invoice);
                db.Entry(invoice).State = EntityState.Modified;

                var res = db.SaveChanges();

                if (res == 1)
                {
                    successful = true;
                }
            }
            return successful;
        }

        private bool ProcessCreditInvoiceApproval(CashSalesInvoice invoice)
        {
            bool transactionSucceeded = false;

            var currentInvoice = db.CashSalesInvoices.Find(invoice.CashSalesInvoiceID);
            var GLTaxRates = db.GLTaxRates.ToList();
            var PaymentMethods = db.PaymentMethods.ToList();
            var AdditionalCostLogs = db.AdditionalCostLogs.ToList();
            var AdditionalCosts = db.AdditionalCosts.ToList();
            var AccountMaps = db.AccountMaps.ToList();
            if (currentInvoice == null)
            {
                ModelState.AddModelError("", "Invoice not found!");
                transactionSucceeded = false;
            }
            else
            {
                OrganizationCustomer OrgCustomer = db.OrganizationCustomers.Where(c => c.OrganizationCustomerID == currentInvoice.OrganizationCustomerId).FirstOrDefault();

                if (OrgCustomer != null && OrgCustomer.IsCredit)
                {

                    CreditSetting creditSetting =
                        db.CreditSettings.Where(c => c.OrganizationID == OrgCustomer.OrganizationCustomerID)
                            .OrderByDescending(c => c.CreditSettingID)
                            .FirstOrDefault();
                    if (creditSetting != null)
                    {
                        if (creditSetting.Quantity < currentInvoice.GrandTotal)
                        {
                            ModelState.AddModelError("InsufficentCredit",
                                "Customer has Insufficient Credit");

                            transactionSucceeded = false;

                        }
                        else
                        {

                            List<LineItem> InvoiceItems =
                                db.LineItems.Where(
                                    l => l.Voucher == currentInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2")
                                    .ToList();

                            if (InvoiceItems != null)
                            {
                                foreach (var InvoiceItem in InvoiceItems)
                                {

                                    InvoiceItem.Status = CRMConstants.CreditPayment;
                                    db.LineItems.Attach(InvoiceItem);
                                    db.Entry(InvoiceItem).State = EntityState.Modified;
                                    db.SaveChanges();
                                }

                                currentInvoice.LastObjectState = CRMConstants.PaymentReceived;
                                db.CashSalesInvoices.Attach(currentInvoice);
                                db.Entry(currentInvoice).State = EntityState.Modified;
                                db.SaveChanges();

                                decimal FinalCredit = decimal.Parse(creditSetting.Quantity.ToString()) -
                                                currentInvoice.GrandTotal;

                                creditSetting.Quantity = FinalCredit;
                                db.CreditSettings.Attach(creditSetting);
                                db.Entry(creditSetting).State = EntityState.Modified;
                                db.SaveChanges();



                                var CreditInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == currentInvoice.CashSalesInvoiceID).FirstOrDefault();
                                if (CreditInvoice != null)
                                {
                                    CreditInvoice.IsOnlineApproved = true;
                                    CreditInvoice.OnlineApprovedBy = User.Identity.Name;
                                    CreditInvoice.OnlineApprovedTime = DateTime.Now;


                                    db.CashSalesInvoices.Attach(CreditInvoice);
                                    db.Entry(CreditInvoice).State = EntityState.Modified;
                                    var result = db.SaveChanges();
                                    if (result >= 1)
                                    {
                                        transactionSucceeded = true;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "No Credit Setting was found for this Customer!");
                    }
                }
            }
            return transactionSucceeded;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}