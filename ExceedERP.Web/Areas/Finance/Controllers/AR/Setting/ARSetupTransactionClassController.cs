using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
     [Authorize(Roles = AccountReceivableAdmin)]
    public class ArSetupTransactionClassController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARSetupTransactionClasses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARSetupTransactionClass> arsetuptransactionclasses = _db.ARSetupTransactionClasses;
            DataSourceResult result = arsetuptransactionclasses.ToDataSourceResult(request, aRSetupTransactionClass => new {
                ARSetupTransactionClassID = aRSetupTransactionClass.ARSetupTransactionClassID,
                ClassName = aRSetupTransactionClass.ClassName,
                Description = aRSetupTransactionClass.Description,
                DateCreated = aRSetupTransactionClass.DateCreated,
                LastModified = aRSetupTransactionClass.LastModified,
                CreatedBy = aRSetupTransactionClass.CreatedBy,
                ModifiedBy = aRSetupTransactionClass.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionClasses_Create([DataSourceRequest]DataSourceRequest request, ARSetupTransactionClass aRSetupTransactionClass)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionClass
                {
                    ClassName = aRSetupTransactionClass.ClassName,
                    Description = aRSetupTransactionClass.Description,
                    DateCreated = DateTime.Now,
                    LastModified = aRSetupTransactionClass.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRSetupTransactionClass.ModifiedBy
                };

                _db.ARSetupTransactionClasses.Add(entity);
                _db.SaveChanges();
                aRSetupTransactionClass.ARSetupTransactionClassID = entity.ARSetupTransactionClassID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR transaction class", "Success", User.Identity.Name, $"created  {aRSetupTransactionClass.ClassName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTransactionClass }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionClasses_Update([DataSourceRequest]DataSourceRequest request, ARSetupTransactionClass aRSetupTransactionClass)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionClass
                {
                    ARSetupTransactionClassID = aRSetupTransactionClass.ARSetupTransactionClassID,
                    ClassName = aRSetupTransactionClass.ClassName,
                    Description = aRSetupTransactionClass.Description,
                    DateCreated = aRSetupTransactionClass.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRSetupTransactionClass.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARSetupTransactionClasses.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR transaction class", "Success", User.Identity.Name, $"updated {aRSetupTransactionClass.ClassName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTransactionClass }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionClasses_Destroy([DataSourceRequest]DataSourceRequest request, ARSetupTransactionClass aRSetupTransactionClass)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionClass
                {
                    ARSetupTransactionClassID = aRSetupTransactionClass.ARSetupTransactionClassID,
                    ClassName = aRSetupTransactionClass.ClassName,
                    Description = aRSetupTransactionClass.Description,
                    DateCreated = aRSetupTransactionClass.DateCreated,
                    LastModified = aRSetupTransactionClass.LastModified,
                    CreatedBy = aRSetupTransactionClass.CreatedBy,
                    ModifiedBy = aRSetupTransactionClass.ModifiedBy
                };

                _db.ARSetupTransactionClasses.Attach(entity);
                _db.ARSetupTransactionClasses.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRSetupTransactionClass }.ToDataSourceResult(request, ModelState));
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
