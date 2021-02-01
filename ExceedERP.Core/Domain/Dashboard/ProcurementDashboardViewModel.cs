using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Dashboard
{
    public class ProcurementSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class TenderSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class ProformaSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }

    public class PurchaseorderSeriesData
    {
        public string Name { get; set; }
        public IEnumerable<decimal> Data { get; set; }
    }
}
