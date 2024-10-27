using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    [Authorize]
    public class TravelFuelExpenseClaimController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Finance/ApTravelFuelExpenseClaim
        public ActionResult Index()
        {
           
            return View();
        }
       
        public ActionResult TravelClaimFuelExpenses_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelFuelExpenseClaim> ApTravelFuelExpenseClaims = db.ApTravelFuelExpenseClaims.Where(x => x.ApTravelExpenseClaimId == id);
            DataSourceResult result = ApTravelFuelExpenseClaims.ToDataSourceResult(request, fuel => new {
                ApTravelFuelExpenseClaimId = fuel.ApTravelFuelExpenseClaimId,
                ApTravelExpenseClaimId = fuel.ApTravelExpenseClaimId,
                PlateNo = fuel.PlateNo,
                InvoiceNo = fuel.InvoiceNo,
                FuelStation = fuel.FuelStation,
                DepartureKM = fuel.DepartureKM,
                ReturnKM = fuel.ReturnKM,
                DifferenceKM = fuel.DifferenceKM,
                FuelType = fuel.FuelType,
                FuelQTY = fuel.FuelQTY,
                FuelTotal = fuel.FuelTotal,
                OilQTY = fuel.OilQTY,
                OilTotal = fuel.OilTotal,
                Minormaintenance = fuel.Minormaintenance,
                Total = fuel.Total
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimFuelExpenses_Create([DataSourceRequest]DataSourceRequest request, ApTravelFuelExpenseClaim fuel, int id)
        {

            fuel.DifferenceKM = fuel.ReturnKM - fuel.DepartureKM;
            fuel.Total = fuel.FuelTotal+fuel.OilTotal+fuel.Minormaintenance;

            if (fuel.DifferenceKM <= 0)
                ModelState.AddModelError("zero", "Return KM should be higher than Departure KM.");

            if (fuel.FuelQTY > 0 && fuel.FuelTotal<=0)
                ModelState.AddModelError("zerofuel", "Please fill fuel total because you filled a Qty.");

            if (fuel.OilQTY > 0 && fuel.OilTotal <= 0)
                ModelState.AddModelError("zerofuel", "Please fill oil total because you filled a Qty.");

            if (ModelState.IsValid)
            {        

                                
                var entity = new ApTravelFuelExpenseClaim
                {                 
                   
                   ApTravelExpenseClaimId = id,
                   InvoiceNo = fuel.InvoiceNo,
                   FuelStation = fuel.FuelStation,
                    DepartureKM = fuel.DepartureKM,
                    ReturnKM = fuel.ReturnKM,
                    DifferenceKM = fuel.DifferenceKM,
                    FuelType = fuel.FuelType,
                    PlateNo = fuel.PlateNo,
                    FuelQTY = fuel.FuelQTY,
                    FuelTotal = fuel.FuelTotal,
                    OilQTY = fuel.OilQTY,
                    OilTotal = fuel.OilTotal,
                    Minormaintenance = fuel.Minormaintenance,
                    Total = fuel.Total,
                };

                db.ApTravelFuelExpenseClaims.Add(entity);
                db.SaveChanges();
                fuel.ApTravelFuelExpenseClaimId = entity.ApTravelFuelExpenseClaimId;
                UpdatePerDiemQty(fuel.ApTravelExpenseClaimId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel claim fuel expense item", "Success", User.Identity.Name, $"created departure item {entity.PlateNo} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { fuel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimFuelExpenses_Update([DataSourceRequest]DataSourceRequest request, ApTravelFuelExpenseClaim fuel)
        {
            fuel.DifferenceKM = fuel.ReturnKM - fuel.DepartureKM;
            fuel.Total = fuel.FuelTotal + fuel.OilTotal + fuel.Minormaintenance;

            if (fuel.DifferenceKM <= 0)
                ModelState.AddModelError("zero", "Return KM should be higher than Departure KM.");

            if (fuel.FuelQTY > 0 && fuel.FuelTotal <= 0)
                ModelState.AddModelError("zerofuel", "Please fill fuel total because you filled a Qty.");

            if (fuel.OilQTY > 0 && fuel.OilTotal <= 0)
                ModelState.AddModelError("zerofuel", "Please fill oil total because you filled a Qty.");

            if (ModelState.IsValid)
            {
                var entity = new ApTravelFuelExpenseClaim
                {

                    ApTravelFuelExpenseClaimId = fuel.ApTravelFuelExpenseClaimId,
                    ApTravelExpenseClaimId = fuel.ApTravelExpenseClaimId,
                    InvoiceNo = fuel.InvoiceNo,
                    FuelStation = fuel.FuelStation,
                    DepartureKM = fuel.DepartureKM,
                    ReturnKM = fuel.ReturnKM,
                    DifferenceKM = fuel.DifferenceKM,
                    FuelType = fuel.FuelType,
                    PlateNo = fuel.PlateNo,
                    FuelQTY = fuel.FuelQTY,
                    FuelTotal = fuel.FuelTotal,
                    OilQTY = fuel.OilQTY,
                    OilTotal = fuel.OilTotal,
                    Minormaintenance = fuel.Minormaintenance,
                    Total = fuel.Total
                };

                db.ApTravelFuelExpenseClaims.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                UpdatePerDiemQty(fuel.ApTravelExpenseClaimId);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care travel claim fuel expense item", "Success", User.Identity.Name, $"updted departure item {entity.PlateNo} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { fuel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelClaimFuelExpenses_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelFuelExpenseClaim fuel)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ApTravelFuelExpenseClaim
                {

                    ApTravelFuelExpenseClaimId = fuel.ApTravelFuelExpenseClaimId,
                    ApTravelExpenseClaimId = fuel.ApTravelExpenseClaimId,
                    InvoiceNo = fuel.InvoiceNo,
                    FuelStation = fuel.FuelStation,
                    DepartureKM = fuel.DepartureKM,
                    ReturnKM = fuel.ReturnKM,
                    DifferenceKM = fuel.DifferenceKM,
                    FuelType = fuel.FuelType,
                    PlateNo = fuel.PlateNo,
                    FuelQTY = fuel.FuelQTY,
                    OilQTY = fuel.OilQTY,
                    FuelTotal = fuel.FuelTotal,
                    OilTotal = fuel.OilTotal,
                    Minormaintenance = fuel.Minormaintenance,
                    Total = fuel.Total,
                };

                db.ApTravelFuelExpenseClaims.Attach(entity);
                db.ApTravelFuelExpenseClaims.Remove(entity);
                db.SaveChanges();
                UpdatePerDiemQty(fuel.ApTravelExpenseClaimId);
            }

            return Json(new[] { fuel }.ToDataSourceResult(request, ModelState));
        }
        
        private void UpdatePerDiemQty(int travelid)
        {

            #region check auFuelStation calculate
           
            #endregion
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}