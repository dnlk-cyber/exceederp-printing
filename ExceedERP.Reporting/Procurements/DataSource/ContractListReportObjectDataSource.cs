using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class ContractListReportObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ContractListReportViewModel> GetPurchases(DateTime from,DateTime to)
        {
            List<ContractListReportViewModel> contractList = new List<ContractListReportViewModel>();
          
                var contracts = db.ContractManagements.Where(x => x.SigningDate >=from && x.SigningDate <= to).ToList();


            foreach (var con in contracts)
            {
                var award = db.BidAwardedSuppliers.Find(con.ProcurementRefNo);
                var tender = db.Tenders.Find(award?.Lot.TenderID);

                var contract = new ContractListReportViewModel
                {
                    CompanyName = con.SupplierName,
                    ItemName = "",
                    BidNo = con.TenderRefNo,
                    ContractRef = con.ContractRefNo,
                    ProcurementMethod = con.PurchaseCategory,
                    SigningDate = con.SigningDate,
                    ExpiryDate = con.DeliveryDate
                };
                //Not Known

                double tot = con.DeliveryDate.Subtract(con.SigningDate).TotalDays;
                contract.DateRange = tot;
                contract.PaymentAmount = con.ContractAmount;
                contract.Branch = db.Branch.Find(con.BranchId)?.Name;
                contract.Advance = con.Advance;//Not Known                    
                contract.Cost = con.ContractAmount;
                contract.TenderType = tender?.TenderType.ToString();


                contractList.Add(contract);


            }



            return contractList;
        }

    }
}
