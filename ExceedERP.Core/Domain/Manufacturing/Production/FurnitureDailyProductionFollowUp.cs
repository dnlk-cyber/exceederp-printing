using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ExceedERP.Core.Domain.Manufacturing.Production
{
    public class FurnitureDailyProductionFollowUp : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureDailyProductionFollowUpId { get; set; }
        [Display(Name ="Work-Shop")]
        public WorkShop WorkShop { get; set; }
        [Display(Name ="Task Category")]
        public int ManifucturingTaskCategoryId { get; set; } 
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        [DataType(DataType.Date), Display(Name ="Date")]
        public DateTime? DateExcuted { get; set; }
        
        [Display(Name = "Daily Production Plan")]
        public decimal DailyProductionPlan { get; set; }
        [Display(Name = "Daily Production Actual")]
        public decimal DailyProductionActual { get; set; } 
        //Foregin-key FurnitureJobOrderProduction
        public int FurnitureJobOrderProductionId { get; set; }
    }
    public class FurnitureProductionDailyFollowUpDelayReason
    {

        [Key]
        public int FurnitureProductionDailyFollowUpDelayReasonId { get; set; }
        public ProductionDelayReason ProductionDelayReason { get; set; }
        public string Remark { get; set; }
        public int FurnitureDailyProductionFollowUpId { get; set; }
        public FurnitureDailyProductionFollowUp FurnitureDailyProductionFollowUp { get; set; }


    }
    public enum ProductionDelayReason
    {
        [Display(Name ="Waiting For Additional Material"), Description("Waiting For Additional Material")]
        Waiting_for_Material,
        [Display(Name = "Waiting For ManPower"), Description("Waiting For ManPower")]
        Waiting_For_ManPower,
        [Display(Name = "Waiting For Special Tool"), Description("Waiting For Special Tool")]
        Waiting_For_Special_Tool,
        [Display(Name = "Was Assigned On Other ProductionLine"), Description("Was Assigned On Other ProductionLine")]
        Was_Assigned_On_Other_ProductionLine,
        [Display(Name = "Other Reasons"), Description("Other Reasons")]
        Other_Reasons

    }
}
