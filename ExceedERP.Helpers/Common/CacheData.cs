using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.ViewModel;
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
            var segmentSequence = MemoryCache.Default[CacheKeys.GlSegmentSetups] as List<GlSegmentSetupCache>;
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
                MemoryCache.Default.Add(CacheKeys.GlSegmentSetups, segmentSequence, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return segmentSequence;
        }

        public List<GLChartOfAccountAccountCache> GetOfAccountAccountCaches()
        {
            var accounts = MemoryCache.Default[CacheKeys.GLChartOfAccountAccounts] as List<GLChartOfAccountAccountCache>;
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
                MemoryCache.Default.Add(CacheKeys.GLChartOfAccountAccounts, accounts, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return accounts;
        }

        public List<GlChartOfAccountSegmentCache> GetChartOfAccountSegmentCaches()
        {
            var segments = MemoryCache.Default[CacheKeys.GlChartOfAccountSegments] as List<GlChartOfAccountSegmentCache>;
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
                MemoryCache.Default.Add(CacheKeys.GlChartOfAccountSegments, segments, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return segments;
        }

        public List<ForeginKeySelectorVm> GetBranchCaches()
        {
            var branches = MemoryCache.Default[CacheKeys.Branchs] as List<ForeginKeySelectorVm>;
            if (branches == null)
            {
                branches = _db.Branch
                    .Select(x => new ForeginKeySelectorVm
                    {
                        Id = x.BranchId,
                        Name = x.Name
                    })
                    .ToList();
                MemoryCache.Default.Add(CacheKeys.Branchs, branches, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return branches;
        }

        public List<ForeginKeySelectorVm> GetBusinessUnitCaches()
        {
            var businessUnits = MemoryCache.Default[CacheKeys.BusinessUnits] as List<ForeginKeySelectorVm>;
            if (businessUnits == null)
            {
                businessUnits = _db.BusinessUnits
                    .Select(x => new ForeginKeySelectorVm
                    {
                        Id = x.BusinessUnitId,
                        Name = x.Name
                    })
                    .ToList();
                MemoryCache.Default.Add(CacheKeys.BusinessUnits, businessUnits, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return businessUnits;
        }

        public List<ForeginKeySelectorVm> GetWorkUnitCaches()
        {
            var workUnits = MemoryCache.Default[CacheKeys.WorkUnits] as List<ForeginKeySelectorVm>;
            if (workUnits == null)
            {
                workUnits = _db.MainCompanyStructures
                    .Select(x => new ForeginKeySelectorVm
                    {
                        Id = x.OrgStructureId,
                        Name = x.Name
                    })
                    .ToList();
                MemoryCache.Default.Add(CacheKeys.WorkUnits, workUnits, ObjectCache.InfiniteAbsoluteExpiration);
            }

            return workUnits;
        }
    }


}
