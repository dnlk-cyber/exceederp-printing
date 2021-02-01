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
    public class BudgetAccountSheduleController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/AccountShedule/
        public ActionResult Index()
        {
            ViewData["BudgetAccountSheduleCategory"] = db.BudgetAccountSheduleCategories.ToList();
            return View();
        }

        public ActionResult BudgetAccountShedule_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetAccountShedule> BudgetAccountShedule = db.BudgetAccountShedules;
            DataSourceResult result = BudgetAccountShedule.ToDataSourceResult(request, AccountShedule => new 
            {
                BudgetAccountSheduleId = AccountShedule.BudgetAccountSheduleId,
                BudgetAccountSheduleCategoryId = AccountShedule.BudgetAccountSheduleCategoryId,
                Index = AccountShedule.Index,
                Remark = AccountShedule.Remark,
                CreatedBy = AccountShedule.CreatedBy,
                DateCreated = AccountShedule.DateCreated,
                LastModified = AccountShedule.LastModified,
                ModifiedBy = AccountShedule.ModifiedBy,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountShedule_Create([DataSourceRequest]DataSourceRequest request, BudgetAccountShedule BudgetAccountShedule)
        {
            List<BudgetAccountShedule> category = db.BudgetAccountShedules.Where(x => x.BudgetAccountSheduleCategoryId == BudgetAccountShedule.BudgetAccountSheduleCategoryId && x.Index == BudgetAccountShedule.Index).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name and Rule Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountShedule
                    {
                        BudgetAccountSheduleId = BudgetAccountShedule.BudgetAccountSheduleId,
                        BudgetAccountSheduleCategoryId = BudgetAccountShedule.BudgetAccountSheduleCategoryId,
                        Index = BudgetAccountShedule.Index,
                        Remark = BudgetAccountShedule.Remark,
                         DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetAccountShedules.Add(entity);
                    db.SaveChanges();



                    BudgetAccountShedule.BudgetAccountSheduleId = entity.BudgetAccountSheduleId;
                }
            }

            return Json(new[] { BudgetAccountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountShedule_Update([DataSourceRequest]DataSourceRequest request, BudgetAccountShedule AccountShedule)
        {
            List<BudgetAccountShedule> category = db.BudgetAccountShedules.Where(x => x.BudgetAccountSheduleCategoryId == AccountShedule.BudgetAccountSheduleCategoryId && x.Index == AccountShedule.Index && x.BudgetAccountSheduleId != AccountShedule.BudgetAccountSheduleId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetAccountShedule
                    {
                        BudgetAccountSheduleId = AccountShedule.BudgetAccountSheduleId,
                        BudgetAccountSheduleCategoryId = AccountShedule.BudgetAccountSheduleCategoryId,
                         LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,
                        DateCreated = AccountShedule.DateCreated,
                        CreatedBy = AccountShedule.CreatedBy,
                    };

                    db.BudgetAccountShedules.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { AccountShedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetAccountShedule_Destroy([DataSourceRequest]DataSourceRequest request, BudgetAccountShedule BudgetAccountShedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetAccountShedule
                {
                    BudgetAccountSheduleId = BudgetAccountShedule.BudgetAccountSheduleId,
                    Index = BudgetAccountShedule.Index,
                    Remark = BudgetAccountShedule.Remark,

                };

                db.BudgetAccountShedules.Attach(entity);
                db.BudgetAccountShedules.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Account Schedule Number " + BudgetAccountShedule.BudgetAccountSheduleId);
            }

            return Json(new[] { BudgetAccountShedule }.ToDataSourceResult(request, ModelState));
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }



    }
}