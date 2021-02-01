using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class RepresentativeElementWeightRangeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult WeightRanges_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<RepresentativeElementWeightRange> elements = db.RepresentativeElementWeightRange.Where(e => e.ProjectRepresentativeElementId == id);
            DataSourceResult result = elements.ToDataSourceResult(request, element => new
            {
                RepresentativeElementWeightRangeId = element.RepresentativeElementWeightRangeId,
                ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                ConfigurationId = element.ConfigurationId,
                PercentageMin = element.PercentageMin,
                PercentageMax = element.PercentageMax,
                
                OrgBranchName = element.OrgBranchName,
                DateCreated = element.DateCreated,
                LastModified = element.LastModified,
                CreatedBy = element.CreatedBy,
                ModifiedBy = element.ModifiedBy
            });

            return Json(result);
        }
        private bool RangeExistsForSeries(int elementId, RepresentativeElementWeightRange record)
        {
            var valueExists = false;
            var weightRange = db.RepresentativeElementWeightRange.FirstOrDefault(rep => rep.ProjectRepresentativeElementId == elementId && rep.ConfigurationId == record.ConfigurationId);
            if (weightRange != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightRange_Create([DataSourceRequest]DataSourceRequest request, RepresentativeElementWeightRange element, int id)
        {
            if (ModelState.IsValid)
            {
                if (RangeExistsForSeries(id, element))
                {
                    ModelState.AddModelError("", "Range exists for this Series");
                }
                else
                {
                    var entity = new RepresentativeElementWeightRange
                    {
                        //RepresentativeElementWeightRangeId = element.RepresentativeElementWeightRangeId,
                        ProjectRepresentativeElementId = id,
                        ConfigurationId = element.ConfigurationId,
                        PercentageMin = element.PercentageMin,
                        PercentageMax = element.PercentageMax,

                        OrgBranchName = element.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = element.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = element.ModifiedBy
                    };

                    db.RepresentativeElementWeightRange.Add(entity);
                    db.SaveChanges();

                    element.RepresentativeElementWeightRangeId = entity.RepresentativeElementWeightRangeId;
                    
                }

            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightRange_Update([DataSourceRequest]DataSourceRequest request, RepresentativeElementWeightRange element)
        {
            if (ModelState.IsValid)
            {
                var entity = new RepresentativeElementWeightRange
                {
                    RepresentativeElementWeightRangeId = element.RepresentativeElementWeightRangeId,
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    ConfigurationId = element.ConfigurationId,
                    PercentageMin = element.PercentageMin,
                    PercentageMax = element.PercentageMax,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.RepresentativeElementWeightRange.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WeightRange_Destroy([DataSourceRequest]DataSourceRequest request, RepresentativeElementWeightRange element)
        {
            if (ModelState.IsValid)
            {
                var entity = new RepresentativeElementWeightRange
                {
                    RepresentativeElementWeightRangeId = element.RepresentativeElementWeightRangeId,
                    ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                    ConfigurationId = element.ConfigurationId,
                    PercentageMin = element.PercentageMin,
                    PercentageMax = element.PercentageMax,

                    OrgBranchName = element.OrgBranchName,
                    DateCreated = element.DateCreated,
                    LastModified = element.LastModified,
                    CreatedBy = element.CreatedBy,
                    ModifiedBy = element.ModifiedBy
                };

                db.RepresentativeElementWeightRange.Attach(entity);
                db.RepresentativeElementWeightRange.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { element }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}