using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.CRM;


namespace ExceedERP.Web.Areas.Budget.Controllers
{
        [AuthorizeRoles( BudgetRoles.BudgetAdmin)] 
    public class CurrencyController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/Currency/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetCurrency_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetCurrency> BudgetCurrency = db.BudgetCurrencies;
            DataSourceResult result = BudgetCurrency.ToDataSourceResult(request, Currency => new BudgetCurrency
            {
                BudgetCurrencyId = Currency.BudgetCurrencyId,
                Name = Currency.Name,

                Description = Currency.Description,
                CreatedBy = Currency.CreatedBy,
                DateCreated = Currency.DateCreated,
                LastModified = Currency.LastModified,
                ModifiedBy = Currency.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCurrency_Create([DataSourceRequest]DataSourceRequest request, BudgetCurrency BudgetCurrency)
        {
          

            if ( db.BudgetCurrencies.Any(x => x.Name == BudgetCurrency.Name))
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
          
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetCurrency
                    {
                        BudgetCurrencyId = BudgetCurrency.BudgetCurrencyId,
                        Name = BudgetCurrency.Name,
                      Description = BudgetCurrency.Description,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetCurrencies.Add(entity);
                    db.SaveChanges();



                    BudgetCurrency.BudgetCurrencyId = entity.BudgetCurrencyId;
                }
            }

            return Json(new[] { BudgetCurrency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCurrency_Update([DataSourceRequest]DataSourceRequest request, BudgetCurrency Currency)
        {



            if (db.BudgetCurrencies.Any(x => x.Name == Currency.Name && x.BudgetCurrencyId != Currency.BudgetCurrencyId))
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetCurrency
                    {
                        BudgetCurrencyId = Currency.BudgetCurrencyId,
                        Name = Currency.Name,
                        Description = Currency.Description,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetCurrencies.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
                return Json(new[] { Currency }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCurrency_Destroy([DataSourceRequest]DataSourceRequest request, BudgetCurrency BudgetCurrency)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetCurrency
                {
                    BudgetCurrencyId = BudgetCurrency.BudgetCurrencyId,


                };

                db.BudgetCurrencies.Attach(entity);
                db.BudgetCurrencies.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + BudgetCurrency.BudgetCurrencyId);
            }

            return Json(new[] { BudgetCurrency }.ToDataSourceResult(request, ModelState));
        }

    
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}