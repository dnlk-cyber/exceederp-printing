using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    //public enum CRMTenderStatus
    //{
    //    Progress,
    //    Won,
    //    AdvancePayment,
    //}
    public class TenderFollowup : TrackUserSettingOperation
    {
        [Key]
        public int TenderFollowupId { get; set; }
        [Display(Name ="Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Bid Bond Amount")]
        public decimal BidBondAmount { get; set; }
        [Display(Name = "Date Anounce + bid validty")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateAnnounceBidValidatiy { get; set; }
        [Display(Name = "Service Charge")]
        public decimal? ServiceCharge { get; set; }

        [Display(Name = "Closing Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime ClosingDate { get; set; }

        [Display(Name = "Opening Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime OpeningDate { get; set; }
        public string Location { get; set; }

        [Display(Name = "Count Down Till Submission")]
        [NotMapped]
        public string CountDownTillSubmission { get; set; }

        [Display(Name = "Document Preparation Start Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DocumentPreparationStartDate { get; set; }

        public string Status { get; set; }

        [Display(Name = "Tender Result")]
        public string TenderResult { get; set; }
        public string Remark { get; set; }
        [Display(Name = "Latest Letter")]
        public string LatestLetter { get; set; }
        [Display(Name = "Contact Person")]
        public string ContactPerson { get; set; }

        [Display(Name = "Bid Amount")]
        public decimal BidAmount { get; set; }
        [Display(Name = "Bid Security Charge")]
        public decimal BidSecurityCharge { get; set; }
        [Display(Name = "Bid Validty")]
        public decimal BidValidty { get; set; }

        [Display(Name = "Reason of Not Participating")]
        public string ReasonOfNotParticipating { get; set; }
        [Display(Name ="Reason of Losing")]
        public string ReasonOfLosing { get; set; }
    }
    public class TenderFollowupLineItem : TrackUserSettingOperation
    {
        [Key]
        public int TenderFollowupLineItemId { get; set; }
        public int TenderFollowupId { get; set; }
        public virtual TenderFollowup TenderFollowup { get; set; }
        [Display(Name = "Category")]
        public string ItemCategoryCode { get; set; }
        [Display(Name = "Product")]
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }

        [NotMapped]
        [Display(Name ="Category")]
        public string CategoryName { get; set; }
        [NotMapped]
        [Display(Name ="Product")]
        public string ItemName { get; set; }

        //
        //  this flag indicates that the given product is not in system
        [Display(Name ="Add product not registered in system")]
        public bool IsExternalProduct { get; set; }
        //
        //  for item not registered in system
        public string Category { get; set; }
        public string Product { get; set; }
        //
    }

}
