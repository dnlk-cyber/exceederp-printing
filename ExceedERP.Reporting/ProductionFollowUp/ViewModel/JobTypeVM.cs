using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
    public class JobTypeVM : Organization
    {
        public string From { get; set; }
        public string To { get; set; }
        public string JobCode { get; set; }
        public List<JobTypeItemVM> JobTypeItemVMs { get; set; }


    }
    public class JobTypeItemVM
    {
        public string JobNo { get; set; }       
        public string JobType { get; set; }       
        public string Client { get; set; }       
        public decimal Quantity { get; set; }       
        public string Copy { get; set; }       
        public string Size { get; set; }       
        public decimal TotalPrice { get; set; }       
        public string Code { get; set; }       
        public string Status { get; set; }       
    }
}
