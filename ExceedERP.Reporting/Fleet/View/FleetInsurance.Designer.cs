namespace ExceedERP.Reporting.Fleet.View
{
    partial class FleetInsurance
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.fleetInsuranceOds = new Telerik.Reporting.ObjectDataSource();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.companyOds = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.headerSubReport1 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox1.StyleName = "Normal.TableHeader";
            this.textBox1.Value = "Plate No";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox2.StyleName = "Normal.TableHeader";
            this.textBox2.Value = "Insurance Company";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox3.StyleName = "Normal.TableHeader";
            this.textBox3.Value = "Premium Cost";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox4.StyleName = "Normal.TableHeader";
            this.textBox4.Value = "Due Date";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox5.StyleName = "Normal.TableHeader";
            this.textBox5.Value = "Expire Date";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.Silver;
            this.textBox6.StyleName = "Normal.TableHeader";
            this.textBox6.Value = "Description";
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detailSection1.Name = "detailSection1";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833266973495483D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox7);
            this.table1.Body.SetCellContent(0, 1, this.textBox8);
            this.table1.Body.SetCellContent(0, 2, this.textBox9);
            this.table1.Body.SetCellContent(0, 3, this.textBox10);
            this.table1.Body.SetCellContent(0, 4, this.textBox11);
            this.table1.Body.SetCellContent(0, 5, this.textBox12);
            tableGroup1.Name = "plateNo";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "insuranceCompany";
            tableGroup2.ReportItem = this.textBox2;
            tableGroup3.Name = "premiumCost";
            tableGroup3.ReportItem = this.textBox3;
            tableGroup4.Name = "dueDate";
            tableGroup4.ReportItem = this.textBox4;
            tableGroup5.Name = "expireDate";
            tableGroup5.ReportItem = this.textBox5;
            tableGroup6.Name = "description";
            tableGroup6.ReportItem = this.textBox6;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.DataSource = this.fleetInsuranceOds;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox8,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.textBox12,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.textBox6});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table1.Name = "table1";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detail";
            this.table1.RowGroups.Add(tableGroup7);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999604225158691D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.table1.StyleName = "Normal.TableNormal";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.StyleName = "Normal.TableBody";
            this.textBox7.Value = "= Fields.PlateNo";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.StyleName = "Normal.TableBody";
            this.textBox8.Value = "= Fields.InsuranceCompany";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.StyleName = "Normal.TableBody";
            this.textBox9.Value = "= Fields.PremiumCost";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.StyleName = "Normal.TableBody";
            this.textBox10.Value = "= Fields.DueDate";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox11.StyleName = "Normal.TableBody";
            this.textBox11.Value = "= Fields.ExpireDate";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.StyleName = "Normal.TableBody";
            this.textBox12.Value = "= Fields.Description";
            // 
            // fleetInsuranceOds
            // 
            this.fleetInsuranceOds.DataMember = "GetFleetInsurance";
            this.fleetInsuranceOds.DataSource = typeof(ExceedERP.Reporting.Fleet.DataSource.FleetInsuranceOds);
            this.fleetInsuranceOds.Name = "fleetInsuranceOds";
            // 
            // textBox14
            // 
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox14.Name = "ReportPageNumberTextBox";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19996057450771332D));
            this.textBox14.Style.Font.Name = "Segoe UI";
            this.textBox14.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // companyOds
            // 
            this.companyOds.DataMember = "GetOrganization";
            this.companyOds.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.companyOds.Name = "companyOds";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReport1;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            // 
            // headerSubReport1
            // 
            this.headerSubReport1.Name = "HeaderSubReport";
            // 
            // FleetInsurance
            // 
            this.DataSource = this.companyOds;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report6";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.Name = "FormTitle";
            reportParameter2.Name = "FormNumber";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Normal.TableNormal")});
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Tahoma";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Name = "Tahoma";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Normal.TableHeader")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule4.Style.Font.Name = "Tahoma";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.ObjectDataSource fleetInsuranceOds;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource companyOds;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.SubReport subReport1;
        private SharedDesigners.HeaderSubReport headerSubReport1;
    }
}