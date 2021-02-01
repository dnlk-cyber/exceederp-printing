namespace ExceedERP.Reporting.Construction.ReportDesign
{
    partial class rptBoQSummary
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.mainTable = new Telerik.Reporting.Table();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.structuresTable = new Telerik.Reporting.Table();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.workCategoriesTable = new Telerik.Reporting.Table();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
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
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.boqSummaryOds = new Telerik.Reporting.ObjectDataSource();
            this.availableProjects = new Telerik.Reporting.ObjectDataSource();
            this.subProjectTypesOds = new Telerik.Reporting.ObjectDataSource();
            this.projectHeaderOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.5D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.mainTable});
            this.detailSection1.Name = "detailSection1";
            // 
            // mainTable
            // 
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.34672954678535461D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.62037897109985352D)));
            this.mainTable.Body.SetCellContent(0, 0, this.textBox3);
            this.mainTable.Body.SetCellContent(1, 0, this.structuresTable);
            tableGroup11.Name = "name";
            this.mainTable.ColumnGroups.Add(tableGroup11);
            this.mainTable.DataSource = this.boqSummaryOds;
            this.mainTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3,
            this.structuresTable});
            this.mainTable.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.mainTable.Name = "mainTable";
            tableGroup13.Name = "group";
            tableGroup14.Name = "group1";
            tableGroup12.ChildGroups.Add(tableGroup13);
            tableGroup12.ChildGroups.Add(tableGroup14);
            tableGroup12.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup12.Name = "detail";
            this.mainTable.RowGroups.Add(tableGroup12);
            this.mainTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.967108428478241D));
            this.mainTable.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.mainTable.StyleName = "Civic.TableNormal";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.34672954678535461D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox3.StyleName = "Civic.TableBody";
            this.textBox3.Value = "= Fields.Name";
            // 
            // structuresTable
            // 
            this.structuresTable.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.StructureTypes"));
            this.structuresTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.532281219959259D)));
            this.structuresTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(5.9656352996826172D)));
            this.structuresTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20767135918140411D)));
            this.structuresTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.41270759701728821D)));
            this.structuresTable.Body.SetCellContent(0, 1, this.textBox4);
            this.structuresTable.Body.SetCellContent(1, 1, this.workCategoriesTable);
            this.structuresTable.Body.SetCellContent(0, 0, this.textBox5);
            this.structuresTable.Body.SetCellContent(1, 0, this.textBox8);
            tableGroup6.Name = "group5";
            tableGroup7.Name = "tableGroup";
            this.structuresTable.ColumnGroups.Add(tableGroup6);
            this.structuresTable.ColumnGroups.Add(tableGroup7);
            this.structuresTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.workCategoriesTable,
            this.textBox5,
            this.textBox8});
            this.structuresTable.Name = "structuresTable";
            tableGroup9.Name = "group2";
            tableGroup10.Name = "group3";
            tableGroup8.ChildGroups.Add(tableGroup9);
            tableGroup8.ChildGroups.Add(tableGroup10);
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup8.Name = "detailTableGroup";
            this.structuresTable.RowGroups.Add(tableGroup8);
            this.structuresTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.62037897109985352D));
            this.structuresTable.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.Code", Telerik.Reporting.SortDirection.Asc));
            this.structuresTable.Style.BorderColor.Default = System.Drawing.Color.LimeGreen;
            this.structuresTable.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Dashed;
            this.structuresTable.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.structuresTable.StyleName = "Civic.TableBody";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.9656352996826172D), Telerik.Reporting.Drawing.Unit.Inch(0.20767135918140411D));
            this.textBox4.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox4.Value = "=Fields.Name";
            // 
            // workCategoriesTable
            // 
            this.workCategoriesTable.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "=Fields.CategoryList"));
            this.workCategoriesTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.60286635160446167D)));
            this.workCategoriesTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(3.652630090713501D)));
            this.workCategoriesTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.7101386785507202D)));
            this.workCategoriesTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.18913860619068146D)));
            this.workCategoriesTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.22356900572776794D)));
            this.workCategoriesTable.Body.SetCellContent(0, 1, this.textBox7);
            this.workCategoriesTable.Body.SetCellContent(0, 2, this.textBox9);
            this.workCategoriesTable.Body.SetCellContent(1, 1, this.textBox1);
            this.workCategoriesTable.Body.SetCellContent(1, 2, this.textBox2);
            this.workCategoriesTable.Body.SetCellContent(0, 0, this.textBox10);
            this.workCategoriesTable.Body.SetCellContent(1, 0, this.textBox11);
            tableGroup1.Name = "group6";
            tableGroup2.Name = "tableGroup1";
            tableGroup3.Name = "tableGroup2";
            this.workCategoriesTable.ColumnGroups.Add(tableGroup1);
            this.workCategoriesTable.ColumnGroups.Add(tableGroup2);
            this.workCategoriesTable.ColumnGroups.Add(tableGroup3);
            this.workCategoriesTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox9,
            this.textBox1,
            this.textBox2,
            this.textBox10,
            this.textBox11});
            this.workCategoriesTable.Name = "workCategoriesTable";
            tableGroup4.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup4.Name = "detailTableGroup1";
            tableGroup5.Name = "group4";
            this.workCategoriesTable.RowGroups.Add(tableGroup4);
            this.workCategoriesTable.RowGroups.Add(tableGroup5);
            this.workCategoriesTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.9656352996826172D), Telerik.Reporting.Drawing.Unit.Inch(0.4127076268196106D));
            this.workCategoriesTable.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.Code", Telerik.Reporting.SortDirection.Asc));
            this.workCategoriesTable.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.workCategoriesTable.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.workCategoriesTable.StyleName = "";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6526305675506592D), Telerik.Reporting.Drawing.Unit.Inch(0.18913860619068146D));
            this.textBox7.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox7.Value = "=Fields.Name";
            // 
            // textBox9
            // 
            this.textBox9.Format = "{0:N2}";
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7101387977600098D), Telerik.Reporting.Drawing.Unit.Inch(0.18913860619068146D));
            this.textBox9.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Value = "=Fields.TotalPrice";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6526305675506592D), Telerik.Reporting.Drawing.Unit.Inch(0.22356903553009033D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.textBox1.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Sub Total";
            // 
            // textBox2
            // 
            this.textBox2.Format = "{0:N2}";
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7101387977600098D), Telerik.Reporting.Drawing.Unit.Inch(0.22356903553009033D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.textBox2.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "=Sum(Fields.TotalPrice)";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.3000001907348633D);
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
            this.pbLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.pbLogo.MimeType = "";
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.pbLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbLogo.Value = "";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7979167699813843D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
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
            this.textBox62.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7979167699813843D), Telerik.Reporting.Drawing.Unit.Inch(0.50624996423721313D));
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
            this.textBox64.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7979167699813843D), Telerik.Reporting.Drawing.Unit.Inch(0.90208333730697632D));
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox64.Style.Font.Bold = true;
            this.textBox64.Style.Font.Name = "Segoe UI";
            this.textBox64.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox64.Value = "BoQ Summary";
            // 
            // textBox66
            // 
            this.textBox66.Format = "{0:d}";
            this.textBox66.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.1910533905029297D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.textBox66.Name = "textBox66";
            this.textBox66.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox66.Value = "= Today()";
            // 
            // textBox65
            // 
            this.textBox65.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3160533905029297D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            this.textBox65.Name = "textBox65";
            this.textBox65.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox65.Style.Font.Bold = true;
            this.textBox65.Value = "Date:";
            // 
            // textBox68
            // 
            this.textBox68.Format = "{0:d}";
            this.textBox68.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.1910533905029297D), Telerik.Reporting.Drawing.Unit.Inch(1.7020834684371948D));
            this.textBox68.Name = "textBox68";
            this.textBox68.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox68.Value = "=Fields.ProjectName";
            // 
            // textBox67
            // 
            this.textBox67.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.3160533905029297D), Telerik.Reporting.Drawing.Unit.Inch(1.7020834684371948D));
            this.textBox67.Name = "textBox67";
            this.textBox67.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox67.Style.Font.Bold = true;
            this.textBox67.Value = "Project:";
            // 
            // shape3
            // 
            this.shape3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(2D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.399960994720459D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.425196647644043D), Telerik.Reporting.Drawing.Unit.Inch(0.60629922151565552D));
            this.textBox6.Name = "ReportPageNumberTextBox";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.39370077848434448D));
            this.textBox6.Style.Font.Name = "Segoe UI";
            this.textBox6.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.53228116035461426D), Telerik.Reporting.Drawing.Unit.Inch(0.20767135918140411D));
            this.textBox5.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "=Fields.Code";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.53228116035461426D), Telerik.Reporting.Drawing.Unit.Inch(0.4127076268196106D));
            this.textBox8.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.StyleName = "";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.60286641120910645D), Telerik.Reporting.Drawing.Unit.Inch(0.18913860619068146D));
            this.textBox10.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox10.StyleName = "";
            this.textBox10.Value = "=Fields.Code";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.60286641120910645D), Telerik.Reporting.Drawing.Unit.Inch(0.22356903553009033D));
            this.textBox11.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox11.StyleName = "";
            // 
            // boqSummaryOds
            // 
            this.boqSummaryOds.DataMember = "GetAllSubProjects";
            this.boqSummaryOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.BoQSummaryOds);
            this.boqSummaryOds.Name = "boqSummaryOds";
            this.boqSummaryOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectid.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("subProjectTypeId", typeof(string), "= Parameters.subProjectTypeId.Value")});
            // 
            // availableProjects
            // 
            this.availableProjects.DataMember = "GetAvailableProjects";
            this.availableProjects.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.ProjectsOds);
            this.availableProjects.Name = "availableProjects";
            // 
            // subProjectTypesOds
            // 
            this.subProjectTypesOds.DataMember = "GetAvailableSubProjectTypes";
            this.subProjectTypesOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.SubProjectTypesOds);
            this.subProjectTypesOds.Name = "subProjectTypesOds";
            this.subProjectTypesOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectid.Value")});
            // 
            // projectHeaderOds
            // 
            this.projectHeaderOds.DataMember = "GetProjectHeader";
            this.projectHeaderOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.BoQSummaryOds);
            this.projectHeaderOds.Name = "projectHeaderOds";
            this.projectHeaderOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectid.Value")});
            // 
            // rptBoQSummary
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "rptBoQSummary";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.availableProjects;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.ProjectInformationId";
            reportParameter1.Name = "projectid";
            reportParameter1.Text = "Projects";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.subProjectTypesOds;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Id";
            reportParameter2.Name = "subProjectTypeId";
            reportParameter2.Text = "Sub-Project types";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.Table), "Civic.TableNormal")});
            styleRule2.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule2.Style.Color = System.Drawing.Color.Black;
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableBody")});
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector1});
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            descendantSelector2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.Table)),
            new Telerik.Reporting.Drawing.StyleSelector(typeof(Telerik.Reporting.ReportItem), "Civic.TableHeader")});
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            descendantSelector2});
            styleRule4.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule4.Style.BorderColor.Default = System.Drawing.Color.Black;
            styleRule4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule4.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule4.Style.Font.Name = "Georgia";
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
        private Telerik.Reporting.Table mainTable;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.ObjectDataSource boqSummaryOds;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource availableProjects;
        private Telerik.Reporting.Table structuresTable;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.Table workCategoriesTable;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.ObjectDataSource projectHeaderOds;
        private Telerik.Reporting.PictureBox pbLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox62;
        private Telerik.Reporting.TextBox textBox64;
        private Telerik.Reporting.TextBox textBox66;
        private Telerik.Reporting.TextBox textBox65;
        private Telerik.Reporting.TextBox textBox68;
        private Telerik.Reporting.TextBox textBox67;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.ObjectDataSource subProjectTypesOds;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox8;
    }
}