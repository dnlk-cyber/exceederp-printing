using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class StockLevel : Operations
    {
        public int StockLevelID { get; set; }

        public string ThingType { get; set; }
        public string Thing { get; set; }
        [Required]
        [DisplayName("Store")]
        public string StoreID { get; set; }

        public double MinimumLevel { get; set; }


        public double MaximumLevel { get; set; }

        public double LeadTime { get; set; }


        public double EconomicOrderQuantity { get; set; }

        public string Remark { get; set; }

    }

}
