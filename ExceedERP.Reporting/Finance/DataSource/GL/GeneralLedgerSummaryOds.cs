using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.GL
{
    [DataObject]
    public class GeneralLedgerSummaryOds : List<StockBalanceOds>, IDisposable
    {
        /**
        * v2 algorithm faster by 10 times... reduced from 7 minutes to 26 seconds
        * optimization using include and selecting only required properties
        */
        private readonly ExceedDbContext _db;
        private readonly AccountHelper _accountHelper;
        public GeneralLedgerSummaryOds()
        {
            _db = new ExceedDbContext();
            _accountHelper = new AccountHelper();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public GeneralLedgerHeaderVm GetGeneralLedgerHeader(string periodFrom, string periodTo, string acctFrom, string acctTo)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            GeneralLedgerHeaderVm headerDs = new GeneralLedgerHeaderVm
            {
                AccountFrom = acctFrom,
                AccountTo = acctTo
            };

            if (!String.IsNullOrEmpty(periodFrom)&&!String.IsNullOrEmpty(periodTo))
            {
                var glPeriodFrom = _db.GLPeriods.Find(int.Parse(periodFrom));
                var glPeriodTo = _db.GLPeriods.Find(int.Parse(periodTo));

                if (glPeriodFrom != null&& glPeriodTo != null)
                {
                    headerDs.DateFrom = glPeriodFrom.DateFrom.Date.ToShortDateString();
                    headerDs.DateTo = glPeriodTo.DateTo.Date.ToShortDateString();
                }
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
        public IOrderedEnumerable<GeneralLedgerAccountVm> GetLedgerBalanceSummary(string periodFrom, string periodTo, string acctFrom, string acctTo,string subaccount,string costcenter, string segmentType, string segmentValue, bool summarizeByAccount,bool showSubAccount)
        {
            _db.Database.CommandTimeout = 7200;//added
            List<GeneralLedgerAccountVm> glBalances = new List<GeneralLedgerAccountVm>();
            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                var glPeriodFrom = _db.GLPeriods.Find(int.Parse(periodFrom));
                var glPeriodTo = _db.GLPeriods.Find(int.Parse(periodTo));
               
                if (glPeriodFrom != null && glPeriodTo != null)
                {
                    DateTime dt11 = glPeriodFrom.DateFrom.Date;
                    DateTime dt22 = glPeriodTo.DateTo.Date;

                    var ledgers = _db.GLLedgerPostings
                        .Include(x => x.GLLedgerPostingLines)
                        
                        .Where(x => String.Compare(x.AccountSegment.ToUpper(), acctFrom.ToUpper(), StringComparison.Ordinal) >=
                                    0)
                        .Where(x => String.Compare(x.AccountSegment.ToUpper(), acctTo.ToUpper(), StringComparison.Ordinal) <=
                                    0)
                       
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
                            GLLedgerPostingLines= x.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22)

                        })
                        .AsNoTracking().AsQueryable();


                    //old var ledgerPostingLines = _db.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22).ToList();
                    //var ledgerPostingLines = _db.GLLedgerPostingLines
                    //    .Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22)
                    //    .Select(x => new
                    //    {
                    //        x.ReferenceDoc,
                    //        x.InvoiceReferenceDoc,
                    //        x.PaymentReference,
                    //        x.AdditionalDescription,
                    //        x.EffectiveDate,
                    //        x.JournalReference,
                    //        x.CategoryName,
                    //        x.Remark,
                    //        x.Description,
                    //        x.GLPeriodId,
                    //        x.Type,
                    //        x.Debit,
                    //        x.Credit,
                    //        x.Balance,
                    //        x.GLLedgerPostingID,
                    //        x.GLLedgerPosting,
                    //        x.GLLedgerPostingLinesID
                    //    })
                    //    .AsNoTracking().ToList();


                    if (!String.IsNullOrEmpty(costcenter))
                        ledgers = ledgers.Where(x => x.CostCenterSegment == costcenter).AsQueryable();

                    if (!String.IsNullOrEmpty(subaccount))
                        ledgers = ledgers.Where(x => x.SubAccountSegment == subaccount).AsQueryable();

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

                    if (summarizeByAccount && showSubAccount)
                    {
                        #region summarize by natural account and sub account

                        List<string> naturalAccounts = ledgers.Select(x => x.AccountSegment).Distinct().ToList();

                        foreach (var naturalAccount in naturalAccounts)
                        {
                            List<string> subAccounts = ledgers
                                .Where(x=>x.AccountSegment==naturalAccount)
                                .Select(x => x.SubAccountSegment).Distinct().ToList();
                            var acc = _db.GLChartOfAccountAccounts.FirstOrDefault(x => x.Values == naturalAccount);
                            if (acc != null)
                            {
                                foreach (var subAccount in subAccounts)
                                {
                                    decimal begining = 0;
                                    decimal debitchange = 0;
                                    decimal creditchange = 0;
                                    decimal ending = 0;
                                    decimal netChange = 0;

                                    string subAccDesc = "";
                                    var type = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                                    var subAcc = _db.GlChartOfAccountSegments.Where(x => x.Values == subAccount).FirstOrDefault(x => x.GLSegmentSetupId == type.GLSegmentSetupId);
                                    subAccDesc = subAcc?.Description;
                                    var lineData = new GeneralLedgerAccountVm
                                    {
                                        AccountCode = acc.Values + "-" + subAccount,
                                        AccountDescription = acc.Description + "-" + subAccDesc
                                    };

                                    //newly added...foreach loop
                                    var includedLedgers = ledgers.Where(x => x.AccountSegment == naturalAccount)
                                        .Where(x => x.SubAccountSegment == subAccount).ToList();
                                    foreach (var includedLedger in includedLedgers)
                                    {


                                        //beg of first month
                                        var beginningBalance = includedLedger.GLLedgerPostingLines
                                            .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                            .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                                        var standardBalance = includedLedger.GLLedgerPostingLines
                                            .Where(x => x.Type == LedgerPostTypes.Standard).ToList();
                                        //ending of last month
                                        var endingBalance = includedLedger.GLLedgerPostingLines
                                            .Where(x => x.GLPeriodId.ToString() == periodTo)
                                            .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                                        #region get sum data

                                        begining = begining + beginningBalance.Sum(x => x.Balance);
                                        debitchange = debitchange + standardBalance.Sum(x => x.Debit);
                                        creditchange = creditchange + standardBalance.Sum(x => x.Credit);
                                        ending = ending + endingBalance.Sum(x => x.Balance);


                                        if (acc.AccountType == GlAccountTypes.Asset ||
                                            acc.AccountType == GlAccountTypes.Expense)
                                        {
                                            netChange = debitchange - creditchange;
                                        }
                                        else
                                        {
                                            netChange = creditchange - debitchange;
                                        }
                                    }

                                    #region data

                                    lineData.BeginningBalance = begining;
                                    lineData.Debit = debitchange;
                                    lineData.Credit = creditchange;
                                    lineData.NetChange = netChange;
                                    lineData.Balance = ending;
                                    glBalances.Add(lineData);
                                    #endregion
                                    #endregion
                                }
                            }
                        }

                        #endregion
                    }
                    else if(summarizeByAccount)
                    {
                        #region summarize by natural account

                        List<string> naturalAccounts = ledgers.Select(x => x.AccountSegment).Distinct().ToList();

                        foreach (var naturalAccount in naturalAccounts)
                        {
                            var acc = _db.GLChartOfAccountAccounts.FirstOrDefault(x => x.Values == naturalAccount);
                            if (acc != null)
                            {

                                decimal begining = 0;
                                decimal debitchange = 0;
                                decimal creditchange =0;
                                decimal ending = 0;
                                decimal netChange = 0;

                                var lineData = new GeneralLedgerAccountVm
                                {
                                    AccountCode = acc.Values,
                                    AccountDescription = acc.Description
                                };
                                //newly added...foreach loop
                                var includedLedgers = ledgers.Where(x => x.AccountSegment == naturalAccount).ToList();
                                foreach (var includedLedger in includedLedgers)
                                {
                                    //beg of first month
                                    var beginningBalance = includedLedger.GLLedgerPostingLines
                                        .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                        .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                                    var standardBalance = includedLedger.GLLedgerPostingLines
                                        .Where(x => x.Type == LedgerPostTypes.Standard).ToList();
                                    //ending of last month
                                    var endingBalance = includedLedger.GLLedgerPostingLines
                                        .Where(x => x.GLPeriodId.ToString() == periodTo)
                                        .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                                    #region get sum data

                                     begining = begining+beginningBalance.Sum(x => x.Balance);
                                     debitchange = debitchange+standardBalance.Sum(x => x.Debit);
                                     creditchange = creditchange+standardBalance.Sum(x => x.Credit);
                                     ending = ending+endingBalance.Sum(x => x.Balance);

                                   
                                    if (acc.AccountType == GlAccountTypes.Asset ||
                                        acc.AccountType == GlAccountTypes.Expense)
                                    {
                                        netChange = debitchange - creditchange;
                                    }
                                    else
                                    {
                                        netChange = creditchange - debitchange;
                                    }
                                }
                               
                               

                                #region data

                                lineData.BeginningBalance = begining;
                                lineData.Debit = debitchange;
                                lineData.Credit = creditchange;
                                lineData.NetChange = netChange;
                                lineData.Balance = ending;
                                glBalances.Add(lineData);

                                #endregion

                                #endregion

                            }
                        }

                        #endregion
                    }
                    else 
                    {
                        #region whole account combination

                        foreach (var ledgerPosting in ledgers)
                        {
                            //beg of first month
                            var beginningBalance = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                            var standardBalance = ledgerPosting.GLLedgerPostingLines
                                .Where(x => x.Type == LedgerPostTypes.Standard).ToList();

                            //ending of last month
                            var endingBalance = ledgerPosting.GLLedgerPostingLines
                                    .Where(x => x.GLPeriodId.ToString() == periodTo)
                                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                            #region get sum data

                            var begining = beginningBalance.Sum(x => x.Balance);
                            var debitsum = standardBalance.Sum(x => x.Debit);
                            var creditsum = standardBalance.Sum(x => x.Credit);
                            var ending = endingBalance.Sum(x => x.Balance);

                            decimal netChange = 0;
                            decimal netDebit = 0;
                            decimal netCredit = 0;
                            if (ledgerPosting.AccountType == GlAccountTypes.Asset ||
                                ledgerPosting.AccountType == GlAccountTypes.Expense)
                            {
                                netChange = debitsum - creditsum;
                                if (netChange > 0)
                                    netDebit = netChange;
                                else
                                    netCredit = Math.Abs(netChange);
                            }
                            else
                            {
                                netChange = creditsum - debitsum;
                                if (netChange > 0)
                                    netCredit = netChange;
                                else
                                    netDebit = Math.Abs(netChange);
                            }

                            #region data

                            var lineData = new GeneralLedgerAccountVm
                            {
                                AccountCode = ledgerPosting.AccountCode,
                                AccountDescription = ledgerPosting.AccountDesc,
                                BeginningBalance = begining,
                                Debit = netDebit,//debitchange,
                                Credit =netCredit,// creditchange,
                                NetChange = netChange,
                                Balance = ending
                            };

                            glBalances.Add(lineData);

                            #endregion

                            #endregion

                        }

                        #endregion
                    }


                }
            }

            return glBalances.OrderBy(x => x.AccountCode);
        }

        //public IOrderedEnumerable<GeneralLedgerAccountVm> GetLedgerBalanceSummary1(string period, string acctFrom, string acctTo, string subaccount, string costcenter)
        //{
        //    List<GeneralLedgerAccountVm> balances = new List<GeneralLedgerAccountVm>();
        //    if (!String.IsNullOrEmpty(period))
        //    {
        //        var glperiod = _db.GLPeriods.Find(int.Parse(period));


        //        if (glperiod != null)
        //        {
        //            DateTime dt11 = glperiod.DateFrom;
        //            DateTime dt22 = glperiod.DateTo;

        //            var ledgers = _db.GLLedgerPostings
        //                .Where(x => Compare(x.AccountSegment.ToUpper(), acctFrom.ToUpper(), StringComparison.Ordinal) >= 0)
        //                .Where(x => Compare(x.AccountSegment.ToUpper(), acctTo.ToUpper(), StringComparison.Ordinal) <= 0).ToList();


        //            var ledgerPostingLines = _db.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22).ToList();


        //            if (!String.IsNullOrEmpty(costcenter))
        //                ledgers = ledgers.Where(x => x.CostCenterSegment == costcenter).ToList();

        //            if (!String.IsNullOrEmpty(subaccount))
        //                ledgers = ledgers.Where(x => x.SubAccountSegment == subaccount).ToList();

        //            foreach (var ledgerPosting in ledgers)
        //            {
        //                var selectedledgerPostingLines = ledgerPostingLines
        //                    .Where(x => x.GLLedgerPosting.Description == ledgerPosting.Description)
        //                    .Where(x => x.Type == LedgerPostTypes.Ending).ToList();


        //                if (selectedledgerPostingLines.Any())
        //                {

        //                    #region current transaction


        //                    #region ending

        //                    var records = selectedledgerPostingLines
        //                        .Where(x => x.GLPeriodId == glperiod.GLPeriodId).ToList();

        //                    foreach (var balance in records)
        //                    {
        //                        var lines = new GeneralLedgerAccountVm
        //                        {
        //                            Id = balance.GLLedgerPostingLinesID.ToString(),
        //                            Description = ledgerPosting.Description,
        //                            AccountDescription = ledgerPosting.AccountDesc,
        //                            Date = balance.EffectiveDate,
        //                            Reference = balance.JournalReference,
        //                            Journal = balance.CategoryName,
        //                            TransDesc = balance.Description,
        //                            Debit = balance.Debit,
        //                            Credit = balance.Credit,
        //                            Balance = balance.Balance,
        //                            Type = (int)balance.Type
        //                        };

        //                        balances.Add(lines);
        //                    }

        //                    #endregion

        //                    #endregion
        //                }




        //            }
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
