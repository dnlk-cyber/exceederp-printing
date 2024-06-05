using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
   public  class RentalInspection : OperationWithThirdValidation
    {
        public int RentalInspectionId { get; set; }
       
        [DisplayName("Equipment Type")]
        [Required]
        public Guid EquipmentTypeID { get; set; }
        
        [DisplayName("Equipment Name")]
        public Guid? EquipmentNameID { get; set; }
        [Required]
        [DisplayName("Model")]
        public string Model { get; set; }
        [Required]
        [DisplayName("Chasis/Serial No.")]
        public string ChasisSerialNo { get; set; }
        [Required]
        [DisplayName("Engine Model")]
        public string EngineModel { get; set; }
        [Required]
        [DisplayName("Engine Serial No.")]
        public string EngineSerialNo { get; set; }
        [Required]
        [DisplayName("Year of Manufacture")]
        public string ManufacturingYear { get; set; }
        [Required]
        [DisplayName("Capacity")]
        public string Capacity { get; set; }
        public string Manufacturer { get; set; }
        [Required]
        [DisplayName("HP")]
        public decimal Hp { get; set; }
        [Required]
        [DisplayName("Operator's License No.")]
        public string OperatorLicenceNo { get; set; }
        [Required]
        [DisplayName("Lessor")]
        public string Lessor { get; set; }
        [Required]
        [DisplayName("Plate No.")]
        public string PlateNo { get; set; }
        [Required]
        [DisplayName("Branch")]

        public int BranchId { get; set; }
        [Required]
        [DisplayName("Hr./Km Meter Reading")]
        public string MeterReading { get; set; }
        public virtual EquipmentType EquipmentType { get; set; }
        public virtual EquipmentName EquipmentName { get; set; }

        public virtual Branch Branch { get; set; }
        [NotMapped]
        public int validationStatus { get; set; }
        [Display(Name ="CC/KW")]
        public string CCKW { get; set; }

    }
}
