using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class SettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }
        public ActionResult Weights()
        {
            return View();
        }

        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "Construction");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new
            {
                ConfigurationID = configuration.ConfigurationId,
                Category = configuration.Category,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark
            });

            return Json(result);
        }

        public ActionResult PEWegihtConfigurations_Read([DataSourceRequest]DataSourceRequest request, string reference)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Category == "Construction" && c.Reference == reference);
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new
            {
                ConfigurationID = configuration.ConfigurationId,
                Category = configuration.Category,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark
            });

            return Json(result);
        }
        private bool DuplicateValueExists(Configuration configuration)
        {
            return db.Configurations
                .FirstOrDefault(c => c.Attribute == configuration.Attribute &&
                                c.CurrentValue == configuration.CurrentValue &&
                                c.ConfigurationId != configuration.ConfigurationId
            ) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PEWegihtConfigurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration, string attribute, string currentValue)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                // var checkExistance= db.Configurations .w
                
                    var entity = new Configuration
                    {
                        Category = "Construction",
                        Reference = "PerformanceEvaluationComponent",//estimation.ToString(),
                        Attribute = attribute,
                        CurrentValue = currentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark
                    };

                    db.Configurations.Add(entity);
                    db.SaveChanges();
                    configuration.ConfigurationId = entity.ConfigurationId;
                
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Create([DataSourceRequest]DataSourceRequest request, Configuration configuration, string attribute, string currentValue)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                // var checkExistance= db.Configurations .w
                if (DuplicateValueExists(configuration))
                {
                    ModelState.AddModelError("", "Duplicate setting attributes not allowed");
                }
                else
                {
                    var entity = new Configuration
                    {
                        Category = "Construction",
                        Reference = attribute,//estimation.ToString(),
                        Attribute = attribute,
                        CurrentValue = currentValue,
                        PreviousValue = configuration.PreviousValue,
                        Remark = configuration.Remark
                    };

                    db.Configurations.Add(entity);
                    db.SaveChanges();
                    configuration.ConfigurationId = entity.ConfigurationId;
                }
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Configurations_Update([DataSourceRequest]DataSourceRequest request, Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                if (DuplicateValueExists(configuration))
                {
                    ModelState.AddModelError("", "Duplicate setting attributes not allowed");
                }
                else
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
                    };

                    db.Configurations.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
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
                    Remark = configuration.Remark
                };

                db.Configurations.Attach(entity);
                db.Configurations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { configuration }.ToDataSourceResult(request, ModelState));
        }
        //public JsonResult GetCascadeAttributes(string Reference)
        //{
        //    List<string> attributes = new List<string>();
        //    Reference = "Construction";
        //    switch (Reference)
        //    {
        //        case "Construction":

        //            attributes =
        //                  Enum.GetValues(typeof(ConstructionSettingCategories))
        //                      .Cast<ConstructionSettingCategories>()
        //                      .Select(v => v.ToString())
        //                      .ToList();
        //            break;
        //    }

        //    return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetCategories()
        //{
        //    List<string> attributes = new List<string>();

        //    attributes =
        //          Enum.GetValues(typeof(ConstructionSettingCategories))
        //              .Cast<ConstructionSettingCategories>()
        //              .Select(v => v.ToString())
        //              .ToList();


        //    return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult GetPerformanceEvaluationAttributes()
        //{
        //    List<string> attributes = new List<string>();
        //    //Reference = "Construction";
        //    //switch (Reference)
        //    //{
        //        //case "Construction":

        //            attributes =
        //                  Enum.GetValues(typeof(PerformanceEvaluationComponentWeightSetting))
        //                      .Cast<PerformanceEvaluationComponentWeightSetting>()
        //                      .Select(v => v.ToString())
        //                      .ToList();
        //            //break;
        //    //}

        //    return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
