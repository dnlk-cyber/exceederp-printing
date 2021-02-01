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
    public class ArRemitAddressController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARRemitAddresses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARRemitAddress> arremitaddresses = _db.ARRemitAddresses;
            DataSourceResult result = arremitaddresses.ToDataSourceResult(request, aRRemitAddress => new {
                ARRemitAddressID = aRRemitAddress.ARRemitAddressID,
                Company = aRRemitAddress.Company,
                Address = aRRemitAddress.Address,
                PostalCode = aRRemitAddress.PostalCode,
                OrgBranchName = aRRemitAddress.OrgBranchName,
                DateCreated = aRRemitAddress.DateCreated,
                LastModified = aRRemitAddress.LastModified,
                CreatedBy = aRRemitAddress.CreatedBy,
                ModifiedBy = aRRemitAddress.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARRemitAddresses_Create([DataSourceRequest]DataSourceRequest request, ARRemitAddress aRRemitAddress)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARRemitAddress
                {
                    Company = aRRemitAddress.Company,
                    Address = aRRemitAddress.Address,
                    PostalCode = aRRemitAddress.PostalCode,
                    OrgBranchName = aRRemitAddress.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRRemitAddress.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRRemitAddress.ModifiedBy
                };

                _db.ARRemitAddresses.Add(entity);
                _db.SaveChanges();
                aRRemitAddress.ARRemitAddressID = entity.ARRemitAddressID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR remit address", "Success", User.Identity.Name, $"created  {aRRemitAddress.Company}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRRemitAddress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARRemitAddresses_Update([DataSourceRequest]DataSourceRequest request, ARRemitAddress aRRemitAddress)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARRemitAddress
                {
                    ARRemitAddressID = aRRemitAddress.ARRemitAddressID,
                    Company = aRRemitAddress.Company,
                    Address = aRRemitAddress.Address,
                    PostalCode = aRRemitAddress.PostalCode,
                    OrgBranchName = aRRemitAddress.OrgBranchName,
                    DateCreated = aRRemitAddress.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRRemitAddress.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARRemitAddresses.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR remit address", "Success", User.Identity.Name, $"updated  {aRRemitAddress.Company}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRRemitAddress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARRemitAddresses_Destroy([DataSourceRequest]DataSourceRequest request, ARRemitAddress aRRemitAddress)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARRemitAddress
                {
                    ARRemitAddressID = aRRemitAddress.ARRemitAddressID,
                    Company = aRRemitAddress.Company,
                    Address = aRRemitAddress.Address,
                    PostalCode = aRRemitAddress.PostalCode,
                    OrgBranchName = aRRemitAddress.OrgBranchName,
                    DateCreated = aRRemitAddress.DateCreated,
                    LastModified = aRRemitAddress.LastModified,
                    CreatedBy = aRRemitAddress.CreatedBy,
                    ModifiedBy = aRRemitAddress.ModifiedBy
                };

                _db.ARRemitAddresses.Attach(entity);
                _db.ARRemitAddresses.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRRemitAddress }.ToDataSourceResult(request, ModelState));
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
