using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
    public class CRMPaymentDistibutionHelper
    {
        //public List<SalesInvoicePaymentDistibution> DistributeCashInvoice(CashSalesInvoice Invoice, LineItem Item, List<AdditionalCost> AdditionalCosts, List<AdditionalCostLog> AdditionalCostLogs, List<GLTaxRate> TaxRates, List<PaymentMethod> PaymentMethods, List<AccountMap> AccountMap)
        //{
        //    AccountHelper AccountHelper = new AccountHelper();

        //    var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();

        //    #region debit total net receivable.
        //    var PaymentMethod = PaymentMethods.Where(x => x.PaymentMethodID.ToString() == Invoice.RecieveMethod).FirstOrDefault();
        //    if (PaymentMethod != null)
        //    {
        //        var totalReceivableAmountDbt = new SalesInvoicePaymentDistibution
        //        {
        //            AccountCode = PaymentMethod.ArAccountCode,
        //            //Debit = Item.Quantity * Item.UnitAmount,
        //            Debit = Item.WithholdTax ? Item.TotalAmount - Item.WithholdingAmount - Item.TaxAmount : Item.TotalAmount - Item.WithholdingAmount,
        //            AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),
        //            CashSalesInvoiceId = Invoice.CashSalesInvoiceID
        //        };
        //        SalesInvoicePaymentDistibutions.Add(totalReceivableAmountDbt);
        //    }
        //    #endregion

        //    var AdditionalCostlog = AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
        //    foreach (var log in AdditionalCostlog)
        //    {
        //        if (log != null)
        //        {
        //            var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //            {
        //                AccountCode = log.AdditionalCost.SalesAccountCode,
        //                Debit = log.Value,
        //                AccountDesc = AccountHelper.GetAccountDesc(log.AdditionalCost.SalesAccountCode),
        //                CashSalesInvoiceId = Invoice.CashSalesInvoiceID

        //            };
        //            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //        }
        //    }
        //    #region debit withholding tax
        //    if (!string.IsNullOrEmpty(Item.WithholdingType))
        //    {
        //        var WithHolding = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.WithholdingType).FirstOrDefault();

        //        if (WithHolding != null)
        //        {
        //            var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //            {
        //                AccountCode = WithHolding.WithholdingAccountCode,
        //                Debit = Item.WithholdingAmount,
        //                AccountDesc = AccountHelper.GetAccountDesc(WithHolding.WithholdingAccountCode),
        //                CashSalesInvoiceId = Invoice.CashSalesInvoiceID

        //            };
        //            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //        }
        //    }
        //    #endregion
        //    #region Credit Tax (VAT)

        //    var TaxRate = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.Tax).FirstOrDefault();
        //    if (TaxRate != null)
        //    {
        //        var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //        {

        //            AccountCode = TaxRate.ArAccountCode,
        //            Credit = Item.TaxAmount,
        //            AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
        //            CashSalesInvoiceId = Invoice.CashSalesInvoiceID
        //        };
        //        SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //        if (Item.WithholdTax)
        //        {

        //            SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //            {

        //                AccountCode = TaxRate.ArAccountCode,
        //                Debit = Item.TaxAmount,
        //                AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
        //                CashSalesInvoiceId = Invoice.CashSalesInvoiceID
        //            };
        //            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //        }
        //    }
        //    #endregion
        //    #region Credit Sub-total
        //    var ProductAccount = AccountMap.Where(x => x.Reference == Item.Product.ProductID.ToString()).FirstOrDefault();

        //    if (ProductAccount != null)
        //    {
        //        var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //        {
        //            AccountCode = ProductAccount.AccountID,
        //            AccountDesc = AccountHelper.GetAccountDesc(ProductAccount.AccountID),
        //            Credit = Item.Quantity * Item.UnitAmount,
        //            CashSalesInvoiceId = Invoice.CashSalesInvoiceID
        //        };
        //        SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //    }
        //    #endregion
        //    return SalesInvoicePaymentDistibutions;
        //}
        public void UpdateCashSalesInvoiceDistribution(CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<SalesInvoicePaymentDistibution> distributions = new List<SalesInvoicePaymentDistibution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();
            var invoiceLineItems = db.LineItems.Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2").ToList();
            //var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == invoice.OrganizationCustomerId.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == invoice.CashSalesInvoiceID).ToList();
            if (existingDistributions.Any())
            {
                db.SalesInvoicePaymentDistibutions.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion
            #region debit net receivable amount
            var payMetMethod = db.PaymentMethods.Where(x => x.PaymentMethodID.ToString() == invoice.RecieveMethod).FirstOrDefault();
            if (payMetMethod != null)
            {
                foreach (var invoiceItem in invoiceLineItems)
                {

                    //AccountHelper accountHelper = new AccountHelper();
                    string acc = payMetMethod.ArAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    //string branchSeg = HrHelper.GetBranchSubAccount(invoiceItem.SalesStore.InventoryStore.BranchId);
                    //string branchSeg = HrHelper.GetBranchSubAccount(invoice.BranchId);
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);


                    if (invoiceItem.TotalAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.WithholdTax ? invoiceItem.TotalAmount - invoiceItem.WithholdingAmount - invoiceItem.TaxAmount : invoiceItem.TotalAmount - invoiceItem.WithholdingAmount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TotalAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = invoiceItem.WithholdTax ? invoiceItem.TotalAmount - invoiceItem.WithholdingAmount - invoiceItem.TaxAmount : invoiceItem.TotalAmount - invoiceItem.WithholdingAmount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
                foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
                {
                    db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();
            }
            #endregion
            #region debit discount
            if (payMetMethod != null)
            {
                foreach (var invoiceItem in invoiceLineItems)
                {
                    string acc = payMetMethod.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.Discount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.Discount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.Discount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = invoiceItem.Discount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
                foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
                {
                    db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();
            }
            #endregion
            #region debit VAT withholding (if VAT withholding is true)
            foreach (var invoiceItem in invoiceLineItems)
            {
                if (invoiceItem.WithholdTax)
                {
                    var tax = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.Tax).FirstOrDefault();

                    if (tax != null)
                    {
                        string acc = tax.WithholdingAccountCode;

                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                        if (invoiceItem.TaxAmount != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Debit += invoiceItem.TaxAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (invoiceItem.TotalAmount != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = invoiceItem.TaxAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, invoiceItem.LineItemID);
                        }
                    }
                }


            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #region debit withholding tax
            foreach (var invoiceItem in invoiceLineItems)
            {
                if (!string.IsNullOrEmpty(invoiceItem.WithholdingType))
                {
                    var WithHolding = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.WithholdingType).FirstOrDefault();

                    if (WithHolding != null)
                    {
                        string acc = WithHolding.WithholdingAccountCode;

                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                        if (invoiceItem.WithholdingAmount != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Debit += invoiceItem.WithholdingAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (invoiceItem.WithholdingAmount != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = invoiceItem.WithholdingAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, invoiceItem.LineItemID);
                        }
                    }
                }


            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion

            #region credit sub-total
            foreach (var invoiceItem in invoiceLineItems)
            {
                // crm category, not inventory
                var category = GetProductCategory(invoiceItem.Product.CategoryID);

                //var productAccount = db.AccountMaps.Where(x => x.Reference == invoiceItem.Product.ProductID.ToString()).FirstOrDefault();
                if (category != null)
                {
                    string acc = category.UnEarnedAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.CalculatedCost != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += invoiceItem.CalculatedCost;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.CalculatedCost != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Credit = invoiceItem.CalculatedCost,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
            #region credit TAX
            foreach (var invoiceItem in invoiceLineItems)
            {
                var taxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.Tax).FirstOrDefault();
                if (taxRate != null)
                {
                    string acc = taxRate.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoiceItem.SalesStore.InventoryStore.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.TaxAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += invoiceItem.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TaxAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Credit = invoiceItem.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
        }
        public List<SalesInvoicePaymentDistibution> DistributeCreditInvoice(CashSalesInvoice Invoice, LineItem Item, List<AdditionalCost> AdditionalCosts, List<AdditionalCostLog> AdditionalCostLogs, List<GLTaxRate> TaxRates, List<PaymentMethod> PaymentMethods, List<AccountMap> AccountMap)
        {
            AccountHelper AccountHelper = new AccountHelper();
            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();

            var PaymentMethod = PaymentMethods.Where(x => x.PaymentMethodID.ToString() == Invoice.RecieveMethod).FirstOrDefault();
            if (PaymentMethod != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {
                    AccountCode = PaymentMethod.ArAccountCode,
                    //Debit = Item.Quantity * Item.UnitAmount,
                    Debit = Item.TotalAmount,
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                    AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),

                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }

            //var AdditionalCostlog = AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
            //foreach (var log in AdditionalCostlog)
            //{
            //    if (log != null)
            //    {
            //        var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            //        {
            //            AccountCode = log.AdditionalCost.SalesAccountCode,
            //            Debit = log.Value,
            //            CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
            //            AccountDesc = AccountHelper.GetAccountDesc(log.AdditionalCost.SalesAccountCode),
            //        };
            //        SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            //    }
            //}

            var TaxRate = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.Tax).FirstOrDefault();

            if (TaxRate != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {

                    AccountCode = TaxRate.ArAccountCode,
                    Credit = Item.TaxAmount,
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                    AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);


            }
            var ProductAccount = AccountMap.Where(x => x.Reference == Item.Product.ProductID.ToString()).FirstOrDefault();

            if (ProductAccount != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {
                    AccountCode = ProductAccount.AccountID,
                    Debit = Item.Quantity * Item.UnitAmount,
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                    AccountDesc = AccountHelper.GetAccountDesc(ProductAccount.AccountID)
                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }
            return SalesInvoicePaymentDistibutions;
        }

        public void DistributeCreditInvoice(CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<SalesInvoicePaymentDistibution> distributions = new List<SalesInvoicePaymentDistibution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();
            var invoiceLineItems = db.LineItems.Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString() && l.VoucherDefinitionID == "2").ToList();
            var customerAccount =
                db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" &&
                                            m.Reference == invoice.OrganizationCustomerId.ToString() &&
                                            m.ControlAccountID == AccountRequirementDefinition.SALES_ACCOUNT.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == invoice.CashSalesInvoiceID).ToList();
            if (existingDistributions.Any())
            {
                db.SalesInvoicePaymentDistibutions.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion

            if (customerAccount != null)
            {
                #region debit total amount
                foreach (var invoiceItem in invoiceLineItems)
                {
                    //AccountHelper accountHelper = new AccountHelper();
                    string acc = customerAccount.AccountID;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.TotalAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.TotalAmount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TotalAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = invoiceItem.TotalAmount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
                foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
                {
                    db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
                #region debit discount
                foreach (var invoiceItem in invoiceLineItems)
                {
                    //AccountHelper accountHelper = new AccountHelper();
                    string acc = customerAccount.AccountID;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.Discount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += invoiceItem.Discount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.Discount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = invoiceItem.Discount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
                foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
                {
                    db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
            }
            #region credit sub-total
            foreach (var invoiceItem in invoiceLineItems)
            {
                var category = GetProductCategory(invoiceItem.Product.CategoryID);
                //var productAccount = db.AccountMaps.Where(x => x.Reference == invoiceItem.Product.ProductID.ToString()).FirstOrDefault();
                if (category != null)
                {
                    string acc = category.UnEarnedAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.CalculatedCost != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += invoiceItem.CalculatedCost;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.CalculatedCost != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Credit = invoiceItem.CalculatedCost,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
            #region credit TAX
            foreach (var invoiceItem in invoiceLineItems)
            {
                var taxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.Tax).FirstOrDefault();
                if (taxRate != null)
                {
                    string acc = taxRate.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (invoiceItem.TaxAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += invoiceItem.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TaxAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Credit = invoiceItem.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, invoiceItem.LineItemID);
                    }
                }
            }
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
            //}



            //var PaymentMethod = PaymentMethods.Where(x => x.PaymentMethodID.ToString() == Invoice.RecieveMethod).FirstOrDefault();
            //if (PaymentMethod != null)
            //{
            //    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            //    {
            //        AccountCode = PaymentMethod.ArAccountCode,
            //        //Debit = Item.Quantity * Item.UnitAmount,
            //        Debit = Item.TotalAmount,
            //        CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
            //        AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),

            //    };
            //    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);



            //var AdditionalCostlog = AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
            //foreach (var log in AdditionalCostlog)
            //{
            //    if (log != null)
            //    {
            //        var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            //        {
            //            AccountCode = log.AdditionalCost.SalesAccountCode,
            //            Debit = log.Value,
            //            CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
            //            AccountDesc = AccountHelper.GetAccountDesc(log.AdditionalCost.SalesAccountCode),
            //        };
            //        SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            //    }
            //}

            //var TaxRate = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.Tax).FirstOrDefault();

            //if (TaxRate != null)
            //{
            //    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            //    {

            //        AccountCode = TaxRate.ArAccountCode,
            //        Credit = Item.TaxAmount,
            //        CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
            //        AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
            //    };
            //    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);


            //}
            //var ProductAccount = AccountMap.Where(x => x.Reference == Item.Product.ProductID.ToString()).FirstOrDefault();

            //if (ProductAccount != null)
            //{
            //    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            //    {
            //        AccountCode = ProductAccount.AccountID,
            //        Debit = Item.Quantity * Item.UnitAmount,
            //        CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
            //        AccountDesc = AccountHelper.GetAccountDesc(ProductAccount.AccountID)
            //    };
            //    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            //}
        }
        public void UpdateAdvancePaymentDistribution(AdvancePayment advancePayment)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<AdvancePaymentDistibution> distributions = new List<AdvancePaymentDistibution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var lineItems = db.AdvancePaymentLines.Where(l => l.AdvancePaymentId == advancePayment.AdvancePaymentId).ToList();
            var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == advancePayment.Customer.ToString() && m.ControlAccountID == AccountRequirementDefinition.SALES_ACCOUNT.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.AdvancePaymentDistibutions.Where(d => d.AdvancePaymentId == advancePayment.AdvancePaymentId).ToList();
            if (existingDistributions.Any())
            {
                db.AdvancePaymentDistibutions.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion

            #region debit total received amount

            foreach (var lineItem in lineItems)
            {

                var payMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == lineItem.RecieveMethod).FirstOrDefault();
                if (payMethod != null)
                {
                    string acc = payMethod.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(advancePayment.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                    if (lineItem.Value != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += lineItem.Value;
                            existingDstn.Credit = 0;
                            existingDstn.AdvancePaymentId = advancePayment.AdvancePaymentId;
                        }
                    }
                    else if (lineItem.Value != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        AdvancePaymentDistibution distribution = new AdvancePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = lineItem.Value,
                            Credit = 0,
                            AdvancePaymentId = advancePayment.AdvancePaymentId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.AdvancePaymentLineId);
                    }
                }
            }
            foreach (AdvancePaymentDistibution d in distributions)
            {
                db.AdvancePaymentDistibutions.Add(d);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            if (customerAccount != null)
            {
                #region Credit total amount
                foreach (var lineItem in lineItems)
                {
                    //AccountHelper accountHelper = new AccountHelper();
                    string acc = customerAccount.AccountID;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(advancePayment.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (lineItem.Value != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += lineItem.Value;
                            existingDstn.Debit = 0;
                            existingDstn.AdvancePaymentId = advancePayment.AdvancePaymentId;
                        }
                    }
                    else if (lineItem.Value != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        AdvancePaymentDistibution distribution = new AdvancePaymentDistibution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Credit = lineItem.Value,
                            Debit = 0,
                            AdvancePaymentId = advancePayment.AdvancePaymentId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.AdvancePaymentLineId);
                    }
                }
                foreach (AdvancePaymentDistibution d in distributions)
                {
                    db.AdvancePaymentDistibutions.Add(d);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();

                #endregion
            }

        }
        public List<AdvancePaymentDistibution> DistributeAdvancePayment(AdvancePaymentLine AdvancePaymentLine, OrganizationCustomer OrganizationCustomer, List<AccountMap> AccountMap)
        {
            ExceedDbContext db = new ExceedDbContext();
            AccountHelper AccountHelper = new AccountHelper();
            var AdvancePaymentDistibution = new List<AdvancePaymentDistibution>();

            var PaymentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == AdvancePaymentLine.RecieveMethod).FirstOrDefault();
            var SalesInvoicePaymentDistibution = new AdvancePaymentDistibution
            {
                AccountCode = PaymentMethod.ArAccountCode,
                Debit = AdvancePaymentLine.Value,
                AdvancePaymentId = AdvancePaymentLine.AdvancePaymentId,
                AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),

            };
            AdvancePaymentDistibution.Add(SalesInvoicePaymentDistibution);


            var CustomerAccount = AccountMap.Where(x => x.Reference == OrganizationCustomer.OrganizationCustomerID.ToString()).FirstOrDefault();

            if (CustomerAccount != null)
            {
                SalesInvoicePaymentDistibution = new AdvancePaymentDistibution
                {
                    AccountCode = CustomerAccount.AccountID,
                    Credit = AdvancePaymentLine.Value,
                    AdvancePaymentId = AdvancePaymentLine.AdvancePaymentLineId,
                    AccountDesc = AccountHelper.GetAccountDesc(CustomerAccount.AccountID)
                };
                AdvancePaymentDistibution.Add(SalesInvoicePaymentDistibution);

            }

            return AdvancePaymentDistibution;
        }
        public void UpdateCreditSettlementDistribution(CreditSettlement creditSettlement)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<CreditSettlementDistribution> distributions = new List<CreditSettlementDistribution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            //var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();
            var creditLineItems = db.LineCreditSettlements.Where(l => l.CreditSettlementId == creditSettlement.CreditSettlementId).ToList();
            var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == creditSettlement.Customer.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.CreditSettlementDistribution.Where(d => d.CreditSettlementId == creditSettlement.CreditSettlementId).ToList();
            if (existingDistributions.Any())
            {
                db.CreditSettlementDistribution.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion
            #region debit net  amount
            var payMentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == creditSettlement.RecieveMethod).FirstOrDefault();
            if (payMentMethod != null)
            {
                foreach (var lineItem in creditLineItems)
                {
                    string acc = payMentMethod.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(creditSettlement.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                    if (lineItem.SettelmentAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += lineItem.NetBalance;
                            existingDstn.Credit = 0;
                            existingDstn.CreditSettlementId = creditSettlement.CreditSettlementId;
                        }
                    }
                    else if (lineItem.SettelmentAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        CreditSettlementDistribution distribution = new CreditSettlementDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = lineItem.NetBalance,
                            Credit = 0,
                            CreditSettlementId = creditSettlement.CreditSettlementId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.LineCreditSettlementId);
                    }
                }
                foreach (CreditSettlementDistribution distribution in distributions)
                {
                    db.CreditSettlementDistribution.Add(distribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();
            }
            #endregion
            #region debit withholding tax
            foreach (var lineItem in creditLineItems)
            {
                //if (!string.IsNullOrEmpty(invoiceItem.WithholdingType))
                //{
                var WithHolding = db.GLTaxRates.Where(x => x.GLTaxRateID == lineItem.WithholdingType).FirstOrDefault();

                if (WithHolding != null)
                {
                    string acc = WithHolding.WithholdingAccountCode;

                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(lineItem.CreditSettlement.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


                    if (lineItem.WithholdingAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += lineItem.WithholdingAmount;
                            existingDstn.Credit = 0;
                            existingDstn.CreditSettlementId = lineItem.CreditSettlementId;
                        }
                    }
                    else if (lineItem.WithholdingAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        CreditSettlementDistribution distribution = new CreditSettlementDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = lineItem.WithholdingAmount,
                            Credit = 0,
                            CreditSettlementId = lineItem.CreditSettlementId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.LineCreditSettlementId);
                    }
                }
                //}


            }
            foreach (CreditSettlementDistribution distribution in distributions)
            {
                db.CreditSettlementDistribution.Add(distribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            if (customerAccount != null)
            {
                #region credit total settlement amount
                foreach (var lineItem in creditLineItems)
                {
                    string acc = customerAccount.AccountID;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(creditSettlement.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                    if (lineItem.SettelmentAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Credit += lineItem.SettelmentAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CreditSettlementId = lineItem.CreditSettlementId;
                        }
                    }
                    else if (lineItem.SettelmentAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        CreditSettlementDistribution distribution = new CreditSettlementDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = lineItem.SettelmentAmount,
                            CreditSettlementId = lineItem.CreditSettlementId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.LineCreditSettlementId);
                    }
                }
                foreach (CreditSettlementDistribution distribution in distributions)
                {
                    db.CreditSettlementDistribution.Add(distribution);
                    db.SaveChanges();
                }
                distributions.Clear();
                checkAccount.Clear();
                #endregion
            }
        }

        //public List<SalesInvoicePaymentDistibution> DistributeCreditSettelment(LineCreditSettlement Item, PaymentMethod PaymentMethod, int Customer, List<AccountMap> AccountMap)
        //{
        //    AccountHelper AccountHelper = new AccountHelper();

        //    ExceedDbContext db = new ExceedDbContext();

        //    var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();


        //    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //    {
        //        AccountCode = PaymentMethod.ArAccountCode,
        //        Debit = Item.SettelmentAmount,
        //        CashSalesInvoiceId = Item.InvoiceId,
        //        AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),

        //    };
        //    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);


        //    var CustomerAccount = AccountMap.Where(x => x.Reference == Customer.ToString()).FirstOrDefault();

        //    if (CustomerAccount != null)
        //    {
        //        SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
        //        {
        //            AccountCode = CustomerAccount.AccountID,
        //            Credit = Item.SettelmentAmount,
        //            CashSalesInvoiceId = Item.InvoiceId,
        //            AccountDesc = AccountHelper.GetAccountDesc(CustomerAccount.AccountID)
        //        };
        //        SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

        //    }

        //    return SalesInvoicePaymentDistibutions;


        //}

        public List<SalesInvoicePaymentDistibution> DistributeProduct(CashSalesInvoice Invoice, ItemCategory ItemCategory, LineItem Item, List<AccountMap> AccountMap)
        {

            AccountHelper AccountHelper = new AccountHelper();

            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();


            var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            {
                AccountCode = ItemCategory.IssueAccountCode,
                Debit = Item.Quantity * Item.UnitAmount,
                CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                AccountDesc = AccountHelper.GetAccountDesc(ItemCategory.IssueAccountCode),

            };
            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);


            SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            {
                AccountCode = ItemCategory.AccountCode,
                Credit = Item.Quantity * Item.UnitAmount,
                CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                AccountDesc = AccountHelper.GetAccountDesc(ItemCategory.AccountCode),

            };
            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);



            return SalesInvoicePaymentDistibutions;
        }

        public void UpdateCRMDepositDistribution(CRMDeposit cRMDeposit)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<CRMDepositDistribution> distributions = new List<CRMDepositDistribution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            //var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();
            var lineItems = db.CRMDepositLine.Where(l => l.CRMDepositId == cRMDeposit.CRMDepositId).ToList();
            //var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == creditSettlement.Customer.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.CRMDepositDistribution.Where(d => d.CRMDepositId == cRMDeposit.CRMDepositId).ToList();
            if (existingDistributions.Any())
            {
                db.CRMDepositDistribution.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion
            #region debit bank account
            foreach (var lineItem in lineItems)
            {
                var payMentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == lineItem.ReceiveMethodId).FirstOrDefault();
                if (payMentMethod != null)
                {
                    string acc = payMentMethod.ArAccountCode;
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(cRMDeposit.BranchId);
                    string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                    if (lineItem.DepositAmount != 0 && checkAccount.ContainsKey(newacc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = newacc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                            existingDstn.Debit += lineItem.DepositAmount;
                            existingDstn.Credit = 0;
                            existingDstn.CRMDepositId = cRMDeposit.CRMDepositId;
                        }
                    }
                    else if (lineItem.DepositAmount != 0 && !checkAccount.ContainsKey(newacc))
                    {
                        CRMDepositDistribution distribution = new CRMDepositDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                            Debit = lineItem.DepositAmount,
                            Credit = 0,
                            CRMDepositId = cRMDeposit.CRMDepositId,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(newacc, lineItem.CRMDepositLineId);
                    }
                }

            }
            foreach (CRMDepositDistribution distribution in distributions)
            {
                db.CRMDepositDistribution.Add(distribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion

            #region credit cashier account
            foreach (var lineItem in lineItems)
            {
                var salesPersonAssignment = db.SalesPersonAssignments.FirstOrDefault(sp => sp.UserName == cRMDeposit.SalesPersonUserName && sp.SalesStoreId == cRMDeposit.StoreID);
                if (salesPersonAssignment != null)
                {
                    // changed logic 
                    //var payMentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == salesPersonAssignment.PaymentMethodId).FirstOrDefault();
                    var payMentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == salesPersonAssignment.CashOnHandPaymentMethodId).FirstOrDefault();
                    if (payMentMethod != null)
                    {
                        string acc = payMentMethod.ArAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(cRMDeposit.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.DepositAmount != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Debit = 0;
                                existingDstn.Credit += lineItem.DepositAmount;
                                existingDstn.CRMDepositId = cRMDeposit.CRMDepositId;
                            }
                        }
                        else if (lineItem.DepositAmount != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            CRMDepositDistribution distribution = new CRMDepositDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = 0,
                                Credit = lineItem.DepositAmount,
                                CRMDepositId = cRMDeposit.CRMDepositId,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.CRMDepositLineId);
                        }
                    }

                }

            }
            foreach (CRMDepositDistribution distribution in distributions)
            {
                db.CRMDepositDistribution.Add(distribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
        }
        public void UpdateShipmentDistribution(SalesShipment salesShipment)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<SalesShipmentDistribution> distributions = new List<SalesShipmentDistribution>();


            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var lineItems = db.SalesShipmentLineItem.Where(l => l.SalesShipmentId == salesShipment.SalesShipmentId).ToList();
            //var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == advancePayment.Customer.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.SalesShipmentDistribution.Where(d => d.SalesShipmentId == salesShipment.SalesShipmentId).ToList();
            if (existingDistributions.Any())
            {
                db.SalesShipmentDistribution.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion

            #region Cost distribution
            #region debit Total product cost…. issued or shipped
            foreach (var lineItem in lineItems)
            {
                //var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
                //var payMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == lineItem.RecieveMethod).FirstOrDefault();
                var product = db.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    var inventoryItem = db.InventoryItems.FirstOrDefault(i => i.ItemCode == product.ItemCode);
                    if (inventoryItem != null)
                    {
                        //var issueAccountCode = inventoryItem.ItemCategory.IssueAccountCode;
                        string acc = inventoryItem.ItemCategory.IssueAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                //existingDstn.Debit += (lineItem.ShipedQuantity * lineItem.Price);
                                existingDstn.Debit += (lineItem.ShipedQuantity * inventoryItem.AvgPrice);
                                existingDstn.Credit = 0;
                                existingDstn.DistributionType = SalesShipmentDistributionType.CostDistribution;
                                existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
                            }
                        }
                        else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesShipmentDistribution distribution = new SalesShipmentDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = lineItem.ShipedQuantity * inventoryItem.AvgPrice,
                                Credit = 0,
                                DistributionType = SalesShipmentDistributionType.CostDistribution,
                                SalesShipmentId = salesShipment.SalesShipmentId,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
                        }
                    }
                }
                //if (productAccount != null)
                //{

                //}
            }
            foreach (SalesShipmentDistribution d in distributions)
            {
                db.SalesShipmentDistribution.Add(d);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #region credit total product stock
            foreach (var lineItem in lineItems)
            {
                //var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
                var product = db.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    var inventoryItem = db.InventoryItems.FirstOrDefault(i => i.ItemCode == product.ItemCode);
                    var crmCategory = GetProductCategory(product.CategoryID);
                    if (inventoryItem != null && crmCategory != null)
                    {
                        //string acc = inventoryItem.ItemCategory.AccountCode;
                        string acc = crmCategory.InventoryAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Credit += (lineItem.ShipedQuantity * inventoryItem.AvgPrice);
                                existingDstn.Debit = 0;
                                existingDstn.DistributionType = SalesShipmentDistributionType.CostDistribution;
                                existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
                            }
                        }
                        else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesShipmentDistribution distribution = new SalesShipmentDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Credit = lineItem.ShipedQuantity * inventoryItem.AvgPrice,
                                Debit = 0,
                                DistributionType = SalesShipmentDistributionType.CostDistribution,
                                SalesShipmentId = salesShipment.SalesShipmentId,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
                        }
                    }
                }
            }
            foreach (SalesShipmentDistribution d in distributions)
            {
                db.SalesShipmentDistribution.Add(d);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #endregion


            #region Revenue distribution
            #region debit total product cost
            foreach (var lineItem in lineItems)
            {
                ///var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
                //var payMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == lineItem.RecieveMethod).FirstOrDefault();
                var product = db.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    var crmCategory = GetProductCategory(product.CategoryID);
                    if (crmCategory != null)
                    {
                        string acc = crmCategory.UnEarnedAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Debit += (lineItem.ShipedQuantity * lineItem.Price);
                                existingDstn.Credit = 0;
                                existingDstn.DistributionType = SalesShipmentDistributionType.RevenuDistribution;
                                existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
                            }
                        }
                        else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesShipmentDistribution distribution = new SalesShipmentDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Debit = lineItem.ShipedQuantity * lineItem.Price,
                                Credit = 0,
                                DistributionType = SalesShipmentDistributionType.RevenuDistribution,
                                SalesShipmentId = salesShipment.SalesShipmentId,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
                        }
                    }
                }
            }
            foreach (SalesShipmentDistribution d in distributions)
            {
                db.SalesShipmentDistribution.Add(d);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #region credit total product issued amount (from invoice)
            foreach (var lineItem in lineItems)
            {
                //var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
                var product = db.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    var crmProductCategory = GetProductCategory(product.CategoryID);
                    if (crmProductCategory != null)
                    {
                        string acc = crmProductCategory.SalesAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Credit += (lineItem.ShipedQuantity * lineItem.Price);
                                existingDstn.Debit = 0;
                                existingDstn.DistributionType = SalesShipmentDistributionType.RevenuDistribution;
                                existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
                            }
                        }
                        else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
                        {
                            SalesShipmentDistribution distribution = new SalesShipmentDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
                                Credit = lineItem.ShipedQuantity * lineItem.Price,
                                Debit = 0,
                                DistributionType = SalesShipmentDistributionType.RevenuDistribution,
                                SalesShipmentId = salesShipment.SalesShipmentId,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
                        }
                    }
                }

            }
            foreach (SalesShipmentDistribution d in distributions)
            {
                db.SalesShipmentDistribution.Add(d);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #endregion

        }


        private ProductCategory GetProductCategory(string categoryId)
        {
            var _db = new ExceedDbContext();
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId);
        }

    }
}
