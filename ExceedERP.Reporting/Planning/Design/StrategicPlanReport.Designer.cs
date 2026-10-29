namespace ExceedERP.Reporting.Planning.Design
{
    partial class StrategicPlanReport
    {
        private void InitializeComponent()
        {
            Telerik.Reporting.InstanceReportSource headerReportSource = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.ReportParameter strategicPlanParameter = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter formTitleParameter = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter formNumberParameter = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule = new Telerik.Reporting.Drawing.StyleRule();
            this.strategicPlanLookupDataSource = new Telerik.Reporting.ObjectDataSource();
            this.strategicPlanReportDataSource = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection = new Telerik.Reporting.ReportHeaderSection();
            this.headerSubReport = new ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape();
            this.subReport = new Telerik.Reporting.SubReport();
            this.filterTextBox = new Telerik.Reporting.TextBox();
            this.detailSection = new Telerik.Reporting.DetailSection();
            this.strategicPlanTable = new Telerik.Reporting.Table();
            this.pageFooterSection = new Telerik.Reporting.PageFooterSection();
            this.pageNumberTextBox = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            this.strategicPlanLookupDataSource.DataMember = "GetStrategicPlans";
            this.strategicPlanLookupDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.StrategicPlanReportODS);
            this.strategicPlanLookupDataSource.Name = "strategicPlanLookupDataSource";

            this.strategicPlanReportDataSource.DataMember = "GetStrategicPlanReport";
            this.strategicPlanReportDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.StrategicPlanReportODS);
            this.strategicPlanReportDataSource.Name = "strategicPlanReportDataSource";
            this.strategicPlanReportDataSource.Parameters.Add(
                new Telerik.Reporting.ObjectDataSourceParameter(
                    "strategicPlanId",
                    typeof(string),
                    "= Parameters.StrategicPlan.Value"));

            this.reportHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(2.45D);
            this.reportHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.subReport,
                this.filterTextBox});
            this.reportHeaderSection.Name = "reportHeaderSection";

            this.headerSubReport.Name = "headerSubReportLandScape";
            headerReportSource.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            headerReportSource.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            headerReportSource.ReportDocument = this.headerSubReport;
            this.subReport.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D),
                Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport.Name = "subReport";
            this.subReport.ReportSource = headerReportSource;
            this.subReport.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(15.2D),
                Telerik.Reporting.Drawing.Unit.Inch(2.03D));

            this.filterTextBox.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D),
                Telerik.Reporting.Drawing.Unit.Inch(2.08D));
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(15.2D),
                Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.filterTextBox.Style.Font.Bold = true;
            this.filterTextBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.filterTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.filterTextBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.filterTextBox.Value = "Strategic Plan: {Parameters.StrategicPlan.Label}";

            this.detailSection.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.detailSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.strategicPlanTable});
            this.detailSection.Name = "detailSection";

            BuildStrategicPlanTable();

            this.pageFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25D);
            this.pageFooterSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.pageNumberTextBox});
            this.pageFooterSection.Name = "pageFooterSection";
            this.pageNumberTextBox.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(12.9D),
                Telerik.Reporting.Drawing.Unit.Inch(0.02D));
            this.pageNumberTextBox.Name = "pageNumberTextBox";
            this.pageNumberTextBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(2.3D),
                Telerik.Reporting.Drawing.Unit.Inch(0.22D));
            this.pageNumberTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageNumberTextBox.Value = "Page: {PageNumber} of {PageCount}";

            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.reportHeaderSection,
                this.detailSection,
                this.pageFooterSection});
            this.Name = "StrategicPlanReport";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
                Telerik.Reporting.Drawing.Unit.Inch(0.3D),
                Telerik.Reporting.Drawing.Unit.Inch(0.3D),
                Telerik.Reporting.Drawing.Unit.Inch(0.3D),
                Telerik.Reporting.Drawing.Unit.Inch(0.3D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;

            strategicPlanParameter.AllowNull = true;
            strategicPlanParameter.AvailableValues.DataSource = this.strategicPlanLookupDataSource;
            strategicPlanParameter.AvailableValues.DisplayMember = "= Fields.DisplayName";
            strategicPlanParameter.AvailableValues.ValueMember = "= Fields.StrategicPlanId";
            strategicPlanParameter.Name = "StrategicPlan";
            strategicPlanParameter.Text = "Strategic Plan";
            strategicPlanParameter.Visible = true;
            formTitleParameter.Name = "FormTitle";
            formTitleParameter.Value = "Strategic Plan Report";
            formNumberParameter.Name = "FormNumber";
            this.ReportParameters.Add(strategicPlanParameter);
            this.ReportParameters.Add(formTitleParameter);
            this.ReportParameters.Add(formNumberParameter);

            styleRule.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
                new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] { styleRule });
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.2D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }

        private void BuildStrategicPlanTable()
        {
            double[] widths = { 0.45D, 1.25D, 2D, 1.15D, 0.9D, 2.2D, 1.5D, 0.7D, 2.6D, 1.7D };
            string[] headings = {
                "S.N", "Plan Period", "Key Result Area", "Item Type", "Fiscal Year",
                "Goal / Objective", "Work Unit", "Weight", "Description", "Remark"
            };
            string[] values = {
                "= Fields.SerialNumber", "= Fields.PlanPeriod", "= Fields.KeyResultArea",
                "= Fields.ItemType", "= Fields.FiscalYear", "= Fields.Name",
                "= Fields.WorkUnit", "= Fields.Weight", "= Fields.Description", "= Fields.Remark"
            };
            var headers = new Telerik.Reporting.TextBox[headings.Length];
            var details = new Telerik.Reporting.TextBox[values.Length];

            this.strategicPlanTable.Body.Rows.Add(
                new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.35D)));

            for (var index = 0; index < widths.Length; index++)
            {
                this.strategicPlanTable.Body.Columns.Add(
                    new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(widths[index])));

                headers[index] = CreateHeaderTextBox("header" + index, headings[index], widths[index]);
                details[index] = CreateDetailTextBox("detail" + index, values[index], widths[index], index);
                this.strategicPlanTable.Body.SetCellContent(0, index, details[index]);

                var columnGroup = new Telerik.Reporting.TableGroup();
                columnGroup.Name = "columnGroup" + index;
                columnGroup.ReportItem = headers[index];
                this.strategicPlanTable.ColumnGroups.Add(columnGroup);
            }

            this.strategicPlanTable.DataSource = this.strategicPlanReportDataSource;
            this.strategicPlanTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                details[0], details[1], details[2], details[3], details[4],
                details[5], details[6], details[7], details[8], details[9],
                headers[0], headers[1], headers[2], headers[3], headers[4],
                headers[5], headers[6], headers[7], headers[8], headers[9]});
            this.strategicPlanTable.Location = new Telerik.Reporting.Drawing.PointU(
                Telerik.Reporting.Drawing.Unit.Inch(0D),
                Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.strategicPlanTable.Name = "strategicPlanTable";
            var detailGroup = new Telerik.Reporting.TableGroup();
            detailGroup.Groupings.Add(new Telerik.Reporting.Grouping(null));
            detailGroup.Name = "detailGroup";
            this.strategicPlanTable.RowGroups.Add(detailGroup);
            this.strategicPlanTable.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(14.45D),
                Telerik.Reporting.Drawing.Unit.Inch(0.9D));
        }

        private static Telerik.Reporting.TextBox CreateHeaderTextBox(string name, string value, double width)
        {
            var textBox = new Telerik.Reporting.TextBox();
            textBox.Name = name;
            textBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(width),
                Telerik.Reporting.Drawing.Unit.Inch(0.55D));
            textBox.Style.BackgroundColor = System.Drawing.Color.Gray;
            textBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            textBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            textBox.Style.Color = System.Drawing.Color.White;
            textBox.Style.Font.Bold = true;
            textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            textBox.Value = value;
            return textBox;
        }

        private static Telerik.Reporting.TextBox CreateDetailTextBox(
            string name,
            string value,
            double width,
            int columnIndex)
        {
            var textBox = new Telerik.Reporting.TextBox();
            textBox.CanGrow = true;
            textBox.Name = name;
            textBox.Size = new Telerik.Reporting.Drawing.SizeU(
                Telerik.Reporting.Drawing.Unit.Inch(width),
                Telerik.Reporting.Drawing.Unit.Inch(0.35D));
            textBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            textBox.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            textBox.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            textBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            textBox.Value = value;

            if (columnIndex == 0 || columnIndex == 3 || columnIndex == 4)
            {
                textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            }
            else if (columnIndex == 7)
            {
                textBox.Format = "{0:N2}";
                textBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            }

            return textBox;
        }

        private Telerik.Reporting.ObjectDataSource strategicPlanLookupDataSource;
        private Telerik.Reporting.ObjectDataSource strategicPlanReportDataSource;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection;
        private ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape headerSubReport;
        private Telerik.Reporting.SubReport subReport;
        private Telerik.Reporting.TextBox filterTextBox;
        private Telerik.Reporting.DetailSection detailSection;
        private Telerik.Reporting.Table strategicPlanTable;
        private Telerik.Reporting.PageFooterSection pageFooterSection;
        private Telerik.Reporting.TextBox pageNumberTextBox;
    }
}
