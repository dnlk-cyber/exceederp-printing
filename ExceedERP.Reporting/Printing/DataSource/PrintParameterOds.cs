using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Printing.DataSource
{
    [DataObject]
    public class PrintParameterOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ParameterVM> GetCustomer()
        {
            return db.OrganizationCustomers.Select(s => new ParameterVM
            {
                Text = s.TradeName,
                Value = s.OrganizationCustomerID.ToString()
            }).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ParameterVM> GetJobType()
        {
            return Enum.GetValues(typeof(PrintJobType))
                       .Cast<PrintJobType>()
                       .Select(s => new ParameterVM
                       {
                           Text = s.ToString(),
                           Value = s.ToString()
                       })
                       .ToList();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ParameterVM> GetJobNo()
        {
            return db.PrintingProductionFollowUps.Select(s => new ParameterVM
            {
                Text = s.PrintingJobOrderItemId.ToString() + "-" + s.PrintJobType.ToString(),
                Value = s.PrintingJobOrderItemId.ToString()
            }).ToList();
        }
    }

}
