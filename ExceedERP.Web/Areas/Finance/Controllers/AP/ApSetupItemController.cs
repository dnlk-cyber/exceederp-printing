using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    [AuthorizeRoles(FinanceConstants.AccountPayableAdmin)]
    public class ApSetupItemController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApSetupItems_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApSetupItem> apsetupitems = _db.ApSetupItems;
            DataSourceResult result = apsetupitems.ToDataSourceResult(request, apSetupItem => new {
                ApSetupItemId = apSetupItem.ApSetupItemId,
                Name = apSetupItem.Name,
                Description = apSetupItem.Description,
                AccountCode = apSetupItem.AccountCode,
                CreationSign = apSetupItem.CreationSign,
                DateCreated = apSetupItem.DateCreated,
                LastModified = apSetupItem.LastModified,
                CreatedBy = apSetupItem.CreatedBy,
                ModifiedBy = apSetupItem.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupItems_Create([DataSourceRequest]DataSourceRequest request, ApSetupItem apSetupItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupItem
                {
                    Name = apSetupItem.Name,
                    Description = apSetupItem.Description,
                    AccountCode = apSetupItem.AccountCode,
                    CreationSign = apSetupItem.CreationSign,
                    DateCreated = DateTime.Now,
                    LastModified = apSetupItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apSetupItem.ModifiedBy
                };

                _db.ApSetupItems.Add(entity);
                _db.SaveChanges();
                apSetupItem.ApSetupItemId = entity.ApSetupItemId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Items setup", "Success", User.Identity.Name, $"created  {apSetupItem.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupItems_Update([DataSourceRequest]DataSourceRequest request, ApSetupItem apSetupItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupItem
                {
                    ApSetupItemId = apSetupItem.ApSetupItemId,
                    Name = apSetupItem.Name,
                    Description = apSetupItem.Description,
                    AccountCode = apSetupItem.AccountCode,
                    CreationSign = apSetupItem.CreationSign,
                    DateCreated = apSetupItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = apSetupItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ApSetupItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Items setup", "Success", User.Identity.Name, $"updated  {apSetupItem.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupItems_Destroy([DataSourceRequest]DataSourceRequest request, ApSetupItem apSetupItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupItem
                {
                    ApSetupItemId = apSetupItem.ApSetupItemId,
                    Name = apSetupItem.Name,
                    Description = apSetupItem.Description,
                    AccountCode = apSetupItem.AccountCode,
                    CreationSign = apSetupItem.CreationSign,
                    DateCreated = apSetupItem.DateCreated,
                    LastModified = apSetupItem.LastModified,
                    CreatedBy = apSetupItem.CreatedBy,
                    ModifiedBy = apSetupItem.ModifiedBy
                };

                _db.ApSetupItems.Attach(entity);
                _db.ApSetupItems.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AP item " + apSetupItem.ApSetupItemId + " " + apSetupItem.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupItem }.ToDataSourceResult(request, ModelState));
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
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
