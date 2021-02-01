using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Budget
{
    public class BudgetAccountSheduleSubCategoryController : Controller
    {

        ExceedDbContext _db = new ExceedDbContext();
        //
        // GET: /Budget/AccountSheduleLine/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetAccountSheduleSubCategory_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BudgetAccountSheduleSubCategory> BudgetAccountSheduleSubCategory = _db.BudgetAccountSheduleSubCategories.Where(x=>x.BudgetAccountSheduleId == id);
            DataSourceResult result = BudgetAccountSheduleSubCategory.ToDataSourceResult(request, accountShedule => new
            {
                BudgetAccountSheduleSubCategoryId = accountShedule.BudgetAccountSheduleSubCategoryId,
                BudgetAccountSheduleId = accountShedule.BudgetAccountSheduleId,
                Name = accountShedule.Name,CreatedBy = accountShedule.CreatedBy,
                DateCreated = accountShedule.DateCreated,
                LastModified = accountShedule.LastModified,
                ModifiedBy = accountShedule.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleSubCategory_Create([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleSubCategory BudgetAccountSheduleSubCategory,int id)
        {
            List<BudgetAccountSheduleSubCategory> category = _db.BudgetAccountSheduleSubCategories.Where(x => x.Name == BudgetAccountSheduleSubCategory.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", "  Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleSubCategory
                    {
                        BudgetAccountSheduleSubCategoryId = BudgetAccountSheduleSubCategory.BudgetAccountSheduleSubCategoryId,
                        BudgetAccountSheduleId = id,
                        Name = BudgetAccountSheduleSubCategory.Name,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name


                    };

                    _db.BudgetAccountSheduleSubCategories.Add(entity);
                    _db.SaveChanges();



                    BudgetAccountSheduleSubCategory.BudgetAccountSheduleSubCategoryId = entity.BudgetAccountSheduleSubCategoryId;
                }
            }

            return Json(new[] { BudgetAccountSheduleSubCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleSubCategory_Update([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleSubCategory accountShedule)
        {
            List<BudgetAccountSheduleSubCategory> category = _db.BudgetAccountSheduleSubCategories.Where(x => x.Name == accountShedule.Name && x.BudgetAccountSheduleSubCategoryId != accountShedule.BudgetAccountSheduleSubCategoryId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleSubCategory
                    {
                        BudgetAccountSheduleSubCategoryId = accountShedule.BudgetAccountSheduleSubCategoryId,
                        BudgetAccountSheduleId = accountShedule.BudgetAccountSheduleId,
                        Name = accountShedule.Name,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        DateCreated = accountShedule.DateCreated,
                        CreatedBy = accountShedule.CreatedBy

                    };

                    _db.BudgetAccountSheduleSubCategories.Attach(entity);
                    _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            return Json(new[] { accountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleSubCategory_Destroy([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleSubCategory BudgetAccountSheduleSubCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetAccountSheduleSubCategory
                {
                    BudgetAccountSheduleSubCategoryId = BudgetAccountSheduleSubCategory.BudgetAccountSheduleSubCategoryId,


                };

                _db.BudgetAccountSheduleSubCategories.Attach(entity);
                _db.BudgetAccountSheduleSubCategories.Remove(entity);
                _db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Line number " + BudgetAccountSheduleSubCategory.BudgetAccountSheduleSubCategoryId);
            }

            return Json(new[] { BudgetAccountSheduleSubCategory }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


    }
}