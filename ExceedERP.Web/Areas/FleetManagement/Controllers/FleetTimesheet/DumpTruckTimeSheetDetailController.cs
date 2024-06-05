using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    public class DumpTruckTimeSheetDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/DumpTruckTimeSheetDetail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DumpTruckTimeSheetsDetail_Read([DataSourceRequest] DataSourceRequest request, int dumpTruckTimesheetId)
        {
            var dumpTruckTimeSheetDetails = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == dumpTruckTimesheetId);

            DataSourceResult result = dumpTruckTimeSheetDetails.ToDataSourceResult(request, model => new DumpTruckTimeSheetDetail
            {
                DumpTruckTimeSheetDetailId = model.DumpTruckTimeSheetDetailId,
                DumpTruckTimeSheetId = model.DumpTruckTimeSheetId,
                LoadingLocationFrom = model.LoadingLocationFrom,
                LoadingLocationTo = model.LoadingLocationTo,
                NumberOfTrip = model.NumberOfTrip,
                TypeOfWork = model.TypeOfWork,
                TotalHaulage = model.TotalHaulage,
                Capacity = model.Capacity,
                ActualDistance = model.ActualDistance,
                RentalRate = model.RentalRate
                
            });

            return Json(result);
        }

        public ActionResult DumpTruckTimeSheetsDetail_Create([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheetDetail model, int dtId)
        {


            if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheetDetail
                {
                    TypeOfWork = model.TypeOfWork,
                    NumberOfTrip = model.NumberOfTrip,
                    LoadingLocationTo = model.LoadingLocationTo,
                    LoadingLocationFrom = model.LoadingLocationFrom,
                    ActualDistance = model.ActualDistance,
                    Capacity = model.Capacity,
                    RentalRate = model.RentalRate,
                    TotalHaulage = model.NumberOfTrip * model.Capacity,
                };
                entity.DumpTruckTimeSheetId = dtId;
                db.DumpTruckTimeSheetDetails.Add(entity);
                db.SaveChanges();
                model.DumpTruckTimeSheetDetailId = entity.DumpTruckTimeSheetDetailId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet Detail registration", "Success", User.Identity.Name, $"created dump truck timesheet Detail {entity.DumpTruckTimeSheetDetailId} ", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DumpTruckTimeSheetsDetail_Update([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheetDetail model)
        {
            
            if (ModelState.IsValid)
            {
                var dumpTruckDetail = db.DumpTruckTimeSheetDetails.FirstOrDefault(x => x.DumpTruckTimeSheetDetailId == model.DumpTruckTimeSheetDetailId);
                dumpTruckDetail.RentalRate = model.RentalRate;
                dumpTruckDetail.TotalHaulage = model.NumberOfTrip * model.Capacity;
                db.Entry(dumpTruckDetail).State = EntityState.Modified;
                db.SaveChanges();
                
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet Detail Update", "Success", User.Identity.Name, $"updated dump truck timesheet Detail {dumpTruckDetail.DumpTruckTimeSheetDetailId} ", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DumpTruckTimeSheetsDetail_Destroy([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheetDetail model)
        {
            if (ModelState.IsValid)
            {
                var dumpTruckTimeSheetDetail = db.DumpTruckTimeSheetDetails.FirstOrDefault(x => x.DumpTruckTimeSheetDetailId == model.DumpTruckTimeSheetDetailId);
                if(dumpTruckTimeSheetDetail != null)
                {
                    db.DumpTruckTimeSheetDetails.Remove(dumpTruckTimeSheetDetail);
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}