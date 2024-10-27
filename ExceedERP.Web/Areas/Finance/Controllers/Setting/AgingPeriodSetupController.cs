using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class AgingPeriodSetupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AgingPeriodSetups_Read([DataSourceRequest]DataSourceRequest request)
        {

            foreach (AgingType type in Enum.GetValues(typeof(AgingType)))
            {
                var type1 = type;
                var agingPeriodSetups = db.AgingPeriodSetups.Where(x => x.Type== type1);
                if (!agingPeriodSetups.Any())
                {
                    var entity = new AgingPeriodSetup
                    {
                        Type = type1,
                        Period1 = 0,
                        Period2 = 0,
                        Period3=0
                    };

                    db.AgingPeriodSetups.Add(entity);
                    db.SaveChanges();

                }
            }

            IQueryable<AgingPeriodSetup> agingperiodsetups = db.AgingPeriodSetups;
            DataSourceResult result = agingperiodsetups.ToDataSourceResult(request, agingPeriodSetup => new {
                AgingPeriodSetupId = agingPeriodSetup.AgingPeriodSetupId,
                Type = agingPeriodSetup.Type,
                Period1 = agingPeriodSetup.Period1,
                Period2 = agingPeriodSetup.Period2,
                Period3 = agingPeriodSetup.Period3,
                DateCreated = agingPeriodSetup.DateCreated,
                LastModified = agingPeriodSetup.LastModified,
                CreatedBy = agingPeriodSetup.CreatedBy,
                ModifiedBy = agingPeriodSetup.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AgingPeriodSetups_Create([DataSourceRequest]DataSourceRequest request, AgingPeriodSetup agingPeriodSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AgingPeriodSetup
                {
                    Type = agingPeriodSetup.Type,
                    Period1 = agingPeriodSetup.Period1,
                    Period2 = agingPeriodSetup.Period2,
                    Period3 = agingPeriodSetup.Period3,
                    DateCreated = agingPeriodSetup.DateCreated,
                    LastModified = agingPeriodSetup.LastModified,
                    CreatedBy = agingPeriodSetup.CreatedBy,
                    ModifiedBy = agingPeriodSetup.ModifiedBy
                };

                db.AgingPeriodSetups.Add(entity);
                db.SaveChanges();
                agingPeriodSetup.AgingPeriodSetupId = entity.AgingPeriodSetupId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Aging period setup", "Success", User.Identity.Name, $"created aging period {entity.Type}", Modules.Finance);
                #endregion
            }

            return Json(new[] { agingPeriodSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AgingPeriodSetups_Update([DataSourceRequest]DataSourceRequest request, AgingPeriodSetup agingPeriodSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AgingPeriodSetup
                {
                    AgingPeriodSetupId = agingPeriodSetup.AgingPeriodSetupId,
                    Type = agingPeriodSetup.Type,
                    Period1 = agingPeriodSetup.Period1,
                    Period2 = agingPeriodSetup.Period2,
                    Period3 = agingPeriodSetup.Period3,
                    DateCreated = agingPeriodSetup.DateCreated,
                    LastModified = agingPeriodSetup.LastModified,
                    CreatedBy = agingPeriodSetup.CreatedBy,
                    ModifiedBy = agingPeriodSetup.ModifiedBy
                };

                db.AgingPeriodSetups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Aging period setup", "Success", User.Identity.Name, $"updated aging period {entity.Type}", Modules.Finance);
                #endregion
            }

            return Json(new[] { agingPeriodSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AgingPeriodSetups_Destroy([DataSourceRequest]DataSourceRequest request, AgingPeriodSetup agingPeriodSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new AgingPeriodSetup
                {
                    AgingPeriodSetupId = agingPeriodSetup.AgingPeriodSetupId,
                    Type = agingPeriodSetup.Type,
                    Period1 = agingPeriodSetup.Period1,
                    Period2 = agingPeriodSetup.Period2,
                    Period3 = agingPeriodSetup.Period3,
                    DateCreated = agingPeriodSetup.DateCreated,
                    LastModified = agingPeriodSetup.LastModified,
                    CreatedBy = agingPeriodSetup.CreatedBy,
                    ModifiedBy = agingPeriodSetup.ModifiedBy
                };

                db.AgingPeriodSetups.Attach(entity);
                db.AgingPeriodSetups.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { agingPeriodSetup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
