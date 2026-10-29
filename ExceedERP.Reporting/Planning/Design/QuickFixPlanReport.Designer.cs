namespace ExceedERP.Reporting.Planning.Design
{
    partial class QuickFixPlanReport
    {
        private void InitializeComponent()
        {
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.headerSubReportLandScape = new ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.header4 = new Telerik.Reporting.TextBox();
            this.header5 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.header6 = new Telerik.Reporting.TextBox();
            this.header7 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.fiscalYearDataSource = new Telerik.Reporting.ObjectDataSource();
            this.quarterDataSource = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection = new Telerik.Reporting.ReportHeaderSection();
            this.filterTextBox = new Telerik.Reporting.TextBox();
            this.subReport = new Telerik.Reporting.SubReport();
            this.detailSection = new Telerik.Reporting.DetailSection();
            this.quickFixPlanTable = new Telerik.Reporting.Table();
            this.detail0 = new Telerik.Reporting.TextBox();
            this.detail1 = new Telerik.Reporting.TextBox();
            this.detail2 = new Telerik.Reporting.TextBox();
            this.detail3 = new Telerik.Reporting.TextBox();
            this.detail4 = new Telerik.Reporting.TextBox();
            this.detail5 = new Telerik.Reporting.TextBox();
            this.detail6 = new Telerik.Reporting.TextBox();
            this.detail7 = new Telerik.Reporting.TextBox();
            this.quickFixPlanDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection = new Telerik.Reporting.PageFooterSection();
            this.pageNumberTextBox = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // headerSubReportLandScape
            // 
            this.headerSubReportLandScape.Name = "HeaderSubReportLandScape";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.49999991059303284D), Telerik.Reporting.Drawing.Unit.Inch(0.79666686058044434D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox1.Style.Color = System.Drawing.Color.White;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "S.N";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.79666686058044434D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox2.Style.Color = System.Drawing.Color.White;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "Major Activities";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.79666686058044434D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox3.Style.Color = System.Drawing.Color.White;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "UOM";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5999996662139893D), Telerik.Reporting.Drawing.Unit.Inch(0.79666686058044434D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox4.Style.Color = System.Drawing.Color.White;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.StyleName = "";
            this.textBox4.Value = "Amount";
            // 
            // header4
            // 
            this.header4.CanGrow = true;
            this.header4.Name = "header4";
            this.header4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.49666678905487061D));
            this.header4.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.header4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header4.Style.Color = System.Drawing.Color.White;
            this.header4.Style.Font.Bold = true;
            this.header4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.header4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header4.Value = "Plan Up To Quarter";
            // 
            // header5
            // 
            this.header5.CanGrow = true;
            this.header5.Name = "header5";
            this.header5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.49666678905487061D));
            this.header5.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.header5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header5.Style.Color = System.Drawing.Color.White;
            this.header5.Style.Font.Bold = true;
            this.header5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.header5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header5.Value = "Actual";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox5.Style.Color = System.Drawing.Color.White;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "Plan Performance Up To Selected Quarter";
            // 
            // header6
            // 
            this.header6.CanGrow = true;
            this.header6.Name = "header6";
            this.header6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9999996423721314D), Telerik.Reporting.Drawing.Unit.Inch(0.49666678905487061D));
            this.header6.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.header6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header6.Style.Color = System.Drawing.Color.White;
            this.header6.Style.Font.Bold = true;
            this.header6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.header6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header6.Value = "Previous Plan";
            // 
            // header7
            // 
            this.header7.CanGrow = true;
            this.header7.Name = "header7";
            this.header7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9999996423721314D), Telerik.Reporting.Drawing.Unit.Inch(0.49666678905487061D));
            this.header7.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.header7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header7.Style.Color = System.Drawing.Color.White;
            this.header7.Style.Font.Bold = true;
            this.header7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.header7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header7.Value = "Revised / Improved Plan";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.9999992847442627D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox7.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.textBox7.Style.Color = System.Drawing.Color.White;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.StyleName = "";
            this.textBox7.Value = "Plan for Remaining Quarter(s)";
            // 
            // fiscalYearDataSource
            // 
            this.fiscalYearDataSource.DataMember = "GetFiscalYear";
            this.fiscalYearDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.FiscalYearODS);
            this.fiscalYearDataSource.Name = "fiscalYearDataSource";
            // 
            // quarterDataSource
            // 
            this.quarterDataSource.DataMember = "GetQuarters";
            this.quarterDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.QuickFixPlanODS);
            this.quarterDataSource.Name = "quarterDataSource";
            // 
            // reportHeaderSection
            // 
            this.reportHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(2.7999999523162842D);
            this.reportHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.filterTextBox,
            this.subReport});
            this.reportHeaderSection.Name = "reportHeaderSection";
            // 
            // filterTextBox
            // 
            this.filterTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(8.0362959124613553E-05D), Telerik.Reporting.Drawing.Unit.Inch(2.0399999618530273D));
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(14.800000190734863D), Telerik.Reporting.Drawing.Unit.Inch(0.31999999284744263D));
            this.filterTextBox.Style.Font.Bold = true;
            this.filterTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.filterTextBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.filterTextBox.Value = "Fiscal Year: {Parameters.FiscalYear.Label}     Up To: {Parameters.UpToQuarter.Lab" +
    "el}";
            // 
            // subReport
            // 
            this.subReport.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.0099212648347020149D));
            this.subReport.Name = "subReport";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReportLandScape;
            this.subReport.ReportSource = instanceReportSource1;
            this.subReport.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(2.0299999713897705D));
            // 
            // detailSection
            // 
            this.detailSection.Height = Telerik.Reporting.Drawing.Unit.Inch(1.5199217796325684D);
            this.detailSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.quickFixPlanTable});
            this.detailSection.Name = "detailSection";
            // 
            // quickFixPlanTable
            // 
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.49999991059303284D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.1000003814697266D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5999995470046997D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.9999996423721314D)));
            this.quickFixPlanTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.9999996423721314D)));
            this.quickFixPlanTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.37999996542930603D)));
            this.quickFixPlanTable.Body.SetCellContent(0, 0, this.detail0);
            this.quickFixPlanTable.Body.SetCellContent(0, 1, this.detail1);
            this.quickFixPlanTable.Body.SetCellContent(0, 2, this.detail2);
            this.quickFixPlanTable.Body.SetCellContent(0, 3, this.detail3);
            this.quickFixPlanTable.Body.SetCellContent(0, 4, this.detail4);
            this.quickFixPlanTable.Body.SetCellContent(0, 5, this.detail5);
            this.quickFixPlanTable.Body.SetCellContent(0, 6, this.detail6);
            this.quickFixPlanTable.Body.SetCellContent(0, 7, this.detail7);
            tableGroup1.Name = "columnGroup0";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "columnGroup1";
            tableGroup2.ReportItem = this.textBox2;
            tableGroup3.Name = "columnGroup2";
            tableGroup3.ReportItem = this.textBox3;
            tableGroup4.Name = "columnGroup3";
            tableGroup4.ReportItem = this.textBox4;
            tableGroup6.Name = "group4";
            tableGroup6.ReportItem = this.header4;
            tableGroup7.Name = "group5";
            tableGroup7.ReportItem = this.header5;
            tableGroup5.ChildGroups.Add(tableGroup6);
            tableGroup5.ChildGroups.Add(tableGroup7);
            tableGroup5.Name = "columnGroup4";
            tableGroup5.ReportItem = this.textBox5;
            tableGroup9.Name = "group6";
            tableGroup9.ReportItem = this.header6;
            tableGroup10.Name = "group7";
            tableGroup10.ReportItem = this.header7;
            tableGroup8.ChildGroups.Add(tableGroup9);
            tableGroup8.ChildGroups.Add(tableGroup10);
            tableGroup8.Name = "columnGroup6";
            tableGroup8.ReportItem = this.textBox7;
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup1);
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup2);
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup3);
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup4);
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup5);
            this.quickFixPlanTable.ColumnGroups.Add(tableGroup8);
            this.quickFixPlanTable.DataSource = this.quickFixPlanDataSource;
            this.quickFixPlanTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail0,
            this.detail1,
            this.detail2,
            this.detail3,
            this.detail4,
            this.detail5,
            this.detail6,
            this.detail7,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.header4,
            this.header5,
            this.textBox7,
            this.header6,
            this.header7});
            this.quickFixPlanTable.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9164224290288985E-05D));
            this.quickFixPlanTable.Name = "quickFixPlanTable";
            tableGroup11.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup11.Name = "detailGroup";
            this.quickFixPlanTable.RowGroups.Add(tableGroup11);
            this.quickFixPlanTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(14.799999237060547D), Telerik.Reporting.Drawing.Unit.Inch(1.1766668558120728D));
            // 
            // detail0
            // 
            this.detail0.CanGrow = true;
            this.detail0.Name = "detail0";
            this.detail0.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail0.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail0.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail0.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail0.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.detail0.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail0.Value = "= Fields.SerialNumber";
            // 
            // detail1
            // 
            this.detail1.CanGrow = true;
            this.detail1.Name = "detail1";
            this.detail1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail1.Value = "= Fields.MajorActivity";
            // 
            // detail2
            // 
            this.detail2.CanGrow = true;
            this.detail2.Name = "detail2";
            this.detail2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.detail2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail2.Value = "= Fields.Unit";
            // 
            // detail3
            // 
            this.detail3.CanGrow = true;
            this.detail3.Format = "{0:N2}";
            this.detail3.Name = "detail3";
            this.detail3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail3.Value = "= Fields.AnnualPlan";
            // 
            // detail4
            // 
            this.detail4.CanGrow = true;
            this.detail4.Format = "{0:N2}";
            this.detail4.Name = "detail4";
            this.detail4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail4.Value = "= Fields.PlanUpToQuarter";
            // 
            // detail5
            // 
            this.detail5.CanGrow = true;
            this.detail5.Format = "{0:N2}";
            this.detail5.Name = "detail5";
            this.detail5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail5.Value = "= Fields.Actual";
            // 
            // detail6
            // 
            this.detail6.CanGrow = true;
            this.detail6.Format = "{0:N2}";
            this.detail6.Name = "detail6";
            this.detail6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail6.Value = "= Fields.PreviousPlanRemainingQuarter";
            // 
            // detail7
            // 
            this.detail7.CanGrow = true;
            this.detail7.Format = "{0:N2}";
            this.detail7.Name = "detail7";
            this.detail7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.detail7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail7.Value = "= Fields.RevisedPlanRemainingQuarter";
            // 
            // quickFixPlanDataSource
            // 
            this.quickFixPlanDataSource.DataMember = "GetQuickFixPlans";
            this.quickFixPlanDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.QuickFixPlanODS);
            this.quickFixPlanDataSource.Name = "quickFixPlanDataSource";
            this.quickFixPlanDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("fiscalYearId", typeof(string), "= Parameters.FiscalYear.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("upToQuarter", typeof(string), "= Parameters.UpToQuarter.Value")});
            // 
            // pageFooterSection
            // 
            this.pageFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25D);
            this.pageFooterSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageNumberTextBox});
            this.pageFooterSection.Name = "pageFooterSection";
            // 
            // pageNumberTextBox
            // 
            this.pageNumberTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(12.800000190734863D), Telerik.Reporting.Drawing.Unit.Inch(0.019999999552965164D));
            this.pageNumberTextBox.Name = "pageNumberTextBox";
            this.pageNumberTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.2199999988079071D));
            this.pageNumberTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageNumberTextBox.Value = "Page: {PageNumber} of {PageCount}";
            // 
            // QuickFixPlanReport
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.reportHeaderSection,
            this.detailSection,
            this.pageFooterSection});
            this.Name = "QuickFixPlanReport";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.fiscalYearDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            reportParameter1.Name = "FiscalYear";
            reportParameter1.Text = "Fiscal Year";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.quarterDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Text";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Value";
            reportParameter2.Name = "UpToQuarter";
            reportParameter2.Text = "Up To Quarter";
            reportParameter2.Visible = true;
            reportParameter3.Name = "FormTitle";
            reportParameter3.Value = "Quick-Fix Plan";
            reportParameter4.Name = "FormNumber";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(15.199999809265137D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        private Telerik.Reporting.ObjectDataSource fiscalYearDataSource;
        private Telerik.Reporting.ObjectDataSource quarterDataSource;
        private Telerik.Reporting.ObjectDataSource quickFixPlanDataSource;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection;
        private Telerik.Reporting.TextBox filterTextBox;
        private Telerik.Reporting.DetailSection detailSection;
        private Telerik.Reporting.Table quickFixPlanTable;
        private Telerik.Reporting.TextBox detail0;
        private Telerik.Reporting.TextBox detail1;
        private Telerik.Reporting.TextBox detail2;
        private Telerik.Reporting.TextBox detail3;
        private Telerik.Reporting.TextBox detail4;
        private Telerik.Reporting.TextBox detail5;
        private Telerik.Reporting.TextBox detail6;
        private Telerik.Reporting.TextBox detail7;
        private Telerik.Reporting.TextBox header4;
        private Telerik.Reporting.TextBox header5;
        private Telerik.Reporting.TextBox header7;
        private Telerik.Reporting.PageFooterSection pageFooterSection;
        private Telerik.Reporting.TextBox pageNumberTextBox;
        private Telerik.Reporting.SubReport subReport;
        private Telerik.Reporting.TextBox header6;
        private ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape headerSubReportLandScape;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox7;
    }
}
