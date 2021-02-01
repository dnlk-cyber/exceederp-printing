using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Receivable.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.Finance.ViewModel.GAAP;

namespace ExceedERP.Reporting.Finance.Receivable.DataSource
{

    [DataObject]
    public class RevenuByBranchOds : List<RevenuByBranchViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly ReportHelper _reportHelper = new ReportHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RevenuByBranchViewModel> GetRevenuByBranch(string periodFrom,string periodTo)
        {
            List<RevenuByBranchViewModel> revenuLists = new List<RevenuByBranchViewModel>();
            var revenueBySalesType = _db.RevenueBySalesTypes.FirstOrDefault();
            var revenueListRanges = _db.RevenueListRanges.ToList();

            if (revenueBySalesType != null && !String.IsNullOrEmpty(periodFrom))
            {
                var fromPeriod = _db.GLPeriods.Find(int.Parse(periodFrom));
                var toPeriod = _db.GLPeriods.Find(int.Parse(periodTo));
                if (fromPeriod != null && toPeriod!=null)
                {
                    var fromBalance =
                        _reportHelper.GetAccountBalance(fromPeriod.GlFiscalYearId, fromPeriod.GLPeriodId);
                    var toBalance =
                        _reportHelper.GetAccountBalance(toPeriod.GlFiscalYearId, toPeriod.GLPeriodId);

                    var periodRangeBalance =
                        _reportHelper.GetAccountBalanceByPeriodRange(toPeriod.GlFiscalYearId, fromPeriod.GLPeriodId, toPeriod.GLPeriodId);

                    #region revenue

                    foreach (var con in revenueBySalesType.Revenues)
                    {
                        var connote = new RevenuByBranchViewModel
                        {
                            AccountNo = con.Description,
                            ServiceType = "",
                            RevenuByBranchList = new List<RevenuByBranchDetail>()
                        };

                        var branches = _db.Branch.ToList();
                        foreach (var branch in branches)
                        {
                            decimal begining = 0;
                            decimal debitchange = 0;
                            decimal creditchange = 0;
                            decimal ending = 0;


                            //get range
                            var range = revenueListRanges.Where(x => x.RevenueListId == con.RevenueListId).ToList();
                            foreach (var r in range)
                            {
                                begining = begining + _reportHelper.GetAccountBegBalancesWithSegmentRangeAndBranch(
                                               fromBalance, r.AccountFrom,r.AccountTo, branch.LocationAccount);

                                debitchange = debitchange + _reportHelper.GetAccountDebitBalancesWithSegmentRangeAndBranch(periodRangeBalance,
                                                  r.AccountFrom, r.AccountTo, branch.LocationAccount);

                                creditchange = creditchange + _reportHelper.GetAccountCreditBalancesWithSegmentRangeAndBranch(periodRangeBalance,
                                                   r.AccountFrom, r.AccountTo, branch.LocationAccount);

                                ending = ending + _reportHelper.GetAccountEndBalancesWithSegmentRangeAndBranch(toBalance,
                                             r.AccountFrom, r.AccountTo, branch.LocationAccount);
                            }

                            var detail = new RevenuByBranchDetail
                            {
                                Branch = branch.Name,
                                BeginningBalance = begining,
                                DebitChange = debitchange,
                                CreditChange = creditchange,
                                NetChange = debitchange- creditchange,
                                EndingBalance = ending,

                            };
                            connote.RevenuByBranchList.Add(detail);
                        }

                        connote.TotalBeginningBalance = connote.RevenuByBranchList.Sum(x => x.BeginningBalance);
                        connote.TotalDebitChange = connote.RevenuByBranchList.Sum(x => x.DebitChange);
                        connote.TotalCreditChange = connote.RevenuByBranchList.Sum(x => x.CreditChange);
                        connote.TotalNetChange = connote.RevenuByBranchList.Sum(x => x.NetChange);
                        connote.TotalEndingBalance = connote.RevenuByBranchList.Sum(x => x.EndingBalance);

                        revenuLists.Add(connote);

                    }

                    #endregion
                }

            }

            return revenuLists;
        }
        //public List<RevenuByBranchViewModel> GetRevenuByBranch(string period)
        //{
        //    List<RevenuByBranchViewModel> revenuLists = new List<RevenuByBranchViewModel>();
        //    var revenueBySalesType = _db.RevenueBySalesTypes.FirstOrDefault();
        //    var revenueListRanges = _db.RevenueListRanges.ToList();

        //    if (revenueBySalesType != null && !String.IsNullOrEmpty(period))
        //    {
        //        var periodObj = _db.GLPeriods.Find(int.Parse(period));
        //        if (periodObj != null)
        //        {
        //            var currnetYearBalance =
        //                _reportHelper.GetAccountBalance(periodObj.GlFiscalYearId, periodObj.GLPeriodId);

        //            #region revenue

        //            foreach (var con in revenueBySalesType.Revenues)
        //            {
        //                var connote = new RevenuByBranchViewModel
        //                {
        //                    AccountNo = con.Description,
        //                    ServiceType = "",
        //                    RevenuByBranchList = new List<RevenuByBranchDetail>()
        //                };

        //                var branches = _db.Branch.ToList();
        //                foreach (var branch in branches)
        //                {
        //                    decimal begining = 0;
        //                    decimal debitchange = 0;
        //                    decimal creditchange = 0;
        //                    decimal ending = 0;


        //                    //get range
        //                    var range = revenueListRanges.Where(x => x.RevenueListId == con.RevenueListId).ToList();
        //                    foreach (var r in range)
        //                    {
        //                        var segment1Ranges =
        //                            _dynamicAccountHelper.GetAccountRange(r.AccountFrom, r.AccountTo, 0);

        //                        var segment2Ranges =
        //                            _dynamicAccountHelper.GetAccountRange(r.AccountFrom, r.AccountTo, 1);

        //                        var segment3Ranges =
        //                            _dynamicAccountHelper.GetAccountRange(r.AccountFrom, r.AccountTo, 2);

        //                        var segment4Ranges =
        //                            _dynamicAccountHelper.GetAccountRange(r.AccountFrom, r.AccountTo, 3);

        //                        var segment5Ranges =
        //                            _dynamicAccountHelper.GetAccountRange(r.AccountFrom, r.AccountTo, 4);


        //                        foreach (var s1 in segment1Ranges)
        //                        {
        //                            foreach (var s2 in segment2Ranges)
        //                            {
        //                                foreach (var s3 in segment3Ranges)
        //                                {
        //                                    foreach (var s4 in segment4Ranges)
        //                                    {
        //                                        foreach (var s5 in segment5Ranges)
        //                                        {

        //                                            begining = begining + _reportHelper.GetAccountBegBalancesWithSegmentAndBranch(
        //                                                           currnetYearBalance, s1, s2, s3, s4, s5, branch.LocationAccount);

        //                                            debitchange = debitchange + _reportHelper.GetAccountDebitBalancesWithSegmentAndBranch(currnetYearBalance,
        //                                                              s1, s2, s3, s4, s5, branch.LocationAccount);

        //                                            creditchange = creditchange + _reportHelper.GetAccountCreditBalancesWithSegmentAndBranch(currnetYearBalance,
        //                                                               s1, s2, s3, s4, s5, branch.LocationAccount);

        //                                            ending = ending + _reportHelper.GetAccountEndBalancesWithSegmentAndBranch(currnetYearBalance,
        //                                                         s1, s2, s3, s4, s5, branch.LocationAccount);
        //                                        }
        //                                    }

        //                                }

        //                            }

        //                        }
        //                    }

        //                    var detail = new RevenuByBranchDetail
        //                    {
        //                        Branch = branch.Name,
        //                        BeginningBalance = begining,
        //                        DebitChange = debitchange,
        //                        CreditChange = creditchange,
        //                        NetChange = debitchange - creditchange,
        //                        EndingBalance = ending,

        //                    };
        //                    connote.RevenuByBranchList.Add(detail);
        //                }

        //                connote.TotalBeginningBalance = connote.RevenuByBranchList.Sum(x => x.BeginningBalance);
        //                connote.TotalDebitChange = connote.RevenuByBranchList.Sum(x => x.DebitChange);
        //                connote.TotalCreditChange = connote.RevenuByBranchList.Sum(x => x.CreditChange);
        //                connote.TotalNetChange = connote.RevenuByBranchList.Sum(x => x.NetChange);
        //                connote.TotalEndingBalance = connote.RevenuByBranchList.Sum(x => x.EndingBalance);

        //                revenuLists.Add(connote);

        //            }

        //            #endregion
        //        }

        //    }

        //    return revenuLists;
        //}

        //public List<RevenuByBranchViewModel> GetRevenuByBranch(string period)
        //{
        //    List<RevenuByBranchViewModel> revenuLists = new List<RevenuByBranchViewModel>();
        //    var revenueBySalesType = _db.RevenueBySalesTypes.FirstOrDefault();
        //    var revenueListRanges = _db.RevenueListRanges.ToList();

        //    if (revenueBySalesType != null && !String.IsNullOrEmpty(period))
        //    {
        //        var periodObj = _db.GLPeriods.Find(int.Parse(period));
        //        if (periodObj != null)
        //        {
        //            var currnetYearBalance =
        //                _reportHelper.GetAccountBalance(periodObj.GlFiscalYearId, periodObj.GLPeriodId);

        //            #region revenue

        //            foreach (var con in revenueBySalesType.Revenues)
        //            {
        //                var connote = new RevenuByBranchViewModel
        //                {
        //                    AccountNo = con.Description,
        //                    ServiceType = "",
        //                    RevenuByBranchList = new List<RevenuByBranchDetail>()
        //                };

        //                var branches = _db.Branch.ToList();
        //                foreach (var branch in branches)
        //                {
        //                    decimal begining = 0;
        //                    decimal debitchange = 0;
        //                    decimal creditchange = 0;
        //                    decimal ending = 0;


        //                    //get range
        //                    var range = revenueListRanges.Where(x => x.RevenueListId == con.RevenueListId).ToList();
        //                    foreach (var r in range)
        //                    {
        //                        string acctFrom = _accountHelper.GetNaturalAccountSegment(r.AccountFrom);
        //                        string acctTo = _accountHelper.GetNaturalAccountSegment(r.AccountTo);
        //                        var accountRange = _db.GLChartOfAccountAccounts
        //                            .Where(x =>
        //                                String.Compare(x.Values.ToUpper(), acctFrom, StringComparison.Ordinal) >= 0)
        //                            .Where(x => String.Compare(x.Values.ToUpper(), acctTo, StringComparison.Ordinal) <=
        //                                        0)
        //                            .ToList();
        //                        foreach (var accRange in accountRange)
        //                        {
        //                            begining = begining + _reportHelper.GetAccountBegBalancesWithBranch(
        //                                           currnetYearBalance, accRange.Values, branch.LocationAccount);

        //                            debitchange = debitchange + _reportHelper.GetAccountDebitBalancesWithBranch(currnetYearBalance,
        //                                    accRange.Values, branch.LocationAccount);

        //                            creditchange = creditchange + _reportHelper.GetAccountCreditBalancesWithBranch(currnetYearBalance,
        //                                    accRange.Values, branch.LocationAccount);

        //                            ending = ending + _reportHelper.GetAccountEndBalancesWithBranch(currnetYearBalance,
        //                                         accRange.Values, branch.LocationAccount);
        //                        }
        //                    }

        //                    var detail = new RevenuByBranchDetail
        //                    {
        //                        Branch = branch.Name,
        //                        BeginningBalance = begining,
        //                        DebitChange = debitchange,
        //                        CreditChange = creditchange,
        //                        NetChange = debitchange - creditchange,
        //                        EndingBalance = ending,

        //                    };
        //                    connote.RevenuByBranchList.Add(detail);
        //                }

        //                connote.TotalBeginningBalance = connote.RevenuByBranchList.Sum(x => x.BeginningBalance);
        //                connote.TotalDebitChange = connote.RevenuByBranchList.Sum(x => x.DebitChange);
        //                connote.TotalCreditChange = connote.RevenuByBranchList.Sum(x => x.CreditChange);
        //                connote.TotalNetChange = connote.RevenuByBranchList.Sum(x => x.NetChange);
        //                connote.TotalEndingBalance = connote.RevenuByBranchList.Sum(x => x.EndingBalance);

        //                revenuLists.Add(connote);

        //            }

        //            #endregion
        //        }

        //    }

        //    return revenuLists;
        //}
    }
}
