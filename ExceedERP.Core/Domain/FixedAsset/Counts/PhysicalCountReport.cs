using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class  PhysicalCountReport : TrackUserOperation
    {
        public int  PhysicalCountReportID { get; set; }
        [Required]
        public string Division { get; set; }
        public string Department { get; set; }
        public string Section { get; set; }
       
        public int GlFiscalYearId { get; set; }
        

        public virtual GlFiscalYear GLFiscalYear { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool isOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        [Display(Name = "Online Approved Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        [Display(Name = "Void Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? VoidTime { get; set; }
        public virtual ICollection< PhysicalCountReportDetails>  PhysicalCountReportDetails { get; set; }
    }
}
