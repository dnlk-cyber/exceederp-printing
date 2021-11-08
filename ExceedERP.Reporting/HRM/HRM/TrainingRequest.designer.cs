using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class TrainingRequest
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainingRequest));
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox45 = new Telerik.Reporting.TextBox();
            this.NoOfMaleEmployee = new Telerik.Reporting.TextBox();
            this.NoOfFemaleEmployee = new Telerik.Reporting.TextBox();
            this.TotalEmployee = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.WorkUnitObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tbltraining = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.TrainingRequestODS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.EmployeeSummarybyAge = new Telerik.Reporting.TextBox();
            this.Localization = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox45
            // 
            this.textBox45.Name = "Name";
            this.textBox45.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.36283835768699646D), Telerik.Reporting.Drawing.Unit.Inch(0.239583358168602D));
            this.textBox45.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox45.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox45.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox45.Style.Font.Bold = true;
            this.textBox45.Style.Font.Name = "Nyala";
            this.textBox45.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.textBox45, "textBox45");
            // 
            // NoOfMaleEmployee
            // 
            this.NoOfMaleEmployee.Name = "NoOfMaleEmployee";
            this.NoOfMaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5175625085830689D), Telerik.Reporting.Drawing.Unit.Inch(0.239583358168602D));
            this.NoOfMaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfMaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfMaleEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.NoOfMaleEmployee.Style.Font.Bold = true;
            this.NoOfMaleEmployee.Style.Font.Name = "Nyala";
            this.NoOfMaleEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.NoOfMaleEmployee, "NoOfMaleEmployee");
            // 
            // NoOfFemaleEmployee
            // 
            this.NoOfFemaleEmployee.Name = "NoOfFemaleEmployee";
            this.NoOfFemaleEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.673377275466919D), Telerik.Reporting.Drawing.Unit.Inch(0.239583358168602D));
            this.NoOfFemaleEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.NoOfFemaleEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.NoOfFemaleEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.NoOfFemaleEmployee.Style.Font.Bold = true;
            this.NoOfFemaleEmployee.Style.Font.Name = "Nyala";
            this.NoOfFemaleEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.NoOfFemaleEmployee, "NoOfFemaleEmployee");
            // 
            // TotalEmployee
            // 
            this.TotalEmployee.Culture = new System.Globalization.CultureInfo("");
            this.TotalEmployee.Name = "TotalEmployee";
            this.TotalEmployee.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3625479936599731D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.TotalEmployee.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.TotalEmployee.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.TotalEmployee.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.TotalEmployee.Style.Font.Bold = true;
            this.TotalEmployee.Style.Font.Name = "Nyala";
            this.TotalEmployee.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.TotalEmployee, "TotalEmployee");
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "Civic.TableHeader";
            resources.ApplyResources(this.textBox1, "textBox1");
            // 
            // WorkUnitObjectDataSource
            // 
            this.WorkUnitObjectDataSource.DataMember = "GetDepartment";
            this.WorkUnitObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.WorkUnitObjectDataSource.Name = "WorkUnitObjectDataSource";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
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
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7455344200134277D), Telerik.Reporting.Drawing.Unit.Inch(1.300039529800415D));
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
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7287869453430176D), Telerik.Reporting.Drawing.Unit.Inch(1.0998821258544922D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4286689758300781D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.txtOrgName, "txtOrgName");
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(1.0998427867889404D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbOrgLogo.Style.Font.Name = "Nyala";
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000787496566773D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4286689758300781D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox25, "textBox25");
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.3958091735839844D), Telerik.Reporting.Drawing.Unit.Inch(1.0999608039855957D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.37073072791099548D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            resources.ApplyResources(this.Date, "Date");
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.795809268951416D), Telerik.Reporting.Drawing.Unit.Inch(1.0999608039855957D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.949725329875946D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Style.Font.Name = "Nyala";
            resources.ApplyResources(this.textBox17, "textBox17");
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.7000001072883606D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tbltraining});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tbltraining
            // 
            this.tbltraining.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.36283841729164124D)));
            this.tbltraining.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.5175637006759644D)));
            this.tbltraining.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.6733745336532593D)));
            this.tbltraining.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3625481128692627D)));
            this.tbltraining.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1D)));
            this.tbltraining.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D)));
            this.tbltraining.Body.SetCellContent(0, 0, this.textBox2);
            this.tbltraining.Body.SetCellContent(0, 2, this.textBox4);
            this.tbltraining.Body.SetCellContent(0, 3, this.textBox6);
            this.tbltraining.Body.SetCellContent(0, 1, this.textBox8);
            this.tbltraining.Body.SetCellContent(0, 4, this.textBox3);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox45;
            tableGroup2.Name = "group";
            tableGroup2.ReportItem = this.NoOfMaleEmployee;
            tableGroup3.Name = "tableGroup1";
            tableGroup3.ReportItem = this.NoOfFemaleEmployee;
            tableGroup4.Name = "tableGroup2";
            tableGroup4.ReportItem = this.TotalEmployee;
            tableGroup5.Name = "group1";
            tableGroup5.ReportItem = this.textBox1;
            this.tbltraining.ColumnGroups.Add(tableGroup1);
            this.tbltraining.ColumnGroups.Add(tableGroup2);
            this.tbltraining.ColumnGroups.Add(tableGroup3);
            this.tbltraining.ColumnGroups.Add(tableGroup4);
            this.tbltraining.ColumnGroups.Add(tableGroup5);
            this.tbltraining.DataSource = this.TrainingRequestODS;
            this.tbltraining.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox8,
            this.textBox4,
            this.textBox6,
            this.textBox3,
            this.textBox45,
            this.NoOfMaleEmployee,
            this.NoOfFemaleEmployee,
            this.TotalEmployee,
            this.textBox1});
            this.tbltraining.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.tbltraining.Name = "tbltraining";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup6.Name = "detailTableGroup";
            this.tbltraining.RowGroups.Add(tableGroup6);
            this.tbltraining.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.9163250923156738D), Telerik.Reporting.Drawing.Unit.Inch(0.47916677594184875D));
            this.tbltraining.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tbltraining.Style.Font.Name = "Nyala";
            this.tbltraining.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.36283835768699646D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            resources.ApplyResources(this.textBox2, "textBox2");
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.673377275466919D), Telerik.Reporting.Drawing.Unit.Inch(0.23958340287208557D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox4, "textBox4");
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3625479936599731D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox6.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox6, "textBox6");
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5175625085830689D), Telerik.Reporting.Drawing.Unit.Inch(0.23958340287208557D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.StyleName = "";
            resources.ApplyResources(this.textBox8, "textBox8");
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958341777324677D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.StyleName = "";
            resources.ApplyResources(this.textBox3, "textBox3");
            // 
            // TrainingRequestODS
            // 
            this.TrainingRequestODS.DataMember = "GetTrainingRequest";
            this.TrainingRequestODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.TrainingRequestODS);
            this.TrainingRequestODS.Name = "TrainingRequestODS";
            this.TrainingRequestODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
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
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1749224662780762D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            resources.ApplyResources(this.textBox16, "textBox16");
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.6748838424682617D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.49995961785316467D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            resources.ApplyResources(this.Page, "Page");
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.55204373598098755D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.EmployeeSummarybyAge,
            this.textBox7,
            this.textBox5});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.49996042251586914D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.733057975769043D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // EmployeeSummarybyAge
            // 
            this.EmployeeSummarybyAge.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.EmployeeSummarybyAge.Name = "EmployeeSummarybyAge";
            this.EmployeeSummarybyAge.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7454547882080078D), Telerik.Reporting.Drawing.Unit.Inch(0.19992078840732575D));
            this.EmployeeSummarybyAge.Style.Font.Bold = true;
            this.EmployeeSummarybyAge.Style.Font.Name = "Nyala";
            this.EmployeeSummarybyAge.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.EmployeeSummarybyAge.Style.Font.Strikeout = false;
            this.EmployeeSummarybyAge.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.EmployeeSummarybyAge, "EmployeeSummarybyAge");
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
            // textBox5
            // 
            this.textBox5.Format = "{0:d}";
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(0.199960395693779D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox5.Style.Font.Name = "Nyala";
            this.textBox5.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox5, "textBox5");
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.199960395693779D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.80000007152557373D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox7.Style.Font.Bold = true;
            resources.ApplyResources(this.textBox7, "textBox7");
            // 
            // TrainingRequest
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
            reportParameter1.AvailableValues.DataSource = this.WorkUnitObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter1.Name = "orgStructureId";
            resources.ApplyResources(reportParameter1, "reportParameter1");
            reportParameter1.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ObjectDataSource TrainingRequestODS;
        private Telerik.Reporting.Table tbltraining;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox45;
        private Telerik.Reporting.TextBox NoOfFemaleEmployee;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox TotalEmployee;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.TextBox EmployeeSummarybyAge;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.ObjectDataSource Localization;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox NoOfMaleEmployee;
        private Telerik.Reporting.ObjectDataSource WorkUnitObjectDataSource;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox5;
    }
}