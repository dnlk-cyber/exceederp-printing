using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Common.HRM
{
    public class BusinessUnit : TrackUserOperation 
    {
        public int BusinessUnitId { get; set; }
        [Required]       
        public string Name { get; set; }
        public string Accountcode { get; set; }
        public string Remark { get; set; }
    }
}