using System.ComponentModel;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class TagSetting
    {
        public int TagSettingId { get; set; }
        public string Name { get; set; }
        [DisplayName("Code Length")]
        public int ItemCodeTrim { get; set; }

        public int IndexCount { get; set; }
        public bool Status { get; set; }
    }
}