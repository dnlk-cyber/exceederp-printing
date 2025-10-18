using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class FurnitureProductionJobCardOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureProductionJobCardViewModel GetJobCard(int id)
        {
            var jobCard = db.FurnitureJobOrderProductions.Find(id);
            if (jobCard != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobCard.CustomerId)?.TradeName ?? "";
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == jobCard.JobTypeId)?.JobTypeName ?? "";

                return new FurnitureProductionJobCardViewModel
                {
                    CustomerName = customer,
                    DeliveryDate = jobCard.DeliveryDate,
                    JobNo = jobCard.JobNo,
                    JobType = jobType,
                    Quantity = jobCard.Quantity,
                    Unit = jobCard.Unit,
                    ApprovedBy = jobCard.OnlineApprovedBy,
                    ApprovedDate = jobCard.OnlineApprovedTime,
                    CheckedBy = jobCard.SendForApprovalBy,
                    CheckedDate = jobCard.SendForApprovalTime,
                    CreatedBy = jobCard.CreatedBy,
                    DateCreated = jobCard.DateCreated,
                    CreatedBySignature = SignatureHelper.GetUserSignature(jobCard.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(jobCard.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(jobCard.SendForApprovalBy)
                };
            }
            return new FurnitureProductionJobCardViewModel();
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureBillofQuantityMaterialViewModel> GetBillOfQuantityMaterial(int id)
        {
            List<FurnitureBillofQuantityMaterialViewModel> list = new List<FurnitureBillofQuantityMaterialViewModel>();

            var boqMs = db.FurnitureProductionBillOfMaterials.Where(x => x.FurnitureJobOrderProductionId == id).ToList();
            foreach (var item in boqMs)
            {
                var itemName = db.ManufacturingMaterialCategoryItems
                    .FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == item.ManufacturingMaterialCategoryItemId)
                    ?.ItemName
                    ??
                    "";

                list.Add(new FurnitureBillofQuantityMaterialViewModel
                {
                    Quantity = item.Quantity,
                    Unit = item.UnitOfMeasurement,
                    WorkShop = item.WorkshopType.ToString(),
                    ItemName = itemName
                });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureBillofQuantityLaborViewModel> GetBOQLabor(int id)
        {
            List<FurnitureBillofQuantityLaborViewModel> list = new List<FurnitureBillofQuantityLaborViewModel>();

            var boqLs = db.FurnitureProductionBillOfLabors.Where(x => x.FurnitureJobOrderProductionId == id).ToList();
            foreach (var item in boqLs)
            {
                var categoryName = db.ManifucturingTaskCategories
                    .FirstOrDefault(x => x.ManifucturingTaskCategoryId == item.ManufacturingTaskCategoryId)?.Name ?? "";
                list.Add(new FurnitureBillofQuantityLaborViewModel
                {
                    EstimatedMinute = item.EstimatedMinute,
                    NoOfEmployee = item.NumberOfEmloyees,
                    TaskCategory = categoryName,
                    WorkShop = item.WorkShopType.ToString()
                });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureJobAssignmentViewModel> GetJobAssignment(int id)
        {
            List<FurnitureJobAssignmentViewModel> list = new List<FurnitureJobAssignmentViewModel>();

            var jobAssignments = db.FurnitureJobAssignments.Where(x => x.FurnitureJobOrderProductionId == id).ToList();
            foreach (var item in jobAssignments)
            {
                var employee = db.Person_Employee.Find(item.EmployeeId);
                var categoryName = db.ManifucturingTaskCategories
                    .FirstOrDefault(x => x.ManifucturingTaskCategoryId == item.TaskCategoryId)?.Name ?? "";
                list.Add(new FurnitureJobAssignmentViewModel
                {
                    EmployeeName = employee.FirstNameEnglish + " " + employee.LastNameEnglish,
                    TaskAssignedDate = item.TaskAssignedDate,
                    TaskCategory = categoryName,
                    TaskEndDate = item.TaskEndDate,
                    Type = item.Type.ToString(),

                });

            }
            return list;
        }
    }
}
