using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
   public  class ArSalesOrderBroadcastTime : TrackUserOperation
    {
        public int ArSalesOrderBroadcastTimeId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        [Display(Name = "Broadcasting Date and Time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime BroadcastDateTime { get; set; }
        public string  Remark { get; set; }
    }
}
