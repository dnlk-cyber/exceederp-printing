using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CRM
{
    public class SupportGroup : TrackUserSettingOperation
    {
        public int SupportGroupId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }
}
