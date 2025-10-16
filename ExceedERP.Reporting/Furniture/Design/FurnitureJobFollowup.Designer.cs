namespace ExceedERP.Reporting.Furniture.Design
{
    partial class FurnitureJobFollowup
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
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.table3 = new Telerik.Reporting.Table();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.sharedods = new Telerik.Reporting.ObjectDataSource();
            this.jobFollowUpHeaderOds = new Telerik.Reporting.ObjectDataSource();
            this.metalOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.8000001907348633D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.textBox24,
            this.textBox23,
            this.pbOrgLogo});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1,
            this.table2,
            this.table3});
            this.detail.Name = "detail";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox27});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.75502842664718628D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.1449713706970215D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.22998708486557007D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2399861216545105D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox2);
            this.table1.Body.SetCellContent(0, 1, this.textBox4);
            this.table1.Body.SetCellContent(1, 0, this.textBox5);
            this.table1.Body.SetCellContent(1, 1, this.textBox6);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "tableGroup1";
            tableGroup2.ReportItem = this.textBox3;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.DataSource = this.jobFollowUpHeaderOds;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox4,
            this.textBox1,
            this.textBox3,
            this.textBox5,
            this.textBox6});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9164224290288985E-05D));
            this.table1.Name = "table1";
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup";
            tableGroup4.Name = "group";
            this.table1.RowGroups.Add(tableGroup3);
            this.table1.RowGroups.Add(tableGroup4);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.8999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0.69996029138565063D));
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.755028247833252D), Telerik.Reporting.Drawing.Unit.Inch(0.22998708486557007D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox1.Style.Font.Underline = true;
            this.textBox1.Value = "Job No:";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.755028247833252D), Telerik.Reporting.Drawing.Unit.Inch(0.22998708486557007D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox2.Style.Font.Underline = true;
            this.textBox2.Value = "Job Type:";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1449713706970215D), Telerik.Reporting.Drawing.Unit.Inch(0.22998708486557007D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox3.Style.Font.Underline = true;
            this.textBox3.Value = "= Fields.JobNo";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1449713706970215D), Telerik.Reporting.Drawing.Unit.Inch(0.22998708486557007D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox4.Style.Font.Underline = true;
            this.textBox4.Value = "= Fields.JobType";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.755028247833252D), Telerik.Reporting.Drawing.Unit.Inch(0.2399861216545105D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox5.Style.Font.Underline = true;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "Customer:";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1449713706970215D), Telerik.Reporting.Drawing.Unit.Inch(0.2399861216545105D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox6.Style.Font.Underline = true;
            this.textBox6.StyleName = "";
            this.textBox6.Value = "= Fields.CustomerName";
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.93679004907608032D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.9632103443145752D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.22998689115047455D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23998643457889557D)));
            this.table2.Body.SetCellContent(0, 0, this.textBox7);
            this.table2.Body.SetCellContent(0, 1, this.textBox8);
            this.table2.Body.SetCellContent(1, 0, this.textBox9);
            this.table2.Body.SetCellContent(1, 1, this.textBox10);
            tableGroup5.Name = "tableGroup";
            tableGroup5.ReportItem = this.textBox11;
            tableGroup6.Name = "tableGroup1";
            tableGroup6.ReportItem = this.textBox12;
            this.table2.ColumnGroups.Add(tableGroup5);
            this.table2.ColumnGroups.Add(tableGroup6);
            this.table2.DataSource = this.jobFollowUpHeaderOds;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox8,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.textBox12});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.5999999046325684D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.table2.Name = "table2";
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup7.Name = "detailTableGroup";
            tableGroup8.Name = "group";
            this.table2.RowGroups.Add(tableGroup7);
            this.table2.RowGroups.Add(tableGroup8);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.69996023178100586D));
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93679004907608032D), Telerik.Reporting.Drawing.Unit.Inch(0.22998690605163574D));
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox7.Style.Font.Underline = true;
            this.textBox7.Value = "Unit:";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9632103443145752D), Telerik.Reporting.Drawing.Unit.Inch(0.22998690605163574D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox8.Style.Font.Underline = true;
            this.textBox8.Value = "= Fields.Unit";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93679004907608032D), Telerik.Reporting.Drawing.Unit.Inch(0.23998647928237915D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox9.Style.Font.Underline = true;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "Delivery Date:";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9632103443145752D), Telerik.Reporting.Drawing.Unit.Inch(0.23998647928237915D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox10.Style.Font.Underline = true;
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= Fields.DeliveryDate";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.93679004907608032D), Telerik.Reporting.Drawing.Unit.Inch(0.22998690605163574D));
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox11.Style.Font.Underline = true;
            this.textBox11.Value = "Quantity:";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9632103443145752D), Telerik.Reporting.Drawing.Unit.Inch(0.22998690605163574D));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox12.Style.Font.Underline = true;
            this.textBox12.Value = "= Fields.Quantity";
            // 
            // table3
            // 
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2685585021972656D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2685585021972656D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.2685585021972656D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3471419811248779D)));
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.3471424579620361D)));
            this.table3.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D)));
            this.table3.Body.SetCellContent(0, 0, this.textBox14);
            this.table3.Body.SetCellContent(0, 1, this.textBox16);
            this.table3.Body.SetCellContent(0, 2, this.textBox18);
            this.table3.Body.SetCellContent(0, 3, this.textBox20);
            this.table3.Body.SetCellContent(0, 4, this.textBox22);
            tableGroup9.Name = "tableGroup2";
            tableGroup9.ReportItem = this.textBox13;
            tableGroup10.Name = "tableGroup3";
            tableGroup10.ReportItem = this.textBox15;
            tableGroup11.Name = "tableGroup4";
            tableGroup11.ReportItem = this.textBox17;
            tableGroup12.Name = "group1";
            tableGroup12.ReportItem = this.textBox19;
            tableGroup13.Name = "group2";
            tableGroup13.ReportItem = this.textBox21;
            this.table3.ColumnGroups.Add(tableGroup9);
            this.table3.ColumnGroups.Add(tableGroup10);
            this.table3.ColumnGroups.Add(tableGroup11);
            this.table3.ColumnGroups.Add(tableGroup12);
            this.table3.ColumnGroups.Add(tableGroup13);
            this.table3.DataSource = this.metalOds;
            this.table3.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14,
            this.textBox16,
            this.textBox18,
            this.textBox13,
            this.textBox15,
            this.textBox17,
            this.textBox19,
            this.textBox20,
            this.textBox21,
            this.textBox22});
            this.table3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.0000001192092896D));
            this.table3.Name = "table3";
            tableGroup14.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup14.Name = "detailTableGroup1";
            this.table3.RowGroups.Add(tableGroup14);
            this.table3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999604225158691D), Telerik.Reporting.Drawing.Unit.Inch(0.5000002384185791D));
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Value = "Employee ";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Value = "= Fields.EmployeeName";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox15.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Value = "Task ";
            // 
            // textBox16
            // 
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox16.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox16.Value = "= Fields.TaskType";
            // 
            // textBox17
            // 
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox17.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.textBox17.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.Value = "Prod. Plan";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2685586214065552D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Value = "= Fields.DailyProdPlan";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3471419811248779D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.StyleName = "";
            this.textBox19.Value = "Prod. Actual";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3471419811248779D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.StyleName = "";
            this.textBox20.Value = "= Fields.ActualProduction";
            // 
            // textBox21
            // 
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3471424579620361D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox21.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.textBox21.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.StyleName = "";
            this.textBox21.Value = "Date";
            // 
            // textBox22
            // 
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3471424579620361D), Telerik.Reporting.Drawing.Unit.Inch(0.25000011920928955D));
            this.textBox22.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox22.StyleName = "";
            this.textBox22.Value = "= Fields.DateExcuted";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.0862627176684327E-07D), Telerik.Reporting.Drawing.Unit.Inch(1.5894570992713852E-08D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2244089841842651D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            this.pbOrgLogo.Style.Font.Name = "Georgia";
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox23
            // 
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.042783984914422E-05D), Telerik.Reporting.Drawing.Unit.Inch(1.4999997615814209D));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4999208450317383D), Telerik.Reporting.Drawing.Unit.Inch(0.19999976456165314D));
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Style.Font.Name = "Georgia";
            this.textBox23.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox23.Style.Font.Underline = false;
            this.textBox23.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox23.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox23.TextWrap = false;
            this.textBox23.Value = "Job Order Production";
            // 
            // textBox24
            // 
            this.textBox24.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.700000524520874D), Telerik.Reporting.Drawing.Unit.Inch(0.7000390887260437D));
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.299999475479126D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox24.Style.Font.Bold = true;
            this.textBox24.Style.Font.Name = "Georgia";
            this.textBox24.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox24.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox24.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.textBox24.TextWrap = false;
            this.textBox24.Value = "= Fields.Name";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.700000524520874D), Telerik.Reporting.Drawing.Unit.Inch(0.39999991655349731D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.3104157447814941D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Georgia";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Bottom;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.5625984668731689D), Telerik.Reporting.Drawing.Unit.Inch(2.5431313588342164E-07D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.374802827835083D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox27.Style.Font.Name = "Segoe UI";
            this.textBox27.Value = "Page: {PageNumber}";
            // 
            // sharedods
            // 
            this.sharedods.DataMember = "GetOrganization";
            this.sharedods.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.sharedods.Name = "sharedods";
            // 
            // jobFollowUpHeaderOds
            // 
            this.jobFollowUpHeaderOds.DataMember = "GetFollowup";
            this.jobFollowUpHeaderOds.DataSource = typeof(ExceedERP.Reporting.Furniture.DataSource.FurnitureProductionFollowupOds);
            this.jobFollowUpHeaderOds.Name = "jobFollowUpHeaderOds";
            this.jobFollowUpHeaderOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(int), "= Parameters.id.Value")});
            // 
            // metalOds
            // 
            this.metalOds.DataMember = "GetMetal";
            this.metalOds.DataSource = typeof(ExceedERP.Reporting.Furniture.DataSource.FurnitureProductionFollowupOds);
            this.metalOds.Name = "metalOds";
            this.metalOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(int), "= Parameters.id.Value")});
            // 
            // FurnitureJobFollowup
            // 
            this.DataSource = this.sharedods;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "FurnitureJobFollowup";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.Name = "id";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.Integer;
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource jobFollowUpHeaderOds;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.Table table3;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.ObjectDataSource metalOds;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.ObjectDataSource sharedods;
    }
}