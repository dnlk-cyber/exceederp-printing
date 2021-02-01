using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Settings
{
    public class ProcurementRuleValueController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GlobalRuleValues_Read([DataSourceRequest]DataSourceRequest request)
        {


            foreach (KeyType type in Enum.GetValues(typeof(KeyType)))
            {
                var type1 = type;
                var keys=db.GlobalRuleValues.Where(x => x.Key == type1);
                if (!keys.Any())
                {
                    var entity = new GlobalRuleValue
                    {
                        Module = Modules.Procurement,
                        Key = type1,
                        Value =""
                    };

                    db.GlobalRuleValues.Add(entity);
                    db.SaveChanges();
                    
                }
            }

            IQueryable<GlobalRuleValue> globalrulevalues = db.GlobalRuleValues.Where(x => x.Module == Modules.Procurement);
            DataSourceResult result = globalrulevalues.ToDataSourceResult(request, globalRuleValue => new {
                GlobalRuleValueId = globalRuleValue.GlobalRuleValueId,
                Module = globalRuleValue.Module,
                Key = globalRuleValue.Key,
                Value = globalRuleValue.Value
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalRuleValues_Create([DataSourceRequest]DataSourceRequest request, GlobalRuleValue globalRuleValue)
        {
            var rule = db.GlobalRuleValues
                .Where(x => x.Module == Modules.Procurement && x.Key == globalRuleValue.Key).ToList();
            if (rule.Any())
            {
                ModelState.AddModelError("exist","The rule for "+globalRuleValue.Key +" exists in this system, you can update it instead.");
            }
            if (ModelState.IsValid)
            {
                var entity = new GlobalRuleValue
                {
                    Module = Modules.Procurement,
                    Key = globalRuleValue.Key,
                    Value = globalRuleValue.Value
                };

                db.GlobalRuleValues.Add(entity);
                db.SaveChanges();
                globalRuleValue.GlobalRuleValueId = entity.GlobalRuleValueId;
            }

            return Json(new[] { globalRuleValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalRuleValues_Update([DataSourceRequest]DataSourceRequest request, GlobalRuleValue globalRuleValue)
        {
            var rule = db.GlobalRuleValues
                .Where(x => x.GlobalRuleValueId != globalRuleValue.GlobalRuleValueId && x.Module == globalRuleValue.Module && x.Key == globalRuleValue.Key).ToList();
            if (rule.Any())
            {
                ModelState.AddModelError("exist", "The rule for " + globalRuleValue.Key + " exists in this system, you can update it instead.");
            }

            if (ModelState.IsValid)
            {
                var entity = new GlobalRuleValue
                {
                    GlobalRuleValueId = globalRuleValue.GlobalRuleValueId,
                    Module = globalRuleValue.Module,
                    Key = globalRuleValue.Key,
                    Value = globalRuleValue.Value
                };

                db.GlobalRuleValues.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { globalRuleValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalRuleValues_Destroy([DataSourceRequest]DataSourceRequest request, GlobalRuleValue globalRuleValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlobalRuleValue
                {
                    GlobalRuleValueId = globalRuleValue.GlobalRuleValueId,
                    Module = globalRuleValue.Module,
                    Key = globalRuleValue.Key,
                    Value = globalRuleValue.Value
                };

                db.GlobalRuleValues.Attach(entity);
                db.GlobalRuleValues.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { globalRuleValue }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
