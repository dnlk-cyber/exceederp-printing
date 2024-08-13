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

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    [Authorize(Roles = Insurance_Admin)]
    [AuthorizeRoles(HrmConstants.HRInsuranceAdmin)]
    public class CourtInsuranceSettingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Insurance_Admin = "Insurance_Admin";
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
      
        public JsonResult GetCategories()
        {
           List<string> attributes=  Enum.GetValues(typeof(InsuranceAttribute))
                              .Cast<InsuranceAttribute>()
                              .Select(v => v.ToString())
                              .ToList();
                 
            return Json(attributes.Select(s => new { Text = s.ToString()}), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
