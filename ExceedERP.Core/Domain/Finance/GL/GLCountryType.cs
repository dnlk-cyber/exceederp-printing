using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.GL
{
    public class GLCountryType : TrackUserSettingOperation
    {
        public int GLCountryTypeID { get; set; }
        [Required]
        [Display(Name = "Country Name")]
        public string CountryName { get; set; }
        public string Description { get; set; }
    }
}
