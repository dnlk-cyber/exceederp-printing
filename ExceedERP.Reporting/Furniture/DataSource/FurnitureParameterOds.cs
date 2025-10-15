using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Furniture.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.Furniture.DataSource
{
    [DataObject]
    public class FurnitureParameterOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FParameterVM> GetCustomer()
        {
            return db.OrganizationCustomers.Select(s => new FParameterVM
            {
                Text = s.TradeName,
                Value = s.OrganizationCustomerID.ToString()
            }).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FParameterVM> GetJobType()
        {
            return Enum.GetValues(typeof(PrintJobType))
                       .Cast<PrintJobType>()
                       .Select(s => new FParameterVM
                       {
                           Text = s.ToString(),
                           Value = s.ToString()
                       })
                       .ToList();
        }

    }

}
