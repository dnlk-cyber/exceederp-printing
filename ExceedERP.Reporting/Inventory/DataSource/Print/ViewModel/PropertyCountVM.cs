using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    public class PropertyCountVm
    {
    }

    public class PropertyCountHeaderViewModel : Organization
    {
        public string VoucherId { get; set; }
        public string Branch { get; set; }
        public string Store { get; set; }
        public DateTime CountDate { get; set; }
    }

    public class PropertyCountLinesViewModel
    {
        public string ItemCategoryCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public decimal PhysicalCount { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public bool Status { get; set; }
    }
}
