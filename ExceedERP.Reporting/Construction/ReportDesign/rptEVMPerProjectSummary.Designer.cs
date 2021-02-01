namespace ExceedERP.Reporting.Construction.ReportDesign
{
    partial class rptEVMPerProjectSummary
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.GraphGroup graphGroup1 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.GraphTitle graphTitle1 = new Telerik.Reporting.GraphTitle();
            Telerik.Reporting.CategoryScale categoryScale1 = new Telerik.Reporting.CategoryScale();
            Telerik.Reporting.NumericalScale numericalScale1 = new Telerik.Reporting.NumericalScale();
            Telerik.Reporting.GraphGroup graphGroup2 = new Telerik.Reporting.GraphGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.graph1 = new Telerik.Reporting.Graph();
            this.cartesianCoordinateSystem1 = new Telerik.Reporting.CartesianCoordinateSystem();
            this.graphAxis2 = new Telerik.Reporting.GraphAxis();
            this.graphAxis1 = new Telerik.Reporting.GraphAxis();
            this.lineSeries1 = new Telerik.Reporting.LineSeries();
            this.lineSeries2 = new Telerik.Reporting.LineSeries();
            this.lineSeries3 = new Telerik.Reporting.LineSeries();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.pbLogo = new Telerik.Reporting.PictureBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.textBox62 = new Telerik.Reporting.TextBox();
            this.textBox64 = new Telerik.Reporting.TextBox();
            this.textBox66 = new Telerik.Reporting.TextBox();
            this.textBox65 = new Telerik.Reporting.TextBox();
            this.textBox68 = new Telerik.Reporting.TextBox();
            this.textBox67 = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.projectSummaryOds = new Telerik.Reporting.ObjectDataSource();
            this.availableProjects = new Telerik.Reporting.ObjectDataSource();
            this.headerOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(3D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.graph1});
            this.detailSection1.Name = "detailSection1";
            // 
            // graph1
            // 
            graphGroup1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.ConcatnatedMonth"));
            graphGroup1.Name = "concatnatedMonthGroup";
            graphGroup1.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.Order", Telerik.Reporting.SortDirection.Asc));
            this.graph1.CategoryGroups.Add(graphGroup1);
            this.graph1.CoordinateSystems.Add(this.cartesianCoordinateSystem1);
            this.graph1.DataSource = this.projectSummaryOds;
            this.graph1.Legend.Style.LineColor = System.Drawing.Color.LightGray;
            this.graph1.Legend.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.graph1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.graph1.Name = "graph1";
            this.graph1.PlotAreaStyle.LineColor = System.Drawing.Color.LightGray;
            this.graph1.PlotAreaStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.graph1.Series.Add(this.lineSeries1);
            this.graph1.Series.Add(this.lineSeries2);
            this.graph1.Series.Add(this.lineSeries3);
            this.graph1.SeriesGroups.Add(graphGroup2);
            this.graph1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(13.800000190734863D), Telerik.Reporting.Drawing.Unit.Inch(3D));
            this.graph1.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.Order", Telerik.Reporting.SortDirection.Asc));
            this.graph1.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.graph1.Style.Padding.Top = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            graphTitle1.Position = Telerik.Reporting.GraphItemPosition.TopCenter;
            graphTitle1.Style.Color = System.Drawing.Color.Teal;
            graphTitle1.Style.LineColor = System.Drawing.Color.LightGray;
            graphTitle1.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Inch(0D);
            graphTitle1.Text = "EVA Complete Graph";
            this.graph1.Titles.Add(graphTitle1);
            // 
            // cartesianCoordinateSystem1
            // 
            this.cartesianCoordinateSystem1.Name = "cartesianCoordinateSystem1";
            this.cartesianCoordinateSystem1.XAxis = this.graphAxis2;
            this.cartesianCoordinateSystem1.YAxis = this.graphAxis1;
            // 
            // graphAxis2
            // 
            this.graphAxis2.LabelAngle = -45;
            this.graphAxis2.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis2.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis2.MinorGridLineStyle.Visible = false;
            this.graphAxis2.Name = "graphAxis2";
            this.graphAxis2.Scale = categoryScale1;
            // 
            // graphAxis1
            // 
            this.graphAxis1.LabelFormat = "{0:N2}";
            this.graphAxis1.MajorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MajorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.LineColor = System.Drawing.Color.LightGray;
            this.graphAxis1.MinorGridLineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.graphAxis1.MinorGridLineStyle.Visible = false;
            this.graphAxis1.Name = "graphAxis1";
            this.graphAxis1.Scale = numericalScale1;
            // 
            // lineSeries1
            // 
            this.lineSeries1.CategoryGroup = graphGroup1;
            this.lineSeries1.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.lineSeries1.DataPointLabel = "= (Fields.BCWSCummulative)";
            this.lineSeries1.DataPointLabelStyle.Visible = false;
            this.lineSeries1.DataPointStyle.Visible = true;
            this.lineSeries1.LegendItem.Value = "\'BCWS\'";
            this.lineSeries1.LineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.lineSeries1.MarkerMaxSize = Telerik.Reporting.Drawing.Unit.Pixel(50D);
            this.lineSeries1.MarkerMinSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries1.MarkerSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries1.Name = "lineSeries1";
            graphGroup2.Name = "seriesGroup";
            this.lineSeries1.SeriesGroup = graphGroup2;
            this.lineSeries1.Size = null;
            this.lineSeries1.ToolTip.Text = "= (Fields.BCWSCummulative)";
            this.lineSeries1.ToolTip.Title = "\'BCWS\'";
            this.lineSeries1.Y = "= (Fields.BCWSCummulative)";
            // 
            // lineSeries2
            // 
            this.lineSeries2.CategoryGroup = graphGroup1;
            this.lineSeries2.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.lineSeries2.DataPointLabel = "= (Fields.BCWPCummulative)";
            this.lineSeries2.DataPointLabelStyle.Visible = false;
            this.lineSeries2.DataPointStyle.Visible = true;
            this.lineSeries2.LegendItem.Value = "\'BCWP\'";
            this.lineSeries2.LineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.lineSeries2.MarkerMaxSize = Telerik.Reporting.Drawing.Unit.Pixel(50D);
            this.lineSeries2.MarkerMinSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries2.MarkerSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries2.Name = "lineSeries2";
            this.lineSeries2.SeriesGroup = graphGroup2;
            this.lineSeries2.Size = null;
            this.lineSeries2.ToolTip.Text = "= (Fields.BCWPCummulative)";
            this.lineSeries2.ToolTip.Title = "\'BCWP\'";
            this.lineSeries2.Y = "= (Fields.BCWPCummulative)";
            // 
            // lineSeries3
            // 
            this.lineSeries3.CategoryGroup = graphGroup1;
            this.lineSeries3.CoordinateSystem = this.cartesianCoordinateSystem1;
            this.lineSeries3.DataPointLabel = "= (Fields.ACWPCummulative)";
            this.lineSeries3.DataPointLabelStyle.Visible = false;
            this.lineSeries3.DataPointStyle.Visible = true;
            this.lineSeries3.LegendItem.Value = "\'ACWP\'";
            this.lineSeries3.LineStyle.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.lineSeries3.MarkerMaxSize = Telerik.Reporting.Drawing.Unit.Pixel(50D);
            this.lineSeries3.MarkerMinSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries3.MarkerSize = Telerik.Reporting.Drawing.Unit.Pixel(5D);
            this.lineSeries3.Name = "lineSeries3";
            this.lineSeries3.SeriesGroup = graphGroup2;
            this.lineSeries3.Size = null;
            this.lineSeries3.ToolTip.Text = "= (Fields.ACWPCummulative)";
            this.lineSeries3.ToolTip.Title = "\'ACWP\'";
            this.lineSeries3.Y = "= (Fields.ACWPCummulative)";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.3999998569488525D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pbLogo,
            this.txtOrgName,
            this.textBox62,
            this.textBox64,
            this.textBox66,
            this.textBox65,
            this.textBox68,
            this.textBox67,
            this.shape3});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // pbLogo
            // 
            this.pbLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10208352655172348D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.pbLogo.MimeType = "";
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.pbLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbLogo.Value = "";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0854158401489258D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // textBox62
            // 
            this.textBox62.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.50624996423721313D));
            this.textBox62.Name = "textBox62";
            this.textBox62.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox62.Style.Font.Bold = true;
            this.textBox62.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox62.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox62.TextWrap = false;
            this.textBox62.Value = "= Fields.Name";
            // 
            // textBox64
            // 
            this.textBox64.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.90208333730697632D));
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox64.Style.Font.Bold = true;
            this.textBox64.Style.Font.Name = "Segoe UI";
            this.textBox64.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox64.Value = "EVA by Project Graphical Summary";
            // 
            // textBox66
            // 
            this.textBox66.Format = "{0:d}";
            this.textBox66.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.0952262878417969D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.textBox66.Name = "textBox66";
            this.textBox66.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox66.Value = "= Today()";
            // 
            // textBox65
            // 
            this.textBox65.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2202258110046387D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.textBox65.Name = "textBox65";
            this.textBox65.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox65.Style.Font.Bold = true;
            this.textBox65.Value = "Date:";
            // 
            // textBox68
            // 
            this.textBox68.Format = "{0:d}";
            this.textBox68.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.0952262878417969D), Telerik.Reporting.Drawing.Unit.Inch(1.7020834684371948D));
            this.textBox68.Name = "textBox68";
            this.textBox68.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox68.Value = "=Fields.ProjectName";
            // 
            // textBox67
            // 
            this.textBox67.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2202258110046387D), Telerik.Reporting.Drawing.Unit.Inch(1.7020834684371948D));
            this.textBox67.Name = "textBox67";
            this.textBox67.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox67.Style.Font.Bold = true;
            this.textBox67.Value = "Project:";
            // 
            // shape3
            // 
            this.shape3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(2.1000001430511475D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(13.799962043762207D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.425196647644043D), Telerik.Reporting.Drawing.Unit.Inch(0.60629922151565552D));
            this.textBox2.Name = "ReportPageNumberTextBox";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.39370077848434448D));
            this.textBox2.Style.Font.Name = "Segoe UI";
            this.textBox2.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // projectSummaryOds
            // 
            this.projectSummaryOds.DataMember = "GetProjectSummary";
            this.projectSummaryOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.EVMPerProjectGraphicalSummaryOds);
            this.projectSummaryOds.Name = "projectSummaryOds";
            this.projectSummaryOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectId.Value")});
            // 
            // availableProjects
            // 
            this.availableProjects.DataMember = "GetAvailableProjects";
            this.availableProjects.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.ProjectsOds);
            this.availableProjects.Name = "availableProjects";
            // 
            // headerOds
            // 
            this.headerOds.DataMember = "GetProjectHeader";
            this.headerOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.EVMPerProjectGraphicalSummaryOds);
            this.headerOds.Name = "headerOds";
            this.headerOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectId.Value")});
            // 
            // rptEVMPerProjectSummary
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "rptEVMPerProjectSummary";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.availableProjects;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.ProjectInformationId";
            reportParameter1.Name = "projectId";
            reportParameter1.Text = "Projects";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(13.90000057220459D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Graph graph1;
        private Telerik.Reporting.CartesianCoordinateSystem cartesianCoordinateSystem1;
        private Telerik.Reporting.GraphAxis graphAxis2;
        private Telerik.Reporting.GraphAxis graphAxis1;
        private Telerik.Reporting.ObjectDataSource projectSummaryOds;
        private Telerik.Reporting.LineSeries lineSeries1;
        private Telerik.Reporting.LineSeries lineSeries2;
        private Telerik.Reporting.LineSeries lineSeries3;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource availableProjects;
        private Telerik.Reporting.ObjectDataSource headerOds;
        private Telerik.Reporting.PictureBox pbLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox62;
        private Telerik.Reporting.TextBox textBox64;
        private Telerik.Reporting.TextBox textBox66;
        private Telerik.Reporting.TextBox textBox65;
        private Telerik.Reporting.TextBox textBox68;
        private Telerik.Reporting.TextBox textBox67;
        private Telerik.Reporting.Shape shape3;
    }
}