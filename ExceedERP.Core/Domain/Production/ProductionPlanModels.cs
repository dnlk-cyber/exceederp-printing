using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    public class SeedProductionPlan : TrackUserOperation
    {
        [Key]
        public int SeedProductionPlanId { get; set; }

        [Display(Name ="Year")]
        public int SeedProductionPlanYearId { get; set; }

        [Display(Name ="Crop/Seed")]
        public int CropId { get; set; }

        [Display(Name ="Seed Type")]
        public CropType Type { get; set; }

        [Display(Name ="Seed Generation")]
        public CropGeneration Generation { get; set; }

        [Display(Name ="Area(ha)")]
        public decimal Area { get; set; }

        [Display(Name ="Yield(Qt/ha)")]
        public decimal YieldQuantity { get; set; }

        [Display(Name ="Total Production(Qts)")]
        public decimal TotalQuantity { get; set; }

        public virtual Crop Crop { get; set; }
        public virtual SeedProductionPlanYear FiscalYear { get; set; }
    }

    public class SeedProductionPlanYear : TrackUserOperation
    {
        [Key]
        public int SeedProductionPlanYearId { get; set; }

        [Display(Name = "Year")]
        public int GlFiscalYearId { get; set; }

        [Display(Name ="Project")]
        public int ProductionProjectId { get; set; }


        public string Remark { get; set; }
        public virtual  GlFiscalYear FiscalYear { get; set; }
        public virtual ProductionProject Project { get; set; }
    }
}
