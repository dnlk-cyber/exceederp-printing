using System;

namespace ExceedERP.Core.Domain.APTS
{
    public class DespensaryStore
    {
        public int DespensaryStoreId { get; set; }
        public int DrugListsId { get; set; }
        public DrugLists DrugLists { get; set; }
        public int DrugUnitId { get; set; }
        public DrugUnit DrugUnit { get; set; }
        public decimal BegStock { get; set; }
        public decimal NewReceived { get; set; }
        public decimal Balance { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public string Remark { get; set; }
    }
}
