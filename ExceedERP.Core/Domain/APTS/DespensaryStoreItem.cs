namespace ExceedERP.Core.Domain.APTS
{
    public class DespensaryStoreItem
    {
        public int DespensaryStoreItemId { get; set; }
        public int DrugStoreId { get; set; }
        public DrugStore DrugStore { get; set; }
        public int DrugListsId { get; set; }
        public DrugLists DrugLists { get; set; }
        public int DrugUnitId { get; set; }
        public DrugUnit DrugUnit { get; set; }
        public decimal BegStock { get; set; }
        public decimal NewReceived { get; set; }
        public decimal Balance { get; set; }
        public string Unit { get; set; }
    }
}
