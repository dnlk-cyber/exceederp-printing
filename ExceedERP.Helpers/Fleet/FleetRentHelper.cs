using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Fleet
{
    public class FleetRentHelper
    {
        ExceedDbContext db = new ExceedDbContext();
        public RentalValidation Machinary(RentalPaymentRequest rentalPaymentRequest, RentalPaymentRequestLine detail)
        {
            var machinery = db.MachineryWorkingTimeSheets
                .Where(x => x.FleetsID == detail.FleetId)
                .ToList();

            machinery = machinery.Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date &&
                x.Date.Date <= rentalPaymentRequest.DurationTo.Date)
                .ToList();

            if (!machinery.Any())
            {
                return RentalValidation.NotFound;
            }
            return RentalValidation.valid;
            
        }
        public RentalValidation Dryload(RentalPaymentRequest rentalPaymentRequest, RentalPaymentRequestLine detail)
        {
            var dryLoading = db.DryLoadingTimeSheets
                .Where(x => x.FleetsID == detail.FleetId)
                .ToList();

            dryLoading = dryLoading.Where(x => x.TripStartDate.Date >= rentalPaymentRequest.DurationFrom.Date &&
                x.TripStartDate.Date <= rentalPaymentRequest.DurationTo.Date)
                .ToList();

            if (!dryLoading.Any())
            {
                return RentalValidation.NotFound;
            }
            return RentalValidation.valid;
        }

        public RentalValidation Dumptruck(RentalPaymentRequest rentalPaymentRequest, RentalPaymentRequestLine detail)
        {
            var dumpTruck = db.DumpTruckTimeSheets
                .Where(x => x.FleetsID == detail.FleetId).ToList();
            if (dumpTruck.Any())
                return RentalValidation.NotFound;
            dumpTruck = dumpTruck.Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date &&
                x.Date.Date <= rentalPaymentRequest.DurationTo.Date)
                .ToList();

            if (!dumpTruck.Any())
            {
                return RentalValidation.NotFound;
            }
            return RentalValidation.valid;
        }
        public RentalValidation Wagondrill(RentalPaymentRequest rentalPaymentRequest, Guid fleetId)
        {
            var wagonDrill = db.WagonDrillTimesheets
                .Where(x => x.FleetsID == fleetId)
                .ToList();
            if (wagonDrill.Any())
            {
                return RentalValidation.valid;
            }
            wagonDrill = wagonDrill.Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date &&
                x.Date.Date <= rentalPaymentRequest.DurationTo.Date)
                .ToList();
            if (!wagonDrill.Any())
            {
                return RentalValidation.NotFound;
            }
            return RentalValidation.valid;
        }

        public RentalValidation Lightvehicle(RentalPaymentRequest rentalPaymentRequest, RentalPaymentRequestLine detail)
        {
            var lightVehicle = db.LightVehicleTimeSheets
                .Where(x => x.FleetsID == detail.FleetId)
                .ToList();
            lightVehicle = lightVehicle.Where(x => x.Date.Date >= rentalPaymentRequest.DurationFrom.Date &&
                x.Date.Date <= rentalPaymentRequest.DurationTo.Date)
                .ToList();
            if (!lightVehicle.Any())
            {
                return RentalValidation.NotFound; 
            }
            return RentalValidation.valid;
        }
        public enum RentalValidation
        {
            valid,
            RentFromExternaNotFound,
            FleetNotFound,
            DateInvalid,
            NotFound
        }
    }
}
