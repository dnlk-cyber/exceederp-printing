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
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class IdentificationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Identifications_Read([DataSourceRequest]DataSourceRequest request, string identificationReferenceType, int id)
        {
            IQueryable<Identification> identifications = db.Identifications.Where(i => i.Reference == id.ToString() && i.ReferenceType == identificationReferenceType);
            DataSourceResult result = identifications.ToDataSourceResult(request, identification => new
            {
                IdentificationId = identification.IdentificationId,
                Reference = id.ToString(),
                ReferenceType = identification.ReferenceType,
                Description = identification.Description,
                IdNumber = identification.IdNumber,
                Type = identification.Type,
                IssueDate = identification.IssueDate,
                ExpiryDate = identification.ExpiryDate,
                Issuer = identification.Issuer,
                CreatedBy = identification.CreatedBy,
                DateCreated = identification.DateCreated,
                LastModified = identification.LastModified,
                ModifiedBy = identification.ModifiedBy,
                OrgBranchName = identification.OrgBranchName,
                Remark = identification.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Create([DataSourceRequest]DataSourceRequest request, Identification identification, string identificationReferenceType, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new Identification
                {
                    Reference = id.ToString(),
                    ReferenceType = identificationReferenceType,
                    Description = identification.Description,
                    IdNumber = identification.IdNumber,
                    Type = identification.Type,
                    IssueDate = identification.IssueDate,
                    ExpiryDate = identification.ExpiryDate,
                    Issuer = identification.Issuer,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = identification.Remark
                };

                db.Identifications.Add(entity);
                db.SaveChanges();
                identification.IdentificationId = entity.IdentificationId;
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Update([DataSourceRequest]DataSourceRequest request, Identification identification)
        {
            if (ModelState.IsValid)
            {
                var entity = new Identification
                {
                    ReferenceType = identification.ReferenceType,
                    IdentificationId = identification.IdentificationId,
                    Reference = identification.Reference,
                    Description = identification.Description,
                    IdNumber = identification.IdNumber,
                    Type = identification.Type,
                    IssueDate = identification.IssueDate,
                    ExpiryDate = identification.ExpiryDate,
                    Issuer = identification.Issuer,
                    Remark = identification.Remark
                };

                db.Identifications.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Identifications_Destroy([DataSourceRequest]DataSourceRequest request, Identification identification)
        {
            if (ModelState.IsValid)
            {
                var entity = new Identification
                {
                    IdentificationId = identification.IdentificationId,
                    Reference = identification.Reference,
                    Description = identification.Description,
                    IdNumber = identification.IdNumber,
                    Type = identification.Type,
                    IssueDate = identification.IssueDate,
                    ExpiryDate = identification.ExpiryDate,
                    Issuer = identification.Issuer,
                    Remark = identification.Remark
                };

                db.Identifications.Attach(entity);
                db.Identifications.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Identification number " + identification.IdentificationId);
		
            }

            return Json(new[] { identification }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
