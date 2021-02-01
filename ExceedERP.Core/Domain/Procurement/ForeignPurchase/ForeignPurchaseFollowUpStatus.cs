using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.ForeignPurchase
{
    public class ForeignPurchaseFollowUpStatus : TrackUserSettingOperation
    {
        public int ForeignPurchaseFollowUpStatusId { get; set; }
        public int ForeignPurchaseId { get; set; }
        public virtual ForeignPurchase ForeignPurchase { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}