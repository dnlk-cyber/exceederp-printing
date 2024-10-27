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
                                                .AsNoTracking()
                                                .ToArray();

				var sortedPeriods = periodList.OrderBy(x => x.DateTo).Select(a=>a.GLPeriodId).ToArray();


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
                        //x.Segment6,
                        //x.Segment7,
                        //x.Segment8,
                       // x.GLLedgerPostingLines
                        GLLedgerPostingLines = x.GLLedgerPostingLines
                        .Where(l => sortedPeriods.Contains(l.GLPeriodId))
						.Select(l => new { 
                            l.GLLedgerPostingLinesID,
                            l.EffectiveDate,
                            l.GLPeriodId,
                            l.Debit,
                            l.Credit,
                            l.Balance,
                            l.Type,
                            l.Remark,
                            l.JournalReference,
                            l.CategoryName,
                            l.AdditionalDescription,
                            l.Description,
                            l.ReferenceDoc,
                            l.InvoiceReferenceDoc,
                            l.PaymentReference
                        })


					})
                    .AsNoTracking().AsQueryable();



				var lastPeriod = periodList.OrderByDescending(x => x.DateTo).Take(1).FirstOrDefault();

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

                        //case 6:
                        //    ledgers = ledgers.Where(x => x.Segment6 == segmentValue).AsQueryable();
                        //    break;
                        //case 7:
                        //    ledgers = ledgers.Where(x => x.Segment7 == segmentValue).AsQueryable();
                        //    break;
                        //case 8:
                        //    ledgers = ledgers.Where(x => x.Segment8 == segmentValue).AsQueryable();
                        //    break;
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
                       
                        foreach (var periodId in sortedPeriods)
                        {
                            #region ending

                            var endingRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == periodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToList();
                            foreach (var balance in endingRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID,
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
                        
                        foreach (var periodId in sortedPeriods)
                        {
                            #region beginning

                            var begginningRecords = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId == periodId)
                                .Where(x => x.Type == LedgerPostTypes.Begining).ToArray();
                            foreach (var balance in begginningRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID,
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
                                .Where(x => x.GLPeriodId == periodId)
                                .Where(x => x.Type == LedgerPostTypes.Standard)
								.OrderBy(x => x.EffectiveDate).ThenBy(x => x.GLLedgerPostingLinesID)
								.ToArray();
                            foreach (var balance in standardRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID,
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
                                .Where(x => x.GLPeriodId == periodId)
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToArray();
                            foreach (var balance in endingRecords)
                            {
                                var lines = new GeneralLedgerVmLines
                                {
                                    Id = balance.GLLedgerPostingLinesID,
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
                                .Where(x => x.Type == LedgerPostTypes.Ending).ToArray();
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
      
        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
