using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement
{
    public interface ITireIssueService
    {
        bool Insert(TireIssue entity);
        bool InsertBatch(List<TireIssue> entityList);
        bool Update(TireIssue entity);

        bool Delete(TireIssue entity);
        TireIssue GetById(Guid id);
        List<TireIssue> GetAll();
    }
}
