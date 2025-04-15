using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.JobCosting.ViewModel
{
   public class JobCostingVM
    {
        public int JobCostId { get; set; }
        public int JobId { get; set; }
        public string JobCategory { get; set; }
        public int  JobCategoryId { get; set; }
        public int  CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string BrandName { get; set; }

        public string JobOrder { get; set; }

        public DateTime JobReceivedDate { get; set; }
        public DateTime   Date { get; set; }
        public string  ReciptNo { get; set; }
        public decimal  Quantity { get; set; }
        public decimal  SellingPrice { get; set; }
        public decimal  OrderSize { get; set; }
        public bool  Void { get; set; }
        public string  Status { get; set; }
        public string  Description { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime?  LastModified { get; set; }
        public  string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
