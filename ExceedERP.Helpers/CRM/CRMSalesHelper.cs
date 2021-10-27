using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ExceedERP.Helpers.CRM
{

    public class CRMSalesHelper
    {


        public decimal GetPartialLineAmount(LineItem LineItem, List<CashSalesPartialLineItem> CashSalesPartialLineItems)
        {

            var PartialSalesQuantity = CashSalesPartialLineItems.Where(x => x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);
            return PartialSalesQuantity;

        }

        public decimal GetPartialLinesAmount(List<LineItem> LineItems, List<CashSalesPartialLineItem> CashSalesPartialLineItems)
        {
            decimal PartialSalesQuantity = decimal.Zero;
            foreach (var LineItem in LineItems)
            {
                PartialSalesQuantity = PartialSalesQuantity + CashSalesPartialLineItems.Where(x => x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);

            }
            return PartialSalesQuantity;

        }

        public decimal GetPartialLinesAmountByPartialItem(List<LineItem> LineItems, List<CashSalesPartialLineItem> CashSalesPartialLineItems, CashSalesPartialItem CashSalesPartialItem)
        {
            decimal PartialSalesQuantity = decimal.Zero;
            foreach (var LineItem in LineItems)
            {
                PartialSalesQuantity = PartialSalesQuantity + CashSalesPartialLineItems.Where(x => x.CashSalesPartialItemId == CashSalesPartialItem.CashSalesPartialItemId && x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);
            }
            return PartialSalesQuantity;

        }

        public decimal GetPartialLineAmountByPartialItem(LineItem LineItem, List<CashSalesPartialLineItem> CashSalesPartialLineItems, CashSalesPartialItem CashSalesPartialItem)
        {
            decimal PartialSalesQuantity = decimal.Zero;

            PartialSalesQuantity = CashSalesPartialLineItems.Where(x => x.CashSalesPartialItemId == CashSalesPartialItem.CashSalesPartialItemId && x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);

            return PartialSalesQuantity;

        }


        public decimal GetPartialLineQuantityByPartialItem(LineItem LineItem, List<CashSalesPartialLineItem> CashSalesPartialLineItems, CashSalesPartialItem CashSalesPartialItem)
        {


            var PartialSalesQuantity = CashSalesPartialLineItems.Where(x => x.CashSalesPartialItemId == CashSalesPartialItem.CashSalesPartialItemId && x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);

            return PartialSalesQuantity;

        }

        public bool ValidatePartialLineAmount(LineItem LineItem, List<CashSalesPartialLineItem> CashSalesPartialLineItems, decimal Value)
        {
            decimal PartialSalesQuantity = decimal.Zero;

            PartialSalesQuantity = CashSalesPartialLineItems.Where(x => x.LineItemID == LineItem.LineItemID).Sum(x => x.PartialPayedAmount);
            PartialSalesQuantity = PartialSalesQuantity + Value;
            if (LineItem.Quantity > PartialSalesQuantity)
            { return true; }

            return false;

        }

        public bool ValidatePartialLineAmount(LineItem LineItem, List<CashSalesPartialLineItem> CashSalesPartialLineItems, int PartialLineId, decimal Value)
        {
            decimal PartialSalesQuantity = decimal.Zero;

            PartialSalesQuantity = CashSalesPartialLineItems.Where(x => x.LineItemID == LineItem.LineItemID && x.CashSalesPartialLineItemId != PartialLineId).Sum(x => x.PartialPayedAmount);
            PartialSalesQuantity = PartialSalesQuantity + Value;

            if (LineItem.Quantity > PartialSalesQuantity)
            { return true; }

            return false;

        }
        public Tuple<List<AdvancePaymentLog>, decimal> ProcessAdvancePayment(CashSalesInvoice Invoice, List<LineItem> LineItems, List<AdvancePaymentLine> AdvancePaymentLine, List<AdvancePayment> AdvancePayments)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal PayedAmount = decimal.Zero;

            List<AdvancePaymentLog> Logs = new List<AdvancePaymentLog>();

            var AdvancePayment = AdvancePayments.Where(x => x.Customer == Invoice.OrganizationCustomerId).FirstOrDefault();

            if (AdvancePayment != null)
            {

                var TotalAmount = LineItems.Sum(x => x.TotalAmount);

                var AdvancePaymentLines = AdvancePaymentLine.Where(x => x.AdvancePaymentId == AdvancePayment.AdvancePaymentId).ToList();





                foreach (var Line in AdvancePaymentLines)
                {
                    if (TotalAmount > Line.Value)
                    {
                        PayedAmount += Line.Value;

                        var Amount = decimal.Zero;



                        if (TotalAmount > 0)
                        {
                            var Log = new AdvancePaymentLog()
                            {

                                AdvancePaymentLineId = Line.AdvancePaymentLineId,
                                Amount = Amount,
                                CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                                DateCreated = DateTime.Now,
                                CreatedBy = HttpContext.Current.User.Identity.Name

                            };
                            Logs.Add(Log);


                        }
                    }
                    else
                    {
                        PayedAmount += Line.Value;

                        var Amount = Line.Value - TotalAmount;

                        if (Amount > 0 && TotalAmount > 0)
                        {
                            var Log = new AdvancePaymentLog()
                            {

                                AdvancePaymentLineId = Line.AdvancePaymentLineId,
                                Amount = Amount,
                                CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                                DateCreated = DateTime.Now,
                                CreatedBy = HttpContext.Current.User.Identity.Name

                            };
                            Logs.Add(Log);


                        }
                        return Tuple.Create(Logs, PayedAmount);


                    }


                }


            }
            return Tuple.Create(Logs, PayedAmount);


        }
        public void ConfigureWitholding(bool WithholdTax, string WithholdingType, List<GLTaxRate> Rate, out decimal WithHoldingAmount, out decimal WithHoldingTaxAmount, decimal TaxAmount, decimal subTotal)
        {
            var GLWithholdingTaxRate = Rate.Where(t => t.GLTaxRateID.ToString() == WithholdingType).FirstOrDefault();
            //WithHoldingAmount = (TotalAmount - (TotalAmount * (GLWithholdingTaxRate.Rate / 100)));
            WithHoldingAmount = subTotal * GLWithholdingTaxRate.CalculatedRate;
            WithHoldingTaxAmount = decimal.Zero;
            if (WithholdTax)
            {
                WithHoldingTaxAmount = TaxAmount;
            }
        }

        public bool SalesInvoiceHasUnShipedLineLitems(CashSalesInvoice salesInvoice, string voucherDefnisionId)
        {
            var db = new ExceedDbContext();
            bool hasUnShipedItems = false;
            var lineItems = db.LineItems.Where(l => l.Voucher == salesInvoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == voucherDefnisionId).ToList();
            if (lineItems.Any())
            {
                foreach (LineItem lineItem in lineItems)
                {
                    decimal remainingQty = lineItem.Quantity;
                    decimal shippedBefore = 0;
                    var existingShipmentLineItem = db.SalesShipmentLineItem.Where(li => li.LineItemId == lineItem.LineItemID).ToList();
                    if (existingShipmentLineItem.Any())
                    {
                        shippedBefore = existingShipmentLineItem.Sum(i => i.ShipedQuantity);
                        remainingQty = lineItem.Quantity - shippedBefore;
                    }
                    if (remainingQty > 0)
                    {
                        hasUnShipedItems = true;
                    }
                }
            }
            return hasUnShipedItems;
        }

        public bool SalesOrderVoucherHasUnShipedLineLitems(SalesOrderVoucher orderVoucher, string voucherDefnisionId)
        {
            var db = new ExceedDbContext();
            bool hasUnShipedItems = false;
            var lineItems = db.LineItems.Where(l => l.Voucher == orderVoucher.SalesOrderVoucherID.ToString() && l.VoucherDefinitionID == voucherDefnisionId).ToList();
            if (lineItems.Any())
            {
                foreach (LineItem lineItem in lineItems)
                {
                    decimal remainingQty = lineItem.Quantity;
                    decimal shippedBefore = 0;
                    var existingShipmentLineItem = db.SalesShipmentLineItem.Where(li => li.LineItemId == lineItem.LineItemID).ToList();
                    if (existingShipmentLineItem.Any())
                    {
                        shippedBefore = existingShipmentLineItem.Sum(i => i.ShipedQuantity);
                        remainingQty = lineItem.Quantity - shippedBefore;
                    }
                    if (remainingQty > 0)
                    {
                        hasUnShipedItems = true;
                    }
                }
            }
            return hasUnShipedItems;
        }

        public void UpdateSalesInvoiceRecordOnLineEdit(CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            var currentInvoice = db.CashSalesInvoices.Find(invoice.CashSalesInvoiceID);
            var lineItems = db.LineItems.Where(l => l.VoucherDefinitionID == "2" && l.Voucher == invoice.CashSalesInvoiceID.ToString()).ToList();
            if (lineItems.Any())
            {
                var total = lineItems.Sum(l => l.TotalAmount);

                currentInvoice.GrandTotal = total;

                db.CashSalesInvoices.Attach(currentInvoice);
                db.Entry(currentInvoice).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public static OrganizationCustomer CreateTempCustomer(string Name, string customerTin = "")
        {
            var db = new ExceedDbContext();
            var OrgCustomer = new OrganizationCustomer();
            var existingCustomerByThisName = db.OrganizationCustomers.FirstOrDefault(c => c.TradeName.ToLower() == Name.ToLower());
            if (existingCustomerByThisName != null)
            {
                return existingCustomerByThisName;
            }
            else
            {
                var OrgCustomerCategory = db.OrganizationCategories.FirstOrDefault();
                if (OrgCustomerCategory != null)
                {
                    OrgCustomer.BrandName = Name;
                    OrgCustomer.TradeName = Name;
                    OrgCustomer.Category = OrgCustomerCategory.OrganizationCategoryID.ToString();
                    OrgCustomer.BusinessType = "Business Type";
                    OrgCustomer.CustomerType = CustomerType.LOCAL;
                    OrgCustomer.TinNo = customerTin;
                    OrgCustomer.IsTemporary = true;
                    OrgCustomer.IsActive = false;
                    OrgCustomer.IsCredit = false;

                    db.OrganizationCustomers.Add(OrgCustomer);
                    int result = db.SaveChanges();

                    if (result > 0)
                        return OrgCustomer;
                }
                return null;
            }
        }

        public static void SendSalesToBankReconciliation(CashSalesInvoice invoice, string userName)
        {
             AccountHelper _accountHelper = new AccountHelper();

            var db = new ExceedDbContext();
            #region pass data for reconciliation

            var distributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == invoice.CashSalesInvoiceID).ToList();

            foreach (SalesInvoicePaymentDistibution distribution in distributions)
            {
                string sourceid = invoice.CashSalesInvoiceID + "-" + distribution.SalesInvoicePaymentDistibutionId;
                //check if this is bank account
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var book = new ReconciliationBookAccount
                    {
                        CreatedBy = userName,
                        DateCreated = DateTime.Now,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit,
                        Date = invoice.Date,
                        Type = BankReconciliationType.Sales.ToString(),
                        ReferenceDoc = !String.IsNullOrWhiteSpace(invoice.ReceiptReference) ? invoice.ReceiptReference : "NA",
                        Reference = !String.IsNullOrWhiteSpace(invoice.References) ? invoice.References : "NA",
                        Source = JournalSources.Sales.ToString(),
                        SourceID = sourceid,
                        AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                    };
                    db.ReconcilationBookAccounts.Add(book);
                    db.SaveChanges();
                }
            }
            #endregion
        }
        public static void SendDepositToBankReconciliation(CRMDeposit deposit, string userName)
        {
            AccountHelper _accountHelper = new AccountHelper();

            var db = new ExceedDbContext();
            #region pass data for reconciliation

            var distributions = db.CRMDepositDistribution.Where(d => d.CRMDepositId == deposit.CRMDepositId).ToList();

            foreach (CRMDepositDistribution distribution in distributions)
            {
                string sourceid = deposit.CRMDepositId + "-" + distribution.CRMDepositDistributionId;
                //check if this is bank account
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var book = new ReconciliationBookAccount
                    {
                        CreatedBy = userName,
                        DateCreated = DateTime.Now,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit,
                        Date = deposit.Date,
                        Type = BankReconciliationType.Deposit.ToString(),
                        ReferenceDoc = !String.IsNullOrWhiteSpace(deposit.Remark) ? deposit.Remark : "NA",
                        Reference = !String.IsNullOrWhiteSpace(deposit.SalesPersonFullName) ? deposit.SalesPersonFullName : "NA",
                        Source = JournalSources.Sales.ToString(),
                        SourceID = sourceid,
                        AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                    };
                    db.ReconcilationBookAccounts.Add(book);
                    db.SaveChanges();
                }
            }
            #endregion
        }
        public static void SendCreditSettlementToBankReconciliation(CreditSettlement creditSettlement, string userName)
        {
            AccountHelper _accountHelper = new AccountHelper();

            var db = new ExceedDbContext();
            #region pass data for reconciliation

            var distributions = db.CreditSettlementDistribution.Where(d => d.CreditSettlementId == creditSettlement.CreditSettlementId).ToList();

            foreach (CreditSettlementDistribution distribution in distributions)
            {
                string sourceid = creditSettlement.CreditSettlementId + "-" + distribution.CreditSettlementDistributionId;
                //check if this is bank account
                if (_accountHelper.IsBankAccount(distribution.AccountCode))
                {
                    var book = new ReconciliationBookAccount
                    {
                        CreatedBy = userName,
                        DateCreated = DateTime.Now,
                        AccountCode = distribution.AccountCode,
                        Debit = distribution.Debit,
                        Credit = distribution.Credit,
                        Date = creditSettlement.Date,
                        Type = BankReconciliationType.Credit_Settlement.ToString(),
                        ReferenceDoc = !String.IsNullOrWhiteSpace(creditSettlement.ReceiptNumber) ? creditSettlement.ReceiptNumber : "NA",
                        Reference = !String.IsNullOrWhiteSpace(creditSettlement.Remark) ? creditSettlement.Remark : "NA",
                        Source = JournalSources.Sales.ToString(),
                        SourceID = sourceid,
                        AccountSegment = _accountHelper.GetNaturalAccountSegment(distribution.AccountCode),
                        SubAccountSegment = _accountHelper.GetSubAccountSegment(distribution.AccountCode)
                    };
                    db.ReconcilationBookAccounts.Add(book);
                    db.SaveChanges();
                }
            }
            #endregion
        }

        public static void CreateShipmentRecordFromInvoice(CashSalesInvoice invoice, string userName)
        {
            if(new CRMSalesHelper().SalesInvoiceHasUnShipedLineLitems(invoice, "2"))
            {
                var db = new ExceedDbContext();
                var record = new SalesShipment
                {
                    Source = ShipmentSource.SalesInvoice,
                    BranchId = invoice.BranchId,
                    StoreID = invoice.StoreID,
                    CRMPeriodId = invoice.CRMPeriodId,
                    ShipmentDate = invoice.Date,
                    IsSendForApproval = true,
                    SendForApprovalBy = userName,
                    SendForApprovalTime = invoice.Date,
                    IsOnlineApproved = true,
                    OnlineApprovedBy = userName,
                    OnlineApprovedTime = invoice.Date,
                    Status = "Approved",
                    Remark = "auto generated",
                    IsOnlineTransferred = false,
                    IsVoid = false,

                    OrganizationCustomerId = invoice.OrganizationCustomerId,
                    CashSalesInvoiceID = invoice.CashSalesInvoiceID,
                    FsInvoiceNumber = invoice.FsInvoiceNo,

                    CreatedBy = userName,
                    DateCreated = DateTime.Now,
                };
                db.SalesShipments.Add(record);
                var res = db.SaveChanges();
                if (res > 0)
                {
                    #region create line
                    var invoiceLines = db.LineItems.Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2").ToList();
                    foreach (LineItem invoiceLineItem in invoiceLines)
                    {
                        decimal remainingQty = invoiceLineItem.Quantity;
                        decimal shippedBefore = 0;
                        var existingShipmentLineItem = db.SalesShipmentLineItem.Where(li => li.LineItemId == invoiceLineItem.LineItemID).ToList();
                        if (existingShipmentLineItem.Any())
                        {
                            shippedBefore = existingShipmentLineItem.Sum(i => i.ShipedQuantity);
                            remainingQty = invoiceLineItem.Quantity - shippedBefore;
                        }
                        if (remainingQty > 0)
                        {
                            var shipmentLineRecord = new SalesShipmentLineItem
                            {
                                SalesShipmentId = record.SalesShipmentId,
                                SalesStoreId = invoiceLineItem.StoreID,
                                ProductId = invoiceLineItem.ThingID,
                                Class = invoiceLineItem.Class,
                                Variety = invoiceLineItem.Variety,

                                LineItemId = invoiceLineItem.LineItemID,
                                Price = invoiceLineItem.UnitAmount,
                                SubTotal = invoiceLineItem.CalculatedCost,
                                TaxAmount = invoiceLineItem.TaxAmount,
                                TotalAmount = invoiceLineItem.TotalAmount,
                                TotalQuantity = invoiceLineItem.Quantity,
                                ShipedQuantity = remainingQty,
                                RemainingQuantity = 0,
                                ShipedQuantityTilPreShipment = shippedBefore,

                                CreatedBy = userName,
                                DateCreated = DateTime.Now,
                                LastModified = null,
                                ModifiedBy = null,
                            };
                            db.SalesShipmentLineItem.Add(shipmentLineRecord);
                            //db.SaveChanges();
                        }

                    }
                    var result = db.SaveChanges();
                    if (result > 0)
                    {
                        #region update 'is-shipped' status of the invoice
                        var selectedInvoice = db.CashSalesInvoices.Find(invoice.CashSalesInvoiceID);
                        if (selectedInvoice != null)
                        {
                            selectedInvoice.IsShipped = true;
                            db.CashSalesInvoices.Attach(selectedInvoice);
                            db.Entry(selectedInvoice).State = EntityState.Modified;

                            db.SaveChanges();
                        }
                        #endregion

                        new CRMPaymentDistibutionHelper().UpdateShipmentDistribution(record);


                        UpdateInvontoryOnAutoGeratingShipment(record);
                    }
                    #endregion
                }
            }
        }

        public void UpdateInvoiceOrSalesOrderShipmentStatusOnShipmentEdit(SalesShipment shipment)
        {
            var db = new ExceedDbContext();
            if (shipment.Source == ShipmentSource.SalesInvoice)
            {
                var invoice = db.CashSalesInvoices.Find(shipment.CashSalesInvoiceID);
                if (invoice != null)
                {
                    if (SalesInvoiceHasUnShipedLineLitems(invoice, "2") == false)
                    {
                        invoice.IsShipped = true;
                    }
                    else
                    {
                        invoice.IsShipped = false;
                    }
                    db.CashSalesInvoices.Attach(invoice);
                    db.Entry(invoice).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            else if (shipment.Source == ShipmentSource.SalesOrder)
            {
                var order = db.SalesOrderVouchers.Find(shipment.SalesOrderVoucherID);
                if (order != null)
                {
                    if (SalesOrderVoucherHasUnShipedLineLitems(order, "1") == false)
                    {
                        order.IsShipped = true;
                    }
                    else
                    {
                        order.IsShipped = false;
                    }

                    db.SalesOrderVouchers.Attach(order);
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        private static bool UpdateInvontoryOnAutoGeratingShipment(SalesShipment shipmentRecord)
        {
            var db = new ExceedDbContext();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            List<BinCardLine> binCardLines = new List<BinCardLine>();
            bool transactionCompletedSuccessfully = false;
            var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == shipmentRecord.SalesShipmentId).ToList();
            foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
            {
                decimal beforeTransaction = 0;
                decimal afterTransaction = 0;
                decimal TransactionQuantity = 0;

                //LineItem items = db.LineItems.Where(x => x.LineItemID.ToString() == item.Id && x.VoucherDefinitionID == "2").FirstOrDefault();
                //if (items.Quantity > 0)
                //{

                var salesStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
                //CashSalesInvoice sales = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID.ToString() == items.Voucher).FirstOrDefault();
                InventoryStore Store =
                  db.InventoryStores.FirstOrDefault(
                      x => x.StoreCode == salesStore.StoreCode);
                Product product =
                    db.Products.FirstOrDefault(x => x.ProductID == shipmentLineItem.ProductId);

                StoreItemAssignment storeItem =
                    db.StoreItemAssignments
                    .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
                                x.InventoryItem.ItemCode == product.ItemCode)
                                .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();
                if (Store != null && product != null && storeItem != null)
                {
                    decimal finalQuantity = storeItem.Balance - shipmentLineItem.ShipedQuantity;

                    if (shipmentLineItem.ShipedQuantity > storeItem.Balance)
                    {
                        finalQuantity = 0;
                    }
                    else
                    {
                        beforeTransaction = storeItem.Balance;
                        storeItem.Balance = finalQuantity;

                        afterTransaction = finalQuantity;
                        TransactionQuantity = shipmentLineItem.ShipedQuantity;

                        db.StoreItemAssignments.Attach(storeItem);
                        db.Entry(storeItem).State = EntityState.Modified;
                        //db.SaveChanges();

                        string payMethod = "";
                        string customer = "";
                        if (shipmentRecord.Source == ShipmentSource.SalesInvoice)
                        {
                            var invoice = db.CashSalesInvoices.FirstOrDefault(i => i.CashSalesInvoiceID == shipmentRecord.CashSalesInvoiceID);
                            if (invoice != null)
                            {
                                customer = invoice.OrganizationCustomer.TradeName;
                                payMethod = invoice.PaymentMethod;
                            }
                        }
                        else
                        {
                            var order = db.SalesOrderVouchers.FirstOrDefault(i => i.SalesOrderVoucherID == shipmentRecord.SalesOrderVoucherID);
                            if (order != null)
                            {
                                customer = order.OrganizationCustomer.TradeName;
                                payMethod = order.PaymentMethod;
                            }
                        }
                        //var Transaction = new StoreTransaction
                        //{
                        //    Voucher = shipmentLineItem.SalesShipment.CashSalesInvoiceID.ToString(),
                        //    Source = salesStore.Description + " -- " + product.Name,
                        //    //Destination = shipmentRecord.CashSalesInvoice.OrganizationCustomer.TradeName,
                        //    Destination = customer,
                        //    Amount = (shipmentLineItem.Price * shipmentLineItem.ShipedQuantity).ToString(),
                        //    Quantity = TransactionQuantity,
                        //    beforeStock = beforeTransaction,
                        //    afterStock = afterTransaction,
                        //    //TransactionType = shipmentRecord.CashSalesInvoice.PaymentMethod
                        //    TransactionType = payMethod
                        //};

                        //db.StoreTransactions.Add(Transaction);
                        //db.SaveChanges();

                        #region update bin card

                        #region bin card line
                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Shipment " + shipmentRecord.SalesShipmentId.ToString(),
                            Reference = shipmentRecord.Source == ShipmentSource.SalesInvoice ? "Invoice " + shipmentRecord.CashSalesInvoiceID : "Sales Order " + shipmentRecord.SalesOrderVoucherID,
                            Date = shipmentRecord.ShipmentDate,
                            IssuedQTY = shipmentLineItem.ShipedQuantity,
                            IssuedUPrice = shipmentLineItem.Price,
                            IssuedTPrice = shipmentLineItem.TotalAmount,
                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = uPrice,
                            BalanceTPrice = uPrice * storeItem.Balance,
                            IssuedTo = "",
                            //IssuedTo = shipmentRecord.OrganizationCustomerId.ToString(),
                            //new    
                            StoreCode = GetInventoryStoreCode(shipmentLineItem.SalesStoreId),
                            GLPeriodId = GetGlPeriod(shipmentRecord.CRMPeriodId),
                            TransactionType = BinCardTransactionTypes.Issue,
                            StatusType = BinCardTransactionStatus.Regular,

                        };
                        db.BinCardLines.Add(binCardLine);
                        binCardLines.Add(binCardLine);
                        // db.SaveChanges();

                        #endregion

                        #endregion

                        //transactionCompletedSuccessfully = true;
                    }
                }


                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                {
                    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(salesStore?.StoreCode);
                    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                        .FirstOrDefault(x => x.ItemCode == product.ItemCode);
                    if (branchItem != null)
                    {
                        branchItem.Balance = branchItem.Balance - shipmentLineItem.ShipedQuantity;
                        db.Entry(branchItem).State = EntityState.Modified;
                        // db.SaveChanges();
                    }
                }

                if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                {
                    var stockItem = db.InventoryItems.Find(product.ItemCode);
                    if (stockItem != null)
                    {
                        stockItem.Balance = stockItem.Balance - shipmentLineItem.ShipedQuantity;
                        db.Entry(stockItem).State = EntityState.Modified;
                        // db.SaveChanges();
                    }
                }
            }
            var result = db.SaveChanges();
            if (result >= 1)
            {
                transactionCompletedSuccessfully = true;
                List<int> ids = binCardLines.Select(b => b.BinCardLineID).ToList();
                InventoryBalanceHelper.AutoRecalculateBincardLines(ids);
            }
            return transactionCompletedSuccessfully;
        }
        private static bool ReverseInventoryChangesOnShipmentVoid(SalesShipment shipmentRecord)
        {
            var db = new ExceedDbContext();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();

            bool transactionCompletedSuccessfully = false;
            var shipmentLineItems = db.SalesShipmentLineItem.Where(li => li.SalesShipmentId == shipmentRecord.SalesShipmentId).ToList();
            var bincardLinesFromThisShipment = db.BinCardLines.Where(bl => bl.ReferenceDoc == $"Shipment ${shipmentRecord.SalesShipmentId}").ToList();
            if (bincardLinesFromThisShipment.Count() <= shipmentLineItems.Count())
            {
                foreach (SalesShipmentLineItem shipmentLineItem in shipmentLineItems)
                {
                    //decimal beforeTransaction = 0;
                    //decimal afterTransaction = 0;
                    //decimal TransactionQuantity = 0;

                    var salesStore = db.SalesStores.Find(shipmentLineItem.SalesStoreId);
                    InventoryStore Store =
                      db.InventoryStores.FirstOrDefault(
                          x => x.StoreCode == salesStore.StoreCode);
                    Product product =
                        db.Products.FirstOrDefault(x => x.ProductID == shipmentLineItem.ProductId);

                    StoreItemAssignment storeItem =
                        db.StoreItemAssignments
                        .Where(x => x.InventoryStore.StoreCode == Store.StoreCode &&
                                    x.InventoryItem.ItemCode == product.ItemCode)
                                    .OrderByDescending(x => x.StoreItemAssignmentID).FirstOrDefault();
                    if (Store != null && product != null && storeItem != null)
                    {
                        decimal finalQuantity = storeItem.Balance + shipmentLineItem.ShipedQuantity;

                        storeItem.Balance = finalQuantity;

                        db.StoreItemAssignments.Attach(storeItem);
                        db.Entry(storeItem).State = EntityState.Modified;
                        //db.SaveChanges();



                        #region update bin card

                        #region bin card line
                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, Store.StoreCode);
                        BinCardLine binCardLine = new BinCardLine
                        {
                            StoreItemAssignment = storeItem,
                            ReferenceDoc = "Shipment " + shipmentRecord.SalesShipmentId.ToString(),
                            Reference = shipmentRecord.Source == ShipmentSource.SalesInvoice ? "Invoice " + shipmentRecord.CashSalesInvoiceID : "Sales Order " + shipmentRecord.SalesOrderVoucherID,
                            Date = shipmentRecord.ShipmentDate,
                            IssuedQTY = shipmentLineItem.ShipedQuantity,
                            IssuedUPrice = shipmentLineItem.Price,
                            IssuedTPrice = shipmentLineItem.TotalAmount,
                            BalanceQTY = storeItem.Balance,
                            BalanceUPrice = uPrice,
                            BalanceTPrice = uPrice * storeItem.Balance,
                            IssuedTo = "",
                            //IssuedTo = shipmentRecord.OrganizationCustomerId.ToString(),
                            //new    
                            StoreCode = GetInventoryStoreCode(shipmentLineItem.SalesStoreId),
                            GLPeriodId = GetGlPeriod(shipmentRecord.CRMPeriodId),
                            TransactionType = BinCardTransactionTypes.Issue,
                            StatusType = BinCardTransactionStatus.Regular,

                        };
                        db.BinCardLines.Add(binCardLine);
                        #endregion

                        #endregion

                    }
                }
                var result = db.SaveChanges();
                if (result >= 1)
                {
                    transactionCompletedSuccessfully = true;
                }
            }

            return transactionCompletedSuccessfully;
        }
        private static string GetInventoryStoreCode(int salesStoreId)
        {
            var db = new ExceedDbContext();
            return db.SalesStores.Find(salesStoreId)?.StoreCode;
        }
        private static int GetGlPeriod(int crmPeriodId)
        {
            var db = new ExceedDbContext();

            int id = 0;
            var crmPeriod = db.CRMPeriods.Find(crmPeriodId);
            if (crmPeriod != null)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(p => p.Name == crmPeriod.Name);
                if (glPeriod != null)
                {
                    id = glPeriod.GLPeriodId;
                }
            }
            return id;
        }


    }

}
