using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{

    public class RevaluationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Revaluations_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetRevaluation> revaluations = db.FixedAssetRevaluations.Where(x => x.AssetRegistrationId == id);
            DataSourceResult result = revaluations.ToDataSourceResult(request, revaluation => new {
                FixedAssetRevaluationID = revaluation.FixedAssetRevaluationID,
                AssetRegistrationId = revaluation.AssetRegistrationId,
                AddedValue = revaluation.AddedValue,
                RevaluationDate = revaluation.RevaluationDate,
                Description = revaluation.Description,
                
                OrgBranchName = revaluation.OrgBranchName,
                DateCreated = revaluation.DateCreated,
                LastModified = revaluation.LastModified,
                CreatedBy = revaluation.CreatedBy,
                ModifiedBy = revaluation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
      public ActionResult Revaluations_Create(int id, [DataSourceRequest]DataSourceRequest request, FixedAssetRevaluation revaluation)
        {
             

            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRevaluation();

                entity.AssetRegistrationId = id;
                entity.RevaluationDate = revaluation.RevaluationDate;
                entity.AddedValue = revaluation.AddedValue;
                entity.Description = revaluation.Description;
                
                entity.OrgBranchName = revaluation.OrgBranchName;
                entity.DateCreated = revaluation.DateCreated;
                entity.LastModified = revaluation.LastModified;
                entity.CreatedBy = revaluation.CreatedBy;
                entity.ModifiedBy = revaluation.ModifiedBy;

                db.FixedAssetRevaluations.Add(entity);
                db.SaveChanges();
                revaluation.FixedAssetRevaluationID = entity.FixedAssetRevaluationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Revaluation ", "Success", User.Identity.Name, $"created Revaluation {entity.FixedAssetRevaluationID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
     public ActionResult Revaluations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRevaluation revaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRevaluation
                {
                    AssetRegistrationId = revaluation.AssetRegistrationId,
                    FixedAssetRevaluationID = revaluation.FixedAssetRevaluationID,
                    RevaluationDate = revaluation.RevaluationDate,
                    AddedValue = revaluation.AddedValue,
                    Description = revaluation.Description,
                    OrgBranchName = revaluation.OrgBranchName,
                    DateCreated = revaluation.DateCreated,
                    LastModified = revaluation.LastModified,
                    CreatedBy = revaluation.CreatedBy,
                    ModifiedBy = revaluation.ModifiedBy
                };

                db.FixedAssetRevaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Revaluation ", "Success", User.Identity.Name, $"Updated Revaluation {entity.FixedAssetRevaluationID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Revaluations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRevaluation revaluation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRevaluation
                {
                    AssetRegistrationId = revaluation.AssetRegistrationId,
                    FixedAssetRevaluationID = revaluation.FixedAssetRevaluationID,
                    RevaluationDate = revaluation.RevaluationDate,
                    AddedValue = revaluation.AddedValue,
                    Description = revaluation.Description,
                    OrgBranchName = revaluation.OrgBranchName,
                    DateCreated = revaluation.DateCreated,
                    LastModified = revaluation.LastModified,
                    CreatedBy = revaluation.CreatedBy,
                    ModifiedBy = revaluation.ModifiedBy
                };

                db.FixedAssetRevaluations.Attach(entity);
                db.FixedAssetRevaluations.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Revaluation {revaluation.FixedAssetRevaluationID } and Tag {revaluation.AssetRegistrationId}");
                #endregion
            }

            return Json(new[] { revaluation }.ToDataSourceResult(request, ModelState));
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
