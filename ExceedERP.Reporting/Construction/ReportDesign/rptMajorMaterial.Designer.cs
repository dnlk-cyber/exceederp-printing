namespace ExceedERP.Reporting.Construction.ReportDesign
{
    partial class rptMajorMaterial
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup8 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.mainTable = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.materialsTable = new Telerik.Reporting.Table();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox67 = new Telerik.Reporting.TextBox();
            this.textBox68 = new Telerik.Reporting.TextBox();
            this.textBox65 = new Telerik.Reporting.TextBox();
            this.textBox66 = new Telerik.Reporting.TextBox();
            this.textBox64 = new Telerik.Reporting.TextBox();
            this.textBox62 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbLogo = new Telerik.Reporting.PictureBox();
            this.majorMaterialsOds = new Telerik.Reporting.ObjectDataSource();
            this.availableProjectsOds = new Telerik.Reporting.ObjectDataSource();
            this.headerOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3000003099441528D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.mainTable});
            this.detailSection1.Name = "detailSection1";
            // 
            // mainTable
            // 
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.31567946076393127D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.57667696475982666D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D)));
            this.mainTable.Body.SetCellContent(0, 0, this.textBox2);
            this.mainTable.Body.SetCellContent(1, 0, this.materialsTable);
            this.mainTable.Body.SetCellContent(2, 0, this.textBox5);
            tableGroup8.Name = "name";
            this.mainTable.ColumnGroups.Add(tableGroup8);
            this.mainTable.DataSource = this.majorMaterialsOds;
            this.mainTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.materialsTable,
            this.textBox5});
            this.mainTable.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.mainTable.Name = "mainTable";
            tableGroup10.Name = "group";
            tableGroup11.Name = "group1";
            tableGroup12.Name = "group2";
            tableGroup9.ChildGroups.Add(tableGroup10);
            tableGroup9.ChildGroups.Add(tableGroup11);
            tableGroup9.ChildGroups.Add(tableGroup12);
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detail";
            this.mainTable.RowGroups.Add(tableGroup9);
            this.mainTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(1.0923564434051514D));
            this.mainTable.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.mainTable.StyleName = "Civic.TableNormal";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.31567949056625366D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox2.Style.Color = System.Drawing.SystemColors.HotTrack;
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox2.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox2.StyleName = "Civic.TableBody";
            this.textBox2.Value = "= Fields.Name";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
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
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.925196647644043D), Telerik.Reporting.Drawing.Unit.Inch(0.60629922151565552D));
            this.textBox4.Name = "ReportPageNumberTextBox";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.39370077848434448D));
            this.textBox4.Style.Font.Name = "Segoe UI";
            this.textBox4.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // materialsTable
            // 
            this.materialsTable.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.MaterialList"));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.778692364692688D)));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.75292664766311646D)));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.80526131391525269D)));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D)));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D)));
            this.materialsTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.1513708829879761D)));
            this.materialsTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D)));
            this.materialsTable.Body.SetCellContent(0, 0, this.textBox7);
            this.materialsTable.Body.SetCellContent(0, 1, this.textBox9);
            this.materialsTable.Body.SetCellContent(0, 2, this.textBox11);
            this.materialsTable.Body.SetCellContent(0, 3, this.textBox13);
            this.materialsTable.Body.SetCellContent(0, 4, this.textBox15);
            this.materialsTable.Body.SetCellContent(0, 5, this.textBox17);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox6;
            tableGroup2.Name = "tableGroup1";
            tableGroup2.ReportItem = this.textBox8;
            tableGroup3.Name = "tableGroup2";
            tableGroup3.ReportItem = this.textBox10;
            tableGroup4.Name = "group3";
            tableGroup4.ReportItem = this.textBox12;
            tableGroup5.Name = "group4";
            tableGroup5.ReportItem = this.textBox14;
            tableGroup6.Name = "group5";
            tableGroup6.ReportItem = this.textBox16;
            this.materialsTable.ColumnGroups.Add(tableGroup1);
            this.materialsTable.ColumnGroups.Add(tableGroup2);
            this.materialsTable.ColumnGroups.Add(tableGroup3);
            this.materialsTable.ColumnGroups.Add(tableGroup4);
            this.materialsTable.ColumnGroups.Add(tableGroup5);
            this.materialsTable.ColumnGroups.Add(tableGroup6);
            this.materialsTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox9,
            this.textBox11,
            this.textBox6,
            this.textBox8,
            this.textBox10,
            this.textBox12,
            this.textBox13,
            this.textBox14,
            this.textBox15,
            this.textBox16,
            this.textBox17});
            this.materialsTable.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.materialsTable.Name = "materialsTable";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detailTableGroup";
            this.materialsTable.RowGroups.Add(tableGroup7);
            this.materialsTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.57667690515518188D));
            this.materialsTable.Sortings.Add(new Telerik.Reporting.Sorting("=Fields.PercentageToTotalCostOfProject", Telerik.Reporting.SortDirection.Desc));
            this.materialsTable.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.materialsTable.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.materialsTable.StyleName = "Civic.TableBody";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7786926031112671D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox6.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.Color = System.Drawing.Color.White;
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox6.Value = "Material Type";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7786926031112671D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox7.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Value = "=Fields.Name";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75292670726776123D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox8.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox8.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.Color = System.Drawing.Color.White;
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox8.Value = "Unit";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.75292670726776123D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox9.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Value = "=Fields.Unit";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80526137351989746D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox10.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.Color = System.Drawing.Color.White;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox10.Value = "Qty";
            // 
            // textBox11
            // 
            this.textBox11.Format = "{0:N2}";
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80526137351989746D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox11.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Value = "=Fields.Quantity";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox12.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox12.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.Color = System.Drawing.Color.White;
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox12.StyleName = "";
            this.textBox12.Value = "Price per unit";
            // 
            // textBox13
            // 
            this.textBox13.Format = "{0:N2}";
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox13.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.StyleName = "";
            this.textBox13.Value = "=Fields.UnitPrice";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox14.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.Color = System.Drawing.Color.White;
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox14.StyleName = "";
            this.textBox14.Value = "Total Cost";
            // 
            // textBox15
            // 
            this.textBox15.Format = "{0:N2}";
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0048326253890991D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox15.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.StyleName = "";
            this.textBox15.Value = "=Fields.TotalCost";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1513707637786865D), Telerik.Reporting.Drawing.Unit.Inch(0.32137894630432129D));
            this.textBox16.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox16.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.Color = System.Drawing.Color.White;
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox16.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox16.StyleName = "";
            this.textBox16.Value = "% wrt Project total Cost";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:P2}";
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1513707637786865D), Telerik.Reporting.Drawing.Unit.Inch(0.255297988653183D));
            this.textBox17.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.textBox17.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox17.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox17.StyleName = "";
            this.textBox17.Value = "=Fields.PercentageToTotalCostOfProject";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4979166984558105D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox5.StyleName = "Civic.TableBody";
            // 
            // shape3
            // 
            this.shape3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.050019580870866776D), Telerik.Reporting.Drawing.Unit.Inch(1.8000000715255737D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4499807357788086D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox67
            // 
            this.textBox67.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2660729885101318D), Telerik.Reporting.Drawing.Unit.Inch(1.5020836591720581D));
            this.textBox67.Name = "textBox67";
            this.textBox67.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox67.Style.Font.Bold = true;
            this.textBox67.Value = "Project:";
            // 
            // textBox68
            // 
            this.textBox68.Format = "{0:d}";
            this.textBox68.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.1410729885101318D), Telerik.Reporting.Drawing.Unit.Inch(1.5020836591720581D));
            this.textBox68.Name = "textBox68";
            this.textBox68.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox68.Value = "=Fields.ProjectName";
            // 
            // textBox65
            // 
            this.textBox65.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2660729885101318D), Telerik.Reporting.Drawing.Unit.Inch(1.2000002861022949D));
            this.textBox65.Name = "textBox65";
            this.textBox65.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox65.Style.Font.Bold = true;
            this.textBox65.Value = "Date:";
            // 
            // textBox66
            // 
            this.textBox66.Format = "{0:d}";
            this.textBox66.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.1410729885101318D), Telerik.Reporting.Drawing.Unit.Inch(1.2000002861022949D));
            this.textBox66.Name = "textBox66";
            this.textBox66.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox66.Value = "= Today()";
            // 
            // textBox64
            // 
            this.textBox64.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7479362487792969D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox64.Name = "textBox64";
            this.textBox64.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox64.Style.Font.Bold = true;
            this.textBox64.Style.Font.Name = "Segoe UI";
            this.textBox64.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox64.Value = "Materials\' Cost % w.r.t. total Project Cost";
            // 
            // textBox62
            // 
            this.textBox62.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7479362487792969D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.textBox62.Name = "textBox62";
            this.textBox62.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox62.Style.Font.Bold = true;
            this.textBox62.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox62.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox62.TextWrap = false;
            this.textBox62.Value = "= Fields.Name";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7479362487792969D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0854158401489258D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // pbLogo
            // 
            this.pbLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.050019580870866776D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.pbLogo.MimeType = "";
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.pbLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbLogo.Value = "";
            // 
            // majorMaterialsOds
            // 
            this.majorMaterialsOds.DataMember = "GetProjectMaterials";
            this.majorMaterialsOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.MajorMaterialsOds);
            this.majorMaterialsOds.Name = "majorMaterialsOds";
            this.majorMaterialsOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectId.Value")});
            // 
            // availableProjectsOds
            // 
            this.availableProjectsOds.DataMember = "GetAvailableProjects";
            this.availableProjectsOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.ProjectsOds);
            this.availableProjectsOds.Name = "availableProjectsOds";
            // 
            // headerOds
            // 
            this.headerOds.DataMember = "GetProjectHeader";
            this.headerOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.MajorMaterialsOds);
            this.headerOds.Name = "headerOds";
            this.headerOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectId.Value")});
            // 
            // rptMajorMaterial
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "rptMajorMaterial";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.availableProjectsOds;
            reportParameter1.AvailableValues.DisplayMember = "=Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "=Fields.ProjectInformationId";
            reportParameter1.Name = "projectId";
            reportParameter1.Text = "Projects";
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
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
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.ObjectDataSource majorMaterialsOds;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Table materialsTable;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.ObjectDataSource availableProjectsOds;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.PictureBox pbLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox62;
        private Telerik.Reporting.TextBox textBox64;
        private Telerik.Reporting.TextBox textBox66;
        private Telerik.Reporting.TextBox textBox65;
        private Telerik.Reporting.TextBox textBox68;
        private Telerik.Reporting.TextBox textBox67;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.ObjectDataSource headerOds;
    }
}