using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class ServiceUsageViewModel : Organization
    {
        public int ServiceUsageId { get; set; }

        public string Type { get; set; }
        public string ServiceName { get; set; }
        public string Customer { get; set; }
        public string PaymentMethod { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string ReceiveMethod { get; set; }

        public string Remark { get; set; }

        public decimal GrandTotal { get; set; }

    }
}
