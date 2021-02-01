﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
  using ExceedERP.Core.Domain.Common;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class TermController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
       
           ViewData["LookupList"] = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERM_TYPE).ToList();
           ViewData["VoucherDefinitionList"] = db.VoucherDefinitions.ToList();
            return View();
        }

        public ActionResult Terms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Term> terms = db.Terms;
            DataSourceResult result = terms.ToDataSourceResult(request, term => new {
                TermID = term.TermID,
                VoucherDefinitionID = term.VoucherDefinitionID,
                LookupID = term.LookupId,
                DefaultValue = term.DefaultValue,
                IsMandatory = term.IsMandatory,
                CreatedBy = term.CreatedBy,
                DateCreated = term.DateCreated,
                LastModified = term.LastModified,
                ModifiedBy = term.ModifiedBy,
                OrgBranchName = term.OrgBranchName,
                Remark = term.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terms_Create([DataSourceRequest]DataSourceRequest request, Term term)
        {
            List<Term> holidayDefinitions = db.Terms.Where(s => s.LookupId == term.LookupId && s.VoucherDefinitionID==term.VoucherDefinitionID).ToList();
              if (holidayDefinitions.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "Term with Voucher Definition Already Exist");
              }
              else
              {
                  if (ModelState.IsValid)
                  {
                      var entity = new Term
                      {
                          VoucherDefinitionID = term.VoucherDefinitionID,
                          CategoryDescription = term.CategoryDescription,
                          
                         LookupId = term.LookupId,
                          DefaultValue = term.DefaultValue,
                          IsMandatory = term.IsMandatory,
                          CreatedBy = User.Identity.Name,
                          DateCreated = DateTime.Now,
                          Remark = term.Remark
                      };

                      db.Terms.Add(entity);
                      db.SaveChanges();
                      term.TermID = entity.TermID;
                  }
              }

            return Json(new[] { term }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terms_Update([DataSourceRequest]DataSourceRequest request, Term term)
        {
            List<Term> holidayDefinitions = db.Terms.Where(s => s.VoucherDefinitionID == term.VoucherDefinitionID && s.TermID!=term.TermID).ToList();
              if (holidayDefinitions.Any())
              {
                  ModelState.AddModelError("DuplicateEntire", "Term with Voucher Definition Already Exist");
              }
              else
              {
                  if (ModelState.IsValid)
                  {
                      var entity = new Term
                      {
                          TermID = term.TermID,
                          VoucherDefinitionID = term.VoucherDefinitionID,
                          LookupId = (term.LookupId),
                          DefaultValue = term.DefaultValue,
                          IsMandatory = term.IsMandatory,
                          Remark = term.Remark
                      };

                      db.Terms.Attach(entity);
                      db.Entry(entity).State = EntityState.Modified;
                      db.SaveChanges();
                  }
              }
            return Json(new[] { term }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Terms_Destroy([DataSourceRequest]DataSourceRequest request, Term term)
        {
            if (ModelState.IsValid)
            {
                var entity = new Term
                {
                    TermID = term.TermID,
                    VoucherDefinitionID = term.VoucherDefinitionID,
                    LookupId = term.LookupId,
                    DefaultValue = term.DefaultValue,
                    IsMandatory = term.IsMandatory,
                    Remark = term.Remark
                };

                db.Terms.Attach(entity);
                db.Terms.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Tax number " + term.TermID);
      
            }

            return Json(new[] { term }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
