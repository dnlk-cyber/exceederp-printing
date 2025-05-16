using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingProductionFollowUpTaskController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id) 
        {
            var entity = db.PrintingProductionFollowUpTasks.Where(x => x.PrintingProductionFollowUpId == id);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingProductionFollowUpTask model, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFollowUpTask
                {
                    EstimatedHours = model.EstimatedHours,
                    LabourActivity = model.LabourActivity,
                    PlannedOutput = model.PlannedOutput,
                    PressCategory = model.PressCategory,
                    MachineTypeId = model.MachineTypeId,
                    
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    Remark = model.Remark,
                    PrintingProductionFollowUpId = id,
                    OperatorId = model.OperatorId,
                    
                    UoM = model.UoM
                };
                db.PrintingProductionFollowUpTasks.Add(entity);
                db.SaveChanges();
                model.PrintingProductionFollowUpTaskId = entity.PrintingProductionFollowUpTaskId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingProductionFollowUpTask model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionFollowUpTask
                {
                    EstimatedHours = model.EstimatedHours,
                    LabourActivity = model.LabourActivity,
                    PlannedOutput = model.PlannedOutput,
                    PressCategory = model.PressCategory,
                    MachineTypeId = model.MachineTypeId,
                    PrintingProductionFollowUpId = model.PrintingProductionFollowUpId,
                    CreatedBy = model.CreatedBy,
                    DateCreated = model.DateCreated,
                    OperatorId = model.OperatorId,
                    
                    Remark = model.Remark,
                    PrintingProductionFollowUpTaskId = model.PrintingProductionFollowUpTaskId,
                    
                    UoM = model.UoM,
                  
                    LastModified = DateTime.Today,
                    ModifiedBy = User.Identity.Name
                };
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingProductionFollowUpTask model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingProductionFollowUpTasks.Find(model.PrintingProductionFollowUpTaskId);
                db.PrintingProductionFollowUpTasks.Remove(entity);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}