using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class GeneralLedgerOds : List<StockBalanceOds>, IDisposable
    {
        /**
        * v2 algorithm faster by 6 times...to test
        * optimization using include and selecting only required properties
        */
        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        public GeneralLedgerOds()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public GeneralLedgerHeaderVm GetGeneralLedgerHeader(string fromperiod, string toperiod, string acctFrom, string acctTo)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            GeneralLedgerHeaderVm headerDs = new GeneralLedgerHeaderVm
            {
                AccountFrom = acctFrom,
                AccountTo = acctTo
            };

            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                int from = int.Parse(fromperiod);
                int to = int.Parse(toperiod);

                headerDs.DateFrom = _db.GLPeriods.Find(from).DateFrom.ToShortDateString();
                headerDs.DateTo = _db.GLPeriods.Find(to).DateTo.ToShortDateString();
            }


            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                        a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<GeneralLedgerVm> GetLedgerBalance(string fromperiod, string toperiod, string acctFrom, string acctTo,string subaccountFrom, string subaccountTo, string costcenter, bool onlysummary, string segmentType, string segmentValue)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<GeneralLedgerVm> balances = new List<GeneralLedgerVm>();
            if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
            {
                int fromPeriodD = int.Parse(fromperiod);
                int toPeriodD = int.Parse(toperiod);

                var from = _db.GLPeriods
                   .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                   .FirstOrDefault(x => x.GLPeriodId == fromPeriodD);

                var to = _db.GLPeriods
                    .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId })//added
                    .FirstOrDefault(x => x.GLPeriodId == toPeriodD);


                var periodList = _db.GLPeriods.Where(x => x.DateFrom >= from.DateFrom)
                                              .Where(x => x.DateTo <= to.DateTo)
                                                .Select(x => new { x.DateFrom, x.DateTo, x.GLPeriodId, x.GlFiscalYear, x.Name })//added
                                                .ToList();

              
                var ledgers = _db.GLLedgerPostings
                    .Include(x => x.GLLedgerPostingLines)
                    .Where(x => String.Compare(x.AccountSegment, acctFrom, StringComparison.Ordinal) >= 0)
                    .Where(x => String.Compare(x.AccountSegment, acctTo, StringComparison.Ordinal) <= 0)
                    .Select(x => new
                    {
                        x.AccountCode,
                        x.SubAccountSegment,
                        x.CostCenterSegment,
                        x.AccountType,
                        x.AccountSegment,
                        x.BranchSegment,
                        x.Segment1,
                        x.Segment2,
                        x.Segment3,
                        x.Segment4,
                        x.Segment5,
                        x.Segment6,
                        x.Segment7,
                        x.Segment8,
                        x.GLLedgerPostingLines
                       // GLLedgerPostingLines = x.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22)

                    })
                    .AsNoTracking().AsQueryable();
               
                

                #region get covered periods for sorting data

              

                var sortedPeriods = periodList.OrderBy(x => x.DateTo).Select(x=>new {x.GLPeriodId}).ToList();
                var lastPeriod = periodList.OrderByDescending(x => x.DateTo).Take(1).FirstOrDefault();
                //remove last period to avoid duplication
                //foreach (var glPeriod in sortedPeriods)
                //{
                //    if(lastPeriod != null && glPeriod.GLPeriodId!=lastPeriod.GLPeriodId)
                //        sortedPeriodsBeforeLast.Add(glPeriod);
                //}
                #endregion

                if (!String.IsNullOrEmpty(costcenter))
                    ledgers = ledgers.Where(x => x.CostCenterSegment == costcenter).AsQueryable();

                if (!String.IsNullOrEmpty(subaccountFrom)&& !String.IsNullOrEmpty(subaccountTo))
                             ledgers = ledgers.Where(x => String.Compare(x.SubAccountSegment, subaccountFrom, StringComparison.Ordinal) >= 0)
                                              .Where(x => String.Compare(x.SubAccountSegment, subaccountTo, StringComparison.Ordinal) <= 0).AsQueryable();

                if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                {
                    var setup = _db.GlSegmentSetups.FirstOrDefault(x => x.Name == segmentType);
                    int index = setup?.Sequence ?? 0;
                    switch (index)
                    {
                        case 1:
                            ledgers = ledgers.Where(x => x.Segment1 == segmentValue).AsQueryable();
                            break;

                        case 2:
                            ledgers = ledgers.Where(x => x.Segment2 == segmentValue).AsQueryable();
                            break;

                        case 3:
                            ledgers = ledgers.Where(x => x.Segment3 == segmentValue).AsQueryable();
                            break;

                        case 4:
                            ledgers = ledgers.Where(x => x.Segment4 == segmentValue).AsQueryable();
                            break;

                        case 5:
                            ledgers = ledgers.Where(x => x.Segment5 == segmentValue).AsQueryable();
                            break;

                        case 6:
                            ledgers = ledgers.Where(x => x.Segment6 == segmentValue).AsQueryable();
                            break;
                        case 7:
                            ledgers = ledgers.Where(x => x.Segment7 == segmentValue).AsQueryable();
                            break;
                        case 8:
                            ledgers = ledgers.Where(x => x.Segment8 == segmentValue).AsQueryable();
                            break;
                    }
                }

                foreach (var ledgerPosting in ledgers)
                {
                    

                    var ledgeritem = new GeneralLedgerVm
                    {
                        AccountCode = ledgerPosting.AccountCode,
                        AccountDescription = _accountHelper.GetAccountDesc(ledgerPosting.AccountCode),
                        GeneralLedgerVmLines = new List<GeneralLedgerVmLines>(),
                        GeneralLedgerVmLastLineses = new List<GeneralLedgerVmLastLines>()
                    };

                    if (onlysummary)
                    {
                        
                        #region current transaction- only summary--ending
                       
                        foreach (var period in sortedPeriods)
                        {
                            #region ending

                            var endingRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in endingRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int)balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLines.Add(lines);
                            }

                            #endregion
                        }

                        if (lastPeriod != null)
                        {
                            #region ending of last period

                            var lastendingRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == lastPeriod.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in lastendingRecords)
                            {
                                var lines = new GeneralLedgerVmLastLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int) balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLastLineses.Add(lines);
                            }

                            #endregion
                        }

                        #endregion
                    }
                    else
                    {
                        #region current transaction - all
                        
                        foreach (var period in sortedPeriods)
                        {
                            #region beginning

                            var begginningRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Begining).ToList();
                            foreach (var balance in begginningRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int)balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLines.Add(lines);
                            }

                            #endregion
                            #region standard

                            var standardRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard)
                                .ToList().OrderBy(x => x.EffectiveDate).ThenBy(x => x.GLLedgerPostingLinesID);
                            foreach (var balance in standardRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    ReferenceDoc = balance.ReferenceDoc,
                                    PaymentReference = balance.PaymentReference,
                                    InvoiceReferenceDoc = balance.InvoiceReferenceDoc,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int)balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLines.Add(lines);
                            }

                            #region netchange movement

                            decimal netChange = 0;
                            //decimal netDebit = 0;
                            //decimal netCredit = 0;

                            var debitchange = standardRecords.Sum(x => x.Debit);
                            var creditchange = standardRecords.Sum(x => x.Credit);                          

                          
                            if (ledgerPosting.AccountType == GlAccountTypes.Asset ||
                                ledgerPosting.AccountType == GlAccountTypes.Expense)
                            {
                                netChange = debitchange - creditchange;
                                //if (netChange > 0)
                                //    netDebit = netChange;
                                //else
                                //    netCredit = Math.Abs(netChange);
                            }
                            else
                            {
                                netChange = creditchange - debitchange;
                                //if (netChange > 0)
                                //    netCredit = netChange;
                                //else
                                //    netDebit = Math.Abs(netChange);
                            }

                            var netChangeLine = new GeneralLedgerVmLines
                            {
                                Journal = "Movement in Current Period",
                                Debit = debitchange,
                                Credit = creditchange,
                                Balance= netChange,
                                Type =3
                            };

                            ledgeritem.GeneralLedgerVmLines.Add(netChangeLine);

                            #endregion

                            #endregion
                            #region ending

                            var endingRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == period.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in endingRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int)balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLines.Add(lines);
                            }

                            #endregion
                        }

                        if (lastPeriod != null)
                        {
                            #region ending of last period

                            var lastendingRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == lastPeriod.GLPeriodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in lastendingRecords)
                            {
                                var lines = new GeneralLedgerVmLastLines
                                {
                                    Id = balance.GLLedgerPostingLinesID.ToString(),
                                    Date = balance.EffectiveDate,
                                    Reference = balance.JournalReference,
                                    Description = balance.Description,
                                    Journal = balance.CategoryName,
                                    TransDesc = balance.AdditionalDescription,
                                    Debit = balance.Debit,
                                    Credit = balance.Credit,
                                    Balance = balance.Balance,
                                    Type = (int)balance.Type,
                                    Remark = balance.Remark
                                };

                                ledgeritem.GeneralLedgerVmLastLineses.Add(lines);
                            }

                            #endregion
                        }

                        #endregion
                    }

                    ledgeritem.GrandDebit = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Debit);
                    ledgeritem.GrandCredit = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Credit);
                    ledgeritem.GrandBalance = ledgeritem.GeneralLedgerVmLastLineses.Sum(x => x.Balance);

                    balances.Add(ledgeritem);
                }
            }

            return balances.OrderBy(x => x.AccountCode);
        }
        //public IOrderedEnumerable<GeneralLedgerVm> GetLedgerBalance(string fromperiod, string toperiod, string acctFrom, string acctTo)
        //{
        //    List<GeneralLedgerVm> balances = new List<GeneralLedgerVm>();
        //    if (!String.IsNullOrEmpty(fromperiod) && !String.IsNullOrEmpty(toperiod))
        //    {
        //        int from = int.Parse(fromperiod);
        //        int to = int.Parse(toperiod);

        //        DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
        //        DateTime dt22 = _db.GLPeriods.Find(to).DateTo;
        //        int min = 0;

        //        var accountRange = _db.GLChartOfAccountAccounts
        //            .Where(x => String.Compare(x.Values.ToUpper(), acctFrom, StringComparison.Ordinal) >= 0)
        //            .Where(x => String.Compare(x.Values.ToUpper(), acctTo, StringComparison.Ordinal) <= 0)
        //            .Select(x => x.Values).ToList();

        //        var ledgerposts = _db.GLLedgerPostings.ToList();
        //        List<GLLedgerPosting> ledgers = new List<GLLedgerPosting>();
        //        foreach (var acct in accountRange)
        //        {
        //            var acct1 = acct;
        //            var exist = ledgerposts.Where(x => x.AccountSegment == acct1);
        //            var glLedgerPostings = exist as IList<GLLedgerPosting> ?? exist.ToList();
        //            if (glLedgerPostings.Any())
        //            {
        //                ledgers.AddRange(glLedgerPostings.ToList());
        //            }
        //        }

        //        var ledgerPostingLines = _db.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22).ToList();


        //        foreach (var ledgerPosting in ledgers)
        //        {
        //            var selectedledgerPostingLines = ledgerPostingLines.Where(x => x.GLLedgerPosting.Description == ledgerPosting.Description).ToList();

        //            var ledgeritem = new GeneralLedgerVm
        //            {
        //                Description = ledgerPosting.Description,
        //                AccountDescription = FinanceHelper.GetAccountDesc(ledgerPosting.Description),
        //                GeneralLedgerVmLines = new List<GeneralLedgerVmLines>()
        //            };

        //            if (selectedledgerPostingLines.Any())
        //            {
        //                min = selectedledgerPostingLines.Min(x => x.GLLedgerPostingLinesID);
        //            }

        //            #region beginning

        //            var beginning = new GeneralLedgerVmLines();
        //            if (min != 0)
        //            {
        //                var temp = selectedledgerPostingLines.Where(x => x.GLLedgerPostingLinesID < min).ToList();
        //                if (temp.Any())
        //                {
        //                    int beginningId = temp.Max(x => x.GLLedgerPostingLinesID);
        //                    var beginningLine = selectedledgerPostingLines.Find(x => x.GLLedgerPostingLinesID == beginningId);

        //                    if (beginningLine != null)
        //                    {
        //                        beginning.Id = beginningLine.GLLedgerPostingLinesID.ToString();
        //                        beginning.Date = beginningLine.EffectiveDate;
        //                        beginning.Reference = beginningLine.JournalReference;
        //                        beginning.Journal = beginningLine.CategoryName;
        //                        beginning.TransDesc = "Beginning Balance";
        //                        beginning.Debit = beginningLine.Debit;
        //                        beginning.Credit = beginningLine.Credit;
        //                        beginning.Balance = beginningLine.Balance;
        //                        ledgeritem.GeneralLedgerVmLines.Add(beginning);
        //                    }
        //                }
        //                else
        //                {
        //                    beginning.Id = "0";
        //                    beginning.Date = null;
        //                    beginning.Reference = "";
        //                    beginning.Journal = "";
        //                    beginning.TransDesc = "Beginning Balance";
        //                    beginning.Debit = 0;
        //                    beginning.Credit = 0;
        //                    beginning.Balance = 0;
        //                    ledgeritem.GeneralLedgerVmLines.Add(beginning);
        //                }

        //            }
        //            else
        //            {
        //                beginning.Id = "0";
        //                beginning.Date = null;
        //                beginning.Reference = "";
        //                beginning.Journal = "";
        //                beginning.TransDesc = "Beginning Balance";
        //                beginning.Debit = 0;
        //                beginning.Credit = 0;
        //                beginning.Balance = 0;
        //                ledgeritem.GeneralLedgerVmLines.Add(beginning);
        //            }

        //            #endregion
        //            #region current transaction

        //            if (selectedledgerPostingLines.Any())
        //            {
        //                var itemNewBinCardLines = selectedledgerPostingLines.Where(x => x.GLLedgerPostingLinesID >= min).ToList();

        //                foreach (var balance in itemNewBinCardLines)
        //                {
        //                    var lines = new GeneralLedgerVmLines
        //                    {
        //                        Id = balance.GLLedgerPostingLinesID.ToString(),
        //                        Date = balance.EffectiveDate,
        //                        Reference = balance.JournalReference,
        //                        Journal = balance.CategoryName,
        //                        TransDesc = balance.Description,
        //                        Debit = balance.Debit,
        //                        Credit = balance.Credit,
        //                        Balance = balance.Balance
        //                    };

        //                    ledgeritem.GeneralLedgerVmLines.Add(lines);
        //                }
        //            }

        //            #endregion




        //            balances.Add(ledgeritem);
        //        }
        //    }

        //    return balances.OrderBy(x => x.Description);
        //}
        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
