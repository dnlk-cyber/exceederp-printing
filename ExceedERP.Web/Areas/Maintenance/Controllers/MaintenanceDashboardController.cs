
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Maintenance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenanceDashboardController : Controller
    {
        // GET: Maintenance/MaintenanceDashboard
        private ExceedDbContext db = new ExceedDbContext();
        // GET: ImportExport/MaintenanceDashboard
        public ActionResult Index()
        {
            return View();
        }
        public IList<string> getLabels()
        {
            var labelArray = new List<string>();
            labelArray.Add("Pending");
            labelArray.Add("Opened");
            labelArray.Add("WaitingApproval");
            labelArray.Add("Closed");
            labelArray.Add("Rejected");

            return labelArray;
        }
        public JsonResult GetCount(DateTime? startDate, DateTime? endDate)
        {
            var datas = getJobOrdersCountByStatus(startDate, endDate,true);
            var labels = getLabels();

            var count = new Dictionary<string, int>();
            int i = 1;
            var labelDatas = labels.Zip(datas, (n, w) => new { Label = n, Data = w });

            foreach (var labelData in labelDatas)
            {
                count.Add(labelData.Label, labelData.Data);
                i++;
            }
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getBarChartMaintenanceOrderDatas(DateTime? startDate, DateTime? endDate)

        {

            var orderLabelList = getLabels();
            var orderDataList = getJobOrdersCountByStatus(startDate, endDate,false);
            var orderStatuses = getMaintenanceCentreJobOrderStatuses(startDate, endDate);
            var OrderVsDates = new Mapper();
            var OrderVsDateServices = new Mapper();
            var orderserviceDataList = getOrdersMaintenanceJoServicesCount(startDate, endDate);
            OrderVsDates.labels = orderLabelList;
            OrderVsDates.datas = orderDataList;
            OrderVsDateServices.labels = orderLabelList;
            OrderVsDateServices.datas = orderserviceDataList;
            var result = new Dictionary<string, Object>();
            result.Add("OrderVsDate", OrderVsDates);
            result.Add("OrderSatuseVsServices", OrderVsDateServices);
            result.Add("OrderStatuses", orderStatuses);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public Dictionary<string, object> getMaintenanceCentreJobOrderStatuses(DateTime? startDate, DateTime? endDate)
        {
            var maintenanceCenters = db.MaintenanceCenter.Where(q=>db.JobOrder.Select(s => s.MaintenanceCenter).Distinct().Contains(q.Name)).Select(s=>s.Name);
            var statuses = new List<string>();
            statuses.Add(MaintenanceConst.Open);
            statuses.Add(MaintenanceConst.Closed);
            statuses.Add(MaintenanceConst.Rejected);

            var labDatas = new Dictionary<string, object>();
            var labels = new List<string>();
            var datas = new List<List<int>>();
            var jobOrders = getJobOrdersBnDate(startDate, endDate, false).ToList();
            foreach (var status in statuses)
            {
                var data = new List<int>();
                foreach (var mainCenter in maintenanceCenters)
            {

                
                    var sts = jobOrders.Where(q=>q.Status==status&&q.MaintenanceCenter==mainCenter)
                        .ToList().Count();
                    data.Add(sts);
                }
                datas.Add(data);

            }
            labDatas.Add("stackedBarLabels", maintenanceCenters);
            labDatas.Add("stackedBarDataSets", statuses);
            labDatas.Add("stackedBarDatas", datas);




            return labDatas;


        }


        public IList<int> getOrdersMaintenanceJoServicesCount(DateTime? startDate, DateTime? endDate)
        {
            var serviceQty = new List<int>();
            var jobOrders = getJobOrdersByDate(startDate, endDate, false);
            var data1 = jobOrders.ElementAt(0);
            var data2 = jobOrders.ElementAt(1);
            var data3 = jobOrders.ElementAt(1);
            var data4 = jobOrders.ElementAt(3);
            var data5 = jobOrders.ElementAt(4);
            serviceQty.Add(db.JobOrderDetail.Where(q => data1.Contains(q.JobOrderID)).ToList().Count());
            serviceQty.Add(db.JobOrderDetail.Where(q => data2.Contains(q.JobOrderID)).ToList().Count());
            serviceQty.Add(db.JobOrderDetail.Where(q => data3.Contains(q.JobOrderID)).ToList().Count());
            serviceQty.Add(db.JobOrderDetail.Where(q => data4.Contains(q.JobOrderID)).ToList().Count());
            serviceQty.Add(db.JobOrderDetail.Where(q => data5.Contains(q.JobOrderID)).ToList().Count());
            return serviceQty;
        }
        public JsonResult GetInExMainteenanceJobOrders(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
            {
                startDate = DateTime.Now.AddDays(-60);
            }

            if (endDate == null)
            {
                endDate = DateTime.Now;
            }

            var labels = new List<string>();
            var datas = new List<decimal>();
            var labDatas = new Mapper();
            labels.Add("External");
            labels.Add("Internal");
            var jobRequests = db.JobOrder.Where(q => q.DateCreated >= startDate && q.DateCreated <= endDate).Select(s => s.Referance);
            datas.Add(db.JobRequest.Where(q => jobRequests.Contains(q.Num)&&q.Source==JobRequestSource.ExternalVehicle).ToList().Count());
            datas.Add(db.JobRequest.Where(q => jobRequests.Contains(q.Num)&&q.Source==JobRequestSource.CompanyVehicle).ToList().Count());
            labDatas.labels = labels;
            labDatas.datas = datas;
            var result = new Dictionary<string, Object>();
            result.Add("MaintenanceVehicles", labDatas);
            result.Add("TotalProducts", labDatas);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public IList<int> getJobOrdersCountByStatus(DateTime? startDate, DateTime? endDate,bool status)
        {
            var counts = new List<int>();
            var datas = getJobOrdersByDate(startDate, endDate,status);
            foreach (var data in datas)
            {
                counts.Add(data.Count());
            }
            return counts;
        }
        public IList<IEnumerable<Guid>> getJobOrdersByDate(DateTime? startDate, DateTime? endDate,bool status,bool flag=false)
        {
            if (startDate == null)
            {
                startDate = DateTime.Now.AddDays(-60);
            }

            if (endDate == null)
            {
                endDate = DateTime.Now;
            }
            var orders = new List<IEnumerable<Guid>>();
            var datas = new List<string>();
            var jobOrders = getJobOrdersBnDate(startDate, endDate,status);
            var pending = jobOrders.ToList()
                .Where(q=>q.Status == MaintenanceConst.Pending)
                .Select(s => new { s.JobOrderID, s.DateCreated,s.Status,s.MaintenanceCenter });
            var opened = jobOrders.ToList()
                .Where(q=>q.Status == MaintenanceConst.Open)
                .Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
            var waitingApproval = jobOrders.ToList()
                .Where(q=>q.Status == MaintenanceConst.WaitingApproval)
                .Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
            var closed = jobOrders.ToList()
                .Where(q=>q.Status == MaintenanceConst.Closed)
                .Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
            var rejected = jobOrders.ToList()
                .Where(q=>q.Status == MaintenanceConst.Rejected)
                .Select(s => new { s.JobOrderID, s.DateCreated, s.Status, s.MaintenanceCenter });
          
            //if(flag)
            //{

            //}
            orders.Add(pending.Select(s=>s.JobOrderID));
            orders.Add(opened.Select(s => s.JobOrderID));
            orders.Add(waitingApproval.Select(s => s.JobOrderID));
            orders.Add(closed.Select(s => s.JobOrderID));
            orders.Add(rejected.Select(s => s.JobOrderID));

            return orders;
        }
        public List<JobOrder> getJobOrdersBnDate(DateTime? startDate, DateTime? endDate,bool status)
        {
            var jobOrders = db.JobOrder.ToList();
            if(!status)
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now.AddDays(-30);
                }

                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                jobOrders = db.JobOrder.Where(q => q.DateCreated >= startDate && q.DateCreated <= endDate).ToList();

            }
            return jobOrders;
        }
        public class Mapper
        {
            public object labels;
            public object datas;

        }
       

    }
}
