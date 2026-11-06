using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetEquipmentRent
{
    public class RentedEquipmentAdvancePaymentController : Controller
    {
        // GET: FleetManagement/RentedEquipmentAdvancePayment
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var advancePayments = db.RentalEquipmentAdvancePayment.Where(x => x.RentFromExternalEquipmentId == id);
            DataSourceResult result = advancePayments.ToDataSourceResult(request, x => new
            {
                RentalEquipmentAdvancePaymentId=x.RentalEquipmentAdvancePaymentId,
                RentFromExternalEquipmentId=x.RentFromExternalEquipmentId,
                AdvancePayed=x.AdvancePayed,
                AdvanceWithTax = x.AdvanceWithTax,
                VAT = x.VAT,
                Tot = x.Tot,
                DateFrom=x.DateFrom,
                DateTo=x.DateTo,
                IsSummaryCalculated=x.IsSummaryCalculated,
                IsCreatedAfterUpdate = x.IsCreatedAfterUpdate,
                CostSheetAmount=x.CostSheetAmount,
                IsFullPayment=x.IsFullPayment,
                FleetId = x.FleetId,
                CreatedBy = x.CreatedBy,
                DateCreated = x.DateCreated,
                LastModified =x.LastModified,
                ModifiedBy= x.ModifiedBy,
                IsAdvancePayed=checkIfPayed(x.RentalEquipmentAdvancePaymentId)
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private bool checkIfPayed(int id)
        {
            var isPayed = false;
            var advancePayement = db.RentalAdvancePaymentRequest.FirstOrDefault(x => x.AdvancePaymentId == id);
            if(advancePayement!=null)
            {
                isPayed = true;
            }
            return isPayed;

        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, RentalEquipmentAdvancePayment advancePayment, int id)
        {
            //var existing = db.RentalEquipmentAdvancePayment.Where(x => x.RentFromExternalEquipmentId == id).OrderByDescending(x => x.RentalEquipmentAdvancePaymentId).FirstOrDefault();
            //    if(existing!=null)
            //{
            //    if(existing.IsFullPayment)
            //    {
            //        if(existing.CostSheetAmount<existing.AdvancePayed)
            //        {
            //            ModelState.AddModelError("not completed", "Previous agreement is not completed");
            //        }
            //    }
            //    if(!existing.IsFullPayment)
            //    {
            //        ModelState.AddModelError("not completed", "Advance payment is already payed for this equipment");
            //    }
            //}
            var equipment = db.RentFromExternalEquipments.FirstOrDefault(x => x.RentFromExternalEquipmentId == id);
            if (advancePayment.IsFullPayment)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(x =>
                    x.DateFrom <= advancePayment.DateFrom &&
                    x.DateTo >= advancePayment.DateTo
                );
                if(glPeriod==null)
                {
                    ModelState.AddModelError("out of range", "The selected dates are not with in the same period");
                }
                var existingAdvance = db.RentalEquipmentAdvancePayment.FirstOrDefault(x => x.FleetId == equipment.FleetId.ToString() && x.DateTo > advancePayment.DateFrom);
                if (existingAdvance != null)
                {
                    ModelState.AddModelError("date mismuch", "You have advance payed whose date to is after the selected date from");
                }
            }

            

                if(ModelState.IsValid)
            {
                var entity = new RentalEquipmentAdvancePayment()
                {
                    RentFromExternalEquipmentId = id,
                    FleetId = equipment.FleetId.ToString(),
                    AdvanceWithTax = advancePayment.AdvancePayed * equipment.Vat + advancePayment.AdvancePayed,
                    DateFrom = advancePayment.DateFrom,
                    DateTo = advancePayment.DateTo,
                    AdvancePayed = advancePayment.AdvancePayed,
                    VAT = advancePayment.AdvancePayed*equipment.Vat,
                    //Tot = advancePayment.AdvancePayed*equipment.Tot,
                    IsFullPayment = advancePayment.IsFullPayment,
                    IsCreatedAfterUpdate = true,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                };
                db.RentalEquipmentAdvancePayment.Add(entity);
                db.SaveChanges();
            }
            return Json(new[] { advancePayment }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, RentalEquipmentAdvancePayment advancePayment)
        {
            var existing = db.RentalEquipmentAdvancePayment.FirstOrDefault(x => x.RentalEquipmentAdvancePaymentId == advancePayment.RentalEquipmentAdvancePaymentId&&!x.IsSummaryCalculated);
            if(existing==null)
            {
                    ModelState.AddModelError("summary calculated", "Timesheet summary is already generated you can not update this");
                
            }
            var equipment = db.RentFromExternalEquipments.FirstOrDefault(x => x.RentFromExternalEquipmentId == advancePayment.RentFromExternalEquipmentId);
            if (advancePayment.IsFullPayment)
            {
                var glPeriod = db.GLPeriods.FirstOrDefault(x =>
                    x.DateFrom <= advancePayment.DateFrom &&
                    x.DateTo >= advancePayment.DateTo
                );
                if (glPeriod == null)
                {
                    ModelState.AddModelError("out of range", "The selected dates are not with in the same period");
                }
                var existingAdvance = db.RentalEquipmentAdvancePayment.FirstOrDefault(x => x.FleetId == equipment.FleetId.ToString() && x.DateTo > advancePayment.DateFrom);
                if (existingAdvance != null)
                {
                    ModelState.AddModelError("date mismuch", "You have advance payed whose date to is after the selected date from");
                }
            }
            if (ModelState.IsValid)
            {
                    existing.AdvancePayed = advancePayment.AdvancePayed;
                    existing.FleetId = equipment.FleetId.ToString();
                    existing.DateFrom = advancePayment.DateFrom;
                    existing.DateTo = advancePayment.DateTo;
                    existing.IsFullPayment = advancePayment.IsFullPayment;
                    existing.ModifiedBy = User.Identity.Name;
                    existing.DateCreated = DateTime.Now;
                db.Entry(existing).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { advancePayment }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, RentalEquipmentAdvancePayment advancePayment)
        {
            var existing = db.RentalEquipmentAdvancePayment.FirstOrDefault(x => x.RentalEquipmentAdvancePaymentId == advancePayment.RentalEquipmentAdvancePaymentId && !x.IsSummaryCalculated);
            if (existing == null)
            {
                ModelState.AddModelError("summary calculated", "Timesheet summary exists you can not update this");

            }
            ModelState.Remove("AdvanceWithTax");
                if(ModelState.IsValid)
            {
                db.RentalEquipmentAdvancePayment.Remove(existing);
                db.SaveChanges();
            }
            return Json(new[] { advancePayment }.ToDataSourceResult(request, ModelState));

        }
    }
}