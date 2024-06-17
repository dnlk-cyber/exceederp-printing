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
    public class ShareAndDividendFiscalYearDetailsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            

            return View();
        }

        public ActionResult ShareAndDividendFiscalYearDetails_Read([DataSourceRequest]DataSourceRequest request, int sDId)
        {
            IQueryable<ShareAndDividendFiscalYearDetail> shareAndDividends = db.ShareAndDividendFiscalYearDetails.Where(x=>x.ShareAndDividendId==sDId);
            DataSourceResult result = shareAndDividends.ToDataSourceResult(request, shareAndDividend => new {
                ShareAndDividendFiscalYearDetailId = shareAndDividend.ShareAndDividendFiscalYearDetailId,
                ShareAndDividendId = shareAndDividend.ShareAndDividendId,
                PaidShare = shareAndDividend.PaidShare,
                PlanToPayShare = shareAndDividend.PlanToPayShare,
                DividendObtaind = shareAndDividend.DividendObtaind,
            });

            return Json(result);
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividendFiscalYearDetails_Create([DataSourceRequest]DataSourceRequest request, ShareAndDividendFiscalYearDetail shareAndDividend,int sDId)
        {
            
            if (ModelState.IsValid)
            {
                shareAndDividend.ShareAndDividendId = sDId;
                var entity = GetMappedObject(shareAndDividend);
                db.ShareAndDividendFiscalYearDetails.Add(entity);
                db.SaveChanges();
                shareAndDividend.ShareAndDividendId = entity.ShareAndDividendId;

                #region operation log


                UserHelper.OperationLog(Request.UserHostAddress, "share And Dividend detail", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                

                #endregion
            }
          

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividendFiscalYearDetails_Update([DataSourceRequest]DataSourceRequest request, ShareAndDividendFiscalYearDetail shareAndDividend)
        {
            
            if (ModelState.IsValid)
            {
                
                var entity = GetMappedObject(shareAndDividend);
                db.ShareAndDividendFiscalYearDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "share And Dividend shared ", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShareAndDividendFiscalYearDetails_Destroy([DataSourceRequest]DataSourceRequest request, ShareAndDividendFiscalYearDetail shareAndDividend)
        {
            
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(shareAndDividend);
                #region delete log
               
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Share And Dividend detail  " + shareAndDividend.ShareAndDividendFiscalYearDetailId);
                #endregion
                db.ShareAndDividendFiscalYearDetails.Attach(entity);
                db.ShareAndDividendFiscalYearDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { shareAndDividend }.ToDataSourceResult(request, ModelState));
        }
        private ShareAndDividendFiscalYearDetail GetMappedObject(ShareAndDividendFiscalYearDetail shareAndDividend)
        {
            var entity = new ShareAndDividendFiscalYearDetail
            {
                ShareAndDividendFiscalYearDetailId = shareAndDividend.ShareAndDividendFiscalYearDetailId,
                ShareAndDividendId = shareAndDividend.ShareAndDividendId,
                PaidShare = shareAndDividend.PaidShare,
                PlanToPayShare = shareAndDividend.PlanToPayShare,
                DividendObtaind = shareAndDividend.DividendObtaind,
                

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
