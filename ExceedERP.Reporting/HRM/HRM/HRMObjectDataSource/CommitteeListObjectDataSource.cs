using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class CommitteeListObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CommitteeList> GetCommitteeList(string vacancyId)
        {
            List<CommitteeList> list = new List<CommitteeList> { };
            CommitteeList committe = new CommitteeList();
            if (vacancyId != null)
            {
                int.TryParse(vacancyId, out int vacId);
                var recruitmentVacancy = db.RecruitmentVacancy.FirstOrDefault(X => X.VacancyId == vacId);
                if (recruitmentVacancy != null)
                {
                    var lookupId = db.Lookups.Where(x => x.LookupType==LookUpTypes.COMMITTEE_TYPE).Select(x => x.LookupId);
                    var committee = db.Committees.FirstOrDefault(x => lookupId.Contains(x.LookupId) && x.IsActive && x.VaccancyCode == recruitmentVacancy.MediaVacancyCode);
                    if (committee != null)
                    {
                        var committes = db.CommitteeMembers.Where(x => x.CommitteeId == committee.CommitteeId && x.IsActive).ToList();

                        if (committes != null)
                        {
                            foreach (var c in committes)
                            {
                                committe = new CommitteeList();
                                committe.CommitteName = c.Employee.FirstNameEnglish + " " + c.Employee.MiddleNameEnglish + " " + c.Employee.MiddleNameEnglish;
                                list.Add(committe);
                            }
                            return list;
                        }


                    }

                }
            }

            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}