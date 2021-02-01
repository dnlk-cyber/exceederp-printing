﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
      [AuthorizeRoles(CRMRoles.CRMAdmin)] 
     public class SerialCodeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SerialCodes_Read([DataSourceRequest]DataSourceRequest request, int lineitemID)
        {
            IQueryable<SerialCode> serialcodes = db.SerialCodes.Where(sc => sc.LineItem == lineitemID.ToString());
            DataSourceResult result = serialcodes.ToDataSourceResult(request, serialCode => new
            {
                SerialCodeID = serialCode.SerialCodeID,
                LineItem = lineitemID,
                SerialDefinition = serialCode.SerialDefinition,
                Number = serialCode.Number,
                CreatedBy = serialCode.CreatedBy,
                DateCreated = serialCode.DateCreated,
                LastModified = serialCode.LastModified,
                ModifiedBy = serialCode.ModifiedBy,
                OrgBranchName = serialCode.OrgBranchName,
                Remark = serialCode.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialCodes_Create([DataSourceRequest]DataSourceRequest request, SerialCode serialCode, int lineitemID)
        {
            if (ModelState.IsValid)
            {
                var entity = new SerialCode
                {
                    LineItem = lineitemID.ToString(),
                    SerialDefinition = serialCode.SerialDefinition,
                    Number = serialCode.Number,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = serialCode.Remark
                };

                db.SerialCodes.Add(entity);
                db.SaveChanges();
                serialCode.SerialCodeID = entity.SerialCodeID;
            }

            return Json(new[] { serialCode }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialCodes_Update([DataSourceRequest]DataSourceRequest request, SerialCode serialCode)
        {
            if (ModelState.IsValid)
            {
                var entity = new SerialCode
                {
                    SerialCodeID = serialCode.SerialCodeID,
                    LineItem = serialCode.LineItem,
                    SerialDefinition = serialCode.SerialDefinition,
                    Number = serialCode.Number,
                    Remark = serialCode.Remark
                };

                db.SerialCodes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { serialCode }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SerialCodes_Destroy([DataSourceRequest]DataSourceRequest request, SerialCode serialCode)
        {
            if (ModelState.IsValid)
            {
                var entity = new SerialCode
                {
                    SerialCodeID = serialCode.SerialCodeID,
                    LineItem = serialCode.LineItem,
                    SerialDefinition = serialCode.SerialDefinition,
                    Number = serialCode.Number,
                    Remark = serialCode.Remark
                };

                db.SerialCodes.Attach(entity);
                db.SerialCodes.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Serial Code number " + serialCode.SerialCodeID);
     
            }

            return Json(new[] { serialCode }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
