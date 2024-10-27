using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
   
   
    public class APTravelClaimDeparture : TrackUserOperation
    {
        public int APTravelClaimDepartureId { get; set; }
        public int ApTravelExpenseClaimId { get; set; }
        public virtual ApTravelExpenseClaim ApTravelExpenseClaim { get; set; }
        [Display(Name = "Type")]
        public DepartureType Type { get; set; }
        [Display(Name = "From")]
        public string From { get; set; }
        [Display(Name = "To")]
        public string To { get; set; }
        [Display(Name = "Departure Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Arrival Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd :hh mm}", ApplyFormatInEditMode = true)]
        public DateTime ArrivalDate { get; set; }

        [Display(Name = "Mode of Transport")]        
        public string TransportMode { get; set; }
        [Display(Name = "Destination Branch/Projectt")]
        public int? DestinationBranchId { get; set; }
        public decimal Duration { get; set; }
        public string Remark { get; set; }
       
    }

}
