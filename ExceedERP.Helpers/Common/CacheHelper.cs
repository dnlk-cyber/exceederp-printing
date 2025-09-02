using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

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


        public static void InvalidateGlSegmentSetupsCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.GlSegmentSetups))
            {
                MemoryCache.Default.Remove(CacheKeys.GlSegmentSetups);
            }
        }


        public static void InvalidateSegmentsCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.GlChartOfAccountSegments))
            {
                MemoryCache.Default.Remove(CacheKeys.GlChartOfAccountSegments);
            }
        }


        public static void InvalidateNaturalAccountCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.GLChartOfAccountAccounts))
            {
                MemoryCache.Default.Remove(CacheKeys.GLChartOfAccountAccounts);
            }
        }

        public static void InvalidateCache(string cacheKey)
        {
            if (MemoryCache.Default.Contains(cacheKey))
            {
                MemoryCache.Default.Remove(cacheKey);
            }
        }

        #region  HRM
        public static void InvalidateBusinessUnitsCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.BusinessUnits))
            {
                MemoryCache.Default.Remove(CacheKeys.BusinessUnits);
            }
        }
        public static void InvalidateBranchsCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.Branchs))
            {
                MemoryCache.Default.Remove(CacheKeys.Branchs);
            }
        }
        public static void InvalidateEmployeeCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.Employees))
            {
                MemoryCache.Default.Remove(CacheKeys.Employees);
            }
        }
        public static void InvalidateWorkUnitsCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.WorkUnits))
            {
                MemoryCache.Default.Remove(CacheKeys.WorkUnits);
            }
        }
        public static void InvalidateIsMultipleBusinessUnitCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.IsMultipleBusinessUnit))
            {
                MemoryCache.Default.Remove(CacheKeys.IsMultipleBusinessUnit);
            }
        }
        public static void InvalidateJobTitlesCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.JobTitles))
            {
                MemoryCache.Default.Remove(CacheKeys.JobTitles);
            }
        }
        public static void InvalidateRankCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.Ranks))
            {
                MemoryCache.Default.Remove(CacheKeys.Ranks);
            }
        }
        public static void InvalidateStepCache()
        {
            if (MemoryCache.Default.Contains(CacheKeys.Steps))
            {
                MemoryCache.Default.Remove(CacheKeys.Steps);
            }
        }

        #endregion




    }


}
