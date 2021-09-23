using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.APTS
{
    public class DrugReceivalModel19 : Operations

    {
        public int DrugReceivalModel19Id { get; set; }
        public int DrugStoreId { get; set; }
        public string ReceiverName { get; set; }
        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReceivingDate { get; set; }
        public string ReceivingDateAmh { get; set; }
        public string Name { get; set; }
        public string ReceiptNo { get; set; }
        public string StoreCode { get; set; }
        public bool IsCurrent { get; set; }
        //public bool IsSendForApproval { get; set; }
        //public string SendForApprovalBy { get; set; }
        //public DateTime SendForApprovalTime { get; set; }
    }
}
