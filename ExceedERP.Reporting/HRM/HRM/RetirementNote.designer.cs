using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class RetirementNote
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.IdNum = new Telerik.Reporting.TextBox();
            this.FullName = new Telerik.Reporting.TextBox();
            this.Sex = new Telerik.Reporting.TextBox();
            this.RemainingYear = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.RetirementDate = new Telerik.Reporting.TextBox();
            this.NotificationNumberObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.RetirementNotificationObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.Page = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.RetirementNotification = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34374997019767761D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.textBox20.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox20.Style.Font.Bold = true;
            this.textBox20.StyleName = "Civic.TableHeader";
            this.textBox20.Value = "No";
            // 
            // IdNum
            // 
            this.IdNum.Name = "IdNum";
            this.IdNum.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93750011920928955D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.IdNum.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.IdNum.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.IdNum.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.IdNum.Style.Font.Bold = true;
            this.IdNum.Style.Font.Name = "Nyala";
            this.IdNum.StyleName = "Civic.TableHeader";
            this.IdNum.Value = "Employee Id";
            // 
            // FullName
            // 
            this.FullName.Name = "FullName";
            this.FullName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7083321809768677D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.FullName.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.FullName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.FullName.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.FullName.Style.Font.Bold = true;
            this.FullName.Style.Font.Name = "Nyala";
            this.FullName.StyleName = "Civic.TableHeader";
            this.FullName.Value = "Full Name";
            // 
            // Sex
            // 
            this.Sex.Name = "Sex";
            this.Sex.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.87180006504058838D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.Sex.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Sex.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Sex.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.Sex.Style.Font.Bold = true;
            this.Sex.Style.Font.Name = "Nyala";
            this.Sex.StyleName = "Civic.TableHeader";
            this.Sex.Value = "Sex";
            // 
            // RemainingYear
            // 
            this.RemainingYear.Name = "RemainingYear";
            this.RemainingYear.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4927809238433838D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.RemainingYear.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.RemainingYear.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.RemainingYear.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.RemainingYear.Style.Font.Bold = true;
            this.RemainingYear.Style.Font.Name = "Nyala";
            this.RemainingYear.StyleName = "Civic.TableHeader";
            this.RemainingYear.Value = "Remaining Days";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.textBox12.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.StyleName = "Civic.TableHeader";
            this.textBox12.Value = "Birth Date";
            // 
            // RetirementDate
            // 
            this.RetirementDate.Name = "RetirementDate";
            this.RetirementDate.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3082317113876343D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.RetirementDate.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.RetirementDate.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.RetirementDate.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.RetirementDate.Style.Font.Bold = true;
            this.RetirementDate.Style.Font.Name = "Nyala";
            this.RetirementDate.StyleName = "Civic.TableHeader";
            this.RetirementDate.Value = "Retirement Date";
            // 
            // NotificationNumberObjectDataSource
            // 
            this.NotificationNumberObjectDataSource.DataMember = "GetRetireNofification";
            this.NotificationNumberObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.RetireNofificationNumberObjectDataSource);
            this.NotificationNumberObjectDataSource.Name = "NotificationNumberObjectDataSource";
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
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14,
            this.Date,
            this.panel2});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D));
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.99992132186889648D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1163666248321533D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Style.Font.Name = "Nyala";
            this.textBox14.Style.Font.Underline = true;
            this.textBox14.Value = "= Fields.Date";
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5D), Telerik.Reporting.Drawing.Unit.Inch(0.99992132186889648D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.39811199903488159D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Style.Font.Name = "Nyala";
            this.Date.Value = "Date:";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.1000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.99980306625366211D));
            this.panel2.Style.Font.Name = "Nyala";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(0.3000786304473877D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.19984245300293D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99976366758346558D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3001575469970703D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.1998028755187988D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.80000019073486328D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.34374991059303284D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.93750011920928955D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.7083340883255005D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.8717997670173645D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4927808046340942D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.999999463558197D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3082315921783447D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958347737789154D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999995827674866D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 4, this.textBox4);
            this.tblEmployeeSumm.Body.SetCellContent(0, 6, this.textBox6);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.textBox8);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.textBox10);
            this.tblEmployeeSumm.Body.SetCellContent(1, 1, this.textBox3);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.textBox5);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.textBox7);
            this.tblEmployeeSumm.Body.SetCellContent(1, 4, this.textBox9);
            this.tblEmployeeSumm.Body.SetCellContent(1, 6, this.textBox11);
            this.tblEmployeeSumm.Body.SetCellContent(0, 5, this.textBox13);
            this.tblEmployeeSumm.Body.SetCellContent(1, 5, this.textBox15);
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox21);
            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.textBox22);
            tableGroup1.Name = "group5";
            tableGroup1.ReportItem = this.textBox20;
            tableGroup2.Name = "group1";
            tableGroup2.ReportItem = this.IdNum;
            tableGroup3.Name = "group2";
            tableGroup3.ReportItem = this.FullName;
            tableGroup4.Name = "group";
            tableGroup4.ReportItem = this.Sex;
            tableGroup5.Name = "tableGroup1";
            tableGroup5.ReportItem = this.RemainingYear;
            tableGroup6.Name = "group4";
            tableGroup6.ReportItem = this.textBox12;
            tableGroup7.Name = "tableGroup2";
            tableGroup7.ReportItem = this.RetirementDate;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup1);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup2);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup3);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup4);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup5);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup6);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup7);
            this.tblEmployeeSumm.DataSource = this.RetirementNotificationObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox21,
            this.textBox10,
            this.textBox2,
            this.textBox8,
            this.textBox4,
            this.textBox13,
            this.textBox6,
            this.textBox22,
            this.textBox3,
            this.textBox5,
            this.textBox7,
            this.textBox9,
            this.textBox15,
            this.textBox11,
            this.textBox20,
            this.IdNum,
            this.FullName,
            this.Sex,
            this.RemainingYear,
            this.textBox12,
            this.RetirementDate});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.047916889190673828D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup8.Name = "detailTableGroup";
            tableGroup9.Name = "group3";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup8);
            this.tblEmployeeSumm.RowGroups.Add(tableGroup9);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.66239595413208D), Telerik.Reporting.Drawing.Unit.Inch(0.67916673421859741D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.Style.Font.Name = "Nyala";
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4927809238433838D), Telerik.Reporting.Drawing.Unit.Inch(0.23958346247673035D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox4.Value = "= Fields.RemainingDays";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3082317113876343D), Telerik.Reporting.Drawing.Unit.Inch(0.23958347737789154D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Value = "= Fields.RetirmentDate";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.87180006504058838D), Telerik.Reporting.Drawing.Unit.Inch(0.23958346247673035D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.Sex";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7083321809768677D), Telerik.Reporting.Drawing.Unit.Inch(0.23958346247673035D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.StyleName = "";
            this.textBox2.Value = "= Fields.FullName";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93750011920928955D), Telerik.Reporting.Drawing.Unit.Inch(0.23958346247673035D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= Fields.IdNumber";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.9375004768371582D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "Total";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7083336114883423D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox5.Style.Font.Bold = true;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= Count(Fields.FullName)";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.87180006504058838D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.StyleName = "";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4927815198898315D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.StyleName = "";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3082317113876343D), Telerik.Reporting.Drawing.Unit.Inch(0.19999995827674866D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958347737789154D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox13.StyleName = "";
            this.textBox13.Value = "= Fields.BirthDate";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.StyleName = "";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34374997019767761D), Telerik.Reporting.Drawing.Unit.Inch(0.23958347737789154D));
            this.textBox21.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox21.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox21.StyleName = "";
            this.textBox21.Value = "= RowNumber(\"tblEmployeeSumm\")";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34374997019767761D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox22.Style.Font.Bold = true;
            this.textBox22.StyleName = "";
            // 
            // RetirementNotificationObjectDataSource
            // 
            this.RetirementNotificationObjectDataSource.DataMember = "GetNotifyRetirmentSummary";
            this.RetirementNotificationObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.NotifyRetirmentObjectDataSource);
            this.RetirementNotificationObjectDataSource.Name = "RetirementNotificationObjectDataSource";
            this.RetirementNotificationObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("value", typeof(string), "= Parameters.value.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.Page,
            this.textBox1});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.0999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(0.80000036954879761D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.50000005960464478D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.6000785827636719D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Style.Font.Name = "Nyala";
            this.textBox1.Value = "=PageNumber";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.85208320617675781D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.RetirementNotification,
            this.shape3,
            this.textBox17,
            this.textBox16,
            this.textBox19,
            this.textBox18});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // RetirementNotification
            // 
            this.RetirementNotification.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.RetirementNotification.Name = "RetirementNotification";
            this.RetirementNotification.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.19999994337558746D));
            this.RetirementNotification.Style.Font.Bold = true;
            this.RetirementNotification.Style.Font.Name = "Nyala";
            this.RetirementNotification.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.RetirementNotification.Style.Font.Underline = true;
            this.RetirementNotification.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.RetirementNotification.Value = "Retirement Notification";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.7999998927116394D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.66239595413208D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.2999211847782135D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Style.Font.Name = "Nyala";
            this.textBox17.Style.Font.Underline = true;
            this.textBox17.Value = "= Parameters.branchId.Label";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.2999211847782135D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.49992135167121887D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "Branch";
            // 
            // textBox19
            // 
            this.textBox19.Format = "{0:d}";
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.70000004768371582D), Telerik.Reporting.Drawing.Unit.Inch(0.49999991059303284D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.0999996662139893D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox19.Style.Font.Name = "Nyala";
            this.textBox19.Style.Font.Underline = true;
            this.textBox19.Value = "= Parameters.orgStructureId.Label";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.49999991059303284D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox18.Style.Font.Bold = true;
            this.textBox18.Style.Font.Name = "Nyala";
            this.textBox18.Value = "Department";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // RetirementNote
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "RetirementNote";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.NotificationNumberObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter1.Name = "value";
            reportParameter1.Text = "No Of Year";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter2.Name = "branchId";
            reportParameter2.Text = "Branch";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter3.Name = "orgStructureId";
            reportParameter3.Text = "Work Unit";
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.7125983238220215D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox RetirementNotification;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox FullName;
        private Telerik.Reporting.TextBox Sex;
        private Telerik.Reporting.TextBox RemainingYear;
        private Telerik.Reporting.TextBox RetirementDate;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.ObjectDataSource NotificationNumberObjectDataSource;
        private Telerik.Reporting.ObjectDataSource RetirementNotificationObjectDataSource;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox IdNum;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox20;
    }
}