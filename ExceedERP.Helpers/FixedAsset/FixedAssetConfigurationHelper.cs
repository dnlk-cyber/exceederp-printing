using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Finance
{
    public class FixedAssetConfigurationHelper
    {
        public static decimal GetAssetMinimumLevel()
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal value = 0;

            var conf = db.Configurations
                    .Where(x => x.Category == "FixedAsset")
                    .Where(x => x.Reference == "FIXED_ASSET_CONSIDERATION")
                    .FirstOrDefault();

            if (conf != null)
            {
                decimal.TryParse(conf.Attribute, out value);
            }
            
           return value;
        }
        
       
      
    }
}
