using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExceedERP.Core.Domain.printing.JobCosting
{
    public class IdleTime
    {
        public int IdleTimeID { get; set; }
        [Required]
        public int GlFiscalYearId { get; set; }
        [Required]
        public int GLPeriodId { get; set; }
        public virtual ICollection<IdleTimeOT> IdleTimeOT { get; set; }
        public virtual ICollection<IdleTimeProductive> IdleTimeProductive { get; set; }
        public virtual ICollection<IdleTimeNonProductiveControllable> IdleTimeNonProductiveControllable { get; set; }
        public virtual ICollection<PrintingIdleTimeNonProductiveNonControllable> PrintingIdleTimeNonProductiveNonControllables { get; set; } 
    }

    public class IdleTimeOT
    {
        public int IdleTimeOTID { get; set; }
        public int IdleTimeID { get; set; }
        public int CostCenterId { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        //public virtual IdleTime IdleTime { get; set; } 
        //[Display(Name = "Manual")]
        //public decimal? OTManualHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTManualPercentage { get; set; }
        //[Display(Name = "Mechanical Composition")]
        //public decimal? OTMechanicalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTMechanicalPercentage { get; set; }
        //[Display(Name = "Camera")]
        //public decimal? OTCameraHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTCameraPercentage { get; set; }
        //[Display(Name = "Offset")]
        //public decimal? OTOffsetHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTOffsetPercentage { get; set; }
        //[Display(Name = "Letter Press")]
        //public decimal? OTLetterPressHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTLetterPressPercentage { get; set; }
        //[Display(Name = "Binding")]
        //public decimal? OTBindingHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTBindingPercentage { get; set; }
        //[Display(Name = "Computer")]
        //public decimal? OTComputerHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTComputerPercentage { get; set; }
        //[Display(Name = "Web Offset")]
        //public decimal? OTWebHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTWebPercentage { get; set; }
        //[Display(Name = "Total")]
        //public decimal? OTTotalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal OTTotalPercentage { get; set; }
    }
    public class IdleTimeProductive
    {
        public int IdleTimeProductiveID { get; set; }
        public int IdleTimeID { get; set; }
        public virtual IdleTime IdleTime { get; set; }
        public int CostCenterId { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        //[Display(Name = "Manual")]
        //public decimal? ProductiveManualHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveManualPercentage { get; set; }
        //[Display(Name = "Mechanical Composition")]
        //public decimal? ProductiveMechanicalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveMechanicalPercentage { get; set; }
        //[Display(Name = "Camera")]
        //public decimal? ProductiveCameraHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveCameraPercentage { get; set; }
        //[Display(Name = "Offset")]
        //public decimal? ProductiveOffsetHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveOffsetPercentage { get; set; }
        //[Display(Name = "Letter Press")]
        //public decimal? ProductiveLetterPressHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveLetterPressPercentage { get; set; }
        //[Display(Name = "Binding")]
        //public decimal? ProductiveBindingHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveBindingPercentage { get; set; }
        //[Display(Name = "Computer")]
        //public decimal? ProductiveComputerHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveComputerPercentage { get; set; }
        //[Display(Name = "Web Offset")]
        //public decimal? ProductiveWebHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveWebPercentage { get; set; }
        //[Display(Name = "Total")]
        //public decimal? ProductiveTotalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal ProductiveTotalPercentage { get; set; }
    }
    public class IdleTimeNonProductiveControllable
    {
        public int IdleTimeNonProductiveControllableID { get; set; }
        public int IdleTimeID { get; set; }
        public virtual IdleTime IdleTime { get; set; }
        [Required]
        [Display(Name = "Idle Time Description")]
        public string IdleTimeCategoryId { get; set; }
        public int CostCenterId { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        //[Display(Name = "Manual")]
        //public decimal? NPCManualHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCManualPercentage { get; set; }
        //[Display(Name = "Mechanical Composition")]
        //public decimal? NPCMechanicalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCMechanicalPercentage { get; set; }
        //[Display(Name = "Camera")]
        //public decimal? NPCCameraHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCCameraPercentage { get; set; }
        //[Display(Name = "Offset")]
        //public decimal? NPCOffsetHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCOffsetPercentage { get; set; }
        //[Display(Name = "Letter Press")]
        //public decimal? NPCLetterPressHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCLetterPressPercentage { get; set; }
        //[Display(Name = "Binding")]
        //public decimal? NPCBindingHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCBindingPercentage { get; set; }
        //[Display(Name = "Computer")]
        //public decimal? NPCComputerHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCComputerPercentage { get; set; }
        //[Display(Name = "Web Offset")]
        //public decimal? NPCWebHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCWebPercentage { get; set; }
        //[Display(Name = "Total")]
        //public decimal? NPCTotalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPCTotalPercentage { get; set; }
    }
    public class PrintingIdleTimeNonProductiveNonControllable
    {
        [Key]
        public int PrintingIdleTimeNonProductiveNonControllableID { get; set; }
        public int IdleTimeID { get; set; }
        public virtual IdleTime IdleTime { get; set; }
        [Required]
        [Display(Name = "Idle Time Description")]
        public string IdleTimeCategoryId { get; set; }
        public int CostCenterId { get; set; }
        public decimal Rate { get; set; }
        public decimal Value { get; set; }
        //[Display(Name = "Manual")]
        //public decimal? NPNCManualHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCManualPercentage { get; set; }
        //[Display(Name = "Mechanical Composition")]
        //public decimal? NPNCMechanicalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCMechanicalPercentage { get; set; }
        //[Display(Name = "Camera")]
        //public decimal? NPNCCameraHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCCameraPercentage { get; set; }
        //[Display(Name = "Offset")]
        //public decimal? NPNCOffsetHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCOffsetPercentage { get; set; }
        //[Display(Name = "Letter Press")]
        //public decimal? NPNCLetterPressHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCLetterPressPercentage { get; set; }
        //[Display(Name = "Binding")]
        //public decimal? NPNCBindingHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCBindingPercentage { get; set; }
        //[Display(Name = "Computer")]
        //public decimal? NPNCComputerHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCComputerPercentage { get; set; }
        //[Display(Name = "Web Offset")]
        //public decimal? NPNCWebHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCWebPercentage { get; set; }
        //[Display(Name = "Total")]
        //public decimal NPNCTotalHour { get; set; }
        //[Display(Name = "%")]
        //public decimal NPNCTotalPercentage { get; set; }
    }
    public class IdelTimeControllabelDescription
    {
        public int IdelTimeControllabelDescriptionID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }
    public class IdelTimeControllabelNonDescription
    {
        public int IdelTimeControllabelNonDescriptionID { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
    }
}