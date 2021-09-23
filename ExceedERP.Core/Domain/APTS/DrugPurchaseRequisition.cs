using System;

namespace ExceedERP.Core.Domain.APTS
{
    public class DrugPurchaseRequisition
    {
        public int DrugPurchaseRequisitionId { get; set; }
        public int BranchId { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestDateAm { get; set; }   
        public string StoreCode { get; set; }
        public int StoreId { get; set; }
    }
}
