using System.Collections.Generic;

namespace ExceedERP.Core.Domain.Dashboard
{
    public class ReceivableSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class PayableSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class SupplierSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class CashAndBankSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class IncomeExpenseSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ApInvoiceStatusSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ApPaymentStatusSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ArInvoiceStatusSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ArCollectionStatusSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class GlDocumentStatusSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class RecentPaymentSeriesData
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }
}
