using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.APTS
{
   public class PatientRegisterDetail
    {
        public int PatientRegisterDetailId { get; set; }
        public int PatientRegisterId { get; set; }
        public int DrugListsId { get; set; }
        public int DrugUnitId { get; set; }
        public string InternalCode { get; set; }
        public decimal TotalQty { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal TotalRetailPrice { get; set; }
        public bool IsUpdated { get; set; }
    }
}
