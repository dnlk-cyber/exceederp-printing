namespace ExceedERP.Core.Domain.CRM
{

    public class ControlAccount
    {
        public int ControlAccountId { get; set; }

        public string Description { get; set; }
        public string NormalBalance { get; set; }
        public string AccountCategory { get; set; }

        public string AccountSubCategory { get; set; }

        public string Remark { get; set; }
    }
}
