using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class AccountMapController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccountMaps_Read([DataSourceRequest]DataSourceRequest request, string accountMapReferenceType, int id)
        {
            IQueryable<AccountMap> accountmaps = db.AccountMaps.Where(r => r.Reference == id.ToString() && r.ReferenceType == accountMapReferenceType);
            DataSourceResult result = accountmaps.ToDataSourceResult(request, accountMap => new
            {
                AccountMapID = accountMap.AccountMapID,
                ControlAccountID = accountMap.ControlAccountID,
                AccountID = accountMap.AccountID,
                ReferenceType = accountMap.ReferenceType,
                Reference = accountMap.Reference,
                CreatedBy = accountMap.CreatedBy,
                DateCreated = accountMap.DateCreated,
                ModifiedBy = accountMap.ModifiedBy,
                LastModified = accountMap.LastModified,
                Remark = accountMap.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountMaps_Create([DataSourceRequest]DataSourceRequest request, AccountMap accountMap, string accountMapReferenceType, int id)
        {
            List<AccountMap> accountMapList =
                db.AccountMaps.Where(ii =>
                        ii.ControlAccountID == accountMap.ControlAccountID.ToString() &&
                        ii.ReferenceType == accountMapReferenceType &&
                        ii.Reference == accountMap.Reference
                        ).ToList();
            if (accountMapList.Count > 0)
            {
                ModelState.AddModelError("InvalidIndex", "Please enter Different Control Accounts or Reference Type  Duplicate  value exist");
            }
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new AccountMap
                    {
                        ControlAccountID = accountMap.ControlAccountID,
                        AccountID = accountMap.AccountID,
                        ReferenceType = accountMapReferenceType,
                        Reference = id.ToString(),
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = accountMap.Remark
                    };

                    db.AccountMaps.Add(entity);
                    db.SaveChanges();
                    accountMap.AccountMapID = entity.AccountMapID;
                }
            }
            return Json(new[] { accountMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountMaps_Update([DataSourceRequest]DataSourceRequest request, AccountMap accountMap)
        {
            if (ModelState.IsValid)
            {
                List<AccountMap> accountMapList =
               db.AccountMaps.Where(ii =>
                       ii.ControlAccountID == accountMap.ControlAccountID.ToString() &&
                       ii.ReferenceType == accountMap.ReferenceType &&
                       ii.Reference == accountMap.Reference &&
                       ii.AccountMapID != accountMap.AccountMapID
                       ).ToList();
                if (accountMapList.Any())
                {
                    ModelState.AddModelError("InvalidIndex", "Please enter Different Control Accounts or Reference Type  Duplicate  value exist");
                }
                else
                {
                    var entity = new AccountMap
                    {
                        AccountMapID = accountMap.AccountMapID,
                        ControlAccountID = accountMap.ControlAccountID,
                        AccountID = accountMap.AccountID,
                        ReferenceType = accountMap.ReferenceType,
                        Reference = accountMap.Reference,
                        CreatedBy = accountMap.CreatedBy,
                        DateCreated = accountMap.DateCreated,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name,

                        Remark = accountMap.Remark
                    };

                    db.AccountMaps.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { accountMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountMaps_Destroy([DataSourceRequest]DataSourceRequest request, AccountMap accountMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccountMap
                {
                    AccountMapID = accountMap.AccountMapID,
                    ControlAccountID = accountMap.ControlAccountID,
                    AccountID = accountMap.AccountID,
                    ReferenceType = accountMap.ReferenceType,
                    Reference = accountMap.Reference,
                    Remark = accountMap.Remark
                };

                db.AccountMaps.Attach(entity);
                db.AccountMaps.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Account Map number " + accountMap.AccountMapID);

            }

            return Json(new[] { accountMap }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
