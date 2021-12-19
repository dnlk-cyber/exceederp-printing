using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System.Drawing;
using System.IO;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class ExperienceLetter
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.EmployeeCodeODS = new Telerik.Reporting.ObjectDataSource();
            this.LanguageODS = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox46 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox43 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.ExpirianceLetterObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // EmployeeCodeODS
            // 
            this.EmployeeCodeODS.DataMember = "GetEmployeeCode";
            this.EmployeeCodeODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.AllEmployeeCodeObjectDataSource);
            this.EmployeeCodeODS.Name = "EmployeeCodeODS";
            // 
            // LanguageODS
            // 
            this.LanguageODS.DataMember = "GetLetterLanguages";
            this.LanguageODS.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.LetterLanguageODS);
            this.LanguageODS.Name = "LanguageODS";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.1000001430511475D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox46,
            this.textBox21,
            this.textBox14,
            this.textBox40,
            this.textBox43,
            this.textBox1,
            this.textBox27,
            this.textBox29,
            this.textBox30,
            this.textBox31});
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
            this.textBox46.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox46.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox46.Value = "= Fields.AmharicName";
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.100001335144043D), Telerik.Reporting.Drawing.Unit.Inch(0.59996062517166138D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox21.Style.Font.Bold = true;
            this.textBox21.Value = "Date:-";
            // 
            // textBox14
            // 
            this.textBox14.Format = "{0:d}";
            this.textBox14.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.59996062517166138D));
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox14.Value = "_____________";
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.100001335144043D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69992130994796753D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox40.Style.Font.Bold = true;
            this.textBox40.Value = "Ref.no :-";
            // 
            // textBox43
            // 
            this.textBox43.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox43.Name = "textBox43";
            this.textBox43.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox43.Style.Font.Bold = true;
            this.textBox43.Value = "_____________";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000007629394531D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox1.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox1.Style.Font.Underline = false;
            this.textBox1.Value = "= Fields.Tel";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.1997634619474411D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox27.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox27.Style.Font.Underline = false;
            this.textBox27.Value = "= Fields.Fax";
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000007629394531D), Telerik.Reporting.Drawing.Unit.Inch(0.39984226226806641D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox29.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox29.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox29.Style.Font.Underline = false;
            this.textBox29.Value = "= Fields.Pobox";
            // 
            // textBox30
            // 
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.59992104768753052D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox30.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox30.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox30.Style.Font.Underline = false;
            this.textBox30.Value = "= Fields.Email";
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.800000011920929D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox31.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox31.Style.Font.Underline = false;
            this.textBox31.Value = "= Fields.Address";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(1.5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox4,
            this.textBox8});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.03125D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.2687506675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox4.Style.Font.Underline = true;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "= Fields.Title";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.712519645690918D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.textBox8.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(14D);
            this.textBox8.Value = "= Fields.Experience";
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.3999999463558197D);
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.Style.Font.Name = "Nyala";
            // 
            // ExpirianceLetterObjectDataSource
            // 
            this.ExpirianceLetterObjectDataSource.DataMember = "GetEmployeeExperience";
            this.ExpirianceLetterObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.ExperienceObjectDataSource);
            this.ExpirianceLetterObjectDataSource.Name = "ExpirianceLetterObjectDataSource";
            this.ExpirianceLetterObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("includeExt", typeof(bool), "= Parameters.includeExt.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("language", typeof(string), "= Parameters.language.Value")});
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // ExperienceLetter
            // 
            this.DataSource = this.ExpirianceLetterObjectDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.reportFooterSection1});
            this.Name = "TerminationLetter";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.EmployeeCodeODS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.EmployeeCode";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmployeeId";
            reportParameter1.Name = "id";
            reportParameter1.Text = "Employee ID";
            reportParameter1.Value = "";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.LanguageODS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Item";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Item";
            reportParameter2.Name = "language";
            reportParameter2.Text = "Language";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.Name = "header";
            reportParameter3.Text = "With Header?";
            reportParameter3.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter3.Value = "=False";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.Name = "includeExt";
            reportParameter4.Text = "Include External";
            reportParameter4.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter4.Value = "= False";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.7125587463378906D);
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
        private Telerik.Reporting.ObjectDataSource LanguageODS;
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
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox31;
    }
}