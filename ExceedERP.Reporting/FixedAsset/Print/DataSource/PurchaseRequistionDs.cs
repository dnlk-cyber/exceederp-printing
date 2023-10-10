using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class PurchaseRequistionDs
    {
        ExceedDbContext dbContext = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetPurchaseRequistionHeader GetPurchaseRequistions(int id)
        {
            string storeCode = "";
            int requetserid = 0;
            var purchaseRequisition = dbContext.PurchaseRequisitions.Find(id);
            if (purchaseRequisition != null && purchaseRequisition.ItemType == PurchaseItemType.Stock)
            {
                var sr = dbContext.StoreRequisitions
                    .FirstOrDefault(x => x.StoreRequisitionID == purchaseRequisition.StoreRequisitionID);
                if (sr != null)
                {
                    storeCode = sr.StoreCode;
                    requetserid = (int)sr.EmployeeId;
                }
            }
            else if (purchaseRequisition != null && purchaseRequisition.ItemType == PurchaseItemType.FixedAsset)
            {
                var fsr = dbContext.FixedAssetRequisitions
                    .FirstOrDefault(x => x.FixedAssetRequisitionId == purchaseRequisition.StoreRequisitionID);
                if (fsr != null)
                {
                    storeCode = fsr.StoreCode;
                    requetserid = fsr.EmployeeId;
                }
            }
            else if (purchaseRequisition != null && purchaseRequisition.ItemType == PurchaseItemType.Service)
            {
                requetserid = purchaseRequisition.EmployeeId;
            }
            var organization = dbContext.Companies.FirstOrDefault(x => x.Category == "0");

            var headerDs = new FixedAssetPurchaseRequistionHeader();
            if (purchaseRequisition != null)
            {
                var authorization =
                    dbContext.PurchaseRequisitionPurchaseDepartmentValidations.FirstOrDefault(
                        x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                var storeRequisition = dbContext.FixedAssetRequisitions.Find(purchaseRequisition.StoreRequisitionID);
                headerDs.PurchaseRequistionNo = InventoryPrintHelper.GetId(purchaseRequisition.PurchaseRequisitionID);
                headerDs.RequestingDate = purchaseRequisition.RequestDate.ToShortDateString();
                headerDs.Requestor =  EmployeeHelper.GetEmployeeEnglishName(requetserid);
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.OnlineApprovedBy));
                headerDs.ApprovalBySignature = SignatureHelper.GetUserSignature(purchaseRequisition.OnlineApprovedBy);
                headerDs.RequestingStore =InventoryPrintHelper.StoreName(storeCode);

                //headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.CreatedBy));
                //if (purchaseRequisition.DateCreated != null)
                //    headerDs.CreatedDate = (DateTime)purchaseRequisition.DateCreated;
                //headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.OnlineApprovedBy));
                //if (purchaseRequisition.OnlineApprovedTime != null)
                //    headerDs.ApprovalDate = (DateTime)purchaseRequisition.OnlineApprovedTime;

                //headerDs.AuthorizerName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseRequisition.SecondOnlineApprovedBy));
                //if (purchaseRequisition.SecondOnlineApprovedTime != null)
                //    headerDs.AuthorizationDate = (DateTime)purchaseRequisition.SecondOnlineApprovedTime;

                if (purchaseRequisition.IsManual)
                {
                    #region manual pr

                    headerDs.CreatedBy =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(purchaseRequisition.CreatedBy));
                    if (purchaseRequisition.DateCreated != null)
                        headerDs.CreatedDate = (DateTime)purchaseRequisition.DateCreated;

                    headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(purchaseRequisition.CreatedBy);

                    headerDs.ApproverName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(purchaseRequisition.OnlineApprovedBy));
                    if (purchaseRequisition.OnlineApprovedTime != null)
                        headerDs.ApprovalDate = (DateTime)purchaseRequisition.OnlineApprovedTime;

                    headerDs.ApprovalBySignature = SignatureHelper.GetUserSignature(purchaseRequisition.OnlineApprovedBy);

                    headerDs.AuthorizerName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(purchaseRequisition.MiddleOnlineApprovedBy));
                    if (purchaseRequisition.MiddleOnlineApprovedTime != null)
                        headerDs.AuthorizationDate = (DateTime)purchaseRequisition.MiddleOnlineApprovedTime;

                    headerDs.AuthorizerBySignature = SignatureHelper.GetUserSignature(purchaseRequisition.MiddleOnlineApprovedBy);

                    // added
                    headerDs.PrApproverName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(purchaseRequisition.SecondOnlineApprovedBy));
                    if (purchaseRequisition.SecondOnlineApprovedTime != null)
                        headerDs.PrApprovalDate = (DateTime)purchaseRequisition.SecondOnlineApprovedTime;

                    

                    #endregion
                }
                else
                {
                    #region auto pr

                    if (storeRequisition != null)
                    {
                        headerDs.CreatedBy =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(storeRequisition.CreatedBy));
                        if (storeRequisition.DateCreated != null)
                            headerDs.CreatedDate = (DateTime)storeRequisition.DateCreated;

                        headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(storeRequisition.CreatedBy);

                        headerDs.ApproverName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(storeRequisition.SecondOnlineApprovedBy));
                        if (storeRequisition.SecondOnlineApprovedTime != null)
                            headerDs.ApprovalDate = (DateTime)storeRequisition.SecondOnlineApprovedTime;

                        headerDs.ApprovalBySignature = SignatureHelper.GetUserSignature(storeRequisition.SecondOnlineApprovedBy);
                    }

                    headerDs.AuthorizerName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(purchaseRequisition.OnlineApprovedBy));
                    if (purchaseRequisition.OnlineApprovedTime != null)
                        headerDs.AuthorizationDate = (DateTime)purchaseRequisition.OnlineApprovedTime;

                    headerDs.AuthorizerBySignature = SignatureHelper.GetUserSignature(purchaseRequisition.OnlineApprovedBy);

                    #endregion
                }

                headerDs.PRDate = purchaseRequisition.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.StoreRequistionNo = InventoryPrintHelper.GetId(purchaseRequisition.StoreRequisitionID);
                headerDs.RequestingDepartment = InventoryPrintHelper.WorkUnitName(purchaseRequisition.OrgStructureId);
                headerDs.ForProject = InventoryPrintHelper.BranchName((int)purchaseRequisition.ForBranchId);

            }


            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    dbContext.Addresses.Where(
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
                    dbContext.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
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
            List<PurchaseRequisitionItem> requisitionItems = dbContext.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id).ToList();
           
            return requisitionItems;
        }
    }
}