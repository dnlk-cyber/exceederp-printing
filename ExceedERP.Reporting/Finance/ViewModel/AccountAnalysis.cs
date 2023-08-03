using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class AccountAnalysisViewModel
    {
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Effective Date")]
        public string EffectiveDate { get; set; }
        [Display(Name = "Journal Reference")]
        public string JournalReference { get; set; }
        [Display(Name = "Category")]
        public string Category { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDescription { get; set; }
        [Display(Name = "Debit")]
        public decimal Debit { get; set; }
        [Display(Name = "Credit")]
        public decimal Credit { get; set; }
        public string AdditionalDescription { get; set; }
        public string CreatedBy { get; set; }
        public string Remark { get; set; }
        public string ReferenceDoc { get; set; }//manual
        public string PaymentReference { get; set; }//receipt or payment reference
        public string InvoiceReferenceDoc { get; set; }//manual
        public string PostStatus { get; set; }
    }
}
