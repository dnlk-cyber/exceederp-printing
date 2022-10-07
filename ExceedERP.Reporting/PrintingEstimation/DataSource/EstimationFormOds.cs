using ExceedERP.Core.Domain.PrintingEstimation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.PrintingEstimation.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.PrintingEstimation.DataSource
{
    [DataObject]
    public class EstimationFormOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PrintingEstimationFormVieModel GetData(string id)
        {
            PrintingEstimationFormVieModel model = new PrintingEstimationFormVieModel();
            int.TryParse(id, out int PrEsId);
            var estimationForm = db.EstimationForms.Find(PrEsId);

            if(estimationForm != null)
            {
                var customerName = db.OrganizationCustomers.Find(estimationForm.CustomerId)?.TradeName;
               
                model.Customer = customerName;
                model.Date = estimationForm.Date;
                model.JobNumber = estimationForm.JobNumber;
                model.Quantity = estimationForm.Quantity;
                model.Size = estimationForm.Size;
                model.TextPaper = estimationForm.TextPaper;
                model.TextPrint = estimationForm.TextPrint;
                model.CoverPrint = estimationForm.CoverPrint;
                model.BindingStyle = estimationForm.BindingStyle;
                model.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(estimationForm.CreatedBy));
                model.CheckedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(estimationForm.CheckedBy));
                model.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(estimationForm.ApprovedBy));
                model.Others = model.Others;


            }


            List<MaterialCost> materialCosts = db.MaterialCosts.Where(q => q.EstimationFormId == PrEsId).ToList();
            List<MaterialCostViewModel> Materials = new List<MaterialCostViewModel>();

            foreach (var materialCost in materialCosts)
            {
                var inventory = db.InventoryItems.Find(materialCost.InventoryId)?.Name;
                MaterialCostViewModel materialCostViewModel = new MaterialCostViewModel();
                materialCostViewModel.Material = inventory;
                materialCostViewModel.Quantity = materialCost.Quantity;
                materialCostViewModel.UnitPrice = materialCost.UnitPrice;
                materialCostViewModel.TotalPrice = materialCost.TotalPrice;
                Materials.Add(materialCostViewModel);

            }
            List<LaborCost> laborCosts = db.LaborCosts.Where(q => q.EstimationFormId == PrEsId).OrderBy(l => l.PrintingSteps).ToList();
            List<LaborCostViewModel> Labors = new List<LaborCostViewModel>();
         
            var laborCostGroups = from laborCost in laborCosts
                                  group laborCost by laborCost.PrintingSteps;
            int i = 0,j=0;
            foreach(var laborCostGroup  in  laborCostGroups)
            {
              
                if(laborCostGroup.Key == ExceedERP.Core.Domain.PrintingEstimation.PrintingSteps.Printing)
                {
                    if(i==0)
                    {
                        LaborCostViewModel laborCostViewModel = new LaborCostViewModel();
                        laborCostViewModel.PrintingStepvalue = "Printing";
                        laborCostViewModel.PrintingSteps = laborCostGroup.Key;

                        Labors.Add(laborCostViewModel);
                    }
                    i++;
                    foreach (var ab in laborCostGroup)
                    {
                        var printingStepValue = db.PrintingSteps.Find(ab.PrintingStepId)?.Name;
                        LaborCostViewModel laborCostViewModel = new LaborCostViewModel();
                        laborCostViewModel.PrintingSteps = laborCostGroup.Key;
                        laborCostViewModel.PrintingStepvalue = printingStepValue;
                        laborCostViewModel.WorkingTime = ab.WorkingTime;
                        laborCostViewModel.CostPerHour = ab.CostPerHour;
                        laborCostViewModel.TotalCost = ab.TotalCost;
                        Labors.Add(laborCostViewModel);
                    }
                }
                else if (laborCostGroup.Key == ExceedERP.Core.Domain.PrintingEstimation.PrintingSteps.Finishing)
                {
                    if (j == 0)
                    {
                        LaborCostViewModel laborCostViewModel = new LaborCostViewModel();
                        laborCostViewModel.PrintingStepvalue = "Finishing";
                        laborCostViewModel.PrintingSteps = laborCostGroup.Key;

                        Labors.Add(laborCostViewModel);
                    }
                    j++;
                    foreach (var ab in laborCostGroup)
                    {
                        var printingStepValue = db.PrintingSteps.Find(ab.PrintingStepId)?.Name;
                        LaborCostViewModel laborCostViewModel = new LaborCostViewModel();
                        laborCostViewModel.PrintingSteps = laborCostGroup.Key;
                        laborCostViewModel.PrintingStepvalue = printingStepValue;
                        laborCostViewModel.WorkingTime = ab.WorkingTime;
                        laborCostViewModel.CostPerHour = ab.CostPerHour;
                        laborCostViewModel.TotalCost = ab.TotalCost;
                        Labors.Add(laborCostViewModel);
                    }
                }
                else if (laborCostGroup.Key == 0)
                {
                    foreach (var ab in laborCostGroup)
                    {
                        var printingStepValue = db.PrintingSteps.Find(ab.PrintingStepId)?.Name;
                        LaborCostViewModel laborCostViewModel = new LaborCostViewModel();
                        laborCostViewModel.PrintingSteps = laborCostGroup.Key;
                        laborCostViewModel.PrintingStepvalue = printingStepValue;
                        laborCostViewModel.WorkingTime = ab.WorkingTime;
                        laborCostViewModel.CostPerHour = ab.CostPerHour;
                        laborCostViewModel.TotalCost = ab.TotalCost;
                        Labors.Add(laborCostViewModel);
                    }
                }

            }
           
            var totalMaterialCost = db.MaterialCosts.Where(q => q.EstimationFormId == PrEsId).ToList().Sum(s => s.TotalPrice);
            var totalLaborCost = db.LaborCosts.Where(q => q.EstimationFormId == PrEsId).ToList().Sum(s => s.TotalCost);

            var overallCost = db.OverallCosts.Where(q => q.EstimationFormId == PrEsId).FirstOrDefault();
            OverallCostViewModel overallCostViewModel = new OverallCostViewModel();
            if (overallCost!= null)
            {

                overallCostViewModel.MaterialCost = totalMaterialCost;
                overallCostViewModel.LaborCost = totalLaborCost;
                overallCostViewModel.OverHeadCost = overallCost.OverHeadCost;
                overallCostViewModel.T = overallCost.T;
                overallCostViewModel.MarginalCost = overallCost.MarginalCost;
                overallCostViewModel.NBT = overallCost.NBT;
                overallCostViewModel.TInpercent = overallCost.TInpercent;
                overallCostViewModel.GT = overallCost.GT;
                overallCostViewModel.Graphic = overallCost.Graphic;
                overallCostViewModel.EstimatedNetPrice = overallCost.EstimatedNetPrice;
            }

            model.MaterialCosts = Materials;
            model.LaborCosts = Labors;
            model.OverallCostViewModel = overallCostViewModel;           

            return model;
        }
    }
}
