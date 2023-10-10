using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    
    [DataObject]
    public class ContractByCategoryReportObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ContractByCategoryReportViewModel> GetContracts(DateTime from, DateTime to, string businessUnitId)
        {
            List<ContractByCategoryReportViewModel> purchaseSupportList = new List<ContractByCategoryReportViewModel>();
            
            var contracts = db.ContractManagements
                .Where(x => x.SigningDate >= from && x.SigningDate <= to)
                    .ToList();

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                contracts = contracts.Where(x => x.BusinessUnitId == bdid).ToList();
            }

            if (contracts.Any())
                {                   

                    foreach (var contract in contracts)
                        {

                            var purchaseSupport = new ContractByCategoryReportViewModel
                            {
                                SupplierName = contract.SupplierName,
                                ContractRefNo = contract.ContractRefNo,
                                TenderRef=contract.TenderRefNo,
                                ContractAmount = contract.ContractAmount
                            };

                         purchaseSupportList.Add(purchaseSupport);
                     }
                }
         

            return purchaseSupportList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ContractByCategoryReportViewModel> GetContractsCompleted(DateTime from, DateTime to, string businessUnitId)
        {
            List<ContractByCategoryReportViewModel> contractsCompleted = new List<ContractByCategoryReportViewModel>();
          

            var contracts = db.ContractManagements.Where(x => x.SigningDate >= from && x.SigningDate <= to).ToList();

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                contracts = contracts.Where(x => x.BusinessUnitId == bdid).ToList();
            }
            
            if (contracts.Any())
            {
                foreach (var contract in contracts)
                {
                    var exist = false;

                    var goodreceive = db.GoodReceives
                        .Where(x => x.IsOnlineApproved && x.IsManual == false && x.IsVoid == false)
                        .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender).FirstOrDefault(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());
                    if (goodreceive != null)
                        exist = true;

                    if (exist == false)
                    {
                        var serviceReceive = db.ServiceReceives
                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                            .FirstOrDefault(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());
                        if (serviceReceive != null)
                            exist = true;
                    }

                    if (exist == false)
                    {
                        var assetReceive = db.FixedAssetReceives
                            .Where(x => x.IsOnlineApproved && x.IsManual == false && x.IsVoid == false)
                            .Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                            .FirstOrDefault(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());
                        if (assetReceive != null)
                            exist = true;
                    }


                    if (exist)
                    {
                        var purchaseSupport = new ContractByCategoryReportViewModel
                        {
                            SupplierName = contract.SupplierName,
                            ContractRefNo = contract.ContractRefNo,
                            TenderRef = contract.TenderRefNo,
                            ContractAmount = contract.ContractAmount,
                            Advance = contract.Advance
                        };

                        contractsCompleted.Add(purchaseSupport);
                    }
                }
            }
            //}

            return contractsCompleted;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ContractByCategoryReportViewModel> GetContractsThatTookAdvance(DateTime from, DateTime to, string businessUnitId)
        {
            List<ContractByCategoryReportViewModel> purchaseSupportList = new List<ContractByCategoryReportViewModel>();


            var contracts = db.ContractManagements
                .Where(x => x.SigningDate >= from && x.SigningDate <= to && x.Advance > 0)
                .ToList();

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                contracts = contracts.Where(x => x.BusinessUnitId == bdid).ToList();
            }

            if (contracts.Any())
            {

               
                foreach (var contract in contracts)
                {

                    var purchaseSupport = new ContractByCategoryReportViewModel
                    {
                        SupplierName = contract.SupplierName,
                        ContractRefNo = contract.ContractRefNo,
                        TenderRef = contract.TenderRefNo,
                        ContractAmount = contract.ContractAmount,
                        Advance = contract.Advance
                    };

                    purchaseSupportList.Add(purchaseSupport);
                }
            }
            

            return purchaseSupportList;
        }

    }
}

