using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common
{
    public class TrackTransaction : TrackUserOperation
    {
        public string Status { get; set; }
        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool isVoid { get; set; }
        public string VoidBy { get; set; }
        public DateTime? VoidTime { get; set; }
        //added
        public bool IsSendForApproval { get; set; }
        public string SendForApprovalBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SendForApprovalTime { get; set; }
    }
}
