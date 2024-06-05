using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class ExternalMaintenanceDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetJobStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Open });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = MaintenanceConst.Closed });

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetCustomer()
        {

            var result = db.OtherCompanyMaintenance.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.NameofCustomer,
                Value = e.NameofCustomer.ToString()


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetEquipment()
        {

            var result = db.OtherCompanyMaintenance.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.EquipmentName,
                Value = e.EquipmentName


            }
            ).ToList();

            return result;

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetOperator()
        {

            var result = db.OtherCompanyMaintenance.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.OperatorName,
                Value = e.OperatorName


            }
            ).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExternalMaintenanceViewModel> GetExternalMaintenance(string Customer, string Equipment, string Operator,
            DateTime startingDate, DateTime endingDate)
        {



            List<ExternalMaintenanceViewModel> MaintenanceList = new List<ExternalMaintenanceViewModel>();
            ExternalMaintenanceViewModel MaintenanceViewModel;
            List<OtherCompanyMaintenance> Maintenances = new List<OtherCompanyMaintenance>();
          
            Maintenances =
                db.OtherCompanyMaintenance.ToList();

            if (!string.IsNullOrEmpty(Equipment))
            {
                Maintenances = Maintenances.Where(x => x.EquipmentName == Equipment).ToList();
            }

            if (!string.IsNullOrEmpty(Customer))
            {
                Maintenances = Maintenances.Where(x => x.NameofCustomer == Customer).ToList();
            }

            if (!string.IsNullOrEmpty(Operator))
            {
                Maintenances = Maintenances.Where(x => x.OperatorName == Operator).ToList();
            }

            if (Maintenances.Any())
                {
                    foreach (var m in Maintenances)
                        {


                            MaintenanceViewModel = new ExternalMaintenanceViewModel();


                            MaintenanceViewModel.EquipmentName = m.EquipmentName;
                            MaintenanceViewModel.BankBranchName = m.BankBranchName;
                            MaintenanceViewModel.ApprovedBy = m.ApprovedBy;
                            MaintenanceViewModel.BankSlipNo = m.BankSlipNo;
                            MaintenanceViewModel.CustomerEmail = m.CustomerEmail;
                            MaintenanceViewModel.CustomerPhone = m.CustomerPhone;
                            MaintenanceViewModel.NameofCustomer = m.NameofCustomer;
                            MaintenanceViewModel.Description = m.Description;
                            MaintenanceViewModel.OperatorName = m.OperatorName;
                        


 
                            MaintenanceList.Add(MaintenanceViewModel);
                        }
                }
            return MaintenanceList;
        }

    }
}
