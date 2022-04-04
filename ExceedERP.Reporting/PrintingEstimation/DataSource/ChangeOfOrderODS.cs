using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.PrintingEstimation.DataSource
{
    [DataObject]
    public class ChangeOfOrderODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ChangeOfOrderVM> GetChangeOfOrder(string id)
        {
            List<ChangeOfOrderVM> changeList = new List<ChangeOfOrderVM>();
            if (id != null)
            {
                int.TryParse(id, out int changeOrderId);
                var changeOfOrder = db.ChangeOrders.FirstOrDefault(x => x.ChangeOrderId == changeOrderId);
                if (changeOfOrder != null)
                {
                    var change = new ChangeOfOrderVM();

                    var jobOrder = db.Jobs.FirstOrDefault(x => x.JobId == changeOfOrder.JobId);
                    if (jobOrder != null)
                    {
                        change.JobOrderNo = jobOrder.JobNo;

                        var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobOrder.JobTypeId);
                        if (jobType != null)
                        {
                            var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                            if (tax != null)
                            {
                                change.Quantity = changeOfOrder.Quantity;
                                change.UnitPrice = Math.Round(changeOfOrder.Price, 2);
                                change.BeforeTax = Math.Round(changeOfOrder.Quantity * changeOfOrder.Price, 2);
                                change.Tax = Math.Round(changeOfOrder.Quantity * changeOfOrder.Price * tax.CalculatedRate, 2);
                                change.TotalPrice = Math.Round((changeOfOrder.Quantity * changeOfOrder.Price) + (changeOfOrder.Quantity * changeOfOrder.Price * tax.CalculatedRate), 2);
                            }
                        }
                    }

                    change.Reason = changeOfOrder.Reason;
                    change.Size = changeOfOrder.Size;
                    change.InvoiceNo = changeOfOrder.InvoiceNo;
                    change.PreparedBy = changeOfOrder.PreparedBy;
                    change.TypeAndQuantityOfMaterialNeeded = changeOfOrder.TypeAndQuantityOfMaterialNeeded;
                    change.Checked = changeOfOrder.Checked;

                    changeList.Add(change);
                }
            }

            return changeList;
        }
    }
}
