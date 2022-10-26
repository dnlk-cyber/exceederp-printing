using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class TransactionReference : TrackUserOperation
    {
        public int TransactionReferenceID { get; set; }

        public string ReferenceType { get; set; }

        public string Referring { get; set; }

        public string Referenced { get; set; }

        public double Value { get; set; }


        public string Remark { get; set; }

    }

}
