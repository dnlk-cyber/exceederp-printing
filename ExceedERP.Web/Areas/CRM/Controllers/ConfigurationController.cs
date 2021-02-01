using System; using ExceedERP.Web.Helpers;
using System.CodeDom;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
 using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;


namespace ExceedERP.Web.Areas.CRM.Controllers
{
        [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class ConfigurationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Configurations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Configuration> configurations = db.Configurations.Where(c => c.Reference != "Leave");
            DataSourceResult result = configurations.ToDataSourceResult(request, configuration => new {
                ConfigurationId = configuration.ConfigurationId,
                Reference = configuration.Reference,
                Attribute = configuration.Attribute,
                CurrentValue = configuration.CurrentValue,
                PreviousValue = configuration.PreviousValue,
                Remark = configuration.Remark
            });

            return Json(result);
        }

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
                   // CreatedBy = User.Identity.Name,
                  //  DateCreated = DateTime.Now,
                    Remark = configuration.Remark
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
                case "VOUCHER_CONFIG":
                    attributes =
                          Enum.GetValues(typeof(PriceSelection))
                              .Cast<PriceSelection>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCascadeCurrentValue(string Attribute)
        {
            List<string> attributes = new List<string>();
            switch (Attribute)

            {
                case "PRICE_SELECTION":
                    attributes =
                          Enum.GetValues(typeof(PriceSelectionValue))
                              .Cast<PriceSelectionValue>()
                              .Select(v => v.ToString())
                              .ToList();
                    break;
               

            }


            return Json(attributes.Select(s => new { Text = s.ToString(), Value = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = new List<string>();

            attributes =
                  Enum.GetValues(typeof(SettingCategories))
                      .Cast<SettingCategories>()
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
