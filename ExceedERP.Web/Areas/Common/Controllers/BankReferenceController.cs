﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
﻿using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    public class BankReferenceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var bankList = db.Banks;
            ViewData["BankList"] = bankList != null ? bankList.ToList() : new List<Bank>();
            return View();
        }
   
        public ActionResult BankReferences_Read([DataSourceRequest]DataSourceRequest request, string  id, string bankReferenceType)
        {
            IQueryable<BankReference> bankreferences = db.BankReferences.Where(br => br.ReferenceType == bankReferenceType && br.Reference == id.ToString()).AsQueryable();
            DataSourceResult result = bankreferences.ToDataSourceResult(request, bankReference => new {
                BankReferenceID = bankReference.BankReferenceId,
                BankId = bankReference.BankId,
                ReferenceType = bankReference.ReferenceType,
                Reference = bankReference.Reference,
                Type = bankReference.Type,
                AccountNumber = bankReference.AccountNumber,
                Remark = bankReference.Remark
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Create([DataSourceRequest]DataSourceRequest request, BankReference bankReference, string id, string bankReferenceType)
        {
            if (ModelState.IsValid)
            {
                var checkexistance = db.BankReferences.Where(x => x.BankId == bankReference .BankId && x.Reference == id.ToString() && x.ReferenceType== bankReferenceType && x.Type == bankReference .Type ).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "This Data is already registered!!");
                }
                else
                {
                    var entity = new BankReference
                    {
                        BankId = bankReference.BankId,
                        ReferenceType = bankReferenceType,
                        Reference = id.ToString(),
                        Type = bankReference.Type,
                        AccountNumber = bankReference.AccountNumber,
                        Remark = bankReference.Remark
                    };
                    db.BankReferences.Add(entity);
                    db.SaveChanges();
                    bankReference.BankReferenceId = entity.BankReferenceId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll employee bank account", "Success", User.Identity.Name, $"added account  {entity.AccountNumber} for employee {entity.Reference} type {entity.Type}  {entity.Remark} ", Modules.Payroll);
                    #endregion
                }
            }

            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Update([DataSourceRequest]DataSourceRequest request, BankReference bankReference)
        {
            if (ModelState.IsValid)
            {

                var entity = new BankReference
                {
                    BankReferenceId = bankReference.BankReferenceId,
                    BankId = bankReference.BankId,
                    ReferenceType = bankReference.ReferenceType,
                    Reference = bankReference.Reference,
                    Type = bankReference.Type,
                    AccountNumber = bankReference.AccountNumber,
                    Remark = bankReference.Remark
                };

                db.BankReferences.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll employee bank account", "Success", User.Identity.Name, $"updated account  {entity.AccountNumber} for employee {entity.Reference} type {entity.Type}  {entity.Remark} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankReferences_Destroy([DataSourceRequest]DataSourceRequest request, BankReference bankReference)
        {
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.BankAccount == bankReference.AccountNumber).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You cannot delete this account. Because it already processed!!");
                }
                else
                {
                    var entity = new BankReference
                    {
                        BankReferenceId = bankReference.BankReferenceId,
                        BankId = bankReference.BankId,
                        ReferenceType = bankReference.ReferenceType,
                        Reference = bankReference.Reference,
                        Type = bankReference.Type,
                        AccountNumber = bankReference.AccountNumber,
                        Remark = bankReference.Remark
                    };
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "payroll employee bank account", "Success", User.Identity.Name, $"deleted account  {entity.AccountNumber} for employee {entity.Reference} type {entity.Type}  {entity.Remark} ", Modules.Payroll);
                    #endregion
                    db.BankReferences.Attach(entity);
                    db.BankReferences.Remove(entity);
                    db.SaveChanges();
                   
                }
            }
            return Json(new[] { bankReference }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
