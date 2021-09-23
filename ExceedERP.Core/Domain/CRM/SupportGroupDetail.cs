using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class SupportGroupDetail: TrackUserSettingOperation
    {
        public int SupportGroupDetailId { get; set; }
        public int SupportGroupId { get; set; }
        public string Name { get; set; }
        public string Remark { get; set; }

        public virtual SupportGroup SupportGroup { get; set; }
     }
    
}
