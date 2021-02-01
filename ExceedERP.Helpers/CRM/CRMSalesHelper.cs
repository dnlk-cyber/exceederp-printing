using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
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

                       

                        if ( TotalAmount > 0)
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

                        var Amount =  Line.Value - TotalAmount ;
                  
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
        public void ConfigureWitholding(bool WithholdTax, string WithholdingType, List<GLTaxRate> Rate, out decimal WithHoldingAmount, out decimal WithHoldingTaxAmount, decimal TaxAmount, decimal TotalAmount)
            {
                var GLWithholdingTaxRate = Rate.Where(t => t.GLTaxRateID.ToString() == WithholdingType).FirstOrDefault();
                WithHoldingAmount = (TotalAmount - (TotalAmount * (GLWithholdingTaxRate.Rate / 100)));

                WithHoldingTaxAmount = decimal.Zero;
                if (WithholdTax)
                {
                    WithHoldingTaxAmount = TaxAmount;
                }
            }
        }

    }
