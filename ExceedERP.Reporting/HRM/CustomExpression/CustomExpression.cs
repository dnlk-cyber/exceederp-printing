using System.Resources;
using ExceedERP.Core.Localization;

namespace ExceedERP.Reporting.HRM.CustomExpression
{

    public static  class CustomExpression
    {


        public static string GetStringOverride(string key)
        {
            ResourceManager myManager = new ResourceManager(typeof(Resources) );
          //  string cultureName = null;
          //cultureName = CultureHelper.GetImplementedCulture(cultureName); // This is safe

          //  // Modify current thread's cultures            
          //  Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            string myString = myManager.GetString(key, System.Globalization.CultureInfo.CurrentCulture);
            return myString;
        }
        //public static string ConvertToWords(decimal amount)
        //{
        //    XAPI.Common.getDaystoWords(3); 
        //}
    }
}
