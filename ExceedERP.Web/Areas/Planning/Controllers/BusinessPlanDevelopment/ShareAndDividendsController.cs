﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment;

namespace ExceedERP.Web.Areas.Planning.Controllers.BusinessPlanDevelopment
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_Risk_User)]
    public class ShareAndDividendsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var glFiscalYears = db.GLFiscalYears.Select(g => new
            {
                g.GlFiscalYearId,
                g.Name,

            }).ToList();
           
            ViewData["GlFiscalYears"] = glFiscalYears;

            return View();
        }

        public ActionResult ShareAndDividends_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ShareAndDividend> shareAndDividends = db.ShareAndDividends;
            DataSourceResult result = shareAndDividends.ToDataSourceResult(request, shareAndDividend => new {
                ShareAndDividendId = shareAndDividend.ShareAndDividendId,
                ShareCompanyName = shareAndDividend.ShareCompanyName,
                SubscribedShareAmtNoofShare = shareAndDividend.SubscribedShareAmtNoofShare,
                SubscribedShareAmtBirr = shareAndDividend.SubscribedShareAmtBirr,
                PaidUpShareAmtNoOfShare = shareAndDividend.PaidUpShareAmtNoOfShare,
                PaidUpShareAmtBirr = shareAndDividend.PaidUpShareAmtBirr,
                RemainingShare = shareAndDividend.RemainingShare,
                GlFiscalYearId = shareAndDividend.GlFiscalYearId,
                DateCreated = shareAndDividend.DateCreated,
                CreatedBy = shareAndDividend.CreatedBy,
                LastModified = shareAndDividend.LastModified,
                ModifiedBy = shareAndDividend.ModifiedBy,
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividends_Create([DataSourceRequest]DataSourceRequest request, ShareAndDividend shareAndDividend)
        {
            
            if (ModelState.IsValid)
            {
                shareAndDividend.DateCreated = DateTime.Now.Date;
                shareAndDividend.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(shareAndDividend);
                db.ShareAndDividends.Add(entity);
                db.SaveChanges();
                shareAndDividend.ShareAndDividendId = entity.ShareAndDividendId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "share And Dividend", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                

                #endregion
            }
          

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividends_Update([DataSourceRequest]DataSourceRequest request, ShareAndDividend shareAndDividend)
        {
            
            if (ModelState.IsValid)
            {
                shareAndDividend.LastModified = DateTime.Now.Date;
                shareAndDividend.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(shareAndDividend);
                db.ShareAndDividends.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "share And Dividend ", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividends_Destroy([DataSourceRequest]DataSourceRequest request, ShareAndDividend shareAndDividend)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(shareAndDividend);
                #region delete log
               
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "    share And Dividend  " + shareAndDividend.ShareCompanyName);
                #endregion
                db.ShareAndDividends.Attach(entity);
                db.ShareAndDividends.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }
        private ShareAndDividend GetMappedObject(ShareAndDividend shareAndDividend)
        {
            var entity = new ShareAndDividend
            {
                ShareAndDividendId = shareAndDividend.ShareAndDividendId,
                ShareCompanyName = shareAndDividend.ShareCompanyName,
                SubscribedShareAmtNoofShare = shareAndDividend.SubscribedShareAmtNoofShare,
                SubscribedShareAmtBirr = shareAndDividend.SubscribedShareAmtBirr,
                PaidUpShareAmtNoOfShare = shareAndDividend.PaidUpShareAmtNoOfShare,
                PaidUpShareAmtBirr = shareAndDividend.PaidUpShareAmtBirr,
                RemainingShare = shareAndDividend.RemainingShare,
                GlFiscalYearId = shareAndDividend.GlFiscalYearId,
                DateCreated = shareAndDividend.DateCreated,
                CreatedBy = shareAndDividend.CreatedBy,
                LastModified = shareAndDividend.LastModified,
                ModifiedBy = shareAndDividend.ModifiedBy,

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
