using System;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class LifeSpan : TrackUserOperation
    {
        public int LifeSpanID { get; set; }
        public string Reference { get; set; }
        public DateTime ProductionDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string Remark { get; set; }


    }

}
