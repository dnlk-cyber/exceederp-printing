using System;
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

    public class TagCodeSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

     
    
        public ActionResult TagSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TagCodeSetting> tagsettings = db.TagCodeSettings;
            DataSourceResult result = tagsettings.ToDataSourceResult(request, tagSetting => new {
                TagCodeSettingId = tagSetting.TagCodeSettingId,
                Type = tagSetting.Type,
                Index = tagSetting.Index,
                Remark = tagSetting.Remark
            });

            return Json(result);
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TagSettings_Create([DataSourceRequest]DataSourceRequest request, TagCodeSetting tagSetting)
        {
            #region validation
            
            var exists = db.TagCodeSettings.Where(x => x.Type == tagSetting.Type).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("type",tagSetting.Type + "already exists in the system.");
            }

            var exists1 = db.TagCodeSettings.Where(x => x.Index == tagSetting.Index).ToList();

            if (exists1.Any())
            {
                ModelState.AddModelError("type", tagSetting.Index + " already exists in the system.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new TagCodeSetting
                {
                    Type = tagSetting.Type,
                    Index = tagSetting.Index,
                    Remark = tagSetting.Remark
                };

                db.TagCodeSettings.Add(entity);
                db.SaveChanges();
                tagSetting.TagCodeSettingId = entity.TagCodeSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "TagCodeSetting", "Success", User.Identity.Name, $"created TagCodeSetting {entity.TagCodeSettingId} Index {entity.Index}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { tagSetting }.ToDataSourceResult(request, ModelState));
        }
    
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TagSettings_Update([DataSourceRequest]DataSourceRequest request, TagCodeSetting tagSetting)
        {
            #region validation

            var exists = db.TagCodeSettings.Where(x => x.TagCodeSettingId != tagSetting.TagCodeSettingId).Where(x => x.Type == tagSetting.Type).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("type", tagSetting.Type + " already exists in the system.");
            }

            var exists1 = db.TagCodeSettings.Where(x => x.TagCodeSettingId != tagSetting.TagCodeSettingId).Where(x => x.Index == tagSetting.Index).ToList();

            if (exists1.Any())
            {
                ModelState.AddModelError("type", tagSetting.Index + " already exists in the system.");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new TagCodeSetting
                {
                    TagCodeSettingId = tagSetting.TagCodeSettingId,
                    Type = tagSetting.Type,
                    Index = tagSetting.Index,
                    Remark = tagSetting.Remark
                };

                db.TagCodeSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "TagCodeSetting", "Success", User.Identity.Name, $"update TagCodeSetting {entity.TagCodeSettingId} Index {entity.Index}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { tagSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TagSettings_Destroy([DataSourceRequest]DataSourceRequest request, TagCodeSetting tagSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new TagCodeSetting
                {
                    Type = tagSetting.Type,
                    Index = tagSetting.Index,
                    Remark = tagSetting.Remark
                };

                db.TagCodeSettings.Attach(entity);
                db.TagCodeSettings.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete TagCodeSetting {tagSetting.TagCodeSettingId} Index {tagSetting.Index}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { tagSetting }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
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
