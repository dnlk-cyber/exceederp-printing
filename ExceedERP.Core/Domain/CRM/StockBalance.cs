using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class StockBalance : Operations
    {
        public int StockBalanceID { get; set; }
        [Required]
        public string StockBalanceType { get; set; }

        public string ThingType { get; set; }

        public string Variety { get; set; }
        public string Thing { get; set; }
        public string Class { get; set; }
        [Required]
        [DisplayName("Store")]
        public string StoreID { get; set; }

        [Required]
        public decimal Quantity { get; set; }
        [Required]
        public string Period { get; set; }


        public string Remark { get; set; }


    }

}
