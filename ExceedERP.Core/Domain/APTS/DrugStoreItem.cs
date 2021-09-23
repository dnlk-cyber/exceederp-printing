namespace ExceedERP.Core.Domain.APTS
{
    public class DrugStoreItem
    {
        public int DrugStoreItemId { get; set; }
        public int DrugStoreId { get; set; }
        public DrugStore DrugStore { get; set; }
        public int DrugListsId { get; set; }
        public DrugLists DrugLists { get; set; }
        public int DrugUnitId { get; set; }
        public DrugUnit DrugUnit { get; set; }
        public string ItemCode { get; set; }
        public decimal Balance { get; set; }
        public bool IsInnerTransfered { get; set; }
    }
}
