using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    [Authorize]
    public class TravelClaimDepartureBudgetController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Finance/APTravelClaimDeparture
        public ActionResult Index()
        {
           
            return View();
        }
       
        public ActionResult TravelClaimDepartures_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<APTravelClaimDeparture> APTravelClaimDepartures = db.APTravelClaimDepartures.Where(x => x.ApTravelExpenseClaimId == id);
            DataSourceResult result = APTravelClaimDepartures.ToDataSourceResult(request, APTravelClaimDeparture => new {
                APTravelClaimDepartureId = APTravelClaimDeparture.APTravelClaimDepartureId,
                ApTravelExpenseClaimId = APTravelClaimDeparture.ApTravelExpenseClaimId,
                Type = APTravelClaimDeparture.Type,
                From = APTravelClaimDeparture.From,
                To = APTravelClaimDeparture.To,
                Date = APTravelClaimDeparture.Date,
                ArrivalDate = APTravelClaimDeparture.ArrivalDate,
                Duration = APTravelClaimDeparture.Duration,
                DestinationBranchId = APTravelClaimDeparture.DestinationBranchId,
                TransportMode = APTravelClaimDeparture.TransportMode,
                Remark = APTravelClaimDeparture.Remark,
                OrgBranchName = APTravelClaimDeparture.OrgBranchName,
                DateCreated = APTravelClaimDeparture.DateCreated,
                LastModified = APTravelClaimDeparture.LastModified,
                CreatedBy = APTravelClaimDeparture.CreatedBy,
                ModifiedBy = APTravelClaimDeparture.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimDepartures_Create([DataSourceRequest]DataSourceRequest request, APTravelClaimDeparture APTravelClaimDeparture, int id)
        {
            #region validation
           

            if (APTravelClaimDeparture.ArrivalDate < APTravelClaimDeparture.Date)
                ModelState.AddModelError("start","Start date should be before arrival date");
                     

            var departures = db.APTravelClaimDepartures.Where(x => x.ApTravelExpenseClaimId==id).ToList();

            bool overlap = departures.Any(x =>
                (APTravelClaimDeparture.Date < x.ArrivalDate && APTravelClaimDeparture.ArrivalDate > x.Date) || // Original overlap check
                (APTravelClaimDeparture.Date >= x.Date && APTravelClaimDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            );

            if (overlap)
            {
                ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            }

            //var prevtravelOrders = db.ApTravelOrders
            //    .Where(x => x.ApTravelExpenseClaimId != id)
            //    .Where(x => x.EmployeeId == travelorder.EmployeeId)
            //    .Where(x => x.IsVoid == false)
            //    .Select(x => x.ApTravelExpenseClaimId)
            //    .ToList();

            //var departures=db.APTravelClaimDepartures.Where(x => prevtravelOrders.Contains(x.ApTravelExpenseClaimId)).ToList();

            //bool overlap = departures.Any(x =>
            //    (APTravelClaimDeparture.Date < x.ArrivalDate && APTravelClaimDeparture.ArrivalDate > x.Date) || // Original overlap check
            //    (APTravelClaimDeparture.Date >= x.Date && APTravelClaimDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            //);

            //if (overlap)
            //{
            //    ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            //}

            //var travelDetails = db.APTravelClaimDepartures
            //.Where(t => prevtravelOrders.Contains(t.ApTravelExpenseClaimId))
            //.GroupBy(t => t.ApTravelExpenseClaimId)
            // .Select(g => new
            // {
            //     TravelOrderId = g.Key,
            //     DepartureDate = g.Where(x => x.Type == DepartureType.Departure).Select(x => x.Date).FirstOrDefault(),
            //     ReturnDate = g.Where(x => x.Type == DepartureType.Return).Select(x => x.ArrivalDate).FirstOrDefault()
            // })
            //.ToList();

            // bool overlapacross = travelDetails.Any(x =>              
            //   (APTravelClaimDeparture.Date >= x.DepartureDate && APTravelClaimDeparture.ArrivalDate <= x.ReturnDate) 
            // );

            //if (overlapacross)
            //{
            //    ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            //}


            #endregion
            APTravelClaimDeparture.Duration = (decimal)(APTravelClaimDeparture.ArrivalDate - APTravelClaimDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {        

                                
                var entity = new APTravelClaimDeparture
                {                 
                   
                   ApTravelExpenseClaimId = id,
                   From = APTravelClaimDeparture.From,
                   To = APTravelClaimDeparture.To,
                   Date = APTravelClaimDeparture.Date,
                    ArrivalDate = APTravelClaimDeparture.ArrivalDate,
                    Duration = APTravelClaimDeparture.Duration,
                    DestinationBranchId = APTravelClaimDeparture.DestinationBranchId,
                    Type = APTravelClaimDeparture.Type,
                   TransportMode = APTravelClaimDeparture.TransportMode,
                    OrgBranchName = APTravelClaimDeparture.OrgBranchName,
                    DateCreated = APTravelClaimDeparture.DateCreated,
                    LastModified = APTravelClaimDeparture.LastModified,
                    CreatedBy = APTravelClaimDeparture.CreatedBy,
                    ModifiedBy = APTravelClaimDeparture.ModifiedBy
                };

                db.APTravelClaimDepartures.Add(entity);
                db.SaveChanges();
                APTravelClaimDeparture.APTravelClaimDepartureId = entity.APTravelClaimDepartureId;
                UpdatePerDiemQty(APTravelClaimDeparture.ApTravelExpenseClaimId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel claim departure item", "Success", User.Identity.Name, $"created departure item {entity.Type} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { APTravelClaimDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimDepartures_Update([DataSourceRequest]DataSourceRequest request, APTravelClaimDeparture APTravelClaimDeparture)
        {
            if (APTravelClaimDeparture.ArrivalDate < APTravelClaimDeparture.Date)
                ModelState.AddModelError("start", "Start date should be before arrival date");


            var departures = db.APTravelClaimDepartures
                .Where(x=>x.APTravelClaimDepartureId!= APTravelClaimDeparture.APTravelClaimDepartureId)
                .Where(x => x.ApTravelExpenseClaimId == APTravelClaimDeparture.ApTravelExpenseClaimId)
                .ToList();

            bool overlap = departures.Any(x =>
                (APTravelClaimDeparture.Date < x.ArrivalDate && APTravelClaimDeparture.ArrivalDate > x.Date) || // Original overlap check
                (APTravelClaimDeparture.Date >= x.Date && APTravelClaimDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            );

            if (overlap)
            {
                ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            }
          

            APTravelClaimDeparture.Duration = (decimal)(APTravelClaimDeparture.ArrivalDate - APTravelClaimDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {
                var entity = new APTravelClaimDeparture
                {

                    APTravelClaimDepartureId = APTravelClaimDeparture.APTravelClaimDepartureId,
                    ApTravelExpenseClaimId = APTravelClaimDeparture.ApTravelExpenseClaimId,
                    From = APTravelClaimDeparture.From,
                    To = APTravelClaimDeparture.To,
                    Date = APTravelClaimDeparture.Date,
                    ArrivalDate = APTravelClaimDeparture.ArrivalDate,
                    Duration = APTravelClaimDeparture.Duration,
                    DestinationBranchId = APTravelClaimDeparture.DestinationBranchId,
                    Type = APTravelClaimDeparture.Type,
                    TransportMode = APTravelClaimDeparture.TransportMode,
                    OrgBranchName = APTravelClaimDeparture.OrgBranchName,
                    DateCreated = APTravelClaimDeparture.DateCreated,
                    LastModified = APTravelClaimDeparture.LastModified,
                    CreatedBy = APTravelClaimDeparture.CreatedBy,
                    ModifiedBy = APTravelClaimDeparture.ModifiedBy
                };

                db.APTravelClaimDepartures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UpdatePerDiemQty(APTravelClaimDeparture.ApTravelExpenseClaimId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel claim departure item", "Success", User.Identity.Name, $"updted departure item {entity.Type} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { APTravelClaimDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimDepartures_Destroy([DataSourceRequest]DataSourceRequest request, APTravelClaimDeparture APTravelClaimDeparture)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new APTravelClaimDeparture
                {

                    APTravelClaimDepartureId = APTravelClaimDeparture.APTravelClaimDepartureId,
                    ApTravelExpenseClaimId = APTravelClaimDeparture.ApTravelExpenseClaimId,
                    From = APTravelClaimDeparture.From,
                    To = APTravelClaimDeparture.To,
                    Date = APTravelClaimDeparture.Date,
                    ArrivalDate = APTravelClaimDeparture.ArrivalDate,
                    Duration = APTravelClaimDeparture.Duration,
                    DestinationBranchId = APTravelClaimDeparture.DestinationBranchId,
                    Type = APTravelClaimDeparture.Type,
                    TransportMode = APTravelClaimDeparture.TransportMode,
                    OrgBranchName = APTravelClaimDeparture.OrgBranchName,
                    DateCreated = APTravelClaimDeparture.DateCreated,
                    LastModified = APTravelClaimDeparture.LastModified,
                    CreatedBy = APTravelClaimDeparture.CreatedBy,
                    ModifiedBy = APTravelClaimDeparture.ModifiedBy
                };

                db.APTravelClaimDepartures.Attach(entity);
                db.APTravelClaimDepartures.Remove(entity);
                db.SaveChanges();
                UpdatePerDiemQty(APTravelClaimDeparture.ApTravelExpenseClaimId);
            }

            return Json(new[] { APTravelClaimDeparture }.ToDataSourceResult(request, ModelState));
        }
        
        private void UpdatePerDiemQty(int travelid)
        {

            #region check auto calculate
           
            #endregion
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}