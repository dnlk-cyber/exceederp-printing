using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
    public class RevaluationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Revaluations_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<Revaluation> reevaluations = db.Revaluations.Where(x => mybraches.Contains(x.OrgBranchName));
            DataSourceResult result = reevaluations.ToDataSourceResult(request, reEvaluation => new
            {
                RevaluationId = reEvaluation.RevaluationId,
                StoreCode = reEvaluation.StoreCode,
                Status = reEvaluation.Status,
                ItemCategoryCode = reEvaluation.ItemCategoryCode,
                CreatedBy = reEvaluation.CreatedBy,
                DateCreated = reEvaluation.DateCreated,
                IsOnlineApproved = reEvaluation.IsOnlineApproved,
                OnlineApprovedTime = reEvaluation.OnlineApprovedTime,
                OnlineApprovedBy = reEvaluation.OnlineApprovedBy,
            });

            return Json(result);
        }

        private void ItemInCategory(Revaluation record)
        {
            RevaluationItem reEvaluatedItem;

            List<StoreItemAssignment> itemInStore = db.StoreItemAssignments.Where(i => i.InventoryItem.ItemCategoryCode == record.ItemCategoryCode && i.StoreCode == record.StoreCode).ToList();

            foreach (var item in itemInStore)
            {
                var itemPrice = db.InventoryItems.Find(item.ItemCode);
                reEvaluatedItem = new RevaluationItem();
                reEvaluatedItem.ItemCode = item.ItemCode;
                reEvaluatedItem.NewPrice = itemPrice.AvgPrice;
                reEvaluatedItem.ItemCategoryCode = itemPrice.ItemCategoryCode;
                reEvaluatedItem.NewPrice = 0;
                reEvaluatedItem.RevaluationId = record.RevaluationId;
                reEvaluatedItem.Status = false;

                db.RevaluationItems.Add(reEvaluatedItem);
                db.SaveChanges();
            }
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Revaluations_Create([DataSourceRequest]DataSourceRequest request, Revaluation revaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Revaluation
                {

                    StoreCode = revaluation.StoreCode,
                    ItemCategoryCode = revaluation.ItemCategoryCode,
                    Status = Enum.GetName(typeof(OperationStatuses), 4),
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    OrgBranchName = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(revaluation.StoreCode).ToString()
                };

                db.Revaluations.Add(entity);
                db.SaveChanges();
                revaluation.RevaluationId = entity.RevaluationId;
                ItemInCategory(entity);
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Revaluations_Update([DataSourceRequest]DataSourceRequest request, Revaluation revaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Revaluation
                {
                    RevaluationId = revaluation.RevaluationId,
                    StoreCode = revaluation.StoreCode,
                    ItemCategoryCode = revaluation.ItemCategoryCode,
                    CreatedBy = revaluation.CreatedBy,
                    DateCreated = revaluation.DateCreated,
                    IsOnlineApproved = revaluation.IsOnlineApproved,
                    OnlineApprovedTime = revaluation.OnlineApprovedTime,
                    OnlineApprovedBy = revaluation.OnlineApprovedBy,
                };

                db.Revaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Revaluations_Destroy([DataSourceRequest]DataSourceRequest request, Revaluation revaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Revaluation
                {
                    RevaluationId = revaluation.RevaluationId,
                    StoreCode = revaluation.StoreCode,
                    ItemCategoryCode = revaluation.ItemCategoryCode,
                    CreatedBy = revaluation.CreatedBy,
                    DateCreated = revaluation.DateCreated,
                    IsOnlineApproved = revaluation.IsOnlineApproved,
                    OnlineApprovedTime = revaluation.OnlineApprovedTime,
                    OnlineApprovedBy = revaluation.OnlineApprovedBy,
                };

                db.Revaluations.Attach(entity);
                db.Revaluations.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted revaluation number " + revaluation.RevaluationId + " " + revaluation.Remark);
                #endregion
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
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
