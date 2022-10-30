using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class LocationUserAssignment : TrackUserOperation
    {
        public int LocationUserAssignmentId { get; set; }
        public string UserName { get; set; }
        public int LocationMapId { get; set; }
        public virtual LocationMap LocationMap { get; set; }

        [NotMapped]
        public string FullName { get; set; }
    }
}
