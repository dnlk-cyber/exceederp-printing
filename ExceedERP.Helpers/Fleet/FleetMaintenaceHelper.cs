using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Fleet
{

    public   class FleetMaintenaceHelper
    {


        public void UpdateOdometerByPlateNo(string plateNo, decimal odometer)
        {
            IFleetService fleetService = new FleetService();
            if (!string.IsNullOrWhiteSpace(plateNo))
            {
                var fleet = fleetService.GetAll().Where(x => x.PlateNumber == plateNo).FirstOrDefault();
                if (fleet != null)
                {

                    fleet.CurentOdoMeter = odometer;
                    fleetService.Update(fleet);
                }
            }

        }
        public   decimal GetMechanicSingleDuration(ServiceCrewWorkLog log)
        {
            decimal TotalHour = 0;

            try
            {
                TotalHour = TotalHour + (log.WorkingHoursMorningto.Value.Hour - log.WorkingHoursMorningFrom.Value.Hour) + (log.WorkingHoursAfternoonTo.Value.Hour - log.WorkingHoursAfternoonFrom.Value.Hour);
            }
            catch (Exception e)
            {
                return TotalHour;
            }
            return TotalHour;
        }


        public   string GetFleetDistributionDuration(VehicleDistribution log)
        {
            string TotalHour = "00:00:00";

            try
            {
                if (log.IsOnlineApproved)
                {
                    if (log.IsOnlineTransferred)
                    {
                        TimeSpan Diffrence =  log.OnlineTransferredTime.Value - log.OnlineApprovedTime.Value;
                        TotalHour = Diffrence.Days + "-" + Diffrence.Hours.ToString() + ":" + Diffrence.Minutes.ToString();
                    }
                    else
                    {
                        TimeSpan Diffrence = DateTime.Now- log.OnlineApprovedTime.Value ;
                        TotalHour = Diffrence.Days + "-" + Diffrence.Hours.ToString() + ":" + Diffrence.Minutes.ToString();

                    }
                }
              
            }
            catch (Exception e)
            {
                return TotalHour;
            }
            return TotalHour;
        }
        public   decimal GetMechanicDuration(List<ServiceCrewWorkLog> logs)
        {
            decimal TotalHour = 0;

            foreach (var log in logs)
            {
                try
                {

                    TotalHour = TotalHour + (log.WorkingHoursMorningto.Value.Hour - log.WorkingHoursMorningFrom.Value.Hour) + (log.WorkingHoursAfternoonTo.Value.Hour - log.WorkingHoursAfternoonFrom.Value.Hour);
                }

                catch (Exception e)
                {

                }
            }
            return TotalHour;
        }
    }

}
