using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
    public class TicketODS
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TicketVM GetTicket(string id)
        {
            var ticketVM = new TicketVM();

            if (!int.TryParse(id, out int ticketId)) return ticketVM;

            var customerTicket = db.CustomerTicket_HelpDesks.Find(ticketId);
            if (customerTicket == null) return ticketVM;
            ticketVM.RequestDate = customerTicket.DateCreated?.ToString("dd.MM.yyyy");
            ticketVM.RequestNumber = customerTicket.CustomerTicket_HelpDeskId;
            var placement = db.Placements
                .Where(x => x.EmployeeId == customerTicket.EmployeeId)
                .OrderByDescending(x => x.PlacementId)
                .FirstOrDefault();

            if (placement != null)
            {
                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
                if (emp != null)
                    ticketVM.Requester = emp.FullNameaEnglish;

                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (mainCompany != null)
                    ticketVM.RequesterDepartment = mainCompany.Name;

                var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                if (branch != null)
                    ticketVM.Branch = branch.Name;

                // Only assign BusinessUnit if Branch was not already assigned
                if (string.IsNullOrEmpty(ticketVM.Branch))
                {
                    var businessUnit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
                    if (businessUnit != null)
                        ticketVM.BusinessUnit = businessUnit.Name;
                }
            }

            // Request Details
            var requestDetail = new RequestDetailVM
            {
                Date = customerTicket.DateCreated?.ToString("dd.MM.yyyy"),
                Priority = customerTicket.TicketPriority.ToString(),
                Description = customerTicket.Description
            };

            var ticketType = db.TicketType_HelpDesks.FirstOrDefault(x => x.TicketType_HelpDeskId == customerTicket.TicketType_HelpDeskId);
            if (ticketType != null)
                requestDetail.TicketType = ticketType.Type;

            var group = db.SupportGroup_HelpDesks.FirstOrDefault(x => x.SupportGroup_HelpDeskId == customerTicket.SupportGroup_HelpDeskId);
            if (group != null)
                requestDetail.Group = group.Name;

            ticketVM.RequestDetails = new List<RequestDetailVM> { requestDetail };
            ticketVM.ResponseDetails = new List<ResponseDetailVM>();

            // Response Details
            var assigns = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == customerTicket.CustomerTicket_HelpDeskId);

            foreach (var assign in assigns)
            {
                var responseDetail = new ResponseDetailVM
                {
                    Problem = assign.RootCause,
                    SolutionGiven = assign.Solution,
                    Remark = $"{assign.Remark} {assign.Remarked}",
                    IsExernal = assign.Handler.ToString()
                };
                if(assign.Handler != TicketHandledBy.N_A)
                {
                    responseDetail.IsExernal = assign.Handler.ToString();
                }
                var assignedTo = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == assign.AssinedTo);
                if (assignedTo != null)
                {
                    responseDetail.AssignedUser = assignedTo.FullNameaEnglish;
                    ticketVM.HandledBy = assignedTo.FullNameaEnglish;

                }

                // You might want to include materials if needed here:
                // var materials = db.MaterialsUseds.Where(x => x.AssignedTicketId == assign.AssignTicket_HelpDeskId).ToList();

                ticketVM.ResponseDetails.Add(responseDetail);
            }

            return ticketVM;
        }

        //public TicketVM GetTicket(string id)
        //{

        //   var Ticket = new TicketVM();
        //    int.TryParse(id, out int ticketId);
        //    var customerTicket = db.CustomerTicket_HelpDesks.Find(ticketId);
        //    if (customerTicket !=null)
        //    {
        //        var placement = db.Placements.Where(x => x.EmployeeId == customerTicket.EmployeeId).OrderByDescending(x => x.PlacementId).FirstOrDefault();
        //        if(placement != null)
        //        {
        //            var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId);
        //            if (emp != null)
        //            {
        //                Ticket.Requester = emp.FullNameaEnglish;
        //            }

        //            var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
        //            if (mainCompany != null)
        //            {
        //                Ticket.RequesterDepartment = mainCompany.Name;
        //            }
        //            var branch = db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
        //            if (branch != null)
        //            {
        //                Ticket.Branch = branch.Name;
        //            }
        //            var businessUnit = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
        //            if (businessUnit != null)
        //            {
        //                Ticket.Branch = branch.Name;
        //            }

        //        }
        //        Ticket.RequestDetails = new List<RequestDetailVM>();
        //        var RequestDetail = new RequestDetailVM();
        //        RequestDetail.Date = customerTicket.DateCreated.Value.ToString("dd.MM.yyyy");
        //        RequestDetail.Priority = customerTicket.TicketPriority.ToString();
        //        RequestDetail.Description = customerTicket.Description;
        //        var ticketType = db.TicketType_HelpDesks
        //            .FirstOrDefault(x => x.TicketType_HelpDeskId == customerTicket.TicketType_HelpDeskId);
        //        if (ticketType != null)
        //        {
        //            RequestDetail.TicketType = ticketType.Type;
        //        }
        //        var group = db.SupportGroup_HelpDesks
        //            .FirstOrDefault(x => x.SupportGroup_HelpDeskId == customerTicket.SupportGroup_HelpDeskId);
        //        if (group != null)
        //        {
        //            RequestDetail.Group = group.Name;
        //        }
        //        Ticket.RequestDetails.Add(RequestDetail);
        //        Ticket.ResponseDetails = new List<ResponseDetailVM>();

        //        var assigns = db.AssignTicket_HelpDesks.Where(x => x.CustomerTicketId == customerTicket.CustomerTicket_HelpDeskId).ToList();

        //        foreach (var assign in assigns)
        //        {
        //            var ResponseDetail = new ResponseDetailVM();
        //            ResponseDetail.Problem =  assign.RootCause ;
        //            ResponseDetail.SolutionGiven =  assign.Solution ;
        //            ResponseDetail.Remark =  assign.Remark+" "+assign.Remarked ;
        //            ResponseDetail.IsExernal =  assign.IsClosed?"External":" Internal" ;

        //            var materials = db.MaterialsUseds.Where(x => x.AssignedTicketId == assign.AssignTicket_HelpDeskId).ToList();

        //            var assignedTo = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == assign.AssinedTo);

        //            if (assignedTo != null)
        //            {
        //                ResponseDetail.AssignedUser = assignedTo.FullNameaEnglish;
        //            }
        //            Ticket.ResponseDetails.Add(ResponseDetail);
        //        }


        //    }




        //    return Ticket;
        //}
    }
}
