using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class CreditSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreditSettings_Read([DataSourceRequest]DataSourceRequest request, int OrgID)
        {
            IQueryable<CreditSetting> CreditSettings = db.CreditSettings.Where(b => b.OrganizationID == OrgID);
            DataSourceResult result = CreditSettings.ToDataSourceResult(request, CreditSetting => new
            {
                CreditSettingID = CreditSetting.CreditSettingID,
                OrganizationID = CreditSetting.OrganizationID,
                Quantity = CreditSetting.Quantity,
                CreatedBy = CreditSetting.CreatedBy,
                DateCreated = CreditSetting.DateCreated,
                LastModified = CreditSetting.LastModified,
                ModifiedBy = CreditSetting.ModifiedBy,
                OrgBranchName = CreditSetting.OrgBranchName,
                Date = CreditSetting.Date

            });

            return Json(result);
        }


        public ActionResult CreditSettings_Create([DataSourceRequest]DataSourceRequest request, CreditSetting CreditSetting, int CustomerID)
        {
            if (ModelState.IsValid)
            {
                var entity = new CreditSetting
                {
                    OrganizationID = CustomerID,
                    Date = DateTime.Now,
                    Quantity = CreditSetting.Quantity,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                };

                db.CreditSettings.Add(entity);
                db.SaveChanges();
                CreditSetting.CreditSettingID = entity.CreditSettingID;

            }
            return Json(new[] { CreditSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettings_Update([DataSourceRequest]DataSourceRequest request, CreditSetting creditSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new CreditSetting
                {
                    CreditSettingID = creditSetting.CreditSettingID,
                    OrganizationID = creditSetting.OrganizationID,
                    Date = creditSetting.Date,
                    Quantity = creditSetting.Quantity,
                    DateCreated = creditSetting.DateCreated,
                    CreatedBy = creditSetting.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.CreditSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { creditSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettings_Destroy([DataSourceRequest]DataSourceRequest request, CreditSetting creditSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = db.CreditSettings.Find(creditSetting.CreditSettingID);

                db.CreditSettings.Attach(entity);
                db.CreditSettings.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { creditSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
