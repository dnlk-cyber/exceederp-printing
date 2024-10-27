using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class JournalTransactionVM
    {
        public string Id { get; set; }
        public string Description { get; set; }
        //
        [Display(Name = "Fiscal Year")]
        public string FiscalYear { get; set; }
        
        [Display(Name = "Period")]
        public string PeriodName { get; set; }
        
        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Sources")]
        public string SourcesName { get; set; }
        [Display(Name = "Journal Reference")]
        public string Reference { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public virtual ICollection<JournalTransactionLineVm> JournalTransactionLineVms { get; set; }=new List<JournalTransactionLineVm>();
    }

    public class JournalTransactionLineVm
    {
        [Display(Name = "Line Id")]
        public string LineId { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Description")]
        public string AccountDesc { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        [Display(Name = "Additional Description")]
        public string AdditionalDescription { get; set; }
    }
}
