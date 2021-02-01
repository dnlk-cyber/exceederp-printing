using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlLedgerPostingLinesController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GLLedgerPostingLines_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLLedgerPostingLines> glLedgerPostingLines = _db.GLLedgerPostingLines.Where(x => x.GLLedgerPostingID == id); ;
            DataSourceResult result = glLedgerPostingLines.ToDataSourceResult(request, glLedgerPostingLine => new
            {
                GLLedgerPostingID = glLedgerPostingLine.GLLedgerPostingID,
                GLLedgerPostingLinesID = glLedgerPostingLine.GLLedgerPostingLinesID,
                JournalNumber = glLedgerPostingLine.JournalNumber,
                Remark = glLedgerPostingLine.Remark,
                AdditionalDescription = glLedgerPostingLine.AdditionalDescription,
                GlFiscalYearId = glLedgerPostingLine.GlFiscalYearId,
                GLPeriodId = glLedgerPostingLine.GLPeriodId,
                EffectiveDate = glLedgerPostingLine.EffectiveDate,
                Debit = glLedgerPostingLine.Debit,
                Credit = glLedgerPostingLine.Credit,
                Balance = glLedgerPostingLine.Balance,
                OrgBranchName = glLedgerPostingLine.OrgBranchName,
                DateCreated = glLedgerPostingLine.DateCreated,
                LastModified = glLedgerPostingLine.LastModified,
                CreatedBy = glLedgerPostingLine.CreatedBy,
                ModifiedBy = glLedgerPostingLine.ModifiedBy
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
