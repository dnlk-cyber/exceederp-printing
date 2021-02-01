using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class AccidentViewModel
    {
        [DisplayName("Plate Number")]
        public string PlateNumber { get; set; }

        [ScaffoldColumn(false)]
        public Guid AccidentID { get; set; }
        public Guid FleetsID { get; set; }
        [DisplayName("Date Of Occurrence")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> DateOfOccurrence { get; set; }
        [DisplayName("Cause Of Accident")]
        [Required]
        public string CauseOfAccident { get; set; }
        [DisplayName(" Level Of Damage")]
        [Required]
        public string LevelOfDamage { get; set; }
        public enum LevelOfDamageenum { Minor, Major, TotalDamage }
        [DisplayName(" Accountable body")]
        [Required]
        public string Accountablebody { get; set; }
        [DisplayName("Accident Location")]
        [Required]
        public string AccidentLocation { get; set; }
        [DisplayName("License No ")]
        [Required]
        public string LicenseNo { get; set; }
        public string Policereport { get; set; }
        public string Selectreport { get; set; }
        public string Description { get; set; }
        [DisplayName("Cost Covered by Insurance")]
        [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyInsurance { get; set; }
        [DisplayName("Cost Covered by Company")]
        [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyCompany { get; set; }
        [DisplayName("Cost Covered by Operator")]
        [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyOperator { get; set; }
        [DisplayName("Cost Covered by Other Bodies ")]
        [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyOtherBodies { get; set; }
    }
}