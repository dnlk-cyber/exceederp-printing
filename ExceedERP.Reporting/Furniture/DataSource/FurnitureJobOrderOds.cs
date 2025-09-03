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
    public class FurnitureJobOrderOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]

        public FurnitureobOrderHeaderViewModel JobOrderHeader(int id)
        {

            FurnitureobOrderHeaderViewModel vm = new FurnitureobOrderHeaderViewModel();
            var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == id);
            if (jobOrder != null)
            {
                var customer = db.OrganizationCustomers.FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);
                return new FurnitureobOrderHeaderViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    CRVNo = jobOrder.CRVNo,
                    JobStartDate = jobOrder.JobStartDate,
                    JobEndDate = jobOrder.DeliveryDate,
                    Remark = jobOrder.Remark,
                    Status = jobOrder.Status,
                    ApprovedBy = jobOrder.OnlineApprovedBy,
                    ApprovedDate = jobOrder.OnlineApprovedTime,
                    CheckedBy = jobOrder.SendForApprovalBy,
                    CheckedDate = jobOrder.SendForApprovalTime,
                    CreatedBy = jobOrder.CreatedBy,
                    DateCreated = jobOrder.DateCreated,
                    CreatedBySignature = SignatureHelper.GetUserSignature(jobOrder.CreatedBy),
                    ApprovedBySignature = SignatureHelper.GetUserSignature(jobOrder.OnlineApprovedBy),
                    CheckedBySignature = SignatureHelper.GetUserSignature(jobOrder.SendForApprovalBy)
                };
            }
            return vm;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureJobOrderDetailViewModel> JobItem(int id)
        {
            List<FurnitureJobOrderDetailViewModel> list = new List<FurnitureJobOrderDetailViewModel>();
            var jobOrder = db.FurnitureJobOrderForms.FirstOrDefault(x => x.FurnitureJobOrderFormId == id);
            if (jobOrder != null)
            {
                var jobItems = db.FurnitureJobOrderItems.Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId).ToList();
                foreach (var item in jobItems)
                {
                    var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);
                    
                    list.Add(new FurnitureJobOrderDetailViewModel
                    {
                        JobType = jobType?.JobTypeName ?? "",
                        Quantity = item.Quantity,
                        JobNO = item.JobNo,
                        UnitPriceWithVAT = item.UnitPriceWithVAT,
                        UoM = item.UnitOfMeasurment,
                        UnitPriceBeforeVAT = item.UnitPriceBeforeVAT,
                        Remark = item.Remark
                    });

                }

            }
            return list;
        }
    }
}
