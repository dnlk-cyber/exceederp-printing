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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetRegistration
{
    public class BoloRegistrationHistoryController : Controller
    {
        // GET: FleetManagement/BoloRegistrationHistory
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
            ViewData["Fleet"] = CacheData.GetFleetsSecondCache().Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).AsQueryable();

            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var brhistory = db.FleetBoloRegistrationsHistorys.AsQueryable() ;
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