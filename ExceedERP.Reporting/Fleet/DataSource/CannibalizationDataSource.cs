using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class CannibalizationDataSource
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetManufacturer()
        {

            var result = db.Manufacturer.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.Name,
                Value = e.ManufacturerID.ToString()

            }
            ).ToList();

            return result;

        }



      

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CannibalizationViewModel> GetCannibalization(string Identity, string category, string ManufacturerYear, string Manufacturer,
           DateTime startingDate, DateTime endingDate)
        {
            List<CannibalizationViewModel> FleetList = new List<CannibalizationViewModel>();
              List<Fleets> Fleets;
          



 


            if (!string.IsNullOrEmpty(Identity))
            {
                if (!string.IsNullOrEmpty(category))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {
                    Fleets =
                        db.Fleets.Where(so => so.EquipmentIdentityID.ToString() == Identity)
                            .ToList();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
                }
                else
                {
                    Fleets = db.Fleets.ToList();
                }
            }
            if (Fleets.Any())
            {
                if (!string.IsNullOrEmpty(Manufacturer))
                {
                    Fleets = Fleets.Where(so => so.Manufacturer == Manufacturer).ToList();
                }
                if (!string.IsNullOrEmpty(ManufacturerYear))
                {
                    Fleets = Fleets.Where(so => so.ManufacturerYear == ManufacturerYear).ToList();
                }
                List<Fleets> List = Fleets;
                string TakeFleet = string.Empty;
                string NeededFleet = string.Empty;
                foreach (Fleets sov in List)
                {


                    List<Cannibalization> Cannibalizations = db.Cannibalization.Where(x => (x.PartNeededFleetsID == sov.FleetsID || x.PartTakenFleetsID == sov.FleetsID) && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                    foreach (var c in Cannibalizations)
                    {

                        var CannibalizationsViewModel = new CannibalizationViewModel();

                        var Takenfleet = db.Fleets.Where(x => x.FleetsID == c.PartTakenFleetsID).FirstOrDefault();

                        if (Takenfleet != null)
                        {
                            TakeFleet = Takenfleet.PlateNumber + " - " + Takenfleet.EquipmentName;
                        }
                        var NeedFleet = db.Fleets.Where(x => x.FleetsID == c.PartNeededFleetsID).FirstOrDefault();

                        if (NeedFleet != null)
                        {
                            NeededFleet = NeedFleet.PlateNumber + " - " + NeedFleet.EquipmentName;
                        }


                        CannibalizationsViewModel.Description = c.Description;
                         CannibalizationsViewModel.PartNeededFleets = NeededFleet;
                        CannibalizationsViewModel.PartTakenFleets = TakeFleet;
                         CannibalizationsViewModel.DateCreated = c.DateCreated;
                       
                        FleetList.Add(CannibalizationsViewModel);

                    }
                }
            }
            return FleetList;

        }
    }
}
