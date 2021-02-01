using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class PayrollDataProviderController : Controller
    {
        // GET: HRM/PayrollDataProvider
        private ExceedDbContext _db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCostCenterValues()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            var costCenterType =
                _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);

            if (costCenterType != null)
            {
                var segmentSetups = _db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == costCenterType.GLSegmentSetupId)
                    .Where(x => x.Enable).AsQueryable();

                foreach (var accountSegment in segmentSetups)
                {
                    var record = new SelectorVm
                    {
                        Name = accountSegment.Values+"-"+accountSegment.Description,
                        Code = accountSegment.Values
                    };
                    list.Add(record);
                }
               
            }
            return Json(list.Select(c => new
                {
                    Code = c.Code,
                    Name = c.Name
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductValues()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            var productType =
                _db.GlSegmentSetups.FirstOrDefault(x => x.Sequence == 4);
            if (productType != null)
            {
                var segmentSetups = _db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == productType.GLSegmentSetupId)
                    .Where(x => x.Enable).AsQueryable();
                foreach (var accountSegment in segmentSetups)
                {
                    var record = new SelectorVm
                    {
                        Name = accountSegment.Values+"-"+accountSegment.Description,
                        Code = accountSegment.Values
                    };
                    list.Add(record);
                }
            }
            return Json(list.Select(c => new
                {
                    Code = c.Code,
                    Name = c.Name
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}