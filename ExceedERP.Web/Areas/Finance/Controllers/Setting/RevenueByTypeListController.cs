﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Finance.Report;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class RevenueListController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RevenueLists_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RevenueList> revenueLists = _db.RevenueLists.Where(x=>x.RevenueBySalesTypeId==id);
            DataSourceResult result = revenueLists.ToDataSourceResult(request, revenueList => new {
                RevenueListId = revenueList.RevenueListId,
               
                Description = revenueList.Description,
                NoteNumber = revenueList.NoteNumber
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueLists_Create(int id,[DataSourceRequest]DataSourceRequest request, RevenueList revenueList)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueList
                {
                    RevenueBySalesTypeId =id,
                   
                    Description = revenueList.Description,
                    NoteNumber = revenueList.NoteNumber
                };

                _db.RevenueLists.Add(entity);
                _db.SaveChanges();
                revenueList.RevenueListId = entity.RevenueListId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type list", "Success", User.Identity.Name, $"created revenue by sales type {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueLists_Update([DataSourceRequest]DataSourceRequest request, RevenueList revenueList)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueList
                {
                    RevenueListId = revenueList.RevenueListId,
                    RevenueBySalesTypeId = revenueList.RevenueBySalesTypeId,
                   
                    Description = revenueList.Description,
                    NoteNumber = revenueList.NoteNumber
                };

                _db.RevenueLists.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type list", "Success", User.Identity.Name, $"updated revenue by sales type {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueList }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueLists_Destroy([DataSourceRequest]DataSourceRequest request, RevenueList revenueList)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueList
                {
                    RevenueListId = revenueList.RevenueListId,
                   
                    Description = revenueList.Description,
                    NoteNumber = revenueList.NoteNumber
                };

                _db.RevenueLists.Attach(entity);
                _db.RevenueLists.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted revenue by sales type list {revenueList.Description}");
                #endregion
            }

            return Json(new[] { revenueList }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
