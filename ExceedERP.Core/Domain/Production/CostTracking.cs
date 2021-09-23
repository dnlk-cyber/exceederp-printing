using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Production
{
    //
    //  Summary :
    //      Used to map the monthly costs of production projects to
    //      production periods
    public class CostTrackingPeriod : TrackUserOperation
    {
        [Key]
        public int CostTrackingPeriodId { get; set; }

        [Display(Name = "Period")]
        public int ProductionPeriodId { get; set; }

        [Display(Name ="Project")]
        public int ProductionProjectId { get; set; }

        public string Remark { get; set; }
        public virtual ProductionPeriod Period { get; set; }
        public virtual ProductionProject Project { get; set; }

    }
    public class ProjectCostByMonth : TrackUserOperation
    {
        [Key]
        public int ProjectCostByMonthId { get; set; }

        [Display(Name ="Period")]
        public int CostTrackingPeriodId { get; set; }

        [Display(Name ="Crop")]
        public int CropId { get; set; }

        [Display(Name = "Seed Type")]
        public CropType Type { get; set; }

        [Display(Name = "Seed Generation")]
        public CropGeneration Generation { get; set; }

        public decimal MaterialCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal TotalCost { get; set; }

        public virtual CostTrackingPeriod Period { get; set; }
        public virtual Crop Crop { get; set; }
    }
    // 
    //  Summary :
    //      used to to store data about the detail of 
    //      cost by month, crop and generation for
    //      machinery operation and labor operaions
    public class ProjectCostByMonthDetail : TrackUserOperation
    {
        [Key]
        public int ProjectCostByMonthDetailId { get; set; }
        public int ProjectCostByMonthId { get; set; }

        public string ActivityName { get; set; }
        public OperationType OperationType { get; set; }
        //public decimal MaterialCost { get; set; }
        public decimal  LaborCost { get; set; }
        public decimal EquipmentCost { get; set; }
        public decimal TotalCost { get; set; }

        public virtual ProjectCostByMonth ProjectCostByMonth { get; set; }
    }

    public class ProjectCostByMonthMaterial : TrackUserOperation
    {
        [Key]
        public int ProjectCostByMonthMaterialId { get; set; }
        public int ProjectCostByMonthId { get; set; }

        public string MaterialName { get; set; }
        public string Unit { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal TotalCost { get; set; }

        public virtual ProjectCostByMonth ProjectCostByMonth { get; set; }
    }
}
