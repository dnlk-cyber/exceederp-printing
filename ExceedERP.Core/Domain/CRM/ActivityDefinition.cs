using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class ActivityDefinition : TrackUserOperation
    {
        public int ActivityDefinitionID { get; set; }

        public string Reference { get; set; }

        public string Description { get; set; }

        public int Index { get; set; }

        public bool HasIssuingEffect { get; set; }

        public String Remark { get; set; }

    }

}
