using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
    public class ItemDurationBase : TrackUserOperation
    {
        [Display(Name = "Productivity /Crew /Day")]
        public decimal? ProductivityOfCrewPerDay { get; set; }
        [Display(Name = "Duration /Crew")]
        public decimal? DurationPerCrew { get; set; }
        [Display(Name = "No. of Crews")]
        public int? NumberOfCrew { get; set; }
        public decimal? EstimatedDuration { get; set; }
    }
}
