using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class LaboratoryDrugRequestItem
    {
        public int LaboratoryDrugRequestItemId { get; set; }
        public int LaboratoryDrugRequestId { get; set; }
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
