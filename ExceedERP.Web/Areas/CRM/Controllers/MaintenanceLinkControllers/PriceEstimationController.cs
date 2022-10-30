using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.SalesQuoteUser, CRMRoles.SalesQuoteApprover, CRMRoles.CRMVoid)]
    public class PriceEstimationController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            SetInitials();

            return View();
        }
       
        public void SetInitials()
        {
            ViewData["Tax"] = db.GLTaxRates.ToList();
            ViewData["ProductCategories"] = db.ProductCategories.Select(c => new { c.ProductCategoryId, c.Description });
            ViewData["Products"] = db.Products.Select(s => new { s.ProductID, Name = s.ItemCode + " " + s.Name }).ToList();
            ViewData["OrganizationCustomers"] = db.OrganizationCustomers;
            
            ViewData["RepairTypes"] = db.RepairType.Select(r => new { RepairTypeId = r.RepairTypeID.ToString(), r.Name });

            ViewData["ProductMainCategories"] = db.ProductCategories.Select(c => new { c.Description, c.ProductCategoryId });
            ViewData["Products"] = db.Products.Select(p => new {Name = p.Name, ProductId = p.ProductID });
            ViewData["BankAccounts"] = db.BankAccounts.Select(p => new { AccountName = p.AccountNumber +" " + p.AccountName + " " + p.BankBranch + " Branch", BankAccountId = p.BankAccountId });

            ViewData["InventoryCategories"] = db.ItemCategorys.Select(s => new {
                ItemCategoryCode = s.ItemCategoryCode,
                Name = s.Name
            }).ToList();
        }
       
       
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
