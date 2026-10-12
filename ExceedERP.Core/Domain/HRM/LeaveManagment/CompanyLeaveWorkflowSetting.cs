using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary> A  CompanyLeaveWorkflowSetting.  </summary>
    ///
    /// <remarks>   Kibrom Abera, Sat, may 9,2026. </remarks>
    /// 
    ////////////////////////////////////////////////////////////////////////////////////////////////////


    public class CompanyLeaveWorkflowSetting : TrackUserOperation
    {

        [Key]
        public int CompanyLeaveWorkflowSettingId { get; set; }
        [Display(Name = "Use Project Manager Approval")]
        public bool UseProjectManagerApproval { get; set; }

        public string Remark { get; set; }


    }



}
