using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ObjectAccountRequirementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ObjectAccountRequirements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ObjectAccountRequirement> objectaccountrequirements = db.ObjectAccountRequirements;
            DataSourceResult result = objectaccountrequirements.ToDataSourceResult(request, objectAccountRequirement => new {
                ObjectAccountRequirementID = objectAccountRequirement.ObjectAccountRequirementID,
                ControlAccount = objectAccountRequirement.ControlAccount,
                CreatedBy = objectAccountRequirement.CreatedBy,
                DateCreated = objectAccountRequirement.DateCreated,
                LastModified = objectAccountRequirement.LastModified,
                ModifiedBy = objectAccountRequirement.ModifiedBy,
                OrgBranchName = objectAccountRequirement.OrgBranchName,
                ObjectType = objectAccountRequirement.ObjectType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectAccountRequirements_Create([DataSourceRequest]DataSourceRequest request, ObjectAccountRequirement objectAccountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectAccountRequirement
                {
                    ControlAccount = objectAccountRequirement.ControlAccount,
                    ObjectType = objectAccountRequirement.ObjectType,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };

                db.ObjectAccountRequirements.Add(entity);
                db.SaveChanges();
                objectAccountRequirement.ObjectAccountRequirementID = entity.ObjectAccountRequirementID;
            }

            return Json(new[] { objectAccountRequirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectAccountRequirements_Update([DataSourceRequest]DataSourceRequest request, ObjectAccountRequirement objectAccountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectAccountRequirement
                {
                    ObjectAccountRequirementID = objectAccountRequirement.ObjectAccountRequirementID,
                    ControlAccount = objectAccountRequirement.ControlAccount,
                    ObjectType = objectAccountRequirement.ObjectType
                };

                db.ObjectAccountRequirements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { objectAccountRequirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectAccountRequirements_Destroy([DataSourceRequest]DataSourceRequest request, ObjectAccountRequirement objectAccountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ObjectAccountRequirement
                {
                    ObjectAccountRequirementID = objectAccountRequirement.ObjectAccountRequirementID,
                    ControlAccount = objectAccountRequirement.ControlAccount,
                    ObjectType = objectAccountRequirement.ObjectType
                };

                db.ObjectAccountRequirements.Attach(entity);
                db.ObjectAccountRequirements.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Object Account Requirement  number " + objectAccountRequirement.ObjectAccountRequirementID);
      
            }

            return Json(new[] { objectAccountRequirement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
