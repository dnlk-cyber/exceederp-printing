using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class RequestToInspectionController : Controller
    {
        // GET: FleetManagement/RequestToInspection
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult RequestToInspection_Create([DataSourceRequest] DataSourceRequest request, RequestToInspection model)
        {
            if (ModelState.IsValid)
            {
                var newModel = new RequestToInspection();
                newModel.RequestToInspectionId = newModel.RequestToInspectionId;
                newModel.Remark = model.Remark;
                newModel.GoToInspection = model.GoToInspection;
                db.RequestToInspections.Add(newModel);
                db.SaveChanges();

            }
            //return Json(new[] { model }.t);
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RequestToInspection_Read([DataSourceRequest] DataSourceRequest request)
        {
            var requestToIn = db.RequestToInspections.ToList();
            DataSourceResult Result = requestToIn.ToDataSourceResult(request, x => new
            {
                RequestToInspectionId = x.RequestToInspectionId,
                Remark = x.Remark,
                GoToInspection = x.GoToInspection
            });
            return Json(Result);
        }
        public ActionResult RequestToInspection_Update([DataSourceRequest] DataSourceRequest request, RequestToInspection model)
        {
            var newModel = db.RequestToInspections.FirstOrDefault(x => x.RequestToInspectionId == model.RequestToInspectionId);
            if (ModelState.IsValid)
            {
                newModel.Remark = model.Remark;
                newModel.GoToInspection = model.GoToInspection;
                db.Entry(newModel).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult RequestToInspection_Destroy([DataSourceRequest] DataSourceRequest request, RequestToInspection model)
        {
            var newModel = db.RequestToInspections.FirstOrDefault(x => x.RequestToInspectionId == model.RequestToInspectionId);
            if (ModelState.IsValid)
            {

                db.RequestToInspections.Remove(newModel);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));

        }

    }
}