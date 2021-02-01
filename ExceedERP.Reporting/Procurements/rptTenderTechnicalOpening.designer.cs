namespace ExceedERP.Reporting.Procurements
{
    partial class rptTenderTechnicalOpening
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(rptTenderTechnicalOpening));
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.SupplierODS = new Telerik.Reporting.ObjectDataSource();
            this.LetThemKnowODS = new Telerik.Reporting.ObjectDataSource();
            this.TranscriptODS = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.txtOrgNameAmharic = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.LetThemKnowWorkunitODS = new Telerik.Reporting.ObjectDataSource();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.TranscriptWorkunitODS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.TechnicalTenderOpeningODS = new Telerik.Reporting.ObjectDataSource();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.table3 = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.ParticipatedSupplierODS = new Telerik.Reporting.ObjectDataSource();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // SupplierODS
            // 
            this.SupplierODS.DataMember = "GetTenderSupplier";
            this.SupplierODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.BidParticipantSupplierObjectDataSource);
            this.SupplierODS.Name = "SupplierODS";
            this.SupplierODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value")});
            // 
            // LetThemKnowODS
            // 
            this.LetThemKnowODS.DataMember = "GetLetThemKnow";
            this.LetThemKnowODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.LetThemKnowODS.Name = "LetThemKnowODS";
            // 
            // TranscriptODS
            // 
            this.TranscriptODS.DataMember = "GetLetThemKnow";
            this.TranscriptODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKnowObjectDataSource);
            this.TranscriptODS.Name = "TranscriptODS";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.5D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgNameAmharic,
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox18,
            this.textBox17,
            this.textBox16,
            this.textBox15});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // txtOrgNameAmharic
            // 
            this.txtOrgNameAmharic.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000791311264038D), Telerik.Reporting.Drawing.Unit.Inch(0.30003941059112549D));
            this.txtOrgNameAmharic.Name = "txtOrgNameAmharic";
            this.txtOrgNameAmharic.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.8531239032745361D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgNameAmharic.Style.Font.Bold = true;
            this.txtOrgNameAmharic.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgNameAmharic.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgNameAmharic.Value = "= Fields.AmharicName";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000791311264038D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.8531243801116943D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(1.0999606847763062D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.4041671752929688D), Telerik.Reporting.Drawing.Unit.Inch(0.89988201856613159D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.900000274181366D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox18.Value = "Ref.No/ቁጥር";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(0.89988201856613159D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox17.Style.Font.Underline = true;
            this.textBox17.Value = "___________";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.4041671752929688D), Telerik.Reporting.Drawing.Unit.Inch(1.0873819589614868D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.900000274181366D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox16.Value = "Date/ቀን";
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3000006675720215D), Telerik.Reporting.Drawing.Unit.Inch(1.0873819589614868D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox15.Style.Font.Underline = true;
            this.textBox15.Value = "___________";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(3.7999999523162842D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox7,
            this.textBox8,
            this.textBox9,
            this.textBox10,
            this.table1,
            this.table2,
            this.table3,
            this.textBox1,
            this.textBox3,
            this.textBox4});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox7
            // 
            this.textBox7.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.2916666269302368D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.4999997615814209D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.Font.Underline = true;
            this.textBox7.Value = "የቴክኒካል ጨረታ መክፈቻ ቀን ስለመግለጽ";
            // 
            // textBox9
            // 
            this.textBox9.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.53750008344650269D), Telerik.Reporting.Drawing.Unit.Inch(2.6977589130401611D));
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox9.Style.Font.Underline = true;
            this.textBox9.Value = "እንዲያዉቁት";
            // 
            // textBox10
            // 
            this.textBox10.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.53750008344650269D), Telerik.Reporting.Drawing.Unit.Inch(3.1999998092651367D));
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2000001668930054D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox10.Style.Font.Underline = true;
            this.textBox10.Value = "ግልባጭ";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.19791656732559204D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.0000004768371582D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table1.Body.SetCellContent(0, 1, this.textBox12);
            this.table1.Body.SetCellContent(0, 0, this.textBox13);
            tableGroup1.Name = "group";
            tableGroup2.Name = "tableGroup";
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.DataSource = this.LetThemKnowWorkunitODS;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox13,
            this.textBox12});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.63125008344650269D), Telerik.Reporting.Drawing.Unit.Inch(2.8979165554046631D));
            this.table1.Name = "table1";
            tableGroup3.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup3.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup3);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.1979169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0000004768371582D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox12.Value = "= Fields.Name";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.19791668653488159D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox13.StyleName = "";
            this.textBox13.Value = "+";
            // 
            // LetThemKnowWorkunitODS
            // 
            this.LetThemKnowWorkunitODS.DataMember = "GetLetThemKnows";
            this.LetThemKnowWorkunitODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKowWorkUnitObjectDataSource);
            this.LetThemKnowWorkunitODS.Name = "LetThemKnowWorkunitODS";
            this.LetThemKnowWorkunitODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.letthem.Value")});
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.14583344757556915D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.0729169845581055D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table2.Body.SetCellContent(0, 1, this.textBox11);
            this.table2.Body.SetCellContent(0, 0, this.textBox14);
            tableGroup4.Name = "group1";
            tableGroup5.Name = "tableGroup";
            this.table2.ColumnGroups.Add(tableGroup4);
            this.table2.ColumnGroups.Add(tableGroup5);
            this.table2.DataSource = this.TranscriptWorkunitODS;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox14,
            this.textBox11});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.63125008344650269D), Telerik.Reporting.Drawing.Unit.Inch(3.3999998569488525D));
            this.table2.Name = "table2";
            tableGroup6.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup6.Name = "detailTableGroup";
            this.table2.RowGroups.Add(tableGroup6);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2187504768371582D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0729169845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox11.Value = "= Fields.Name";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.1458333432674408D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox14.StyleName = "";
            this.textBox14.Value = "+";
            // 
            // TranscriptWorkunitODS
            // 
            this.TranscriptWorkunitODS.DataMember = "GetTranscript";
            this.TranscriptWorkunitODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.TranscriptObjectDataSource);
            this.TranscriptWorkunitODS.Name = "TranscriptWorkunitODS";
            this.TranscriptWorkunitODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("transcript", typeof(object[]), "= Parameters.transcript.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // TechnicalTenderOpeningODS
            // 
            this.TechnicalTenderOpeningODS.DataMember = "GetTechnicalOpenTender";
            this.TechnicalTenderOpeningODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.TechnicalTenderOpeningObjectDataSource);
            this.TechnicalTenderOpeningODS.Name = "TechnicalTenderOpeningODS";
            this.TechnicalTenderOpeningODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value")});
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.29992136359214783D), Telerik.Reporting.Drawing.Unit.Inch(0.20007865130901337D));
            this.textBox1.Value = "ለ ：-";
            // 
            // table3
            // 
            this.table3.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.4791669845581055D)));
            this.table3.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table3.Body.SetCellContent(0, 0, this.textBox2);
            tableGroup7.Name = "tableGroup1";
            this.table3.ColumnGroups.Add(tableGroup7);
            this.table3.DataSource = this.ParticipatedSupplierODS;
            this.table3.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2});
            this.table3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.table3.Name = "table3";
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup8.Name = "detailTableGroup1";
            this.table3.RowGroups.Add(tableGroup8);
            this.table3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4791669845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4791669845581055D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox2.Value = "= Fields.SupplierName";
            // 
            // ParticipatedSupplierODS
            // 
            this.ParticipatedSupplierODS.DataMember = "GetParticipants";
            this.ParticipatedSupplierODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.CancelledSupplierObjectDataSource);
            this.ParticipatedSupplierODS.Name = "ParticipatedSupplierODS";
            this.ParticipatedSupplierODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value")});
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.2000012397766113D), Telerik.Reporting.Drawing.Unit.Inch(2D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2999999523162842D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox3.Value = "ከሰላምታ ጋር";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D), Telerik.Reporting.Drawing.Unit.Inch(1.2999998331069946D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.7999606132507324D), Telerik.Reporting.Drawing.Unit.Inch(0.60000056028366089D));
            this.textBox8.Value = resources.GetString("textBox8.Value");
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.77708357572555542D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.50000005960464478D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox4.Style.Font.Bold = true;
            this.textBox4.Value = "ጉዳዩ ：-";
            // 
            // rptTenderTechnicalOpening
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "rptTenderTechnicalOpening";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.SupplierODS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.SupplierName";
            reportParameter1.AvailableValues.ValueMember = "= Fields.ID";
            reportParameter1.Name = "supplier";
            reportParameter1.Text = "Supplier";
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.LetThemKnowODS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.MultiValue = true;
            reportParameter2.Name = "letthem";
            reportParameter2.Text = "Let Them Know";
            reportParameter2.Value = "= First(Fields.OrgStructureId)";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.TranscriptODS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter3.MultiValue = true;
            reportParameter3.Name = "transcript";
            reportParameter3.Text = "CC";
            reportParameter3.Value = "= First(Fields.OrgStructureId)";
            reportParameter3.Visible = true;
            reportParameter4.AllowNull = true;
            reportParameter4.Name = "id";
            reportParameter4.Text = "ID";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.9000000953674316D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ObjectDataSource SupplierODS;
        private Telerik.Reporting.ObjectDataSource LetThemKnowODS;
        private Telerik.Reporting.ObjectDataSource TranscriptODS;
        private Telerik.Reporting.ObjectDataSource LetThemKnowWorkunitODS;
        private Telerik.Reporting.ObjectDataSource TranscriptWorkunitODS;
        private Telerik.Reporting.ObjectDataSource TechnicalTenderOpeningODS;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox txtOrgNameAmharic;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.Table table3;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.ObjectDataSource ParticipatedSupplierODS;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox4;
    }
}