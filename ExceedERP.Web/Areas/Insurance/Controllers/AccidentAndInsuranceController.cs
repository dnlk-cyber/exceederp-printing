using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
    public class AccidentAndInsuranceController : Controller
    {
        // GET: HRM/AccidentAndInsurance
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Accident_Read([DataSourceRequest] DataSourceRequest request)
        {
            var accident = db.Accident.Where(x => x.IsSendForApproval);
            var accidents = accident.Select(a => new Accident
            {
                FleetsID = a.FleetsID,
                Policereport = a.Fleet.PlateNumber,
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
                CreatedBy = a.CreatedBy

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
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    CreatedBy = Accident.CreatedBy
                };
                db.Accident.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //_Accidentervice.Update(Accident);
            }

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
                db.Accident.Attach(entity);
                db.Accident.Remove(entity);
                db.SaveChanges();

                //_Accidentervice.Update(Accident);
            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }
    }
}