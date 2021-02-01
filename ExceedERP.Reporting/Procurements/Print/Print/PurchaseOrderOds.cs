using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Procurements.Print.VM;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
    [DataObject]
    public class PurchaseOrderOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public PoHeader GetPurchaseOrder(int id)
        {
            var purchaseOrder = _db.PurchaseOrders.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            PoHeader headerDs = new PoHeader();
            if (purchaseOrder != null)
            {
                headerDs.PoNo = purchaseOrder.PurchaseOrderID.ToString();
                if (purchaseOrder.PurchaseRequisitionID != null)
                    headerDs.PRNo = InventoryPrintHelper.GetId((int) purchaseOrder.PurchaseRequisitionID);
                headerDs.PoDate = purchaseOrder.OrderDate;
                headerDs.Supplier = purchaseOrder.SupplierName;
                headerDs.DeliverTo = purchaseOrder.DeliveryPlace;
                headerDs.DeliveryPeriod = purchaseOrder.DeliveryDate;
                headerDs.Proforma = purchaseOrder.ProformaPurchaseID.ToString();

               

                if (purchaseOrder.IsManual)
                {
                    #region prepared checker approver

                    headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(purchaseOrder.CreatedBy));
                    if (purchaseOrder.DateCreated != null)
                        headerDs.CreateDate =purchaseOrder.DateCreated!=null? purchaseOrder.DateCreated.ToString():"";

                    var purchaseOrderValidations = _db.PurchaseOrderValidations
                        .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID).ToList();

                    if (purchaseOrderValidations.Any())
                    {
                        int maxid = purchaseOrderValidations.Max(x => x.PurchaseOrderValidationID);
                        var approver =
                            purchaseOrderValidations.FirstOrDefault(x => x.PurchaseOrderValidationID == maxid);
                        if (approver != null)
                        {
                            headerDs.Approver =
                                EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                            headerDs.ApprovedDate =approver.DateCreated!=null? approver.DateCreated.ToString():"";

                            var checker = purchaseOrderValidations.OrderByDescending(x => x.PurchaseOrderValidationID)
                                .FirstOrDefault(x => x.PurchaseOrderValidationID < maxid);

                            if (checker != null)
                            {
                                headerDs.Checker =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(checker.CreatedBy));
                                headerDs.CheckedDate =checker.DateCreated!=null? checker.DateCreated.ToString():"";
                            }
                            else
                            {
                                headerDs.Checker =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(purchaseOrder.SendForApprovalBy));
                                headerDs.CheckedDate = purchaseOrder.SendForApprovalTime!=null? purchaseOrder.SendForApprovalTime.ToString():"";

                            }
                        }

                    }

                    #endregion
                }
                else 
                {
                    #region prepared checker approver

                    var proforma = _db.ProformaPurchases.Find(purchaseOrder.ProformaPurchaseID);

                    if (proforma != null)
                    {
                        headerDs.CreatedBy =
                            EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(proforma.CreatedBy));
                        if (purchaseOrder.DateCreated != null)
                            headerDs.CreateDate = proforma.DateCreated!=null? proforma.DateCreated.ToString():"";
                    }

                    var purchaseValidations = _db.PurchaseValidations
                        .Where(x => x.ProformaPurchaseID == purchaseOrder.ProformaPurchaseID).ToList();

                    if (purchaseValidations.Any())
                    {
                        int maxid = purchaseValidations.Max(x => x.PurchaseValidationID);
                        var approver =
                            purchaseValidations.FirstOrDefault(x => x.PurchaseValidationID == maxid);
                        if (approver != null)
                        {
                            headerDs.Approver =
                                EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                            headerDs.ApprovedDate = approver.DateCreated!=null? approver.DateCreated.ToString():"";

                            var checker = purchaseValidations.OrderByDescending(x => x.PurchaseValidationID)
                                .FirstOrDefault(x => x.PurchaseValidationID < maxid);

                            if (checker != null)
                            {
                                headerDs.Checker =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(checker.CreatedBy));
                                headerDs.CheckedDate = checker.DateCreated!=null ? checker.DateCreated.ToString():"";
                            }
                            else
                            {
                                headerDs.Checker =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(proforma?.SendForApprovalBy));
                                headerDs.CheckedDate = proforma?.SendForApprovalTime!=null? proforma.SendForApprovalTime.ToString():"";

                            }
                        }

                    }

                    #endregion
                }

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
        public List<PurchaseOrderItem> PoItems(int id)
        {
            List<PurchaseOrderItem> orderItems = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();
            foreach (var item in orderItems)
            {
                item.ItemName = item.ItemName + "-" + item.Name;
            }
            return orderItems;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PoPurchaseRequisition> PrList(int id)
        {
            List<PoPurchaseRequisition> prList = _db.PoPurchaseRequisitions.Where(x => x.PurchaseOrderID == id).ToList();
            return prList;
        }
    }
}