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
    public class BudgetAccountSheduleCategoryController : Controller
    {
        ExceedDbContext _db = new ExceedDbContext();
        //
        // GET: /Budget/AccountShedule/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetAccountSheduleCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetAccountSheduleCategory> budgetAccountSheduleCategory = _db.BudgetAccountSheduleCategories;
            DataSourceResult result = budgetAccountSheduleCategory.ToDataSourceResult(request, accountShedule => new
            {
                BudgetAccountSheduleCategoryId = accountShedule.BudgetAccountSheduleCategoryId,
                Name = accountShedule.Name,
                SheduleCategoryType = accountShedule.SheduleCategoryType,
                Remark = accountShedule.Remark,
                CreatedBy = accountShedule.CreatedBy,
                DateCreated = accountShedule.DateCreated,
                LastModified = accountShedule.LastModified,
                ModifiedBy = accountShedule.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleCategory_Create([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleCategory budgetAccountSheduleCategory)
        {
            List<BudgetAccountSheduleCategory> category = _db.BudgetAccountSheduleCategories.Where(x => x.Name == budgetAccountSheduleCategory.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleCategory
                    {
                        Name = budgetAccountSheduleCategory.Name,
                        SheduleCategoryType = budgetAccountSheduleCategory.SheduleCategoryType,
                        Remark = budgetAccountSheduleCategory.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    _db.BudgetAccountSheduleCategories.Add(entity);
                    _db.SaveChanges();



                    budgetAccountSheduleCategory.Name = entity.Name;
                }
            }

            return Json(new[] { budgetAccountSheduleCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleCategory_Update([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleCategory accountShedule)
        {
            List<BudgetAccountSheduleCategory> category = _db.BudgetAccountSheduleCategories.Where(x => x.BudgetAccountSheduleCategoryId == accountShedule.BudgetAccountSheduleCategoryId && x.Name != accountShedule.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleCategory
                    {
                        BudgetAccountSheduleCategoryId = accountShedule.BudgetAccountSheduleCategoryId,
                        Name = accountShedule.Name,
                        SheduleCategoryType = accountShedule.SheduleCategoryType,
                        Remark = accountShedule.Remark,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        DateCreated = accountShedule.DateCreated,
                        CreatedBy = accountShedule.CreatedBy,
                    };

                    _db.BudgetAccountSheduleCategories.Attach(entity);
                    _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            return Json(new[] { accountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleCategory_Destroy([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleCategory budgetAccountSheduleCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetAccountSheduleCategory
                {
                    Name = budgetAccountSheduleCategory.Name,
               
                    Remark = budgetAccountSheduleCategory.Remark,

                };

                _db.BudgetAccountSheduleCategories.Attach(entity);
                _db.BudgetAccountSheduleCategories.Remove(entity);
                _db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Account Schedule Category Number " + budgetAccountSheduleCategory.Name);
            }

            return Json(new[] { budgetAccountSheduleCategory }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult BudgetScheduleCategoryDdBind()
        {

            var budgetAccountSheduleCategories = _db.BudgetAccountSheduleCategories.Select(s => new  
            {

                AccountDesc = s.Name,
                AccountCode = s.BudgetAccountSheduleCategoryId 

            }).ToList();

            return Json( budgetAccountSheduleCategories, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }



    }
}