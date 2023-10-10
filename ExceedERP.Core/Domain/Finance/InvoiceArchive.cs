using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance
{
    public class InvoiceArchive : TrackUserSettingOperation
    {
        [Key]
        public int InvoiceArchiveId { get; set; }
        public string PaidFor { get; set; }
        public string InvoiceNo { get; set; }
        public string VoucherNo { get; set; }
        public decimal AmountPaid { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime DatePaid { get; set; }

        public string BoxFileNo { get; set; }
    }
}
