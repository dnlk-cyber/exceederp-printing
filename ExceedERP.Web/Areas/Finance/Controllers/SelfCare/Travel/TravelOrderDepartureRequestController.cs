using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    [Authorize]
    public class TravelOrderDepartureRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Finance/ApTravelOrderDeparture
        public ActionResult Index()
        {
           
            return View();
        }
       
        public ActionResult TravelOrderDepartures_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderDeparture> aptravelorderdepartures = db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
            DataSourceResult result = aptravelorderdepartures.ToDataSourceResult(request, apTravelOrderDeparture => new {
                ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                ApTravelOrderId = apTravelOrderDeparture.ApTravelOrderId,
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
        public ActionResult TravelOrderDepartures_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture, int id)
        {
            #region validation
           

            if (apTravelOrderDeparture.ArrivalDate < apTravelOrderDeparture.Date)
                ModelState.AddModelError("start","Start date should be before arrival date");
                     

            var departures = db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId==id).ToList();

            bool overlap = departures.Any(x =>
                (apTravelOrderDeparture.Date < x.ArrivalDate && apTravelOrderDeparture.ArrivalDate > x.Date) || // Original overlap check
                (apTravelOrderDeparture.Date >= x.Date && apTravelOrderDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            );

            if (overlap)
            {
                ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            }

            //var prevtravelOrders = db.ApTravelOrders
            //    .Where(x => x.ApTravelOrderId != id)
            //    .Where(x => x.EmployeeId == travelorder.EmployeeId)
            //    .Where(x => x.IsVoid == false)
            //    .Select(x => x.ApTravelOrderId)
            //    .ToList();

            //var departures=db.ApTravelOrderDepartures.Where(x => prevtravelOrders.Contains(x.ApTravelOrderId)).ToList();

            //bool overlap = departures.Any(x =>
            //    (apTravelOrderDeparture.Date < x.ArrivalDate && apTravelOrderDeparture.ArrivalDate > x.Date) || // Original overlap check
            //    (apTravelOrderDeparture.Date >= x.Date && apTravelOrderDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            //);

            //if (overlap)
            //{
            //    ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            //}

            //var travelDetails = db.ApTravelOrderDepartures
            //.Where(t => prevtravelOrders.Contains(t.ApTravelOrderId))
            //.GroupBy(t => t.ApTravelOrderId)
            // .Select(g => new
            // {
            //     TravelOrderId = g.Key,
            //     DepartureDate = g.Where(x => x.Type == DepartureType.Departure).Select(x => x.Date).FirstOrDefault(),
            //     ReturnDate = g.Where(x => x.Type == DepartureType.Return).Select(x => x.ArrivalDate).FirstOrDefault()
            // })
            //.ToList();

            // bool overlapacross = travelDetails.Any(x =>              
            //   (apTravelOrderDeparture.Date >= x.DepartureDate && apTravelOrderDeparture.ArrivalDate <= x.ReturnDate) 
            // );

            //if (overlapacross)
            //{
            //    ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            //}


            #endregion
            apTravelOrderDeparture.Duration = (decimal)(apTravelOrderDeparture.ArrivalDate - apTravelOrderDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {        

                                
                var entity = new ApTravelOrderDeparture
                {                 
                   
                   ApTravelOrderId = id,
                   From = apTravelOrderDeparture.From,
                   To = apTravelOrderDeparture.To,
                   Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    Type = apTravelOrderDeparture.Type,
                   TravelOrder= apTravelOrderDeparture.TravelOrder,
                   TransportMode = apTravelOrderDeparture.TransportMode,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                db.ApTravelOrderDepartures.Add(entity);
                db.SaveChanges();
                apTravelOrderDeparture.ApTravelOrderDepartureId = entity.ApTravelOrderDepartureId;
                UpdatePerDiemQty(apTravelOrderDeparture.ApTravelOrderId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order departure item", "Success", User.Identity.Name, $"created departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderDepartures_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture)
        {
            if (apTravelOrderDeparture.ArrivalDate < apTravelOrderDeparture.Date)
                ModelState.AddModelError("start", "Start date should be before arrival date");


            var departures = db.ApTravelOrderDepartures
                .Where(x=>x.ApTravelOrderDepartureId!= apTravelOrderDeparture.ApTravelOrderDepartureId)
                .Where(x => x.ApTravelOrderId == apTravelOrderDeparture.ApTravelOrderId)
                .ToList();

            bool overlap = departures.Any(x =>
                (apTravelOrderDeparture.Date < x.ArrivalDate && apTravelOrderDeparture.ArrivalDate > x.Date) || // Original overlap check
                (apTravelOrderDeparture.Date >= x.Date && apTravelOrderDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            );

            if (overlap)
            {
                ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            }
            // var travelorder = db.ApTravelOrders.Find(apTravelOrderDeparture.ApTravelOrderId);
            // var prevtravelOrders = db.ApTravelOrders
            //     .Where(x => x.ApTravelOrderId != apTravelOrderDeparture.ApTravelOrderId)
            //     .Where(x => x.EmployeeId == travelorder.EmployeeId)
            //     .Where(x =>/* x.IsOnlineApproved == true &&*/ x.IsVoid == false)
            //     .Select(x => x.ApTravelOrderId)
            //     .ToList();

            // var departures = db.ApTravelOrderDepartures.Where(x => prevtravelOrders.Contains(x.ApTravelOrderId)).ToList();

            // bool overlap = departures.Any(x =>
            //         (apTravelOrderDeparture.Date < x.ArrivalDate && apTravelOrderDeparture.ArrivalDate > x.Date) || // Original overlap check
            //         (apTravelOrderDeparture.Date >= x.Date && apTravelOrderDeparture.ArrivalDate <= x.ArrivalDate) // Date range completely inside another
            //     );

            // if (overlap)
            // {
            //     ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            // }

            // var travelDetails = db.ApTravelOrderDepartures
            //.Where(t => prevtravelOrders.Contains(t.ApTravelOrderId))
            //.GroupBy(t => t.ApTravelOrderId)
            // .Select(g => new
            // {
            //     TravelOrderId = g.Key,
            //     DepartureDate = g.Where(x => x.Type == DepartureType.Departure).Select(x => x.Date).FirstOrDefault(),
            //     ReturnDate = g.Where(x => x.Type == DepartureType.Return).Select(x => x.ArrivalDate).FirstOrDefault()
            // })
            //.ToList();

            // bool overlapacross = travelDetails.Any(x =>
            //   (apTravelOrderDeparture.Date >= x.DepartureDate && apTravelOrderDeparture.ArrivalDate <= x.ReturnDate)
            // );

            // if (overlapacross)
            // {
            //     ModelState.AddModelError("overlap", "Departure date or arrival date overlapped previous travel order.");
            // }

            apTravelOrderDeparture.Duration = (decimal)(apTravelOrderDeparture.ArrivalDate - apTravelOrderDeparture.Date).TotalDays;

            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {

                    ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                    ApTravelOrderId = apTravelOrderDeparture.ApTravelOrderId,
                    From = apTravelOrderDeparture.From,
                    To = apTravelOrderDeparture.To,
                    Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    Type = apTravelOrderDeparture.Type,
                    TravelOrder = apTravelOrderDeparture.TravelOrder,
                    TransportMode = apTravelOrderDeparture.TransportMode,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                db.ApTravelOrderDepartures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UpdatePerDiemQty(apTravelOrderDeparture.ApTravelOrderId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order departure item", "Success", User.Identity.Name, $"updted departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderDepartures_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrderDeparture)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {

                    ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                    ApTravelOrderId = apTravelOrderDeparture.ApTravelOrderId,
                    From = apTravelOrderDeparture.From,
                    To = apTravelOrderDeparture.To,
                    Date = apTravelOrderDeparture.Date,
                    ArrivalDate = apTravelOrderDeparture.ArrivalDate,
                    Duration = apTravelOrderDeparture.Duration,
                    DestinationBranchId = apTravelOrderDeparture.DestinationBranchId,
                    Type = apTravelOrderDeparture.Type,
                    TravelOrder = apTravelOrderDeparture.TravelOrder,
                    TransportMode = apTravelOrderDeparture.TransportMode,
                    OrgBranchName = apTravelOrderDeparture.OrgBranchName,
                    DateCreated = apTravelOrderDeparture.DateCreated,
                    LastModified = apTravelOrderDeparture.LastModified,
                    CreatedBy = apTravelOrderDeparture.CreatedBy,
                    ModifiedBy = apTravelOrderDeparture.ModifiedBy
                };

                db.ApTravelOrderDepartures.Attach(entity);
                db.ApTravelOrderDepartures.Remove(entity);
                db.SaveChanges();
                UpdatePerDiemQty(apTravelOrderDeparture.ApTravelOrderId);
            }

            return Json(new[] { apTravelOrderDeparture }.ToDataSourceResult(request, ModelState));
        }
        
        private void UpdatePerDiemQty(int travelid)
        {

            #region check auto calculate
            if (FinanceConfigurationHelper.AutoCalculatetravelPerdiem())
            {
                var perdiem = db.ApTravelOrderAdvancePayments
                    .Where(x => x.Type == TravelPaymentType.PerDiem)
                    .Where(x => x.ApTravelOrderId == travelid)
                    .FirstOrDefault();

                if (perdiem != null)
                {
                    var departureAndReturn = db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == travelid).ToList();
                    var departureDate = departureAndReturn.Where(x => x.Type == DepartureType.Departure);
                    var returnDate = departureAndReturn.Where(x => x.Type == DepartureType.Return);

                    if (departureDate.Any() && returnDate.Any() && perdiem != null)
                    {
                        perdiem.Quantity = (int)(returnDate.FirstOrDefault().ArrivalDate - departureDate.FirstOrDefault().Date).Days + 1;
                        perdiem.Total = perdiem.UnitPrice * perdiem.Quantity;
                        db.ApTravelOrderAdvancePayments.Attach(perdiem);
                        db.Entry(perdiem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        perdiem.Quantity = 0;
                        perdiem.Total = perdiem.UnitPrice * perdiem.Quantity;
                        db.ApTravelOrderAdvancePayments.Attach(perdiem);
                        db.Entry(perdiem).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            #endregion
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}