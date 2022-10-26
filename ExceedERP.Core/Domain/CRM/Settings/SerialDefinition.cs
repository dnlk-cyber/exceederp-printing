using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class SerialDefinition : TrackUserOperation
    {
        public int SerialDefinitionID { get; set; }

        public string Thing { get; set; }
        [Required]
        public string Description { get; set; }

        public bool IsMandatory { get; set; }

        public string Remark { get; set; }

    }
}
