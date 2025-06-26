using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class PmSetting:TrackUserSettingOperation
    {
        public int PmSettingId {get;set;}
        public string PmName { get; set; }
        public string Description { get; set; }
    }

    public class PmScheduler: TrackUserSettingOperation
    {
        public int PmSchedulerId { get; set; }
        public string EquipmentNameId { get; set; }
        public string PlateNumber { get; set; }
        public string Location { get; set; }
        public string TaskId { get; set; }
        public int PmSettingId { get; set; }
        public int DateFrequency { get; set; }
        public decimal KMFrequency { get; set; }
        [DataType(DataType.Date), Display(Name = "Last PM Date")]
        public DateTime LastPmDate { get; set; }
        public decimal LastPmKM { get; set; }
        [DataType(DataType.Date), Display(Name = "Next PM Date")]
        public DateTime NextPmDate { get; set; }
        public decimal NextPmKM { get; set; }
        public bool IsExpired { get; set; }
        public bool WorkDelay { get; set; }
        public string DelayReason { get; set; }
    }
    public class SOPDocument
    {
        public int SOPDocumentId { get; set; }
        public int PmSchedulerId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }

    }
}
