using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class TaxTransactionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TaxTransactions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TaxTransaction> taxtransactions = db.TaxTransactions;
            DataSourceResult result = taxtransactions.ToDataSourceResult(request, taxTransaction => new {
                TaxTransactionID = taxTransaction.TaxTransactionID,
                Voucher = taxTransaction.Voucher,
                TaxType = taxTransaction.TaxType,
                TaxableAmount = taxTransaction.TaxableAmount,
                CreatedBy = taxTransaction.CreatedBy,
                DateCreated = taxTransaction.DateCreated,
                LastModified = taxTransaction.LastModified,
                ModifiedBy = taxTransaction.ModifiedBy,
                OrgBranchName = taxTransaction.OrgBranchName,
                TaxAmount = taxTransaction.TaxAmount
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
