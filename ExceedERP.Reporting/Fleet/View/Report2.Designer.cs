namespace ExceedERP.Reporting.Fleet.View
{
    partial class Report2
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.GraphGroup graphGroup1 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup3 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup4 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup5 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup6 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphGroup graphGroup7 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphTitle graphTitle1 = new Telerik.Reporting.GraphTitle();
            Telerik.Reporting.NumericalScale numericalScale1 = new Telerik.Reporting.NumericalScale();
            Telerik.Reporting.CategoryScale categoryScale1 = new Telerik.Reporting.CategoryScale();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report2));
            Telerik.Reporting.GraphGroup graphGroup2 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.SummaryGraphDS = new Telerik.Reporting.ObjectDataSource();
            this.graph1 = new Telerik.Reporting.Graph();
            this.cartesianCoordinateSystem1 = new Telerik.Reporting.CartesianCoordinateSystem();
            this.graphAxis1 = new Telerik.Reporting.GraphAxis();
            this.graphAxis2 = new Telerik.Reporting.GraphAxis();
            this.barSeries1 = new Telerik.Reporting.BarSeries();
            this.barSeries2 = new Telerik.Reporting.BarSeries();
            this.barSeries3 = new Telerik.Reporting.BarSeries();
            this.barSeries4 = new Telerik.Reporting.BarSeries();
            this.barSeries5 = new Telerik.Reporting.BarSeries();
            this.barSeries6 = new Telerik.Reporting.BarSeries();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(7.619999885559082D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.graph1});
            this.detailSection1.Name = "detailSection1";
            // 
            // SummaryGraphDS
            // 
            this.SummaryGraphDS.DataMember = "GetSummaryInfo";
            this.SummaryGraphDS.DataSource = typeof( ExceedERP.Reporting.Fleet.FleetMaintenance.DataSource.SummaryDataSource);
            this.SummaryGraphDS.Name = "SummaryGraphDS";
            this.SummaryGraphDS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("FiscalYear", typeof(string), "= Parameters.FiscalYear.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("Period", typeof(string), "= Parameters.Period.Value")});
            // 
            // graph1
            // 
            graphGroup1.Name = "categoryGroup";
            this.graph1.CategoryGroups.Add(graphGroup1);
            this.graph1.CoordinateSystems.Add(this.cartesianCoordinateSystem1);
            this.graph1.DataSource = this.SummaryGraphDS;
            this.graph1.Legend.Style.LineColor = System.Drawing.Color.LightGray;
            this.graph1.Legend.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.graph1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.graph1.Name = "graph1";
            this.graph1.PlotAreaStyle.LineColor = System.Drawing.Color.LightGray;
            this.graph1.PlotAreaStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.graph1.Series.Add(this.barSeries1);
            this.graph1.Series.Add(this.barSeries2);
            this.graph1.Series.Add(this.barSeries3);
            this.graph1.Series.Add(this.barSeries4);
            this.graph1.Series.Add(this.barSeries5);
            this.graph1.Series.Add(this.barSeries6);
            graphGroup7.ChildGroups.Add(graphGroup2);
            graphGroup7.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalTireCost"));
            graphGroup7.Name = "totalTireCostGroup";
            graphGroup7.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalTireCost", Telerik.Reporting.SortDirection.Asc));
            graphGroup6.ChildGroups.Add(graphGroup7);
            graphGroup6.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalOtherCost"));
            graphGroup6.Name = "totalOtherCostGroup";
            graphGroup6.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalOtherCost", Telerik.Reporting.SortDirection.Asc));
            graphGroup5.ChildGroups.Add(graphGroup6);
            graphGroup5.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalOilLubricantCost"));
            graphGroup5.Name = "totalOilLubricantCostGroup";
            graphGroup5.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalOilLubricantCost", Telerik.Reporting.SortDirection.Asc));
            graphGroup4.ChildGroups.Add(graphGroup5);
            graphGroup4.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalSalvageCost"));
            graphGroup4.Name = "totalSalvageCostGroup";
            graphGroup4.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalSalvageCost", Telerik.Reporting.SortDirection.Asc));
            graphGroup3.ChildGroups.Add(graphGroup4);
            graphGroup3.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalSpareCost"));
            graphGroup3.Name = "totalSpareCostGroup";
            graphGroup3.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalSpareCost", Telerik.Reporting.SortDirection.Asc));
            this.graph1.SeriesGroups.Add(graphGroup3);
            this.graph1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(3D));
            this.graph1.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            graphTitle1.Position = Telerik.Reporting.GraphItemPosition.TopCenter;
            graphTitle1.Style.LineColor = System.Drawing.Color.LightGray;
            graphTitle1.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            graphTitle1.Text = "graph1";
            this.graph1.Titles.Add(graphTitle1);
            // 
            // cartesianCoordinateSystem1
            // 
            this.cartesianCoordinateSystem1.Name = "cartesianCoordinateSystem1";
            this.cartesianCoordinateSystem1.XAxis = this.graphAxis2;
            this.cartesianCoordinateSystem1.YAxis = this.graphAxis1;
            // 
            // graphAxis1
            // 
            this.graphAxis1.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.Visible = false;
            this.graphAxis1.Name = "graphAxis1";
            this.graphAxis1.Scale = numericalScale1;
            // 
            // graphAxis2
            // 
            this.graphAxis2.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.Visible = false;
            this.graphAxis2.Name = "graphAxis2";
            this.graphAxis2.Scale = categoryScale1;
            // 
            // barSeries1
            // 
            this.barSeries1.CategoryGroup = graphGroup1;
            this.barSeries1.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries1.DataPointLabel = "= Sum(Fields.TotalTireCost)";
            this.barSeries1.DataPointLabelStyle.Visible = false;
            this.barSeries1.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries1.DataPointStyle.Visible = true;
            this.barSeries1.LegendItem.Value = resources.GetString("barSeries1.LegendItem.Value");
            this.barSeries1.Name = "barSeries1";
            graphGroup2.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalLabourCost"));
            graphGroup2.Name = "totalLabourCostGroup";
            graphGroup2.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.TotalLabourCost", Telerik.Reporting.SortDirection.Asc));
            this.barSeries1.SeriesGroup = graphGroup2;
            this.barSeries1.ToolTip.Text = "= Sum(Fields.TotalTireCost)";
            this.barSeries1.ToolTip.Title = resources.GetString("barSeries1.ToolTip.Title");
            this.barSeries1.Y = "= Sum(Fields.TotalTireCost)";
            // 
            // barSeries2
            // 
            this.barSeries2.CategoryGroup = graphGroup1;
            this.barSeries2.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries2.DataPointLabel = "= Sum(Fields.TotalSalvageCost)";
            this.barSeries2.DataPointLabelStyle.Visible = false;
            this.barSeries2.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries2.DataPointStyle.Visible = true;
            this.barSeries2.LegendItem.Value = resources.GetString("barSeries2.LegendItem.Value");
            this.barSeries2.Name = "barSeries2";
            this.barSeries2.SeriesGroup = graphGroup2;
            this.barSeries2.ToolTip.Text = "= Sum(Fields.TotalSalvageCost)";
            this.barSeries2.ToolTip.Title = resources.GetString("barSeries2.ToolTip.Title");
            this.barSeries2.Y = "= Sum(Fields.TotalSalvageCost)";
            // 
            // barSeries3
            // 
            this.barSeries3.CategoryGroup = graphGroup1;
            this.barSeries3.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries3.DataPointLabel = "= Sum(Fields.TotalOtherCost)";
            this.barSeries3.DataPointLabelStyle.Visible = false;
            this.barSeries3.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries3.DataPointStyle.Visible = true;
            this.barSeries3.LegendItem.Value = resources.GetString("barSeries3.LegendItem.Value");
            this.barSeries3.Name = "barSeries3";
            this.barSeries3.SeriesGroup = graphGroup2;
            this.barSeries3.ToolTip.Text = "= Sum(Fields.TotalOtherCost)";
            this.barSeries3.ToolTip.Title = resources.GetString("barSeries3.ToolTip.Title");
            this.barSeries3.Y = "= Sum(Fields.TotalOtherCost)";
            // 
            // barSeries4
            // 
            this.barSeries4.CategoryGroup = graphGroup1;
            this.barSeries4.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries4.DataPointLabel = "= Sum(Fields.TotalOilLubricantCost)";
            this.barSeries4.DataPointLabelStyle.Visible = false;
            this.barSeries4.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries4.DataPointStyle.Visible = true;
            this.barSeries4.LegendItem.Value = resources.GetString("barSeries4.LegendItem.Value");
            this.barSeries4.Name = "barSeries4";
            this.barSeries4.SeriesGroup = graphGroup2;
            this.barSeries4.ToolTip.Text = "= Sum(Fields.TotalOilLubricantCost)";
            this.barSeries4.ToolTip.Title = resources.GetString("barSeries4.ToolTip.Title");
            this.barSeries4.Y = "= Sum(Fields.TotalOilLubricantCost)";
            // 
            // barSeries5
            // 
            this.barSeries5.CategoryGroup = graphGroup1;
            this.barSeries5.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries5.DataPointLabel = "= Sum(Fields.TotalSpareCost)";
            this.barSeries5.DataPointLabelStyle.Visible = false;
            this.barSeries5.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries5.DataPointStyle.Visible = true;
            this.barSeries5.LegendItem.Value = resources.GetString("barSeries5.LegendItem.Value");
            this.barSeries5.Name = "barSeries5";
            this.barSeries5.SeriesGroup = graphGroup2;
            this.barSeries5.ToolTip.Text = "= Sum(Fields.TotalSpareCost)";
            this.barSeries5.ToolTip.Title = resources.GetString("barSeries5.ToolTip.Title");
            this.barSeries5.Y = "= Sum(Fields.TotalSpareCost)";
            // 
            // barSeries6
            // 
            this.barSeries6.CategoryGroup = graphGroup1;
            this.barSeries6.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.barSeries6.DataPointLabel = "= Sum(Fields.TotalLabourCost)";
            this.barSeries6.DataPointLabelStyle.Visible = false;
            this.barSeries6.DataPointStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Cm(0D);
            this.barSeries6.DataPointStyle.Visible = true;
            this.barSeries6.LegendItem.Value = resources.GetString("barSeries6.LegendItem.Value");
            this.barSeries6.Name = "barSeries6";
            this.barSeries6.SeriesGroup = graphGroup2;
            this.barSeries6.ToolTip.Text = "= Sum(Fields.TotalLabourCost)";
            this.barSeries6.ToolTip.Title = resources.GetString("barSeries6.ToolTip.Title");
            this.barSeries6.Y = "= Sum(Fields.TotalLabourCost)";
            // 
            // textBox1
            // 
            this.textBox1.Name = "ReportNameTextBox";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(15D), Telerik.Reporting.Drawing.Unit.Cm(2D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Name = "Segoe UI";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox1.Value = "Report2";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(11D), Telerik.Reporting.Drawing.Unit.Cm(1.5D));
            this.textBox2.Name = "ReportPageNumberTextBox";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.textBox2.Style.Font.Name = "Segoe UI";
            this.textBox2.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // Report2
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "Report2";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D), Telerik.Reporting.Drawing.Unit.Mm(25.399999618530273D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowBlank = false;
            reportParameter1.Name = "FiscalYear";
            reportParameter1.Text = "Fiscal Year";
            reportParameter1.Visible = true;
            reportParameter2.AllowBlank = false;
            reportParameter2.Name = "Period";
            reportParameter2.Text = "Period";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(15D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Graph graph1;
        private Telerik.Reporting.CartesianCoordinateSystem cartesianCoordinateSystem1;
        private Telerik.Reporting.GraphAxis graphAxis2;
        private Telerik.Reporting.GraphAxis graphAxis1;
        private Telerik.Reporting.ObjectDataSource SummaryGraphDS;
        private Telerik.Reporting.BarSeries barSeries1;
        private Telerik.Reporting.BarSeries barSeries2;
        private Telerik.Reporting.BarSeries barSeries3;
        private Telerik.Reporting.BarSeries barSeries4;
        private Telerik.Reporting.BarSeries barSeries5;
        private Telerik.Reporting.BarSeries barSeries6;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;

    }
}