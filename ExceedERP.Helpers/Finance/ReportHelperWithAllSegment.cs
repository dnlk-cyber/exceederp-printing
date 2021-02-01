using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.GL;
using static System.String;

namespace ExceedERP.Helpers.Finance
{
    public partial class ReportHelper
    {
        //with all segment
        public decimal GetAccountEndBalancesWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public decimal GetAccountEndBalancesByBranchWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom,string branchAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }


        public decimal GetAccountStandardBalancesWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Standard)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public decimal GetAccountBegBalancesWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountBegBalancesByBranchWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string branchAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        //with all segment and natural account
        public decimal GetAccountBegBalancesByAccountWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string account)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();

            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountEndBalancesByAccountWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom,string account)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();
                
            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Where(x=>x.GLLedgerPosting.AccountSegment==account)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public decimal GetAccountBegBalancesByAccountAndBranchWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string account, string branchAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();

            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Select(x=>new {x.GLLedgerPosting,x.Balance}).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountEndBalancesByAccountAndBranchWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string account, string branchAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();

            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBranchAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();

                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == branchAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == branchAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == branchAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == branchAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == branchAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == branchAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == branchAccount).ToList();
                        break;

                }

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        public decimal GetAccountBegBalancesByAccountAndBizUnitWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string account, string bizUnitAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();

            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBizUnitAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == bizUnitAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == bizUnitAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == bizUnitAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == bizUnitAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == bizUnitAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == bizUnitAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == bizUnitAccount).ToList();
                        break;

                }

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
        public decimal GetAccountEndBalancesByAccountAndBizUnitWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string account, string bizUnitAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            int accountindex = 0;
            accountindex = _accountHelper.IsDynamicCoA() ? _accountHelper.GetAccountSequenceIndex(GlSegmentTypes.Account) : _dynamicAccountHelper.GetNaturalAccountSequenceIndex();

            switch (accountindex)
            {
                case 0:
                    seg1From = account;
                    seg1To = account;
                    break;
                case 1:
                    seg2From = account;
                    seg2To = account;
                    break;
                case 2:
                    seg3From = account;
                    seg3To = account;
                    break;
                case 3:
                    seg4From = account;
                    seg4To = account;
                    break;
                case 4:
                    seg5From = account;
                    seg5To = account;
                    break;
            }

            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBizUnitAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Where(x => x.GLLedgerPosting.AccountSegment == account)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();

                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == bizUnitAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == bizUnitAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == bizUnitAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == bizUnitAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == bizUnitAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == bizUnitAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == bizUnitAccount).ToList();
                        break;

                }

                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }

        //with all segment and business unit account
        public decimal GetAccountBegBalancesByBizUnitWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string bizUnitAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearBegBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBizUnitAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Begining)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == bizUnitAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == bizUnitAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == bizUnitAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == bizUnitAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == bizUnitAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == bizUnitAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == bizUnitAccount).ToList();
                        break;

                }
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearBegBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearBegBalances;
        }
        public decimal GetAccountEndBalancesByBizUnitWithAllranges(List<GLLedgerPostingLines> yearBalance, string fromAccCom, string toAccCom, string bizUnitAccount)
        {
            int accountLength = _accountHelper.GetAccountCombLength();
            decimal accountyearEndBalances = 0;
            string seg1From = "";
            string seg1To = "";
            string seg2From = "";
            string seg2To = "";
            string seg3From = "";
            string seg3To = "";
            string seg4From = "";
            string seg4To = "";
            string seg5From = "";
            string seg5To = "";

            switch (accountLength)
            {
                case 1:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    break;

                case 2:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    break;
                case 3:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    break;
                case 4:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    break;
                case 5:
                    seg1From = fromAccCom.Split('-')[0];
                    seg1To = toAccCom.Split('-')[0];
                    seg2From = fromAccCom.Split('-')[1];
                    seg2To = toAccCom.Split('-')[1];
                    seg3From = fromAccCom.Split('-')[2];
                    seg3To = toAccCom.Split('-')[2];
                    seg4From = fromAccCom.Split('-')[3];
                    seg4To = toAccCom.Split('-')[3];
                    seg5From = fromAccCom.Split('-')[4];
                    seg5To = toAccCom.Split('-')[4];
                    break;
            }


            if (yearBalance.Any())
            {
                int index = _dynamicAccountHelper.GetBizUnitAccountSequenceIndex();
                var balanceObj = yearBalance.Where(x => x.Type == LedgerPostTypes.Ending)
                    .Select(x => new { x.GLLedgerPosting, x.Balance }).ToList();
                switch (index)
                {
                    case 0:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment1 == bizUnitAccount).ToList();
                        break;
                    case 1:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment2 == bizUnitAccount).ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment3 == bizUnitAccount).ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment5 == bizUnitAccount).ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment6 == bizUnitAccount).ToList();
                        break;
                    case 6:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment7 == bizUnitAccount).ToList();
                        break;
                    case 7:
                        balanceObj = balanceObj.Where(x => x.GLLedgerPosting.Segment8 == bizUnitAccount).ToList();
                        break;

                }
                switch (accountLength)
                {
                    case 1:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 2:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 3:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .ToList();
                        break;
                    case 4:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;
                    case 5:
                        balanceObj = balanceObj
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment1.ToUpper(), seg1To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment2.ToUpper(), seg2To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment3.ToUpper(), seg3To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment4.ToUpper(), seg4To.ToUpper(), StringComparison.Ordinal) <= 0)

                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5From.ToUpper(), StringComparison.Ordinal) >= 0)
                            .Where(x => Compare(x.GLLedgerPosting.Segment5.ToUpper(), seg5To.ToUpper(), StringComparison.Ordinal) <= 0)
                            .ToList();
                        break;

                }
                accountyearEndBalances = balanceObj.Sum(x => x.Balance);
            }

            return accountyearEndBalances;
        }
    }
}
