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
    public class FixedAssetGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult FixedAssetGroups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetGroup> fixedassetgroups = db.FixedAssetGroups;
            DataSourceResult result = fixedassetgroups.ToDataSourceResult(request, fixedAssetGroup => new
            {
                FixedAssetGroupID = fixedAssetGroup.FixedAssetGroupID,
                Name = fixedAssetGroup.Name,
                Description = fixedAssetGroup.Description,
                PoolingPercentage = fixedAssetGroup.PoolingPercentage,
                StraightLinePercentage = fixedAssetGroup.StraightLinePercentage,
                SalvageValue = fixedAssetGroup.SalvageValue,
                DateCreated = fixedAssetGroup.DateCreated,
                LastModified = fixedAssetGroup.LastModified,
                CreatedBy = fixedAssetGroup.CreatedBy,
                ModifiedBy = fixedAssetGroup.ModifiedBy
            });

            return Json(result);
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetGroups_Create([DataSourceRequest]DataSourceRequest request, FixedAssetGroup fixedAssetGroup)
        {
            #region validation

            bool valid = true;

            var exists = db.FixedAssetGroups.Where(x => x.Name == fixedAssetGroup.Name).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", fixedAssetGroup.Name + " already exists in the system.");
                valid = false;
            }


            #endregion
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetGroup
                    {
                        Name = fixedAssetGroup.Name,
                        Description = fixedAssetGroup.Description,
                        PoolingPercentage = fixedAssetGroup.PoolingPercentage,
                        StraightLinePercentage = fixedAssetGroup.StraightLinePercentage,
                        SalvageValue = fixedAssetGroup.SalvageValue,
                        DateCreated = fixedAssetGroup.DateCreated,
                        LastModified = fixedAssetGroup.LastModified,
                        CreatedBy = fixedAssetGroup.CreatedBy,
                        ModifiedBy = fixedAssetGroup.ModifiedBy
                    };

                    db.FixedAssetGroups.Add(entity);
                    db.SaveChanges();
                    fixedAssetGroup.FixedAssetGroupID = entity.FixedAssetGroupID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetGroup", "Success", User.Identity.Name, $"created FixedAssetGroup {entity.FixedAssetGroupID} Name {entity.Name}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetGroup }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetGroups_Update([DataSourceRequest]DataSourceRequest request, FixedAssetGroup fixedAssetGroup)
        {
            #region validation

            bool valid = true;

            var exists = db.FixedAssetGroups.Where(x => x.FixedAssetGroupID != fixedAssetGroup.FixedAssetGroupID).Where(x => x.Name == fixedAssetGroup.Name).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("Name", fixedAssetGroup.Name + " already exists in the system.");
                valid = false;
            }


            #endregion

            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FixedAssetGroup
                    {
                        FixedAssetGroupID = fixedAssetGroup.FixedAssetGroupID,
                        Name = fixedAssetGroup.Name,
                        Description = fixedAssetGroup.Description,
                        PoolingPercentage = fixedAssetGroup.PoolingPercentage,
                        StraightLinePercentage = fixedAssetGroup.StraightLinePercentage,
                        SalvageValue = fixedAssetGroup.SalvageValue,
                        DateCreated = fixedAssetGroup.DateCreated,
                        LastModified = fixedAssetGroup.LastModified,
                        CreatedBy = fixedAssetGroup.CreatedBy,
                        ModifiedBy = fixedAssetGroup.ModifiedBy
                    };

                    db.FixedAssetGroups.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetGroup", "Success", User.Identity.Name, $"update FixedAssetGroup {entity.FixedAssetGroupID} Name {entity.Name}", Modules.FixedAsset);
                    #endregion
                }
            }

            return Json(new[] { fixedAssetGroup }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetGroups_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetGroup fixedAssetGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetGroup
                {
                    FixedAssetGroupID = fixedAssetGroup.FixedAssetGroupID,
                    Name = fixedAssetGroup.Name,
                    Description = fixedAssetGroup.Description,
                    PoolingPercentage = fixedAssetGroup.PoolingPercentage,
                    StraightLinePercentage = fixedAssetGroup.StraightLinePercentage,
                    SalvageValue = fixedAssetGroup.SalvageValue,
                    DateCreated = fixedAssetGroup.DateCreated,
                    LastModified = fixedAssetGroup.LastModified,
                    CreatedBy = fixedAssetGroup.CreatedBy,
                    ModifiedBy = fixedAssetGroup.ModifiedBy
                };

                db.FixedAssetGroups.Attach(entity);
                db.FixedAssetGroups.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"delete FixedAssetGroup {fixedAssetGroup.FixedAssetGroupID} Name {fixedAssetGroup.Name}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetGroup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
