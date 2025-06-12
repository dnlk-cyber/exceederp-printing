using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Inventory;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.CRM
{
    public class CRMPaymentDistibutionHelper
    {
        public void UpdateCashSalesInvoiceDistribution(CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            Dictionary<string, string> checkAccount = new Dictionary<string, string>();
            List<SalesInvoicePaymentDistibution> distributions = new List<SalesInvoicePaymentDistibution>();

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var invoiceLineItems = db.LineItems
                .AsNoTracking()
                .Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString() 
                    && l.VoucherDefinitionID == "2")
                .ToList();
            var invoiceServiceLines = db.ServiceLineItems
                .AsNoTracking()
                .Where(l => l.VoucherId == invoice.CashSalesInvoiceID
                    && l.VoucherType == CRMVoucherType.SalesInvoice)
                .ToList();
            var additionalCosts = db.AdditionalCostLogs
                .AsNoTracking()
                .Where(l => l.VoucherId == invoice.CashSalesInvoiceID
                    && l.VoucherType == VoucherType.SalesInvoice)
                .ToList();
            //var customerAccount = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == invoice.OrganizationCustomerId.ToString()).FirstOrDefault();

            #region Remove Existing data
            var existingDistributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == invoice.CashSalesInvoiceID).ToList();
            if (existingDistributions.Any())
            {
                db.SalesInvoicePaymentDistibutions.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion
            var salesStore = db.SalesStores.Find(invoice.StoreID);
            string businessUnitCode = db.BusinessUnits.Find(salesStore?.InventoryStore.BusinessUnitId)?.Accountcode;

            #region debit net receivable amount
            decimal withholdingSum = invoiceLineItems.Sum(l => l.WithholdingAmount + l.WithholdTaxAmount)
                                         + invoiceServiceLines.Sum(s => s.WithholdingAmount + s.WithholdTaxAmount);

            decimal netOfPaidWithCash = invoice.GrandTotal - invoice.AdvancePayment;
            decimal netOfPaidWithAdvance = invoice.AdvancePayment;
            if (invoice.AdvancePayment > netOfPaidWithCash)
            {
                netOfPaidWithAdvance = netOfPaidWithAdvance - withholdingSum;
            }
            else
            {
                netOfPaidWithCash = netOfPaidWithCash - withholdingSum;
            }
            if (netOfPaidWithAdvance > 0)
            {
                var customerAccount = db.AccountMaps.FirstOrDefault(m => m.ReferenceType == "Organization_Customer"
                    && m.Reference == invoice.OrganizationCustomerId.ToString()
                    && m.ControlAccountID == AccountRequirementDefinition.SALES_ACCOUNT.ToString());
                if (customerAccount != null && !string.IsNullOrEmpty(customerAccount.AccountID))
                {
                    string acc = customerAccount.AccountID;

                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }
                    if (checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Debit += netOfPaidWithAdvance;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (!checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Debit = netOfPaidWithAdvance,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoice.CashSalesInvoiceID.ToString());
                    }
                }
            }
            var payMetMethod = db.PaymentMethods.Where(x => x.PaymentMethodID.ToString() == invoice.RecieveMethod).FirstOrDefault();
            if (payMetMethod != null && !string.IsNullOrEmpty(payMetMethod.ArAccountCode))
            {
                string acc = payMetMethod.ArAccountCode;

                int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                if (!string.IsNullOrEmpty(businessUnitCode))
                {
                    acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                }
                if (!isReplacingBranchDisabled)
                {
                    acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                }
                if (netOfPaidWithCash != 0 && checkAccount.ContainsKey(acc))
                {
                    var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                    if (existingDstn != null)
                    {
                        existingDstn.AccountCode = acc;
                        existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                        existingDstn.Debit += netOfPaidWithCash;
                        existingDstn.Credit = 0;
                        existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                    }
                }
                else if (netOfPaidWithCash != 0 && !checkAccount.ContainsKey(acc))
                {
                    SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                    {
                        AccountCode = acc,
                        AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                        Debit = netOfPaidWithCash,
                        Credit = 0,
                        CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                    };
                    distributions.Add(distribution);
                    checkAccount.Add(acc, invoice.CashSalesInvoiceID.ToString());
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
            if (payMetMethod != null && !string.IsNullOrEmpty(payMetMethod.ArAccountCode))
            {
                foreach (var invoiceItem in invoiceLineItems)
                {
                    string acc = payMetMethod.ArAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.Discount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Debit += invoiceItem.Discount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.Discount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Debit = invoiceItem.Discount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.Voucher);
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
            #region product line items
            foreach (var invoiceItem in invoiceLineItems)
            {
                if (invoiceItem.WithholdTax)
                {
                    var vatWithholdingSetting = db.GLVatWithholdings.FirstOrDefault();

                    if (vatWithholdingSetting != null && !string.IsNullOrEmpty(vatWithholdingSetting.WithholdingAccountCode))
                    {
                        string acc = vatWithholdingSetting.WithholdingAccountCode;

                        int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                        int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        if (!string.IsNullOrEmpty(businessUnitCode))
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                        }
                        if (!isReplacingBranchDisabled)
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                        }
                        if (invoiceItem.WithholdTaxAmount != 0 && checkAccount.ContainsKey(acc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = acc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                                existingDstn.Debit += invoiceItem.WithholdTaxAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (invoiceItem.WithholdTaxAmount != 0 && !checkAccount.ContainsKey(acc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = acc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                Debit = invoiceItem.WithholdTaxAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(acc, invoiceItem.Voucher);
                        }
                    }
                }
            }
            #endregion
            #region services
            foreach (var service in invoiceServiceLines)
            {
                if (service.WithholdTax)
                {
                    var tax = db.GLTaxRates.Where(x => x.GLTaxRateID == service.TaxTypeId).FirstOrDefault();

                    if (tax != null && !string.IsNullOrEmpty(tax.WithholdingAccountCode))
                    {
                        string acc = tax.WithholdingAccountCode;

                        int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                        int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        if (!string.IsNullOrEmpty(businessUnitCode))
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                        }
                        if (!isReplacingBranchDisabled)
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                        }
                        if (service.WithholdTaxAmount != 0 && checkAccount.ContainsKey(acc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = acc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                                existingDstn.Debit += service.WithholdTaxAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (service.WithholdTaxAmount != 0 && !checkAccount.ContainsKey(acc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = acc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                Debit = service.WithholdTaxAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(acc, service.VoucherId.ToString());
                        }
                    }
                }
            }
            #endregion
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion
            #region debit withholding tax
            #region product line items
            foreach (var invoiceItem in invoiceLineItems)
            {
                if (!string.IsNullOrEmpty(invoiceItem.WithholdingType))
                {
                    var WithHolding = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.WithholdingType).FirstOrDefault();

                    if (WithHolding != null && !string.IsNullOrEmpty(WithHolding.WithholdingAccountCode))
                    {
                        string acc = WithHolding.WithholdingAccountCode;

                        int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                        int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        if (!string.IsNullOrEmpty(businessUnitCode))
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                        }
                        if (!isReplacingBranchDisabled)
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                        }

                        if (invoiceItem.WithholdingAmount != 0 && checkAccount.ContainsKey(acc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = acc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                                existingDstn.Debit += invoiceItem.WithholdingAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (invoiceItem.WithholdingAmount != 0 && !checkAccount.ContainsKey(acc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = acc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                Debit = invoiceItem.WithholdingAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(acc, invoiceItem.Voucher);
                        }
                    }
                }
            }
            #endregion
            #region srvices
            foreach (var service in invoiceServiceLines)
            {
                if (service.WithholdingTypeId != null)
                {
                    var WithHolding = db.GLTaxRates.Where(x => x.GLTaxRateID == service.WithholdingTypeId).FirstOrDefault();

                    if (WithHolding != null && !string.IsNullOrEmpty(WithHolding.WithholdingAccountCode))
                    {
                        string acc = WithHolding.WithholdingAccountCode;

                        int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                        //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                        int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                        if (!string.IsNullOrEmpty(businessUnitCode))
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                        }
                        if (!isReplacingBranchDisabled)
                        {
                            acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                        }

                        if (service.WithholdingAmount != 0 && checkAccount.ContainsKey(acc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = acc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                                existingDstn.Debit += service.WithholdingAmount;
                                existingDstn.Credit = 0;
                                existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                            }
                        }
                        else if (service.WithholdingAmount != 0 && !checkAccount.ContainsKey(acc))
                        {
                            SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                            {
                                AccountCode = acc,
                                AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                                Debit = service.WithholdingAmount,
                                Credit = 0,
                                CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                            };
                            distributions.Add(distribution);
                            checkAccount.Add(acc, service.VoucherId.ToString());
                        }
                    }
                }


            }
            #endregion
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();
            #endregion

            #region credit sub-total (revenue)
            #region product line items
            foreach (var invoiceItem in invoiceLineItems)
            {
                // crm category, not inventory
                var category = GetProductCategory(invoiceItem.Product.CategoryID);

                //var productAccount = db.AccountMaps.Where(x => x.Reference == invoiceItem.Product.ProductID.ToString()).FirstOrDefault();
                if (category != null && !string.IsNullOrEmpty(category.SalesAccountCode))
                {
                    string acc = category.SalesAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.CalculatedCost != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += invoiceItem.CalculatedCost;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.CalculatedCost != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = invoiceItem.CalculatedCost,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.Voucher);
                    }
                }
            }
            #endregion
            #region services
            foreach (var service in invoiceServiceLines)
            {
                // crm category, not inventory
                var category = GetProductCategoryForServicice(service);

                if (category != null && !string.IsNullOrEmpty(category.UnEarnedAccountCode))
                {
                    string acc = category.UnEarnedAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (service.SubTotal != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += service.SubTotal;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (service.SubTotal != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = service.SubTotal,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, service.VoucherId.ToString());
                    }
                }
            }
            #endregion
            #region additional costs
            foreach (var additionalCost in additionalCosts)
            {
                var costType = additionalCost.AdditionalCost;

                if (costType != null && !string.IsNullOrEmpty(costType.SalesAccountCode))
                {
                    string acc = costType.SalesAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (additionalCost.CostAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += additionalCost.CostAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (additionalCost.CostAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = additionalCost.CostAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, additionalCost.VoucherId.ToString());
                    }
                }
            }
            #endregion
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
            #region credit TAX
            #region product line items
            foreach (var invoiceItem in invoiceLineItems)
            {
                var taxRate = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == invoiceItem.Tax).FirstOrDefault();
                if (taxRate != null && !string.IsNullOrEmpty(taxRate.ArAccountCode) && invoiceItem.TaxAmount != 0)
                {
                    string acc = taxRate.ArAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(salesStore?.InventoryStore?.BranchId ?? -1);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.TaxAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += invoiceItem.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TaxAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = invoiceItem.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.Voucher);
                    }
                }
            }
            #endregion
            #region services
            foreach (var service in invoiceServiceLines)
            {
                var taxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == service.TaxTypeId);
                if (taxRate != null && !string.IsNullOrEmpty(taxRate.ArAccountCode) && service.TaxAmount != 0)
                {
                    string acc = taxRate.ArAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }
                    if (service.TaxAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += service.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (service.TaxAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = service.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, service.VoucherId.ToString());
                    }
                }
            }
            #endregion
            #region additional costs
            foreach (var additionalCost in additionalCosts)
            {
                var taxRate = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == additionalCost.GLTaxRateID);
                if (taxRate != null && !string.IsNullOrEmpty(taxRate.ArAccountCode) && additionalCost.TaxAmount != 0)
                {
                    string acc = taxRate.ArAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }
                    if (additionalCost.TaxAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += additionalCost.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (additionalCost.TaxAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = additionalCost.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, additionalCost.VoucherId.ToString());
                    }
                }
            }
            #endregion
            foreach (SalesInvoicePaymentDistibution invoiceDistribution in distributions)
            {
                db.SalesInvoicePaymentDistibutions.Add(invoiceDistribution);
                db.SaveChanges();
            }
            distributions.Clear();
            checkAccount.Clear();

            #endregion
        }

        public void DistributeCreditInvoice(CashSalesInvoice invoice)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<SalesInvoicePaymentDistibution> distributions = new List<SalesInvoicePaymentDistibution>();

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

            var salesStore = db.SalesStores.Find(invoice.StoreID);
            string businessUnitCode = db.BusinessUnits.Find(salesStore?.InventoryStore.BusinessUnitId)?.Accountcode;

            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            var invoiceLineItems = db.LineItems
                .AsNoTracking()
                .Where(l => l.Voucher == invoice.CashSalesInvoiceID.ToString()
                    && l.VoucherDefinitionID == "2")
                .ToList();
            var customerAccount =
                db.AccountMaps.FirstOrDefault(m => m.ReferenceType == "Organization_Customer" &&
                                            m.Reference == invoice.OrganizationCustomerId.ToString() &&
                                            m.ControlAccountID == AccountRequirementDefinition.SALES_ACCOUNT.ToString());

            #region Remove Existing data
            var existingDistributions = db.SalesInvoicePaymentDistibutions.Where(d => d.CashSalesInvoiceId == invoice.CashSalesInvoiceID).ToList();
            if (existingDistributions.Any())
            {
                db.SalesInvoicePaymentDistibutions.RemoveRange(existingDistributions);
                db.SaveChanges();
            }
            #endregion

            if (customerAccount != null && !string.IsNullOrEmpty(customerAccount.AccountID))
            {
                #region debit total amount
                foreach (var invoiceItem in invoiceLineItems)
                {
                    //AccountHelper accountHelper = new AccountHelper();
                    string acc = customerAccount.AccountID;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.TotalAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Debit += invoiceItem.TotalAmount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TotalAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Debit = invoiceItem.TotalAmount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.LineItemID);
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
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.Discount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Debit += invoiceItem.Discount;
                            existingDstn.Credit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.Discount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Debit = invoiceItem.Discount,
                            Credit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.LineItemID);
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
                if (category != null && !string.IsNullOrEmpty(category.UnEarnedAccountCode))
                {
                    string acc = category.UnEarnedAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.CalculatedCost != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += invoiceItem.CalculatedCost;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.CalculatedCost != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = invoiceItem.CalculatedCost,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.LineItemID);
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
                if (taxRate != null && !string.IsNullOrEmpty(taxRate.ApAccountCode))
                {
                    string acc = taxRate.ApAccountCode;
                    int branchAccountIndex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string branchSeg = HrHelper.GetBranchLocationAccount(invoice.BranchId);
                    //int subaccountindex = _dynamicAccount.GetSubAccountSequenceIndex();
                    int bUtIndex = _dynamicAccount.GetBizUnitAccountSequenceIndex();
                    if (!string.IsNullOrEmpty(businessUnitCode))
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, bUtIndex, businessUnitCode);
                    }
                    if (!isReplacingBranchDisabled)
                    {
                        acc = _dynamicAccount.ReplaceSegment(acc, branchAccountIndex, branchSeg);
                    }

                    if (invoiceItem.TaxAmount != 0 && checkAccount.ContainsKey(acc))
                    {
                        var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == acc);
                        if (existingDstn != null)
                        {
                            existingDstn.AccountCode = acc;
                            existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(acc);
                            existingDstn.Credit += invoiceItem.TaxAmount;
                            existingDstn.Debit = 0;
                            existingDstn.CashSalesInvoiceId = invoice.CashSalesInvoiceID;
                        }
                    }
                    else if (invoiceItem.TaxAmount != 0 && !checkAccount.ContainsKey(acc))
                    {
                        SalesInvoicePaymentDistibution distribution = new SalesInvoicePaymentDistibution
                        {
                            AccountCode = acc,
                            AccountDesc = _dynamicAccount.GetAccountDesc(acc),
                            Credit = invoiceItem.TaxAmount,
                            Debit = 0,
                            CashSalesInvoiceId = invoice.CashSalesInvoiceID,
                        };
                        distributions.Add(distribution);
                        checkAccount.Add(acc, invoiceItem.LineItemID);
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

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

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
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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
        public void UpdateCreditSettlementDistribution(CreditSettlement creditSettlement)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<CreditSettlementDistribution> distributions = new List<CreditSettlementDistribution>();

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

            DynamicAccountHelper _dynamicAccount = new DynamicAccountHelper();
            //var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();
            var creditLineItems = db.LineCreditSettlements.Where(l => l.CreditSettlementId == creditSettlement.CreditSettlementId).ToList();
            var customerAccount = db.AccountMaps
                .FirstOrDefault(m => m.ReferenceType == "Organization_Customer" 
                    && m.Reference == creditSettlement.Customer.ToString()
                    && m.ControlAccountID == AccountRequirementDefinition.SALES_ACCOUNT.ToString());

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

            foreach (var lineItem in creditLineItems)
            {
                string acc = lineItem.Source == SettlementAmountSource.Cash 
                    ? payMentMethod?.ArAccountCode
                    : customerAccount?.AccountID;
                if (!string.IsNullOrEmpty(acc))
                {
                    int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                    string subAccountSeG = HrHelper.GetBranchLocationAccount(creditSettlement.BranchId);
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);


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
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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

        public void UpdateCRMDepositDistribution(CRMDeposit cRMDeposit)
        {
            var db = new ExceedDbContext();
            Dictionary<string, int> checkAccount = new Dictionary<string, int>();
            List<CRMDepositDistribution> distributions = new List<CRMDepositDistribution>();

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

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
                    string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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
                        string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

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

            bool isReplacingBranchDisabled = CRMConfigurationHelper.IsReplacingBranchSegmentInDistributionDisabled();

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
                    var salesStore = db.SalesStores.Find(lineItem.SalesStoreId);
                    var inventoryItem = db.InventoryItems.FirstOrDefault(i => i.ItemCode == product.ItemCode);
                    if (salesStore != null && inventoryItem != null)
                    {
                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, salesStore.StoreCode);

                        //var issueAccountCode = inventoryItem.ItemCategory.IssueAccountCode;
                        string acc = inventoryItem.ItemCategory.IssueAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                //existingDstn.Debit += (lineItem.ShipedQuantity * lineItem.Price);
                                existingDstn.Debit += (lineItem.ShipedQuantity * uPrice);
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
                                Debit = lineItem.ShipedQuantity * uPrice,
                                Credit = 0,
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
            #region credit total product stock
            foreach (var lineItem in lineItems)
            {
                //var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
                var product = db.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    var inventoryItem = db.InventoryItems.FirstOrDefault(i => i.ItemCode == product.ItemCode);
                    var crmCategory = GetProductCategory(product.CategoryID);
                    var salesStore = db.SalesStores.Find(lineItem.SalesStoreId);
                    if (inventoryItem != null && crmCategory != null && salesStore != null)
                    {
                        var uPrice = InventoryHelper.GetItemAveragePrice(product.ItemCode, salesStore.StoreCode);

                        //string acc = inventoryItem.ItemCategory.AccountCode;
                        string acc = crmCategory.InventoryAccountCode;
                        int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
                        string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
                        string newacc = isReplacingBranchDisabled ? acc : _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

                        if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
                        {
                            var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
                            if (existingDstn != null)
                            {
                                existingDstn.AccountCode = newacc;
                                existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
                                existingDstn.Credit += (lineItem.ShipedQuantity * uPrice);
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
                                Credit = lineItem.ShipedQuantity * uPrice,
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
            //#region debit total product cost
            //foreach (var lineItem in lineItems)
            //{
            //    ///var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
            //    //var payMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == lineItem.RecieveMethod).FirstOrDefault();
            //    var product = db.Products.Find(lineItem.ProductId);
            //    if (product != null)
            //    {
            //        var crmCategory = GetProductCategory(product.CategoryID);
            //        if (crmCategory != null)
            //        {
            //            string acc = crmCategory.UnEarnedAccountCode;
            //            int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
            //            string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
            //            string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

            //            if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
            //            {
            //                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
            //                if (existingDstn != null)
            //                {
            //                    existingDstn.AccountCode = newacc;
            //                    existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
            //                    existingDstn.Debit += (lineItem.ShipedQuantity * lineItem.Price);
            //                    existingDstn.Credit = 0;
            //                    existingDstn.DistributionType = SalesShipmentDistributionType.RevenuDistribution;
            //                    existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
            //                }
            //            }
            //            else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
            //            {
            //                SalesShipmentDistribution distribution = new SalesShipmentDistribution
            //                {
            //                    AccountCode = newacc,
            //                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
            //                    Debit = lineItem.ShipedQuantity * lineItem.Price,
            //                    Credit = 0,
            //                    DistributionType = SalesShipmentDistributionType.RevenuDistribution,
            //                    SalesShipmentId = salesShipment.SalesShipmentId,
            //                };
            //                distributions.Add(distribution);
            //                checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
            //            }
            //        }
            //    }
            //}
            //foreach (SalesShipmentDistribution d in distributions)
            //{
            //    db.SalesShipmentDistribution.Add(d);
            //    db.SaveChanges();
            //}
            //distributions.Clear();
            //checkAccount.Clear();
            //#endregion
            //#region credit total product issued amount (from invoice)
            //foreach (var lineItem in lineItems)
            //{
            //    //var productAccount = db.AccountMaps.Where(x => x.Reference == lineItem.ProductId.ToString()).FirstOrDefault();
            //    var product = db.Products.Find(lineItem.ProductId);
            //    if (product != null)
            //    {
            //        var crmProductCategory = GetProductCategory(product.CategoryID);
            //        if (crmProductCategory != null)
            //        {
            //            string acc = crmProductCategory.SalesAccountCode;
            //            int subaccountindex = _dynamicAccount.GetBranchAccountSequenceIndex();
            //            string subAccountSeG = HrHelper.GetBranchLocationAccount(salesShipment.BranchId);
            //            string newacc = _dynamicAccount.ReplaceSegment(acc, subaccountindex, subAccountSeG);

            //            if (lineItem.ShipedQuantity != 0 && checkAccount.ContainsKey(newacc))
            //            {
            //                var existingDstn = distributions.FirstOrDefault(d => d.AccountCode == newacc);
            //                if (existingDstn != null)
            //                {
            //                    existingDstn.AccountCode = newacc;
            //                    existingDstn.AccountDesc = _dynamicAccount.GetAccountDesc(newacc);
            //                    existingDstn.Credit += (lineItem.ShipedQuantity * lineItem.Price);
            //                    existingDstn.Debit = 0;
            //                    existingDstn.DistributionType = SalesShipmentDistributionType.RevenuDistribution;
            //                    existingDstn.SalesShipmentId = salesShipment.SalesShipmentId;
            //                }
            //            }
            //            else if (lineItem.ShipedQuantity != 0 && !checkAccount.ContainsKey(newacc))
            //            {
            //                SalesShipmentDistribution distribution = new SalesShipmentDistribution
            //                {
            //                    AccountCode = newacc,
            //                    AccountDesc = _dynamicAccount.GetAccountDesc(newacc),
            //                    Credit = lineItem.ShipedQuantity * lineItem.Price,
            //                    Debit = 0,
            //                    DistributionType = SalesShipmentDistributionType.RevenuDistribution,
            //                    SalesShipmentId = salesShipment.SalesShipmentId,
            //                };
            //                distributions.Add(distribution);
            //                checkAccount.Add(newacc, lineItem.SalesShipmentLineItemId);
            //            }
            //        }
            //    }

            //}
            //foreach (SalesShipmentDistribution d in distributions)
            //{
            //    db.SalesShipmentDistribution.Add(d);
            //    db.SaveChanges();
            //}
            //distributions.Clear();
            //checkAccount.Clear();
            //#endregion
            #endregion

        }

        private ProductCategory GetProductCategory(string categoryId)
        {
            var _db = new ExceedDbContext();
            return _db.ProductCategories.FirstOrDefault(c => c.ProductCategoryId.ToString() == categoryId);
        }
        private ProductCategory GetProductCategoryForServicice(ServiceLineItem service)
        {
            var _db = new ExceedDbContext();
            var repairType = _db.RepairType.FirstOrDefault(r => r.RepairTypeID.ToString() == service.ServiceTypeId);
            if (repairType != null)
            {
                return _db.ProductCategories.FirstOrDefault(c => c.InventoryCategoryCode == repairType.CategoryCode);
            }
            return null;
        }
    }
}
