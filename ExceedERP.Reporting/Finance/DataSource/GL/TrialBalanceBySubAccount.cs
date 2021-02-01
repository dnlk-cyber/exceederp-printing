using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    class TrialBalanceBySubAccount : List<TrialBalalnceViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<TrialBalalnceViewModel> TrialBalanceReport(string fromperiod, string toperiod, string subaccount, string segmentType, string segmentValue, bool summarizeByAccount)
        {
            List<TrialBalalnceViewModel> list = new List<TrialBalalnceViewModel> { };


            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                var allNaturalAccounts = _db.GLChartOfAccountAccounts.Select(x => new { x.Values, x.Description, x.AccountType }).ToList();
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);

         
                var periodList = _db.GLPeriods.Where(x => x.GLPeriodId >= from && x.GLPeriodId <= to)
                    .Select(x=> new {x.GlFiscalYear,x.Name,x.GLPeriodId}).ToList();

                var postedAccounts = _db.GLLedgerPostings
                    .Select(x => new
                    {
                        x.GLLedgerPostingID,
                        x.AccountType,
                        x.AccountCode,
                        x.AccountDesc,
                        x.AccountSegment,
                        x.SubAccountSegment,
                        x.BranchSegment,
                        x.CostCenterSegment,
                        x.Segment1,
                        x.Segment2,
                        x.Segment3,
                        x.Segment4,
                        x.Segment5,
                        x.Segment6,
                        x.Segment7,
                        x.Segment8,
                        GLLedgerPostingLines = x.GLLedgerPostingLines.Where(b => b.Type == LedgerPostTypes.Ending)//added

                    })
                    .OrderBy(x=>x.AccountSegment)//sort by natural account
                    .AsQueryable();

                //filter by subaccount
                if (!String.IsNullOrEmpty(subaccount))
                    postedAccounts = postedAccounts.Where(x => x.SubAccountSegment == subaccount).OrderBy(x => x.AccountSegment).AsQueryable();
                               


                if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                {
                    if (_accountHelper.IsDynamicCoA())
                    {
                        var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                        int index = setup?.Sequence ?? 0;
                        switch (index)
                        {
                            case 1:
                                postedAccounts = postedAccounts.Where(x => x.Segment1 == segmentValue).AsQueryable();
                                break;

                            case 2:
                                postedAccounts = postedAccounts.Where(x => x.Segment2 == segmentValue).AsQueryable();
                                break;

                            case 3:
                                postedAccounts = postedAccounts.Where(x => x.Segment3 == segmentValue).AsQueryable();
                                break;

                            case 4:
                                postedAccounts = postedAccounts.Where(x => x.Segment4 == segmentValue).AsQueryable();
                                break;

                            case 5:
                                postedAccounts = postedAccounts.Where(x => x.Segment5 == segmentValue).AsQueryable();
                                break;

                            case 6:
                                postedAccounts = postedAccounts.Where(x => x.Segment6 == segmentValue).AsQueryable();
                                break;
                            case 7:
                                postedAccounts = postedAccounts.Where(x => x.Segment7 == segmentValue).AsQueryable();
                                break;
                            case 8:
                                postedAccounts = postedAccounts.Where(x => x.Segment8 == segmentValue).AsQueryable();
                                break;
                        }
                    }
                    else
                    {
                        switch (segmentType)
                        {
                            case "Branch":
                                postedAccounts = postedAccounts.Where(x => x.BranchSegment == segmentValue).AsQueryable();
                                break;

                            case "Cost Center":
                                postedAccounts = postedAccounts.Where(x => x.CostCenterSegment == segmentValue).AsQueryable();
                                break;

                            case "Sub Account":
                                postedAccounts = postedAccounts.Where(x => x.SubAccountSegment == segmentValue).AsQueryable();
                                break;
                        }
                    }
                }

                //List<int> ledgerids = postedAccounts.Select(x => x.GLLedgerPostingID).ToList();

                //var postedLines = _db.GLLedgerPostingLines
                //       .Select(x => new { x.GLPeriodId, x.Type, x.Debit, x.Credit, x.Balance, x.GLLedgerPostingID, x.GLLedgerPosting })//added
                //       .Where(x=>ledgerids.Contains(x.GLLedgerPostingID))//added to filter only covered ledger recorded
                //       .AsQueryable();

                foreach (var period in periodList)
                {
                    var trialBalalnce = new TrialBalalnceViewModel
                    {
                        Year = period.GlFiscalYear.Name,
                        Period = period.Name,
                        TrialBalalnceLines = new List<TrialBalalnceLines>()
                    };

                   
                    decimal accountBalance = 0;
                    if (summarizeByAccount)
                    {
                        #region summarize by natural account
                        List<string> naturalAccounts = postedAccounts.Select(x => x.AccountSegment).Distinct().ToList();

                        foreach (var naturalAccount in naturalAccounts)
                        {
                            var acc = allNaturalAccounts.FirstOrDefault(x => x.Values == naturalAccount);
                            if (acc != null)
                            {
                                var record = new TrialBalalnceLines
                                {
                                    AccountCode = acc.Values,
                                    AccountDescription = acc.Description
                                };
                                //get end
                                var endRecord = postedAccounts
                                    .Where(x => x.AccountSegment == naturalAccount)
                                    .SelectMany(x => x.GLLedgerPostingLines).Where(b => b.GLPeriodId == period.GLPeriodId)
                                    .ToList();

                                if (endRecord.Any())
                                {
                                    accountBalance = endRecord.Sum(x => x.Balance);

                                    if (acc.AccountType == GlAccountTypes.Asset ||
                                        acc.AccountType == GlAccountTypes.Expense)
                                    {
                                        if (accountBalance > 0)
                                        {
                                            record.Credit = 0;
                                            record.Debit = accountBalance;
                                        }
                                        else if (accountBalance < 0)
                                        {
                                            record.Credit = Math.Abs(accountBalance);
                                            record.Debit = 0;
                                        }


                                    }
                                    else if (acc.AccountType == GlAccountTypes.Liability ||
                                             acc.AccountType == GlAccountTypes.Capital ||
                                             acc.AccountType == GlAccountTypes.Revenue)
                                    {
                                        if (accountBalance > 0)
                                        {
                                            record.Credit = accountBalance;
                                            record.Debit = 0;
                                        }
                                        else if (accountBalance < 0)
                                        {
                                            record.Credit = 0;
                                            record.Debit = Math.Abs(accountBalance);
                                        }


                                    }


                                    if (accountBalance != 0)
                                    {
                                        trialBalalnce.TrialBalalnceLines.Add(record);
                                    }
                                }
                            }
                            else
                            {
                                Logger.Debug($"{naturalAccount} is not found in the natural account list, but exists in the ledger.");
                            }

                        }


                        #endregion
                    }
                    else
                    {
                        #region whole account combination
                        var selectedpostedAccounts = postedAccounts.ToList();
                        foreach (var postedAccount in selectedpostedAccounts)
                        {
                            var record = new TrialBalalnceLines
                            {
                                AccountCode = postedAccount.AccountCode,
                                AccountDescription = postedAccount.AccountDesc
                            };
                            //get end
                            var endRecord = postedAccount.GLLedgerPostingLines
                                .FirstOrDefault(x => x.GLPeriodId == period.GLPeriodId);

                            if (endRecord != null)
                                accountBalance = endRecord.Balance;
                            else
                                accountBalance = 0;

                            if (postedAccount.AccountType == GlAccountTypes.Asset ||
                                postedAccount.AccountType == GlAccountTypes.Expense)
                            {
                                if (accountBalance > 0)
                                {
                                    record.Credit = 0;
                                    record.Debit = accountBalance;
                                }
                                else if (accountBalance < 0)
                                {
                                    record.Credit = Math.Abs(accountBalance);
                                    record.Debit = 0;
                                }


                            }
                            else if (postedAccount.AccountType == GlAccountTypes.Liability ||
                                     postedAccount.AccountType == GlAccountTypes.Capital ||
                                     postedAccount.AccountType == GlAccountTypes.Revenue)
                            {
                                if (accountBalance > 0)
                                {
                                    record.Credit = accountBalance;
                                    record.Debit = 0;
                                }
                                else if (accountBalance < 0)
                                {
                                    record.Credit = 0;
                                    record.Debit = Math.Abs(accountBalance);
                                }


                            }


                            if (accountBalance != 0)
                            {
                                trialBalalnce.TrialBalalnceLines.Add(record);
                            }


                        }
                        #endregion
                    }
                    list.Add(trialBalalnce);

                }
                return list;

            }
            return list;
        }
       


    }

}
