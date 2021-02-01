﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class LetterStatusController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
     
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
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
        public ActionResult LetterStatus_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            IQueryable<LetterStatus> letterstatus = db.LetterStatus.Where( x=> x.BookNo ==id);
            DataSourceResult result = letterstatus.ToDataSourceResult(request, letterStatus => new {
                LetterStatusId = letterStatus.LetterStatusId,
                BookNo = letterStatus.BookNo,
                Date = letterStatus.Date,
                Decision = letterStatus.Decision,
                Location = letterStatus.Location,
                DateAmharic = letterStatus.DateAmharic
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterStatus_Create([DataSourceRequest]DataSourceRequest request, LetterStatus letterStatus, string idTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterStatus
                {
                    BookNo = idTwo,
                    Date = Date,
                    Decision = letterStatus.Decision,
                    Location = letterStatus.Location,
                    DateAmharic = DateAmharic
                };

                db.LetterStatus.Add(entity);
                db.SaveChanges();
                letterStatus.LetterStatusId = entity.LetterStatusId;
            }

            return Json(new[] { letterStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterStatus_Update([DataSourceRequest]DataSourceRequest request, LetterStatus letterStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)

            {
                if (DateAmharic == null)
                {
                    DateAmharic = letterStatus.DateAmharic;
                    var splitDate = DateAmharic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new LetterStatus
                {
                    LetterStatusId = letterStatus.LetterStatusId,
                    BookNo = letterStatus.BookNo,
                    Date = Date,
                    Decision = letterStatus.Decision,
                    Location = letterStatus.Location,
                    DateAmharic = DateAmharic
                };

                db.LetterStatus.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { letterStatus }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LetterStatus_Destroy([DataSourceRequest]DataSourceRequest request, LetterStatus letterStatus)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterStatus
                {
                    LetterStatusId = letterStatus.LetterStatusId,
                    BookNo = letterStatus.BookNo,
                    Decision = letterStatus.Decision,
                    Date = letterStatus.Date,
                    Location = letterStatus.Location,
                    DateAmharic = letterStatus.DateAmharic
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted letter status information of book no " + letterStatus.BookNo);
                #endregion
                db.LetterStatus.Attach(entity);
                db.LetterStatus.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { letterStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
