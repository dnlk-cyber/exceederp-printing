using System;
using System.Data.Entity;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    // [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
    public class GLBranchAccountMapController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string AmharicName { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            ViewData["types"] = types;

            var branches = db.Branch;
            ViewData["branches"] = branches;
        }
        public void GetAmharicName(string car)
        {
            AmharicName = car;
        }

        public ActionResult Branch_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Branch> branchs = db.Branch;
            DataSourceResult result = branchs.ToDataSourceResult(request, branch => new
            {
                BranchId = branch.BranchId,
                ParentBranchId = branch.ParentBranchId,
                Name = branch.Name,
                Remark = branch.Remark,
                DateCreated = branch.DateCreated,
                LastModified = branch.LastModified,
                CreatedBy = branch.CreatedBy,
                ModifiedBy = branch.ModifiedBy,
                AmharicName = branch.AmharicName,
                Code = branch.Code,
                LocationAccount = branch.LocationAccount,
                BranchTypes = branch.BranchTypes,
                Abbreviation = branch.Abbreviation,
                IsActive = branch.IsActive,
                // LookupId = branch.LookupId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branch_Create([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            var branches = db.Branch.ToList();
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string jobName = branch.Name;
                var checkExistance = branches.FirstOrDefault(x => x.Code == branch.Code || x.Name.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));

                if (checkExistance != null)
                {
                    ModelState.AddModelError("", "The branch name or code must be unique!!");
                }
                else
                {
                    var entity = new Branch
                    {
                        Name = branch.Name,
                        ParentBranchId = branch.ParentBranchId,
                        Remark = branch.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = branch.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = branch.ModifiedBy,
                        AmharicName = branch.AmharicName,
                        Code = branch.Code,
                        LocationAccount = branch.LocationAccount,
                        BranchTypes = branch.BranchTypes,
                        Abbreviation = branch.Abbreviation,
                        IsActive = branch.IsActive,
                        // LookupId = branch.LookupId
                    };

                    db.Branch.Add(entity);
                    db.SaveChanges();
                    branch.BranchId = entity.BranchId;

                }
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branch_Update([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
           
            if (ModelState.IsValid)
            {
                if (AmharicName == null) AmharicName = branch.AmharicName;
                var entity = new Branch
                {
                    BranchId = branch.BranchId,
                    ParentBranchId = branch.ParentBranchId,
                    Name = branch.Name,
                    Remark = branch.Remark,
                    DateCreated = branch.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = branch.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    AmharicName = branch.AmharicName,
                    Code = branch.Code,
                    LocationAccount = branch.LocationAccount,
                    BranchTypes = branch.BranchTypes,
                    Abbreviation = branch.Abbreviation,
                    IsActive = branch.IsActive,
                    //LookupId = branch.LookupId

                };

                db.Branch.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }

        public JsonResult GetBranchAccounts()
        {

            var locationSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.Location);
            var locations = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == locationSeg.GLSegmentSetupId);
            return Json(locations.Select(p => new
            {
                Code = p.Values,
                Name = p.Values+"-"+p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
