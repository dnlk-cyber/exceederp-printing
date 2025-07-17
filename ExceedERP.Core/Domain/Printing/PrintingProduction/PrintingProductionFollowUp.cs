using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Printing.PrintingProduction
{
    
    public class PrintingProductionFollowUp : PrintingCommon
    {
        [Key]
        public int PrintingProductionFollowUpId { get; set; }
        [Display(Name ="Job No.")]
        public int PrintingJobOrderItemId { get; set; } // Eg. 102
        [Display(Name = "Customer")]
        public int CustomerId { get; set; } // Eg. Education Minster
        public PrintJobType PrintJobType { get; set; } // Eg. Book
        public BindingStyle BindingStyle { get; set; } // Eg. Perfect
        [Display(Name = "Paper for text")]
        public int TextPaperSizeId { get; set; } // Eg. A4
        public int CoverPaperSizeId { get; set; } // Eg. A3
        public int TextNoOfColor { get; set; }
        public int CoverNoOfColor { get; set; }
        public decimal Quantity { get; set; } // Eg. 10000
        public int Pages { get; set; } // Eg. 200
        public string JobDescription { get; set; } // Eg. Grade 12 Physics Text Book
        
    }
    public class PrintingProductionFollowUpTask : Operations
    {
        [Key]
        public int PrintingProductionFollowUpTaskId { get; set; }
        [Display(Name = "Process Category")]
        public PressCategory PressCategory { get; set; } // Eg. Pre-Press
        [Display(Name = "Activity Type")]
        public LabourActivity LabourActivity { get; set; } // Eg. Plate Making
        [Display(Name = "Machine Type")]
        public int? MachineTypeId { get; set; }
        [Display(Name ="Assigned To")]
        public int OperatorId { get; set; } // Eg. Abebe
        public string UoM { get; set; } // Eg. Plate
        public decimal PlannedOutput { get; set; } // Eg. 100
        public decimal EstimatedHours { get; set; } // Eg. 8 hrs 
        public string Remark { get; set; }

        // Fk
        public int PrintingProductionFollowUpId { get; set; }

    }
    public class PrintingProductionDailyFollowUpDetail : TrackUserSettingOperation
    {
        [Key]
        public int PrintingProductionDailyFollowUpDetailId { get; set; } 
        
        [Display(Name ="Operated By")]  
        public int OperatorId { get; set; } // Eg. Abebe
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime ProductionStartTime { get; set; } // Eg. 5/13/2025 7:10 PM
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]  
        public DateTime ProductionEndTime { get; set; }  // Eg. 5/13/2025 10:00 PM
        public string UoM { get; set; } // Eg. Plate
      
        public decimal ActualOutput { get; set; } // Eg. 80 
        public double TimeSpentHours { get; set; } // Eg. 3 hrs
        public string Remark { get; set; } 

        //Fk
        public int PrintingProductionFollowUpTaskId { get; set; }

    }
    public class PrintingDailyFollowUpDelayReason : TrackUserOperation
    {
        [Key]
        public int PrintingDailyFollowUpDelayReasonId { get; set; } 
        public string Reason { get; set; }
        public string Description { get; set; }
        //fk
        public int PrintingProductionDailyFollowUpDetailId { get; set; }
    }

    public class PrintingProductionAllottedRawMaterial : TrackUserOperation
    {
        [Key]
        public int PrintingProductionAllottedRawMaterialId { get; set; }
        [Display(Name = "Material Type")]
        public EstimationMaterialType EstimationMaterialType { get; set; }
        [Display(Name = "Item Category")]
        public int PrintingMaterialCategoryId { get; set; }
        [Display(Name = "Item Name")]
        public int PrintingMaterialCategoryItemId { get; set; }
        public string UnitOfMeasurment { get; set; }
        public decimal Quantity { get; set; }
       
        //FK
        public int PrintingProductionFollowUpId { get; set; }

    }

}
