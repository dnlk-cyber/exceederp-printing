using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class GuaranteeListReportObjectDataSource
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GuaranteeListReportViewModel> GetGuarantee(DateTime from, DateTime to,string gtype)
        {
            List<GuaranteeListReportViewModel> guaranteeList = new List<GuaranteeListReportViewModel>();

            var guarantees = _db.Guarantees
                .Where(x => x.ExpireDate >= from && x.ExpireDate <= to && (x.ValidityStatus == GuaranteeValidityStatus.Active || x.ValidityStatus == GuaranteeValidityStatus.Expired)).ToList();

            if (!String.IsNullOrEmpty(gtype))
                guarantees = guarantees.Where(x => x.GuaranteeType.ToString() == gtype).ToList();

                foreach (var guarantee in guarantees)
                {
                    var guaranteeRecord = new GuaranteeListReportViewModel
                    {
                        SupplierName = guarantee.SupplierName,
                        Purpose = guarantee.Purpose,
                        Bank = guarantee.Bank + "_" + guarantee.BankBranch,
                        Amount = guarantee.Amount,
                        ContractRefNo = guarantee.ContractRefNo,
                        GuaranteeRefNo = guarantee.GuaranteeRefNo
                    };

                    guaranteeList.Add(guaranteeRecord);
                }
            return guaranteeList;
        }
        public List<GuaranteeListReportViewModel> GetGuaranteeRequested(DateTime from, DateTime to, string gtype)
        {
            List<GuaranteeListReportViewModel> guaranteeList = new List<GuaranteeListReportViewModel>();

            var guaranteeRenewalRequests = _db.GuaranteeRenewalRequests.Where(x => x.Date >= from && x.Date >= to).ToList();
            
            foreach (var guaranteeRenewalRequest in guaranteeRenewalRequests)
            {
                var guarantee = _db.Guarantees.FirstOrDefault(x => x.GuaranteeId == guaranteeRenewalRequest.GuaranteeId);
               
                    
                if (guarantee != null)
                {
                    var guarantees = new GuaranteeListReportViewModel
                    {
                        SupplierName = guarantee.SupplierName,
                        Purpose = guarantee.Purpose,
                        Bank = guarantee.Bank + "_" + guarantee.BankBranch,
                        Amount = guarantee.Amount,
                        ContractRefNo = guarantee.ContractRefNo,
                        GuaranteeRefNo = guarantee.GuaranteeRefNo
                    };

                    if ((String.IsNullOrEmpty(gtype))|| (!String.IsNullOrEmpty(gtype)&&guarantee.GuaranteeType.ToString()==gtype))
                            guaranteeList.Add(guarantees);
                }
            }

            return guaranteeList;
        }
        public List<GuaranteeListReportViewModel> GetGuaranteeRenewed(DateTime from, DateTime to, string gtype)
        {
            List<GuaranteeListReportViewModel> guaranteeList = new List<GuaranteeListReportViewModel>();


            var guaranteeRenewed = _db.GuaranteeRenewals.Where(x => x.Date >= from && x.Date <= to).ToList();

            foreach (var guarantee1 in guaranteeRenewed)
            {
                var guarantee = _db.Guarantees.FirstOrDefault(x => x.RenewalStatus == GuaranteeRenewalStatus.Renewed && x.GuaranteeId == guarantee1.GuaranteeId && (x.ValidityStatus == GuaranteeValidityStatus.Active || x.ValidityStatus == GuaranteeValidityStatus.Expired));
                if (guarantee != null)
                {
                    var guaranteeS = new GuaranteeListReportViewModel
                    {
                        SupplierName = guarantee.SupplierName,
                        Purpose = guarantee.Purpose,
                        Bank = guarantee.Bank + "_" + guarantee.BankBranch,
                        Amount = guarantee.Amount,
                        ContractRefNo = guarantee.ContractRefNo,
                        GuaranteeRefNo = guarantee.GuaranteeRefNo
                    };
                    if ((String.IsNullOrEmpty(gtype)) || (!String.IsNullOrEmpty(gtype) && guarantee.GuaranteeType.ToString() == gtype))
                        guaranteeList.Add(guaranteeS);
                }
            }

            return guaranteeList;
        }
        public List<GuaranteeListReportViewModel> GetGuaranteeRequestedButNotRenewd(DateTime from, DateTime to, string gtype)
        {
            List<GuaranteeListReportViewModel> guaranteeList = new List<GuaranteeListReportViewModel>();


            var requested = _db.GuaranteeRenewalRequests.Where(x => x.Date >= from && x.Date <= to).ToList();
            foreach (var request in requested)
            {
                var guarantee = _db.Guarantees.FirstOrDefault(x => x.RenewalStatus != GuaranteeRenewalStatus.Renewed && x.GuaranteeId == request.GuaranteeId);
                if (guarantee != null)
                {
                    var guaranteeS = new GuaranteeListReportViewModel
                    {
                        SupplierName = guarantee.SupplierName,
                        Purpose = guarantee.Purpose,
                        Bank = guarantee.Bank + "_" + guarantee.BankBranch,
                        Amount = guarantee.Amount,
                        ContractRefNo = guarantee.ContractRefNo,
                        GuaranteeRefNo = guarantee.GuaranteeRefNo
                    };
                    if ((String.IsNullOrEmpty(gtype)) || (!String.IsNullOrEmpty(gtype) && guarantee.GuaranteeType.ToString() == gtype))
                        guaranteeList.Add(guaranteeS);
                }
            }

            return guaranteeList;
        }
    }
}
