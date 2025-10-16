using ExceedERP.Core.Domain.Manufacturing.Setting;
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
    public class FurnitureProductionFollowupOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureFollowupViewModel GetFollowup(int id)
        {
            var jobCard = db.FurnitureJobOrderProductions.Find(id);
            if (jobCard != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobCard.CustomerId)?.TradeName ?? "";
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == jobCard.JobTypeId)?.JobTypeName ?? "";

                return new FurnitureFollowupViewModel
                {
                    CustomerName = customer,
                    JobType = jobType,
                    DeliveryDate = jobCard.DeliveryDate,
                    JobNo = jobCard.JobNo,
                    Quantity = jobCard.Quantity,
                    Unit = jobCard.Unit
                };
            }
            return new FurnitureFollowupViewModel();

        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyFollowupViewModel> GetMetal(int id)
        {
            List<DailyFollowupViewModel> list = new List<DailyFollowupViewModel>();
            var metals = db.FurnitureDailyProductionFollowUps
                           .Where(x => x.FurnitureDailyProductionFollowUpId == id)
                           .Where(x => x.WorkShop == WorkShop.Metal)
                           .ToList(); foreach (var item in metals)
            {
                var employee = db.Person_Employee.Find(item.EmployeeId);
                var categoryName = db.ManifucturingTaskCategories
                    .FirstOrDefault(x => x.ManifucturingTaskCategoryId == item.ManifucturingTaskCategoryId)?.Name ?? "";

                list.Add(new DailyFollowupViewModel
                {
                    ActualProduction = item.DailyProductionActual,
                    DailyProdPlan = item.DailyProductionPlan,
                    DateExcuted = item.DateExcuted,
                    EmployeeName = employee?.FirstNameEnglish + " " + employee?.LastNameEnglish ?? "",
                    TaskType = categoryName,
                    WorkShop = item.WorkShop.ToString()
                });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyFollowupViewModel> GetWood(int id)
        {
            List<DailyFollowupViewModel> list = new List<DailyFollowupViewModel>();
            var wood = db.FurnitureDailyProductionFollowUps
                .Where(x => x.FurnitureDailyProductionFollowUpId == id)
                .Where(x => x.WorkShop == WorkShop.WoodWork)
                .ToList();
            foreach (var item in wood)
            {
                var employee = db.Person_Employee.Find(item.EmployeeId);
                var categoryName = db.ManifucturingTaskCategories
                    .FirstOrDefault(x => x.ManifucturingTaskCategoryId == item.ManifucturingTaskCategoryId)?.Name ?? "";

                list.Add(new DailyFollowupViewModel
                {
                    ActualProduction = item.DailyProductionActual,
                    DailyProdPlan = item.DailyProductionPlan,
                    DateExcuted = item.DateExcuted,
                    EmployeeName = employee?.FirstNameEnglish + " " + employee?.LastNameEnglish ?? "",
                    TaskType = categoryName,
                    WorkShop = item.WorkShop.ToString()
                });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyFollowupViewModel> GetAssembly(int id)
        {
            List<DailyFollowupViewModel> list = new List<DailyFollowupViewModel>();
            var assembly = db.FurnitureDailyProductionFollowUps
                .Where(x => x.FurnitureDailyProductionFollowUpId == id)
                .Where(x => x.WorkShop == WorkShop.Finishing_Assembly)
                .ToList();
            foreach (var item in assembly)
            {
                var employee = db.Person_Employee.Find(item.EmployeeId);
                var categoryName = db.ManifucturingTaskCategories
                    .FirstOrDefault(x => x.ManifucturingTaskCategoryId == item.ManifucturingTaskCategoryId)?.Name ?? "";

                list.Add(new DailyFollowupViewModel
                {
                    ActualProduction = item.DailyProductionActual,
                    DailyProdPlan = item.DailyProductionPlan,
                    DateExcuted = item.DateExcuted,
                    EmployeeName = employee?.FirstNameEnglish + " " + employee?.LastNameEnglish ?? "",
                    TaskType = categoryName,
                    WorkShop = item.WorkShop.ToString()
                });
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<UnfinishedMaterialTransferViewModel> GetUnfinishedMaterialTransfer(int id)
        {
            List<UnfinishedMaterialTransferViewModel> list = new List<UnfinishedMaterialTransferViewModel>();
            var assembly = db.FurnitureUnfinishedMaterialTransferForms
                .Where(x => x.FurnitureJobOrderProductionId == id)
                .ToList();
            foreach (var item in assembly)
            {
                var transferer = db.Person_Employee.Find(item.TransfererId);
                var receiver = db.Person_Employee.Find(item.ReceiverId);
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId)?.JobTypeName ?? "";

                list.Add(new UnfinishedMaterialTransferViewModel
                {
                    JobType = jobType,
                    Quality = item.QualityGrade.ToString(),
                    Quantity = item.Quantity,
                    Reciever = receiver?.FirstNameEnglish + " " + receiver?.LastNameEnglish ?? "",
                    Transferrer = transferer?.FirstNameEnglish + " " + transferer?.LastNameEnglish ?? "",
                    TransferDate = item.TransferDate
                });
            }
            return list;
        }


    }
}
