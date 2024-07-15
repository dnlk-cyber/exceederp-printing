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
 using ExceedERP.Core.Domain.HRM.PerformanceManagment;
 using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
     [AuthorizeRoles(HrmConstants.HRPerformanceAdmin,
        HrmConstants.HRAdmin)]
    public class PerformanceRankController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult PerformanceRank_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PerformanceRank> performancerank = db.PerformanceRank;
            DataSourceResult result = performancerank.ToDataSourceResult(request, performanceRank => new {
                PerformanceRankId = performanceRank.PerformanceRankId,
                Minimum = performanceRank.Minimum,
                Maximum = performanceRank.Maximum,
                BonusRate = performanceRank.BonusRate,
                Remark = performanceRank.Remark,
                Description = performanceRank.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceRank_Create([DataSourceRequest]DataSourceRequest request, PerformanceRank performanceRank)
        {
            if (ModelState.IsValid)
            {
                if (performanceRank.Minimum > performanceRank.Maximum)
                {
                    ModelState.AddModelError("", "Please Enter Valid Value. Minimum value should be less than maximum value!!");
                }
                else
                {
                    var entity = new PerformanceRank
                    {
                        Minimum = performanceRank.Minimum,
                        Maximum = performanceRank.Maximum,
                        BonusRate = performanceRank.BonusRate,
                        Remark = performanceRank.Remark,
                        Description = performanceRank.Description
                    };

                    db.PerformanceRank.Add(entity);
                    db.SaveChanges();
                    performanceRank.PerformanceRankId = entity.PerformanceRankId;

                    #region operation log

                        UserHelper.OperationLog(Request.UserHostAddress, "Performance Rank ", "Success", User.Identity.Name, User.Identity.Name + " add Performance Rank called " + entity.Description + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                   

                    #endregion
                }
            }

            return Json(new[] { performanceRank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceRank_Update([DataSourceRequest]DataSourceRequest request, PerformanceRank performanceRank)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceRank
                {
                    PerformanceRankId = performanceRank.PerformanceRankId,
                    Minimum = performanceRank.Minimum,
                    Maximum = performanceRank.Maximum,
                    BonusRate = performanceRank.BonusRate,
                    Remark = performanceRank.Remark,
                    Description = performanceRank.Description
                };

                db.PerformanceRank.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

               
                    UserHelper.OperationLog(Request.UserHostAddress, "Performance Rank ", "Success", User.Identity.Name, User.Identity.Name + " update Performance Rank called " + entity.Description + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                

                #endregion

            }

            return Json(new[] { performanceRank }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PerformanceRank_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceRank performanceRank)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceRank
                {
                    PerformanceRankId = performanceRank.PerformanceRankId,
                    Minimum = performanceRank.Minimum,
                    Maximum = performanceRank.Maximum,
                    BonusRate = performanceRank.BonusRate,
                    Remark = performanceRank.Remark,
                    Description = performanceRank.Description
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted performance Rank information performance Rank Name " + performanceRank.Description);
                #endregion
                db.PerformanceRank.Attach(entity);
                db.PerformanceRank.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { performanceRank }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
