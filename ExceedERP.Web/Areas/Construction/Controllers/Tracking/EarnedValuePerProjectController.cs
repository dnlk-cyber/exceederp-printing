using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    [AuthorizeRoles(ConstructionConstants.ProjectTracking)]
    public class EarnedValuePerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/EarnedValuePerProject
        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;

            //List<YearViewModel> years = new List<YearViewModel>();
            ////List<string> years = new List<string>();
            //for (int i = -5; i <= 5; i++)
            //{
            //    var yearValue = DateTime.Now.AddYears(i).ToString("yyyy");
            //    var year = new YearViewModel();
            //    year.Title = yearValue;
            //    years.Add(year);
            //    //years.Add(DateTime.Now.AddYears(i).ToString("yyyy"));
            //}
            //ViewData["Years"] = years;
            //ViewData["ProjectTypeList"] = db.ProjectTypes.ToList();

            ViewData["Periods"] = db.ConstructionPeriod.Select(p => new { p.ConstructionPeriodId, p.Name }).ToList();

            ViewData["Branch"] = db.Branch.Select(b => new { b.Name, b.BranchId }).ToList();

            ViewData["Projects"] = db.ProjectInformation.Select(p => new { p.Name, p.ProjectInformationId}).ToList();
            return View();
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation
                .Where(x => (x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED) && !x.IsDeleted && ids.Contains(x.ProjectInformationId));
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new EVMProjectGeneralInfomationViewModel
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,

                ContractAmount = projectInformation.ContractAmount,
                ExpectedProfitRate = ConsSettingHelper.GetProjectSpecificSettingValue(projectInformation.ProjectInformationId, ProjectSettingAttributes.ExpectedProfitRate),
                ExpectedProfit = this.CalculateExpectedProfitAmount(projectInformation.ProjectInformationId),
                BAC = this.CalculateBudgetAtCompletionAmount(projectInformation.ProjectInformationId),


            });

            return Json(result);
        }

        public ActionResult EarnedValuePerProject_Read([DataSourceRequest]DataSourceRequest request, int projectIdRead)
        {
            IQueryable<EarnedValuePerProject> earnedValues = db.EarnedValuePerProject.Where(pt => pt.ProjectInformationId == projectIdRead).OrderBy(t => t.ProjectInformationId);
            DataSourceResult result = earnedValues.ToDataSourceResult(request, earnedValue => new
            {
                EarnedValuePerProjectId = earnedValue.EarnedValuePerProjectId,
                GlFiscalYear = earnedValue.GlFiscalYear,
                ConstructionPeriodId = earnedValue.ConstructionPeriodId,
                BAC = earnedValue.BAC,
                BCWS = earnedValue.BCWS,
                BCWSRaw = earnedValue.BCWSRaw,
                BCWP = earnedValue.BCWP,
                BCWPRaw = earnedValue.BCWPRaw,
                ACWP = earnedValue.ACWP,
                ACWPRaw = earnedValue.ACWPRaw,
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
                ProjectInformationId = earnedValue.ProjectInformationId,

                OrgBranchName = earnedValue.OrgBranchName,
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
            var existingTerm = db.EarnedValuePerProject.FirstOrDefault(ev =>
                                        ev.ProjectInformationId == projectId &&
                                        ev.ConstructionPeriodId == model.ConstructionPeriodId);
            if (existingTerm != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        private bool ProjectInformationNotSetProperly(int projectId)
        {
            var currentProject = db.ProjectInformation.Find(projectId);
            var contractAmount = new decimal();
            var valueNotSet = false;
            var expectedProfit = this.CalculateExpectedProfitAmount(projectId);

            if (currentProject != null)
            {
                contractAmount = currentProject.ContractAmount;
            }
            if (expectedProfit <= 0 || contractAmount <= 0)
            {
                valueNotSet = true;
            }
            return valueNotSet;
        }
        private decimal GetCalculatedValueForTerm(string termAbbriviation, EarnedValuePerProject currentModel)
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerProject_Create([DataSourceRequest]DataSourceRequest request, EarnedValuePerProject earnedValue, int projectIdCreate, AvailableProjectTypes projectType)
        {
            if (ModelState.IsValid)
            {
                var errorExists = false;
                var profitRate = ConsSettingHelper.GetProjectSpecificSettingValue(projectIdCreate, ProjectSettingAttributes.ExpectedProfitRate);
                var requiredFactor = (100 - profitRate) / 100;

                if (CheckExistance(earnedValue, projectIdCreate))
                {
                    ModelState.AddModelError("", "value exists for this period!!");
                    errorExists = true;
                }
                else if (ProjectInformationNotSetProperly(projectIdCreate))
                {
                    ModelState.AddModelError("", "Contract amount and Expected Profit Rate Values are not properly set for this project!!!");
                    errorExists = true;
                }
                if (!errorExists)
                {
                    var entity = new EarnedValuePerProject
                    {
                        //EarnedValuePerProjectId = earnedValue.EarnedValuePerProjectId,
                        GlFiscalYear = ConsSettingHelper.GetConstructionPeriodFiscalYear(earnedValue.ConstructionPeriodId),
                        ConstructionPeriodId = earnedValue.ConstructionPeriodId,
                        BAC = this.CalculateBudgetAtCompletionAmount(projectIdCreate),
                        //BAC = earnedValue.BAC,
                        BCWS = earnedValue.BCWS,
                        BCWSRaw = earnedValue.BCWSRaw,
                        BCWP = earnedValue.BCWP,
                        BCWPRaw = earnedValue.BCWPRaw,
                        ACWP = earnedValue.ACWP,
                        ACWPRaw = earnedValue.ACWPRaw,
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
                        ProjectInformationId = projectIdCreate,

                        OrgBranchName = earnedValue.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = earnedValue.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = earnedValue.ModifiedBy
                    };
                    if (projectType != AvailableProjectTypes.Road)
                    {
                        entity.BCWS = this.CalculateCummulativeValue(requiredFactor, entity, "BCWS");
                        entity.BCWP = this.CalculateCummulativeValue(requiredFactor, entity, "BCWP");
                        entity.ACWP = this.CalculateCummulativeValue(requiredFactor, entity, "ACWP");
                    }

                    entity.PCP = (this.GetCalculatedValueForTerm("PCP", entity)) * 100;
                    entity.PCA = (this.GetCalculatedValueForTerm("PCA", entity)) * 100;
                    entity.CV = this.GetCalculatedValueForTerm("CV", entity);
                    entity.CVPercentage = (this.GetCalculatedValueForTerm("CV%", entity));
                    entity.CPI = this.GetCalculatedValueForTerm("CPI", entity);
                    entity.SV = this.GetCalculatedValueForTerm("SV", entity);
                    entity.SVPercentage = (this.GetCalculatedValueForTerm("SV%", entity));
                    entity.SPI = this.GetCalculatedValueForTerm("SPI", entity);
                    entity.CR = this.GetCalculatedValueForTerm("CR", entity);

                    entity.TCPI = this.GetCalculatedValueForTerm("TCPI", entity);
                    entity.TSPI = this.GetCalculatedValueForTerm("TSPI", entity);
                    entity.EAC = this.GetCalculatedValueForTerm("EAC", entity);
                    entity.VAC = this.GetCalculatedValueForTerm("VAC", entity);
                    entity.ETC = this.GetCalculatedValueForTerm("ETC", entity);
                    entity.ISAC = entity.SPI != 0 ? entity.BAC / entity.SPI : 0;

                    db.EarnedValuePerProject.Add(entity);
                    db.SaveChanges();
                    earnedValue.EarnedValuePerProjectId = entity.EarnedValuePerProjectId;

                }
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerProject_Update([DataSourceRequest]DataSourceRequest request, EarnedValuePerProject earnedValue, AvailableProjectTypes projectType)
        {
            if (ModelState.IsValid)
            {
                var profitRate = ConsSettingHelper.GetProjectSpecificSettingValue(earnedValue.ProjectInformationId, ProjectSettingAttributes.ExpectedProfitRate);
                var requiredFactor = (100 - profitRate) / 100;

                var entity = new EarnedValuePerProject
                {
                    EarnedValuePerProjectId = earnedValue.EarnedValuePerProjectId,
                    GlFiscalYear = ConsSettingHelper.GetConstructionPeriodFiscalYear(earnedValue.ConstructionPeriodId),
                    ConstructionPeriodId = earnedValue.ConstructionPeriodId,
                    //BAC = this.CalculateBudgetAtCompletionAmount(earnedValue.ProjectInformationId),
                    BAC = earnedValue.BAC,
                    BCWS = earnedValue.BCWS,
                    BCWSRaw = earnedValue.BCWSRaw,
                    BCWP = earnedValue.BCWP,
                    BCWPRaw = earnedValue.BCWPRaw,
                    ACWP = earnedValue.ACWP,
                    ACWPRaw = earnedValue.ACWPRaw,
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
                    ProjectInformationId = earnedValue.ProjectInformationId,

                    OrgBranchName = earnedValue.OrgBranchName,
                    DateCreated = earnedValue.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = earnedValue.CreatedBy,
                    ModifiedBy = User.Identity.Name

                };
                if (projectType != AvailableProjectTypes.Road)
                {
                    entity.BCWS = this.CalculateCummulativeValue(requiredFactor, entity, "BCWS");
                    entity.BCWP = this.CalculateCummulativeValue(requiredFactor, entity, "BCWP");
                    entity.ACWP = this.CalculateCummulativeValue(requiredFactor, entity, "ACWP");
                }
                entity.PCP = (this.GetCalculatedValueForTerm("PCP", entity)) * 100;
                entity.PCA = (this.GetCalculatedValueForTerm("PCA", entity)) * 100;
                entity.CV = this.GetCalculatedValueForTerm("CV", entity);
                entity.CVPercentage = (this.GetCalculatedValueForTerm("CV%", entity));
                entity.CPI = this.GetCalculatedValueForTerm("CPI", entity);
                entity.SV = this.GetCalculatedValueForTerm("SV", entity);
                entity.SVPercentage = (this.GetCalculatedValueForTerm("SV%", entity));
                entity.SPI = this.GetCalculatedValueForTerm("SPI", entity);
                entity.CR = this.GetCalculatedValueForTerm("CR", entity);

                entity.TCPI = this.GetCalculatedValueForTerm("TCPI", entity);
                entity.TSPI = this.GetCalculatedValueForTerm("TSPI", entity);
                entity.EAC = this.GetCalculatedValueForTerm("EAC", entity);
                entity.VAC = this.GetCalculatedValueForTerm("VAC", entity);
                entity.ETC = this.GetCalculatedValueForTerm("ETC", entity);
                entity.ISAC = entity.SPI != 0 ? entity.BAC / entity.SPI : 0;

                db.EarnedValuePerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EarnedValuePerProject_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValuePerProject earnedValue)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValuePerProject
                {
                    EarnedValuePerProjectId = earnedValue.EarnedValuePerProjectId,
                    GlFiscalYear = earnedValue.GlFiscalYear,
                    ConstructionPeriodId = earnedValue.ConstructionPeriodId,
                    BAC = earnedValue.BAC,
                    BCWS = earnedValue.BCWS,
                    BCWSRaw = earnedValue.BCWSRaw,
                    BCWP = earnedValue.BCWP,
                    BCWPRaw = earnedValue.BCWPRaw,
                    ACWP = earnedValue.ACWP,
                    ACWPRaw = earnedValue.ACWPRaw,
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
                    ProjectInformationId = earnedValue.ProjectInformationId,

                    OrgBranchName = earnedValue.OrgBranchName,
                    CreatedBy = earnedValue.CreatedBy,
                    DateCreated = earnedValue.DateCreated,
                    ModifiedBy = earnedValue.ModifiedBy,
                    LastModified = earnedValue.LastModified

                };

                db.EarnedValuePerProject.Attach(entity);
                db.EarnedValuePerProject.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { earnedValue }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateExpectedProfitAmount(int projectId)
        {
            decimal value = new decimal();
            var project = db.ProjectInformation.Find(projectId);
            if (project != null)
            {
                var expectedProfitRate = ConsSettingHelper.GetProjectSpecificSettingValue(project.ProjectInformationId, ProjectSettingAttributes.ExpectedProfitRate);
                value = (expectedProfitRate * project.ContractAmount) / 100;
            }
            return value;
        }
        private decimal CalculateBudgetAtCompletionAmount(int projectId)
        {
            decimal value = new decimal();
            var project = db.ProjectInformation.Find(projectId);
            if (project != null)
            {
                var expectedProfitRate = ConsSettingHelper.GetProjectSpecificSettingValue(project.ProjectInformationId, ProjectSettingAttributes.ExpectedProfitRate);
                var expectedProfit = (expectedProfitRate * project.ContractAmount) / 100;
                value = project.ContractAmount - expectedProfit;
            }

            return value;
        }
        private decimal CalculateCummulativeValue(decimal factor, EarnedValuePerProject model, string attribute)
        {
            //var allPeriods = db.ConstructionPeriod.ToList();
            var calculatedValue = new decimal();
            var existingRecords =
                db.EarnedValuePerProject.Where(evm => evm.ProjectInformationId == model.ProjectInformationId && evm.EarnedValuePerProjectId != model.EarnedValuePerProjectId).ToList();
            var currentPeriod = db.ConstructionPeriod.Find(model.ConstructionPeriodId);

            List<EarnedValuePerProject> list = new List<EarnedValuePerProject>();
            if (existingRecords.Any() && currentPeriod != null)
            {
                foreach (EarnedValuePerProject earnedValue in existingRecords)
                {
                    var period = db.ConstructionPeriod.Find(earnedValue.ConstructionPeriodId);
                    if (period != null)
                    {
                        if (period.DateTo < currentPeriod.DateFrom)
                        {
                            list.Add(earnedValue);
                        }
                    }
                }
            }
            if (attribute == "BCWS")
            {
                if (list.Any())
                {
                    var sumOfExistingRawBCWS = list.Sum(v => v.BCWSRaw);
                    calculatedValue = (sumOfExistingRawBCWS + model.BCWSRaw) * factor;
                }
                else calculatedValue = model.BCWSRaw * factor;
            }
            else if (attribute == "BCWP")
            {
                if (list.Any())
                {
                    var sumOfExistingRawBCWP = list.Sum(v => v.BCWPRaw);
                    calculatedValue = (sumOfExistingRawBCWP + model.BCWPRaw) * factor;
                }
                else calculatedValue = model.BCWPRaw * factor;
            }
            else if (attribute == "ACWP")
            {
                if (list.Any())
                {
                    var sumOfExistingRawACWP = list.Sum(v => v.ACWPRaw);
                    calculatedValue = sumOfExistingRawACWP + model.ACWPRaw;
                }
                else calculatedValue = model.ACWPRaw;
            }
            return calculatedValue;
        }
        //  Summary :
        //      Used to calculate the input values in earned value management from performance evaluation
        public decimal GetCalculatedBudgetedCostOfWorkScheduled(int constructionPeriodId, int projectId)
        {
            //var values = new decimal[3];
            decimal bcws = 0;
            var performanceEvaluationPeriod = db.PeriodMapping.FirstOrDefault(ep => ep.ConstructionPeriodId == constructionPeriodId && ep.ProjectInformationId == projectId);
            var profitSetting = db.ProjectSpecificSettingAttributes.FirstOrDefault(s => s.ProjectInformationId == projectId && s.Attribute == ProjectSettingAttributes.ExpectedProfitRate);
            if (performanceEvaluationPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.FirstOrDefault(pr => pr.PeriodMappingId == performanceEvaluationPeriod.PeriodMappingId);
                if (progress != null)
                {
                    var plannedAmounntForCurrentMonth = progress.PlannedAmount;
                    bcws = plannedAmounntForCurrentMonth;
                    if (profitSetting != null)
                    {
                        var profitRate = profitSetting.Value;
                        bcws = plannedAmounntForCurrentMonth * ((100 - profitRate)/100);
                    }
                }
            }
            //values[0] = bcws; values[1] = bcwp; values[2] = acwp;
            return bcws;
        }
        // BCWP from performance
        public decimal GetCalculatedBudgetedCostOfWorkPerformed(int constructionPeriodId, int projectId)
        {
            decimal bcwp = 0;
            var performanceEvaluationPeriod = db.PeriodMapping.FirstOrDefault(ep => ep.ConstructionPeriodId == constructionPeriodId && ep.ProjectInformationId == projectId);
            var profitSetting = db.ProjectSpecificSettingAttributes.FirstOrDefault(s => s.ProjectInformationId == projectId && s.Attribute == ProjectSettingAttributes.ExpectedProfitRate);
            if (performanceEvaluationPeriod != null)
            {
                var progress = db.MonthlyProjectProgress.FirstOrDefault(pr => pr.PeriodMappingId == performanceEvaluationPeriod.PeriodMappingId);
                if (progress != null)
                {
                    var executedAmountForTheMonth = progress.ExecutedAmount;
                    bcwp = executedAmountForTheMonth;
                    if (profitSetting != null)
                    {
                        var profitRate = profitSetting.Value;
                        bcwp = executedAmountForTheMonth * ((100 - profitRate)/100);
                    }
                }
            }
            return bcwp;
        }
        public decimal GetCalculatedActualCostOfWorkPerformed(int constructionPeriodId, int projectId)
        {
            decimal acwp = 0;
            var performanceEvaluationPeriod = db.PeriodMapping.FirstOrDefault(ep => ep.ConstructionPeriodId == constructionPeriodId && ep.ProjectInformationId == projectId);
            //var profitSetting = db.ProjectSpecificSettingAttributes.FirstOrDefault(s => s.ProjectInformationId == projectId && s.Attribute == ProjectSettingAttributes.ExpectedProfitRate);
            if (performanceEvaluationPeriod != null)
            {
                var cost = db.MonthlyProjectCost.FirstOrDefault(pr => pr.PeriodMappingId == performanceEvaluationPeriod.PeriodMappingId);
                if (cost != null)
                {
                    var actualProjectOverHeadForTheMonth = cost.ActualProjectOHCost;
                    var actualDirecCostForTheMonth = cost.ActualDirectCost;
                    acwp = actualProjectOverHeadForTheMonth + actualDirecCostForTheMonth;
                }
            }
            return acwp;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}