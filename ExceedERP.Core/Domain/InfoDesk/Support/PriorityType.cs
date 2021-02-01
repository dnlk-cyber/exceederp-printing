using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class PriorityType_HelpDesk : TrackUserSettingOperation
    {

        [DisplayName(" Id")]
        public int PriorityType_HelpDeskId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Color { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        [DefaultValue("")]
        [DisplayName("Color")]
        public string ColorHolder { get; set; }
    }
}
