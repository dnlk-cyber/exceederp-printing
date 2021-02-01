using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles =  Payroll_Admin)]
    public class PayrollPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
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


        public ActionResult ClosePeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.PayrollPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    var glPeriods = _db.PayrollPeriods.Where(x=> x.DateTo<=glPeriod.DateFrom && x.Status ==FiscalYearStatus.Open);
                    if (!glPeriods.Any())
                    {
                        glPeriod.Status = FiscalYearStatus.Close;
                        _db.PayrollPeriods.Attach(glPeriod);
                        _db.Entry(glPeriod).State = EntityState.Modified;
                        _db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll period", "Success", User.Identity.Name, $"closed period {glPeriod.Name}", Modules.Payroll);
                        #endregion
                    }
                    else

                    {
                        return new JavaScriptResult() { Script = "alert(' Please close the previous periods first!!');" };
                    }

                    #region  log
                 //   UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    #endregion
                }


            }

            return RedirectToAction("Index");

        }

        public ActionResult OpenPeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.PayrollPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.PayrollPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll period", "Success", User.Identity.Name, $"open period {glPeriod.Name}", Modules.Payroll);
                    #endregion

                    #region  log
                    //  UserHelper.MiscLog(Request.UserHostAddress, "Inventory Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name);
                    #endregion
                }


            }

            return RedirectToAction("Index");

        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
