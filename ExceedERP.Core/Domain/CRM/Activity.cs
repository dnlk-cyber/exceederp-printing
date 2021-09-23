using System;

namespace ExceedERP.Core.Domain.CRM
{

    public class Activity
    {
        public int ActivityID { get; set; }
        public string Reference { get; set; }
        public string ActivityDefinition { get; set; }
        public string User { get; set; }

        public DateTime StartTimeStamp { get; set; }

        public DateTime EndTimeStamp { get; set; }

        public string OrganizationUnitDefinition { get; set; }

        public string Remark { get; set; }

    }


}
