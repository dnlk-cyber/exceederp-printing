
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin)]
    public class PeriodStatusController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";

        public ActionResult Index()
        {
            ViewData["Period"] = db.GLPeriods.ToList();
            ViewData["Branches"] = db.Branch.ToList();
            ViewData["BusinessUnit"] = db.BusinessUnits.Select(x => new { x.BusinessUnitId, x.Name }).ToList();

            return View();
        }

        public ActionResult PeriodStatus_Read([DataSourceRequest] DataSourceRequest request)
        {
            var periodStatus = db.PeriodStatuses;
            DataSourceResult result = periodStatus.ToDataSourceResult(request, periodstatus => new
            {
                PeriodStatusId = periodstatus.PeriodStatusId,
                GLPeriodId = periodstatus.GLPeriodId,
                BranchId = periodstatus.BranchId,
                Status = periodstatus.Status,
                Remark = periodstatus.Remark,
                IsLocked = periodstatus.IsLocked
            });
            return Json(result);
        }

       
        public JsonResult Unlock(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var status = db.PeriodStatuses.Find(did);
                if (status != null && status.IsLocked)
                {
                    status.IsLocked = false;
                    db.PeriodStatuses.Attach(status);
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                    #region unlock log
                    UserHelper.OperationLog(Request.UserHostAddress, "Unlock", "Success", User.Identity.Name, "Disabled unlocked  " + status.GLPeriodId, Modules.Payroll);
                    #endregion
                }
               
            }

            var formattedData = new
            {
                Message = "Operation Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}