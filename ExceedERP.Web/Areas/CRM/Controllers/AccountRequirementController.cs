﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
﻿using ExceedERP.Core;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
 using ExceedERP.Core.Domain.CRM;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class AccountRequirementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AccountRequirements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AccountRequirement> accountrequirements = db.AccountRequirements;
            DataSourceResult result = accountrequirements.ToDataSourceResult(request, accountRequirement => new
            {
                AccountRequirementID = accountRequirement.AccountRequirementID,
                ControlAccountID = accountRequirement.ControlAccountID,
                ObjectType = accountRequirement.ObjectType,
                IsMandatory = accountRequirement.IsMandatory,
                CreatedBy = accountRequirement.CreatedBy,
                DateCreated = accountRequirement.DateCreated,
                LastModified = accountRequirement.LastModified,
                ModifiedBy = accountRequirement.ModifiedBy,
                OrgBranchName = accountRequirement.OrgBranchName,
                Remark = accountRequirement.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountRequirements_Create([DataSourceRequest]DataSourceRequest request, AccountRequirement accountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccountRequirement
                {
                    ControlAccountID = accountRequirement.ControlAccountID,
                    ObjectType = accountRequirement.ObjectType,
                    IsMandatory = accountRequirement.IsMandatory,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = accountRequirement.Remark
                };

                db.AccountRequirements.Add(entity);
                db.SaveChanges();
                accountRequirement.AccountRequirementID = entity.AccountRequirementID;
            }

            return Json(new[] { accountRequirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountRequirements_Update([DataSourceRequest]DataSourceRequest request, AccountRequirement accountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccountRequirement
                {
                    AccountRequirementID = accountRequirement.AccountRequirementID,
                    ControlAccountID = accountRequirement.ControlAccountID,
                    ObjectType = accountRequirement.ObjectType,
                    IsMandatory = accountRequirement.IsMandatory,
                    CreatedBy = accountRequirement.CreatedBy,
                    DateCreated = accountRequirement.DateCreated,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name,

                    Remark = accountRequirement.Remark
                };

                db.AccountRequirements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { accountRequirement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccountRequirements_Destroy([DataSourceRequest]DataSourceRequest request, AccountRequirement accountRequirement)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccountRequirement
                {
                    AccountRequirementID = accountRequirement.AccountRequirementID,
                    ControlAccountID = accountRequirement.ControlAccountID,
                    ObjectType = accountRequirement.ObjectType,
                    IsMandatory = accountRequirement.IsMandatory,
                    Remark = accountRequirement.Remark
                };

                db.AccountRequirements.Attach(entity);
                db.AccountRequirements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { accountRequirement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
