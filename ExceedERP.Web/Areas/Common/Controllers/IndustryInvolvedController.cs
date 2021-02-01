﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
﻿using Microsoft.Ajax.Utilities;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    public class IndustryInvolvedController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndustryInvolveds_Read([DataSourceRequest]DataSourceRequest request, int referenceIndustryInvolvment, string industryInvolvedReferenceType)
        {
            IQueryable<IndustryInvolved> industryinvolveds = db.IndustryInvolveds.Where(ii => ii.Reference == referenceIndustryInvolvment.ToString() && ii.ReferenceType == industryInvolvedReferenceType);
            DataSourceResult result = industryinvolveds.ToDataSourceResult(request, industryInvolved => new {
                IndustryInvolvedID = industryInvolved.IndustryInvolvedID,
                Index = industryInvolved.Index,
                Reference = industryInvolved.Reference,
                Industry = industryInvolved.Industry,
                ReferenceType=industryInvolved.ReferenceType,
                Remark = industryInvolved.Remark,
                DateCreated = industryInvolved.DateCreated,
                LastModified = industryInvolved.LastModified,
                CreatedBy = industryInvolved.CreatedBy,
                ModifiedBy = industryInvolved.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndustryInvolveds_Create([DataSourceRequest]DataSourceRequest request, IndustryInvolved industryInvolved, int referenceIndustryInvolvment, string industryInvolvedReferenceType)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new IndustryInvolved
                {
                    Index = industryInvolved.Index,
                    Reference = referenceIndustryInvolvment.ToString(),
                    ReferenceType = industryInvolvedReferenceType,
                    Industry = industryInvolved.Industry,
                    Remark = industryInvolved.Remark,
                    DateCreated = DateTime .Now ,
                    LastModified = industryInvolved.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = industryInvolved.ModifiedBy
                };

                db.IndustryInvolveds.Add(entity);
                db.SaveChanges();
                industryInvolved.IndustryInvolvedID = entity.IndustryInvolvedID;
            }

            return Json(new[] { industryInvolved }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndustryInvolveds_Update([DataSourceRequest]DataSourceRequest request, IndustryInvolved industryInvolved)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new IndustryInvolved
                {
                    IndustryInvolvedID = industryInvolved.IndustryInvolvedID,
                    Index = industryInvolved.Index,
                    ReferenceType = industryInvolved.ReferenceType,
                    Reference = industryInvolved.Reference,
                    Industry = industryInvolved.Industry,
                    Remark = industryInvolved.Remark,
                    DateCreated = industryInvolved.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = industryInvolved.CreatedBy,
                    ModifiedBy = User .Identity .Name 
                };

                db.IndustryInvolveds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { industryInvolved }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndustryInvolveds_Destroy([DataSourceRequest]DataSourceRequest request, IndustryInvolved industryInvolved)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new IndustryInvolved
                {
                    IndustryInvolvedID = industryInvolved.IndustryInvolvedID,
                    Index = industryInvolved.Index,
                    Reference = industryInvolved.Reference,
                    Industry = industryInvolved.Industry,
                    Remark = industryInvolved.Remark,
                    DateCreated = industryInvolved.DateCreated,
                    LastModified = industryInvolved.LastModified,
                    CreatedBy = industryInvolved.CreatedBy,
                    ModifiedBy = industryInvolved.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Industry Involvement " + industryInvolved.ReferenceType + " " + industryInvolved.Industry.ToString());
                #endregion
                db.IndustryInvolveds.Attach(entity);
                db.IndustryInvolveds.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { industryInvolved }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
