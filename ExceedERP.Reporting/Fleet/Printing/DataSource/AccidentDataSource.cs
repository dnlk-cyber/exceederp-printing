 
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using ExceedERP.Reporting.Fleet.Printing.ViewModel;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

namespace ExceedERP.Reporting.Accident.DataSource
{
    [DataObject]
    public class AccidentDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

      

        

        [DataObjectMethod(DataObjectMethodType.Select)]
        public  AccidentViewModel  GetAccidentList(string Id)
        {
            IAccidentService _AccidentService = new AccidentService();
            List<AccidentViewModel> AccidentList = new List<AccidentViewModel>();
            AccidentViewModel AccidentViewModel;
           var Accident = _AccidentService.GetById(Guid.Parse(Id));
            if(Accident != null)
            {
           
                    AccidentViewModel = new AccidentViewModel();
                    AccidentViewModel.AccidentLocation = Accident.AccidentLocation;
                    AccidentViewModel.Accountablebody = Accident.Accountablebody;
                    AccidentViewModel.CauseOfAccident = Accident.CauseOfAccident;
                    AccidentViewModel.DateOfOccurrence = Accident.DateOfOccurrence;
                    AccidentViewModel.Description =  Accident.Description ;
                    AccidentViewModel.LevelOfDamage = Accident.LevelOfDamage;
                    AccidentViewModel.FleetsID = Accident.Fleet.PlateNumber;
                    

                    return AccidentViewModel;
                 
              }
        
            return new AccidentViewModel();
        }

    }
}
