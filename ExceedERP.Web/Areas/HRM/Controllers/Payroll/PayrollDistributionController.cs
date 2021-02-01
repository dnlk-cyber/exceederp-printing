using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class PayrollDistributionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int PeriodId { get; set; }
        public static int BranchId { get; set; }
        public ActionResult Index()
        {

            List<PayrollDistribution> PayrollDistributions = db.PayrollDistributions.Where(p => p.GLPeriodId == PeriodId).ToList();
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var lists = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });

            var period = lists.FirstOrDefault();
            if (period != null)
            {
                ViewBag.Period = period.g.Name;
            }
            else
            {
                ViewBag.Period = "";
            }
            ViewBag.TDebit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PeriodId).Sum(x => x.Debit)).ToString("#,##0.00");
            ViewBag.TCredit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PeriodId).Sum(x => x.Credit)).ToString("#,##0.00");
            return View();
        }

        public ActionResult OpenDistribution()
        {
            return RedirectToAction("Index");
        }
        public ActionResult PayrollDistributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PayrollDistribution> PayrollDistributions = db.PayrollDistributions.Where(p => p.GLPeriodId == PeriodId).ToList();
            var period = db.PayrollPeriods .Where(x => x.GLPeriodId == PeriodId).FirstOrDefault();
            if (period != null)
            {
                ViewBag.Period = period.Name;
            }
            else
            {
                ViewBag.Period = "";
            }
            string debit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PeriodId).Sum(x => x.Debit)).ToString("#,##0.00");
            string credit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PeriodId).Sum(x => x.Credit)).ToString("#,##0.00");
            IQueryable<PayrollDistribution> payrolldistributions = db.PayrollDistributions;

            List<PayrollDistribution> distributions = new List<PayrollDistribution>();
            PayrollDistribution distribution = new PayrollDistribution();
            if (period != null) distribution.GLPeriodId = period.GLPeriodId;
            distribution.AccountCode = "";
            distribution.AccountDesc = "";
            distribution.Debit = Convert.ToDecimal(debit);
            distribution.Credit = Convert.ToDecimal(credit);
            distributions.Add(distribution);
            DataSourceResult result = payrolldistributions.ToDataSourceResult(request, payrollDistribution => new
            {
                //PayrollDistributionID = payrollDistribution.PayrollDistributionID,
              //  CreatedOn = payrollDistribution.CreatedOn,
               // CreatedBy = payrollDistribution.CreatedBy,
              //  Period = payrollDistribution.Period,
                AccountCode = payrollDistribution.AccountCode,
                AccountDesc = payrollDistribution.AccountDesc,
                Debit = payrollDistribution.Debit,
                Credit =payrollDistribution. Credit,
                OrgBranchName = payrollDistribution.OrgBranchName,
                CostCenterCode = payrollDistribution.CostCenterCode,
                DateCreated = payrollDistribution.DateCreated,
                LastModified = payrollDistribution.LastModified,
                CreatedBy = payrollDistribution.CreatedBy,
                ModifiedBy = payrollDistribution.ModifiedBy
               // Status = payrollDistribution.Status
                //isSendForApproval = payrollDistribution.isSendForApproval,
                //isOnlineApproved = payrollDistribution.isOnlineApproved,
                //OnlineApprovedBy = payrollDistribution.OnlineApprovedBy,
                //OnlineApprovedTime = payrollDistribution.OnlineApprovedTime,
                //isVoid = payrollDistribution.isVoid,
                //VoidBy = payrollDistribution.VoidBy,
                //VoidTime = payrollDistribution.VoidTime
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Payroll Distribution");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Payroll Distribution");
            #endregion
            return File(fileContents, contentType, fileName);
        }
    }
}
