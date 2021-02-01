namespace ExceedERP.Reporting.Budget.Print.View
{
    partial class BudgetRequestWithOutHeaderTransfer
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group3 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group4 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            this.monthGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.monthGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.yearGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.yearGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.totalAmountGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.totalAmountGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.labelsGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.requestNoCaptionTextBox = new Telerik.Reporting.TextBox();
            this.BudgetRequestTransferDS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.pageInfoTextBox = new Telerik.Reporting.TextBox();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.requestNoDataTextBox = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // monthGroupFooterSection
            // 
            this.monthGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.monthGroupFooterSection.Name = "monthGroupFooterSection";
            this.monthGroupFooterSection.Style.Visible = false;
            // 
            // monthGroupHeaderSection
            // 
            this.monthGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.078740514814853668D);
            this.monthGroupHeaderSection.Name = "monthGroupHeaderSection";
            // 
            // yearGroupFooterSection
            // 
            this.yearGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.yearGroupFooterSection.Name = "yearGroupFooterSection";
            this.yearGroupFooterSection.Style.Visible = false;
            // 
            // yearGroupHeaderSection
            // 
            this.yearGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.052083354443311691D);
            this.yearGroupHeaderSection.Name = "yearGroupHeaderSection";
            this.yearGroupHeaderSection.Style.Visible = false;
            // 
            // totalAmountGroupFooterSection
            // 
            this.totalAmountGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.totalAmountGroupFooterSection.Name = "totalAmountGroupFooterSection";
            this.totalAmountGroupFooterSection.Style.Visible = false;
            // 
            // totalAmountGroupHeaderSection
            // 
            this.totalAmountGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.totalAmountGroupHeaderSection.Name = "totalAmountGroupHeaderSection";
            this.totalAmountGroupHeaderSection.Style.Visible = false;
            // 
            // labelsGroupFooterSection
            // 
            this.labelsGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.labelsGroupFooterSection.Name = "labelsGroupFooterSection";
            this.labelsGroupFooterSection.Style.Visible = false;
            // 
            // labelsGroupHeaderSection
            // 
            this.labelsGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.labelsGroupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.requestNoCaptionTextBox});
            this.labelsGroupHeaderSection.Name = "labelsGroupHeaderSection";
            this.labelsGroupHeaderSection.PrintOnEveryPage = true;
            // 
            // requestNoCaptionTextBox
            // 
            this.requestNoCaptionTextBox.CanGrow = true;
            this.requestNoCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.02083333395421505D), Telerik.Reporting.Drawing.Unit.Inch(0.02083333395421505D));
            this.requestNoCaptionTextBox.Name = "requestNoCaptionTextBox";
            this.requestNoCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.3969817161560059D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.requestNoCaptionTextBox.Style.Visible = false;
            this.requestNoCaptionTextBox.StyleName = "Caption";
            this.requestNoCaptionTextBox.Value = "Request No";
            // 
            // BudgetRequestTransferDS
            // 
            this.BudgetRequestTransferDS.DataMember = "GetBudgetRequests";
            this.BudgetRequestTransferDS.DataSource = typeof(ExceedERP.Reporting.Budget.Print.DataSource.BudgetRequest);
            this.BudgetRequestTransferDS.Name = "BudgetRequestTransferDS";
            this.BudgetRequestTransferDS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("Id", typeof(string), "= Parameters.Id.Value")});
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.pageFooter.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageInfoTextBox});
            this.pageFooter.Name = "pageFooter";
            // 
            // pageInfoTextBox
            // 
            this.pageInfoTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.7297408580780029D), Telerik.Reporting.Drawing.Unit.Inch(0.02083333395421505D));
            this.pageInfoTextBox.Name = "pageInfoTextBox";
            this.pageInfoTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.6880741119384766D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.pageInfoTextBox.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.pageInfoTextBox.StyleName = "PageInfo";
            this.pageInfoTextBox.Value = "=PageNumber";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Inch(4.0944881439208984D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox15,
            this.textBox1,
            this.textBox2,
            this.textBox3});
            this.reportHeader.Name = "reportHeader";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.19685038924217224D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.4177756309509277D), Telerik.Reporting.Drawing.Unit.Inch(3.8976376056671143D));
            this.textBox15.Style.Font.Bold = false;
            this.textBox15.Style.Font.Name = "Arial Black";
            this.textBox15.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.textBox15.Style.Font.Strikeout = true;
            this.textBox15.Style.Font.Underline = true;
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox15.TextWrap = false;
            this.textBox15.Value = "_________________________________________________________________________________" +
    "________________________________________________________________________________" +
    "___________________________________";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(0.62598437070846558D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.1000785827636719D), Telerik.Reporting.Drawing.Unit.Inch(0.86851340532302856D));
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.Value = "ለ ：- ግንባታ ፋይናንስ ድጋፍ የስራ ሂደት\r\n       አዲስ አበባ \r\nከ：- {Fields.Branch}\r\n\r\n            " +
    "                                      ጉዳዪ：- በጀትን ስለመጠየቅ ይመለከታል\r\n\r\n             ";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(1.4945765733718872D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.1000785827636719D), Telerik.Reporting.Drawing.Unit.Inch(0.63132888078689575D));
            this.textBox2.Style.Font.Bold = false;
            this.textBox2.Value = "                     ፕሮጀክቱ ከ {Fields.Year} {Fields.Month} ወር ለተከናወኑ ስራዎች  ክዚህ ደብዳ" +
    "ቤ ጋር አባሪ በተደረገው ዝርዝር መሰረት {Fields.TotalAmount}  በጀት አንዲላክልን አንጠይቃለን᎓᎓";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(2.1259844303131104D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.1000785827636719D), Telerik.Reporting.Drawing.Unit.Inch(1.3000001907348633D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Value = "አንዲያውቁት               \r\n * ግንባታ ፋይናንስ ድጋፍ የስራ ሂደት ስራ አስኪያጅ\r\n       አዲስ አበባ \r\nግልባጭ" +
    "\r\n* ለፐሮ/ግ/አ/አስ/ኬዝ ቲም\r\n* ለፐሮ/ቢሮ ምህንድስና ቲም\r\n* ለፐሮ/ፉይናንስ ኣፊሰር  \r\n \r\n       \r\n\r\n    " +
    "         ";
            // 
            // reportFooter
            // 
            this.reportFooter.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.reportFooter.Name = "reportFooter";
            this.reportFooter.Style.Visible = false;
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.28125D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.requestNoDataTextBox});
            this.detail.Name = "detail";
            // 
            // requestNoDataTextBox
            // 
            this.requestNoDataTextBox.CanGrow = true;
            this.requestNoDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.02083333395421505D), Telerik.Reporting.Drawing.Unit.Inch(0.02083333395421505D));
            this.requestNoDataTextBox.Name = "requestNoDataTextBox";
            this.requestNoDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.3969817161560059D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.requestNoDataTextBox.Style.Visible = false;
            this.requestNoDataTextBox.StyleName = "Data";
            this.requestNoDataTextBox.Value = "= Fields.RequestNo";
            // 
            // BudgetRequestWithOutHeaderTransfer
            // 
            this.DataSource = this.BudgetRequestTransferDS;
            group1.GroupFooter = this.monthGroupFooterSection;
            group1.GroupHeader = this.monthGroupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.Month"));
            group1.Name = "monthGroup";
            group2.GroupFooter = this.yearGroupFooterSection;
            group2.GroupHeader = this.yearGroupHeaderSection;
            group2.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.Year"));
            group2.Name = "yearGroup";
            group3.GroupFooter = this.totalAmountGroupFooterSection;
            group3.GroupHeader = this.totalAmountGroupHeaderSection;
            group3.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.TotalAmount"));
            group3.Name = "totalAmountGroup";
            group4.GroupFooter = this.labelsGroupFooterSection;
            group4.GroupHeader = this.labelsGroupHeaderSection;
            group4.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2,
            group3,
            group4});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.monthGroupHeaderSection,
            this.monthGroupFooterSection,
            this.yearGroupHeaderSection,
            this.yearGroupFooterSection,
            this.totalAmountGroupHeaderSection,
            this.totalAmountGroupFooterSection,
            this.labelsGroupHeaderSection,
            this.labelsGroupFooterSection,
            this.pageFooter,
            this.reportHeader,
            this.reportFooter,
            this.detail});
            this.Name = "BudgetRequestTransfer";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.Name = "Id";
            reportParameter1.Text = "Id";
            this.ReportParameters.Add(reportParameter1);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Title")});
            styleRule2.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(97)))), ((int)(((byte)(74)))));
            styleRule2.Style.Font.Name = "Georgia";
            styleRule2.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(18D);
            styleRule3.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Caption")});
            styleRule3.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            styleRule3.Style.BorderColor.Default = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(168)))), ((int)(((byte)(212)))));
            styleRule3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Dotted;
            styleRule3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            styleRule3.Style.Color = System.Drawing.Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(238)))), ((int)(((byte)(243)))));
            styleRule3.Style.Font.Name = "Georgia";
            styleRule3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            styleRule3.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule4.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("Data")});
            styleRule4.Style.Font.Name = "Georgia";
            styleRule4.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            styleRule4.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            styleRule5.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.StyleSelector("PageInfo")});
            styleRule5.Style.Font.Name = "Georgia";
            styleRule5.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            styleRule5.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1,
            styleRule2,
            styleRule3,
            styleRule4,
            styleRule5});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.4178147315979D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ObjectDataSource BudgetRequestTransferDS;
        private Telerik.Reporting.GroupHeaderSection monthGroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection monthGroupFooterSection;
        private Telerik.Reporting.GroupHeaderSection yearGroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection yearGroupFooterSection;
        private Telerik.Reporting.GroupHeaderSection totalAmountGroupHeaderSection;
        private Telerik.Reporting.GroupFooterSection totalAmountGroupFooterSection;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeaderSection;
        private Telerik.Reporting.TextBox requestNoCaptionTextBox;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooterSection;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.TextBox pageInfoTextBox;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox requestNoDataTextBox;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;

    }
}