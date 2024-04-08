using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Manufacturing.Production
{
    public class FurnitureJobAssignment : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureJobAssignmentId { get; set; }   
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; } 
        [Display(Name = "Type")]
        public WorkShop Type { get; set; }
        [Display(Name = "Task Category")]
        public int TaskCategoryId { get; set; }               
        [Display(Name = "Time Spent")]
        public decimal TimeSpent { get; set; }
        [Display(Name ="Task Assigned Date"), DataType(DataType.Date)]
        public DateTime TaskAssignedDate { get; set; }
        [Display(Name = "Task End Date"), DataType(DataType.Date)]
        public DateTime TaskEndDate { get; set; }
        [Display(Name ="Daily Production Plan")]
        public decimal DailyProductionPlan { get; set; }
        public int FurnitureJobOrderProductionId { get; set; } 
        public FurnitureJobOrderProduction FurnitureJobOrderProduction { get; set; } 
    }
}
