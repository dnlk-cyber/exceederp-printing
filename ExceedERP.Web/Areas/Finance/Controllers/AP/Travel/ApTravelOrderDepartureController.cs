using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelOrderDepartureController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelOrderDepartures_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderDeparture> aptravelorderdepartures = _db.ApTravelOrderDepartures.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = aptravelorderdepartures.ToDataSourceResult(request, apTravelOrderDeparture => new {
                ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                Type = apTravelOrderDeparture.Type,
                From = apTravelOrderDeparture.From,
                To = apTravelOrderDeparture.To,
                Date = apTravelOrderDeparture.Date,
                ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                Duration = apTravelOrderDeparture.Duration,
                DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                TransportMode = apTravelOrderDeparture.TransportMode,
                Remark = apTravelOrderDeparture.Remark,
                OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                DateCreated = apTravelOrderDeparture.DateCreated,
                LastModified = apTravelOrderDeparture.LastModified,
                CreatedBy = apTravelOrderDeparture.CreatedBy,
                ModifiedBy = apTravelOrderDeparture.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartures_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture)
        {
            if (apTravelOrderDeparture.ArrivalDate < apTravelOrderDeparture.Date)
                ModelState.AddModelError("start", "Start date should be before arrival date");

            apTravelOrderDeparture.Duration = (decimal)(apTravelOrderDeparture.ArrivalDate - apTravelOrderDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {
                    ApTravelOrderId=id,
                    Type = apTravelOrderDeparture.Type,
                    From = apTravelOrderDeparture.From,
                    To = apTravelOrderDeparture.To,
                    Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    TransportMode = apTravelOrderDeparture.TransportMode,
                    Remark = apTravelOrderDeparture.Remark,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                _db.ApTravelOrderDepartures.Add(entity);
                _db.SaveChanges();
                apTravelOrderDeparture.ApTravelOrderDepartureId = entity.ApTravelOrderDepartureId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order departure item", "Success", User.Identity.Name, $"created departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartures_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture)
        {
            if (apTravelOrderDeparture.ArrivalDate < apTravelOrderDeparture.Date)
                ModelState.AddModelError("start", "Start date should be before arrival date");

            apTravelOrderDeparture.Duration = (decimal)(apTravelOrderDeparture.ArrivalDate - apTravelOrderDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {
                    ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                    Type = apTravelOrderDeparture.Type,
                    From = apTravelOrderDeparture.From,
                    To = apTravelOrderDeparture.To,
                    Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    TransportMode = apTravelOrderDeparture.TransportMode,
                    Remark = apTravelOrderDeparture.Remark,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                _db.ApTravelOrderDepartures.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order departure item", "Success", User.Identity.Name, $"updated departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartures_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {
                    ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                    Type = apTravelOrderDeparture.Type,
                    From = apTravelOrderDeparture.From,
                    To = apTravelOrderDeparture.To,
                    Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    TransportMode = apTravelOrderDeparture.TransportMode,
                    Remark = apTravelOrderDeparture.Remark,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                _db.ApTravelOrderDepartures.Attach(entity);
                _db.ApTravelOrderDepartures.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted travel order departure item " + apTravelOrderDeparture.ApTravelOrderId + " " + apTravelOrderDeparture.From, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }
        

      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
