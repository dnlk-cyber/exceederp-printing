using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class StoreReturnDs
    {
        private readonly ExceedDbContext _dbContext = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public StoreReturnHeaderVm GetStoreReturn(int id)
        {
            var storeReturn = _dbContext.StoreReturns.Find(id);
            var organization = _dbContext.Companies.FirstOrDefault(x => x.Category == "0");

            StoreReturnHeaderVm headerDs = new StoreReturnHeaderVm();
            if (storeReturn != null)
            {
                headerDs.VoucherId = storeReturn.StoreReturnID;
                headerDs.VoucherDate = storeReturn.ReturnDate.ToShortDateString();
                headerDs.Reason = storeReturn.Reason;
                headerDs.Letter = storeReturn.ConfirmationLetter;
                headerDs.Reference = storeReturn.Reference;


                headerDs.VoucherStore = storeReturn.StoreCode;
                headerDs.VoucherApprovedDate = storeReturn.OnlineApprovedTime.GetValueOrDefault().ToShortDateString();
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(storeReturn.CreatedBy));
                headerDs.CreatedDate = storeReturn.DateCreated.GetValueOrDefault().ToShortDateString();
            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _dbContext.Addresses.Where(
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
                    _dbContext.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StoreReturnItems> ReturnItems(int id)
        {
            List<StoreReturnItems> storeReturn = _dbContext.StoreReturnItemss.Where(x => x.StoreReturnID == id).ToList();
            return storeReturn;
        }
    }
}