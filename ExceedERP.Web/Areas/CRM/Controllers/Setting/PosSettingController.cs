using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers.Setting
{
    public class PosSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PosSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PosSetting> possetting = db.PosSetting;
            DataSourceResult result = possetting.ToDataSourceResult(request, posSetting => new
            {
                PosSettingId = posSetting.PosSettingId,
                SettingType = posSetting.SettingType,
                Value = posSetting.Value,
                OrgBranchName = posSetting.OrgBranchName,
                DateCreated = posSetting.DateCreated,
                LastModified = posSetting.LastModified,
                CreatedBy = posSetting.CreatedBy,
                ModifiedBy = posSetting.ModifiedBy
            });

            return Json(result);
        }
        private bool SettingExis(PosSetting model)
        {
            return db.PosSetting.FirstOrDefault(p => p.PosSettingId != model.PosSettingId && model.SettingType == p.SettingType) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PosSetting_Create([DataSourceRequest]DataSourceRequest request, PosSetting posSetting)
        {
            if (ModelState.IsValid)
            {
                if (SettingExis(posSetting))
                {
                    ModelState.AddModelError("", "Duplicate Setting not allowed");
                }
                else
                {
                    var entity = new PosSetting
                    {
                        SettingType = posSetting.SettingType,
                        Value = posSetting.Value,
                        OrgBranchName = posSetting.OrgBranchName,
                        DateCreated = posSetting.DateCreated,
                        LastModified = posSetting.LastModified,
                        CreatedBy = posSetting.CreatedBy,
                        ModifiedBy = posSetting.ModifiedBy
                    };

                    db.PosSetting.Add(entity);
                    db.SaveChanges();
                    posSetting.PosSettingId = entity.PosSettingId;
                }
            }
            return Json(new[] { posSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PosSetting_Update([DataSourceRequest]DataSourceRequest request, PosSetting posSetting)
        {
            if (ModelState.IsValid)
            {
                if (SettingExis(posSetting))
                {
                    ModelState.AddModelError("", "Duplicate Setting not allowed");
                }
                else
                {
                    var entity = new PosSetting
                    {
                        PosSettingId = posSetting.PosSettingId,
                        SettingType = posSetting.SettingType,
                        Value = posSetting.Value,
                        OrgBranchName = posSetting.OrgBranchName,
                        DateCreated = posSetting.DateCreated,
                        LastModified = posSetting.LastModified,
                        CreatedBy = posSetting.CreatedBy,
                        ModifiedBy = posSetting.ModifiedBy
                    };

                    db.PosSetting.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return Json(new[] { posSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PosSetting_Destroy([DataSourceRequest]DataSourceRequest request, PosSetting posSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new PosSetting
                {
                    PosSettingId = posSetting.PosSettingId,
                    SettingType = posSetting.SettingType,
                    Value = posSetting.Value,
                    OrgBranchName = posSetting.OrgBranchName,
                    DateCreated = posSetting.DateCreated,
                    LastModified = posSetting.LastModified,
                    CreatedBy = posSetting.CreatedBy,
                    ModifiedBy = posSetting.ModifiedBy
                };

                db.PosSetting.Attach(entity);
                db.PosSetting.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { posSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
