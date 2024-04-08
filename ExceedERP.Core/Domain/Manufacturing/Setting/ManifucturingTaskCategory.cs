using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Setting
{
   
    public class ManifucturingTaskCategory
    {
        public int ManifucturingTaskCategoryId { get; set; }
        public WorkShop Type { get; set; }
        public string Name { get; set; }
        [Display(Name="Standard Hour")]
        public decimal StandardHour { get; set; }
        [Display(Name="Hour Rate")]
        public decimal HourRate { get; set; }

    }
    public enum WorkShop
    {
        
        Metal = 1,
        WoodWork,
        [Display(Name = "Finishing/Assembly")]
        Finishing_Assembly        
    }
}
