using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectTakeOffLineForKgController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index()
        //{
        //    var unitWegight = db.UnitRate.ToList();
        //    ViewData["Diameters"] = unitWegight;
        //    return View();
        //}

        public ActionResult ProjectTakeOffLineForKg_Read([DataSourceRequest]DataSourceRequest request, int idKg)
        {
            IQueryable<ProjectTakeOffLineForKg> projecttakeofflineforkg = db.ProjectTakeOffLineForKg.Where( x=> x.ProjectTakeOffId == idKg );
            DataSourceResult result = projecttakeofflineforkg.ToDataSourceResult(request, projectTakeOffLineForKg => new {
                ProjectTakeOffLineForKgId = projectTakeOffLineForKg.ProjectTakeOffLineForKgId,
                Description = projectTakeOffLineForKg.Description,
                Type = projectTakeOffLineForKg.Type,
                //UnitRateId = projectTakeOffLineForKg.UnitRateId ,

                Location = projectTakeOffLineForKg.Location,
                DeformedBarId = projectTakeOffLineForKg.DeformedBarId,
                Diameter = projectTakeOffLineForKg.Diameter,
                TotalWeight = projectTakeOffLineForKg.TotalWeight,
                DateOfInformation = projectTakeOffLineForKg.DateOfInformation,

                Shape = projectTakeOffLineForKg.Shape,
                Length = projectTakeOffLineForKg.Length,
                NoOfBars = projectTakeOffLineForKg.NoOfBars,
                NoOfElement = projectTakeOffLineForKg.NoOfElement,
                TotalBar = projectTakeOffLineForKg.TotalBar,
                TotalLength = projectTakeOffLineForKg.TotalLength,
                GrandTotal = projectTakeOffLineForKg.GrandTotal,
                ProjectTakeOffId = projectTakeOffLineForKg.ProjectTakeOffId,
                //UnitRate = projectTakeOffLineForKg.UnitRate,
                OrgBranchName = projectTakeOffLineForKg.OrgBranchName,
                DateCreated = projectTakeOffLineForKg.DateCreated,
                LastModified = projectTakeOffLineForKg.LastModified,
                CreatedBy = projectTakeOffLineForKg.CreatedBy,
                ModifiedBy = projectTakeOffLineForKg.ModifiedBy 
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLineForKg_Create([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLineForKg projectTakeOffLineForKg, int idTwoKg)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLineForKg
                {
                    Description = projectTakeOffLineForKg.Description,
                    Type = projectTakeOffLineForKg.Type,
                    //UnitRateId = projectTakeOffLineForKg.UnitRateId ,

                    Location = projectTakeOffLineForKg.Location,
                    DeformedBarId = projectTakeOffLineForKg.DeformedBarId,
                    Diameter = GetBarDiameter(projectTakeOffLineForKg),
                    TotalWeight = CalculateBarWeight(GetBarUnitWeight(projectTakeOffLineForKg), CalculateTotalBarLength(projectTakeOffLineForKg)),


                    Shape = projectTakeOffLineForKg.Shape,
                    Length = projectTakeOffLineForKg.Length,
                    NoOfBars = projectTakeOffLineForKg.NoOfBars,
                    NoOfElement = projectTakeOffLineForKg.NoOfElement,
                    TotalBar = projectTakeOffLineForKg.TotalBar,
                    TotalLength = CalculateTotalBarLength(projectTakeOffLineForKg),
                    GrandTotal = projectTakeOffLineForKg.GrandTotal,
                    ProjectTakeOffId = idTwoKg ,
                    DateOfInformation = projectTakeOffLineForKg.DateOfInformation,

                    OrgBranchName = projectTakeOffLineForKg.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectTakeOffLineForKg.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectTakeOffLineForKg.ModifiedBy 
                };
               
                db.ProjectTakeOffLineForKg.Add(entity);
                db.SaveChanges();
                projectTakeOffLineForKg.ProjectTakeOffLineForKgId = entity.ProjectTakeOffLineForKgId;

                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);
            }

            return Json(new[] { projectTakeOffLineForKg }.ToDataSourceResult(request, ModelState));
        }

      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLineForKg_Update([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLineForKg projectTakeOffLineForKg)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLineForKg
                {
                    ProjectTakeOffLineForKgId = projectTakeOffLineForKg.ProjectTakeOffLineForKgId,
                    Description = projectTakeOffLineForKg.Description,
                    //UnitRateId = projectTakeOffLineForKg.UnitRateId ,

                    Location = projectTakeOffLineForKg.Location,
                    DeformedBarId = projectTakeOffLineForKg.DeformedBarId,
                    Diameter = GetBarDiameter(projectTakeOffLineForKg),
                    TotalWeight = CalculateBarWeight(GetBarUnitWeight(projectTakeOffLineForKg), CalculateTotalBarLength(projectTakeOffLineForKg)),
                    DateOfInformation = projectTakeOffLineForKg.DateOfInformation,

                    Type = projectTakeOffLineForKg.Type,
                    Shape = projectTakeOffLineForKg.Shape,
                    Length = projectTakeOffLineForKg.Length,
                    NoOfBars = projectTakeOffLineForKg.NoOfBars,
                    NoOfElement = projectTakeOffLineForKg.NoOfElement,
                    TotalBar = projectTakeOffLineForKg.TotalBar,
                    TotalLength = CalculateTotalBarLength(projectTakeOffLineForKg),
                    GrandTotal = projectTakeOffLineForKg.GrandTotal,
                    ProjectTakeOffId = projectTakeOffLineForKg.ProjectTakeOffId ,
                    OrgBranchName = projectTakeOffLineForKg.OrgBranchName,
                    DateCreated = projectTakeOffLineForKg.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectTakeOffLineForKg.CreatedBy,
                    ModifiedBy = User.Identity .Name  
                     
                };
                
                db.ProjectTakeOffLineForKg.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);
            }

            return Json(new[] { projectTakeOffLineForKg }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLineForKg_Destroy([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLineForKg projectTakeOffLineForKg)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLineForKg
                {
                    ProjectTakeOffLineForKgId = projectTakeOffLineForKg.ProjectTakeOffLineForKgId,
                    Description = projectTakeOffLineForKg.Description,
                    //UnitRateId = projectTakeOffLineForKg.UnitRateId ,

                    Location = projectTakeOffLineForKg.Location,
                    DeformedBarId = projectTakeOffLineForKg.DeformedBarId,
                    Diameter = projectTakeOffLineForKg.Diameter,
                    TotalWeight = projectTakeOffLineForKg.TotalWeight,
                    DateOfInformation = projectTakeOffLineForKg.DateOfInformation,

                    Type = projectTakeOffLineForKg.Type,
                    Shape = projectTakeOffLineForKg.Shape,
                    Length = projectTakeOffLineForKg.Length,
                    NoOfBars = projectTakeOffLineForKg.NoOfBars,
                    NoOfElement = projectTakeOffLineForKg.NoOfElement,
                    TotalBar = projectTakeOffLineForKg.TotalBar,
                    TotalLength = projectTakeOffLineForKg.TotalLength,
                    GrandTotal = projectTakeOffLineForKg.GrandTotal,
                    ProjectTakeOffId = projectTakeOffLineForKg.ProjectTakeOffId ,
                    OrgBranchName = projectTakeOffLineForKg.OrgBranchName,
                    DateCreated = projectTakeOffLineForKg.DateCreated,
                    LastModified = projectTakeOffLineForKg.LastModified,
                    CreatedBy = projectTakeOffLineForKg.CreatedBy,
                    ModifiedBy = projectTakeOffLineForKg.ModifiedBy 
                   
                };

                db.ProjectTakeOffLineForKg.Attach(entity);
                db.ProjectTakeOffLineForKg.Remove(entity);
                db.SaveChanges();

                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);

            }

            return Json(new[] { projectTakeOffLineForKg }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        private decimal CalculateTotalBarLength(ProjectTakeOffLineForKg model)
        {
            decimal total = new decimal();
            if (model != null)
            {
                total = model.Length * model.NoOfBars * model.NoOfElement;
            }
            return total;
        }

        private decimal GetBarDiameter(ProjectTakeOffLineForKg model)
        {
            decimal diameter = new decimal();
            if (model != null)
            {
                var bar = db.DeformedBars.Find(model.DeformedBarId);
                diameter = bar.Diameter;
            }
            return diameter;
        }

        private decimal GetBarUnitWeight(ProjectTakeOffLineForKg model)
        {
            decimal unitWeight = new decimal();
            if (model != null)
            {
                var bar = db.DeformedBars.Find(model.DeformedBarId);
                unitWeight = bar.UnitWeight;
            }
            return unitWeight;
        }
        private decimal CalculateBarWeight(decimal unitWeight, decimal totalLength)
        {
            decimal weight = unitWeight * totalLength;
            weight = Math.Round(weight, 4);
            return weight;

        }
    }
}
