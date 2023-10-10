using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Vml.Office;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Helpers.Inventory;

namespace ExceedERP.Helpers.FixedAsset
{
    public static class FixedAssetHelper
    {
        
       /// <summary>
       /// A static method that takes assetRegistrationId and storeCode as a param and returns true if this asset exists in the LocationTracking model
       /// </summary>
       /// <param name="regId"></param>
       /// <param name="storeCode"></param>
       /// <returns></returns>
        public static bool IsAssetCurrentLocationStore(int regId,string storeCode)
        {
            var Db = new ExceedDbContext();
            bool isStore = false;

            var record = Db.LocationTrackings.Where(x => x.AssetRegistrationId == regId).ToList();
           
            if (record.Any())
            {
                var maxid = record.Max(x => x.LocationTrackingId);
                var locationTracking = Db.LocationTrackings.Find(maxid);
                if (locationTracking != null && locationTracking.StoreCode == storeCode)
                {
                    isStore = true;
                }
            }
            return isStore;
        }

        public static void UpdateAssetLocation(int assetid, int branchid, int employeeid, string storecode, LocationType type, string user, FixedAssetTransactionType transactionType, string tno)
        {
            var db = new ExceedDbContext();

            var asset = db.AssetRegistrations.Find(assetid);

            var newLocation = new LocationTracking();

            var previosLocation = db.LocationTrackings.Where(x => x.AssetRegistrationId == asset.AssetRegistrationId);

            if (previosLocation.Any())
            {
                #region update prev location

                int maxid = previosLocation.Max(x => x.LocationTrackingId);

                var prevRecord = previosLocation.FirstOrDefault(x => x.LocationTrackingId == maxid);

                if (prevRecord != null)
                {
                    prevRecord.IsCurrent = false;

                    prevRecord.EndDateDate = DateTime.Today;

                    prevRecord.LastModified = DateTime.Now;

                    prevRecord.ModifiedBy = user;

                    db.Entry(prevRecord).State = EntityState.Modified;
                }

                db.SaveChanges();

                #endregion
                
            }
            #region add new location

            if (asset != null)
            {
                newLocation.AssetRegistrationId = asset.AssetRegistrationId;
                newLocation.TransactionType = transactionType;
                newLocation.TransactionNo =tno;
                newLocation.LocationType = type;
                newLocation.EmployeeId = employeeid.ToString();
                newLocation.StartDate = DateTime.Today;
                newLocation.BranchId = branchid;
                newLocation.StoreCode = storecode;
                newLocation.IsCurrent = true;
                newLocation.DateCreated = DateTime.Now;
                newLocation.CreatedBy = user;
                db.LocationTrackings.Add(newLocation);
                db.SaveChanges();
            }

            #endregion
           
            
        }


        #region Update LocationTracking for UserCardItemsController
        public static void UpdateLocationTracking(UserCardItems userCardItems, string user, int branchId)
        {
            ExceedDbContext db = new ExceedDbContext();

            LocationTracking locationTrackingCurrent = new LocationTracking();
            if (userCardItems != null)
            {

                var asset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == userCardItems.AssetRegistrationId);

                if (asset != null)
                {
                    #region Modify previouse LocationTracking

                    var locationTrackingPrevs =
                        db.LocationTrackings.Where(x => x.AssetRegistrationId == userCardItems.AssetRegistrationId && x.IsCurrent).ToList();

                    if (locationTrackingPrevs.Any())
                    {
                        foreach (var locationTrackingPrev in locationTrackingPrevs)
                        {
                            locationTrackingPrev.IsCurrent = false;
                            locationTrackingPrev.EndDateDate = DateTime.Now;
                            locationTrackingPrev.LastModified = DateTime.Now;
                            locationTrackingPrev.ModifiedBy = user;
                            db.Entry(locationTrackingPrev).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    #endregion

                    #region Record New LocationTracking

                    locationTrackingCurrent.AssetRegistrationId = userCardItems.AssetRegistrationId;
                    locationTrackingCurrent.LocationType = LocationType.Employee;
                    locationTrackingCurrent.StartDate = DateTime.Now;
                    locationTrackingCurrent.Description = userCardItems.Remark;
                    locationTrackingCurrent.StoreCode = null;
                    locationTrackingCurrent.EmployeeId = userCardItems.EmployeeId.ToString();
                    locationTrackingCurrent.BranchId = branchId;
                    locationTrackingCurrent.TransactionType = FixedAssetTransactionType.Issue;
                    locationTrackingCurrent.TransactionNo = null;
                    locationTrackingCurrent.IsCurrent = true;
                    locationTrackingCurrent.OrgBranchName = userCardItems.OrgBranchName;
                    locationTrackingCurrent.DateCreated = DateTime.Now;
                    locationTrackingCurrent.CreatedBy = user;
                    db.LocationTrackings.Add(locationTrackingCurrent);
                    db.SaveChanges();

                    #endregion
                }
            }
        }
        #endregion



        #region UserCard and LocationTracking for AssetRegistration
        /// <summary>
        /// update userCard for asset registration
        /// </summary>
        /// <param name="assetRegistrationId"></param>
        /// <param name="user"></param>
        public static void UpdateUserCardFromAssetRegistration(int assetRegistrationId, string user)
        {

            var db = new ExceedDbContext();

            UserCardItems userCard = new UserCardItems();

            var asset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == asset.EmployeeId);

            
            var userCardItem = db.UserCardItems.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            #region update usercard
            // if we are editing entry in AssetRegistration then just modify the record in UserCard
            if (asset != null && employee != null)
            {

                if (userCardItem != null)
                {
                    userCardItem.EmployeeId = employee.EmployeeId;
                    userCardItem.EmployeeFullName = employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                    userCardItem.AssetRegistrationId = asset.AssetRegistrationId;
                    userCardItem.AssetRegistration = asset;
                    userCardItem.Name = asset.Name;
                    userCardItem.UOM = asset.Unit;
                    userCardItem.Quanity = asset.Quanity;
                    userCardItem.UnitPrice = asset.BookValue;
                    userCardItem.Tag = asset.Tag;
                    userCardItem.SerialNo = asset.SerialNo;
                    userCardItem.SIVNo = asset.SIV;
                    userCardItem.GRNDate = asset.GRNDate;
                    userCardItem.GRN = asset.GRN;
                    userCardItem.Remark = asset.Remark;
                    userCardItem.OrgBranchName = asset.OrgBranchName;
                    userCardItem.Active = true;
                    userCardItem.IssueDate = DateTime.Today;

                    userCardItem.DateCreated = DateTime.Now;
                    userCardItem.CreatedBy = user;

                    //modify usercard in db
                    db.UserCardItems.Attach(userCardItem);
                    db.Entry(userCardItem).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else  // else create new userCard
                {
                    userCard.EmployeeId = employee.EmployeeId;
                    userCard.EmployeeFullName =
                        employee.FirstName + " " + employee.MiddleName + " " + employee.LastName;
                    userCard.AssetRegistrationId = asset.AssetRegistrationId;
                    userCard.AssetRegistration = asset;
                    userCard.Name = asset.Name;
                    userCard.UOM = asset.Unit;
                    userCard.Quanity = asset.Quanity;
                    userCard.UnitPrice = asset.BookValue;
                    userCard.Tag = asset.Tag;
                    userCard.SerialNo = asset.SerialNo;
                    userCard.SIVNo = asset.SIV;
                    userCard.GRNDate = asset.GRNDate;
                    userCard.GRN = asset.GRN;
                    userCard.Remark = asset.Remark;
                    userCard.OrgBranchName = asset.OrgBranchName;
                    userCard.Active = true;
                    userCard.IssueDate = DateTime.Today;

                    userCard.DateCreated = DateTime.Now;
                    userCard.CreatedBy = user;

                    //create new entry in db
                    db.UserCardItems.Add(userCard);
                    db.SaveChanges();

                }
            }

            #endregion
        }
        /// <summary>
        /// Updates LocationTracking for AssetRegistration
        /// </summary>
        /// <param name="assetRegistrationId"></param>
        /// <param name="user"></param>
        public static void UpdateAssetLocationTrackingForAssetRegistration(int assetRegistrationId, string user)
        {
            var db = new ExceedDbContext();

            var asset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            var locationTrackingUpdate =
                db.LocationTrackings.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            var locationTracking = new LocationTracking();

            #region add new location


            if (asset != null)
            {
                if (locationTrackingUpdate == null)
                {
                    locationTracking.AssetRegistrationId = asset.AssetRegistrationId;

                    locationTracking.LocationType = asset.StoreCode != null ? LocationType.Store :
                        asset.EmployeeId != null ? LocationType.Employee : LocationType.NotSet;
                    locationTracking.EmployeeId = asset.EmployeeId;
                    locationTracking.Description = null;

                    locationTracking.StartDate = DateTime.Today;
                    locationTracking.BranchId = asset.BranchId;
                    locationTracking.StoreCode = asset.StoreCode;
                    locationTracking.OrgStructureId = asset.OrgStructureId;
                    locationTracking.TransactionType = asset.EmployeeId != null
                        ? FixedAssetTransactionType.UserCard
                        : FixedAssetTransactionType.Store;
                    locationTracking.TransactionNo = null;
                    locationTracking.OrgBranchName = asset.OrgBranchName;
                    locationTracking.IsCurrent = true;
                    locationTracking.DateCreated = DateTime.Now;
                    locationTracking.CreatedBy = user;

                    //create in database
                    db.LocationTrackings.Add(locationTracking);
                    db.SaveChanges();
                }
                else
                {
                    locationTrackingUpdate.AssetRegistrationId = asset.AssetRegistrationId;

                    locationTrackingUpdate.LocationType = asset.StoreCode != null ? LocationType.Store :
                        asset.EmployeeId != null ? LocationType.Employee : LocationType.NotSet;
                    locationTrackingUpdate.EmployeeId = asset.EmployeeId;
                    locationTrackingUpdate.Description = null;

                    locationTrackingUpdate.StartDate = DateTime.Today;
                    locationTrackingUpdate.BranchId = asset.BranchId;
                    locationTrackingUpdate.StoreCode = asset.StoreCode;
                    locationTrackingUpdate.OrgStructureId = asset.OrgStructureId;
                    locationTrackingUpdate.TransactionType = asset.EmployeeId != null
                        ? FixedAssetTransactionType.UserCard
                        : FixedAssetTransactionType.Store;
                    locationTrackingUpdate.TransactionNo = null;
                    locationTrackingUpdate.OrgBranchName = asset.OrgBranchName;
                    locationTrackingUpdate.IsCurrent = true;
                    locationTrackingUpdate.DateCreated = DateTime.Now;
                    locationTrackingUpdate.CreatedBy = user;

                    //modify in database
                    db.LocationTrackings.Attach(locationTrackingUpdate);
                    db.Entry(locationTrackingUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            #endregion


        }

        #endregion


        #region Update UserCard and LocationTracking for FixedAssetIssueItem
        /// <summary>
        /// Updates UserCard for FixedAssetIssueItem
        /// </summary>
        /// <param name="issueItem"></param>
        /// <param name="user"></param>
        public static void UpdateUserCard(FixedAssetIssueItem issueItem, string user)
        {

            var db = new ExceedDbContext();

            UserCardItems userCard = new UserCardItems();


            var assetRegistrationId = issueItem.AssetRegistrationId;

            var employeeId = issueItem.FixedAssetIssue.EmployeeId;
            

            var fixedAssetIssueId = issueItem.FixedAssetIssueID;

            var assetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId.Equals(assetRegistrationId));

            var requester = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.Equals(employeeId));
            
            
            var fixedAssetIssue = db.FixedAssetIssues.FirstOrDefault(x => x.FixedAssetIssueID == fixedAssetIssueId);
           

            if (fixedAssetIssue != null)
            {
                var recipientId = fixedAssetIssue.RecipientId;
            
                var recipient = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.Equals(fixedAssetIssue.RecipientId));

                string orgId = "";

                #region Update UserCard

                if (recipient != null)
                {
                    userCard.EmployeeId = recipientId;
                    userCard.EmployeeFullName = recipient.FirstNameEnglish + " " + recipient.MiddleNameEnglish + " " +
                                                recipient.LastNameEnglish;
                    orgId = GetEmployeeDepartmentIdByEmpId(recipientId).ToString();


                    if (assetRegistration != null)
                    {
                        userCard.AssetRegistrationId = assetRegistration.AssetRegistrationId;
                        userCard.Name = assetRegistration.Name;
                        userCard.Tag = assetRegistration.Tag;
                        userCard.UOM = issueItem.Unit;
                        userCard.IssueDate = fixedAssetIssue.IssueDate;
                        userCard.UnitPrice = assetRegistration.BookValue; //issueItem.UnitPrice;
                        userCard.Quanity = issueItem.IssuedQuantity;
                        userCard.SerialNo = issueItem.SerialNo;
                        userCard.SIVNo = fixedAssetIssue.FixedAssetIssueID.ToString();
                        userCard.Active = true;
                        userCard.OrgBranchName = "";
                        userCard.DateCreated = DateTime.Now;
                        userCard.LastModified = DateTime.Now;
                        userCard.CreatedBy = user;

                        //save on db
                        db.UserCardItems.Add(userCard);
                        db.SaveChanges();
                    }
                }
            }
            
            #endregion

        }
        /// <summary>
        /// Updates asset Info for FixedAssetIssueItem
        /// </summary>
        /// <param name="issueItem"></param>
        /// <param name="user"></param>
        /// <param name="orgId"></param>
        public static void UpdateAssetInfo(FixedAssetIssueItem issueItem, string user)
        {
            var db = new ExceedDbContext();

            var assetRegistrationId = issueItem.AssetRegistrationId;


            var fixedAssetIssueId = issueItem.FixedAssetIssueID;

            var assetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId.Equals(assetRegistrationId));


            var assetIssue = db.FixedAssetIssues.FirstOrDefault(x => x.FixedAssetIssueID.Equals(fixedAssetIssueId));


            if (assetIssue != null)
            {
                var recipientId = assetIssue.RecipientId;

                var storeCode = assetIssue.StoreCode;

                var orgId = GetEmployeeDepartmentIdByEmpId(recipientId).ToString();

                if (assetRegistration != null)
                {
                    assetRegistration.StoreCode = storeCode;
                    assetRegistration.SIV = issueItem.FixedAssetIssueID.ToString();
                    assetRegistration.EmployeeId = recipientId.ToString();
                    assetRegistration.OrgStructureId = orgId;

                    //save on db
                    db.AssetRegistrations.Attach(assetRegistration);
                    db.Entry(assetRegistration).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            
        }
        /// <summary>
        /// Updates locationTracking for FixedAssetIssueItem
        /// </summary>
        /// <param name="issueItem"></param>
        /// <param name="user"></param>
        /// <param name="orgId"></param>
        public static void UpdateLocationTracking(FixedAssetIssueItem issueItem, string user)
        {
            ExceedDbContext db = new ExceedDbContext();

            var assetRegistrationId = issueItem.AssetRegistrationId;


            var fixedAssetIssueId = issueItem.FixedAssetIssueID;

            var assetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            var locationTrackingPrv = db.LocationTrackings.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);

            #region Modify prev locationTracking 
            if (locationTrackingPrv != null)
            {
                locationTrackingPrv.IsCurrent = false;
                locationTrackingPrv.EndDateDate = DateTime.Now;

                db.LocationTrackings.Attach(locationTrackingPrv);
                db.Entry(locationTrackingPrv).State = EntityState.Modified;
                db.SaveChanges();
            }
            #endregion

            var assetIssue = db.FixedAssetIssues.FirstOrDefault(x => x.FixedAssetIssueID.Equals(fixedAssetIssueId));

            if (assetIssue != null)
            {
                var recipientId = assetIssue.RecipientId;

                var orgId = GetEmployeeDepartmentIdByEmpId(recipientId);

                var storeCode = assetIssue.StoreCode;

                if (assetRegistration != null)
                {
                    var location = new LocationTracking
                    {
                        AssetRegistrationId = assetRegistration.AssetRegistrationId,
                        LocationType = LocationType.Employee,
                        StartDate = DateTime.Today,
                        StoreCode = storeCode,
                        BranchId = issueItem.FixedAssetIssue.BranchId,
                        EmployeeId = recipientId.ToString(),
                        OrgStructureId = orgId.ToString(),
                        TransactionType = FixedAssetTransactionType.Issue,
                        TransactionNo = issueItem.FixedAssetIssueID.ToString(),
                        DateCreated = DateTime.Today,
                        CreatedBy = user
                    };
                    db.LocationTrackings.Add(location);
                    db.SaveChanges();
                }

            }

      
        }
        #endregion

        #region Update for FixedAssetReturnItem
        /// <summary>
        /// update locationTracking for FixedAssetReturnItem
        /// </summary>
        /// <param name="fixedAssetReturnItem"></param>
        /// <param name="user"></param>
        public static void UpdateLocationTracking(FixedAssetReturnItem fixedAssetReturnItem, string user)
        {
            ExceedDbContext db = new ExceedDbContext();
            string orgId = null;
            var assetRegistrationId = fixedAssetReturnItem.AssetRegistrationId;
            var asset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId);
            var fixedAssetReturn = db.FixedAssetReturns.FirstOrDefault(x =>
                x.FixedAssetReturnId.Equals(fixedAssetReturnItem.FixedAssetReturnId));
            if (fixedAssetReturn != null)
            {

                var employeeId = fixedAssetReturn.EmployeeId;

                orgId = GetEmployeeDepartmentIdByEmpId(employeeId).ToString();
            }

            var pastLocations = db.LocationTrackings.Where(x => x.AssetRegistrationId == assetRegistrationId && x.IsCurrent == true).ToList();
            if (pastLocations.Any())
            {

                foreach (var pastLocation in pastLocations)
                {
                    pastLocation.IsCurrent = false;
                    db.Entry(pastLocation).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }


            if (asset != null)
            {

                var currentLocation = new LocationTracking
                {
                    AssetRegistrationId = asset.AssetRegistrationId,
                    StoreCode = fixedAssetReturnItem.FixedAssetReturn.StoreCode,
                    //EmployeeId = employeeId,
                    OrgStructureId = orgId,
                    LocationType = LocationType.Store,
                    StartDate = DateTime.Today,
                    BranchId = InventoryHelper.GetStoreBranch(fixedAssetReturnItem.FixedAssetReturn.StoreCode),
                    TransactionType = FixedAssetTransactionType.Return,
                    TransactionNo = fixedAssetReturnItem.FixedAssetReturnId.ToString(),
                    DateCreated = DateTime.Now,
                    CreatedBy = user,
                };

                db.LocationTrackings.Add(currentLocation);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// update asset info for FixedAssetReturnItem
        /// </summary>
        /// <param name="fixedAssetReturnItem"></param>
        public static void UpdateAssetInfo(FixedAssetReturnItem fixedAssetReturnItem)
        {
            ExceedDbContext db = new ExceedDbContext();

            var assetRegistrationId = fixedAssetReturnItem.AssetRegistrationId;

            var fixedAssetReturn = db.FixedAssetReturns.FirstOrDefault(x =>
                x.FixedAssetReturnId.Equals(fixedAssetReturnItem.FixedAssetReturnId));

            if (fixedAssetReturn is null) return;

            var employeeId = fixedAssetReturn.EmployeeId;
            var orgId = GetEmployeeDepartmentIdByEmpId(employeeId);
            var asset = db.AssetRegistrations.Find(assetRegistrationId);

            if (asset != null)
            {

                asset.StoreCode = fixedAssetReturnItem.FixedAssetReturn.StoreCode;
                asset.EmployeeId = employeeId.ToString();
                asset.OrgStructureId = orgId.ToString();
                asset.LastModified = DateTime.Now;

                //save on db
                db.Entry(asset).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// removes the previous UserCard for FixedAssetReturnItem
        /// </summary>
        /// <param name="fixedAssetReturnItem"></param>
        public static void RemovePreviousUserCard(FixedAssetReturnItem fixedAssetReturnItem)
        {
            ExceedDbContext db = new ExceedDbContext();

            int assetRegistrationId = fixedAssetReturnItem.AssetRegistrationId;

            var userCardItems = db.UserCardItems.Where(x => x.AssetRegistrationId == assetRegistrationId).ToList();

            foreach (var userCardItem in userCardItems)
            {
                db.Entry(userCardItem).State = EntityState.Deleted;
                db.SaveChanges();
            }
        }
        #endregion


        #region Update for AssetTransfer

  /// <summary>
  /// update usercard for AssetTransfer
  /// </summary>
  /// <param name="assetTransferItem"></param>
  /// <param name="user"></param>
        public static void UpdateUserCard(AssetTransferItem assetTransferItem, string user)
        {
            ExceedDbContext db = new ExceedDbContext();

            UserCardItems userCardItems = new UserCardItems();

            int assetRegistrationId = assetTransferItem.AssetRegistrationId;

            //AssetRegistration
            var asset = db.AssetRegistrations.Find(assetRegistrationId);

            var assetTransferId = assetTransferItem.AssetTransferId;

            //AssetTransfer
            var assetTransfer = db.AssetTransfers.Find(assetTransferId);

            if (assetTransfer != null && asset != null)
            {
                
                if (assetTransfer.Type == TransferType.PersonToPerson)
                {
                    var custodianBeforeId = assetTransfer.CustodianBeforeTransfer;
                    var transferToId = assetTransfer.TransferTo;

                    var custodianBeforeTransfer = db.Person_Employee.Find(custodianBeforeId);
                    
                    var transferTo = db.Person_Employee.Find(transferToId);

                    var transferToEmpId = transferTo?.EmployeeId.ToString();



                    var orgId = transferTo != null
                        ? GetEmployeeDepartmentIdByEmpId(transferTo.EmployeeId).ToString()
                        : "";


                    #region Update Current UserCard

                    if (transferTo != null)
                    {

                        userCardItems.EmployeeId = transferTo.EmployeeId;
                        userCardItems.EmployeeFullName =
                            transferTo.FirstNameEnglish + " " + transferTo.MiddleNameEnglish + " " +
                            transferTo.LastNameEnglish;

                        userCardItems.UOM = asset.Unit;
                        userCardItems.AssetRegistrationId = assetRegistrationId;
                        userCardItems.Name = asset.Name;
                        userCardItems.Tag = asset.Tag;
                        userCardItems.IssueDate = assetTransfer.TransferDate;
                        userCardItems.UnitPrice = asset.BookValue;
                        userCardItems.Quanity = assetTransferItem.Quantity;
                        userCardItems.SerialNo = assetTransferItem.SerialNo;
                        userCardItems.SIVNo = asset.SIV;
                        userCardItems.GRN = asset.GRN;
                        userCardItems.GRNDate = asset.GRNDate;
                        userCardItems.Remark = assetTransferItem.Remark;
                        userCardItems.OrgBranchName = "";
                        userCardItems.Active = true;
                        userCardItems.DateCreated = DateTime.Now;
                        userCardItems.CreatedBy = user;
                    }

                    //save on db
                    db.UserCardItems.Add(userCardItems);
                    db.SaveChanges();

                    // Inactive transferer userCard
                    #region make the previouse userCard inactive

                    var transfererUserCard = db.UserCardItems.FirstOrDefault(x => x.AssetRegistrationId == assetRegistrationId && x.EmployeeId == custodianBeforeId && x.Active == true);
                    if (transfererUserCard != null)
                    {
                        transfererUserCard.Active = false;
                        db.Entry(transfererUserCard).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion

                    #region Update assetInfo

                    asset.EmployeeId = transferToEmpId;
                    asset.OrgStructureId = orgId;
                    asset.LastModified = DateTime.Now;
                    asset.ModifiedBy = user;
                    db.Entry(asset).State = EntityState.Modified;
                    db.SaveChanges();

                    #endregion
                    //update location tracking 
                    UpdateLocationTracking(assetTransferItem, transferToEmpId, custodianBeforeId.ToString(), user, orgId);

                    //update previouse userCard
                    //UpdatePreviousUserCard(asset, custodianBeforeTransfer);
                    #endregion
                }

                if (assetTransfer.Type == TransferType.StoreToStore)
                {

                }
            }

           
        }

  /// <summary>
  /// Updates LocationTracking for AssetTransfer
  /// </summary>
  /// <param name="assetTransferItem"></param>
  /// <param name="user"></param>
        public static void UpdateLocationTrackingForStore(AssetTransferItem assetTransferItem, string user)
        {
            ExceedDbContext db = new ExceedDbContext();

            var assetTransferId = assetTransferItem.AssetTransferId;

            //Find AssetTransfer
            var assetTransfer = db.AssetTransfers.Find(assetTransferId);

            if (assetTransfer != null)
            {

                var fromStoreCode = assetTransfer.FromStoreCode;

                var toStoreCode = assetTransfer.ToStoreCode;

                var toBranchId = assetTransfer?.ToBranchId ?? 0;

                var asset = db.AssetRegistrations.Find(assetTransferItem.AssetRegistrationId);

                if (asset != null)
                {
                    #region Update Current LocationTracking record

                    var location = new LocationTracking
                    {
                        AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                        LocationType = LocationType.Store,
                        StoreCode = toStoreCode,
                        OrgStructureId = "",
                        StartDate = DateTime.Now,
                        BranchId =  toBranchId,
                        TransactionType = FixedAssetTransactionType.AssetTransfer,
                        TransactionNo = assetTransferItem.AssetTransferId.ToString(),
                        OrgBranchName = asset.OrgBranchName,
                        IsCurrent = true,
                        DateCreated = DateTime.Today,
                        CreatedBy = user
                    };
                    db.LocationTrackings.Add(location);
                    db.SaveChanges();

                    #endregion
                }

                #region Update Previouse LocationTracking record

                var locationTrackingRecordForStore = db.LocationTrackings.Where(x =>
                    x.AssetRegistrationId == asset.AssetRegistrationId && x.StoreCode == fromStoreCode &&
                    x.IsCurrent == true).ToList();

                foreach (var locationTracking in locationTrackingRecordForStore)
                {
                    locationTracking.IsCurrent = false;
                    locationTracking.EndDateDate = DateTime.Now;
                    locationTracking.TransactionType = FixedAssetTransactionType.AssetTransfer;
                    db.Entry(locationTracking).State = EntityState.Modified;
                    db.SaveChanges();
                }
                #endregion

                #region Update Asset info

                if (asset != null)
                {
                    asset.StoreCode = toStoreCode;
                    asset.BranchId = toBranchId;
                    asset.LastModified = DateTime.Now;
                    asset.ModifiedBy = user;
                    db.Entry(asset).State = EntityState.Modified;
                    db.SaveChanges();
                }

                #endregion
            }


        }

  /// <summary>
  /// updates locationtracking for AssetTransfer
  /// </summary>
  /// <param name="assetTransferItem"></param>
  /// <param name="transferToId"></param>
  /// <param name="custodianBeforeId"></param>
  /// <param name="user"></param>
  /// <param name="orgId"></param>
        public static void UpdateLocationTracking(AssetTransferItem assetTransferItem, string transferToId, string custodianBeforeId, string user, string orgId)
        {
            ExceedDbContext db = new ExceedDbContext();

            var asset = db.AssetRegistrations.Find(assetTransferItem.AssetRegistrationId);

            var assetTransferId = assetTransferItem.AssetTransferId;

            var assetTransfer = db.AssetTransfers.Find(assetTransferId);

            if (assetTransfer != null)
            {
                var toBranchId = assetTransfer?.ToBranchId ?? 0;

                if (asset != null)
                {
                    var location = new LocationTracking
                    {
                        AssetRegistrationId = assetTransferItem.AssetRegistrationId,
                        LocationType = LocationType.Employee,
                        EmployeeId = transferToId,
                        OrgStructureId = orgId,
                        StartDate = DateTime.Now,
                        BranchId = toBranchId, 
                        TransactionType = FixedAssetTransactionType.AssetTransfer,
                        TransactionNo = assetTransferItem.AssetTransferId.ToString(),
                        OrgBranchName = asset.OrgBranchName,
                        IsCurrent = true,
                        DateCreated = DateTime.Today,
                        CreatedBy = user
                    };
                    db.LocationTrackings.Add(location);
                    db.SaveChanges();
                }

                #region Update Previouse LocationTracking

                var custodianBeforeLocationTracking = db.LocationTrackings.Where(x =>
                    x.AssetRegistrationId == asset.AssetRegistrationId && x.EmployeeId == custodianBeforeId &&
                    x.IsCurrent == true).ToList();
                foreach (var locationTracking in custodianBeforeLocationTracking)
                {
                    locationTracking.IsCurrent = false;
                    locationTracking.EndDateDate = DateTime.Now;
                    locationTracking.TransactionType = FixedAssetTransactionType.AssetTransfer;
                    db.Entry(locationTracking).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            #endregion
        }

        #endregion


        /// <summary>
        /// makes previous UseCard to inactive
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="custodianBefore"></param>
        public static void UpdatePreviousUserCard(AssetRegistration asset, Person_Employee custodianBefore)
        {
            ExceedDbContext db = new ExceedDbContext();
            var previouseUserCardItemsList = db.UserCardItems.Where(x =>
                    x.AssetRegistrationId == asset.AssetRegistrationId && x.EmployeeId == custodianBefore.EmployeeId)
                .ToList();
            foreach (var userCardItem in previouseUserCardItemsList)
            {
                userCardItem.Active = false;

                //save to db
                db.Entry(userCardItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// makes previous UserCard to inactive
        /// </summary>
        /// <param name="fixedAssetReturnItem"></param>
        public static void UpdatePreviousUserCard(FixedAssetReturnItem fixedAssetReturnItem)
        {
            ExceedDbContext db = new ExceedDbContext();

            int assetRegistrationId = fixedAssetReturnItem.AssetRegistrationId;

            var userCardItems = db.UserCardItems.Where(x => x.AssetRegistrationId == assetRegistrationId);

            foreach (var userCardItem in userCardItems)
            {
                userCardItem.Active = false;
                //save in db
                db.Entry(userCardItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        
        public static void UpdateAssetInfo(int assetId, string employeeId, string orgId)
        {

            ExceedDbContext db = new ExceedDbContext();

            var asset = db.AssetRegistrations.Single(x => x.AssetRegistrationId == assetId);

            if (asset == null) return;
            asset.EmployeeId = employeeId;
            asset.OrgStructureId = orgId;

            //save db
            db.Entry(asset).State = EntityState.Modified;
            db.SaveChanges();

        }

        
        
        

       
        public static int GetDepreciationMonths(DateTime? grnDate, DateTime quarterEndDate)
        {
            ExceedDbContext Db = new ExceedDbContext();
            int months = 0;
            var periods = Db.GLPeriods.Where(x => x.DateFrom >= grnDate)
                .Where(x => x.DateTo <= quarterEndDate).ToList();
            months = periods.Count();
            if (grnDate?.Day < 15)//if bought before the 15th of the month,it is counted
                months = months + 1;
            return months;
        }

        private static int GetEmployeeDepartmentIdByEmpId(int empId)
        {
            ExceedDbContext db = new ExceedDbContext();

            int department = 0;

            var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (placment != null)
            {
                department = placment.OrgStructureId;
            }

            return department;

        }
        
        //for assetDisposal
        public static void UpdateAssetRegistrationAfterDisposal(AssetDisposal assetDisposal, string user)
        {
            ExceedDbContext db = new ExceedDbContext();

            List<AssetRegistration> assetList = new List<AssetRegistration>();
            List<LocationTracking> locationTrackingList = new List<LocationTracking>();

            //LocationTracking locationTracking = new LocationTracking();
            if (assetDisposal == null) return;
            var disposalItems = db.DisposalItems.Where(x => x.AssetDisposalId == assetDisposal.AssetDisposalId)
                .ToList();

            foreach (var item in disposalItems)
            {
                locationTrackingList = db.LocationTrackings.Where(x => x.AssetRegistrationId == item.AssetRegistrationId).ToList();
                var asset = db.AssetRegistrations.Find(item.AssetRegistrationId);

                //locationTrackingList.Add(locationTracking);
                assetList.Add(asset);
            }

            // Update Last Location Tracking history to inactive
            foreach(var locationTracking in locationTrackingList)
            {

                locationTracking.IsCurrent = false;
                
                db.Entry(locationTracking).State = EntityState.Modified;
                db.SaveChanges();
              
            }
            var locationTrackingCurrent = new LocationTracking();

            foreach(var item in disposalItems)
            {
                locationTrackingCurrent.AssetRegistrationId = item.AssetRegistrationId;              
                locationTrackingCurrent.StartDate = DateTime.Now;               
                locationTrackingCurrent.TransactionType = FixedAssetTransactionType.Disposed;                
                locationTrackingCurrent.IsCurrent = true;
                locationTrackingCurrent.DisposalType = assetDisposal.DisposalType;
                locationTrackingCurrent.DateCreated = DateTime.Now;
                locationTrackingCurrent.CreatedBy = user;
                locationTrackingCurrent.BranchId = 0;


                db.LocationTrackings.Add(locationTrackingCurrent);
                db.SaveChanges();
            }
            

            foreach (var asset in assetList)
            {
                asset.Status = RegistrationStatus.Disposed;
                asset.LastModified = DateTime.Now;
                asset.ModifiedBy = user;
                db.Entry(asset).State = EntityState.Modified;
                db.SaveChanges();
            }
        }


    }
}
