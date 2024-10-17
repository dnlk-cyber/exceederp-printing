using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Settings
{
    [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin)]
    public class FixedAssetConditionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult FixedAssetConditions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetCondition> fixedAssetConditions = _db.FixedAssetConditions;
            DataSourceResult result = fixedAssetConditions.ToDataSourceResult(request, fixedAssetCondition => new
            {
                FixedAssetConditionId = fixedAssetCondition.FixedAssetConditionId,
                Name = fixedAssetCondition.Name,
                Description = fixedAssetCondition.Description,
                DateCreated = fixedAssetCondition.DateCreated,
                LastModified = fixedAssetCondition.LastModified,
                CreatedBy = fixedAssetCondition.CreatedBy,
                ModifiedBy = fixedAssetCondition.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetConditions_Create([DataSourceRequest]DataSourceRequest request, FixedAssetCondition fixedAssetCondition)
        {
            #region validation

            bool valid = true;

            var exists = _db.FixedAssetConditions.Where(x => x.Name == fixedAssetCondition.Name).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", fixedAssetCondition.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetCondition
                    {
                        Name = fixedAssetCondition.Name,
                        Description = fixedAssetCondition.Description,
                        DateCreated = fixedAssetCondition.DateCreated,
                        LastModified = fixedAssetCondition.LastModified,
                        CreatedBy = fixedAssetCondition.CreatedBy,
                        ModifiedBy = fixedAssetCondition.ModifiedBy
                    };

                    _db.FixedAssetConditions.Add(entity);
                    _db.SaveChanges();
                    fixedAssetCondition.FixedAssetConditionId = entity.FixedAssetConditionId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetCondition", "Success", User.Identity.Name, $"created  FixedAssetCondition {entity.FixedAssetConditionId} Name {entity.Name}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetCondition }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetConditions_Update([DataSourceRequest]DataSourceRequest request, FixedAssetCondition fixedAssetCondition)
        {
            #region validation

            bool valid = true;

            var exists = _db.FixedAssetConditions.Where(x => x.FixedAssetConditionId != fixedAssetCondition.FixedAssetConditionId).Where(x => x.Name == fixedAssetCondition.Name).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", fixedAssetCondition.Name + " already exists in the system.");
                valid = false;
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetCondition
                    {
                        FixedAssetConditionId = fixedAssetCondition.FixedAssetConditionId,
                        Name = fixedAssetCondition.Name,
                        Description = fixedAssetCondition.Description,
                        DateCreated = fixedAssetCondition.DateCreated,
                        LastModified = fixedAssetCondition.LastModified,
                        CreatedBy = fixedAssetCondition.CreatedBy,
                        ModifiedBy = fixedAssetCondition.ModifiedBy
                    };

                    _db.FixedAssetConditions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetCondition", "Success", User.Identity.Name, $"updated FixedAssetCondition {entity.FixedAssetConditionId} Name {entity.Name}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetCondition }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetConditions_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetCondition fixedAssetCondition)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetCondition
                {
                    FixedAssetConditionId = fixedAssetCondition.FixedAssetConditionId,
                    Name = fixedAssetCondition.Name,
                    Description = fixedAssetCondition.Description,
                    DateCreated = fixedAssetCondition.DateCreated,
                    LastModified = fixedAssetCondition.LastModified,
                    CreatedBy = fixedAssetCondition.CreatedBy,
                    ModifiedBy = fixedAssetCondition.ModifiedBy
                };

                _db.FixedAssetConditions.Attach(entity);
                _db.FixedAssetConditions.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted  FixedAssetCondition {entity.FixedAssetConditionId} Name {entity.Name}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetCondition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
