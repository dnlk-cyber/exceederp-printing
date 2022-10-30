using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.CRM
{
    public class Term : TrackUserOperation
   {
       public int TermID { get; set; }

       [DisplayName("Voucher Definition")]
      [Required]
      public int VoucherDefinitionID { get; set; }
       [Required]
       [DisplayName("Category")]
       public string LookupId { get; set; }

       [NotMapped]
       public string CategoryDescription { get; set; }
       [Required]
       public string DefaultValue { get; set; }
       public bool IsMandatory { get; set; }

       public string Remark { get; set; }
       public virtual Lookup Lookup { get; set; }
       public virtual VoucherDefinition VoucherDefinition { get; set; }
   }
}