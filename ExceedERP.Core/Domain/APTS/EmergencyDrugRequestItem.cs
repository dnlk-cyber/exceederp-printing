namespace ExceedERP.Core.Domain.APTS
{
    public class EmergencyDrugRequestItem
    {
        public int EmergencyDrugRequestItemId { get; set; }
        public int EmergencyDrugRequestId { get; set; }
        public int DrugListsId { get; set; }
        public int DrugUnitId { get; set; }
        public decimal BegStock { get; set; }
        public decimal ReceivedQty { get; set; }
        public decimal Loss { get; set; }
        public decimal EndStock { get; set; }
        public decimal QtyRequested { get; set; }
        public decimal QtyApproved { get; set; }
        public decimal QtyAllowed { get; set; }
    }
}
