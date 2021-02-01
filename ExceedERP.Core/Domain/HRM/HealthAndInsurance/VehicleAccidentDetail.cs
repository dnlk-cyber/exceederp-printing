using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class VehicleAccidentDetail
    {
        [Key]
        public int VehicleAccidentDetailId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_VehicleAccidentDetail_AccidentType", ResourceType = typeof(Localization.Resources))]
        public AccidentType AccidentType { get; set; }
        [Display(Name = "HR_HealthAndInsurance_VehicleAccidentDetail_IsThirdPartAccident", ResourceType = typeof(Localization.Resources))]
        public bool IsThirdPartAccident { get; set; }
        [Display(Name = "HR_HealthAndInsurance_VehicleAccidentDetail_AccidentLevel", ResourceType = typeof(Localization.Resources))]
        public AccidentLevel AccidentLevel { get; set; }
        public int FixedAssetAccidentRecordId { get; set; }
        public virtual FixedAssetAccidentRecord FixedAssetAccidentRecord { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        [StringLength(200)]
        public string Remark { get; set; }
    }
}