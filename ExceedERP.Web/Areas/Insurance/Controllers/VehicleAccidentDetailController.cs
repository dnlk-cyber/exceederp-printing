﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    public class VehicleAccidentDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult VehicleAccidentDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    IQueryable<VehicleAccidentDetail> vehicleaccidentdetail = db.VehicleAccidentDetails.Where( x=> x.FixedAssetAccidentRecordId==id);
        //    DataSourceResult result = vehicleaccidentdetail.ToDataSourceResult(request, vehicleAccidentDetail => new {
        //        VehicleAccidentDetailId = vehicleAccidentDetail.VehicleAccidentDetailId,
        //        AccidentType = vehicleAccidentDetail.AccidentType,
        //        IsThirdPartAccident = vehicleAccidentDetail.IsThirdPartAccident,
        //        AccidentLevel = vehicleAccidentDetail.AccidentLevel,
        //        Remark = vehicleAccidentDetail.Remark
        //    });

        //    return Json(result);
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult VehicleAccidentDetail_Create([DataSourceRequest]DataSourceRequest request, VehicleAccidentDetail vehicleAccidentDetail, int idTwo)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new VehicleAccidentDetail
        //        {
        //            AccidentType = vehicleAccidentDetail.AccidentType,
        //            IsThirdPartAccident = vehicleAccidentDetail.IsThirdPartAccident,
        //            AccidentLevel = vehicleAccidentDetail.AccidentLevel,
        //            FixedAssetAccidentRecordId = idTwo,
        //            Remark = vehicleAccidentDetail.Remark
        //        };

        //        db.VehicleAccidentDetails.Add(entity);
        //        db.SaveChanges();
        //        vehicleAccidentDetail.VehicleAccidentDetailId = entity.VehicleAccidentDetailId;
        //    }

        //    return Json(new[] { vehicleAccidentDetail }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult VehicleAccidentDetail_Update([DataSourceRequest]DataSourceRequest request, VehicleAccidentDetail vehicleAccidentDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new VehicleAccidentDetail
        //        {
        //            VehicleAccidentDetailId = vehicleAccidentDetail.VehicleAccidentDetailId,
        //            AccidentType = vehicleAccidentDetail.AccidentType,
        //            IsThirdPartAccident = vehicleAccidentDetail.IsThirdPartAccident,
        //            AccidentLevel = vehicleAccidentDetail.AccidentLevel,
        //            Remark = vehicleAccidentDetail.Remark
        //        };

        //        db.VehicleAccidentDetails.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { vehicleAccidentDetail }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult VehicleAccidentDetail_Destroy([DataSourceRequest]DataSourceRequest request, VehicleAccidentDetail vehicleAccidentDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new VehicleAccidentDetail
        //        {
        //            VehicleAccidentDetailId = vehicleAccidentDetail.VehicleAccidentDetailId,
        //            AccidentType = vehicleAccidentDetail.AccidentType,
        //            IsThirdPartAccident = vehicleAccidentDetail.IsThirdPartAccident,
        //            AccidentLevel = vehicleAccidentDetail.AccidentLevel,
        //            Remark = vehicleAccidentDetail.Remark
        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Vehicle Accident Detail Recored " + vehicleAccidentDetail.FixedAssetAccidentRecordId);
        //        #endregion
        //        db.VehicleAccidentDetails.Attach(entity);
        //        db.VehicleAccidentDetails.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { vehicleAccidentDetail }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
