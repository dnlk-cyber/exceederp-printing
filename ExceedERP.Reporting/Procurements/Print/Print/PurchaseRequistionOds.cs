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
    public class PurchaseRequistionOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PurchaseRequistionHeader GetPurchaseRequistions(int id)
        {
            PurchaseRequisition pr = db.PurchaseRequisitions.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
          
            PurchaseRequistionHeader headerDs = new PurchaseRequistionHeader();
            if (pr != null)
            {
                var storeRequisition = db.StoreRequisitions.Find(pr.StoreRequisitionID);
                var fixedRequisition = db.FixedAssetRequisitions.Find(pr.StoreRequisitionID);
                headerDs.PurchaseRequistionNo = InventoryPrintHelper.GetId(pr.PurchaseRequisitionID);
                headerDs.RequestingDate = pr.RequestDate.ToShortDateString();
                headerDs.Model = pr.ModelNo;
                headerDs.PlateNo = pr.PlateNo;
                headerDs.Remark = pr.Remark;
                //for non service
                if (storeRequisition != null)
                {
                    headerDs.RequestingStore = InventoryPrintHelper.StoreName(storeRequisition.StoreCode);
                }

                headerDs.Requestor = InventoryPrintHelper.EmployeeName(pr.EmployeeId);
                headerDs.RequestingProject = InventoryPrintHelper.BranchName(pr.BranchId);
                if(pr.ForBranchId!=null)
                    headerDs.ForProject = InventoryPrintHelper.BranchName((int)pr.ForBranchId);

                if (pr.IsManual)
                {
                    #region manual pr

                    headerDs.CreatedBy =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(pr.CreatedBy));
                    if (pr.DateCreated != null)
                        headerDs.CreatedDate = (DateTime) pr.DateCreated;

                    headerDs.ApproverName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(pr.OnlineApprovedBy));
                    if (pr.OnlineApprovedTime != null)
                        headerDs.ApprovalDate = (DateTime) pr.OnlineApprovedTime;

                    headerDs.AuthorizerName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(pr.SecondOnlineApprovedBy));
                    if (pr.SecondOnlineApprovedTime != null)
                        headerDs.AuthorizationDate = (DateTime) pr.SecondOnlineApprovedTime;

                    // added
                    headerDs.PrApproverName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(pr.MiddleOnlineApprovedBy));
                    if (pr.MiddleOnlineApprovedTime != null)
                        headerDs.PrApprovalDate = (DateTime)pr.MiddleOnlineApprovedTime;

                    
                    #endregion
                }
                else
                {
                    #region auto pr

                    if (pr.ItemType == PurchaseItemType.Stock)
                    {
                        if (storeRequisition != null)
                        {
                            headerDs.CreatedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(storeRequisition.CreatedBy));
                            if (storeRequisition.DateCreated != null)
                                headerDs.CreatedDate = (DateTime)storeRequisition.DateCreated;

                            headerDs.ApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(storeRequisition.SecondOnlineApprovedBy));
                            if (storeRequisition.SecondOnlineApprovedTime != null)
                                headerDs.ApprovalDate = (DateTime)storeRequisition.SecondOnlineApprovedTime;

                            //
                            headerDs.PrApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(pr.OnlineApprovedBy));
                            if (pr.OnlineApprovedTime != null)
                                headerDs.PrApprovalDate = (DateTime)pr.OnlineApprovedTime;

                            headerDs.AuthorizerName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(pr.SecondOnlineApprovedBy));
                            if (pr.SecondOnlineApprovedTime != null)
                                headerDs.AuthorizationDate = (DateTime)pr.SecondOnlineApprovedTime;
                        }

                    }
                    else if (pr.ItemType == PurchaseItemType.FixedAsset)
                    {
                        if (fixedRequisition != null)
                        {
                            headerDs.CreatedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(fixedRequisition.CreatedBy));
                            if (fixedRequisition.DateCreated != null)
                                headerDs.CreatedDate = (DateTime)fixedRequisition.DateCreated;

                            headerDs.ApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(fixedRequisition.SecondOnlineApprovedBy));
                            if (fixedRequisition.SecondOnlineApprovedTime != null)
                                headerDs.ApprovalDate = (DateTime)fixedRequisition.SecondOnlineApprovedTime;

                            //
                            headerDs.PrApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(pr.OnlineApprovedBy));
                            if (pr.OnlineApprovedTime != null)
                                headerDs.PrApprovalDate = (DateTime)pr.OnlineApprovedTime;

                            headerDs.AuthorizerName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(pr.SecondOnlineApprovedBy));
                            if (pr.SecondOnlineApprovedTime != null)
                                headerDs.AuthorizationDate = (DateTime)pr.SecondOnlineApprovedTime;
                        }

                    }
                    else if (pr.ItemType == PurchaseItemType.Service)
                    {
                        headerDs.CreatedBy =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(pr.CreatedBy));
                        if (pr.DateCreated != null)
                            headerDs.CreatedDate = (DateTime)pr.DateCreated;

                        headerDs.ApproverName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(pr.OnlineApprovedBy));
                        if (pr.OnlineApprovedTime != null)
                            headerDs.ApprovalDate = (DateTime)pr.OnlineApprovedTime;

                        headerDs.PrApproverName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(pr.MiddleOnlineApprovedBy));
                        if (pr.MiddleOnlineApprovedTime != null)
                            headerDs.PrApprovalDate = (DateTime)pr.MiddleOnlineApprovedTime;

                        headerDs.AuthorizerName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(pr.SecondOnlineApprovedBy));
                        if (pr.SecondOnlineApprovedTime != null)
                            headerDs.AuthorizationDate = (DateTime)pr.SecondOnlineApprovedTime;

                    }
                   
                    

                    #endregion
                }

                headerDs.PRDate = pr.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.StoreRequistionNo = InventoryPrintHelper.GetId(pr.StoreRequisitionID);
                headerDs.RequestingDepartment = InventoryPrintHelper.WorkUnitName(pr.OrgStructureId);

               
            }

           
            
            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
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
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
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
            List<PurchaseRequisitionItem> requisitionItems = db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id).ToList();
            
            return requisitionItems;
        }
    }
}