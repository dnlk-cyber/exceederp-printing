using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace ExceedERP.Helpers.Common
{
    public class CacheHelper
    {
        public const string GlSegmentSetups = "GlSegmentSetups";
        public const string GlChartOfAccountSegments = "GlChartOfAccountSegments";
        public const string GLChartOfAccountAccounts = "GLChartOfAccountAccounts";

        public static void ResetCache()
        {
            List<string> cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }


        public static void InvalidateGlSegmentSetupsCache()
        {
            if (MemoryCache.Default.Contains(GlSegmentSetups))
            {
                MemoryCache.Default.Remove(GlSegmentSetups);
            }
        }


        public static void InvalidateSegmentsCache()
        {
            if (MemoryCache.Default.Contains(GlChartOfAccountSegments))
            {
                MemoryCache.Default.Remove(GlChartOfAccountSegments);
            }
        }


        public static void InvalidateNaturalAccountCache()
        {
            if (MemoryCache.Default.Contains(GLChartOfAccountAccounts))
            {
                MemoryCache.Default.Remove(GLChartOfAccountAccounts);
            }
        }




    }


}
