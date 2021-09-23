using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class SupportGroup : TrackUserSettingOperation
    {
        public int SupportGroupId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}
