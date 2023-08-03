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
    public class ApBusinessTypeController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
       

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult APBusinessTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<APBusinessType> apbusinesstypes = _db.APBusinessTypes;
            DataSourceResult result = apbusinesstypes.ToDataSourceResult(request, aPBusinessType => new
            {
                APBusinessTypeID = aPBusinessType.APBusinessTypeID,
                TypeOfBusiness = aPBusinessType.TypeOfBusiness,
                Description = aPBusinessType.Description,
                DateCreated = DateTime.Now,
                LastModified = aPBusinessType.LastModified,
                CreatedBy = User.Identity.Name,
                ModifiedBy = aPBusinessType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APBusinessTypes_Create([DataSourceRequest]DataSourceRequest request, APBusinessType aPBusinessType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APBusinessType
                {
                    TypeOfBusiness = aPBusinessType.TypeOfBusiness,
                    Description = aPBusinessType.Description,
                    DateCreated = aPBusinessType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aPBusinessType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.APBusinessTypes.Add(entity);
                _db.SaveChanges();
                aPBusinessType.APBusinessTypeID = entity.APBusinessTypeID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP business type", "Success", User.Identity.Name, $"created  {aPBusinessType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPBusinessType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APBusinessTypes_Update([DataSourceRequest]DataSourceRequest request, APBusinessType aPBusinessType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APBusinessType
                {
                    APBusinessTypeID = aPBusinessType.APBusinessTypeID,
                    TypeOfBusiness = aPBusinessType.TypeOfBusiness,
                    Description = aPBusinessType.Description,
                    DateCreated = aPBusinessType.DateCreated,
                    LastModified = aPBusinessType.LastModified,
                    CreatedBy = aPBusinessType.CreatedBy,
                    ModifiedBy = aPBusinessType.ModifiedBy
                };

                _db.APBusinessTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP Business type", "Success", User.Identity.Name, $"updated  {aPBusinessType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPBusinessType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APBusinessTypes_Destroy([DataSourceRequest]DataSourceRequest request, APBusinessType aPBusinessType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APBusinessType
                {
                    APBusinessTypeID = aPBusinessType.APBusinessTypeID,
                    TypeOfBusiness = aPBusinessType.TypeOfBusiness,
                    Description = aPBusinessType.Description,
                    DateCreated = aPBusinessType.DateCreated,
                    LastModified = aPBusinessType.LastModified,
                    CreatedBy = aPBusinessType.CreatedBy,
                    ModifiedBy = aPBusinessType.ModifiedBy
                };

                _db.APBusinessTypes.Attach(entity);
                _db.APBusinessTypes.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted business type " + aPBusinessType.APBusinessTypeID + " " + aPBusinessType.TypeOfBusiness, Modules.Finance);
                #endregion
            }

            return Json(new[] { aPBusinessType }.ToDataSourceResult(request, ModelState));
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
