using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ExceedERP.Core.Domain.Global.GlobalEnum;

namespace ExceedERP.Helpers.Common
{
    public class GlobalHelper
    {
        public static bool IsDepreciationMonthly()
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {

                bool isbyPeriod = false;

                string reference = SecurityHelper.Encrypt("DepreciationSchedule", XAPI.Common.KEY);


                //string currentValue = SecurityHelper.Encrypt(currentValue, XAPI.Common.KEY);

                var conf = db.Configurations
                    .Where(x => x.Category == "S")
                    .Where(x => x.Reference == reference).FirstOrDefault();

                if (conf != null)
                {
                    string attributeValue = SecurityHelper.Encrypt("Monthly", XAPI.Common.KEY);
                    if (attributeValue == conf.Attribute)
                        isbyPeriod = true;
                }

                return isbyPeriod;
            }
        }
        public static bool IsProcessAllBranchPayroll()
        {
            return false;
        }
        public static bool IsMultipleBusinessUnit()
        {
            ExceedDbContext db = new ExceedDbContext();
            bool isMultiple = false;

            string reference = SecurityHelper.Encrypt("BusinessUnit", XAPI.Common.KEY);

           
            //string currentValue = SecurityHelper.Encrypt(currentValue, XAPI.Common.KEY);

            var conf = db.Configurations
                .Where(x => x.Category == "S")
                .Where(x => x.Reference == reference).FirstOrDefault();

            if(conf!=null)
            {
                string attributeValue = SecurityHelper.Encrypt("Multiple", XAPI.Common.KEY);
                if (attributeValue == conf.Attribute)
                    isMultiple = true;
            }

            return isMultiple;
        }

        public static bool IsPayrollModebranchAndBusinessUnit()
        {
            ExceedDbContext db = new ExceedDbContext();
            bool isBranchAndBusinessunit = false;

            string reference = SecurityHelper.Encrypt("PayrollProcessMode", XAPI.Common.KEY);

            var conf = db.Configurations
                .Where(x => x.Category == "S")
                .Where(x => x.Reference == reference).FirstOrDefault();

            if (conf != null)
            {
                string attributeValue = SecurityHelper.Encrypt("BranchAndBusinessUnit", XAPI.Common.KEY);
                if (attributeValue == conf.Attribute)
                    isBranchAndBusinessunit = true;
            }

            return isBranchAndBusinessunit;
        }

        public static FiscalYearTypeAttribute GetFiscalYearType()
        {
            ExceedDbContext db = new ExceedDbContext();           

            string reference = SecurityHelper.Encrypt("FiscalYearType", XAPI.Common.KEY);

            var conf = db.Configurations
                .Where(x => x.Category == "S")
                .Where(x => x.Reference == reference).FirstOrDefault();

            if (conf != null)
            {
                string standardValue = SecurityHelper.Encrypt("Standard", XAPI.Common.KEY);
                string ethiopianValue = SecurityHelper.Encrypt("Ethiopian", XAPI.Common.KEY);
                if(conf.Attribute== standardValue)
                {
                    return FiscalYearTypeAttribute.Standard;
                }
                else if (conf.Attribute == ethiopianValue)
                {
                    return FiscalYearTypeAttribute.Ethiopian;
                }
            }

            return FiscalYearTypeAttribute.Unknown;
        }

        public static bool IsElectronicSignatureEnabled()
        {
            return InventoryConfigurationHelper.IsElectronicSignatureEnabled();
        }

        public static string GetCurrentCulture()
        {
            ExceedDbContext db = new ExceedDbContext();


            //var conf = db.GlobalCultures.FirstOrDefault();
            //if (conf != null)
            //{
            //    return conf.Culture;
            //}

            return Thread.CurrentThread.CurrentCulture.Name;
        }
    }
  
}
