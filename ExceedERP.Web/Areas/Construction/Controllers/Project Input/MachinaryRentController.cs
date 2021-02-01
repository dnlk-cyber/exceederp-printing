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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class MachinaryRentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var items = db.RentOtherSuppliers.AsQueryable();
            if (items.Any())
            {
                var itemsList = items.Select(x => new { ItemCode = x.RentOtherSuppliersID, Name = x.Description }).AsQueryable();
                ViewData["Items"] = itemsList;
                return View();
            }
            ViewData["Items"] = items;
            return View();
        }

        public ActionResult MachinaryRent_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MachinaryRent> machinaryrents = db.MachinaryRent;
            DataSourceResult result = machinaryrents.ToDataSourceResult(request, machinaryRent => new
            {
                MachinaryRentId = machinaryRent.MachinaryRentId,
                //FuelType = machinaryRent.FuelType,
                ItemCode = machinaryRent.ItemCode,
                RatedCapaciy = machinaryRent.RatedCapaciy,
                HP = machinaryRent.HP,
                Model = machinaryRent.Model,
                YearManufacture = machinaryRent.YearManufacture,
                RentalWithFuel = machinaryRent.RentalWithFuel,
                FuelPerHour = machinaryRent.FuelPerHour,
                BirrPerLitter = machinaryRent.BirrPerLitter,
                FleetsID = machinaryRent.FleetsID,
                FuelCostPerHr = machinaryRent.FuelCostPerHr,
                Rental = machinaryRent.Rental,
                ApprovedCost = machinaryRent.ApprovedCost
            });

            return Json(result);
        }
        private bool CheckExisatnce(Guid equipmentId)
        {
            var equipment = db.MachinaryRent.Where(x => x.FleetsID == equipmentId).FirstOrDefault();
            if (equipment != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachinaryRent_Create([DataSourceRequest]DataSourceRequest request, MachinaryRent machinaryRent)
        {
            if (ModelState.IsValid)
            {
                if (CheckExisatnce(machinaryRent.FleetsID))
                {

                    ModelState.AddModelError("", "This Equipment is already registered!!");
                }
                else
                {
                    var entity = new MachinaryRent
                    {
                        //RentOtherSuppliersID = machinaryRent.RentOtherSuppliersID,
                        //FuelType = machinaryRent.FuelType,
                        ItemCode = machinaryRent.ItemCode,
                        RatedCapaciy = machinaryRent.RatedCapaciy,
                        HP = machinaryRent.HP,
                        Model = machinaryRent.Model,
                        YearManufacture = machinaryRent.YearManufacture,
                        RentalWithFuel = machinaryRent.RentalWithFuel,
                        FuelPerHour = machinaryRent.FuelPerHour,
                        BirrPerLitter = GetFuelCostForTheGivenFuel(machinaryRent.ItemCode),
                        FleetsID = machinaryRent.FleetsID,
                        FuelCostPerHr = machinaryRent.FuelCostPerHr,
                        Rental = FleetsHelper.GetEquipmentRentalCost(machinaryRent.FleetsID),
                        ApprovedCost = machinaryRent.ApprovedCost
                    };
                    entity.FuelCostPerHr = entity.FuelPerHour * GetFuelCostForTheGivenFuel(entity.ItemCode);
                    entity.RentalWithFuel = entity.Rental + entity.FuelPerHour * GetFuelCostForTheGivenFuel(entity.ItemCode);
                    entity.ApprovedCost = entity.RentalWithFuel;

                    db.MachinaryRent.Add(entity);
                    db.SaveChanges();
                    machinaryRent.MachinaryRentId = entity.MachinaryRentId;
                }
            }

            return Json(new[] { machinaryRent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachinaryRent_Update([DataSourceRequest]DataSourceRequest request, MachinaryRent machinaryRent)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachinaryRent
                {
                    //FuelType = machinaryRent.FuelType,
                    ItemCode = machinaryRent.ItemCode,
                    RatedCapaciy = machinaryRent.RatedCapaciy,
                    HP = machinaryRent.HP,
                    MachinaryRentId = machinaryRent.MachinaryRentId,
                    Model = machinaryRent.Model,
                    YearManufacture = machinaryRent.YearManufacture,
                    RentalWithFuel = machinaryRent.RentalWithFuel,
                    FuelPerHour = machinaryRent.FuelPerHour,
                    BirrPerLitter = GetFuelCostForTheGivenFuel(machinaryRent.ItemCode),
                    FleetsID = machinaryRent.FleetsID,
                    FuelCostPerHr = machinaryRent.FuelCostPerHr,
                    Rental = FleetsHelper.GetEquipmentRentalCost(machinaryRent.FleetsID),
                    ApprovedCost = machinaryRent.ApprovedCost
                };
                entity.FuelCostPerHr = entity.FuelPerHour * GetFuelCostForTheGivenFuel(entity.ItemCode);
                entity.RentalWithFuel = entity.Rental + entity.FuelPerHour * GetFuelCostForTheGivenFuel(entity.ItemCode);
                db.MachinaryRent.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { machinaryRent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachinaryRent_Destroy([DataSourceRequest]DataSourceRequest request, MachinaryRent machinaryRent)
        {
            if (ModelState.IsValid)
            {
                var entity = new MachinaryRent
                {
                    RatedCapaciy = machinaryRent.RatedCapaciy,
                    HP = machinaryRent.HP,
                    MachinaryRentId = machinaryRent.MachinaryRentId,
                    Model = machinaryRent.Model,
                    YearManufacture = machinaryRent.YearManufacture,
                    RentalWithFuel = machinaryRent.RentalWithFuel,
                    FuelPerHour = machinaryRent.FuelPerHour,
                    BirrPerLitter = machinaryRent.BirrPerLitter,
                    FleetsID = machinaryRent.FleetsID,
                    FuelCostPerHr = machinaryRent.FuelCostPerHr,
                    Rental = machinaryRent.Rental,
                    ApprovedCost = machinaryRent.ApprovedCost
                };

                db.MachinaryRent.Attach(entity);
                db.MachinaryRent.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { machinaryRent }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private string GetCurrentMonth()
        {
            DateTime currentDay = DateTime.Now;
            string month = currentDay.ToString("MMMM");
            return month.Substring(0, 3);
        }
        private decimal GetFuelCostForTheGivenFuel(string itemCode)
        {
            string currentMonth = this.GetCurrentMonth();
            decimal price = new decimal();
            //List<MonthlyFuelCost> list = db.MonthlyFuelCost.ToList();
            //foreach (var item in list)
            //{
            //    var monthString = item.Month.ToString();
            //    if (monthString == currentMonth)
            //    {
            //        if (item.FuelType == fuelType)
            //        {
            //            price = item.Price;
            //        }
            //    }

            //}
            var latestPrice = new MaterialPriceIndex();
            var currentFuelTypes = db.MaterialPriceIndexIndices.Where(item => item.ItemCode == itemCode).ToList();
            if (currentFuelTypes.Any())
            {
                DateTime latestDate = currentFuelTypes.Max(m => m.DateOfInformation);
                latestPrice = currentFuelTypes.FirstOrDefault(itm => itm.DateOfInformation == latestDate);
            }
            price = latestPrice.ApprovedCost;
            //if (price == 0)
            //{

            //    ModelState.AddModelError("error", "fuel cost not set for the current month");

            //}
            return price;
        }
    }
}
