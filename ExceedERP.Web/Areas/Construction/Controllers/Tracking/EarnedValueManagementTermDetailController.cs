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

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    public class EarnedValueManagementTermDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/EarnedValuePerProjectDetail
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult EarnedValuetDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EarnedValueManagementTermDetail> earnedValueDetails = db.EarnedValueManagementTermDetail.Where(ev => ev.EarnedValueManagementTermId == id);
            DataSourceResult result = earnedValueDetails.ToDataSourceResult(request, earnedValueDetail => new {
                EarnedValueManagementTermDetailId = earnedValueDetail.EarnedValueManagementTermDetailId,
                EarnedValueManagementTermId = earnedValueDetail.EarnedValueManagementTermId,
                OperandOne = earnedValueDetail.OperandOne,
                OperandOneHiddenValue = earnedValueDetail.OperandOneHiddenValue,
                OperandTwo = earnedValueDetail.OperandTwo,
                OperandTwoHiddenValue = earnedValueDetail.OperandTwoHiddenValue,
                Operator = earnedValueDetail.Operator,
                Formula = earnedValueDetail.Formula,
                HiddenFormulaValue = earnedValueDetail.HiddenFormulaValue
            });

            return Json(result, JsonRequestBehavior.AllowGet);
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
        private bool CheckExistance(int modelId)
        {
            var valueExists = false;
            var existingFormula = db.EarnedValueManagementTermDetail.Where(evDetail => evDetail.EarnedValueManagementTermId == modelId).FirstOrDefault();
            if (existingFormula != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuetDetail_Create([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermDetail earnedValueDetail, int idCreate)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(idCreate))
                {
                    ModelState.AddModelError("", "Multiple Formulae cannot be used!!");
                }
                else
                {

                    var entity = new EarnedValueManagementTermDetail
                    {
                        //EarnedValuePerProjectDetailId = earnedValueDetail.EarnedValuePerProjectDetailId,
                        EarnedValueManagementTermId = idCreate,
                        OperandOne = earnedValueDetail.OperandOne,
                        OperandOneHiddenValue = earnedValueDetail.OperandOne,
                        OperandTwo = earnedValueDetail.OperandTwo,
                        OperandTwoHiddenValue = earnedValueDetail.OperandTwo,
                        Operator = earnedValueDetail.Operator,
                        Formula = earnedValueDetail.Formula
                    };
                    entity.HiddenFormulaValue = entity.OperandOne + "#" + entity.Operator.ToString() + "#" + entity.OperandTwo;
                    entity.Formula = entity.OperandOne + " " + this.GetOperator(entity.Operator) + " " + entity.OperandTwo;

                    db.EarnedValueManagementTermDetail.Add(entity);
                    db.SaveChanges();
                    earnedValueDetail.EarnedValueManagementTermDetailId = entity.EarnedValueManagementTermDetailId;

                    this.UpdateEVMTermFormula(entity.EarnedValueManagementTermId, entity);

                }
            }

            return Json(new[] { earnedValueDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuetDetail_Update([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermDetail earnedValueDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTermDetail
                {
                    EarnedValueManagementTermDetailId = earnedValueDetail.EarnedValueManagementTermDetailId,
                    EarnedValueManagementTermId = earnedValueDetail.EarnedValueManagementTermId,
                    OperandOne = earnedValueDetail.OperandOne,
                    OperandOneHiddenValue = earnedValueDetail.OperandOne,
                    OperandTwo = earnedValueDetail.OperandTwo,
                    OperandTwoHiddenValue = earnedValueDetail.OperandTwo,
                    Operator = earnedValueDetail.Operator,
                    Formula = earnedValueDetail.Formula
                };
                entity.HiddenFormulaValue = entity.OperandOne + "#" + entity.Operator.ToString() + "#" + entity.OperandTwo;
                entity.Formula = entity.OperandOne + " " + this.GetOperator(entity.Operator) + " " + entity.OperandTwo;

                db.EarnedValueManagementTermDetail.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                this.UpdateEVMTermFormula(entity.EarnedValueManagementTermId, entity);
            }

            return Json(new[] { earnedValueDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuetDetail_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValueManagementTermDetail earnedValueDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementTermDetail
                {
                    EarnedValueManagementTermDetailId = earnedValueDetail.EarnedValueManagementTermDetailId,
                    EarnedValueManagementTermId = earnedValueDetail.EarnedValueManagementTermId,
                    OperandOne = earnedValueDetail.OperandOne,
                    OperandOneHiddenValue = earnedValueDetail.OperandOneHiddenValue,
                    OperandTwo = earnedValueDetail.OperandTwo,
                    OperandTwoHiddenValue = earnedValueDetail.OperandTwoHiddenValue,
                    Operator = earnedValueDetail.Operator,
                    Formula = earnedValueDetail.Formula,
                    HiddenFormulaValue = earnedValueDetail.HiddenFormulaValue
                };

                db.EarnedValueManagementTermDetail.Attach(entity);
                db.EarnedValueManagementTermDetail.Remove(entity);
                db.SaveChanges();

                this.RemoveFormula(entity.EarnedValueManagementTermId);
            }

            return Json(new[] { earnedValueDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        // 
        //  Summary:
        //      Update the formula of the EVA Term after adding the formula assignment
        private void UpdateEVMTermFormula(int termId, EarnedValueManagementTermDetail detail)
        {
            var dbInstance = new ExceedDbContext();
            var currentTerm = dbInstance.EarnedValueManagementTerms.Find(termId);
            if(currentTerm != null)
            {
                currentTerm.Formula = detail.Formula;

                dbInstance.EarnedValueManagementTerms.Attach(currentTerm);
                dbInstance.Entry(currentTerm).State = EntityState.Modified;
                dbInstance.SaveChanges();
            }
        }
        private void RemoveFormula(int termId)
        {
            var dbInstance = new ExceedDbContext();
            var currentTerm = dbInstance.EarnedValueManagementTerms.Find(termId);
            if (currentTerm != null)
            {
                
                currentTerm.Formula = "";

                dbInstance.EarnedValueManagementTerms.Attach(currentTerm);
                dbInstance.Entry(currentTerm).State = EntityState.Modified;
                dbInstance.SaveChanges();
            }
        }
    }
}