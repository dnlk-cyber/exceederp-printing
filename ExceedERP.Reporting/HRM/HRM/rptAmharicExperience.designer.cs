namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptAmharicExperience
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rptAmharicExperience));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.shape3 = new Telerik.Reporting.Shape();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.tblexpe = new Telerik.Reporting.Table();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.ExpirianceLetterObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // EmployeeCodeODS
            // 
            this.EmployeeCodeODS.DataMember = "GetEmployeeCode";
            this.EmployeeCodeODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AllEmployeeCodeObjectDataSource);
            this.EmployeeCodeODS.Name = "EmployeeCodeODS";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.8000000715255737D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel2,
            this.shape3});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox46,
            this.textBox2,
            this.textBox27,
            this.textBox29,
            this.textBox30,
            this.textBox31,
            this.textBox21,
            this.textBox14,
            this.textBox40,
            this.textBox43});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.9999995231628418D), Telerik.Reporting.Drawing.Unit.Inch(1.7000000476837158D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.683516263961792D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
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
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3999212980270386D), Telerik.Reporting.Drawing.Unit.Inch(1.0227521657943726D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6835157871246338D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox46.Style.Font.Bold = true;
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox46.Value = "= Fields.AmharicName";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox2.Style.Font.Underline = false;
            this.textBox2.Value = "= Fields.Tel";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.22251541912555695D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox27.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox27.Style.Font.Underline = false;
            this.textBox27.Value = "= Fields.Fax";
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.42259421944618225D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox29.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox29.Style.Font.Underline = false;
            this.textBox29.Value = "= Fields.Pobox";
            // 
            // textBox30
            // 
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.62267303466796875D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox30.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox30.Style.Font.Underline = false;
            this.textBox30.Value = "= Fields.Email";
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.82275199890136719D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox31.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox31.Style.Font.Underline = false;
            this.textBox31.Value = "= Fields.Address";
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.5000004768371582D), Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Value = "ቀን:-";
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.1979174613952637D), Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Value = "_____________";
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.5000004768371582D), Telerik.Reporting.Drawing.Unit.Inch(1.0999212265014648D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox40.Style.Font.Bold = true;
            this.textBox40.Value = "ቁጥር :-";
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.0999212265014648D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Value = "_____________";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18731816112995148D), Telerik.Reporting.Drawing.Unit.Inch(1.6000000238418579D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2126822471618652D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(3.6000003814697266D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.textBox13,
            this.tblexpe,
            this.textBox15,
            this.textBox16,
            this.textBox17,
            this.textBox18,
            this.textBox1});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.8000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.40000009536743164D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.3999996185302734D), Telerik.Reporting.Drawing.Unit.Inch(0.30000016093254089D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(15D);
            this.textBox4.Style.Font.Underline = true;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "ለ ሚ መ ለ ከ ተ ው ሁ ሉ";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.520830512046814D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox13.Value = "ከሠላምታ ጋር ";
            // 
            // tblexpe
            // 
            this.tblexpe.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.Expriance"));
            this.tblexpe.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.16666597127914429D)));
            this.tblexpe.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(7.2216677665710449D)));
            this.tblexpe.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.34850183129310608D)));
            this.tblexpe.Body.SetCellContent(0, 1, this.textBox19);
            this.tblexpe.Body.SetCellContent(0, 0, this.textBox5);
            tableGroup1.Name = "group7";
            tableGroup2.Name = "group2";
            this.tblexpe.ColumnGroups.Add(tableGroup1);
            this.tblexpe.ColumnGroups.Add(tableGroup2);
            this.tblexpe.DataSource = this.ExpirianceLetterObjectDataSource;
            this.tblexpe.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox5,
            this.textBox19});
            this.tblexpe.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D), Telerik.Reporting.Drawing.Unit.Inch(1.2999998331069946D));
            this.tblexpe.Name = "tblexpe";
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup1";
            this.tblexpe.RowGroups.Add(tableGroup3);
            this.tblexpe.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.388333797454834D), Telerik.Reporting.Drawing.Unit.Inch(0.34850183129310608D));
            this.tblexpe.Style.BorderColor.Default = System.Drawing.Color.White;
            this.tblexpe.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.tblexpe.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(11D);
            this.tblexpe.Style.LineColor = System.Drawing.Color.White;
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2216687202453613D), Telerik.Reporting.Drawing.Unit.Inch(0.34850183129310608D));
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox19.Style.Font.Bold = false;
            this.textBox19.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox19.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox19.Value = "{=Fields.JobTitleName} ከ {=Fields.From} እስከ {=Fields.To}  {=Fields.Branch} {=Fiel" +
    "ds.RecruitmentType}";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.16666629910469055D), Telerik.Reporting.Drawing.Unit.Inch(0.34850183129310608D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.textBox5.Style.Font.Bold = false;
            this.textBox5.Style.Font.Name = "Nyala";
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox5.Style.Padding.Bottom = Telerik.Reporting.Drawing.Unit.Inch(0D);
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Justify;
            this.textBox5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= RowNumber(\"tblexpe\")";
            // 
            // ExpirianceLetterObjectDataSource
            // 
            this.ExpirianceLetterObjectDataSource.DataMember = "GetEmployeeAmharicExperience";
            this.ExpirianceLetterObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.AmharicExperienceLetterObjectDataSource);
            this.ExpirianceLetterObjectDataSource.Name = "ExpirianceLetterObjectDataSource";
            this.ExpirianceLetterObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value")});
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D), Telerik.Reporting.Drawing.Unit.Inch(1.7000004053115845D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.7999992370605469D), Telerik.Reporting.Drawing.Unit.Inch(0.6040881872177124D));
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox15.Value = resources.GetString("textBox15.Value");
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D), Telerik.Reporting.Drawing.Unit.Inch(2.90000057220459D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.84375011920928955D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Underline = true;
            this.textBox16.Value = "ግልባጭ፡-";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(3.3000009059906006D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7208337783813477D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox17.Style.Font.Bold = true;
            this.textBox17.Style.Font.Underline = true;
            this.textBox17.Value = "= Fields.Abbreviation";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(3.1000006198883057D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7208337783813477D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox18.Value = "  ለግል ማህደር";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D), Telerik.Reporting.Drawing.Unit.Inch(0.70007878541946411D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8208303451538086D), Telerik.Reporting.Drawing.Unit.Inch(0.599842369556427D));
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(13D);
            this.textBox1.Value = "የ{= Fields.TempOne} ባልደረባ የሆኑት  {= Fields.ApplicantName} በመ/ቤታችን ውስጥ ያገለገሉበትን ጊዜ " +
    "የሚገልፅ የስራ ልምድ \r\nማስረጃ እንድንፅፍላቸው በ {= Fields.Date} ጠይቀዋል\r\nበዚሁ መሠረት ከግል ማህደራቸው በተገኝ" +
    "ው መረጃ መሰረት፡-";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // rptAmharicExperience
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "rptAmharicExperience";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Pixel(1D), Telerik.Reporting.Drawing.Unit.Pixel(1D), Telerik.Reporting.Drawing.Unit.Pixel(1D), Telerik.Reporting.Drawing.Unit.Pixel(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.EmployeeCodeODS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.EmployeeCode";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmployeeId";
            reportParameter1.Name = "id";
            reportParameter1.Text = "Employee ID";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "header";
            reportParameter2.Text = "With Header";
            reportParameter2.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter2.Value = "=False ";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.1000003814697266D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.Table tblexpe;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.ObjectDataSource ExpirianceLetterObjectDataSource;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.ObjectDataSource EmployeeCodeODS;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox43;
    }
}