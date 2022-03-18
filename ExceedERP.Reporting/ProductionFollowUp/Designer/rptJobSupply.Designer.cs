namespace ExceedERP.Reporting.ProductionFollowUp.Designer
{
    partial class rptJobSupply
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
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox36 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detail.Name = "detail";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // table1
            // 
            this.table1.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.JobTypeItemVMs"));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.74960070848464966D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.1100268363952637D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.89153832197189331D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.88063293695449829D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.26816970109939575D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Cm(4.9569053649902344D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.31387788057327271D)));
            this.table1.Body.SetCellContent(0, 1, this.textBox8);
            this.table1.Body.SetCellContent(0, 0, this.textBox40);
            this.table1.Body.SetCellContent(0, 2, this.textBox10, 1, 3);
            this.table1.Body.SetCellContent(0, 5, this.textBox4);
            tableGroup1.Name = "group";
            tableGroup1.ReportItem = this.textBox36;
            tableGroup2.Name = "description";
            tableGroup2.ReportItem = this.textBox1;
            tableGroup4.Name = "group2";
            tableGroup5.Name = "group3";
            tableGroup6.Name = "size";
            tableGroup3.ChildGroups.Add(tableGroup4);
            tableGroup3.ChildGroups.Add(tableGroup5);
            tableGroup3.ChildGroups.Add(tableGroup6);
            tableGroup3.Name = "quantity";
            tableGroup3.ReportItem = this.textBox3;
            tableGroup8.Name = "group4";
            tableGroup7.ChildGroups.Add(tableGroup8);
            tableGroup7.Name = "group1";
            tableGroup7.ReportItem = this.textBox2;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox40,
            this.textBox8,
            this.textBox10,
            this.textBox4,
            this.textBox36,
            this.textBox1,
            this.textBox3,
            this.textBox2});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(-0.011564254760742188D), Telerik.Reporting.Drawing.Unit.Inch(0.7480316162109375D));
            this.table1.Name = "table1";
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detail";
            this.table1.RowGroups.Add(tableGroup9);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.851506233215332D), Telerik.Reporting.Drawing.Unit.Inch(0.62775570154190063D));
            this.table1.StyleName = "Civic.TableNormal";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1100268363952637D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.StyleName = "Civic.TableBody";
            this.textBox8.Value = "=Fields.JobType";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0403409004211426D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox10.StyleName = "Civic.TableBody";
            this.textBox10.Value = "=Fields.TotalPrice";
            // 
            // textBox40
            // 
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.74960082769393921D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox40.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox40.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox40.StyleName = "Civic.TableBody";
            this.textBox40.Value = "=Fields.Code";
            // 
            // textBox36
            // 
            this.textBox36.Name = "textBox36";
            this.textBox36.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.74960082769393921D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox36.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox36.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox36.Style.Color = System.Drawing.Color.Black;
            this.textBox36.Style.Font.Bold = true;
            this.textBox36.StyleName = "Civic.TableHeader";
            this.textBox36.Value = "ኮድ";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1100268363952637D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Color = System.Drawing.Color.Black;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "Civic.TableHeader";
            this.textBox1.Value = "የስራው ዓይነት";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0403409004211426D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.Color = System.Drawing.Color.Black;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.StyleName = "Civic.TableHeader";
            this.textBox3.Value = "ከ {=Fields.From} እስከ {=Fields.To}";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.35709062218666077D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8503546714782715D), Telerik.Reporting.Drawing.Unit.Inch(0.62712198495864868D));
            this.textBox9.Style.Font.Bold = false;
            this.textBox9.Style.Font.Name = "Segoe UI";
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(15D);
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.Value = "ከ  {=Fields.From}        እስከ    {=Fields.To}   ዓ.ም  የኣንድ ወር የስራ ኣቅርቦት ስራ         " +
    "                                                ኣፈጻጸም ሪፖርት  ";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.9568967819213867D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.StyleName = "Civic.TableHeader";
            this.textBox2.Value = "የማዘዣ ብዛት";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.9568967819213867D), Telerik.Reporting.Drawing.Unit.Inch(0.31387785077095032D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.StyleName = "Civic.TableBody";
            // 
            // groupHeaderSection
            // 
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.groupFooterSection.Name = "groupFooterSection";
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(2.5D);
            this.groupFooterSection1.Name = "groupFooterSection1";
            // 
            // rptJobSupply
            // 
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Name = "group5";
            group2.GroupFooter = this.groupFooterSection1;
            group2.GroupHeader = this.groupHeaderSection1;
            group2.Name = "group6";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.groupHeaderSection,
            this.groupFooterSection,
            this.groupHeaderSection1,
            this.groupFooterSection1});
            this.Name = "rptJobSupply";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(20D), Telerik.Reporting.Drawing.Unit.Mm(20D), Telerik.Reporting.Drawing.Unit.Mm(20D), Telerik.Reporting.Drawing.Unit.Mm(20D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(17.399999618530273D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox36;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.GroupFooterSection groupFooterSection1;
    }
}