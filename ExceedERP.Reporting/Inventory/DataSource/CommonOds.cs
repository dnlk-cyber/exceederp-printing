using ExceedERP.Core.Domain.Common.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class CommonOds : List<SelectorVm>
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectorVm> GetStockType()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            var record = new SelectorVm
            {
                Code = "Merchandise",
                Name = "Merchandise"
            };
            list.Add(record);
            var record1 = new SelectorVm
            {
                Code = "Consumable",
                Name = "Consumable"
            };
            list.Add(record1);
            return list;
        }
    }
}
