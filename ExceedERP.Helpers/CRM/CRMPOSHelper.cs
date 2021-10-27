using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using System;

using System.Linq;
using System.Web;
using System.Xml;

namespace ExceedERP.Helpers.CRM
{



    public class CRMPOSHelper
    {




        public  void GenerateCashSalePrintXml(CashSalesInvoice Invoice, bool IsPartial, int PartialId)
        {
            ExceedDbContext db = new ExceedDbContext();




            if (Invoice != null)
            {
                try
                {
                    #region MyRegion

                    #region generate xml

                    if (IsPartial)
                    {
                        var CashSalesPartialItem = Invoice.CashSalesPartialItem.Where(x => x.CashSalesPartialItemId == PartialId).FirstOrDefault();

                        XmlDocument xmlDoc = new XmlDocument();
                        XmlNode rootNode = xmlDoc.CreateElement("Invoice");
                        xmlDoc.AppendChild(rootNode);

                        XmlNode userNode10 = xmlDoc.CreateElement("Plant_Code");
                        userNode10.InnerText = "";
                        rootNode.AppendChild(userNode10);

                        XmlNode userNode11 = xmlDoc.CreateElement("Plant_Name");
                        userNode11.InnerText = "";
                        rootNode.AppendChild(userNode11);

                        XmlNode userNode120 = xmlDoc.CreateElement("Invoice_Type");
                        userNode120.InnerText = "Invoice";
                        rootNode.AppendChild(userNode120);

                        XmlNode userNode12 = xmlDoc.CreateElement("Reference_Number");
                        userNode12.InnerText = Invoice.CashSalesInvoiceID.ToString();
                        rootNode.AppendChild(userNode12);

                        XmlNode userNode13 = xmlDoc.CreateElement("Reference_Number_1");
                        userNode13.InnerText = Invoice.References;
                        rootNode.AppendChild(userNode13);

                        XmlNode userNode14 = xmlDoc.CreateElement("Reference_Number_2");
                        userNode14.InnerText = "";
                        rootNode.AppendChild(userNode14);

                        XmlNode userNode15 = xmlDoc.CreateElement("Reference_Number_3");
                        userNode15.InnerText = "";
                        rootNode.AppendChild(userNode15);


                        XmlNode userNode2 = xmlDoc.CreateElement("Invoice_Date");
                        userNode2.InnerText = Invoice.Date.ToString("yyyy-MM-dd HH:mm:ss");
                        rootNode.AppendChild(userNode2);

                        XmlNode userNode3 = xmlDoc.CreateElement("Customer_Code");
                        userNode3.InnerText = Invoice.OrganizationCustomerId.ToString();
                        rootNode.AppendChild(userNode3);

                        XmlNode userNode4 = xmlDoc.CreateElement("Customer_Name");
                        userNode4.InnerText = Invoice.OrganizationCustomer.TradeName;
                        rootNode.AppendChild(userNode4);

                        XmlNode userNode41 = xmlDoc.CreateElement("Customer_TIN");
                        userNode41.InnerText = Invoice.OrganizationCustomer.TinNo;
                        rootNode.AppendChild(userNode41);

                        XmlNode userNode5 = xmlDoc.CreateElement("Payment_Type");
                        userNode5.InnerText = "";
                        rootNode.AppendChild(userNode5);

                        XmlNode userNode6 = xmlDoc.CreateElement("Invoice_DiscOrAdd_Amount");
                        userNode6.InnerText = "0.00";
                        rootNode.AppendChild(userNode6);

                        foreach (var Item in CashSalesPartialItem.CashSalesPartialLineItem)
                        {

                            XmlNode userNode7 = xmlDoc.CreateElement("Line_Items");
                            rootNode.AppendChild(userNode7);

                            XmlNode userNode70 = xmlDoc.CreateElement("Item_ID");
                            if (Item != null) userNode70.InnerText = Item.LineItem.Product.ItemCode;
                            userNode7.AppendChild(userNode70);

                            XmlNode userNode701 = xmlDoc.CreateElement("Item_Description");
                            if (Item != null) userNode701.InnerText = Item.LineItem.Product.Name;
                            userNode7.AppendChild(userNode701);

                            XmlNode userNode71 = xmlDoc.CreateElement("Item_Quantity");
                            userNode71.InnerText = Item.PartialPayedAmount.ToString();
                            userNode7.AppendChild(userNode71);

                            XmlNode userNode72 = xmlDoc.CreateElement("Item_UOM");
                            userNode72.InnerText = Item.LineItem.Product.UOM;
                            userNode7.AppendChild(userNode72);

                            XmlNode userNode73 = xmlDoc.CreateElement("Item_Unit_Price");
                            userNode73.InnerText = Item.LineItem.UnitAmount.ToString();
                            userNode7.AppendChild(userNode73);

                            var percent = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID.ToString() == Item.LineItem.Tax);

                            if (percent != null)
                            {
                                XmlNode userNode74 = xmlDoc.CreateElement("Item_Tax_Percent");
                                userNode74.InnerText = percent.Rate.ToString(); ;
                                userNode7.AppendChild(userNode74);
                            }

                            XmlNode userNode741 = xmlDoc.CreateElement("Item_DiscOrAdd_Amount");
                            userNode741.InnerText = "";
                            userNode7.AppendChild(userNode741);

                        }

                        // Save the document to a file and auto-indent the output.
                        string subPath = "~/App_Data/CashSalesInvoice/Export";
                        string physicalPath = HttpContext.Current.Server.MapPath(subPath + "/" + Invoice.CashSalesInvoiceID.ToString() + ".xml");
                        using (XmlTextWriter writer = new XmlTextWriter(physicalPath, null))
                        {
                            writer.Formatting = Formatting.Indented;
                            xmlDoc.Save(writer);
                        }


                          CashSalesPartialItem = db.CashSalesPartialItems.Where(x => x.CashSalesPartialItemId == PartialId).FirstOrDefault();

                        CashSalesPartialItem.IsReceiptPrinted = true;
                        db.Entry(CashSalesPartialItem).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }


                    if (!IsPartial)
                    {
                        var LineItems = db.LineItems.Where(x => x.Voucher == Invoice.CashSalesInvoiceID.ToString()).ToList();

                        XmlDocument xmlDoc = new XmlDocument();
                        XmlNode rootNode = xmlDoc.CreateElement("Invoice");
                        xmlDoc.AppendChild(rootNode);

                        XmlNode userNode10 = xmlDoc.CreateElement("Plant_Code");
                        userNode10.InnerText = "";
                        rootNode.AppendChild(userNode10);

                        XmlNode userNode11 = xmlDoc.CreateElement("Plant_Name");
                        userNode11.InnerText = "";
                        rootNode.AppendChild(userNode11);

                        XmlNode userNode120 = xmlDoc.CreateElement("Invoice_Type");
                        userNode120.InnerText = "Invoice";
                        rootNode.AppendChild(userNode120);

                        XmlNode userNode12 = xmlDoc.CreateElement("Reference_Number");
                        userNode12.InnerText = Invoice.CashSalesInvoiceID.ToString();
                        rootNode.AppendChild(userNode12);

                        XmlNode userNode13 = xmlDoc.CreateElement("Reference_Number_1");
                        userNode13.InnerText = Invoice.References;
                        rootNode.AppendChild(userNode13);

                        XmlNode userNode14 = xmlDoc.CreateElement("Reference_Number_2");
                        userNode14.InnerText = "";
                        rootNode.AppendChild(userNode14);

                        XmlNode userNode15 = xmlDoc.CreateElement("Reference_Number_3");
                        userNode15.InnerText = "";
                        rootNode.AppendChild(userNode15);


                        XmlNode userNode2 = xmlDoc.CreateElement("Invoice_Date");
                        userNode2.InnerText = Invoice.Date.ToString("yyyy-MM-dd HH:mm:ss");
                        rootNode.AppendChild(userNode2);

                        XmlNode userNode3 = xmlDoc.CreateElement("Customer_Code");
                        userNode3.InnerText = Invoice.OrganizationCustomerId.ToString();
                        rootNode.AppendChild(userNode3);

                        XmlNode userNode4 = xmlDoc.CreateElement("Customer_Name");
                        userNode4.InnerText = Invoice.OrganizationCustomer.TradeName;
                        rootNode.AppendChild(userNode4);

                        XmlNode userNode41 = xmlDoc.CreateElement("Customer_TIN");
                        userNode41.InnerText = Invoice.OrganizationCustomer.TinNo;
                        rootNode.AppendChild(userNode41);

                        XmlNode userNode5 = xmlDoc.CreateElement("Payment_Type");
                        userNode5.InnerText = "";
                        rootNode.AppendChild(userNode5);

                        XmlNode userNode6 = xmlDoc.CreateElement("Invoice_DiscOrAdd_Amount");
                        userNode6.InnerText = "0.00";
                        rootNode.AppendChild(userNode6);

                        foreach (var Item in LineItems)
                        {

                            XmlNode userNode7 = xmlDoc.CreateElement("Line_Items");
                            rootNode.AppendChild(userNode7);

                            XmlNode userNode70 = xmlDoc.CreateElement("Item_ID");
                            if (Item != null) userNode70.InnerText = Item.Product.ItemCode;
                            userNode7.AppendChild(userNode70);

                            XmlNode userNode701 = xmlDoc.CreateElement("Item_Description");
                            if (Item != null) userNode701.InnerText = Item.Product.Name;
                            userNode7.AppendChild(userNode701);

                            XmlNode userNode71 = xmlDoc.CreateElement("Item_Quantity");
                            userNode71.InnerText = Item.Quantity.ToString();
                            userNode7.AppendChild(userNode71);

                            XmlNode userNode72 = xmlDoc.CreateElement("Item_UOM");
                            userNode72.InnerText = Item.Product.UOM;
                            userNode7.AppendChild(userNode72);

                            XmlNode userNode73 = xmlDoc.CreateElement("Item_Unit_Price");
                            userNode73.InnerText = Item.UnitAmount.ToString();
                            userNode7.AppendChild(userNode73);

                            var percent = db.GLTaxRates.Where(x => x.GLTaxRateID.ToString() == Item.Tax).FirstOrDefault();

                            if (percent != null)
                            {
                                XmlNode userNode74 = xmlDoc.CreateElement("Item_Tax_Percent");
                                userNode74.InnerText = percent.Rate.ToString();
                                userNode7.AppendChild(userNode74);
                            }

                            XmlNode userNode741 = xmlDoc.CreateElement("Item_DiscOrAdd_Amount");
                            userNode741.InnerText = "";
                            userNode7.AppendChild(userNode741);

                        }

                        // Save the document to a file and auto-indent the output.
                        string subPath = "~/App_Data/CashSalesInvoice/Export";
                        string physicalPath = HttpContext.Current.Server.MapPath(subPath + "/" + Invoice.CashSalesInvoiceID.ToString() + ".xml");
                        using (XmlTextWriter writer = new XmlTextWriter(physicalPath, null))
                        {
                            writer.Formatting = Formatting.Indented;
                            xmlDoc.Save(writer);
                        }


                        var CashSaleInvoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID == Invoice.CashSalesInvoiceID).FirstOrDefault();
                        CashSaleInvoice.IsReceiptPrinted = true;
                        db.Entry(CashSaleInvoice).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    #endregion
                    #endregion
                }
                catch (Exception e)
                {

                }

            }


        }



    }

}
