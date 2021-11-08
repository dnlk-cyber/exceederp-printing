using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.HRM.SelfCare
{
    public class RequestesStatus
    {
        [Key]
        public int RequestStatusId { get; set; }
        public int RequestId { get; set; }
        public string ReferenceType { get; set; }
        [Required]
        [Display(Name = "HR_SelfCare_RequestesStatus_Status", ResourceType = typeof(Localization.Resources))]
        public RequestStatus Status { get; set; }
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Required]
        [Display(Name = "HR_SelfCare_RequestesStatus_Date", ResourceType = typeof(Localization.Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "HR_SelfCare_RequestesStatus_Date", ResourceType = typeof(Localization.Resources))]
        public string DateReqStatAm { get; set; }
        [Display(Name = "HR_SelfCare_RequestesStatus_ChangedBy", ResourceType = typeof(Localization.Resources))]
        public string ChangedBy { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "Requeststatus_Approver", ResourceType = typeof(Localization.Resources))]

        public int? ApproverId { get; set; }
        [NotMapped]
        public int? ApproverDepartment { get; set; }
    }
}