using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class ApTravelOrderDepartureRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Finance/ApTravelOrderDeparture
        public ActionResult Index()
        {
           
            return View();
        }
       
        public ActionResult ApTravelOrderDepartures_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderDeparture> aptravelorderdepartures = db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == id);
            DataSourceResult result = aptravelorderdepartures.ToDataSourceResult(request, apTravelOrderDeparture => new {
                ApTravelOrderDepartureId = apTravelOrderDeparture.ApTravelOrderDepartureId,
                ApTravelOrderId = apTravelOrderDeparture.ApTravelOrderId,
                Type = apTravelOrderDeparture.Type,
                From = apTravelOrderDeparture.From,
                To = apTravelOrderDeparture.To,
                Date = apTravelOrderDeparture.Date,
                Time = apTravelOrderDeparture.Time,
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
        public ActionResult ApTravelOrderDepartures_Create([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrder,int id)
        {


            if (ModelState.IsValid)
            {           

                                
                var entity = new ApTravelOrderDeparture
                {                 
                   
                   ApTravelOrderId = id,
                   From = apTravelOrder.From,
                   To = apTravelOrder.To,
                   Date = apTravelOrder.Date,
                   Time = apTravelOrder.Time,
                   Type = apTravelOrder.Type,
                   TravelOrder= apTravelOrder.TravelOrder,
                   TransportMode = apTravelOrder.TransportMode,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                db.ApTravelOrderDepartures.Add(entity);
                db.SaveChanges();
                apTravelOrder.ApTravelOrderDepartureId = entity.ApTravelOrderDepartureId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order departure item", "Success", User.Identity.Name, $"created departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartures_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {

                    ApTravelOrderDepartureId = apTravelOrder.ApTravelOrderDepartureId,
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    From = apTravelOrder.From,
                    To = apTravelOrder.To,
                    Date = apTravelOrder.Date,
                    Time = apTravelOrder.Time,
                    Type = apTravelOrder.Type,
                    TravelOrder = apTravelOrder.TravelOrder,
                    TransportMode = apTravelOrder.TransportMode,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                db.ApTravelOrderDepartures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel order departure item", "Success", User.Identity.Name, $"updted departure item {entity.Type} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderDepartures_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderDeparture apTravelOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderDeparture
                {

                    ApTravelOrderDepartureId = apTravelOrder.ApTravelOrderDepartureId,
                    ApTravelOrderId = apTravelOrder.ApTravelOrderId,
                    From = apTravelOrder.From,
                    To = apTravelOrder.To,
                    Date = apTravelOrder.Date,
                    Time = apTravelOrder.Time,
                    Type = apTravelOrder.Type,
                    TravelOrder = apTravelOrder.TravelOrder,
                    TransportMode = apTravelOrder.TransportMode,
                    OrgBranchName = apTravelOrder.OrgBranchName,
                    DateCreated = apTravelOrder.DateCreated,
                    LastModified = apTravelOrder.LastModified,
                    CreatedBy = apTravelOrder.CreatedBy,
                    ModifiedBy = apTravelOrder.ModifiedBy
                };

                db.ApTravelOrderDepartures.Attach(entity);
                db.ApTravelOrderDepartures.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { apTravelOrder }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}