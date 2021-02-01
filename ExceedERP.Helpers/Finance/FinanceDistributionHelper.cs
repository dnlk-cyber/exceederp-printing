using System;
using System.Collections.Generic;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Helpers.Finance
{
    public class FinanceDistributionHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper _accountHelper;
        //AR--sales order
        public  void UpdateArInvoiceDistribution(int id)
        {
            List<ArInvoiceDistribution> distributions = new List<ArInvoiceDistribution>();
            _accountHelper =new AccountHelper();
            var aRInvoice = _db.ARInvoices.Find(id);
            if (aRInvoice != null)
            {
                var lines = _db.ARInvoiceLines.Where(x => x.ArInvoiceId ==id).AsNoTracking().ToList();
                if (lines.Any())
                {

                    #region clear existing distribution
                    var record = _db.ARInvoiceDistributions.Where(x => x.ArInvoiceId == id).ToList();
                    _db.ARInvoiceDistributions.RemoveRange(record);
                    _db.SaveChanges();
                    #endregion

                    if ((aRInvoice.TransactionClass == TransactionClass.Invoice) || (aRInvoice.TransactionClass == TransactionClass.DebitMemo))
                    {
                        var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRInvoice.CustomerId);
                        if (customer != null)
                        {
                            #region debit receivable

                            decimal totalAmount = lines.Sum(x => x.TotalAmount);
                            if (totalAmount > 0)
                            {
                                string accountCode = customer.AccountCode;
                                var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                                if (invoiceDistribution != null)
                                {
                                    invoiceDistribution.AccountCode = accountCode;
                                    invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                                    invoiceDistribution.Debit += totalAmount;
                                    invoiceDistribution.Credit = 0;
                                    invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                }
                                else
                                {
                                    ArInvoiceDistribution defaultDistribution = new ArInvoiceDistribution
                                    {
                                        AccountCode = accountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                        Debit = totalAmount,
                                        Credit = 0,
                                        ArInvoiceId = aRInvoice.ArInvoiceId
                                    };

                                    distributions.Add(defaultDistribution);
                                }

                               
                            }

                            
                            #endregion
                        }
                        #region get discount account

                        string discountAccount = "";
                        var setting = _db.FinanceSettings.FirstOrDefault();
                        if (setting != null) discountAccount = setting.DiscountArAccount;

                        #endregion
                        #region debit discount
                        decimal totalDiscount = lines.Sum(x => x.Discount);

                        if (totalDiscount > 0)
                        {
                            var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == discountAccount);
                            if (invoiceDistribution != null)
                            {
                                invoiceDistribution.AccountCode = discountAccount;
                                invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(discountAccount);
                                invoiceDistribution.Debit += totalDiscount;
                                invoiceDistribution.Credit = 0;
                                invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                            }
                            else
                            {
                                var defaultDistribution = new ArInvoiceDistribution
                                {
                                    AccountCode = discountAccount,
                                    AccountDesc = _accountHelper.GetAccountDesc(discountAccount),
                                    Debit = totalDiscount,
                                    Credit = 0,
                                    ArInvoiceId = aRInvoice.ArInvoiceId
                                };

                                distributions.Add(defaultDistribution);

                            }

                        }

                        #endregion
                        foreach (var line in lines)
                        {
                            decimal revenueTotal = line.SubTotal + line.Discount;
                            #region credit revenue

                            if (revenueTotal > 0)
                            {
                                var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == line.AccountCode);
                                if (invoiceDistribution != null)
                                {
                                    invoiceDistribution.AccountCode = line.AccountCode;
                                    invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode);
                                    invoiceDistribution.Debit = 0;
                                    invoiceDistribution.Credit += revenueTotal;
                                    invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                }
                                else
                                {

                                    var defaultDistribution =new ArInvoiceDistribution
                                        {
                                            AccountCode = line.AccountCode,
                                            AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode),
                                            Debit = 0,
                                            Credit = revenueTotal,
                                            ArInvoiceId = aRInvoice.ArInvoiceId
                                        };
                                    distributions.Add(defaultDistribution);
                                }
                            }

                            #endregion
                            #region credit tax

                            if (line.TaxAmount > 0)
                            {
                               
                                var taxRule = _db.GLTaxRates.Find(line.GLTaxRateID);
                                if (taxRule != null)
                                {
                                    string taxAccount = taxRule.ArAccountCode;
                                    var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == taxAccount);
                                    if (invoiceDistribution != null)
                                    {
                                        invoiceDistribution.AccountCode = taxAccount;
                                        invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(taxAccount);
                                        invoiceDistribution.Debit = 0;
                                        invoiceDistribution.Credit += line.TaxAmount;
                                        invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                    }
                                    else
                                    {
                                        var defaultDistribution = new ArInvoiceDistribution
                                        {
                                            AccountCode = taxAccount,
                                            AccountDesc = _accountHelper.GetAccountDesc(taxAccount),
                                            Debit = 0,
                                            Credit = line.TaxAmount,
                                            ArInvoiceId = aRInvoice.ArInvoiceId
                                        };
                                        distributions.Add(defaultDistribution);

                                    }
                                  
                                }
                            }

                            #endregion
                        }
                        

                    }
                   
                    else if (aRInvoice.TransactionClass == TransactionClass.CreditMemo)
                    {
                        var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == aRInvoice.CustomerId);
                        if (customer != null)
                        {
                            #region debit receivable

                            decimal totalAmount = lines.Sum(x => x.TotalAmount);
                            if (totalAmount > 0)
                            {
                                string accountCode = customer.AccountCode;
                                var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == accountCode);
                                if (invoiceDistribution != null)
                                {
                                    invoiceDistribution.AccountCode = accountCode;
                                    invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(accountCode);
                                    invoiceDistribution.Debit += totalAmount;
                                    invoiceDistribution.Credit = 0;
                                    invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                }
                                else
                                {
                                    ArInvoiceDistribution defaultDistribution = new ArInvoiceDistribution
                                    {
                                        AccountCode = accountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(accountCode),
                                        Debit = totalAmount,
                                        Credit = 0,
                                        ArInvoiceId = aRInvoice.ArInvoiceId
                                    };

                                    distributions.Add(defaultDistribution);
                                }


                            }


                            #endregion
                        }
                        #region get discount account

                        string discountAccount = "";
                        var setting = _db.FinanceSettings.FirstOrDefault();
                        if (setting != null) discountAccount = setting.DiscountArAccount;

                        #endregion
                        #region debit discount
                        decimal totalDiscount = lines.Sum(x => x.Discount);

                        if (totalDiscount > 0)
                        {
                            var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == discountAccount);
                            if (invoiceDistribution != null)
                            {
                                invoiceDistribution.AccountCode = discountAccount;
                                invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(discountAccount);
                                invoiceDistribution.Debit = 0;
                                invoiceDistribution.Credit += totalDiscount;
                                invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                            }
                            else
                            {
                                var defaultDistribution = new ArInvoiceDistribution
                                {
                                    AccountCode = discountAccount,
                                    AccountDesc = _accountHelper.GetAccountDesc(discountAccount),
                                    Debit = 0,
                                    Credit = totalDiscount,
                                    ArInvoiceId = aRInvoice.ArInvoiceId
                                };

                                distributions.Add(defaultDistribution);

                            }

                        }

                        #endregion
                        foreach (var line in lines)
                        {
                            decimal revenueTotal = line.SubTotal + line.Discount;
                            #region credit revenue

                            if (revenueTotal > 0)
                            {
                                var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == line.AccountCode);
                                if (invoiceDistribution != null)
                                {
                                    invoiceDistribution.AccountCode = line.AccountCode;
                                    invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode);
                                    invoiceDistribution.Debit += revenueTotal;
                                    invoiceDistribution.Credit = 0; 
                                    invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                }
                                else
                                {

                                    var defaultDistribution = new ArInvoiceDistribution
                                    {
                                        AccountCode = line.AccountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode),
                                        Debit = revenueTotal,
                                        Credit =0 ,
                                        ArInvoiceId = aRInvoice.ArInvoiceId
                                    };
                                    distributions.Add(defaultDistribution);
                                }
                            }

                            #endregion
                            #region credit tax

                            if (line.TaxAmount > 0)
                            {

                                var taxRule = _db.GLTaxRates.Find(line.GLTaxRateID);
                                if (taxRule != null)
                                {
                                    string taxAccount = taxRule.ArAccountCode;
                                    var invoiceDistribution = distributions.FirstOrDefault(x => x.AccountCode == taxAccount);
                                    if (invoiceDistribution != null)
                                    {
                                        invoiceDistribution.AccountCode = taxAccount;
                                        invoiceDistribution.AccountDesc = _accountHelper.GetAccountDesc(taxAccount);
                                        invoiceDistribution.Debit += line.TaxAmount;
                                        invoiceDistribution.Credit = 0;
                                        invoiceDistribution.ArInvoiceId = aRInvoice.ArInvoiceId;
                                    }
                                    else
                                    {
                                        var defaultDistribution = new ArInvoiceDistribution
                                        {
                                            AccountCode = taxAccount,
                                            AccountDesc = _accountHelper.GetAccountDesc(taxAccount),
                                            Debit = line.TaxAmount,
                                            Credit = 0,
                                            ArInvoiceId = aRInvoice.ArInvoiceId
                                        };
                                        distributions.Add(defaultDistribution);

                                    }

                                }
                            }

                            #endregion
                        }
                    }

                    //persist
                    foreach (var distribution in distributions)
                    {
                        _db.ARInvoiceDistributions.Add(distribution);
                        _db.SaveChanges();
                    }

                }
            }

        }

        public  void UpdateArMiscCollectionDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.ARMiscelaneousCollections.Find(id);
            if (record != null)
            {
                ARMiscelaneousCollectionDistribution defaultDistribution;

                #region clear existing distribution

                var records =
                    _db.ARMiscelaneousCollectionDistributions.Where(
                        x => x.ARMiscelaneousCollectionID == record.ARMiscelaneousCollectionID).ToList();
                _db.ARMiscelaneousCollectionDistributions.RemoveRange(records);
                _db.SaveChanges();

                #endregion

                var collectionlines = _db.ArMiscelaneousCollectionLines.Where(x => x.ARMiscelaneousCollectionID == id).AsNoTracking().ToList();
                decimal totalnetreceipt = collectionlines.Sum(x => x.NetReceipt);

                #region debit bank/cash

                if (totalnetreceipt > 0)
                {
                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.ReceiptMethod);
                    defaultDistribution = new ARMiscelaneousCollectionDistribution();
                    if (paymentMethod != null)
                    {
                        defaultDistribution.AccountCode = paymentMethod.ArAccountCode;
                        defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ArAccountCode);
                    }
                    defaultDistribution.Debit = totalnetreceipt;
                    defaultDistribution.Credit = 0;
                    defaultDistribution.ARMiscelaneousCollectionID = record.ARMiscelaneousCollectionID;
                    _db.ARMiscelaneousCollectionDistributions.Add(defaultDistribution);
                    _db.SaveChanges();
                }

                #endregion

                foreach (var line in collectionlines)
                {
                    #region debit vat withholding

                    if (line.WithholdTaxAmount > 0)
                    {

                        var withhodingtaxaccount = _db.GLVatWithholdings.FirstOrDefault();
                        defaultDistribution = new ARMiscelaneousCollectionDistribution();
                        if (withhodingtaxaccount != null)
                        {
                            defaultDistribution.AccountCode = withhodingtaxaccount.WithholdingAccountCode;
                            defaultDistribution.AccountDesc =
                                _accountHelper.GetAccountDesc(withhodingtaxaccount.WithholdingAccountCode);
                        }
                        defaultDistribution.Debit = line.WithholdTaxAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ARMiscelaneousCollectionID = record.ARMiscelaneousCollectionID;
                        _db.ARMiscelaneousCollectionDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion

                    #region debit 2% withholding

                    if (line.WithholdingAmount > 0)
                    {
                        var withhodingaccount = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == line.WithholdingType);
                        defaultDistribution = new ARMiscelaneousCollectionDistribution();
                        if (withhodingaccount != null)
                        {
                            defaultDistribution.AccountCode = withhodingaccount.WithholdingAccountCode;
                            defaultDistribution.AccountDesc =
                                _accountHelper.GetAccountDesc(withhodingaccount.WithholdingAccountCode);
                        }
                        defaultDistribution.Debit = line.WithholdingAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ARMiscelaneousCollectionID = record.ARMiscelaneousCollectionID;
                        _db.ARMiscelaneousCollectionDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion

                    #region credit receivable

                    if (line.Amount > 0)
                    {
                        defaultDistribution =
                            new ARMiscelaneousCollectionDistribution
                            {
                                AccountCode = line.RevenueAccount,
                                AccountDesc = _accountHelper.GetAccountDesc(line.RevenueAccount),
                                Debit = 0,
                                Credit = line.Amount,
                                ARMiscelaneousCollectionID = record.ARMiscelaneousCollectionID
                            };
                        _db.ARMiscelaneousCollectionDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion

                    #region credit tax

                    if (line.TaxAmount > 0)
                    {
                        defaultDistribution = new ARMiscelaneousCollectionDistribution();
                        //get  account
                        var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == line.TaxType);
                        if (tax != null)
                        {
                            defaultDistribution.AccountCode = tax.ArAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(tax.ArAccountCode);
                        }
                        defaultDistribution.Debit = 0;
                        defaultDistribution.Credit = line.TaxAmount;
                        defaultDistribution.ARMiscelaneousCollectionID = record.ARMiscelaneousCollectionID;
                        _db.ARMiscelaneousCollectionDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion
                }
            }
        }

        public  void UpdateArStandardCollectionDistribution(int id)
        {
            List<ArStandardCollectionDistribution> distributions = new List<ArStandardCollectionDistribution>();
            _accountHelper = new AccountHelper();
            var record = _db.ARStandardCollections.Find(id);
            var lines = _db.ARStandardCollectionLines.Where(x => x.ArStandardCollectionId == id).AsNoTracking()
                .ToList();

            if (record != null)
            {
                #region clear existing distribution

                var records =
                    _db.ARStandardCollectionDistributions.Where(
                        x => x.ArStandardCollectionId == record.ArStandardCollectionId).ToList();
                _db.ARStandardCollectionDistributions.RemoveRange(records);
                _db.SaveChanges();

                #endregion

                #region get prepayment account
                var prepayment = _db.ARSetupReceiptBalanceAccounts.FirstOrDefault();
                #endregion

                #region debit net collection bank/cash
                decimal netamount = lines.Sum(x => x.NetAmount);
                if (netamount > 0)
                {
                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.ReceiptMethod);
                    
                    if (paymentMethod != null)
                    {
                        var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == paymentMethod.ArAccountCode);
                        if (collectionDistribution != null)
                        {
                            collectionDistribution.AccountCode = paymentMethod.ArAccountCode;
                            collectionDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ArAccountCode);
                            collectionDistribution.Debit += netamount;
                            collectionDistribution.Credit = 0;
                            collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                        }
                        else
                        {
                            var defaultDistribution =new ArStandardCollectionDistribution
                                {
                                    AccountCode = paymentMethod.ArAccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ArAccountCode),
                                    Debit = netamount,
                                    Credit = 0,
                                    ArStandardCollectionId = record.ArStandardCollectionId
                                };
                            distributions.Add(defaultDistribution);
                        }
                    }

                }

                #endregion

                #region debit prepayment

                decimal prepaymentAmount = record.ARStandardCollectionLines.Sum(x => x.PrepaymentAmount);
                if (prepaymentAmount > 0 && prepayment!=null)
                {
                    var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == prepayment.PrepaymentAccountCode);
                    if (collectionDistribution != null)
                    {
                        collectionDistribution.AccountCode = prepayment.PrepaymentAccountCode;
                        collectionDistribution.AccountDesc = _accountHelper.GetAccountDesc(prepayment.PrepaymentAccountCode);
                        collectionDistribution.Debit += prepaymentAmount;
                        collectionDistribution.Credit = 0;
                        collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                    }
                    else
                    {
                        var defaultDistribution =new ArStandardCollectionDistribution
                            {
                                AccountCode = prepayment.PrepaymentAccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(prepayment.PrepaymentAccountCode),
                                Debit = prepaymentAmount,
                                Credit = 0,
                                ArStandardCollectionId = record.ArStandardCollectionId
                            };
                        distributions.Add(defaultDistribution);
                    }
                }

                #endregion

                #region get customer account
                var customer = _db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == record.CustomerId);


                #endregion
               

                foreach (var line in lines)
                {

                    #region debit vat withholding
                    //var taxRule = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == line.TaxType);
                    var withhodingtaxaccount = _db.GLVatWithholdings.FirstOrDefault();
                    if (line.WithholdTaxAmount > 0 && withhodingtaxaccount != null)
                    {
                        var collectionDistribution =
                            distributions.FirstOrDefault(x => x.AccountCode == withhodingtaxaccount.WithholdingAccountCode);
                        if (collectionDistribution != null)
                        {
                            collectionDistribution.AccountCode = withhodingtaxaccount.WithholdingAccountCode;
                            collectionDistribution.AccountDesc =
                                _accountHelper.GetAccountDesc(withhodingtaxaccount.WithholdingAccountCode);
                            collectionDistribution.Debit += line.WithholdTaxAmount;
                            collectionDistribution.Credit = 0;
                            collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                        }
                        else
                        {
                            var defaultDistribution = new ArStandardCollectionDistribution
                            {
                                AccountCode = withhodingtaxaccount.WithholdingAccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(withhodingtaxaccount.WithholdingAccountCode),
                                Debit = line.WithholdTaxAmount,
                                Credit = 0,
                                ArStandardCollectionId = record.ArStandardCollectionId
                            };
                            distributions.Add(defaultDistribution);

                        }
                    }

                    #endregion
                    #region debit 2% withholding
                    var withhodingaccount = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == line.WithholdingType);

                    if (line.WithholdingAmount > 0 && withhodingaccount != null)
                    {
                       
                        var collectionDistribution =
                            distributions.FirstOrDefault(x => x.AccountCode == withhodingaccount.WithholdingAccountCode);
                        if (collectionDistribution != null)
                        {
                            collectionDistribution.AccountCode = withhodingaccount.WithholdingAccountCode;
                            collectionDistribution.AccountDesc =
                                _accountHelper.GetAccountDesc(withhodingaccount.WithholdingAccountCode);
                            collectionDistribution.Debit += line.WithholdingAmount;
                            collectionDistribution.Credit = 0;
                            collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                        }
                        else
                        {
                            var defaultDistribution = new ArStandardCollectionDistribution
                            {
                                AccountCode = withhodingaccount.WithholdingAccountCode,
                                AccountDesc =
                                    _accountHelper.GetAccountDesc(withhodingaccount.WithholdingAccountCode),
                                Debit = line.WithholdingAmount,
                                Credit = 0,
                                ArStandardCollectionId = record.ArStandardCollectionId
                            };

                            distributions.Add(defaultDistribution);
                        }
                        

                    }

                    #endregion

                    if (line.IsPrepayment && prepayment != null)
                    {
                        #region credit prepayment //replace sub account

                        decimal applyAmount = line.ApplyAmount;
                        if (applyAmount > 0)
                        {
                            var collectionDistribution =distributions.FirstOrDefault(x => x.AccountCode == prepayment.PrepaymentAccountCode);
                            if (collectionDistribution != null)
                            {
                                collectionDistribution.AccountCode = prepayment.PrepaymentAccountCode;
                                collectionDistribution.AccountDesc =
                                    _accountHelper.GetAccountDesc(prepayment.PrepaymentAccountCode);
                                collectionDistribution.Debit = 0;
                                collectionDistribution.Credit += applyAmount;
                                collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                            }
                            else
                            {
                                var defaultDistribution =new ArStandardCollectionDistribution
                                    {
                                        AccountCode = prepayment.PrepaymentAccountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(prepayment.PrepaymentAccountCode),
                                        Debit = 0,
                                        Credit = applyAmount,
                                        ArStandardCollectionId = record.ArStandardCollectionId
                                    };
                                distributions.Add(defaultDistribution);
                            }
                        }

                        #endregion
                    }
                    else
                    {
                       

                        if (line.Source == InvoiceSource.Invoice)
                        {
                            decimal receivable = line.NetAmount+line.WithholdingAmount+line.WithholdTaxAmount;//sub total
                            #region credit receivable -- customer

                            if (receivable > 0 && customer != null)
                            {
                                var collectionDistribution = distributions.FirstOrDefault(x =>x.AccountCode == customer.AccountCode);
                               
                                if (collectionDistribution != null)
                                {
                                    collectionDistribution.AccountCode = customer.AccountCode;
                                    collectionDistribution.AccountDesc =
                                        _accountHelper.GetAccountDesc(customer.AccountCode);
                                    collectionDistribution.Debit = 0;
                                    collectionDistribution.Credit += receivable;
                                    collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                                }
                                else
                                {
                                    var defaultDistribution =new ArStandardCollectionDistribution
                                        {
                                            AccountCode = customer.AccountCode,
                                            AccountDesc = _accountHelper.GetAccountDesc(customer.AccountCode),
                                            Debit = 0,
                                            Credit = receivable,
                                            ArStandardCollectionId = record.ArStandardCollectionId
                                        };
                                    distributions.Add(defaultDistribution);

                                }
                            }

                            #endregion
                           
                        }

                        else if (line.Source == InvoiceSource.SalesOrder)
                        {
                            //#region credit tax
                            //var tax = _db.GLTaxRates.FirstOrDefault(x => x.TaxName == line.TaxType);

                            //if (tax != null && line.TaxAmount > 0)
                            //{
                            //    var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == tax.ArAccountCode);

                            //    if (collectionDistribution != null)
                            //    {
                            //        collectionDistribution.AccountCode = tax.ArAccountCode;
                            //        collectionDistribution.AccountDesc =
                            //            _accountHelper.GetAccountDesc(tax.ArAccountCode);
                            //        collectionDistribution.Debit = 0;
                            //        collectionDistribution.Credit += line.TaxAmount;
                            //        collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                            //    }
                            //    else
                            //    {
                            //        var defaultDistribution = new ArStandardCollectionDistribution
                            //        {
                            //            AccountCode = tax.ArAccountCode,
                            //            AccountDesc = _accountHelper.GetAccountDesc(tax.ArAccountCode),
                            //            Debit = 0,
                            //            Credit = line.TaxAmount,
                            //            ArStandardCollectionId = record.ArStandardCollectionId
                            //        };
                            //        distributions.Add(defaultDistribution);
                            //    }

                            //}

                            //#endregion
                            #region get discount account

                            string discountAccount = "";
                            var setting = _db.FinanceSettings.FirstOrDefault();
                            if (setting != null) discountAccount = setting.DiscountArAccount;

                            #endregion
                            #region get sales order

                            int salesorderid = int.Parse(line.InvoiceNumber.Split('_')[1]);
                            var salesorder = _db.ArSalesOrders.Find(salesorderid);
                            #endregion
                           
                            if (salesorder != null && salesorder.OrderType == OrderType.Cash)
                            {
                                #region get sales order lines
                                var salesorderlines = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == salesorderid).ToList();
                                decimal totalDiscount = salesorderlines.Sum(x => x.Discount);
                                #endregion
                                #region debit discount
                                
                                if (totalDiscount >0)
                                {
                                    var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == discountAccount);

                                    if (collectionDistribution != null)
                                    {
                                        collectionDistribution.AccountCode = discountAccount;
                                        collectionDistribution.AccountDesc =
                                            _accountHelper.GetAccountDesc(discountAccount);
                                        collectionDistribution.Debit += totalDiscount;
                                        collectionDistribution.Credit = 0;
                                        collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                                    }
                                    else
                                    {
                                        var defaultDistribution = new ArStandardCollectionDistribution
                                        {
                                            AccountCode = discountAccount,
                                            AccountDesc = _accountHelper.GetAccountDesc(discountAccount),
                                            Debit = totalDiscount,
                                            Credit = 0,
                                            ArStandardCollectionId = record.ArStandardCollectionId
                                        };
                                        distributions.Add(defaultDistribution);
                                    }

                                }

                                #endregion
                                foreach (var salesline in salesorderlines)
                                {
                                    #region credit revenue-- product..
                                    if (salesline.SalesPrice > 0)
                                    {
                                        var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == salesline.AccountCode);

                                        if (collectionDistribution != null)
                                        {
                                            collectionDistribution.AccountCode = salesline.AccountCode;
                                            collectionDistribution.AccountDesc =
                                                _accountHelper.GetAccountDesc(salesline.AccountCode);
                                            collectionDistribution.Debit = 0;
                                            collectionDistribution.Credit += salesline.SalesPrice;
                                            collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                                        }
                                        else
                                        {
                                            var defaultDistribution = new ArStandardCollectionDistribution
                                            {
                                                AccountCode = salesline.AccountCode,
                                                AccountDesc = _accountHelper.GetAccountDesc(salesline.AccountCode),
                                                Debit = 0,
                                                Credit = salesline.SalesPrice,
                                                ArStandardCollectionId = record.ArStandardCollectionId
                                            };
                                            distributions.Add(defaultDistribution);
                                        }

                                    }

                                    #endregion
                                    #region credit tax

                                    var tax = _db.GLTaxRates.Find(salesline.GLTaxRateID);
                                    if (tax != null && salesline.TaxAmount > 0 && tax.CalculatedRate != 0)
                                    {
                                        var collectionDistribution = distributions.FirstOrDefault(x => x.AccountCode == tax.ArAccountCode);

                                        if (collectionDistribution != null)
                                        {
                                            collectionDistribution.AccountCode = tax.ArAccountCode;
                                            collectionDistribution.AccountDesc =
                                                _accountHelper.GetAccountDesc(tax.ArAccountCode);
                                            collectionDistribution.Debit = 0;
                                            collectionDistribution.Credit += salesline.TaxAmount;
                                            collectionDistribution.ArStandardCollectionId = record.ArStandardCollectionId;
                                        }
                                        else
                                        {
                                            var defaultDistribution = new ArStandardCollectionDistribution
                                            {
                                                AccountCode = tax.ArAccountCode,
                                                AccountDesc = _accountHelper.GetAccountDesc(tax.ArAccountCode),
                                                Debit = 0,
                                                Credit = salesline.TaxAmount,
                                                ArStandardCollectionId = record.ArStandardCollectionId
                                            };
                                            distributions.Add(defaultDistribution);
                                        }



                                    }

                                    #endregion
                                }

                            }

                        }

                    }
                }

                foreach (var distribution in distributions)
                {
                    _db.ARStandardCollectionDistributions.Add(distribution);
                    _db.SaveChanges();
                }
            }
        }

        //AP..petty cash,traver order,expense
        public  void UpdateApInvoiceDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.APPaybleInvoices.Find(id);
       
            if (record != null)
            {
                var lines = _db.APPaybleInvoicesLines.Where(x => x.ApPaybleInvoicesId == id).AsNoTracking().ToList();
                if (lines.Any())
                {

                    #region clear existing distribution
                    var records = _db.APPaybleInvoiceDistributions.Where(x => x.ApPaybleInvoicesId == id).AsNoTracking().ToList();
                    _db.APPaybleInvoiceDistributions.RemoveRange(records);
                    _db.SaveChanges();
                    #endregion

                    if (record.Type == ApInvoiceTransactionType.Invoice)
                    {
                        var supplierAccount = _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == record.SupplierId);
                        ApPaybleInvoiceDistribution defaultDistribution;
                        if (supplierAccount != null)
                        {
                            #region credit net payable

                            if (record.NetPayable > 0)
                            {
                                string accountPayable = supplierAccount.AccountCode;
                                defaultDistribution = new ApPaybleInvoiceDistribution
                                {
                                    AccountCode = accountPayable,
                                    AccountDesc = _accountHelper.GetAccountDesc(accountPayable),
                                    Debit = 0,
                                    Credit = record.NetPayable,
                                    ApPaybleInvoicesId = record.ApPaybleInvoicesId
                                };
                                _db.APPaybleInvoiceDistributions.Add(defaultDistribution);
                                _db.SaveChanges();
                            }

                            #endregion
                        }

                        foreach (ApPaybleInvoicesLines line in record.APPaybleInvoicesLines)
                        {
                            #region debit expense

                            if (line.InvoiceAmount > 0)
                            {
                                defaultDistribution = new ApPaybleInvoiceDistribution
                                {
                                    AccountCode = line.AccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode),
                                    Debit = line.InvoiceAmount,
                                    Credit = 0,
                                    ApPaybleInvoicesId = line.ApPaybleInvoicesId
                                };
                                _db.APPaybleInvoiceDistributions.Add(defaultDistribution);
                                _db.SaveChanges();
                            }

                            #endregion
                            #region debit tax
                            var taxRule = _db.GLTaxRates.Where(x => x.TaxName == line.TaxType).ToList();
                            if (taxRule.Any())
                            {
                                var tax = taxRule.FirstOrDefault();
                                if (tax != null)
                                {
                                    if (line.TaxAmount > 0)
                                    {
                                        defaultDistribution = new ApPaybleInvoiceDistribution
                                        {
                                            AccountCode = tax.ApAccountCode,
                                            AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode),
                                            Debit = line.TaxAmount,
                                            Credit = 0,
                                            ApPaybleInvoicesId = line.ApPaybleInvoicesId
                                        };
                                        _db.APPaybleInvoiceDistributions.Add(defaultDistribution);
                                        _db.SaveChanges();
                                    }
                                }
                            }

                            #endregion
                            #region credit withhelding 2
                            var withholdingRule = _db.GLTaxRates.Where(x => x.TaxName == line.WithholdingType).ToList();
                            if (withholdingRule != null)
                            {
                                var withholding = withholdingRule.FirstOrDefault();
                                if (withholding != null)
                                {
                                    if (line.WithholdingAmount > 0)
                                    {
                                        defaultDistribution = new ApPaybleInvoiceDistribution();
                                        defaultDistribution.AccountCode = withholding.WithholdingAccountCode;
                                        defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(withholding.WithholdingAccountCode);
                                        defaultDistribution.Debit = 0;
                                        defaultDistribution.Credit = line.WithholdingAmount;
                                        defaultDistribution.ApPaybleInvoicesId = line.ApPaybleInvoicesId;
                                        _db.APPaybleInvoiceDistributions.Add(defaultDistribution);
                                        _db.SaveChanges();
                                    }
                                }
                            }

                            #endregion
                            #region credit vat withholding

                            if (line.WithholdTaxAmount > 0)
                            {
                                var withhodingtaxaccount = _db.GLVatWithholdings.FirstOrDefault();
                                defaultDistribution = new ApPaybleInvoiceDistribution();
                                if (withhodingtaxaccount != null)
                                {
                                    defaultDistribution.AccountCode = withhodingtaxaccount.WithholdingAccountCode;
                                    defaultDistribution.AccountDesc =
                                        _accountHelper.GetAccountDesc(withhodingtaxaccount.WithholdingAccountCode);
                                }
                                defaultDistribution.Debit = 0;
                                defaultDistribution.Credit = line.WithholdTaxAmount;
                                defaultDistribution.ApPaybleInvoicesId = record.ApPaybleInvoicesId;
                                _db.APPaybleInvoiceDistributions.Add(defaultDistribution);
                                _db.SaveChanges();
                            }

                            #endregion
                        }

                    }
                    else if (record.Type == ApInvoiceTransactionType.Prepayment)
                    {

                    }

                    else
                    {

                    }
                }
            }

        }
        public  void UpdateApMiscPaymentDistribution(int id)
        {
            List<ApMiscellaneousPaymentDistribution> distributions = new List<ApMiscellaneousPaymentDistribution>();
            _accountHelper = new AccountHelper();
            var record = _db.APMiscellaneousPayments.Find(id);
            if (record != null)
            {
                #region clear existing distribution
                var records = _db.APMiscellaneousPaymentDistributions.Where(x => x.ApMiscellaneousPaymentId == id).ToList();
                _db.APMiscellaneousPaymentDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion
                

                var paymentlines = _db.ApMiscellaneousPaymentLines.Where(x => x.ApMiscellaneousPaymentId == id).AsNoTracking().ToList();
                decimal totalnet = paymentlines.Sum(x => x.NetPayable);

                foreach (var line in paymentlines)
                {
                    #region debit request amount

                    if (line.Amount > 0)
                    {
                        var distribution = distributions.FirstOrDefault(x => x.AccountCode == line.AccountCode);
                        if (distribution != null)
                        {
                            distribution.AccountCode = line.AccountCode;
                            distribution.AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode);
                            distribution.Debit += line.Amount;
                            distribution.Credit = 0;
                            distribution.ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId;
                        }
                        else
                        {
                            var defaultDistribution = new ApMiscellaneousPaymentDistribution
                            {
                                AccountCode = line.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(line.AccountCode),
                                Debit = line.Amount,
                                Credit = 0,
                                ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId
                            };
                            distributions.Add(defaultDistribution);
                        }
                    }

                    #endregion
                    #region debit tax

                    if (line.TaxAmount > 0)
                    {
                        //get  account
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            var distribution = distributions.FirstOrDefault(x => x.AccountCode == tax.ApAccountCode);
                            if (distribution != null)
                            {
                                distribution.AccountCode = tax.ApAccountCode;
                                distribution.AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode);
                                distribution.Debit += line.TaxAmount;
                                distribution.Credit = 0;
                                distribution.ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId;
                            }
                            else
                            {
                                var defaultDistribution =new ApMiscellaneousPaymentDistribution
                                    {
                                        AccountCode = tax.ApAccountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode),
                                        Debit = line.TaxAmount,
                                        Credit = 0,
                                        ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId
                                    };
                                distributions.Add(defaultDistribution);
                            }
                           
                        }
                       
                    }

                    #endregion
                    #region credit withholding 2%

                    if (line.WithholdingAmount > 0)
                    {
                        
                        //get  account
                        var withholding = _db.GLTaxRates.Find(line.WithholdingType);
                        if (withholding != null)
                        {
                            var distribution = distributions.FirstOrDefault(x => x.AccountCode == withholding.WithholdingAccountCode);
                            if (distribution != null)
                            {
                                distribution.AccountCode = withholding.WithholdingAccountCode;
                                distribution.AccountDesc = _accountHelper.GetAccountDesc(withholding.WithholdingAccountCode);
                                distribution.Debit = 0;
                                distribution.Credit += line.WithholdingAmount;
                                distribution.ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId;
                            }
                            else
                            {
                                var defaultDistribution =new ApMiscellaneousPaymentDistribution
                                    {
                                        AccountCode = withholding.WithholdingAccountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(withholding.WithholdingAccountCode),
                                        Debit = 0,
                                        Credit = line.WithholdingAmount,
                                        ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId
                                    };
                                distributions.Add(defaultDistribution);
                            }
                        }
                           
                        }
                       

                    #endregion
                    #region credit withholding tax

                    if (line.WithholdTaxAmount > 0)
                    {
                        
                        //get  account
                        var vatWithholding = _db.GLVatWithholdings.FirstOrDefault();
                        if (vatWithholding != null)
                        {
                            var distribution = distributions.FirstOrDefault(x => x.AccountCode == vatWithholding.WithholdingAccountCode);
                            if (distribution != null)
                            {
                                distribution.AccountCode = vatWithholding.WithholdingAccountCode;
                                distribution.AccountDesc =_accountHelper.GetAccountDesc(vatWithholding.WithholdingAccountCode);
                                distribution.Debit = 0;
                                distribution.Credit += line.WithholdTaxAmount;
                                distribution.ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId;
                            }
                            else
                            {
                                var defaultDistribution =new ApMiscellaneousPaymentDistribution
                                    {
                                        AccountCode = vatWithholding.WithholdingAccountCode,
                                        AccountDesc =_accountHelper.GetAccountDesc(vatWithholding.WithholdingAccountCode),
                                        Debit = 0,
                                        Credit = line.WithholdTaxAmount,
                                        ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId
                                    };
                                distributions.Add(defaultDistribution);
                            }
                          
                        }
                        
                    }

                    #endregion
                }

                #region credit bank/cash

                if (totalnet > 0)
                {
                    
                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.PaymentMethod);

                    if (paymentMethod != null)
                    {
                        var distribution = distributions.FirstOrDefault(x => x.AccountCode == paymentMethod.ApAccountCode);
                        if (distribution != null)
                        {
                            distribution.AccountCode = paymentMethod.ApAccountCode;
                            distribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            distribution.Debit = 0;
                            distribution.Credit += totalnet;
                            distribution.ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId;
                        }
                        else
                        {
                            var defaultDistribution =new ApMiscellaneousPaymentDistribution
                                {
                                    AccountCode = paymentMethod.ApAccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode),
                                    Debit = 0,
                                    Credit = totalnet,
                                    ApMiscellaneousPaymentId = record.ApMiscellaneousPaymentId
                                };
                            distributions.Add(defaultDistribution);
                        }
                       
                    }
                    
                }

                #endregion


                foreach (var distribution in distributions)
                {
                    _db.APMiscellaneousPaymentDistributions.Add(distribution);
                    _db.SaveChanges();
                }
            }

        }
        public  void UpdateApDirectPaymentDistribution(int id)
        {
           
            _accountHelper = new AccountHelper();
            var record = _db.ApDirectPayments.Find(id);
            string lcAccountCode = "";           
            if (record != null)
            {
                int did = int.Parse(record.SourceId.Split('_')[1]);
                string sourceType = record.SourceId.Split('_')[0];
                
                var lccost = _db.ForeignPurchaseCosts.Find(did);
                if (lccost != null)
                {
                    var lcaccount = _db.ForeignPurchaseAccounts
                        .FirstOrDefault(x => x.ForeignPurchaseId == lccost.ForeignPurchaseId);
                    if (lcaccount != null) lcAccountCode = lcaccount.AccountCode;
                }

                #region clear existing distribution
                var records = _db.ApDirectPaymentDistributions.Where(x => x.ApDirectPaymentId == id).ToList();
                _db.ApDirectPaymentDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion
                ApDirectPaymentDistribution defaultDistribution;

                var paymentlines = _db.ApDirectPaymentLines.Where(x => x.ApDirectPaymentId == id).AsNoTracking().ToList();
                decimal total = paymentlines.Sum(x => x.TotalAmount);
                decimal totalnet = paymentlines.Sum(x => x.NetPayable);
                #region debit expense
                switch (sourceType)
                {
                    case "LC":
                        if (!string.IsNullOrEmpty(lcAccountCode))
                        {
                            #region debit lcaccount

                            if (total > 0)
                            {
                                defaultDistribution = new ApDirectPaymentDistribution
                                {
                                    AccountCode = lcAccountCode,
                                    AccountDesc = _accountHelper.GetAccountDesc(lcAccountCode),
                                    Debit = total,
                                    Credit = 0,
                                    ApDirectPaymentId = record.ApDirectPaymentId
                                };
                                _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                                _db.SaveChanges();
                            }

                            #endregion
                        }
                        break;
                    case "PAYMENT-REQUEST":
                        var paymentRequestLines = _db.PaymentRequestLines.Where(x => x.PaymentRequestId == did).ToList();
                        foreach(var line in paymentRequestLines)
                        {
                            var setupitem = _db.ApSetupItems.Find(int.Parse(line.ItemName));
                            if (setupitem != null)
                            {
                                #region debit expense account

                                if (line.Amount > 0)
                                {
                                    defaultDistribution = new ApDirectPaymentDistribution
                                    {
                                        AccountCode = setupitem.AccountCode,
                                        AccountDesc = _accountHelper.GetAccountDesc(setupitem.AccountCode),
                                        Debit = line.Amount,
                                        Credit = 0,
                                        ApDirectPaymentId = record.ApDirectPaymentId
                                    };
                                    _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                                    _db.SaveChanges();
                                }

                                #endregion
                            }
                        }
                        break;
                    case "TRAVEL-ORDER":

                        break;
                    case "EXPENSE-CLAIM":

                        break;
                }
                #endregion
                
                foreach (var line in paymentlines)
                {
                    
                    #region debit tax

                    if (line.TaxAmount > 0)
                    {

                        defaultDistribution = new ApDirectPaymentDistribution();
                        //get  account
                        var tax = _db.GLTaxRates.Find(line.GLTaxRateID);
                        if (tax != null)
                        {
                            defaultDistribution.AccountCode = tax.ApAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode);
                        }
                        defaultDistribution.Debit = line.TaxAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ApDirectPaymentId = record.ApDirectPaymentId;
                        _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion
                    #region credit withholding 2%

                    if (line.WithholdingAmount > 0)
                    {
                        defaultDistribution = new ApDirectPaymentDistribution();
                        //get  account
                        var withholding = _db.GLTaxRates.Find(line.WithholdingType);
                        if (withholding != null)
                        {
                            defaultDistribution.AccountCode = withholding.WithholdingAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(withholding.WithholdingAccountCode);
                        }
                        defaultDistribution.Debit = 0;
                        defaultDistribution.Credit = line.WithholdingAmount;
                        defaultDistribution.ApDirectPaymentId = record.ApDirectPaymentId;
                        _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion
                    #region credit withholding tax

                    if (line.WithholdTaxAmount > 0)
                    {
                        defaultDistribution = new ApDirectPaymentDistribution();
                        //get  account
                        var vatWithholding = _db.GLVatWithholdings.FirstOrDefault();
                        if (vatWithholding != null)
                        {
                            defaultDistribution.AccountCode = vatWithholding.WithholdingAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(vatWithholding.WithholdingAccountCode);
                        }
                        defaultDistribution.Debit = 0;
                        defaultDistribution.Credit = line.WithholdTaxAmount;
                        defaultDistribution.ApDirectPaymentId = record.ApDirectPaymentId;
                        _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion
                }

                #region credit bank/cash

                if (totalnet > 0)
                {
                    defaultDistribution = new ApDirectPaymentDistribution();
                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.PaymentMethod);

                    if (paymentMethod != null)
                    {
                        defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                        defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                    }
                    defaultDistribution.Debit = 0;
                    defaultDistribution.Credit = totalnet;
                    defaultDistribution.ApDirectPaymentId = record.ApDirectPaymentId;
                    _db.ApDirectPaymentDistributions.Add(defaultDistribution);
                    _db.SaveChanges();
                }

                #endregion

            }

        }
        public void UpdateApPaymentSettlementDistribution(int id)
        {
        }
            public  void UpdateApPaymentDistribution(int id)//check cache problem
        {
            _accountHelper = new AccountHelper();
            var record = _db.APPayments.Find(id);

            if (record != null)
            {
                #region clear existing distribution
                var records = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id).ToList();
                _db.APPaymentDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion

                ApPaymentDistribution defaultDistribution;
                #region credit bank/cash

                decimal net = record.NetAmount - record.TaxAmount;

                if (net > 0)
                {

                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.PaymentMethod);
                    defaultDistribution = new ApPaymentDistribution();
                    if (paymentMethod != null)
                    {
                        defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                        defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                    }
                    defaultDistribution.Debit = 0;
                    defaultDistribution.Credit = net;
                    defaultDistribution.ApPaymentId = record.ApPaymentId;
                    _db.APPaymentDistributions.Add(defaultDistribution);
                    _db.SaveChanges();
                }

                #endregion
                #region credit tax

                var tax = _db.GLTaxRates.Find(record.GLTaxRateID);
                if (tax != null)
                {
                    defaultDistribution = new ApPaymentDistribution
                    {
                        AccountCode = tax.ApAccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode),
                        Debit = 0,
                        Credit = record.TaxAmount,
                        ApPaymentId = record.ApPaymentId
                    };
                    _db.APPaymentDistributions.Add(defaultDistribution);
                    _db.SaveChanges();

                }

                #endregion

                #region get prepayment account
                var prepayment = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type==AdvanceType.Supplier);
                #endregion

                //#region credit prepayment

                //if (record.PrepaymentAmount > 0)
                //{

                //    defaultDistribution = new ApPaymentDistribution();

                //    if (prepayment != null)
                //    {
                //        AccountHelper accountHelper = new AccountHelper();
                //        string acc = prepayment.PrepaymentAccountCode;
                //        string[] account = acc.Split('-');
                //        string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                //        string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                //        string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                //        string subAccountSeG =record.SupplierId;
                //        string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                //            costCenterSeG, subAccountSeG);

                //        defaultDistribution.AccountCode = newacc;
                //        defaultDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                //        defaultDistribution.Debit = 0;
                //        defaultDistribution.Credit = record.PrepaymentAmount;
                //        defaultDistribution.ApPaymentId = record.ApPaymentId;
                //        _db.APPaymentDistributions.Add(defaultDistribution);
                //        _db.SaveChanges();
                //    }
                   
                //}

                //#endregion

                if (record.IsPrepayment)
                {
                    if (record.CollectedByPurchaser)
                    {
                        #region debit purchaser prepayment

                        #region get prepayment account of purchaser
                        var purchaseracc = _db.Purchasers.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);
                        #endregion

                        if (record.PrepaymentAmount > 0 && purchaseracc!=null)
                        {

                            //collector account
                            defaultDistribution = new ApPaymentDistribution();

                            if (prepayment != null)
                            {
                                defaultDistribution.AccountCode = purchaseracc.AccountCode;
                                defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(prepayment.PrepaymentAccountCode);
                            }
                            defaultDistribution.Debit = record.PrepaymentAmount;
                            defaultDistribution.Credit = 0;
                            defaultDistribution.ApPaymentId = record.ApPaymentId;
                            _db.APPaymentDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }

                        #endregion
                    }
                    if (!record.CollectedByPurchaser)
                    {
                        #region debit supplier prepayment

                        if (record.PrepaymentAmount > 0)
                        {

                            //collector account
                            defaultDistribution = new ApPaymentDistribution();

                            if (prepayment != null)
                            {
                                AccountHelper accountHelper = new AccountHelper();
                                string acc = prepayment.PrepaymentAccountCode;
                                string[] account = acc.Split('-');
                                string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                                string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                                string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                                string subAccountSeG = record.SupplierId;
                                string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,
                                    costCenterSeG, subAccountSeG);

                                defaultDistribution.AccountCode = newacc;
                                defaultDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            }
                            defaultDistribution.Debit = record.PrepaymentAmount;
                            defaultDistribution.Credit = 0;
                            defaultDistribution.ApPaymentId = record.ApPaymentId;
                            _db.APPaymentDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }

                        #endregion

                    }


                }
                if (!record.IsPrepayment)
                {
                    #region debit payable

                    if (record.PaymentAmount > 0)
                    {
                        defaultDistribution = new ApPaymentDistribution();
                        //get payable account
                        var invoice = _db.APPaybleInvoices.FirstOrDefault(x => x.InvoiceNumber == record.InvoiceNumber);
                        var supplierAccount =
                            _db.ProcurementSuppliers.FirstOrDefault(x => x.SupplierId == invoice.SupplierId);

                        if (supplierAccount != null)
                        {
                            defaultDistribution.AccountCode = supplierAccount.AccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(supplierAccount.AccountCode);
                        }
                        defaultDistribution.Debit = record.PaymentAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ApPaymentId = record.ApPaymentId;
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion

                }
            }

        }
        public  void UpdateApAdvancePaymentDistribution(int id)//check cache problem
        {
            _accountHelper = new AccountHelper();
            var record = _db.APPayments.Find(id);

            if (record != null)
            {
                #region clear existing distribution

                var records = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id).ToList();
                _db.APPaymentDistributions.RemoveRange(records);
                _db.SaveChanges();

                #endregion

                ApPaymentDistribution defaultDistribution;

                if (record.CollectedByPurchaser)
                {
                    #region get prepayment account
                    var prepaymentPurchaser = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Purchaser);
                    #endregion
                    #region debit purchaser prepayment

                    #region get prepayment account of purchaser
                    var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);
                    #endregion

                    if (record.PrepaymentAmount > 0 && employeeSubAccount != null)
                    {

                        //collector account
                        defaultDistribution = new ApPaymentDistribution();

                        if (prepaymentPurchaser != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string acc = prepaymentPurchaser.PrepaymentAccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = employeeSubAccount.SubAccountCode;
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);

                            defaultDistribution.AccountCode = newacc;
                            defaultDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                        }
                        defaultDistribution.Debit = record.PrepaymentAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ApPaymentId = record.ApPaymentId;
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion
                }
                if (!record.CollectedByPurchaser)
                {
                    #region get prepayment account
                    var prepaymentSupplier = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Supplier);
                    #endregion
                    #region debit supplier prepayment

                    if (record.PrepaymentAmount > 0)
                    {

                        //collector account
                        defaultDistribution = new ApPaymentDistribution();

                        if (prepaymentSupplier != null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            if (!String.IsNullOrWhiteSpace(record.SupplierId))
                            {
                                string acc = prepaymentSupplier.PrepaymentAccountCode;
                                string[] account = acc.Split('-');
                                string acccountSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                                string locationSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                                string costCenterSeG =account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                                string subAccountSeG = record.SupplierId;
                                string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);

                                defaultDistribution.AccountCode = newacc;
                                defaultDistribution.AccountDesc = accountHelper.GetAccountDesc(newacc);
                            }
                            else
                            {
                                defaultDistribution.AccountCode = prepaymentSupplier.PrepaymentAccountCode;
                                defaultDistribution.AccountDesc = accountHelper.GetAccountDesc(prepaymentSupplier.PrepaymentAccountCode);
                            }
                        }
                        defaultDistribution.Debit = record.PrepaymentAmount;
                        defaultDistribution.Credit = 0;
                        defaultDistribution.ApPaymentId = record.ApPaymentId;
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }

                    #endregion

                }

                #region debit tax

                if (record.TaxAmount > 0)
                {
                    var tax = _db.GLTaxRates.Find(record.GLTaxRateID);
                    if (tax != null)
                    {
                        defaultDistribution = new ApPaymentDistribution
                        {
                            AccountCode = tax.ApAccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(tax.ApAccountCode),
                            Debit = record.TaxAmount,
                            Credit = 0,
                            ApPaymentId = record.ApPaymentId
                        };
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();

                    }
                }

                #endregion

               
                #region credit withholding

                if (record.WithholdingAmount > 0)
                {
                    var withholding = _db.GLTaxRates.Find(int.Parse(record.WithholdingType));
                    if (withholding != null)
                    {
                        defaultDistribution = new ApPaymentDistribution
                        {
                            AccountCode = withholding.WithholdingAccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(withholding.WithholdingAccountCode),
                            Debit = 0,
                            Credit = record.WithholdingAmount,
                            ApPaymentId = record.ApPaymentId
                        };
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();

                    }
                }

                 #endregion
                #region credit tax withholding

                if (record.WithholdTaxAmount > 0)
                {
                    var vatWithholding = _db.GLVatWithholdings.FirstOrDefault();
                    if (vatWithholding != null)
                    {
                        defaultDistribution = new ApPaymentDistribution
                        {
                            AccountCode = vatWithholding.WithholdingAccountCode,
                            AccountDesc = _accountHelper.GetAccountDesc(vatWithholding.WithholdingAccountCode),
                            Debit = 0,
                            Credit = record.WithholdTaxAmount,
                            ApPaymentId = record.ApPaymentId
                        };
                        _db.APPaymentDistributions.Add(defaultDistribution);
                        _db.SaveChanges();

                    }
                }

                #endregion
                #region credit bank/cash

                decimal net = record.NetAmount - record.TaxAmount;

                if (net > 0)
                {

                    var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == record.PaymentMethod);
                    defaultDistribution = new ApPaymentDistribution();
                    if (paymentMethod != null)
                    {
                        defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                        defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                    }

                    defaultDistribution.Debit = 0;
                    defaultDistribution.Credit = net;
                    defaultDistribution.ApPaymentId = record.ApPaymentId;
                    _db.APPaymentDistributions.Add(defaultDistribution);
                    _db.SaveChanges();
                }

                #endregion
            }

        }
        public  void UpdateApTravelOrderDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.ApTravelOrders.Find(id);

            if (record != null)
            {
                #region clear existing distribution
                var records = _db.ApTravelOrderDistributions.Where(x => x.ApTravelOrderId == id).ToList();
                _db.ApTravelOrderDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion
                #region debit advance payment
                //get employee advance account

                #region get prepayment account
                var prepayment = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Employee);
                #endregion
                if (record.AdvanceAmount > 0)
                {
                    #region get prepayment account of employee
                    var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);
                    #endregion
                    if (prepayment != null && employeeSubAccount!=null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = prepayment.PrepaymentAccountCode;
                        string[] account = acc.Split('-');
                        string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        string subAccountSeG = employeeSubAccount.SubAccountCode;
                        string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG,costCenterSeG, subAccountSeG);


                        ApTravelOrderDistribution defaultDistribution = new ApTravelOrderDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = record.AdvanceAmount,
                            Credit = 0,
                            ApTravelOrderId = record.ApTravelOrderId
                        };

                        _db.ApTravelOrderDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }

                #endregion
                #region credit bank/cash

                if (record.AdvanceAmount > 0)
                {
                    var advance =
                        _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == record.ApTravelOrderId);
                    if (advance.Any())
                    {
                        var paymentMethod =
                            _db.PaymentMethods.FirstOrDefault(x => x.Method == advance.FirstOrDefault().PaymentMethod);
                        ApTravelOrderDistribution defaultDistribution = new ApTravelOrderDistribution();
                        if (paymentMethod != null)
                        {
                            defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            defaultDistribution.Debit = 0;
                            defaultDistribution.Credit = record.AdvanceAmount;
                            defaultDistribution.ApTravelOrderId = record.ApTravelOrderId;
                            _db.ApTravelOrderDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }
                    }

                }

                #endregion

                
            }

        }
        public  void UpdateApTravelOrdeExpenseClaimDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.ApTravelExpenseClaims.Find(id);
            ApTravelOrder travelorder = _db.ApTravelOrders.Find(id);

            if (record != null)
            {
                #region clear existing distribution
                var records = _db.ApTravelExpenseDistributions.Where(x => x.ApTravelExpenseClaimId == record.ApTravelExpenseClaimId).ToList();
                _db.ApTravelExpenseDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion

                #region get prepayment account
                var prepayment = _db.ApSetupReceiptBalanceAccounts.FirstOrDefault(x => x.Type == AdvanceType.Employee);
                #endregion

                #region debit expense

                if (travelorder != null && travelorder.AdvanceAmount > 0)
                {
                    var advance =
                        _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == record.ApTravelOrderId);
                    if (advance.Any())
                    {
                        var paymentMethod =
                            _db.PaymentMethods.FirstOrDefault(x => x.Method == advance.FirstOrDefault().PaymentMethod);
                       
                        ApTravelExpenseDistribution defaultDistribution = new ApTravelExpenseDistribution();
                        if (paymentMethod != null)
                        {
                            defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            defaultDistribution.Debit = travelorder.AdvanceAmount;
                            defaultDistribution.Credit = 0;
                            defaultDistribution.ApTravelExpenseClaimId = record.ApTravelExpenseClaimId;
                            _db.ApTravelExpenseDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }
                    }

                }

                #endregion
                #region credit employee advance payment
                //get employee advance account

                
                if (travelorder != null && travelorder.AdvanceAmount > 0)
                {
                    #region get prepayment account of employee
                    var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);
                    #endregion
                    if (prepayment != null&& employeeSubAccount!=null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = prepayment.PrepaymentAccountCode;
                        string[] account = acc.Split('-');
                        string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        string subAccountSeG = employeeSubAccount.SubAccountCode;
                        string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                        ApTravelExpenseDistribution defaultDistribution = new ApTravelExpenseDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = travelorder.AdvanceAmount,
                            ApTravelExpenseClaimId = record.ApTravelExpenseClaimId
                        };

                        _db.ApTravelExpenseDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }

                #endregion
                //payment
                var payments = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                decimal totalpayment = payments.Sum(x => x.Amount);
                if (totalpayment > 0)
                {
                    #region debit expense

                    ApTravelExpenseDistribution defaultDistribution =new ApTravelExpenseDistribution
                        {
                            AccountCode = "NA",
                            AccountDesc = "NA",
                            Debit = totalpayment,
                            Credit = 0,
                            ApTravelExpenseClaimId = record.ApTravelExpenseClaimId
                        };
                    _db.ApTravelExpenseDistributions.Add(defaultDistribution);
                    _db.SaveChanges();

                    #endregion
                    foreach (var payment in payments)
                    {
                        #region credit bank/cash

                        var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == payment.PaymentMethod);
                        ApTravelExpenseDistribution distribution = new ApTravelExpenseDistribution();
                        if (paymentMethod != null)
                        {
                            distribution.AccountCode = paymentMethod.ApAccountCode;
                            distribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            distribution.Debit = 0;
                            distribution.Credit = totalpayment;
                            defaultDistribution.AdditionalDescription = payment.Reference;
                            distribution.ApTravelExpenseClaimId = record.ApTravelExpenseClaimId;
                            _db.ApTravelExpenseDistributions.Add(distribution);
                            _db.SaveChanges();
                        }

                        #endregion
                    }
                }

                //refund
                var expenseRefunds = _db.ApTravelExpenseRefunds.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                decimal totalrefund = expenseRefunds.Sum(x => x.Amount);
                if (totalrefund > 0)
                {
                    foreach (var expenseRefund in expenseRefunds)
                    {
                        #region debit bank/cash

                        var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == expenseRefund.PaymentMethod);
                        ApTravelExpenseDistribution distribution = new ApTravelExpenseDistribution();
                        if (paymentMethod != null)
                        {
                            distribution.AccountCode = paymentMethod.ApAccountCode;
                            distribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            distribution.Debit =totalrefund;
                            distribution.Credit = 0;
                            distribution.ApTravelExpenseClaimId = record.ApTravelExpenseClaimId;
                            distribution.AdditionalDescription = expenseRefund.Reference;
                            _db.ApTravelExpenseDistributions.Add(distribution);
                            _db.SaveChanges();
                        }

                        #endregion
                    }
                    #region credit employee advance payment
                  
                    if (travelorder != null && travelorder.AdvanceAmount > 0)
                    {
                        #region get prepayment account of employee
                        var employeeSubAccount = _db.EmployeeSubAccounts.FirstOrDefault(x => x.EmployeeId == record.EmployeeId);
                        #endregion
                        if (prepayment != null&& employeeSubAccount!=null)
                        {
                            AccountHelper accountHelper = new AccountHelper();
                            string acc = prepayment.PrepaymentAccountCode;
                            string[] account = acc.Split('-');
                            string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                            string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                            string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                            string subAccountSeG = employeeSubAccount.SubAccountCode;
                            string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                            ApTravelExpenseDistribution defaultDistribution = new ApTravelExpenseDistribution
                            {
                                AccountCode = newacc,
                                AccountDesc = accountHelper.GetAccountDesc(newacc),
                                Debit = 0,
                                Credit = totalrefund,
                                ApTravelExpenseClaimId = record.ApTravelExpenseClaimId
                            };

                            _db.ApTravelExpenseDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }
                    }

                    #endregion
                }
            }

        }
        public  void UpdateApPettyCashDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.ApPettyCash.Find(id);

            if (record != null)
            {
                #region clear existing distribution
                var records = _db.ApPettyCashDistributions.Where(x => x.ApPettyCashId == record.ApPettyCashId).ToList();
                _db.ApPettyCashDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion
                #region get cashier account
                var cashierAccount = _db.CashierSetups.Find(record.ApCashierSetupId);
                #endregion
                #region payment

                var payments = _db.ApPettyCashPayments.Where(x => x.ApPettyCashId == id).ToList();
                decimal totalPaymentAmount = payments.Sum(x => x.Amount);
                #region debit cashier account
                //get employee advance account

              
                if (totalPaymentAmount > 0)
                {
                    if (cashierAccount != null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = cashierAccount.AccountCode;
                        string[] account = acc.Split('-');
                        string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];//will be replaced
                        string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                        ApPettyCashDistribution defaultDistribution = new ApPettyCashDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = totalPaymentAmount,
                            Credit = 0,
                            ApPettyCashId = record.ApPettyCashId
                        };

                        _db.ApPettyCashDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }

                #endregion
                #region credit bank/cash

                foreach (var payment in payments)
                {
                    if (payment.Amount > 0)
                    {

                        var paymentMethod = _db.PaymentMethods.FirstOrDefault(x => x.Method == payment.PaymentMethod);
                        ApPettyCashDistribution defaultDistribution = new ApPettyCashDistribution();
                        if (paymentMethod != null)
                        {
                            defaultDistribution.AccountCode = paymentMethod.ApAccountCode;
                            defaultDistribution.AccountDesc = _accountHelper.GetAccountDesc(paymentMethod.ApAccountCode);
                            defaultDistribution.Debit = 0;
                            defaultDistribution.Credit = payment.Amount;
                            defaultDistribution.AdditionalDescription = payment.Reference;
                            defaultDistribution.ApPettyCashId = record.ApPettyCashId;
                            _db.ApPettyCashDistributions.Add(defaultDistribution);
                            _db.SaveChanges();
                        }

                    }
                }


                #endregion
                #endregion
                #region utilization

                var cashUtilizations = _db.ApPettyCashUtilizations.Where(x => x.ApPettyCashId == id).ToList();
                decimal totalUtilizationAmount = cashUtilizations.Sum(x => x.Amount);
               
                #region debit expense

                foreach (var utilization in cashUtilizations)
                {
                    if (utilization.Amount > 0)
                    {
                        ApPettyCashDistribution defaultDistribution =
                            new ApPettyCashDistribution
                            {
                                AccountCode = utilization.AccountCode,
                                AccountDesc = _accountHelper.GetAccountDesc(utilization.AccountCode),
                                Debit = utilization.Amount,
                                Credit = 0,
                                ApPettyCashId = record.ApPettyCashId
                            };
                        _db.ApPettyCashDistributions.Add(defaultDistribution);
                        _db.SaveChanges();

                    }
                }


                #endregion
                #region credit cashier account


                if (totalUtilizationAmount > 0)
                {
                    if (cashierAccount != null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = cashierAccount.AccountCode;
                        string[] account = acc.Split('-');
                        string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];//will be replaced
                        string newacc = accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                        ApPettyCashDistribution defaultDistribution = new ApPettyCashDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = totalUtilizationAmount,
                            ApPettyCashId = record.ApPettyCashId
                        };

                        _db.ApPettyCashDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }

                #endregion
                #endregion
            }

        }
        public  void UpdateBankServiceChargeDistribution(int id)
        {
            _accountHelper = new AccountHelper();
            var record = _db.ReconciliationSchedules.Find(id);

            if (record != null)
            {
                decimal amount = record.ServiceCharges.Sum(x => x.Amount);
                #region clear existing distribution
                var records = _db.ReconciliationBankServiceChargeDistributions.Where(x => x.ReconciliationScheduleId ==id).ToList();
                _db.ReconciliationBankServiceChargeDistributions.RemoveRange(records);
                _db.SaveChanges();
                #endregion

                #region debit bank
                #region bank account account

                var reconciliationSchedule = _db.ReconciliationSchedules.FirstOrDefault(x => x.ReconciliationScheduleId == record.ReconciliationScheduleId);
                var bankaccount = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
                #endregion
               
                if (amount > 0)
                {
                    if (bankaccount != null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = bankaccount.AccountCode;
                        //string[] account = acc.Split('-');
                        //string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        //string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        //string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        //string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];//will be replaced
                        string newacc = acc;//accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                        ReconciliationBankServiceChargeDistribution defaultDistribution = new ReconciliationBankServiceChargeDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = amount,
                            Credit = 0,
                            ReconciliationScheduleId = record.ReconciliationScheduleId
                        };

                        _db.ReconciliationBankServiceChargeDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }
                #endregion
                #region credit service charge account
                #region get service charge expense account

                var financeSetting = _db.FinanceSettings.FirstOrDefault();
                #endregion

               
                if (amount > 0)
                {
                    if (financeSetting != null)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        string acc = financeSetting.BankServiceChargeExpense;
                        //string[] account = acc.Split('-');
                        //string acccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account)];
                        //string locationSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Location)];
                        //string costCenterSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.CostCenter)];
                        //string subAccountSeG = account[accountHelper.GetAccountSequenceIndex(GlSegmentTypes.SubAccount)];//will be replaced
                        string newacc = acc;//accountHelper.RearrangeAccountSequence(acccountSeG, locationSeG, costCenterSeG, subAccountSeG);


                        ReconciliationBankServiceChargeDistribution defaultDistribution = new ReconciliationBankServiceChargeDistribution
                        {
                            AccountCode = newacc,
                            AccountDesc = accountHelper.GetAccountDesc(newacc),
                            Debit = 0,
                            Credit = amount,
                            ReconciliationScheduleId = record.ReconciliationScheduleId
                        };

                        _db.ReconciliationBankServiceChargeDistributions.Add(defaultDistribution);
                        _db.SaveChanges();
                    }
                }
                #endregion
            }

        }
    }
}
