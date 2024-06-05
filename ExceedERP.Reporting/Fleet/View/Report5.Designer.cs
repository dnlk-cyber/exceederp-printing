namespace ExceedERP.Reporting.Fleet.View
{
    partial class Report5
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
            Telerik.Reporting.TableGroup tableGroup9 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.Group group1 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group2 = new Telerik.Reporting.Group();
            Telerik.Reporting.Group group3 = new Telerik.Reporting.Group();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.detail = new Telerik.Reporting.DetailSection();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.groupHeaderSection = new Telerik.Reporting.GroupHeaderSection();
            this.groupFooterSection = new Telerik.Reporting.GroupFooterSection();
            this.groupHeaderSection1 = new Telerik.Reporting.GroupHeaderSection();
            this.groupFooterSection1 = new Telerik.Reporting.GroupFooterSection();
            this.objectDataSource1 = new Telerik.Reporting.ObjectDataSource();
            this.groupHeaderSection2 = new Telerik.Reporting.GroupHeaderSection();
            this.groupFooterSection2 = new Telerik.Reporting.GroupFooterSection();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox21 = new Telerik.Reporting.TextBox();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.70000004768371582D);
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox17,
            this.textBox18,
            this.textBox21,
            this.textBox2,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.textBox8});
            this.detail.Name = "detail";
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.5D);
            this.pageFooterSection1.Name = "pageFooterSection1";
            // 
            // groupHeaderSection
            // 
            this.groupHeaderSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.2000197172164917D);
            this.groupHeaderSection.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.groupHeaderSection.Name = "groupHeaderSection";
            // 
            // groupFooterSection
            // 
            this.groupFooterSection.Height = Telerik.Reporting.Drawing.Unit.Inch(0.0347222238779068D);
            this.groupFooterSection.Name = "groupFooterSection";
            // 
            // groupHeaderSection1
            // 
            this.groupHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20007885992527008D);
            this.groupHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox1});
            this.groupHeaderSection1.Name = "groupHeaderSection1";
            this.groupHeaderSection1.Style.BackgroundColor = System.Drawing.Color.Silver;
            // 
            // groupFooterSection1
            // 
            this.groupFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.099881492555141449D);
            this.groupFooterSection1.Name = "groupFooterSection1";
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataMember = "MachineryAndVehicle";
            this.objectDataSource1.DataSource = typeof(ExceedERP.Reporting.Fleet.DataSource.MachineryAndVehiclesStatusODS);
            this.objectDataSource1.Name = "objectDataSource1";
            this.objectDataSource1.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("equipmentTypeId", typeof(string), "= Parameters.equipmentTypeId.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.status.Value")});
            // 
            // groupHeaderSection2
            // 
            this.groupHeaderSection2.Height = Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D);
            this.groupHeaderSection2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox3});
            this.groupHeaderSection2.Name = "groupHeaderSection2";
            this.groupHeaderSection2.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            // 
            // groupFooterSection2
            // 
            this.groupFooterSection2.Height = Telerik.Reporting.Drawing.Unit.Inch(0.099881701171398163D);
            this.groupFooterSection2.Name = "groupFooterSection2";
            // 
            // textBox3
            // 
            this.textBox3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.1999998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox3.Value = "= Fields.MachineryAndVehicleType";
            // 
            // textBox1
            // 
            this.textBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D));
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(3.1999998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox1.Value = "= Fields.OwnRental";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.974999725818634D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.97499996423721313D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.974999725818634D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox9);
            this.table1.Body.SetCellContent(0, 2, this.textBox10);
            this.table1.Body.SetCellContent(0, 3, this.textBox11);
            this.table1.Body.SetCellContent(0, 5, this.textBox12);
            this.table1.Body.SetCellContent(0, 7, this.textBox14);
            this.table1.Body.SetCellContent(0, 4, this.textBox20);
            this.table1.Body.SetCellContent(0, 6, this.textBox7);
            this.table1.Body.SetCellContent(0, 1, this.textBox13);
            tableGroup1.Name = "machineryAndVehicleType";
            tableGroup2.Name = "group2";
            tableGroup3.Name = "make";
            tableGroup4.Name = "model";
            tableGroup5.Name = "group";
            tableGroup6.Name = "operator";
            tableGroup7.Name = "group1";
            tableGroup8.Name = "problem";
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.ColumnGroups.Add(tableGroup8);
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox9,
            this.textBox13,
            this.textBox10,
            this.textBox11,
            this.textBox20,
            this.textBox12,
            this.textBox7,
            this.textBox14});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(-0.12500020861625671D), Telerik.Reporting.Drawing.Unit.Inch(3.9458274841308594E-05D));
            this.table1.Name = "table1";
            tableGroup9.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup9.Name = "detail";
            this.table1.RowGroups.Add(tableGroup9);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.8249998092651367D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.table1.StyleName = "BlueOpal.TableNormal";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.StyleName = "BlueOpal.TableBody";
            this.textBox9.Value = "Machinery and Vehicle Type";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.StyleName = "BlueOpal.TableBody";
            this.textBox10.Value = "Make";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.StyleName = "BlueOpal.TableBody";
            this.textBox11.Value = "Model";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.StyleName = "BlueOpal.TableBody";
            this.textBox12.Value = "Oper";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000009059906006D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.StyleName = "BlueOpal.TableBody";
            this.textBox14.Value = "Problem";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.97499996423721313D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.StyleName = "BlueOpal.TableBody";
            this.textBox20.Value = "Ser/Plt";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.974999725818634D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.StyleName = "BlueOpal.TableBody";
            this.textBox7.Value = "Status";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.974999725818634D), Telerik.Reporting.Drawing.Unit.Inch(0.19998025894165039D));
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.StyleName = "BlueOpal.TableBody";
            this.textBox13.Value = "External";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(7.8837074397597462E-05D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.654999852180481D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox17.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox17.Value = "= Fields.MachineryAndVehicleType";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.654999852180481D), Telerik.Reporting.Drawing.Unit.Inch(7.8837074397597462E-05D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.974999725818634D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Value = "= Fields.OwnRental";
            // 
            // textBox21
            // 
            this.textBox21.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.6299996376037598D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox21.Name = "textBox21";
            this.textBox21.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000003099441528D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox21.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox21.Value = "= Fields.Model";
            // 
            // textBox2
            // 
            this.textBox2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.4099996089935303D), Telerik.Reporting.Drawing.Unit.Inch(7.8943041444290429E-05D));
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.79000008106231689D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Value = "= Fields.Model";
            // 
            // textBox4
            // 
            this.textBox4.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.2000787258148193D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.96492087841033936D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Value = "= Fields.SerialPlateNo";
            // 
            // textBox5
            // 
            this.textBox5.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.1649994850158691D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.78000003099441528D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Value = "= Fields.Operator";
            // 
            // textBox6
            // 
            this.textBox6.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(4.9450783729553223D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.974920928478241D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Value = "= Fields.Status";
            // 
            // textBox8
            // 
            this.textBox8.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.9200782775878906D), Telerik.Reporting.Drawing.Unit.Inch(7.915496826171875E-05D));
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.77992123365402222D), Telerik.Reporting.Drawing.Unit.Inch(0.20000001788139343D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Value = "= Fields.Problem";
            // 
            // Report5
            // 
            this.DataSource = this.objectDataSource1;
            group1.GroupFooter = this.groupFooterSection;
            group1.GroupHeader = this.groupHeaderSection;
            group1.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.OwnRental"));
            group1.Name = "group";
            group2.GroupFooter = this.groupFooterSection1;
            group2.GroupHeader = this.groupHeaderSection1;
            group2.Name = "group1";
            group3.GroupFooter = this.groupFooterSection2;
            group3.GroupHeader = this.groupHeaderSection2;
            group3.Name = "group2";
            this.Groups.AddRange(new Telerik.Reporting.Group[] {
            group1,
            group2,
            group3});
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.groupHeaderSection,
            this.groupFooterSection,
            this.groupHeaderSection1,
            this.groupFooterSection1,
            this.groupHeaderSection2,
            this.groupFooterSection2});
            this.Name = "Report5";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.Name = "equipmentTypeId";
            reportParameter1.Text = "equipmentTypeId";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.Name = "status";
            reportParameter2.Text = "status";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.6999998092651367D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource objectDataSource1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection;
        private Telerik.Reporting.GroupFooterSection groupFooterSection;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection1;
        private Telerik.Reporting.GroupFooterSection groupFooterSection1;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.GroupHeaderSection groupHeaderSection2;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.GroupFooterSection groupFooterSection2;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox21;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox13;
    }
}