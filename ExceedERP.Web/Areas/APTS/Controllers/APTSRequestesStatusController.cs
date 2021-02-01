using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Areas.HRM.Controllers.Leave;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Core.Domain.APTS;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class APTSRequestesStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Index()
        {
            return View();
        }


        public ActionResult APTSRequestesStatus_Read([DataSourceRequest]DataSourceRequest request, int id, string referenceType)
        {
            GetStatusType();

            HRMInputs.RequestId = id;
            HRMInputs.ReferenceType = referenceType;
            IQueryable<APTSRequestStatus> requestesstatus = db.APTSRequestStatus.Where(x => x.RequestId == id && x.ReferenceType == referenceType);
            DataSourceResult result = requestesstatus.ToDataSourceResult(request, requestesStatus => new
            {
                APTSRequestStatusId = requestesStatus.APTSRequestStatusId,
                RequestId = requestesStatus.RequestId,
                ReferenceType = requestesStatus.ReferenceType,
                Status = requestesStatus.Status,
                Date = requestesStatus.Date,
                Remark = requestesStatus.Remark,
                DateReqStatAm = requestesStatus.DateReqStatAm
            });
            return Json(result);


        }
        private void GetStatusType()
        {
            var ststus = Enum.GetValues(typeof(DrugRequestStatus))
                 .Cast<DrugRequestStatus>().ToList();

            var ststuses = ststus.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["RequestStatus"] = ststuses;
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APTSRequestesStatus_Create([DataSourceRequest]DataSourceRequest request, APTSRequestStatus requestesStatus, int id, string referenceType2)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var drugRequest = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == id);
                if (drugRequest != null)
                {
                    var entity = new APTSRequestStatus
                    {
                        RequestId = id,
                        ReferenceType = referenceType2,
                        Status = requestesStatus.Status,
                        Date = DateTime.Now,
                        DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                        Remark = requestesStatus.Remark
                    };

                    db.APTSRequestStatus.Add(entity);
                    db.SaveChanges();
                    requestesStatus.APTSRequestStatusId = entity.APTSRequestStatusId;
                }
                else
                {
                    ModelState.AddModelError("Drug Request", "you have to choose store first");
                }
                //

            }

            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APTSRequestesStatus_Update([DataSourceRequest]DataSourceRequest request, APTSRequestStatus requestesStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new APTSRequestStatus
                {
                    APTSRequestStatusId = requestesStatus.APTSRequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = DateTime.Now,
                    DateReqStatAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Remark = requestesStatus.Remark
                };

                db.APTSRequestStatus.Add(entity);
                db.SaveChanges();
                requestesStatus.APTSRequestStatusId = entity.APTSRequestStatusId;
                if(entity.Status == DrugRequestStatus.APPROVE)
                {
                    var drugRequest = db.DrugRequests.FirstOrDefault(x=>x.DrugRequestId == entity.RequestId);
                    if(drugRequest != null)
                    {
                        drugRequest.IsOnlineApproved = true;
                        
                        db.Entry(drugRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (entity.Status == DrugRequestStatus.REJECT)
                {
                    var drugRequest = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == entity.RequestId);
                    if (drugRequest != null)
                    {
                        drugRequest.IsRejected = true;

                        db.Entry(drugRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (entity.Status == DrugRequestStatus.WAITING_FOR_APPROVAL)
                {
                    var drugRequest = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == entity.RequestId);
                    if (drugRequest != null)
                    {
                        drugRequest.IsSendForApproval = true;

                        db.Entry(drugRequest).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

            }
            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }
        public double GetWeekDays(DateTime startDate, DateTime endDate)
        {
            double noOfWeek = 0;
            var sundayweekendSetting = db.Weekends.Where(x => x.Name == WeekendName.SUNDAY).FirstOrDefault();
            var saturdayweekendSetting = db.Weekends.Where(x => x.Name == WeekendName.SATURDAY).FirstOrDefault();
            List<DateTime> dayList = new List<DateTime>();
            TimeSpan diff = endDate - startDate;
            int days = diff.Days;
            for (var i = 0; i <= days; i++)
            {
                var testDate = startDate.AddDays(i);
                switch (testDate.DayOfWeek)
                {
                    case DayOfWeek.Saturday:

                        if (saturdayweekendSetting != null)
                        {
                            if (saturdayweekendSetting.IsHalfDay)
                            {
                                noOfWeek += 0.5;
                            }
                            else
                            {
                                noOfWeek += 1;

                            }
                        }
                        break;
                    case DayOfWeek.Sunday:

                        if (sundayweekendSetting != null)
                        {
                            if (sundayweekendSetting.IsHalfDay)
                            {
                                noOfWeek += 0.5;
                            }
                            else
                            {
                                noOfWeek += 1;

                            }
                        }
                        //dayList.Add(testDate);
                        break;
                }
            }
            return noOfWeek;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APTSRequestesStatus_Destroy([DataSourceRequest]DataSourceRequest request, APTSRequestStatus requestesStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new APTSRequestStatus
                {
                    APTSRequestStatusId = requestesStatus.APTSRequestStatusId,
                    RequestId = requestesStatus.RequestId,
                    ReferenceType = requestesStatus.ReferenceType,
                    Status = requestesStatus.Status,
                    Date = requestesStatus.Date,
                    DateReqStatAm = requestesStatus.DateReqStatAm,
                    Remark = requestesStatus.Remark
                };
                #region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recruitment request of " + requestesStatus.ReferenceType + " and status" + requestesStatus.Status.ToString());
                #endregion
                db.APTSRequestStatus.Attach(entity);
                db.APTSRequestStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { requestesStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
