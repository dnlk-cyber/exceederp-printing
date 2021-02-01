﻿using System; 
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

using ExceedERP.Core;
 using ExceedERP.Core.Domain.CRM;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class ActivityDefinitionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ActivityDefinitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ActivityDefinition> activitydefinitions = db.ActivityDefinitions;
            DataSourceResult result = activitydefinitions.ToDataSourceResult(request, activityDefinition => new {
                ActivityDefinitionID = activityDefinition.ActivityDefinitionID,
                Reference = activityDefinition.Reference,
                Description = activityDefinition.Description,
                Index = activityDefinition.Index,
                HasIssuingEffect = activityDefinition.HasIssuingEffect,
                CreatedBy = activityDefinition.CreatedBy,
                DateCreated = activityDefinition.DateCreated,
                LastModified = activityDefinition.LastModified,
                ModifiedBy = activityDefinition.ModifiedBy,
                OrgBranchName = activityDefinition.OrgBranchName,
                Remark = activityDefinition.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActivityDefinitions_Create([DataSourceRequest]DataSourceRequest request, ActivityDefinition activityDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new ActivityDefinition
                {
                    Reference = activityDefinition.Reference,
                    Description = activityDefinition.Description,
                    Index = activityDefinition.Index,
                    HasIssuingEffect = activityDefinition.HasIssuingEffect,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = activityDefinition.Remark
                };

                db.ActivityDefinitions.Add(entity);
                db.SaveChanges();
                activityDefinition.ActivityDefinitionID = entity.ActivityDefinitionID;
            }

            return Json(new[] { activityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActivityDefinitions_Update([DataSourceRequest]DataSourceRequest request, ActivityDefinition activityDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new ActivityDefinition
                {
                    ActivityDefinitionID = activityDefinition.ActivityDefinitionID,
                    Reference = activityDefinition.Reference,
                    Description = activityDefinition.Description,
                    Index = activityDefinition.Index,
                    HasIssuingEffect = activityDefinition.HasIssuingEffect,
                    CreatedBy = activityDefinition.CreatedBy,
                    DateCreated = activityDefinition.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,
                    Remark = activityDefinition.Remark
                };

                db.ActivityDefinitions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { activityDefinition }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ActivityDefinitions_Destroy([DataSourceRequest]DataSourceRequest request, ActivityDefinition activityDefinition)
        {
            if (ModelState.IsValid)
            {
                var entity = new ActivityDefinition
                {
                    ActivityDefinitionID = activityDefinition.ActivityDefinitionID,
                    Reference = activityDefinition.Reference,
                    Description = activityDefinition.Description,
                    Index = activityDefinition.Index,
                    HasIssuingEffect = activityDefinition.HasIssuingEffect,
                    Remark = activityDefinition.Remark
                };

                db.ActivityDefinitions.Attach(entity);
                db.ActivityDefinitions.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Activity Definition number " + activityDefinition.ActivityDefinitionID);
			
            }

            return Json(new[] { activityDefinition }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
