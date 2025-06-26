using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class CustomerTicket_HelpDesk : TrackUserOperation
    {
        public int CustomerTicket_HelpDeskId { get; set; }
        [DisplayName("Ticket Type")]
        public int TicketType_HelpDeskId { get; set; }
        //public int? Requester { get; set; }
        public string Description { get; set; }
        [DisplayName("Priority")]
        public TicketPriority TicketPriority { get; set; }
        [DisplayName("Group")]
        public int SupportGroup_HelpDeskId { get; set; }
        [DisplayName("Status")]
        public TicketStatus TicketStatus { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int? BranchId { get; set; }
        //public virtual Branch Branch { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        //public virtual MainCompanyStructure OrgStructure { get; set; }
        public int Rate { get; set; }
        [DisplayName("Assigned To")]
        public int? AssignedTo { get; set; }
        public string Role { get; set; }
        [DisplayName(" Assigned Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? AssignedDate { get; set; }
        [DisplayName("Material Used")]
        public int? MaterialItemsId { get; set; }
        [DisplayName("Item/device Category")]
        public int? FixedAssetCategoryId { get; set; }
        [DisplayName("Sub Category")]
        public int? FixedAssetSubCategoryId { get; set; }
        [DisplayName("Asset")]
        public int? AssetRegistrationId { get; set; }
        [Display(Name = "Item Name")]
        public string ItemName { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        public int? Quantity { get; set; }
        [DisplayName("Priority Color")]
        public string PriorityColor { get; set; }
        //public TicketStatus TicketStatus { get; set; }       
        //public TicketPriority TicketPriority { get; set; }
        public string Remark{ get; set; }
        [DisplayName(" Due Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? DueDate { get; set; }
        [DisplayName("Closed Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ClosedDate { get; set; }
        [DisplayName("Over-Due")]
        public decimal OverDue { get; set; }
        [DisplayName("Root-Cuase")]
        public string RootCuase { get; set; }
        [DisplayName("Solution Given")]
        public string SolutionGiven { get; set; }
        [DisplayName("Requester(Employee)")]
        public int? EmployeeId { get; set; }
        [DisplayName("Age In Days")]
        public string AgeOfTicket { get; set; }
        [DisplayName("Pending Reason")]
        public string PendingReason { get; set; }
        public bool IsAssigned { get; set; }
        public bool IsSendForApproval { get; set; }
        public DateTime? SendApprovalTime { get; set; }
        public string SendApprovalBy { get; set; }
        public bool IsConfirmedByTheRequester { get; set; }
        public bool IsSendForClosing { get; set; }
        //public int Rate { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMultipleAssign { get; set; }
        public TicketHandledBy Handler { get; set; }
        [DisplayName("IP Address")]
        public string RequesterIPAddress { get; set; }
        public virtual TicketType_HelpDesk TicketType_HelpDesk { get; set; }
        public virtual SupportGroup_HelpDesk SupportGroup_HelpDesk { get; set; }
        public virtual MaterialItems MaterialItems { get; set; }
        public virtual Person_Employee Person_Employee { get; set; }
    }



    public class CustomerTicketView
    {
        [Key]
        public int CustomerTicketID { get; set; }
        public int CustomerID { get; set; }
        public string TicketDefintion { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string TicketGroup { get; set; }

        public string EscalationLevel { get; set; }

        public CustomerTicketSourceType Source { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        public string Remark { get; set; }

    }


    public class TicketAssignment : TrackUserOperation
    {
        public int TicketAssignmentID { get; set; }

        public string TicketID { get; set; }

        public string TicketActivityDefinitionID { get; set; }

        public string AssignedBy { get; set; }
        public string ProcessedBy { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime AssignmentDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }
        public string Remark { get; set; }
    }

    public class TicketRole : TrackUserOperation
    {
        public int TicketRoleID { get; set; }
        public string TicketActivityDefinitionID { get; set; }
        public string RoleID { get; set; }

       
        public bool NeedsPassKey { get; set; }

        public string Remark;
    }

    public class CustomerTicketAttachment
    {
        public int CustomerTicketAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int CustomerTicket_HelpDeskId { get; set; }
        public virtual CustomerTicket_HelpDesk Ticket_HelpDesk { get; set; }

    }

    //public enum TicketPriorityType
    //{
    //    Low,
    //    Medium,
    //    Urgent,
    //    TopUrgent
    //}

    public enum CustomerTicketSourceType
    {
   
        Customer,
        Employee,
        Unknown
    }

    public enum TicketTiming
    {
        On_Time ,Late
    }
    public enum SourceTicket
    {


        Direct,
        In_Direct
    }
    public enum TicketHandledBy
    {

        N_A,
        Internal,
        External
    }
}
public enum TicketStatus
{
    [Display(Name = "Waiting for Response")]
    Waiting,
    [Display(Name = "Ticket Assigned")]
    Assigned,
    [Display(Name = "Issue Resolved")]
    Resolved,
    [Display(Name = "Ticket Closed")]
    Closed,
    [Display(Name = "Not Solved")]
    NOT_SOLVED,
    [Display(Name = "Waiting for Confirmation")]
    WAITING_FOR_CONFIRMATION,
    Pending


}
//public enum TicketStatus
//{
//    Waiting,
//    Assigned,
//    Resolved,
//    Closed,
//    NOT_SOLVED,
//    WAITING_FOR_CONFIRMATION

//}
public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}
