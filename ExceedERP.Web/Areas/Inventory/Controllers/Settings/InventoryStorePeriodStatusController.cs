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
    public class InventoryStorePeriodStatusController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            
            return View();
        }

        private void UpdatePeriodList(string store)
        {
            var inventoryPeriods = _db.InventoryPeriods.ToList();
            foreach (var period in inventoryPeriods)
            {
                var exist = _db.InventoryStorePeriodStatuses
                    .Where(x=>x.InventoryPeriodId==period.InventoryPeriodId)
                    .FirstOrDefault(x => x.StoreCode == store);
                if (exist == null)
                {
                    var entity = new InventoryStorePeriodStatus
                    {
                        StoreCode = store,
                        InventoryPeriodId = period.InventoryPeriodId,
                        Status = period.Status,
                        DateCreated = period.DateCreated,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy
                    };
                    _db.InventoryStorePeriodStatuses.Add(entity);
                    _db.SaveChanges();
                }

            }
        }

        private string GetPeriodName(int id)
        {
            return _db.InventoryPeriods.Find(id)?.Name;
        }
        public ActionResult InventoryStorePeriodStatuses_Read([DataSourceRequest]DataSourceRequest request,string storecode)
        {
            UpdatePeriodList(storecode);

            IQueryable<InventoryStorePeriodStatus> inventoryStorePeriodStatuses = _db.InventoryStorePeriodStatuses.Where(x=>x.StoreCode==storecode).OrderByDescending(x=>x.InventoryPeriodId);
            DataSourceResult result = inventoryStorePeriodStatuses.ToDataSourceResult(request, period => new
            {
               
                InventoryStorePeriodStatusId = period.InventoryStorePeriodStatusId,
                StoreCode = period.StoreCode,
                InventoryPeriodId = period.InventoryPeriodId,
                PeriodName= GetPeriodName(period.InventoryPeriodId),
                Status = period.Status,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy
            });

            return Json(result);
        }


        public JsonResult CloseAllPeriod(string id)
        {
            if (id != null)
            {
                var periods = _db.InventoryStorePeriodStatuses.Where(x => x.Status != FiscalYearStatus.Close).Where(x => x.StoreCode == id).ToList();
                foreach (var period in periods)
                {
                    CloseStatus(period.InventoryStorePeriodStatusId);
                }

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory period", "Success", User.Identity.Name, $"closed all periods for store {id} ", Modules.Inventory);
                #endregion
            }

            var formattedData1 = new
            {
                Message = "Completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public JsonResult ClosePeriod(string id)
        {
            if (id != null)
            {
                CloseStatus(int.Parse(id));
            }

            var formattedData1 = new
            {
                Message = "Completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        private void CloseStatus(int id)
        {
            var glPeriod = _db.InventoryStorePeriodStatuses.Find(id);
            if (glPeriod != null)
            {
                glPeriod.Status = FiscalYearStatus.Close;
                _db.InventoryStorePeriodStatuses.Attach(glPeriod);
                _db.Entry(glPeriod).State = EntityState.Modified;
                _db.SaveChanges();

                #region  log

                UserHelper.OperationLog(Request.UserHostAddress, "Inventory Period Close", "Success", User.Identity.Name,
                    "Period " + glPeriod.StoreCode, Modules.Inventory);

                #endregion
            }
        }

        public JsonResult OpenAllPeriod(string id)
        {
            if (id != null)
            {
                var periods = _db.InventoryStorePeriodStatuses.Where(x => x.StoreCode == id).Where(x=>x.Status!=FiscalYearStatus.Open).ToList();
                foreach (var period in periods)
                {
                    OpenStatus(period.InventoryStorePeriodStatusId);
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory period", "Success", User.Identity.Name, $"opened all periods for store {id} ", Modules.Inventory);
                #endregion
            }

            var formattedData1 = new
            {
                Message = "Completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public JsonResult OpenPeriod(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var periodStatus = _db.InventoryStorePeriodStatuses.Find(did);
                if (periodStatus != null)
                {
                    var inventoryP = _db.InventoryPeriods.Find(periodStatus.InventoryPeriodId);
                    if (inventoryP != null && inventoryP.Status == FiscalYearStatus.Close)
                    {
                        var formattedData = new
                        {
                            Message = $"Inventory period {inventoryP.Name} is closed hence can't be open for this store.",
                            Status = "OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        OpenStatus(int.Parse(id));
                    }
                    
                }
            }

            var formattedData1 = new
            {
                Message = "Completed.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }

        private void OpenStatus(int id)
        {
            var glPeriod = _db.InventoryStorePeriodStatuses.Find(id);
            if (glPeriod != null)
            {
                glPeriod.Status = FiscalYearStatus.Open;
                _db.InventoryStorePeriodStatuses.Attach(glPeriod);
                _db.Entry(glPeriod).State = EntityState.Modified;
                _db.SaveChanges();

                #region  log

                UserHelper.OperationLog(Request.UserHostAddress, "Inventory Period Open", "Success", User.Identity.Name,
                    "Period " + glPeriod.StoreCode, Modules.Inventory);

                #endregion
            }
        }


        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
