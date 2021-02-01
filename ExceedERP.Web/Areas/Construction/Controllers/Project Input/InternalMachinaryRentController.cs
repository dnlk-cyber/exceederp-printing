using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class InternalMachinaryRentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var fleetItems = db.Fleets.Where(x => x.Status == "Active").AsQueryable();
            ViewData["InternalItems"] = fleetItems.Select(x => new { ItemCode = x.FleetsID, Name = x.EquipmentName+"_"+x.SerialNo  }).ToList();
            return View();
        }
        private bool CheckExisatnce(Guid equipmentId)
        {
            var equipment = db.InternalMachinaryRent.Where(x => x.FleetsID == equipmentId).FirstOrDefault();
            if (equipment != null)
            {
                return true;
            }
            return false;
        }
        public ActionResult InternalMachinaryRent_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InternalMachinaryRent> internalmachinaryrents = db.InternalMachinaryRent;
            DataSourceResult result = internalmachinaryrents.ToDataSourceResult(request, internalMachinaryRent => new {
                InternalMachinaryRentId = internalMachinaryRent.InternalMachinaryRentId,
                PurchaseCost = internalMachinaryRent.PurchaseCost,
                MaximumLife = internalMachinaryRent.MaximumLife,
                DepCostPerHour = internalMachinaryRent.DepCostPerHour,
                HourlyRate = internalMachinaryRent.HourlyRate,
                Remark = internalMachinaryRent.Remark,
                FleetsID = internalMachinaryRent.FleetsID,

                Model =  internalMachinaryRent.Model,
                Capacity = internalMachinaryRent.Capacity,
                HoursUsedPerYear = internalMachinaryRent.HoursUsedPerYear,
                InvestmentCost = internalMachinaryRent.InvestmentCost,
                TotalOwningCost = internalMachinaryRent.TotalOwningCost,
                FuelCost = internalMachinaryRent.FuelCost,
                LubCost = internalMachinaryRent.LubCost,
                MaintAndRepair = internalMachinaryRent.MaintAndRepair,
                TyreCost = internalMachinaryRent.TyreCost,
                TotalOperatingCost = internalMachinaryRent.TotalOperatingCost,
                IndexForSeverCondition = internalMachinaryRent.IndexForSeverCondition,
                RatePerHourWithoutFuel = internalMachinaryRent.RatePerHourWithoutFuel
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalMachinaryRent_Create([DataSourceRequest]DataSourceRequest request, InternalMachinaryRent internalMachinaryRent)
        {
            if (ModelState.IsValid)
            {
                if (CheckExisatnce(internalMachinaryRent.FleetsID))
                {

                    ModelState.AddModelError("", "This Equipment is already registered!!");
                }
                else
                {
                    var entity = new InternalMachinaryRent
                    {
                        PurchaseCost = GetPurchaseCost(internalMachinaryRent.FleetsID),
                        MaximumLife = internalMachinaryRent.MaximumLife,
                        //DepCostPerHour = internalMachinaryRent.DepCostPerHour,
                        Remark = internalMachinaryRent.Remark,
                        FleetsID = internalMachinaryRent.FleetsID,

                        Model = this.GetFleets(internalMachinaryRent.FleetsID).Model,
                        Capacity = this.GetFleets(internalMachinaryRent.FleetsID).Capacity,
                        HoursUsedPerYear = internalMachinaryRent.HoursUsedPerYear,
                        InvestmentCost = internalMachinaryRent.InvestmentCost,
                        //TotalOwningCost = internalMachinaryRent.DepCostPerHour + internalMachinaryRent.InvestmentCost,
                        FuelCost = internalMachinaryRent.FuelCost,
                        LubCost = internalMachinaryRent.LubCost,
                        MaintAndRepair = internalMachinaryRent.MaintAndRepair,
                        TyreCost = internalMachinaryRent.TyreCost,
                        TotalOperatingCost = internalMachinaryRent.FuelCost +
                                             internalMachinaryRent.LubCost +
                                             internalMachinaryRent.MaintAndRepair +
                                             internalMachinaryRent.TyreCost,
                        //HourlyRate = internalMachinaryRent.FuelCost +
                        //                     internalMachinaryRent.LubCost +
                        //                     internalMachinaryRent.MaintAndRepair +
                        //                     internalMachinaryRent.TyreCost +
                        //                     internalMachinaryRent.DepCostPerHour + 
                        //                     internalMachinaryRent.InvestmentCost,
                    };
                    entity.DepCostPerHour = entity.PurchaseCost / (entity.MaximumLife * entity.HoursUsedPerYear);
                    entity.TotalOwningCost = entity.DepCostPerHour + entity.InvestmentCost;
                    entity.HourlyRate = entity.FuelCost +
                                             entity.LubCost +
                                             entity.MaintAndRepair +
                                             entity.TyreCost +
                                             entity.DepCostPerHour +
                                             entity.InvestmentCost;

                    entity.IndexForSeverCondition = entity.HourlyRate;
                    entity.RatePerHourWithoutFuel = entity.HourlyRate - entity.FuelCost;

                    db.InternalMachinaryRent.Add(entity);
                    db.SaveChanges();
                    internalMachinaryRent.InternalMachinaryRentId = entity.InternalMachinaryRentId;
                }
            }

            return Json(new[] { internalMachinaryRent }.ToDataSourceResult(request, ModelState));
        }
        private decimal GetPurchaseCost(Guid id)
        {
            var equpmnet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
            if (equpmnet !=null)
            {
                return equpmnet.PurchasingCost;
            }
            return 0;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalMachinaryRent_Update([DataSourceRequest]DataSourceRequest request, InternalMachinaryRent internalMachinaryRent)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalMachinaryRent
                {
                    InternalMachinaryRentId = internalMachinaryRent.InternalMachinaryRentId,
                    PurchaseCost = GetPurchaseCost(internalMachinaryRent.FleetsID),
                    MaximumLife = internalMachinaryRent.MaximumLife,
                    //DepCostPerHour = internalMachinaryRent.DepCostPerHour,
                    Remark = internalMachinaryRent.Remark,
                    FleetsID = internalMachinaryRent.FleetsID,

                    Model = this.GetFleets(internalMachinaryRent.FleetsID).Model,
                    Capacity = this.GetFleets(internalMachinaryRent.FleetsID).Capacity,
                    HoursUsedPerYear = internalMachinaryRent.HoursUsedPerYear,
                    InvestmentCost = internalMachinaryRent.InvestmentCost,
                    //TotalOwningCost = internalMachinaryRent.DepCostPerHour + internalMachinaryRent.InvestmentCost,
                    FuelCost = internalMachinaryRent.FuelCost,
                    LubCost = internalMachinaryRent.LubCost,
                    MaintAndRepair = internalMachinaryRent.MaintAndRepair,
                    TyreCost = internalMachinaryRent.TyreCost,
                    TotalOperatingCost = internalMachinaryRent.FuelCost +
                                             internalMachinaryRent.LubCost +
                                             internalMachinaryRent.MaintAndRepair +
                                             internalMachinaryRent.TyreCost,
                    //HourlyRate = internalMachinaryRent.FuelCost +
                    //                         internalMachinaryRent.LubCost +
                    //                         internalMachinaryRent.MaintAndRepair +
                    //                         internalMachinaryRent.TyreCost +
                    //                         internalMachinaryRent.DepCostPerHour +
                    //                         internalMachinaryRent.InvestmentCost,
                };
                entity.DepCostPerHour = entity.PurchaseCost / (entity.MaximumLife * entity.HoursUsedPerYear);
                entity.TotalOwningCost = entity.DepCostPerHour + entity.InvestmentCost;
                entity.HourlyRate = entity.FuelCost +
                                         entity.LubCost +
                                         entity.MaintAndRepair +
                                         entity.TyreCost +
                                         entity.DepCostPerHour +
                                         entity.InvestmentCost;

                entity.IndexForSeverCondition = entity.HourlyRate;
                entity.RatePerHourWithoutFuel = entity.HourlyRate - entity.FuelCost;

                db.InternalMachinaryRent.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalMachinaryRent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalMachinaryRent_Destroy([DataSourceRequest]DataSourceRequest request, InternalMachinaryRent internalMachinaryRent)
        {
            if (ModelState.IsValid)
            {
              
                    var entity = new InternalMachinaryRent
                    {
                        InternalMachinaryRentId = internalMachinaryRent.InternalMachinaryRentId,
                        PurchaseCost = internalMachinaryRent.PurchaseCost,
                        MaximumLife = internalMachinaryRent.MaximumLife,
                        DepCostPerHour = internalMachinaryRent.DepCostPerHour,
                        HourlyRate = internalMachinaryRent.HourlyRate,
                        Remark = internalMachinaryRent.Remark,
                        FleetsID = internalMachinaryRent.FleetsID,

                        Model = internalMachinaryRent.Model,
                        Capacity = internalMachinaryRent.Capacity,
                        HoursUsedPerYear = internalMachinaryRent.HoursUsedPerYear,
                        InvestmentCost = internalMachinaryRent.InvestmentCost,
                        TotalOwningCost = internalMachinaryRent.TotalOwningCost,
                        FuelCost = internalMachinaryRent.FuelCost,
                        LubCost = internalMachinaryRent.LubCost,
                        MaintAndRepair = internalMachinaryRent.MaintAndRepair,
                        TyreCost = internalMachinaryRent.TyreCost,
                        TotalOperatingCost = internalMachinaryRent.TotalOperatingCost,
                        IndexForSeverCondition = internalMachinaryRent.IndexForSeverCondition,
                        RatePerHourWithoutFuel = internalMachinaryRent.RatePerHourWithoutFuel
                    };

                    db.InternalMachinaryRent.Attach(entity);
                    db.InternalMachinaryRent.Remove(entity);
                    db.SaveChanges();
                
            }

            return Json(new[] { internalMachinaryRent }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private Fleets GetFleets(Guid id)
        {
            Fleets fleet = new Fleets();
            var temp = db.Fleets.Find(id);
            if(temp != null)
            {
                fleet = temp;
            }
            return fleet;
        }
    }
}
