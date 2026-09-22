using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public class FleetBoloRegistration : TrackUserSettingOperation
    {
        [Key]
        public int FleetBoloRegistrationId { get; set; }
        public Guid EquipmentCategoryId { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public Guid EquipmentNameId { get; set; }
        [NotMapped]
        public bool AddToHistory { get; set; }

        [Display(Name ="Plate Number")]
        public Guid FleetId { get; set; }
        [DataType(DataType.Date), Display(Name ="Bolo Registration Date")]
        public DateTime BoloRegistrationDate { get; set; }
        [DataType(DataType.Date), Display(Name = "Bolo Expiration Date")]
        public DateTime BoloExpireDate { get; set; }
        public string Remark { get; set; }


    }
    public class FleetBoloRegistrationHistory : TrackUserSettingOperation
    {
        [Key]
        public int FleetBoloRegistrationHistoryId { get; set; }
        public int FleetBoloRegistrationId { get; set; }
        public Guid EquipmentCategoryId { get; set; }
        public Guid EquipmentTypeId { get; set; }
        public Guid EquipmentNameId { get; set; }

        [Display(Name = "Plate Number")]
        public Guid FleetId { get; set; }
        [DataType(DataType.Date), Display(Name = "Bolo Registration Date")]
        public DateTime BoloRegistrationDate { get; set; }
        [DataType(DataType.Date), Display(Name = "Bolo Expiration Date")]
        public DateTime BoloExpireDate { get; set; }
        public string Remark { get; set; }
    }
    public class BoloRegistrationDocument
    {
        public int BoloRegistrationDocumentId { get; set; }
        public int FleetBoloRegistrationId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
}
