using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Printing;
using ExceedERP.Reporting.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.DataSource
{
    [DataObject]
    public class PrintJobFollowUpOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintJobFollowUpViewModel GetFollowUp(int id)
        {
            var productionFollowUp = db.PrintingProductionFollowUps.FirstOrDefault(x => x.PrintingProductionFollowUpId == id);
            if(productionFollowUp != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == productionFollowUp.CustomerId);
                var textPaper = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == productionFollowUp.TextPaperSizeId);
                var coverPaper = db.PrintingPaperSizes.FirstOrDefault(x => x.PrintingPaperSizeId == productionFollowUp.CoverPaperSizeId);
                
                return new PrintJobFollowUpViewModel
                {
                    CoverNoColor = productionFollowUp.CoverNoOfColor,
                    CustomerName = customer.TradeName,
                    JobOrderNo = productionFollowUp.PrintingJobOrderItemId.ToString(),
                    JobDescription = productionFollowUp.JobDescription,
                    JobType = productionFollowUp.PrintJobType.ToString(),
                    Page = productionFollowUp.Pages,
                    PaperForCover = coverPaper.PaperSizeName,
                    PaperForText = textPaper.PaperSizeName,
                    Quantity = productionFollowUp.Quantity,
                    TextNoColor = productionFollowUp.TextNoOfColor,
                    BindingType = productionFollowUp.BindingStyle.ToString(),
                    ApprovedBy = productionFollowUp.OnlineApprovedBy,
                    ApprovedDate = productionFollowUp.OnlineApprovedTime,
                    CheckedBy = productionFollowUp.IsOnlineTransferredBy,
                    CheckedDate = productionFollowUp.OnlineTransferredTime,
                    DateCreated = productionFollowUp.DateCreated,
                    CreatedBy = productionFollowUp.CreatedBy,
                    CreatedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.SendForApprovalBy),
                    // Pre-Press
                    PrePressApprovedBy = productionFollowUp.PrePressApprovedBy,
                    PrePressApprovedTime = productionFollowUp.PrePressApprovedTime,
                    PrePressCheckedBy = productionFollowUp.PrePressCheckedBy,
                    PrePressCheckedTime = productionFollowUp.PrePressCheckedTime,
                    
                    //PrePressCreatedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.CreatedBy),
                    PrePressApprovedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PrePressApprovedBy),
                    PresPressCheckedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PrePressCheckedBy),
                    //Press
                    PressApprovedBy = productionFollowUp.PressApprovedBy,
                    PressApprovedTime = productionFollowUp.PressApprovedTime,
                    PressCheckedBy = productionFollowUp.PressCheckedBy,
                    PressCheckedTime = productionFollowUp.PressCheckedTime,
                    
                    //PressCreatedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.CreatedBy),
                    PressApprovedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PressApprovedBy),
                    PressCheckedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PressCheckedBy),
                    //Post-Press

                    PostPressApprovedBy = productionFollowUp.PostPressApprovedBy,
                    PostPressApprovedTime = productionFollowUp.PostPressApprovedTime,
                    PostPressCheckedBy = productionFollowUp.PostPressCheckedBy,
                    PostPressCheckedTime = productionFollowUp.PostPressCheckedTime,

                    //PostPressCreatedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.CreatedBy),
                    PostPressApprovedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PostPressApprovedBy),
                    PostPressCheckedBySignature = SignatureHelper.GetUserSignature(productionFollowUp.PostPressCheckedBy),
                };
            }
            return new PrintJobFollowUpViewModel();
        }
       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobFollowUpTaskViweModel> GetJobFollowUpTask(int id, string category = null)
        {
            List<JobFollowUpTaskViweModel> list = new List<JobFollowUpTaskViweModel>();
            var followUpTasks = db.PrintingProductionFollowUpTasks.Where(x => x.PrintingProductionFollowUpId == id).ToList();
            if (Enum.TryParse<PressCategory>(category, out var parsedCategory))
            {
                followUpTasks = followUpTasks.Where(x => x.PressCategory == parsedCategory).ToList();
            }
            foreach (var followUpTask in followUpTasks)
            {
                var printMachine = db.PrintingMachineTypes.FirstOrDefault(x => x.PrintingMachineTypeId == followUpTask.MachineTypeId);
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == followUpTask.OperatorId);
                var vm = new JobFollowUpTaskViweModel
                {
                    Activity = followUpTask.LabourActivity.ToString(),
                    AssingedTo = employee?.FirstNameEnglish + "" + employee?.LastNameEnglish ?? "",
                    EstimatedHour = followUpTask.EstimatedHours,
                    MachineType = printMachine?.MachineTypeName,
                    PlannedOutPut = followUpTask.PlannedOutput,
                    ProcessCategory = followUpTask.PressCategory.ToString(),
                    Unit = followUpTask.UoM,
                    FollowUpDetails = new List<DailyFollowUpDetailViewModel>()
                };
                var followUpDetails = db.PrintingProductionDailyFollowUpDetails
                   .Where(x => x.PrintingProductionFollowUpTaskId == followUpTask.PrintingProductionFollowUpTaskId)
                   .ToList();

                foreach (var item in followUpDetails)
                {
                    vm.FollowUpDetails.Add(new DailyFollowUpDetailViewModel
                    {
                        ActualOutput = item.ActualOutput,
                        EndTime = item.ProductionEndTime,
                        OperatorName = employee?.FirstNameEnglish + "" + employee?.LastNameEnglish ?? "",
                        StartTime = item.ProductionStartTime,
                        TimeSpent = item.TimeSpentHours,
                        Unit = item.UoM
                    });

                }
                list.Add(vm);
               
            }
            return list.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyFollowUpDetailViewModel> GetDailyFollowupDetail(int id)
        {
            List<DailyFollowUpDetailViewModel> list = new List<DailyFollowUpDetailViewModel>();

            var followUpTasks = db.PrintingProductionFollowUpTasks.Where(x => x.PrintingProductionFollowUpId == id).ToList();
            foreach (var followUpTask in followUpTasks)
            {
                var followUpDetails = db.PrintingProductionDailyFollowUpDetails
                    .Where(x => x.PrintingProductionFollowUpTaskId == followUpTask.PrintingProductionFollowUpTaskId)
                    .ToList();

                foreach (var item in followUpDetails)
                {
                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == item.OperatorId);
                    list.Add(new DailyFollowUpDetailViewModel
                    {
                        ActualOutput = item.ActualOutput,
                        EndTime = item.ProductionEndTime,
                        OperatorName = employee?.FirstNameEnglish + "" + employee?.LastNameEnglish ?? "",
                        StartTime = item.ProductionStartTime,
                        TimeSpent = item.TimeSpentHours,
                        Unit = item.UoM
                    });

                }
            }

            return list;
            
        }
    }
}
