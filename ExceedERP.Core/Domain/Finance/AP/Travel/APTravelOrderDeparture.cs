using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.Travel
{
    public enum DepartureType
    {
        Departure,
        Return
    }
    public class ApTravelOrderDeparture : TrackUserOperation
    {
        public int ApTravelOrderDepartureId { get; set; }
        public int ApTravelOrderId { get; set; }
        public virtual ApTravelOrder TravelOrder { get; set; }
        [Display(Name = "Type")]
        public DepartureType Type { get; set; }
        [Display(Name = "From")]
        public string From { get; set; }
        [Display(Name = "To")]
        public string To { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Display(Name = "Mode of Transport")]
        public string TransportMode { get; set; }
        public string Remark { get; set; }
       
    }
}
