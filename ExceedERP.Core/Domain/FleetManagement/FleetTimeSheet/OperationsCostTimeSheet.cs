using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet
{
    public class OperationsCostTimeSheet : TrackUserSettingOperation
    {
        public bool TimeSheetSendForAproval { get; set; }
        public bool CostSheetSendForAproval { get; set; }
        public string TimeSheetSendForApprovalBy { get; set; }
        public string CostSheetSendForApprovalBy { get; set; }
        public DateTime? TimeSheetSendForApprovalTime { get; set; }
        public DateTime? CostSheetSendForApprovalTime { get; set; }
        
        public bool TimeSheetApproved { get; set; }
        public bool CostSheetApproved { get; set; }
        public string TimeSheetApprovedBy { get; set; }
        public string CostSheetApprovedBy { get; set; }
        public DateTime? TimeSheetApprovedDate { get; set; }
        public DateTime? CostSheetApprovedDate { get; set; }

        [Display(Name ="Status")]
        public string TimeSheetStatus { get; set; }
        [Display(Name ="Status")]
        public string CostSheetStatus { get; set; }
    }

    public class TimeSheetApprovalNew : TrackUserSettingOperation
    {
        [Key]
        public int TimeSheetApprovalNewId { get; set; }
        public bool IsApproved { get; set; }
        public int? TimeSheetId { get; set; }
       
        public string Status { get; set; }
        public string Remark { get; set; }
        public TimeSheetType TimeSheetType { get; set; }
    }
    public class CostSheetApproval : TrackUserSettingOperation
    {
        [Key]
        public int CostSheetApprovalId { get; set; }
        public bool IsApproved { get; set; }
        public int? CostSheetId { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }
        public TimeSheetType TimeSheetType { get; set; }
    }
    

}
