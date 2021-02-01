using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Admin)]
    public class PayrollPeriodCourtController : BaseController
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            var glperiods = _db.GLPeriods.ToList();
            foreach (var period in glperiods)
            {
                var exist = _db.PayrollPeriods.FirstOrDefault(x => x.Name == period.Name);
                if (exist == null)
                {
                    var entity = new PayrollPeriod
                    {
                        Name = period.Name,
                        GLPeriodId = period.GLPeriodId,
                        //PayrollPeriodId = period.GLPeriodId,
                        DateFrom = period.DateFrom,
                        DateTo = period.DateTo,
                        Status = period.Status,
                        OrgBranchName = period.OrgBranchName,
                        DateCreated = period.DateCreated,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy,
                        GlFiscalYearId = period.GlFiscalYearId
                    };
                    _db.PayrollPeriods.Add(entity);
                    _db.SaveChanges();
                   
                    _db = new ExceedDbContext();
                    var branchs = _db.Branch;
                    foreach (var b in branchs)
                    {
                        var entitye = new PeriodStatus
                        {
                           BranchId = b.BranchId,
                            Status = PeriodStatusList.NEW,

                            GLPeriodId = period.GLPeriodId
                        };
                        _db = new ExceedDbContext();
                        _db.PeriodStatuses.Add(entitye);
                        _db.SaveChanges();
                    }
                }

            }
            return View();
        }

        public ActionResult InventoryPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<PayrollPeriod> inventoryPeriods = _db.PayrollPeriods;
            DataSourceResult result = inventoryPeriods.ToDataSourceResult(request, gLPeriod => new
            {

                GLPeriodId = gLPeriod.GLPeriodId,
                GlFiscalYearId = gLPeriod.GlFiscalYearId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }


       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
