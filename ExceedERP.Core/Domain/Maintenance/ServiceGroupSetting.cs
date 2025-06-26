using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Maintenance
{
    public class ServiceGroupSetting:TrackUserSettingOperation
    {
        public int ServiceGroupSettingId {get;set;}
        public string ServiceGroupName { get; set; }
        public string Description { get; set; }
    }

   
}
