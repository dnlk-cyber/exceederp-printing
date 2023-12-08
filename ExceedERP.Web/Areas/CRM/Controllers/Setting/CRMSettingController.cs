using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers.Setting
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class CRMSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "CRM");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationId = configuration.ConfigurationId,
                Category = configuration.Category,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark,
               
                DateCreated = configuration.DateCreated,
                LastModified = configuration.LastModified,

                CreatedBy = configuration.CreatedBy,

                ModifiedBy = configuration.ModifiedBy 
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration,string Attribute, string CurrentValue)
        {
            var checkexistance = db.Configurations
                    .Where(x => x.Category == "CRM")
                    .Where(x => x.Reference == configuration.Reference)
                    .Where(x => x.Attribute == Attribute)
                    .FirstOrDefault();
            if (checkexistance != null)
                ModelState.AddModelError("", "This setting is already registered!!");

            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    Category = "CRM",
                    Reference = configuration.Reference,
                    Attribute = Attribute,
                    CurrentValue = CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,

                    DateCreated = DateTime.Now,
                    LastModified = configuration.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = configuration.ModifiedBy
                };

                db.Configurations.Add(entity);
                db.SaveChanges();
                configuration.ConfigurationId = entity.ConfigurationId;
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            var checkexistance = db.Configurations
                .Where(x=>x.ConfigurationId!=configuration.ConfigurationId)
                   .Where(x => x.Category == "CRM")
                   .Where(x => x.Reference == configuration.Reference)
                   .Where(x => x.Attribute == configuration.Attribute)
                   .FirstOrDefault();
            if (checkexistance != null)
                ModelState.AddModelError("", "This setting is already registered!!");

            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = configuration.Category,
                    Reference = configuration.Reference,
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,
                    DateCreated = configuration.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = configuration.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Category = configuration.Category,
                    Reference = configuration.Reference,
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark,
                    DateCreated = configuration.DateCreated,
                    LastModified = configuration.LastModified,

                    CreatedBy = configuration.CreatedBy,

                    ModifiedBy = configuration.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted CRM setting information of setting " + configuration.Attribute);
                #endregion
                db.Configurations.Attach(entity);
                db.Configurations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetCascadeAttributes(string Reference)
        {
            List<string> attributes = new List<string>();
            switch (Reference)
            {
                case "DISABLE_REPLACING_BRANCH_SEGMENT":
                    attributes =
                          Enum.GetValues(typeof(DisableReplacingBranchSegment))
                              .Cast<DisableReplacingBranchSegment>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            var attributes =
                  Enum.GetValues(typeof(CRMSettingCategories))
                      .Cast<CRMSettingCategories>()
                      .Select(v => v.ToString())
                      .ToList();
            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
