using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.CRM;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class StatusType_HelpDesk : TrackUserSettingOperation
    {
        [DisplayName(" Id")]
        public int StatusType_HelpDeskId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string Color { get; set; }
        public InfoDeskFinalizeType Status { get; set; }
        public string Remark { get; set; }
        [NotMapped]
        [DefaultValue("")]
        [DisplayName("Color")]
        public string ColorHolder { get; set; }
    }

}
