namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class RecruitmentRequestLetter
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
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.EmployeeobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.LetThemKnowODS = new Telerik.Reporting.ObjectDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox29 = new Telerik.Reporting.TextBox();
            this.textBox30 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox31 = new Telerik.Reporting.TextBox();
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();
            this.RecruitmentRequestODS = new Telerik.Reporting.ObjectDataSource();
            this.WorkUnitsODS = new Telerik.Reporting.ObjectDataSource();
            this.TranscriptODS = new Telerik.Reporting.ObjectDataSource();
            this.RecruitmentObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.htmlTextBox1 = new Telerik.Reporting.HtmlTextBox();
            this.htmlTextBox2 = new Telerik.Reporting.HtmlTextBox();
            this.htmlTextBox3 = new Telerik.Reporting.HtmlTextBox();
            this.htmlTextBox4 = new Telerik.Reporting.HtmlTextBox();
            this.htmlTextBox5 = new Telerik.Reporting.HtmlTextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // EmployeeobjectDataSource
            // 
            this.EmployeeobjectDataSource.DataMember = "GetEmployees";
            this.EmployeeobjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.RecruitmentEmployeeDataSource);
            this.EmployeeobjectDataSource.Name = "EmployeeobjectDataSource";
            // 
            // LetThemKnowODS
            // 
            this.LetThemKnowODS.DataMember = "GetLetThemKnow";
            this.LetThemKnowODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.LetThemKnowODS.Name = "LetThemKnowODS";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(2.5000786781311035D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.htmlTextBox1,
            this.htmlTextBox2,
            this.htmlTextBox3,
            this.htmlTextBox4,
            this.htmlTextBox5});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.39999985694885254D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel2});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox6,
            this.textBox29,
            this.textBox30,
            this.textBox11,
            this.textBox24,
            this.textBox17,
            this.textBox27,
            this.textBox16,
            this.textBox4,
            this.textBox31});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.7125592231750488D), Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D));
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4835164546966553D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1000001430511475D), Telerik.Reporting.Drawing.Unit.Inch(0.89992111921310425D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.1000000238418579D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4835164546966553D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.Font.Name = "Nyala";
            this.textBox6.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.Value = "= Fields.AmharicName";
            // 
            // textBox29
            // 
            this.textBox29.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.8042712211608887D), Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D));
            this.textBox29.Name = "textBox29";
            this.textBox29.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59565037488937378D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox29.Style.Font.Bold = true;
            this.textBox29.Style.Font.Italic = false;
            this.textBox29.Value = "ቀን፡";
            // 
            // textBox30
            // 
            this.textBox30.Format = "{0:d}";
            this.textBox30.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D));
            this.textBox30.Name = "textBox30";
            this.textBox30.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000000476837158D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox30.Style.Font.Bold = true;
            this.textBox30.Style.Font.Italic = false;
            this.textBox30.Style.Font.Underline = true;
            this.textBox30.Value = "=Fields.Date";
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.8042712211608887D), Telerik.Reporting.Drawing.Unit.Inch(0.99992132186889648D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59565037488937378D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox11.Style.Font.Bold = true;
            this.textBox11.Style.Font.Italic = false;
            this.textBox11.Value = "ቁጥር：";
            // 
            // textBox24
            // 
            this.textBox24.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.99992132186889648D));
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox24.Style.Font.Bold = true;
            this.textBox24.Style.Font.Italic = false;
            this.textBox24.Style.Font.Underline = true;
            this.textBox24.Value = "____________________";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.900001049041748D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox17.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox17.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox17.Style.Font.Underline = false;
            this.textBox17.Value = "= Fields.Tel";
            // 
            // textBox27
            // 
            this.textBox27.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.900001049041748D), Telerik.Reporting.Drawing.Unit.Inch(0.19976337254047394D));
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox27.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox27.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox27.Style.Font.Underline = false;
            this.textBox27.Value = "= Fields.Fax";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.900001049041748D), Telerik.Reporting.Drawing.Unit.Inch(0.39984217286109924D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox16.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox16.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox16.Style.Font.Underline = false;
            this.textBox16.Value = "= Fields.Pobox";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.900001049041748D), Telerik.Reporting.Drawing.Unit.Inch(0.59992098808288574D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox4.Style.Font.Underline = false;
            this.textBox4.Value = "= Fields.Email";
            // 
            // textBox31
            // 
            this.textBox31.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.79999995231628418D));
            this.textBox31.Name = "textBox31";
            this.textBox31.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40000057220459D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox31.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(121)))), ((int)(((byte)(185)))));
            this.textBox31.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox31.Style.Font.Underline = false;
            this.textBox31.Value = "= Fields.Address";
            // 
            // reportFooterSection1
            // 
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.60000044107437134D);
            this.reportFooterSection1.Name = "reportFooterSection1";
            this.reportFooterSection1.Style.Font.Name = "Nyala";
            // 
            // RecruitmentRequestODS
            // 
            this.RecruitmentRequestODS.DataMember = "GetRecruitmentRequestLetter";
            this.RecruitmentRequestODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.RecruitmentRequestLetterODS);
            this.RecruitmentRequestODS.Name = "RecruitmentRequestODS";
            this.RecruitmentRequestODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value")});
            // 
            // WorkUnitsODS
            // 
            this.WorkUnitsODS.DataMember = "GetLetThemKnows";
            this.WorkUnitsODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKowWorkUnitObjectDataSource);
            this.WorkUnitsODS.Name = "WorkUnitsODS";
            this.WorkUnitsODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.letthem.Value")});
            // 
            // TranscriptODS
            // 
            this.TranscriptODS.DataMember = "GetTranscript";
            this.TranscriptODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.TranscriptObjectDataSource);
            this.TranscriptODS.Name = "TranscriptODS";
            this.TranscriptODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("transcript", typeof(object[]), "= Parameters.transcript.Value")});
            // 
            // RecruitmentObjectDataSource
            // 
            this.RecruitmentObjectDataSource.DataMember = "GetEmployeeJob";
            this.RecruitmentObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.RecruitmentObjectDataSource);
            this.RecruitmentObjectDataSource.Name = "RecruitmentObjectDataSource";
            this.RecruitmentObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("Id", typeof(string), "= Parameters.empCode.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.letthem.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("transcript", typeof(object[]), "= Parameters.transcript.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("header", typeof(bool), "= Parameters.header.Value")});
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.90000009536743164D), Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.09999942779541D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Style.Font.Name = "Nyala";
            this.textBox2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox2.Style.Font.Underline = true;
            this.textBox2.Value = "Recruitment Request Letter";
            // 
            // htmlTextBox1
            // 
            this.htmlTextBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.htmlTextBox1.Name = "htmlTextBox1";
            this.htmlTextBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.htmlTextBox1.Value = "<strong>Branch</strong> &nbsp;<span style=\"text-decoration: underline\">{=Fields.B" +
    "ranch}</span>";
            // 
            // htmlTextBox2
            // 
            this.htmlTextBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.10000000149011612D), Telerik.Reporting.Drawing.Unit.Inch(0.80000013113021851D));
            this.htmlTextBox2.Name = "htmlTextBox2";
            this.htmlTextBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.htmlTextBox2.Value = "<strong>Department</strong> &nbsp;<span style=\"text-decoration: underline\">{=Fiel" +
    "ds.Department}</span>";
            // 
            // htmlTextBox3
            // 
            this.htmlTextBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999941885471344D), Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D));
            this.htmlTextBox3.Name = "htmlTextBox3";
            this.htmlTextBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.htmlTextBox3.Value = "<strong>Jobtitle</strong> &nbsp;<span style=\"text-decoration: underline\">{=Fields" +
    ".JobTitle}</span>";
            // 
            // htmlTextBox4
            // 
            this.htmlTextBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999964237213135D), Telerik.Reporting.Drawing.Unit.Inch(1.399999737739563D));
            this.htmlTextBox4.Name = "htmlTextBox4";
            this.htmlTextBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.htmlTextBox4.Value = "<strong>Department Approver</strong> &nbsp;<span style=\"text-decoration: underlin" +
    "e\">{=Fields.DepApprover}</span>";
            // 
            // htmlTextBox5
            // 
            this.htmlTextBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.099999986588954926D), Telerik.Reporting.Drawing.Unit.Inch(1.6999998092651367D));
            this.htmlTextBox5.Name = "htmlTextBox5";
            this.htmlTextBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.htmlTextBox5.Value = "<strong>Approver</strong> &nbsp;<span style=\"text-decoration: underline\">{=Fields" +
    ".Approver}</span>";
            // 
            // RecruitmentRequestLetter
            // 
            this.DataSource = this.RecruitmentRequestODS;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportFooterSection1});
            this.Name = "TerminationLetter";
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.EmployeeobjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.FullNameAmharic";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmployeeCode";
            reportParameter1.Name = "empCode";
            reportParameter1.Text = "ሰራተኛ";
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.LetThemKnowODS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.MultiValue = true;
            reportParameter2.Name = "letthem";
            reportParameter2.Text = "እንዲያውቁት";
            reportParameter2.Value = "=First(Fields.Name)";
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.LetThemKnowODS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter3.MultiValue = true;
            reportParameter3.Name = "transcript";
            reportParameter3.Text = "ግልባጭ";
            reportParameter3.Value = "=First(Fields.Name)";
            reportParameter4.AllowNull = true;
            reportParameter4.Name = "header";
            reportParameter4.Text = "With Header?";
            reportParameter4.Type = Telerik.Reporting.ReportParameterType.Boolean;
            reportParameter4.Value = "=True";
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.AutoRefresh = true;
            reportParameter5.Name = "id";
            reportParameter5.Text = "Id";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.7125983238220215D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource RecruitmentObjectDataSource;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.ObjectDataSource EmployeeobjectDataSource;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.ObjectDataSource WorkUnitsODS;
        private Telerik.Reporting.ObjectDataSource TranscriptODS;
        private Telerik.Reporting.ObjectDataSource LetThemKnowODS;
        private Telerik.Reporting.TextBox textBox29;
        private Telerik.Reporting.TextBox textBox30;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox31;
        private System.ComponentModel.IContainer components;
        private Telerik.Reporting.ObjectDataSource RecruitmentRequestODS;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.HtmlTextBox htmlTextBox1;
        private Telerik.Reporting.HtmlTextBox htmlTextBox2;
        private Telerik.Reporting.HtmlTextBox htmlTextBox3;
        private Telerik.Reporting.HtmlTextBox htmlTextBox4;
        private Telerik.Reporting.HtmlTextBox htmlTextBox5;
    }
}