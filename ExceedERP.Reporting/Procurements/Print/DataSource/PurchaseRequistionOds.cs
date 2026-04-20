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
using ExceedERP.DataAccess.Migrations;
using System.Data.Entity;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class PurchaseRequistionOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PurchaseRequistionHeader GetPurchaseRequistions(int id)
        {
            var record = db.PurchaseRequisitions.Find(id);

            //if (record == null || !record.IsDepartmentOnlineApproved || !record.IsOnlineApproved || record.IsVoid)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("You are not authorized to access this resource."),
            //        ReasonPhrase = "Unauthorized"
            //    });
            //}
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
          
            PurchaseRequistionHeader headerDs = new PurchaseRequistionHeader();
            if (record != null)
            {
               // var storeRequisition = db.StoreRequisitions.Find(record.StoreRequisitionID);
               // var fixedRequisition = db.FixedAssetRequisitions.Find(record.StoreRequisitionID);
                headerDs.PurchaseRequistionNo = InventoryPrintHelper.GetId(record.PurchaseRequisitionID);
                headerDs.RequestingDate = record.RequestDate.ToShortDateString();
                headerDs.Model = record.ModelNo;
                headerDs.PlateNo = record.PlateNo;
                headerDs.Remark = record.Remark;
                

                //if (record == null || !record.IsOnlineApproved || record.IsVoid)
                //{
                //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                //    {
                //        Content = new StringContent("You are not authorized to access this resource."),
                //        ReasonPhrase = "Unauthorized"
                //    });
                //}
                if (record != null && record.ItemType == PurchaseItemType.Stock)
                {
                    var sr = db.StoreRequisitions
                        .FirstOrDefault(x => x.StoreRequisitionID == record.StoreRequisitionID);
                    if (sr != null)
                    {
                        headerDs.RequestingStore = InventoryPrintHelper.StoreName(sr.StoreCode);
                    }
                }
                else if (record != null && record.ItemType == PurchaseItemType.FixedAsset)
                {
                    var fsr = db.FixedAssetRequisitions
                        .FirstOrDefault(x => x.FixedAssetRequisitionId == record.StoreRequisitionID);
                    if (fsr != null)
                    {
                        headerDs.RequestingStore = InventoryPrintHelper.StoreName(fsr.StoreCode);
                    }
                }
                else if (record != null && record.ItemType == PurchaseItemType.Service)
                {
                    //requetserid = record.EmployeeId;
                }

                headerDs.Requestor = InventoryPrintHelper.EmployeeName(record.EmployeeId);
                headerDs.RequestingProject = InventoryPrintHelper.BranchName(record.BranchId);
                if(record.ForBranchId!=null)
                    headerDs.ForProject = InventoryPrintHelper.BranchName((int)record.ForBranchId);

                if (record.IsManual)
                {
                    #region manual pr

                    headerDs.CreatedBy =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(record.CreatedBy));
                    headerDs.CreatedBySignature = SignatureHelper.GetUserSignature(record.CreatedBy);
                    if (record.DateCreated != null)
                        headerDs.CreatedDate = (DateTime) record.DateCreated;

                    headerDs.RequestedBy =
                      EmployeeHelper.GetEmployeeEnglishName(record.EmployeeId);
                    headerDs.RequestedBySignature = SignatureHelper.GetUserSignature(UserHelper.GetEmployeeUserName(record.EmployeeId));
                    if (record.RequestDate != null)
                        headerDs.RequestDate = record.RequestDate;


                   
                     headerDs.ApproverName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(record.OnlineApprovedBy));
                        headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(record.OnlineApprovedBy);

                        if (record.OnlineApprovedTime != null)
                            headerDs.ApprovalDate = (DateTime)record.OnlineApprovedTime;
                    

                    headerDs.AuthorizerName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(record.SecondOnlineApprovedBy));
                    headerDs.AuthorizedBySignature = SignatureHelper.GetUserSignature(record.SecondOnlineApprovedBy);

                    if (record.SecondOnlineApprovedTime != null)
                        headerDs.AuthorizationDate = (DateTime) record.SecondOnlineApprovedTime;

                    // added
                    headerDs.PrApproverName =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(record.MiddleOnlineApprovedBy));
                    headerDs.PrApprovedBySignature = SignatureHelper.GetUserSignature(record.MiddleOnlineApprovedBy);

                    if (record.MiddleOnlineApprovedTime != null)
                        headerDs.PrApprovalDate = (DateTime)record.MiddleOnlineApprovedTime;


                    if (record.ItemType == PurchaseItemType.Service)
                    {

                        var prDepartmentValidations = db.PurchaseRequisitionUserDepartmentValidations
                                   .Where(x => x.Status == ApprovalStatuses.Approve)
                                   .Where(x => x.PurchaseRequisitionID == record.PurchaseRequisitionID).ToList();

                        if (prDepartmentValidations.Any())
                        {
                            int maxid = prDepartmentValidations.Max(x => x.PurchaseRequisitionUserDepartmentValidationID);

                            var prevApprovers = prDepartmentValidations.Where(x => x.PurchaseRequisitionUserDepartmentValidationID < maxid);
                            if (prevApprovers.Any())
                            {
                                int secondMaxid = prevApprovers.Max(x => x.PurchaseRequisitionUserDepartmentValidationID);
                                var approver = prDepartmentValidations.FirstOrDefault(x => x.PurchaseRequisitionUserDepartmentValidationID == secondMaxid);
                                if (approver != null)
                                {
                                    headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                                    headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(approver.CreatedBy);
                                    if (approver.DateCreated != null)
                                        headerDs.ApprovalDate = (DateTime)approver.DateCreated;
                                }

                            }
                        }
                       

                        headerDs.PrApproverName =
                                                 EmployeeHelper.GetEmployeeEnglishName(
                                                     UserHelper.GetUserEmployeeId(record.DepartmentOnlineApprovedBy));
                        headerDs.PrApprovedBySignature = SignatureHelper.GetUserSignature(record.DepartmentOnlineApprovedBy);

                        if (record.DepartmentOnlineApprovedTime != null)
                            headerDs.PrApprovalDate = (DateTime)record.DepartmentOnlineApprovedTime;
                    }

                    #endregion
                }
                else
                {
                    #region auto pr

                    if (record.ItemType == PurchaseItemType.Stock)
                    {
                        var storeRequisition = db.StoreRequisitions.Find(record.StoreRequisitionID);

                        if (storeRequisition != null)
                        {
                            headerDs.CreatedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(storeRequisition.CreatedBy));
                            headerDs.CreatedBySignature = SignatureHelper.GetUserSignature(storeRequisition.CreatedBy);

                            if (storeRequisition.DateCreated != null)
                                headerDs.CreatedDate = (DateTime)storeRequisition.DateCreated;

                            headerDs.RequestedBy =
                              EmployeeHelper.GetEmployeeEnglishName(storeRequisition.EmployeeId);
                            headerDs.RequestedBySignature = SignatureHelper.GetUserSignature(UserHelper.GetEmployeeUserName(storeRequisition.EmployeeId));
                            if (storeRequisition.RequestDate != null)
                                headerDs.RequestDate = storeRequisition.RequestDate;

                            //headerDs.ApproverName =
                            //    EmployeeHelper.GetEmployeeEnglishName(
                            //        UserHelper.GetUserEmployeeId(storeRequisition.SecondOnlineApprovedBy));
                            //headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(storeRequisition.SecondOnlineApprovedBy);

                            //if (storeRequisition.SecondOnlineApprovedTime != null)
                            //    headerDs.ApprovalDate = (DateTime)storeRequisition.SecondOnlineApprovedTime;

                            headerDs.ApproverName =
                              EmployeeHelper.GetEmployeeEnglishName(
                                  UserHelper.GetUserEmployeeId(storeRequisition.OnlineApprovedBy));
                            headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(storeRequisition.OnlineApprovedBy);

                            if (storeRequisition.OnlineApprovedTime != null)
                                headerDs.ApprovalDate = (DateTime)storeRequisition.OnlineApprovedTime;


                            headerDs.PrApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(record.OnlineApprovedBy));
                            headerDs.PrApprovedBySignature = SignatureHelper.GetUserSignature(record.OnlineApprovedBy);

                            if (record.OnlineApprovedTime != null)
                                headerDs.PrApprovalDate = (DateTime)record.OnlineApprovedTime;

                            headerDs.AuthorizerName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(record.SecondOnlineApprovedBy));
                            headerDs.AuthorizedBySignature = SignatureHelper.GetUserSignature(record.SecondOnlineApprovedBy);

                            if (record.SecondOnlineApprovedTime != null)
                                headerDs.AuthorizationDate = (DateTime)record.SecondOnlineApprovedTime;
                        }

                    }
                    else if (record.ItemType == PurchaseItemType.FixedAsset)
                    {
                        var fixedRequisition = db.FixedAssetRequisitions.Find(record.StoreRequisitionID);

                        if (fixedRequisition != null)
                        {
                            headerDs.CreatedBy =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(fixedRequisition.CreatedBy));
                            headerDs.CreatedBySignature = SignatureHelper.GetUserSignature(fixedRequisition.CreatedBy);

                            if (fixedRequisition.DateCreated != null)
                                headerDs.CreatedDate = (DateTime)fixedRequisition.DateCreated;

                            headerDs.RequestedBy =
                             EmployeeHelper.GetEmployeeEnglishName(fixedRequisition.EmployeeId);
                            headerDs.RequestedBySignature = SignatureHelper.GetUserSignature(UserHelper.GetEmployeeUserName(fixedRequisition.EmployeeId));
                            if (fixedRequisition.RequestDate != null)
                                headerDs.RequestDate = fixedRequisition.RequestDate;
                            //headerDs.ApproverName =
                            //    EmployeeHelper.GetEmployeeEnglishName(
                            //        UserHelper.GetUserEmployeeId(fixedRequisition.SecondOnlineApprovedBy));
                            //headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(fixedRequisition.SecondOnlineApprovedBy);

                            //if (fixedRequisition.SecondOnlineApprovedTime != null)
                            //    headerDs.ApprovalDate = (DateTime)fixedRequisition.SecondOnlineApprovedTime;

                            headerDs.ApproverName =
                             EmployeeHelper.GetEmployeeEnglishName(
                                 UserHelper.GetUserEmployeeId(fixedRequisition.OnlineApprovedBy));
                            headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(fixedRequisition.OnlineApprovedBy);

                            if (fixedRequisition.OnlineApprovedTime != null)
                                headerDs.ApprovalDate = (DateTime)fixedRequisition.OnlineApprovedTime;

                            //
                            headerDs.PrApproverName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(record.OnlineApprovedBy));
                            headerDs.PrApprovedBySignature = SignatureHelper.GetUserSignature(record.OnlineApprovedBy);

                            if (record.OnlineApprovedTime != null)
                                headerDs.PrApprovalDate = (DateTime)record.OnlineApprovedTime;

                            headerDs.AuthorizerName =
                                EmployeeHelper.GetEmployeeEnglishName(
                                    UserHelper.GetUserEmployeeId(record.SecondOnlineApprovedBy));
                            headerDs.AuthorizedBySignature = SignatureHelper.GetUserSignature(record.SecondOnlineApprovedBy);

                            if (record.SecondOnlineApprovedTime != null)
                                headerDs.AuthorizationDate = (DateTime)record.SecondOnlineApprovedTime;
                        }

                    }
                    else if (record.ItemType == PurchaseItemType.Service)
                    {
                        headerDs.CreatedBy =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(record.CreatedBy));
                        headerDs.CreatedBySignature = SignatureHelper.GetUserSignature(record.CreatedBy);

                        if (record.DateCreated != null)
                            headerDs.CreatedDate = (DateTime)record.DateCreated;

                        //headerDs.ApproverName =
                        //    EmployeeHelper.GetEmployeeEnglishName(
                        //        UserHelper.GetUserEmployeeId(record.OnlineApprovedBy));
                        //headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(record.OnlineApprovedBy);

                        //if (record.OnlineApprovedTime != null)
                        //    headerDs.ApprovalDate = (DateTime)record.OnlineApprovedTime;

                        headerDs.RequestedBy =
                          EmployeeHelper.GetEmployeeEnglishName(record.EmployeeId);
                        headerDs.RequestedBySignature = SignatureHelper.GetUserSignature(UserHelper.GetEmployeeUserName(record.EmployeeId));
                        if (record.RequestDate != null)
                            headerDs.RequestDate = record.RequestDate;                   

                        

                        headerDs.AuthorizerName =
                            EmployeeHelper.GetEmployeeEnglishName(
                                UserHelper.GetUserEmployeeId(record.SecondOnlineApprovedBy));
                        headerDs.AuthorizedBySignature = SignatureHelper.GetUserSignature(record.SecondOnlineApprovedBy);

                        if (record.SecondOnlineApprovedTime != null)
                            headerDs.AuthorizationDate = (DateTime)record.SecondOnlineApprovedTime;

                        var prDepartmentValidations = db.PurchaseRequisitionUserDepartmentValidations
                                   .Where(x => x.Status == ApprovalStatuses.Approve)
                                   .Where(x => x.PurchaseRequisitionID == record.PurchaseRequisitionID).ToList();

                        if (prDepartmentValidations.Any())
                        {
                            int maxid = prDepartmentValidations.Max(x => x.PurchaseRequisitionUserDepartmentValidationID);

                            var prevApprovers = prDepartmentValidations.Where(x => x.PurchaseRequisitionUserDepartmentValidationID < maxid);
                            if (prevApprovers.Any())
                            {
                                int secondMaxid = prevApprovers.Max(x => x.PurchaseRequisitionUserDepartmentValidationID);
                                var approver = prDepartmentValidations.FirstOrDefault(x => x.PurchaseRequisitionUserDepartmentValidationID == secondMaxid);
                                if (approver != null)
                                {
                                    headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                                    headerDs.ApprovedBySignature = SignatureHelper.GetUserSignature(approver.CreatedBy);
                                    if (approver.DateCreated != null)
                                        headerDs.ApprovalDate = (DateTime)approver.DateCreated;
                                }

                            }
                        }


                        headerDs.PrApproverName =
                                                 EmployeeHelper.GetEmployeeEnglishName(
                                                     UserHelper.GetUserEmployeeId(record.DepartmentOnlineApprovedBy));
                        headerDs.PrApprovedBySignature = SignatureHelper.GetUserSignature(record.DepartmentOnlineApprovedBy);

                        if (record.DepartmentOnlineApprovedTime != null)
                            headerDs.PrApprovalDate = (DateTime)record.DepartmentOnlineApprovedTime;
                    }
                   
                    

                    #endregion
                }

                headerDs.BudgetAuthorizerName =
                       EmployeeHelper.GetEmployeeEnglishName(
                           UserHelper.GetUserEmployeeId(record.BudgetApprovedBy));
                if (record.BudgetApprovedTime != null)
                    headerDs.BudgetAuthorizationDate = (DateTime)record.BudgetApprovedTime;

                headerDs.BudgetAuthorizationSignature = SignatureHelper.GetUserSignature(record.BudgetApprovedBy);

                //headerDs.PRDate = record.RequestDate.ToShortDateString(); //record.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.StoreRequistionNo =record.ItemType!= PurchaseItemType.Service? InventoryPrintHelper.GetId(record.StoreRequisitionID):"";
                headerDs.RequestingDepartment = InventoryPrintHelper.WorkUnitName(record.OrgStructureId);

               
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
            var record = db.PurchaseRequisitions.Find(id);

            //if (record == null || !record.IsDepartmentOnlineApproved || !record.IsOnlineApproved || record.IsVoid)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("You are not authorized to access this resource."),
            //        ReasonPhrase = "Unauthorized"
            //    });
            //}
            List<PurchaseRequisitionItem> requisitionItems = db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisitionID == id).ToList();
            
            return requisitionItems;
        }
    }
}