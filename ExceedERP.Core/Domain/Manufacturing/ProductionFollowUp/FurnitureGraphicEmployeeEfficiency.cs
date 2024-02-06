using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp
{
    public class FurnitureGraphicEmployeeEfficiency
    {
        public int FurnitureGraphicEmployeeEfficiencyID { get; set; }

        [Display(Name = "Date Of Production")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfProduction { get; set; }


        [Display(Name = "Code Of Time")]
        public FurnitureCodeOfTime CodeOfTime { get; set; }
        [Display(Name = "Job Order Number")]
        public int FurnitureJobID { get; set; }


        [Display(Name = "Completed Type Job")]
        public FurnitureCompletedJobType CompletedTypeJob { get; set; }

        [Required]
        [Display(Name = "Furniture Employee ID")]
        public string FurnitureEmployeeID { get; set; }

        [Required]
        [Display(Name = "Started Hours")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime StartedHours { get; set; }

        [Required]
        [Display(Name = "Completed Hours")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime CompletedHours { get; set; }

        [Required]
        [Display(Name = "Consumed Hours")]
        public string ConsumedHours { get; set; }


        [Display(Name = "Cost Center")]
        public FurnitureCost_Center3 CostCenter { get; set; }

        [Display(Name = "Approved Absence Hrs")]
        public double HoursSpend1 { get; set; }
        [Display(Name = "Unapproved Absence Hrs")]
        public double HoursSpend2 { get; set; }
        [Display(Name = "Permisson With out SaleryHrs")]
        public double HoursSpend3 { get; set; }
        [Display(Name = "Make Ready Hrs")]
        public double HoursSpend6 { get; set; }
        [Display(Name = "Correction Hrs")]
        public double HoursSpend7 { get; set; }
        [Display(Name = "Setup Time Hrs")]
        public double HoursSpend8 { get; set; }
        [Display(Name = "Power Failur Hrs")]
        public double HoursSpend9 { get; set; }
        [Display(Name = "Waiting For Job Hrs")]
        public double HoursSpend10 { get; set; }
        [Display(Name = "Waiting For Material Hrs")]
        public double HoursSpend11 { get; set; }
        [Display(Name = "Waiting For Plate Hrs")]
        public double HoursSpend12 { get; set; }
        [Display(Name = "Maching Bareackage  Hrs")]
        public double HoursSpend13 { get; set; }
        [Display(Name = "Planned Maintenance Hrs")]
        public double HoursSpend14 { get; set; }
        [Display(Name = "Waiting For Proof Hrs")]
        public double HoursSpend17 { get; set; }
        [Display(Name = "With out job Order Hrs")]
        public double HoursSpend18 { get; set; }
        [Display(Name = "Other Reason Hrs")]
        public double HoursSpend19 { get; set; }
    }

}
