using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;


namespace ExceedERP.Core.Domain.CRM
{
    public class CustomerService : TrackUserSettingOperation
            {
                [Key]
              public int CustomerServiceId { get; set; }
                  [Required]
                public string Description { get; set; }
                public ExceedERP.Core.Domain.CRM.CRMEnums.ServiceType Type { get; set; }
                public string Remark { get; set; }

            }
}
