using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Web.Areas.FleetManagement.Models
{
    public class RentalInspectionViewModel
    {
        public int RentalInspectionId { get; set; }
        [DisplayName("Equipment Type")]
        [Required]
        public Guid EquipmentTypeID { get; set; }
        [DisplayName("Equipment Make")]
        public string EquipmentMake { get; set; }
        [DisplayName("Model")]
        public string Model { get; set; }
        [DisplayName("Chasis/Serial No.")]
        public string ChasisSerialNo { get; set; }
        [DisplayName("Engine Model")]
        public string EngineModel { get; set; }
        [DisplayName("Engine Serial No.")]
        public string EngineSerialNo { get; set; }
        [DisplayName("Year of Manufacture")]
        public string ManufacturingYear { get; set; }
        [DisplayName("Capacity")]
        public string Capacity { get; set; }
        [DisplayName("Plate No.")]
        public string PlateNo { get; set; }
   
        public bool IsRegistered { get; set; }
    }
}