using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.Archive
{
    public class SubjectType
    {
        public int SubjectTypeId { get; set; }
        [Display(Name ="Subject Type")]
        public string SubjectTypeName { get; set; }
        public string SubjectCode { get; set; }
        [NotMapped]
        public string CodeWithName => string.Concat(SubjectCode + " - " + SubjectTypeName);
        public string Remark { get; set; }

    }
}
