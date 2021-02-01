using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class AccidentDataSource
    {

        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetLevelOfDamage()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetRegistration.Accident.LevelOfDamageenum));

            return result;

        }
       
        
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccidentViewModel> GetFleetList(string Identity, string category, string Manufacturer, string LevelDamage,
            DateTime startingDate, DateTime endingDate)
        {
            List<AccidentViewModel> AccidentList = new List<AccidentViewModel>();
            AccidentViewModel FleetViewModel;
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
                        db.Fleets.Where(
                            so =>
                              
                                so.EquipmentIdentityID.ToString() == Identity)
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
                List<Fleets> List = Fleets;
                foreach (Fleets sov in List)
                {

                    var Accidents = db.Accident.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();
                    if (!string.IsNullOrEmpty(LevelDamage))
                    {
                        Accidents = Accidents.Where(so => so.LevelOfDamage == LevelDamage).ToList();
                    }
              

                    if (Accidents.Any())
                    {
                        foreach (var a in Accidents)
                        {
                            FleetViewModel = new AccidentViewModel();
                            FleetViewModel.Description = sov.PlateNumber + " - " + sov.EquipmentName;
                            FleetViewModel.CostcoveredbyCompany = decimal.Parse((a.CostcoveredbyCompany).ToString());
                            FleetViewModel.CostcoveredbyInsurance = decimal.Parse((a.CostcoveredbyInsurance).ToString());
                            FleetViewModel.CostcoveredbyOperator = decimal.Parse((a.CostcoveredbyOperator).ToString());
                            FleetViewModel.CostcoveredbyOtherBodies = decimal.Parse((a.CostcoveredbyOtherBodies).ToString());
                            FleetViewModel.AccidentLocation = a.AccidentLocation;
                            FleetViewModel.DateOfOccurrence = a.DateOfOccurrence;
                            FleetViewModel.LevelOfDamage = ((ExceedERP.Reporting.Fleet.ViewModel.AccidentViewModel.LevelOfDamageenum)int.Parse( a.LevelOfDamage)).ToString();

                      
                            AccidentList.Add(FleetViewModel);
                        }
                    }

                }
            }

            return AccidentList;
        }

    }
}
