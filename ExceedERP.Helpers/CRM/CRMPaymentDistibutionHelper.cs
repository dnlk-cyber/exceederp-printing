using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.CRM
{
    public class CRMPaymentDistibutionHelper
    {
        public  List<SalesInvoicePaymentDistibution> DistributeCashInvoice(CashSalesInvoice Invoice, LineItem Item, List<AdditionalCost> AdditionalCosts, List<AdditionalCostLog> AdditionalCostLogs, List<GLTaxRate> TaxRates, List<PaymentMethod> PaymentMethods, List<AccountMap> AccountMap)
        {
            AccountHelper AccountHelper = new AccountHelper();

            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();

            var PaymentMethod = PaymentMethods.Where(x => x.PaymentMethodID.ToString() == Invoice.RecieveMethod).FirstOrDefault();
            if (PaymentMethod != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {
                    AccountCode = PaymentMethod.ArAccountCode,
                    Debit = Item.Quantity * Item.UnitAmount,
                    AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID

                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }

            var AdditionalCostlog = AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
            foreach (var log in AdditionalCostlog)
            {
                if (log != null)
                {
                    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                    {
                        AccountCode = log.AdditionalCost.SalesAccountCode,
                        Debit = log.Value,
                        AccountDesc = AccountHelper.GetAccountDesc(log.AdditionalCost.SalesAccountCode),
                        CashSalesInvoiceId = Invoice.CashSalesInvoiceID

                    };
                    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

                }
            }
            if (!string.IsNullOrEmpty(Item.WithholdingType))
            {
                var WithHolding = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.WithholdingType).FirstOrDefault();

                if (WithHolding != null)
                {
                    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                    {
                        AccountCode = WithHolding.WithholdingAccountCode,
                        Debit = Item.WithholdingAmount,
                        AccountDesc = AccountHelper.GetAccountDesc(WithHolding.WithholdingAccountCode),
                        CashSalesInvoiceId = Invoice.CashSalesInvoiceID

                    };
                    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

                }
            }
            var TaxRate = TaxRates.Where(x => x.GLTaxRateID.ToString() == Item.Tax).FirstOrDefault();

            if (TaxRate != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {

                    AccountCode = TaxRate.ArAccountCode,
                    Credit = Item.TaxAmount,
                    AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID
                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

                if (Item.WithholdTax)
                {

                    SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                  {

                      AccountCode = TaxRate.ArAccountCode,
                      Debit = Item.TaxAmount,
                      AccountDesc = AccountHelper.GetAccountDesc(TaxRate.ArAccountCode),
                      CashSalesInvoiceId = Invoice.CashSalesInvoiceID
                  };
                    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

                }
            }
            var ProductAccount = AccountMap.Where(x => x.Reference == Item.Product.ProductID.ToString()).FirstOrDefault();

            if (ProductAccount != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {
                    AccountCode = ProductAccount.AccountID,
                    AccountDesc = AccountHelper.GetAccountDesc(ProductAccount.AccountID),
                    Debit = Item.Quantity * Item.UnitAmount,
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID
                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }
            return SalesInvoicePaymentDistibutions;
        }

        public  List<SalesInvoicePaymentDistibution> DistributeCreditInvoice(CashSalesInvoice Invoice, LineItem Item, List<AdditionalCost> AdditionalCosts, List<AdditionalCostLog> AdditionalCostLogs, List<GLTaxRate> TaxRates, List<PaymentMethod> PaymentMethods, List<AccountMap> AccountMap)
        {
            AccountHelper AccountHelper = new AccountHelper();
            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();

            var PaymentMethod = PaymentMethods.Where(x => x.PaymentMethodID.ToString() == Invoice.RecieveMethod).FirstOrDefault();
            if (PaymentMethod != null)
            {
                var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                {
                    AccountCode = PaymentMethod.ArAccountCode,
                    Debit = Item.Quantity * Item.UnitAmount,
                    CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                    AccountDesc = AccountHelper.GetAccountDesc(PaymentMethod.ArAccountCode),

                };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }

            var AdditionalCostlog = AdditionalCostLogs.Where(x => x.LineItemId == Item.LineItemID).ToList();
            foreach (var log in AdditionalCostlog)
            {
                if (log != null)
                {
                    var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
                    {
                        AccountCode = log.AdditionalCost.SalesAccountCode,
                        Debit = log.Value,
                        CashSalesInvoiceId = Invoice.CashSalesInvoiceID,
                        AccountDesc = AccountHelper.GetAccountDesc(log.AdditionalCost.SalesAccountCode),
                    };
                    SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

                }
            }

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


        public  List<AdvancePaymentDistibution> DistributeAdvancePayment(AdvancePaymentLine AdvancePaymentLine, OrganizationCustomer OrganizationCustomer, List<AccountMap> AccountMap)
        {
            ExceedDbContext db = new ExceedDbContext();
            AccountHelper AccountHelper = new AccountHelper();
            var AdvancePaymentDistibution = new List<AdvancePaymentDistibution>();

            var PaymentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == AdvancePaymentLine.RecieveMethod).FirstOrDefault();
            var SalesInvoicePaymentDistibution = new AdvancePaymentDistibution
            {
                AccountCode = PaymentMethod.ArAccountCode,
                Debit = AdvancePaymentLine.Value,
                AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
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
                   AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
                   AccountDesc = AccountHelper.GetAccountDesc(CustomerAccount.AccountID)
               };
                AdvancePaymentDistibution.Add(SalesInvoicePaymentDistibution);

            }

            return AdvancePaymentDistibution;
        }


        public  List<SalesInvoicePaymentDistibution> DistributeCreditSettelment(LineCreditSettlement Item, PaymentMethod PaymentMethod, int Customer, List<AccountMap> AccountMap)
        {
            AccountHelper AccountHelper = new AccountHelper();

            ExceedDbContext db= new ExceedDbContext();
            
            var SalesInvoicePaymentDistibutions = new List<SalesInvoicePaymentDistibution>();

 
            var SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
            {
                AccountCode = PaymentMethod.ArAccountCode,
                Debit = Item.SettelmentAmount,
                CashSalesInvoiceId = Item.InvoiceId,
                AccountDesc = AccountHelper.GetAccountDesc( PaymentMethod.ArAccountCode),

            };
            SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);


            var CustomerAccount = AccountMap.Where(x => x.Reference == Customer.ToString()).FirstOrDefault();

            if (CustomerAccount != null)
            {
                SalesInvoicePaymentDistibution = new SalesInvoicePaymentDistibution
               {
                   AccountCode = CustomerAccount.AccountID,
                   Credit = Item.SettelmentAmount,
                   CashSalesInvoiceId = Item.InvoiceId,
                   AccountDesc = AccountHelper.GetAccountDesc(CustomerAccount.AccountID)
               };
                SalesInvoicePaymentDistibutions.Add(SalesInvoicePaymentDistibution);

            }

            return SalesInvoicePaymentDistibutions;


        }

        public  List<SalesInvoicePaymentDistibution> DistributeProduct(CashSalesInvoice Invoice, ItemCategory ItemCategory, LineItem Item , List<AccountMap> AccountMap)
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



    }
}
