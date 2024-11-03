using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{

    [AuthorizeRoles(HrmConstants.HRAdmin)]
    public class RegionNationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();
            ViewData["NationList"] = lookups;
        }

        public ActionResult RegionNation_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RegionNation> regionnation = db.RegionNation;
            DataSourceResult result = regionnation.ToDataSourceResult(request, regionNation => new
            {
                RegionNationId = regionNation.RegionNationId,
                Region = regionNation.Region,
                LookupId = regionNation.LookupId,
                Remark = regionNation.Remark,
                OrgBranchName = regionNation.OrgBranchName,
                DateCreated = regionNation.DateCreated,
                LastModified = regionNation.LastModified,
                CreatedBy = regionNation.CreatedBy,
                ModifiedBy = regionNation.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegionNation_Create([DataSourceRequest]DataSourceRequest request, RegionNation regionNation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkExstance = db.RegionNation.Where(x => x.LookupId == regionNation.LookupId && x.Region == regionNation.Region).FirstOrDefault();
                if (checkExstance != null)
                {
                    ModelState.AddModelError("", "This Mapping is aleardy exist!!");
                }
                else
                {



                    var entity = new RegionNation
                    {
                        LookupId = regionNation.LookupId,
                        Region = regionNation.Region,
                        Remark = regionNation.Remark,
                        OrgBranchName = regionNation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = regionNation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = regionNation.ModifiedBy
                    };

                    db.RegionNation.Add(entity);
                    db.SaveChanges();
                    regionNation.RegionNationId = entity.RegionNationId;
                }
            }

            return Json(new[] { regionNation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegionNation_Update([DataSourceRequest]DataSourceRequest request, RegionNation regionNation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RegionNation
                {
                    LookupId = regionNation.LookupId,
                    RegionNationId = regionNation.RegionNationId,
                    Region = regionNation.Region,
                    Remark = regionNation.Remark,
                    OrgBranchName = regionNation.OrgBranchName,
                    DateCreated = regionNation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = regionNation.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.RegionNation.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { regionNation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RegionNation_Destroy([DataSourceRequest]DataSourceRequest request, RegionNation regionNation)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new RegionNation
                {
                    LookupId = regionNation.LookupId,
                    RegionNationId = regionNation.RegionNationId,
                    Region = regionNation.Region,
                    Remark = regionNation.Remark,
                    OrgBranchName = regionNation.OrgBranchName,
                    DateCreated = regionNation.DateCreated,
                    LastModified = regionNation.LastModified,
                    CreatedBy = regionNation.CreatedBy,
                    ModifiedBy = regionNation.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted nation list by regioon information of region " + regionNation.Region.ToString() + " nation " + regionNation.Lookup.Description);
                #endregion
                db.RegionNation.Attach(entity);
                db.RegionNation.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { regionNation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
