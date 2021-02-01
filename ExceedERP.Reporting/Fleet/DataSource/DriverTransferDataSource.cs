using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class DriverTransferDataSource
    {

        ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Helpers.Fleet.FleetBindingHelper.HelperObject> GetOperators()
        {

            var result = db.Operator.Select(s => new Helpers.Fleet.FleetBindingHelper.HelperObject
            {
                Name = s.FullName,
                Value = s.OperatorID.ToString()
            }).ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DriverTransferViewModel> GetDriverTransfer(string FromDriver, string ToDriver, DateTime startingDate,
            DateTime endingDate)
        {
            List<DriverTransferViewModel> DriverTransferList = new List<DriverTransferViewModel>();
            DriverTransferViewModel DriverTransferModel;
            List<DriverVehicleTransfer> DriverTransfers = db.DriverVehicleTransfer.ToList();


 
            if (!string.IsNullOrEmpty(FromDriver))
            {

                DriverTransfers = DriverTransfers.Where(x => x.FromOperatorId == FromDriver).ToList();
            }

            if (!string.IsNullOrEmpty(FromDriver) && !string.IsNullOrEmpty(ToDriver))
            {

                DriverTransfers = DriverTransfers.Where(x => x.FromOperatorId == FromDriver && x.ToOperatorId == ToDriver).ToList();
            }

            DriverTransfers = DriverTransfers.Where(x => x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();
           
            foreach (var DriverTransfer in DriverTransfers)
            {


                DriverTransferModel = new DriverTransferViewModel();



                DriverTransferModel.Description = DriverTransfer.Description;
                DriverTransferModel.FromOperator = DriverTransfer.FromOperatorId;
                DriverTransferModel.ToOperator = DriverTransfer.ToOperatorId;
                DriverTransferModel.StartingDate = DriverTransfer.StartingDate;
                DriverTransferModel.ToDate = DriverTransfer.ToDate;


 
                DriverTransferList.Add(DriverTransferModel);
            }

            return DriverTransferList;
        }
    }
}