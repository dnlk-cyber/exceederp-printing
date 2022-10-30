using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public  class QueueType : TrackUserOperation
    {
        [Key]
        public int QueueTypeId { get; set; }
        public string QueueName { get; set; }
        public string Description { get; set; }
    }
}
