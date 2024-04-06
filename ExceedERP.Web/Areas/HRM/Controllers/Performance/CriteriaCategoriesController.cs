﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
     [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class CriteriaCategoriesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult CriteriaCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CriteriaCategory> categories = db.CriteriaCategories;
            DataSourceResult result = categories.ToDataSourceResult(request, category => new {
                CriteriaCategoryId = category.CriteriaCategoryId,
                Name = category.Name,
                Description = category.Description,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CriteriaCategories_Create([DataSourceRequest]DataSourceRequest request, CriteriaCategory category)
        {
            
          
                if (ModelState.IsValid)
            {
               
                    var entity = new CriteriaCategory
                    {
                        CriteriaCategoryId = category.CriteriaCategoryId,
                        Name = category.Name,
                        Description = category.Description,
                    };

                    db.CriteriaCategories.Add(entity);
                    db.SaveChanges();
                    category.CriteriaCategoryId = entity.CriteriaCategoryId;
                
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CriteriaCategories_Update([DataSourceRequest]DataSourceRequest request, CriteriaCategory category)
        {
            if (ModelState.IsValid)
            {
                var entity = new CriteriaCategory
                {
                    CriteriaCategoryId = category.CriteriaCategoryId,
                    Name = category.Name,
                    Description = category.Description,

                };

                db.CriteriaCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CriteriaCategories_Destroy([DataSourceRequest]DataSourceRequest request, CriteriaCategory category)
        {
            if (ModelState.IsValid)
            {
                var entity = new CriteriaCategory
                {
                    CriteriaCategoryId = category.CriteriaCategoryId,
                    Name = category.Name,
                    Description = category.Description,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Criteria Category Id " + category.CriteriaCategoryId);
                #endregion
                db.CriteriaCategories.Attach(entity);
                db.CriteriaCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { category }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
