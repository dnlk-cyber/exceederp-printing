using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class VoucherExtension : TrackUserOperation
    {
        public int VoucherExtensionID { get; set; }
        public string VoucherDefinition { get; set; }
        public string Description { get; set; }
        public double Index { get; set; }

        public bool IsMandatory { get; set; }

        public string Remark { get; set; }

    }

}
