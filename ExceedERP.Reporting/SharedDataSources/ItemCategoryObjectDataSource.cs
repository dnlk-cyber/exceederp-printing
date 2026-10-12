using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;


namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class ItemCategoryObjectDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        ////[DataObjectMethod(DataObjectMethodType.Select)]
        ////public List<string> getInventoryCategories()
        ////{

        ////    List<string> list = new List<string> {};
            
        ////    var cats = db.ItemCategorys
        ////        .ToList()
        ////        .Select(s => new
        ////        {
        ////            FullCategorySelector = s.ItemCategoryCode + "_" + s.Name
        ////        }).OrderBy(x => x.FullCategorySelector);

        ////    if (!cats.Any())
        ////    {
        ////        return list;
        ////    }

        ////    return cats.Select(x => x.FullCategorySelector).ToList();
        ////}
    }

    
    
}