using ExceedERP.Core.Domain.Common;
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

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class EVMTermOperandComponentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/EVMTermOperandComponent
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OperandComponent_Read([DataSourceRequest]DataSourceRequest request, int detailIdRead, int parent)
        {
            var parentOperand = (EVMParentOperand)parent;
            IQueryable<EVMTermOperandComponent> operandComponents = db.EVMTermOperandComponents.Where(ev => ev.EarnedValueManagementTermDetailId == detailIdRead && ev.ParentOperand == parentOperand);
            DataSourceResult result = operandComponents.ToDataSourceResult(request, operandComponent => new {
                EVMTermOperandComponentId = operandComponent.EVMTermOperandComponentId,
                EarnedValueManagementTermDetailId = operandComponent.EarnedValueManagementTermDetailId,
                OperandOne = operandComponent.OperandOne,
                OperandTwo = operandComponent.OperandTwo,
                Operator = operandComponent.Operator,
                Formula = operandComponent.Formula,
                ParentOperand = operandComponent.ParentOperand,
                HiddenFormulaValue = operandComponent.HiddenFormulaValue
            });

            return Json(result);
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
        private bool CheckExistance(int modelId, int typeId)
        {
            EVMParentOperand currentOperand = (EVMParentOperand)typeId;
            var valueExists = false;
            var existingComponent = db.EVMTermOperandComponents.Where(evmCom => evmCom.EarnedValueManagementTermDetailId == modelId && evmCom.ParentOperand == currentOperand).FirstOrDefault();
            if (existingComponent != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OperandComponent_Create([DataSourceRequest]DataSourceRequest request, EVMTermOperandComponent operandComponent, int detailIdCreate, int parentType)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(detailIdCreate, parentType))
                {
                    ModelState.AddModelError("", "Multiple value cannot be used here !!");
                }
                else
                {

                    var entity = new EVMTermOperandComponent
                    {
                        //EarnedValuePerProjectDetailId = earnedValueDetail.EarnedValuePerProjectDetailId,
                        EarnedValueManagementTermDetailId = detailIdCreate,
                        OperandOne = operandComponent.OperandOne,
                        OperandTwo = operandComponent.OperandTwo,
                        Operator = operandComponent.Operator,
                        Formula = operandComponent.Formula,
                        ParentOperand = (EVMParentOperand)parentType,
                        HiddenFormulaValue = operandComponent.HiddenFormulaValue,
                        
                    };
                    entity.HiddenFormulaValue = entity.OperandOne + "$" + entity.Operator.ToString() + "$" + entity.OperandTwo ;
                    entity.Formula = entity.OperandOne + " " + this.GetOperator(entity.Operator) + " " + entity.OperandTwo;

                    db.EVMTermOperandComponents.Add(entity);
                    db.SaveChanges();
                    operandComponent.EVMTermOperandComponentId = entity.EVMTermOperandComponentId;

                    this.UpdateCascadedFormula(entity.EarnedValueManagementTermDetailId, entity);

                }
            }

            return Json(new[] { operandComponent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OperandComponent_Update([DataSourceRequest]DataSourceRequest request, EVMTermOperandComponent operandComponent)
        {
            if (ModelState.IsValid)
            {
                var entity = new EVMTermOperandComponent
                {
                    EVMTermOperandComponentId = operandComponent.EVMTermOperandComponentId,
                    EarnedValueManagementTermDetailId = operandComponent.EarnedValueManagementTermDetailId,
                    OperandOne = operandComponent.OperandOne,
                    OperandTwo = operandComponent.OperandTwo,
                    Operator = operandComponent.Operator,
                    Formula = operandComponent.Formula,
                    ParentOperand = operandComponent.ParentOperand,
                    HiddenFormulaValue = operandComponent.HiddenFormulaValue
                };
                entity.HiddenFormulaValue = entity.OperandOne + "$" + entity.Operator.ToString() + "$" + entity.OperandTwo;
                entity.Formula = entity.OperandOne + " " + this.GetOperator(entity.Operator) + " " + entity.OperandTwo ;

                db.EVMTermOperandComponents.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                this.UpdateCascadedFormula(entity.EarnedValueManagementTermDetailId, entity);
            }

            return Json(new[] { operandComponent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OperandComponent_Destroy([DataSourceRequest]DataSourceRequest request, EVMTermOperandComponent operandComponent)
        {
            if (ModelState.IsValid)
            {
                var entity = new EVMTermOperandComponent
                {
                    EVMTermOperandComponentId = operandComponent.EVMTermOperandComponentId,
                    EarnedValueManagementTermDetailId = operandComponent.EarnedValueManagementTermDetailId,
                    OperandOne = operandComponent.OperandOne,
                    OperandTwo = operandComponent.OperandTwo,
                    Operator = operandComponent.Operator,
                    Formula = operandComponent.Formula,
                    ParentOperand = operandComponent.ParentOperand,
                    HiddenFormulaValue = operandComponent.HiddenFormulaValue
                };

                db.EVMTermOperandComponents.Attach(entity);
                db.EVMTermOperandComponents.Remove(entity);
                db.SaveChanges();

                this.RemoveFormula(entity.EarnedValueManagementTermDetailId, entity);
            }

            return Json(new[] { operandComponent }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void UpdateCascadedFormula(int detailId, EVMTermOperandComponent operandComponent)
        {
            var dbInstance = new ExceedDbContext();
            var currentTermDetail = dbInstance.EarnedValueManagementTermDetail.Find(detailId);
            var currentTerm = new EarnedValueManagementTerm();
            if (currentTermDetail != null)
            {
                currentTerm = dbInstance.EarnedValueManagementTerms.Find(currentTermDetail.EarnedValueManagementTermId);

                if (operandComponent.ParentOperand.ToString() == "OperandOne")
                {
                    currentTermDetail.OperandOne = operandComponent.Formula;
                    currentTermDetail.OperandOneHiddenValue = operandComponent.HiddenFormulaValue;
                    currentTermDetail.Formula = "( " + operandComponent.Formula + " ) " + this.GetOperator(currentTermDetail.Operator) + " ( " + currentTermDetail.OperandTwo +" )";
                    currentTermDetail.HiddenFormulaValue = operandComponent.HiddenFormulaValue + "#" + currentTermDetail.Operator.ToString() + "#" + currentTermDetail.OperandTwoHiddenValue;
                }
                else if(operandComponent.ParentOperand.ToString() == "OperandTwo") {
                    currentTermDetail.OperandTwo = operandComponent.Formula;
                    currentTermDetail.OperandTwoHiddenValue = operandComponent.HiddenFormulaValue;
                    currentTermDetail.Formula =  "( "+ currentTermDetail.OperandOne +" ) " + this.GetOperator(currentTermDetail.Operator) + " ( " + operandComponent.Formula + " )";
                    currentTermDetail.HiddenFormulaValue = currentTermDetail.OperandOneHiddenValue + "#" + currentTermDetail.Operator.ToString() + "#" + operandComponent.HiddenFormulaValue;
                }

                dbInstance.EarnedValueManagementTermDetail.Attach(currentTermDetail);
                dbInstance.Entry(currentTermDetail).State = EntityState.Modified;
                dbInstance.SaveChanges();
            }
            if(currentTerm != null)
            {
                currentTerm.Formula = currentTermDetail.Formula;

                dbInstance.EarnedValueManagementTerms.Attach(currentTerm);
                dbInstance.Entry(currentTerm).State = EntityState.Modified;
                dbInstance.SaveChanges();
            }
        }
        private void RemoveFormula(int detailId, EVMTermOperandComponent operandComponent)
        {
            var dbInstance = new ExceedDbContext();
            var currentDetail = dbInstance.EarnedValueManagementTermDetail.Find(detailId);
            var currentTerm = new EarnedValueManagementTerm();
            if (currentDetail != null)
            {
                currentTerm = dbInstance.EarnedValueManagementTerms.Find(currentDetail.EarnedValueManagementTermId);
                if (operandComponent.ParentOperand.ToString() == "OperandOne") {
                    currentDetail.OperandOne = null;
                    currentDetail.OperandOneHiddenValue = null;

                    currentDetail.Formula = currentDetail.OperandTwo;
                    currentDetail.HiddenFormulaValue = currentDetail.OperandTwoHiddenValue;
                }
                else if (operandComponent.ParentOperand.ToString() == "OperandTwo")
                {
                    currentDetail.OperandTwo = null;
                    currentDetail.OperandTwoHiddenValue = null;

                    currentDetail.Formula = currentDetail.OperandOne;
                    currentDetail.HiddenFormulaValue = currentDetail.OperandOneHiddenValue;
                }
                dbInstance.EarnedValueManagementTermDetail.Attach(currentDetail);
                dbInstance.Entry(currentDetail).State = EntityState.Modified;
                dbInstance.SaveChanges();

                //dbInstance = new ExceedDbContext();
                //var currentTerm = dbInstance.EarnedValueManagementTerms.Find(currentDetail);
                currentTerm.Formula = currentDetail.Formula;
                dbInstance.EarnedValueManagementTerms.Attach(currentTerm);
                dbInstance.Entry(currentTerm).State = EntityState.Modified;
                dbInstance.SaveChanges();
            }


        }
    }
}