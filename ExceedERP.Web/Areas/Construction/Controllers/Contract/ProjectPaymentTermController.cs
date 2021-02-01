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

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class ProjectPaymentTermController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult PaymentAttribute_Read([DataSourceRequest]DataSourceRequest request, int contractIdReadTerm)
        {
            IQueryable<ProjectPaymentTerm> attributes = db.ProjectPaymentTerms.Where(t => t.ProjectContractId == contractIdReadTerm);
            DataSourceResult result = attributes.ToDataSourceResult(request, attribute => new {
                ProjectPaymentTermId = attribute.ProjectPaymentTermId,
                ProjectContractId = attribute.ProjectContractId,
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

        private bool CheckExistance(ProjectPaymentTerm model, int contractId)
        {
            bool attributeExists = false;
            var attribute = db.ProjectPaymentTerms.FirstOrDefault(at => at.AttributeName == model.AttributeName && at.ProjectContractId == contractId);
            if (attribute != null)
            {
                attributeExists = true;
            }
            return attributeExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Create([DataSourceRequest]DataSourceRequest request, ProjectPaymentTerm attribute, int contractIdCreateTerm)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(attribute, contractIdCreateTerm))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                }
                else
                {
                    var entity = new ProjectPaymentTerm
                    {
                        ProjectContractId = contractIdCreateTerm,
                        AttributeName = attribute.AttributeName,
                        Value = attribute.Value,

                        OrgBranchName = attribute.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = attribute.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = attribute.ModifiedBy
                    };

                    db.ProjectPaymentTerms.Add(entity);
                    db.SaveChanges();
                    attribute.ProjectPaymentTermId = entity.ProjectPaymentTermId;

                }
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Update([DataSourceRequest]DataSourceRequest request, ProjectPaymentTerm attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentTerm
                {
                    ProjectPaymentTermId = attribute.ProjectPaymentTermId,
                    ProjectContractId = attribute.ProjectContractId,
                    AttributeName = attribute.AttributeName,
                    Value = attribute.Value,

                    OrgBranchName = attribute.OrgBranchName,
                    DateCreated = attribute.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = attribute.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectPaymentTerms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attribute }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentAttribute_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPaymentTerm attribute)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPaymentTerm
                {
                    ProjectPaymentTermId = attribute.ProjectPaymentTermId,
                    ProjectContractId = attribute.ProjectContractId,
                    AttributeName = attribute.AttributeName,
                    Value = attribute.Value,

                    OrgBranchName = attribute.OrgBranchName,
                    DateCreated = attribute.DateCreated,
                    LastModified = attribute.LastModified,
                    CreatedBy = attribute.CreatedBy,
                    ModifiedBy = attribute.ModifiedBy
                };

                db.ProjectPaymentTerms.Attach(entity);
                db.ProjectPaymentTerms.Remove(entity);
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