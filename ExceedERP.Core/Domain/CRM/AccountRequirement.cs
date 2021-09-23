using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{



    public class AccountRequirement : TrackUserOperation
    {
        public int AccountRequirementID { get; set; }

        public string ControlAccountID { get; set; }

        public string ObjectType { get; set; }

        public bool IsMandatory { get; set; }

        public string Remark { get; set; }
    }


}
