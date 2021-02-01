using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class SupportGroup_HelpDesk : TrackUserSettingOperation
    {
        public int SupportGroup_HelpDeskId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}
