using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
using System.ComponentModel;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class MVPmExecutionReportHeaderODS
    {

        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
       
        public MVPmExecutionReportHeaderVM GetMVPmExecutionReportHeader(string MCId, DateTime From, DateTime To)
        {
            MVPmExecutionReportHeaderVM list = new MVPmExecutionReportHeaderVM();
            if(MCId!=null)
            {
                list.MaintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == MCId).Name;
                list.From = From.ToString("yyyy-MM-dd");
                list.To = To.ToString("yyyy-MM-dd");
            }
           
            return list;
        }
    }
    public class MVPmExecutionReportHeaderVM
    {
       
        public string From { get; set; }
        public string To { get; set; }
        public string MaintenanceCenter { get; set; }

    }
}
