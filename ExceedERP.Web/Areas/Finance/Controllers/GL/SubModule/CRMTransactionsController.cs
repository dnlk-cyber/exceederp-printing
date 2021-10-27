using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL.SubModule
{
    //[AuthorizeRoles(CRMRoles.CRMBatchTransfer)]
    public class CRMTransactionsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private CRMTransferHelper _transferHelper;
        private string _response = "SUCCESS";
        
        public ActionResult CashSalesInvoice()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.ToList();
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["VoucherList"] = db.SalesQuoteVouchers;
            ViewData["CartList"] = db.Carts;
            ViewData["PaymentMethod"] = db.PaymentMethods;
            var termList = db.Terms;
            var lookUpList = db.Lookups.ToList();
            foreach (Term term in termList)
            {
                Lookup selectedCaegory = lookUpList.Where(l => l.LookupId.ToString() == term.LookupId).ToList().FirstOrDefault();
                if (selectedCaegory != null)
                {
                    term.CategoryDescription = selectedCaegory.Description;
                }
            }

            ViewData["TermList"] = termList.AsQueryable();
            ViewData["SerialDefinitionList"] = db.SerialDefinitions;
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers;
            ViewData["StoreList"] = db.Stores;
            ViewData["VoucherExtensionList"] = db.VoucherExtensions;
            //ViewData["PeriodList"] = db.Periods;
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded && s.PaymentMethod != CRMConstants.CreditPayment);

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["PaymentTerms"] = db.PaymentTerms.Select(b => new { PaymentTermsID = b.PaymentTermsID, PaymentTermsDays = b.PaymentTermsDays + " Days" }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();

            return View();
        }
        public ActionResult CashSalesInvoices_Read([DataSourceRequest]DataSourceRequest request)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            var invoiceVouchers = db.CashSalesInvoices
                .Where(i => (i.PaymentMethod == CRMConstants.CashPaymentMethod && bIds.Contains(i.BranchId)  && i.IsOnlineApproved && !i.IsOnlineTransferred && i.IsManual) ||
                (i.PaymentMethod == CRMConstants.CashPaymentMethod && bIds.Contains(i.BranchId)  && !i.IsManual && i.IsOnlineApproved && i.IsReceiptPrinted && !i.IsOnlineTransferred)
                ).ToList();

            DataSourceResult result = invoiceVouchers.ToDataSourceResult(request, vou => new CashSalesInvoice
            {
                CashSalesInvoiceID = vou.CashSalesInvoiceID,
                Period = vou.Period,
                OrganizationCustomerId = vou.OrganizationCustomerId,
                Date = vou.Date,
                GrandTotal = vou.GrandTotal,
                Source = vou.Source,
                ShipmentGenerationMethod = vou.ShipmentGenerationMethod,
                IsIssued = vou.IsIssued,
                AdvancPayment = vou.AdvancPayment,
                OnHandPayment = vou.OnHandPayment,
                IssuedDate = vou.IssuedDate,
                IsVoid = vou.IsVoid,
                References = vou.References,
                CRMPeriodId = vou.CRMPeriodId,
                BranchId = vou.BranchId,
                StoreID = vou.StoreID,
                PaymentTermsID = vou.PaymentTermsID,
                ReceiptReference = vou.ReceiptReference,
                IsManual = vou.IsManual,
                IsPaidWithCash = vou.IsPaidWithCash,

                IsSendForApproval = vou.IsSendForApproval,
                PaymentMethod = vou.PaymentMethod,
                Remark = vou.Remark,
                LastObjectState = vou.LastObjectState,
                RecieveMethod = vou.RecieveMethod,
                ModifiedBy = vou.ModifiedBy,
                LastModified = vou.LastModified,
                DateCreated = vou.DateCreated,
                CreatedBy = vou.CreatedBy,
                IsOnlineApproved = vou.IsOnlineApproved,
                IsOnlineTransferred = vou.IsOnlineTransferred,
                IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                OnlineApprovedBy = vou.OnlineApprovedBy,
                OnlineApprovedTime = vou.OnlineApprovedTime,
                OnlineTransferredTime = vou.OnlineTransferredTime,
                OrgBranchName = vou.OrgBranchName,
                SendForApprovalBy = vou.SendForApprovalBy,
                SendForApprovalTime = vou.SendForApprovalTime,
                Status = vou.LastObjectState,

                IsSendForHold = vou.IsSendForHold,
                SendForHoldTime = vou.SendForHoldTime,
                SendForHoldBy = vou.SendForHoldBy,
                IsReceiptPrinted = vou.IsReceiptPrinted,
                InvoicePrintTime = vou.InvoicePrintTime,
                InvoicePrintedBy = vou.InvoicePrintedBy,
                InvoiceNumber = vou.InvoiceNumber,
                FsInvoiceNo = vou.FsInvoiceNo,
                MachineID = vou.MachineID,
                PosPrintType = vou.PosPrintType,
                CustomerTINNumber = vou.OrganizationCustomer.TinNo
            });
            return Json(result);


        }
        public JsonResult BatchtransferCashSalesInoice(string crmPeriodId)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            int did = int.Parse(crmPeriodId);
            var period = db.CRMPeriods.Find(did);

            int succeededCount = 0;
            int failedCount = 0;

            var untransffered = db.CashSalesInvoices.Where(x => x.CRMPeriodId == did && bIds.Contains(x.BranchId))
                .Where(x => (x.PaymentMethod == CRMConstants.CashPaymentMethod && x.IsOnlineApproved && x.IsManual && !x.IsOnlineTransferred /*&& x.IsVoid == false*/) ||
                (x.PaymentMethod == CRMConstants.CashPaymentMethod && x.IsOnlineApproved && !x.IsManual && x.IsReceiptPrinted && !x.IsOnlineTransferred /*&& x.IsVoid == false*/)
                ).ToList();

            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new CRMTransferHelper();
                _response = _transferHelper.CashSalesInvoice(record.CashSalesInvoiceID, "Bach Transfer", User.Identity.Name);
                if (_response == "SUCCESS") { succeededCount += 1; }
                else { failedCount += 1; }
                //Logger.Debug("Cash Sales transfer ID:" + record.CashSalesInvoiceID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module CRM Cash Sales", "Success", User.Identity.Name, $"batch transfer for period {crmPeriodId} ", Modules.CRM);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records found. {succeededCount} transfered, {failedCount} ignored.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreditSalesInvoice()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["VoucherExtension"] = db.VoucherExtensions.ToList();
            ViewData["Thing"] = db.Products.ToList();
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["VoucherList"] = db.SalesQuoteVouchers;
            ViewData["CartList"] = db.Carts;
            ViewData["PaymentMethod"] = db.PaymentMethods;
            var termList = db.Terms;
            var lookUpList = db.Lookups.ToList();
            foreach (Term term in termList)
            {
                Lookup selectedCaegory = lookUpList.Where(l => l.LookupId.ToString() == term.LookupId).ToList().FirstOrDefault();
                if (selectedCaegory != null)
                {
                    term.CategoryDescription = selectedCaegory.Description;
                }
            }

            ViewData["TermList"] = termList.AsQueryable();
            ViewData["SerialDefinitionList"] = db.SerialDefinitions;
            ViewData["VoucherReferenceList"] = db.SalesQuoteVouchers;
            ViewData["StoreList"] = db.Stores;
            ViewData["VoucherExtensionList"] = db.VoucherExtensions;
            ViewData["PeriodList"] = db.Periods;
            ViewData["SalesOrderVoucherList"] = db.SalesOrderVouchers.Where(s => s.LastObjectState != CRMConstants.Forwarded && s.PaymentMethod == CRMConstants.CreditPayment);

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["PaymentTerms"] = db.PaymentTerms.Select(b => new { PaymentTermsID = b.PaymentTermsID, PaymentTermsDays = b.PaymentTermsDays + " Days" }).ToList();
            //ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            ViewData["AddtionalCostTypes"] = db.AdditionalCosts.Select(a => new { a.AdditionalCostId, a.Name }).ToList();


            return View();
        }
        public ActionResult CreditsSalesInvoices_Read([DataSourceRequest]DataSourceRequest request)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            var invoiceVouchers = db.CashSalesInvoices
                .Where(i => (i.PaymentMethod == CRMConstants.CreditPaymentMethod && bIds.Contains(i.BranchId) /*&& !i.IsVoid*/ && i.IsOnlineApproved && !i.IsOnlineTransferred && i.IsManual) ||
                (i.PaymentMethod == CRMConstants.CreditPaymentMethod && bIds.Contains(i.BranchId) /*&& !i.IsVoid*/ && !i.IsManual && i.IsOnlineApproved && i.IsReceiptPrinted && !i.IsOnlineTransferred)
                ).ToList();

            DataSourceResult result = invoiceVouchers.ToDataSourceResult(request, vou => new CashSalesInvoice
            {
                CashSalesInvoiceID = vou.CashSalesInvoiceID,
                Period = vou.Period,
                OrganizationCustomerId = vou.OrganizationCustomerId,
                Date = vou.Date,
                GrandTotal = vou.GrandTotal,
                Source = vou.Source,
                ShipmentGenerationMethod = vou.ShipmentGenerationMethod,
                IsIssued = vou.IsIssued,
                AdvancPayment = vou.AdvancPayment,
                OnHandPayment = vou.OnHandPayment,
                IssuedDate = vou.IssuedDate,
                IsVoid = vou.IsVoid,
                References = vou.References,
                CRMPeriodId = vou.CRMPeriodId,
                BranchId = vou.BranchId,
                StoreID = vou.StoreID,
                PaymentTermsID = vou.PaymentTermsID,
                ReceiptReference = vou.ReceiptReference,
                IsManual = vou.IsManual,
                IsPaidWithCash = vou.IsPaidWithCash,

                IsSendForApproval = vou.IsSendForApproval,
                PaymentMethod = vou.PaymentMethod,
                Remark = vou.Remark,
                LastObjectState = vou.LastObjectState,
                RecieveMethod = vou.RecieveMethod,
                ModifiedBy = vou.ModifiedBy,
                LastModified = vou.LastModified,
                DateCreated = vou.DateCreated,
                CreatedBy = vou.CreatedBy,
                IsOnlineApproved = vou.IsOnlineApproved,
                IsOnlineTransferred = vou.IsOnlineTransferred,
                IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                OnlineApprovedBy = vou.OnlineApprovedBy,
                OnlineApprovedTime = vou.OnlineApprovedTime,
                OnlineTransferredTime = vou.OnlineTransferredTime,
                OrgBranchName = vou.OrgBranchName,
                SendForApprovalBy = vou.SendForApprovalBy,
                SendForApprovalTime = vou.SendForApprovalTime,
                Status = vou.LastObjectState,

                IsSendForHold = vou.IsSendForHold,
                SendForHoldTime = vou.SendForHoldTime,
                SendForHoldBy = vou.SendForHoldBy,
                IsReceiptPrinted = vou.IsReceiptPrinted,
                InvoicePrintTime = vou.InvoicePrintTime,
                InvoicePrintedBy = vou.InvoicePrintedBy,
                InvoiceNumber = vou.InvoiceNumber,
                FsInvoiceNo = vou.FsInvoiceNo,
                MachineID = vou.MachineID,
                PosPrintType = vou.PosPrintType,
                CustomerTINNumber = vou.OrganizationCustomer.TinNo
            });
            return Json(result);
        }
        public JsonResult BatchtransferCreditSalesInoice(string crmPeriodId)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            int did = int.Parse(crmPeriodId);
            var period = db.CRMPeriods.Find(did);

            int succeddedCount = 0;
            int failedCount = 0;
            var untransffered = db.CashSalesInvoices
                .Where(x => x.CRMPeriodId == did && bIds.Contains(x.BranchId))
                .Where(x => (x.PaymentMethod == CRMConstants.CreditPaymentMethod && x.IsOnlineApproved && x.IsManual && !x.IsOnlineTransferred /*&& x.IsVoid == false*/) ||
                (x.PaymentMethod == CRMConstants.CreditPaymentMethod && x.IsOnlineApproved && !x.IsManual && x.IsReceiptPrinted && !x.IsOnlineTransferred /*&& x.IsVoid == false*/)
                ).ToList();

            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new CRMTransferHelper();
                _response = _transferHelper.CreditSalesInvoice(record.CashSalesInvoiceID, "Bach Transfer", User.Identity.Name);
                if (_response == "SUCCESS")
                {
                    succeddedCount += 1;
                }
                else
                {
                    failedCount += 1;
                }
                //Logger.Debug("Cash Sales transfer ID:" + record.CashSalesInvoiceID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module CRM Credit Sales", "Success", User.Identity.Name, $"batch transfer for period {crmPeriodId} ", Modules.CRM);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records found. {succeddedCount} transfered, {failedCount} ignored.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Shipment()
        {
            ViewData["SalesInvoices"] = db.CashSalesInvoices.Select(s => new { CashSalesInvoiceID = s.CashSalesInvoiceID, InvoiceNumberDisplay = s.CashSalesInvoiceID }).ToList();
            ViewData["SalesOrderVouchers"] = db.SalesOrderVouchers.Select(s => new { SalesOrderVoucherID = s.SalesOrderVoucherID }).ToList();
            ViewData["StoreList"] = db.Stores;
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["Thing"] = db.Products.ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();

            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            return View();
        }
        public ActionResult Shipments_Read([DataSourceRequest]DataSourceRequest request)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            IQueryable<SalesShipment> shipments = db.SalesShipments.Where(s => s.IsOnlineApproved && bIds.Contains(s.BranchId) && !s.IsOnlineTransferred).OrderByDescending(s => s.SalesShipmentId);

            DataSourceResult result = shipments.ToDataSourceResult(request, shipment => new SalesShipment
            {
                SalesShipmentId = shipment.SalesShipmentId,
                ShipmentDate = shipment.ShipmentDate,
                CashSalesInvoiceID = shipment.CashSalesInvoiceID,
                SalesOrderVoucherID = shipment.SalesOrderVoucherID,
                Source = shipment.Source,
                OrganizationCustomerId = shipment.OrganizationCustomerId,
                BranchId = shipment.BranchId,
                StoreID = shipment.StoreID,
                CRMPeriodId = shipment.CRMPeriodId,
                MethodOfShipment = shipment.MethodOfShipment,
                PlateNo = shipment.PlateNo,
                FsInvoiceNumber = shipment.FsInvoiceNumber,
                TrailerPlateNo = shipment.TrailerPlateNo,

                Status = shipment.Status,
                IsVoid = shipment.IsVoid,
                IsOnlineApproved = shipment.IsOnlineApproved,
                OnlineApprovedBy = shipment.OnlineApprovedBy,
                IsOnlineTransferred = shipment.IsOnlineTransferred,
                IsOnlineTransferredBy = shipment.IsOnlineTransferredBy,
                OnlineApprovedTime = shipment.OnlineApprovedTime,
                OnlineTransferredTime = shipment.OnlineTransferredTime,
                IsSendForApproval = shipment.IsSendForApproval,
                SendForApprovalBy = shipment.SendForApprovalBy,
                SendForApprovalTime = shipment.SendForApprovalTime,
                VoidBy = shipment.VoidBy,
                VoidTime = shipment.VoidTime,

                CreatedBy = shipment.CreatedBy,
                DateCreated = shipment.DateCreated,
                LastModified = shipment.LastModified,
                ModifiedBy = shipment.ModifiedBy,
                OrgBranchName = shipment.OrgBranchName,
                Remark = shipment.Remark
            });

            return Json(result);
        }
        public JsonResult BatchtransferShipment(string crmPeriodId)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            int did = int.Parse(crmPeriodId);
            var period = db.CRMPeriods.Find(did);

            int succeededCount = 0;
            int failedCount = 0;

            var untransffered = db.SalesShipments
                .Where(s => s.CRMPeriodId == did && s.IsOnlineApproved && !s.IsOnlineTransferred && bIds.Contains(s.BranchId)).OrderByDescending(s => s.SalesShipmentId).ToList();


            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new CRMTransferHelper();
                _response = _transferHelper.Shipment(record.SalesShipmentId, "Bach Transfer", User.Identity.Name);
                if (_response == "SUCCESS") { succeededCount += 1; }
                else { failedCount += 1; }
                //Logger.Debug("Cash Sales transfer ID:" + record.CashSalesInvoiceID + _response);
            }

            #region auto job



            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module CRM Shipmentsm", "Success", User.Identity.Name, $"batch transfer for period {crmPeriodId} ", Modules.CRM);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records found. {succeededCount} transfered, {failedCount} ignored.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Deposit()
        {
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();
            ViewData["SalesStore"] = db.SalesStores;
            ViewData["PaymentMethod"] = db.PaymentMethods;

            return View();
        }
        public ActionResult CRMDeposit_Read([DataSourceRequest]DataSourceRequest request)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            IQueryable<CRMDeposit> crmdeposit = db.CRMDeposit.Where(i => bIds.Contains(i.BranchId)  && i.IsOnlineApproved && !i.IsOnlineTransferred);
            DataSourceResult result = crmdeposit.ToDataSourceResult(request, cRMDeposit => new
            {
                CRMDepositId = cRMDeposit.CRMDepositId,
                SalesPersonUserName = cRMDeposit.SalesPersonUserName,
                SalesPersonFullName = cRMDeposit.SalesPersonFullName,
                BranchId = cRMDeposit.BranchId,
                StoreID = cRMDeposit.StoreID,
                CRMPeriodId = cRMDeposit.CRMPeriodId,
                Date = cRMDeposit.Date,
                Status = cRMDeposit.Status,
                Remark = cRMDeposit.Remark,
                IsVoid = cRMDeposit.IsVoid,
                VoidBy = cRMDeposit.VoidBy,
                VoidTime = cRMDeposit.VoidTime,
                IsOnlineApproved = cRMDeposit.IsOnlineApproved,
                OnlineApprovedBy = cRMDeposit.OnlineApprovedBy,
                OnlineApprovedTime = cRMDeposit.OnlineApprovedTime,
                IsOnlineTransferred = cRMDeposit.IsOnlineTransferred,
                IsOnlineTransferredBy = cRMDeposit.IsOnlineTransferredBy,
                OnlineTransferredTime = cRMDeposit.OnlineTransferredTime,
                IsSendForApproval = cRMDeposit.IsSendForApproval,
                OrgBranchName = cRMDeposit.OrgBranchName,
                SendForApprovalBy = cRMDeposit.SendForApprovalBy,
                SendForApprovalTime = cRMDeposit.SendForApprovalTime,
                DateCreated = cRMDeposit.DateCreated,
                LastModified = cRMDeposit.LastModified,
                CreatedBy = cRMDeposit.CreatedBy,
                ModifiedBy = cRMDeposit.ModifiedBy
            });

            return Json(result);
        }
        public JsonResult BatchtransferDeposit(string crmPeriodId)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            int did = int.Parse(crmPeriodId);
            var period = db.CRMPeriods.Find(did);

            int succeededCount = 0;
            int failedCount = 0;

            var untransffered = db.CRMDeposit
                .Where(s => s.CRMPeriodId == did && s.IsOnlineApproved && !s.IsOnlineTransferred && bIds.Contains(s.BranchId)).OrderByDescending(s => s.CRMDepositId).ToList();


            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new CRMTransferHelper();
                _response = _transferHelper.Deposit(record.CRMDepositId, "Bach Transfer", User.Identity.Name);
                if (_response == "SUCCESS") { succeededCount += 1; }
                else { failedCount += 1; }
                //Logger.Debug("Cash Sales transfer ID:" + record.CashSalesInvoiceID + _response);
            }

            #region auto job
            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module CRM Deposit", "Success", User.Identity.Name, $"batch transfer for period {crmPeriodId} ", Modules.CRM);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records found. {succeededCount} transfered, {failedCount} ignored.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreditSettlement()
        {

            ViewData["PaymentMethod"] = db.PaymentMethods.ToList();
            ViewData["OrganizationCustomer"] = db.OrganizationCustomers.ToList();
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name }).ToList();
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["CRMPeriods"] = db.CRMPeriods.Select(b => new { b.CRMPeriodId, b.Name }).ToList();

            return View();
        }
        public ActionResult CreditSettlements_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<CreditSettlement> voulist = new List<CreditSettlement>();

            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            var creditSettlements = db.CreditSettlement.Where(s => bIds.Contains(s.BranchId) /*&& !s.IsVoid*/ && s.IsOnlineApproved && !s.IsOnlineTransferred).ToList();


            foreach (var vou in creditSettlements)
            {
                var CreditSales = db.CashSalesInvoices.Where(x => x.OrganizationCustomerId == vou.Customer && x.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();

                decimal GrandTotal = decimal.Zero;
                decimal RemainingAmount = decimal.Zero;
                decimal totalSettlementAmount = decimal.Zero;

                var SettlementLine = db.LineCreditSettlements.Where(x => x.CreditSettlementId == vou.CreditSettlementId).ToList();
                totalSettlementAmount = SettlementLine.Any() ? SettlementLine.Sum(l => l.SettelmentAmount) : 0;

                if (CreditSales.Any())
                {
                    GrandTotal = CreditSales.Sum(s => s.GrandTotal);
                    RemainingAmount = CreditSales.Sum(s => s.AmountDue);
                }

                //RemainingAmount = GrandTotal - totalSettlementAmount;
                string SalesStatus = string.Empty;


                voulist.Add(new CreditSettlement
                {
                    CreditSettlementId = vou.CreditSettlementId,
                    Customer = vou.Customer,
                    Date = vou.Date,
                    GrandTotal = GrandTotal,
                    SettlementAmount = totalSettlementAmount,
                    Status = vou.Status,
                    Remaining = RemainingAmount,
                    Remark = vou.Remark,
                    CRMPeriodId = vou.CRMPeriodId,
                    ReceiptNumber = vou.ReceiptNumber,

                    BranchId = vou.BranchId,
                    RecieveMethod = vou.RecieveMethod,
                    ModifiedBy = vou.ModifiedBy,
                    LastModified = vou.LastModified,
                    DateCreated = vou.DateCreated,
                    CreatedBy = vou.CreatedBy,
                    IsOnlineApproved = vou.IsOnlineApproved,
                    IsOnlineTransferred = vou.IsOnlineTransferred,
                    IsOnlineTransferredBy = vou.IsOnlineTransferredBy,
                    OnlineApprovedBy = vou.OnlineApprovedBy,
                    OnlineApprovedTime = vou.OnlineApprovedTime,
                    OnlineTransferredTime = vou.OnlineTransferredTime,
                    OrgBranchName = vou.OrgBranchName,
                    SendForApprovalBy = vou.SendForApprovalBy,
                    IsSendForApproval = vou.IsSendForApproval,
                    IsVoid = vou.IsVoid,
                    SendForApprovalTime = vou.SendForApprovalTime
                });
            }

            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public JsonResult BatchtransferCreditSettlement(string crmPeriodId)
        {
            var userBranches = new ApplicationBranchManager().GetUsersBranchByName(User.Identity.Name);
            var bIds = new HashSet<int>(userBranches.Select(b => b.BranchId));

            int did = int.Parse(crmPeriodId);
            var period = db.CRMPeriods.Find(did);

            int succeededCount = 0;
            int failedCount = 0;

            var untransffered = db.CreditSettlement
                .Where(s => s.CRMPeriodId == did /*&& !s.IsVoid*/ && s.IsOnlineApproved && !s.IsOnlineTransferred && bIds.Contains(s.BranchId)).OrderByDescending(s => s.CreditSettlementId).ToList();


            if (!untransffered.Any())
            {
                var formattedData1 = new
                {
                    Message = "Data not found for transffer.",
                    Status = "NOT_OK"
                };
                return Json(formattedData1, JsonRequestBehavior.AllowGet);
            }

            foreach (var record in untransffered)
            {
                _transferHelper = new CRMTransferHelper();
                _response = _transferHelper.CreditSettlement(record.CreditSettlementId, "Bach Transfer", User.Identity.Name);
                if (_response == "SUCCESS") { succeededCount += 1; }
                else { failedCount += 1; }
                //Logger.Debug("Cash Sales transfer ID:" + record.CashSalesInvoiceID + _response);
            }

            #region auto job
            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "GL sub module CRM Deposit", "Success", User.Identity.Name, $"batch transfer for period {crmPeriodId} ", Modules.CRM);
            #endregion

            var formattedData = new
            {
                Message = $"Operation Completed. {untransffered.Count} records found. {succeededCount} transfered, {failedCount} ignored.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

    }
}