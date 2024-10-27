using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
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
            var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            var subAccounts =
                _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId)
                                            .Where(x => x.Enable).Where(x => x.Parent == false).ToList();

            return Json(subAccounts.Select(c => 
            new {
                value = c.Values,
                desc = c.Values + "_" + c.Description }), 
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCostCenter()
        {
            var costCenterSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var costCenters =
                _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costCenterSeg.GLSegmentSetupId)
                                            .Where(x => x.Enable).Where(x => x.Parent == false).ToList();

       
            return Json(costCenters.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocations()
        {
            var locationSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations =
                _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId)
                                            .Where(x => x.Enable).Where(x => x.Parent == false).ToList();


            return Json(locations.Select(c => new { value = c.Values, desc = c.Values + "_" + c.Description }), JsonRequestBehavior.AllowGet);
        }


        public IEnumerable<GLChartOfAccountSegment> GetSubAccountsList()
        {
            var items = (List<GLChartOfAccountSegment>)System.Runtime.Caching.MemoryCache.Default["SubAccounts"];
            if (items == null)
            {
                var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
                var subAccounts =
                    _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId)
                                                .Where(x => x.Enable).Where(x => x.Parent == false).ToList();
                
                System.Runtime.Caching.MemoryCache.Default["SubAccounts"] = subAccountSeg;
            }
            


            return items.Select(item => new GLChartOfAccountSegment
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
            //var segmentSequence = _db.GlSegmentSequences.FirstOrDefault();
            //if (segmentSequence != null)
            //{
            //    ViewBag.Segment1 = segmentSequence.FirstSegment;
            //    ViewBag.Segment2 = segmentSequence.SecondSegment;
            //    ViewBag.Segment3 = segmentSequence.ThirdSegment;
            //    ViewBag.Segment4 = segmentSequence.FourthSegment;
            //}
                
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