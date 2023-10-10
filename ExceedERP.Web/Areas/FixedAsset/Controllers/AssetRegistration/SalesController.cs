using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
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
    
    public class SalesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
           
            return View();
        }

         [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Sales_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Sales> sales = db.Sales.Where(x => x.AssetRegistrationId == id);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new {
                SalesID = sale.SalesID,
                AssetRegistrationId = sale.AssetRegistrationId,
                Amount = sale.Amount,
                GLTaxRateID = sale.GLTaxRateID,
                TaxAmount = sale.TaxAmount,
                TotalAmount = sale.TotalAmount,
                Description = sale.Description,
                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sales_Create(int id, [DataSourceRequest]DataSourceRequest request, Sales sales)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add AssetRegistration First");
            }
            var tax = db.GLTaxRates.Find(sales.GLTaxRateID);
            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new Sales
                    {
                        AssetRegistrationId = id,
                        Amount = sales.Amount,
                        GLTaxRateID = sales.GLTaxRateID,
                        TaxAmount = sales.Amount*tax.CalculatedRate,
                        TotalAmount = sales.Amount + (sales.Amount * tax.CalculatedRate),
                        Description = sales.Description,
                        OrgBranchName = sales.OrgBranchName,
                        DateCreated = sales.DateCreated,
                        LastModified = sales.LastModified,
                        CreatedBy = sales.CreatedBy,
                        ModifiedBy = sales.ModifiedBy
                    };



                    db.Sales.Add(entity);
                    db.SaveChanges();
                    sales.SalesID = entity.SalesID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "sales ", "Success", User.Identity.Name, $"created sales {entity.SalesID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                    #endregion

                }
            }

            return Json(new[] { sales }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sales_Update([DataSourceRequest]DataSourceRequest request, Sales sales)
        {
            var tax = db.GLTaxRates.Find(sales.GLTaxRateID);
            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new Sales
                    {
                        AssetRegistrationId = sales.AssetRegistrationId,
                        SalesID = sales.SalesID,
                        Amount = sales.Amount,
                        GLTaxRateID = sales.GLTaxRateID,
                        TaxAmount = sales.Amount * tax.CalculatedRate,
                        TotalAmount = sales.Amount + (sales.Amount * tax.CalculatedRate),
                        Description = sales.Description,
                        OrgBranchName = sales.OrgBranchName,
                        DateCreated = sales.DateCreated,
                        LastModified = sales.LastModified,
                        CreatedBy = sales.CreatedBy,
                        ModifiedBy = sales.ModifiedBy
                    };

                    db.Sales.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "sales ", "Success", User.Identity.Name, $"Updated sales {entity.SalesID} Tag {entity.AssetRegistrationId}", Modules.FixedAsset);
                    #endregion
                }
                db.SaveChanges();

                
            }

            return Json(new[] { sales }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Sales_Destroy([DataSourceRequest]DataSourceRequest request, Sales sales)
        {
            if (ModelState.IsValid)
            {
                var entity = new Sales
                {
                    AssetRegistrationId = sales.AssetRegistrationId,
                    SalesID = sales.SalesID,
                    Amount = sales.Amount,
                    GLTaxRateID = sales.GLTaxRateID,
                    TaxAmount = sales.TaxAmount,
                    TotalAmount = sales.TotalAmount,
                    Description = sales.Description,
                    OrgBranchName = sales.OrgBranchName,
                    DateCreated = sales.DateCreated,
                    LastModified = sales.LastModified,
                    CreatedBy = sales.CreatedBy,
                    ModifiedBy = sales.ModifiedBy
                };

                db.Sales.Attach(entity);
                db.Sales.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted Sales {sales.SalesID } and Tag {sales.AssetRegistrationId}");
                #endregion
            }

            return Json(new[] { sales }.ToDataSourceResult(request, ModelState));
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
