using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class ApSetupItem : TrackUserSettingOperation
    {
        public int ApSetupItemId { get; set; }
        [Required]
        [Display(Name = "Item Name")]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Creation Sign")]
        public string CreationSign { get; set; }
    }
}
