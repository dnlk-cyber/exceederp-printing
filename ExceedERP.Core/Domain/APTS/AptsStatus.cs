namespace ExceedERP.Core.Domain.APTS
{
    public class AptsStatus
    {
        public bool IsOnlineApproved { get; set; }
        public bool IsDepartmentApproved { get; set; }
        public bool IsSendForApproval { get; set; }
        public bool IsRejected { get; set; }
        public bool IsPending { get; set; }
    }
}
