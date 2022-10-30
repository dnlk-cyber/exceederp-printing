using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class CustomerController : Controller
    {
        // GET: CRM/Customer
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var periodList = db.Periods;
            var accountList = db.Accounts;
            var bankList = db.Banks;
            var organizationCategory = db.OrganizationCategories;
            ViewData["PeriodList"] = periodList.AsQueryable();
            ViewData["AccountList"] = accountList.AsQueryable();
            ViewData["BankList"] = bankList.AsQueryable();
            ViewData["OrganizationCategory"] = organizationCategory.AsQueryable();
            return View();
        }
    }
}