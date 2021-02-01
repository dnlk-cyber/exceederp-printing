using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    

    [DataObject]
    public class LetThemKowWorkUnitObjectDataSource : List<LetThemKnow>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LetThemKnow> GetLetThemKnows(object[] letthem)
        {

            LetThemKnow letThemKnow = new LetThemKnow();
            List<LetThemKnow> letThemKnowList = new List<LetThemKnow>();

            if (letthem != null)
            {
                if (letthem.Any())
                {
                    foreach (var let in letthem)
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
    }
}