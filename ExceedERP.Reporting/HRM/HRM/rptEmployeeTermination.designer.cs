using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptEmployeeTermination
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
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.IdNumber = new Telerik.Reporting.TextBox();
            this.FullName = new Telerik.Reporting.TextBox();
            this.Sex = new Telerik.Reporting.TextBox();
            this.Department = new Telerik.Reporting.TextBox();
            this.Branch = new Telerik.Reporting.TextBox();
            this.JobTitle = new Telerik.Reporting.TextBox();
            this.Reason = new Telerik.Reporting.TextBox();
            this.TerminationDate = new Telerik.Reporting.TextBox();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.ReasonObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.EmployeeTerminationObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.Termination = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // IdNumber
            // 
            this.IdNumber.Name = "IdNumber";
            this.IdNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583301544189453D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.IdNumber.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.IdNumber.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.IdNumber.Style.Font.Bold = true;
            this.IdNumber.Style.Font.Name = "Nyala";
            this.IdNumber.StyleName = "Civic.TableHeader";
            this.IdNumber.Value = "Employee Id";
            // 
            // FullName
            // 
            this.FullName.Name = "FullName";
            this.FullName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4895840883255005D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.FullName.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.FullName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.FullName.Style.Font.Bold = true;
            this.FullName.Style.Font.Name = "Nyala";
            this.FullName.StyleName = "Civic.TableHeader";
            this.FullName.Value = "Full Name";
            // 
            // Sex
            // 
            this.Sex.Name = "Sex";
            this.Sex.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.56971549987792969D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.Sex.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Sex.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Sex.Style.Font.Bold = true;
            this.Sex.Style.Font.Name = "Nyala";
            this.Sex.StyleName = "Civic.TableHeader";
            this.Sex.Value = "Sex";
            // 
            // Department
            // 
            this.Department.Name = "Department";
            this.Department.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4270831346511841D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.Department.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Department.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Department.Style.Font.Bold = true;
            this.Department.Style.Font.Name = "Nyala";
            this.Department.StyleName = "Civic.TableHeader";
            this.Department.Value = "Department";
            // 
            // Branch
            // 
            this.Branch.Name = "Branch";
            this.Branch.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.Branch.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Branch.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Branch.Style.Font.Bold = true;
            this.Branch.Style.Font.Name = "Nyala";
            this.Branch.StyleName = "Civic.TableHeader";
            this.Branch.Value = "Branch";
            // 
            // JobTitle
            // 
            this.JobTitle.Name = "JobTitle";
            this.JobTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98236745595932007D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.JobTitle.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.JobTitle.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.JobTitle.Style.Font.Bold = true;
            this.JobTitle.Style.Font.Name = "Nyala";
            this.JobTitle.StyleName = "Civic.TableHeader";
            this.JobTitle.Value = "Job Title";
            // 
            // Reason
            // 
            this.Reason.Name = "Reason";
            this.Reason.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.82291668653488159D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.Reason.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Reason.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Reason.Style.Font.Bold = true;
            this.Reason.Style.Font.Name = "Nyala";
            this.Reason.StyleName = "Civic.TableHeader";
            this.Reason.Value = "Reason";
            // 
            // TerminationDate
            // 
            this.TerminationDate.Name = "TerminationDate";
            this.TerminationDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.TerminationDate.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.TerminationDate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TerminationDate.Style.Font.Bold = true;
            this.TerminationDate.Style.Font.Name = "Nyala";
            this.TerminationDate.StyleName = "Civic.TableHeader";
            this.TerminationDate.Value = "Termination Date";
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
            // ReasonObjectDataSource
            // 
            this.ReasonObjectDataSource.DataMember = "GetTerminationType";
            this.ReasonObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.TerminationTypeObjectDataSource);
            this.ReasonObjectDataSource.Name = "ReasonObjectDataSource";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel2,
            this.Date,
            this.textBox14});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1875D), Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D));
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1875D), Telerik.Reporting.Drawing.Unit.Inch(0.99996054172515869D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.300118088722229D), Telerik.Reporting.Drawing.Unit.Inch(0.30007860064506531D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8873810768127441D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.300118088722229D), Telerik.Reporting.Drawing.Unit.Inch(3.929932790924795E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8873810768127441D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Name = "Nyala";
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.99996083974838257D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59565037488937378D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Value = "Date:";
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.7957291603088379D), Telerik.Reporting.Drawing.Unit.Inch(0.99996083974838257D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3917311429977417D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Value = "= Fields.Date";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.89583265781402588D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.48958420753479D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.56971538066864014D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4270832538604736D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.98236763477325439D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.82291632890701294D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 5, this.textBox4);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.textBox8);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 6, this.textBox6);
            this.tblEmployeeSumm.Body.SetCellContent(0, 7, this.textBox10);
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox12);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.textBox17);
            this.tblEmployeeSumm.Body.SetCellContent(0, 4, this.textBox19);
            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.textBox1);
            this.tblEmployeeSumm.Body.SetCellContent(1, 1, this.textBox3);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.textBox5);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.textBox7);
            this.tblEmployeeSumm.Body.SetCellContent(1, 4, this.textBox9);
            this.tblEmployeeSumm.Body.SetCellContent(1, 5, this.textBox11);
            this.tblEmployeeSumm.Body.SetCellContent(1, 6, this.textBox13);
            this.tblEmployeeSumm.Body.SetCellContent(1, 7, this.textBox15);
            tableGroup1.Name = "group4";
            tableGroup1.ReportItem = this.IdNumber;
            tableGroup2.Name = "group2";
            tableGroup2.ReportItem = this.FullName;
            tableGroup3.Name = "group";
            tableGroup3.ReportItem = this.Sex;
            tableGroup4.Name = "group5";
            tableGroup4.ReportItem = this.Department;
            tableGroup5.Name = "group6";
            tableGroup5.ReportItem = this.Branch;
            tableGroup6.Name = "tableGroup1";
            tableGroup6.ReportItem = this.JobTitle;
            tableGroup7.Name = "group1";
            tableGroup7.ReportItem = this.Reason;
            tableGroup8.Name = "group3";
            tableGroup8.ReportItem = this.TerminationDate;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup1);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup2);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup3);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup4);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup5);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup6);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup7);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup8);
            this.tblEmployeeSumm.DataSource = this.EmployeeTerminationObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox12,
            this.textBox2,
            this.textBox8,
            this.textBox17,
            this.textBox19,
            this.textBox4,
            this.textBox6,
            this.textBox10,
            this.textBox1,
            this.textBox3,
            this.textBox5,
            this.textBox7,
            this.textBox9,
            this.textBox11,
            this.textBox13,
            this.textBox15,
            this.IdNumber,
            this.FullName,
            this.Sex,
            this.Department,
            this.Branch,
            this.JobTitle,
            this.Reason,
            this.TerminationDate});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detailTableGroup";
            tableGroup10.Name = "group7";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup9);
            this.tblEmployeeSumm.RowGroups.Add(tableGroup10);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1874990463256836D), Telerik.Reporting.Drawing.Unit.Inch(0.67916661500930786D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98236745595932007D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox4.Value = "= Fields.JobTitle";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.56971549987792969D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.Sex";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4895840883255005D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "= Fields.FullName";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.82291668653488159D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.StyleName = "";
            this.textBox6.Value = "= Fields.Reason";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= Fields.TerminationDate";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583301544189453D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.StyleName = "";
            this.textBox12.Value = "= Fields.EmployeeId";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4270831346511841D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox17.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox17.StyleName = "";
            this.textBox17.Value = "= Fields.Departement";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.StyleName = "";
            this.textBox19.Value = "= Fields.Branch";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583265781402588D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Total";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.48958420753479D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "= Count(Fields.FullName)";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.56971538066864014D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.StyleName = "";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4270832538604736D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.StyleName = "";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.StyleName = "";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98236763477325439D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.StyleName = "";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.82291632890701294D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.StyleName = "";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.StyleName = "";
            // 
            // EmployeeTerminationObjectDataSource
            // 
            this.EmployeeTerminationObjectDataSource.DataMember = "GetEmployeeTermination";
            this.EmployeeTerminationObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.EmployeeTerminationObjectDataSource);
            this.EmployeeTerminationObjectDataSource.Name = "EmployeeTerminationObjectDataSource";
            this.EmployeeTerminationObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("reason", typeof(string), "= Parameters.reason.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("from", typeof(System.DateTime), "= Parameters.from.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("to", typeof(System.DateTime), "= Parameters.to.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.Page});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.61265754699707D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "=PageNumber";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.1503543853759766D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.46222439408302307D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.Termination});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0041910805739462376D), Telerik.Reporting.Drawing.Unit.Inch(0.40000009536743164D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1832695007324219D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // Termination
            // 
            this.Termination.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.Termination.Name = "Termination";
            this.Termination.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.1875D), Telerik.Reporting.Drawing.Unit.Inch(0.34579402208328247D));
            this.Termination.Style.Font.Bold = true;
            this.Termination.Style.Font.Name = "Nyala";
            this.Termination.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.Termination.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Termination.Value = "Termination";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptEmployeeTermination
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "rptEmployeeJobTitle";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter1.Name = "branchId";
            reportParameter1.Text = "Branch";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.Name = "orgStructureId";
            reportParameter2.Text = "Work Unit";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.ReasonObjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Description";
            reportParameter3.AvailableValues.ValueMember = "= Fields.LookupId";
            reportParameter3.Name = "reason";
            reportParameter3.Text = "Reason";
            reportParameter3.Visible = true;
            reportParameter4.Name = "from";
            reportParameter4.Text = "From";
            reportParameter4.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter4.Value = "= Today()";
            reportParameter4.Visible = true;
            reportParameter5.Name = "to";
            reportParameter5.Text = "To";
            reportParameter5.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter5.Value = "= Today()";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.1875D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox Termination;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox FullName;
        private Telerik.Reporting.TextBox Sex;
        private Telerik.Reporting.TextBox JobTitle;
        private Telerik.Reporting.TextBox Reason;
        private Telerik.Reporting.TextBox TerminationDate;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.ObjectDataSource ReasonObjectDataSource;
        private Telerik.Reporting.ObjectDataSource EmployeeTerminationObjectDataSource;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox IdNumber;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox Department;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox Branch;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
    }
}