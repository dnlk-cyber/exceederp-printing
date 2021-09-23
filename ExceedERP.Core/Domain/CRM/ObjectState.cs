using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{

    public class ObjectState : TrackUserOperation
    {

        public int ObjectStateID { get; set; }

        public string Reference { get; set; }
        [Required]
        [DisplayName("Object State Definition")]
        public string ObjectStateDefinitionID { get; set; }
        public string Remark { get; set; }
    }
}
