using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.APTS
{
    public class DrugRequestItem
    {
        public int DrugRequestItemId { get; set; }
        public int DrugRequestId { get; set; }
        public virtual DrugRequest DrugRequest { get; set; }
        public int DrugListsId { get; set; }
        public virtual DrugLists DrugLists { get; set; }
        public int DrugUnitId { get; set; }
        public virtual DrugUnit DrugUnit { get; set; }
        public decimal BegStock { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal Loss { get; set; }
        public decimal EndStock { get; set; }
        public decimal QtyRequested { get; set; }
        public decimal QtyApproved { get; set; }
        public decimal QtyAllowed { get; set; }
        [NotMapped]
        public bool IsApproved { get; set; }

    }
}
