namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class ProbationEmployeeChangeOfPayroll
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProbationEmployeeChangeOfPayroll));
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.headerSubReport1 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();
            this.DepartmentobjectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox28 = new Telerik.Reporting.TextBox();
            this.CCobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.textBox38 = new Telerik.Reporting.TextBox();
            this.textBox41 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox47 = new Telerik.Reporting.TextBox();
            this.ChangeOfPayrollBenefitODS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.ChangeOFPayrollODS = new Telerik.Reporting.ObjectDataSource();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox24
            // 
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.3125D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox24.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox24.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox24.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            resources.ApplyResources(this.textBox24, "textBox24");
            // 
            // headerSubReport1
            // 
            this.headerSubReport1.Name = "HeaderSubReport";
            // 
            // DepartmentobjectDataSource1
            // 
            this.DepartmentobjectDataSource1.DataMember = "GetDepartments";
            this.DepartmentobjectDataSource1.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.AllDepartmentsListODS);
            this.DepartmentobjectDataSource1.Name = "DepartmentobjectDataSource1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(4.7000002861022949D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.table1,
            this.textBox39,
            this.textBox38,
            this.textBox41,
            this.textBox40,
            this.textBox47,
            this.textBox1,
            this.textBox2});
            this.detail.Name = "detail";
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18750064074993134D), Telerik.Reporting.Drawing.Unit.Inch(1.3952757120132446D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.5124993324279785D), Telerik.Reporting.Drawing.Unit.Inch(1.5000003576278687D));
            this.textBox11.Style.Color = System.Drawing.Color.Black;
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox11.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox11, "textBox11");
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.40625026822090149D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(3.9062497615814209D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox25);
            this.table1.Body.SetCellContent(0, 1, this.textBox28);
            tableGroup2.Name = "group2";
            tableGroup3.Name = "tableGroup1";
            tableGroup1.ChildGroups.Add(tableGroup2);
            tableGroup1.ChildGroups.Add(tableGroup3);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox24;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.DataSource = this.CCobjectDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox25,
            this.textBox28,
            this.textBox24});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18750064074993134D), Telerik.Reporting.Drawing.Unit.Inch(3.8952760696411133D));
            this.table1.Name = "table1";
            tableGroup4.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup4.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup4);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.3125D), Telerik.Reporting.Drawing.Unit.Inch(0.4791666567325592D));
            // 
            // textBox25
            // 
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.40625005960464478D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox25.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox25.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            resources.ApplyResources(this.textBox25, "textBox25");
            // 
            // textBox28
            // 
            this.textBox28.Name = "textBox28";
            this.textBox28.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.90625D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox28.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox28.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            resources.ApplyResources(this.textBox28, "textBox28");
            // 
            // CCobjectDataSource
            // 
            this.CCobjectDataSource.DataMember = "GetLetThemKnows";
            this.CCobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKowWorkUnitObjectDataSource);
            this.CCobjectDataSource.Name = "CCobjectDataSource";
            this.CCobjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.CC.Value")});
            // 
            // textBox39
            // 
            this.textBox39.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18750064074993134D), Telerik.Reporting.Drawing.Unit.Inch(2.995197057723999D));
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4124996662139893D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox39.Style.Color = System.Drawing.Color.Black;
            this.textBox39.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox39.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox39, "textBox39");
            // 
            // textBox38
            // 
            this.textBox38.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.53125065565109253D), Telerik.Reporting.Drawing.Unit.Inch(0.40569242835044861D));
            this.textBox38.Name = "textBox38";
            this.textBox38.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2687497138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox38.Style.Color = System.Drawing.Color.Black;
            this.textBox38.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox38.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox38, "textBox38");
            // 
            // textBox41
            // 
            this.textBox41.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.50000065565109253D), Telerik.Reporting.Drawing.Unit.Inch(0.19519679248332977D));
            this.textBox41.Name = "textBox41";
            this.textBox41.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox41.Style.Color = System.Drawing.Color.Black;
            this.textBox41.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox41.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox41, "textBox41");
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18750064074993134D), Telerik.Reporting.Drawing.Unit.Inch(3.1952760219573975D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4124996662139893D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox40.Style.Color = System.Drawing.Color.Black;
            this.textBox40.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox40.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox40, "textBox40");
            // 
            // textBox47
            // 
            this.textBox47.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5000007152557373D), Telerik.Reporting.Drawing.Unit.Inch(0.99527579545974731D));
            this.textBox47.Name = "textBox47";
            this.textBox47.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2124996185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox47.Style.Color = System.Drawing.Color.Black;
            this.textBox47.Style.Font.Bold = true;
            this.textBox47.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox47.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox47, "textBox47");
            // 
            // ChangeOfPayrollBenefitODS
            // 
            this.ChangeOfPayrollBenefitODS.DataMember = "GetBenefits";
            this.ChangeOfPayrollBenefitODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.ChangeOfPayrollBenefitObjectDataSource);
            this.ChangeOfPayrollBenefitODS.Name = "ChangeOfPayrollBenefitODS";
            this.ChangeOfPayrollBenefitODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // ChangeOFPayrollODS
            // 
            this.ChangeOFPayrollODS.DataMember = "GetChangeOfPayroll";
            this.ChangeOFPayrollODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.ChangeOfPayrollObjectDataSource);
            this.ChangeOFPayrollODS.Name = "ChangeOFPayrollODS";
            this.ChangeOFPayrollODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value")});
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Cm(1.5359995365142822D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.subReport1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReport1;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(22.097801208496094D), Telerik.Reporting.Drawing.Unit.Cm(1.5358994007110596D));
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.7000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.20561361312866211D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            resources.ApplyResources(this.textBox1, "textBox1");
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.7000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.40569242835044861D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            resources.ApplyResources(this.textBox2, "textBox2");
            // 
            // ProbationEmployeeChangeOfPayroll
            // 
            this.DataSource = this.ChangeOFPayrollODS;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "ChangeOfPayroll";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.Name = "id";
            resources.ApplyResources(reportParameter1, "reportParameter1");
            reportParameter1.Value = "";
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "header";
            resources.ApplyResources(reportParameter2, "reportParameter2");
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter2.Value = "= True";
            reportParameter2.Visible = true;
            reportParameter3.Name = "FormTitle";
            reportParameter4.Name = "FormNumber";
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.DepartmentobjectDataSource1;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter5.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter5.MultiValue = true;
            reportParameter5.Name = "CC";
            resources.ApplyResources(reportParameter5, "reportParameter5");
            reportParameter5.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.7000007629394531D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ObjectDataSource ChangeOFPayrollODS;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox28;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.ObjectDataSource ChangeOfPayrollBenefitODS;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox textBox38;
        private Telerik.Reporting.TextBox textBox41;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox47;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.SubReport subReport1;
        private SharedDesigners.HeaderSubReport headerSubReport1;
        private Telerik.Reporting.ObjectDataSource DepartmentobjectDataSource1;
        private Telerik.Reporting.ObjectDataSource CCobjectDataSource;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
    }
}