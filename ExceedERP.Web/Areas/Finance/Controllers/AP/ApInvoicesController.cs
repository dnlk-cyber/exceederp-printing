using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApInvoicesController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper _accountHelper = new AccountHelper();
        private TaxHelper _taxHelper = new TaxHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            PopulateViewData();


            return View();
        }

        private void PopulateViewData()
        {

            var terms = _db.PaymentTerms;

            ViewData["terms"] = terms.Select(e => new
            {
                PaymentTermsDays = e.PaymentTermsDays
            }).AsQueryable();

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new
            {
                SupplierId = c.SupplierId,
                Name = c.Name
            }).AsQueryable();

            var tax = _db.GLTaxRates.Where(x => x.ApplyToAp).ToList();

            ViewData["tax"] = tax;

            var productorService = _db.ApSetupItems.ToList();

            ViewData["productorServices"] = productorService
                .Select(e => new
                {
                    Name = e.Name
                })
                .AsQueryable();


            var witholdings = _db.GLTaxRates.Where(x => x.ApplyToWithholding).ToList();

            ViewData["witholdingType"] = witholdings
                .Select(e => new
                {
                    TaxName = e.TaxName
                })
                .AsQueryable();

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        public ActionResult APPaybleInvoices_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApPaybleInvoices> appaybleinvoices = _db.APPaybleInvoices
                .Where(x=>x.IsManual==false)
                .Where(x => mybraches.Contains(x.OrgBranchName));

            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApInvoiceTranferableList();
                appaybleinvoices =
                    appaybleinvoices.Where(x => transfferableList.Contains(x.ApPaybleInvoicesId));
            }
            DataSourceResult result = appaybleinvoices.ToDataSourceResult(request, aPPaybleInvoices => new
            {
                ApPaybleInvoicesId = aPPaybleInvoices.ApPaybleInvoicesId,
                SourceId = aPPaybleInvoices.SourceId,
                IsManual = aPPaybleInvoices.IsManual,
                SupplierName = aPPaybleInvoices.SupplierName,
                IsNewSupplier = aPPaybleInvoices.IsNewSupplier,
                SupplierId = aPPaybleInvoices.SupplierId,
                EmployeeId = aPPaybleInvoices.EmployeeId,
                Type = aPPaybleInvoices.Type,
                PrNo = aPPaybleInvoices.PrNo,
                PoNum = aPPaybleInvoices.PoNum,
                PurposeOfPayment = aPPaybleInvoices.PurposeOfPayment,
                InvoiceDate = aPPaybleInvoices.InvoiceDate,
                InvoiceNumber = aPPaybleInvoices.InvoiceNumber,
                PaymentTerms = aPPaybleInvoices.PaymentTerms,
                InvoiceAmount = aPPaybleInvoices.InvoiceAmount,
                AdvanceAmount = aPPaybleInvoices.AdvanceAmount,
                TaxAmount = aPPaybleInvoices.TaxAmount,
                WithholdingAmount = aPPaybleInvoices.WithholdingAmount,
                Subtotal = aPPaybleInvoices.Subtotal,
                WithholdTaxAmount = aPPaybleInvoices.WithholdTaxAmount,
                NetPayable = aPPaybleInvoices.NetPayable,
                AmountDue = aPPaybleInvoices.AmountDue,
                InvoiceStatus = aPPaybleInvoices.InvoiceStatus,
                IsPostedToSecondLedger = aPPaybleInvoices.IsPostedToSecondLedger,

                Status = aPPaybleInvoices.Status,
                Source = aPPaybleInvoices.Source,
                Grn = aPPaybleInvoices.Grn,

                IsSendForApproval = aPPaybleInvoices.IsSendForApproval,
                SendForApprovalBy = aPPaybleInvoices.SendForApprovalBy,
                SendForApprovalTime = aPPaybleInvoices.SendForApprovalTime,

                IsOnlineApproved = aPPaybleInvoices.IsOnlineApproved,
                OnlineApprovedBy = aPPaybleInvoices.OnlineApprovedBy,
                OnlineApprovedTime = aPPaybleInvoices.OnlineApprovedTime,


                IsOnlineTransferred = aPPaybleInvoices.IsOnlineTransferred,
                IsOnlineTransferredBy = aPPaybleInvoices.IsOnlineTransferredBy,
                OnlineTransferredTime = aPPaybleInvoices.OnlineTransferredTime,

                IsVoid = aPPaybleInvoices.IsVoid,
                VoidBy = aPPaybleInvoices.VoidBy,
                VoidTime = aPPaybleInvoices.VoidTime,
                OrgBranchName = aPPaybleInvoices.OrgBranchName,
                DateCreated = aPPaybleInvoices.DateCreated,
                LastModified = aPPaybleInvoices.LastModified,
                CreatedBy = aPPaybleInvoices.CreatedBy,
                ModifiedBy = aPPaybleInvoices.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Create([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices aPPaybleInvoices)
        {
            #region validation
            #region advance payment

            var paymentRequest = _db.ContractPayments.FirstOrDefault(x => x.ContractPaymentId.ToString() == aPPaybleInvoices.SourceId);
            if (paymentRequest != null)
            {
                var contract = _db.ContractManagements.Find(paymentRequest.ContractManagementID);

                var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
                    x.PurchaseReference == contract.ProcurementRefNo.ToString()).ToList();

                aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);
            }

           

            #endregion

            #endregion

            #region old validation

            //string grn = aPPaybleInvoices.Grn.Split('_')[1];
            //string grnType = aPPaybleInvoices.Grn.Split('_')[0];
            //int intId = int.Parse(grn);
            //switch (grnType)
            //{
            //    case "STOCK":
            //        var goodReceive = _db.GoodReceives.Find(intId);
            //        var exists = _db.APPaybleInvoices.Where(x=>x.Source==GrnSource.StockGrn).Where(x => x.Grn == goodReceive.GoodReceiveId.ToString()).ToList();
            //        if(exists.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (goodReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = goodReceive.GoodReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = goodReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.StockGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(goodReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x=>x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(goodReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //    case "SERVICE":
            //        var serviceReceive = _db.ServiceReceives.Find(intId);
            //        var existsservice = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.ServiceGrn).Where(x => x.Grn == serviceReceive.ServiceReceiveId.ToString()).ToList();
            //        if (existsservice.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (serviceReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = serviceReceive.ServiceReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = serviceReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.ServiceGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(serviceReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(serviceReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(serviceReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //    case "FIXEDASSET":
            //        var fixedAssetReceive = _db.FixedAssetReceives.Find(intId);
            //        var existsFixedAsset = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.FixedssetGrn).Where(x => x.Grn == fixedAssetReceive.FixedAssetReceiveId.ToString()).ToList();
            //        if (existsFixedAsset.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (fixedAssetReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = fixedAssetReceive.FixedAssetReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = fixedAssetReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.FixedssetGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(fixedAssetReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //}



            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApPaybleInvoices
                {
                    IsManual = false,
                    SourceId=aPPaybleInvoices.SourceId,
                    SupplierName = aPPaybleInvoices.SupplierName,
                    SupplierId = aPPaybleInvoices.SupplierId,
                    IsNewSupplier = aPPaybleInvoices.IsNewSupplier,
                    EmployeeId = aPPaybleInvoices.EmployeeId,
                    Type = aPPaybleInvoices.Type,
                    PrNo = aPPaybleInvoices.PrNo,
                    PoNum = aPPaybleInvoices.PoNum,
                    PurposeOfPayment = aPPaybleInvoices.PurposeOfPayment,
                    InvoiceDate = aPPaybleInvoices.InvoiceDate.Date,
                    InvoiceNumber = aPPaybleInvoices.InvoiceNumber,
                    PaymentTerms = aPPaybleInvoices.PaymentTerms,
                    InvoiceAmount = aPPaybleInvoices.InvoiceAmount,
                    AdvanceAmount = aPPaybleInvoices.AdvanceAmount,
                    TaxAmount = aPPaybleInvoices.TaxAmount,
                    WithholdingAmount = aPPaybleInvoices.WithholdingAmount,
                    Subtotal = aPPaybleInvoices.Subtotal,
                    WithholdTaxAmount = aPPaybleInvoices.WithholdTaxAmount,
                    NetPayable = aPPaybleInvoices.NetPayable,
                    AmountDue = aPPaybleInvoices.AmountDue,
                    InvoiceStatus = aPPaybleInvoices.InvoiceStatus,
                    IsPostedToSecondLedger = aPPaybleInvoices.IsPostedToSecondLedger,

                    IsSendForApproval = aPPaybleInvoices.IsSendForApproval,
                    IsOnlineTransferred = aPPaybleInvoices.IsOnlineTransferred,
                    IsOnlineTransferredBy = aPPaybleInvoices.IsOnlineTransferredBy,
                    OnlineTransferredTime = aPPaybleInvoices.OnlineTransferredTime,
                    Status = aPPaybleInvoices.Status,
                    Source = aPPaybleInvoices.Source,
                    Grn = aPPaybleInvoices.Grn,
                    IsOnlineApproved = aPPaybleInvoices.IsOnlineApproved,
                    OnlineApprovedBy = aPPaybleInvoices.OnlineApprovedBy,
                    OnlineApprovedTime = aPPaybleInvoices.OnlineApprovedTime,
                    IsVoid = aPPaybleInvoices.IsVoid,
                    VoidBy = aPPaybleInvoices.VoidBy,
                    VoidTime = aPPaybleInvoices.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = aPPaybleInvoices.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPPaybleInvoices.ModifiedBy
                };

                _db.APPaybleInvoices.Add(entity);
                _db.SaveChanges();
                aPPaybleInvoices.ApPaybleInvoicesId = entity.ApPaybleInvoicesId;

                GetContractGrnItems(entity);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Invoice", "Success", User.Identity.Name, $"created invoice {entity.ApPaybleInvoicesId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPPaybleInvoices }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Update([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices aPPaybleInvoices)
        {

            #region validation
            #region advance payment

            var paymentRequest = _db.ContractPayments.FirstOrDefault(x => x.ContractPaymentId.ToString() == aPPaybleInvoices.SourceId);
            if (paymentRequest != null)
            {
                var contract = _db.ContractManagements.Find(paymentRequest.ContractManagementID);

                var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
                    x.PurchaseReference == contract.ProcurementRefNo.ToString()).ToList();

                aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);
            }

            #endregion
            #endregion

            #region old validation

            //string grn = aPPaybleInvoices.Grn.Split('_')[1];
            //string grnType = aPPaybleInvoices.Grn.Split('_')[0];
            //int intId = int.Parse(grn);
            //switch (grnType)
            //{
            //    case "STOCK":
            //        var goodReceive = _db.GoodReceives.Find(intId);
            //        var exists = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.StockGrn).Where(x => x.Grn == goodReceive.GoodReceiveId.ToString()).ToList();
            //        if (exists.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (goodReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = goodReceive.GoodReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = goodReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.StockGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(goodReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(goodReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //    case "SERVICE":
            //        var serviceReceive = _db.ServiceReceives.Find(intId);
            //        var existsservice = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.ServiceGrn).Where(x => x.Grn == serviceReceive.ServiceReceiveId.ToString()).ToList();
            //        if (existsservice.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (serviceReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = serviceReceive.ServiceReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = serviceReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.ServiceGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(serviceReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(serviceReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(serviceReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //    case "FIXEDASSET":
            //        var fixedAssetReceive = _db.FixedAssetReceives.Find(intId);
            //        var existsFixedAsset = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.FixedssetGrn).Where(x => x.Grn == fixedAssetReceive.FixedAssetReceiveId.ToString()).ToList();
            //        if (existsFixedAsset.Any())
            //            ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


            //        #region get data
            //        if (fixedAssetReceive != null)
            //        {
            //            aPPaybleInvoices.Grn = fixedAssetReceive.FixedAssetReceiveId.ToString();
            //            aPPaybleInvoices.PoNum = fixedAssetReceive.ReceiveRef.ToString();
            //            aPPaybleInvoices.Source = GrnSource.FixedssetGrn;

            //            ReceiveType type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
            //            if (type != null)
            //            {
            //                switch (type.Type)
            //                {
            //                    case GoodReceiveType.PurchaseOrder:
            //                        int pid = int.Parse(fixedAssetReceive.ReceiveRef);
            //                        PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
            //                        if (purchaseOrder != null)
            //                        {
            //                            aPPaybleInvoices.PrNo = purchaseOrder.PurchaseRequisitionID.ToString();
            //                            aPPaybleInvoices.EmployeeId = purchaseOrder.ProcurementOfficer;
            //                            aPPaybleInvoices.SupplierId = purchaseOrder.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = purchaseOrder.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = purchaseOrder.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == purchaseOrder.PurchaseOrderID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                    case GoodReceiveType.Tender:
            //                        int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
            //                        BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
            //                        if (award != null)
            //                        {
            //                            //not possible to get pr since merged prs
            //                            //prno = award.Lot.pu.ToString();
            //                            var lot = _db.Lots.Find(award.LotID);
            //                            if (lot != null)
            //                            {
            //                                var tender = _db.Tenders.Find(lot.TenderID);
            //                                if (tender != null) aPPaybleInvoices.EmployeeId = tender.ProcurementOfficer;
            //                            }
            //                            aPPaybleInvoices.SupplierId = award.SupplierId;
            //                            aPPaybleInvoices.IsNewSupplier = award.IsNewSupplier;
            //                            aPPaybleInvoices.SupplierName = award.SupplierName;

            //                            #region advance payment

            //                            var appayment = _db.APPayments.Where(x => x.IsPrepayment).Where(x =>
            //                                x.PurchaseReference == award.BidAwardedSupplierID.ToString()).ToList();

            //                            aPPaybleInvoices.AdvanceAmount = appayment.Sum(x => x.PrepaymentAmount);

            //                            #endregion
            //                        }
            //                        break;
            //                }
            //            }
            //        }
            //        #endregion
            //        break;

            //}

            #endregion

            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.APPaybleInvoices.Find(aPPaybleInvoices.ApPaybleInvoicesId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ApPaybleInvoices>()
                        .Local
                        .FirstOrDefault(f => f.ApPaybleInvoicesId == parent.ApPaybleInvoicesId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoices
                    {
                        ApPaybleInvoicesId = aPPaybleInvoices.ApPaybleInvoicesId,
                        SourceId = aPPaybleInvoices.SourceId,
                        IsManual = aPPaybleInvoices.IsManual,
                        SupplierName = aPPaybleInvoices.SupplierName,
                        IsNewSupplier = aPPaybleInvoices.IsNewSupplier,
                        SupplierId = aPPaybleInvoices.SupplierId,
                        EmployeeId = aPPaybleInvoices.EmployeeId,
                        Type = aPPaybleInvoices.Type,
                        PrNo = aPPaybleInvoices.PrNo,
                        PoNum = aPPaybleInvoices.PoNum,
                        PurposeOfPayment = aPPaybleInvoices.PurposeOfPayment,
                        InvoiceDate = aPPaybleInvoices.InvoiceDate,
                        InvoiceNumber = aPPaybleInvoices.InvoiceNumber,
                        PaymentTerms = aPPaybleInvoices.PaymentTerms,
                        InvoiceAmount = aPPaybleInvoices.InvoiceAmount,
                        AdvanceAmount = aPPaybleInvoices.AdvanceAmount,
                        TaxAmount = aPPaybleInvoices.TaxAmount,
                        WithholdingAmount = aPPaybleInvoices.WithholdingAmount,
                        Subtotal = aPPaybleInvoices.Subtotal,
                        WithholdTaxAmount = aPPaybleInvoices.WithholdTaxAmount,
                        NetPayable = aPPaybleInvoices.NetPayable,
                        AmountDue = aPPaybleInvoices.AmountDue,
                        InvoiceStatus = aPPaybleInvoices.InvoiceStatus,
                        IsPostedToSecondLedger = aPPaybleInvoices.IsPostedToSecondLedger,

                        IsSendForApproval = aPPaybleInvoices.IsSendForApproval,
                        IsOnlineTransferred = aPPaybleInvoices.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPPaybleInvoices.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPPaybleInvoices.OnlineTransferredTime,
                        Status = aPPaybleInvoices.Status,
                        Source = aPPaybleInvoices.Source,
                        IsOnlineApproved = aPPaybleInvoices.IsOnlineApproved,
                        OnlineApprovedBy = aPPaybleInvoices.OnlineApprovedBy,
                        OnlineApprovedTime = aPPaybleInvoices.OnlineApprovedTime,
                        IsVoid = aPPaybleInvoices.IsVoid,
                        VoidBy = aPPaybleInvoices.VoidBy,
                        VoidTime = aPPaybleInvoices.VoidTime,
                        OrgBranchName = aPPaybleInvoices.OrgBranchName,
                        DateCreated = aPPaybleInvoices.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPPaybleInvoices.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                   
                    _db.APPaybleInvoices.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    GetContractGrnItems(entity);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP Invoice", "Success", User.Identity.Name, $"updated invoice {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPPaybleInvoices }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoices_Destroy([DataSourceRequest]DataSourceRequest request, ApPaybleInvoices aPPaybleInvoices)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.APPaybleInvoices.Find(aPPaybleInvoices.ApPaybleInvoicesId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ApPaybleInvoices>()
                        .Local
                        .FirstOrDefault(f => f.ApPaybleInvoicesId == parent.ApPaybleInvoicesId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoices
                    {
                        ApPaybleInvoicesId = aPPaybleInvoices.ApPaybleInvoicesId,
                        IsManual = aPPaybleInvoices.IsManual,
                        SupplierName = aPPaybleInvoices.SupplierName,
                        SupplierId = aPPaybleInvoices.SupplierId,
                        EmployeeId = aPPaybleInvoices.EmployeeId,
                        Type = aPPaybleInvoices.Type,
                        PrNo = aPPaybleInvoices.PrNo,
                        PoNum = aPPaybleInvoices.PoNum,
                        PurposeOfPayment = aPPaybleInvoices.PurposeOfPayment,
                        InvoiceDate = aPPaybleInvoices.InvoiceDate,
                        InvoiceNumber = aPPaybleInvoices.InvoiceNumber,
                        PaymentTerms = aPPaybleInvoices.PaymentTerms,
                        InvoiceAmount = aPPaybleInvoices.InvoiceAmount,
                        AdvanceAmount = aPPaybleInvoices.AdvanceAmount,
                        TaxAmount = aPPaybleInvoices.TaxAmount,
                        WithholdingAmount = aPPaybleInvoices.WithholdingAmount,
                        Subtotal = aPPaybleInvoices.Subtotal,
                        WithholdTaxAmount = aPPaybleInvoices.WithholdTaxAmount,
                        NetPayable = aPPaybleInvoices.NetPayable,
                        AmountDue = aPPaybleInvoices.AmountDue,
                        InvoiceStatus = aPPaybleInvoices.InvoiceStatus,
                        IsPostedToSecondLedger = aPPaybleInvoices.IsPostedToSecondLedger,

                        IsSendForApproval = aPPaybleInvoices.IsSendForApproval,
                        IsOnlineTransferred = aPPaybleInvoices.IsOnlineTransferred,
                        IsOnlineTransferredBy = aPPaybleInvoices.IsOnlineTransferredBy,
                        OnlineTransferredTime = aPPaybleInvoices.OnlineTransferredTime,
                        Status = aPPaybleInvoices.Status,
                        Source = aPPaybleInvoices.Source,
                        Grn = aPPaybleInvoices.Grn,
                        IsOnlineApproved = aPPaybleInvoices.IsOnlineApproved,
                        OnlineApprovedBy = aPPaybleInvoices.OnlineApprovedBy,
                        OnlineApprovedTime = aPPaybleInvoices.OnlineApprovedTime,
                        IsVoid = aPPaybleInvoices.IsVoid,
                        VoidBy = aPPaybleInvoices.VoidBy,
                        VoidTime = aPPaybleInvoices.VoidTime,
                        OrgBranchName = aPPaybleInvoices.OrgBranchName,
                        DateCreated = aPPaybleInvoices.DateCreated,
                        LastModified = aPPaybleInvoices.LastModified,
                        CreatedBy = aPPaybleInvoices.CreatedBy,
                        ModifiedBy = aPPaybleInvoices.ModifiedBy
                    };

                    _db.APPaybleInvoices.Attach(entity);
                    _db.APPaybleInvoices.Remove(entity);
                    _db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payable invoice " + aPPaybleInvoices.ApPaybleInvoicesId + " " + aPPaybleInvoices.Remark, Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPPaybleInvoices }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.APPaybleInvoices.Find(id);
            #region check segment length validation

            var distributions = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == id).ToList();
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
            decimal sumTotalinvoiceLines = 0;
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            try
            {
                sumTotalinvoiceLines = _db.APPaybleInvoicesLines.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.InvoiceAmount);

            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add invoice lines and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            try
            {
                sumTotalDisDebit = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.Debit);
                sumTotalDisCredit = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == record.ApPaybleInvoicesId).Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            //if (record != null && record.InvoiceAmount != sumTotalinvoiceLines)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Invoice amount and total of invoice lines is not equal.Please correct it and try again.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}
            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again.",
                    Status = "NOT_OK"
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

            if (record.APPaybleInvoicesLines.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsSendForApproval && !record.IsOnlineApproved && record.APPaybleInvoicesLines.Count > 0)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.APPaybleInvoices.Attach(record);
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


        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.APPaybleInvoices.Find(myid);
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
                    .Where(x => x.Source == JournalSources.AP_Invoice.ToString())
                    .Where(x => x.SourceId == record.ApPaybleInvoicesId.ToString())
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
                foreach (var dist in record.APPaybleInvoiceDistribution)
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
                var distribution = record.APPaybleInvoiceDistribution.ToList();
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

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.InvoiceDate.Date)
                    .Where(x => x.DateTo >= record.InvoiceDate.Date).ToList();
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
               
                if (record.APPaybleInvoiceDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Invoice",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Invoice",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Invoice",
                        SourcesName = "AP",
                        JournalNumber = "APINV." + record.ApPaybleInvoicesId.ToString(),
                        SourceId = record.ApPaybleInvoicesId.ToString(),
                        Source = JournalSources.AP_Invoice.ToString(),
                        EffectiveDate = record.InvoiceDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = "",
                        ReferenceDoc ="",
                        BranchId=0,//to change
                        InvoiceReferenceDoc =record.InvoiceNumber,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.APPaybleInvoiceDistribution)
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

        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.APPaybleInvoices.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred && record.APPaybleInvoicesLines.Count > 0)
            {
                #region validate invoice

                var payments = _db.APPayments.Where(x => x.InvoiceNumber == record.ApPaybleInvoicesId.ToString())
                    .ToList();
                if (payments.Any())
                {
                    var formattedData1 = new
                    {
                        Message = $"The invoice number {record.ApPaybleInvoicesId} is already applied in payment \n hence cannot be void.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData1, JsonRequestBehavior.AllowGet);
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
                _db.APPaybleInvoices.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();


                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payable invoice", "Success", User.Identity.Name, $"void payable invoice {id} ", Modules.Finance);
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

            if (record.APPaybleInvoicesLines.Count == 0)
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

        private void GetContractGrnItems(ApPaybleInvoices aPPaybleInvoice)
        {
            int intId = int.Parse(aPPaybleInvoice.SourceId);
            var payment = _db.ContractPayments.Find(intId);
            var grns = _db.ContractPaymentGrns.Where(x => x.ContractPaymentId == payment.ContractPaymentId).ToList();

            foreach (var grn in grns)
            {
                #region get data

                switch (grn.ItemType)
                {
                    case PurchaseItemType.Stock:

                        #region stock

                        #region lines

                        var receiveitems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId==grn.GrnNo).ToList();
                        foreach (var item in receiveitems)
                        {
                            ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                Name = item.ItemCode,
                                GrnNo= grn.GrnNo.ToString(),
                                APPaybleInvoices = aPPaybleInvoice,
                                Description = item.InventoryItem.Name,
                                AccountCode = item.ItemCategory.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(item.ItemCategory.AccountCode),
                                InvoiceAmount = item.SubTotal,
                                AdvanceAmount = 0,
                                NetPayable = item.SubTotal + item.TaxAmount,
                                ReceiptNo = "",
                                TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                                TaxAmount = item.TaxAmount,
                                WithholdingType = "NA",
                                WithholdingAmount = 0,
                                WithholdTax = false,
                                WithholdTaxAmount = 0,
                                PoNo = aPPaybleInvoice.PoNum
                            };
                            _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                            _db.SaveChanges();
                        }
                        #endregion

                        #region distribution
                        var ditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                        foreach (var item in ditributions)
                        {
                            ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                AccountCode = item.AccountCode,
                                AccountDesc = item.AccountDesc,
                                AdditionalDescription = item.AdditionalDescription,
                                Debit = item.Debit,
                                Credit = item.Credit,
                            };

                            _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                            _db.SaveChanges();
                        }
                        #endregion
                        _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                        _db.SaveChanges();

                        #endregion
                        break;

                    case PurchaseItemType.Service:

                        #region service

                        #region lines

                        var servicereceiveitems = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == grn.GrnNo).ToList();
                        foreach (var item in servicereceiveitems)
                        {
                            var serviceCategory = _db.ServiceCategories.FirstOrDefault(x => x.ServiceCategoryCode == item.ServiceCategoryCode);
                            ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                GrnNo= grn.GrnNo.ToString(),
                                Name = item.ServiceItemCode,
                                APPaybleInvoices = aPPaybleInvoice,
                                Description = serviceCategory?.Name,
                                AccountCode = serviceCategory?.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(serviceCategory?.AccountCode),
                                InvoiceAmount = item.SubTotal,
                                AdvanceAmount = 0,
                                NetPayable = item.SubTotal + item.TaxAmount,
                                ReceiptNo = "",
                                TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                                TaxAmount = item.TaxAmount,
                                WithholdingType = "NA",
                                WithholdingAmount = 0,
                                WithholdTax = false,
                                WithholdTaxAmount = 0,
                                PoNo = aPPaybleInvoice.PoNum
                            };
                            _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                            _db.SaveChanges();
                        }
                        #endregion

                        #region distribution
                        var serviceditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                        foreach (var item in serviceditributions)
                        {
                            ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                AccountCode = item.AccountCode,
                                AccountDesc = item.AccountDesc,
                                AdditionalDescription = item.AdditionalDescription,
                                Debit = item.Debit,
                                Credit = item.Credit,
                            };

                            _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                            _db.SaveChanges();
                        }
                        #endregion
                        _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                        _db.SaveChanges();

                        #endregion
                        break;

                    case PurchaseItemType.FixedAsset:

                        #region fixed

                        #region lines

                        var fixedreceiveitems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == grn.GrnNo).ToList();
                        foreach (var item in fixedreceiveitems)
                        {
                            ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                Name = item.Name,
                                GrnNo= grn.GrnNo.ToString(),
                                APPaybleInvoices = aPPaybleInvoice,
                                Description = item.FixedAssetSubCategory.SubCategoryName,
                                AccountCode = "NA",
                                AccountDesc = "NA",
                                InvoiceAmount = item.SubTotal,
                                AdvanceAmount = 0,
                                NetPayable = item.SubTotal + item.TaxAmount,
                                ReceiptNo = "",
                                TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                                TaxAmount = item.TaxAmount,
                                WithholdingType = "NA",
                                WithholdingAmount = 0,
                                WithholdTax = false,
                                WithholdTaxAmount = 0,
                                PoNo = aPPaybleInvoice.PoNum
                            };
                            _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                            _db.SaveChanges();
                        }
                        #endregion

                        #region distribution
                        var fixedditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                        foreach (var item in fixedditributions)
                        {
                            ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                            {
                                ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                                AccountCode = item.AccountCode,
                                AccountDesc = item.AccountDesc,
                                AdditionalDescription = item.AdditionalDescription,
                                Debit = item.Debit,
                                Credit = item.Credit,
                            };

                            _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                            _db.SaveChanges();
                        }
                        #endregion
                        _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                        _db.SaveChanges();

                        #endregion
                        break;

                }

                #endregion
            }

        }

        //old
        private void GetGrnItems(ApPaybleInvoices aPPaybleInvoice)
        {
            int intId = int.Parse(aPPaybleInvoice.Grn);
            #region get data

            switch (aPPaybleInvoice.Source)
            {
                case GrnSource.StockGrn:

                    #region stock

                    #region lines

                    var receiveitems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == intId).ToList();
                    foreach (var item in receiveitems)
                    {
                        ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            Name = item.ItemCode,
                            APPaybleInvoices = aPPaybleInvoice,
                            Description = item.InventoryItem.Name,
                            AccountCode = item.ItemCategory.AccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(item.ItemCategory.AccountCode),
                            InvoiceAmount = item.SubTotal,
                            AdvanceAmount = 0,
                            NetPayable = item.SubTotal + item.TaxAmount,
                            ReceiptNo = "",
                            TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                            TaxAmount = item.TaxAmount,
                            WithholdingType = "NA",
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            PoNo = aPPaybleInvoice.PoNum
                        };
                        _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                        _db.SaveChanges();
                    }
                    #endregion
              
                    #region distribution
                    var ditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                    foreach (var item in ditributions)
                    {
                        ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            AccountCode = item.AccountCode,
                            AccountDesc = item.AccountDesc,
                            AdditionalDescription = item.AdditionalDescription,
                            Debit = item.Debit,
                            Credit = item.Credit,
                        };

                        _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                        _db.SaveChanges();
                    }
                    #endregion
                    _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    break;

                case GrnSource.ServiceGrn:

                    #region service

                    #region lines

                    var servicereceiveitems = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == intId).ToList();
                    foreach (var item in servicereceiveitems)
                    {
                        var serviceCategory = _db.ServiceCategories.FirstOrDefault(x => x.ServiceCategoryCode == item.ServiceCategoryCode);
                        ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            Name = item.ServiceItemCode,
                            APPaybleInvoices = aPPaybleInvoice,
                            Description = serviceCategory?.Name,
                            AccountCode = serviceCategory?.AccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(serviceCategory?.AccountCode),
                            InvoiceAmount = item.SubTotal,
                            AdvanceAmount = 0,
                            NetPayable = item.SubTotal + item.TaxAmount,
                            ReceiptNo = "",
                            TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                            TaxAmount = item.TaxAmount,
                            WithholdingType = "NA",
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            PoNo = aPPaybleInvoice.PoNum
                        };
                        _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                        _db.SaveChanges();
                    }
                    #endregion
                   
                    #region distribution
                    var serviceditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                    foreach (var item in serviceditributions)
                    {
                        ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            AccountCode = item.AccountCode,
                            AccountDesc = item.AccountDesc,
                            AdditionalDescription = item.AdditionalDescription,
                            Debit = item.Debit,
                            Credit = item.Credit,
                        };

                        _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                        _db.SaveChanges();
                    }
                    #endregion
                    _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    break;

                case GrnSource.FixedssetGrn:

                    #region fixed

                    #region lines

                    var fixedreceiveitems = _db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveId == intId).ToList();
                    foreach (var item in fixedreceiveitems)
                    {
                        ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            Name = item.Name,
                            APPaybleInvoices = aPPaybleInvoice,
                            Description = item.FixedAssetSubCategory.SubCategoryName,
                            AccountCode = "NA",
                            AccountDesc = "NA",
                            InvoiceAmount = item.SubTotal,
                            AdvanceAmount = 0,
                            NetPayable = item.SubTotal + item.TaxAmount,
                            ReceiptNo = "",
                            TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                            TaxAmount = item.TaxAmount,
                            WithholdingType = "NA",
                            WithholdingAmount = 0,
                            WithholdTax = false,
                            WithholdTaxAmount = 0,
                            PoNo = aPPaybleInvoice.PoNum
                        };
                        _db.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                        _db.SaveChanges();
                    }
                    #endregion
                   
                    #region distribution
                    var fixedditributions = _db.GoodReceiveDistributions.Where(x => x.GoodReceiveId == intId).ToList();

                    foreach (var item in fixedditributions)
                    {
                        ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                        {
                            ApPaybleInvoicesId = aPPaybleInvoice.ApPaybleInvoicesId,
                            AccountCode = item.AccountCode,
                            AccountDesc = item.AccountDesc,
                            AdditionalDescription = item.AdditionalDescription,
                            Debit = item.Debit,
                            Credit = item.Credit,
                        };

                        _db.APPaybleInvoiceDistributions.Add(paybleInvoiceDistribution);
                        _db.SaveChanges();
                    }
                    #endregion
                    _db.Entry(aPPaybleInvoice).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    break;

            }
          
            #endregion
        }

        //not used
        public JsonResult GetGrnDataToAp(string id)
        {
            if (id == null)
            {
                var formattedData = new
                {
                    Message = "Please select GRN Number",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            int intId = int.Parse(id);
            var goodReceive = _db.GoodReceives.FirstOrDefault(x => x.GoodReceiveId == intId);
            string prno = "";
            #region get pr
            if (goodReceive != null)
            {
                ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                if (type != null)
                {
                    switch (type.Type)
                    {
                        case GoodReceiveType.PurchaseOrder:
                            int pid = int.Parse(goodReceive.ReceiveRef);
                            PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                            if (purchaseOrder != null)
                            {
                                prno = purchaseOrder.PurchaseRequisitionID.ToString();
                            }
                            break;
                        case GoodReceiveType.Tender:
                            int awardid = int.Parse(goodReceive.ReceiveRef);
                            BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                            if (award != null)
                            {
                                //not possible to get pr since merged prs
                                //prno = award.Lot.pu.ToString();
                            }
                            break;
                    }
                }
            }
            #endregion

            var exists = _db.APPaybleInvoices.FirstOrDefault(x => x.Grn == goodReceive.GoodReceiveId.ToString());
            if (exists != null)
            {
                var formattedData = new
                {
                    Message = "The GRN Number already exists in Payable Invoice.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            #region build invoice header
            if (goodReceive != null)
            {
                var apPaybleInvoices = new ApPaybleInvoices
                {
                    SupplierId = goodReceive.SupplierId,
                    //EmployeeId=goo
                    Type = ApInvoiceTransactionType.Invoice,
                    PoNum = goodReceive.ReceiveRef.ToString(),
                    Grn = goodReceive.GoodReceiveId.ToString(),
                    PrNo = prno,
                    PurposeOfPayment = "",
                    InvoiceDate = DateTime.Today,
                    InvoiceNumber = "",
                    PaymentTerms = 0,
                    InvoiceAmount = 0,
                    AdvanceAmount = 0,
                    TaxAmount = 0,
                    WithholdingAmount = 0,
                    Subtotal = 0,
                    WithholdTaxAmount = 0,
                    NetPayable = 0,
                    AmountDue = 0,
                    InvoiceStatus = InvoiceStatus.Open,
                    IsSendForApproval = false,
                    IsOnlineTransferred = false,
                    Status = "Editing",
                    Source = GrnSource.StockGrn,
                    OrgBranchName = "",
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    APPaybleInvoicesLines = new List<ApPaybleInvoicesLines>(),
                    APPaybleInvoiceDistribution = new List<ApPaybleInvoiceDistribution>()
                };
                _db.APPaybleInvoices.Add(apPaybleInvoices);
                _db.SaveChanges();
                #region lines

                var receiveitems = _db.GoodReceiveItems.Where(x => x.GoodReceiveId == intId).ToList();
                foreach (var item in receiveitems)
                {
                    ApPaybleInvoicesLines paybleInvoicesLines = new ApPaybleInvoicesLines
                    {
                        ApPaybleInvoicesId = apPaybleInvoices.ApPaybleInvoicesId,
                        Name = item.ItemCode,
                        APPaybleInvoices = apPaybleInvoices,
                        Description = item.InventoryItem.Name,
                        AccountCode = item.ItemCategory.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(item.ItemCategory.AccountCode),
                        InvoiceAmount = item.SubTotal,
                        AdvanceAmount = 0,
                        NetPayable = item.SubTotal + item.TaxAmount,
                        ReceiptNo = "",
                        TaxType = _taxHelper.GetTaxName(item.GLTaxRateID),
                        TaxAmount = item.TaxAmount,
                        WithholdingType = "NA",
                        WithholdingAmount = 0,
                        WithholdTax = false,
                        WithholdTaxAmount = 0,
                        PoNo = apPaybleInvoices.PoNum
                    };

                    apPaybleInvoices.APPaybleInvoicesLines.Add(paybleInvoicesLines);
                }
                #endregion
                //update apPaybleInvoices
                apPaybleInvoices.InvoiceAmount = apPaybleInvoices.APPaybleInvoicesLines.Sum(x => x.InvoiceAmount);
                apPaybleInvoices.NetPayable = apPaybleInvoices.APPaybleInvoicesLines.Sum(x => x.NetPayable);
                apPaybleInvoices.AmountDue = apPaybleInvoices.NetPayable;

                //save
                #region distribution

                foreach (var item in goodReceive.GoodReceiveDistribution)
                {
                    ApPaybleInvoiceDistribution paybleInvoiceDistribution = new ApPaybleInvoiceDistribution
                    {
                        ApPaybleInvoicesId = apPaybleInvoices.ApPaybleInvoicesId,
                        AccountCode = item.AccountCode,
                        AccountDesc = item.AccountDesc,
                        AdditionalDescription = item.AdditionalDescription,
                        Debit = item.Debit,
                        Credit = item.Credit,
                    };

                    apPaybleInvoices.APPaybleInvoiceDistribution.Add(paybleInvoiceDistribution);
                }
                #endregion

                _db.Entry(apPaybleInvoices).State = EntityState.Modified;
                _db.APPaybleInvoiceDistributions.AddRange(apPaybleInvoices.APPaybleInvoiceDistribution);
                _db.SaveChanges();

            }

            #endregion
            var formattedData1 = new
            {
                Message = "Applied.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        public ActionResult PurchaseHeader_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<PurchaseViewModel> purchases = new List<PurchaseViewModel>();

            var paybleInvoices = _db.APPaybleInvoices.Find(id);

            if (!String.IsNullOrWhiteSpace(paybleInvoices?.Grn))
            {
                string grntype = paybleInvoices.Grn.Split('_')[0];
                int grnno = int.Parse(paybleInvoices.Grn.Split('_')[0]);

                #region stock

                if (grntype == "STOCK")
                {
                    var goodReceive = _db.GoodReceives.Find(grnno);
                    if (goodReceive != null)
                    {
                        var receiveType = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                        if (receiveType != null && receiveType.Type == GoodReceiveType.PurchaseOrder)
                        {
                            #region purchase order

                            var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(goodReceive.ReceiveRef));
                            if (purchaseOrder != null)
                            {
                                PurchaseViewModel record = new PurchaseViewModel
                                {
                                    PurchaseId = purchaseOrder.PurchaseOrderID,
                                    PoTypeEnum = PurchaseType.PurchaseOrder,
                                    ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                                    SupplierId = purchaseOrder.SupplierId,
                                    IsNewSupplier = purchaseOrder.IsNewSupplier,
                                    SupplierName = purchaseOrder.SupplierName,
                                    ProcurementMethod = purchaseOrder.ProcurementMethod,
                                    PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                                    ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                                    OrgStructureId = purchaseOrder.OrgStructureId,
                                    OrderDate = purchaseOrder.OrderDate,
                                    DeliveryDate = purchaseOrder.DeliveryDate,
                                    DeliveryPlace = purchaseOrder.DeliveryPlace,
                                    Status = purchaseOrder.Status,
                                    Remark = purchaseOrder.Remark,

                                    IsSendForApproval = purchaseOrder.IsSendForApproval,
                                    SendForApprovalBy = purchaseOrder.SendForApprovalBy,
                                    SendForApprovalTime = purchaseOrder.SendForApprovalTime,

                                    IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                                    OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                                    OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,

                                    IsVoid = purchaseOrder.IsVoid,
                                    VoidBy = purchaseOrder.VoidBy,
                                    VoidTime = purchaseOrder.VoidTime,

                                    DateCreated = purchaseOrder.DateCreated,
                                    LastModified = purchaseOrder.LastModified,
                                    CreatedBy = purchaseOrder.CreatedBy,
                                    ModifiedBy = purchaseOrder.ModifiedBy
                                };
                                purchases.Add(record);
                            }

                            #endregion
                        }

                        if (receiveType != null && receiveType.Type == GoodReceiveType.Tender)
                        {
                            #region award

                            var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(goodReceive.ReceiveRef));

                            if (bidAwardedSupplier != null)
                            {
                                var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                                if (lot != null)
                                {
                                    var tender = _db.Tenders.Find(lot.TenderID);

                                    if (tender != null)
                                    {
                                        PurchaseViewModel record = new PurchaseViewModel
                                        {
                                            PurchaseId = bidAwardedSupplier.BidAwardedSupplierID,
                                            PoTypeEnum = PurchaseType.TenderAward,
                                            ProcurementOfficer = tender.ProcurementOfficer,
                                            TenderRef = tender.TenderReference,
                                            SupplierId = bidAwardedSupplier.SupplierId,
                                            IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                                            SupplierName = bidAwardedSupplier.SupplierName,
                                            ProcurementMethod = ProcurementMethod.Tender,
                                            OrderDate = bidAwardedSupplier.AwardDate,
                                            Status = tender.Status,
                                            Remark = bidAwardedSupplier.Remark,

                                            IsSendForApproval = tender.IsSendForApproval,
                                            SendForApprovalBy = tender.SendForApprovalBy,
                                            SendForApprovalTime = tender.SendForApprovalTime,

                                            IsOnlineApproved = tender.IsOnlineApproved,
                                            OnlineApprovedBy = tender.OnlineApprovedBy,
                                            OnlineApprovedTime = tender.OnlineApprovedTime,

                                            IsVoid = tender.IsVoid,
                                            VoidBy = tender.VoidBy,
                                            VoidTime = tender.VoidTime,

                                            DateCreated = bidAwardedSupplier.DateCreated,
                                            LastModified = bidAwardedSupplier.LastModified,
                                            CreatedBy = bidAwardedSupplier.CreatedBy,
                                            ModifiedBy = bidAwardedSupplier.ModifiedBy
                                        };
                                        purchases.Add(record);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                }

                #endregion
                #region services

                else if (grntype == "SERVICE")
                {
                    var goodReceive = _db.ServiceReceives.Find(grnno);
                    if (goodReceive != null)
                    {
                        var receiveType = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                        if (receiveType != null && receiveType.Type == GoodReceiveType.PurchaseOrder)
                        {
                            #region purchase order

                            var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(goodReceive.ReceiveRef));
                            if (purchaseOrder != null)
                            {
                                PurchaseViewModel record = new PurchaseViewModel
                                {
                                    PurchaseId = purchaseOrder.PurchaseOrderID,
                                    PoTypeEnum = PurchaseType.PurchaseOrder,
                                    ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                                    SupplierId = purchaseOrder.SupplierId,
                                    IsNewSupplier = purchaseOrder.IsNewSupplier,
                                    SupplierName = purchaseOrder.SupplierName,
                                    ProcurementMethod = purchaseOrder.ProcurementMethod,
                                    PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                                    ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                                    OrgStructureId = purchaseOrder.OrgStructureId,
                                    OrderDate = purchaseOrder.OrderDate,
                                    DeliveryDate = purchaseOrder.DeliveryDate,
                                    DeliveryPlace = purchaseOrder.DeliveryPlace,
                                    Status = purchaseOrder.Status,
                                    Remark = purchaseOrder.Remark,

                                    IsSendForApproval = purchaseOrder.IsSendForApproval,
                                    SendForApprovalBy = purchaseOrder.SendForApprovalBy,
                                    SendForApprovalTime = purchaseOrder.SendForApprovalTime,

                                    IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                                    OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                                    OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,

                                    IsVoid = purchaseOrder.IsVoid,
                                    VoidBy = purchaseOrder.VoidBy,
                                    VoidTime = purchaseOrder.VoidTime,

                                    DateCreated = purchaseOrder.DateCreated,
                                    LastModified = purchaseOrder.LastModified,
                                    CreatedBy = purchaseOrder.CreatedBy,
                                    ModifiedBy = purchaseOrder.ModifiedBy
                                };
                                purchases.Add(record);
                            }

                            #endregion
                        }

                        if (receiveType != null && receiveType.Type == GoodReceiveType.Tender)
                        {
                            #region award

                            var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(goodReceive.ReceiveRef));

                            if (bidAwardedSupplier != null)
                            {
                                var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                                if (lot != null)
                                {
                                    var tender = _db.Tenders.Find(lot.TenderID);

                                    if (tender != null)
                                    {
                                        PurchaseViewModel record = new PurchaseViewModel
                                        {
                                            PurchaseId = bidAwardedSupplier.BidAwardedSupplierID,
                                            PoTypeEnum = PurchaseType.TenderAward,
                                            ProcurementOfficer = tender.ProcurementOfficer,
                                            TenderRef = tender.TenderReference,
                                            SupplierId = bidAwardedSupplier.SupplierId,
                                            IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                                            SupplierName = bidAwardedSupplier.SupplierName,
                                            ProcurementMethod = ProcurementMethod.Tender,
                                            OrderDate = bidAwardedSupplier.AwardDate,
                                            Status = tender.Status,
                                            Remark = bidAwardedSupplier.Remark,

                                            IsSendForApproval = tender.IsSendForApproval,
                                            SendForApprovalBy = tender.SendForApprovalBy,
                                            SendForApprovalTime = tender.SendForApprovalTime,

                                            IsOnlineApproved = tender.IsOnlineApproved,
                                            OnlineApprovedBy = tender.OnlineApprovedBy,
                                            OnlineApprovedTime = tender.OnlineApprovedTime,

                                            IsVoid = tender.IsVoid,
                                            VoidBy = tender.VoidBy,
                                            VoidTime = tender.VoidTime,

                                            DateCreated = bidAwardedSupplier.DateCreated,
                                            LastModified = bidAwardedSupplier.LastModified,
                                            CreatedBy = bidAwardedSupplier.CreatedBy,
                                            ModifiedBy = bidAwardedSupplier.ModifiedBy
                                        };
                                        purchases.Add(record);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                }

                #endregion
                #region fixed asset

                else if (grntype == "FIXEDASSET")
                {
                    var goodReceive = _db.FixedAssetReceives.Find(grnno);
                    if (goodReceive != null)
                    {
                        var receiveType = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                        if (receiveType != null && receiveType.Type == GoodReceiveType.PurchaseOrder)
                        {
                            #region purchase order

                            var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(goodReceive.ReceiveRef));
                            if (purchaseOrder != null)
                            {
                                PurchaseViewModel record = new PurchaseViewModel
                                {
                                    PurchaseId = purchaseOrder.PurchaseOrderID,
                                    PoTypeEnum = PurchaseType.PurchaseOrder,
                                    ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                                    SupplierId = purchaseOrder.SupplierId,
                                    IsNewSupplier = purchaseOrder.IsNewSupplier,
                                    SupplierName = purchaseOrder.SupplierName,
                                    ProcurementMethod = purchaseOrder.ProcurementMethod,
                                    PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                                    ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                                    OrgStructureId = purchaseOrder.OrgStructureId,
                                    OrderDate = purchaseOrder.OrderDate,
                                    DeliveryDate = purchaseOrder.DeliveryDate,
                                    DeliveryPlace = purchaseOrder.DeliveryPlace,
                                    Status = purchaseOrder.Status,
                                    Remark = purchaseOrder.Remark,

                                    IsSendForApproval = purchaseOrder.IsSendForApproval,
                                    SendForApprovalBy = purchaseOrder.SendForApprovalBy,
                                    SendForApprovalTime = purchaseOrder.SendForApprovalTime,

                                    IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                                    OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                                    OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,

                                    IsVoid = purchaseOrder.IsVoid,
                                    VoidBy = purchaseOrder.VoidBy,
                                    VoidTime = purchaseOrder.VoidTime,

                                    DateCreated = purchaseOrder.DateCreated,
                                    LastModified = purchaseOrder.LastModified,
                                    CreatedBy = purchaseOrder.CreatedBy,
                                    ModifiedBy = purchaseOrder.ModifiedBy
                                };
                                purchases.Add(record);
                            }

                            #endregion
                        }

                        if (receiveType != null && receiveType.Type == GoodReceiveType.Tender)
                        {
                            #region award

                            var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(goodReceive.ReceiveRef));

                            if (bidAwardedSupplier != null)
                            {
                                var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                                if (lot != null)
                                {
                                    var tender = _db.Tenders.Find(lot.TenderID);

                                    if (tender != null)
                                    {
                                        PurchaseViewModel record = new PurchaseViewModel
                                        {
                                            PurchaseId = bidAwardedSupplier.BidAwardedSupplierID,
                                            PoTypeEnum = PurchaseType.TenderAward,
                                            ProcurementOfficer = tender.ProcurementOfficer,
                                            TenderRef = tender.TenderReference,
                                            SupplierId = bidAwardedSupplier.SupplierId,
                                            IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                                            SupplierName = bidAwardedSupplier.SupplierName,
                                            ProcurementMethod = ProcurementMethod.Tender,
                                            OrderDate = bidAwardedSupplier.AwardDate,
                                            Status = tender.Status,
                                            Remark = bidAwardedSupplier.Remark,

                                            IsSendForApproval = tender.IsSendForApproval,
                                            SendForApprovalBy = tender.SendForApprovalBy,
                                            SendForApprovalTime = tender.SendForApprovalTime,

                                            IsOnlineApproved = tender.IsOnlineApproved,
                                            OnlineApprovedBy = tender.OnlineApprovedBy,
                                            OnlineApprovedTime = tender.OnlineApprovedTime,

                                            IsVoid = tender.IsVoid,
                                            VoidBy = tender.VoidBy,
                                            VoidTime = tender.VoidTime,

                                            DateCreated = bidAwardedSupplier.DateCreated,
                                            LastModified = bidAwardedSupplier.LastModified,
                                            CreatedBy = bidAwardedSupplier.CreatedBy,
                                            ModifiedBy = bidAwardedSupplier.ModifiedBy
                                        };
                                        purchases.Add(record);
                                    }
                                }
                            }

                            #endregion
                        }
                    }

                }

                #endregion
            }

            IQueryable<PurchaseViewModel> purchaseViewModels = purchases.AsQueryable();
            DataSourceResult result = purchaseViewModels.ToDataSourceResult(request, purchaseViewModel => new {
                PurchaseId = purchaseViewModel.PurchaseId,
                TenderRef = purchaseViewModel.TenderRef,
                PoTypeEnum = purchaseViewModel.PoTypeEnum,
                ProcurementOfficer = purchaseViewModel.ProcurementOfficer,
                SupplierId = purchaseViewModel.SupplierId,
                IsNewSupplier = purchaseViewModel.IsNewSupplier,
                SupplierName = purchaseViewModel.SupplierName,
                ProcurementMethod = purchaseViewModel.ProcurementMethod,
                PurchaseRequisitionID = purchaseViewModel.PurchaseRequisitionID,
                ProformaPurchaseID = purchaseViewModel.ProformaPurchaseID,
                OrgStructureId = purchaseViewModel.OrgStructureId,
                OrderDate = purchaseViewModel.OrderDate,
                DeliveryDate = purchaseViewModel.DeliveryDate,
                DeliveryPlace = purchaseViewModel.DeliveryPlace,
                Status = purchaseViewModel.Status,
                Remark = purchaseViewModel.Remark,

                IsSendForApproval = purchaseViewModel.IsSendForApproval,
                SendForApprovalBy = purchaseViewModel.SendForApprovalBy,
                SendForApprovalTime = purchaseViewModel.SendForApprovalTime,

                IsOnlineApproved = purchaseViewModel.IsOnlineApproved,
                OnlineApprovedBy = purchaseViewModel.OnlineApprovedBy,
                OnlineApprovedTime = purchaseViewModel.OnlineApprovedTime,

                IsVoid = purchaseViewModel.IsVoid,
                VoidBy = purchaseViewModel.VoidBy,
                VoidTime = purchaseViewModel.VoidTime,

                DateCreated = purchaseViewModel.DateCreated,
                LastModified = purchaseViewModel.LastModified,
                CreatedBy = purchaseViewModel.CreatedBy,
                ModifiedBy = purchaseViewModel.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchaseItem_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchaseItemViewModel> purchases = new List<PurchaseItemViewModel>();

            if (type == PurchaseType.PurchaseOrder)
            {
                #region purchase order

                var purchaseOrder = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var purchaseOrderItem in purchaseOrder)
                {

                    PurchaseItemViewModel record = new PurchaseItemViewModel
                    {
                        CatCode = purchaseOrderItem.CatCode,
                        CategoryName = purchaseOrderItem.CategoryName,
                        ItmCode = purchaseOrderItem.ItmCode,
                        ItemName = purchaseOrderItem.ItemName,
                        PartNo = purchaseOrderItem.PartNo,
                        Quantity = purchaseOrderItem.Quantity,
                        IsApproved = purchaseOrderItem.IsApproved,
                        ItemSpecification = purchaseOrderItem.ItemSpecification,
                        UoM = purchaseOrderItem.UoM,
                        ItemType = purchaseOrderItem.ItemType,
                        UnitPrice = purchaseOrderItem.UnitPrice,
                        GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                        TaxAmount = purchaseOrderItem.TaxAmount,
                        SubTotal = purchaseOrderItem.SubTotal,
                        Total = purchaseOrderItem.Total,
                        Remark = purchaseOrderItem.Remark,
                        OrgBranchName = purchaseOrderItem.OrgBranchName,
                        DateCreated = purchaseOrderItem.DateCreated,
                        LastModified = purchaseOrderItem.LastModified,
                        CreatedBy = purchaseOrderItem.CreatedBy,
                        ModifiedBy = purchaseOrderItem.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion

            }
            else if (type == PurchaseType.TenderAward)
            {
                #region award

                var awardedSuppliers = _db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == id).ToList();
                foreach (var items in awardedSuppliers)
                {

                    PurchaseItemViewModel record = new PurchaseItemViewModel
                    {
                        CatCode = items.CatCode,
                        CategoryName = items.CategoryName,
                        ItmCode = items.ItmCode,
                        ItemName = items.ItemName,
                        PartNo = items.PartNo,
                        Quantity = items.Quanity,
                        IsApproved = items.IsApproved,
                        ItemSpecification = items.ItemSpecification,
                        UoM = items.UoM,
                        ItemType = items.ItemType,
                        UnitPrice = items.FinalUPrice,
                        GLTaxRateID = items.GLTaxRateID,
                        TaxAmount = items.TaxAmount,
                        SubTotal = items.FinalUPrice * items.Quanity,
                        Total = items.TPrice,
                        Remark = items.BidAwardedSupplier.Remark,
                        OrgBranchName = items.OrgBranchName,
                        DateCreated = items.DateCreated,
                        LastModified = items.LastModified,
                        CreatedBy = items.CreatedBy,
                        ModifiedBy = items.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion

            }

            IQueryable<PurchaseItemViewModel> purchaseItemViewModels = purchases.AsQueryable();
            DataSourceResult result = purchaseItemViewModels.ToDataSourceResult(request, itemViewModels => new {
                CatCode = itemViewModels.CatCode + "-" + itemViewModels.CategoryName,
                CategoryName = itemViewModels.CategoryName,
                ItmCode = itemViewModels.ItmCode + "-" + itemViewModels.ItemName,
                ItemName = itemViewModels.ItemName,
                PartNo = itemViewModels.PartNo,
                Quantity = itemViewModels.Quantity,
                IsApproved = itemViewModels.IsApproved,
                ItemSpecification = itemViewModels.ItemSpecification,
                UoM = itemViewModels.UoM,
                ItemType = itemViewModels.ItemType,
                UnitPrice = itemViewModels.UnitPrice,
                GLTaxRateID = itemViewModels.GLTaxRateID,
                TaxAmount = itemViewModels.TaxAmount,
                SubTotal = itemViewModels.SubTotal,
                Total = itemViewModels.Total,
                Remark = itemViewModels.Remark,
                OrgBranchName = itemViewModels.OrgBranchName,
                DateCreated = itemViewModels.DateCreated,
                LastModified = itemViewModels.LastModified,
                CreatedBy = itemViewModels.CreatedBy,
                ModifiedBy = itemViewModels.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchasePaymentSchedule_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchasePaymentScheduleViewModel> purchases = new List<PurchasePaymentScheduleViewModel>();

            if (type == PurchaseType.PurchaseOrder)
            {
                var paymentSchedules = _db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var paymentSchedule in paymentSchedules)
                {

                    PurchasePaymentScheduleViewModel record = new PurchasePaymentScheduleViewModel
                    {
                        PaymentType = paymentSchedule.PaymentType,
                        Percentage = paymentSchedule.Percentage,
                        Remark = paymentSchedule.Remark,
                        OrgBranchName = paymentSchedule.OrgBranchName,
                        DateCreated = paymentSchedule.DateCreated,
                        LastModified = paymentSchedule.LastModified,
                        CreatedBy = paymentSchedule.CreatedBy,
                        ModifiedBy = paymentSchedule.ModifiedBy
                    };
                    purchases.Add(record);
                }
            }

            IQueryable<PurchasePaymentScheduleViewModel> paymentScheduleViewModels = purchases.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new {
                PaymentType = paymentScheduleViewModel.PaymentType,
                Percentage = paymentScheduleViewModel.Percentage,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchaseValidation_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchaseValidationViewModel> purchases = new List<PurchaseValidationViewModel>();

            if (type == PurchaseType.PurchaseOrder)
            {
                #region purchase order

                var orderValidations = _db.PurchaseOrderValidations.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var validation in orderValidations)
                {

                    PurchaseValidationViewModel record = new PurchaseValidationViewModel
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion
            }
            if (type == PurchaseType.TenderAward)
            {
                #region tender award

                var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(id);

                if (bidAwardedSupplier != null)
                {
                    var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                    if (lot != null)
                    {
                        var tender = _db.Tenders.Find(lot.TenderID);

                        var tenderValidations = _db.TenderValidations.Where(x => x.TenderID == tender.TenderID).ToList();
                        foreach (var validation in tenderValidations)
                        {

                            PurchaseValidationViewModel record = new PurchaseValidationViewModel
                            {
                                Status = validation.Status,
                                Remark = validation.Remark,
                                OrgBranchName = validation.OrgBranchName,
                                DateCreated = validation.DateCreated,
                                LastModified = validation.LastModified,
                                CreatedBy = validation.CreatedBy,
                                ModifiedBy = validation.ModifiedBy
                            };
                            purchases.Add(record);
                        }
                    }
                }



                #endregion
            }

            IQueryable<PurchaseValidationViewModel> paymentScheduleViewModels = purchases.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new {
                Status = paymentScheduleViewModel.Status,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
