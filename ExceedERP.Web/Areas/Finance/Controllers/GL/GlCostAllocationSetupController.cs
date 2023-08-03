using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlCostAllocationSetupController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var glSegmentSetups = _db.GlSegmentSetups;
            ViewData["SetupSegments"] = glSegmentSetups;
        }

        public ActionResult GlCostAllocationSetups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlCostAllocationSetup> GlCostAllocationSetups = _db.GlCostAllocationSetups;
            DataSourceResult result = GlCostAllocationSetups.ToDataSourceResult(request, GlCostAllocationSetup => new {
                GlCostAllocationSetupId = GlCostAllocationSetup.GlCostAllocationSetupId,
                GLSegmentSetupId = GlCostAllocationSetup.GLSegmentSetupId,
                SegmentCode = GlCostAllocationSetup.SegmentCode,
                CostCenterCode = GlCostAllocationSetup.CostCenterCode,
                OrgBranchName = GlCostAllocationSetup.OrgBranchName,
                DateCreated = GlCostAllocationSetup.DateCreated,
                LastModified = GlCostAllocationSetup.LastModified,
                CreatedBy = GlCostAllocationSetup.CreatedBy,
                ModifiedBy = GlCostAllocationSetup.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlCostAllocationSetups_Create([DataSourceRequest]DataSourceRequest request, GlCostAllocationSetup GlCostAllocationSetup)
        {
            #region validation
           
            var exist = _db.GlCostAllocationSetups              
                .Where(x => x.CostCenterCode == GlCostAllocationSetup.CostCenterCode).ToList();

            if (exist.Any())
                ModelState.AddModelError("exist", $"The account value {GlCostAllocationSetup.CostCenterCode} already exists.");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GlCostAllocationSetup
                {
                    GLSegmentSetupId = GlCostAllocationSetup.GLSegmentSetupId,
                    SegmentCode = GlCostAllocationSetup.SegmentCode,
                    CostCenterCode = GlCostAllocationSetup.CostCenterCode,
                    OrgBranchName = GlCostAllocationSetup.OrgBranchName,
                    DateCreated = GlCostAllocationSetup.DateCreated,
                    LastModified = GlCostAllocationSetup.LastModified,
                    CreatedBy = GlCostAllocationSetup.CreatedBy,
                    ModifiedBy = GlCostAllocationSetup.ModifiedBy
                };

                _db.GlCostAllocationSetups.Add(entity);
                _db.SaveChanges();
                GlCostAllocationSetup.GlCostAllocationSetupId = entity.GlCostAllocationSetupId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Gl Cost allocation segment ", "Success", User.Identity.Name, $"created segment {entity.SegmentCode} and cost center {entity.CostCenterCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { GlCostAllocationSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlCostAllocationSetups_Update([DataSourceRequest]DataSourceRequest request, GlCostAllocationSetup glCostAllocationSetup)
        {
            #region validation
     
            var exist = _db.GlCostAllocationSetups
                .Where(x => x.GlCostAllocationSetupId != glCostAllocationSetup.GlCostAllocationSetupId)            
                .Where(x => x.CostCenterCode == glCostAllocationSetup.CostCenterCode).ToList();

            if(exist.Any())
                ModelState.AddModelError("exist", $"The account value {glCostAllocationSetup.CostCenterCode} already exists.");
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GlCostAllocationSetup
                {
                    GLSegmentSetupId = glCostAllocationSetup.GLSegmentSetupId,
                    GlCostAllocationSetupId = glCostAllocationSetup.GlCostAllocationSetupId,                
                    SegmentCode = glCostAllocationSetup.SegmentCode,
                    CostCenterCode = glCostAllocationSetup.CostCenterCode,
                    OrgBranchName = glCostAllocationSetup.OrgBranchName,
                    DateCreated = glCostAllocationSetup.DateCreated,
                    LastModified = glCostAllocationSetup.LastModified,
                    CreatedBy = glCostAllocationSetup.CreatedBy,
                    ModifiedBy = glCostAllocationSetup.ModifiedBy
                };

                _db.GlCostAllocationSetups.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Gl Cost allocation segment ", "Success", User.Identity.Name, $"updated segment {entity.SegmentCode} and cost center {entity.CostCenterCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { glCostAllocationSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlCostAllocationSetups_Destroy([DataSourceRequest]DataSourceRequest request, GlCostAllocationSetup GlCostAllocationSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new GlCostAllocationSetup
                {
                    GLSegmentSetupId = GlCostAllocationSetup.GLSegmentSetupId,
                    GlCostAllocationSetupId = GlCostAllocationSetup.GlCostAllocationSetupId,
                    SegmentCode = GlCostAllocationSetup.SegmentCode,
                    CostCenterCode = GlCostAllocationSetup.CostCenterCode,
                    OrgBranchName = GlCostAllocationSetup.OrgBranchName,
                    DateCreated = GlCostAllocationSetup.DateCreated,
                    LastModified = GlCostAllocationSetup.LastModified,
                    CreatedBy = GlCostAllocationSetup.CreatedBy,
                    ModifiedBy = GlCostAllocationSetup.ModifiedBy
                };

                _db.GlCostAllocationSetups.Attach(entity);
                _db.GlCostAllocationSetups.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { GlCostAllocationSetup }.ToDataSourceResult(request, ModelState));
        }

      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
