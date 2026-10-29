namespace ExceedERP.Reporting.Planning.Design
{
    partial class PlanVsActualQuarterlyReport
    {
        private void InitializeComponent()
        {
            var instanceReportSource = new Telerik.Reporting.InstanceReportSource();
            var fiscalYearParameter = new Telerik.Reporting.ReportParameter();
            var workUnitParameter = new Telerik.Reporting.ReportParameter();
            var formTitleParameter = new Telerik.Reporting.ReportParameter();
            var formNumberParameter = new Telerik.Reporting.ReportParameter();
            var styleRule = new Telerik.Reporting.Drawing.StyleRule();

            this.headerSubReportLandScape = new ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape();
            this.fiscalYearObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.workUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.planVsActualQuarterlyObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection = new Telerik.Reporting.ReportHeaderSection();
            this.subReport = new Telerik.Reporting.SubReport();
            this.filterTextBox = new Telerik.Reporting.TextBox();
            this.detailSection = new Telerik.Reporting.DetailSection();
            this.table = new Telerik.Reporting.Table();
            this.pageFooterSection = new Telerik.Reporting.PageFooterSection();
            this.pageNumberTextBox = new Telerik.Reporting.TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            this.headerSubReportLandScape.Name = "HeaderSubReportLandScape";

            this.fiscalYearObjectDataSource.DataMember = "GetFiscalYear";
            this.fiscalYearObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.FiscalYearODS);
            this.fiscalYearObjectDataSource.Name = "fiscalYearObjectDataSource";

            this.workUnitObjectDataSource.DataMember = "GetWorkUnit";
            this.workUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.WorkUnitODS);
            this.workUnitObjectDataSource.Name = "workUnitObjectDataSource";

            this.planVsActualQuarterlyObjectDataSource.DataMember = "GetPlanVsActualQuarterly";
            this.planVsActualQuarterlyObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.PlanVsActualQuarterlyODS);
            this.planVsActualQuarterlyObjectDataSource.Name = "planVsActualQuarterlyObjectDataSource";
            this.planVsActualQuarterlyObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
                new Telerik.Reporting.ObjectDataSourceParameter("planyear", typeof(string), "= Parameters.PlanFiscalYear.Value"),
                new Telerik.Reporting.ObjectDataSourceParameter("workUnitId", typeof(string), "= Parameters.WorkUnit.Value")
            });

            this.reportHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(1.32D);
            this.reportHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.subReport,
                this.filterTextBox
            });
            this.reportHeaderSection.Name = "reportHeaderSection";

            this.subReport.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport.Name = "subReport";
            instanceReportSource.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource.ReportDocument = this.headerSubReportLandScape;
            this.subReport.ReportSource = instanceReportSource;
            this.subReport.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(11.1D), Telerik.Reporting.Drawing.Unit.Inch(1.02D));

            this.filterTextBox.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.03D));
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(11.1D), Telerik.Reporting.Drawing.Unit.Inch(0.28D));
            this.filterTextBox.Style.Font.Bold = true;
            this.filterTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.filterTextBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.filterTextBox.Value = "Fiscal Year: {Parameters.PlanFiscalYear.Label}     Work Unit: {Parameters.WorkUnit.Label}";

            var descriptiveHeaders = new[]
            {
                "S.N", "Work Unit", "Objective", "Indicator", "Sub Indicator", "Description", "Unit", "KPI Type",
                "Baseline", "Target"
            };
            var fields = new[]
            {
                "SerialNumber", "WorkUnit", "Objective", "Indicator", "SubIndicator", "Description", "Unit", "KPIType",
                "Baseline", "Target", "AnnualPlan", "AnnualActual", "AnnualPercent",
                "QuarterOnePlan", "QuarterOneActual", "QuarterOnePercent",
                "QuarterTwoPlan", "QuarterTwoActual", "QuarterTwoPercent",
                "QuarterThreePlan", "QuarterThreeActual", "QuarterThreePercent",
                "QuarterFourPlan", "QuarterFourActual", "QuarterFourPercent"
            };
            var widths = new[]
            {
                0.24D, 0.73D, 0.80D, 0.73D, 0.73D, 0.84D, 0.32D, 0.39D, 0.39D, 0.38D,
                0.37D, 0.37D, 0.37D, 0.37D, 0.37D, 0.37D, 0.37D, 0.37D, 0.37D,
                0.37D, 0.37D, 0.37D, 0.37D, 0.37D, 0.37D
            };

            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.38D)));
            var tableItems = new System.Collections.Generic.List<Telerik.Reporting.ReportItemBase>();
            for (var index = 0; index < fields.Length; index++)
            {
                this.table.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(
                    Telerik.Reporting.Drawing.Unit.Inch(widths[index])));

                var detailTextBox = CreateDetailTextBox(index, fields[index], widths[index]);
                this.table.Body.SetCellContent(0, index, detailTextBox);
                tableItems.Add(detailTextBox);
            }

            this.table.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.3D)));
            var totalLabelTextBox = CreateTotalTextBox("totalLabel", "Total", widths[0]);
            totalLabelTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.table.Body.SetCellContent(1, 0, totalLabelTextBox, 1, 10);
            tableItems.Add(totalLabelTextBox);
            for (var index = 10; index < fields.Length; index++)
            {
                var isPercentColumn = (index - 10) % 3 == 2;
                string totalValue;
                if (isPercentColumn)
                {
                    var planField = "Sum(Fields." + fields[index - 2] + ")";
                    var actualField = "Sum(Fields." + fields[index - 1] + ")";
                    totalValue = "= IIf(" + planField + " = 0, 0, 100 * " + actualField + " / IIf(" + planField + " = 0, 1, " + planField + "))";
                }
                else
                {
                    totalValue = "= Sum(Fields." + fields[index] + ")";
                }

                var totalTextBox = CreateTotalTextBox("total" + index, totalValue, widths[index]);
                totalTextBox.Format = isPercentColumn ? "{0:N2}%" : "{0:N2}";
                this.table.Body.SetCellContent(1, index, totalTextBox);
                tableItems.Add(totalTextBox);
            }

            for (var index = 0; index < descriptiveHeaders.Length; index++)
            {
                var headerTextBox = CreateHeaderTextBox(
                    "header" + index,
                    descriptiveHeaders[index],
                    widths[index],
                    0.62D,
                    7D,
                    System.Drawing.Color.LightGray);
                var columnGroup = new Telerik.Reporting.TableGroup
                {
                    Name = "columnGroup" + index,
                    ReportItem = headerTextBox
                };
                this.table.ColumnGroups.Add(columnGroup);
                tableItems.Add(headerTextBox);
            }

            var periodHeaders = new[] { "Annual", "Quarter 1", "Quarter 2", "Quarter 3", "Quarter 4" };
            var subHeaders = new[] { "Plan", "Actual", "%" };
            for (var periodIndex = 0; periodIndex < periodHeaders.Length; periodIndex++)
            {
                var firstColumnIndex = 10 + (periodIndex * 3);
                var periodWidth = widths[firstColumnIndex] + widths[firstColumnIndex + 1] + widths[firstColumnIndex + 2];
                var periodHeaderTextBox = CreateHeaderTextBox(
                    "periodHeader" + periodIndex,
                    periodHeaders[periodIndex],
                    periodWidth,
                    0.30D,
                    7D,
                    System.Drawing.Color.LightSteelBlue);
                var periodGroup = new Telerik.Reporting.TableGroup
                {
                    Name = "periodGroup" + periodIndex,
                    ReportItem = periodHeaderTextBox
                };
                tableItems.Add(periodHeaderTextBox);

                for (var subHeaderIndex = 0; subHeaderIndex < subHeaders.Length; subHeaderIndex++)
                {
                    var columnIndex = firstColumnIndex + subHeaderIndex;
                    var subHeaderTextBox = CreateHeaderTextBox(
                        "periodSubHeader" + periodIndex + "_" + subHeaderIndex,
                        subHeaders[subHeaderIndex],
                        widths[columnIndex],
                        0.32D,
                        6D,
                        System.Drawing.Color.LightGray);
                    var subHeaderGroup = new Telerik.Reporting.TableGroup
                    {
                        Name = "periodSubGroup" + periodIndex + "_" + subHeaderIndex,
                        ReportItem = subHeaderTextBox
                    };
                    periodGroup.ChildGroups.Add(subHeaderGroup);
                    tableItems.Add(subHeaderTextBox);
                }

                this.table.ColumnGroups.Add(periodGroup);
            }

            var detailGroup = new Telerik.Reporting.TableGroup { Name = "detail" };
            detailGroup.Groupings.Add(new Telerik.Reporting.Grouping(null));
            this.table.RowGroups.Add(detailGroup);
            var totalRowGroup = new Telerik.Reporting.TableGroup { Name = "totalRow" };
            this.table.RowGroups.Add(totalRowGroup);
            this.table.DataSource = this.planVsActualQuarterlyObjectDataSource;
            this.table.Items.AddRange(tableItems.ToArray());
            this.table.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table.Name = "table";
            this.table.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(11.1D), Telerik.Reporting.Drawing.Unit.Inch(1D));

            this.detailSection.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.detailSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] { this.table });
            this.detailSection.Name = "detailSection";

            this.pageFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25D);
            this.pageFooterSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] { this.pageNumberTextBox });
            this.pageFooterSection.Name = "pageFooterSection";

            this.pageNumberTextBox.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(9.6D), Telerik.Reporting.Drawing.Unit.Inch(0.02D));
            this.pageNumberTextBox.Name = "pageNumberTextBox";
            this.pageNumberTextBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.pageNumberTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageNumberTextBox.Value = "Page: {PageNumber} of {PageCount}";

            fiscalYearParameter.AllowNull = true;
            fiscalYearParameter.AvailableValues.DataSource = this.fiscalYearObjectDataSource;
            fiscalYearParameter.AvailableValues.DisplayMember = "= Fields.Name";
            fiscalYearParameter.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            fiscalYearParameter.Name = "PlanFiscalYear";
            fiscalYearParameter.Text = "Planning Year";
            fiscalYearParameter.Visible = true;

            workUnitParameter.AllowNull = true;
            workUnitParameter.AvailableValues.DataSource = this.workUnitObjectDataSource;
            workUnitParameter.AvailableValues.DisplayMember = "= Fields.Name";
            workUnitParameter.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            workUnitParameter.Name = "WorkUnit";
            workUnitParameter.Text = "Work Unit";
            workUnitParameter.Visible = true;

            formTitleParameter.AllowNull = true;
            formTitleParameter.Name = "FormTitle";
            formNumberParameter.AllowNull = true;
            formNumberParameter.Name = "FormNumber";

            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.reportHeaderSection,
                this.detailSection,
                this.pageFooterSection
            });
            this.Name = "PlanVsActualQuarterlyReport";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
                Telerik.Reporting.Drawing.Unit.Inch(0.25D), Telerik.Reporting.Drawing.Unit.Inch(0.25D),
                Telerik.Reporting.Drawing.Unit.Inch(0.3D), Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.ReportParameters.Add(fiscalYearParameter);
            this.ReportParameters.Add(workUnitParameter);
            this.ReportParameters.Add(formTitleParameter);
            this.ReportParameters.Add(formNumberParameter);

            styleRule.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))
            });
            styleRule.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(1D);
            styleRule.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(1D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] { styleRule });
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(11.1D);

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        private static Telerik.Reporting.TextBox CreateHeaderTextBox(
            string name,
            string value,
            double width,
            double height,
            double fontSize,
            System.Drawing.Color backgroundColor)
        {
            var textBox = new Telerik.Reporting.TextBox
            {
                CanGrow = true,
                Name = name,
                Size = new Telerik.Reporting.Drawing.SizeU(
                    Telerik.Reporting.Drawing.Unit.Inch(width), Telerik.Reporting.Drawing.Unit.Inch(height)),
                Value = value
            };
            textBox.Style.BackgroundColor = backgroundColor;
            textBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            textBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            textBox.Style.Font.Bold = true;
            textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(fontSize);
            textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            return textBox;
        }

        private static Telerik.Reporting.TextBox CreateTotalTextBox(string name, string value, double width)
        {
            var textBox = new Telerik.Reporting.TextBox
            {
                CanGrow = true,
                Name = name,
                Size = new Telerik.Reporting.Drawing.SizeU(
                    Telerik.Reporting.Drawing.Unit.Inch(width), Telerik.Reporting.Drawing.Unit.Inch(0.3D)),
                Value = value
            };
            textBox.Style.BackgroundColor = System.Drawing.Color.LightGray;
            textBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            textBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            textBox.Style.Font.Bold = true;
            textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6D);
            textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            return textBox;
        }

        private static Telerik.Reporting.TextBox CreateDetailTextBox(int index, string field, double width)
        {
            var textBox = new Telerik.Reporting.TextBox
            {
                CanGrow = true,
                Name = "detail" + index,
                Size = new Telerik.Reporting.Drawing.SizeU(
                    Telerik.Reporting.Drawing.Unit.Inch(width), Telerik.Reporting.Drawing.Unit.Inch(0.38D)),
                Value = "= Fields." + field
            };
            textBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            textBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(6D);
            textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;

            if (index >= 8)
            {
                textBox.Format = index == 12 || index == 15 || index == 18 || index == 21 || index == 24
                    ? "{0:N2}%"
                    : "{0:N2}";
                textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            }
            else
            {
                textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            }

            return textBox;
        }

        private Telerik.Reporting.ReportHeaderSection reportHeaderSection;
        private Telerik.Reporting.DetailSection detailSection;
        private Telerik.Reporting.PageFooterSection pageFooterSection;
        private Telerik.Reporting.SubReport subReport;
        private ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape headerSubReportLandScape;
        private Telerik.Reporting.TextBox filterTextBox;
        private Telerik.Reporting.TextBox pageNumberTextBox;
        private Telerik.Reporting.Table table;
        private Telerik.Reporting.ObjectDataSource fiscalYearObjectDataSource;
        private Telerik.Reporting.ObjectDataSource workUnitObjectDataSource;
        private Telerik.Reporting.ObjectDataSource planVsActualQuarterlyObjectDataSource;
    }
}
