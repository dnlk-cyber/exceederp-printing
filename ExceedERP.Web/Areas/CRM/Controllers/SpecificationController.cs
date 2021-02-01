using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class SpecificationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Specifications_Read([DataSourceRequest]DataSourceRequest request, int specificationThingID, string specificationThingType)
        {
            IQueryable<Specification> specifications = db.Specifications.Where(s => s.ThingID == specificationThingID.ToString() && s.ThingType == specificationThingType);
            DataSourceResult result = specifications.ToDataSourceResult(request, specification => new
            {
                SpecificationID = specification.SpecificationID,
                ThingID = specification.ThingID,
                IsKey = specification.IsKey,
                Category = specification.Category,
                Index = specification.Index,
                Attribute = specification.Attribute,
                Value = specification.Value,
                CreatedBy = specification.CreatedBy,
                DateCreated = specification.DateCreated,
                LastModified = specification.LastModified,
                ModifiedBy = specification.ModifiedBy,
                OrgBranchName = specification.OrgBranchName,
                Remark = specification.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Specifications_Create([DataSourceRequest]DataSourceRequest request, Specification specification, int specificationThingID, string specificationThingType)
        {
            List<Specification> specificationList = db.Specifications.Where(s => s.ThingID == specificationThingID.ToString() && s.Index == specification.Index ).ToList();

            if (specificationList.Count>0)
            {
                ModelState.AddModelError("IndexError", "Duplicate Index entry");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new Specification
                    {
                        ThingID = specificationThingID.ToString(),
                        ThingType = specificationThingType,
                        IsKey = specification.IsKey,
                        Category = specification.Category,
                        Index = specification.Index,
                        Attribute = specification.Attribute,
                        Value = specification.Value,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = specification.Remark
                    };

                    db.Specifications.Add(entity);
                    db.SaveChanges();
                    specification.SpecificationID = entity.SpecificationID;
                }
            }

            return Json(new[] { specification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Specifications_Update([DataSourceRequest]DataSourceRequest request, Specification specification)
        {
            if (ModelState.IsValid)
            {
                var entity = new Specification
                {
                    SpecificationID = specification.SpecificationID,
                    ThingID = specification.ThingID,
                    ThingType = specification.ThingType,
                    IsKey = specification.IsKey,
                    Category = specification.Category,
                    Index = specification.Index,
                    Attribute = specification.Attribute,
                    Value = specification.Value,
                    Remark = specification.Remark
                };

                db.Specifications.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { specification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Specifications_Destroy([DataSourceRequest]DataSourceRequest request, Specification specification)
        {
            if (ModelState.IsValid)
            {
                var entity = new Specification
                {
                    SpecificationID = specification.SpecificationID,
                    ThingID = specification.ThingID,
                    ThingType = specification.ThingType,
                    IsKey = specification.IsKey,
                    Category = specification.Category,
                    Index = specification.Index,
                    Attribute = specification.Attribute,
                    Value = specification.Value,
                    Remark = specification.Remark
                };

                db.Specifications.Attach(entity);
                db.Specifications.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Specification number " + specification.SpecificationID);
     
            }

            return Json(new[] { specification }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
