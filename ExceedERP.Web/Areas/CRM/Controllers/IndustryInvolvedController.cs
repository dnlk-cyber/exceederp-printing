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
  using ExceedERP.Core.Domain.Common;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
﻿using Microsoft.Ajax.Utilities;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class IndustryInvolvedController : Controller
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
                CreatedBy = industryInvolved.CreatedBy,
                DateCreated = industryInvolved.DateCreated,
                LastModified = industryInvolved.LastModified,
                ModifiedBy = industryInvolved.ModifiedBy,
                OrgBranchName = industryInvolved.OrgBranchName,
                Remark = industryInvolved.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndustryInvolveds_Create([DataSourceRequest]DataSourceRequest request, IndustryInvolved industryInvolved, int referenceIndustryInvolvment, string industryInvolvedReferenceType)
        {
            List<IndustryInvolved> industryInvolvedList = db.IndustryInvolveds.Where(ii => ii.Reference == referenceIndustryInvolvment.ToString() && ii.ReferenceType == industryInvolvedReferenceType && ii.Index == industryInvolved.Index && ii.Industry==industryInvolved.Industry).ToList();
            if (industryInvolvedList.Count>0)
            {
                ModelState.AddModelError("InvalidIndex","Please enter valid index value");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new IndustryInvolved
                    {
                        Index = industryInvolved.Index,
                        Reference = referenceIndustryInvolvment.ToString(),
                        ReferenceType = industryInvolvedReferenceType,
                        Industry = industryInvolved.Industry,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = industryInvolved.Remark
                    };

                    db.IndustryInvolveds.Add(entity);
                    db.SaveChanges();
                    industryInvolved.IndustryInvolvedID = entity.IndustryInvolvedID;
                }
            }
          return Json(new[] { industryInvolved }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IndustryInvolveds_Update([DataSourceRequest]DataSourceRequest request, IndustryInvolved industryInvolved)
        {
            if (ModelState.IsValid)
            {
                var entity = new IndustryInvolved
                {
                    IndustryInvolvedID = industryInvolved.IndustryInvolvedID,
                    Index = industryInvolved.Index,
                    ReferenceType = industryInvolved.ReferenceType,
                    Reference = industryInvolved.Reference,
                    Industry = industryInvolved.Industry,
                    Remark = industryInvolved.Remark
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
            if (ModelState.IsValid)
            {
                var entity = new IndustryInvolved
                {
                    IndustryInvolvedID = industryInvolved.IndustryInvolvedID,
                    Index = industryInvolved.Index,
                    Reference = industryInvolved.Reference,
                    Industry = industryInvolved.Industry,
                    Remark = industryInvolved.Remark
                };

                db.IndustryInvolveds.Attach(entity);
                db.IndustryInvolveds.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted industry Involved number " + industryInvolved.IndustryInvolvedID);
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
