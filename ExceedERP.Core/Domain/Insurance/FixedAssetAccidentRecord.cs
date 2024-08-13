using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.Insurance
{
    public class FixedAssetAccidentRecord : TrackUserOperation
    {
        [Key]
        public int FixedAssetAccidentRecordId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_AssetRegistrationId", ResourceType = typeof(Localization.Resources))]
        public Guid FleetsID { get; set; }
        //public virtual Fleets Fleets { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_PlateNumber", ResourceType = typeof(Localization.Resources))]
        public string PlateNumber { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_Amount", ResourceType = typeof(Localization.Resources))]
        public double Amount { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_BranchId", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [StringLength(200)]
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}