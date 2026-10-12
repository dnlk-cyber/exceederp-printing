using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
   
    [DataObject]
    public class TranscriptObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LetThemKnow> GetTranscript(object[] transcript)
        {

            LetThemKnow letThemKnow = new LetThemKnow();
            List<LetThemKnow> letThemKnowList = new List<LetThemKnow>();
            if (transcript != null)
            {
                if (transcript.Any())
                {
                    foreach (var let in transcript)
                    {
                        var mainCompany = db.MainCompanyStructures
                            .FirstOrDefault(x => x.OrgStructureId.ToString() == let.ToString());
                        letThemKnow = new LetThemKnow();
                        letThemKnow.OrgStructureId = mainCompany.OrgStructureId;
                        letThemKnow.Name = mainCompany.Name;
                        letThemKnowList.Add(letThemKnow);
                    }


                }
            }

            return letThemKnowList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
