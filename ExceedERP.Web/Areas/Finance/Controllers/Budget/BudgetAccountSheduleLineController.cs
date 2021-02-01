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
    public class BudgetAccountSheduleLineController : Controller
    {

        ExceedDbContext _db = new ExceedDbContext();
        //
        // GET: /Budget/AccountSheduleLine/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetAccountSheduleLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BudgetAccountSheduleLine> budgetAccountSheduleLine = _db.BudgetAccountSheduleLines.Where(x=>x.BudgetAccountSheduleSubCategoryId == id);
            DataSourceResult result = budgetAccountSheduleLine.ToDataSourceResult(request, accountShedule => new
            {
                BudgetAccountSheduleLineId = accountShedule.BudgetAccountSheduleLineId,
                BudgetAccountSheduleSubCategoryId = accountShedule.BudgetAccountSheduleSubCategoryId,
                Name = accountShedule.Name,
                CreatedBy = accountShedule.CreatedBy,
                DateCreated = accountShedule.DateCreated,
                LastModified = accountShedule.LastModified,
                ModifiedBy = accountShedule.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleLine_Create([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleLine budgetAccountSheduleLine,int id)
        {
            List<BudgetAccountSheduleLine> category = _db.BudgetAccountSheduleLines.Where(x => x.Name == budgetAccountSheduleLine.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", "  Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleLine
                    {
                        BudgetAccountSheduleLineId = budgetAccountSheduleLine.BudgetAccountSheduleLineId,
                        BudgetAccountSheduleSubCategoryId = id,
                        Name = budgetAccountSheduleLine.Name,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name


                    };

                    _db.BudgetAccountSheduleLines.Add(entity);
                    _db.SaveChanges();



                    budgetAccountSheduleLine.BudgetAccountSheduleLineId = entity.BudgetAccountSheduleLineId;
                }
            }

            return Json(new[] { budgetAccountSheduleLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleLine_Update([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleLine accountShedule)
        {
            List<BudgetAccountSheduleLine> category = _db.BudgetAccountSheduleLines.Where(x => x.Name == accountShedule.Name && x.BudgetAccountSheduleLineId != accountShedule.BudgetAccountSheduleLineId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountSheduleLine
                    {
                        BudgetAccountSheduleLineId = accountShedule.BudgetAccountSheduleLineId,
                        BudgetAccountSheduleSubCategoryId = accountShedule.BudgetAccountSheduleSubCategoryId,
                        Name = accountShedule.Name,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        DateCreated = accountShedule.DateCreated,
                        CreatedBy = accountShedule.CreatedBy

                    };

                    _db.BudgetAccountSheduleLines.Attach(entity);
                    _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();

                }
            }
            return Json(new[] { accountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountSheduleLine_Destroy([DataSourceRequest]DataSourceRequest request, BudgetAccountSheduleLine budgetAccountSheduleLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetAccountSheduleLine
                {
                    BudgetAccountSheduleLineId = budgetAccountSheduleLine.BudgetAccountSheduleLineId,


                };

                _db.BudgetAccountSheduleLines.Attach(entity);
                _db.BudgetAccountSheduleLines.Remove(entity);
                _db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Line number " + budgetAccountSheduleLine.BudgetAccountSheduleLineId);
            }

            return Json(new[] { budgetAccountSheduleLine }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }


    }
}