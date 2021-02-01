using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Common
{
    public class GlobalHelper
    {
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
    }
}
