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
    public class UserBusinessUnitMapController : Controller
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
            var store = db.BusinessUnits.AsQueryable();
            ViewData["businessUnit"] = store;

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
        public ActionResult UserBusinessUnitMaps_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<UserBusinessUnitMap> UserBusinessUnitMaps = db.UserBusinessUnitMaps;
            DataSourceResult result = UserBusinessUnitMaps.ToDataSourceResult(request, UserBusinessUnitMap => new
            {
                UserBusinessUnitMapId = UserBusinessUnitMap.UserBusinessUnitMapId,
                UserId = UserBusinessUnitMap.UserId,
                BusinessUnitId = UserBusinessUnitMap.BusinessUnitId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserBusinessUnitMaps_Create([DataSourceRequest]DataSourceRequest request, UserBusinessUnitMap UserBusinessUnitMap)
        {

            #region validation

            bool valid = true;
            var exists = db.UserBusinessUnitMaps.Where(x => x.BusinessUnitId == UserBusinessUnitMap.BusinessUnitId)
                                                  .Where(x => x.UserId == UserBusinessUnitMap.UserId).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Business Unit Map","The selected user is already assigned in business unit "+UserBusinessUnitMap.BusinessUnitId);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                

                if (valid)
                {

                    var entity = new UserBusinessUnitMap
                    {
                        BusinessUnitId = UserBusinessUnitMap.BusinessUnitId,
                        UserId = UserBusinessUnitMap.UserId,
                        //CreatedBy = User.Identity.Name,
                        //DateCreated = DateTime.Now
                    };

                    db.UserBusinessUnitMaps.Add(entity);
                    db.SaveChanges();
                    UserBusinessUnitMap.UserBusinessUnitMapId = entity.UserBusinessUnitMapId;
                }
            }

            return Json(new[] { UserBusinessUnitMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserBusinessUnitMaps_Update([DataSourceRequest]DataSourceRequest request, UserBusinessUnitMap UserBusinessUnitMap)
        {
            #region validation

            bool valid = true;
            var exists = db.UserBusinessUnitMaps.Where(x => x.UserBusinessUnitMapId != UserBusinessUnitMap.UserBusinessUnitMapId)
                                    .Where(x => x.BusinessUnitId == UserBusinessUnitMap.BusinessUnitId)
                                                  .Where(x => x.UserId == UserBusinessUnitMap.UserId).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Business Unit Map", "The selected user is already assigned in business unit " + UserBusinessUnitMap.BusinessUnitId);
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new UserBusinessUnitMap
                    {
                        UserBusinessUnitMapId = UserBusinessUnitMap.UserBusinessUnitMapId,
                        UserId = UserBusinessUnitMap.UserId,
                        BusinessUnitId = UserBusinessUnitMap.BusinessUnitId,
                        //CreatedBy =UserBusinessUnitMap.CreatedBy,
                        //DateCreated=UserBusinessUnitMap.DateCreated,
                        //ModifiedBy = User.Identity.Name,
                        //LastModified = DateTime.Now
                    };

                    db.UserBusinessUnitMaps.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { UserBusinessUnitMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UserBusinessUnitMaps_Destroy([DataSourceRequest]DataSourceRequest request, UserBusinessUnitMap UserBusinessUnitMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new UserBusinessUnitMap
                {
                    UserBusinessUnitMapId = UserBusinessUnitMap.UserBusinessUnitMapId,
                    UserId = UserBusinessUnitMap.UserId
                };

                db.UserBusinessUnitMaps.Attach(entity);
                db.UserBusinessUnitMaps.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted business unit " + UserBusinessUnitMap.BusinessUnitId + " " + UserBusinessUnitMap.UserId);
                #endregion
            }

            return Json(new[] { UserBusinessUnitMap }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
