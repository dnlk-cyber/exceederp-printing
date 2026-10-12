using System;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    partial class DailyLaborPrintOut
    {
        #region Component Designer generated code
        /// <summary>
        /// Required method for Telerik Reporting designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Telerik.Reporting.InstanceReportSource instanceReportSource1 = new Telerik.Reporting.InstanceReportSource();
            Telerik.Reporting.TableGroup tableGroup1 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup2 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup3 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup4 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup5 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup6 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.TableGroup tableGroup7 = new Telerik.Reporting.TableGroup();
            Telerik.Reporting.ReportParameter reportParameter1 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter2 = new Telerik.Reporting.ReportParameter();
            Telerik.Reporting.ReportParameter reportParameter3 = new Telerik.Reporting.ReportParameter();

            this.headerSubReport1 = new ExceedERP.Reporting.SharedDesigners.HeaderSubReport();

            // Table Headers
            this.thSN = new Telerik.Reporting.TextBox();
            this.thEmployeeName = new Telerik.Reporting.TextBox();
            this.thTotalDays = new Telerik.Reporting.TextBox();
            this.thTotalWage = new Telerik.Reporting.TextBox();
            this.thIncomeTax = new Telerik.Reporting.TextBox();
            this.thNetPay = new Telerik.Reporting.TextBox();

            // Data Sources
            this.dailyLaborDataSource = new Telerik.Reporting.ObjectDataSource();

            // Sections & Table
            this.reportHeaderSection1 = new Telerik.Reporting.ReportHeaderSection();
            this.subReport1 = new Telerik.Reporting.SubReport();
            this.txtBranchLabel = new Telerik.Reporting.TextBox();
            this.txtPeriodLabel = new Telerik.Reporting.TextBox();

            this.detailSection1 = new Telerik.Reporting.DetailSection();
            this.table1 = new Telerik.Reporting.Table();

            // Table Detail Textboxes
            this.tbSN = new Telerik.Reporting.TextBox();
            this.tbEmployeeName = new Telerik.Reporting.TextBox();
            this.tbTotalDays = new Telerik.Reporting.TextBox();
            this.tbTotalWage = new Telerik.Reporting.TextBox();
            this.tbIncomeTax = new Telerik.Reporting.TextBox();
            this.tbNetPay = new Telerik.Reporting.TextBox();

            // Report Footer Section (Approvals & Signatures)
            this.reportFooterSection1 = new Telerik.Reporting.ReportFooterSection();

            // Approver Boxes (Prepared, Site, Material, Office, HRA, Authorized)
            this.txtPreparedLabel = new Telerik.Reporting.TextBox();
            this.txtPreparedName = new Telerik.Reporting.TextBox();
            this.picPreparedSig = new Telerik.Reporting.PictureBox();

            this.txtSiteLabel = new Telerik.Reporting.TextBox();
            this.txtSiteName = new Telerik.Reporting.TextBox();
            this.picSiteSig = new Telerik.Reporting.PictureBox();

            this.txtMaterialLabel = new Telerik.Reporting.TextBox();
            this.txtMaterialName = new Telerik.Reporting.TextBox();
            this.picMaterialSig = new Telerik.Reporting.PictureBox();

            this.txtOfficeLabel = new Telerik.Reporting.TextBox();
            this.txtOfficeName = new Telerik.Reporting.TextBox();
            this.picOfficeSig = new Telerik.Reporting.PictureBox();

            this.txtHraLabel = new Telerik.Reporting.TextBox();
            this.txtHraName = new Telerik.Reporting.TextBox();
            this.picHraSig = new Telerik.Reporting.PictureBox();

            this.txtAuthLabel = new Telerik.Reporting.TextBox();
            this.txtAuthName = new Telerik.Reporting.TextBox();
            this.picAuthSig = new Telerik.Reporting.PictureBox();

            this.pageFooterSection1 = new Telerik.Reporting.PageFooterSection();
            this.txtPageNumber = new Telerik.Reporting.TextBox();

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();

            // ---------------------------------------------------------
            // headerSubReport1
            // ---------------------------------------------------------
            this.headerSubReport1.Name = "HeaderSubReport";

            // ---------------------------------------------------------
            // Table Header Styling (Inline)
            // ---------------------------------------------------------
            this.thSN.Name = "thSN";
            this.thSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.50D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thSN.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thSN.Style.Color = System.Drawing.Color.White;
            this.thSN.Style.Font.Bold = true;
            this.thSN.Style.Font.Name = "Segoe UI";
            this.thSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thSN.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thSN.Value = "S/N";

            this.thEmployeeName.Name = "thEmployeeName";
            this.thEmployeeName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.40D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thEmployeeName.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thEmployeeName.Style.Color = System.Drawing.Color.White;
            this.thEmployeeName.Style.Font.Bold = true;
            this.thEmployeeName.Style.Font.Name = "Segoe UI";
            this.thEmployeeName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thEmployeeName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thEmployeeName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thEmployeeName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thEmployeeName.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thEmployeeName.Value = "Employee Name";

            this.thTotalDays.Name = "thTotalDays";
            this.thTotalDays.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.10D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thTotalDays.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thTotalDays.Style.Color = System.Drawing.Color.White;
            this.thTotalDays.Style.Font.Bold = true;
            this.thTotalDays.Style.Font.Name = "Segoe UI";
            this.thTotalDays.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thTotalDays.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thTotalDays.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thTotalDays.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thTotalDays.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thTotalDays.Value = "Total Days";

            this.thTotalWage.Name = "thTotalWage";
            this.thTotalWage.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.30D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thTotalWage.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thTotalWage.Style.Color = System.Drawing.Color.White;
            this.thTotalWage.Style.Font.Bold = true;
            this.thTotalWage.Style.Font.Name = "Segoe UI";
            this.thTotalWage.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thTotalWage.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thTotalWage.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thTotalWage.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thTotalWage.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thTotalWage.Value = "Total Wage";

            this.thIncomeTax.Name = "thIncomeTax";
            this.thIncomeTax.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.20D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thIncomeTax.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thIncomeTax.Style.Color = System.Drawing.Color.White;
            this.thIncomeTax.Style.Font.Bold = true;
            this.thIncomeTax.Style.Font.Name = "Segoe UI";
            this.thIncomeTax.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thIncomeTax.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thIncomeTax.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thIncomeTax.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thIncomeTax.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thIncomeTax.Value = "Income Tax";

            this.thNetPay.Name = "thNetPay";
            this.thNetPay.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.30D), Telerik.Reporting.Drawing.Unit.Inch(0.40D));
            this.thNetPay.Style.BackgroundColor = System.Drawing.Color.FromArgb(10, 77, 140);
            this.thNetPay.Style.Color = System.Drawing.Color.White;
            this.thNetPay.Style.Font.Bold = true;
            this.thNetPay.Style.Font.Name = "Segoe UI";
            this.thNetPay.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9.5D);
            this.thNetPay.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.thNetPay.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.thNetPay.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.thNetPay.Style.BorderColor.Default = System.Drawing.Color.LightGray;
            this.thNetPay.Value = "Net Pay";

            // ---------------------------------------------------------
            // Object Data Source Configuration
            // ---------------------------------------------------------
            this.dailyLaborDataSource.DataMember = "GetDailyLaborPrintOutData";
            this.dailyLaborDataSource.DataSource = typeof(ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource.DailyLaborPrintOutObjectDataSource);
            this.dailyLaborDataSource.Name = "dailyLaborDataSource";
            this.dailyLaborDataSource.Parameters.AddRange(new Telerik.Reporting.ObjectDataSourceParameter[] {
                new Telerik.Reporting.ObjectDataSourceParameter("id", typeof(int), "= Parameters.id.Value")
            });

            // ---------------------------------------------------------
            // Report Header Section
            // ---------------------------------------------------------
            this.reportHeaderSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.30D);
            this.reportHeaderSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.subReport1,
                this.txtBranchLabel,
                this.txtPeriodLabel
            });
            this.reportHeaderSection1.Name = "reportHeaderSection1";

            this.subReport1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0D));
            this.subReport1.Name = "subReport1";
            instanceReportSource1.ReportDocument = this.headerSubReport1;
            this.subReport1.ReportSource = instanceReportSource1;
            this.subReport1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.80D), Telerik.Reporting.Drawing.Unit.Inch(0.55D));

            this.txtBranchLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.65D));
            this.txtBranchLabel.Name = "txtBranchLabel";
            this.txtBranchLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtBranchLabel.Style.Font.Bold = true;
            this.txtBranchLabel.Style.Font.Name = "Segoe UI";
            this.txtBranchLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.txtBranchLabel.Value = "=\"Branch / Project: \" + Fields.BranchName";

            this.txtPeriodLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.95D));
            this.txtPeriodLabel.Name = "txtPeriodLabel";
            this.txtPeriodLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(4D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtPeriodLabel.Style.Font.Bold = true;
            this.txtPeriodLabel.Style.Font.Name = "Segoe UI";
            this.txtPeriodLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(10D);
            this.txtPeriodLabel.Value = "=\"Payroll Period: \" + Fields.PeriodName + \" (\" + Fields.FiscalYearName + \")\"";

            // ---------------------------------------------------------
            // Detail Section & Table
            // ---------------------------------------------------------
            this.detailSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(1.50D);
            this.detailSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] { this.table1 });
            this.detailSection1.Name = "detailSection1";

            this.table1.Bindings.Add(new Telerik.Reporting.Binding("DataSource", "= Fields.LaborerEmployees"));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(0.50D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(2.40D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.10D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.30D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.20D)));
            this.table1.Body.Columns.Add(new Telerik.Reporting.TableBodyColumn(Telerik.Reporting.Drawing.Unit.Inch(1.30D)));

            this.table1.Body.Rows.Add(new Telerik.Reporting.TableBodyRow(Telerik.Reporting.Drawing.Unit.Inch(0.25D)));

            this.table1.Body.SetCellContent(0, 0, this.tbSN);
            this.table1.Body.SetCellContent(0, 1, this.tbEmployeeName);
            this.table1.Body.SetCellContent(0, 2, this.tbTotalDays);
            this.table1.Body.SetCellContent(0, 3, this.tbTotalWage);
            this.table1.Body.SetCellContent(0, 4, this.tbIncomeTax);
            this.table1.Body.SetCellContent(0, 5, this.tbNetPay);

            tableGroup1.ReportItem = this.thSN;
            tableGroup2.ReportItem = this.thEmployeeName;
            tableGroup3.ReportItem = this.thTotalDays;
            tableGroup4.ReportItem = this.thTotalWage;
            tableGroup5.ReportItem = this.thIncomeTax;
            tableGroup6.ReportItem = this.thNetPay;

            this.table1.ColumnGroups.Add(tableGroup1);
            this.table1.ColumnGroups.Add(tableGroup2);
            this.table1.ColumnGroups.Add(tableGroup3);
            this.table1.ColumnGroups.Add(tableGroup4);
            this.table1.ColumnGroups.Add(tableGroup5);
            this.table1.ColumnGroups.Add(tableGroup6);

            this.table1.DataSource = null;
            this.table1.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0D), Telerik.Reporting.Drawing.Unit.Inch(0.10D));
            this.table1.Name = "table1";

            // Row group iteration over collection items
            tableGroup7.Groupings.Add(new Telerik.Reporting.Grouping("= Fields.EmployeeFullName"));
            tableGroup7.Name = "detailRow";
            this.table1.RowGroups.Add(tableGroup7);

            this.table1.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(7.80D), Telerik.Reporting.Drawing.Unit.Inch(0.65D));

            // Detail Textboxes (Direct item bindings)
            this.tbSN.Name = "tbSN";
            this.tbSN.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(0.5D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbSN.Style.Font.Name = "Segoe UI";
            this.tbSN.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbSN.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbSN.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbSN.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbSN.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbSN.Value = "= RowNumber(\"table1\")";

            this.tbEmployeeName.Name = "tbEmployeeName";
            this.tbEmployeeName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.4D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbEmployeeName.Style.Font.Name = "Segoe UI";
            this.tbEmployeeName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbEmployeeName.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Left;
            this.tbEmployeeName.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbEmployeeName.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbEmployeeName.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbEmployeeName.Value = "= Fields.EmployeeFullName";

            this.tbTotalDays.Name = "tbTotalDays";
            this.tbTotalDays.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.1D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbTotalDays.Style.Font.Name = "Segoe UI";
            this.tbTotalDays.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbTotalDays.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Center;
            this.tbTotalDays.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbTotalDays.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbTotalDays.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbTotalDays.Value = "= Fields.TotalDays";

            this.tbTotalWage.Name = "tbTotalWage";
            this.tbTotalWage.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbTotalWage.Style.Font.Name = "Segoe UI";
            this.tbTotalWage.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbTotalWage.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbTotalWage.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbTotalWage.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbTotalWage.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbTotalWage.Value = "= Format(\"{0:N2}\", Fields.TotalWage)";

            this.tbIncomeTax.Name = "tbIncomeTax";
            this.tbIncomeTax.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbIncomeTax.Style.Font.Name = "Segoe UI";
            this.tbIncomeTax.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbIncomeTax.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbIncomeTax.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbIncomeTax.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbIncomeTax.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbIncomeTax.Value = "= Format(\"{0:N2}\", Fields.IncomeTax)";

            this.tbNetPay.Name = "tbNetPay";
            this.tbNetPay.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.3D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.tbNetPay.Style.Font.Name = "Segoe UI";
            this.tbNetPay.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(9D);
            this.tbNetPay.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.tbNetPay.Style.VerticalAlign = Telerik.Reporting.Drawing.VerticalAlign.Middle;
            this.tbNetPay.Style.BorderStyle.Default = Telerik.Reporting.Drawing.BorderType.Solid;
            this.tbNetPay.Style.BorderColor.Default = System.Drawing.Color.Gainsboro;
            this.tbNetPay.Value = "= Format(\"{0:N2}\", Fields.NetPay)";

            // ---------------------------------------------------------
            // Report Footer Section (Workflow Approvers & Signatures)
            // ---------------------------------------------------------
            this.reportFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(2.20D);
            this.reportFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.txtPreparedLabel, this.txtPreparedName, this.picPreparedSig,
                this.txtSiteLabel, this.txtSiteName, this.picSiteSig,
                this.txtMaterialLabel, this.txtMaterialName, this.picMaterialSig,
                this.txtOfficeLabel, this.txtOfficeName, this.picOfficeSig,
                this.txtHraLabel, this.txtHraName, this.picHraSig,
                this.txtAuthLabel, this.txtAuthName, this.picAuthSig
            });
            this.reportFooterSection1.Name = "reportFooterSection1";

            // --- 1. Prepared By ---
            this.txtPreparedLabel.Name = "txtPreparedLabel";
            this.txtPreparedLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.txtPreparedLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtPreparedLabel.Style.Font.Bold = true;
            this.txtPreparedLabel.Style.Font.Name = "Segoe UI";
            this.txtPreparedLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtPreparedLabel.Value = "Prepared By (Personnel)";

            this.picPreparedSig.Name = "picPreparedSig";
            this.picPreparedSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.32D));
            this.picPreparedSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picPreparedSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picPreparedSig.Value = "= Fields.PreparedByHR.Signature";

            this.txtPreparedName.Name = "txtPreparedName";
            this.txtPreparedName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(0.8D));
            this.txtPreparedName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtPreparedName.Style.Font.Name = "Segoe UI";
            this.txtPreparedName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtPreparedName.Style.Color = System.Drawing.Color.DimGray;
            this.txtPreparedName.Value = "= Fields.PreparedByHR.FullName + \" (\" + Fields.PreparedByHR.JobTitle + \")\"";

            // --- 2. Checked By (Site) ---
            this.txtSiteLabel.Name = "txtSiteLabel";
            this.txtSiteLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.txtSiteLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtSiteLabel.Style.Font.Bold = true;
            this.txtSiteLabel.Style.Font.Name = "Segoe UI";
            this.txtSiteLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtSiteLabel.Value = "Checked By (Site / Foreman)";

            this.picSiteSig.Name = "picSiteSig";
            this.picSiteSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(0.32D));
            this.picSiteSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picSiteSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picSiteSig.Value = "= Fields.CheckedBySite.Signature";

            this.txtSiteName.Name = "txtSiteName";
            this.txtSiteName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(0.8D));
            this.txtSiteName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtSiteName.Style.Font.Name = "Segoe UI";
            this.txtSiteName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtSiteName.Style.Color = System.Drawing.Color.DimGray;
            this.txtSiteName.Value = "= Fields.CheckedBySite.FullName + \" (\" + Fields.CheckedBySite.JobTitle + \")\"";

            // --- 3. Approved By (Project HR) ---
            this.txtMaterialLabel.Name = "txtMaterialLabel";
            this.txtMaterialLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(0.1D));
            this.txtMaterialLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtMaterialLabel.Style.Font.Bold = true;
            this.txtMaterialLabel.Style.Font.Name = "Segoe UI";
            this.txtMaterialLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtMaterialLabel.Value = "Approved By (Project HR)";

            this.picMaterialSig.Name = "picMaterialSig";
            this.picMaterialSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(0.32D));
            this.picMaterialSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picMaterialSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picMaterialSig.Value = "= Fields.ApprovedByMaterial.Signature";

            this.txtMaterialName.Name = "txtMaterialName";
            this.txtMaterialName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(0.8D));
            this.txtMaterialName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtMaterialName.Style.Font.Name = "Segoe UI";
            this.txtMaterialName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtMaterialName.Style.Color = System.Drawing.Color.DimGray;
            this.txtMaterialName.Value = "= Fields.ApprovedByMaterial.FullName + \" (\" + Fields.ApprovedByMaterial.JobTitle + \")\"";

            // --- 4. Checked By (Project Manager) ---
            this.txtOfficeLabel.Name = "txtOfficeLabel";
            this.txtOfficeLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(1.15D));
            this.txtOfficeLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtOfficeLabel.Style.Font.Bold = true;
            this.txtOfficeLabel.Style.Font.Name = "Segoe UI";
            this.txtOfficeLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtOfficeLabel.Value = "Checked By (Project Manager)";

            this.picOfficeSig.Name = "picOfficeSig";
            this.picOfficeSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(1.37D));
            this.picOfficeSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picOfficeSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picOfficeSig.Value = "= Fields.CheckedByOffice.Signature";

            this.txtOfficeName.Name = "txtOfficeName";
            this.txtOfficeName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(0.2D), Telerik.Reporting.Drawing.Unit.Inch(1.85D));
            this.txtOfficeName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtOfficeName.Style.Font.Name = "Segoe UI";
            this.txtOfficeName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtOfficeName.Style.Color = System.Drawing.Color.DimGray;
            this.txtOfficeName.Value = "= Fields.CheckedByOffice.FullName + \" (\" + Fields.CheckedByOffice.JobTitle + \")\"";

            // --- 5. Approved By (HO Operations) ---
            this.txtHraLabel.Name = "txtHraLabel";
            this.txtHraLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(1.15D));
            this.txtHraLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtHraLabel.Style.Font.Bold = true;
            this.txtHraLabel.Style.Font.Name = "Segoe UI";
            this.txtHraLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtHraLabel.Value = "Approved By (HO Operations)";

            this.picHraSig.Name = "picHraSig";
            this.picHraSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(1.37D));
            this.picHraSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picHraSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picHraSig.Value = "= Fields.ApprovedByHRA.Signature";

            this.txtHraName.Name = "txtHraName";
            this.txtHraName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(2.8D), Telerik.Reporting.Drawing.Unit.Inch(1.85D));
            this.txtHraName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtHraName.Style.Font.Name = "Segoe UI";
            this.txtHraName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtHraName.Style.Color = System.Drawing.Color.DimGray;
            this.txtHraName.Value = "= Fields.ApprovedByHRA.FullName + \" (\" + Fields.ApprovedByHRA.JobTitle + \")\"";

            // --- 6. Final Approve (HR Manager) ---
            this.txtAuthLabel.Name = "txtAuthLabel";
            this.txtAuthLabel.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(1.15D));
            this.txtAuthLabel.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.2D));
            this.txtAuthLabel.Style.Font.Bold = true;
            this.txtAuthLabel.Style.Font.Name = "Segoe UI";
            this.txtAuthLabel.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtAuthLabel.Value = "Final Approve (HR Manager)";

            this.picAuthSig.Name = "picAuthSig";
            this.picAuthSig.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(1.37D));
            this.picAuthSig.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.5D), Telerik.Reporting.Drawing.Unit.Inch(0.45D));
            this.picAuthSig.Sizing = Telerik.Reporting.Drawing.ImageSizeMode.ScaleProportional;
            this.picAuthSig.Value = "= Fields.AuthorizedBy.Signature";

            this.txtAuthName.Name = "txtAuthName";
            this.txtAuthName.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(5.4D), Telerik.Reporting.Drawing.Unit.Inch(1.85D));
            this.txtAuthName.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(2.2D), Telerik.Reporting.Drawing.Unit.Inch(0.25D));
            this.txtAuthName.Style.Font.Name = "Segoe UI";
            this.txtAuthName.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8D);
            this.txtAuthName.Style.Color = System.Drawing.Color.DimGray;
            this.txtAuthName.Value = "= Fields.AuthorizedBy.FullName + \" (\" + Fields.AuthorizedBy.JobTitle + \")\"";

            // ---------------------------------------------------------
            // Page Footer Section
            // ---------------------------------------------------------
            this.pageFooterSection1.Height = Telerik.Reporting.Drawing.Unit.Inch(0.35D);
            this.pageFooterSection1.Items.AddRange(new Telerik.Reporting.ReportItemBase[] { this.txtPageNumber });
            this.pageFooterSection1.Name = "pageFooterSection1";

            this.txtPageNumber.Location = new Telerik.Reporting.Drawing.PointU(Telerik.Reporting.Drawing.Unit.Inch(6D), Telerik.Reporting.Drawing.Unit.Inch(0.05D));
            this.txtPageNumber.Name = "txtPageNumber";
            this.txtPageNumber.Size = new Telerik.Reporting.Drawing.SizeU(Telerik.Reporting.Drawing.Unit.Inch(1.80D), Telerik.Reporting.Drawing.Unit.Inch(0.20D));
            this.txtPageNumber.Style.Font.Name = "Segoe UI";
            this.txtPageNumber.Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.5D);
            this.txtPageNumber.Style.TextAlign = Telerik.Reporting.Drawing.HorizontalAlign.Right;
            this.txtPageNumber.Value = "Page: {PageNumber}";

            // ---------------------------------------------------------
            // Report Setup & Parameters
            // ---------------------------------------------------------
            this.DataSource = this.dailyLaborDataSource;
            this.Items.AddRange(new Telerik.Reporting.ReportItemBase[] {
                this.reportHeaderSection1,
                this.detailSection1,
                this.reportFooterSection1,
                this.pageFooterSection1
            });
            this.Name = "DailyLaborPrintOut";
            this.PageSettings.Margins = new Telerik.Reporting.Drawing.MarginsU(
                Telerik.Reporting.Drawing.Unit.Inch(0.5D),
                Telerik.Reporting.Drawing.Unit.Inch(0.5D),
                Telerik.Reporting.Drawing.Unit.Inch(0.5D),
                Telerik.Reporting.Drawing.Unit.Inch(0.5D)
            );
            this.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;

            reportParameter1.AllowNull = true;
            reportParameter1.Name = "id";
            reportParameter1.Type = Telerik.Reporting.ReportParameterType.Integer;

            reportParameter2.AllowNull = true;
            reportParameter2.Name = "FormTitle";

            reportParameter3.AllowNull = true;
            reportParameter3.Name = "FormNumber";

            this.ReportParameters.Add(reportParameter1);
            this.ReportParameters.Add(reportParameter2);
            this.ReportParameters.Add(reportParameter3);
            this.Width = Telerik.Reporting.Drawing.Unit.Inch(7.80D);

            ((System.ComponentModel.ISupportInitialize)(this.headerSubReport1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }
        #endregion

        private Telerik.Reporting.ReportHeaderSection reportHeaderSection1;
        private Telerik.Reporting.DetailSection detailSection1;
        private Telerik.Reporting.ReportFooterSection reportFooterSection1;
        private Telerik.Reporting.PageFooterSection pageFooterSection1;
        private Telerik.Reporting.SubReport subReport1;
        private SharedDesigners.HeaderSubReport headerSubReport1;
        private Telerik.Reporting.ObjectDataSource dailyLaborDataSource;
        private Telerik.Reporting.Table table1;
        private Telerik.Reporting.TextBox thSN, thEmployeeName, thTotalDays, thTotalWage, thIncomeTax, thNetPay;
        private Telerik.Reporting.TextBox tbSN, tbEmployeeName, tbTotalDays, tbTotalWage, tbIncomeTax, tbNetPay;
        private Telerik.Reporting.TextBox txtBranchLabel, txtPeriodLabel;
        private Telerik.Reporting.TextBox txtPreparedLabel, txtPreparedName, txtSiteLabel, txtSiteName, txtMaterialLabel, txtMaterialName;
        private Telerik.Reporting.TextBox txtOfficeLabel, txtOfficeName, txtHraLabel, txtHraName, txtAuthLabel, txtAuthName;
        private Telerik.Reporting.PictureBox picPreparedSig, picSiteSig, picMaterialSig, picOfficeSig, picHraSig, picAuthSig;
        private Telerik.Reporting.TextBox txtPageNumber;
    }
}