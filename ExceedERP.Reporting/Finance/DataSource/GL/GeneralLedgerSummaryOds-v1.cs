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
    public class GeneralLedgerSummaryOds : List<StockBalanceOds>
    {
        /**
       * v1 algorithm slower by 6 times from v1
      
       */
        readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();

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
                            x.Segment8

                        }).AsNoTracking().AsQueryable();


                    //var ledgerPostingLines = _db.GLLedgerPostingLines.Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22).ToList();
                    var ledgerPostingLines = _db.GLLedgerPostingLines
                        .Where(b => b.EffectiveDate >= dt11 && b.EffectiveDate <= dt22)
                        .Select(x => new
                        {
                            x.ReferenceDoc,
                            x.InvoiceReferenceDoc,
                            x.PaymentReference,
                            x.AdditionalDescription,
                            x.EffectiveDate,
                            x.JournalReference,
                            x.CategoryName,
                            x.Remark,
                            x.Description,
                            x.GLPeriodId,
                            x.Type,
                            x.Debit,
                            x.Credit,
                            x.Balance,
                            x.GLLedgerPostingID,
                            x.GLLedgerPosting,
                            x.GLLedgerPostingLinesID
                        })
                        .AsNoTracking().ToList();


                    if (!String.IsNullOrEmpty(costcenter))
                        ledgers = ledgers.Where(x => x.CostCenterSegment == costcenter).AsQueryable();

                    if (!String.IsNullOrEmpty(subaccount))
                        ledgers = ledgers.Where(x => x.SubAccountSegment == subaccount).AsQueryable();

                    if (!String.IsNullOrEmpty(segmentType) && !String.IsNullOrEmpty(segmentValue))
                    {
                        if (_accountHelper.IsDynamicCoA())
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
                        else
                        {
                            switch (segmentType)
                            {
                                case "Branch":
                                    ledgers = ledgers.Where(x => x.BranchSegment == segmentValue).AsQueryable();
                                    break;

                                case "Cost Center":
                                    ledgers = ledgers.Where(x => x.CostCenterSegment == segmentValue).AsQueryable();
                                    break;

                                case "Sub Account":
                                    ledgers = ledgers.Where(x => x.SubAccountSegment == segmentValue).AsQueryable();
                                    break;
                            }
                        }
                    }

                    if (summarizeByAccount && showSubAccount)
                    {
                        #region summarize by natural account and sub account

                        List<string> naturalAccounts = ledgers.Select(x => x.AccountSegment).Distinct().ToList();

                        foreach (var naturalAccount in naturalAccounts)
                        {
                            List<string> subAccounts = ledgers.Where(x=>x.AccountSegment==naturalAccount).Select(x => x.SubAccountSegment).Distinct().ToList();
                            var acc = _db.GLChartOfAccountAccounts.FirstOrDefault(x => x.Values == naturalAccount);
                            if (acc != null)
                            {
                               
                                if (ledgerPostingLines.Any())
                                {
                                    foreach (var subAccount in subAccounts)
                                    {
                                        string subAccDesc = "";
                                        if(_accountHelper.IsDynamicCoA())
                                        {
                                            var type = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                                            var subAcc = _db.GlChartOfAccountSegments.Where(x=>x.Values==subAccount).FirstOrDefault(x => x.GLSegmentSetupId == type.GLSegmentSetupId);
                                            subAccDesc = subAcc?.Description;
                                        }
                                        else
                                        {
                                            var subAcc = _db.GLChartOfAccountSubAccounts.FirstOrDefault(x => x.Values == subAccount);
                                            subAccDesc = subAcc?.Description;
                                        }
                                        var lineData = new GeneralLedgerAccountVm
                                        {
                                            AccountCode = acc.Values+"-"+ subAccount,
                                            AccountDescription = acc.Description+"-"+subAccDesc
                                        };
                                        //beg of first month
                                        var beginningBalance = ledgerPostingLines
                                            .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subAccount)
                                            .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                            .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                                        var standardBalance = ledgerPostingLines
                                            .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subAccount)
                                            .Where(x => x.Type == LedgerPostTypes.Standard).ToList();
                                        //ending of last month
                                        var endingBalance = ledgerPostingLines
                                            .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                            .Where(x => x.GLLedgerPosting.SubAccountSegment == subAccount)
                                            .Where(x => x.GLPeriodId.ToString() == periodTo)
                                            .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                                        #region get sum data

                                        var begining = beginningBalance.Sum(x => x.Balance);
                                        var debitchange = standardBalance.Sum(x => x.Debit);
                                        var creditchange = standardBalance.Sum(x => x.Credit);
                                        var ending = endingBalance.Sum(x => x.Balance);

                                        decimal netChange = 0;
                                        if (acc.AccountType == GlAccountTypes.Asset ||
                                            acc.AccountType == GlAccountTypes.Expense)
                                        {
                                            netChange = debitchange - creditchange;
                                        }
                                        else
                                        {
                                            netChange = creditchange - debitchange;
                                        }

                                        #region data

                                        lineData.BeginningBalance = begining;
                                        lineData.Debit = debitchange;
                                        lineData.Credit = creditchange;
                                        lineData.NetChange = netChange;
                                        lineData.Balance = ending;
                                        glBalances.Add(lineData);
                                    }

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
                                
                                if (ledgerPostingLines.Any())
                                {
                                    var lineData = new GeneralLedgerAccountVm
                                    {
                                        AccountCode = acc.Values,
                                        AccountDescription = acc.Description
                                    };
                                    //beg of first month
                                    var beginningBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                        .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                        .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                                    var standardBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                        .Where(x => x.Type == LedgerPostTypes.Standard).ToList();
                                    //ending of last month
                                    var endingBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountSegment == naturalAccount)
                                        .Where(x => x.GLPeriodId.ToString() == periodTo)
                                        .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                                    #region get sum data

                                    var begining = beginningBalance.Sum(x => x.Balance);
                                    var debitchange = standardBalance.Sum(x => x.Debit);
                                    var creditchange = standardBalance.Sum(x => x.Credit);
                                    var ending = endingBalance.Sum(x => x.Balance);

                                    decimal netChange = 0;
                                    if (acc.AccountType == GlAccountTypes.Asset ||
                                        acc.AccountType == GlAccountTypes.Expense)
                                    {
                                        netChange = debitchange - creditchange;
                                    }
                                    else
                                    {
                                        netChange = creditchange - debitchange;
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
                    else 
                    {
                        #region whole account combination
                        
                            if (ledgerPostingLines.Any())
                            {
                               
                                foreach (var ledgerPosting in ledgers)
                                {
                                    //beg of first month
                                    var beginningBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode == ledgerPosting.AccountCode)
                                        .Where(x => x.GLPeriodId.ToString() == periodFrom)
                                        .Where(x => x.Type == LedgerPostTypes.Begining).ToList();

                                    var standardBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode == ledgerPosting.AccountCode)
                                        .Where(x => x.Type == LedgerPostTypes.Standard).ToList();

                                             //ending of last month
                                var endingBalance = ledgerPostingLines
                                        .Where(x => x.GLLedgerPosting.AccountCode == ledgerPosting.AccountCode)
                                        .Where(x => x.GLPeriodId.ToString() == periodTo)
                                        .Where(x => x.Type == LedgerPostTypes.Ending).ToList();

                                #region get sum data

                                var begining = beginningBalance.Sum(x => x.Balance);
                                    var debitchange = standardBalance.Sum(x => x.Debit);
                                    var creditchange = standardBalance.Sum(x => x.Credit);
                                    var ending = endingBalance.Sum(x => x.Balance);

                                    decimal netChange = 0;
                                    if (ledgerPosting.AccountType == GlAccountTypes.Asset ||
                                        ledgerPosting.AccountType == GlAccountTypes.Expense)
                                    {
                                        netChange = debitchange - creditchange;
                                    }
                                    else
                                    {
                                        netChange = creditchange - debitchange;
                                    }

                                    #region data

                                    var lineData = new GeneralLedgerAccountVm
                                    {
                                        AccountCode = ledgerPosting.AccountCode,
                                        AccountDescription = ledgerPosting.AccountDesc,
                                        BeginningBalance = begining,
                                        Debit = debitchange,
                                        Credit = creditchange,
                                        NetChange = netChange,
                                        Balance = ending
                                    };

                                    glBalances.Add(lineData);

                                    #endregion

                                    #endregion

                                }
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

    }
}
