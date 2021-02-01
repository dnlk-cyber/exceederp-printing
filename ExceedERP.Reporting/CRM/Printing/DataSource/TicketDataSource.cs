using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.Printing.DataSource
{
    [DataObject]
    public class TicketDataSource : List<TicketViewModel>
    {

        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetOrganizationCustomers()
        {
            ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject OrgCustomer = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> Customer = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            var Customers = db.OrganizationCustomers.ToList();

            foreach (var c in Customers)
            {
                OrgCustomer = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
                {
                    Name = c.TradeName,
                    Id = c.OrganizationCustomerID
                };
                Customer.Add(OrgCustomer);
            }

            return Customer;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetStatus()
        {
            ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject Status = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject();
            List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> Statues = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();

            Status.Name = "UNFORWARDED";
            Status.Id = 1;
            Statues.Add(Status);


            Status.Name = "FORWARDED";
            Status.Id = 2;
            Statues.Add(Status);

            return Statues;
        }

        public string GetAge(DateTime AssignedDate, bool Status, bool IsActive, DateTime ClosedDate)
        {
            string Duration = string.Empty;
            try
            {
             
                if (Status && IsActive)
                {
                    TimeSpan Span = ClosedDate - AssignedDate;
                    Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
                }
                else
                {
                    if (IsActive)
                    {
                        TimeSpan Span = DateTime.Now - AssignedDate;
                        Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
                    }
                    else
                    {
                        TimeSpan Span = DateTime.Now - DateTime.Now;
                        Duration = Span.Days.ToString() + " Day " + Span.Hours.ToString() + " Hours";
                    }
                }
            }
            catch(Exception e)
            { }
            return Duration;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TicketViewModel> GetTicketList(string id)
        {
            List<TicketViewModel> TicketViewModels = new List<TicketViewModel>();
            var OrgCustomers = db.OrganizationCustomers.ToList();

            var ticket = db.CustomerTickets.Where(x => x.CustomerTicketID.ToString() == id).FirstOrDefault();

            if (ticket != null)
            {

                var asssinments = db.AssignTickets.Where(x => x.CustomerTicketId.ToString() == id).ToList();
                foreach (var assignment in asssinments)
                {
                  if(  assignment.DateCreated == null)
                    { assignment.DateCreated = DateTime.Now; }
                    assignment.Age = GetAge(DateTime.Parse(assignment.DateCreated.ToString()), assignment.IsClosed, assignment.IsActive, assignment.ClosedDate);


                    var TicketViewModel = new TicketViewModel
                    {
                        AssignedTo = assignment.AssinedTo,
                        Age = assignment.Age,
                        CustomerTicketID = ticket.CustomerTicketID,
                        CustomerID = ticket.CustomerID,
                        CustomerAddress = ticket.CustomerAddress,
                        IsSendForClosing = ticket.IsSendForClosing,
                        Description = ticket.Description,
                        Priority = ticket.Priority,
                        EscalationLevel = ticket.OrganizationCustomer.EsclationLevel,
                        TicketGroup = ticket.TicketGroup,
                        CreatedOn = ticket.CreatedOn,
                        DueDate = ticket.DueDate,
                        CreatedBy = ticket.CreatedBy,
                        DateCreated = ticket.DateCreated,
                        LastModified = ticket.LastModified,
                        StatusType = ticket.StatusType,
                         PriorityColor = ticket.Priority,
                        StatusColor = ticket.StatusType,
                        Remark = ticket.Remark

                    };
                    TicketViewModels.Add(TicketViewModel);
                }
            }
            
            return TicketViewModels;
        }


    }
}





