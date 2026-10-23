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
    public class BudgetTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /Budget/BudgetType/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BudgetType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetType> BudgetTypes = db.BudgetTypes;
            DataSourceResult result = BudgetTypes.ToDataSourceResult(request, BudgetType => new
            {
                BudgetTypeId = BudgetType.BudgetTypeId,
                Name = BudgetType.Name,
                Description = BudgetType.Description,
                CreatedBy = BudgetType.CreatedBy,
                DateCreated = BudgetType.DateCreated,
                LastModified = BudgetType.LastModified,
                ModifiedBy = BudgetType.ModifiedBy


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetType_Create([DataSourceRequest]DataSourceRequest request, BudgetType BudgetType)
        {
            List<BudgetType> category = db.BudgetTypes.Where(x => x.Name == BudgetType.Name).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", "  Name Already Exists !");

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetType
                    {
                        BudgetTypeId = BudgetType.BudgetTypeId,
                        Name = BudgetType.Name,
                        Description = BudgetType.Description,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetTypes.Add(entity);
                    db.SaveChanges();



                    BudgetType.BudgetTypeId = entity.BudgetTypeId;
                }
            }

            return Json(new[] { BudgetType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetType_Update([DataSourceRequest]DataSourceRequest request, BudgetType BudgetType)
        {
            List<BudgetType> category = db.BudgetTypes.Where(x => x.Name == BudgetType.Name && x.BudgetTypeId != BudgetType.BudgetTypeId).ToList();


            if (category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");

            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new BudgetType
                    {
                        BudgetTypeId = BudgetType.BudgetTypeId,
                        Name = BudgetType.Name,
                        Description = BudgetType.Description,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetTypes.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { BudgetType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetType_Destroy([DataSourceRequest]DataSourceRequest request, BudgetType BudgetType)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetType
                {
                    BudgetTypeId = BudgetType.BudgetTypeId,


                };

                db.BudgetTypes.Attach(entity);
                db.BudgetTypes.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + BudgetType.BudgetTypeId);
            }

            return Json(new[] { BudgetType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        public ActionResult BudgetTypeDDBind()
        {

            var BudgetTypes = db.BudgetTypes.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BudgetTypeId.ToString()
            });

            return Json(BudgetTypes, JsonRequestBehavior.AllowGet);
        }

    }
}
