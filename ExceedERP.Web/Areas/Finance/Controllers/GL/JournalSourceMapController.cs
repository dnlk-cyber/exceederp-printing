﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class JournalSourceMapController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin, FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin, FinanceConstants.GeneralLedgerView)]
        public ActionResult JournalSourceMaps_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JournalSourceMap> journalsourcemaps = _db.JournalSourceMaps.Where(x=>x.GLJournalCategoriersID==id);
            DataSourceResult result = journalsourcemaps.ToDataSourceResult(request, journalSourceMap => new {
                JournalSourceMapId = journalSourceMap.JournalSourceMapId,
                GLJournalCategoriersID = id,
                Source = journalSourceMap.Source,
                Remark = journalSourceMap.Remark,
                DateCreated = journalSourceMap.DateCreated,
                LastModified = journalSourceMap.LastModified,
                CreatedBy = journalSourceMap.CreatedBy,
                ModifiedBy = journalSourceMap.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JournalSourceMaps_Create(int id,[DataSourceRequest]DataSourceRequest request, JournalSourceMap journalSourceMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new JournalSourceMap
                {
                    GLJournalCategoriersID = id,
                    Source = journalSourceMap.Source,
                    Remark = journalSourceMap.Remark,
                    DateCreated = journalSourceMap.DateCreated,
                    LastModified = journalSourceMap.LastModified,
                    CreatedBy = journalSourceMap.CreatedBy,
                    ModifiedBy = journalSourceMap.ModifiedBy
                };

                _db.JournalSourceMaps.Add(entity);
                _db.SaveChanges();
                journalSourceMap.JournalSourceMapId = entity.JournalSourceMapId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal source map setup", "Success", User.Identity.Name, $"created journal source map setup {entity.GLJournalCategoriersID}", Modules.Finance);
                #endregion
            }

            return Json(new[] { journalSourceMap }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JournalSourceMaps_Update([DataSourceRequest]DataSourceRequest request, JournalSourceMap journalSourceMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new JournalSourceMap
                {
                    JournalSourceMapId = journalSourceMap.JournalSourceMapId,
                    GLJournalCategoriersID = journalSourceMap.GLJournalCategoriersID,
                    Source = journalSourceMap.Source,
                    Remark = journalSourceMap.Remark,
                    DateCreated = journalSourceMap.DateCreated,
                    LastModified = journalSourceMap.LastModified,
                    CreatedBy = journalSourceMap.CreatedBy,
                    ModifiedBy = journalSourceMap.ModifiedBy
                };

                _db.JournalSourceMaps.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL journal source map setup", "Success", User.Identity.Name, $"updated journal source map setup {entity.GLJournalCategoriersID}", Modules.Finance);
                #endregion
            }

            return Json(new[] { journalSourceMap }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JournalSourceMaps_Destroy([DataSourceRequest]DataSourceRequest request, JournalSourceMap journalSourceMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new JournalSourceMap
                {
                    JournalSourceMapId = journalSourceMap.JournalSourceMapId,
                    Source = journalSourceMap.Source,
                    Remark = journalSourceMap.Remark,
                    DateCreated = journalSourceMap.DateCreated,
                    LastModified = journalSourceMap.LastModified,
                    CreatedBy = journalSourceMap.CreatedBy,
                    ModifiedBy = journalSourceMap.ModifiedBy
                };

                _db.JournalSourceMaps.Attach(entity);
                _db.JournalSourceMaps.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { journalSourceMap }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
