using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class ArMiscCollectionOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ArMiscCollectionViewModel MiscCollectionHeader(int id)
        {
            var collection = _db.ARMiscelaneousCollections.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            ArMiscCollectionViewModel headerDs = new ArMiscCollectionViewModel();

            if (collection != null)
            {
                headerDs.VoucherId = collection.ARMiscelaneousCollectionID.ToString();
                headerDs.CustomerName = collection.CustomerName;
                headerDs.DocTitile = collection.IsDirectCollection ? "COLLECTION VOUCHER" : "MISCELLANEOUS COLLECTION VOUCHER";
                headerDs.ReceiptAmount = collection.ArMiscelaneousCollectionLines.Sum(x=>x.NetReceipt);
                headerDs.ReceiptMethod = collection.ReceiptMethod;
                headerDs.ReceiptDate = collection.ReceiptDate;
                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(collection.CreatedBy));
                headerDs.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(collection.OnlineApprovedBy));
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
        public List<ArMiscelaneousCollectionLine> ArMiscCollectionDistribution(int id)
        {
            var miscelaneousCollectionLines = _db.ArMiscelaneousCollectionLines.Where(x => x.ARMiscelaneousCollectionID == id).ToList();
            return miscelaneousCollectionLines;
        }
    }
}
