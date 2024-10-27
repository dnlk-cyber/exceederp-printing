using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Caching;

namespace ExceedERP.Helpers.Common
{
    public class CacheData
    {
        private readonly ExceedDbContext _db;
        public CacheData()
        {
            _db = new ExceedDbContext();

        }

        public List<GlSegmentSetupCache> GetSegmentSetupCaches()
        {
            var segmentSequence = MemoryCache.Default[CacheHelper.GlSegmentSetups] as List<GlSegmentSetupCache>;
            if (segmentSequence == null)
            {
                segmentSequence = _db.GlSegmentSetups
                                    .Where(x => x.Enabled)
                                    .Select(x => new GlSegmentSetupCache
                                    {
                                        Sequence = x.Sequence,
                                        SegmentType = x.SegmentType,
                                        GLSegmentSetupId = x.GLSegmentSetupId,
                                        Name = x.Name,
                                    })
                                    .AsNoTracking()
                                    .ToList();
                MemoryCache.Default.Add(CacheHelper.GlSegmentSetups, segmentSequence, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return segmentSequence;
        }

        public List<GLChartOfAccountAccountCache> GetOfAccountAccountCaches()
        {
            var accounts = MemoryCache.Default[CacheHelper.GLChartOfAccountAccounts] as List<GLChartOfAccountAccountCache>;
            if (accounts == null)
            {
                accounts = _db.GLChartOfAccountAccounts
                    .Where(x => x.Enable && x.Parent == false)
                    .OrderBy(x => x.Values)
                    .Select(x => new GLChartOfAccountAccountCache
                    {
                        Values = x.Values,
                        Description = x.Description
                    })
                    .ToList();
                MemoryCache.Default.Add(CacheHelper.GLChartOfAccountAccounts, accounts, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return accounts;
        }

        public List<GlChartOfAccountSegmentCache> GetChartOfAccountSegmentCaches()
        {
            var segments = MemoryCache.Default[CacheHelper.GlChartOfAccountSegments] as List<GlChartOfAccountSegmentCache>;
            if (segments == null)
            {
                segments = _db.GlChartOfAccountSegments
                    .Where(x => x.Enable && x.Parent == false)
                    .OrderBy(x => x.Values)
                    .Select(x => new GlChartOfAccountSegmentCache
                    {
                        GLSegmentSetupId = x.GLSegmentSetupId,
                        Values = x.Values,
                        Description = x.Description
                    })
                    .ToList();
                MemoryCache.Default.Add(CacheHelper.GlChartOfAccountSegments, segments, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return segments;
        }


    }


}
