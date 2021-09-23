using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

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
