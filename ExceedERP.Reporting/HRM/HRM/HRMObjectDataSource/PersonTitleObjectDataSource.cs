using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    
    [DataObject]
    public class PersonTitleObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PersonTitleViewModel> GetLongTermEducation()
        {
            List<PersonTitleViewModel> personTitleList = new List<PersonTitleViewModel>();
            PersonTitleViewModel personTitle = new PersonTitleViewModel();
            var title = db.Lookups.Where(x => x.LookupType == LookUpTypes.PERSON_TITLE).ToList();
            foreach(var t in title)
            {
                personTitle = new PersonTitleViewModel();
                personTitle.PersonTitleId = t.LookupId;
                personTitle.Title = t.Description;

                personTitleList.Add(personTitle);
            }
            
            return personTitleList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
