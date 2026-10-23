using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Reporting.ViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ExceedERP.Core.Domain.Common;
using System.Drawing;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Budget;
//using ExceedERP.Core.Domain.Budget.Task;
using ExceedERP.Reporting.Budget.Print.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Reporting.Budget.Print.DataSource
{
    [DataObject]
    public class BudgetRequest : List<WorkUnitReportViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetRequestViewModel> GetBudgetRequests(string Id)
        {
            List<BudgetRequestViewModel> BudgetList = new List<BudgetRequestViewModel>();
          

            #region SharedDataSource
            string AmharicName = "";
            string Name = "";
            string Address = "";
         
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }

            }

            #endregion


            var BudgetRequest = db.BudgetRequests.Where(x=>x.BudgetRequestID.ToString() == Id).FirstOrDefault();
         
            if (BudgetRequest != null)
            {
                var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
                var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
                var WorkUnit = glChartOfAccountCostCenters.Where(x => x.Values == BudgetRequest.WorkUnit).FirstOrDefault();

                foreach (var RequestLine in BudgetRequest.BudgetRequestLine)
                {
                    var BudgetRequestViewModel = new BudgetRequestViewModel();

                    if (WorkUnit != null)
                        BudgetRequestViewModel.WorkUnit = WorkUnit.Description;

                    BudgetRequestViewModel.Branch = BudgetRequest.Branch.Name;
    
                    BudgetRequestViewModel.LineType = RequestLine.BudgetRequestType.ToString();
                    BudgetRequestViewModel.TotalAmount = RequestLine.Amount;
                    BudgetRequestViewModel.Month = BudgetRequest.FromDate.Date + " -- " + BudgetRequest.ToDate.Date;
                    BudgetRequestViewModel.Category = RequestLine.Remark;
                    BudgetRequestViewModel.RequestNo = BudgetRequest.BudgetRequestID;
                    BudgetRequestViewModel.Year = BudgetRequest.BudgetYear;
                    BudgetRequestViewModel.RequestNo = BudgetRequest.BudgetRequestID;


                    BudgetRequestViewModel.AmharicName = AmharicName;
                    BudgetRequestViewModel.Name = Name;
                    BudgetRequestViewModel.Address =Address;
                    BudgetRequestViewModel.LogoFileName = LogoFileName;

                    BudgetList.Add(BudgetRequestViewModel);
                 }
            }
            else
            {
                return BudgetList;
            }

            return BudgetList;
        }

        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public List<TaskCategory> GetPlanCategory()
        //{


        //    db = new ExceedDbContext();
        //    var result = db.TaskCategorys.ToList();
        //    return result;

        //}
    }
}



