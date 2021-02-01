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
    public class EmployeeSubAccountController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;



        }

        public ActionResult EmployeeSubAccounts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<EmployeeSubAccount> employeesubaccounts = db.EmployeeSubAccounts;
            DataSourceResult result = employeesubaccounts.ToDataSourceResult(request, employeeSubAccount => new {
                EmployeeSubAccountId = employeeSubAccount.EmployeeSubAccountId,
                EmployeeId = employeeSubAccount.EmployeeId,
                SubAccountCode = employeeSubAccount.SubAccountCode,
                Enable = employeeSubAccount.Enable,
                OrgBranchName = employeeSubAccount.OrgBranchName,
                DateCreated = employeeSubAccount.DateCreated,
                LastModified = employeeSubAccount.LastModified,
                CreatedBy = employeeSubAccount.CreatedBy,
                ModifiedBy = employeeSubAccount.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSubAccounts_Create([DataSourceRequest]DataSourceRequest request, EmployeeSubAccount employeeSubAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeSubAccount
                {
                    EmployeeId = employeeSubAccount.EmployeeId,
                    SubAccountCode = employeeSubAccount.SubAccountCode,
                    Enable = employeeSubAccount.Enable,
                    OrgBranchName = employeeSubAccount.OrgBranchName,
                    DateCreated = employeeSubAccount.DateCreated,
                    LastModified = employeeSubAccount.LastModified,
                    CreatedBy = employeeSubAccount.CreatedBy,
                    ModifiedBy = employeeSubAccount.ModifiedBy
                };

                db.EmployeeSubAccounts.Add(entity);
                db.SaveChanges();
                employeeSubAccount.EmployeeSubAccountId = entity.EmployeeSubAccountId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee sub account setup", "Success", User.Identity.Name, $"created sub account {entity.SubAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { employeeSubAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSubAccounts_Update([DataSourceRequest]DataSourceRequest request, EmployeeSubAccount employeeSubAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeSubAccount
                {
                    EmployeeSubAccountId = employeeSubAccount.EmployeeSubAccountId,
                    EmployeeId = employeeSubAccount.EmployeeId,
                    SubAccountCode = employeeSubAccount.SubAccountCode,
                    Enable = employeeSubAccount.Enable,
                    OrgBranchName = employeeSubAccount.OrgBranchName,
                    DateCreated = employeeSubAccount.DateCreated,
                    LastModified = employeeSubAccount.LastModified,
                    CreatedBy = employeeSubAccount.CreatedBy,
                    ModifiedBy = employeeSubAccount.ModifiedBy
                };

                db.EmployeeSubAccounts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee sub account setup", "Success", User.Identity.Name, $"updated sub account {entity.SubAccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { employeeSubAccount }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeSubAccounts_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeSubAccount employeeSubAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeSubAccount
                {
                    EmployeeSubAccountId = employeeSubAccount.EmployeeSubAccountId,
                    EmployeeId = employeeSubAccount.EmployeeId,
                    SubAccountCode = employeeSubAccount.SubAccountCode,
                    Enable = employeeSubAccount.Enable,
                    OrgBranchName = employeeSubAccount.OrgBranchName,
                    DateCreated = employeeSubAccount.DateCreated,
                    LastModified = employeeSubAccount.LastModified,
                    CreatedBy = employeeSubAccount.CreatedBy,
                    ModifiedBy = employeeSubAccount.ModifiedBy
                };

                db.EmployeeSubAccounts.Attach(entity);
                db.EmployeeSubAccounts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeSubAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
