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
    public class ApSupplierRegistrationController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
       
        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
 
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
 
        public ActionResult APSupplierRegistrations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<APSupplierRegistration> apsupplierregistrations = _db.APSupplierRegistrations;
            DataSourceResult result = apsupplierregistrations.ToDataSourceResult(request, aPSupplierRegistration => new {
                APSupplierRegistrationID = aPSupplierRegistration.APSupplierRegistrationID,
                SupplierName = aPSupplierRegistration.SupplierName,
                Account = aPSupplierRegistration.Account,
                PhoneNo = aPSupplierRegistration.PhoneNo,
                TINNo = aPSupplierRegistration.TINNo,
                Address1 = aPSupplierRegistration.Address1,
                Address2 = aPSupplierRegistration.Address2,
                ContactName = aPSupplierRegistration.ContactName,
                Country = aPSupplierRegistration.Country,
                SupplierType = aPSupplierRegistration.SupplierType,
                BusinessType = aPSupplierRegistration.BusinessType,
                IsActive = aPSupplierRegistration.IsActive,
                OrgBranchName = aPSupplierRegistration.OrgBranchName,
                DateCreated = aPSupplierRegistration.DateCreated,
                LastModified = aPSupplierRegistration.LastModified,
                CreatedBy = aPSupplierRegistration.CreatedBy,
                ModifiedBy = aPSupplierRegistration.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierRegistrations_Create([DataSourceRequest]DataSourceRequest request, APSupplierRegistration aPSupplierRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierRegistration
                {
                    SupplierName = aPSupplierRegistration.SupplierName,
                    Account = aPSupplierRegistration.Account,
                    PhoneNo = aPSupplierRegistration.PhoneNo,
                    TINNo = aPSupplierRegistration.TINNo,
                    Address1 = aPSupplierRegistration.Address1,
                    Address2 = aPSupplierRegistration.Address2,
                    ContactName = aPSupplierRegistration.ContactName,
                    Country = aPSupplierRegistration.Country,
                    SupplierType = aPSupplierRegistration.SupplierType,
                    BusinessType = aPSupplierRegistration.BusinessType,
                    IsActive = aPSupplierRegistration.IsActive,
                    OrgBranchName = aPSupplierRegistration.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aPSupplierRegistration.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPSupplierRegistration.ModifiedBy
                };

                _db.APSupplierRegistrations.Add(entity);
                _db.SaveChanges();
                aPSupplierRegistration.APSupplierRegistrationID = entity.APSupplierRegistrationID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier registration", "Success", User.Identity.Name, $"created supplier {entity.SupplierName} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierRegistrations_Update([DataSourceRequest]DataSourceRequest request, APSupplierRegistration aPSupplierRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierRegistration
                {
                    APSupplierRegistrationID = aPSupplierRegistration.APSupplierRegistrationID,
                    SupplierName = aPSupplierRegistration.SupplierName,
                    Account = aPSupplierRegistration.Account,
                    PhoneNo = aPSupplierRegistration.PhoneNo,
                    TINNo = aPSupplierRegistration.TINNo,
                    Address1 = aPSupplierRegistration.Address1,
                    Address2 = aPSupplierRegistration.Address2,
                    ContactName = aPSupplierRegistration.ContactName,
                    Country = aPSupplierRegistration.Country,
                    SupplierType = aPSupplierRegistration.SupplierType,
                    BusinessType = aPSupplierRegistration.BusinessType,
                    IsActive = aPSupplierRegistration.IsActive,
                    OrgBranchName = aPSupplierRegistration.OrgBranchName,
                    DateCreated = aPSupplierRegistration.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aPSupplierRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.APSupplierRegistrations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier registration", "Success", User.Identity.Name, $"updated supplier {entity.SupplierName} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierRegistrations_Destroy([DataSourceRequest]DataSourceRequest request, APSupplierRegistration aPSupplierRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierRegistration
                {
                    APSupplierRegistrationID = aPSupplierRegistration.APSupplierRegistrationID,
                    SupplierName = aPSupplierRegistration.SupplierName,
                    Account = aPSupplierRegistration.Account,
                    PhoneNo = aPSupplierRegistration.PhoneNo,
                    TINNo = aPSupplierRegistration.TINNo,
                    Address1 = aPSupplierRegistration.Address1,
                    Address2 = aPSupplierRegistration.Address2,
                    ContactName = aPSupplierRegistration.ContactName,
                    Country = aPSupplierRegistration.Country,
                    SupplierType = aPSupplierRegistration.SupplierType,
                    BusinessType = aPSupplierRegistration.BusinessType,
                    IsActive = aPSupplierRegistration.IsActive,
                    OrgBranchName = aPSupplierRegistration.OrgBranchName,
                    DateCreated = aPSupplierRegistration.DateCreated,
                    LastModified = aPSupplierRegistration.LastModified,
                    CreatedBy = aPSupplierRegistration.CreatedBy,
                    ModifiedBy = aPSupplierRegistration.ModifiedBy
                };

                _db.APSupplierRegistrations.Attach(entity);
                _db.APSupplierRegistrations.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AP supplier " + aPSupplierRegistration.SupplierName + " " + aPSupplierRegistration.SupplierType, Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
 
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
