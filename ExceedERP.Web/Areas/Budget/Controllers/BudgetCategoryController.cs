using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
 

namespace ExceedERP.Web.Areas.Budget.Controllers
{
       [AuthorizeRoles( BudgetRoles.BudgetAdmin)] 
    public class BudgetCategoryController : Controller
    {
        //
        // GET: /Budget/BudgetCategory/


        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["BudgetControl"] = db.BudgetControls.ToList();

            return View();
        }

        public ActionResult BudgetCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetCategory> BudgetCategory = db.BudgetCategorys;
            DataSourceResult result = BudgetCategory.ToDataSourceResult(request, budgetCategory => new BudgetCategory
            {
                BudgetCategoryID = budgetCategory.BudgetCategoryID,
                Name = budgetCategory.Name,
                 BudgetControlId = budgetCategory.BudgetControlId,
                FromAccountCode = budgetCategory.FromAccountCode,
                ToAccountCode = budgetCategory.ToAccountCode,
                Description = budgetCategory.Description,
                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Create([DataSourceRequest]DataSourceRequest request, BudgetCategory BudgetCategory)
        {
            List<BudgetCategory> category = db.BudgetCategorys.Where(x => x.Name == BudgetCategory.Name).ToList();

            if (!ModelState.IsValid)
            {
                return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
   
            }

            if(category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");
                return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
   
      
            }
            else if (BudgetCategory.FromAccountCode.Equals(BudgetCategory.ToAccountCode))
            {

                ModelState.AddModelError("SameAccountCode", " the FromAccountCode and To Account Code must Be Different !");
                return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
   
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetCategory
                    {
                        BudgetCategoryID = BudgetCategory.BudgetCategoryID,
                        Name = BudgetCategory.Name,
                        FromAccountCode = BudgetCategory.FromAccountCode,
                        ToAccountCode = BudgetCategory.ToAccountCode,
                        Description = BudgetCategory.Description,
                        BudgetControlId = BudgetCategory.BudgetControlId,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetCategorys.Add(entity);
                    db.SaveChanges();



                    BudgetCategory.BudgetCategoryID = entity.BudgetCategoryID;
                }
                else
                {
                    return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
      
                }
            }

            return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Update([DataSourceRequest]DataSourceRequest request, BudgetCategory BudgetCategory)
        {

            List<BudgetCategory> category = db.BudgetCategorys.Where(x => x.Name == BudgetCategory.Name && x.BudgetCategoryID != BudgetCategory.BudgetCategoryID).ToList();

            if (BudgetCategory.FromAccountCode.Equals(BudgetCategory.ToAccountCode))
            {

                ModelState.AddModelError("SameAccountCode", " the FromAccountCode and To Account Code must Be Different !");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetCategory
                    {
                        BudgetCategoryID = BudgetCategory.BudgetCategoryID,
                        Name = BudgetCategory.Name,
                        BudgetControlId = BudgetCategory.BudgetControlId,
                        FromAccountCode = BudgetCategory.FromAccountCode,
                        ToAccountCode = BudgetCategory.ToAccountCode,
                        Description = BudgetCategory.Description,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetCategorys.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Destroy([DataSourceRequest]DataSourceRequest request, BudgetCategory BudgetCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetCategory
                {
                    BudgetCategoryID = BudgetCategory.BudgetCategoryID,


                };

                db.BudgetCategorys.Attach(entity);
                db.BudgetCategorys.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + BudgetCategory.BudgetCategoryID);
            }

            return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}