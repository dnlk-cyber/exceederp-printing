 
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

namespace ExceedERP.Reporting.Insurance.DataSource
{
    [DataObject]
    public class InsuranceDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

      

        

        [DataObjectMethod(DataObjectMethodType.Select)]
        public  InsuranceViewModel  GetInsuranceList(string Id)
        {
            IInsuranceService _InsuranceService = new InsuranceService();
            List<InsuranceViewModel> InsuranceList = new List<InsuranceViewModel>();
            InsuranceViewModel InsuranceViewModel;
           var Insurance = _InsuranceService.GetById(Guid.Parse(Id));
            if(Insurance != null)
            {
           
                    InsuranceViewModel = new InsuranceViewModel();
                 //   InsuranceViewModel.E = Insurance.ExpireDate;
                    //InsuranceViewModel.Accountablebody = Insurance.Accountablebody;
                    //InsuranceViewModel.CauseOfInsurance = Insurance.CauseOfInsurance;
                    //InsuranceViewModel.DateOfOccurrence = Insurance.DateOfOccurrence;
                    //InsuranceViewModel.Description =  Insurance.Description ;
                    //InsuranceViewModel.LevelOfDamage = Insurance.LevelOfDamage;
                    //InsuranceViewModel.FleetsID = Insurance.Fleet.PlateNumber;
                    

                    return InsuranceViewModel;
                 
              }
        
            return new InsuranceViewModel();
        }

    }
}
