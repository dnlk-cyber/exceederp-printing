using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptEmpEduLevel
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.LevelofEducation = new Telerik.Reporting.TextBox();
            this.NoOfMaleEmployee = new Telerik.Reporting.TextBox();
            this.NoOfFemaleEmployee = new Telerik.Reporting.TextBox();
            this.TotalEmployee = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.Total = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.byEducationalLevel = new Telerik.Reporting.TextBox();
            this.empSumaryObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.eduLevelObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.StatusODS = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // LevelofEducation
            // 
            this.LevelofEducation.Name = "LevelofEducation";
            this.LevelofEducation.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3003398180007935D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.LevelofEducation.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.LevelofEducation.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.LevelofEducation.Style.Font.Bold = true;
            this.LevelofEducation.Style.Font.Name = "Nyala";
            this.LevelofEducation.StyleName = "Civic.TableHeader";
            this.LevelofEducation.Value = "Level of Education";
            // 
            // NoOfMaleEmployee
            // 
            this.NoOfMaleEmployee.Name = "NoOfMaleEmployee";
            this.NoOfMaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4967288970947266D), Telerik.Reporting.Drawing.Unit.Inch(0.23958331346511841D));
            this.NoOfMaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfMaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfMaleEmployee.Style.Font.Bold = true;
            this.NoOfMaleEmployee.Style.Font.Name = "Nyala";
            this.NoOfMaleEmployee.StyleName = "Civic.TableHeader";
            this.NoOfMaleEmployee.Value = "No Of Male Employee";
            // 
            // NoOfFemaleEmployee
            // 
            this.NoOfFemaleEmployee.Name = "NoOfFemaleEmployee";
            this.NoOfFemaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6525440216064453D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.NoOfFemaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfFemaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfFemaleEmployee.Style.Font.Bold = true;
            this.NoOfFemaleEmployee.Style.Font.Name = "Nyala";
            this.NoOfFemaleEmployee.StyleName = "Civic.TableHeader";
            this.NoOfFemaleEmployee.Value = "No Of Female Employee";
            // 
            // TotalEmployee
            // 
            this.TotalEmployee.Name = "TotalEmployee";
            this.TotalEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1437982320785523D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.TotalEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.TotalEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalEmployee.Style.Font.Bold = true;
            this.TotalEmployee.Style.Font.Name = "Nyala";
            this.TotalEmployee.StyleName = "Civic.TableHeader";
            this.TotalEmployee.Value = "Total Employee";
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
            this.Date,
            this.textBox17,
            this.panel2});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0041910805739462376D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5958094596862793D), Telerik.Reporting.Drawing.Unit.Inch(1.300039529800415D));
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8958091735839844D), Telerik.Reporting.Drawing.Unit.Inch(1.1000003814697266D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59565037488937378D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Style.Font.Name = "Nyala";
            this.Date.Value = "Date:";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.4958095550537109D), Telerik.Reporting.Drawing.Unit.Inch(1.1000003814697266D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0999996662139893D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Style.Font.Name = "Nyala";
            this.textBox17.Value = "= Fields.Date";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5957698822021484D), Telerik.Reporting.Drawing.Unit.Inch(1.0998821258544922D));
            this.panel2.Style.Font.Name = "Nyala";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.300157904624939D), Telerik.Reporting.Drawing.Unit.Inch(0.30007860064506531D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2932538986206055D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
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
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(1.0998427867889404D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2933335304260254D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Name = "Nyala";
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.7000001072883606D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3003401756286621D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.4967292547225952D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6525434255599976D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.1437983512878418D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.239583358168602D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.textBox4);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.textBox6);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.textBox8);
            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.Total);
            this.tblEmployeeSumm.Body.SetCellContent(1, 1, this.textBox9);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.textBox10);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.textBox11);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.LevelofEducation;
            tableGroup2.Name = "group";
            tableGroup2.ReportItem = this.NoOfMaleEmployee;
            tableGroup3.Name = "tableGroup1";
            tableGroup3.ReportItem = this.NoOfFemaleEmployee;
            tableGroup4.Name = "tableGroup2";
            tableGroup4.ReportItem = this.TotalEmployee;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup1);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup2);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup3);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup4);
            this.tblEmployeeSumm.DataSource = this.empSumaryObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox8,
            this.textBox4,
            this.textBox6,
            this.Total,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.LevelofEducation,
            this.NoOfMaleEmployee,
            this.NoOfFemaleEmployee,
            this.TotalEmployee});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup";
            tableGroup6.Name = "group1";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup5);
            this.tblEmployeeSumm.RowGroups.Add(tableGroup6);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5934114456176758D), Telerik.Reporting.Drawing.Unit.Inch(0.67916667461395264D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.Style.Font.Name = "Nyala";
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3003398180007935D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833432674408D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Value = "= Fields.SummaryName";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6525440216064453D), Telerik.Reporting.Drawing.Unit.Inch(0.239583358168602D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "= Fields.NumberOfFemaleEmployee";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1437982320785523D), Telerik.Reporting.Drawing.Unit.Inch(0.23958337306976318D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Value = "= Fields.TotalEmployee";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4967288970947266D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833432674408D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.NumberOfMaleEmployee";
            // 
            // Total
            // 
            this.Total.Name = "Total";
            this.Total.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3003398180007935D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.Total.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Total.Style.Font.Bold = true;
            this.Total.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Total.StyleName = "";
            this.Total.Value = "Total";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4967288970947266D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.Font.Bold = true;
            this.textBox9.Style.Font.Italic = false;
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "= Sum(Fields.NumberOfMaleEmployee)";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6525440216064453D), Telerik.Reporting.Drawing.Unit.Inch(0.20000003278255463D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= Sum(Fields.NumberOfFemaleEmployee)";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1437982320785523D), Telerik.Reporting.Drawing.Unit.Inch(0.20000004768371582D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "= Sum(Fields.TotalEmployee)";
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
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.0228390693664551D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "=PageNumber";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5707175731658936D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.45204296708106995D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.39996051788330078D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.byEducationalLevel});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.34783777594566345D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.5976424217224121D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // byEducationalLevel
            // 
            this.byEducationalLevel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.byEducationalLevel.Name = "byEducationalLevel";
            this.byEducationalLevel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.byEducationalLevel.Style.Font.Bold = true;
            this.byEducationalLevel.Style.Font.Name = "Nyala";
            this.byEducationalLevel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.byEducationalLevel.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.byEducationalLevel.Value = "Employee Summary by Educational Level";
            // 
            // empSumaryObjectDataSource
            // 
            this.empSumaryObjectDataSource.DataMember = "GetEmployeeEduLevelSummary";
            this.empSumaryObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.EmployeeEduLevelSummaryObjectDataSource);
            this.empSumaryObjectDataSource.Name = "empSumaryObjectDataSource";
            this.empSumaryObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("orgStructureId", typeof(string), "= Parameters.orgStructureId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.status.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("eduLevel", typeof(string), "= Parameters.eduLevel.Value")});
            // 
            // eduLevelObjectDataSource
            // 
            this.eduLevelObjectDataSource.DataMember = "GetEduLevel";
            this.eduLevelObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EduLevelObjectDataSource);
            this.eduLevelObjectDataSource.Name = "eduLevelObjectDataSource";
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
            // StatusODS
            // 
            this.StatusODS.DataMember = "GetStatus";
            this.StatusODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeStatusODS);
            this.StatusODS.Name = "StatusODS";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptEmpEduLevel
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report1";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.eduLevelObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Description";
            reportParameter1.AvailableValues.ValueMember = "= Fields.LookupId";
            reportParameter1.Name = "eduLevel";
            reportParameter1.Text = "Education Level";
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
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.StatusODS;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter4.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter4.Name = "status";
            reportParameter4.Text = "Status";
            reportParameter4.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(5.6000003814697266D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ObjectDataSource eduLevelObjectDataSource;
        private Telerik.Reporting.ObjectDataSource empSumaryObjectDataSource;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox LevelofEducation;
        private Telerik.Reporting.TextBox NoOfFemaleEmployee;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox NoOfMaleEmployee;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox TotalEmployee;
        private Telerik.Reporting.TextBox Total;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox byEducationalLevel;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.ObjectDataSource StatusODS;
    }
}