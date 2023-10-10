using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
     [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class InventoryPeriodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            UpdatePeriodList();
            return View();
        }

        private void UpdatePeriodList()
        {
            var glperiods = _db.GLPeriods.ToList();
            foreach (var period in glperiods)
            {
                var exist = _db.InventoryPeriods.FirstOrDefault(x => x.Name == period.Name);
                if (exist == null)
                {
                    var entity = new InventoryPeriod
                    {
                        Name = period.Name,
                        DateFrom = period.DateFrom.Date,
                        DateTo = period.DateTo.Date,
                        Status = period.Status,
                        OrgBranchName = period.OrgBranchName,
                        DateCreated = period.DateCreated,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy
                    };
                    _db.InventoryPeriods.Add(entity);
                    _db.SaveChanges();
                }
            }
        }

        public ActionResult InventoryPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {


            IQueryable<InventoryPeriod> inventoryPeriods = _db.InventoryPeriods;
            DataSourceResult result = inventoryPeriods.ToDataSourceResult(request, gLPeriod => new
            {
               
                InventoryPeriodId = gLPeriod.InventoryPeriodId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                Status = gLPeriod.Status,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }


        public ActionResult ClosePeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.InventoryPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status == FiscalYearStatus.Open)
                {
                    glPeriod.Status = FiscalYearStatus.Close;
                    _db.InventoryPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                   // _db.SaveChanges();

                    #region apply to all stores
                    var storePeriods=_db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == glPeriod.InventoryPeriodId).ToList();
                    foreach ( var storePeriod in storePeriods )
                    {
                        storePeriod.Status = FiscalYearStatus.Close;
                        _db.InventoryPeriods.Attach(glPeriod);
                        _db.Entry(glPeriod).State = EntityState.Modified;
                        
                    }
                    #endregion

                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Inventory Period Close", "Success", User.Identity.Name, "Period " + glPeriod.Name,Modules.Inventory);
                    #endregion
                }


            }

            return RedirectToAction("Index");

        }

        public ActionResult OpenPeriod(string id)
        {
            if (id != null)
            {
                var glPeriod = _db.InventoryPeriods.Find(int.Parse(id));
                if (glPeriod != null && glPeriod.Status != FiscalYearStatus.PermanetlyClosed)
                {
                    glPeriod.Status = FiscalYearStatus.Open;
                    _db.InventoryPeriods.Attach(glPeriod);
                    _db.Entry(glPeriod).State = EntityState.Modified;
                    //_db.SaveChanges();

                    #region apply to all stores
                    var storePeriods = _db.InventoryStorePeriodStatuses
                    .Where(x => x.InventoryPeriodId == glPeriod.InventoryPeriodId).ToList();
                    foreach (var storePeriod in storePeriods)
                    {
                        storePeriod.Status = FiscalYearStatus.Open;
                        _db.InventoryPeriods.Attach(glPeriod);
                        _db.Entry(glPeriod).State = EntityState.Modified;

                    }
                    #endregion

                    _db.SaveChanges();

                    #region  log
                    UserHelper.OperationLog(Request.UserHostAddress, "Inventory Period Open", "Success", User.Identity.Name, "Period " + glPeriod.Name, Modules.Inventory);
                    #endregion
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
