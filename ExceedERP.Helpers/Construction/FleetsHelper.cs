using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class FleetsHelper
    {
        public static decimal GetEquipmentRentalCost(Guid equipmentId) {
            var db = new ExceedDbContext();
            decimal rentalCost = new decimal();
            var currentEquipment = db.Fleets.Where(fl => fl.FleetsID == equipmentId).FirstOrDefault();
            if(currentEquipment != null)
            {
                rentalCost = currentEquipment.RentalRate;
            }
            return rentalCost;
        }
    }
}
