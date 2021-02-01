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
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class SubAccountRangeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubAccountRanges_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SubAccountRange> subaccountranges = db.SubAccountRanges;
            DataSourceResult result = subaccountranges.ToDataSourceResult(request, subAccountRange => new {
                SubAccountRangeId = subAccountRange.SubAccountRangeId,
                From = subAccountRange.From,
                To = subAccountRange.To,
                Type = subAccountRange.Type,
                OrgBranchName = subAccountRange.OrgBranchName,
                DateCreated = subAccountRange.DateCreated,
                LastModified = subAccountRange.LastModified,
                CreatedBy = subAccountRange.CreatedBy,
                ModifiedBy = subAccountRange.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubAccountRanges_Create([DataSourceRequest]DataSourceRequest request, SubAccountRange subAccountRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubAccountRange
                {
                    From = subAccountRange.From,
                    To = subAccountRange.To,
                    Type = subAccountRange.Type,
                    OrgBranchName = subAccountRange.OrgBranchName,
                    DateCreated = subAccountRange.DateCreated,
                    LastModified = subAccountRange.LastModified,
                    CreatedBy = subAccountRange.CreatedBy,
                    ModifiedBy = subAccountRange.ModifiedBy
                };

                db.SubAccountRanges.Add(entity);
                db.SaveChanges();
                subAccountRange.SubAccountRangeId = entity.SubAccountRangeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance sub account range", "Success", User.Identity.Name, $"created range {entity.From} to {entity.To} of type {entity.Type}", Modules.Finance);
                #endregion
            }

            return Json(new[] { subAccountRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubAccountRanges_Update([DataSourceRequest]DataSourceRequest request, SubAccountRange subAccountRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubAccountRange
                {
                    SubAccountRangeId = subAccountRange.SubAccountRangeId,
                    From = subAccountRange.From,
                    To = subAccountRange.To,
                    Type = subAccountRange.Type,
                    OrgBranchName = subAccountRange.OrgBranchName,
                    DateCreated = subAccountRange.DateCreated,
                    LastModified = subAccountRange.LastModified,
                    CreatedBy = subAccountRange.CreatedBy,
                    ModifiedBy = subAccountRange.ModifiedBy
                };

                db.SubAccountRanges.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance sub account range", "Success", User.Identity.Name, $"created range {entity.From} to {entity.To} of type {entity.Type}", Modules.Finance);
                #endregion
            }

            return Json(new[] { subAccountRange }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubAccountRanges_Destroy([DataSourceRequest]DataSourceRequest request, SubAccountRange subAccountRange)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubAccountRange
                {
                    SubAccountRangeId = subAccountRange.SubAccountRangeId,
                    From = subAccountRange.From,
                    To = subAccountRange.To,
                    Type = subAccountRange.Type,
                    OrgBranchName = subAccountRange.OrgBranchName,
                    DateCreated = subAccountRange.DateCreated,
                    LastModified = subAccountRange.LastModified,
                    CreatedBy = subAccountRange.CreatedBy,
                    ModifiedBy = subAccountRange.ModifiedBy
                };

                db.SubAccountRanges.Attach(entity);
                db.SubAccountRanges.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subAccountRange }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
