using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
