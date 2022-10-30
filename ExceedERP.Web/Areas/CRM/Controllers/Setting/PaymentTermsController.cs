using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class PaymentTermsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        // GET: CRM/PaymentTerms
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PaymentTerms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentTerms> paymentterms = _db.PaymentTerms;
            DataSourceResult result = paymentterms.ToDataSourceResult(request, paymentTerms => new {
                PaymentTermsID = paymentTerms.PaymentTermsID,
                PaymentTermsDays = paymentTerms.PaymentTermsDays,
                Description = paymentTerms.Description,
                OrgBranchName = paymentTerms.OrgBranchName,
                DateCreated = paymentTerms.DateCreated,
                LastModified = paymentTerms.LastModified,
                CreatedBy = paymentTerms.CreatedBy,
                ModifiedBy = paymentTerms.ModifiedBy
            });

            return Json(result);
        }
    }
}