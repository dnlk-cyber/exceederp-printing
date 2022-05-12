using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class JobSupplyVM :Organization
    { 
            public string From { get; set; }
            public string To { get; set; }
            public string TotalQuantity { get; set; }
            public string TotalJob { get; set; }
            public string Proforma { get; set; }
            public string DirectOrder { get; set; }
            public List<JobSupplyItemVM> JobSupplyItemVMs { get; set; }

        }
    public class JobSupplyItemVM
    {
        public string JobType { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalQuantity { get; set; }

        public string Code { get; set; }
    }
    }

