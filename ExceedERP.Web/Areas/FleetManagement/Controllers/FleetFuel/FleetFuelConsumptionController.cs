using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetFuel
{
    public class FleetFuelConsumptionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetFuelConsumption
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            decimal fuelSum = 0;
            var vehicleDistribution = db.VehicleDistribution.ToList();
            foreach (var item in vehicleDistribution)
            {
                var apTravelOrder = db.ApTravelOrders.Where(x => x.VehicleDistributionID == item.VehicleDistributionID).ToList();
                foreach (var travelOrder in apTravelOrder)
                {

                    var advancePayments = db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == travelOrder.ApTravelOrderId && x.Type == TravelPaymentType.Fuel).ToList();
                    foreach (var advancePayment in advancePayments)
                    {
                        fuelSum += advancePayment.Total;
                    }
                    var claims = db.ApTravelExpenseClaims.Where(x => x.ApTravelOrderId == travelOrder.ApTravelOrderId).ToList();
                    foreach (var claim in claims)
                    {
                        var claimExpenses =db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == claim.ApTravelExpenseClaimId && x.Type == TravelPaymentType.Fuel).ToList();
                        foreach (var claimExpese in claimExpenses)
                        {
                            fuelSum += claimExpese.Total;
                        }
                    }
                }

                var storeRequisitions = db.StoreRequisitions.Where(x => x.VehicleDistributionID == item.VehicleDistributionID).ToList();
                foreach (var storeRequisition in storeRequisitions)
                {
                    var storeRequisitionItems = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == storeRequisition.StoreRequisitionID).ToList();
                    foreach (var storeRequisitionItem in storeRequisitionItems)
                    {
                        fuelSum += storeRequisitionItem.ApprovedQuantity;
                    }
                }
                    
               
            }

            
            return View();
        }
        
    }
}

//StoreRequisitionItem