using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApCashierSetupController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            
            var branches = _db.Branch;
            ViewData["Branches"] = branches;
        }
        public ActionResult CashierSetups_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApCashierSetup> cashiersetups = _db.CashierSetups;
            DataSourceResult result = cashiersetups.ToDataSourceResult(request, apCashierSetup => new {
                ApCashierSetupId = apCashierSetup.ApCashierSetupId,
                Cashier = apCashierSetup.Cashier,
                Description = apCashierSetup.Description,
                BranchId = apCashierSetup.BranchId,
                AccountCode = apCashierSetup.AccountCode,
                IsActive = apCashierSetup.IsActive,
                ActiveInactiveTime = apCashierSetup.ActiveInactiveTime,
                DateCreated = apCashierSetup.DateCreated,
                LastModified = apCashierSetup.LastModified,
                CreatedBy = apCashierSetup.CreatedBy,
                ModifiedBy = apCashierSetup.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashierSetups_Create([DataSourceRequest]DataSourceRequest request, ApCashierSetup apCashierSetup)
        {

            #region get sub account description

            var segmentSetup = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);

            if (segmentSetup != null)
            {
                var subAccounts = _db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == segmentSetup.GLSegmentSetupId)
                    .FirstOrDefault(x => x.Values == apCashierSetup.Cashier);
                apCashierSetup.Description = subAccounts?.Description;
            }


            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApCashierSetup
                {
                    Cashier = apCashierSetup.Cashier,
                    Description = apCashierSetup.Description,
                    BranchId = apCashierSetup.BranchId,
                    AccountCode = apCashierSetup.AccountCode,
                    IsActive = apCashierSetup.IsActive,
                    ActiveInactiveTime = apCashierSetup.ActiveInactiveTime,
                    DateCreated = apCashierSetup.DateCreated,
                    LastModified = apCashierSetup.LastModified,
                    CreatedBy = apCashierSetup.CreatedBy,
                    ModifiedBy = apCashierSetup.ModifiedBy
                };

                _db.CashierSetups.Add(entity);
                _db.SaveChanges();
                apCashierSetup.ApCashierSetupId = entity.ApCashierSetupId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Cashier type", "Success", User.Identity.Name, $"created  {apCashierSetup.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apCashierSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashierSetups_Update([DataSourceRequest]DataSourceRequest request, ApCashierSetup apCashierSetup)
        {
            #region get sub account description

            var segmentSetup = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);

            if (segmentSetup != null)
            {
                var subAccounts = _db.GlChartOfAccountSegments
                    .Where(x => x.GLSegmentSetupId == segmentSetup.GLSegmentSetupId)
                    .FirstOrDefault(x => x.Values == apCashierSetup.Cashier);
                apCashierSetup.Description = subAccounts?.Description;
            }


            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApCashierSetup
                {
                    ApCashierSetupId = apCashierSetup.ApCashierSetupId,
                    Cashier = apCashierSetup.Cashier,
                    Description = apCashierSetup.Description,
                    BranchId = apCashierSetup.BranchId,
                    AccountCode = apCashierSetup.AccountCode,
                    IsActive = apCashierSetup.IsActive,
                    ActiveInactiveTime = apCashierSetup.ActiveInactiveTime,
                    DateCreated = apCashierSetup.DateCreated,
                    LastModified = apCashierSetup.LastModified,
                    CreatedBy = apCashierSetup.CreatedBy,
                    ModifiedBy = apCashierSetup.ModifiedBy
                };

                _db.CashierSetups.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Cashier type", "Success", User.Identity.Name, $"updated  {apCashierSetup.AccountCode}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apCashierSetup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashierSetups_Destroy([DataSourceRequest]DataSourceRequest request, ApCashierSetup apCashierSetup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApCashierSetup
                {
                    ApCashierSetupId = apCashierSetup.ApCashierSetupId,
                    Cashier = apCashierSetup.Cashier,
                    Description = apCashierSetup.Description,
                    BranchId = apCashierSetup.BranchId,
                    AccountCode = apCashierSetup.AccountCode,
                    IsActive = apCashierSetup.IsActive,
                    ActiveInactiveTime = apCashierSetup.ActiveInactiveTime,
                    DateCreated = apCashierSetup.DateCreated,
                    LastModified = apCashierSetup.LastModified,
                    CreatedBy = apCashierSetup.CreatedBy,
                    ModifiedBy = apCashierSetup.ModifiedBy
                };

                _db.CashierSetups.Attach(entity);
                _db.CashierSetups.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted cashier setup " + apCashierSetup.ApCashierSetupId + " " + apCashierSetup.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { apCashierSetup }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var item = _db.CashierSetups.FirstOrDefault(p => p.ApCashierSetupId == did);
                if (item != null && item.IsActive)
                {
                    item.IsActive = false;
                    item.ActiveInactiveTime=DateTime.Now;
                    #region disable log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Disabled cashier  " + item.Cashier);
                    #endregion
                }
                else
                {
                    if (item != null)
                    {
                        item.IsActive = true;
                        item.ActiveInactiveTime = DateTime.Now;
                        #region enable log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Enable", "Success", User.Identity.Name, "Enabled cashier  " + item.Cashier);
                        #endregion
                    }
                }
                if (item != null)
                {
                    _db.CashierSetups.Attach(item);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();
                }

              
            }

            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
