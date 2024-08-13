﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Insurance;

namespace ExceedERP.Web.Areas.Insurance.Controllers
{
   
    [AuthorizeRoles(HrmConstants.HRHealthUser,HrmConstants.HRHealthAdmin)]
    public class AccidentMedicalRecordController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccidentMedicalRecords_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<AccidentMedicalRecord> accidentmedicalrecords = db.AccidentMedicalRecords.Where(x=> x.AccidentRecordId ==id);
            DataSourceResult result = accidentmedicalrecords.ToDataSourceResult(request, accidentMedicalRecord => new {
                AccidentMedicalRecordId = accidentMedicalRecord.AccidentMedicalRecordId,
                MedicalTreatment = accidentMedicalRecord.MedicalTreatment,
                InstitutionName = accidentMedicalRecord.InstitutionName,
                AccidentRecordId = accidentMedicalRecord.AccidentRecordId,
                MedicalCost = accidentMedicalRecord.MedicalCost,
                Ramrk = accidentMedicalRecord.Ramrk
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentMedicalRecords_Create([DataSourceRequest]DataSourceRequest request, AccidentMedicalRecord accidentMedicalRecord, int id)
        {
           
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AccidentMedicalRecord
                {
                    MedicalTreatment = accidentMedicalRecord.MedicalTreatment,
                    InstitutionName = accidentMedicalRecord.InstitutionName,
                    MedicalCost = accidentMedicalRecord.MedicalCost,
                    AccidentRecordId = id,
                    Ramrk = accidentMedicalRecord.Ramrk
                };

                db.AccidentMedicalRecords.Add(entity);
                db.SaveChanges();
                accidentMedicalRecord.AccidentMedicalRecordId = entity.AccidentMedicalRecordId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "AccidentMedicalRecord", "Success", User.Identity.Name, User.Identity.Name + " add accident medical recored at Date " + DateTime.Today+" Medical treatment "+entity.MedicalTreatment +" institution called "+entity.InstitutionName, Modules.Health);

                #endregion
            }

            return Json(new[] { accidentMedicalRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentMedicalRecords_Update([DataSourceRequest]DataSourceRequest request, AccidentMedicalRecord accidentMedicalRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccidentMedicalRecord
                {
                    AccidentMedicalRecordId = accidentMedicalRecord.AccidentMedicalRecordId,
                    MedicalTreatment = accidentMedicalRecord.MedicalTreatment,
                    InstitutionName = accidentMedicalRecord.InstitutionName,
                    AccidentRecordId = accidentMedicalRecord.AccidentRecordId,
                    MedicalCost = accidentMedicalRecord.MedicalCost,
                    Ramrk = accidentMedicalRecord.Ramrk
                };

                db.AccidentMedicalRecords.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "AccidentMedicalRecord", "Success", User.Identity.Name, User.Identity.Name + " update accident medical recored at Date " + DateTime.Today + " Medical treatment " + entity.MedicalTreatment + " institution called " + entity.InstitutionName, Modules.Health);

                #endregion
            }

            return Json(new[] { accidentMedicalRecord }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccidentMedicalRecords_Destroy([DataSourceRequest]DataSourceRequest request, AccidentMedicalRecord accidentMedicalRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccidentMedicalRecord
                {
                    AccidentMedicalRecordId = accidentMedicalRecord.AccidentMedicalRecordId,
                    MedicalTreatment = accidentMedicalRecord.MedicalTreatment,
                    InstitutionName = accidentMedicalRecord.InstitutionName,
                    MedicalCost = accidentMedicalRecord.MedicalCost,
                    AccidentRecordId = accidentMedicalRecord.AccidentRecordId,
                    Ramrk = accidentMedicalRecord.Ramrk
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted accident record  information of institution name " + accidentMedicalRecord.InstitutionName);
                #endregion
                db.AccidentMedicalRecords.Attach(entity);
                db.AccidentMedicalRecords.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { accidentMedicalRecord }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
