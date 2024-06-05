namespace ExceedERP.Reporting.Fleet.View
{
    partial class MachineryAndVehiclesStatusReport
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
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.equipmentTypeParameter = new Telerik.Reporting.ObjectDataSource();
            this.statusParameterOds = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.pictureBox1 = new Telerik.Reporting.PictureBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox42 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.machineryAndVehicleOds = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.784722089767456D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox1.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox1.Value = "Make";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.784722089767456D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox3.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox3.Value = "Model";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.916666567325592D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox5.Value = "Serial/Plate No.";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox7.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox7.StyleName = "";
            this.textBox7.Value = "Operators/Drivers";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "Status";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "Problem";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.Gray;
            this.textBox13.StyleName = "";
            // 
            // equipmentTypeParameter
            // 
            this.equipmentTypeParameter.DataMember = "GetEquipmentType";
            this.equipmentTypeParameter.DataSource = typeof(ExceedERP.Reporting.Fleet.DataSource.MachineryAndVehicleStatusParameterOds);
            this.equipmentTypeParameter.Name = "equipmentTypeParameter";
            // 
            // statusParameterOds
            // 
            this.statusParameterOds.DataMember = "GetStatus";
            this.statusParameterOds.DataSource = typeof(ExceedERP.Reporting.Fleet.DataSource.MachineryAndVehicleStatusParameterOds);
            this.statusParameterOds.Name = "statusParameterOds";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3999999761581421D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pictureBox1,
            this.textBox15,
            this.textBox16,
            this.textBox19,
            this.textBox17,
            this.textBox18,
            this.textBox20,
            this.textBox42});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.084761463105678558D), Telerik.Reporting.Drawing.Unit.Inch(0.10000000894069672D));
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.69999992847442627D), Telerik.Reporting.Drawing.Unit.Inch(0.70000004768371582D));
            // 
            // textBox15
            // 
            this.textBox15.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox15.Value = "a";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.9999997615814209D), Telerik.Reporting.Drawing.Unit.Inch(0.5D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox16.Value = "b";
            // 
            // textBox19
            // 
            this.textBox19.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.7999606728553772D));
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4861106872558594D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox19.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox19.Value = "OCC Machinery & Vehicles Status Reporting Form";
            // 
            // textBox17
            // 
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.8000001907348633D), Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox17.Value = "Status Reporting Date";
            // 
            // textBox18
            // 
            this.textBox18.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.3000392913818359D), Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D));
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999999284744263D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox18.Value = "= Today()";
            // 
            // textBox20
            // 
            this.textBox20.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.0000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D));
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1999994516372681D), Telerik.Reporting.Drawing.Unit.Inch(0.19999991357326508D));
            this.textBox20.Value = "Name of Project";
            // 
            // textBox42
            // 
            this.textBox42.Format = "{0:d}";
            this.textBox42.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.2000784873962402D), Telerik.Reporting.Drawing.Unit.Inch(1.0999999046325684D));
            this.textBox42.Name = "textBox42";
            this.textBox42.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.28607177734375D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox42.Value = "c";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(1.5D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.table1});
            this.detail.Name = "detail";
            // 
            // table1
            // 
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78472226858139038D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.78472226858139038D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.91666668653488159D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999988079071045D)));
            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D)));
            this.table1.Body.SetCellContent(0, 0, this.textBox2);
            this.table1.Body.SetCellContent(0, 1, this.textBox4);
            this.table1.Body.SetCellContent(0, 2, this.textBox6);
            this.table1.Body.SetCellContent(0, 3, this.textBox8);
            this.table1.Body.SetCellContent(0, 4, this.textBox10);
            this.table1.Body.SetCellContent(0, 5, this.textBox12);
            this.table1.Body.SetCellContent(0, 6, this.textBox14);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.textBox1;
            tableGroup2.Name = "tableGroup1";
            tableGroup2.ReportItem = this.textBox3;
            tableGroup3.Name = "tableGroup2";
            tableGroup3.ReportItem = this.textBox5;
            tableGroup4.Name = "group";
            tableGroup4.ReportItem = this.textBox7;
            tableGroup5.Name = "group1";
            tableGroup5.ReportItem = this.textBox9;
            tableGroup6.Name = "group2";
            tableGroup6.ReportItem = this.textBox11;
            tableGroup7.Name = "group3";
            tableGroup7.ReportItem = this.textBox13;
            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);
            this.table1.ColumnGroups.Add(tableGroup7);
            this.table1.DataSource = this.machineryAndVehicleOds;
            this.table1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox4,
            this.textBox6,
            this.textBox8,
            this.textBox10,
            this.textBox12,
            this.textBox14,
            this.textBox1,
            this.textBox3,
            this.textBox5,
            this.textBox7,
            this.textBox9,
            this.textBox11,
            this.textBox13});
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(0.59999996423721313D));
            this.table1.Name = "table1";
            tableGroup8.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup8.Name = "detailTableGroup";
            this.table1.RowGroups.Add(tableGroup8);
            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(6.4861106872558594D), Telerik.Reporting.Drawing.Unit.Inch(0.59999996423721313D));
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.784722089767456D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox2.Value = "= Fields.Make";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.784722089767456D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox4.Value = "= Fields.Model";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.916666567325592D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox6.Value = "= Fields.SerialPlateNo";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.Operators";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox10.StyleName = "";
            this.textBox10.Value = "= Fields.Status";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox12.StyleName = "";
            this.textBox12.Value = "= Fields.Problem";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.29999998211860657D));
            this.textBox14.StyleName = "";
            // 
            // machineryAndVehicleOds
            // 
            this.machineryAndVehicleOds.DataMember = "MachineryAndVehicle";
            this.machineryAndVehicleOds.DataSource = typeof(ExceedERP.Reporting.Fleet.DataSource.MachineryAndVehiclesStatusODS);
            this.machineryAndVehicleOds.Name = "machineryAndVehicleOds";
            this.machineryAndVehicleOds.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("equipmentTypeId", typeof(string), "= Parameters.EquipmentType.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("status", typeof(string), "= Parameters.Status.Value")});
            // 
            // MachineryAndVehiclesStatusReport
            // 
            this.DataSource = this.machineryAndVehicleOds;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail});
            this.Name = "MachineryAndVehiclesStatusReport";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.equipmentTypeParameter;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.Id";
            reportParameter1.Name = "EquipmentType";
            reportParameter1.Text = "EquipmentType";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.statusParameterOds;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.Id";
            reportParameter2.Name = "Status";
            reportParameter2.Text = "Status";
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
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(6.5000786781311035D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.ObjectDataSource machineryAndVehicleOds;
        private Telerik.Reporting.PictureBox pictureBox1;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox textBox19;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox textBox42;
        private Telerik.Reporting.ObjectDataSource equipmentTypeParameter;
        private Telerik.Reporting.ObjectDataSource statusParameterOds;
    }
}