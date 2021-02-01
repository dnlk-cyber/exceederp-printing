using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
