using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.ProductionFollowUp
{
    public class JobType
    {
        public int JobTypeId { get; set; }
        public string Type { get; set; }
        [Display(Name ="Tax Type")]
        public int GLTaxRateID { get; set; }

    }
    public class JobTypeItem
    {
        public int JobTypeItemId { get; set; }
        public int JobTypeId { get; set; }
        public string Name { get; set; }
    }
}
