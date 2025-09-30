using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System.Drawing;
using System.IO;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class TemplateLetters
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TemplateLetters));
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
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.ExpirianceLetterObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.LetThemobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
            this.LanguageobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TemplateTypeobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.CCobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2999606132507324D), Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D));
            resources.ApplyResources(this.textBox2, "textBox2");
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000001430511475D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox46});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.pageHeaderSection1.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.9835166931152344D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Name = "Microsoft Sans Serif";
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.txtOrgName, "txtOrgName");
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.89992129802703857D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // textBox46
            // 
            this.textBox46.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox46.Name = "textBox46";
            this.textBox46.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.9835166931152344D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox46.Style.Font.Bold = true;
            this.textBox46.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox46, "textBox46");
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.50000005960464478D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox21, "textBox21");
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:D}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4000792503356934D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2999210357666016D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox14, "textBox14");
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.7000001072883606D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox40.Style.Font.Bold = true;
            this.textBox40.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox40, "textBox40");
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6576719284057617D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.0423285961151123D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Style.Font.Name = "Microsoft Sans Serif";
            resources.ApplyResources(this.textBox43, "textBox43");
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(1.8000003099441528D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.textBox8,
            this.textBox43,
            this.textBox14,
            this.textBox40,
            this.textBox21,
            this.table1,
            this.textBox7});
            this.detail.Name = "detail";
            this.detail.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.detail.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9164224290288985E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.1743922233581543D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.299921989440918D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox4.Style.Font.Underline = true;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            resources.ApplyResources(this.textBox4, "textBox4");
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2687892913818359D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.textBox8.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(14D);
            resources.ApplyResources(this.textBox8, "textBox8");
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.18800985813140869D)));
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
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.15601475536823273D), Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D));
            this.table1.Name = "table1";
            tableGroup5.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup5.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup5);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2999610900878906D), Telerik.Reporting.Drawing.Unit.Inch(0.40625005960464478D));
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
            this.textBox5.StyleName = "";
            resources.ApplyResources(this.textBox5, "textBox5");
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5999997854232788D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            resources.ApplyResources(this.textBox7, "textBox7");
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.60000008344650269D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox27,
            this.textBox29,
            this.textBox31,
            this.textBox30,
            this.textBox6});
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox1.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox1.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox1, "textBox1");
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox27.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox27.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox27.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox27, "textBox27");
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(1.5894572413799324E-07D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox29.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox29.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox29.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox29, "textBox29");
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox31.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox31.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox31.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox31, "textBox31");
            // 
            // textBox30
            // 
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4000003337860107D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox30.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox30.Style.Font.Name = "Microsoft Sans Serif";
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox30.Style.Font.Underline = false;
            resources.ApplyResources(this.textBox30, "textBox30");
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.6576714515686035D), Telerik.Reporting.Drawing.Unit.Inch(0.40007877349853516D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4423288106918335D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Name = "Nyala";
            resources.ApplyResources(this.textBox6, "textBox6");
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
            new Telerik.Reporting.ObjectDataSourceParameter("to", typeof(System.Nullable<System.DateTime>), "= Parameters.ToDate.Value")});
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
            // CCobjectDataSource
            // 
            this.CCobjectDataSource.DataMember = "GetLetThemKnow";
            this.CCobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.CCobjectDataSource.Name = "CCobjectDataSource";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // TemplateLetters
            // 
            this.DataSource = this.ExpirianceLetterObjectDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.reportFooterSection1});
            this.Name = "TerminationLetter";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
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
            resources.ApplyResources(reportParameter5, "reportParameter5");
            reportParameter5.Visible = true;
            reportParameter6.AllowNull = true;
            reportParameter6.AvailableValues.DataSource = this.CCobjectDataSource;
            reportParameter6.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter6.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter6.MultiValue = true;
            reportParameter6.Name = "CC";
            resources.ApplyResources(reportParameter6, "reportParameter6");
            reportParameter6.Visible = true;
            reportParameter7.AllowNull = true;
            reportParameter7.Name = "from";
            resources.ApplyResources(reportParameter7, "reportParameter7");
            reportParameter7.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter7.Visible = true;
            reportParameter8.AllowNull = true;
            reportParameter8.Name = "ToDate";
            resources.ApplyResources(reportParameter8, "reportParameter8");
            reportParameter8.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter8.Visible = true;
            reportParameter9.AllowNull = true;
            reportParameter9.Name = "Date";
            resources.ApplyResources(reportParameter9, "reportParameter9");
            reportParameter9.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter9.Visible = true;
            reportParameter10.AllowNull = true;
            reportParameter10.Name = "Refnum";
            resources.ApplyResources(reportParameter10, "reportParameter10");
            reportParameter10.Visible = true;
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
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.ObjectDataSource ExpirianceLetterObjectDataSource;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.ObjectDataSource EmployeeCodeODS;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox46;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox43;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox31;
        private Telerik.Reporting.TextBox textBox30;
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
    }
}