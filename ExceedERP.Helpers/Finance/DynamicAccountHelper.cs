using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using System.Runtime.Caching;
using System.Text;
using ExceedERP.Helpers.Common;


namespace ExceedERP.Helpers.Finance
{
    public class DynamicAccountHelper
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public string GetAccountDesc(string accountAcode)
        {
            var sequence = MemoryCache.Default[CacheHelper.GlSegmentSetups] as List<GlSegmentSetupCache>;
            if (sequence == null)
            {
                sequence = _db.GlSegmentSetups
                             .Where(x => x.Enabled)
                             .Select(x => new GlSegmentSetupCache
                             {
                                 Sequence = x.Sequence,
                                 SegmentType = x.SegmentType,
                                 GLSegmentSetupId = x.GLSegmentSetupId,
                                 Name = x.Name,
                             })
                             .ToList();
                MemoryCache.Default.Add(CacheHelper.GlSegmentSetups, sequence, ObjectCache.InfiniteAbsoluteExpiration);
            }

            var otherSegments = MemoryCache.Default[CacheHelper.GlChartOfAccountSegments] as List<GlChartOfAccountSegmentCache>;
            if (otherSegments == null)
            {
                otherSegments = _db.GlChartOfAccountSegments
                                  .Select(x => new GlChartOfAccountSegmentCache
                                  {
                                      Values = x.Values,
                                      Description = x.Description,
                                      GLSegmentSetupId = x.GLSegmentSetupId
                                  })
                                  .ToList();
                MemoryCache.Default.Add(CacheHelper.GlChartOfAccountSegments, otherSegments, ObjectCache.InfiniteAbsoluteExpiration);
            }

            var accountSegments = MemoryCache.Default[CacheHelper.GLChartOfAccountAccounts] as List<GLChartOfAccountAccountCache>;
            if (accountSegments == null)
            {
                accountSegments = _db.GLChartOfAccountAccounts
                                    .Select(x => new GLChartOfAccountAccountCache
                                    {
                                        Values = x.Values,
                                        Description = x.Description
                                    })
                                    .ToList();
                MemoryCache.Default.Add(CacheHelper.GLChartOfAccountAccounts, accountSegments, ObjectCache.InfiniteAbsoluteExpiration);
            }

            var descBuilder = new StringBuilder();
            try
            {
                if (sequence.Any())
                {
                    string[] segments = accountAcode.Split('-');
                    List<string> descriptions = new List<string>();

                    for (int i = 0; i < segments.Length; i++)
                    {
                        var specificSegment = sequence.FirstOrDefault(x => x.Sequence == i + 1);
                        if (specificSegment != null)
                        {
                            if (specificSegment.SegmentType == DynamicSegmentTypes.NaturalAccount)
                            {
                                var value = accountSegments.FirstOrDefault(x => x.Values == segments[i]);
                                if (value != null) descriptions.Add(value.Description);
                            }
                            else
                            {
                                var value = otherSegments.FirstOrDefault(x => x.GLSegmentSetupId == specificSegment.GLSegmentSetupId && x.Values == segments[i]);
                                if (value != null) descriptions.Add(value.Description);
                            }
                        }
                    }

                    descBuilder.Append(string.Join("-", descriptions));
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return descBuilder.ToString();
        }


        public string GetAccountDesc_old(string accountAcode)
        {
            var sequence = _db.GlSegmentSetups.Where(x => x.Enabled).Select(x => new { x.Sequence, x.SegmentType, x.GLSegmentSetupId }).ToList();
            var otherSegments = _db.GlChartOfAccountSegments.Select(x => new { x.Values, x.Description, x.GLSegmentSetupId }).ToList();
            var accountSegments = _db.GLChartOfAccountAccounts.Select(x => new { x.Values, x.Description }).ToList();

            string desc = "";
            try
            {
                if (sequence.Any())
                {
                    string[] segments = accountAcode.Split('-');

                    for (int i = 0; i < segments.Length; i++)
                    {
                        var specificSegment = sequence.FirstOrDefault(x => x.Sequence == i + 1);
                        if (specificSegment != null && specificSegment.SegmentType == DynamicSegmentTypes.NaturalAccount)
                        {
                            var value = accountSegments.FirstOrDefault(x => x.Values == segments[i]);
                            if (value != null) desc = desc + "-" + value.Description;
                        }
                        else if (specificSegment != null && specificSegment.SegmentType != DynamicSegmentTypes.NaturalAccount)
                        {
                            var value = otherSegments.Where(x => x.GLSegmentSetupId == specificSegment.GLSegmentSetupId).FirstOrDefault(x => x.Values == segments[i]);
                            if (value != null) desc = desc + "-" + value.Description;

                        }
                    }

                    desc = desc.Remove(0, 1);//remove first -
                }
            }
            catch (Exception x)
            {
                // ignored
            }
            return desc;
        }

        public int GetNaturalAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups
                    .Where(x => x.Enabled).FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.NaturalAccount);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }

        public int GetCostCenterAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups
                    .Where(x => x.Enabled).FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }

        public int GetBranchAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups
                    .Where(x => x.Enabled)
                    .FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }


        public int GetBizUnitAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups
                     .Where(x => x.Enabled)
                     .FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.BusinessUnit);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }

        public int GetSubAccountSequenceIndex()
        {
            int index = -1;//not found

            using (var context = new ExceedDbContext())
            {
                var sequence = context.GlSegmentSetups
                    .Where(x => x.Enabled)
                    .FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                if (sequence != null)
                {
                    index = sequence.Sequence - 1;
                }
            }

            return index;
        }

        public string GeAccountSegment(string accountCombination, int index)
        {
            string account = "";
            try
            {
                account = accountCombination.Split('-')[index];
            }
            catch (Exception)
            {

                //log if index is out of range;
            }

            return account;
        }

        public decimal GeAccountLastBalance(List<GLLedgerPostingLines> records, string accountCode)
        {
            decimal balance = 0;
            var rangebalances = records.Where(x => x.GLLedgerPosting.AccountCode.ToUpper().Equals(accountCode)).ToList();
            foreach (var rangebalance in rangebalances)
            {
                int maxid =
                    rangebalances.Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode)
                        .Max(x => x.GLLedgerPostingLinesID);
                var lastbalance =
                    rangebalances
                        .Where(x => x.GLLedgerPosting.AccountCode == rangebalance.GLLedgerPosting.AccountCode).FirstOrDefault(x => x.GLLedgerPostingLinesID == maxid);
                if (lastbalance != null)
                    balance = balance + lastbalance.Balance;
            }

            return balance;
        }

        public List<string> GetAccountRange(string fromAccountCombination, string toAccountCombination, int index)
        {
            List<string> accountRange = null;
            var fromSegment = GeAccountSegment(fromAccountCombination, index);
            var toSegment = GeAccountSegment(toAccountCombination, index);

            using (var context = new ExceedDbContext())
            {
                var segmentSetup = context.GlSegmentSetups.FirstOrDefault(x => x.Sequence == index + 1);
                if (segmentSetup != null && segmentSetup.SegmentType == DynamicSegmentTypes.NaturalAccount)
                {
                    accountRange = context.GLChartOfAccountAccounts
                        .Where(x => String.Compare(x.Values.ToUpper(), fromSegment, StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.Values.ToUpper(), toSegment, StringComparison.Ordinal) <= 0)
                        .Select(x => x.Values)
                        .ToList();
                }
                else
                {
                    accountRange = context.GlChartOfAccountSegments
                        .Where(x => x.GLSegmentSetupId == segmentSetup.GLSegmentSetupId)
                        .Where(x => String.Compare(x.Values.ToUpper(), fromSegment, StringComparison.Ordinal) >= 0)
                        .Where(x => String.Compare(x.Values.ToUpper(), toSegment, StringComparison.Ordinal) <= 0)
                        .Select(x => x.Values)
                        .ToList();
                }
            }

            return accountRange;
        }

        public string ReplaceSegment(string accountAcode, int index, string value)
        {
            if (index >= 0 && !string.IsNullOrEmpty(value))
            {
                string newAccComb = "";
                string[] account = accountAcode.Split('-');
                account[index] = value;

                foreach (var t in account)
                {
                    newAccComb = newAccComb + "-" + t;
                }

                if (newAccComb.Length > 0)
                {
                    newAccComb = newAccComb.Remove(0, 1);
                }
                return newAccComb;
            }
            else
            {
                return accountAcode;
            }
        }

        public string ValidateSegmentCombinationLength(string accountComb)
        {
            string errror = "";
            int combLength = accountComb.Length;
            int totalLength = 0;
            var length = _db.GlSegmentSetups.Where(x => x.Enabled).ToList();
            foreach (var l in length)
            {
                totalLength = totalLength + l.SegmentLength;
            }

            totalLength = totalLength + length.Count - 1;//number of - underscores added
            if (combLength != totalLength)
                errror = $"Account combination segment length for {accountComb} should be {totalLength} characters.";
            if (length.Count == 0)
            {
                errror = "Segment length setting is not defined.";
            }

            return errror;
        }

    }


    public class GlSegmentSetupCache
    {
        public int Sequence { get; set; }
        public DynamicSegmentTypes SegmentType { get; set; }
        public int GLSegmentSetupId { get; set; }
        public string Name { get; set; }
    }
    public class GlChartOfAccountSegmentCache
    {
        public string Values { get; set; }
        public string Description { get; set; }
        public int GLSegmentSetupId { get; set; }
    }
    public class GLChartOfAccountAccountCache
    {
        public string Values { get; set; }
        public string Description { get; set; }
    }

}
