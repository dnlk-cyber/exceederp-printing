using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
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

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class LaboratoryTestPatientCardController : Controller
    {
        // GET: HRM/LaboratoryTestPatientCard
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
        }
        public ActionResult LaboratoryTestPatientCard_Read([DataSourceRequest]DataSourceRequest request, int id,int id2,int id3)
        {
            var tests = db.LaboratoryTestPatientCards.Where(x => x.PatientId == id && x.PatientCardDetailId == id2 && x.LaboratoryCatagoryPatientCardId == id3);
            DataSourceResult result = tests.ToDataSourceResult(request, laboratoryTestPatientCard => new
            {
                LaboratoryTestPatientCardId = laboratoryTestPatientCard.LaboratoryTestPatientCardId,
                LaboratoryCatagoryPatientCardId = laboratoryTestPatientCard.LaboratoryCatagoryPatientCardId,
                PatientCardDetailId = laboratoryTestPatientCard.PatientCardDetailId,
                PatientId = laboratoryTestPatientCard.PatientId,
                Reference = laboratoryTestPatientCard.Reference,
                Result = laboratoryTestPatientCard.Result,
                Test = laboratoryTestPatientCard.Test,
                IsSentBackToDoctor = laboratoryTestPatientCard.IsSentBackToDoctor
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTestPatientCard_Create([DataSourceRequest]DataSourceRequest request, LaboratoryTestPatientCard laboratoryTestPatientCard, int id,int id2,int id3)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new LaboratoryTestPatientCard
                {
                    LaboratoryCatagoryPatientCardId = id3,
                    PatientCardDetailId = id2,
                    PatientId = id,
                    Reference = laboratoryTestPatientCard.Reference,
                    Result = laboratoryTestPatientCard.Result,
                    Test = laboratoryTestPatientCard.Test
                };

                db.LaboratoryTestPatientCards.Add(entity);
                db.SaveChanges();
                laboratoryTestPatientCard.LaboratoryTestPatientCardId = entity.LaboratoryTestPatientCardId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryTestPatientCard", "Success", User.Identity.Name, User.Identity.Name + " add Laboratory Tests at patient card called " + entity.Test + " at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { laboratoryTestPatientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTestPatientCard_Update([DataSourceRequest]DataSourceRequest request, LaboratoryTestPatientCard laboratoryTestPatientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryTestPatientCard
                {
                    LaboratoryTestPatientCardId = laboratoryTestPatientCard.LaboratoryTestPatientCardId,
                    LaboratoryCatagoryPatientCardId = laboratoryTestPatientCard.LaboratoryCatagoryPatientCardId,
                    PatientCardDetailId = laboratoryTestPatientCard.PatientCardDetailId,
                    PatientId = laboratoryTestPatientCard.PatientId,
                    Reference = laboratoryTestPatientCard.Reference,
                    Result = laboratoryTestPatientCard.Result,
                    Test = laboratoryTestPatientCard.Test
                };



                db.LaboratoryTestPatientCards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryTestPatientCard", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory Tests at patient card called " + entity.Test + " at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { laboratoryTestPatientCard }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTestPatientCardSentToLaboratory_Update([DataSourceRequest]DataSourceRequest request, LaboratoryTestPatientCard laboratoryTestPatientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryTestPatientCard
                {
                    LaboratoryTestPatientCardId = laboratoryTestPatientCard.LaboratoryTestPatientCardId,
                    LaboratoryCatagoryPatientCardId = laboratoryTestPatientCard.LaboratoryCatagoryPatientCardId,
                    PatientCardDetailId = laboratoryTestPatientCard.PatientCardDetailId,
                    PatientId = laboratoryTestPatientCard.PatientId,
                    Reference = laboratoryTestPatientCard.Reference,
                    Result = laboratoryTestPatientCard.Result,
                    Test = laboratoryTestPatientCard.Test
                };



                db.LaboratoryTestPatientCards.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x=>x.PatientCardDetailId == entity.PatientCardDetailId);
                //if(patientCardDetail != null)
                //{
                //    patientCardDetail.IsNew = false;

                //    db.Entry(patientCardDetail).State = EntityState.Modified;
                //    db.SaveChanges();
                //}

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "LaboratoryTestPatientCard", "Success", User.Identity.Name, User.Identity.Name + " update Laboratory Tests at patient card called " + entity.Test + " at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { laboratoryTestPatientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LaboratoryTestPatientCard_Destroy([DataSourceRequest]DataSourceRequest request, LaboratoryTestPatientCard laboratoryTestPatientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new LaboratoryTestPatientCard
                {
                    LaboratoryTestPatientCardId = laboratoryTestPatientCard.LaboratoryTestPatientCardId,
                    LaboratoryCatagoryPatientCardId = laboratoryTestPatientCard.LaboratoryCatagoryPatientCardId,
                    PatientCardDetailId = laboratoryTestPatientCard.PatientCardDetailId,
                    PatientId = laboratoryTestPatientCard.PatientId,
                    Reference = laboratoryTestPatientCard.Reference,
                    Result = laboratoryTestPatientCard.Result,
                    Test = laboratoryTestPatientCard.Test
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Laboratory Test at patient card called " + entity.Test);
                #endregion
                db.LaboratoryTestPatientCards.Attach(entity);
                db.LaboratoryTestPatientCards.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { laboratoryTestPatientCard }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}