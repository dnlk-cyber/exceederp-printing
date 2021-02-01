using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Web.Areas.Finance.Models
{
    public class GlBalanceImport
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Account Description")]
        public string AccountDesc { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public string Balance { get; set; }
    }
}