
using ExceedERP.DataAccess.Context;
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
    public class PrintingEstimationOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingEstimationViewModel GetEstimation(int id)
        {           
            var estimation = db.PrintingCostEstimations.FirstOrDefault(x => x.PrintingCostEstimationId == id);
            if(estimation != null)
            {
                var bindingStyle = db.PrintingBindingStyles.Find(estimation.BindingStyleId);
                var jobType = db.PrintingJobTypes.FirstOrDefault(x => x.PrintingJobTypeId == estimation.JobTypeId);
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == estimation.CustomerId);
                var paperSize = db.PrintingPaperSizes.Find(estimation.PrintingCostEstimationId);               
                return new PrintingEstimationViewModel
                {
                    BindingStyle = bindingStyle.BindingStyleName,
                    CoverNoOfColors = estimation.PrintingCostEstimationId,
                    CustomerName = customer.TradeName,
                    JobDescription = estimation.JobDescription,
                    JobType = jobType.JobTypeName,
                     NoOfPages = estimation.TotalTextNoPages,
                     PaperSize = paperSize.PaperSizeName,
                     Quantity = estimation.ProductQuantity,
                     TextNoOfColors = estimation.TextNoOfColors
                };
            }
            return new PrintingEstimationViewModel();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintingMaterialViewModel> GetMaterial(int id)
        {
            List<PrintingMaterialViewModel> list = new List<PrintingMaterialViewModel>();
            var materialEstimations = db.PrintingEstimationMaterialCosts.Where(x => x.PrintingCostEstimationId == id).ToList();
            
            foreach (var material in materialEstimations)
            {
                var category = db.PrintingMaterialCategories.Find(material.PrintingMaterialCategoryId);
                var item = db.PrintingMaterialCategoryItems.Find(material.PrintingMaterialCategoryItemId);

                list.Add(new PrintingMaterialViewModel
                {
                    ItemCategory = category.CategoryName,
                    ItemName = item.ItemName,
                    Quantity = material.Quantity,
                    TotalCost = material.TotalCost,
                    UnitCost = material.UnitCost,
                    UoM = material.UnitOfMeasurment
                });
            }

            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintingLaborViewModel> GetLabor(int id)
        {
            List<PrintingLaborViewModel> list = new List<PrintingLaborViewModel>();
            var estimationLaborCosts = db.PrintingEstimationLaborCosts.Where(x => x.PrintingEstimationLaborCostId == id).ToList();
            foreach (var item in estimationLaborCosts)
            {
                var printingProcess = db.PrintingProcesses.Find(item.PrintingProcessId);
                var printingMachine = db.PrintingMachineTypes.Find(item.PrintingMachineTypeId);
                list.Add(new PrintingLaborViewModel
                {
                    EstimatedHour = item.EstimatedHours,
                    LaborRate = item.LaborRate,
                    PrintingProcess = printingProcess.ProcessName,
                    ProcessCategory = item.ProcessCategory,
                    MachineType = printingMachine.MachineTypeName,
                });

            }
            return list;            
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public OverAllCostViewModel GetOverAllCost(int id)
        {
            var overAllCost = db.PrintingOverAllCosts.FirstOrDefault(x => x.PrintingCostEstimationId == id);
            if(overAllCost != null)
            {
                return new OverAllCostViewModel
                {
                    AdminCost = overAllCost.AdminstrativeCost,
                    GrandTotal = overAllCost.GrandTotal,
                    GraphicsCost = overAllCost.GraphicsCost,
                    LaborCost = overAllCost.LaborCost,
                    MaterialCost = overAllCost.MaterialCost,
                    OverHeadCost = overAllCost.OverHeadCost,
                    ProfitMargin = overAllCost.ProfitMargin,
                    TotalProductionCost = overAllCost.TotalProductionCost
                };
            }
            return new OverAllCostViewModel();
        }
    }
}
