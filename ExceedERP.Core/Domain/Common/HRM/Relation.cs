using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class Relation
    {
        [Key]
        public int RelationId { get; set; }
        [Required]
        public string Refereing { get; set; }
        [Required]
        public string Reference { get; set; }
        [Required]
        [Display(Name = "Relation Type")]
        public Lookup RelationType { get; set; }
        public string Remark { get; set; }
    }
}