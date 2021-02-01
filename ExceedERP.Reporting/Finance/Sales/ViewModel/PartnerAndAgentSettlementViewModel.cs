using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.Sales.ViewModel
{
   

    public class PartnerAndAgentSettlementViewModel
{
        [Display(Name = "Partner ID.")]
        public string PartnerId { get; set; }
        [Display(Name = "Partner Name")]
        public string PartnerName { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        public List<TransactionVm> PartnerTransactionVms { get; set; }
    }
}
