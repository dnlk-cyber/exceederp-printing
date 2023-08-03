using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLJournalHeaderVm 
    {
        public int GlRecordJournalId { get; set; }
        [Required]
        [Display(Name = "Journal Name")]
        public string JournalName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
       
        [Required]
        [Display(Name = "Period")]
        public string PeriodName { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Sources")]
        public string SourcesName { get; set; }
        [Display(Name = "Journal Reference")]
        public string JournalNumber { get; set; }
        [Display(Name = "Effective Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }
        public string Posting { get; set; }
        public bool IsReversed { get; set; }
        public bool IsPosted { get; set; }
        
        public string PostedBy { get; set; }
        public DateTime? PostedTime { get; set; }
       
        public decimal DebitSum { get; set; }
        public string Remark { get; set; }
        //ap ,ar,or gl
        public string Source { get; set; }
        public string SourceId { get; set; }
        public bool IsVoidDoc { get; set; }//to record voided manual documents
        public string ReferenceDoc { get; set; }//manual
        public string PaymentReference { get; set; }//receipt or payment reference
        public string InvoiceReferenceDoc { get; set; }//manual
        public int BranchId { get; set; }
        public virtual List<GLJournalLineVm> GLJournalLineVms { get; set; }=new List<GLJournalLineVm>();    
       
    }

    public class GLJournalLineVm
    {
        public int GLJournalLineVmId { get; set; }
       
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        [Range(0.0, 9999999999, ErrorMessage = "Debit can't be negative value")]
        public decimal Debit { get; set; }
        [Range(0.0, 9999999999, ErrorMessage = "Credit can't be negative value")]
        public decimal Credit { get; set; }
        [Display(Name = "Additional Description")]
        public string AdditionalDescription { get; set; }       
    }
}
