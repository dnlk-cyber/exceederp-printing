using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlLedgerPostingController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerPostUser)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var periods = _db.GLPeriods;
            var years = _db.GLFiscalYears;
            ViewData["Periods"] = periods;
            ViewData["Years"] = years;
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerPostUser)]
        public ActionResult GLLedgerPostings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLLedgerPosting> glledgerpostings = _db.GLLedgerPostings;
            DataSourceResult result = glledgerpostings.ToDataSourceResult(request, gLLedgerPosting => new
            {
                GLLedgerPostingID = gLLedgerPosting.GLLedgerPostingID,
                AccountCode = gLLedgerPosting.AccountCode,
                AccountDesc = gLLedgerPosting.AccountDesc,
                AccountType = gLLedgerPosting.AccountType,
                AccountSegment = gLLedgerPosting.AccountSegment,
                BranchSegment = gLLedgerPosting.BranchSegment,
                CostCenterSegment = gLLedgerPosting.CostCenterSegment,
                SubAccountSegment = gLLedgerPosting.SubAccountSegment,
                ChannelSegment = gLLedgerPosting.ChannelSegment,
                OrgBranchName = gLLedgerPosting.OrgBranchName,
                DateCreated = gLLedgerPosting.DateCreated,
                LastModified = gLLedgerPosting.LastModified,
                CreatedBy = gLLedgerPosting.CreatedBy,
                ModifiedBy = gLLedgerPosting.ModifiedBy
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
