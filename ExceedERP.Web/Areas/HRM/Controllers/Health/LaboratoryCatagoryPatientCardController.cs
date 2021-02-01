using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
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

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class LaboratoryCatagoryPatientCardController : Controller
    {
        // GET: HRM/LaboratoryCatagoryPatientCard
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private void PopulateViewData()
        {
            //ViewData["employees"] = db.Person_Employee.ToList();
            //ViewData["drugList"] = db.DrugLists.ToList();
            //ViewData["drugUnit"] = db.DrugUnits.ToList();
            ViewData["LabTests"] = db.LaboratoryTests.ToList();
        }
        public ActionResult LaboratoryCatagoryPatientCard_Read([DataSourceRequest]DataSourceRequest request,int id,int id2)
        {
            var laboratoryCatagories = db.LaboratoryCatagoryPatientCards.Where(x=>x.PatientId == id && x.PatientCardDetailId == id2).ToList();
            DataSourceResult result = laboratoryCatagories.ToDataSourceResult(request, laboratoryCatagoryPatientCard => new
            {
                LaboratoryCatagoryPatientCardId = laboratoryCatagoryPatientCard.LaboratoryCatagoryPatientCardId,
                PatientCardDetailId = laboratoryCatagoryPatientCard.PatientCardDetailId,
                PatientId = laboratoryCatagoryPatientCard.PatientId,
                Catagory = laboratoryCatagoryPatientCard.Catagory

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryCatagoryPatientCard_Create([DataSourceRequest]DataSourceRequest request, LaboratoryCatagoryPatientCard laboratoryCatagoryPatientCard, int id, int id2)
        {

            if (ModelState.IsValid)
            {
                var entity = new LaboratoryCatagoryPatientCard
                {
                    PatientCardDetailId = id2,
                    PatientId = id,
                    Catagory = laboratoryCatagoryPatientCard.Catagory
                };

                db.LaboratoryCatagoryPatientCards.Add(entity);
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryCatagoryPatientCard", "Success", User.Identity.Name, User.Identity.Name + " add new Laboratory Catagory in patient card called " + entity.Catagory + " at date " + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { laboratoryCatagoryPatientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryCatagoryPatientCard_Update([DataSourceRequest]DataSourceRequest request, LaboratoryCatagoryPatientCard laboratoryCatagoryPatientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryCatagoryPatientCard
                {
                    LaboratoryCatagoryPatientCardId = laboratoryCatagoryPatientCard.LaboratoryCatagoryPatientCardId,                 
                    PatientCardDetailId = laboratoryCatagoryPatientCard.PatientCardDetailId,
                    PatientId = laboratoryCatagoryPatientCard.PatientId,
                    Catagory = laboratoryCatagoryPatientCard.Catagory
                };


                db.LaboratoryCatagoryPatientCards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryCatagoryPatientCard", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory Catagory in patient card called " + entity.Catagory + " at date " + DateTime.Today, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { laboratoryCatagoryPatientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryCatagoryPatientCard_Destroy([DataSourceRequest]DataSourceRequest request, LaboratoryCatagoryPatientCard laboratoryCatagoryPatientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryCatagoryPatientCard
                {
                    LaboratoryCatagoryPatientCardId = laboratoryCatagoryPatientCard.LaboratoryCatagoryPatientCardId,
                    PatientCardDetailId = laboratoryCatagoryPatientCard.PatientCardDetailId,
                    PatientId = laboratoryCatagoryPatientCard.PatientId,
                    Catagory = laboratoryCatagoryPatientCard.Catagory
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Laboratory catagory at patient card called " + entity.Catagory);
                #endregion
                db.LaboratoryCatagoryPatientCards.Attach(entity);
                db.LaboratoryCatagoryPatientCards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { laboratoryCatagoryPatientCard }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}