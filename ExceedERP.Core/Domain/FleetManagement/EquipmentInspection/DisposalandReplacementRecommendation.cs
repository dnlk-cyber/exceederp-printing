
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public partial class DisposalandReplacementRecommendation : Operations
    {
        [ScaffoldColumn(false)]
        public Guid DisposalandReplacementRecommendationID { get; set; }
        [Required]
        [DisplayName("Fleet")]
        public Guid FleetsID { get; set; }
        [Required]
        public string RecommendedBy { get; set; }
        [Required]
        public string Status { get; set; }
        public enum Statusenum { Disposed, ToBeDisposed, ToBeReplaced }
        [Required]
        [DisplayName("Year of Replacement")]
        public string YearofReplacement { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }
        public string ApprovedBy { get; set; }
        [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }

    }
}