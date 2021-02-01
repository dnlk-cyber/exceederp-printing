namespace ExceedERP.Reporting.CRM.View
{
    partial class CustomerServiceUsage
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter4 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter5 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule2 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule3 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule4 = new Telerik.Reporting.Drawing.StyleRule();
            Telerik.Reporting.Drawing.StyleRule styleRule5 = new Telerik.Reporting.Drawing.StyleRule();
            this.typeGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.typeGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.labelsGroupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.labelsGroupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.serviceNameCaptionTextBox = new Telerik.Reporting.TextBox();
            this.customerCaptionTextBox = new Telerik.Reporting.TextBox();
            this.grandTotalCaptionTextBox = new Telerik.Reporting.TextBox();
            this.remarkCaptionTextBox = new Telerik.Reporting.TextBox();
            this.CustomerServiceDS = new Telerik.Reporting.ObjectDataSource();
            this.CustomerDS = new Telerik.Reporting.ObjectDataSource();
            this.ServiceTypeDS = new Telerik.Reporting.ObjectDataSource();
            this.pageFooter = new Telerik.Reporting.PageFooterSection();
            this.reportHeader = new Telerik.Reporting.ReportHeaderSection();
            this.textBox48 = new Telerik.Reporting.TextBox();
            this.textBox40 = new Telerik.Reporting.TextBox();
            this.textBox39 = new Telerik.Reporting.TextBox();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox23 = new Telerik.Reporting.TextBox();
            this.reportFooter = new Telerik.Reporting.ReportFooterSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.serviceNameDataTextBox = new Telerik.Reporting.TextBox();
            this.customerDataTextBox = new Telerik.Reporting.TextBox();
            this.grandTotalDataTextBox = new Telerik.Reporting.TextBox();
            this.remarkDataTextBox = new Telerik.Reporting.TextBox();
            this.CustomerServiceUsageDS = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // typeGroupFooterSection
            // 
            this.typeGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.typeGroupFooterSection.Name = "typeGroupFooterSection";
            // 
            // typeGroupHeaderSection
            // 
            this.typeGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.typeGroupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1,
            this.textBox2});
            this.typeGroupHeaderSection.Name = "typeGroupHeaderSection";
            // 
            // textBox1
            // 
            this.textBox1.CanGrow = true;
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.3372104167938232D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(4.6625890731811523D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox1.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.textBox1.StyleName = "Caption";
            this.textBox1.Value = "Type:";
            // 
            // textBox2
            // 
            this.textBox2.CanGrow = true;
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(6.9999995231628418D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(12.602499961853027D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.textBox2.StyleName = "Data";
            this.textBox2.Value = "= Fields.Type";
            // 
            // labelsGroupFooterSection
            // 
            this.labelsGroupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.labelsGroupFooterSection.Name = "labelsGroupFooterSection";
            this.labelsGroupFooterSection.Style.Visible = false;
            // 
            // labelsGroupHeaderSection
            // 
            this.labelsGroupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.labelsGroupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.serviceNameCaptionTextBox,
            this.customerCaptionTextBox,
            this.grandTotalCaptionTextBox,
            this.remarkCaptionTextBox});
            this.labelsGroupHeaderSection.Name = "labelsGroupHeaderSection";
            this.labelsGroupHeaderSection.PrintOnEveryPage = true;
            // 
            // serviceNameCaptionTextBox
            // 
            this.serviceNameCaptionTextBox.CanGrow = true;
            this.serviceNameCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.3372104167938232D), Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D));
            this.serviceNameCaptionTextBox.Name = "serviceNameCaptionTextBox";
            this.serviceNameCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.serviceNameCaptionTextBox.StyleName = "Caption";
            this.serviceNameCaptionTextBox.Value = "Service Name";
            // 
            // customerCaptionTextBox
            // 
            this.customerCaptionTextBox.CanGrow = true;
            this.customerCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(6.2248063087463379D), Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D));
            this.customerCaptionTextBox.Name = "customerCaptionTextBox";
            this.customerCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.customerCaptionTextBox.StyleName = "Caption";
            this.customerCaptionTextBox.Value = "Customer";
            // 
            // grandTotalCaptionTextBox
            // 
            this.grandTotalCaptionTextBox.CanGrow = true;
            this.grandTotalCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(10.11240291595459D), Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D));
            this.grandTotalCaptionTextBox.Name = "grandTotalCaptionTextBox";
            this.grandTotalCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.grandTotalCaptionTextBox.StyleName = "Caption";
            this.grandTotalCaptionTextBox.Value = "Grand Total";
            // 
            // remarkCaptionTextBox
            // 
            this.remarkCaptionTextBox.CanGrow = true;
            this.remarkCaptionTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(13.999999046325684D), Telerik.Reporting.Drawing.Unit.Cm(0.052916664630174637D));
            this.remarkCaptionTextBox.Name = "remarkCaptionTextBox";
            this.remarkCaptionTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(5.8999991416931152D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.remarkCaptionTextBox.StyleName = "Caption";
            this.remarkCaptionTextBox.Value = "Remark";
            // 
            // CustomerServiceDS
            // 
            this.CustomerServiceDS.DataMember = "GetCustomerServiceList";
            this.CustomerServiceDS.DataSource = typeof(ExceedERP.Reporting.CRM.DataSource.ServiceUsageDataSource);
            this.CustomerServiceDS.Name = "CustomerServiceDS";
            // 
            // CustomerDS
            // 
            this.CustomerDS.DataMember = "GetCustomerList";
            this.CustomerDS.DataSource = typeof(ExceedERP.Reporting.CRM.DataSource.ServiceUsageDataSource);
            this.CustomerDS.Name = "CustomerDS";
            // 
            // ServiceTypeDS
            // 
            this.ServiceTypeDS.DataMember = "GetServiceType";
            this.ServiceTypeDS.DataSource = typeof(ExceedERP.Reporting.CRM.DataSource.ServiceUsageDataSource);
            this.ServiceTypeDS.Name = "ServiceTypeDS";
            // 
            // pageFooter
            // 
            this.pageFooter.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.pageFooter.Name = "pageFooter";
            // 
            // reportHeader
            // 
            this.reportHeader.Height = Telerik.Reporting.Drawing.Unit.Cm(3.5343835353851318D);
            this.reportHeader.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox48,
            this.textBox40,
            this.textBox39,
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox23});
            this.reportHeader.Name = "reportHeader";
            // 
            // textBox48
            // 
            this.textBox48.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0852196216583252D), Telerik.Reporting.Drawing.Unit.Inch(1.0175050497055054D));
            this.textBox48.Name = "textBox48";
            this.textBox48.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.5562517642974854D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.textBox48.Style.Font.Bold = true;
            this.textBox48.Style.Font.Name = "Segoe UI";
            this.textBox48.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.textBox48.Value = "Service Usage Report";
            // 
            // textBox40
            // 
            this.textBox40.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.6000003814697266D), Telerik.Reporting.Drawing.Unit.Inch(1.1175050735473633D));
            this.textBox40.Name = "textBox40";
            this.textBox40.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox40.Style.Font.Bold = true;
            this.textBox40.Value = "Date:";
            // 
            // textBox39
            // 
            this.textBox39.Format = "{0:d}";
            this.textBox39.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.10007905960083D), Telerik.Reporting.Drawing.Unit.Inch(1.1175050735473633D));
            this.textBox39.Name = "textBox39";
            this.textBox39.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox39.Value = "= Today()";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.16882880032062531D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4500012397766113D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.txtOrgName.TextWrap = false;
            this.txtOrgName.Value = "= Fields.AmharicName";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.16882880032062531D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(0.99996060132980347D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox23
            // 
            this.textBox23.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.0852196216583252D), Telerik.Reporting.Drawing.Unit.Inch(0.5542454719543457D));
            this.textBox23.Name = "textBox23";
            this.textBox23.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4.4395856857299805D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox23.Style.Font.Bold = true;
            this.textBox23.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox23.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox23.TextWrap = false;
            this.textBox23.Value = "= Fields.Name";
            // 
            // reportFooter
            // 
            this.reportFooter.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.reportFooter.Name = "reportFooter";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Cm(0.71437495946884155D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.serviceNameDataTextBox,
            this.customerDataTextBox,
            this.grandTotalDataTextBox,
            this.remarkDataTextBox});
            this.detail.Name = "detail";
            // 
            // serviceNameDataTextBox
            // 
            this.serviceNameDataTextBox.CanGrow = true;
            this.serviceNameDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(2.3372104167938232D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.serviceNameDataTextBox.Name = "serviceNameDataTextBox";
            this.serviceNameDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.serviceNameDataTextBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.serviceNameDataTextBox.StyleName = "Data";
            this.serviceNameDataTextBox.Value = "= Fields.ServiceName";
            // 
            // customerDataTextBox
            // 
            this.customerDataTextBox.CanGrow = true;
            this.customerDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(6.2248063087463379D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.customerDataTextBox.Name = "customerDataTextBox";
            this.customerDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.customerDataTextBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.customerDataTextBox.StyleName = "Data";
            this.customerDataTextBox.Value = "= Fields.Customer";
            // 
            // grandTotalDataTextBox
            // 
            this.grandTotalDataTextBox.CanGrow = true;
            this.grandTotalDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(10.11240291595459D), Telerik.Reporting.Drawing.Unit.Cm(0.00010012308484874666D));
            this.grandTotalDataTextBox.Name = "grandTotalDataTextBox";
            this.grandTotalDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(3.8873958587646484D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.grandTotalDataTextBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.grandTotalDataTextBox.StyleName = "Data";
            this.grandTotalDataTextBox.Value = "= Fields.GrandTotal";
            // 
            // remarkDataTextBox
            // 
            this.remarkDataTextBox.CanGrow = true;
            this.remarkDataTextBox.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Cm(13.999999046325684D), Telerik.Reporting.Drawing.Unit.Cm(0D));
            this.remarkDataTextBox.Name = "remarkDataTextBox";
            this.remarkDataTextBox.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Cm(5.8749990463256836D), Telerik.Reporting.Drawing.Unit.Cm(0.60000002384185791D));
            this.remarkDataTextBox.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.remarkDataTextBox.StyleName = "Data";
            this.remarkDataTextBox.Value = "= Fields.Remark";
            // 
            // CustomerServiceUsageDS
            // 
            this.CustomerServiceUsageDS.DataMember = "GetServiceUsageList";
            this.CustomerServiceUsageDS.DataSource = typeof(ExceedERP.Reporting.CRM.DataSource.ServiceUsageDataSource);
            this.CustomerServiceUsageDS.Name = "CustomerServiceUsageDS";
            this.CustomerServiceUsageDS.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("ServiceName", typeof(string), "= Parameters.ServiceName.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("customer", typeof(string), "= Parameters.customer.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("Type", typeof(string), "= Parameters.Type.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("startingDate", typeof(System.DateTime), "= Parameters.startingDate.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("endingDate", typeof(System.DateTime), "= Parameters.endingDate.Value")});
            // 
            // CustomerServiceUsage
            // 
            this.DataSource = this.CustomerServiceUsageDS;
            group1.GroupFooter = this.typeGroupFooterSection;
            group1.GroupHeader = this.typeGroupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.Type"));
            group1.Name = "typeGroup";
            group2.GroupFooter = this.labelsGroupFooterSection;
            group2.GroupHeader = this.labelsGroupHeaderSection;
            group2.Name = "labelsGroup";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.typeGroupHeaderSection,
            this.typeGroupFooterSection,
            this.labelsGroupHeaderSection,
            this.labelsGroupFooterSection,
            this.pageFooter,
            this.reportHeader,
            this.reportFooter,
            this.detail});
            this.Name = "CustomerServiceUsage";
            this.PageSettings.ContinuousPaper = false;
            this.PageSettings.Landscape = false;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D), Telerik.Reporting.Drawing.Unit.Mm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
            reportParameter1.AllowBlank = false;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.CustomerServiceDS;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Description";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Description";
            reportParameter1.Name = "ServiceName";
            reportParameter1.Text = "Service Name";
            reportParameter1.Visible = true;
            reportParameter2.AllowBlank = false;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.CustomerDS;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.TradeName";
            reportParameter2.AvailableValues.ValueMember = "= Fields.TradeName";
            reportParameter2.Name = "customer";
            reportParameter2.Text = "Customer";
            reportParameter2.Visible = true;
            reportParameter3.AllowBlank = false;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.ServiceTypeDS;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Text";
            reportParameter3.AvailableValues.ValueMember = "= Fields.Text";
            reportParameter3.Name = "Type";
            reportParameter3.Text = "Service Type";
            reportParameter3.Visible = true;
            reportParameter4.AllowBlank = false;
            reportParameter4.Name = "startingDate";
            reportParameter4.Text = "Start Date";
            reportParameter4.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter4.Value = "= Now()";
            reportParameter4.Visible = true;
            reportParameter5.AllowBlank = false;
            reportParameter5.Name = "endingDate";
            reportParameter5.Text = "To Date";
            reportParameter5.Type = Telerik.Reporting.ReportParameterType.DateTime;
            reportParameter5.Value = "= Now()";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Cm(20.600000381469727D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.ObjectDataSource CustomerServiceUsageDS;
        private Telerik.Reporting.ObjectDataSource CustomerServiceDS;
        private Telerik.Reporting.ObjectDataSource CustomerDS;
        private Telerik.Reporting.ObjectDataSource ServiceTypeDS;
        private Telerik.Reporting.GroupHeaderSection typeGroupHeaderSection;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.GroupFooterSection typeGroupFooterSection;
        private Telerik.Reporting.GroupHeaderSection labelsGroupHeaderSection;
        private Telerik.Reporting.TextBox serviceNameCaptionTextBox;
        private Telerik.Reporting.TextBox customerCaptionTextBox;
        private Telerik.Reporting.TextBox grandTotalCaptionTextBox;
        private Telerik.Reporting.TextBox remarkCaptionTextBox;
        private Telerik.Reporting.GroupFooterSection labelsGroupFooterSection;
        private Telerik.Reporting.PageFooterSection pageFooter;
        private Telerik.Reporting.ReportHeaderSection reportHeader;
        private Telerik.Reporting.ReportFooterSection reportFooter;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.TextBox serviceNameDataTextBox;
        private Telerik.Reporting.TextBox customerDataTextBox;
        private Telerik.Reporting.TextBox grandTotalDataTextBox;
        private Telerik.Reporting.TextBox remarkDataTextBox;
        private Telerik.Reporting.TextBox textBox48;
        private Telerik.Reporting.TextBox textBox40;
        private Telerik.Reporting.TextBox textBox39;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox23;

    }
}