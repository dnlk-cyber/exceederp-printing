using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class StoreKeeperAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext appdb = new ApplicationDbContext();

        public ActionResult Index()
        {
            PopulateStoreAndEmployee();
            return View();
        }
        private void PopulateStoreAndEmployee()
        {
            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;

            var employee = db.Person_Employee;

            var selectedEmployee = employee.Select(i => new { Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish, Code = i.EmployeeId })
                .AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var users = appdb.Users.AsQueryable();
            var selectedUser = users.Select(i => new {
                Name = i.UserName,
                Code = i.Id
            })
                .AsQueryable();

            ViewData["users"] = selectedUser;

        }
        public ActionResult StoreKeeperAssignments_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StoreKeeperAssignment> storekeeperassignments = db.StoreKeeperAssignments;
            DataSourceResult result = storekeeperassignments.ToDataSourceResult(request, storeKeeperAssignment => new
            {
                StoreKeeperAssignmentID = storeKeeperAssignment.StoreKeeperAssignmentID,
                StoreKeeper = storeKeeperAssignment.StoreKeeper,
                StoreCode = storeKeeperAssignment.StoreCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreKeeperAssignments_Create([DataSourceRequest]DataSourceRequest request, StoreKeeperAssignment storeKeeperAssignment)
        {

            #region validation

            bool valid = true;
            var exists = db.StoreKeeperAssignments.Where(x => x.StoreCode == storeKeeperAssignment.StoreCode)
                                                  .Where(x => x.StoreKeeper == storeKeeperAssignment.StoreKeeper).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Store Keeper","The selected store keeper is already assigned in store "+storeKeeperAssignment.StoreCode);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                

                if (valid)
                {

                    var entity = new StoreKeeperAssignment
                    {
                        StoreCode = storeKeeperAssignment.StoreCode,
                        StoreKeeper = storeKeeperAssignment.StoreKeeper,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.StoreKeeperAssignments.Add(entity);
                    db.SaveChanges();
                    storeKeeperAssignment.StoreKeeperAssignmentID = entity.StoreKeeperAssignmentID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store keeper assignment", "Success", User.Identity.Name, $"created store keeper {entity.StoreKeeper} in store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { storeKeeperAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreKeeperAssignments_Update([DataSourceRequest]DataSourceRequest request, StoreKeeperAssignment storeKeeperAssignment)
        {
            #region validation

            bool valid = true;
            var exists = db.StoreKeeperAssignments.Where(x => x.StoreKeeperAssignmentID != storeKeeperAssignment.StoreKeeperAssignmentID)
                                    .Where(x => x.StoreCode == storeKeeperAssignment.StoreCode)
                                                  .Where(x => x.StoreKeeper == storeKeeperAssignment.StoreKeeper).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Store Keeper", "The selected store keeper is already assigned in store " + storeKeeperAssignment.StoreCode);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new StoreKeeperAssignment
                    {
                        StoreKeeperAssignmentID = storeKeeperAssignment.StoreKeeperAssignmentID,
                        StoreKeeper = storeKeeperAssignment.StoreKeeper,
                        StoreCode = storeKeeperAssignment.StoreCode,
                        CreatedBy =storeKeeperAssignment.CreatedBy,
                        DateCreated=storeKeeperAssignment.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.StoreKeeperAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store keeper assignment", "Success", User.Identity.Name, $"updated store keeper  {entity.StoreKeeper} in store {entity.StoreCode}  ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { storeKeeperAssignment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreKeeperAssignments_Destroy([DataSourceRequest]DataSourceRequest request, StoreKeeperAssignment storeKeeperAssignment)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreKeeperAssignment
                {
                    StoreKeeperAssignmentID = storeKeeperAssignment.StoreKeeperAssignmentID,
                    StoreKeeper = storeKeeperAssignment.StoreKeeper
                };

                db.StoreKeeperAssignments.Attach(entity);
                db.StoreKeeperAssignments.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store keeper " + storeKeeperAssignment.StoreCode + " " + storeKeeperAssignment.StoreKeeper);
                #endregion
            }

            return Json(new[] { storeKeeperAssignment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
