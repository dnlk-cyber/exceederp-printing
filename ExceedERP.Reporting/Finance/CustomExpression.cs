using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Reporting.Finance
{
    public static class CustomExpression
    {
        private static readonly ExceedDbContext Db = new ExceedDbContext();
        

        public static string GetArAginglabel(int period)
        {
            string label = "";
            var periodSetup = Db.AgingPeriodSetups.FirstOrDefault(x => x.Type == AgingType.AccountReceivable);
            if (periodSetup != null)
            {
                switch (period)
                {
                    case 1:
                        label =  "1-" + periodSetup.Period1.ToString() + " Days";
                        break;
                    case 2:
                        label = periodSetup.Period1.ToString() + "-" + periodSetup.Period2.ToString() + " Days";
                        break;
                    case 3:
                        label = periodSetup.Period2.ToString() + "-" + periodSetup.Period3.ToString() + " Days";
                        break;
                    case 4:
                        label = periodSetup.Period3.ToString() + " + Days";
                        break;
                    default:
                        break;
                }
                
            }

            return label;
        }
        public static string GetApAginglabel(int period)
        {
            string label = "";
            var periodSetup = Db.AgingPeriodSetups.FirstOrDefault(x => x.Type == AgingType.AccountPayable);
            if (periodSetup != null)
            {
                switch (period)
                {
                    case 1:
                        label = "1-" + periodSetup.Period1.ToString() + " Days";
                        break;
                    case 2:
                        label = periodSetup.Period1.ToString() + "-" + periodSetup.Period2.ToString() + " Days";
                        break;
                    case 3:
                        label = periodSetup.Period2.ToString() + "-" + periodSetup.Period3.ToString() + " Days";
                        break;
                    case 4:
                        label = periodSetup.Period3.ToString() + " + Days";
                        break;
                    default:
                        break;
                }

            }

            return label;
        }
        public static string GetTaxName(int id)
        {
            string tax = "";
            var taxRate = Db.GLTaxRates.Find(id);
            if (taxRate != null)
            {
                tax = taxRate.TaxName;
            }

            return tax;
        }
        public static DateTime GetFYearEndDate(string fyear)
        {
            DateTime enddate = DateTime.Today;
            if (fyear != null)
            {
                var glyear = Db.GLFiscalYears.Find(int.Parse(fyear));
                if (glyear != null)
                {
                    enddate = glyear.DateTo;
                }
            }

            return enddate;
        }
        public static string GetFYearEndYear(string fyear)
        {
            string enddate = "Current Fiscal Year";
            if (fyear != null)
            {
                var fiscalYear = Db.GLFiscalYears.Find(int.Parse(fyear));
                if (fiscalYear != null)
                {
                    enddate = fiscalYear.Name;
                }
            }

            return enddate+" (ETB)";
        }

        public static string GetPreviousFYearEndYear(string fyear)
        {
            FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();
            string enddate = "Previous Fiscal Year";
            if (fyear != null)
            {
                int prevyear = _fiscalYearHelper.GetPreviousFYearId(int.Parse(fyear));
                if (prevyear != null)
                {
                    var glyear = Db.GLFiscalYears.Find(prevyear);
                    if (glyear != null)
                    {
                        enddate = glyear.Name;
                    }

                }
            }
            return enddate + " (ETB)";
        }

        public static string GetNumberInWords(decimal number)
        {
            string newNumber = number.ToString();
            newNumber = XAPI.NumberExtensions.toWords((int)number);
            return newNumber;
        }
        public static string GetPaddedLeftVoucher(string voucherid)
        {
            return voucherid.PadLeft(5, '0');
        }
    }
}
