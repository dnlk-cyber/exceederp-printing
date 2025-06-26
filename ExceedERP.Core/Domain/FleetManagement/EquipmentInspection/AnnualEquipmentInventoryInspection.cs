using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public partial class AnnualEquipmentInventoryInspection : Operations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid AnnualEquipmentInventoryInspectionID { get; set; } 

        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }

        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }

        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }

        [DisplayName("Fleet")]
        public Guid FleetsID { get; set; }

        public string Make { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string YearOfManufacturing { get; set; }
        public string EngineModel { get; set; }
        public string EngineSerailNumber { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Operator { get; set; }
       
        [Required]
        [DisplayName("Inspected By")]
        public string InspectedBy { get; set; }
        
        [Required]
        [DisplayName("Date of Inspection")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofInspection { get; set; }
        
    }

    public class InspectionStatusKey: TrackUserSettingOperation
    {
        public int InspectionStatusKeyId { get; set; }
        public string StatusKey { get; set; }
        public string Remark { get; set; }
    }
    public partial class AnnualEquipmentInventoryInspectionVM
    {
        public Guid AnnualEquipmentInventoryInspectionID { get; set; }
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }

        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }

        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }

        [DisplayName("Fleet")]
        public Guid FleetsID { get; set; }

        [DisplayName("Inspected By")]
        public string InspectedBy { get; set; }
        
            [DisplayName("Date of Inspection")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateofInspection { get; set; }
        
    }

    
}