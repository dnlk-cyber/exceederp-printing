using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers
{

    [AuthorizeRoles(BudgetRoles.BudgetAdmin)]
    public class BudgetControlController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/BudgetControl/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetControl_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetControl> BudgetControls = db.BudgetControls;
            DataSourceResult result = BudgetControls.ToDataSourceResult(request, BudgetControl => new 
            {
                BudgetControlId = BudgetControl.BudgetControlId,
                Name = BudgetControl.Name,
                ReserveAmount = BudgetControl.ReserveAmount,
                Percentage = BudgetControl.Percentage,
                Remark = BudgetControl.Remark,
                CreatedBy = BudgetControl.CreatedBy,
                DateCreated = BudgetControl.DateCreated,
                LastModified = BudgetControl.LastModified,
                ModifiedBy = BudgetControl.ModifiedBy


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetControl_Create([DataSourceRequest]DataSourceRequest request, BudgetControl BudgetControl)
        {
             List<BudgetControl> category = db.BudgetControls.Where(x => x.Name == BudgetControl.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", "  Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetControl
                    { 
                        BudgetControlId =BudgetControl.BudgetControlId,
                        Name = BudgetControl.Name,
                        ReserveAmount = BudgetControl.ReserveAmount,
                        Percentage = BudgetControl.Percentage,
                        Remark = BudgetControl.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetControls.Add(entity);
                    db.SaveChanges();



                    BudgetControl.BudgetControlId = entity.BudgetControlId;
                }
            }

            return Json(new[] { BudgetControl }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetControl_Update([DataSourceRequest]DataSourceRequest request, BudgetControl BudgetControl)
        {
            List<BudgetControl> category = db.BudgetControls.Where(x => x.Name == BudgetControl.Name && x.BudgetControlId != BudgetControl.BudgetControlId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetControl
                    {
                        BudgetControlId = BudgetControl.BudgetControlId,
                        Name = BudgetControl.Name,
                      
                        ReserveAmount = BudgetControl.ReserveAmount,
                        Percentage = BudgetControl.Percentage,
                        Remark = BudgetControl.Remark,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetControls.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { BudgetControl }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetControl_Destroy([DataSourceRequest]DataSourceRequest request, BudgetControl BudgetControl)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetControl
                {
                    BudgetControlId = BudgetControl.BudgetControlId,


                };

                db.BudgetControls.Attach(entity);
                db.BudgetControls.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + BudgetControl.BudgetControlId);
            }

            return Json(new[] { BudgetControl }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult ControlDDBind()
        {

            var workunit = db.BudgetControls.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetControlId.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }

    }
}
