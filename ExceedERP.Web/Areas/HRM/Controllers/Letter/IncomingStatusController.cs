
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Letter
{

    [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
    public class IncomingStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
 
        public static DateTime Date { get; set; }
        public static string DateAmahric { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmahric = value;
                var splitDate = DateAmahric.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRLettersUser)]
        public ActionResult IncommingStatus_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IncommingStatus> incommingstatus = db.IncommingStatus.Where(i => i.IncommingId == id);
            DataSourceResult result = incommingstatus.ToDataSourceResult(request, incommingStatus => new
            {
                IncommingStatusId = incommingStatus.IncommingStatusId,
                IncommingId = incommingStatus.IncommingId,
                Date = incommingStatus.Date,
                Decsion = incommingStatus.Decsion,
                Remark = incommingStatus.Remark,
                DateAmharic = incommingStatus.DateAmharic
            });

            return Json(result);
        }
   
        [AuthorizeRoles( HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncommingStatus_Create([DataSourceRequest]DataSourceRequest request, IncommingStatus incommingStatus, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new IncommingStatus
                {
                    IncommingId = id,
                    Date = Date,
                    Decsion = incommingStatus.Decsion,
                    Remark = incommingStatus.Remark,
                    DateAmharic = DateAmahric
                };

                db.IncommingStatus.Add(entity);
                db.SaveChanges();
                incommingStatus.IncommingStatusId = entity.IncommingStatusId;
            }

            return Json(new[] { incommingStatus }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles( HrmConstants.HRLettersUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncommingStatus_Update([DataSourceRequest]DataSourceRequest request, IncommingStatus incommingStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmahric == null)
                {
                    DateAmahric = incommingStatus.DateAmharic;
                    var splitDate = DateAmahric.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }


                var entity = new IncommingStatus
                {
                    IncommingStatusId = incommingStatus.IncommingStatusId,
                    IncommingId = incommingStatus.IncommingId,
                    Date = Date,
                    Decsion = incommingStatus.Decsion,
                    Remark = incommingStatus.Remark,
                    DateAmharic = DateAmahric
                };

                db.IncommingStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { incommingStatus }.ToDataSourceResult(request, ModelState));
        }
   
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IncommingStatus_Destroy([DataSourceRequest]DataSourceRequest request, IncommingStatus incommingStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new IncommingStatus
                {
                    IncommingStatusId = incommingStatus.IncommingStatusId,
                    IncommingId = incommingStatus.IncommingId,
                    Date = incommingStatus.Date,
                    Decsion = incommingStatus.Decsion,
                    Remark = incommingStatus.Remark,
                    DateAmharic = incommingStatus.DateAmharic
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter status of incomming Id" + incommingStatus.IncommingId);
                #endregion
                db.IncommingStatus.Attach(entity);
                db.IncommingStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { incommingStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
