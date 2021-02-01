using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Repository;
using ExceedERP.Web.Areas.Inventory.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class AccountCodeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        

        // GET: Finance/Description
      
        public JsonResult GetAccounts()
        {
           
            List<GLChartOfAccountAccount> accounts = _db.GLChartOfAccountAccounts.Where(x => x.Enable).Where(x => x.Parent == false).ToList();
           

            return Json(accounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubAccounts()
        {
            
            List<GLChartOfAccountSubAccount> subAccounts = _db.GLChartOfAccountSubAccounts.Where(x => x.Enable).Where(x => x.Parent == false).ToList();

            return Json(subAccounts.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostCenter()
        {
            List<GLChartOfAccountCostCenter> costCenters = _db.GLChartOfAccountCostCenters.Where(x => x.Enable).Where(x => x.Parent == false).ToList();

            return Json(costCenters.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocations()
        {
            List<GLChartOfAccountLocation> locations =_db.GLChartOfAccountLocations.Where(x => x.Enable).Where(x => x.Parent == false).ToList();

            return Json(locations.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }


        public IEnumerable<GLChartOfAccountSubAccount> GetSubAccountsList()
        {
            var items = (List<GLChartOfAccountSubAccount>)System.Runtime.Caching.MemoryCache.Default["SubAccounts"];
            if (items == null)
            {
                items  = _db.GLChartOfAccountSubAccounts
                    .Where(x => x.Enable)
                    .Where(x => x.Parent == false).ToList();
                System.Runtime.Caching.MemoryCache.Default["SubAccounts"] = items;
            }
            


            return items.Select(item => new GLChartOfAccountSubAccount
            {
                Values = item.Values,
                Description = item.Values + "_" + item.Description
            });
        }
        //non fuel
        public ActionResult VirtualSubAccount_Read([DataSourceRequest] DataSourceRequest request, string text)
        {
            var items = GetSubAccountsList();

            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p =>
                    p.Values.ToString().ToUpper().Contains(text.ToUpper()) ||
                    p.Description.Contains(text.ToUpper())).AsQueryable();
            }
            return Json(items.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //non fuel
        public ActionResult SubAccount_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = GetSubAccountsList();

            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.Values== value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GlAccountSelector(string q)
        {
            ViewBag.PreviousAccount = q;
            var segmentSequence = _db.GlSegmentSequences.FirstOrDefault();
            if (segmentSequence != null)
            {
                ViewBag.Segment1 = segmentSequence.FirstSegment;
                ViewBag.Segment2 = segmentSequence.SecondSegment;
                ViewBag.Segment3 = segmentSequence.ThirdSegment;
                ViewBag.Segment4 = segmentSequence.FourthSegment;
            }
                
            return View();
        }

        public ActionResult ArAccountSelector()
        {
            return View();
        }

        public ActionResult ApAccountSelector()
        {
            return View();
        }

        public ActionResult WithholdingAccountSelector()
        {
            return View();
        }

        public ActionResult ApCustomerAccount()
        {
            return View();
        }

        public ActionResult GlAccountFromSelector()
        {
            return View();
        }

        public ActionResult GlAccountToSelector()
        {
            return View();
        }
    }
}