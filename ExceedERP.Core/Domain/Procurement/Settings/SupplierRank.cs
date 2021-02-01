using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Settings
{
    public class SupplierRank : TrackUserOperation
    {
        public int SupplierRankId { get; set; }
        [Index(IsUnique = true)]
        [StringLength(45)]
        [DisplayName("Rank Name")]
        public string Name { get; set; }
        [DisplayName("Description")]
        public string Description { get; set;  }

    }
}