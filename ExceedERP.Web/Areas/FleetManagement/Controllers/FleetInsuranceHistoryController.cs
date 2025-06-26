using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   
    public class FleetInsuranceHistoryController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();

            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
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
    }
}