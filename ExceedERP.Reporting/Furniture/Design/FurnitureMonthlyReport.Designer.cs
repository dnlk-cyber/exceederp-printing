namespace ExceedERP.Reporting.Furniture.Design
{
    partial class FurnitureMonthlyReport
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.masterOds = new Telerik.Reporting.ObjectDataSource();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.table3 = new Telerik.Reporting.Table();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.parameterOds = new Telerik.Reporting.ObjectDataSource();
            this.sharedods = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.7999999523162842D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6,
            this.pbOrgLogo,
            this.txtOrgName,
            this.textBox55});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table2});
            this.detail.Name = "detail";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000049471855164D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // masterOds
            // 
            this.masterOds.DataMember = "GetMonthlyReport";
            this.masterOds.DataSource = typeof(ExceedERP.Reporting.Furniture.DataSource.FurnitureMonthlyReportOds);
            this.masterOds.Name = "masterOds";
            this.masterOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("from", typeof(System.DateTime), "= Parameters.from.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("to", typeof(System.DateTime), "= Parameters.to.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("customerId", typeof(string), "= Parameters.customerId.Value")});
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.70860558748245239D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.34999996423721313D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999995827674866D)));
            this.table2.Body.SetCellContent(0, 1, this.textBox7);
            this.table2.Body.SetCellContent(0, 0, this.textBox11);
            this.table2.Body.SetCellContent(0, 2, this.table3, 1, 3);
            this.table2.Body.SetCellContent(1, 3, this.textBox16);
            this.table2.Body.SetCellContent(1, 4, this.textBox17);
            this.table2.Body.SetCellContent(1, 0, this.textBox9, 1, 3);
            tableGroup6.Name = "group";
            tableGroup6.ReportItem = this.textBox10;
            tableGroup7.Name = "customerName";
            tableGroup7.ReportItem = this.textBox12;
            tableGroup8.Name = "jobType";
            tableGroup8.ReportItem = this.textBox14;
            tableGroup9.Name = "quantity";
            tableGroup9.ReportItem = this.textBox18;
            tableGroup10.Name = "totalCost";
            tableGroup10.ReportItem = this.textBox19;
            this.table2.ColumnGroups.Add(tableGroup6);
            this.table2.ColumnGroups.Add(tableGroup7);
            this.table2.ColumnGroups.Add(tableGroup8);
            this.table2.ColumnGroups.Add(tableGroup9);
            this.table2.ColumnGroups.Add(tableGroup10);
            this.table2.DataSource = this.masterOds;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.textBox7,
            this.table3,
            this.textBox9,
            this.textBox16,
            this.textBox17,
            this.textBox10,
            this.textBox12,
            this.textBox14,
            this.textBox18,
            this.textBox19});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0090737962163985E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.625D));
            this.table2.Name = "table2";
            tableGroup12.Name = "group1";
            tableGroup11.ChildGroups.Add(tableGroup12);
            tableGroup11.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup11.Name = "detail";
            tableGroup14.Name = "group4";
            tableGroup13.ChildGroups.Add(tableGroup14);
            tableGroup13.Name = "group3";
            this.table2.RowGroups.Add(tableGroup11);
            this.table2.RowGroups.Add(tableGroup13);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999599456787109D), Telerik.Reporting.Drawing.Unit.Inch(0.75D));
            this.table2.StyleName = "Normal.TableNormal";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.StyleName = "Normal.TableBody";
            this.textBox7.Value = "= Fields.CustomerName";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.70860540866851807D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.textBox11.StyleName = "Normal.TableBody";
            this.textBox11.Value = "=RowNumber(\"table1\")";
            // 
            // table3
            // 
            this.table3.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.PrintJobVms"));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4478384256362915D)));
            this.table3.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D)));
            this.table3.Body.SetCellContent(0, 0, this.textBox8);
            this.table3.Body.SetCellContent(0, 1, this.textBox13);
            this.table3.Body.SetCellContent(0, 2, this.textBox15);
            tableGroup1.Name = "tableGroup";
            tableGroup2.Name = "tableGroup1";
            tableGroup3.Name = "tableGroup2";
            this.table3.ColumnGroups.Add(tableGroup1);
            this.table3.ColumnGroups.Add(tableGroup2);
            this.table3.ColumnGroups.Add(tableGroup3);
            this.table3.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox8,
            this.textBox13,
            this.textBox15});
            this.table3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D));
            this.table3.Name = "table3";
            tableGroup5.Name = "group2";
            tableGroup4.ChildGroups.Add(tableGroup5);
            tableGroup4.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup4.Name = "detailTableGroup";
            this.table3.RowGroups.Add(tableGroup4);
            this.table3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.3435158729553223D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.table3.StyleName = "Normal.TableBody";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Value = "=Fields.JobType";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Value = "=Fields.Quantity";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.35000002384185791D));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Value = "=Fields.TotalCost";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox16.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox16.Style.Color = System.Drawing.Color.White;
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox16.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox16.StyleName = "Normal.TableBody";
            this.textBox16.Value = "=Sum(Fields.TotalQuantity)";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478386640548706D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox17.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox17.Style.Color = System.Drawing.Color.White;
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox17.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox17.StyleName = "Normal.TableBody";
            this.textBox17.Value = "=Sum(Fields.TotalCost)";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6042828559875488D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox9.Style.Color = System.Drawing.Color.White;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox9.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox9.StyleName = "Normal.TableBody";
            this.textBox9.Value = "SUM";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.70860540866851807D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox10.Style.Color = System.Drawing.Color.White;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.StyleName = "Normal.TableHeader";
            this.textBox10.Value = "S.No";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478387832641602D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox12.Style.Color = System.Drawing.Color.White;
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.StyleName = "Normal.TableHeader";
            this.textBox12.Value = "Customer Name";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478387832641602D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox14.Style.Color = System.Drawing.Color.White;
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.StyleName = "Normal.TableHeader";
            this.textBox14.Value = "Job Type";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478387832641602D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox18.Style.Color = System.Drawing.Color.White;
            this.textBox18.Style.Font.Bold = true;
            this.textBox18.StyleName = "Normal.TableHeader";
            this.textBox18.Value = "Quantity";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4478387832641602D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox19.Style.Color = System.Drawing.Color.White;
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.StyleName = "Normal.TableHeader";
            this.textBox19.Value = "Total Cost";
            // 
            // textBox55
            // 
            this.textBox55.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000202417373657D), Telerik.Reporting.Drawing.Unit.Inch(0.59999978542327881D));
            this.textBox55.Name = "textBox55";
            this.textBox55.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.0999212265014648D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox55.Style.Font.Bold = true;
            this.textBox55.Style.Font.Name = "Georgia";
            this.textBox55.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox55.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox55.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox55.TextWrap = false;
            this.textBox55.Value = "= Fields.Name";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000202417373657D), Telerik.Reporting.Drawing.Unit.Inch(0.2999609112739563D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.0999212265014648D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Georgia";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0599361960194074E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.10000030696392059D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999602794647217D), Telerik.Reporting.Drawing.Unit.Inch(1.1999996900558472D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbOrgLogo.Style.Font.Name = "Georgia";
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.9994057664880529E-05D), Telerik.Reporting.Drawing.Unit.Inch(1.4999995231628418D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.49992036819458D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.Value = "Monthly Report";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.5D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D), Telerik.Reporting.Drawing.Unit.Inch(0.19996045529842377D));
            this.textBox1.Style.Font.Name = "Segoe UI";
            this.textBox1.Value = "Page: {PageNumber}";
            // 
            // parameterOds
            // 
            this.parameterOds.DataMember = "GetCustomer";
            this.parameterOds.DataSource = typeof(ExceedERP.Reporting.Printing.DataSource.PrintParameterOds);
            this.parameterOds.Name = "parameterOds";
            // 
            // sharedods
            // 
            this.sharedods.DataMember = "GetOrganization";
            this.sharedods.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.sharedods.Name = "sharedods";
            // 
            // FurnitureMonthlyReport
            // 
            this.DataSource = this.sharedods;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "FurnitureMonthlyReport";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.Name = "from";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter1.Value = "=Today()";
            reportParameter1.Visible = true;
            reportParameter2.Name = "to";
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter2.Value = "=Today()";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.parameterOds;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Text";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Value";
            reportParameter3.Name = "customerId";
            reportParameter3.Text = "Customer Name";
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource masterOds;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.Table table3;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.ObjectDataSource parameterOds;
        private Telerik.Reporting.ObjectDataSource sharedods;
    }
}