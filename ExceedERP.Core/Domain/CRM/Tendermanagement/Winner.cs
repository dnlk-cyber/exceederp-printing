using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
namespace ExceedERP.Core.Domain.CRM.TenderManagement

{
    public class Winner:TrackUserSettingOperation
    {
        public int WinnerId { get; set; }
        public int SalesLotId { get; set; }
        public string SalesLotName { get; set; }
        public int SalesLotItemId { get; set; }
        public string CustomerName { get; set; }
        public string TinNumber { get; set; }
        public string CustormerAddress { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal WinningPrice { get; set; }
        public int ProducCategoryId { get; set; }
        public string ProductCategoryDesc { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }

    }
}
