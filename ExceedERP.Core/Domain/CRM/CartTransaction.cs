using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class CartTransaction : TrackUserOperation
    {
        public int CartTransactionID { get; set; }
        public string Cart { get; set; }
        public string Voucher { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }
        public string Remark { get; set; }
    }

}
