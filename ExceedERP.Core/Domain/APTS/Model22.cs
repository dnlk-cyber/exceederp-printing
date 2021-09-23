using System;

namespace ExceedERP.Core.Domain.APTS
{
    public class Model22
    {
        public int Model22Id { get; set; }
        public int RequisitionModel20Id { get; set; }
        public RequisitionModel20 RequisitionModel20 { get; set; }
        //public int DrugListsId { get; set; }
        //public DrugLists DrugLists { get; set; }
        public string Unit { get; set; }
        public int QuantityIssued { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal RetailPrice { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsOut { get; set; }
        public bool IsNew { get; set; }
        public DateTime? DispensaryEntryDate { get; set; }
        public DateTime? DateCreated { get; set; }
        public string CreatedBy { get; set; }
    }
}
