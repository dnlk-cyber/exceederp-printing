using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class AccountMap : TrackUserOperation
    {
        public int AccountMapID { get; set; }
        [Required]
        [DisplayName("Control Accounts")]
        public string ControlAccountID { get; set; }
        [Required]
        [DisplayName("Accounts")]
        public string AccountID { get; set; }

        public string ReferenceType { get; set; }

        public string Reference { get; set; }

        public string Remark { get; set; }

    }

}
