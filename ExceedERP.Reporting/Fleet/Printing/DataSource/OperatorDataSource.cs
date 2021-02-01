 
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

namespace ExceedERP.Reporting.Operator.DataSource
{
    [DataObject]
    public class OperatorDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

      

        

        [DataObjectMethod(DataObjectMethodType.Select)]
        public  OperatorViewModel  GetOperatorList(string Id)
        {
            IOperatorService _OperatorService = new OperatorService();
            List<OperatorViewModel> OperatorList = new List<OperatorViewModel>();
            OperatorViewModel OperatorViewModel;
           var Operator = _OperatorService.GetById(Guid.Parse(Id));
            if(Operator != null)
            {
           
                    OperatorViewModel = new OperatorViewModel();
                    OperatorViewModel.DateOfTermination = Operator.DateOfTermination;
                    OperatorViewModel.EducationLevel = Operator.EducationLevel;
                    OperatorViewModel.Email = Operator.Email;
                    OperatorViewModel.FullName = Operator.FullName;
                    OperatorViewModel.Gender =  Operator.Gender ;
                    OperatorViewModel.GuaranteeAddress = Operator.GuaranteeAddress;
                    OperatorViewModel.GuaranteeName = Operator.GuaranteeName;
                    OperatorViewModel.GuaranteePhoneNumber = Operator.GuaranteePhoneNumber;
                    OperatorViewModel.HireDate = Operator.HireDate;
                    OperatorViewModel.JobTitle = Operator.JobTitle;
                    OperatorViewModel.LicenseCity = Operator.LicenseCity;
                    OperatorViewModel.LicenseExpireDate = Operator.LicenseExpireDate;
                    OperatorViewModel.LicenseIssuedDate = Operator.LicenseIssuedDate;

                    OperatorViewModel.LicenseLevel = Operator.LicenseLevel;
                    OperatorViewModel.LicenseNo = Operator.LicenseNo;
                    OperatorViewModel.LicenseRenewalDate = Operator.LicenseRenewalDate;
                    OperatorViewModel.MobilePhone = Operator.MobilePhone;
                    OperatorViewModel.NameOfTraningCenter = Operator.NameOfTraningCenter;


                    return OperatorViewModel;
                 
              }
        
            return new OperatorViewModel();
        }

    }
}
