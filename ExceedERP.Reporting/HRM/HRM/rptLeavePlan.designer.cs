using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;
using ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource;

namespace ExceedERP.Reporting.HRM.HRM
{
    partial class rptLeavePlan
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
            Telerik.Reporting.TableGroup tableGroup10 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup11 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.Drawing.StyleRule styleRule1 = new Telerik.Reporting.Drawing.StyleRule();
            this.IdNumber = new Telerik.Reporting.TextBox();
            this.FullName = new Telerik.Reporting.TextBox();
            this.Departmant = new Telerik.Reporting.TextBox();
            this.Year = new Telerik.Reporting.TextBox();
            this.From = new Telerik.Reporting.TextBox();
            this.To = new Telerik.Reporting.TextBox();
            this.Amount = new Telerik.Reporting.TextBox();
            this.Remark = new Telerik.Reporting.TextBox();
            this.Signiture = new Telerik.Reporting.TextBox();
            this.BranchObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.DepartmentObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.PeriodobjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageHeaderSection1 = new Telerik.Reporting.PageHeaderSection();
            this.panel1 = new Telerik.Reporting.Panel();
            this.panel2 = new Telerik.Reporting.Panel();
            this.txtOrgName = new Telerik.Reporting.TextBox();
            this.pbOrgLogo = new Telerik.Reporting.PictureBox();
            this.textBox25 = new Telerik.Reporting.TextBox();
            this.Date = new Telerik.Reporting.TextBox();
            this.textBox17 = new Telerik.Reporting.TextBox();
            this.detail = new Telerik.Reporting.DetailSection();
            this.tblEmployeeSumm = new Telerik.Reporting.Table();
            this.textBox2 = new Telerik.Reporting.TextBox();
            this.textBox4 = new Telerik.Reporting.TextBox();
            this.textBox8 = new Telerik.Reporting.TextBox();
            this.textBox18 = new Telerik.Reporting.TextBox();
            this.textBox5 = new Telerik.Reporting.TextBox();
            this.textBox9 = new Telerik.Reporting.TextBox();
            this.textBox11 = new Telerik.Reporting.TextBox();
            this.textBox13 = new Telerik.Reporting.TextBox();
            this.textBox20 = new Telerik.Reporting.TextBox();
            this.textBox1 = new Telerik.Reporting.TextBox();
            this.textBox3 = new Telerik.Reporting.TextBox();
            this.textBox6 = new Telerik.Reporting.TextBox();
            this.textBox7 = new Telerik.Reporting.TextBox();
            this.textBox10 = new Telerik.Reporting.TextBox();
            this.textBox12 = new Telerik.Reporting.TextBox();
            this.textBox14 = new Telerik.Reporting.TextBox();
            this.textBox15 = new Telerik.Reporting.TextBox();
            this.textBox19 = new Telerik.Reporting.TextBox();
            this.LeavePlanObjectDataSource = new Telerik.Reporting.ObjectDataSource();
            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.textBox16 = new Telerik.Reporting.TextBox();
            this.Page = new Telerik.Reporting.TextBox();
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.shape3 = new Telerik.Reporting.Shape();
            this.LeavePlan = new Telerik.Reporting.TextBox();
            this.Organization = new Telerik.Reporting.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // IdNumber
            // 
            this.IdNumber.Name = "IdNumber";
            this.IdNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.87325698137283325D), Telerik.Reporting.Drawing.Unit.Inch(0.35416677594184875D));
            this.IdNumber.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.IdNumber.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.IdNumber.Style.Font.Bold = true;
            this.IdNumber.Style.Font.Name = "Nyala";
            this.IdNumber.StyleName = "Civic.TableHeader";
            this.IdNumber.Value = "የሰራተኛ መለያ ቁጥር";
            // 
            // FullName
            // 
            this.FullName.Name = "FullName";
            this.FullName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.892565131187439D), Telerik.Reporting.Drawing.Unit.Inch(0.35416680574417114D));
            this.FullName.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.FullName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.FullName.Style.Font.Bold = true;
            this.FullName.Style.Font.Name = "Nyala";
            this.FullName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.FullName.StyleName = "Civic.TableHeader";
            this.FullName.Value = "ሙሉ ስም";
            // 
            // Departmant
            // 
            this.Departmant.Name = "Departmant";
            this.Departmant.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.35416677594184875D));
            this.Departmant.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Departmant.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Departmant.Style.Font.Bold = true;
            this.Departmant.Style.Font.Name = "Nyala";
            this.Departmant.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Departmant.StyleName = "Civic.TableHeader";
            this.Departmant.Value = "የስራ ክፍል";
            // 
            // Year
            // 
            this.Year.Name = "Year";
            this.Year.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90254318714141846D), Telerik.Reporting.Drawing.Unit.Inch(0.35416677594184875D));
            this.Year.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Year.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Year.Style.Font.Bold = true;
            this.Year.Style.Font.Name = "Nyala";
            this.Year.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Year.StyleName = "Civic.TableHeader";
            this.Year.Value = "ዓመት";
            // 
            // From
            // 
            this.From.Name = "From";
            this.From.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.92708432674407959D), Telerik.Reporting.Drawing.Unit.Inch(0.35416680574417114D));
            this.From.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.From.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.From.Style.Font.Bold = true;
            this.From.Style.Font.Name = "Nyala";
            this.From.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.From.StyleName = "Civic.TableHeader";
            this.From.Value = "ከ";
            // 
            // To
            // 
            this.To.Name = "To";
            this.To.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583331346511841D), Telerik.Reporting.Drawing.Unit.Inch(0.35416680574417114D));
            this.To.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.To.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.To.Style.Font.Bold = true;
            this.To.Style.Font.Name = "Nyala";
            this.To.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.To.StyleName = "Civic.TableHeader";
            this.To.Value = "ለ";
            // 
            // Amount
            // 
            this.Amount.Name = "Amount";
            this.Amount.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66666662693023682D), Telerik.Reporting.Drawing.Unit.Inch(0.35416680574417114D));
            this.Amount.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Amount.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Amount.Style.Font.Bold = true;
            this.Amount.Style.Font.Name = "Nyala";
            this.Amount.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Amount.StyleName = "Civic.TableHeader";
            this.Amount.Value = "የዕረፍት ብዛት";
            // 
            // Remark
            // 
            this.Remark.Name = "Remark";
            this.Remark.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999994039535522D), Telerik.Reporting.Drawing.Unit.Inch(0.35416677594184875D));
            this.Remark.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Remark.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Remark.Style.Font.Bold = true;
            this.Remark.Style.Font.Name = "Nyala";
            this.Remark.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Remark.StyleName = "Civic.TableHeader";
            this.Remark.Value = "ምርመራ";
            // 
            // Signiture
            // 
            this.Signiture.Name = "Signiture";
            this.Signiture.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8437495231628418D), Telerik.Reporting.Drawing.Unit.Inch(0.35416680574417114D));
            this.Signiture.Style.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(174)))), ((int)(((byte)(173)))));
            this.Signiture.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.Signiture.Style.Font.Bold = true;
            this.Signiture.Style.Font.Name = "Nyala";
            this.Signiture.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.Signiture.StyleName = "Civic.TableHeader";
            this.Signiture.Value = "ፊርማ";
            // 
            // BranchObjectDataSource
            // 
            this.BranchObjectDataSource.DataMember = "GetBranch";
            this.BranchObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.BranchObjectDataSource);
            this.BranchObjectDataSource.Name = "BranchObjectDataSource";
            // 
            // DepartmentObjectDataSource
            // 
            this.DepartmentObjectDataSource.DataMember = "GetDepartment";
            this.DepartmentObjectDataSource.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.DepartmentObjectDataSource);
            this.DepartmentObjectDataSource.Name = "DepartmentObjectDataSource";
            // 
            // PeriodobjectDataSource
            // 
            this.PeriodobjectDataSource.DataMember = "GetLeavePeriods";
            this.PeriodobjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LeavePeriodsObjectDataSource);
            this.PeriodobjectDataSource.Name = "PeriodobjectDataSource";
            // 
            // pageHeaderSection1
            // 
            this.pageHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.3000396490097046D);
            this.pageHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel1});
            this.pageHeaderSection1.Name = "pageHeaderSection1";
            this.pageHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // panel1
            // 
            this.panel1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.panel2,
            this.Date,
            this.textBox17});
            this.panel1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.0041910805739462376D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.panel1.Name = "panel1";
            this.panel1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.8246555328369141D), Telerik.Reporting.Drawing.Unit.Inch(1.300039529800415D));
            // 
            // panel2
            // 
            this.panel2.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.txtOrgName,
            this.pbOrgLogo,
            this.textBox25});
            this.panel2.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(3.9418537198798731E-05D), Telerik.Reporting.Drawing.Unit.Inch(3.9378803194267675E-05D));
            this.panel2.Name = "panel2";
            this.panel2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.8246164321899414D), Telerik.Reporting.Drawing.Unit.Inch(1.0998821258544922D));
            this.panel2.Style.Font.Name = "Nyala";
            // 
            // txtOrgName
            // 
            this.txtOrgName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3000789880752564D), Telerik.Reporting.Drawing.Unit.Inch(0.30007860064506531D));
            this.txtOrgName.Name = "txtOrgName";
            this.txtOrgName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.5245370864868164D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.txtOrgName.Style.Font.Bold = true;
            this.txtOrgName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.txtOrgName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.txtOrgName.Value = "= Fields.Name";
            // 
            // pbOrgLogo
            // 
            this.pbOrgLogo.Bindings.Add(new Telerik.Reporting.Binding("Value", "= Fields.LogoFileName"));
            this.pbOrgLogo.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.pbOrgLogo.Name = "pbOrgLogo";
            this.pbOrgLogo.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3000000715255737D), Telerik.Reporting.Drawing.Unit.Inch(1.0998427867889404D));
            this.pbOrgLogo.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.Stretch;
            // 
            // textBox25
            // 
            this.textBox25.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(1.3084144592285156D), Telerik.Reporting.Drawing.Unit.Inch(3.929932790924795E-05D));
            this.textBox25.Name = "textBox25";
            this.textBox25.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.5162014961242676D), Telerik.Reporting.Drawing.Unit.Inch(0.29996061325073242D));
            this.textBox25.Style.Font.Bold = true;
            this.textBox25.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(12D);
            this.textBox25.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox25.Value = "= Fields.AmharicName";
            // 
            // Date
            // 
            this.Date.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.4958095550537109D), Telerik.Reporting.Drawing.Unit.Inch(1.1000396013259888D));
            this.Date.Name = "Date";
            this.Date.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.59565037488937378D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.Date.Style.Font.Bold = true;
            this.Date.Value = "Date:";
            // 
            // textBox17
            // 
            this.textBox17.Format = "{0:d}";
            this.textBox17.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.0958094596862793D), Telerik.Reporting.Drawing.Unit.Inch(1.1000396013259888D));
            this.textBox17.Name = "textBox17";
            this.textBox17.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748028755187988D), Telerik.Reporting.Drawing.Unit.Inch(0.19999997317790985D));
            this.textBox17.Value = "= Fields.Date";
            // 
            // detail
            // 
            this.detail.Height = Telerik.Reporting.Drawing.Unit.Inch(0.59378987550735474D);
            this.detail.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.tblEmployeeSumm});
            this.detail.Name = "detail";
            this.detail.Style.Font.Name = "Nyala";
            // 
            // tblEmployeeSumm
            // 
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.8732568621635437D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.892565131187439D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.0000002384185791D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.902542769908905D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.92708432674407959D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.895833432674408D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.66666668653488159D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.99999940395355225D)));
            this.tblEmployeeSumm.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.84374940395355225D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.23958346247673035D)));
            this.tblEmployeeSumm.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.19999998807907105D)));
            this.tblEmployeeSumm.Body.SetCellContent(0, 0, this.textBox2);
            this.tblEmployeeSumm.Body.SetCellContent(0, 3, this.textBox4);
            this.tblEmployeeSumm.Body.SetCellContent(0, 1, this.textBox8);
            this.tblEmployeeSumm.Body.SetCellContent(0, 4, this.textBox18);
            this.tblEmployeeSumm.Body.SetCellContent(0, 5, this.textBox5);
            this.tblEmployeeSumm.Body.SetCellContent(0, 6, this.textBox9);
            this.tblEmployeeSumm.Body.SetCellContent(0, 7, this.textBox11);
            this.tblEmployeeSumm.Body.SetCellContent(0, 8, this.textBox13);
            this.tblEmployeeSumm.Body.SetCellContent(0, 2, this.textBox20);
            this.tblEmployeeSumm.Body.SetCellContent(1, 0, this.textBox1);
            this.tblEmployeeSumm.Body.SetCellContent(1, 1, this.textBox3);
            this.tblEmployeeSumm.Body.SetCellContent(1, 2, this.textBox6);
            this.tblEmployeeSumm.Body.SetCellContent(1, 3, this.textBox7);
            this.tblEmployeeSumm.Body.SetCellContent(1, 4, this.textBox10);
            this.tblEmployeeSumm.Body.SetCellContent(1, 5, this.textBox12);
            this.tblEmployeeSumm.Body.SetCellContent(1, 6, this.textBox14);
            this.tblEmployeeSumm.Body.SetCellContent(1, 7, this.textBox15);
            this.tblEmployeeSumm.Body.SetCellContent(1, 8, this.textBox19);
            tableGroup1.Name = "tableGroup";
            tableGroup1.ReportItem = this.IdNumber;
            tableGroup2.Name = "group";
            tableGroup2.ReportItem = this.FullName;
            tableGroup3.Name = "group6";
            tableGroup3.ReportItem = this.Departmant;
            tableGroup4.Name = "tableGroup1";
            tableGroup4.ReportItem = this.Year;
            tableGroup5.Name = "group4";
            tableGroup5.ReportItem = this.From;
            tableGroup6.Name = "group1";
            tableGroup6.ReportItem = this.To;
            tableGroup7.Name = "group2";
            tableGroup7.ReportItem = this.Amount;
            tableGroup8.Name = "group3";
            tableGroup8.ReportItem = this.Remark;
            tableGroup9.Name = "group5";
            tableGroup9.ReportItem = this.Signiture;
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup1);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup2);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup3);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup4);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup5);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup6);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup7);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup8);
            this.tblEmployeeSumm.ColumnGroups.Add(tableGroup9);
            this.tblEmployeeSumm.DataSource = this.LeavePlanObjectDataSource;
            this.tblEmployeeSumm.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox2,
            this.textBox8,
            this.textBox20,
            this.textBox4,
            this.textBox18,
            this.textBox5,
            this.textBox9,
            this.textBox11,
            this.textBox13,
            this.textBox1,
            this.textBox3,
            this.textBox6,
            this.textBox7,
            this.textBox10,
            this.textBox12,
            this.textBox14,
            this.textBox15,
            this.textBox19,
            this.IdNumber,
            this.FullName,
            this.Departmant,
            this.Year,
            this.From,
            this.To,
            this.Amount,
            this.Remark,
            this.Signiture});
            this.tblEmployeeSumm.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.004230499267578125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.tblEmployeeSumm.Name = "tblEmployeeSumm";
            tableGroup10.Groupings.Add(new Telerik.Reporting.Grouping(null));
            tableGroup10.Name = "detailTableGroup";
            tableGroup11.Name = "group7";
            this.tblEmployeeSumm.RowGroups.Add(tableGroup10);
            this.tblEmployeeSumm.RowGroups.Add(tableGroup11);
            this.tblEmployeeSumm.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(9.00169849395752D), Telerik.Reporting.Drawing.Unit.Inch(0.79375022649765015D));
            this.tblEmployeeSumm.Sortings.Add(new Telerik.Reporting.Sorting("", Telerik.Reporting.SortDirection.Asc));
            this.tblEmployeeSumm.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tblEmployeeSumm.Style.BorderStyle.Left = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tblEmployeeSumm.StyleName = "Civic.TableHeader";
            // 
            // textBox2
            // 
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.873257040977478D), Telerik.Reporting.Drawing.Unit.Inch(0.23958349227905273D));
            this.textBox2.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox2.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.textBox2.Value = "= Fields.Id";
            // 
            // textBox4
            // 
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.90254318714141846D), Telerik.Reporting.Drawing.Unit.Inch(0.23958350718021393D));
            this.textBox4.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox4.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox4.Value = "= Fields.LeavePeriod";
            // 
            // textBox8
            // 
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.892565131187439D), Telerik.Reporting.Drawing.Unit.Inch(0.23958344757556915D));
            this.textBox8.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox8.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox8.StyleName = "";
            this.textBox8.Value = "= Fields.FullName";
            // 
            // textBox18
            // 
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.92708432674407959D), Telerik.Reporting.Drawing.Unit.Inch(0.23958344757556915D));
            this.textBox18.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox18.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox18.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox18.StyleName = "";
            this.textBox18.Value = "= Fields.From";
            // 
            // textBox5
            // 
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583331346511841D), Telerik.Reporting.Drawing.Unit.Inch(0.23958344757556915D));
            this.textBox5.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox5.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox5.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox5.StyleName = "";
            this.textBox5.Value = "= Fields.To";
            // 
            // textBox9
            // 
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66666662693023682D), Telerik.Reporting.Drawing.Unit.Inch(0.23958347737789154D));
            this.textBox9.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox9.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox9.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox9.StyleName = "";
            this.textBox9.Value = "= Fields.Amount";
            // 
            // textBox11
            // 
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958352208137512D));
            this.textBox11.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox11.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox11.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox11.StyleName = "";
            this.textBox11.Value = "= Fields.Remark";
            // 
            // textBox13
            // 
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8437495231628418D), Telerik.Reporting.Drawing.Unit.Inch(0.23958344757556915D));
            this.textBox13.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox13.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox13.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox13.StyleName = "";
            this.textBox13.Value = "";
            // 
            // textBox20
            // 
            this.textBox20.Name = "textBox20";
            this.textBox20.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1D), Telerik.Reporting.Drawing.Unit.Inch(0.23958349227905273D));
            this.textBox20.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox20.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox20.StyleName = "";
            this.textBox20.Value = "= Fields.Department";
            // 
            // textBox1
            // 
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.8732568621635437D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox1.Style.Font.Bold = true;
            this.textBox1.StyleName = "";
            this.textBox1.Value = "Total";
            // 
            // textBox3
            // 
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.892565131187439D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox3.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox3.Style.Font.Bold = true;
            this.textBox3.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox3.StyleName = "";
            this.textBox3.Value = "= Count(Fields.FullName)";
            // 
            // textBox6
            // 
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.0000002384185791D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox6.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox6.Style.Font.Bold = true;
            this.textBox6.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox6.StyleName = "";
            // 
            // textBox7
            // 
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.9025428295135498D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox7.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox7.Style.Font.Bold = true;
            this.textBox7.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox7.StyleName = "";
            // 
            // textBox10
            // 
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.92708438634872437D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox10.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox10.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox10.Style.Font.Bold = true;
            this.textBox10.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox10.StyleName = "";
            // 
            // textBox12
            // 
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.89583337306976318D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox12.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox12.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox12.Style.Font.Bold = true;
            this.textBox12.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox12.StyleName = "";
            // 
            // textBox14
            // 
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.66666668653488159D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox14.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox14.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox14.Style.Font.Bold = true;
            this.textBox14.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox14.StyleName = "";
            this.textBox14.Value = "= Sum(Fields.Amount)";
            // 
            // textBox15
            // 
            this.textBox15.Name = "textBox15";
            this.textBox15.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.99999940395355225D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox15.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox15.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox15.Style.Font.Bold = true;
            this.textBox15.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox15.StyleName = "";
            // 
            // textBox19
            // 
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.843749463558197D), Telerik.Reporting.Drawing.Unit.Inch(0.20000000298023224D));
            this.textBox19.Style.BackgroundColor = System.Drawing.Color.White;
            this.textBox19.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.textBox19.Style.Font.Bold = true;
            this.textBox19.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.textBox19.StyleName = "";
            // 
            // LeavePlanObjectDataSource
            // 
            this.LeavePlanObjectDataSource.DataMember = "GetLeavePlan";
            this.LeavePlanObjectDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.LeavePlanObjectDataSource);
            this.LeavePlanObjectDataSource.Name = "LeavePlanObjectDataSource";
            this.LeavePlanObjectDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
            new Telerik.Reporting.ObjectDataSourceParameter("period", typeof(string), "= Parameters.period.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("dept", typeof(string), "= Parameters.dept.Value"),
            new Telerik.Reporting.ObjectDataSourceParameter("branchId", typeof(string), "= Parameters.branchId.Value")});
            // 
            // pageFooterSection1
            // 
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.textBox16,
            this.Page});
            this.pageFooterSection1.Name = "pageFooterSection1";
            this.pageFooterSection1.Style.Font.Name = "Nyala";
            // 
            // textBox16
            // 
            this.textBox16.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(7.254004955291748D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.textBox16.Name = "textBox16";
            this.textBox16.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5748031139373779D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.textBox16.Style.Font.Bold = true;
            this.textBox16.Style.Font.Name = "Nyala";
            this.textBox16.Value = "Page: {PageNumber}";
            // 
            // Page
            // 
            this.Page.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6.8330483436584473D), Telerik.Reporting.Drawing.Unit.Inch(0.80000048875808716D));
            this.Page.Name = "Page";
            this.Page.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.42087754607200623D), Telerik.Reporting.Drawing.Unit.Inch(0.19995993375778198D));
            this.Page.Style.Font.Bold = true;
            this.Page.Style.Font.Name = "Nyala";
            this.Page.Value = "Page:";
            // 
            // reportHeaderSection1
            // 
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.39996051788330078D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.shape3,
            this.LeavePlan});
            this.reportHeaderSection1.Name = "reportHeaderSection1";
            this.reportHeaderSection1.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.None;
            this.reportHeaderSection1.Style.Font.Name = "Nyala";
            // 
            // shape3
            // 
            this.shape3.Angle = 0D;
            this.shape3.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.29996046423912048D));
            this.shape3.Name = "shape3";
            this.shape3.ShapeType = new Telerik.Reporting.Drawing.Shapes.LineShape(Telerik.Reporting.Drawing.Shapes.LineDirection.NWSE);
            this.shape3.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.9999217987060547D), Telerik.Reporting.Drawing.Unit.Point(3.75D));
            this.shape3.Stretch = true;
            this.shape3.Style.BorderStyle.Bottom = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderStyle.Top = Telerik.Reporting.Drawing.BorderType.Solid;
            this.shape3.Style.BorderWidth.Bottom = Telerik.Reporting.Drawing.Unit.Point(0.5D);
            this.shape3.Style.BorderWidth.Top = Telerik.Reporting.Drawing.Unit.Point(1.5D);
            this.shape3.Style.Color = System.Drawing.Color.Transparent;
            // 
            // LeavePlan
            // 
            this.LeavePlan.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.03125D), Telerik.Reporting.Drawing.Unit.Inch(3.9577484130859375E-05D));
            this.LeavePlan.Name = "LeavePlan";
            this.LeavePlan.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(8.7975578308105469D), Telerik.Reporting.Drawing.Unit.Inch(0.30000001192092896D));
            this.LeavePlan.Style.Font.Bold = true;
            this.LeavePlan.Style.Font.Name = "Nyala";
            this.LeavePlan.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(14D);
            this.LeavePlan.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.LeavePlan.Value = "የዓመት ፈቃድ ዕቅድ";
            // 
            // Organization
            // 
            this.Organization.DataMember = "GetOrganization";
            this.Organization.DataSource = typeof(ExceedERP.Reporting.SharedDataSources.OrganizationObjectDataSource);
            this.Organization.Name = "Organization";
            // 
            // rptLeavePlan
            // 
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
            this.pageHeaderSection1,
            this.detail,
            this.pageFooterSection1,
            this.reportHeaderSection1});
            this.Name = "Report1";
            this.PageSettings.Landscape = true;
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D), Telerik.Reporting.Drawing.Unit.Cm(1D));
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;
            reportParameter1.AllowNull = true;
            reportParameter1.AvailableValues.DataSource = this.BranchObjectDataSource;
            reportParameter1.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter1.AvailableValues.ValueMember = "= Fields.BranchId";
            reportParameter1.Name = "branchId";
            reportParameter1.Text = "Branch";
            reportParameter1.Visible = true;
            reportParameter2.AllowNull = true;
            reportParameter2.AvailableValues.DataSource = this.DepartmentObjectDataSource;
            reportParameter2.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter2.AvailableValues.ValueMember = "= Fields.OrgStructureId";
            reportParameter2.Name = "dept";
            reportParameter2.Text = "Work Unit";
            reportParameter2.Visible = true;
            reportParameter3.AllowNull = true;
            reportParameter3.AvailableValues.DataSource = this.PeriodobjectDataSource;
            reportParameter3.AvailableValues.DisplayMember = "= Fields.Name";
            reportParameter3.AvailableValues.ValueMember = "= Fields.GlFiscalYearId";
            reportParameter3.Name = "period";
            reportParameter3.Text = "Period";
            reportParameter3.Visible = true;
            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            styleRule1.Selectors.AddRange(new Telerik.Reporting.Drawing.ISelector[] {
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.TextItemBase)),
            new Telerik.Reporting.Drawing.TypeSelector(typeof(Telerik.Reporting.HtmlTextBox))});
            styleRule1.Style.Padding.Left = Telerik.Reporting.Drawing.Unit.Point(2D);
            styleRule1.Style.Padding.Right = Telerik.Reporting.Drawing.Unit.Point(2D);
            this.StyleSheet.AddRange(new Telerik.Reporting.Drawing.StyleRule[] {
            styleRule1});
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(8.9999608993530273D);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private Telerik.Reporting.PageHeaderSection pageHeaderSection1;
        private Telerik.Reporting.DetailSection detail;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.ObjectDataSource Organization;
        private Telerik.Reporting.Panel panel1;
        private Telerik.Reporting.ObjectDataSource LeavePlanObjectDataSource;
        private Telerik.Reporting.Table tblEmployeeSumm;
        private Telerik.Reporting.TextBox textBox2;
        private Telerik.Reporting.TextBox textBox4;
        private Telerik.Reporting.TextBox IdNumber;
        private Telerik.Reporting.TextBox Year;
        private Telerik.Reporting.TextBox textBox8;
        private Telerik.Reporting.TextBox FullName;
        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.Shape shape3;
        private Telerik.Reporting.TextBox LeavePlan;
        private Telerik.Reporting.TextBox textBox16;
        private Telerik.Reporting.TextBox Date;
        private Telerik.Reporting.TextBox textBox17;
        private Telerik.Reporting.Panel panel2;
        private Telerik.Reporting.TextBox txtOrgName;
        private Telerik.Reporting.PictureBox pbOrgLogo;
        private Telerik.Reporting.TextBox textBox25;
        private Telerik.Reporting.ObjectDataSource PeriodobjectDataSource;
        private Telerik.Reporting.TextBox textBox18;
        private Telerik.Reporting.TextBox From;
        private Telerik.Reporting.TextBox textBox5;
        private Telerik.Reporting.TextBox textBox9;
        private Telerik.Reporting.TextBox textBox11;
        private Telerik.Reporting.TextBox textBox13;
        private Telerik.Reporting.TextBox To;
        private Telerik.Reporting.TextBox Amount;
        private Telerik.Reporting.TextBox Remark;
        private Telerik.Reporting.TextBox Signiture;
        private Telerik.Reporting.ObjectDataSource DepartmentObjectDataSource;
        private Telerik.Reporting.TextBox textBox20;
        private Telerik.Reporting.TextBox Departmant;
        private Telerik.Reporting.ObjectDataSource BranchObjectDataSource;
        private Telerik.Reporting.TextBox Page;
        private Telerik.Reporting.TextBox textBox1;
        private Telerik.Reporting.TextBox textBox3;
        private Telerik.Reporting.TextBox textBox6;
        private Telerik.Reporting.TextBox textBox7;
        private Telerik.Reporting.TextBox textBox10;
        private Telerik.Reporting.TextBox textBox12;
        private Telerik.Reporting.TextBox textBox14;
        private Telerik.Reporting.TextBox textBox15;
        private Telerik.Reporting.TextBox textBox19;
    }
}