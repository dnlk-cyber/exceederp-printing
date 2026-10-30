using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using ExceedERP.Web.Models;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Termination
{

    [AuthorizeRoles(HrmConstants.HRTerminationUser, HrmConstants.HRAdmin)]
    public class StaffDebtorBalanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult StaffDebtorBalances_Read([DataSourceRequest] DataSourceRequest request, int employeeId, int terminationId)
        {
            db.Database.CommandTimeout = 7200;
            IQueryable<StaffDebtorBalance> balances = db.StaffDebtorBalances.Where(x => x.EmployeeId == employeeId&&x.TerminationId==terminationId);

            DataSourceResult result = balances.ToDataSourceResult(request, item => new
            {
                StaffDebtorBalanceId = item.StaffDebtorBalanceId,
                EmployeeId = item.EmployeeId,
                TerminationId = item.TerminationId,
                AccountCode = item.AccountCode,
                Description = item.Description,
                Debit = item.Debit,
                Credit = item.Credit,
                Balance = item.Balance ?? ((item.Debit ?? 0) - (item.Credit ?? 0)),
                Remark = item.Remark,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StaffDebtorBalances_Create([DataSourceRequest] DataSourceRequest request, StaffDebtorBalance balance, int employeeId, int? fileOwner)
        {
            ModelState.Clear();

            if (balance.Debit == null && balance.Credit == null)
            {
                ModelState.AddModelError("", "Either Debit or Credit amount is required.");
                return Json(new[] { balance }.ToDataSourceResult(request, ModelState));
            }

            if (ModelState.IsValid)
            {
                var entity = new StaffDebtorBalance
                {
                    EmployeeId = employeeId,
                    TerminationId = fileOwner ?? balance.TerminationId,
                    AccountCode = balance.AccountCode,
                    Description = balance.Description,
                    Debit = balance.Debit,
                    Credit = balance.Credit,
                    Balance = (balance.Debit ?? 0) - (balance.Credit ?? 0),
                    Remark = balance.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.StaffDebtorBalances.Add(entity);
                db.SaveChanges();

                balance.StaffDebtorBalanceId = entity.StaffDebtorBalanceId;

                #region Operation Log
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "StaffDebtorBalance",
                    "Success",
                    User.Identity.Name,
                    $"Created Staff Debtor Balance (ID: {entity.StaffDebtorBalanceId}) for Employee ID: {employeeId}",
                    Modules.HRM
                );
                #endregion
            }

            return Json(new[] { balance }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StaffDebtorBalances_Update([DataSourceRequest] DataSourceRequest request, StaffDebtorBalance balance)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.StaffDebtorBalances.FirstOrDefault(x => x.StaffDebtorBalanceId == balance.StaffDebtorBalanceId);
                if (entity != null)
                {
                    entity.AccountCode = balance.AccountCode;
                    entity.Description = balance.Description;
                    entity.Debit = balance.Debit;
                    entity.Credit = balance.Credit;
                    entity.Balance = (balance.Debit ?? 0) - (balance.Credit ?? 0);
                    entity.Remark = balance.Remark;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region Operation Log
                    UserHelper.OperationLog(Request.UserHostAddress,"StaffDebtorBalance","Success",User.Identity.Name,
                        $"Updated Staff Debtor Balance (ID: {entity.StaffDebtorBalanceId})",
                        Modules.HRM
                    );
                    #endregion
                }
                else
                {
                    ModelState.AddModelError("", "Record not found.");
                }
            }

            return Json(new[] { balance }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin, HrmConstants.HRTerminationUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StaffDebtorBalances_Destroy([DataSourceRequest] DataSourceRequest request, StaffDebtorBalance balance)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = db.StaffDebtorBalances.FirstOrDefault(x => x.StaffDebtorBalanceId == balance.StaffDebtorBalanceId);
                if (entity != null)
                {
                    #region Delete Log
                    UserHelper.DeleteLog(
                        Request.UserHostAddress,
                        "Delete",
                        "Success",
                        User.Identity.Name,
                        $"Deleted Staff Debtor Balance record (ID: {entity.StaffDebtorBalanceId}) for Employee ID: {entity.EmployeeId}"
                    );
                    #endregion

                    db.StaffDebtorBalances.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { balance }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }

}
