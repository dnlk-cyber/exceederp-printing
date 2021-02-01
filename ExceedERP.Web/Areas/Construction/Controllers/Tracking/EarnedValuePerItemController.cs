using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    [AuthorizeRoles(ConstructionConstants.ProjectTracking)]
    public class EarnedValuePerItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/EarnedValuePerItem
        public ActionResult Index()
        {
            #region concatnated work items
            //var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemName).ToList();
            //var workItemsWithConcatnatedValue = workItems.Select(w => new { w.WorkItemPerProjectId, Name = w.ItemName + w.} );
            //var workItem = new WorkItemPerProject();
            var workItemsWithConcatnatedValue =
                (from w in db.WorkItemPerProject.OrderBy(w => w.ItemCode)
                 join b in db.Blocks
                 on w.BlockId equals b.BlockId
                 //join sp in db.SubProjectType
                 //on b.SubProjectTypeId equals sp.SubProjectTypeId
                 select new
                 {
                     WorkItemPerProjectId = w.WorkItemPerProjectId,
                     ItemName = w.ItemName + " - " + b.Name
                 }).ToList();
            //foreach (var item in workItems)
            //{
            //    var tempWorkItem = new WorkItemPerProject();
            //    if (item.BlockId != null)
            //    {
            //        tempWorkItem = item;
            //        var workItemBlock = db.Blocks.Find(item.BlockId);
            //        if (workItemBlock != null)
            //        {
            //            tempWorkItem.ItemName = item.ItemName + " of " + workItemBlock.Name;
            //        }
            //    }
            //    workItemsWithConcatnatedValue.Add(tempWorkItem);
            //} 
            #endregion
            //var projectWorkItems = db.WorkItemPerProject.ToList();
            ViewData["ProjectWorkItems"] = workItemsWithConcatnatedValue;

            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            ViewData["ConstructionPeriods"] = db.ConstructionPeriod.Select(p => new { p.ConstructionPeriodId, p.Name }).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();

            ViewData["Projects"] = db.ProjectInformation.Select(p => new { p.Name, p.ProjectInformationId }).ToList();

            return View();
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation
                .Where(x => (x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED) && !x.IsDeleted && ids.Contains(x.ProjectInformationId));
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }

        public ActionResult EarnedValuePerItem_Read([DataSourceRequest]DataSourceRequest request, int periodId)
        {
            IQueryable<EarnedValuePerItem> earnedValues = db.EarnedValuePerItem.Where(pt => pt.EarnedValueManagementPeriodId == periodId).OrderBy(t => t.WorkItemPerProjectId);
            DataSourceResult result = earnedValues.ToDataSourceResult(request, earnedValue => new
            {
                EarnedValuePerItemId = earnedValue.EarnedValuePerItemId,
                EarnedValueManagementPeriodId = earnedValue.EarnedValueManagementPeriodId,
                GlFiscalYear = earnedValue.GlFiscalYear,
                WorkItemPerProjectId = earnedValue.WorkItemPerProjectId,
                Unit = earnedValue.Unit,
                BudgetedQuantity = earnedValue.BudgetedQuantity,
                PlannedQuanity = earnedValue.PlannedQuanity,
                ExecutedQuantity = earnedValue.ExecutedQuantity,
                EstimatedUnitPrice = earnedValue.EstimatedUnitPrice,
                ActualUnitPrice = earnedValue.ActualUnitPrice,

                BAC = earnedValue.BAC,
                BCWS = earnedValue.BCWS,
                BCWP = earnedValue.BCWP,
                ACWP = earnedValue.ACWP,
                PCP = earnedValue.PCP,
                PCA = earnedValue.PCA,
                CV = earnedValue.CV,
                CVPercentage = earnedValue.CVPercentage,
                CPI = earnedValue.CPI,
                SV = earnedValue.SV,
                SVPercentage = earnedValue.SVPercentage,
                SPI = earnedValue.SPI,
                CR = earnedValue.CR,
                TCPI = earnedValue.TCPI,
                TSPI = earnedValue.TSPI,
                EAC = earnedValue.EAC,
                VAC = earnedValue.VAC,
                ETC = earnedValue.ETC,
                ISAC = earnedValue.ISAC,

                CreatedBy = earnedValue.CreatedBy,
                DateCreated = earnedValue.DateCreated,
                ModifiedBy = earnedValue.ModifiedBy,
                LastModified = earnedValue.LastModified

            });

            return Json(result);
        }

        private bool CheckExistance(EarnedValuePerProject model, int projectId)
        {
            var valueExists = false;
            var existingTerm = db.EarnedValuePerProject.Where(ev =>
                                        ev.ProjectInformationId == projectId &&
                                        ev.GlFiscalYear == model.GlFiscalYear &&
                                        ev.ConstructionPeriodId == model.ConstructionPeriodId).FirstOrDefault();
            if (existingTerm != null)
            {
                valueExists = true;
            }
            return valueExists;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerItem_Create([DataSourceRequest]DataSourceRequest request, EarnedValuePerItem earnedValue, int periodId)
        {
            if (ModelState.IsValid)
            {

                var entity = new EarnedValuePerItem
                {
                    //EarnedValuePerItemId = earnedValue.EarnedValuePerItemId,
                    WorkItemPerProjectId = earnedValue.WorkItemPerProjectId,
                    EarnedValueManagementPeriodId = periodId,
                    GlFiscalYear = ConsSettingHelper.GetEVMPeriodFiscalYear(periodId),
                    Unit = ResourceUtilizationHelper.GetWorkItemUnitOfMeasurnment(earnedValue.WorkItemPerProjectId).Abbreviation,
                    BudgetedQuantity = earnedValue.BudgetedQuantity,
                    PlannedQuanity = earnedValue.PlannedQuanity,
                    ExecutedQuantity = earnedValue.ExecutedQuantity,
                    EstimatedUnitPrice = earnedValue.EstimatedUnitPrice,
                    ActualUnitPrice = earnedValue.ActualUnitPrice,

                    BAC = earnedValue.BudgetedQuantity * earnedValue.EstimatedUnitPrice,
                    BCWS = earnedValue.PlannedQuanity * earnedValue.EstimatedUnitPrice,
                    BCWP = earnedValue.ExecutedQuantity * earnedValue.EstimatedUnitPrice,
                    ACWP = earnedValue.ExecutedQuantity * earnedValue.ActualUnitPrice,
                    PCP = earnedValue.PCP,
                    PCA = earnedValue.PCA,
                    CV = earnedValue.CV,
                    CVPercentage = earnedValue.CVPercentage,
                    CPI = earnedValue.CPI,
                    SV = earnedValue.SV,
                    SVPercentage = earnedValue.SVPercentage,
                    SPI = earnedValue.SPI,
                    CR = earnedValue.CR,
                    TCPI = earnedValue.TCPI,
                    TSPI = earnedValue.TSPI,
                    EAC = earnedValue.EAC,
                    VAC = earnedValue.VAC,
                    ETC = earnedValue.ETC,
                    ISAC = earnedValue.ISAC,

                    DateCreated = DateTime.Now,
                    LastModified = earnedValue.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = earnedValue.ModifiedBy
                };
                entity.PCP = (this.GetCalculatedValueForTerm("PCP", entity)) * 100;
                entity.PCA = (this.GetCalculatedValueForTerm("PCA", entity)) * 100;
                entity.CV = this.GetCalculatedValueForTerm("CV", entity);
                entity.CVPercentage = this.GetCalculatedValueForTerm("CV%", entity);
                entity.CPI = this.GetCalculatedValueForTerm("CPI", entity);
                entity.SV = this.GetCalculatedValueForTerm("SV", entity);
                entity.SVPercentage = this.GetCalculatedValueForTerm("SV%", entity);
                entity.SPI = this.GetCalculatedValueForTerm("SPI", entity);
                entity.CR = this.GetCalculatedValueForTerm("CR", entity);

                entity.TCPI = this.GetCalculatedValueForTerm("TCPI", entity);
                entity.TSPI = this.GetCalculatedValueForTerm("TSPI", entity);
                entity.EAC = this.GetCalculatedValueForTerm("EAC", entity);
                entity.VAC = this.GetCalculatedValueForTerm("VAC", entity);
                entity.ETC = this.GetCalculatedValueForTerm("ETC", entity);
                entity.ISAC = entity.SPI != 0 ? entity.BAC / entity.SPI : 0;

                db.EarnedValuePerItem.Add(entity);
                db.SaveChanges();
                earnedValue.EarnedValuePerItemId = entity.EarnedValuePerItemId;

                //}
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerItem_Update([DataSourceRequest]DataSourceRequest request, EarnedValuePerItem earnedValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValuePerItem
                {
                    EarnedValuePerItemId = earnedValue.EarnedValuePerItemId,
                    EarnedValueManagementPeriodId = earnedValue.EarnedValueManagementPeriodId,
                    GlFiscalYear = earnedValue.GlFiscalYear,
                    WorkItemPerProjectId = earnedValue.WorkItemPerProjectId,
                    Unit = ResourceUtilizationHelper.GetWorkItemUnitOfMeasurnment(earnedValue.WorkItemPerProjectId).Abbreviation,
                    BudgetedQuantity = earnedValue.BudgetedQuantity,
                    PlannedQuanity = earnedValue.PlannedQuanity,
                    ExecutedQuantity = earnedValue.ExecutedQuantity,
                    EstimatedUnitPrice = earnedValue.EstimatedUnitPrice,
                    ActualUnitPrice = earnedValue.ActualUnitPrice,

                    BAC = earnedValue.BudgetedQuantity * earnedValue.EstimatedUnitPrice,
                    BCWS = earnedValue.PlannedQuanity * earnedValue.EstimatedUnitPrice,
                    BCWP = earnedValue.ExecutedQuantity * earnedValue.EstimatedUnitPrice,
                    ACWP = earnedValue.ExecutedQuantity * earnedValue.ActualUnitPrice,
                    PCP = earnedValue.PCP,
                    PCA = earnedValue.PCA,
                    CV = earnedValue.CV,
                    CVPercentage = earnedValue.CVPercentage,
                    CPI = earnedValue.CPI,
                    SV = earnedValue.SV,
                    SVPercentage = earnedValue.SVPercentage,
                    SPI = earnedValue.SPI,
                    CR = earnedValue.CR,
                    TCPI = earnedValue.TCPI,
                    TSPI = earnedValue.TSPI,
                    EAC = earnedValue.EAC,
                    VAC = earnedValue.VAC,
                    ETC = earnedValue.ETC,
                    ISAC = earnedValue.ISAC,

                    DateCreated = earnedValue.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = earnedValue.CreatedBy,
                    ModifiedBy = User.Identity.Name

                };
                entity.PCP = (this.GetCalculatedValueForTerm("PCP", entity)) * 100;
                entity.PCA = (this.GetCalculatedValueForTerm("PCA", entity)) * 100;
                entity.CV = this.GetCalculatedValueForTerm("CV", entity);
                entity.CVPercentage = this.GetCalculatedValueForTerm("CV%", entity);
                entity.CPI = this.GetCalculatedValueForTerm("CPI", entity);
                entity.SV = this.GetCalculatedValueForTerm("SV", entity);
                entity.SVPercentage = this.GetCalculatedValueForTerm("SV%", entity);
                entity.SPI = this.GetCalculatedValueForTerm("SPI", entity);
                entity.CR = this.GetCalculatedValueForTerm("CR", entity);

                entity.TCPI = this.GetCalculatedValueForTerm("TCPI", entity);
                entity.TSPI = this.GetCalculatedValueForTerm("TSPI", entity);
                entity.EAC = this.GetCalculatedValueForTerm("EAC", entity);
                entity.VAC = this.GetCalculatedValueForTerm("VAC", entity);
                entity.ETC = this.GetCalculatedValueForTerm("ETC", entity);
                entity.ISAC = entity.SPI != 0 ? entity.BAC / entity.SPI : 0;

                db.EarnedValuePerItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerItem_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValuePerItem earnedValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValuePerItem
                {
                    EarnedValuePerItemId = earnedValue.EarnedValuePerItemId,
                    EarnedValueManagementPeriodId = earnedValue.EarnedValueManagementPeriodId,
                    GlFiscalYear = earnedValue.GlFiscalYear,
                    WorkItemPerProjectId = earnedValue.WorkItemPerProjectId,
                    Unit = earnedValue.Unit,
                    BudgetedQuantity = earnedValue.BudgetedQuantity,
                    PlannedQuanity = earnedValue.PlannedQuanity,
                    ExecutedQuantity = earnedValue.ExecutedQuantity,
                    EstimatedUnitPrice = earnedValue.EstimatedUnitPrice,
                    ActualUnitPrice = earnedValue.ActualUnitPrice,

                    BAC = earnedValue.BAC,
                    BCWS = earnedValue.BCWS,
                    BCWP = earnedValue.BCWP,
                    ACWP = earnedValue.ACWP,
                    PCP = earnedValue.PCP,
                    PCA = earnedValue.PCA,
                    CV = earnedValue.CV,
                    CVPercentage = earnedValue.CVPercentage,
                    CPI = earnedValue.CPI,
                    SV = earnedValue.SV,
                    SVPercentage = earnedValue.SVPercentage,
                    SPI = earnedValue.SPI,
                    CR = earnedValue.CR,
                    TCPI = earnedValue.TCPI,
                    TSPI = earnedValue.TSPI,
                    EAC = earnedValue.EAC,
                    VAC = earnedValue.VAC,
                    ETC = earnedValue.ETC,
                    ISAC = earnedValue.ISAC,

                    CreatedBy = earnedValue.CreatedBy,
                    DateCreated = earnedValue.DateCreated,
                    ModifiedBy = earnedValue.ModifiedBy,
                    LastModified = earnedValue.LastModified

                };

                db.EarnedValuePerItem.Attach(entity);
                db.EarnedValuePerItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }
        private decimal GetCalculatedValueForTerm(string termAbbriviation, EarnedValuePerItem currentModel)
        {
            var currentTerm = db.EarnedValueManagementTerms.Where(t => t.Abbreviation == termAbbriviation).FirstOrDefault();
            var concatnatedFormula = "";
            var operandOne = "";
            var currentOptr = "";
            var operandTwo = "";

            decimal operandOneValue = new decimal(), operandTwoValue = new decimal();
            decimal result = new decimal();
            if (currentTerm != null)
            {
                var termDetail = db.EarnedValueManagementTermDetail
                    .Where(evd => evd.EarnedValueManagementTermId == currentTerm.EarnedValueManagementTermId)
                    .FirstOrDefault();
                if (termDetail != null)
                {
                    concatnatedFormula = termDetail.HiddenFormulaValue;
                    string[] components = concatnatedFormula.Split('#');

                    operandOne = components[0];
                    currentOptr = components[1];
                    operandTwo = components[2];

                    // change the current model to Dictionary inorder to acces its properties and
                    // their values in key-value pair manner ....
                    var dict = currentModel.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(currentModel, null));
                    //

                    // operand one is combination of other operands
                    if (operandOne.IndexOf('$') > -1)
                    {
                        string[] operandOneContent = operandOne.Split('$');
                        var innerOperandOne = operandOneContent[0];
                        var innerOperation = operandOneContent[1];
                        var innerOperandTwo = operandOneContent[2];

                        var innerOperandOneValue = new decimal();
                        var innerOperandTwovalue = new decimal();
                        if (dict.ContainsKey(innerOperandOne))
                        {
                            innerOperandOneValue = (decimal)dict[innerOperandOne];
                        }
                        if (dict.ContainsKey(innerOperandTwo))
                        {
                            innerOperandTwovalue = (decimal)dict[innerOperandTwo];
                        }
                        #region finding the inner value of operand one
                        if (innerOperation == "Multiplication")
                        {
                            operandOneValue = innerOperandOneValue * innerOperandTwovalue;
                        }
                        else if (innerOperation == "Division" && innerOperandTwovalue != 0)
                        {
                            operandOneValue = innerOperandOneValue / innerOperandTwovalue;
                        }
                        else if (innerOperation == "Subtraction")
                        {
                            operandOneValue = innerOperandOneValue - innerOperandTwovalue;
                        }
                        else if (innerOperation == "Addition")
                        {
                            operandOneValue = innerOperandOneValue + innerOperandTwovalue;
                        }
                        #endregion
                    }
                    // operand two is combination of other operands
                    if (operandTwo.IndexOf('$') > -1)
                    {
                        string[] operanTwoContent = operandTwo.Split('$');
                        var innerOperandOne = operanTwoContent[0];
                        var innerOperation = operanTwoContent[1];
                        var innerOperandTwo = operanTwoContent[2];

                        var innerOperandOneValue = new decimal();
                        var innerOperandTwovalue = new decimal();
                        if (dict.ContainsKey(innerOperandOne))
                        {
                            innerOperandOneValue = (decimal)dict[innerOperandOne];
                        }
                        if (dict.ContainsKey(innerOperandTwo))
                        {
                            innerOperandTwovalue = (decimal)dict[innerOperandTwo];
                        }
                        #region finding the inner value of operand two
                        if (innerOperation == "Multiplication")
                        {
                            operandTwoValue = innerOperandOneValue * innerOperandTwovalue;
                        }
                        else if (innerOperation == "Division" && innerOperandTwovalue != 0)
                        {
                            operandTwoValue = innerOperandOneValue / innerOperandTwovalue;
                        }
                        else if (innerOperation == "Subtraction")
                        {
                            operandTwoValue = innerOperandOneValue - innerOperandTwovalue;
                        }
                        else if (innerOperation == "Addition")
                        {
                            operandTwoValue = innerOperandOneValue + innerOperandTwovalue;
                        }
                        #endregion
                    }

                    // operand one and operand two are just one value, not combination of other operations
                    if (dict.ContainsKey(operandOne))
                    {
                        operandOneValue = (decimal)dict[operandOne];
                    }
                    if (dict.ContainsKey(operandTwo))
                    {
                        operandTwoValue = (decimal)dict[operandTwo];
                    }


                    if (currentOptr == "Multiplication")
                    {
                        result = operandOneValue * operandTwoValue;
                    }
                    else if (currentOptr == "Division" && operandTwoValue != 0)
                    {
                        result = operandOneValue / operandTwoValue;
                    }
                    else if (currentOptr == "Subtraction")
                    {
                        result = operandOneValue - operandTwoValue;
                    }
                    else if (currentOptr == "Addition")
                    {
                        result = operandOneValue + operandTwoValue;
                    }

                    // check if this term has constant multiplier, or any constants....
                    var constantComponent = db.EarnedValueManagementTermFormulaConstants.FirstOrDefault(c => c.EarnedValueManagementTermDetailId == termDetail.EarnedValueManagementTermDetailId);
                    if (constantComponent != null)
                    {
                        var concatnatedConstantValue = constantComponent.HiddenFormulaValue;
                        if (concatnatedConstantValue.IndexOf('#') > -1)
                        {
                            string[] constantValueSubConponents = concatnatedConstantValue.Split('#');
                            var constantValueOperator = constantValueSubConponents[0];
                            var constantValueNumber = constantValueSubConponents[1];

                            var decimalValueOfConstant = Convert.ToDecimal(constantValueNumber);

                            if (constantValueOperator == "Multiplication")
                            {
                                result = result * decimalValueOfConstant;
                            }
                            else if (constantValueOperator == "Division" && decimalValueOfConstant != 0)
                            {
                                result = result / decimalValueOfConstant;
                            }
                            else if (constantValueOperator == "Subtraction")
                            {
                                result = result - decimalValueOfConstant;
                            }
                            else if (constantValueOperator == "Addition")
                            {
                                result = result + decimalValueOfConstant;
                            }
                        }
                    }
                }

            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}