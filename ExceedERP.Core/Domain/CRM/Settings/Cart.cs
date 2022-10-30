namespace ExceedERP.Core.Domain.CRM
{
    public class Cart
    {
        public int CartID { get; set; }

        public string Description { get; set; }

        public int Index { get; set; }

        public int? Parent { get; set; }


        public bool IsActive { get; set; }

        public string Remark { get; set; }
    }
  

}
