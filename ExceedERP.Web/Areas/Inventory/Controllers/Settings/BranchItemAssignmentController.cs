using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class BranchItemAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateCategories();
            return View();
        }

        private void PopulateCategories()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var branches = db.Branch;
            ViewData["Branches"] = branches;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult BranchItemAssignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BranchItemAssignment> branchitemassignments = db.BranchItemAssignments;
            DataSourceResult result = branchitemassignments.ToDataSourceResult(request, branchItemAssignment => new {
                BranchItemAssignmentId = branchItemAssignment.BranchItemAssignmentId,
                ItemCategoryCode = branchItemAssignment.ItemCategoryCode,
                BranchId = branchItemAssignment.BranchId,
                ItemCode = branchItemAssignment.ItemCode,
                ReOrderPoint = branchItemAssignment.ReOrderPoint,
                MinPoint = branchItemAssignment.MinPoint,
                MaxPoint = branchItemAssignment.MaxPoint,
                Balance = branchItemAssignment.Balance,
                AvgPrice = branchItemAssignment.AvgPrice,
                DateCreated = branchItemAssignment.DateCreated,
                LastModified = branchItemAssignment.LastModified,
                CreatedBy = branchItemAssignment.CreatedBy,
                ModifiedBy = branchItemAssignment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchItemAssignments_Create([DataSourceRequest]DataSourceRequest request, BranchItemAssignment branchItemAssignment)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                    bool valid = true;
                    var exists = db.BranchItemAssignments.Where(x => x.BranchId == branchItemAssignment.BranchId)
                                                          .Where(x => x.ItemCode == branchItemAssignment.ItemCode).ToList();

                    if (exists.Any())
                    {
                        ModelState.AddModelError("Branch Item",
                            branchItemAssignment.BranchId + " and " + branchItemAssignment.ItemCode + " already exists in the system.");
                        valid = false;
                    }

                    #endregion

                if (valid)
                {
                    var entity = new BranchItemAssignment
                    {
                        BranchId = branchItemAssignment.BranchId,
                        ItemCategoryCode = branchItemAssignment.ItemCategoryCode,
                        ItemCode = branchItemAssignment.ItemCode,
                        ReOrderPoint = branchItemAssignment.ReOrderPoint,
                        MinPoint = branchItemAssignment.MinPoint,
                        MaxPoint = branchItemAssignment.MaxPoint,
                        Balance = branchItemAssignment.Balance,
                        AvgPrice = branchItemAssignment.AvgPrice,
                        DateCreated =DateTime.Now,
                        LastModified = branchItemAssignment.LastModified,
                        CreatedBy =User.Identity.Name,
                        ModifiedBy = branchItemAssignment.ModifiedBy
                    };

                    db.BranchItemAssignments.Add(entity);
                    db.SaveChanges();
                    branchItemAssignment.BranchItemAssignmentId = entity.BranchItemAssignmentId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "branch item setting", "Success", User.Identity.Name, $"created item {entity.ItemCode} for branch {entity.Branch.Name}", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { branchItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchItemAssignments_Update([DataSourceRequest]DataSourceRequest request, BranchItemAssignment branchItemAssignment)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                    bool valid = true;
                    var exists = db.BranchItemAssignments.Where(x=>x.BranchItemAssignmentId!=branchItemAssignment.BranchItemAssignmentId)
                                                          .Where(x => x.BranchId == branchItemAssignment.BranchId)
                                                          .Where(x => x.ItemCode == branchItemAssignment.ItemCode).ToList();

                    if (exists.Any())
                    {
                        ModelState.AddModelError("Branch Item",
                            branchItemAssignment.BranchId + " and " + branchItemAssignment.ItemCode + " already exists in the system.");
                        valid = false;
                    }

                    #endregion

                if (valid)
                {
                    var entity = new BranchItemAssignment
                    {
                        BranchId = branchItemAssignment.BranchId,
                        BranchItemAssignmentId = branchItemAssignment.BranchItemAssignmentId,
                        ItemCategoryCode = branchItemAssignment.ItemCategoryCode,
                        ItemCode = branchItemAssignment.ItemCode,
                        ReOrderPoint = branchItemAssignment.ReOrderPoint,
                        MinPoint = branchItemAssignment.MinPoint,
                        MaxPoint = branchItemAssignment.MaxPoint,
                        Balance = branchItemAssignment.Balance,
                        AvgPrice = branchItemAssignment.AvgPrice,
                        DateCreated = branchItemAssignment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = branchItemAssignment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BranchItemAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "branch item setting", "Success", User.Identity.Name, $"updated item {entity.ItemCode} for branch {entity.Branch.Name}", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { branchItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BranchItemAssignments_Destroy([DataSourceRequest]DataSourceRequest request, BranchItemAssignment branchItemAssignment)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                    bool valid = true;
                    valid = false;
                    #endregion

                if (valid)
                {
                    var entity = new BranchItemAssignment
                    {
                        BranchId = branchItemAssignment.BranchId,
                        BranchItemAssignmentId = branchItemAssignment.BranchItemAssignmentId,
                        ItemCategoryCode = branchItemAssignment.ItemCategoryCode,
                        ItemCode = branchItemAssignment.ItemCode,
                        ReOrderPoint = branchItemAssignment.ReOrderPoint,
                        MinPoint = branchItemAssignment.MinPoint,
                        MaxPoint = branchItemAssignment.MaxPoint,
                        Balance = branchItemAssignment.Balance,
                        AvgPrice = branchItemAssignment.AvgPrice,
                        DateCreated = branchItemAssignment.DateCreated,
                        LastModified = branchItemAssignment.LastModified,
                        CreatedBy = branchItemAssignment.CreatedBy,
                        ModifiedBy = branchItemAssignment.ModifiedBy
                    };

                    db.BranchItemAssignments.Attach(entity);
                    db.BranchItemAssignments.Remove(entity);
                    db.SaveChanges();
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted branch item assignment " + branchItemAssignment.Branch.Name+ " and "+branchItemAssignment.InventoryItem.Name);
                    #endregion
                }
            }

            return Json(new[] { branchItemAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
