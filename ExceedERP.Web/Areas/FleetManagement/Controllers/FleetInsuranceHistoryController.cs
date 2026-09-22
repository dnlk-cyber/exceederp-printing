using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   
    public class FleetInsuranceHistoryController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
       i => new
       {
           Text = i.Name,
           Value = i.Id
       }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).ToList();
            ViewData["InsuranceCompany"] = db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName }).ToList();
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var FIhistory = db.FleetInsuranceHistorys.ToList();
            var result = FIhistory.ToDataSourceResult(request, fleetsInsurance => new
            {
                FleetInsuranceHistoryId = fleetsInsurance.FleetInsuranceHistoryId,
                EquipmentCategoryId = fleetsInsurance.EquipmentCategoryId,
                EquipmentNameId = fleetsInsurance.EquipmentNameId,
                EquipmentTypeId = fleetsInsurance.EquipmentTypeId,
                ExpireDate = fleetsInsurance.ExpireDate,
                FleetInsuranceId = fleetsInsurance.FleetInsuranceId,
                FleetsID = fleetsInsurance.FleetsID,
               HazardOfExpiryDate = fleetsInsurance.HazardOfExpiryDate,
               InsuranceCompanyId = fleetsInsurance.InsuranceCompanyId,
                Description = fleetsInsurance.Description,
               InsuranceType = fleetsInsurance.InsuranceType,
                PolicyNo = fleetsInsurance.PolicyNo,
               Remark = fleetsInsurance.Remark,
               ThirdParty = fleetsInsurance.ThirdParty,
                ThirdPartyInsurance = fleetsInsurance.ThirdPartyInsurance,
                ComprehensiveInsurance = fleetsInsurance.ComprehensiveInsurance


        });
            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}