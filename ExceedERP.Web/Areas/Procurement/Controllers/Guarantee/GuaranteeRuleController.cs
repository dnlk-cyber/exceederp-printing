using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    [AuthorizeRoles(ProcurementConstants.ProcurementAdmin)]
    public class GuaranteeRuleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GuaranteeRules_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GuaranteeRule> guaranteerules = db.GuaranteeRules;
            DataSourceResult result = guaranteerules.ToDataSourceResult(request, guaranteeRule => new {
                GuaranteeRuleId = guaranteeRule.GuaranteeRuleId,
                ProcurementCategory = guaranteeRule.ProcurementCategory,
                GuaranteeType = guaranteeRule.GuaranteeType,
                GuaranteeMode = guaranteeRule.GuaranteeMode,
                DateCreated = guaranteeRule.DateCreated,
                LastModified = guaranteeRule.LastModified,
                CreatedBy = guaranteeRule.CreatedBy,
                ModifiedBy = guaranteeRule.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRules_Create([DataSourceRequest]DataSourceRequest request, GuaranteeRule guaranteeRule)
        {
            #region validation

            bool valid = true;
            var exists = db.GuaranteeRules
                .Where(x => x.ProcurementCategory == guaranteeRule.ProcurementCategory)
                 .Where(x => x.GuaranteeType == guaranteeRule.GuaranteeType)
                  .Where(x => x.GuaranteeMode == guaranteeRule.GuaranteeMode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("exists", "This combination is already exists in the system.");
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                
                if (valid)
                {
                    var entity = new GuaranteeRule
                    {
                        ProcurementCategory = guaranteeRule.ProcurementCategory,
                        GuaranteeType = guaranteeRule.GuaranteeType,
                        GuaranteeMode = guaranteeRule.GuaranteeMode,
                        DateCreated = guaranteeRule.DateCreated,
                        LastModified = guaranteeRule.LastModified,
                        CreatedBy = guaranteeRule.CreatedBy,
                        ModifiedBy = guaranteeRule.ModifiedBy
                    };

                    db.GuaranteeRules.Add(entity);
                    db.SaveChanges();
                    guaranteeRule.GuaranteeRuleId = entity.GuaranteeRuleId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Guarantee rule setup", "Success", User.Identity.Name, $"created guarantee rule {entity.GuaranteeRuleId}", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { guaranteeRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRules_Update([DataSourceRequest]DataSourceRequest request, GuaranteeRule guaranteeRule)
        {
            #region validation

            bool valid = true;
            var exists = db.GuaranteeRules.Where(x => x.GuaranteeRuleId != guaranteeRule.GuaranteeRuleId)
                .Where(x => x.ProcurementCategory == guaranteeRule.ProcurementCategory)
                 .Where(x => x.GuaranteeType == guaranteeRule.GuaranteeType)
                  .Where(x => x.GuaranteeMode == guaranteeRule.GuaranteeMode).ToList();

            if (exists.Any())
            {
                ModelState.AddModelError("exists", "This combination is already exists in the system.");
                valid = false;
            }

            #endregion
            if (ModelState.IsValid)
            {
                
                if (valid)
                {
                    var entity = new GuaranteeRule
                    {
                        GuaranteeRuleId = guaranteeRule.GuaranteeRuleId,
                        ProcurementCategory = guaranteeRule.ProcurementCategory,
                        GuaranteeType = guaranteeRule.GuaranteeType,
                        GuaranteeMode = guaranteeRule.GuaranteeMode,
                        DateCreated = guaranteeRule.DateCreated,
                        LastModified = guaranteeRule.LastModified,
                        CreatedBy = guaranteeRule.CreatedBy,
                        ModifiedBy = guaranteeRule.ModifiedBy
                    };

                    db.GuaranteeRules.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Guarantee rule setup", "Success", User.Identity.Name, $"updated guarantee rule {entity.GuaranteeRuleId}", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { guaranteeRule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeRules_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeRule guaranteeRule)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeRule
                {
                    GuaranteeRuleId = guaranteeRule.GuaranteeRuleId,
                    ProcurementCategory = guaranteeRule.ProcurementCategory,
                    GuaranteeType = guaranteeRule.GuaranteeType,
                    GuaranteeMode = guaranteeRule.GuaranteeMode,
                    DateCreated = guaranteeRule.DateCreated,
                    LastModified = guaranteeRule.LastModified,
                    CreatedBy = guaranteeRule.CreatedBy,
                    ModifiedBy = guaranteeRule.ModifiedBy
                };

                db.GuaranteeRules.Attach(entity);
                db.GuaranteeRules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeRule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
