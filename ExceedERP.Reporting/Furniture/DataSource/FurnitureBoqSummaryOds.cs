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
    public class FurnitureBoqSummaryOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FurnitureBoqViewModel GetBoq(int id)
        {
            FurnitureBoqViewModel vm = new FurnitureBoqViewModel();
            var boq = db.FurnitureBillOfQuantities.FirstOrDefault(x => x.FurnitureBillOfQuantityId == id);
            if (boq != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == boq.CustomerId);
                var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == boq.JobTypeId);
                return new FurnitureBoqViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    JobType = jobType?.JobTypeName ?? "",
                    JobCode = boq.JobNo,
                    Quantity = boq.Quantity,
                    ApprovedBy = boq.OnlineApprovedBy,
                    ApprovedDate = boq.OnlineApprovedTime,
                    CheckedBy = boq.SendForApprovalBy,
                    CheckedDate = boq.SendForApprovalTime,
                    CreatedBy = boq.CreatedBy,
                    DateCreated = boq.DateCreated,
                    CreatedBySignature = SignatureHelper.GetUserSignature(boq.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(boq.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(boq.SendForApprovalBy)
                };

            }
            return vm;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<MaterialBoqViewModel> GetMaterial(int id)
        {
            List<MaterialBoqViewModel> list = new List<MaterialBoqViewModel>();
            var boq = db.FurnitureBillOfQuantities.FirstOrDefault(x => x.FurnitureBillOfQuantityId == id);
            if (boq != null)
            {
                var materials = db.FurnitureBOQMaterials.Where(x => x.FurnitureBillOfQuantityId == boq.FurnitureBillOfQuantityId).ToList();
                foreach (var material in materials)
                {
                    var category = db.ManufacturingMaterialCategories.Find(material.ManufacturingMaterialCategoryId);
                    var item = db.ManufacturingMaterialCategoryItems.FirstOrDefault(x => x.ManufacturingMaterialCategoryItemId == material.ManufacturingMaterialCategoryItemId);
                    list.Add(new MaterialBoqViewModel
                    {
                        MaterialCategory = category?.ManufacturingCategory ?? "",
                        MaterialItem = item?.ItemName ?? "",
                        Quantity = material.Quantity,
                        Unit = material.UnitOfMeasurement,
                        WorkShop = material.WorkshopType
                    });
                }
            }
            return list;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<LaborBoqViewModel> GetLabor(int id)
        {
            List<LaborBoqViewModel> list = new List<LaborBoqViewModel>();
            var boq = db.FurnitureBillOfQuantities.FirstOrDefault(x => x.FurnitureBillOfQuantityId == id);
            if (boq != null)
            {
                var labors = db.FurnitureBOQLabors.Where(x => x.FurnitureBillOfQuantityId == boq.FurnitureBillOfQuantityId).ToList();
                var groupedLabor = labors.GroupBy(x => x.WorkShopType).ToList();
                foreach (var item in groupedLabor)
                {
                    list.Add(new LaborBoqViewModel
                    {
                        WorkShop = item.Key,
                        NumberOfEmployees = item.Sum(x => (int?)x.NumberOfEmloyees) ?? 0,
                        EstimatedMinute = item.Sum(x => (decimal?)x.EstimatedMinute) ?? 0
                    });
                }
                

            }
            return list;
        }
    }
}
