using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class ItemCategoryObjectDataSource 
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetInventoryCategories()
        {
            var cats = db.ItemCategorys
                        .ToList()
                        .Select(s => new
                        {
                            FullCategorySelector = s.ItemCategoryCode + "_" + s.Name
                        }).OrderBy(x => x.FullCategorySelector);

            return cats.Select(x => x.FullCategorySelector).ToList();
        }
    }
}
