using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
    public class ArCustomerProfileController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();



        [AuthorizeRoles(FinanceConstants.AccountReceivableView, FinanceConstants.AccountReceivableCollection, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {

            var customerTypes = _db.ARCustomerTypes;
            ViewData["customerTypes"] = customerTypes.Select(e => new
            {
                CustomerTypeName = e.CustomerTypeName
            }).AsQueryable();


            var countries = _db.GLCountryTypes;
            ViewData["countries"] = countries.Select(e => new
            {
                CountryName = e.CountryName
            }).AsQueryable();
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView + "," + FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARCustomerProfiles_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArCustomerProfile> arcustomerprofiles = _db.ARCustomerProfiles;
            DataSourceResult result = arcustomerprofiles.ToDataSourceResult(request, aRCustomerProfile => new
            {
                ArCustomerProfileId = aRCustomerProfile.ArCustomerProfileId,
                CustomerId = aRCustomerProfile.CustomerId,
                CompanyName = aRCustomerProfile.CompanyName,
                AccountCode = aRCustomerProfile.AccountCode,
                TinNo = aRCustomerProfile.TinNo,
                CustomerType = aRCustomerProfile.CustomerType,
                ContactName = aRCustomerProfile.ContactName,
                PhoneNumber = aRCustomerProfile.PhoneNumber,
                Address = aRCustomerProfile.Address,
                Country = aRCustomerProfile.Country,
                IsActive = aRCustomerProfile.IsActive,
                OrgBranchName = aRCustomerProfile.OrgBranchName,
                DateCreated = aRCustomerProfile.DateCreated,
                LastModified = aRCustomerProfile.LastModified,
                CreatedBy = aRCustomerProfile.CreatedBy,
                ModifiedBy = aRCustomerProfile.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerProfiles_Create([DataSourceRequest]DataSourceRequest request, ArCustomerProfile aRCustomerProfile)
        {
            #region validation
            var profile = _db.ARCustomerProfiles;
            var exists = profile.Where(x => x.CompanyName == aRCustomerProfile.CompanyName).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The customer name already exists.");
            }

            var exists1 = profile.Where(x => x.CustomerId == aRCustomerProfile.CustomerId).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The customer ID already exists.");
            }
            if (string.IsNullOrEmpty(aRCustomerProfile.PhoneNumber))
            {
                ModelState.AddModelError("", "Phone number is required");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArCustomerProfile
                {
                    CustomerId = aRCustomerProfile.CustomerId,
                    CompanyName = aRCustomerProfile.CompanyName,
                    AccountCode = aRCustomerProfile.AccountCode,
                    TinNo = aRCustomerProfile.TinNo,
                    CustomerType = aRCustomerProfile.CustomerType,
                    ContactName = aRCustomerProfile.ContactName,
                    PhoneNumber = aRCustomerProfile.PhoneNumber,
                    Address = aRCustomerProfile.Address,
                    Country = aRCustomerProfile.Country,
                    IsActive = aRCustomerProfile.IsActive,
                    OrgBranchName = aRCustomerProfile.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRCustomerProfile.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRCustomerProfile.ModifiedBy
                };

                _db.ARCustomerProfiles.Add(entity);
                _db.SaveChanges();
                aRCustomerProfile.ArCustomerProfileId = entity.ArCustomerProfileId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR customer profile", "Success", User.Identity.Name, $"created  {aRCustomerProfile.CompanyName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRCustomerProfile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerProfiles_Update([DataSourceRequest]DataSourceRequest request, ArCustomerProfile aRCustomerProfile)
        {
            #region validation

            var profile = _db.ARCustomerProfiles;
            var exists = profile.Where(x => x.ArCustomerProfileId != aRCustomerProfile.ArCustomerProfileId)
                                 .Where(x => x.CompanyName == aRCustomerProfile.CompanyName).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The customer name already exists.");
            }

            var exists1 = profile.Where(x => x.ArCustomerProfileId != aRCustomerProfile.ArCustomerProfileId)
                                                    .Where(x => x.CustomerId == aRCustomerProfile.CustomerId).AsQueryable();
            if (exists1.Any())
            {
                ModelState.AddModelError("exist", "The customer ID already exists.");
            }
            if (string.IsNullOrEmpty(aRCustomerProfile.PhoneNumber))
            {
                ModelState.AddModelError("", "Phone number is required");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArCustomerProfile
                {
                    ArCustomerProfileId = aRCustomerProfile.ArCustomerProfileId,
                    CustomerId = aRCustomerProfile.CustomerId,
                    CompanyName = aRCustomerProfile.CompanyName,
                    AccountCode = aRCustomerProfile.AccountCode,
                    TinNo = aRCustomerProfile.TinNo,
                    CustomerType = aRCustomerProfile.CustomerType,
                    ContactName = aRCustomerProfile.ContactName,
                    PhoneNumber = aRCustomerProfile.PhoneNumber,
                    Address = aRCustomerProfile.Address,
                    Country = aRCustomerProfile.Country,
                    IsActive = aRCustomerProfile.IsActive,
                    OrgBranchName = aRCustomerProfile.OrgBranchName,
                    DateCreated = aRCustomerProfile.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRCustomerProfile.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARCustomerProfiles.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR customer profile", "Success", User.Identity.Name, $"updated  {aRCustomerProfile.CompanyName}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRCustomerProfile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerProfiles_Destroy([DataSourceRequest]DataSourceRequest request, ArCustomerProfile aRCustomerProfile)
        {
            //if (ModelState.IsValid)
            //{
            //    var entity = new ArCustomerProfile
            //    {
            //        ArCustomerProfileId = aRCustomerProfile.ArCustomerProfileId,
            //        CustomerId = aRCustomerProfile.CustomerId,
            //        CompanyName = aRCustomerProfile.CompanyName,
            //        Description = aRCustomerProfile.Description,
            //        TinNo = aRCustomerProfile.TinNo,
            //        CustomerType = aRCustomerProfile.CustomerType,
            //        ContactName = aRCustomerProfile.ContactName,
            //        PhoneNumber = aRCustomerProfile.PhoneNumber,
            //        Address = aRCustomerProfile.Address,
            //        Country = aRCustomerProfile.Country,
            //        IsActive = aRCustomerProfile.IsActive,
            //        OrgBranchName = aRCustomerProfile.OrgBranchName,
            //        DateCreated = aRCustomerProfile.DateCreated,
            //        LastModified = aRCustomerProfile.LastModified,
            //        CreatedBy = aRCustomerProfile.CreatedBy,
            //        ModifiedBy = aRCustomerProfile.ModifiedBy
            //    };

            //    _db.ARCustomerProfiles.Attach(entity);
            //    _db.ARCustomerProfiles.Remove(entity);
            //    _db.SaveChanges();
            //}

            return Json(new[] { aRCustomerProfile }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView + "," + FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView + "," + FinanceConstants.AccountReceivableCollection + "," + FinanceConstants.AccountReceivableInvoice)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        public ActionResult UpdateEmptyPhoneWithDefault()
        {
            var customersWithNoPhone = _db.ARCustomerProfiles.Where(c => c.PhoneNumber == null).ToList();
            if (customersWithNoPhone.Any())
            {
                var defaultPhone = "+251 – 11000000";
                foreach (var item in customersWithNoPhone)
                {
                    item.PhoneNumber = defaultPhone;

                    _db.ARCustomerProfiles.Attach(item);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                //if (result > 0)
                //{
                //    return Json(new
                //    {
                //        Message = $"{customersWithNoPhone.Count} Customers set to default phone number ({defaultPhone})",
                //        Status = "OK"
                //    }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json(new
                //    {
                //        Message = $"Operation canceled.",
                //        Status = "Error"
                //    }, JsonRequestBehavior.AllowGet);
                //}
            }
            //else
            //{
            //    return Json(new
            //    {
            //        Message = $"All customers have phone numbers.",
            //        Status = "Error"
            //    }, JsonRequestBehavior.AllowGet);
            //}
            return Redirect(nameof(Index));
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
