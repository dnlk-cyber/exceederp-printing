﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;
using XAPI;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    
    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class PatientCardDetailController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }



        }
        [AuthorizeRoles( HrmConstants.HRHealthUser)]
        public ActionResult PatientCardDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PatientCardDetail> patientcarddetail = db.PatientCardDetail.Where( p=> p.PatientId ==id);
            DataSourceResult result = patientcarddetail.ToDataSourceResult(request, patientCardDetail => new {
                PatientCardDetailId = patientCardDetail.PatientCardDetailId,
                PatientId = patientCardDetail.PatientId,
                Date = patientCardDetail.Date,
                DiagnosisTratment = patientCardDetail.DiagnosisTratment,
                DateAmharic = patientCardDetail.DateAmharic,
                IsClosed = patientCardDetail.IsClosed,
                IsTested = patientCardDetail.IsTested,
                IsSentToLaboratory = patientCardDetail.IsSentToLaboratory,
                IsSentDespensary = patientCardDetail.IsSentDespensary,
            });

            return Json(result);
        }

        public ActionResult PatientCardDetailSentToLaboratory_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var patientcarddetail = db.PatientCardDetail.Where(p => p.PatientId == id && p.IsSentToLaboratory && !p.IsTested).ToList();
          
            DataSourceResult result = patientcarddetail.ToDataSourceResult(request, patientCardDetail => new
            {
                PatientCardDetailId = patientCardDetail.PatientCardDetailId,
                PatientId = patientCardDetail.PatientId,
                Date = patientCardDetail.Date,
                DiagnosisTratment = patientCardDetail.DiagnosisTratment,
                DateAmharic = patientCardDetail.DateAmharic,
            });
            return Json(result);
        }
        [AuthorizeRoles( HrmConstants.HRHealthUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientCardDetail_Create([DataSourceRequest]DataSourceRequest request, PatientCardDetail patientCardDetail, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    ModelState.AddModelError("", "Date  value is mandatory!!");
                }
                else
                {
                    var entity = new PatientCardDetail
                    {
                        PatientId = id,
                        Date = Date,
                        DiagnosisTratment = patientCardDetail.DiagnosisTratment,
                        DateAmharic = DateAmharic,
                    };

                    db.PatientCardDetail.Add(entity);
                    db.SaveChanges();
                    patientCardDetail.PatientCardDetailId = entity.PatientCardDetailId;

                    #region operation log                

                    UserHelper.OperationLog(Request.UserHostAddress, "Patient Card Detail", "Success", User.Identity.Name, User.Identity.Name + " add Patient Card detail at date " + entity.DateAmharic + " treatment " + entity.DiagnosisTratment, Modules.HealthAndInsurance);

                    #endregion

                    var patientCard = db.PatientCard.FirstOrDefault(x=>x.PatientId == entity.PatientId);
                    if(patientCard != null)
                    {
                        patientCard.IsNew = false;
                        patientCard.LastModifiedDate = DateTime.Now;

                        db.Entry(patientCard).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
            }

            return Json(new[] { patientCardDetail }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientCardDetail_Update([DataSourceRequest]DataSourceRequest request, PatientCardDetail patientCardDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = patientCardDetail.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new PatientCardDetail
                {
                    PatientCardDetailId = patientCardDetail.PatientCardDetailId,
                    PatientId = patientCardDetail.PatientId,
                    Date = Date,
                    DiagnosisTratment = patientCardDetail.DiagnosisTratment,
                    DateAmharic = DateAmharic,
                };

                db.PatientCardDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Card Detail", "Success", User.Identity.Name, User.Identity.Name + " update Patient Card detail at date " + entity.DateAmharic + " treatment " + entity.DiagnosisTratment, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientCardDetail }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRHealthAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientCardDetail_Destroy([DataSourceRequest]DataSourceRequest request, PatientCardDetail patientCardDetail)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new PatientCardDetail
                {
                    PatientCardDetailId = patientCardDetail.PatientCardDetailId,
                    PatientId = patientCardDetail.PatientId,
                    Date = patientCardDetail.Date,
                    DiagnosisTratment = patientCardDetail.DiagnosisTratment,
                    DateAmharic = patientCardDetail.DateAmharic,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted patient card detail information of card id " + patientCardDetail.PatientId);
                #endregion
                db.PatientCardDetail.Attach(entity);
                db.PatientCardDetail.Remove(entity);
                db.SaveChanges();

                var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId == entity.PatientId);
                if (patientCard != null)
                {
                    var ppNo = db.PatientCardDetail.Where(x=>x.PatientId == patientCard.PatientId).Count();
                    if(ppNo == 0)
                    {
                        patientCard.IsNew = true;

                        db.Entry(patientCard).State = EntityState.Modified;
                        db.SaveChanges();

                    }

                }
            }

            return Json(new[] { patientCardDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
