using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Core.Domain.InfoDesk.License;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.License
{
    [Authorize]
    public class SoftwareLicensesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private ApplicationDbContext _user = new ApplicationDbContext();


        public ActionResult Index()
        {
            ViewData["department"] = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, x.Name }).ToList();
            ViewData["users"] = _user.Users.Select(x => new { UserId=x.Id, Name=x.UserName }).ToList();
            ViewData["employees"]   =  db.Person_Employee.Where(x => x.IsActive)
                        .Select(x=>new { x.EmployeeId,
                                          FullNameaEnglish =x.EmployeeCode+"-"+x.FirstNameEnglish+" "+x.MiddleNameEnglish+" "+x.LastNameEnglish}).ToList();
            return View();
        }
  
        public ActionResult SoftwareLicenses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SoftwareLicense> softwareLicenses = db.SoftwareLicenses;
            DataSourceResult result = softwareLicenses.ToDataSourceResult(request, softwareLicense => new {
                SoftwareLicenseId = softwareLicense.SoftwareLicenseId,
                LicenseId = softwareLicense.LicenseId,
                SoftwareName = softwareLicense.SoftwareName,
                Version = softwareLicense.Version,
                Vendor = softwareLicense.Vendor,
                LicenseType = softwareLicense.LicenseType,
                LicenseKey = softwareLicense.LicenseKey,
                PurchaseDate = softwareLicense.PurchaseDate,
                LicenseExpiryOrRenewalDate = softwareLicense.LicenseExpiryOrRenewalDate,
                NumberofLicenses = softwareLicense.NumberofLicenses,
                NumberofUsers = softwareLicense.NumberofUsers,
                CostperLicense = softwareLicense.CostperLicense,
                TotalCost = softwareLicense.TotalCost,
                OrgStructureId = softwareLicense.OrgStructureId,
                AssignedUsers = softwareLicense.AssignedUsers,
                LicenseStatus = softwareLicense.LicenseStatus,
                UsageLimitations = softwareLicense.UsageLimitations,
                RenewalNotificationDate = softwareLicense.RenewalNotificationDate,
                IsRenewalNotified = softwareLicense.IsRenewalNotified,
                SupportContact = softwareLicense.SupportContact,
                CommentsOrNotes = softwareLicense.CommentsOrNotes,
                DateCreated = softwareLicense.DateCreated,
                LastModified = softwareLicense.LastModified,
                CreatedBy = softwareLicense.CreatedBy,
                ModifiedBy = softwareLicense.ModifiedBy
            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SoftwareLicenses_Create([DataSourceRequest]DataSourceRequest request, SoftwareLicense softwareLicense)
        {
            softwareLicense.TotalCost = softwareLicense.NumberofLicenses * softwareLicense.CostperLicense;
            if (ModelState.IsValid)
            {
                softwareLicense.DateCreated = DateTime.Now;
                softwareLicense.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(softwareLicense);
                db.SoftwareLicenses.Add(entity);
                db.SaveChanges();
                softwareLicense.SoftwareLicenseId = entity.SoftwareLicenseId;
            }

            return Json(new[] { softwareLicense }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SoftwareLicenses_Update([DataSourceRequest]DataSourceRequest request, SoftwareLicense softwareLicense)
        {
            softwareLicense.TotalCost = softwareLicense.NumberofLicenses * softwareLicense.CostperLicense;
            if (ModelState.IsValid)
            {
                softwareLicense.LastModified = DateTime.Now;
                softwareLicense.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(softwareLicense);
                db.SoftwareLicenses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { softwareLicense }.ToDataSourceResult(request, ModelState));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SoftwareLicenses_Destroy([DataSourceRequest]DataSourceRequest request, SoftwareLicense softwareLicense)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(softwareLicense);
                db.SoftwareLicenses.Attach(entity);
                db.SoftwareLicenses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { softwareLicense }.ToDataSourceResult(request, ModelState));
        }

        private SoftwareLicense GetMappedObject(SoftwareLicense softwareLicense)
        {
            var entity = new SoftwareLicense
            {
                SoftwareLicenseId = softwareLicense.SoftwareLicenseId,
                LicenseId = softwareLicense.LicenseId,
                SoftwareName = softwareLicense.SoftwareName,
                Version = softwareLicense.Version,
                Vendor = softwareLicense.Vendor,
                LicenseType = softwareLicense.LicenseType,
                LicenseKey = softwareLicense.LicenseKey,
                PurchaseDate = softwareLicense.PurchaseDate,
                LicenseExpiryOrRenewalDate = softwareLicense.LicenseExpiryOrRenewalDate,
                NumberofLicenses = softwareLicense.NumberofLicenses,
                NumberofUsers = softwareLicense.NumberofUsers,
                CostperLicense = softwareLicense.CostperLicense,
                TotalCost = softwareLicense.TotalCost,
                OrgStructureId = softwareLicense.OrgStructureId,
                AssignedUsers = softwareLicense.AssignedUsers,
                LicenseStatus = softwareLicense.LicenseStatus,
                UsageLimitations = softwareLicense.UsageLimitations,
                RenewalNotificationDate = softwareLicense.RenewalNotificationDate,
                IsRenewalNotified = softwareLicense.IsRenewalNotified,
                SupportContact = softwareLicense.SupportContact,
                CommentsOrNotes = softwareLicense.CommentsOrNotes,
                DateCreated = softwareLicense.DateCreated,
                LastModified = softwareLicense.LastModified,
                CreatedBy = softwareLicense.CreatedBy,
                ModifiedBy = softwareLicense.ModifiedBy
            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
