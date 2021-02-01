using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Reporting.Construction.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.Construction.DataSource
{
    //class EarnedValuePerProjectOds
    //{
    //}
    [DataObject]
    public class EarnedValuePerProjectOds : List<EarnedValuePerProjectViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        public string ConstructionInputHelper { get; private set; }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EarnedValuePerProjectViewModel> GetEarnedValueAnalysis(string projectId)
        {
            var list = new List<EarnedValuePerProjectViewModel>();
            int id = Convert.ToInt32(projectId);
            var currentProject = db.ProjectInformation.Find(id);
            if(currentProject != null)
            {
                var evms = db.EarnedValuePerProject.Where(eva => eva.ProjectInformationId == id).ToList();
                if (evms.Any())
                {
                    foreach (EarnedValuePerProject evm in evms)
                    {
                        var record = new EarnedValuePerProjectViewModel
                        {
                            ProjectName = currentProject.Name,
                            Year = evm.GlFiscalYear,
                            Month = ConstructionInputsHelper.GetConstructionPeriod(evm.ConstructionPeriodId),
                            BAC = evm.BAC,
                            PV = evm.BCWS,
                            EV = evm.BCWP,
                            AC = evm.ACWP,
                            PCP = evm.PCP,
                            PCA = evm.PCA,
                            CV = evm.CV,
                            CVPercentage = evm.CVPercentage,
                            CPI = evm.CPI,
                            SV = evm.SV,
                            SVPercentage = evm.SVPercentage,
                            SPI = evm.SPI,
                            CR = evm.CR,
                            TCPI = evm.TCPI,
                            TSPI = evm.TSPI,
                            EAC = evm.EAC,
                            VAC = evm.VAC,
                            ETC = evm.ETC,
                            ISAC = evm.ISAC,
                        };
                        list.Add(record);
                    }
                }
            
            }
            return list;
        }
    }
}
