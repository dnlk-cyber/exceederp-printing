using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Misc.ViewModel
{
    public class PostPerformanceModel
    {
        public string User { get; set; }
        public decimal Count { get; set; }

        public string Remark { get; set; }

    }

    public  class AuditTrailViewModel
        {
        public string AccountCode { get; set; }

        public string JournalName { get; set; }
        
        public string Description { get; set; }
      
        public string JournalNumber { get; set; }
         
        public string JournalReference { get; set; }
       
        public string CategoryName { get; set; }
       
        public string SourcesName { get; set; }
        
        public DateTime EffectiveDate { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string AdditionalDescription { get; set; } 
        public string Remark { get; set; }
        public string ReferenceDoc { get; set; }
        public string PaymentReference { get; set; } 
        public string InvoiceReferenceDoc { get; set; }
    }
}
