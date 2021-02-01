using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class ServiceRequistionDs
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PurchaseRequistionHeader GetPurchaseRequistions(int id)
        {
            PurchaseRequisition purchaseRequisition = _db.PurchaseRequisitions.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            PurchaseRequistionHeader headerDs = new PurchaseRequistionHeader
            {
                PurchaseRequistionNo = InventoryPrintHelper.GetId(purchaseRequisition.PurchaseRequisitionID),
                RequestingDate = purchaseRequisition.RequestDate.ToShortDateString(),
                Requestor = EmployeeHelper.GetEmployeeEnglishName(purchaseRequisition.EmployeeId),
                ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.OnlineApprovedBy)),
                AuthorizerName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.MiddleOnlineApprovedBy)),
                RequestingDepartment = InventoryPrintHelper.WorkUnitName(purchaseRequisition.OrgStructureId)
            };

            if (purchaseRequisition.DateCreated != null)
                headerDs.CreatedDate = (DateTime)purchaseRequisition.DateCreated;

           
            if (purchaseRequisition.OnlineApprovedTime != null)
                headerDs.ApprovalDate = (DateTime)purchaseRequisition.OnlineApprovedTime;

           
            if (purchaseRequisition.MiddleOnlineApprovedTime != null)
                headerDs.AuthorizationDate = (DateTime)purchaseRequisition.MiddleOnlineApprovedTime;
            
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
        public List<PurchaseRequisitionItem> RequisitionItems(int id)
        {
            List<PurchaseRequisitionItem> requisitionItems = _db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id).ToList();
            return requisitionItems;
        }
    }
}