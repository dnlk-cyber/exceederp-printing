using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class PatientRegisterDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: APTS/PatientRegisterDetail
        public ActionResult Index()
        {
            return View();
        }
        public void PopulateViewData()
        {
            var drug = db.DrugLists.ToList();
            ViewData["drugList"] = drug;
            ViewData["Units"] = db.DrugUnits.ToList();
            ViewData["employees"] = db.Person_Employee.ToList();
        }
        public ActionResult PatientRegisterDetail_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var patientRegisterDetails = db.PatientRegisterDetails.Where(x=>x.PatientRegisterId == id).ToList();
            DataSourceResult result = patientRegisterDetails.ToDataSourceResult(request, patientRegisterDetail => new
            {
                PatientRegisterDetailId = patientRegisterDetail.PatientRegisterDetailId,
                PatientRegisterId = patientRegisterDetail.PatientRegisterId,
                DrugListsId = patientRegisterDetail.DrugListsId,
                TotalQty = patientRegisterDetail.TotalQty,
                RetailPrice = patientRegisterDetail.RetailPrice,
                TotalRetailPrice = patientRegisterDetail.TotalRetailPrice,
                DrugUnitId = patientRegisterDetail.DrugUnitId,
                InternalCode = patientRegisterDetail.InternalCode,
                IsUpdated = patientRegisterDetail.IsUpdated,
            });

            return Json(result);
        }
        public ActionResult PatientRegisterDetail_Create([DataSourceRequest]DataSourceRequest request, PatientRegisterDetail patientRegisterDetail, int id)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
            

                var entity = new PatientRegisterDetail
                {
                    PatientRegisterDetailId = patientRegisterDetail.PatientRegisterDetailId,
                    PatientRegisterId = id,
                    DrugListsId = patientRegisterDetail.DrugListsId,
                    TotalQty = patientRegisterDetail.TotalQty,
                    RetailPrice = patientRegisterDetail.RetailPrice,
                    TotalRetailPrice = patientRegisterDetail.TotalQty * patientRegisterDetail.RetailPrice,
                    DrugUnitId = patientRegisterDetail.DrugUnitId,
                    InternalCode = patientRegisterDetail.InternalCode,
                    IsUpdated = patientRegisterDetail.IsUpdated,
                };
                db.PatientRegisterDetails.Add(entity);
                db.SaveChanges();
                patientRegisterDetail.PatientRegisterDetailId = entity.PatientRegisterDetailId;

                //var despensary = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == entity.DrugListsId && x.DrugUnitId == entity.DrugUnitId);
                //if (despensary != null)
                //{
                //    despensary.Balance = despensary.Balance - entity.TotalQty;

                //    db.Entry(despensary).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
            }

            return Json(new[] { patientRegisterDetail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PatientRegisterDetail_Update([DataSourceRequest]DataSourceRequest request, PatientRegisterDetail patientRegisterDetail)
        {
            if (ModelState.IsValid)
            {
                //var internalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == patientRegister.DrugListsId);

                var entity = new PatientRegisterDetail
                {
                    PatientRegisterDetailId = patientRegisterDetail.PatientRegisterDetailId,
                    PatientRegisterId = patientRegisterDetail.PatientRegisterId,
                    DrugListsId = patientRegisterDetail.DrugListsId,
                    TotalQty = patientRegisterDetail.TotalQty,
                    RetailPrice = patientRegisterDetail.RetailPrice,
                    TotalRetailPrice = patientRegisterDetail.TotalQty * patientRegisterDetail.RetailPrice,
                    DrugUnitId = patientRegisterDetail.DrugUnitId,
                    InternalCode = patientRegisterDetail.InternalCode,
                    IsUpdated = patientRegisterDetail.IsUpdated
                };

                db.PatientRegisterDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { patientRegisterDetail }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PatientRegisterDetail_Destroy([DataSourceRequest]DataSourceRequest request, PatientRegisterDetail patientRegisterDetail)
        {
            if (ModelState.IsValid)
            {
                var internalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == patientRegisterDetail.DrugListsId);

                var entity = new PatientRegisterDetail
                {
                    PatientRegisterDetailId = patientRegisterDetail.PatientRegisterDetailId,
                    PatientRegisterId = patientRegisterDetail.PatientRegisterId,
                    DrugListsId = patientRegisterDetail.DrugListsId,
                    TotalQty = patientRegisterDetail.TotalQty,
                    RetailPrice = patientRegisterDetail.RetailPrice,
                    TotalRetailPrice = patientRegisterDetail.TotalQty * patientRegisterDetail.RetailPrice,
                    DrugUnitId = patientRegisterDetail.DrugUnitId,
                    InternalCode = patientRegisterDetail.InternalCode,
                    IsUpdated = patientRegisterDetail.IsUpdated,
                };

            

                db.PatientRegisterDetails.Attach(entity);
                db.PatientRegisterDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientRegisterDetail
    }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}