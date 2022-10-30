using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class Accounts : TrackUserOperation
    {
        public int AccountsID { get; set; }
        [Required]
        public string ControlAccount { get; set; }
        [Required]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Parent { get; set; }

        public string Remark { get; set; }

    }
}
