﻿using System;
﻿using System.CodeDom;
﻿using System.Collections.Generic;
﻿using System.Configuration;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.DataAccess.Context;
﻿using Configuration = ExceedERP.Core.Domain.Common.HRM.Configuration;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    public class ConfigurationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = HR_Admin)]
        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where( c=> c.Reference !="Leave");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationID = configuration.ConfigurationId,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark
            });

            return Json(result);
        }
          [Authorize(Roles = HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration, string Attribute, string CurrentValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    Reference = configuration.Reference,
                    Attribute = Attribute,
                    CurrentValue = CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark
                };

                db.Configurations.Add(entity);
                db.SaveChanges();
                configuration.ConfigurationId = entity.ConfigurationId;
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = HR_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                var entity = new Configuration
                {
                    ConfigurationId = configuration.ConfigurationId,
                    Reference = configuration.Reference,
                    Attribute = configuration.Attribute,
                    CurrentValue = configuration.CurrentValue,
                    PreviousValue = configuration.PreviousValue,
                    Remark = configuration.Remark
                };

                db.Configurations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
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

        public JsonResult GetCascadeCurrentValue(string Attribute)
        {
            List<string> attributes = new List<string>();
            //switch  (Attribute)
                
            //{
            //    case "Pension":
            //    attributes =
            //          Enum.GetValues(typeof(PensionAttribute  ))
            //              .Cast<PensionAttribute>()
            //              .Select(v => v.ToString())
            //              .ToList();
            //     break;
            //    case "Provident_Fund":
            //     attributes =
            //           Enum.GetValues(typeof(ProvidentAttribute))
            //               .Cast<ProvidentAttribute>()
            //               .Select(v => v.ToString())
            //               .ToList();
            //     break;

            //}


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();
          
                attributes =
                      Enum.GetValues(typeof(SettingCategories ))
                          .Cast<SettingCategories>()
                          .Select(v => v.ToString())
                          .ToList();
           

            return Json(attributes.Select(s => new { Text=s.ToString(),Value=s.ToString ()}), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
