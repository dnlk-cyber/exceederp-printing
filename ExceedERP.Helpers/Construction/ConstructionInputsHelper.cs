using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class ConstructionInputsHelper
    {
        public static string GetJobTitleName(int titleId)
        {
            ExceedDbContext db = new ExceedDbContext();
            var name = "";
            var jobTitle = db.ProjectJobTitles.Where(l => l.ProjectJobTitleId == titleId).FirstOrDefault();
            if (jobTitle != null)
            {
                name = jobTitle.JobTitle;
            }
            return name;
        }
        public static decimal GetRentalRate(Guid id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal hourlyOutputRate = 0;
            var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
            if (fleet != null)
            {
                if (fleet.IsExternal)
                {
                    var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.ApprovedCost;
                    }
                }
                else
                {
                    var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.HourlyRate;
                    }
                }
            }

            return hourlyOutputRate;
        }
        public static string GetConstructionPeriod(int id)
        {
            var db = new ExceedDbContext();
            var name = "";
            var currentPeriod = db.ConstructionPeriod.Find(id);
            if (currentPeriod != null)
            {
                name = currentPeriod.Name;
            }
            return name;
        }
        public static decimal GetMateriaTrasportCost(string itemCode)
        {
            var db = new ExceedDbContext();
            var cost = new decimal();
            var itemInDb = db.MaterialTransportationCost.FirstOrDefault(m => m.ItemCode == itemCode);
            if(itemInDb != null)
            {
                cost = itemInDb.TotalCostPerUnit;
            }
            return cost;
        }
        public static decimal RecalculateMaterialTransPortCost(string itemCode, decimal distanceFromSiteToSource) {
            var db = new ExceedDbContext();
            var totalCostPerUnit = new decimal();
            var record = db.MaterialTransportationCost.FirstOrDefault(m => m.ItemCode == itemCode);
            if(record != null && distanceFromSiteToSource > 0)
            {
                var requiredTime = record.AverageSpeedOfTrack > 0 ? distanceFromSiteToSource / record.AverageSpeedOfTrack : 0;
                var totalRentalCost = requiredTime * record.RentalCostWithFuel * 2;
                var totalPerdiemCost = (record.DriverPerdiem * requiredTime * 2) / 8 + (record.AssistantDriverPerdiem * requiredTime * 2) / 8;
                var totalCost = totalRentalCost + totalPerdiemCost;
                var  costPerUnit = totalCost / record.TrackCapacityPerTrip;
                totalCostPerUnit = costPerUnit + record.LoadingUnloadingCost;


            }
            return totalCostPerUnit;
        }
    }
}
