using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.CMS
{
    public class City : TrackUserSettingOperation
    {
        [Key]
        public int CityId { get; set; }
        public Region Region { get; set; }
        public string Name { get; set; }

    }
}
