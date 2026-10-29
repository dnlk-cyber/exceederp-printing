namespace ExceedERP.Reporting.Planning.Design
{
    partial class QuarterlyPlanReport
    {
        private void InitializeComponent()
        {
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
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup15 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup16 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.headerSubReportLandScape = new ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape();
            this.fiscalYearObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.workUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.header0 = new Telerik.Reporting.TextBox();
            this.header1 = new Telerik.Reporting.TextBox();
            this.header2 = new Telerik.Reporting.TextBox();
            this.header3 = new Telerik.Reporting.TextBox();
            this.header4 = new Telerik.Reporting.TextBox();
            this.header5 = new Telerik.Reporting.TextBox();
            this.header6 = new Telerik.Reporting.TextBox();
            this.header7 = new Telerik.Reporting.TextBox();
            this.header8 = new Telerik.Reporting.TextBox();
            this.header9 = new Telerik.Reporting.TextBox();
            this.header10 = new Telerik.Reporting.TextBox();
            this.header11 = new Telerik.Reporting.TextBox();
            this.header12 = new Telerik.Reporting.TextBox();
            this.header13 = new Telerik.Reporting.TextBox();
            this.header14 = new Telerik.Reporting.TextBox();
            this.quarterlyPlanObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.detail0 = new Telerik.Reporting.TextBox();
            this.detail1 = new Telerik.Reporting.TextBox();
            this.detail2 = new Telerik.Reporting.TextBox();
            this.detail3 = new Telerik.Reporting.TextBox();
            this.detail4 = new Telerik.Reporting.TextBox();
            this.detail5 = new Telerik.Reporting.TextBox();
            this.detail6 = new Telerik.Reporting.TextBox();
            this.detail7 = new Telerik.Reporting.TextBox();
            this.detail8 = new Telerik.Reporting.TextBox();
            this.detail9 = new Telerik.Reporting.TextBox();
            this.detail10 = new Telerik.Reporting.TextBox();
            this.detail11 = new Telerik.Reporting.TextBox();
            this.detail12 = new Telerik.Reporting.TextBox();
            this.detail13 = new Telerik.Reporting.TextBox();
            this.detail14 = new Telerik.Reporting.TextBox();
            this.reportHeaderSection = new Telerik.Reporting.ReportHeaderSection();
            this.subReport = new Telerik.Reporting.SubReport();
            this.filterTextBox = new Telerik.Reporting.TextBox();
            this.pageFooterSection = new Telerik.Reporting.PageFooterSection();
            this.pageNumber = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReportLandScape)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // headerSubReportLandScape
            // 
            this.headerSubReportLandScape.Name = "HeaderSubReportLandScape";
            // 
            // fiscalYearObjectDataSource
            // 
            this.fiscalYearObjectDataSource.DataMember = "GetFiscalYear";
            this.fiscalYearObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.FiscalYearODS);
            this.fiscalYearObjectDataSource.Name = "fiscalYearObjectDataSource";
            // 
            // workUnitObjectDataSource
            // 
            this.workUnitObjectDataSource.DataMember = "GetWorkUnit";
            this.workUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.WorkUnitODS);
            this.workUnitObjectDataSource.Name = "workUnitObjectDataSource";
            // 
            // header0
            // 
            this.header0.CanGrow = true;
            this.header0.Name = "header0";
            this.header0.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header0.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header0.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header0.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header0.Style.Font.Bold = true;
            this.header0.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header0.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header0.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header0.Value = "S.N";
            // 
            // header1
            // 
            this.header1.CanGrow = true;
            this.header1.Name = "header1";
            this.header1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header1.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header1.Style.Font.Bold = true;
            this.header1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header1.Value = "Work Unit";
            // 
            // header2
            // 
            this.header2.CanGrow = true;
            this.header2.Name = "header2";
            this.header2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header2.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header2.Style.Font.Bold = true;
            this.header2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header2.Value = "Objective";
            // 
            // header3
            // 
            this.header3.CanGrow = true;
            this.header3.Name = "header3";
            this.header3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header3.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header3.Style.Font.Bold = true;
            this.header3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header3.Value = "Indicator";
            // 
            // header4
            // 
            this.header4.CanGrow = true;
            this.header4.Name = "header4";
            this.header4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header4.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header4.Style.Font.Bold = true;
            this.header4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header4.Value = "Sub Indicator";
            // 
            // header5
            // 
            this.header5.CanGrow = true;
            this.header5.Name = "header5";
            this.header5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header5.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header5.Style.Font.Bold = true;
            this.header5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header5.Value = "Description";
            // 
            // header6
            // 
            this.header6.CanGrow = true;
            this.header6.Name = "header6";
            this.header6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.60000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header6.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header6.Style.Font.Bold = true;
            this.header6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header6.Value = "Unit";
            // 
            // header7
            // 
            this.header7.CanGrow = true;
            this.header7.Name = "header7";
            this.header7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header7.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header7.Style.Font.Bold = true;
            this.header7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header7.Value = "KPI Type";
            // 
            // header8
            // 
            this.header8.CanGrow = true;
            this.header8.Name = "header8";
            this.header8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header8.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header8.Style.Font.Bold = true;
            this.header8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header8.Value = "Baseline";
            // 
            // header9
            // 
            this.header9.CanGrow = true;
            this.header9.Name = "header9";
            this.header9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header9.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header9.Style.Font.Bold = true;
            this.header9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header9.Value = "Annual Plan";
            // 
            // header10
            // 
            this.header10.CanGrow = true;
            this.header10.Name = "header10";
            this.header10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header10.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header10.Style.Font.Bold = true;
            this.header10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header10.Value = "Target";
            // 
            // header11
            // 
            this.header11.CanGrow = true;
            this.header11.Name = "header11";
            this.header11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header11.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header11.Style.Font.Bold = true;
            this.header11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header11.Value = "Q1 (1-3)";
            // 
            // header12
            // 
            this.header12.CanGrow = true;
            this.header12.Name = "header12";
            this.header12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header12.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header12.Style.Font.Bold = true;
            this.header12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header12.Value = "Q2 (4-6)";
            // 
            // header13
            // 
            this.header13.CanGrow = true;
            this.header13.Name = "header13";
            this.header13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header13.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header13.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header13.Style.Font.Bold = true;
            this.header13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header13.Value = "Q3 (7-9)";
            // 
            // header14
            // 
            this.header14.CanGrow = true;
            this.header14.Name = "header14";
            this.header14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.51999998092651367D));
            this.header14.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.header14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.header14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.header14.Style.Font.Bold = true;
            this.header14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.header14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.header14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.header14.Value = "Q4 (10-12)";
            // 
            // quarterlyPlanObjectDataSource
            // 
            this.quarterlyPlanObjectDataSource.DataMember = "GetQuarterlyPlan";
            this.quarterlyPlanObjectDataSource.DataSource = typeof(ExceedERP.Reporting.Planning.DataSource.QuarterlyPlanODS);
            this.quarterlyPlanObjectDataSource.Name = "quarterlyPlanObjectDataSource";
            this.quarterlyPlanObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("planyear", typeof(object[]), "= Parameters.PlanFiscalYear.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("workUnitId", typeof(string), "= Parameters.WorkUnit.Value")});
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.89999997615814209D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detail.Name = "detail";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6999999284744263D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.59999996423721313D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.37999996542930603D)));
            this.table1.Body.SetCellContent(0, 0, this.detail0);
            this.table1.Body.SetCellContent(0, 1, this.detail1);
            this.table1.Body.SetCellContent(0, 2, this.detail2);
            this.table1.Body.SetCellContent(0, 3, this.detail3);
            this.table1.Body.SetCellContent(0, 4, this.detail4);
            this.table1.Body.SetCellContent(0, 5, this.detail5);
            this.table1.Body.SetCellContent(0, 6, this.detail6);
            this.table1.Body.SetCellContent(0, 7, this.detail7);
            this.table1.Body.SetCellContent(0, 8, this.detail8);
            this.table1.Body.SetCellContent(0, 9, this.detail9);
            this.table1.Body.SetCellContent(0, 10, this.detail10);
            this.table1.Body.SetCellContent(0, 11, this.detail11);
            this.table1.Body.SetCellContent(0, 12, this.detail12);
            this.table1.Body.SetCellContent(0, 13, this.detail13);
            this.table1.Body.SetCellContent(0, 14, this.detail14);
            tableGroup1.Name = "columnGroup1";
            tableGroup1.ReportItem = this.header0;
            tableGroup2.Name = "columnGroup2";
            tableGroup2.ReportItem = this.header1;
            tableGroup3.Name = "columnGroup3";
            tableGroup3.ReportItem = this.header2;
            tableGroup4.Name = "columnGroup4";
            tableGroup4.ReportItem = this.header3;
            tableGroup5.Name = "columnGroup5";
            tableGroup5.ReportItem = this.header4;
            tableGroup6.Name = "columnGroup6";
            tableGroup6.ReportItem = this.header5;
            tableGroup7.Name = "columnGroup7";
            tableGroup7.ReportItem = this.header6;
            tableGroup8.Name = "columnGroup8";
            tableGroup8.ReportItem = this.header7;
            tableGroup9.Name = "columnGroup9";
            tableGroup9.ReportItem = this.header8;
            tableGroup10.Name = "columnGroup10";
            tableGroup10.ReportItem = this.header9;
            tableGroup11.Name = "columnGroup11";
            tableGroup11.ReportItem = this.header10;
            tableGroup12.Name = "columnGroup12";
            tableGroup12.ReportItem = this.header11;
            tableGroup13.Name = "columnGroup13";
            tableGroup13.ReportItem = this.header12;
            tableGroup14.Name = "columnGroup14";
            tableGroup14.ReportItem = this.header13;
            tableGroup15.Name = "columnGroup15";
            tableGroup15.ReportItem = this.header14;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.ColumnGroups.Add(tableGroup8);
            this.table1.ColumnGroups.Add(tableGroup9);
            this.table1.ColumnGroups.Add(tableGroup10);
            this.table1.ColumnGroups.Add(tableGroup11);
            this.table1.ColumnGroups.Add(tableGroup12);
            this.table1.ColumnGroups.Add(tableGroup13);
            this.table1.ColumnGroups.Add(tableGroup14);
            this.table1.ColumnGroups.Add(tableGroup15);
            this.table1.DataSource = this.quarterlyPlanObjectDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail0,
            this.detail1,
            this.detail2,
            this.detail3,
            this.detail4,
            this.detail5,
            this.detail6,
            this.detail7,
            this.detail8,
            this.detail9,
            this.detail10,
            this.detail11,
            this.detail12,
            this.detail13,
            this.detail14,
            this.header0,
            this.header1,
            this.header2,
            this.header3,
            this.header4,
            this.header5,
            this.header6,
            this.header7,
            this.header8,
            this.header9,
            this.header10,
            this.header11,
            this.header12,
            this.header13,
            this.header14});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table1.Name = "table1";
            tableGroup16.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup16.Name = "detail";
            this.table1.RowGroups.Add(tableGroup16);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(0.89999997615814209D));
            this.table1.StyleName = "Normal.TableNormal";
            // 
            // detail0
            // 
            this.detail0.CanGrow = true;
            this.detail0.Name = "detail0";
            this.detail0.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail0.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail0.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail0.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail0.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail0.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail0.Value = "= Fields.SerialNumber";
            // 
            // detail1
            // 
            this.detail1.CanGrow = true;
            this.detail1.Name = "detail1";
            this.detail1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail1.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail1.Value = "= Fields.WorkUnit";
            // 
            // detail2
            // 
            this.detail2.CanGrow = true;
            this.detail2.Name = "detail2";
            this.detail2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail2.Value = "= Fields.Objective";
            // 
            // detail3
            // 
            this.detail3.CanGrow = true;
            this.detail3.Name = "detail3";
            this.detail3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail3.Value = "= Fields.Indicator";
            // 
            // detail4
            // 
            this.detail4.CanGrow = true;
            this.detail4.Name = "detail4";
            this.detail4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail4.Value = "= Fields.SubIndicator";
            // 
            // detail5
            // 
            this.detail5.CanGrow = true;
            this.detail5.Name = "detail5";
            this.detail5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail5.Value = "= Fields.Description";
            // 
            // detail6
            // 
            this.detail6.CanGrow = true;
            this.detail6.Name = "detail6";
            this.detail6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.60000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail6.Value = "= Fields.Unit";
            // 
            // detail7
            // 
            this.detail7.CanGrow = true;
            this.detail7.Name = "detail7";
            this.detail7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.detail7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail7.Value = "= Fields.KPIType";
            // 
            // detail8
            // 
            this.detail8.CanGrow = true;
            this.detail8.Format = "{0:N2}";
            this.detail8.Name = "detail8";
            this.detail8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail8.Value = "= Fields.Baseline";
            // 
            // detail9
            // 
            this.detail9.CanGrow = true;
            this.detail9.Format = "{0:N2}";
            this.detail9.Name = "detail9";
            this.detail9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail9.Value = "= Fields.AnnualPlan";
            // 
            // detail10
            // 
            this.detail10.CanGrow = true;
            this.detail10.Format = "{0:N2}";
            this.detail10.Name = "detail10";
            this.detail10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail10.Value = "= Fields.Target";
            // 
            // detail11
            // 
            this.detail11.CanGrow = true;
            this.detail11.Format = "{0:N2}";
            this.detail11.Name = "detail11";
            this.detail11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail11.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail11.Value = "= Fields.QuarterOne";
            // 
            // detail12
            // 
            this.detail12.CanGrow = true;
            this.detail12.Format = "{0:N2}";
            this.detail12.Name = "detail12";
            this.detail12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail12.Value = "= Fields.QuarterTwo";
            // 
            // detail13
            // 
            this.detail13.CanGrow = true;
            this.detail13.Format = "{0:N2}";
            this.detail13.Name = "detail13";
            this.detail13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail13.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail13.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail13.Value = "= Fields.QuarterThree";
            // 
            // detail14
            // 
            this.detail14.CanGrow = true;
            this.detail14.Format = "{0:N2}";
            this.detail14.Name = "detail14";
            this.detail14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.37999999523162842D));
            this.detail14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail14.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.detail14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.detail14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.detail14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.detail14.Value = "= Fields.QuarterFour";
            // 
            // reportHeaderSection
            // 
            this.reportHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3200000524520874D);
            this.reportHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport,
            this.filterTextBox});
            this.reportHeaderSection.Name = "reportHeaderSection";
            // 
            // subReport
            // 
            this.subReport.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport.Name = "subReport";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReportLandScape;
            this.subReport.ReportSource = instanceReportSource1;
            this.subReport.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(1.0199999809265137D));
            // 
            // filterTextBox
            // 
            this.filterTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.0299999713897705D));
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(15.199999809265137D), Telerik.Reporting.Drawing.Unit.Inch(0.2800000011920929D));
            this.filterTextBox.Style.Font.Bold = true;
            this.filterTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.filterTextBox.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.filterTextBox.Value = "Fiscal Year: {Join(\", \", Parameters.PlanFiscalYear.Label)}     Work Unit: {Parameters.WorkUni" +
    "t.Label}";
            // 
            // pageFooterSection
            // 
            this.pageFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.25D);
            this.pageFooterSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageNumber});
            this.pageFooterSection.Name = "pageFooterSection";
            // 
            // pageNumber
            // 
            this.pageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(13.699999809265137D), Telerik.Reporting.Drawing.Unit.Inch(0.019999999552965164D));
            this.pageNumber.Name = "pageNumber";
            this.pageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.pageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageNumber.Value = "Page: {PageNumber} of {PageCount}";
            // 
            // QuarterlyPlanReport
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.reportHeaderSection,
            this.detail,
            this.pageFooterSection});
            this.Name = "QuarterlyPlanReport";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A3;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.fiscalYearObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            reportParameter1.MultiValue = true;
            reportParameter1.Name = "PlanFiscalYear";
            reportParameter1.Text = "Planning Year";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.workUnitObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.Name = "WorkUnit";
            reportParameter2.Text = "Work Unit";
            reportParameter2.Visible = true;
            reportParameter3.Name = "FormTitle";
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

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection;
        private Telerik.Reporting.PageFooterSection pageFooterSection;
        private Telerik.Reporting.SubReport subReport;
        private ExceedERP.Reporting.SharedDesigners.HeaderSubReportLandScape headerSubReportLandScape;
        private Telerik.Reporting.ObjectDataSource quarterlyPlanObjectDataSource;
        private Telerik.Reporting.ObjectDataSource fiscalYearObjectDataSource;
        private Telerik.Reporting.ObjectDataSource workUnitObjectDataSource;
        private Telerik.Reporting.TextBox filterTextBox;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox pageNumber;
        private Telerik.Reporting.TextBox header0;
        private Telerik.Reporting.TextBox header1;
        private Telerik.Reporting.TextBox header2;
        private Telerik.Reporting.TextBox header3;
        private Telerik.Reporting.TextBox header4;
        private Telerik.Reporting.TextBox header5;
        private Telerik.Reporting.TextBox header6;
        private Telerik.Reporting.TextBox header7;
        private Telerik.Reporting.TextBox header8;
        private Telerik.Reporting.TextBox header9;
        private Telerik.Reporting.TextBox header10;
        private Telerik.Reporting.TextBox header11;
        private Telerik.Reporting.TextBox header12;
        private Telerik.Reporting.TextBox header13;
        private Telerik.Reporting.TextBox header14;
        private Telerik.Reporting.TextBox detail0;
        private Telerik.Reporting.TextBox detail1;
        private Telerik.Reporting.TextBox detail2;
        private Telerik.Reporting.TextBox detail3;
        private Telerik.Reporting.TextBox detail4;
        private Telerik.Reporting.TextBox detail5;
        private Telerik.Reporting.TextBox detail6;
        private Telerik.Reporting.TextBox detail7;
        private Telerik.Reporting.TextBox detail8;
        private Telerik.Reporting.TextBox detail9;
        private Telerik.Reporting.TextBox detail10;
        private Telerik.Reporting.TextBox detail11;
        private Telerik.Reporting.TextBox detail12;
        private Telerik.Reporting.TextBox detail13;
        private Telerik.Reporting.TextBox detail14;
    }
}
