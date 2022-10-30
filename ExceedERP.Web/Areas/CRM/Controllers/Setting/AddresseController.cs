using System;
using ExceedERP.Web.Helpers;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class AddresseController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Addresses_Read([DataSourceRequest]DataSourceRequest request, string referenceID, string addressReferenceType)
        {
            IQueryable<Address> addresses = db.Addresses.Where(ad => ad.Reference == referenceID.ToString() && ad.ReferenceType == addressReferenceType);
            DataSourceResult result = addresses.ToDataSourceResult(request, address => new
            {
                AddressId = address.AddressId,
                ReferenceType = address.ReferenceType,
                Reference = address.Reference,
                Category = address.Category,
                Value = address.Value,
                CreatedBy = address.CreatedBy,
                DateCreated = address.DateCreated,
                LastModified = address.LastModified,
                ModifiedBy = address.ModifiedBy,
                OrgBranchName = address.OrgBranchName,
                Remark = address.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Create([DataSourceRequest]DataSourceRequest request, Address address, string referenceID, string addressReferenceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new Address
                {
                    Reference = referenceID.ToString(),
                    Category = address.Category,
                    Value = address.Value,
                    ReferenceType = addressReferenceType,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = address.Remark
                };

                db.Addresses.Add(entity);
                db.SaveChanges();
                address.AddressId = entity.AddressId;
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Update([DataSourceRequest]DataSourceRequest request, Address address)
        {
            if (ModelState.IsValid)
            {
                var entity = new Address
                {
                    ReferenceType = address.ReferenceType,
                    AddressId = address.AddressId,
                    Reference = address.Reference,
                    Category = address.Category,
                    Value = address.Value,
                    Remark = address.Remark
                };

                db.Addresses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Addresses_Destroy([DataSourceRequest]DataSourceRequest request, Address address)
        {
            if (ModelState.IsValid)
            {
                var entity = new Address
                {
                    AddressId = address.AddressId,
                    Reference = address.Reference,
                    Category = address.Category,
                    Value = address.Value,
                    Remark = address.Remark
                };

                db.Addresses.Attach(entity);
                db.Addresses.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Address number " + address.AddressId);

            }

            return Json(new[] { address }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
