using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System.Drawing;
using System.IO;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class TemplatePortraitLetters
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplatePortraitLetters));
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter6 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter7 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter8 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter9 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter10 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter11 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter12 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter13 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter14 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter15 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.htmlTextBox1 = new Telerik.Reporting.HtmlTextBox();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.ExpirianceLetterObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.LetThemobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
            this.LanguageobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TemplateTypeobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TemplateTypeSeetingobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.CCobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.SignerEmployeeNBameobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2999606132507324D), Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D));
            resources.ApplyResources(this.textBox2, "textBox2");
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.1277499198913574D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.972171425819397D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Style.Font.Name = "Nyala";
            this.textBox21.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            resources.ApplyResources(this.textBox21, "textBox21");
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:dd.MM.yyyy}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.09999942779541D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Style.Font.Name = "Nyala";
            this.textBox14.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            resources.ApplyResources(this.textBox14, "textBox14");
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.1277499198913574D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.972170889377594D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox40.Style.Font.Bold = false;
            this.textBox40.Style.Font.Name = "Nyala";
            this.textBox40.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            resources.ApplyResources(this.textBox40, "textBox40");
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.09999942779541D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox43.Style.Font.Bold = false;
            this.textBox43.Style.Font.Name = "Nyala";
            this.textBox43.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            resources.ApplyResources(this.textBox43, "textBox43");
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2.9000000953674316D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.textBox43,
            this.textBox14,
            this.textBox40,
            this.textBox21,
            this.table1,
            this.textBox7,
            this.textBox9,
            this.textBox10,
            this.pictureBox1,
            this.htmlTextBox1});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.detail.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.18804900348186493D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.6999607086181641D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Name = "Nyala";
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox4.Style.Font.Underline = true;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox4, "textBox4");
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.18800981342792511D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.0559756755828857D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.0559756755828857D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958337306976318D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox3);
            this.table1.Body.SetCellContent(0, 1, this.textBox5, 1, 2);
            tableGroup2.Name = "group";
            tableGroup3.Name = "tableGroup1";
            tableGroup4.Name = "tableGroup2";
            tableGroup1.ChildGroups.Add(tableGroup2);
            tableGroup1.ChildGroups.Add(tableGroup3);
            tableGroup1.ChildGroups.Add(tableGroup4);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox2;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.DataSource = this.LetThemobjectDataSource;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3,
            this.textBox5,
            this.textBox2});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D), Telerik.Reporting.Drawing.Unit.Inch(1.6600790023803711D));
            this.table1.Name = "table1";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup5);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2999610900878906D), Telerik.Reporting.Drawing.Unit.Inch(0.40625005960464478D));
            this.table1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.18800985813140869D), Telerik.Reporting.Drawing.Unit.Inch(0.23958337306976318D));
            resources.ApplyResources(this.textBox3, "textBox3");
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.1119513511657715D), Telerik.Reporting.Drawing.Unit.Inch(0.23958337306976318D));
            this.textBox5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.textBox5.StyleName = "";
            resources.ApplyResources(this.textBox5, "textBox5");
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(1.2999998331069946D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0998821258544922D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox7.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            resources.ApplyResources(this.textBox7, "textBox7");
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(1.9600788354873657D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0999214649200439D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox9.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            resources.ApplyResources(this.textBox9, "textBox9");
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(2.1601579189300537D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0998821258544922D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox10.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            resources.ApplyResources(this.textBox10, "textBox10");
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.7999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(1.5000003576278687D));
            this.pictureBox1.MimeType = "";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4700000286102295D), Telerik.Reporting.Drawing.Unit.Inch(0.46000000834465027D));
            this.pictureBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(10D);
            this.pictureBox1.Value = "= Fields.Signature";
            // 
            // htmlTextBox1
            // 
            this.htmlTextBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D));
            this.htmlTextBox1.Name = "htmlTextBox1";
            this.htmlTextBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7D), Telerik.Reporting.Drawing.Unit.Inch(0.40000009536743164D));
            this.htmlTextBox1.Style.BackgroundColor = System.Drawing.Color.White;
            this.htmlTextBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.htmlTextBox1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.htmlTextBox1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            resources.ApplyResources(this.htmlTextBox1, "htmlTextBox1");
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.69999980926513672D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox27,
            this.textBox29,
            this.textBox31,
            this.textBox30,
            this.textBox6,
            this.textBox12,
            this.textBox11,
            this.textBox13,
            this.textBox15,
            this.textBox16});
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox1.Style.Font.Name = "Nyala";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox1.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox1, "textBox1");
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.1999998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox27.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox27.Style.Font.Name = "Nyala";
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox27.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox27, "textBox27");
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.600000262260437D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox29.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox29.Style.Font.Name = "Nyala";
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox29.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox29, "textBox29");
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox31.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox31.Style.Font.Name = "Nyala";
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox31.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox31, "textBox31");
            // 
            // textBox30
            // 
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.3999998569488525D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox30.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox30.Style.Font.Name = "Nyala";
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox30.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox30, "textBox30");
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4101181030273438D), Telerik.Reporting.Drawing.Unit.Inch(4.0372211515204981E-05D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0798022747039795D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Name = "Nyala";
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            resources.ApplyResources(this.textBox6, "textBox6");
            // 
            // textBox12
            // 
            this.textBox12.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.40007877349853516D));
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox12.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox12.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            resources.ApplyResources(this.textBox12, "textBox12");
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2000007629394531D), Telerik.Reporting.Drawing.Unit.Inch(0.40007910132408142D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox11.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox11.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox11, "textBox11");
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.40007910132408142D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox13.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox13.Style.Font.Bold = true;
            this.textBox13.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox13.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox13, "textBox13");
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.40007910132408142D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox15.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox15.Style.Font.Underline = true;
            resources.ApplyResources(this.textBox15, "textBox15");
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.81011962890625D), Telerik.Reporting.Drawing.Unit.Inch(0.20003938674926758D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2898800373077393D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox16.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox16.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox16, "textBox16");
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox46});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2000395059585571D), Telerik.Reporting.Drawing.Unit.Inch(0.69996070861816406D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.9835166931152344D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Nyala";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.txtOrgName, "txtOrgName");
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.20003955066204071D), Telerik.Reporting.Drawing.Unit.Inch(0.20003946125507355D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.89992129802703857D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2001184225082398D), Telerik.Reporting.Drawing.Unit.Inch(0.40000009536743164D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.9835166931152344D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox46.Style.Font.Bold = true;
            this.textBox46.Style.Font.Name = "Nyala";
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox46, "textBox46");
            // 
            // ExpirianceLetterObjectDataSource
            // 
            this.ExpirianceLetterObjectDataSource.DataMember = "GetEmployeeTemplateLetters";
            this.ExpirianceLetterObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.TemplateLettersObjectDataSource);
            this.ExpirianceLetterObjectDataSource.Name = "ExpirianceLetterObjectDataSource";
            this.ExpirianceLetterObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("includeExt", typeof(bool), "= Parameters.includeExt.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("language", typeof(string), "= Parameters.language.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("type", typeof(string), "= Parameters.TemplateType.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("from", typeof(System.Nullable<System.DateTime>), "= Parameters.from.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("to", typeof(System.Nullable<System.DateTime>), "= Parameters.ToDate.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("templatetype", typeof(string), "= Parameters.TemplateTypeSetting.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("headId", typeof(string), "= Parameters.SignerName.Value")});
            // 
            // LetThemobjectDataSource
            // 
            this.LetThemobjectDataSource.DataMember = "GetLetThemKnows";
            this.LetThemobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKowWorkUnitObjectDataSource);
            this.LetThemobjectDataSource.Name = "LetThemobjectDataSource";
            this.LetThemobjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.CC.Value")});
            // 
            // EmployeeCodeODS
            // 
            this.EmployeeCodeODS.DataMember = "GetEmployeeCode";
            this.EmployeeCodeODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AllEmployeeCodeObjectDataSource);
            this.EmployeeCodeODS.Name = "EmployeeCodeODS";
            // 
            // LanguageobjectDataSource
            // 
            this.LanguageobjectDataSource.DataMember = "GetLanguages";
            this.LanguageobjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.LanguageODS);
            this.LanguageobjectDataSource.Name = "LanguageobjectDataSource";
            // 
            // TemplateTypeobjectDataSource
            // 
            this.TemplateTypeobjectDataSource.DataMember = "GetTemplateType";
            this.TemplateTypeobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.TemplateTypeODS);
            this.TemplateTypeobjectDataSource.Name = "TemplateTypeobjectDataSource";
            // 
            // TemplateTypeSeetingobjectDataSource
            // 
            this.TemplateTypeSeetingobjectDataSource.DataMember = "GetTemplateTypeSetting";
            this.TemplateTypeSeetingobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.TemplateTypeODS);
            this.TemplateTypeSeetingobjectDataSource.Name = "TemplateTypeSeetingobjectDataSource";
            // 
            // CCobjectDataSource
            // 
            this.CCobjectDataSource.DataMember = "GetLetThemKnow";
            this.CCobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.CCobjectDataSource.Name = "CCobjectDataSource";
            // 
            // SignerEmployeeNBameobjectDataSource
            // 
            this.SignerEmployeeNBameobjectDataSource.DataMember = "GetActiveandInActiveEmployeeCode";
            this.SignerEmployeeNBameobjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AllEmployeeObjectDataSource);
            this.SignerEmployeeNBameobjectDataSource.Name = "SignerEmployeeNBameobjectDataSource";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // TemplatePortraitLetters
            // 
            this.DataSource = this.ExpirianceLetterObjectDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail,
            this.reportFooterSection1,
            this.reportHeaderSection1});
            this.Name = "TemplatePortraitLetters";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(10D), Telerik.Reporting.Drawing.Unit.Mm(10D), Telerik.Reporting.Drawing.Unit.Mm(0D), Telerik.Reporting.Drawing.Unit.Mm(0D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.EmployeeCodeODS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.EmployeeCode";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmployeeId";
            reportParameter1.Name = "id";
            resources.ApplyResources(reportParameter1, "reportParameter1");
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.LanguageobjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Item ";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter2.Name = "language";
            resources.ApplyResources(reportParameter2, "reportParameter2");
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.Name = "header";
            resources.ApplyResources(reportParameter3, "reportParameter3");
            reportParameter3.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter3.Value = "=False";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.Name = "includeExt";
            resources.ApplyResources(reportParameter4, "reportParameter4");
            reportParameter4.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter4.Value = "= False";
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.AvailableValues.DataSource = this.TemplateTypeobjectDataSource;
            reportParameter5.AvailableValues.DisplayMember = "= Fields.DisplayName";
            reportParameter5.AvailableValues.ValueMember = "= Fields.Value";
            reportParameter5.Name = "TemplateType";
            reportParameter5.Visible = true;
            reportParameter6.AllowNull = true;
            reportParameter6.AvailableValues.DataSource = this.TemplateTypeSeetingobjectDataSource;
            reportParameter6.AvailableValues.DisplayMember = "= Fields.DisplayName";
            reportParameter6.AvailableValues.ValueMember = "= Fields.Value";
            reportParameter6.Name = "TemplateTypeSetting";
            resources.ApplyResources(reportParameter6, "reportParameter6");
            reportParameter6.Visible = true;
            reportParameter7.AllowNull = true;
            reportParameter7.AvailableValues.DataSource = this.CCobjectDataSource;
            reportParameter7.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter7.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter7.MultiValue = true;
            reportParameter7.Name = "CC";
            reportParameter7.Visible = true;
            reportParameter8.AllowNull = true;
            reportParameter8.Name = "from";
            resources.ApplyResources(reportParameter8, "reportParameter8");
            reportParameter8.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter8.Visible = true;
            reportParameter9.AllowNull = true;
            reportParameter9.Name = "ToDate";
            resources.ApplyResources(reportParameter9, "reportParameter9");
            reportParameter9.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter9.Visible = true;
            reportParameter10.AllowNull = true;
            reportParameter10.Name = "Date";
            resources.ApplyResources(reportParameter10, "reportParameter10");
            reportParameter10.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter10.Visible = true;
            reportParameter11.AllowNull = true;
            reportParameter11.Name = "Refnum";
            resources.ApplyResources(reportParameter11, "reportParameter11");
            reportParameter11.Visible = true;
            reportParameter12.AllowNull = true;
            reportParameter12.AvailableValues.DataSource = this.SignerEmployeeNBameobjectDataSource;
            reportParameter12.AvailableValues.DisplayMember = "= Fields.EmployeeName";
            reportParameter12.AvailableValues.ValueMember = "= Fields.EmployeeId";
            reportParameter12.Name = "SignerName";
            resources.ApplyResources(reportParameter12, "reportParameter12");
            reportParameter12.Visible = true;
            reportParameter13.AllowNull = true;
            reportParameter13.Name = "Moto";
            resources.ApplyResources(reportParameter13, "reportParameter13");
            reportParameter13.Visible = true;
            reportParameter14.AllowNull = true;
            reportParameter14.Name = "FormNumber";
            reportParameter15.AllowNull = true;
            reportParameter15.Name = "FormTitle";
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.ReportParameters.Add(reportParameter4);
            this.ReportParameters.Add(reportParameter5);
            this.ReportParameters.Add(reportParameter6);
            this.ReportParameters.Add(reportParameter7);
            this.ReportParameters.Add(reportParameter8);
            this.ReportParameters.Add(reportParameter9);
            this.ReportParameters.Add(reportParameter10);
            this.ReportParameters.Add(reportParameter11);
            this.ReportParameters.Add(reportParameter12);
            this.ReportParameters.Add(reportParameter13);
            this.ReportParameters.Add(reportParameter14);
            this.ReportParameters.Add(reportParameter15);
            this.Style.Font.Name = "Nyala";
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.489959716796875D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.ObjectDataSource ExpirianceLetterObjectDataSource;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.ObjectDataSource EmployeeCodeODS;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.ObjectDataSource LanguageobjectDataSource;
        private Telerik.Reporting.ObjectDataSource TemplateTypeobjectDataSource;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.ObjectDataSource LetThemobjectDataSource;
        private Telerik.Reporting.ObjectDataSource CCobjectDataSource;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.ObjectDataSource TemplateTypeSeetingobjectDataSource;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.ObjectDataSource SignerEmployeeNBameobjectDataSource;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.HtmlTextBox htmlTextBox1;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox46;
    }
}