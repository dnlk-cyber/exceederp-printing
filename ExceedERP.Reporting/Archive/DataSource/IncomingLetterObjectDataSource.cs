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
    class IncomingLetterObjectDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<IncomingLetterVM> GetIncomingLetter(DateTime? fromDate,DateTime? toDate)
        {
            List<IncomingLetterVM> list = new List<IncomingLetterVM>();
            
            var incommingletters = db.InstitutionIncomingLetters.AsNoTracking().ToList();
            if(fromDate.HasValue)
            {
                incommingletters = incommingletters.Where(x => x.RecivedDate.Date >= fromDate.Value.Date).ToList();
            }
            if (toDate.HasValue)
            {
                incommingletters = incommingletters.Where(x => x.RecivedDate.Date <= toDate.Value.Date).ToList();
            }
            foreach(var incommingletter in incommingletters)
            {
                var LetterList  = new IncomingLetterVM();
                string sendto = "";
                LetterList.DocumentNumber = incommingletter.FileNo;
                LetterList.IncomingDate = incommingletter.RecivedDateAmh;
                LetterList.TypeOfLetter = incommingletter.Subject;
                LetterList.ReceivedFrom = incommingletter.SenderName;
                var letterSender = db.LetterSender.Where(x => x.InstitutionIncomingLetterId == incommingletter.InstitutionIncomingLetterId).DistinctByField(x=>x.SenderDepartment).ToList();
                int count = 0;
                foreach (var reciever in letterSender)
                {
                    count++;
                    var depart = db.MainCompanyStructures.Find(reciever.SenderDepartment);
                    if(depart != null&&(count !=1||count != letterSender.Count()))
                    {
                        sendto += depart.Name + ", ";
                    }
                    else if(depart != null && !(count != 1 || count != letterSender.Count()))
                    {
                        sendto += depart.Name;
                    }
                }
                LetterList.ForwardTo = sendto;

                list.Add(LetterList);

            }


            return list;
        }

       
    }
}
