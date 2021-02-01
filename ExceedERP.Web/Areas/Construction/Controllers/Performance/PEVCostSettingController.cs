using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class PEVCostSettingController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PEVCostSettings_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PEVCostSetting> costSettings = db.PEVCostSetting.Where(pev => pev.ProjectInformationId == id);
            DataSourceResult result = costSettings.ToDataSourceResult(request, costSetting => new
            {
                PEVCostSettingId = costSetting.PEVCostSettingId,
                Attribute = costSetting.Attribute,
                Value = costSetting.Value,
                Description = costSetting.Description,
                ProjectInformationId = costSetting.ProjectInformationId,

                OrgBranchName = costSetting.OrgBranchName,
                DateCreated = costSetting.DateCreated,
                LastModified = costSetting.LastModified,
                CreatedBy = costSetting.CreatedBy,
                ModifiedBy = costSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PEVCostSetting_Create([DataSourceRequest]DataSourceRequest request, PEVCostSetting costSetting, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PEVCostSetting
                {
                    Attribute = costSetting.Attribute,
                    Value = costSetting.Value,
                    Description = costSetting.Description,
                    ProjectInformationId = id,

                    OrgBranchName = costSetting.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = costSetting.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = costSetting.ModifiedBy
                };

                db.PEVCostSetting.Add(entity);
                db.SaveChanges();
                costSetting.PEVCostSettingId = entity.PEVCostSettingId;


            }
            return Json(new[] { costSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PEVCostSetting_Update([DataSourceRequest]DataSourceRequest request, PEVCostSetting costSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new PEVCostSetting
                {
                    PEVCostSettingId = costSetting.PEVCostSettingId,
                    Attribute = costSetting.Attribute,
                    Value = costSetting.Value,
                    Description = costSetting.Description,
                    ProjectInformationId = costSetting.ProjectInformationId,

                    OrgBranchName = costSetting.OrgBranchName,
                    DateCreated = costSetting.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = costSetting.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PEVCostSetting.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { costSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PEVCostSetting_Destroy([DataSourceRequest]DataSourceRequest request, PEVCostSetting costSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new PEVCostSetting
                {
                    PEVCostSettingId = costSetting.PEVCostSettingId,
                    Attribute = costSetting.Attribute,
                    Value = costSetting.Value,
                    Description = costSetting.Description,
                    ProjectInformationId = costSetting.ProjectInformationId,

                    OrgBranchName = costSetting.OrgBranchName,
                    DateCreated = costSetting.DateCreated,
                    LastModified = costSetting.LastModified,
                    CreatedBy = costSetting.CreatedBy,
                    ModifiedBy = costSetting.ModifiedBy
                };

                db.PEVCostSetting.Attach(entity);
                db.PEVCostSetting.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { costSetting }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}