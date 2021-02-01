using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.DataAccess.Context;
using  ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{
    [DataObject]
    public class LogisticPlanDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LogisticPlanViewModel> GetLogisticPlan(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {

            List<LogisticPlanViewModel> FleetList = new List<LogisticPlanViewModel>();
            LogisticPlanViewModel LogisticModel;


            #region for ShardedDataSource
            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (Address == null)
                        {
                            Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            #endregion


            var LogisticPlans = db.LogisticPlans.ToList();


            if (!string.IsNullOrEmpty(Type))
            {
                LogisticPlans = LogisticPlans.Where(x => x.EquipmentTypeId.ToString() == Type).ToList();
            }
          

            foreach (var Logistic in LogisticPlans)
            {
                var LogisticDetailPlans = db.LogisticPlanDetails.Where(x => x.LogisticPlanId == Logistic.LogisticPlanId).ToList();
                foreach (var LogisticDetailPlan in LogisticDetailPlans)
                {
                    LogisticModel = new LogisticPlanViewModel();
                    try
                    {
                        decimal stockBalance = 0;
                        var issues = db.IssueItems.Where(x => x.Issue.JobOrderNo != null || x.Issue.JobOrderNo != string.Empty).Where(x => x.InventoryItem.ItemCode == LogisticDetailPlan.InventoryItem.ItemCode).ToList();
                       
                        LogisticModel.ItemName = LogisticDetailPlan.InventoryItem.Name;
                        LogisticModel.PartNo = LogisticDetailPlan.InventoryItem.ItemCode;
                        LogisticModel.Service = LogisticDetailPlan.Service.ServiceName;
                        LogisticModel.UOM = LogisticDetailPlan.InventoryItem.UoMName;
                        LogisticModel.Quantity = LogisticDetailPlan.Quantity;
                        LogisticModel.ActualCost = issues.Sum(x=>x.Total);
                        LogisticModel.PlanCost = LogisticDetailPlan.Quantity * LogisticDetailPlan.InventoryItem.TotalPrice;
                        LogisticModel.ActualQuantity = issues.Sum(x => x.IssuedQuantity);
                        foreach (var item in issues.DistinctByField(x => x.ItemCode))
                        {
                            var inventory_item =db.InventoryItems.Where(x => x.ItemCode == item.ItemCode).FirstOrDefault();
                            if(inventory_item!= null)
                                stockBalance += inventory_item.StoreItemAssignments.Sum(y => y.Balance);
                        }
                        LogisticModel.WareHouseQuantity = stockBalance;
                        LogisticModel.PurchaseOrderQuantity = 0;
                        LogisticModel.AmharicName = AmharicName;
                        LogisticModel.Name = Name;
                        LogisticModel.LogoFileName = LogoFileName;
                        LogisticModel.Address = Address;
                     
                    }
                    catch(Exception e)
                    {

                    }
                    FleetList.Add(LogisticModel); 
                  

                }


            }
            return FleetList;
        }
    }
}
