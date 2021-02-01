using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptVacantPostion
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.Department = new Telerik.Reporting.TextBox();
            this.JobTitle = new Telerik.Reporting.TextBox();
            this.TobeStaff = new Telerik.Reporting.TextBox();
            this.CurrentStaf = new Telerik.Reporting.TextBox();
            this.VacantPostion = new Telerik.Reporting.TextBox();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.DepartmentObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.Total = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.DetailVacantPosObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.VacantPosition = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Department
            // 
            this.Department.Name = "Department";
            this.Department.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8836730718612671D), Telerik.Reporting.Drawing.Unit.Inch(0.406509667634964D));
            this.Department.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Department.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Department.Style.Font.Bold = true;
            this.Department.Style.Font.Name = "Nyala";
            this.Department.StyleName = "Civic.TableHeader";
            this.Department.Value = "የስራ ክፍል";
            // 
            // JobTitle
            // 
            this.JobTitle.Name = "JobTitle";
            this.JobTitle.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1946451663970947D), Telerik.Reporting.Drawing.Unit.Inch(0.406509667634964D));
            this.JobTitle.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.JobTitle.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.JobTitle.Style.Font.Bold = true;
            this.JobTitle.Style.Font.Name = "Nyala";
            this.JobTitle.StyleName = "Civic.TableHeader";
            this.JobTitle.Value = "የስራ መደብ";
            // 
            // TobeStaff
            // 
            this.TobeStaff.Name = "TobeStaff";
            this.TobeStaff.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.85045933723449707D), Telerik.Reporting.Drawing.Unit.Inch(0.406509667634964D));
            this.TobeStaff.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.TobeStaff.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TobeStaff.Style.Font.Bold = true;
            this.TobeStaff.Style.Font.Name = "Nyala";
            this.TobeStaff.StyleName = "Civic.TableHeader";
            this.TobeStaff.Value = "መሆን ያለበት";
            // 
            // CurrentStaf
            // 
            this.CurrentStaf.Name = "CurrentStaf";
            this.CurrentStaf.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.87296414375305176D), Telerik.Reporting.Drawing.Unit.Inch(0.406509667634964D));
            this.CurrentStaf.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.CurrentStaf.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.CurrentStaf.Style.Font.Bold = true;
            this.CurrentStaf.Style.Font.Name = "Nyala";
            this.CurrentStaf.StyleName = "Civic.TableHeader";
            this.CurrentStaf.Value = "አሁን ያለው";
            // 
            // VacantPostion
            // 
            this.VacantPostion.Name = "VacantPostion";
            this.VacantPostion.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D), Telerik.Reporting.Drawing.Unit.Inch(0.406509667634964D));
            this.VacantPostion.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.VacantPostion.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.VacantPostion.Style.Font.Bold = true;
            this.VacantPostion.Style.Font.Name = "Nyala";
            this.VacantPostion.StyleName = "Civic.TableHeader";
            this.VacantPostion.Value = "ክፍት ቦታ";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            // 
            // DepartmentObjectDataSource
            // 
            this.DepartmentObjectDataSource.DataMember = "GetDepartment";
            this.DepartmentObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.DepartmentObjectDataSource.Name = "DepartmentObjectDataSource";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3000396490097046D);
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
            this.textBox17});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0041910805739462376D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8017807006835938D), Telerik.Reporting.Drawing.Unit.Inch(1.300039529800415D));
            this.panel1.Style.Font.Name = "Nyala";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8016629219055176D), Telerik.Reporting.Drawing.Unit.Inch(1.0998821258544922D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5015449523925781D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Nyala";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(1.0998427867889404D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5015449523925781D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.32507848739624D), Telerik.Reporting.Drawing.Unit.Inch(1.0999608039855957D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.47065195441246033D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Value = "Date:";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.795809268951416D), Telerik.Reporting.Drawing.Unit.Inch(1.0999608039855957D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.949725329875946D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Value = "= Fields.Date";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.89999991655349731D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.8836730718612671D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.1946451663970947D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.85045933723449707D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.87296414375305176D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999982118606567D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.46636733412742615D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.table1, 1, 4);
            tableGroup7.Name = "tableGroup";
            tableGroup7.ReportItem = this.Department;
            tableGroup8.Name = "group";
            tableGroup8.ReportItem = this.JobTitle;
            tableGroup9.Name = "tableGroup1";
            tableGroup9.ReportItem = this.TobeStaff;
            tableGroup10.Name = "tableGroup2";
            tableGroup10.ReportItem = this.CurrentStaf;
            tableGroup11.Name = "group2";
            tableGroup11.ReportItem = this.VacantPostion;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup7);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup8);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup9);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup10);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup11);
            this.tblEmployeeSumm.DataSource = this.DetailVacantPosObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.table1,
            this.Department,
            this.JobTitle,
            this.TobeStaff,
            this.CurrentStaf,
            this.VacantPostion});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup12.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup12.Name = "detailTableGroup";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup12);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8017416000366211D), Telerik.Reporting.Drawing.Unit.Inch(0.87287700176239014D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.8836729526519775D), Telerik.Reporting.Drawing.Unit.Inch(0.46636730432510376D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Value = "= Fields.Department";
            // 
            // table1
            // 
            this.table1.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.vacantDetail"));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.2065296173095703D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.83904993534088135D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.89124405384063721D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.981244683265686D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.24312831461429596D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.223239004611969D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox6);
            this.table1.Body.SetCellContent(0, 1, this.textBox18);
            this.table1.Body.SetCellContent(0, 2, this.textBox20);
            this.table1.Body.SetCellContent(0, 3, this.textBox22);
            this.table1.Body.SetCellContent(1, 0, this.Total);
            this.table1.Body.SetCellContent(1, 1, this.textBox4);
            this.table1.Body.SetCellContent(1, 2, this.textBox8);
            this.table1.Body.SetCellContent(1, 3, this.textBox9);
            tableGroup1.Name = "tableGroup3";
            tableGroup2.Name = "tableGroup4";
            tableGroup3.Name = "tableGroup5";
            tableGroup4.Name = "group3";
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox6,
            this.textBox18,
            this.textBox20,
            this.textBox22,
            this.Total,
            this.textBox4,
            this.textBox8,
            this.textBox9});
            this.table1.Name = "table1";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup1";
            tableGroup6.Name = "group1";
            this.table1.RowGroups.Add(tableGroup5);
            this.table1.RowGroups.Add(tableGroup6);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.9180684089660645D), Telerik.Reporting.Drawing.Unit.Inch(0.46636730432510376D));
            this.table1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.table1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.table1.StyleName = "";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2065298557281494D), Telerik.Reporting.Drawing.Unit.Inch(0.24312831461429596D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Value = "=Fields.Jobtitle";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8390500545501709D), Telerik.Reporting.Drawing.Unit.Inch(0.24312831461429596D));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Value = "=Fields.Shouldbe";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.891244113445282D), Telerik.Reporting.Drawing.Unit.Inch(0.24312831461429596D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Value = "=Fields.Current";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98124480247497559D), Telerik.Reporting.Drawing.Unit.Inch(0.24312831461429596D));
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.StyleName = "";
            this.textBox22.Value = "=Fields.Vacant";
            // 
            // Total
            // 
            this.Total.Name = "Total";
            this.Total.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2065298557281494D), Telerik.Reporting.Drawing.Unit.Inch(0.22323895990848541D));
            this.Total.Style.BackgroundColor = System.Drawing.Color.CornflowerBlue;
            this.Total.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Total.Style.Font.Bold = true;
            this.Total.StyleName = "";
            this.Total.Value = "ድምር";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8390500545501709D), Telerik.Reporting.Drawing.Unit.Inch(0.22323895990848541D));
            this.textBox4.Style.BackgroundColor = System.Drawing.Color.CornflowerBlue;
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.StyleName = "";
            this.textBox4.Value = "=Sum(Fields.Shouldbe)";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.891244113445282D), Telerik.Reporting.Drawing.Unit.Inch(0.22323895990848541D));
            this.textBox8.Style.BackgroundColor = System.Drawing.Color.CornflowerBlue;
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "=Sum(Fields.Current)";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.98124480247497559D), Telerik.Reporting.Drawing.Unit.Inch(0.22323895990848541D));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.CornflowerBlue;
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "=Sum(Fields.Vacant)";
            // 
            // DetailVacantPosObjectDataSource
            // 
            this.DetailVacantPosObjectDataSource.DataMember = "GetVacantPostionByDept";
            this.DetailVacantPosObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.VacantPostionsByDepartmentObjectDataSource);
            this.DetailVacantPosObjectDataSource.Name = "DetailVacantPosObjectDataSource";
            this.DetailVacantPosObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("org", typeof(string), "= Parameters.org.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value")});
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
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2311687469482422D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "Page: {PageNumber}";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7929439544677734D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.43814635276794434D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.39996051788330078D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.VacantPosition});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.29996046423912048D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.805931568145752D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // VacantPosition
            // 
            this.VacantPosition.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.VacantPosition.Name = "VacantPosition";
            this.VacantPosition.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8016238212585449D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.VacantPosition.Style.Font.Bold = true;
            this.VacantPosition.Style.Font.Name = "Nyala";
            this.VacantPosition.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.VacantPosition.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.VacantPosition.Value = "Vacant Position";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptVacantPostion
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report1";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter1.Name = "branchId";
            reportParameter1.Text = "ቅርንጫፍ";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.DepartmentObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.Name = "org";
            reportParameter2.Text = "የስራ ክፍል";
            reportParameter2.Value = "";
            reportParameter2.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.805971622467041D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox Department;
        private Telerik.Reporting.TextBox TobeStaff;
        private Telerik.Reporting.TextBox JobTitle;
        private Telerik.Reporting.TextBox CurrentStaf;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox VacantPosition;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox VacantPostion;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox Total;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.ObjectDataSource DepartmentObjectDataSource;
        private Telerik.Reporting.ObjectDataSource DetailVacantPosObjectDataSource;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.TextBox Page;
    }
}