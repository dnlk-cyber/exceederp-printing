using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.CRM.Tender.DataSource
{
    [DataObject]
    public class ParticipantsListODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ParticipantsListVM> GetParticipantsList(string id)
        {
            List<ParticipantsListVM> list = new List<ParticipantsListVM>();
            var SelectWinners = db.Winners.Where(x=>x.SalesLotId.ToString()==id).Select(x=>new {tin=x.TinNumber}).Distinct().ToList();
                foreach(var g in SelectWinners)
                {

                    var participant = new ParticipantsListVM();
                    var FetchParticipant = db.SalesLotParticipants.FirstOrDefault(x => x.TinNumber == g.tin);
                    if(FetchParticipant != null)
                    {

                           participant.CustormerName = FetchParticipant.CustomerName;
                           participant.TinNumber = g.tin;
                           list.Add(participant);
                    }
                   
                }
            return list;
        }
    }

    public class ParticipantsListVM
    {
        public string CustormerName { get; set; }
        public string TinNumber { get; set; }
    }
    
}
