﻿using System;
﻿using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Inventory;
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Areas.Inventory.Models;
 using ExceedERP.Web.Helpers;
 using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    
    public class RevaluationItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult RevaluationItems_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<RevaluationItem> reevaluationItems = db.RevaluationItems.Where(i => i.RevaluationId == id);
            DataSourceResult result = reevaluationItems.ToDataSourceResult(request, reEvaluationItem => new {
                ItemCode = reEvaluationItem.ItemCode,
                RevaluationItemId = reEvaluationItem.RevaluationItemId,
                RevaluationId = reEvaluationItem.RevaluationId,
                NewPrice = reEvaluationItem.NewPrice,
                Status = reEvaluationItem.Status,
                ItemCategoryCode = reEvaluationItem.ItemCategoryCode
            });

            return Json(result);
        }


       [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevaluationItems_Create([DataSourceRequest]DataSourceRequest request, RevaluationItem revaluationItem , int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationItem
                {
                    RevaluationId = id,
                    NewPrice = revaluationItem.NewPrice,
                    Status = revaluationItem.Status,
                    ItemCategoryCode = revaluationItem.ItemCategoryCode,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };

                db.RevaluationItems.Add(entity);
                db.SaveChanges();
                revaluationItem.RevaluationItemId = entity.RevaluationItemId;
            }

            return Json(new[] { revaluationItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevaluationItems_Update([DataSourceRequest]DataSourceRequest request, RevaluationItem revaluationItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationItem
                {
                    RevaluationItemId = revaluationItem.RevaluationItemId,
                    PreviousPrice = revaluationItem.PreviousPrice,
                    RevaluationId = revaluationItem.RevaluationId,
                    NewPrice = revaluationItem.NewPrice,
                    Status = true,
                    ItemCategoryCode = revaluationItem.ItemCategoryCode,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now
                };

                db.RevaluationItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { revaluationItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevaluationItems_Destroy([DataSourceRequest]DataSourceRequest request, RevaluationItem revaluationItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationItem
                {
                    RevaluationItemId = revaluationItem.RevaluationItemId,
                    NewPrice = revaluationItem.NewPrice,
                    Status = revaluationItem.Status,
                    ItemCategoryCode = revaluationItem.ItemCategoryCode
                };

                db.RevaluationItems.Attach(entity);
                db.RevaluationItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted revaluation item " + revaluationItem.RevaluationId + " " + revaluationItem.ItemCode);
                #endregion
            }

            return Json(new[] { revaluationItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
