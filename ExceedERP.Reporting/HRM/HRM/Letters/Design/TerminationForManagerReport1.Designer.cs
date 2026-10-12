namespace ExceedERP.Reporting.HRM.HRM.Letters.Design
{
    partial class TerminationForManagerReport1
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminationForManagerReport1));
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.EmployeesobjectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            this.CCobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.TranscriptODS = new Telerik.Reporting.ObjectDataSource();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.TerminationForManagersobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.pictureBox2 = new Telerik.Reporting.PictureBox();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.pictureBox3 = new Telerik.Reporting.PictureBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // EmployeesobjectDataSource1
            // 
            this.EmployeesobjectDataSource1.DataMember = "GetEmployees";
            this.EmployeesobjectDataSource1.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.EmployeeDataSource);
            this.EmployeesobjectDataSource1.Name = "EmployeesobjectDataSource1";
            // 
            // CCobjectDataSource
            // 
            this.CCobjectDataSource.DataMember = "GetLetThemKnow";
            this.CCobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.CCobjectDataSource.Name = "CCobjectDataSource";
            // 
            // TranscriptODS
            // 
            this.TranscriptODS.DataMember = "GetTranscript";
            this.TranscriptODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.TranscriptObjectDataSource);
            this.TranscriptODS.Name = "TranscriptODS";
            this.TranscriptODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("transcript", typeof(object[]), "= Parameters.CCWorkUnit.Value")});
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(7.5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox13,
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox4,
            this.table2,
            this.textBox8,
            this.pictureBox3,
            this.textBox18,
            this.textBox11,
            this.textBox16,
            this.textBox17});
            this.detail.Name = "detail";
            // 
            // textBox13
            // 
            this.textBox13.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.3999996185302734D), Telerik.Reporting.Drawing.Unit.Inch(2.5D));
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox13.Value = "ከሰላምታ ጋር";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.098211079835891724D), Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.0588264465332031D), Telerik.Reporting.Drawing.Unit.Inch(1.2527562379837036D));
            this.textBox1.Style.Font.Bold = false;
            this.textBox1.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox1.Value = resources.GetString("textBox1.Value");
            // 
            // textBox2
            // 
            this.textBox2.Angle = 0D;
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.098172254860401154D), Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0018277168273926D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox2.Style.Font.Bold = true;
            this.textBox2.Value = "ለአቶ ወ/ሮ/ሪት :";
            // 
            // textBox3
            // 
            this.textBox3.Angle = 0D;
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.100078821182251D), Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9453949928283691D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Value = "= Fields.FullName";
            // 
            // textBox4
            // 
            this.textBox4.Angle = 0D;
            this.textBox4.Format = "{0}";
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.098172254860401154D), Telerik.Reporting.Drawing.Unit.Inch(0.60007911920547485D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.85635364055633545D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox4.Value = "ኢ.ኮ.ስ.ኮ ";
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.16666637361049652D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.3541674613952637D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D)));
            this.table2.Body.SetCellContent(0, 1, this.textBox14);
            this.table2.Body.SetCellContent(0, 0, this.textBox15);
            tableGroup1.Name = "group5";
            tableGroup2.Name = "tableGroup1";
            this.table2.ColumnGroups.Add(tableGroup1);
            this.table2.ColumnGroups.Add(tableGroup2);
            this.table2.ColumnHeadersPrintOnEveryPage = true;
            this.table2.DataSource = this.TranscriptODS;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox15,
            this.textBox14});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11999999731779099D), Telerik.Reporting.Drawing.Unit.Inch(3.5D));
            this.table2.Name = "table2";
            tableGroup4.Name = "group2";
            tableGroup3.ChildGroups.Add(tableGroup4);
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup1";
            this.table2.RowGroups.Add(tableGroup3);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.5208334922790527D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.3541669845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.23958329856395721D));
            this.textBox14.Value = "= Fields.Name";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox15.StyleName = "";
            this.textBox15.Value = "+";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11999998986721039D), Telerik.Reporting.Drawing.Unit.Inch(3.2999207973480225D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox8.Style.Font.Bold = true;
            this.textBox8.Style.Font.Underline = true;
            this.textBox8.Value = "ግልባጭ፡-";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.40000024437904358D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox42,
            this.Page,
            this.textBox10,
            this.textBox7});
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // textBox42
            // 
            this.textBox42.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.1571168899536133D), Telerik.Reporting.Drawing.Unit.Inch(0.10866207629442215D));
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0331822633743286D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox42.Style.Font.Bold = true;
            this.textBox42.Style.Font.Name = "Nyala";
            this.textBox42.Value = "{PageNumber}";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.7072691917419434D), Telerik.Reporting.Drawing.Unit.Inch(0.10866207629442215D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.44976893067359924D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "page:";
            // 
            // TerminationForManagersobjectDataSource
            // 
            this.TerminationForManagersobjectDataSource.DataMember = "GetTeminationManager";
            this.TerminationForManagersobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.Letters.DataSource.TeminationManagerObjectDatasource);
            this.TerminationForManagersobjectDataSource.Name = "TerminationForManagersobjectDataSource";
            this.TerminationForManagersobjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("empId", typeof(string), "= Parameters.EmpId.Value")});
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pictureBox1,
            this.pictureBox2});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.1999998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.7000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(1.2999604940414429D));
            this.pictureBox2.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Style.BackgroundImage.ImageData")));
            this.pictureBox2.Style.BackgroundImage.MimeType = "image/jpeg";
            this.pictureBox2.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.6999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(1.2999604940414429D));
            this.pictureBox1.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Style.BackgroundImage.ImageData")));
            this.pictureBox1.Style.BackgroundImage.MimeType = "image/jpeg";
            this.pictureBox1.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            // 
            // textBox18
            // 
            this.textBox18.Angle = 195D;
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.1571164131164551D), Telerik.Reporting.Drawing.Unit.Inch(0.69996005296707153D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.034722644835710526D), Telerik.Reporting.Drawing.Unit.Inch(6.7999997138977051D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.Green;
            this.textBox18.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox18.Style.LineWidth = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.textBox18.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox18.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(0D);
            this.textBox18.Value = "";
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.2999997138977051D), Telerik.Reporting.Drawing.Unit.Inch(0.69999992847442627D));
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5999619960784912D), Telerik.Reporting.Drawing.Unit.Inch(6.7999200820922852D));
            this.pictureBox3.Style.BackgroundImage.ImageData = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Style.BackgroundImage.ImageData")));
            this.pictureBox3.Style.BackgroundImage.MimeType = "image/png";
            this.pictureBox3.Style.BackgroundImage.Repeat = Telerik.Reporting.Drawing.BackgroundRepeat.NoRepeat;
            this.pictureBox3.Style.BorderWidth.Default = Telerik.Reporting.Drawing.Unit.Pixel(1D);
            this.pictureBox3.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Pixel(8D);
            // 
            // textBox11
            // 
            this.textBox11.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.0571565628051758D), Telerik.Reporting.Drawing.Unit.Inch(0.19992171227931976D));
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5428445339202881D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox11.Value = "ቁጥር:___________";
            // 
            // textBox16
            // 
            this.textBox16.Format = "{0:dd.MM.yyyy}";
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox16.Value = "= Today()";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.0571556091308594D), Telerik.Reporting.Drawing.Unit.Inch(0.40000003576278687D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.34276625514030457D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox17.Value = "ቀን፣";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.28666645288467407D), Telerik.Reporting.Drawing.Unit.Inch(0.10866207629442215D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.0133333206176758D), Telerik.Reporting.Drawing.Unit.Inch(0.19996006786823273D));
            this.textBox7.Style.Color = System.Drawing.Color.Green;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.Value = "የተሻለ ነገን እንገነባለን";
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.899468356394209E-05D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.1570377349853516D), Telerik.Reporting.Drawing.Unit.Inch(0.0347222238779068D));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.Green;
            this.textBox10.Value = "";
            // 
            // TerminationForManagerReport1
            // 
            this.DataSource = this.TerminationForManagersobjectDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.detail,
            this.pageFooterSection1,
            this.pageHeaderSection1});
            this.Name = "ContractHiringReport";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.EmployeesobjectDataSource1;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.FullName";
            reportParameter1.AvailableValues.ValueMember = "= Fields.EmpId";
            reportParameter1.Name = "EmpId";
            reportParameter1.Text = "Full Name";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "FormNumber";
            reportParameter3.Name = "FormTitle";
            reportParameter4.AllowNull = true;
            reportParameter4.AvailableValues.DataSource = this.CCobjectDataSource;
            reportParameter4.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter4.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter4.MultiValue = true;
            reportParameter4.Name = "CCWorkUnit";
            reportParameter4.Text = "CC";
            reportParameter4.Visible = true;
            reportParameter5.AllowNull = true;
            reportParameter5.Name = "ApplicationDate";
            reportParameter5.Text = "ያመለከቱት ቀን";
            reportParameter5.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter5.Value = "= Today()";
            reportParameter5.Visible = true;
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.90000057220459D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.ObjectDataSource TranscriptODS;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.ObjectDataSource EmployeesobjectDataSource1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.ObjectDataSource CCobjectDataSource;
        private Telerik.Reporting.ObjectDataSource TerminationForManagersobjectDataSource;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox8;
        public Telerik.Reporting.Table table2;
        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.PictureBox pictureBox2;
        private Telerik.Reporting.PictureBox pictureBox3;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox7;
    }
}