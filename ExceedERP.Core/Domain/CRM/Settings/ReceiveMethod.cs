using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class ReceiveMethod : TrackUserOperation
    {
        public int ReceiveMethodID { get; set; }
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

    }
}
