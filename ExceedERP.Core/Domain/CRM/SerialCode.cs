using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class SerialCode : TrackUserOperation
    {
        public int SerialCodeID { get; set; }

        public string LineItem { get; set; }

        public string SerialDefinition { get; set; }

        public string Number { get; set; }

        public string Remark { get; set; }

    }

   
}
