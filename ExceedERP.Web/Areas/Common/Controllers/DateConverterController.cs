using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Common
{
    public class DateConverterController : Controller
    {
        // GET: HRM/DateConvertor
        public ActionResult Index()
        {
            return View();
        }
        
        public string ConvertToGergorian(string date)
        {
            if(date != null)
            {
                var splitDate = date.Split('/');
                var converted = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                return converted.ToShortDateString();
            }

            return null;
        }
        public string ConvertToEthiopian(DateTime date)
        {
            var converted = XAPI.EthiopicDateTime.GetEthiopicDate(date.Day, date.Month, date.Year);
            return converted;
        }
    }
}