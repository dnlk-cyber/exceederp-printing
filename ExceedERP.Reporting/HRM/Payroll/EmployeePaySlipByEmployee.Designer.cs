using ExceedERP.Reporting.HRM.Payroll.DataSource;

namespace ExceedERP.Reporting.HRM.Payroll
{
    partial class EmployeePaySlipByEmployee
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup15 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup16 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup17 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup18 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup19 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup20 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup21 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup22 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup23 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup24 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup25 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup26 = new Telerik.Reporting.TableGroup();
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.periodObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.costCenterObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.empStatusObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox32 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox33 = new Telerik.Reporting.TextBox();
            this.table3 = new Telerik.Reporting.Table();
            this.textBox35 = new Telerik.Reporting.TextBox();
            this.textBox37 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.PaySlipObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.employeeObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // periodObjectDataSource
            // 
            this.periodObjectDataSource.DataMember = "GetPayrollPeriods";
            this.periodObjectDataSource.DataSource = typeof(PayrollPeriodsObjectDataSource);
            this.periodObjectDataSource.Name = "periodObjectDataSource";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            // 
            // costCenterObjectDataSource
            // 
            this.costCenterObjectDataSource.DataMember = "GetPayrollCostCenters";
            this.costCenterObjectDataSource.DataSource = typeof(PayrollCostCenterObjectDataSource);
            this.costCenterObjectDataSource.Name = "costCenterObjectDataSource";
            // 
            // empStatusObjectDataSource
            // 
            this.empStatusObjectDataSource.DataMember = "GetEmpStatus";
            this.empStatusObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.EmployeeStatusObjectDataSource);
            this.empStatusObjectDataSource.Name = "empStatusObjectDataSource";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detail.Name = "detail";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.74896240234375D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.81753408908844D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5729182958602905D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4375005960464478D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3854167461395264D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.208332896232605D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.11900615692138672D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23801231384277344D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24221353232860565D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.39260056614875793D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.11900615692138672D)));
            this.table1.Body.SetCellContent(1, 0, this.textBox27, 1, 6);
            this.table1.Body.SetCellContent(2, 0, this.textBox2, 1, 6);
            this.table1.Body.SetCellContent(3, 0, this.textBox3, 1, 6);
            this.table1.Body.SetCellContent(4, 0, this.textBox8);
            this.table1.Body.SetCellContent(4, 1, this.textBox13);
            this.table1.Body.SetCellContent(4, 2, this.textBox16);
            this.table1.Body.SetCellContent(4, 5, this.textBox23);
            this.table1.Body.SetCellContent(4, 3, this.textBox25);
            this.table1.Body.SetCellContent(5, 0, this.table2, 1, 2);
            this.table1.Body.SetCellContent(5, 2, this.table3, 1, 4);
            this.table1.Body.SetCellContent(6, 0, this.textBox9, 1, 6);
            this.table1.Body.SetCellContent(0, 0, this.textBox14, 1, 6);
            this.table1.Body.SetCellContent(4, 4, this.textBox5);
            tableGroup13.Name = "tableGroup2";
            tableGroup14.Name = "tableGroup6";
            tableGroup15.Name = "group9";
            tableGroup16.Name = "group14";
            tableGroup17.Name = "group15";
            tableGroup18.Name = "group8";
            this.table1.ColumnGroups.Add(tableGroup13);
            this.table1.ColumnGroups.Add(tableGroup14);
            this.table1.ColumnGroups.Add(tableGroup15);
            this.table1.ColumnGroups.Add(tableGroup16);
            this.table1.ColumnGroups.Add(tableGroup17);
            this.table1.ColumnGroups.Add(tableGroup18);
            this.table1.DataSource = this.PaySlipObjectDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14,
            this.textBox27,
            this.textBox2,
            this.textBox3,
            this.textBox8,
            this.textBox13,
            this.textBox16,
            this.textBox25,
            this.textBox5,
            this.textBox23,
            this.table2,
            this.table3,
            this.textBox9});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.table1.Name = "table1";
            tableGroup20.Name = "group12";
            tableGroup21.Name = "group4";
            tableGroup22.Name = "group5";
            tableGroup23.Name = "group6";
            tableGroup24.Name = "group13";
            tableGroup25.Name = "group7";
            tableGroup26.Name = "group3";
            tableGroup19.ChildGroups.Add(tableGroup20);
            tableGroup19.ChildGroups.Add(tableGroup21);
            tableGroup19.ChildGroups.Add(tableGroup22);
            tableGroup19.ChildGroups.Add(tableGroup23);
            tableGroup19.ChildGroups.Add(tableGroup24);
            tableGroup19.ChildGroups.Add(tableGroup25);
            tableGroup19.ChildGroups.Add(tableGroup26);
            tableGroup19.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup19.Name = "detailTableGroup2";
            this.table1.RowGroups.Add(tableGroup19);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(1.5908387899398804D));
            this.table1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.table1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            // 
            // textBox27
            // 
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(0.23801229894161224D));
            this.textBox27.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox27.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox27.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox27.Style.Font.Bold = true;
            this.textBox27.Style.Font.Name = "Segoe UI";
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
            this.textBox27.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox27.Value = "= Fields.Company";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(0.24221353232860565D));
            this.textBox2.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.Font.Bold = false;
            this.textBox2.Style.Font.Name = "Segoe UI";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox2.StyleName = "";
            this.textBox2.Value = "Employee Name:  {= Fields.EmpFullName}\t      Employee ID:  {= Fields.EmpID}   Mon" +
    "th:  {= Fields.PayrollPeriodName}   Year: {=Fields.FiscalYear}    Working Days: " +
    "{=Fields.WorkingDays}  ";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.LightGray;
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.Font.Bold = false;
            this.textBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox3.StyleName = "";
            this.textBox3.Value = "Department  : {=Fields.Deparatment} \t\t Bank Branch : {=Fields.BranchName} \t      " +
    "    Net Pay :  {=Fields.NetPay}";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7489621639251709D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox8.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Font.Italic = true;
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox8.StyleName = "";
            this.textBox8.Value = "EARNINGS";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8175348043441773D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Style.Font.Italic = true;
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox13.StyleName = "";
            this.textBox13.Value = "AMOUNT";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5729180574417114D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox16.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Italic = true;
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox16.StyleName = "";
            this.textBox16.Value = "DEDUCTIONS";
            // 
            // textBox23
            // 
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.208332896232605D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox23.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox23.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Style.Font.Italic = true;
            this.textBox23.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox23.StyleName = "";
            this.textBox23.Value = "Out Standing";
            // 
            // textBox25
            // 
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4375001192092896D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox25.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox25.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Italic = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox25.StyleName = "";
            this.textBox25.Value = "AMOUNT";
            // 
            // table2
            // 
            this.table2.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.PayrollProcessAdditions"));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.7524245977401733D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8140717744827271D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20006997883319855D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19253064692020416D)));
            this.table2.Body.SetCellContent(0, 0, this.textBox29);
            this.table2.Body.SetCellContent(0, 1, this.textBox32);
            this.table2.Body.SetCellContent(1, 0, this.textBox4);
            this.table2.Body.SetCellContent(1, 1, this.textBox33);
            tableGroup1.Name = "tableGroup7";
            tableGroup2.Name = "tableGroup8";
            this.table2.ColumnGroups.Add(tableGroup1);
            this.table2.ColumnGroups.Add(tableGroup2);
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox29,
            this.textBox32,
            this.textBox4,
            this.textBox33});
            this.table2.Name = "table2";
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup3";
            tableGroup4.Name = "group10";
            this.table2.RowGroups.Add(tableGroup3);
            this.table2.RowGroups.Add(tableGroup4);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.5664963722229004D), Telerik.Reporting.Drawing.Unit.Inch(0.39260062575340271D));
            this.table2.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.table2.Style.Font.Italic = true;
            this.table2.StyleName = "";
            // 
            // textBox29
            // 
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7524243593215942D), Telerik.Reporting.Drawing.Unit.Inch(0.20006999373435974D));
            this.textBox29.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox29.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox29.Value = "= Fields.Name";
            // 
            // textBox32
            // 
            this.textBox32.Format = "{0:N2}";
            this.textBox32.Name = "textBox32";
            this.textBox32.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8140718936920166D), Telerik.Reporting.Drawing.Unit.Inch(0.20006999373435974D));
            this.textBox32.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox32.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox32.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox32.Value = "= Fields.Amount";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7524243593215942D), Telerik.Reporting.Drawing.Unit.Inch(0.19253063201904297D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox4.StyleName = "";
            this.textBox4.Value = "Total Earnings";
            // 
            // textBox33
            // 
            this.textBox33.Format = "{0:N2}";
            this.textBox33.Name = "textBox33";
            this.textBox33.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8140718936920166D), Telerik.Reporting.Drawing.Unit.Inch(0.19253063201904297D));
            this.textBox33.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox33.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox33.Style.Font.Bold = true;
            this.textBox33.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox33.StyleName = "";
            this.textBox33.Value = "=  Sum(Fields.Amount)";
            // 
            // table3
            // 
            this.table3.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.PayrollProcessDeductions"));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.592887282371521D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4194951057434082D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3745536804199219D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2172330617904663D)));
            this.table3.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19850420951843262D)));
            this.table3.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19409637153148651D)));
            this.table3.Body.SetCellContent(0, 0, this.textBox35);
            this.table3.Body.SetCellContent(0, 1, this.textBox37);
            this.table3.Body.SetCellContent(1, 0, this.textBox1);
            this.table3.Body.SetCellContent(1, 1, this.textBox7);
            this.table3.Body.SetCellContent(0, 3, this.textBox18);
            this.table3.Body.SetCellContent(1, 3, this.textBox10);
            this.table3.Body.SetCellContent(0, 2, this.textBox6);
            this.table3.Body.SetCellContent(1, 2, this.textBox11);
            tableGroup5.Name = "tableGroup10";
            tableGroup6.Name = "tableGroup11";
            tableGroup7.Name = "group16";
            tableGroup8.Name = "group11";
            this.table3.ColumnGroups.Add(tableGroup5);
            this.table3.ColumnGroups.Add(tableGroup6);
            this.table3.ColumnGroups.Add(tableGroup7);
            this.table3.ColumnGroups.Add(tableGroup8);
            this.table3.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox35,
            this.textBox37,
            this.textBox6,
            this.textBox18,
            this.textBox1,
            this.textBox7,
            this.textBox11,
            this.textBox10});
            this.table3.Name = "table3";
            tableGroup10.Name = "group";
            tableGroup9.ChildGroups.Add(tableGroup10);
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detailTableGroup4";
            tableGroup12.Name = "group2";
            tableGroup11.ChildGroups.Add(tableGroup12);
            tableGroup11.Name = "group1";
            this.table3.RowGroups.Add(tableGroup9);
            this.table3.RowGroups.Add(tableGroup11);
            this.table3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.6041688919067383D), Telerik.Reporting.Drawing.Unit.Inch(0.39260059595108032D));
            this.table3.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.table3.StyleName = "";
            // 
            // textBox35
            // 
            this.textBox35.Name = "textBox35";
            this.textBox35.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5928871631622315D), Telerik.Reporting.Drawing.Unit.Inch(0.19850420951843262D));
            this.textBox35.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox35.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox35.Style.Font.Italic = true;
            this.textBox35.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox35.Value = "= Fields.Name";
            // 
            // textBox37
            // 
            this.textBox37.Format = "{0:N2}";
            this.textBox37.Name = "textBox37";
            this.textBox37.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4194953441619873D), Telerik.Reporting.Drawing.Unit.Inch(0.19850420951843262D));
            this.textBox37.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox37.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox37.Style.Font.Italic = true;
            this.textBox37.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox37.Value = "= Fields.Amount";
            // 
            // textBox1
            // 
            this.textBox1.Format = "";
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5928871631622315D), Telerik.Reporting.Drawing.Unit.Inch(0.1940964013338089D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Italic = true;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Total Deduction";
            // 
            // textBox7
            // 
            this.textBox7.Format = "{0:N2}";
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4194953441619873D), Telerik.Reporting.Drawing.Unit.Inch(0.1940964013338089D));
            this.textBox7.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Italic = true;
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox7.StyleName = "";
            this.textBox7.Value = "=  Sum(Fields.Amount)";
            // 
            // textBox18
            // 
            this.textBox18.Format = "{0:N2}";
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2172330617904663D), Telerik.Reporting.Drawing.Unit.Inch(0.19850420951843262D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.Font.Italic = true;
            this.textBox18.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox18.StyleName = "";
            this.textBox18.Value = "=Fields.OutstandingAmount";
            // 
            // textBox10
            // 
            this.textBox10.Format = "{0:N2}";
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2172330617904663D), Telerik.Reporting.Drawing.Unit.Inch(0.19409637153148651D));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.Font.Italic = true;
            this.textBox10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox10.StyleName = "";
            this.textBox10.Value = "=Sum(Fields.OutstandingAmount)";
            // 
            // textBox6
            // 
            this.textBox6.Format = "{0:#.}";
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3745532035827637D), Telerik.Reporting.Drawing.Unit.Inch(0.19850420951843262D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.Font.Italic = true;
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.textBox6.StyleName = "";
            this.textBox6.Value = "=Fields.Months";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3745532035827637D), Telerik.Reporting.Drawing.Unit.Inch(0.1940964013338089D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Italic = true;
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox11.StyleName = "";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(0.11900614947080612D));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.Khaki;
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderStyle.Right = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox9.Style.Font.Italic = true;
            this.textBox9.Style.LineColor = System.Drawing.Color.Crimson;
            this.textBox9.StyleName = "";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.17066478729248D), Telerik.Reporting.Drawing.Unit.Inch(0.11900614947080612D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.Khaki;
            this.textBox14.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.Font.Name = "Segoe UI";
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox14.StyleName = "";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3854166269302368D), Telerik.Reporting.Drawing.Unit.Inch(0.24000002443790436D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.Transparent;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.Style.Font.Italic = true;
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox5.StyleName = "";
            this.textBox5.Value = "Months";
            // 
            // PaySlipObjectDataSource
            // 
            this.PaySlipObjectDataSource.DataMember = "GetLoggedinEmployeePaySlipReport";
            this.PaySlipObjectDataSource.DataSource = typeof(EmployeePaySlipObjectDataSource);
            this.PaySlipObjectDataSource.Name = "PaySlipObjectDataSource";
            this.PaySlipObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("period", typeof(string), "= Parameters.period.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("employee", typeof(string), "= Parameters.employee.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.58722180128097534D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox19});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.pageFooterSection1.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.None;
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.7833614349365234D), Telerik.Reporting.Drawing.Unit.Inch(0.38722166419029236D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4081364870071411D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.Style.Font.Name = "Segoe UI";
            this.textBox19.Value = "Page: {PageNumber}";
            // 
            // employeeObjectDataSource
            // 
            this.employeeObjectDataSource.DataMember = "GetEmployeesList";
            this.employeeObjectDataSource.DataSource = typeof(EmployeeListObjectDataSource);
            this.employeeObjectDataSource.Name = "employeeObjectDataSource";
            // 
            // EmployeePaySlipByEmployee
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail,
            this.pageFooterSection1});
            this.Name = "Report1";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AutoRefresh = true;
            reportParameter1.AvailableValues.DataSource = this.periodObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter1.Name = "period";
            reportParameter1.Text = "Period";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AutoRefresh = true;
            reportParameter2.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter2.Name = "branchId";
            reportParameter2.Text = "Branch";
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.costCenterObjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter3.Name = "costCenter";
            reportParameter3.Text = "Cost Center";
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.empStatusObjectDataSource;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter4.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter4.Name = "empStatus";
            reportParameter4.Text = "Employee Status";
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter5.Name = "employee";
            reportParameter5.Text = "Employee Id";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            this.Style.Font.Name = "Arial Unicode MS";
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(9.1914968490600586D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.ObjectDataSource periodObjectDataSource;
        private Telerik.Reporting.ObjectDataSource empStatusObjectDataSource;
        private Telerik.Reporting.ObjectDataSource employeeObjectDataSource;
        private Telerik.Reporting.ObjectDataSource costCenterObjectDataSource;
        private Telerik.Reporting.ObjectDataSource PaySlipObjectDataSource;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox32;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox33;
        private Telerik.Reporting.Table table3;
        private Telerik.Reporting.TextBox textBox35;
        private Telerik.Reporting.TextBox textBox37;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox5;
    }
}