using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Archive.ViewModel;

namespace ExceedERP.Reporting.Archive.DataSource
{
    [DataObject]
    public class OutgoingngLetterObjectDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OutgoingngLetterVM> GetOutgoingLetter(DateTime? fromDate,DateTime? toDate)
        {
            List<OutgoingngLetterVM> list = new List<OutgoingngLetterVM>();
            
            var outgoingLetters = db.InstitutionOutgoingLetters.AsNoTracking().ToList();
            if(fromDate.HasValue)
            {
                outgoingLetters = outgoingLetters.Where(x => x.OutgoingLetterDate.Date >= fromDate.Value.Date).ToList();
            }
            if (toDate.HasValue)
            {
                outgoingLetters = outgoingLetters.Where(x => x.OutgoingLetterDate.Date <= toDate.Value.Date).ToList();
            }
            foreach(var outgoingLetter in outgoingLetters)
            {
                var LetterList  = new OutgoingngLetterVM();
                string sendto = "";
                LetterList.DocumentNumber = outgoingLetter.FileNo;
                LetterList.OutgoingDate = outgoingLetter.OutgoingLetterDateAm;
                LetterList.TypeOfLetter = outgoingLetter.Subject;
                LetterList.OutgoingLetterTo = outgoingLetter.OwnerName;
                LetterList.DeliveredDate = outgoingLetter.OutgoingLetterDateAm;
                var letterSender = db.LetterReceivers.Where(x => x.LetterForMultipleStaffId == outgoingLetter.InstitutionOutgoingLetterId).DistinctByField(x=>x.EmployeeId).ToList();
                int count = 0;
                foreach (var reciever in letterSender)
                {
                    count++;
                    var depart = db.MainCompanyStructures.Find(reciever.LetterReceiverId);
                    if(depart != null&&(count !=1||count != letterSender.Count()))
                    {
                        sendto += depart.Name + ", ";
                    }
                    else if(depart != null && !(count != 1 || count != letterSender.Count()))
                    {
                        sendto += depart.Name;
                    }

                }
                LetterList.OutgoingLetterTo = sendto;

                list.Add(LetterList);

            }


            return list;
        }

       
    }
}
