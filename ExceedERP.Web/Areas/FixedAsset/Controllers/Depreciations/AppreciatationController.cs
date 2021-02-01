using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Depreciations
{

   
    public class AppreciatationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Appreciations_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<Appreciation> appreciations = db.Appreciations;
            DataSourceResult result = appreciations.ToDataSourceResult(request, appreciation => new {
                AppreciationID = appreciation.AppreciationID,
                AssetRegistrationId = appreciation.AssetRegistrationId,
                Description = appreciation.Description,
                Amount = appreciation.Amount,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appreciations_Create([DataSourceRequest]DataSourceRequest request, Appreciation appreciation , int id)
      {
           var registration = db.AssetRegistrations.Find(id);
           
            if (ModelState.IsValid)
            {
                if (registration != null && registration.BookValue < appreciation.Amount)
                {
                    ModelState.AddModelError("over","Appreciation of "+appreciation.Amount+" is more than the current book value ("+registration.BookValue+") of the item.");
                }
                var entity = new Appreciation
                {
                    AppreciationDate = appreciation.AppreciationDate,
                    Description = appreciation.Description,
                    Amount = appreciation.Amount,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    AssetRegistrationId = id
                };

                db.Appreciations.Add(entity);
                db.SaveChanges();
                appreciation.AppreciationID = entity.AppreciationID;
            }

            return Json(new[] { appreciation }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appreciations_Update([DataSourceRequest]DataSourceRequest request, Appreciation appreciation)
        {
            var registration = db.AssetRegistrations.Find(appreciation.RegistrationId);

            if (ModelState.IsValid)
            {
                if (registration != null && registration.BookValue < appreciation.Amount)
                {
                    ModelState.AddModelError("over", "Appreciation of " + appreciation.Amount + " is more than the current book value (" + registration.BookValue + ") of the item.");
                }
                var entity = new Appreciation
                {
                    AssetRegistrationId = appreciation.AssetRegistrationId,
                    AppreciationID = appreciation.AppreciationID,
                    AppreciationDate = appreciation.AppreciationDate,
                    Description = appreciation.Description,
                    Amount = appreciation.Amount,
                };

                db.Appreciations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { appreciation }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appreciations_Destroy([DataSourceRequest]DataSourceRequest request, Appreciation appreciation)
        {
            if (ModelState.IsValid)
            {
                var entity = new Appreciation
                {
                    AssetRegistrationId = appreciation.AssetRegistrationId,
                    AppreciationID = appreciation.AppreciationID,
                    AppreciationDate = appreciation.AppreciationDate,
                    Description = appreciation.Description,
                    Amount = appreciation.Amount,
                };

                db.Appreciations.Attach(entity);
                db.Appreciations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { appreciation }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
