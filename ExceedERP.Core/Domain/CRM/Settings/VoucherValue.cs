using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class VoucherValue : TrackUserOperation
    {
        public int VoucherValueID { get; set; }
        public string VoucherType { get; set; }

        public string Voucher { get; set; }



        public double Discount { get; set; }

        public double SubTotal { get; set; }

        public double AdditionalCharge { get; set; }

        public double Remark { get; set; }
    }

}
