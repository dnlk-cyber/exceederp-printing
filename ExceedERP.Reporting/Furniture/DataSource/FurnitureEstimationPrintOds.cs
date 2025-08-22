using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Furniture.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Furniture.DataSource
{
    [DataObject]
    public class FurnitureEstimationPrintOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureEstimationViewModel GetHeader(int id)
        {
            FurnitureEstimationViewModel vm = new FurnitureEstimationViewModel();
            var estimation = db.FurnitureEstimationForms.FirstOrDefault(x => x.FurnitureEstimationId == id);
            if(estimation != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == estimation.CustomerId);
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == estimation.JobTypeId);

                return new FurnitureEstimationViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    Date = estimation.DateCreated,
                    EstimationNo = estimation.FurnitureEstimationId,
                    JobType = jobType?.JobTypeName ?? "",
                    Quantity = estimation.Quantity,
                    ApprovedBy = estimation.OnlineApprovedBy,
                    ApprovedDate = estimation.OnlineApprovedTime,
                    CheckedBy = estimation.SendForApprovalBy,
                    CheckedDate = estimation.SendForApprovalTime,
                    CreatedBy = estimation.CreatedBy,
                    DateCreated = estimation.DateCreated,
                };
            }
            return vm;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<MatrialCostViewModel> MaterialCost(int id)
        {
            List<MatrialCostViewModel> list = new List<MatrialCostViewModel>();
            var estimation = db.FurnitureEstimationForms.FirstOrDefault(x => x.FurnitureEstimationId == id);
            if(estimation != null)
            {
                var materialCosts = db.FurnitureMaterialCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId);
                foreach (var materialCost in materialCosts) 
                {
                    var item = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == materialCost.ManufacturingMaterialCategoryItemId);
                    list.Add(new MatrialCostViewModel
                    {
                        ItemName = item?.ItemName ?? "",
                        Quantity = materialCost.Quantity,
                        TotalCost = materialCost.TotalPrice,
                        UnitCost = materialCost.UnitPrice,
                        UOM = materialCost.UnitOfMeasurment
                    });
                }
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<LaborCostViewModel> LaborCost(int id)
        {
            List<LaborCostViewModel> list = new List<LaborCostViewModel>();
            var estimation = db.FurnitureEstimationForms.FirstOrDefault(x => x.FurnitureEstimationId == id);
            if (estimation != null)
            {
                var labors = db.DirectLaborCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
                foreach (var labor in labors)
                {
                    list.Add(new LaborCostViewModel
                    {
                        CostPerMinute = labor.CostPerMinute,
                        TaskType = labor.TaskType,
                        TotalCost = labor.TotalCost,
                        TotalWorkingTime = labor.WorkingTime
                    });
                }
            }
            return list; 
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<OverAllCostViewModel> OverAllCost(int id)
        {
            List<OverAllCostViewModel> list = new List<OverAllCostViewModel>();
            var estimation = db.FurnitureEstimationForms.FirstOrDefault(x => x.FurnitureEstimationId == id);
            if (estimation != null)
            {
                var costs = db.FurnitureOverallCosts.Where(x => x.FurnitureEstimationId == estimation.FurnitureEstimationId).ToList();
                foreach (var item in costs)
                {
                    list.Add(new OverAllCostViewModel
                    {
                        TotalCost = item.GrandTotalCost,
                        AdminstCost = item.AdministrativeCost,
                        LaborCost = item.LabourCost,
                        ManufacturedCost = item.ManufacturingCost,
                        MaterialCost = item.MaterialCost,
                        OverHeadCost = item.OverHeadCost,
                        ProfitMargin = item.ProfitMargin,
                        SellingPrice = item.SellingPrice
                    });
                }
            }
            return list;
        }
    }
}
