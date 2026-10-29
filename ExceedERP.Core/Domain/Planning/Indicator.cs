using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Planning
{
  
    public class Indicator
    {
        public int IndicatorId { get; set; }
        [DisplayName("Indicator-Name")]
        public int GlobalIndicatorId { get; set; }
        [Display(Name= "Indicator")]
        public string Name { get; set; }
        public decimal? Weight { get; set; }
        [Display(Name = "Has Sub-Indicator")]
        public bool HasSubIndicator { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public int ParentId { get; set; }
        public string Description { get; set; }
        [UIHint("UoMTemplate")]
        public string Unit { get; set; }
        public KPIType KPIType { get; set; }
       
        public int ObjectiveId { get; set; }
        public Objective GetObjective { get; set; }


    }
    public enum KPIType
    {
        //Number
        Percentage,
        Quantity,
        Finance,
        Amount,
        Days,
        Hours
    }

   
}
