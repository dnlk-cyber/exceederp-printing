﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Insurance
{
 
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]

    public class InsuranceSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
      
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where( x=> x.Category =="Insurance");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationID = configuration.ConfigurationId,
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
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    Category = "Insurance",
                    Reference = configuration.Reference,
                    Attribute = "Insurance",
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark
                    ,
                    DateCreated = DateTime.Now,
                    LastModified = configuration.LastModified,

                    CreatedBy = User.Identity.Name,

                    ModifiedBy = configuration.ModifiedBy 
                };

                db.Configurations.Add(entity);
                db.SaveChanges();
                configuration.ConfigurationId = entity.ConfigurationId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Setting", "Success", User.Identity.Name, User.Identity.Name + " add Insurance Setting at Date " + DateTime.Today + " Reference " + entity.Reference, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
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
                     DateCreated = configuration.DateCreated ,
                    LastModified =DateTime .Now ,
                    CreatedBy = configuration.CreatedBy ,
                    ModifiedBy = User.Identity.Name
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Insurance Setting", "Success", User.Identity.Name, User.Identity.Name + " update Insurance Setting at Date " + DateTime.Today + " Reference " + entity.Reference, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Destroy([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            ModelState.Clear();
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
                    Remark = configuration.Remark
                    ,

                    DateCreated = configuration.DateCreated,
                    LastModified = configuration.LastModified,

                    CreatedBy = configuration.CreatedBy,

                    ModifiedBy = configuration.ModifiedBy 
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted insrance setting  information of " + configuration.Attribute);
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


                case "OVERTIME":

                    attributes =
                          Enum.GetValues(typeof(OverTimeAttribute))
                              .Cast<OverTimeAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PENSION":
                    attributes =
                          Enum.GetValues(typeof(PensionAttribute))
                              .Cast<PensionAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
                case "PROVIDENT_FUND":
                    attributes =
                          Enum.GetValues(typeof(ProvidentAttribute))
                              .Cast<ProvidentAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();
            var businessClass = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS).ToList();

            attributes = businessClass.Select(x => x.Description).ToList() ;
                 
            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
