using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetResources
{
    public class AssignedEquipmentAdditionalInfoController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AssignedEquipmentAdditionalInfo
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssignedEquipAdditionalInfo_Read([DataSourceRequest] DataSourceRequest request, Guid assignedEquipmentId)
        {
            var assignedEquipAddInfos = db.AssignedEquipmentAdditionalInfos.Where(x => x.AssignedEquipmentId == assignedEquipmentId);
            var result = assignedEquipAddInfos.ToDataSourceResult(request, model => new AssignedEquipmentAdditionalInfo
            {

                AssignedEquipmentAdditionalInfoId = model.AssignedEquipmentAdditionalInfoId,
                FromDeppot = model.FromDeppot,
                FromProject = model.FromProject,
                GreaseReq = model.GreaseReq,
                InCash = model.InCash,
                KmGrandTotal = model.KmGrandTotal,
                KmInterSite = model.KmInterSite,
                KmTravelledPerLitre = model.KmTravelledPerLitre,
                KmTravelledTotal = model.KmTravelledTotal,
                OilReqForEngine = model.OilReqForEngine,
                OilReqForOther = model.OilReqForOther,
                TotalBirr = model.TotalBirr,
                TotalInLitre = model.TotalInLitre,
                PlateNo = model.PlateNo,
                AssignedEquipmentId = model.AssignedEquipmentId,
                ReqFuelInLiter = model.ReqFuelInLiter,
                TotalPerdium = model.TotalPerdium,
                FuelRate = model.FuelRate,
                InCashLiter = model.InCashLiter
            });
            return Json(result);
        }
        public ActionResult AssignedEquipAdditionalInfo_Create([DataSourceRequest] DataSourceRequest request, AssignedEquipmentAdditionalInfo model, Guid asEquipId)
        {
            if (ModelState.IsValid)
            {
                //var assignedEquip = db.AssignedEquipment.FirstOrDefault(x => x.AssignedEquipmentID == asEquipId);
                //var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == assignedEquip.FleetId);
                model.AssignedEquipmentId = asEquipId;
                //model.KmTravelledPerLitre = (float)fleet.MinFuelStandard;
                db.AssignedEquipmentAdditionalInfos.Add(model);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult AssignedEquipAdditionalInfo_Update([DataSourceRequest] DataSourceRequest request, AssignedEquipmentAdditionalInfo model)
        {
            if (ModelState.IsValid)
            {
                db.AssignedEquipmentAdditionalInfos.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult AssignedEquipAdditionalInfo_Delete([DataSourceRequest] DataSourceRequest request, AssignedEquipmentAdditionalInfo model)
        {
            if (ModelState.IsValid)
            {
                var assignedEquipAddInfo = db.AssignedEquipmentAdditionalInfos.FirstOrDefault(x => x.AssignedEquipmentAdditionalInfoId == model.AssignedEquipmentAdditionalInfoId);
                if(assignedEquipAddInfo != null)
                {
                    db.AssignedEquipmentAdditionalInfos.Remove(assignedEquipAddInfo);
                    db.SaveChanges();
                }
                
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public JsonResult GetAssignedEquipment(Guid assignedEquipmentId)
        {
            if (assignedEquipmentId != null)
            {
                var assignedEquipment = db.AssignedEquipment.Find(assignedEquipmentId);
                if (assignedEquipment != null)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == assignedEquipment.FleetId);
                    if(fleet != null)
                    {
                        var result = new
                        {
                            FuelConsumptionRate = (float)fleet.MaxFuelStandard
                        };
                    return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(null);
        }
    }
}