﻿using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class BankAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BankAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BankAccount> bankaccounts = _db.BankAccounts;
            DataSourceResult result = bankaccounts.ToDataSourceResult(request, bankAccount => new
            {
                BankAccountId = bankAccount.BankAccountId,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAdress = bankAccount.BankAdress,
                AccountCode = bankAccount.AccountCode,
                AccountDesc = bankAccount.AccountDesc,
                AccountControl = bankAccount.AccountControl,
                AccountUse = bankAccount.AccountUse,
                Status = bankAccount.Status,
                OrgBranchName = bankAccount.OrgBranchName,
                DateCreated = bankAccount.DateCreated,
                LastModified = bankAccount.LastModified,
                CreatedBy = bankAccount.CreatedBy,
                ModifiedBy = bankAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccounts_Create([DataSourceRequest]DataSourceRequest request, BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccount
                {
                    AccountName = bankAccount.AccountName,
                    AccountNumber = bankAccount.AccountNumber,
                    BankName = bankAccount.BankName,
                    BankBranch = bankAccount.BankBranch,
                    BankAdress = bankAccount.BankAdress,
                    AccountCode = bankAccount.AccountCode,
                    AccountDesc = bankAccount.AccountDesc,
                    AccountControl = bankAccount.AccountControl,
                    AccountUse = bankAccount.AccountUse,
                    Status = bankAccount.Status,
                    OrgBranchName = bankAccount.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = bankAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = bankAccount.ModifiedBy
                };

                _db.BankAccounts.Add(entity);
                _db.SaveChanges();
                bankAccount.BankAccountId = entity.BankAccountId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank Account", "Success", User.Identity.Name, $"created account {entity.AccountCode}  {entity.AccountNumber}", Modules.Finance);
                #endregion
            }

            return Json(new[] { bankAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccounts_Update([DataSourceRequest]DataSourceRequest request, BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccount
                {
                    BankAccountId = bankAccount.BankAccountId,
                    AccountName = bankAccount.AccountName,
                    AccountNumber = bankAccount.AccountNumber,
                    BankName = bankAccount.BankName,
                    BankBranch = bankAccount.BankBranch,
                    BankAdress = bankAccount.BankAdress,
                    AccountCode = bankAccount.AccountCode,
                    AccountDesc = bankAccount.AccountDesc,
                    AccountControl = bankAccount.AccountControl,
                    AccountUse = bankAccount.AccountUse,
                    Status = bankAccount.Status,
                    OrgBranchName = bankAccount.OrgBranchName,
                    DateCreated = bankAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = bankAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.BankAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                bankAccount.BankAccountId = entity.BankAccountId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Bank Account", "Success", User.Identity.Name, $"modified account {entity.AccountCode}  {entity.AccountNumber}", Modules.Finance);
                #endregion
            }

            return Json(new[] { bankAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BankAccounts_Destroy([DataSourceRequest]DataSourceRequest request, BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new BankAccount
                {
                    BankAccountId = bankAccount.BankAccountId,
                    AccountName = bankAccount.AccountName,
                    AccountNumber = bankAccount.AccountNumber,
                    BankName = bankAccount.BankName,
                    BankBranch = bankAccount.BankBranch,
                    BankAdress = bankAccount.BankAdress,
                    AccountCode = bankAccount.AccountCode,
                    AccountDesc = bankAccount.AccountDesc,
                    AccountControl = bankAccount.AccountControl,
                    AccountUse = bankAccount.AccountUse,
                    Status = bankAccount.Status,
                    OrgBranchName = bankAccount.OrgBranchName,
                    DateCreated = bankAccount.DateCreated,
                    LastModified = bankAccount.LastModified,
                    CreatedBy = bankAccount.CreatedBy,
                    ModifiedBy = bankAccount.ModifiedBy
                };

                _db.BankAccounts.Attach(entity);
                _db.BankAccounts.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted bank account " + bankAccount.AccountNumber + " " + bankAccount.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { bankAccount }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
