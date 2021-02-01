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

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    public class EarnedValueManagementTermFormulaConstantController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult EVMTermFormulaConstant_Read([DataSourceRequest]DataSourceRequest request, int dIdRead)
        {
            IQueryable<EarnedValueManagementTermFormulaConstant> constants = db.EarnedValueManagementTermFormulaConstants.Where(c => c.EarnedValueManagementTermDetailId == dIdRead);
            DataSourceResult result = constants.ToDataSourceResult(request, constant => new
            {
                EarnedValueManagementTermFormulaConstantId = constant.EarnedValueManagementTermFormulaConstantId,
                EarnedValueManagementTermDetailId = constant.EarnedValueManagementTermDetailId,
                Operator = constant.Operator,
                ConstantValue = constant.ConstantValue,
                FormulaValue = constant.FormulaValue,
                HiddenFormulaValue = constant.HiddenFormulaValue,

                OrgBranchName = constant.OrgBranchName,
                DateCreated = constant.DateCreated,
                LastModified = constant.LastModified,
                CreatedBy = constant.CreatedBy,
                ModifiedBy = constant.ModifiedBy
            });

            return Json(result);
        }
        private bool ConstantOperatorExists(int id)
        {
            var valueExists = false;
            var existingConst = db.EarnedValueManagementTermFormulaConstants.FirstOrDefault(c => c.EarnedValueManagementTermDetailId == id);
            if (existingConst != null)
            {
                valueExists = true;
            }
            return valueExists;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EVMTermFormulaConstant_Create([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermFormulaConstant constant, int dIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (ConstantOperatorExists(dIdCreate))
                {
                    ModelState.AddModelError("", "Multiple Constants are not supported!");
                }
                else
                {
                    var entity = new EarnedValueManagementTermFormulaConstant
                    {
                        //EarnedValueManagementTermFormulaConstantId = constant.EarnedValueManagementTermFormulaConstantId,
                        EarnedValueManagementTermDetailId = dIdCreate,
                        Operator = constant.Operator,
                        ConstantValue = constant.ConstantValue,
                        FormulaValue = constant.FormulaValue,
                        HiddenFormulaValue = constant.HiddenFormulaValue,

                        OrgBranchName = constant.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = constant.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = constant.ModifiedBy
                    };
                    entity.HiddenFormulaValue = entity.Operator.ToString() + "#" + entity.ConstantValue;
                    entity.FormulaValue = " " + this.GetOperator(entity.Operator) + " " + entity.ConstantValue;

                    db.EarnedValueManagementTermFormulaConstants.Add(entity);
                    db.SaveChanges();
                    constant.EarnedValueManagementTermFormulaConstantId = entity.EarnedValueManagementTermFormulaConstantId;

                    this.UpdateTermFormula(dIdCreate, entity);
                }

                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);
                //DelayAnalysisHelper.UpdateSuccessorsDates(entity.DelayAnalysisTaskId);
            }

            return Json(new[] { constant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EVMTermFormulaConstant_Update([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermFormulaConstant constant)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTermFormulaConstant
                {
                    EarnedValueManagementTermFormulaConstantId = constant.EarnedValueManagementTermFormulaConstantId,
                    EarnedValueManagementTermDetailId = constant.EarnedValueManagementTermDetailId,
                    Operator = constant.Operator,
                    ConstantValue = constant.ConstantValue,
                    FormulaValue = constant.FormulaValue,
                    HiddenFormulaValue = constant.HiddenFormulaValue,

                    OrgBranchName = constant.OrgBranchName,
                    DateCreated = constant.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = constant.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.HiddenFormulaValue = entity.Operator.ToString() + "#" + entity.ConstantValue;
                entity.FormulaValue = "... " + this.GetOperator(entity.Operator) + " " + entity.ConstantValue;

                db.EarnedValueManagementTermFormulaConstants.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);
                //DelayAnalysisHelper.UpdateSuccessorsDates(entity.DelayAnalysisTaskId);

            }

            return Json(new[] { constant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EVMTermFormulaConstant_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermFormulaConstant constant)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTermFormulaConstant
                {
                    EarnedValueManagementTermFormulaConstantId = constant.EarnedValueManagementTermFormulaConstantId,
                    EarnedValueManagementTermDetailId = constant.EarnedValueManagementTermDetailId,
                    Operator = constant.Operator,
                    ConstantValue = constant.ConstantValue,
                    FormulaValue = constant.FormulaValue,
                    HiddenFormulaValue  = constant.HiddenFormulaValue,

                    OrgBranchName = constant.OrgBranchName,
                    DateCreated = constant.DateCreated,
                    LastModified = constant.LastModified,
                    CreatedBy = constant.CreatedBy,
                    ModifiedBy = constant.ModifiedBy
                };

                db.EarnedValueManagementTermFormulaConstants.Attach(entity);
                db.EarnedValueManagementTermFormulaConstants.Remove(entity);
                db.SaveChanges();

                this.UpdateTermFormula(entity.EarnedValueManagementTermDetailId);
                //DelayAnalysisHelper.UpdateDelayAnalysisTask(entity.DelayAnalysisTaskId);

            }

            return Json(new[] { constant }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        private string GetOperator(EVMOperation operation)
        {
            string result = "+";
            var currentOperator = operation.ToString();
            if (currentOperator == "Multiplication")
            {
                result = "*";
            }
            else if (currentOperator == "Division")
            {
                result = "/";
            }
            else if (currentOperator == "Subtraction")
            {
                result = "-";
            }
            else if (currentOperator == "Addition")
            {
                result = "+";
            }
            return result;
        }
        private void UpdateTermFormula(int detailId, EarnedValueManagementTermFormulaConstant model) {
            var currentFormula = db.EarnedValueManagementTermDetail.FirstOrDefault(d => d.EarnedValueManagementTermDetailId == detailId);
            if(currentFormula != null)
            {
                currentFormula.Formula = "(" + currentFormula.Formula + ")" + model.FormulaValue;

                db.EarnedValueManagementTermDetail.Attach(currentFormula);
                db.Entry(currentFormula).State = EntityState.Modified;
                db.SaveChanges();

                var mainTerm = db.EarnedValueManagementTerms.FirstOrDefault(t => t.EarnedValueManagementTermId == currentFormula.EarnedValueManagementTermId);
                if(mainTerm != null)
                {
                    mainTerm.Formula = currentFormula.Formula;

                    db.EarnedValueManagementTerms.Attach(mainTerm);
                    db.Entry(mainTerm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        private void UpdateTermFormula(int formulaId)
        {
            var currentFormula = db.EarnedValueManagementTermDetail.FirstOrDefault(d => d.EarnedValueManagementTermDetailId == formulaId);
            if(currentFormula != null)
            {
                var operandOneComponent = db.EVMTermOperandComponents.FirstOrDefault(opc => opc.EarnedValueManagementTermDetailId == currentFormula.EarnedValueManagementTermDetailId && opc.ParentOperand == EVMParentOperand.OperandOne);
                var operandTwoComponent = db.EVMTermOperandComponents.FirstOrDefault(c => c.EarnedValueManagementTermDetailId == currentFormula.EarnedValueManagementTermDetailId && c.ParentOperand == EVMParentOperand.OperandTwo);

                if (operandOneComponent == null && operandTwoComponent == null)
                {
                    currentFormula.Formula = currentFormula.OperandOne + " " + this.GetOperator(currentFormula.Operator) + " " + currentFormula.OperandTwo;
                }
                else if (operandOneComponent != null && operandTwoComponent == null)
                {
                    currentFormula.Formula = "(" + currentFormula.OperandOne + ")" + " " + this.GetOperator(currentFormula.Operator) + " " + currentFormula.OperandTwo;
                }
                else if (operandOneComponent == null && operandTwoComponent != null)
                {
                    currentFormula.Formula = currentFormula.OperandOne + " " + this.GetOperator(currentFormula.Operator) + " (" + currentFormula.OperandTwo + ")";
                }
                else {
                    currentFormula.Formula = "(" + currentFormula.OperandOne + ")" + " " + this.GetOperator(currentFormula.Operator) + " (" + currentFormula.OperandTwo + ")";
                }

                db.EarnedValueManagementTermDetail.Attach(currentFormula);
                db.Entry(currentFormula).State = EntityState.Modified;
                db.SaveChanges();

                var targetTerm = db.EarnedValueManagementTerms.FirstOrDefault(t => t.EarnedValueManagementTermId == currentFormula.EarnedValueManagementTermId);
                if(targetTerm != null)
                {
                    targetTerm.Formula = currentFormula.Formula;

                    db.EarnedValueManagementTerms.Attach(targetTerm);
                    db.Entry(targetTerm).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
    }
}