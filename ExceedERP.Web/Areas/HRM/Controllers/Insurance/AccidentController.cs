using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
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

namespace ExceedERP.Web.Areas.HRM.Controllers
{
    public class AccidentController : Controller
    {
        // GET: HRM/Accident
        ExceedDbContext db = new ExceedDbContext();
        public static DateTime RenewalDate { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            ViewData["Branches"] = db.Accident.ToList();

        }
        public JsonResult GetEquipment()
        {
            List<Fleets> fleetList = new List<Fleets>();
            var accidents = db.Accident.Where(x => x.IsSendForApproval).ToList();
            foreach (var accident in accidents)
            {
                var fleet = db.Fleets.Find(accident.FleetsID);
                fleetList.Add(fleet);
            }
            return Json(fleetList.Select(p => new
            {
                FleetsID = p.FleetsID,
                EquipmentName = p.EquipmentName,
            }), JsonRequestBehavior.AllowGet);
        }
        public string GetModel(Guid fleetId)
        {
            string model = "";
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                model = fleet.Model;
            }
            return model;
        }
        public string GetSerial(Guid fleetId)
        {
            string serial = "";
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                serial = fleet.SerialNo;
            }
            return serial;
        }
        public string GetEngine(Guid fleetId)
        {
            string engine = "";
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                engine = fleet.EngineNumber; 
            }
            return engine;
        }
        public decimal GetPurchaseValue(Guid fleetId)
        {
            decimal purchase = 0;
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                purchase = fleet.PurchasingCost; 
            }
            return purchase;
        }
        public string GetManufacureYear(Guid fleetId)
        {
            string year = "";
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                year = fleet.ManufacturerYear; 
            }
            return year;
        }
        public JsonResult Accident_Read([DataSourceRequest] DataSourceRequest request)
        {
            var accident = db.Accident.Where(x => x.IsSendForApproval).ToList();
            var accidents = accident.Select(a => new Accident
            {

                FleetsID = a.FleetsID,
                Insured = a.Insured,
                FullName = a.FullName,
                Address = a.Address,
                Occupation = a.Occupation,
                IsHornHeared = a.IsHornHeared,
                PersonWasAtAccident = a.PersonWasAtAccident,
                ThirdPartyDamage = a.ThirdPartyDamage,
                PhoneNo = a.PhoneNo,
                PolicyNo = a.PolicyNo,
                RenewalDate = a.RenewalDate,
                RenewalDateAmh = a.RenewalDateAmh,
                DoThePoliceRecoredIt = a.DoThePoliceRecoredIt,
                DoYouHaveMoreThanOneInsurance = a.DoYouHaveMoreThanOneInsurance,
                DoYouHaveOtherInsuranceCanEarnForTheAccident = a.DoYouHaveOtherInsuranceCanEarnForTheAccident,
                //Equipment = a.Fleet.UnitNum+" - " + a.Fleet.PlateNumber,
                AccidentLocation = a.AccidentLocation,
                Accountablebody = a.Accountablebody,
                CauseOfAccident = a.CauseOfAccident,
                CostcoveredbyCompany = a.CostcoveredbyCompany,
                DateOfOccurrence = a.DateOfOccurrence,
                CostcoveredbyInsurance = a.CostcoveredbyInsurance,
                CostcoveredbyOperator = a.CostcoveredbyOperator,
                CostcoveredbyOtherBodies = a.CostcoveredbyOtherBodies,
                Description = a.Description,
                LevelOfDamage = a.LevelOfDamage,
                LicenseNo = a.LicenseNo,
                AccidentID = a.AccidentID,
                DateCreated = a.DateCreated,
                LastModified = a.LastModified,
                ModifiedBy = a.ModifiedBy,
                Equipment = a.Equipment,
                VehicleSpeed = a.VehicleSpeed,
                CurrentValue = a.CurrentValue,
                VehicleDistanceFromRoadEdge = a.VehicleDistanceFromRoadEdge,
                InjuredPersons = a.InjuredPersons,
                WittnessDescription = a.WittnessDescription,
                AssetDamage = a.AssetDamage,
                WitnessInYourCar = a.WitnessInYourCar,
                ManufacturedYear = GetManufacureYear(a.FleetsID),
                PurchaseCost = GetPurchaseValue(a.FleetsID),
                Model = GetModel(a.FleetsID),
                EngineNo = GetEngine(a.FleetsID),
                PlateNo = a.PlateNo,
                PolicyIdNo = a.PolicyIdNo,
                SerialNo = GetSerial(a.FleetsID)

            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult Accident_Create([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {

            ModelState.Remove("AccidentID");
            Accident.AccidentID = Guid.NewGuid();
            Accident.CreatedBy = User.Identity.Name;
            Accident.DateCreated = DateTime.Now;


            if (Accident != null)
            {
                //_Accidentervice.Insert(Accident);
            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Accident_Update([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {

            var entity = db.Accident.FirstOrDefault(x => x.AccidentID == Accident.AccidentID);
            if (entity != null)
            {
                entity.AccidentID = Accident.AccidentID;
                entity.FleetsID = Accident.FleetsID;
                entity.Insured = Accident.Insured;
                entity.FullName = Accident.FullName;
                entity.Address = Accident.Address;
                entity.Occupation = Accident.Occupation;
                entity.PhoneNo = Accident.PhoneNo;
                entity.PolicyNo = Accident.PolicyNo;
                entity.RenewalDate = Accident.RenewalDate;//RenewalDate,
                entity.RenewalDateAmh = Accident.RenewalDateAmh;
                entity.DoThePoliceRecoredIt = Accident.DoThePoliceRecoredIt;
                entity.DoYouHaveMoreThanOneInsurance = Accident.DoYouHaveMoreThanOneInsurance;
                entity.DoYouHaveOtherInsuranceCanEarnForTheAccident = Accident.DoYouHaveOtherInsuranceCanEarnForTheAccident;
                var plate = db.Fleets.Find(Accident.FleetsID);
                if (plate != null)
                {
                    entity.Policereport = plate.PlateNumber;
                }

                entity.AccidentLocation = Accident.AccidentLocation;
                entity.Accountablebody = Accident.Accountablebody;
                entity.CauseOfAccident = Accident.CauseOfAccident;
                entity.CostcoveredbyCompany = Accident.CostcoveredbyCompany;
                entity.DateOfOccurrence = DateTime.Now.Date;//Accident.DateOfOccurrence,
                entity.CostcoveredbyInsurance = Accident.CostcoveredbyInsurance;
                entity.CostcoveredbyOperator = Accident.CostcoveredbyOperator;
                entity.CostcoveredbyOtherBodies = Accident.CostcoveredbyOtherBodies;
                entity.Description = Accident.Description;
                entity.LevelOfDamage = Accident.LevelOfDamage;
                entity.LicenseNo = Accident.LicenseNo;
                entity.DateCreated = Accident.DateCreated;
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                entity.CreatedBy = Accident.CreatedBy;
                entity.CurrentValue = Accident.CurrentValue;
                entity.VehicleDistanceFromRoadEdge = Accident.VehicleDistanceFromRoadEdge;
                entity.VehicleSpeed = Accident.VehicleSpeed;
                entity.InjuredPersons = Accident.InjuredPersons;
                entity.CostcoveredbyOtherBodies = Accident.CostcoveredbyOtherBodies;
                entity.WittnessDescription = Accident.WittnessDescription;
                entity.AssetDamage = Accident.AssetDamage;
                entity.WitnessInYourCar = Accident.WitnessInYourCar;

                // };
                //db.Accident.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            //ModelState.Clear();
            //if (ModelState.IsValid)
            //{
            //    if(Accident.RenewalDateAmh != null)
            //    {
            //        var splitDate = Accident.RenewalDateAmh.Split('/');
            //        RenewalDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            //    }

            //    //var entity = new Accident();
            //    // {


            //    //_Accidentervice.Update(Accident);
            //}

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Accident_Destroy([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {

            if (ModelState.IsValid)
            {
                var entity = new Accident
                {
                    FleetsID = Accident.FleetsID,
                    Policereport = Accident.Fleet.PlateNumber,
                    AccidentLocation = Accident.AccidentLocation,
                    Accountablebody = Accident.Accountablebody,
                    CauseOfAccident = Accident.CauseOfAccident,
                    CostcoveredbyCompany = Accident.CostcoveredbyCompany,
                    DateOfOccurrence = Accident.DateOfOccurrence,
                    CostcoveredbyInsurance = Accident.CostcoveredbyInsurance,
                    CostcoveredbyOperator = Accident.CostcoveredbyOperator,
                    CostcoveredbyOtherBodies = Accident.CostcoveredbyOtherBodies,
                    Description = Accident.Description,
                    LevelOfDamage = Accident.LevelOfDamage,
                    LicenseNo = Accident.LicenseNo,
                    AccidentID = Accident.AccidentID,
                    DateCreated = Accident.DateCreated,
                    LastModified = Accident.LastModified,
                    ModifiedBy = User.Identity.Name,
                    CreatedBy = Accident.CreatedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Accident Recored " + Accident.PlateNo);
                #endregion
                db.Accident.Attach(entity);
                db.Accident.Remove(entity);
                db.SaveChanges();

                //_Accidentervice.Update(Accident);
            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }
    }
}