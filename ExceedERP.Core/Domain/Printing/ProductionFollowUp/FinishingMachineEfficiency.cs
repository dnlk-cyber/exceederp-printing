//using ExceedERP.Core.Domain.Printing.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.ProductionFollowUp
{
    public class FinishingMachineEfficiency
    {
        public int FinishingMachineEfficiencyID { get; set; }

        [Display(Name = "Date Of Production")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfProduction { get; set; }


        [Display(Name = "Code Of Time")]
        public CodeOfTime CodeOfTime { get; set; }

        [StringLength(50)]
        [Display(Name = "Job Order Number")]
        public string JobOrderNumber { get; set; }
        //rename to job order number
        [Display(Name = "Job Order Number")]
        public int JobID { get; set; }

        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }


        [Display(Name = "Type Job")]
        public Job_Type TypeJob { get; set; }

        [StringLength(50)]
        [Display(Name = "Sub Job Type")]
        public string SubTypeJob { get; set; }


        [Display(Name = "Cost Center")]
        public Cost_Center CostCenter { get; set; }

        [Required]
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }

        [StringLength(50)]
        [Display(Name = "Machine Code")]
        public string MachineCode { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime StartedHours { get; set; }

        [Display(Name = "Completed Hours")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:t}", ApplyFormatInEditMode = true)]
        public DateTime CompletedHours { get; set; }

        [Display(Name = "Consumed Hours")]
        public String ConsumedHours { get; set; }

        [Display(Name = "Completed Quantity")]
        public int CompletedQuantity { get; set; }

        [StringLength(50)]
        [Display(Name = "Completed Type Job")]
        public string CompletedTypeJob { get; set; }

        [StringLength(50)]
        [Display(Name = "Completed Size")]
        public string CompletedSize { get; set; }

        [Display(Name = "Standard Quantity")]
        public int StandardQuantity { get; set; }

        [Display(Name = "Expected Quantity")]
        public double ExpectedQuantity { get; set; }
        [Display(Name = "Variation With Expected Quantity")]
        public double VariationWithExpectedQuantity { get; set; }
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
