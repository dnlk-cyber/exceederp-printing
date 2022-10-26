using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.AdvancePaymentUser, CRMRoles.AdvancePaymentApprover)]
    public class AdvancePaymentDistibutionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: CRM/AdvancePaymentDistibution
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdvancePaymentDistribution_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var advancePayment = db.AdvancePaymentDistibutions.Where(x => x.AdvancePaymentId == Id).ToList();

            
                return Json(advancePayment.Select(s => new AdvancePaymentDistibution
                {
                    AdvancePaymentDistibutionId = s.AdvancePaymentDistibutionId,
                    AccountCode = s.AccountCode,
                    AccountDesc = s.AccountDesc,
                    Credit = s.Credit,
                    Debit = s.Debit,
                }
                ).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

            



        }
    }
}