
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System.ComponentModel;

using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.DataSource
{
            public  class TicketObjectDataSource
    {

        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Reporting.CRM.ViewModel.Ticket> GetCustomerTicketList(string ticketNumber, string customer, string status,
            DateTime startingDate, DateTime endingDate)
        {
            List<ExceedERP.Reporting.CRM.ViewModel.Ticket> quoteList = new List<ExceedERP.Reporting.CRM.ViewModel.Ticket>();
            ExceedERP.Reporting.CRM.ViewModel.Ticket ticketViewModel;
            List<CustomerTicket> CustomerTickets;
            var product = db.Products.ToList();


            CustomerTickets = db.CustomerTickets.ToList();

            if (!string.IsNullOrEmpty(ticketNumber))
            {
                
                    CustomerTickets = CustomerTickets.Where(so => so.CustomerTicketID.ToString() == ticketNumber).ToList();
                }
            if (!string.IsNullOrEmpty(customer))
            {
                CustomerTickets =
                        CustomerTickets.Where(
                            so =>
                                so.CustomerID.ToString() == customer  )
                            .ToList();
                }
             
          
            
            List<CustomerTicket> CustomerTicket = CustomerTickets.Where(x => x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();
            foreach (CustomerTicket sov in CustomerTicket)
            {
                        String OrgName="";
                    var Org = db.OrganizationCustomers.Where(x=>x.OrganizationCustomerID == sov.CustomerID).FirstOrDefault();
                    if (Org != null)
                    {
                        OrgName = Org.TradeName;
                    }
               
                    ticketViewModel = new  ExceedERP.Reporting.CRM.ViewModel.Ticket();
                    ticketViewModel.Customer =OrgName;
                    ticketViewModel.TicketDescription = sov.Description;

                   ticketViewModel.Source = Enum.GetName(sov.Source.GetType(), sov.Source);

                   ticketViewModel.Priority =   sov.Priority ;
                    ticketViewModel.CreatedOn = sov.DateCreated;
                    ticketViewModel.TicketID = sov.CustomerTicketID ;
                    ticketViewModel.TicketGroup = sov.TicketGroup;
                          

                    quoteList.Add(ticketViewModel);

               
            }
            return quoteList;
        }


    }
}
