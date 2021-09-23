using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class Specification : TrackUserOperation
    {
        public int SpecificationID { get; set; }

        public string ThingType { get; set; }
        public string ThingID { get; set; }
        public bool IsKey { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public string Attribute { get; set; }
        [Required]
        public string Value { get; set; }


        public string Remark { get; set; }

    }

}
