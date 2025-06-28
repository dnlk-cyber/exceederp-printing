using ExceedERP.DataAccess.Context;
//using ExceedERP.Reporting.CRM.Tender.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Reporting.CRM.Tender.DataSource
{
    [DataObject]
   public class InvoiceReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]

        public InvoicReportVM GetInvoiceReport()
        {

            LineItem invoice = new LineItem();
         
           // invoice.
            return null;
        }

    }



    public class InvoicReportVM
    {
        
       

    }
}
