using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class ObjectStateDefinition : TrackUserOperation
    {
        public int ObjectStateDefinitionID { get; set; }
        [Required]
        public string GSLType { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Font { get; set; }

        public string Remark { get; set; }


    }

}
