using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common
{
    public class TrackUserSettingOperation
    {
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }
        public DateTime? LastModified { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
   
    public class Operations : TrackUserSettingOperation
    {
        public bool IsPostedToSecondLedger { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public bool IsVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string IsOnlineTransferredBy { get; set; }
        public DateTime? OnlineTransferredTime { get; set; }
        //added
        public bool IsSendForApproval { get; set; }
        public string OrgBranchName { get; set; }
        public string SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalTime { get; set; }
    }

    public class DepartmentOperations : TrackUserSettingOperation
    {
        public bool IsPostedToSecondLedger { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public bool IsVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string IsOnlineTransferredBy { get; set; }
        public DateTime? OnlineTransferredTime { get; set; }
        //added
        public bool IsSendForApproval { get; set; }
        public string OrgBranchName { get; set; }
        public string SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalTime { get; set; }
        //for departmental ..like self service
        public bool IsDepartmentOnlineApproved { get; set; }
        public string DepartmentOnlineApprovedBy { get; set; }
        public DateTime? DepartmentOnlineApprovedTime { get; set; }
        public bool IsDepartmentSendForApproval { get; set; }
        public string DepartmentSendForApprovalBy { get; set; }
        public DateTime? DepartmentSendForApprovalTime { get; set; }
    }
    public class OperationWithSecondValidation : Operations
    {
        public bool IsSecondOnlineApproved { get; set; }
        public string SecondOnlineApprovedBy { get; set; }
        public DateTime? SecondOnlineApprovedTime { get; set; }
        
        public bool IsSecondSendForApproval { get; set; }
        public string SecondSendForApprovalBy { get; set; }
        public DateTime? SecondSendForApprovalTime { get; set; }
    }
    public class OperationWithThirdValidation : Operations
    {
        public bool IsMiddleOnlineApproved { get; set; }
        public string MiddleOnlineApprovedBy { get; set; }
        public DateTime? MiddleOnlineApprovedTime { get; set; }

        public bool IsMiddleSendForApproval { get; set; }
        public string MiddleSendForApprovalBy { get; set; }
        public DateTime? MiddleSendForApprovalTime { get; set; }
        //
        public bool IsSecondOnlineApproved { get; set; }
        public string SecondOnlineApprovedBy { get; set; }
        public DateTime? SecondOnlineApprovedTime { get; set; }

        public bool IsSecondSendForApproval { get; set; }
        public string SecondSendForApprovalBy { get; set; }
        public DateTime? SecondSendForApprovalTime { get; set; }

        public bool IsSecondVoid { get; set; }
        public string SecondVoidBy { get; set; }
        public DateTime? SecondVoidTime { get; set; }
    }
    public class TrackUserOperation : TrackUserSettingOperation
    {
        public string OrgBranchName { get; set; }
    }                                                         
}
