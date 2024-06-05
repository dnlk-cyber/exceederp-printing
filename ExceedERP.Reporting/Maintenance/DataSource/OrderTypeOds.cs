using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Maintenance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{

    [DataObject]
    public class OrderTypeOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReportParameterViewModel> GetOrderType()
        {
            List<ReportParameterViewModel> list = new List<ReportParameterViewModel>()
            {
             new ReportParameterViewModel
            {
                Id = "JobOrder",
                Name = "JobOrder"

            },
              new ReportParameterViewModel
            {
                Id = "ServiceCenter",
                Name = "ServiceCenter"

            },
              new ReportParameterViewModel
            {
                Id = "WorkShop",
                Name = "WorkShop"

            }

        };
            
           
           

            return list;
        }
    }
}

