using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
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
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetAllJobPhases()
        {
            List<SelectListItem> jobPhases = new List<SelectListItem>();
            jobPhases = (Enum.GetValues(typeof(ProductionDepartment)).Cast<ProductionDepartment>().Select(
        e => new SelectListItem() { Text = e.ToString(), Value = e.ToString() })).ToList();

            return jobPhases;
        }
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
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetCustomers()
        {
            var custList = db.OrganizationCustomers.ToList()
                .Select(s =>  new SelectListItem{
                    Value = s.OrganizationCustomerID.ToString(),
                    Text = s.TradeName
                }).ToList();

            return custList;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetJobTypes()
        {
            var custList = db.JobCategories.Where(q=>!q.HaveParent).ToList()
                .Select(s => new SelectListItem
                {
                    Value = s.JobCategoryId.ToString(),
                    Text = s.JobName
                }).ToList();

            return custList;
        }
    }
}
