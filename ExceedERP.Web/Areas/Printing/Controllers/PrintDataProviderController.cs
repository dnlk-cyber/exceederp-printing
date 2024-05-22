using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        

        public JsonResult PrintMachineType()
        {
            var printMachine = db.PrintingMachineTypes;
            var result = printMachine.Select(s => new
            {
                Text = s.MachineTypeName,
                Value = s.PrintingMachineTypeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult PrintPaperSize()
        {
            var paperSize = db.PrintingPaperSizes;
            var result = paperSize.Select(s => new
            {
                Text = s.PaperSizeName,
                Value = s.PrintingPaperSizeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JobType()
        {
            var jobType = db.PrintingJobTypes;
            var result = jobType.Select(s => new
            {
                Text = s.JobTypeName,
                Value = s.PrintingJobTypeId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PrintingProcess(PrintingProcessCategory processCategory)
        {
            var printingProcess = db.PrintingProcesses.Where(x => x.PrintingCategory == processCategory);
            var result = printingProcess.Select(s => new
            {
                Text = s.ProcessName,
                Value = s.PrintingProcessId
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}