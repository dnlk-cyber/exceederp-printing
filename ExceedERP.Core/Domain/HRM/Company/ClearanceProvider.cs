using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class ClearanceProvider: DepartmentNewOperations 
    {
        [Key ]
        public int ClearanceProviderId { get; set; }
        [Display(Name = "HR_Company_ClearanceProvider_WorkUnit", ResourceType = typeof(Localization.Resources))]
        [Required]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_Company_ClearanceProvider_Name", ResourceType = typeof(Localization.Resources))]
        public string Name { get; set; }
        [Display(Name = "Enum_LookUpTypes_CLEARANCE_TYPE", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [NotMapped]
        public string FullName { get; set; }

    }

    public class ClearanceProviderDetail: TrackUserSettingOperation
    {
        [Key ]
        public int ClearanceProviderDetailId { get; set; }
        [Display(Name = "Approver Work Unit")]
        [Required]
        public int OrgStructureId { get; set; }
        [Display(Name = "JobTitle")]
        public int? LookupId { get; set; }
        [Display(Name="Approver  Name")]
        public int? HeadId { get; set; }
        public int ClearanceProviderId { get; set; }
        public virtual ClearanceProvider Clearance { get; set; }

    }

    public class ClearanceRequest: DepartmentNewOperations 
    {

        [Key ]
        public int ClearanceRequestId { get; set; }
        [Display(Name = "Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Enum_LookUpTypes_CLEARANCE_TYPE", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        
    }
    public class ClearanceRequestApprover: DepartmentNewOperations 
    {

        [Key ]
        public int ClearanceRequestApproverId { get; set; }
        [Display(Name = "Approver Work Unit")]
        public int ApproverOrgStructureId { get; set; }
        [Display(Name = "Requester Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Enum_LookUpTypes_CLEARANCE_TYPE", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        [Display(Name = "Request No_")]
        public int ClearanceRequestId { get; set; }
        public  ClearanceRequest Request { get; set; }


    }
    public class ClearanceRequestStatus: DepartmentNewOperations 
    {
        [Key ]
        public int ClearanceRequestStatusId { get; set; }
        [Display(Name = "Clearance Provider Work Unit")]
        public int OrgStructureId { get; set; }
        public bool IsAproved { get; set; }
        public int ClearanceRequestId { get; set; }
        public virtual ClearanceRequest Request { get; set; }


    }

}