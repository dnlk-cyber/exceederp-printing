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
    class TrialBalanceDataSource : List<TrialBalalnceViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<TrialBalalnceViewModel> TrialBalanceReport(string fromperiod, string toperiod, string balancingSegment, string amounttype,string account, string segmentType, string segmentValue, bool summarizeByAccount, string businessUnit)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<TrialBalalnceViewModel> list = new List<TrialBalalnceViewModel> { };


            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                var allNaturalAccounts = _db.GLChartOfAccountAccounts.Select(x => new {x.Values, x.Description,x.AccountType}).ToList();
                int fromPeriodD = int.Parse(fromperiod);
                int toPeriodD = int.Parse(toperiod);

                var from = _db.GLPeriods
                    .Select(x => new {x.DateFrom, x.DateTo, x.GLPeriodId})//added
                    .FirstOrDefault(x => x.GLPeriodId == fromPeriodD);

                var to = _db.GLPeriods
                    .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                    .FirstOrDefault(x => x.GLPeriodId == toPeriodD);


                var periodList = _db.GLPeriods.Where(x => x.DateFrom >= from.DateFrom)
                                              .Where(x=>x.DateTo <= to.DateTo)
                                                .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId,x.GlFiscalYear,x.Name })//added
                                                .ToList();

                var postedAccounts = _db.GLLedgerPostings
                                       //.Include(x => x.GLLedgerPostingLines)
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
                                           x.BusinessUnit,
                                           x.Segment1,
                                           x.Segment2,
                                           x.Segment3,
                                           x.Segment4,
                                           x.Segment5,
                                           x.Segment6,
                                           x.Segment7,
                                           x.Segment8,
                                           GLLedgerPostingLines = x.GLLedgerPostingLines.Where(b=>b.Type== LedgerPostTypes.Ending)//added

                                       })
                                        .OrderBy(x => x.AccountSegment).AsQueryable();

                if (!String.IsNullOrEmpty(account))
                    postedAccounts = postedAccounts.Where(x => x.AccountSegment == account).AsQueryable();

                if (!String.IsNullOrEmpty(businessUnit))
                    postedAccounts = postedAccounts.Where(x => x.BusinessUnit == businessUnit).AsQueryable();

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
                //                .Select(x => new { x.GLPeriodId, x.Type, x.Debit, x.Credit, x.Balance, x.GLLedgerPostingID, x.GLLedgerPosting })//added
                //                //.Where(x => ledgerids.Contains(x.GLLedgerPostingID))//added to filter only covered ledger recorded
                //                .Where(x => x.Type == LedgerPostTypes.Ending)//added 
                //                .AsQueryable();

                //sort
               // var sortedpostedAccounts = postedAccounts.OrderBy(x => x.AccountSegment).ToList();

                foreach (var period in periodList)
                {
                    //postedLines = postedLines
                    //             .Where(x => x.GLPeriodId == period.GLPeriodId)
                    //             .AsQueryable();

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
                                    .SelectMany(x=>x.GLLedgerPostingLines).Where(b=>b.GLPeriodId == period.GLPeriodId)
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
                        // postedAccounts = postedAccounts.Where(x => x.AccountCode == "21104-00-0C4-S2-PM020 " ||x.AccountCode== "21104-00-0C2-S1-PG006            ");

                        foreach (var postedAccount in postedAccounts)
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
                            {
                                accountBalance = endRecord.Balance;

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


                        }


                        #endregion
                    }



                    //for diagnostics if not equal
                    decimal sumDebit = trialBalalnce.TrialBalalnceLines.Sum(x => x.Debit);
                    decimal sumCredit = trialBalalnce.TrialBalalnceLines.Sum(x => x.Credit);
                    decimal netDebit = 0;
                    decimal netCredit = 0;
                    if (sumDebit > sumCredit)
                        netDebit = sumDebit - sumCredit;
                    if (sumDebit > sumCredit)
                        netCredit = sumCredit - sumDebit;
                    if(sumDebit!=sumCredit)
                    {
                        var record = new TrialBalalnceLines
                        {
                            AccountCode = "Unknown",
                            AccountDescription = "Unknown",
                            Debit=netDebit,
                            Credit=netCredit,
                        };

                        //trialBalalnce.TrialBalalnceLines.Add(record);
                    }

                    list.Add(trialBalalnce);

                }

                //if(list.Sum(x=>x.debi))
                return list;

            }
            return list;
        }
        //public List<TrialBalalnceViewModel> TrialBalanceReport1(string fromperiod, string toperiod, string balancingSegment, string amounttype, string account, bool summarizeByAccount)
        //{
        //    List<TrialBalalnceViewModel> list = new List<TrialBalalnceViewModel> { };
        //    TrialBalalnceLines record;


        //    if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
        //    {
        //        int from = int.Parse(fromperiod);
        //        int to = int.Parse(toperiod);

        //        //var fromperiodObj = _db.GLPeriods.Find(from);
        //        //var toperiodObj = _db.GLPeriods.Find(to);

        //        var periodList = _db.GLPeriods.Where(x => x.GLPeriodId >= from && x.GLPeriodId <= to).ToList();
        //        var postedAccounts = _db.GLLedgerPostings.OrderBy(x => x.AccountSegment).ToList();
        //        if (!String.IsNullOrEmpty(account))
        //        {
        //            postedAccounts = postedAccounts.Where(x => x.AccountSegment == account).ToList();
        //        }

        //        foreach (var period in periodList)
        //        {
        //            var trialBalalnce = new TrialBalalnceViewModel
        //            {
        //                Year = period.GlFiscalYear.Name,
        //                Period = period.Name,
        //                TrialBalalnceLines = new List<TrialBalalnceLines>()
        //            };


        //            var postedLines = _db.GLLedgerPostingLines.AsQueryable();

        //            decimal accountBalance = 0;
        //            if (postedLines.Any())
        //            {
        //                foreach (var postedAccount in postedAccounts)
        //                {
        //                    record = new TrialBalalnceLines
        //                    {
        //                        AccountCode = postedAccount.AccountCode,
        //                        AccountDescription = postedAccount.AccountDesc
        //                    };
        //                    //get end
        //                    var endRecord = postedLines.Where(x => x.GLLedgerPostingID == postedAccount.GLLedgerPostingID).Where(x => x.GLPeriodId == period.GLPeriodId).FirstOrDefault(x => x.Type == LedgerPostTypes.Ending);
        //                    if (endRecord != null)
        //                        accountBalance = endRecord.Balance;
        //                    else
        //                        accountBalance = 0;

        //                    if (postedAccount.AccountType == GlAccountTypes.Asset ||
        //                        postedAccount.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        if (accountBalance > 0)
        //                        {
        //                            record.Credit = 0;
        //                            record.Debit = accountBalance;
        //                        }
        //                        else if (accountBalance < 0)
        //                        {
        //                            record.Credit = Math.Abs(accountBalance);
        //                            record.Debit = 0;
        //                        }


        //                    }
        //                    else if (postedAccount.AccountType == GlAccountTypes.Liability ||
        //                             postedAccount.AccountType == GlAccountTypes.Capital ||
        //                             postedAccount.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        if (accountBalance > 0)
        //                        {
        //                            record.Credit = accountBalance;
        //                            record.Debit = 0;
        //                        }
        //                        else if (accountBalance < 0)
        //                        {
        //                            record.Credit = 0;
        //                            record.Debit = Math.Abs(accountBalance);
        //                        }


        //                    }


        //                    if (accountBalance != 0)
        //                    {
        //                        trialBalalnce.TrialBalalnceLines.Add(record);
        //                    }


        //                }

        //            }
        //            list.Add(trialBalalnce);

        //        }
        //        return list;

        //    }
        //    return list;
        //}
        //public List<TrialBalalnceViewModel> TrialBalanceReport(string fromperiod, string toperiod, string balancingSegment,string amounttype)
        //{
        //    List<TrialBalalnceViewModel> list = new List<TrialBalalnceViewModel> { };
        //    TrialBalalnceLines record;


        //    if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
        //    {
        //        int from = int.Parse(fromperiod);
        //        int to = int.Parse(toperiod);

        //        //var fromperiodObj = _db.GLPeriods.Find(from);
        //        //var toperiodObj = _db.GLPeriods.Find(to);

        //        var periodList = _db.GLPeriods.Where(x => x.GLPeriodId >= from && x.GLPeriodId <= to).ToList();
        //        var postedAccounts = _db.GLLedgerPostings.ToList();
        //        foreach (var period in periodList)
        //        {
        //            var trialBalalnce = new TrialBalalnceViewModel
        //            {
        //                Year = period.GlFiscalYear.Name,
        //                Period = period.Name,
        //                TrialBalalnceLines = new List<TrialBalalnceLines>()
        //            };


        //            var postedLines = _db.GLLedgerPostingLines.Where(x => x.EffectiveDate >= period.DateFrom)
        //                                                      .Where(x => x.EffectiveDate <= period.DateTo).ToList();
        //            decimal accountBalance = 0;
        //            if (postedLines.Any())
        //            {
        //                foreach (var postedAccount in postedAccounts)
        //                {
        //                    record = new TrialBalalnceLines();
        //                    record.Description = postedAccount.Description;
        //                    record.AccountDescription = postedAccount.AccountDesc;
        //                    //get max balance
        //                    accountBalance =ReportHelper.GetPostedAccountLatestBalance(postedAccount.Description, postedLines);
        //                    if (postedAccount.AccountType == GlAccountTypes.Asset ||
        //                        postedAccount.AccountType == GlAccountTypes.Expense)
        //                    {
        //                        if (accountBalance > 0)
        //                        {
        //                            record.Credit = 0;
        //                            record.Debit = accountBalance;
        //                        }
        //                        else if (accountBalance < 0)
        //                        {
        //                            record.Credit = Math.Abs(accountBalance);
        //                            record.Debit = 0;
        //                        }


        //                    }
        //                    else if (postedAccount.AccountType == GlAccountTypes.Liability ||
        //                             postedAccount.AccountType == GlAccountTypes.Capital ||
        //                             postedAccount.AccountType == GlAccountTypes.Revenue)
        //                    {
        //                        if (accountBalance > 0)
        //                        {
        //                            record.Credit = accountBalance;
        //                            record.Debit = 0;
        //                        }
        //                        else if (accountBalance < 0)
        //                        {
        //                            record.Credit = 0;
        //                            record.Debit = Math.Abs(accountBalance);
        //                        }


        //                    }


        //                    if (accountBalance != 0)
        //                    {
        //                        trialBalalnce.TrialBalalnceLines.Add(record);
        //                    }


        //                }

        //            }
        //            list.Add(trialBalalnce);

        //        }
        //        return list;

        //    }
        //   return list;
        //}
        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<TrialBalalnceViewModel> TrialBalanceReport1(string Branch, DateTime date)
        //{

        //    List<TrialBalalnceViewModel> list = new List<TrialBalalnceViewModel> { };
        //    try
        //    {
        //        DateTime dt00 = FiscalYearHelper.GetFiscalYearBeginningDate(date);

        //        TrialBalalnceViewModel record;

        //        var locations = _db.GLChartOfAccountLocations.ToList();
        //        var allAccounts = _db.GLChartOfAccountAccounts.ToList();
        //        var costcenters = _db.GLChartOfAccountCostCenters.ToList();
        //        var subaccounts = _db.GLChartOfAccountSubAccounts.ToList();

        //        var assetAccounts = _db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Asset).ToList();
        //        var liabilityAccounts = _db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Liability).ToList();
        //        var capitalAccounts = _db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Capital).ToList();
        //        var revenueAccounts = _db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Revenue).ToList();
        //        var expenseAccounts = _db.GLChartOfAccountAccounts.Where(x => x.AccountType == GlAccountTypes.Expense).ToList();

        //        var transactions = _db.GLLedgerPostingLines.Where(x => x.EffectiveDate <= date).ToList();

        //        if (String.IsNullOrEmpty(Branch))
        //        {
        //            #region all branches
        //            #region asset
        //            foreach (GLChartOfAccountAccount account in assetAccounts)
        //            {
        //                foreach (GLChartOfAccountLocation location in locations)
        //                {
        //                    foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                    {
        //                        foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                        {
        //                            record = new TrialBalalnceViewModel();
        //                            record.Description = (account.Values + "-" + location.Values + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                            record.AccountDescription = account.Description + "-" + location.Description + "-" + cost.Description + "-" + sub.Description;

        //                            var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //                            if (debitcreditObj != null)
        //                            {
        //                                var maxId = debitcreditObj.Max(x => x.GLLedgerPostingLinesID);
        //                                var latestRecord = debitcreditObj.SingleOrDefault(x => x.GLLedgerPostingLinesID == maxId);
        //                                if (latestRecord != null) record.Debit = latestRecord.Balance;
        //                                record.Credit = 0;
        //                                if (record.Debit > 0)
        //                                {
        //                                    list.Add(record);
        //                                }
        //                            }

        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region liability
        //            foreach (GLChartOfAccountAccount account in liabilityAccounts)
        //            {
        //                foreach (GLChartOfAccountLocation location in locations)
        //                {
        //                    foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                    {
        //                        foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                        {
        //                            record = new TrialBalalnceViewModel();
        //                            record.Description = (account.Values + "-" + location.Values + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                            record.AccountDescription = account.Description + "-" + location.Description + "-" + cost.Description + "-" + sub.Description;

        //                            var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //                            if (debitcreditObj != null)
        //                            {
        //                                var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                                var latestRecord = debitcreditObj.SingleOrDefault(x => x.EffectiveDate == maxDate);
        //                                record.Debit = 0;
        //                                if (latestRecord != null) record.Credit = latestRecord.Balance;
        //                                if (record.Credit > 0)
        //                                {
        //                                    list.Add(record);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            //#region capital
        //            //foreach (GLChartOfAccountAccount account in capitalAccounts)
        //            //{
        //            //    foreach (GLChartOfAccountLocation location in locations)
        //            //    {
        //            //        foreach (GLChartOfAccountCostCenter cost in costcenters)
        //            //        {
        //            //            foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //            //            {
        //            //                record = new TrialBalalnceViewModel();
        //            //                record.Description = (account.Values + "-" + location.Values + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //            //                record.AccountDescription = account.Description + "-" + location.Description + "-" + cost.Description + "-" + sub.Description;

        //            //                var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //            //                if (debitcreditObj != null)
        //            //                {
        //            //                    var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //            //                    var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //            //                    record.Debit = 0;
        //            //                    record.Credit = latestRecord.Balance;
        //            //                    list.Add(record);
        //            //                }
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //#endregion
        //            //#region revenue
        //            //foreach (GLChartOfAccountAccount account in revenueAccounts)
        //            //{
        //            //    foreach (GLChartOfAccountLocation location in locations)
        //            //    {
        //            //        foreach (GLChartOfAccountCostCenter cost in costcenters)
        //            //        {
        //            //            foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //            //            {
        //            //                record = new TrialBalalnceViewModel();
        //            //                record.Description = (account.Values + "-" + location.Values + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //            //                record.AccountDescription = account.Description + "-" + location.Description + "-" + cost.Description + "-" + sub.Description;

        //            //                var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper())
        //            //                                                 .Where(x => x.EffectiveDate >= dt00).ToList();

        //            //                if (debitcreditObj != null)
        //            //                {
        //            //                    var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //            //                    var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //            //                    record.Debit = 0;
        //            //                    record.Credit = latestRecord.Balance;
        //            //                    list.Add(record);
        //            //                }
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //#endregion
        //            //#region expense
        //            //foreach (GLChartOfAccountAccount account in expenseAccounts)
        //            //{
        //            //    foreach (GLChartOfAccountLocation location in locations)
        //            //    {
        //            //        foreach (GLChartOfAccountCostCenter cost in costcenters)
        //            //        {
        //            //            foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //            //            {
        //            //                record = new TrialBalalnceViewModel();
        //            //                record.Description = (account.Values + "-" + location.Values + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //            //                record.AccountDescription = account.Description + "-" + location.Description + "-" + cost.Description + "-" + sub.Description;

        //            //                var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper())
        //            //                                                 .Where(x => x.EffectiveDate >= dt00).ToList();

        //            //                if (debitcreditObj != null)
        //            //                {
        //            //                    var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //            //                    var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //            //                    record.Debit = latestRecord.Balance;
        //            //                    record.Credit = 0;
        //            //                    list.Add(record);
        //            //                }
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //            //#endregion
        //            #endregion
        //        }
        //        else
        //        {
        //            #region specific branche
        //            #region asset
        //            foreach (GLChartOfAccountAccount account in assetAccounts)
        //            {
        //                foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                {
        //                    foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                    {
        //                        record = new TrialBalalnceViewModel();
        //                        record.Description = (account.Values + "-" + Branch + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                        record.AccountDescription = account.Description + "-" + Branch + "-" + cost.Description + "-" + sub.Description;

        //                        var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //                        if (debitcreditObj != null)
        //                        {
        //                            var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                            var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //                            record.Debit = latestRecord.Balance;
        //                            record.Credit = 0;
        //                            list.Add(record);
        //                        }

        //                    }
        //                }
        //            }
        //            #endregion
        //            #region liability
        //            foreach (GLChartOfAccountAccount account in liabilityAccounts)
        //            {
        //                foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                {
        //                    foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                    {
        //                        record = new TrialBalalnceViewModel();
        //                        record.Description = (account.Values + "-" + Branch + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                        record.AccountDescription = account.Description + "-" + account.Description + "-" + cost.Description + "-" + sub.Description;

        //                        var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //                        if (debitcreditObj != null)
        //                        {
        //                            var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                            var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //                            record.Debit = 0;
        //                            record.Credit = latestRecord.Balance;
        //                            list.Add(record);
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region capital
        //            foreach (GLChartOfAccountAccount account in capitalAccounts)
        //            {
        //                foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                {
        //                    foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                    {
        //                        record = new TrialBalalnceViewModel();
        //                        record.Description = (account.Values + "-" + Branch + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                        record.AccountDescription = account.Description + "-" + account.Description + "-" + cost.Description + "-" + sub.Description;

        //                        var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper()).ToList();
        //                        if (debitcreditObj != null)
        //                        {
        //                            var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                            var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //                            record.Debit = 0;
        //                            record.Credit = latestRecord.Balance;
        //                            list.Add(record);
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region revenue
        //            foreach (GLChartOfAccountAccount account in revenueAccounts)
        //            {
        //                foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                {
        //                    foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                    {
        //                        record = new TrialBalalnceViewModel();
        //                        record.Description = (account.Values + "-" + Branch + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                        record.AccountDescription = account.Description + "-" + account.Description + "-" + cost.Description + "-" + sub.Description;

        //                        var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper())
        //                                                         .Where(x => x.EffectiveDate >= dt00).ToList();

        //                        if (debitcreditObj != null)
        //                        {
        //                            var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                            var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //                            record.Debit = 0;
        //                            record.Credit = latestRecord.Balance;
        //                            list.Add(record);
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #region expense
        //            foreach (GLChartOfAccountAccount account in expenseAccounts)
        //            {
        //                foreach (GLChartOfAccountCostCenter cost in costcenters)
        //                {
        //                    foreach (GLChartOfAccountSubAccount sub in subaccounts)
        //                    {
        //                        record = new TrialBalalnceViewModel();
        //                        record.Description = (account.Values + "-" + Branch + "-" + cost.Values + "-" + sub.Values).ToUpper();
        //                        record.AccountDescription = account.Description + "-" + account.Description + "-" + cost.Description + "-" + sub.Description;

        //                        var debitcreditObj = transactions.Where(x => x.GLLedgerPosting.Description.ToUpper() == record.Description.ToUpper())
        //                                                         .Where(x => x.EffectiveDate >= dt00).ToList();

        //                        if (debitcreditObj != null)
        //                        {
        //                            var maxDate = debitcreditObj.Max(x => x.EffectiveDate);
        //                            var latestRecord = debitcreditObj.Where(x => x.EffectiveDate == maxDate).SingleOrDefault();
        //                            record.Debit = latestRecord.Balance;
        //                            record.Credit = 0;
        //                            list.Add(record);
        //                        }
        //                    }
        //                }
        //            }
        //            #endregion
        //            #endregion
        //        }

        //        return list;
        //    }
        //    catch (Exception)
        //    {
        //        return list;
        //    }
        //    return list;
        //}


    }

}
