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
    public class MaintenanceParametersOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ReportParameterViewModel> GetStatus()
        {
            List<ReportParameterViewModel> list = new List<ReportParameterViewModel>()
            {
                 new ReportParameterViewModel
            {
                Id = "Pending",
                Name = "Pending"

            },
             new ReportParameterViewModel
            {
                Id = "Open",
                Name = "Open"

            },
             
              new ReportParameterViewModel
            {
                Id = "Assigned",
                Name = "Assigned"

            },
              new ReportParameterViewModel
            {
                Id = "Sent For Closing",
                Name = "Sent For Closing"

            },
              new ReportParameterViewModel
            {
                Id = "Waiting Technical Approval",
                Name = "Waiting Technical Approval"

            },
              new ReportParameterViewModel
            {
                Id = "Waiting Administrative Approval",
                Name = "Waiting Administrative Approval"

            },
              new ReportParameterViewModel
            {
                Id = "Approved",
                Name = "Approved"

            }
              ,
              new ReportParameterViewModel
            {
                Id = "Closed",
                Name = "Closed"

            },
              new ReportParameterViewModel
            {
                Id = "Delivered",
                Name = "Delivered"

            }


        };
            
           
           

            return list;
        }
    }
}

