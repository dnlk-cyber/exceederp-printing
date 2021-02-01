namespace ExceedERP.Reporting.Procurements
{
    partial class TechnicalPassFail
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup12 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup13 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup14 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup15 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup16 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup17 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup18 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter6 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter7 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter8 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter9 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter10 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            this.SupplierODS = new Telerik.Reporting.ObjectDataSource();
            this.ParticiapantSupplierODS = new Telerik.Reporting.ObjectDataSource();
            this.LetThemKnowODS = new Telerik.Reporting.ObjectDataSource();
            this.TranscriptODS = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.txtOrgNameAmharic = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.txtTo = new Telerik.Reporting.TextBox();
            this.txtsubjectmain = new Telerik.Reporting.TextBox();
            this.txtSubject = new Telerik.Reporting.TextBox();
            this.txtCity = new Telerik.Reporting.TextBox();
            this.table2 = new Telerik.Reporting.Table();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.textBox24 = new Telerik.Reporting.TextBox();
            this.LetThemKnowWorkunitODS = new Telerik.Reporting.ObjectDataSource();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox22 = new Telerik.Reporting.TextBox();
            this.table4 = new Telerik.Reporting.Table();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.textBox27 = new Telerik.Reporting.TextBox();
            this.ParticipantSupplierTranscriptODS = new Telerik.Reporting.ObjectDataSource();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.TranscriptWorkunitODS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.letterOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // SupplierODS
            // 
            this.SupplierODS.DataMember = "GetPartcipantSupplier";
            this.SupplierODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.ReportTemplateDS);
            this.SupplierODS.Name = "SupplierODS";
            this.SupplierODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(string), "= Parameters.id.Value")});
            // 
            // ParticiapantSupplierODS
            // 
            this.ParticiapantSupplierODS.DataMember = "GetParticiipanSuppliers";
            this.ParticiapantSupplierODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.TranscriptWithSupplierObjectDataSource);
            this.ParticiapantSupplierODS.Name = "ParticiapantSupplierODS";
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
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pbOrgLogo,
            this.txtOrgName,
            this.txtOrgNameAmharic,
            this.textBox5,
            this.textBox18});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.pbOrgLogo.MimeType = "";
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(1.0999606847763062D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.pbOrgLogo.Value = "= Fields.LogoFileName";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(3.9339065551757812E-05D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.8531243801116943D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // txtOrgNameAmharic
            // 
            this.txtOrgNameAmharic.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.4000000953674316D), Telerik.Reporting.Drawing.Unit.Inch(0.30000004172325134D));
            this.txtOrgNameAmharic.Name = "txtOrgNameAmharic";
            this.txtOrgNameAmharic.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.8531239032745361D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgNameAmharic.Style.Font.Bold = true;
            this.txtOrgNameAmharic.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgNameAmharic.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgNameAmharic.Value = "= Fields.AmharicName";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(0.90000003576278687D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9604161977767944D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox5.Value = "Ref.No.__________________";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(0.70000004768371582D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.9604165554046631D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox18.Value = "Date:   _________________";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(5.1000003814697266D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3,
            this.textBox1,
            this.textBox2,
            this.textBox4,
            this.textBox6,
            this.txtTo,
            this.txtsubjectmain,
            this.txtSubject,
            this.txtCity,
            this.table2,
            this.textBox21,
            this.textBox22,
            this.table4,
            this.textBox8,
            this.table1});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.053124111145734787D), Telerik.Reporting.Drawing.Unit.Inch(1.2895833253860474D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7468762397766113D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox3.Style.Font.Name = "Nyala";
            this.textBox3.Value = "= Fields.Let4";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.053124111145734787D), Telerik.Reporting.Drawing.Unit.Inch(0.99791669845581055D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7468762397766113D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox1.Style.Font.Name = "Nyala";
            this.textBox1.Value = "= Fields.Let1 + Fields.Branch + Fields.Let2 + Fields.TenderRefNo +  Fields.Let3";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.053122837096452713D), Telerik.Reporting.Drawing.Unit.Inch(1.6979168653488159D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7468776702880859D), Telerik.Reporting.Drawing.Unit.Inch(0.3020833432674408D));
            this.textBox2.Style.Font.Name = "Nyala";
            this.textBox2.Value = "";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.053124111145734787D), Telerik.Reporting.Drawing.Unit.Inch(2.097916841506958D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(5.7468762397766113D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.textBox4.Style.Font.Name = "Nyala";
            this.textBox4.Value = "= Fields.Summary";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.2000002861022949D), Telerik.Reporting.Drawing.Unit.Inch(2.4000000953674316D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5531238317489624D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox6.Style.Font.Name = "Nyala";
            this.textBox6.Value = "ከሰላምታ ጋር";
            // 
            // txtTo
            // 
            this.txtTo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.11041673272848129D), Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D));
            this.txtTo.Name = "txtTo";
            this.txtTo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.0895833969116211D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.txtTo.Style.Font.Bold = true;
            this.txtTo.Style.Font.Name = "Nyala";
            this.txtTo.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtTo.Style.Font.Underline = false;
            this.txtTo.Value = "ለ：- {= Parameters.supplier.Label}";
            // 
            // txtsubjectmain
            // 
            this.txtsubjectmain.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D));
            this.txtsubjectmain.Name = "txtsubjectmain";
            this.txtsubjectmain.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.9999217987060547D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.txtsubjectmain.Style.Font.Bold = true;
            this.txtsubjectmain.Style.Font.Name = "Nyala";
            this.txtsubjectmain.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtsubjectmain.Style.Font.Underline = true;
            this.txtsubjectmain.Value = "= Fields.Subject";
            // 
            // txtSubject
            // 
            this.txtSubject.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.59375D), Telerik.Reporting.Drawing.Unit.Inch(0.699999988079071D));
            this.txtSubject.Name = "txtSubject";
            this.txtSubject.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90000009536743164D), Telerik.Reporting.Drawing.Unit.Inch(0.20000012218952179D));
            this.txtSubject.Style.Color = System.Drawing.Color.Black;
            this.txtSubject.Style.Font.Bold = true;
            this.txtSubject.Style.Font.Name = "Nyala";
            this.txtSubject.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtSubject.Value = "ጉዳዩ :-";
            // 
            // txtCity
            // 
            this.txtCity.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(0.40000009536743164D));
            this.txtCity.Name = "txtCity";
            this.txtCity.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.txtCity.Style.Font.Bold = true;
            this.txtCity.Style.Font.Name = "Nyala";
            this.txtCity.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtCity.Style.Font.Underline = true;
            this.txtCity.Value = "= Fields.City";
            // 
            // table2
            // 
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.16666650772094727D)));
            this.table2.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.0833334922790527D)));
            this.table2.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table2.Body.SetCellContent(0, 1, this.textBox23);
            this.table2.Body.SetCellContent(0, 0, this.textBox24);
            tableGroup10.Name = "group5";
            tableGroup11.Name = "tableGroup1";
            this.table2.ColumnGroups.Add(tableGroup10);
            this.table2.ColumnGroups.Add(tableGroup11);
            this.table2.DataSource = this.LetThemKnowWorkunitODS;
            this.table2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox24,
            this.textBox23});
            this.table2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.16250006854534149D), Telerik.Reporting.Drawing.Unit.Inch(3.0000002384185791D));
            this.table2.Name = "table2";
            tableGroup12.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup12.Name = "detailTableGroup1";
            this.table2.RowGroups.Add(tableGroup12);
            this.table2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.25D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.table2.Style.Font.Name = "Nyala";
            // 
            // textBox23
            // 
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0833334922790527D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox23.Value = "= Fields.Name";
            // 
            // textBox24
            // 
            this.textBox24.Name = "textBox24";
            this.textBox24.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox24.StyleName = "";
            this.textBox24.Value = "+";
            // 
            // LetThemKnowWorkunitODS
            // 
            this.LetThemKnowWorkunitODS.DataMember = "GetLetThemKnows";
            this.LetThemKnowWorkunitODS.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LetThemKowWorkUnitObjectDataSource);
            this.LetThemKnowWorkunitODS.Name = "LetThemKnowWorkunitODS";
            this.LetThemKnowWorkunitODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("letthem", typeof(object[]), "= Parameters.letthemknow.Value")});
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(2.7999999523162842D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7853378057479858D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox21.Style.Font.Name = "Nyala";
            this.textBox21.Value = "እንዲያዉቁት";
            // 
            // textBox22
            // 
            this.textBox22.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.1000000610947609D), Telerik.Reporting.Drawing.Unit.Inch(3.3999998569488525D));
            this.textBox22.Name = "textBox22";
            this.textBox22.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.7853378057479858D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox22.Style.Font.Name = "Nyala";
            this.textBox22.Value = "ግልባጭ";
            // 
            // table4
            // 
            this.table4.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.16666653752326965D)));
            this.table4.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.0729169845581055D)));
            this.table4.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table4.Body.SetCellContent(0, 1, this.textBox25);
            this.table4.Body.SetCellContent(0, 0, this.textBox27);
            tableGroup13.Name = "group5";
            tableGroup14.Name = "tableGroup1";
            this.table4.ColumnGroups.Add(tableGroup13);
            this.table4.ColumnGroups.Add(tableGroup14);
            this.table4.DataSource = this.ParticipantSupplierTranscriptODS;
            this.table4.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox27,
            this.textBox25});
            this.table4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.16250006854534149D), Telerik.Reporting.Drawing.Unit.Inch(3.6000001430511475D));
            this.table4.Name = "table4";
            tableGroup15.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup15.Name = "detailTableGroup1";
            this.table4.RowGroups.Add(tableGroup15);
            this.table4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2395834922790527D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.table4.Style.Font.Name = "Nyala";
            // 
            // textBox25
            // 
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0729165077209473D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox25.Value = "= Fields.Name";
            // 
            // textBox27
            // 
            this.textBox27.Name = "textBox27";
            this.textBox27.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox27.StyleName = "";
            this.textBox27.Value = "+";
            // 
            // ParticipantSupplierTranscriptODS
            // 
            this.ParticipantSupplierTranscriptODS.DataMember = "GetSupplierForTranscript";
            this.ParticipantSupplierTranscriptODS.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.TranscriptWithSupplierObjectDataSource);
            this.ParticipantSupplierTranscriptODS.Name = "ParticipantSupplierTranscriptODS";
            this.ParticipantSupplierTranscriptODS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("participantsupplier", typeof(object[]), "= Parameters.participantsupplier.Value")});
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.40000000596046448D), Telerik.Reporting.Drawing.Unit.Inch(4.3000006675720215D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0853378772735596D), Telerik.Reporting.Drawing.Unit.Inch(0.19999980926513672D));
            this.textBox8.Value = "መ.ኮ.ኢ";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.16666653752326965D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(4.0729169845581055D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D)));
            this.table1.Body.SetCellContent(0, 1, this.textBox10);
            this.table1.Body.SetCellContent(0, 0, this.textBox11);
            tableGroup16.Name = "group5";
            tableGroup17.Name = "tableGroup1";
            this.table1.ColumnGroups.Add(tableGroup16);
            this.table1.ColumnGroups.Add(tableGroup17);
            this.table1.DataSource = this.TranscriptWorkunitODS;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox11,
            this.textBox10});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.16250006854534149D), Telerik.Reporting.Drawing.Unit.Inch(4.0000004768371582D));
            this.table1.Name = "table1";
            tableGroup18.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup18.Name = "detailTableGroup1";
            this.table1.RowGroups.Add(tableGroup18);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.2395834922790527D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.0729165077209473D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox10.Value = "= Fields.Name";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.1666666716337204D), Telerik.Reporting.Drawing.Unit.Inch(0.2395833283662796D));
            this.textBox11.StyleName = "";
            this.textBox11.Value = "+";
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
            // letterOds
            // 
            this.letterOds.DataMember = "GetPassFailAmharic";
            this.letterOds.DataSource = typeof(ExceedERP.Reporting.Procurements.DataSource.TechnicalPassFailObjectDataSource);
            this.letterOds.Name = "letterOds";
            // 
            // TechnicalPassFail
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1});
            this.Name = "TechnicalPassFail";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter6.AllowNull = true;
            reportParameter6.Name = "id";
            reportParameter6.Text = "ID";
            reportParameter7.AllowNull = true;
            reportParameter7.AvailableValues.DataSource = this.SupplierODS;
            reportParameter7.AvailableValues.DisplayMember = "= Fields.SupplierName";
            reportParameter7.AvailableValues.ValueMember = "= Fields.SupplierName";
            reportParameter7.Name = "supplier";
            reportParameter7.Text = "ተጫራቾች";
            reportParameter7.Visible = true;
            reportParameter8.AllowNull = true;
            reportParameter8.AvailableValues.DataSource = this.ParticiapantSupplierODS;
            reportParameter8.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter8.AvailableValues.ValueMember = "= Fields.Id";
            reportParameter8.MultiValue = true;
            reportParameter8.Name = "participantsupplier";
            reportParameter8.Text = "ግልባጭ ለተጫራቾች";
            reportParameter8.Value = "= First(Fields.Id)";
            reportParameter8.Visible = true;
            reportParameter9.AllowNull = true;
            reportParameter9.AvailableValues.DataSource = this.LetThemKnowODS;
            reportParameter9.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter9.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter9.MultiValue = true;
            reportParameter9.Name = "letthemknow";
            reportParameter9.Text = "እንዲያዉቁት";
            reportParameter9.Value = "= First(Fields.OrgStructureId)";
            reportParameter9.Visible = true;
            reportParameter10.AllowNull = true;
            reportParameter10.AvailableValues.DataSource = this.TranscriptODS;
            reportParameter10.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter10.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter10.MultiValue = true;
            reportParameter10.Name = "transcript";
            reportParameter10.Text = "ግልባጭ";
            reportParameter10.Value = "= First(Fields.OrgStructureId)";
            reportParameter10.Visible = true;
            this.ReportParameters.Add(reportParameter6);
            this.ReportParameters.Add(reportParameter7);
            this.ReportParameters.Add(reportParameter8);
            this.ReportParameters.Add(reportParameter9);
            this.ReportParameters.Add(reportParameter10);
            styleRule2.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule2.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule2.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule2});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.TextBox txtOrgNameAmharic;
        private Telerik.Reporting.ObjectDataSource letterOds;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.ObjectDataSource SupplierODS;
        private Telerik.Reporting.TextBox txtTo;
        private Telerik.Reporting.TextBox txtsubjectmain;
        private Telerik.Reporting.TextBox txtSubject;
        private Telerik.Reporting.TextBox txtCity;
        private Telerik.Reporting.ObjectDataSource LetThemKnowODS;
        private Telerik.Reporting.ObjectDataSource TranscriptODS;
        private Telerik.Reporting.Table table2;
        private Telerik.Reporting.TextBox textBox23;
        private Telerik.Reporting.TextBox textBox24;
        private Telerik.Reporting.ObjectDataSource LetThemKnowWorkunitODS;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox22;
        private Telerik.Reporting.Table table4;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.TextBox textBox27;
        private Telerik.Reporting.ObjectDataSource ParticipantSupplierTranscriptODS;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.ObjectDataSource ParticiapantSupplierODS;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.ObjectDataSource TranscriptWorkunitODS;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox18;
    }
}