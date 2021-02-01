using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Common
{
    public class CacheHelper
    {
        
        public static void ResetCache()
        {
            List<string> cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }

        

    }

    
}
