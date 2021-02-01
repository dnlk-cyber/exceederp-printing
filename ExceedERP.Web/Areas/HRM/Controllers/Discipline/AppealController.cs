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
 using ExceedERP.Core.Domain.HRM.Discipline;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Discipline
{

    [AuthorizeRoles(HrmConstants.HRDisciplineUser)]

    public class AppealController : Controller
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
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }
        public ActionResult Appeal_Read([DataSourceRequest]DataSourceRequest request, int ida)
        {
            IQueryable<Appeal> appeals = db.Appeal.Where (x=> x.DisciplinaryCaseId ==ida);
            DataSourceResult result = appeals.ToDataSourceResult(request, appeal => new {
                AppealId = appeal.AppealId,
                AppealDetail = appeal.AppealDetail,
                Date = appeal.Date,
                DateAmharic = appeal.DateAmharic,
                Remark = appeal.Remark,
                DisciplinaryCaseId = appeal.DisciplinaryCaseId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appeal_Create([DataSourceRequest]DataSourceRequest request, Appeal appeal, int idaTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Appeal
                    {
                        AppealDetail = appeal.AppealDetail,
                        Date = Date,
                        DateAmharic = DateAmharic,
                        Remark = appeal.Remark,
                        DisciplinaryCaseId = idaTwo
                    };

                    db.Appeal.Add(entity);
                    db.SaveChanges();
                    appeal.AppealId = entity.AppealId;

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Appeal", "Success", User.Identity.Name, User.Identity.Name + " add new appeal at date "+DateTime.Today , Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { appeal }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appeal_Update([DataSourceRequest]DataSourceRequest request, Appeal appeal)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmharic == null)
                {
                    DateAmharic = appeal.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (Date.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Appeal
                    {
                        AppealId = appeal.AppealId,
                        DisciplinaryCaseId = appeal.DisciplinaryCaseId,

                        AppealDetail = appeal.AppealDetail,
                        Date = Date,
                        DateAmharic = DateAmharic,
                        Remark = appeal.Remark
                    };

                    db.Appeal.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    UserHelper.OperationLog(Request.UserHostAddress, "Appeal", "Success", User.Identity.Name, User.Identity.Name + " update the appeal at date "+ DateTime.Today, Modules.HRM);

                    #endregion
                }
            }

            return Json(new[] { appeal }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Appeal_Destroy([DataSourceRequest]DataSourceRequest request, Appeal appeal)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Appeal
                {
                    AppealId = appeal.AppealId,
                    AppealDetail = appeal.AppealDetail,
                    Date = appeal.Date,
                    DateAmharic = appeal.DateAmharic,
                    Remark = appeal.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted appeal information of appeal id" + appeal.AppealId);
                #endregion
                db.Appeal.Attach(entity);
                db.Appeal.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { appeal }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
