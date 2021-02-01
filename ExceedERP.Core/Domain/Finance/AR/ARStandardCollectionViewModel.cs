using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArStandardCollectionViewModel : TrackUserOperation
    {
        public int ArStandardCollectionViewModelId { get; set; }
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }
        [Display(Name = "Installement")]
        public string InstallementNo { get; set; }
        [Display(Name = "Apply Amount")]
        public decimal ApplyAmount { get; set; }
        [Display(Name = "Balance Due")]
        public string BalanceDue { get; set; }
        [Display(Name = "Apply Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ApplyDate { get; set; }
    }
}
