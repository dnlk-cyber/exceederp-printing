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
       public List<DriverTransferViewModel> GetDriverTransfer(string FromDriver, string ToDriver, DateTime startingDate, DateTime endingDate)
{
    // Start with IQueryable to avoid early materialization
    IQueryable<DriverVehicleTransfer> query = db.DriverVehicleTransfer;

    // Apply filters before materialization
    if (!string.IsNullOrEmpty(FromDriver))
    {
        query = query.Where(x => x.FromOperatorId == FromDriver);
    }

    if (!string.IsNullOrEmpty(ToDriver))
    {
        query = query.Where(x => x.ToOperatorId == ToDriver);
    }

    query = query.Where(x => x.DateCreated >= startingDate && x.DateCreated <= endingDate);

    // Project directly into the ViewModel
    var result = query.Select(DriverTransfer => new DriverTransferViewModel
    {
        Description = DriverTransfer.Description,
        FromOperator = DriverTransfer.FromOperatorId,
        ToOperator = DriverTransfer.ToOperatorId,
        StartingDate = DriverTransfer.StartingDate,
        ToDate = DriverTransfer.ToDate
    }).ToList();

    return result;
}
    }
}