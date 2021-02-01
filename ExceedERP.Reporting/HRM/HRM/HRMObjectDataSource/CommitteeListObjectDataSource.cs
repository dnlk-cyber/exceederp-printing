using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class CommitteeListObjectDataSource : List<CommitteeList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CommitteeList> GetCommitteeList(string vacancyCod)
        {
            List<CommitteeList> list = new List<CommitteeList> { };
            CommitteeList committe = new CommitteeList();
            var committee = db.Committees.FirstOrDefault(x=>x.CommitteeType==CommitteType.RECRUITMENT && x.IsActive && x.VaccancyCode== vacancyCod);
            if(committee!=null)
            {
                
                    var committes = db.CommitteeMembers.Where(x => x.CommitteeId == committee.CommitteeId && x.IsActive).ToList();

                    if (committes!=null)
                    {
                        foreach (var c in committes)
                        {
                            committe = new CommitteeList();
                            committe.CommitteName = c.Employee.FirstName + " " + c.Employee.MiddleName + " " + c.Employee.LastName;
                            list.Add(committe);
                        }
                        return list;
                    }
                
               
            }
          

            return list;
        }
    }
}