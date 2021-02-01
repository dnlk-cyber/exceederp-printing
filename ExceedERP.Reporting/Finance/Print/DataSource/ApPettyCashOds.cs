using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ApPettyCashOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ApPettyCashVm ApPettyCashHeader(int id)
        {
            var apPettyCash = _db.ApPettyCash.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ApPettyCashVm headerDs = new ApPettyCashVm();

            if (apPettyCash != null)
            {
                headerDs.VoucherId = apPettyCash.ApPettyCashId.ToString();
                headerDs.CashierName = apPettyCash.Cashier.Cashier;
                headerDs.Reference = apPettyCash.Reference;
                headerDs.PurposeOfPayment = apPettyCash.PurposeOfPayment;
                headerDs.Date = apPettyCash.Date.ToShortDateString();
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(apPettyCash.CreatedBy)); 
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(apPettyCash.OnlineApprovedBy));
            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApPettyCashPayment> ApPettyCashPayment(int id)
        {
     
           return _db.ApPettyCashPayments.Where(x => x.ApPettyCashId == id).ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApPettyCashUtilization> ApPettyCashUtilization(int id)
        {

            return _db.ApPettyCashUtilizations.Where(x => x.ApPettyCashId == id).ToList();
        }
    }
}
