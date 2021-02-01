using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.InfoDesk.Support
{
    public class SupportGroupDetail_HelpDesk: TrackUserSettingOperation
    {
        public int SupportGroupDetail_HelpDeskId { get; set; }
        public int SupportGroupId { get; set; }
        public int EmployeeId { get; set; }
        public string Remark { get; set; }

        public virtual SupportGroup_HelpDesk SupportGroup { get; set; }
     }
    
}
