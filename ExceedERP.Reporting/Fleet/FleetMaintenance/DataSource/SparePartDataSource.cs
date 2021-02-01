using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.FleetMaintenance.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource
{

    [DataObject]
    public class SparePartDataSource
    {

        ExceedDbContext db = new ExceedDbContext();
        IJobOrderService _JobOrderService = new JobOrderService();
        IServiceService _ServiceService = new ServiceService();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WorkUnitSection> GetWorkUnitSection()
        {


            db = new ExceedDbContext();
            var result = db.WorkUnitSection.ToList();

            return result;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ServiceSpareViewModel> GetSparePart(DateTime StartingDate, DateTime EndingDate, string WorkUnitSection, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial, string Location)
        {
            List<ServiceSpareViewModel> ServiceList = new List<ServiceSpareViewModel>();
            var Branchs = db.Branch.ToList();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();


 
            Guid RequestId = Guid.Empty;
            List<JobOrder> jobs = db.JobOrder.Where(x => x.DateIssued > StartingDate && x.DateIssued < EndingDate ).ToList();
            string BranchLocation = string.Empty;


            foreach (var job in jobs)
            {

                var Request = db.JobRequest.Where(x => x.Num == job.Referance).FirstOrDefault();
                if (Request != null)
                {
                    RequestId = Request.JobRequestId;
                }
                if (FleetGenericHelper.FilterSingleFleet(job.FleetsID, Category, Type, PlateNo, ChasisNo, Serial, string.Empty, Location, Model))
                {
                    var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == job.Num.ToString() || x.JobOrderNo == job.JobOrderID.ToString()  );
                    foreach (var StoreRequestion in StoreRequestions)
                    {
                        var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();
                        var SpareMap = db.InventorySparePartMapping.ToList();
                        foreach (var Map in SpareMap)
                        {
                            foreach (var Requistion in Requistions.Where(x=>x.InventoryItem.ItemCategory.ItemCategoryCode == Map.InventoryCategoryId))
                            {
                                var Branch = Branchs.Where(x => x.BranchId == Requistion.Issue.BranchId).FirstOrDefault();
                                if (Branch != null)
                                {
                                    BranchLocation = Branch.Name;
                                } 
                                if (Map.InventoryCategoryId == Requistion.InventoryItem.ItemCategory.ItemCategoryCode)
                                {


                                    ServiceList.Add(new ServiceSpareViewModel { ServiceSpareID = Guid.NewGuid(), Location = BranchLocation, Type = Map.SpareType.ToString(), InventoryItemId = Requistion.InventoryItem.ItemCode, UsedQuantity = Requistion.Total, Quantity = Requistion.IssuedQuantity, Date = Requistion.DateCreated, UnitPrice = Requistion.UnitPrice });


                                }
                            }
                        }
                    }
                }
            }
     



            



               return ServiceList;
            
        }





    }
}

