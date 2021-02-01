using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = Payroll_Admin)]
    public class ContributionController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Admin = "Payroll_Admin";
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lookups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContributionType> lookups = db.ContributionTypes;
            DataSourceResult result = lookups.ToDataSourceResult(request, lookup => new {
                ContributionTypeId = lookup.ContributionTypeId,               
                Description = lookup.Description,
                ContributionCategory = lookup.ContributionCategory,             
                Remark = lookup.Remark,
                DateCreated = lookup.DateCreated,
                LastModified = lookup.LastModified,
                CreatedBy = lookup.CreatedBy,
                ModifiedBy = lookup.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Create([DataSourceRequest]DataSourceRequest request, ContributionType lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExitance = db.ContributionTypes.Where(x => x.ContributionCategory == lookup.ContributionCategory && x.Description == lookup.Description).FirstOrDefault();
                 if (checkExitance != null)
                 {
                     ModelState.AddModelError("", "This contribution type is already exist!!");
                 }
                 else
                 {
                     var entity = new ContributionType
                     {
                       //  ContributionTypeId = lookup.ContributionTypeId,
                         Description = lookup.Description,
                         ContributionCategory = lookup.ContributionCategory,
                         Remark = lookup.Remark,
                         DateCreated = System.DateTime.Now,
                         LastModified = lookup.LastModified,
                         CreatedBy = User.Identity.Name,
                         ModifiedBy = lookup.ModifiedBy
                     };

                     db.ContributionTypes.Add(entity);
                     db.SaveChanges();
                     lookup.ContributionTypeId = entity.ContributionTypeId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll contribution", "Success", User.Identity.Name, $"added contribution type  {entity.Description} of category {entity.ContributionCategory} ", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Update([DataSourceRequest]DataSourceRequest request, ContributionType lookup)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExitance = db.PayrollProcessDeductions.Where(x => x.Name == lookup.Description).FirstOrDefault();
                 if (checkExitance != null)
                 {
                     ModelState.AddModelError("", "You cannot delete this contribution type. It already processed!!");
                 }
                 else
                 {
                     var entity = new ContributionType
                     {
                         ContributionTypeId = lookup.ContributionTypeId,
                         Description = lookup.Description,
                         ContributionCategory = lookup.ContributionCategory,
                         Remark = lookup.Remark,
                         DateCreated = lookup.DateCreated,
                         LastModified = System.DateTime.Now,
                         CreatedBy = lookup.CreatedBy,
                         ModifiedBy = User.Identity.Name
                     };

                     db.ContributionTypes.Attach(entity);
                     db.Entry(entity).State = EntityState.Modified;
                     db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll contribution", "Success", User.Identity.Name, $"updated contribution type  {entity.Description} of category {entity.ContributionCategory} ", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Lookups_Destroy([DataSourceRequest]DataSourceRequest request, ContributionType  lookup)
   {
       ModelState.Clear();
            if (ModelState.IsValid)
            {
                 var checkExitance = db.PayrollProcessDeductions.Where (x=> x.Name==lookup.Description).FirstOrDefault();
                 if (checkExitance != null)
                 {
                     ModelState.AddModelError("", "You cannot delete this contribution type. It already processed!!");
                 }
                 else
                 {
                     var entity = new ContributionType
                     {
                          ContributionTypeId = lookup.ContributionTypeId,
                         Description = lookup.Description,
                         ContributionCategory = lookup.ContributionCategory,
                         Remark = lookup.Remark,
                         DateCreated = lookup.DateCreated,
                         LastModified = lookup.LastModified,
                         CreatedBy = lookup.CreatedBy,
                         ModifiedBy = lookup.ModifiedBy
                     };
                     #region delete log
                     UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Contribution information of type " + lookup.Description,Modules.Payroll);
                     #endregion
                     db.ContributionTypes.Attach(entity);
                     db.ContributionTypes.Remove(entity);
                     db.SaveChanges();
                 }
            }

            return Json(new[] { lookup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
