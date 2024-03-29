using ExceedERP.Core.Domain.Manufacturing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Production
{
    public class FurnitureJobAssignment
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
        [Display(Name ="Task Assigned Date"), DataType(DataType.DateTime)]
        public DateTime TaskAssignedDate { get; set; }
        [Display(Name = "Task End Date"), DataType(DataType.DateTime)]
        public DateTime TaskEndDate { get; set; }

        public int FurnitureJobOrderProductionId { get; set; } 
        public FurnitureJobOrderProduction FurnitureJobOrderProduction { get; set; } 
    }
}
