using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PlanningHierarchyImportsController : Controller
    {
        private const int MaximumImportRows = 10000;
        private const int MaximumFileSize = 5 * 1024 * 1024;
        private readonly ExceedDbContext db = new ExceedDbContext();

        [HttpGet]
        public ActionResult DownloadTemplate()
        {
            using (var package = new ExcelPackage())
            {
                CreateImportSheet(package);
                CreateInstructionsSheet(package);
                CreateWorkUnitsSheet(package);

                return File(
                    package.GetAsByteArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "PlanningHierarchyImportTemplate.xlsx");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Import(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return ImportError("Select an Excel file to import.");
            }

            if (!string.Equals(Path.GetExtension(file.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return ImportError("Only .xlsx Excel files are supported. Download and use the supplied template.");
            }

            if (file.ContentLength > MaximumFileSize)
            {
                return ImportError("The Excel file cannot be larger than 5 MB.");
            }

            List<ImportRow> rows;
            try
            {
                rows = ParseRows(file);
            }
            catch (PlanningImportException exception)
            {
                return ImportError(exception.Message);
            }
            catch (Exception)
            {
                return ImportError("The Excel file could not be read. Verify that it is a valid .xlsx file and uses the supplied template.");
            }

            if (!rows.Any())
            {
                return ImportError("The import sheet does not contain any data rows.");
            }

            ImportResult result;
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    result = ImportRows(rows);
                    transaction.Commit();
                }
                catch (PlanningImportException exception)
                {
                    transaction.Rollback();
                    return ImportError(exception.Message);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return ImportError("The import could not be saved. No planning records were created.");
                }
            }

            try
            {
                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Planning Excel Import",
                    "Success",
                    User.Identity.Name,
                    result.Message,
                    Modules.Planning);
            }
            catch (Exception)
            {
                // The import is already committed; a logging failure must not report it as rolled back.
            }

            return Json(new
            {
                Success = true,
                Message = result.Message,
                result.GlobalIndicatorsCreated,
                result.ObjectivesCreated,
                result.IndicatorsCreated,
                result.IndicatorsReused
            });
        }

        private ImportResult ImportRows(IEnumerable<ImportRow> rows)
        {
            var workUnits = db.MainCompanyStructures.ToList();
            var globalIndicators = db.GlobalIndicators.ToList();
            var objectives = db.Objectives.ToList();
            var indicators = db.Indicators.ToList();
            var result = new ImportResult();
            var rowIndicatorMap = new Dictionary<string, Indicator>();

            var globalIndicatorMap = globalIndicators
                .Where(x => !string.IsNullOrWhiteSpace(x.Indicator))
                .GroupBy(x => Normalize(x.Indicator))
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.GlobalIndicatorId).First());

            var objectiveMap = objectives
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => ObjectiveKey(x.OrgStructureId, x.Name))
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.ObjectiveId).First());

            foreach (var row in rows)
            {
                var workUnit = ResolveWorkUnit(row, workUnits);
                var rowKey = Normalize(row.RowKey);
                if (rowKey.Length > 0 && rowIndicatorMap.ContainsKey(rowKey))
                {
                    throw RowError(row, "Row Key is duplicated. Every populated Row Key must be unique in the file.");
                }

                GlobalIndicator globalIndicator;
                var globalKey = Normalize(row.Indicator);
                if (!globalIndicatorMap.TryGetValue(globalKey, out globalIndicator))
                {
                    globalIndicator = new GlobalIndicator
                    {
                        Indicator = Clean(row.Indicator),
                        Description = CleanOrNull(row.IndicatorDescription),
                        IsActive = row.IsActive,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.GlobalIndicators.Add(globalIndicator);
                    db.SaveChanges();
                    globalIndicatorMap.Add(globalKey, globalIndicator);
                    result.GlobalIndicatorsCreated++;
                }

                Objective objective;
                var objectiveKey = ObjectiveKey(workUnit.OrgStructureId, row.Objective);
                if (!objectiveMap.TryGetValue(objectiveKey, out objective))
                {
                    objective = new Objective
                    {
                        OrgStructureId = workUnit.OrgStructureId,
                        Name = Clean(row.Objective),
                        Weight = row.ObjectiveWeight,
                        Description = CleanOrNull(row.ObjectiveDescription),
                        Remark = CleanOrNull(row.ObjectiveRemark),
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.Objectives.Add(objective);
                    db.SaveChanges();
                    objectiveMap.Add(objectiveKey, objective);
                    result.ObjectivesCreated++;
                }
                else if (row.ObjectiveWeight.HasValue && objective.Weight != row.ObjectiveWeight)
                {
                    objective.Weight = row.ObjectiveWeight;
                    db.SaveChanges();
                }

                var parent = ResolveParent(row, objective.ObjectiveId, indicators, rowIndicatorMap);
                var parentId = parent == null ? 0 : parent.IndicatorId;
                var normalizedUnit = Normalize(row.Unit);

                var indicator = indicators.FirstOrDefault(x =>
                    x.GlobalIndicatorId == globalIndicator.GlobalIndicatorId &&
                    x.ObjectiveId == objective.ObjectiveId &&
                    x.ParentId == parentId &&
                    x.KPIType == row.KpiType &&
                    Normalize(x.Unit) == normalizedUnit);

                if (indicator == null)
                {
                    indicator = new Indicator
                    {
                        GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                        Name = globalIndicator.Indicator,
                        Description = CleanOrNull(row.IndicatorDescription),
                        Weight = row.IndicatorWeight,
                        Unit = CleanOrNull(row.Unit),
                        KPIType = row.KpiType,
                        ObjectiveId = objective.ObjectiveId,
                        ParentId = parentId,
                        HasSubIndicator = false,
                        IsActive = globalIndicator.IsActive
                    };

                    db.Indicators.Add(indicator);
                    db.SaveChanges();
                    indicators.Add(indicator);
                    result.IndicatorsCreated++;
                }
                else
                {
                    if (row.IndicatorWeight.HasValue && indicator.Weight != row.IndicatorWeight)
                    {
                        indicator.Weight = row.IndicatorWeight;
                        db.SaveChanges();
                    }

                    result.IndicatorsReused++;
                }

                if (rowKey.Length > 0)
                {
                    rowIndicatorMap.Add(rowKey, indicator);
                }

                if (parent != null && !parent.HasSubIndicator)
                {
                    parent.HasSubIndicator = true;
                    db.SaveChanges();
                }
            }

            result.RowsProcessed = rows.Count();
            return result;
        }

        private static ExceedERP.Core.Domain.Common.HRM.MainCompanyStructure ResolveWorkUnit(
            ImportRow row,
            IList<ExceedERP.Core.Domain.Common.HRM.MainCompanyStructure> workUnits)
        {
            int workUnitId;
            if (int.TryParse(Clean(row.WorkUnit), out workUnitId))
            {
                var byId = workUnits.FirstOrDefault(x => x.OrgStructureId == workUnitId);
                if (byId != null)
                {
                    return byId;
                }
            }

            var key = Normalize(row.WorkUnit);
            var matches = workUnits.Where(x =>
                    Normalize(x.Name) == key || Normalize(x.Code) == key)
                .ToList();

            if (matches.Count == 1)
            {
                return matches[0];
            }

            if (matches.Count > 1)
            {
                throw RowError(row, "Work Unit is ambiguous. Use its numeric ID from the Work Units sheet.");
            }

            throw RowError(row, "Work Unit was not found. Use an ID, code, or exact name from the Work Units sheet.");
        }

        private static Indicator ResolveParent(
            ImportRow row,
            int objectiveId,
            IList<Indicator> indicators,
            IDictionary<string, Indicator> rowIndicatorMap)
        {
            if (!string.IsNullOrWhiteSpace(row.ParentRowKey))
            {
                Indicator parentByRowKey;
                if (!rowIndicatorMap.TryGetValue(Normalize(row.ParentRowKey), out parentByRowKey))
                {
                    throw RowError(row, "Parent Row Key was not found. Put every parent row before its child rows.");
                }

                if (parentByRowKey.ObjectiveId != objectiveId)
                {
                    throw RowError(row, "Parent Row Key belongs to a different objective.");
                }

                return parentByRowKey;
            }

            if (string.IsNullOrWhiteSpace(row.ParentIndicator))
            {
                return null;
            }

            var parentKey = Normalize(row.ParentIndicator);
            var matches = indicators.Where(x =>
                    x.ObjectiveId == objectiveId && Normalize(x.Name) == parentKey)
                .ToList();

            if (matches.Count == 1)
            {
                return matches[0];
            }

            if (matches.Count > 1)
            {
                throw RowError(row, "Parent Indicator matches more than one indicator in this objective.");
            }

            throw RowError(row, "Parent Indicator was not found in this objective. Put every parent row before its child rows.");
        }

        private static List<ImportRow> ParseRows(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                var worksheet = package.Workbook.Worksheets["Planning Import"] ??
                                package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null || worksheet.Dimension == null)
                {
                    throw new PlanningImportException("The workbook does not contain an import worksheet.");
                }

                var columns = ReadHeaderColumns(worksheet);
                var workUnitColumn = RequireColumn(columns, "Work Unit", "Work Unit ID", "OrgStructureId", "Department");
                var objectiveColumn = RequireColumn(columns, "Objective", "Objective Name");
                var indicatorColumn = RequireColumn(columns, "Indicator", "Indicator Name", "Global Indicator");
                var objectiveWeightColumn = FindColumn(columns, "Objective Weight", "ObjectiveWeight");
                var indicatorWeightColumn = FindColumn(columns, "Indicator Weight", "IndicatorWeight", "Activity Weight", "ActivityWeight", "Weight");
                var objectiveDescriptionColumn = FindColumn(columns, "Objective Description");
                var objectiveRemarkColumn = FindColumn(columns, "Objective Remark", "Remark");
                var rowKeyColumn = FindColumn(columns, "Row Key", "Indicator Row Key");
                var parentRowKeyColumn = FindColumn(columns, "Parent Row Key");
                var parentIndicatorColumn = FindColumn(columns, "Parent Indicator", "Parent Indicator Name");
                var unitColumn = FindColumn(columns, "Unit");
                var kpiTypeColumn = FindColumn(columns, "KPI Type", "KPIType");
                var indicatorDescriptionColumn = FindColumn(columns, "Indicator Description", "Description");
                var isActiveColumn = FindColumn(columns, "Is Active", "Active");

                var lastRow = worksheet.Dimension.End.Row;
                if (lastRow - 1 > MaximumImportRows)
                {
                    throw new PlanningImportException("A maximum of " + MaximumImportRows + " data rows can be imported at once.");
                }

                var rows = new List<ImportRow>();
                for (var rowNumber = 2; rowNumber <= lastRow; rowNumber++)
                {
                    var workUnit = CellText(worksheet, rowNumber, workUnitColumn);
                    var objective = CellText(worksheet, rowNumber, objectiveColumn);
                    var indicator = CellText(worksheet, rowNumber, indicatorColumn);

                    if (string.IsNullOrWhiteSpace(workUnit) &&
                        string.IsNullOrWhiteSpace(objective) &&
                        string.IsNullOrWhiteSpace(indicator))
                    {
                        continue;
                    }

                    var row = new ImportRow
                    {
                        RowNumber = rowNumber,
                        WorkUnit = workUnit,
                        Objective = objective,
                        ObjectiveWeight = ParseWeight(CellText(worksheet, rowNumber, objectiveWeightColumn), rowNumber, "Objective Weight"),
                        ObjectiveDescription = CellText(worksheet, rowNumber, objectiveDescriptionColumn),
                        ObjectiveRemark = CellText(worksheet, rowNumber, objectiveRemarkColumn),
                        RowKey = CellText(worksheet, rowNumber, rowKeyColumn),
                        Indicator = indicator,
                        IndicatorWeight = ParseWeight(CellText(worksheet, rowNumber, indicatorWeightColumn), rowNumber, "Indicator Weight"),
                        ParentRowKey = CellText(worksheet, rowNumber, parentRowKeyColumn),
                        ParentIndicator = CellText(worksheet, rowNumber, parentIndicatorColumn),
                        Unit = CellText(worksheet, rowNumber, unitColumn),
                        IndicatorDescription = CellText(worksheet, rowNumber, indicatorDescriptionColumn),
                        IsActive = ParseIsActive(CellText(worksheet, rowNumber, isActiveColumn), rowNumber)
                    };

                    if (string.IsNullOrWhiteSpace(row.WorkUnit))
                    {
                        throw RowError(row, "Work Unit is required.");
                    }

                    if (string.IsNullOrWhiteSpace(row.Objective))
                    {
                        throw RowError(row, "Objective is required.");
                    }

                    if (string.IsNullOrWhiteSpace(row.Indicator))
                    {
                        throw RowError(row, "Indicator is required.");
                    }

                    row.KpiType = ParseKpiType(CellText(worksheet, rowNumber, kpiTypeColumn), rowNumber);
                    rows.Add(row);
                }

                return rows;
            }
        }

        private static Dictionary<string, int> ReadHeaderColumns(ExcelWorksheet worksheet)
        {
            var columns = new Dictionary<string, int>();
            for (var column = 1; column <= worksheet.Dimension.End.Column; column++)
            {
                var header = NormalizeHeader(worksheet.Cells[1, column].Text);
                if (!string.IsNullOrWhiteSpace(header) && !columns.ContainsKey(header))
                {
                    columns.Add(header, column);
                }
            }

            return columns;
        }

        private static int RequireColumn(IDictionary<string, int> columns, params string[] names)
        {
            var column = FindColumn(columns, names);
            if (column == 0)
            {
                throw new PlanningImportException("Missing required column: " + names[0] + ".");
            }

            return column;
        }

        private static int FindColumn(IDictionary<string, int> columns, params string[] names)
        {
            foreach (var name in names)
            {
                int column;
                if (columns.TryGetValue(NormalizeHeader(name), out column))
                {
                    return column;
                }
            }

            return 0;
        }

        private static string CellText(ExcelWorksheet worksheet, int row, int column)
        {
            return column == 0 ? null : CleanOrNull(worksheet.Cells[row, column].Text);
        }

        private static decimal? ParseWeight(string value, int rowNumber, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            decimal result;
            if (decimal.TryParse(value.Trim(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result) ||
                decimal.TryParse(value.Trim(), out result))
            {
                if (result < 0)
                {
                    throw new PlanningImportException("Row " + rowNumber + ": " + fieldName + " cannot be negative.");
                }

                return result;
            }

            throw new PlanningImportException("Row " + rowNumber + ": " + fieldName + " must be a valid number.");
        }

        private static KPIType ParseKpiType(string value, int rowNumber)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return KPIType.Quantity;
            }

            KPIType result;
            if (Enum.TryParse(value.Trim(), true, out result) && Enum.IsDefined(typeof(KPIType), result))
            {
                return result;
            }

            throw new PlanningImportException(
                "Row " + rowNumber + ": KPI Type must be Quantity, Finance, Amount, Days, or Hours.");
        }

        private static bool ParseIsActive(string value, int rowNumber)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            bool result;
            if (bool.TryParse(value.Trim(), out result))
            {
                return result;
            }

            switch (Normalize(value))
            {
                case "1":
                case "YES":
                case "Y":
                case "ACTIVE":
                    return true;
                case "0":
                case "NO":
                case "N":
                case "INACTIVE":
                    return false;
                default:
                    throw new PlanningImportException(
                        "Row " + rowNumber + ": Is Active must be TRUE/FALSE, YES/NO, or 1/0.");
            }
        }

        private void CreateImportSheet(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Planning Import");
            var headers = new[]
            {
                "Work Unit",
                "Objective",
                "Objective Weight",
                "Objective Description",
                "Objective Remark",
                "Row Key",
                "Indicator",
                "Indicator Weight",
                "Parent Row Key",
                "Parent Indicator",
                "Unit",
                "KPI Type",
                "Indicator Description",
                "Is Active"
            };

            for (var column = 1; column <= headers.Length; column++)
            {
                worksheet.Cells[1, column].Value = headers[column - 1];
            }

            using (var header = worksheet.Cells[1, 1, 1, headers.Length])
            {
                header.Style.Font.Bold = true;
                header.Style.Font.Color.SetColor(System.Drawing.Color.White);
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            }

            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells[1, 1, 1, headers.Length].AutoFilter = true;
            worksheet.Column(1).Width = 18;
            worksheet.Column(2).Width = 28;
            worksheet.Column(3).Width = 18;
            worksheet.Column(4).Width = 32;
            worksheet.Column(5).Width = 24;
            worksheet.Column(6).Width = 18;
            worksheet.Column(7).Width = 28;
            worksheet.Column(8).Width = 18;
            worksheet.Column(9).Width = 18;
            worksheet.Column(10).Width = 28;
            worksheet.Column(11).Width = 14;
            worksheet.Column(12).Width = 16;
            worksheet.Column(13).Width = 32;
            worksheet.Column(14).Width = 12;

            var kpiTypeValidation = worksheet.DataValidations.AddListValidation("L2:L5000");
            kpiTypeValidation.Formula.Values.Add("Quantity");
            kpiTypeValidation.Formula.Values.Add("Finance");
            kpiTypeValidation.Formula.Values.Add("Amount");
            kpiTypeValidation.Formula.Values.Add("Days");
            kpiTypeValidation.Formula.Values.Add("Hours");

            var activeValidation = worksheet.DataValidations.AddListValidation("N2:N5000");
            activeValidation.Formula.Values.Add("TRUE");
            activeValidation.Formula.Values.Add("FALSE");
        }

        private static void CreateInstructionsSheet(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Instructions");
            worksheet.Cells[1, 1].Value = "Planning hierarchy import";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[3, 1].Value = "1.";
            worksheet.Cells[3, 2].Value = "Enter one indicator or sub-indicator per row on the Planning Import sheet.";
            worksheet.Cells[4, 1].Value = "2.";
            worksheet.Cells[4, 2].Value = "Work Unit accepts the numeric ID, code, or exact name shown on the Work Units sheet.";
            worksheet.Cells[5, 1].Value = "3.";
            worksheet.Cells[5, 2].Value = "Give each row a unique Row Key. Leave Parent Row Key blank for a top-level indicator.";
            worksheet.Cells[6, 1].Value = "4.";
            worksheet.Cells[6, 2].Value = "For a sub-indicator, enter its parent's Row Key and put the parent row first.";
            worksheet.Cells[7, 1].Value = "5.";
            worksheet.Cells[7, 2].Value = "Repeat Work Unit and Objective on every row, including sub-indicator rows.";
            worksheet.Cells[8, 1].Value = "6.";
            worksheet.Cells[8, 2].Value = "Objective Weight and Indicator Weight accept decimal or numeric values (e.g. 19 or 1.1).";
            worksheet.Cells[9, 1].Value = "7.";
            worksheet.Cells[9, 2].Value = "Existing global indicators, objectives, and matching hierarchy records are reused.";
            worksheet.Cells[11, 1].Value = "Example hierarchy order";
            worksheet.Cells[11, 1].Style.Font.Bold = true;
            worksheet.Cells[12, 1].Value = "Row Key";
            worksheet.Cells[12, 2].Value = "Indicator";
            worksheet.Cells[12, 3].Value = "Indicator Weight";
            worksheet.Cells[12, 4].Value = "Parent Row Key";
            worksheet.Cells[13, 1].Value = "I-001";
            worksheet.Cells[13, 2].Value = "Increase customer satisfaction";
            worksheet.Cells[13, 3].Value = "15";
            worksheet.Cells[14, 1].Value = "I-002";
            worksheet.Cells[14, 2].Value = "Resolve customer complaints";
            worksheet.Cells[14, 3].Value = "8.5";
            worksheet.Cells[14, 4].Value = "I-001";
            worksheet.Cells[15, 1].Value = "I-003";
            worksheet.Cells[15, 2].Value = "Resolve priority complaints";
            worksheet.Cells[15, 3].Value = "6.5";
            worksheet.Cells[15, 4].Value = "I-002";
            worksheet.Column(1).Width = 24;
            worksheet.Column(2).Width = 55;
            worksheet.Column(3).Width = 18;
            worksheet.Column(4).Width = 24;
        }

        private void CreateWorkUnitsSheet(ExcelPackage package)
        {
            var worksheet = package.Workbook.Worksheets.Add("Work Units");
            worksheet.Cells[1, 1].Value = "Work Unit ID";
            worksheet.Cells[1, 2].Value = "Code";
            worksheet.Cells[1, 3].Value = "Name";

            var workUnits = db.MainCompanyStructures
                .OrderBy(x => x.Name)
                .Select(x => new { x.OrgStructureId, x.Code, x.Name })
                .ToList();

            for (var index = 0; index < workUnits.Count; index++)
            {
                var row = index + 2;
                worksheet.Cells[row, 1].Value = workUnits[index].OrgStructureId;
                worksheet.Cells[row, 2].Value = workUnits[index].Code;
                worksheet.Cells[row, 3].Value = workUnits[index].Name;
            }

            using (var header = worksheet.Cells[1, 1, 1, 3])
            {
                header.Style.Font.Bold = true;
                header.Style.Font.Color.SetColor(System.Drawing.Color.White);
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.SteelBlue);
            }

            worksheet.View.FreezePanes(2, 1);
            worksheet.Cells[1, 1, Math.Max(workUnits.Count + 1, 1), 3].AutoFilter = true;
            worksheet.Column(1).Width = 16;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 45;
        }

        private JsonResult ImportError(string message)
        {
            return Json(new { Success = false, Message = message });
        }

        private static PlanningImportException RowError(ImportRow row, string message)
        {
            return new PlanningImportException("Row " + row.RowNumber + ": " + message);
        }

        private static string ObjectiveKey(int workUnitId, string objective)
        {
            return workUnitId + "|" + Normalize(objective);
        }

        private static string NormalizeHeader(string value)
        {
            return new string((value ?? string.Empty)
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }

        private static string Normalize(string value)
        {
            return string.Join(
                    " ",
                    (value ?? string.Empty).Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                .Trim()
                .ToUpperInvariant();
        }

        private static string Clean(string value)
        {
            return string.Join(
                    " ",
                    (value ?? string.Empty).Split((char[])null, StringSplitOptions.RemoveEmptyEntries))
                .Trim();
        }

        private static string CleanOrNull(string value)
        {
            var cleaned = Clean(value);
            return cleaned.Length == 0 ? null : cleaned;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        private class ImportRow
        {
            public int RowNumber { get; set; }
            public string WorkUnit { get; set; }
            public string Objective { get; set; }
            public decimal? ObjectiveWeight { get; set; }
            public string ObjectiveDescription { get; set; }
            public string ObjectiveRemark { get; set; }
            public string RowKey { get; set; }
            public string Indicator { get; set; }
            public decimal? IndicatorWeight { get; set; }
            public string ParentRowKey { get; set; }
            public string ParentIndicator { get; set; }
            public string Unit { get; set; }
            public KPIType KpiType { get; set; }
            public string IndicatorDescription { get; set; }
            public bool IsActive { get; set; }
        }

        private class ImportResult
        {
            public int RowsProcessed { get; set; }
            public int GlobalIndicatorsCreated { get; set; }
            public int ObjectivesCreated { get; set; }
            public int IndicatorsCreated { get; set; }
            public int IndicatorsReused { get; set; }

            public string Message
            {
                get
                {
                    return RowsProcessed + " row(s) imported: " +
                           GlobalIndicatorsCreated + " global indicator(s), " +
                           ObjectivesCreated + " objective(s), and " +
                           IndicatorsCreated + " hierarchy indicator(s) created; " +
                           IndicatorsReused + " matching hierarchy indicator(s) reused.";
                }
            }
        }

        private class PlanningImportException : Exception
        {
            public PlanningImportException(string message)
                : base(message)
            {
            }
        }
    }
}
