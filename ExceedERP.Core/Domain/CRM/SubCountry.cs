using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class SubCountry : TrackUserOperation
    {
        public int SubCountryID { get; set; }
        [Required]
        [DisplayName("Country")]
        public string CountryID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string AlternativeName { get; set; }


        public string Parent { get; set; }

        public string Remark { get; set; }

    }
}
