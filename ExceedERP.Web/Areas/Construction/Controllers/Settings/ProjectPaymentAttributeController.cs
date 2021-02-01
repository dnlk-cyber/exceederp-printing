using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ProjectPaymentAttributeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult PaymentAttribute_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectPaymentAttribute> attributes = db.ProjectPaymentAttributes;
            DataSourceResult result = attributes.ToDataSourceResult(request, attribute => new {
                ProjectPaymentAttributeId = attribute.ProjectPaymentAttributeId,
                AttributeName = attribute.AttributeName,
                Value = attribute.Value,

                OrgBranchName = attribute.OrgBranchName,
                DateCreated = attribute.DateCreated,
                LastModified = attribute.LastModified,
                CreatedBy = attribute.CreatedBy,
                ModifiedBy = attribute.ModifiedBy
            });

            return Json(result);
        }

        private bool CheckExistance(ProjectPaymentAttribute model)
        {
            bool attributeExists = false;
            var attribute = db.ProjectPaymentAttributes.Where(at => at.AttributeName == model.AttributeName).FirstOrDefault();
            if (attribute != null)
            {
                attributeExists = true;
            }
            return attributeExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymentAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(attribute))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                }
                else
                {
                    var entity = new ProjectPaymentAttribute
                    {
                        ProjectPaymentAttributeId = attribute.ProjectPaymentAttributeId,
                        AttributeName = attribute.AttributeName,
                        Value = attribute.Value,

                        OrgBranchName = attribute.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = attribute.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = attribute.ModifiedBy
                    };

                    db.ProjectPaymentAttributes.Add(entity);
                    db.SaveChanges();
                    attribute.ProjectPaymentAttributeId = entity.ProjectPaymentAttributeId;

                }
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymentAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentAttribute
                {
                    ProjectPaymentAttributeId = attribute.ProjectPaymentAttributeId,
                    AttributeName = attribute.AttributeName,
                    Value = attribute.Value,

                    OrgBranchName = attribute.OrgBranchName,
                    DateCreated = attribute.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = attribute.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectPaymentAttributes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymentAttribute attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentAttribute
                {
                    ProjectPaymentAttributeId = attribute.ProjectPaymentAttributeId,
                    AttributeName = attribute.AttributeName,
                    Value = attribute.Value,

                    OrgBranchName = attribute.OrgBranchName,
                    DateCreated = attribute.DateCreated,
                    LastModified = attribute.LastModified,
                    CreatedBy = attribute.CreatedBy,
                    ModifiedBy = attribute.ModifiedBy
                };

                db.ProjectPaymentAttributes.Attach(entity);
                db.ProjectPaymentAttributes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}