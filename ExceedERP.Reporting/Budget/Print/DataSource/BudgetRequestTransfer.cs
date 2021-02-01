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
using ExceedERP.Core.Domain.Budget.Task;
using ExceedERP.Reporting.Budget.Print.ViewModel;

namespace ExceedERP.Reporting.Budget.Print.DataSource
{
    [DataObject]
    public class BudgetRequestTransfer : List<WorkUnitReportViewModel>
    {
        public ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BudgetRequestViewModel> GetBudgetRequestTransfer(string Id)
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


 var BudgetRequestTransferLog = db.BudgetRequests.Where(x=>x.BudgetRequestID.ToString() == Id).FirstOrDefault();
 var BudgetRequestLine = db.BudgetRequestLines.Where(x => x.BudgetRequestId.ToString() == Id).ToList();

 if (BudgetRequestTransferLog != null)
 {
    
         var BudgetRequestViewModel = new BudgetRequestViewModel();
         var HeadOfficeBankName = string.Empty;
         var HeadOfficeBankAddress = string.Empty;
         var HeadOfficeBankNumber = string.Empty;

         var BranchOfficeBankName = string.Empty;
         var BranchOfficeBankAddress = string.Empty;
         var BranchOfficeBankNumber = string.Empty;
         var Branch = db.Branch.Where(x => x.BranchId == BudgetRequestTransferLog.BranchId).FirstOrDefault();
        
     var HeadOfficeBank = db.BudgetBankAccountMaps.Where(x => x.IsHeadOffice).FirstOrDefault();
     var BranchOfficeBank = db.BudgetBankAccountMaps.Where(x => !x.IsHeadOffice && x.BranchId == BudgetRequestTransferLog.BranchId).FirstOrDefault();


     if (HeadOfficeBank != null)
     {
         HeadOfficeBankName = HeadOfficeBank.Name;
         HeadOfficeBankAddress = HeadOfficeBank.Address;
         HeadOfficeBankNumber = HeadOfficeBank.BankNumber;
     
     }

     if (BranchOfficeBank != null)
     {
         BranchOfficeBankName = BranchOfficeBank.Name;
         BranchOfficeBankAddress = BranchOfficeBank.Address;
         BranchOfficeBankNumber = BranchOfficeBank.BankNumber;
     }
         if (Branch != null)
         {
             BudgetRequestViewModel.Branch = Branch.Name;
         }
         BudgetRequestViewModel.TotalAmount = BudgetRequestLine.Sum(x => x.Amount);
             BudgetRequestViewModel.Month = BudgetRequestTransferLog.FromDate.Date.ToShortDateString() + " - " + BudgetRequestTransferLog.ToDate.ToShortDateString();
             BudgetRequestViewModel.Year = BudgetRequestTransferLog.BudgetYear;
             BudgetRequestViewModel.RequestNo = BudgetRequestTransferLog.BudgetRequestID;

             BudgetRequestViewModel.AmharicName = AmharicName;
             BudgetRequestViewModel.Name = Name;
             BudgetRequestViewModel.Address = Address;


             BudgetRequestViewModel.HeadOfficeBankAddress = HeadOfficeBankAddress;
             BudgetRequestViewModel.HeadOfficeBankName = HeadOfficeBankName;
             BudgetRequestViewModel.HeadOfficeBankNumber = HeadOfficeBankNumber;

             BudgetRequestViewModel.BranchBankAddress = BranchOfficeBankAddress;
             BudgetRequestViewModel.BranchBankName = BranchOfficeBankName;
             BudgetRequestViewModel.BranchBankNumber = BranchOfficeBankNumber;

             BudgetRequestViewModel.AmharicName = AmharicName;
             BudgetRequestViewModel.Name = Name;
             BudgetRequestViewModel.Address = Address;

             BudgetRequestViewModel.LogoFileName = LogoFileName;

             BudgetList.Add(BudgetRequestViewModel);
         
 }
 else
 {
     return BudgetList;
 }
            return BudgetList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TaskCategory> GetPlanCategory()
        {


            db = new ExceedDbContext();
            var result = db.TaskCategorys.ToList();
            return result;

        }
    }
}



