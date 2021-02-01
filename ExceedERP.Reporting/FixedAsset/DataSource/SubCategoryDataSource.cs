using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
   
    [DataObject]
    public class SubCategoryDataSource
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetSubCategory> GetSubCategory()
        {
           
            List<FixedAssetSubCategory> assetSubCategories = _db.FixedAssetSubCategories.ToList();
            return assetSubCategories;
        }

    }
}
