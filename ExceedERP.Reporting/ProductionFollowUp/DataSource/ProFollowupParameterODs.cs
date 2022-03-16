using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class ProFollowupParameterODs
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobPhases()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases.Add(new SelectListItem
            {
                Value = JobPhase.Printing.ToString(),
                Text = JobPhase.Printing.ToString()
            });
            jobPhases.Add(new SelectListItem
            {
                Value = JobPhase.Finishing.ToString(),
                Text = JobPhase.Finishing.ToString()
            });
            {

            };
            return jobPhases;
        }
    }
}
