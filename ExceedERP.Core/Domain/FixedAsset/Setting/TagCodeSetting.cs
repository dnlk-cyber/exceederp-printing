namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class TagCodeSetting
    {
        public int TagCodeSettingId { get; set; }
        public TagAbbreviationType Type { get; set; }
        
        public int Index { get; set; }

        public string Remark { get; set; }
    }
}