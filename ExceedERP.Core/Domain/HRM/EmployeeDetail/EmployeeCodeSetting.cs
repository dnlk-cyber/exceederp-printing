namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    public class EmployeeCodeSetting
    {
        public int EmployeeCodeSettingId { get; set; }
        public EmployeeIDAbbreviationType Type { get; set; }
        public int Index { get; set; }
        public string Remark { get; set; }
    }
}
