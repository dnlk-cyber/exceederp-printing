namespace ExceedERP.Reporting.Construction.ReportDesign
{
    partial class rptMiscellaneousExpenses
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector1 = new Telerik.Reporting.Drawing.DescendantSelector();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.DescendantSelector descendantSelector2 = new Telerik.Reporting.Drawing.DescendantSelector();
            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.mainTable = new Telerik.Reporting.Table();
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
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox98 = new Telerik.Reporting.TextBox();
            this.textBox99 = new Telerik.Reporting.TextBox();
            this.textBox100 = new Telerik.Reporting.TextBox();
            this.textBox101 = new Telerik.Reporting.TextBox();
            this.textBox102 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbLogo = new Telerik.Reporting.PictureBox();
            this.miscellaneousExpensesOds = new Telerik.Reporting.ObjectDataSource();
            this.availableProjectsOds = new Telerik.Reporting.ObjectDataSource();
            this.headerOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detailSection1
            // 
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.mainTable});
            this.detailSection1.Name = "detailSection1";
            // 
            // mainTable
            // 
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.67708319425582886D)));
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.5187509059906006D)));
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3729163408279419D)));
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3833335638046265D)));
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6541666984558106D)));
            this.mainTable.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3833335638046265D)));
            this.mainTable.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D)));
            this.mainTable.Body.SetCellContent(0, 1, this.textBox6);
            this.mainTable.Body.SetCellContent(0, 2, this.textBox7);
            this.mainTable.Body.SetCellContent(0, 3, this.textBox8);
            this.mainTable.Body.SetCellContent(0, 4, this.textBox9);
            this.mainTable.Body.SetCellContent(0, 5, this.textBox10);
            this.mainTable.Body.SetCellContent(0, 0, this.textBox14);
            tableGroup1.Name = "group";
            tableGroup1.ReportItem = this.textBox13;
            tableGroup2.Name = "description";
            tableGroup2.ReportItem = this.textBox1;
            tableGroup3.Name = "previousAmount";
            tableGroup3.ReportItem = this.textBox2;
            tableGroup4.Name = "thisMonthAmount";
            tableGroup4.ReportItem = this.textBox3;
            tableGroup5.Name = "toDateAmount";
            tableGroup5.ReportItem = this.textBox4;
            tableGroup6.Name = "remark";
            tableGroup6.ReportItem = this.textBox5;
            this.mainTable.ColumnGroups.Add(tableGroup1);
            this.mainTable.ColumnGroups.Add(tableGroup2);
            this.mainTable.ColumnGroups.Add(tableGroup3);
            this.mainTable.ColumnGroups.Add(tableGroup4);
            this.mainTable.ColumnGroups.Add(tableGroup5);
            this.mainTable.ColumnGroups.Add(tableGroup6);
            this.mainTable.DataSource = this.miscellaneousExpensesOds;
            this.mainTable.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6,
            this.textBox7,
            this.textBox8,
            this.textBox9,
            this.textBox10,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.textBox5,
            this.textBox13,
            this.textBox14});
            this.mainTable.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.mainTable.Name = "mainTable";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detail";
            this.mainTable.RowGroups.Add(tableGroup7);
            this.mainTable.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.9895839691162109D), Telerik.Reporting.Drawing.Unit.Inch(0.58749997615814209D));
            this.mainTable.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.mainTable.StyleName = "Civic.TableNormal";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5187509059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox1.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox1.StyleName = "Civic.TableHeader";
            this.textBox1.Value = "Description";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3729163408279419D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox2.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox2.StyleName = "Civic.TableHeader";
            this.textBox2.Value = "Previous Amount";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3833335638046265D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox3.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox3.StyleName = "Civic.TableHeader";
            this.textBox3.Value = "This Month Amount";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6541666984558106D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox4.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox4.StyleName = "Civic.TableHeader";
            this.textBox4.Value = "To Date Amount";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3833335638046265D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox5.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox5.StyleName = "Civic.TableHeader";
            this.textBox5.Value = "Remark";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5187509059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox6.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox6.StyleName = "Civic.TableBody";
            this.textBox6.Value = "= Fields.Description";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3729162216186523D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox7.StyleName = "Civic.TableBody";
            this.textBox7.Value = "= Fields.PreviousAmount";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3833332061767578D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox8.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox8.StyleName = "Civic.TableBody";
            this.textBox8.Value = "= Fields.ThisMonthAmount";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6541668176651D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox9.StyleName = "Civic.TableBody";
            this.textBox9.Value = "= Fields.ToDateAmount";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3833338022232056D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox10.StyleName = "Civic.TableBody";
            this.textBox10.Value = "= Fields.Remark";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.2000000476837158D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pbLogo,
            this.txtOrgName,
            this.textBox102,
            this.textBox101,
            this.textBox100,
            this.textBox99,
            this.textBox98,
            this.textBox21,
            this.shape3});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.425196647644043D), Telerik.Reporting.Drawing.Unit.Inch(0.60629922151565552D));
            this.textBox12.Name = "ReportPageNumberTextBox";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.39370077848434448D));
            this.textBox12.Style.Font.Name = "Segoe UI";
            this.textBox12.Value = "Page: {PageNumber}";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox12});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.67708319425582886D), Telerik.Reporting.Drawing.Unit.Inch(0.38749998807907104D));
            this.textBox13.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox13.StyleName = "Civic.TableHeader";
            this.textBox13.Value = "No.";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.67708325386047363D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox14.Style.BorderColor.Default = System.Drawing.Color.Silver;
            this.textBox14.StyleName = "Civic.TableBody";
            // 
            // shape3
            // 
            this.shape3.Anchoring = ((Telerik.Reporting.AnchoringStyles)((Telerik.Reporting.AnchoringStyles.Left | Telerik.Reporting.AnchoringStyles.Right)));
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0062500634230673313D), Telerik.Reporting.Drawing.Unit.Inch(2.0062501430511475D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.EW);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.96458625793457D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3395829200744629D), Telerik.Reporting.Drawing.Unit.Inch(1.6937500238418579D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Value = "Project:";
            // 
            // textBox98
            // 
            this.textBox98.Format = "{0:d}";
            this.textBox98.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.2145829200744629D), Telerik.Reporting.Drawing.Unit.Inch(1.6937500238418579D));
            this.textBox98.Name = "textBox98";
            this.textBox98.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox98.Value = "=Fields.ProjectName";
            // 
            // textBox99
            // 
            this.textBox99.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3395829200744629D), Telerik.Reporting.Drawing.Unit.Inch(1.4020833969116211D));
            this.textBox99.Name = "textBox99";
            this.textBox99.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66518974304199219D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox99.Style.Font.Bold = true;
            this.textBox99.Value = "Date:";
            // 
            // textBox100
            // 
            this.textBox100.Format = "{0:d}";
            this.textBox100.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.2145829200744629D), Telerik.Reporting.Drawing.Unit.Inch(1.4020833969116211D));
            this.textBox100.Name = "textBox100";
            this.textBox100.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.6818637847900391D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox100.Value = "= Today()";
            // 
            // textBox101
            // 
            this.textBox101.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7875000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.89166665077209473D));
            this.textBox101.Name = "textBox101";
            this.textBox101.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox101.Style.Font.Bold = true;
            this.textBox101.Style.Font.Name = "Segoe UI";
            this.textBox101.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox101.Value = "Miscellaneous of Expense";
            // 
            // textBox102
            // 
            this.textBox102.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7875000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.50624996423721313D));
            this.textBox102.Name = "textBox102";
            this.textBox102.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0750002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox102.Style.Font.Bold = true;
            this.textBox102.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox102.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox102.TextWrap = false;
            this.textBox102.Value = "= Fields.Name";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.7875000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
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
            this.pbLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.pbLogo.MimeType = "";
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.pbLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbLogo.Value = "";
            // 
            // miscellaneousExpensesOds
            // 
            this.miscellaneousExpensesOds.DataMember = "GetMiscellaneousExpenses";
            this.miscellaneousExpensesOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.MiscellaneousExpensesOds);
            this.miscellaneousExpensesOds.Name = "miscellaneousExpensesOds";
            this.miscellaneousExpensesOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
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
            this.headerOds.DataSource = typeof(ExceedERP.Reporting.Construction.DataSource.MiscellaneousExpensesOds);
            this.headerOds.Name = "headerOds";
            this.headerOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("projectId", typeof(string), "= Parameters.projectId.Value")});
            // 
            // rptMiscellaneousExpenses
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detailSection1,
            this.pageHeaderSection1,
            this.pageFooterSection1});
            this.Name = "rptMiscellaneousExpenses";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.availableProjectsOds;
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(9.0000009536743164D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.Table mainTable;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.ObjectDataSource miscellaneousExpensesOds;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.ObjectDataSource availableProjectsOds;
        private Telerik.Reporting.ObjectDataSource headerOds;
        private Telerik.Reporting.PictureBox pbLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox102;
        private Telerik.Reporting.TextBox textBox101;
        private Telerik.Reporting.TextBox textBox100;
        private Telerik.Reporting.TextBox textBox99;
        private Telerik.Reporting.TextBox textBox98;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.Shape shape3;
    }
}