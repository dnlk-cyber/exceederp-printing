using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesReturn : Operations
    {
        [Key]
        public int SalesReturnId { get; set; }

        [Display(Name ="Customer")]
        public int CustomerId { get; set; }

        [Display(Name ="TIN")]
        public string CustomerTin { get; set; }
        
        [Display(Name ="Branch")]
        public int BranchId { get; set; }

        [Display(Name ="Period")]
        public int CRMPeriodId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime Date { get; set; }

		[Required]
        public string Reason { get; set; }

		[Display(Name = "Total Amount")]
		public decimal TotalAmount { get; set; }
		[Display(Name = "Remaining")]
		public decimal AmountDue { get; set; }
		[Display(Name = "Is Paid?")]
		public bool Paid { get; set; }

		public int BusinessUnitId { get; set; }
	}
}
