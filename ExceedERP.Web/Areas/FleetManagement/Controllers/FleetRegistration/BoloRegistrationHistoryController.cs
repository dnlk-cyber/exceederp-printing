using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetRegistration
{
    public class BoloRegistrationHistoryController : Controller
    {
        // GET: FleetManagement/BoloRegistrationHistory
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
        
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var brhistory = db.FleetBoloRegistrationsHistorys.ToList();
            var result = brhistory.ToDataSourceResult(request, model => new
            {
                FleetBoloRegistrationHistoryId=model.FleetBoloRegistrationHistoryId,
                FleetBoloRegistrationId = model.FleetBoloRegistrationId,
                FleetId = model.FleetId,
                BoloExpireDate = model.BoloExpireDate,
                BoloRegistrationDate = model.BoloRegistrationDate,
                Remark = model.Remark,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentNameId = model.EquipmentNameId,
                EquipmentTypeId = model.EquipmentTypeId
            });
            return Json(result);
        }
    }
}