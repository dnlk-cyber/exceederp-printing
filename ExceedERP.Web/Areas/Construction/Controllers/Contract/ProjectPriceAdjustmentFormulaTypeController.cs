using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class ProjectPriceAdjustmentFormulaTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ProjectPriceAdjustmentFormulaType
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FormulaTypes_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<ProjectPriceAdjustmentFormulaType> formulas = db.ProjectPriceAdjustmentFormulaTypes.Where(f => f.ProjectInformationId == projectId);
            DataSourceResult result = formulas.ToDataSourceResult(request, formula => new {
                ProjectPriceAdjustmentFormulaTypeId = formula.ProjectPriceAdjustmentFormulaTypeId,
                ProjectInformationId = formula.ProjectInformationId,
                FormulaType = formula.FormulaType,
                Remark = formula.Remark,
               

                OrgBranchName = formula.OrgBranchName,
                DateCreated = formula.DateCreated,
                LastModified = formula.LastModified,
                CreatedBy = formula.CreatedBy,
                ModifiedBy = formula.ModifiedBy ?? " ",

                
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private bool FormulaTypesettingExists(int projectId) {
            var formulaTypeExists = false;
            var temp = db.ProjectPriceAdjustmentFormulaTypes.FirstOrDefault(f => f.ProjectInformationId == projectId);
            if(temp != null)
            {
                formulaTypeExists = true;
            }
            return formulaTypeExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormulaType_Create([DataSourceRequest]DataSourceRequest request, ProjectPriceAdjustmentFormulaType formula, int projectId)
        {
            if (ModelState.IsValid)
            {
                if (FormulaTypesettingExists(projectId))
                {
                    ModelState.AddModelError("", "Formula type is set already!");
                }
                else
                {
                    var entity = new ProjectPriceAdjustmentFormulaType
                    {
                        //ProjectPriceAdjustmentFormulaTypeId = formula.ProjectPriceAdjustmentFormulaTypeId,
                        ProjectInformationId = projectId,
                        FormulaType = formula.FormulaType,
                        Remark = formula.Remark,

                        OrgBranchName = formula.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = formula.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = formula.ModifiedBy,

                    };

                    db.ProjectPriceAdjustmentFormulaTypes.Add(entity);
                    db.SaveChanges();
                    formula.ProjectPriceAdjustmentFormulaTypeId = entity.ProjectPriceAdjustmentFormulaTypeId;
                }
                
            }

            return Json(new[] { formula }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormulaType_Update([DataSourceRequest]DataSourceRequest request, ProjectPriceAdjustmentFormulaType formula)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPriceAdjustmentFormulaType
                {
                    ProjectPriceAdjustmentFormulaTypeId = formula.ProjectPriceAdjustmentFormulaTypeId,
                    ProjectInformationId = formula.ProjectInformationId,
                    FormulaType = formula.FormulaType,
                    Remark = formula.Remark,


                    OrgBranchName = formula.OrgBranchName,
                    DateCreated = formula.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = formula.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    
                };

                db.ProjectPriceAdjustmentFormulaTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { formula }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FormulaType_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPriceAdjustmentFormulaType formula)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPriceAdjustmentFormulaType
                {
                    ProjectPriceAdjustmentFormulaTypeId = formula.ProjectPriceAdjustmentFormulaTypeId,
                    ProjectInformationId = formula.ProjectInformationId,
                    FormulaType = formula.FormulaType,
                    Remark = formula.Remark,

                    OrgBranchName = formula.OrgBranchName,
                    DateCreated = formula.DateCreated,
                    LastModified = formula.LastModified,
                    CreatedBy = formula.CreatedBy,
                    ModifiedBy = formula.ModifiedBy,

                    
                };

                db.ProjectPriceAdjustmentFormulaTypes.Attach(entity);
                db.ProjectPriceAdjustmentFormulaTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { formula }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}