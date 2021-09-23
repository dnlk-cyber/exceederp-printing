using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class CreditSetting : TrackUserOperation
    {
        public int CreditSettingID { get; set; }
        public int OrganizationID { get; set; }
        [Required]
        public decimal Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

    }

 

}
