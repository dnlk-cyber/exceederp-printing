using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using System.Linq;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance;
using System;

namespace ExceedERP.Web.Controllers
{

    public class GlobalUserAccessLogsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public class LogView
        {
            public string  Path { get; set; }
            public string FileName { get; set; }
        }
        public ActionResult Error()
        {
            List<LogView> loglist = new List<LogView>();

            string subpath = "~/App_Data/log";

            string[] fileEntries = Directory.GetFiles(Server.MapPath(subpath), "*.*",SearchOption.AllDirectories);
            
            for (int i= fileEntries.Length-1; i >= 0; i--)
            {
                string path = fileEntries[i];
                var logFile = new LogView
                {
                    FileName = Path.GetFileNameWithoutExtension(path),
                    Path = path
                };
                loglist.Add(logFile);
            }
            
            return View(loglist);
        }

        public JsonResult GetErrorLogDetail(string file)
        {
            string subpath = "~/App_Data/log/";
            var dataFile = Server.MapPath(subpath+file+".txt");
            var userData = System.IO.File.ReadAllText(dataFile);
            

            var formattedData = new
            {
                LodData = userData
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Export()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]

        public ActionResult Delete()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult OperationLog()
        {
            return View();
        }



        [AuthorizeRoles("Payroll_Report", "Payroll_View")]
        public ActionResult PayrollOperationLog()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
        public ActionResult FinanceOperationLog()
        {
            return View();
        }

        public ActionResult GlobalUserAccessLogs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalUserAccessLog> accessLogs = _db.GlobalUserAccessLog.OrderByDescending(x=>x.TimeAccessed);
            DataSourceResult result = accessLogs.ToDataSourceResult(request, accessLog => new
            {
                GlobalUserAccessLogID = accessLog.GlobalUserAccessLogID,
                IPAddress = accessLog.IPAddress,
                UserName = accessLog.UserName,
                Description = accessLog.Description,
                TimeAccessed = accessLog.TimeAccessed,
                Type = accessLog.Type,
                Status = accessLog.Status
            });

            return Json(result);
        }

        public ActionResult GlobalUserExportLogs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalExportLog> exportLogs = _db.GlobalExportLog.OrderByDescending(x => x.TimeAccessed);
            DataSourceResult result = exportLogs.ToDataSourceResult(request, accessLog => new
            {
                GlobalExportLogId = accessLog.GlobalExportLogId,
                IpAddress = accessLog.IpAddress,
                UserName = accessLog.UserName,
                Description = accessLog.Description,
                TimeAccessed = accessLog.TimeAccessed,
                Type = accessLog.Type,
                Status = accessLog.Status
            });

            return Json(result);
        }

        public ActionResult GlobalUserDeleteLogs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GlobalDeleteLog> deleteLogs = _db.GlobalDeleteLogs.OrderByDescending(x => x.TimeAccessed);
            DataSourceResult result = deleteLogs.ToDataSourceResult(request, accessLog => new
            {
                GlobalDeleteLogId = accessLog.GlobalDeleteLogId,
                IpAddress = accessLog.IpAddress,
                UserName = accessLog.UserName,
                Description = accessLog.Description,
                TimeAccessed = accessLog.TimeAccessed,
                Type = accessLog.Type,
                Status = accessLog.Status
            });

            return Json(result);
        }
        public ActionResult GlobalUserOperationLogs_Read([DataSourceRequest]DataSourceRequest request, DateTime? fromDate,DateTime? toDate,string module = null)
        {
            IQueryable<GlobalOperationLog> logs =  _db.GlobalOperationLogs.AsNoTracking();

            if (!string.IsNullOrEmpty(module))
            {
                logs = logs.Where(x => x.Module.ToString() == module);
            }

            // Apply date range filtering
            if (fromDate.HasValue)
            {
                logs = logs.Where(x => x.TimeAccessed >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                DateTime newToDate = toDate.Value.AddDays(1);
                logs = logs.Where(x => x.TimeAccessed <= newToDate);
            }

            // Optional default: if no filters provided, show last 15 days
            if (!fromDate.HasValue && !toDate.HasValue && !request.Filters?.Any() == true)
            {
                var defaultFrom = DateTime.UtcNow.AddDays(-15);
                logs = logs.Where(x => x.TimeAccessed >= defaultFrom);
            }


            DataSourceResult result = logs.ToDataSourceResult(request, accessLog => new
            {
                GlobalOperationLogId = accessLog.GlobalOperationLogId,
                IpAddress = accessLog.IpAddress,
                UserName = accessLog.UserName,
                Description = accessLog.Description,
                TimeAccessed = accessLog.TimeAccessed,
                Type = accessLog.Type,
                Status = accessLog.Status,
                Module = accessLog.Module
            });

            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
