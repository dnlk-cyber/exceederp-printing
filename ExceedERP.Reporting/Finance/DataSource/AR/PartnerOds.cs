using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
//using ExceedERP.Core.Domain.Finance.Report;

namespace ExceedERP.Reporting.Finance.DataSource.AR
{
    [DataObject]
    public class PartnerOds : List<PartnerListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PartnerListViewModel> GetPartnerList()
        {
            List<PartnerListViewModel> list = new List<PartnerListViewModel> { };

            var partners = _db.Partners.Select(x=>new {x.PartnerCode,x.Name,x.PartnerId,x.Country,x.IsActive}).ToList();
           
            foreach (var partner in partners)
            {
                var record = new PartnerListViewModel
                {
                    Code = partner.PartnerCode,
                    Name = partner.Name,
                    PartnerId = partner.PartnerId,
                    Country = partner.Country,
                    IsActive = partner.IsActive ? "Yes" : "No"
                };
                list.Add(record);
            }
            return list;
        }

       
    }
}
