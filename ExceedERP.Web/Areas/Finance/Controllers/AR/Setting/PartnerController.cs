using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
    public class PartnerController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        public ActionResult Update()
        {
            var partners = _db.Partners;
            var subaccount = _db.GlSegmentSetups
                .FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            foreach (var partner in partners)
            {
                var subaccountDes = _db.GlChartOfAccountSegments
                    .Where(x=>x.Description==partner.Name)
                    .FirstOrDefault(x => x.GLSegmentSetupId == subaccount.GLSegmentSetupId);
                if (subaccountDes != null)
                {
                    partner.PartnerCode = subaccountDes.Values;
                    _db.Partners.Attach(partner);
                    _db.Entry(partner).State = EntityState.Modified;
                }
            }
           
            _db.SaveChanges();
            return View("Success");
        }

        private void PopulateViewData()
        {

           
            var partnerTypes = _db.PartnerTypes;
            var selectedtypea = partnerTypes.Select(
                i => new
                {
                    Name = i.TypeName,
                    Code = i.PartnerTypeId
                }).AsQueryable();
            ViewData["PartnerTypes"] = selectedtypea;
        }

        public ActionResult Partners_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Partner> partners = _db.Partners;
            DataSourceResult result = partners.ToDataSourceResult(request, partner => new {
                PartnerId = partner.PartnerId,
                PartnerCode = partner.PartnerCode,
                Name = partner.Name,
                PartnerTypeId = partner.PartnerTypeId,
                ContactName = partner.ContactName,
                PhoneNumber = partner.PhoneNumber,
                Address = partner.Address,
                Country = partner.Country,
                IsActive = partner.IsActive,
                OrgBranchName = partner.OrgBranchName,
                DateCreated = partner.DateCreated,
                LastModified = partner.LastModified,
                CreatedBy = partner.CreatedBy,
                ModifiedBy = partner.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Partners_Create([DataSourceRequest]DataSourceRequest request, Partner partner)
        {
            #region validation

            if (!String.IsNullOrEmpty(partner.PartnerCode))
            {
                var exists = _db.Partners.Where(x => x.PartnerCode == partner.PartnerCode).ToList();
                if (exists.Any())
                    ModelState.AddModelError("code", $"The code {partner.PartnerCode} already exisits");
            }
            else
            {
                ModelState.AddModelError("code", $"Please fill code");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new Partner
                {
                    PartnerCode = partner.PartnerCode,
                    Name = partner.Name,
                    PartnerTypeId = partner.PartnerTypeId,
                    ContactName = partner.ContactName,
                    PhoneNumber = partner.PhoneNumber,
                    Address = partner.Address,
                    Country = partner.Country,
                    IsActive = partner.IsActive,
                    OrgBranchName = partner.OrgBranchName,
                    DateCreated = partner.DateCreated,
                    LastModified = partner.LastModified,
                    CreatedBy = partner.CreatedBy,
                    ModifiedBy = partner.ModifiedBy
                };

                _db.Partners.Add(entity);
                _db.SaveChanges();
                partner.PartnerId = entity.PartnerId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR partner", "Success", User.Identity.Name, $"created  {partner.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { partner }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Partners_Update([DataSourceRequest]DataSourceRequest request, Partner partner)
        {
            #region validation

            if (!String.IsNullOrEmpty(partner.PartnerCode))
            {
                var exists = _db.Partners
                    .Where(x=>x.PartnerId!=partner.PartnerId).Where(x => x.PartnerCode == partner.PartnerCode).ToList();
                if (exists.Any())
                    ModelState.AddModelError("code", $"The code {partner.PartnerCode} already exisits");
            }
            else
            {
                ModelState.AddModelError("code", $"Please fill code");
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new Partner
                {
                    PartnerId = partner.PartnerId,
                    PartnerCode = partner.PartnerCode,
                    Name = partner.Name,
                    PartnerTypeId = partner.PartnerTypeId,
                    ContactName = partner.ContactName,
                    PhoneNumber = partner.PhoneNumber,
                    Address = partner.Address,
                    Country = partner.Country,
                    IsActive = partner.IsActive,
                    OrgBranchName = partner.OrgBranchName,
                    DateCreated = partner.DateCreated,
                    LastModified = partner.LastModified,
                    CreatedBy = partner.CreatedBy,
                    ModifiedBy = partner.ModifiedBy
                };

                _db.Partners.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR partner", "Success", User.Identity.Name, $"updated  {partner.Name}", Modules.Finance);
                #endregion
            }

            return Json(new[] { partner }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Partners_Destroy([DataSourceRequest]DataSourceRequest request, Partner partner)
        {
            if (ModelState.IsValid)
            {
                var entity = new Partner
                {
                    PartnerId = partner.PartnerId,
                    Name = partner.Name,
                    PartnerTypeId = partner.PartnerTypeId,
                    ContactName = partner.ContactName,
                    PhoneNumber = partner.PhoneNumber,
                    Address = partner.Address,
                    Country = partner.Country,
                    IsActive = partner.IsActive,
                    OrgBranchName = partner.OrgBranchName,
                    DateCreated = partner.DateCreated,
                    LastModified = partner.LastModified,
                    CreatedBy = partner.CreatedBy,
                    ModifiedBy = partner.ModifiedBy
                };

                _db.Partners.Attach(entity);
                _db.Partners.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { partner }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
