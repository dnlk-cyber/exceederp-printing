using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptEmpAge
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rptEmpAge));
            Telerik.Reporting.DataColumn dataColumn1 = new Telerik.Reporting.DataColumn();
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
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter6 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter7 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter8 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.NoOfMaleEmployee = new Telerik.Reporting.TextBox();
            this.NoOfFemaleEmployee = new Telerik.Reporting.TextBox();
            this.TotalEmployee = new Telerik.Reporting.TextBox();
            this.headerSubReport2 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();
            this.ageObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.SexODS = new Telerik.Reporting.ObjectDataSource();
            this.StatusODS = new Telerik.Reporting.ObjectDataSource();
            this.csvColumnHiding = new Telerik.Reporting.CsvDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.empSumaryObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.Localization = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34374991059303284D), Telerik.Reporting.Drawing.Unit.Inch(0.24690474569797516D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox18.Style.Font.Bold = true;
            this.textBox18.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox18.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.textBox18, "textBox18");
            // 
            // textBox45
            // 
            this.textBox45.Name = "Name";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1753383874893189D), Telerik.Reporting.Drawing.Unit.Inch(0.24690473079681397D));
            this.textBox45.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox45.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox45.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox45.Style.Font.Bold = true;
            this.textBox45.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox45.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.textBox45, "textBox45");
            // 
            // NoOfMaleEmployee
            // 
            this.NoOfMaleEmployee.Name = "NoOfMaleEmployee";
            this.NoOfMaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.734230637550354D), Telerik.Reporting.Drawing.Unit.Inch(0.24690476059913635D));
            this.NoOfMaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfMaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfMaleEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.NoOfMaleEmployee.Style.Font.Bold = true;
            this.NoOfMaleEmployee.Style.Font.Name = "Microsoft Sans Serif";
            this.NoOfMaleEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.NoOfMaleEmployee, "NoOfMaleEmployee");
            // 
            // NoOfFemaleEmployee
            // 
            this.NoOfFemaleEmployee.Name = "NoOfFemaleEmployee";
            this.NoOfFemaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6733753681182861D), Telerik.Reporting.Drawing.Unit.Inch(0.24690473079681397D));
            this.NoOfFemaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfFemaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfFemaleEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.NoOfFemaleEmployee.Style.Font.Bold = true;
            this.NoOfFemaleEmployee.Style.Font.Name = "Microsoft Sans Serif";
            this.NoOfFemaleEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.NoOfFemaleEmployee, "NoOfFemaleEmployee");
            // 
            // TotalEmployee
            // 
            this.TotalEmployee.Culture = new System.Globalization.CultureInfo("");
            this.TotalEmployee.Name = "TotalEmployee";
            this.TotalEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4458819627761841D), Telerik.Reporting.Drawing.Unit.Inch(0.24690474569797516D));
            this.TotalEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.TotalEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.TotalEmployee.Style.Font.Bold = true;
            this.TotalEmployee.Style.Font.Name = "Microsoft Sans Serif";
            this.TotalEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.TotalEmployee, "TotalEmployee");
            // 
            // headerSubReport2
            // 
            this.headerSubReport2.Name = "HeaderSubReport";
            // 
            // ageObjectDataSource
            // 
            this.ageObjectDataSource.DataMember = "GetAge";
            this.ageObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AgeObjectDataSource);
            this.ageObjectDataSource.Name = "ageObjectDataSource";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            // 
            // WorkUnitObjectDataSource
            // 
            this.WorkUnitObjectDataSource.DataMember = "GetDepartment";
            this.WorkUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.WorkUnitObjectDataSource.Name = "WorkUnitObjectDataSource";
            // 
            // SexODS
            // 
            this.SexODS.DataMember = "GetSex";
            this.SexODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.SexObjectDataSource);
            this.SexODS.Name = "SexODS";
            // 
            // StatusODS
            // 
            this.StatusODS.DataMember = "GetStatus";
            this.StatusODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeStatusODS);
            this.StatusODS.Name = "StatusODS";
            // 
            // csvColumnHiding
            // 
            dataColumn1.Name = "Column1";
            this.csvColumnHiding.Columns.Add(dataColumn1);
            this.csvColumnHiding.FieldSeparators = new char[] {
        ','};
            this.csvColumnHiding.Name = "csvColumnHiding";
            this.csvColumnHiding.RecordSeparators = new char[] {
        '\r',
        '\n'};
            this.csvColumnHiding.Source = "NoOfMaleEmployee\r\nNoOfFemaleEmployee\r\nTotalEmployee";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(1.8242945671081543D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm,
            this.textBox3,
            this.textBox1,
            this.textBox7,
            this.textBox5,
            this.textBox13,
            this.textBox12,
            this.textBox15,
            this.textBox14});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.3437497615814209D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.1753377914428711D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.734230637550354D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6733769178390503D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4458818435668945D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24690476059913635D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20611183345317841D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.textBox4);
            this.tblEmployeeSumm.Body.SetCellContent(0, 4, this.textBox6);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.textBox8);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.textBox9);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.textBox10);
            this.tblEmployeeSumm.Body.SetCellContent(1, 4, this.textBox11);
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox19);
            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.textBox20, 1, 2);
            tableGroup1.Name = "group2";
            tableGroup1.ReportItem = this.textBox18;
            tableGroup2.Name = "tableGroup";
            tableGroup2.ReportItem = this.textBox45;
            tableGroup3.Filters.Add(new Telerik.Reporting.Filter("=\'NoOfMaleEmployee\'", Telerik.Reporting.FilterOperator.In, "= Parameters.columns.Value"));
            tableGroup3.Name = "NoOfMaleEmployee";
            tableGroup3.ReportItem = this.NoOfMaleEmployee;
            tableGroup4.Name = "tableGroup1";
            tableGroup4.ReportItem = this.NoOfFemaleEmployee;
            tableGroup5.Name = "tableGroup2";
            tableGroup5.ReportItem = this.TotalEmployee;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup1);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup2);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup3);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup4);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup5);
            this.tblEmployeeSumm.DataSource = this.empSumaryObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox19,
            this.textBox2,
            this.textBox8,
            this.textBox4,
            this.textBox6,
            this.textBox20,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.textBox18,
            this.textBox45,
            this.NoOfMaleEmployee,
            this.NoOfFemaleEmployee,
            this.TotalEmployee});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0011619568103924394D), Telerik.Reporting.Drawing.Unit.Inch(0.69425481557846069D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup6.Name = "detailTableGroup";
            tableGroup7.Name = "group1";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup6);
            this.tblEmployeeSumm.RowGroups.Add(tableGroup7);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.37257719039917D), Telerik.Reporting.Drawing.Unit.Inch(0.6999213695526123D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("= Fields.index", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.Style.Font.Name = "Nyala";
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1753383874893189D), Telerik.Reporting.Drawing.Unit.Inch(0.24690479040145874D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            resources.ApplyResources(this.textBox2, "textBox2");
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6733753681182861D), Telerik.Reporting.Drawing.Unit.Inch(0.24690479040145874D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox4, "textBox4");
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4458819627761841D), Telerik.Reporting.Drawing.Unit.Inch(0.24690474569797516D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox6, "textBox6");
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.734230637550354D), Telerik.Reporting.Drawing.Unit.Inch(0.24690476059913635D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.StyleName = "";
            resources.ApplyResources(this.textBox8, "textBox8");
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.734230637550354D), Telerik.Reporting.Drawing.Unit.Inch(0.2061118483543396D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Italic = false;
            this.textBox9.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.StyleName = "";
            resources.ApplyResources(this.textBox9, "textBox9");
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6733753681182861D), Telerik.Reporting.Drawing.Unit.Inch(0.20611171424388886D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.StyleName = "";
            resources.ApplyResources(this.textBox10, "textBox10");
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4458819627761841D), Telerik.Reporting.Drawing.Unit.Inch(0.20611183345317841D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "";
            resources.ApplyResources(this.textBox11, "textBox11");
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34374991059303284D), Telerik.Reporting.Drawing.Unit.Inch(0.24690476059913635D));
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox19.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox19.StyleName = "";
            resources.ApplyResources(this.textBox19, "textBox19");
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5190885066986084D), Telerik.Reporting.Drawing.Unit.Inch(0.20611180365085602D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.StyleName = "";
            // 
            // empSumaryObjectDataSource
            // 
            this.empSumaryObjectDataSource.DataMember = "GetEmployeeSummary";
            this.empSumaryObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.EmployeeSummaryObjectDataSource);
            this.empSumaryObjectDataSource.Name = "empSumaryObjectDataSource";
            this.empSumaryObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("sex", typeof(string), "= Parameters.sex.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.status.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("age", typeof(string), "= Parameters.age.Value")});
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0096633909270167351D), Telerik.Reporting.Drawing.Unit.Inch(0.07551167905330658D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89576953649520874D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox3, "textBox3");
            // 
            // textBox1
            // 
            this.textBox1.Format = "{0:d}";
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.905511736869812D), Telerik.Reporting.Drawing.Unit.Inch(0.07551167905330658D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.7290329933166504D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox1.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox1, "textBox1");
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.00543289165943861D), Telerik.Reporting.Drawing.Unit.Inch(0.27559050917625427D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89999985694885254D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox7, "textBox7");
            // 
            // textBox5
            // 
            this.textBox5.Format = "{0:d}";
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.905511736869812D), Telerik.Reporting.Drawing.Unit.Inch(0.27559050917625427D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.7290327548980713D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox5.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox5, "textBox5");
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.6346235275268555D), Telerik.Reporting.Drawing.Unit.Inch(0.07551167905330658D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.4707304835319519D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox13, "textBox13");
            // 
            // textBox12
            // 
            this.textBox12.Format = "{0:d}";
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1054329872131348D), Telerik.Reporting.Drawing.Unit.Inch(0.07551167905330658D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4291908740997315D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox12.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox12, "textBox12");
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.6346235275268555D), Telerik.Reporting.Drawing.Unit.Inch(0.27559050917625427D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.47073078155517578D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox15, "textBox15");
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1054329872131348D), Telerik.Reporting.Drawing.Unit.Inch(0.27559050917625427D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4291908740997315D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox14, "textBox14");
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.19999933242797852D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.Page});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1749224662780762D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox16, "textBox16");
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.6291904449462891D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.54565298557281494D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.Page, "Page");
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.70783436298370361D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.subReport1});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0085014346987009048D), Telerik.Reporting.Drawing.Unit.Inch(0.58972424268722534D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.3652377128601074D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // subReport1
            // 
            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(0.021593643352389336D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormTitle", "= Parameters.FormTitle.Value"));
            instanceReportSource1.Parameters.Add(new Telerik.Reporting.Parameter("FormNumber", "= Parameters.FormNumber.Value"));
            instanceReportSource1.ReportDocument = this.headerSubReport2;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(16.167703628540039D), Telerik.Reporting.Drawing.Unit.Cm(1.1978991031646729D));
            // 
            // Localization
            // 
            this.Localization.Name = "Localization";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptEmpAge
            // 
            this.DataSource = this.csvColumnHiding;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report1";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.ageObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter1.Name = "age";
            resources.ApplyResources(reportParameter1, "reportParameter1");
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter2.Name = "branchId";
            resources.ApplyResources(reportParameter2, "reportParameter2");
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter3.Name = "orgStructureId";
            resources.ApplyResources(reportParameter3, "reportParameter3");
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.SexODS;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter4.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter4.Name = "sex";
            resources.ApplyResources(reportParameter4, "reportParameter4");
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.StatusODS;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter5.Name = "status";
            resources.ApplyResources(reportParameter5, "reportParameter5");
            reportParameter5.Visible = true;
            reportParameter6.AllowNull = true;
            reportParameter6.AutoRefresh = true;
            reportParameter6.AvailableValues.DataSource = this.csvColumnHiding;
            reportParameter6.AvailableValues.DisplayMember = "= Fields.Column1";
            reportParameter6.AvailableValues.ValueMember = "= Fields.Column1";
            reportParameter6.MultiValue = true;
            reportParameter6.Name = "columns";
            resources.ApplyResources(reportParameter6, "reportParameter6");
            reportParameter6.Value = "= AllValues(Fields.Column1,true)";
            reportParameter6.Visible = true;
            reportParameter7.AllowNull = true;
            reportParameter7.Name = "FormTitle";
            reportParameter8.AllowNull = true;
            reportParameter8.Name = "FormNumber";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            this.ReportParameters.Add(reportParameter6);
            this.ReportParameters.Add(reportParameter7);
            this.ReportParameters.Add(reportParameter8);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.37795352935791D);
            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.ObjectDataSource ageObjectDataSource;
        private Telerik.Reporting.ObjectDataSource empSumaryObjectDataSource;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox NoOfFemaleEmployee;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox TotalEmployee;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.ObjectDataSource Localization;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox NoOfMaleEmployee;
        private Telerik.Reporting.ObjectDataSource SexODS;
        private Telerik.Reporting.ObjectDataSource StatusODS;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.CsvDataSource csvColumnHiding;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.SubReport subReport1;
        private Telerik.Reporting.Shape shape3;
        private SharedDesigners.HeaderSubReport headerSubReport2;
    }
}