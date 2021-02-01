using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Settings
{
    [AuthorizeRoles(ProcurementConstants.ProcurementAdmin)]
    public class PurchasersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            
            var branches = db.Branch;

            ViewData["Branches"] = branches;
          
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

           

        }

        public ActionResult Purchasers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Purchaser> purchasers = db.Purchasers;
            DataSourceResult result = purchasers.ToDataSourceResult(request, purchaser => new {
                PurchaserId = purchaser.PurchaserId,
                EmployeeId = purchaser.EmployeeId,
                BranchId = purchaser.BranchId,
                AccountCode = purchaser.AccountCode,
                Enable = purchaser.Enable,
                OrgBranchName = purchaser.OrgBranchName,
                DateCreated = purchaser.DateCreated,
                LastModified = purchaser.LastModified,
                CreatedBy = purchaser.CreatedBy,
                ModifiedBy = purchaser.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchasers_Create([DataSourceRequest]DataSourceRequest request, Purchaser purchaser)
        {
            if (ModelState.IsValid)
            {
                var entity = new Purchaser
                {
                    EmployeeId = purchaser.EmployeeId,
                    BranchId = purchaser.BranchId,
                    Enable = purchaser.Enable,
                    AccountCode = purchaser.AccountCode,
                    OrgBranchName = purchaser.OrgBranchName,
                    DateCreated = purchaser.DateCreated,
                    LastModified = purchaser.LastModified,
                    CreatedBy = purchaser.CreatedBy,
                    ModifiedBy = purchaser.ModifiedBy
                };

                db.Purchasers.Add(entity);
                db.SaveChanges();
                purchaser.PurchaserId = entity.PurchaserId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchaser setup", "Success", User.Identity.Name, $"created purchaser {entity.AccountCode}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaser }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchasers_Update([DataSourceRequest]DataSourceRequest request, Purchaser purchaser)
        {
            if (ModelState.IsValid)
            {
                var entity = new Purchaser
                {
                    PurchaserId = purchaser.PurchaserId,
                    EmployeeId = purchaser.EmployeeId,
                    BranchId = purchaser.BranchId,
                    Enable = purchaser.Enable,
                    AccountCode = purchaser.AccountCode,
                    OrgBranchName = purchaser.OrgBranchName,
                    DateCreated = purchaser.DateCreated,
                    LastModified = purchaser.LastModified,
                    CreatedBy = purchaser.CreatedBy,
                    ModifiedBy = purchaser.ModifiedBy
                };

                db.Purchasers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Purchaser setup", "Success", User.Identity.Name, $"updated purchaser {entity.AccountCode}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { purchaser }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchasers_Destroy([DataSourceRequest]DataSourceRequest request, Purchaser purchaser)
        {
            if (ModelState.IsValid)
            {
                var entity = new Purchaser
                {
                    PurchaserId = purchaser.PurchaserId,
                    EmployeeId = purchaser.EmployeeId,
                    BranchId = purchaser.BranchId,
                    Enable = purchaser.Enable,
                    AccountCode = purchaser.AccountCode,
                    OrgBranchName = purchaser.OrgBranchName,
                    DateCreated = purchaser.DateCreated,
                    LastModified = purchaser.LastModified,
                    CreatedBy = purchaser.CreatedBy,
                    ModifiedBy = purchaser.ModifiedBy
                };

                db.Purchasers.Attach(entity);
                db.Purchasers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaser }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
