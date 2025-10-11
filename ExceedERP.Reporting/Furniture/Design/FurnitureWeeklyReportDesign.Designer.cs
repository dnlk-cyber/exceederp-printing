namespace ExceedERP.Reporting.Furniture.Design
{
    partial class FurnitureWeeklyReportDesign
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
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.objectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox55 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.sharedods = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detailSection1.Name = "detailSection1";
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataMember = "GetReport";
            this.objectDataSource1.DataSource = typeof(ExceedERP.Reporting.Furniture.DataSource.FurnitureWeeklyReportds);
            this.objectDataSource1.Name = "objectDataSource1";
            this.objectDataSource1.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("from", typeof(System.DateTime), "= Parameters.from.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("to", typeof(System.DateTime), "= Parameters.to.Value")});
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.26666665077209473D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox7);
            this.table1.Body.SetCellContent(0, 1, this.textBox8);
            this.table1.Body.SetCellContent(0, 2, this.textBox9);
            this.table1.Body.SetCellContent(0, 3, this.textBox10);
            this.table1.Body.SetCellContent(0, 4, this.textBox11);
            this.table1.Body.SetCellContent(0, 5, this.textBox12);
            this.table1.Body.SetCellContent(1, 3, this.textBox17);
            this.table1.Body.SetCellContent(1, 4, this.textBox18);
            this.table1.Body.SetCellContent(1, 5, this.textBox19);
            this.table1.Body.SetCellContent(1, 0, this.textBox13, 1, 3);
            tableGroup1.Name = "jobNo";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "jobType";
            tableGroup2.ReportItem = this.textBox2;
            tableGroup3.Name = "uOM";
            tableGroup3.ReportItem = this.textBox3;
            tableGroup4.Name = "quantity";
            tableGroup4.ReportItem = this.textBox4;
            tableGroup5.Name = "unitPrice";
            tableGroup5.ReportItem = this.textBox5;
            tableGroup6.Name = "totalPrice";
            tableGroup6.ReportItem = this.textBox6;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.DataSource = this.objectDataSource1;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox8,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.textBox12,
            this.textBox13,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.textBox17,
            this.textBox18,
            this.textBox19});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.table1.Name = "table1";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detail";
            tableGroup8.Name = "group";
            this.table1.RowGroups.Add(tableGroup7);
            this.table1.RowGroups.Add(tableGroup8);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999599456787109D), Telerik.Reporting.Drawing.Unit.Inch(0.77499985694885254D));
            this.table1.StyleName = "Normal.TableNormal";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox1.Style.Color = System.Drawing.Color.White;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "Normal.TableHeader";
            this.textBox1.Value = "Job No";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox2.Style.Color = System.Drawing.Color.White;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.StyleName = "Normal.TableHeader";
            this.textBox2.Value = "Job Type";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox3.Style.Color = System.Drawing.Color.White;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.StyleName = "Normal.TableHeader";
            this.textBox3.Value = "UOM";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox4.Style.Color = System.Drawing.Color.White;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.StyleName = "Normal.TableHeader";
            this.textBox4.Value = "Quantity";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox5.Style.Color = System.Drawing.Color.White;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.StyleName = "Normal.TableHeader";
            this.textBox5.Value = "Unit Price";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833268165588379D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox6.Style.Color = System.Drawing.Color.White;
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.StyleName = "Normal.TableHeader";
            this.textBox6.Value = "Total Price";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox7.StyleName = "Normal.TableBody";
            this.textBox7.Value = "= Fields.JobNo";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox8.StyleName = "Normal.TableBody";
            this.textBox8.Value = "= Fields.JobType";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox9.StyleName = "Normal.TableBody";
            this.textBox9.Value = "= Fields.UOM";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox10.StyleName = "Normal.TableBody";
            this.textBox10.Value = "= Fields.Quantity";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox11.StyleName = "Normal.TableBody";
            this.textBox11.Value = "= Fields.UnitPrice";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.30833327770233154D));
            this.textBox12.StyleName = "Normal.TableBody";
            this.textBox12.Value = "= Fields.TotalPrice";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.8999999761581421D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox15,
            this.pbOrgLogo,
            this.txtOrgName,
            this.textBox55});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox14
            // 
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox14.Name = "ReportPageNumberTextBox";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4000004529953003D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox14.Style.Font.Name = "Segoe UI";
            this.textBox14.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000006258487701D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2499797344207764D), Telerik.Reporting.Drawing.Unit.Inch(0.26666665077209473D));
            this.textBox13.StyleName = "Normal.TableBody";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.26666665077209473D));
            this.textBox17.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox17.Style.Color = System.Drawing.Color.White;
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.StyleName = "Normal.TableBody";
            this.textBox17.Value = "=Sum(Fields.Quantity)";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.26666665077209473D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox18.Style.Color = System.Drawing.Color.White;
            this.textBox18.Style.Font.Bold = true;
            this.textBox18.StyleName = "Normal.TableBody";
            this.textBox18.Value = "=Sum(Fields.UnitPrice)";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0833265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.26666665077209473D));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox19.Style.Color = System.Drawing.Color.White;
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.StyleName = "Normal.TableBody";
            this.textBox19.Value = "=Sum(Fields.TotalPrice)";
            // 
            // textBox55
            // 
            this.textBox55.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000201225280762D), Telerik.Reporting.Drawing.Unit.Inch(0.69999992847442627D));
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
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000201225280762D), Telerik.Reporting.Drawing.Unit.Inch(0.39996102452278137D));
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
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0345050870673731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.10000018775463104D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999602794647217D), Telerik.Reporting.Drawing.Unit.Inch(1.1999996900558472D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbOrgLogo.Style.Font.Name = "Georgia";
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.9739744756370783E-05D), Telerik.Reporting.Drawing.Unit.Inch(1.5999996662139893D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.49992036819458D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox15.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox15.Value = "Monthly Report";
            // 
            // sharedods
            // 
            this.sharedods.DataMember = "GetOrganization";
            this.sharedods.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.sharedods.Name = "sharedods";
            // 
            // FurnitureWeeklyReportDesign
            // 
            this.DataSource = this.sharedods;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "FurnitureWeeklyReportDesign";
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
        private Telerik.Reporting.ObjectDataSource objectDataSource1;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox55;
        private Telerik.Reporting.ObjectDataSource sharedods;
    }
}