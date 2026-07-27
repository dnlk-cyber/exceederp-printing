using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Xml;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{

	public class APTBPaymentController : Controller
	{
		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.DisbursementTBPayment)]

		public ActionResult Index()
		{
			PopulateViewData();
			return View();
		}

		private void PopulateViewData()
		{
			var paymentMethod = _db.PaymentMethods.ToList();

			ViewData["methods"] = paymentMethod.Select(e => new PaymentMethod
			{
				Method = e.Method
			}).OrderBy(m => m.Method);

			var suppliers = _db.ProcurementSuppliers.ToList();

			ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
			{
				Name = c.Name,
				SupplierId = c.SupplierId
			});

			var invoices =
				_db.APPaybleInvoices.Where(x => x.IsOnlineApproved).Where(x => x.InvoiceStatus == InvoiceStatus.Open).ToList();

			ViewData["invoices"] = invoices
				.Select(e => new ApPaybleInvoices
				{
					InvoiceNumber = e.InvoiceNumber
				})
				.OrderBy(e => e.InvoiceNumber);

			//procu
			var items = _db.PurchaseOrderItems;

			var selecteditems = items.Select(
				i => new
				{
					Name = i.ItemName,
					ItmCode = i.ItmCode
				}).AsQueryable();

			ViewData["Items"] = selecteditems;

			var selectedcatedories = items.Select(
				i => new
				{
					Name = i.CategoryName,
					CatCode = i.CatCode
				}).AsQueryable();
			ViewData["Category"] = selectedcatedories;
			var tax = _db.GLTaxRates;
			ViewData["tax"] = tax;

			var employee = _db.Person_Employee;
			var selectedEmployee = employee.Select(
				i => new
				{
					Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
					Code = i.EmployeeId
				}).AsQueryable();
			ViewData["personEmployees"] = selectedEmployee;
		}
 

		[AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.DisbursementTBPayment)]
		public ActionResult APTBPayments_Read([DataSourceRequest]DataSourceRequest request)
		{
			List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
			IQueryable<ApTPayment> ApTPayments = _db.ApTPayments.Where(x => mybraches.Contains(x.OrgBranchName));

		 
			DataSourceResult result = ApTPayments.ToDataSourceResult(request, ApTPayment => new
			{
				ApTPaymentId = ApTPayment.ApTPaymentId,
				PaymentNum = ApTPayment.PaymentNum,
				SupplierName = ApTPayment.SupplierName,
				SupplierId = ApTPayment.SupplierId,
				PaymentType = ApTPayment.PaymentType,
				PaymentDate = ApTPayment.PaymentDate,
				TCode = ApTPayment.TCode,
				TAccount = ApTPayment.TAccount,
				Amount = ApTPayment.Amount,

				Remark = ApTPayment.Remark,
				IsSendForApproval = ApTPayment.IsSendForApproval,
				IsOnlineTransferred = ApTPayment.IsOnlineTransferred,
				IsOnlineTransferredBy = ApTPayment.IsOnlineTransferredBy,
				OnlineTransferredTime = ApTPayment.OnlineTransferredTime,
				Status = ApTPayment.Status,
				IsOnlineApproved = ApTPayment.IsOnlineApproved,
				OnlineApprovedBy = ApTPayment.OnlineApprovedBy,
				OnlineApprovedTime = ApTPayment.OnlineApprovedTime,
				IsVoid = ApTPayment.IsVoid,
				VoidBy = ApTPayment.VoidBy,
				VoidTime = ApTPayment.VoidTime,
				OrgBranchName = ApTPayment.OrgBranchName,
				DateCreated = ApTPayment.DateCreated,
				LastModified = ApTPayment.LastModified,
				CreatedBy = ApTPayment.CreatedBy,
				ModifiedBy = ApTPayment.ModifiedBy
			});

			return Json(result);
		}

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APTBPayments_Create([DataSourceRequest]DataSourceRequest request, ApTPayment ApTPayment)
		{
			


			#region validation       
					

			if (!String.IsNullOrEmpty(ApTPayment.PaymentReference))
			{
				var exists = _db.ApTPayments.Where(x => x.PaymentReference == ApTPayment.PaymentReference && x.IsVoid == false).ToList();

				if (exists.Any())
					ModelState.AddModelError("Payment Reference", $"{ApTPayment.PaymentReference}  already exists in the system.");
			}
			#endregion
		   

			if (ModelState.IsValid)
			{
				var entity = new ApTPayment
				{
					PaymentNum = ApTPayment.PaymentNum,
					SupplierName = ApTPayment.SupplierName,
					SupplierId = ApTPayment.SupplierId,
					PaymentType = ApTPayment.PaymentType,
					PaymentDate = ApTPayment.PaymentDate,
					TCode = ApTPayment.TCode,
					TAccount = ApTPayment.TAccount,
					Amount = ApTPayment.Amount,

					Remark = ApTPayment.Remark,
					IsSendForApproval = ApTPayment.IsSendForApproval,
					IsOnlineTransferred = ApTPayment.IsOnlineTransferred,
					IsOnlineTransferredBy = ApTPayment.IsOnlineTransferredBy,
					OnlineTransferredTime = ApTPayment.OnlineTransferredTime,
					Status = ApTPayment.Status,
					IsOnlineApproved = ApTPayment.IsOnlineApproved,
					OnlineApprovedBy = ApTPayment.OnlineApprovedBy,
					OnlineApprovedTime = ApTPayment.OnlineApprovedTime,
					IsVoid = ApTPayment.IsVoid,
					VoidBy = ApTPayment.VoidBy,
					VoidTime = ApTPayment.VoidTime,
					OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
					DateCreated = DateTime.Now,
					LastModified = ApTPayment.LastModified,
					CreatedBy = User.Identity.Name,
					ModifiedBy = ApTPayment.ModifiedBy
				};

				_db.ApTPayments.Add(entity);
				_db.SaveChanges();
				ApTPayment.ApTPaymentId = entity.ApTPaymentId;

				

				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "AP Telebirr payment", "Success", User.Identity.Name, $"created Telebirr payment {entity.ApTPaymentId} ", Modules.Finance);
				#endregion
			}
			return Json(new[] { ApTPayment }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APTBPayments_Update([DataSourceRequest]DataSourceRequest request, ApTPayment ApTPayment)
		{
			

			#region validation

			
	   

			if (!String.IsNullOrEmpty(ApTPayment.PaymentReference))
			{
				var exists = _db.ApTPayments
					.Where(x=>x.ApTPaymentId!=ApTPayment.ApTPaymentId).Where(x => x.PaymentReference == ApTPayment.PaymentReference && x.IsVoid == false).ToList();

				if (exists.Any())
					ModelState.AddModelError("Payment Reference", $"{ApTPayment.PaymentReference}  already exists in the system.");
			}
			#endregion
		   

			#region validated record

			var parent = _db.ApTPayments.Find(ApTPayment.ApTPaymentId);
			if (parent != null)
			{
				if (parent.IsOnlineApproved)
					ModelState.AddModelError("approved", "You cannot manipulate this record");

				#region detach local version
				var local = _db.Set<ApTPayment>()
					.Local
					.FirstOrDefault(f => f.ApTPaymentId == parent.ApTPaymentId);
				if (local != null)
				{
					_db.Entry(local).State = EntityState.Detached;
				}
				#endregion

			}


			#endregion


			if (ModelState.IsValid)
			{
					var entity = new ApTPayment
					{
						ApTPaymentId = ApTPayment.ApTPaymentId,
						PaymentNum = ApTPayment.PaymentNum,
						SupplierName = ApTPayment.SupplierName,
						SupplierId = ApTPayment.SupplierId,
						PaymentType = ApTPayment.PaymentType,
						PaymentDate = ApTPayment.PaymentDate,
						TCode = ApTPayment.TCode,
						TAccount = ApTPayment.TAccount,
						Amount = ApTPayment.Amount,
						Remark = ApTPayment.Remark,
						IsSendForApproval = ApTPayment.IsSendForApproval,
						IsOnlineTransferred = ApTPayment.IsOnlineTransferred,
						IsOnlineTransferredBy = ApTPayment.IsOnlineTransferredBy,
						OnlineTransferredTime = ApTPayment.OnlineTransferredTime,
						Status = ApTPayment.Status,
						IsOnlineApproved = ApTPayment.IsOnlineApproved,
						OnlineApprovedBy = ApTPayment.OnlineApprovedBy,
						OnlineApprovedTime = ApTPayment.OnlineApprovedTime,
						IsVoid = ApTPayment.IsVoid,
						VoidBy = ApTPayment.VoidBy,
						VoidTime = ApTPayment.VoidTime,
						OrgBranchName = ApTPayment.OrgBranchName,
						DateCreated = ApTPayment.DateCreated,
						LastModified = DateTime.Now,
						CreatedBy = ApTPayment.CreatedBy,
						ModifiedBy = User.Identity.Name
					};

					_db.ApTPayments.Attach(entity);
					_db.Entry(entity).State = EntityState.Modified;
					_db.SaveChanges();


			
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "AP payment", "Success", User.Identity.Name, $"updated payment {entity.ApTPaymentId} ", Modules.Finance);
				#endregion

			}
			return Json(new[] { ApTPayment }.ToDataSourceResult(request, ModelState));
		}

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult APTBPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApTPayment ApTPayment)
		{
			bool isvalid = true;
			if (ModelState.IsValid)
			{
				#region validated record

				var parent = _db.ApTPayments.Find(ApTPayment.ApTPaymentId);
				if (parent != null)
				{
					if (parent.IsOnlineApproved)
					{
						isvalid = false;
						//ViewBag.Note = "You cannot manipulate this record";
						//return View("Error");
					}

					#region detach local version
					var local = _db.Set<ApTPayment>()
						.Local
						.FirstOrDefault(f => f.ApTPaymentId == parent.ApTPaymentId);
					if (local != null)
					{
						_db.Entry(local).State = EntityState.Detached;
					}
					#endregion

				}


				#endregion

				if (isvalid)
				{
					var entity = new ApTPayment
					{
						ApTPaymentId = ApTPayment.ApTPaymentId,
						PaymentNum = ApTPayment.PaymentNum,
						SupplierName = ApTPayment.SupplierName,
						SupplierId = ApTPayment.SupplierId,
						PaymentType = ApTPayment.PaymentType,
						PaymentDate = ApTPayment.PaymentDate,
						TCode = ApTPayment.TCode,
						TAccount = ApTPayment.TAccount,
						Amount = ApTPayment.Amount,

						Remark = ApTPayment.Remark,
						IsSendForApproval = ApTPayment.IsSendForApproval,
						IsOnlineTransferred = ApTPayment.IsOnlineTransferred,
						IsOnlineTransferredBy = ApTPayment.IsOnlineTransferredBy,
						OnlineTransferredTime = ApTPayment.OnlineTransferredTime,
						Status = ApTPayment.Status,
						IsOnlineApproved = ApTPayment.IsOnlineApproved,
						OnlineApprovedBy = ApTPayment.OnlineApprovedBy,
						OnlineApprovedTime = ApTPayment.OnlineApprovedTime,
						IsVoid = ApTPayment.IsVoid,
						VoidBy = ApTPayment.VoidBy,
						VoidTime = ApTPayment.VoidTime,
						OrgBranchName = ApTPayment.OrgBranchName,
						DateCreated = ApTPayment.DateCreated,
						LastModified = ApTPayment.LastModified,
						CreatedBy = ApTPayment.CreatedBy,
						ModifiedBy = ApTPayment.ModifiedBy
					};

					_db.ApTPayments.Attach(entity);
					_db.ApTPayments.Remove(entity);
					_db.SaveChanges();


				
					#region delete log
					UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment " + ApTPayment.ApTPaymentId + " " + ApTPayment.PaymentReference, Modules.Finance);
					#endregion
				}
			}
			return Json(new[] { ApTPayment }.ToDataSourceResult(request, ModelState));
		}

	  

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		public JsonResult SendForApproval(int id)
		{
			var record = _db.ApTPayments.Find(id);        

			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			
			if (record.IsSendForApproval)
			{
				var formattedData = new
				{
					Message = "The record is already sent for approval you cannot send again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is already approved.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (!record.IsSendForApproval && !record.IsOnlineApproved)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 4);
				record.IsSendForApproval = true;
				record.SendForApprovalBy = User.Identity.Name;
				record.SendForApprovalTime = DateTime.Now;
				_db.ApTPayments.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();
				var formattedData = new
				{
					Message = "Successfully Sent.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			return null;
		}

	  


		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		public JsonResult Void(int id)
		{
			var record = _db.ApTPayments.Find(id);
			if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
			{
				record.Status = Enum.GetName(typeof(OperationStatuses), 10);
				record.IsOnlineApproved = false;
				record.IsSendForApproval = false;
				record.VoidBy = User.Identity.Name;
				record.VoidTime = DateTime.Now;
				record.IsVoid = false;
				record.VoidBy = User.Identity.Name;
				record.VoidTime = DateTime.Now;
				_db.ApTPayments.Attach(record);
				_db.Entry(record).State = EntityState.Modified;
				_db.SaveChanges();


				//copy
				//CopyRecord(id);
				#region operation log
				UserHelper.OperationLog(Request.UserHostAddress, "payment", "Success", User.Identity.Name, $"void payment {id} ", Modules.Finance);
				#endregion

				var formattedData = new
				{
					Message = "Void Successful.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

		   
			if (record.IsVoid)
			{
				var formattedData = new
				{
					Message = "The record is already void.You can not void again.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (record.IsOnlineTransferred)
			{
				var formattedData = new
				{
					Message = "The record is transferred to GL.You can not void .",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			if (!record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved hence you can modify by editing.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}


			return null;
		}

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		public JsonResult TransferTB(int id)
		{
			var record = _db.ApTPayments.Find(id);

			if (record == null)
			{
				var formattedData = new
				{
					Message = "The record is not found.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsVoid)
			{
				var formattedData = new
				{
					Message = "The record is void.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (record.IsOnlineTransferred)
			{
				var formattedData = new
				{
					Message = "The record is transferred already.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			if (!record.IsOnlineApproved)
			{
				var formattedData = new
				{
					Message = "The record is not approved.",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}

			try
			{
				var url = "http://196.188.120.3:10001/payment/services/SYNCAPIRequestMgrService";

				var httpRequest = (HttpWebRequest)WebRequest.Create(url);
				httpRequest.Method = "POST";

				httpRequest.ContentType = "text+xml";


				var data = @"<soapenv:Envelope
				xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
				xmlns:api=""http://cps.huawei.com/synccpsinterface/api_requestmgr""
				xmlns:req=""http://cps.huawei.com/synccpsinterface/request""
				xmlns:com=""http://cps.huawei.com/synccpsinterface/common"">
				<soapenv:Header/>
				<soapenv:Body>
					<api:Request>
						<req:Header>
							<req:Version>1.0</req:Version>
							<req:CommandID>InitTrans_Business to Business Payment</req:CommandID>
							<req:OriginatorConversationID>49ea5e94efc40a7a652f8b97db931e6</req:OriginatorConversationID>
							<req:Caller>
								<req:CallerType>2</req:CallerType>
								<req:ThirdPartyID>ECWC</req:ThirdPartyID>
								<req:Password>Ko+d/8EL6d25gzNtmnb2qf9yUEHgUbdO5qdt2T/cFdk=</req:Password>
								<req:ResultURL>http://10.180.73.190:8003/mockAPIResultMgrBinding</req:ResultURL>
							</req:Caller>
							<req:KeyOwner>1</req:KeyOwner>
							<req:Timestamp>20190625202317</req:Timestamp>
						</req:Header>
						<req:Body>
							<req:Identity>
								<req:Initiator>
									<req:IdentifierType>12</req:IdentifierType>
									<req:Identifier>334401</req:Identifier>
									<req:SecurityCredential>gnasRuxpwj1P542A84K41wtVN059dmqhQUH4Cl49in8=</req:SecurityCredential>
									<req:ShortCode>3344</req:ShortCode>
								</req:Initiator>
								<req:PrimaryParty>
									<req:IdentifierType>4</req:IdentifierType>
									<req:Identifier>3344</req:Identifier>
								</req:PrimaryParty>
								<req:ReceiverParty>
									<req:IdentifierType>4</req:IdentifierType>
									<req:Identifier>" + record.TCode + @"</req:Identifier>
								</req:ReceiverParty>
							</req:Identity>
							<req:TransactionRequest>
								<req:Parameters>
									<req:Amount>" + record.Amount + @"</req:Amount>
									<req:Currency>ETB</req:Currency>
								</req:Parameters>
							</req:TransactionRequest>
							<req:Remark>Test Remark </req:Remark>
						</req:Body>
					</api:Request>
				</soapenv:Body>
			</soapenv:Envelope>";


				using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
				{
					streamWriter.Write(data);
				}

				var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
				using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
				{
					var result = streamReader.ReadToEnd();
					Console.WriteLine(result);
				}

				Console.WriteLine(httpResponse.StatusCode);
				//handle error conditions like balance
				//only successful should be alerted successful
				var formattedData = new
				{
					Message = "Transfer is successful.",
					Status = "OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
			catch(Exception ex)
			{
				var formattedData = new
				{
					Message = $"Error occurred.\n{ex.Message}",
					Status = "NOT_OK"
				};
				return Json(formattedData, JsonRequestBehavior.AllowGet);
			}
		}
		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		public  JsonResult GetTBName(int id)
		{
			var record = _db.ApTPayments.Find(id);

			string name = "";
			var url = "http://196.188.120.3:10001/payment/services/SYNCAPIRequestMgrService";

			var httpRequest = (HttpWebRequest)WebRequest.Create(url);
			httpRequest.Method = "POST";
			httpRequest.ContentType = "text+xml";

			var data = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:api=""http://cps.huawei.com/synccpsinterface/api_requestmgr"" xmlns:req=""http://cps.huawei.com/synccpsinterface/request"" xmlns:com=""http://cps.huawei.com/synccpsinterface/common"">
		   <soapenv:Header/>
		   <soapenv:Body>
			  <api:Request>
				 <req:Header>
					<req:Version>1.0</req:Version>
					<req:CommandID>QueryOrganizationInfo</req:CommandID>
				   <req:Caller>
						 <req:CallerType>2</req:CallerType>
					   <req:ThirdPartyID>EK_Broker</req:ThirdPartyID>
					   <req:Password>x5nB9aECb9cZo3ke2GEeQNmumm4yB8MGXwHHy0SuVjU=</req:Password>
					</req:Caller>
					<req:KeyOwner>1</req:KeyOwner>
					<req:Timestamp>20150101010101</req:Timestamp>
				 </req:Header>
				 <req:Body>
					<req:Identity>
					   <req:Initiator>
						  <req:IdentifierType>14</req:IdentifierType>
						  <req:Identifier>Bank</req:Identifier>
						  <req:SecurityCredential>nha3P0PxnYAq5Low5N1Lb2nnWT00/TVVM2HdaJCQJE8=</req:SecurityCredential>
					   </req:Initiator>
					   <req:ReceiverParty>
						  <req:IdentifierType>4</req:IdentifierType>
						  <req:Identifier>3344</req:Identifier>
					   </req:ReceiverParty>
					</req:Identity>
					<req:QueryOrganizationInfoRequest/>
					<req:Remark>test</req:Remark>
				 </req:Body>
			  </api:Request>
		   </soapenv:Body>
		</soapenv:Envelope>";


			using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
			{
				streamWriter.Write(data);
			}

			var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				var result = streamReader.ReadToEnd();
				Console.WriteLine(result);

				XmlDocument nameXDoc = new XmlDocument();
				nameXDoc.LoadXml(result);

				XmlNodeList resultType = nameXDoc.GetElementsByTagName("res:ResultType");
				Console.WriteLine(resultType[0].InnerText);

				XmlNodeList resultCode = nameXDoc.GetElementsByTagName("res:ResultCode");
				Console.WriteLine(resultCode[0].InnerText);

				XmlNodeList resultDesc = nameXDoc.GetElementsByTagName("res:ResultDesc");
				Console.WriteLine(resultDesc[0].InnerText);

				XmlNodeList bNodeList = nameXDoc.GetElementsByTagName("res:OrganizationBasicData");
				foreach (XmlNode xmlNode in bNodeList)
				{
					Console.WriteLine(xmlNode.FirstChild["res:AccountNo"].InnerText);
					Console.WriteLine(xmlNode.FirstChild["res:AvailableBalance"].InnerText);
				}
			}

			Console.WriteLine(httpResponse.StatusCode);

			var formattedData = new
			{
				Name = name
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);
		}

		[AuthorizeRoles(FinanceConstants.DisbursementTBPayment)]
		public JsonResult CheckB(int id)
		{
			decimal bal = 0;
			var url = "http://196.188.120.3:10001/payment/services/SYNCAPIRequestMgrService";

			var httpRequest = (HttpWebRequest)WebRequest.Create(url);
			httpRequest.Method = "POST";
			httpRequest.ContentType = "text+xml";

			var data = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:com=""http://cps.huawei.com/synccpsinterface/common"" xmlns:cus=""http://cps.huawei.com/cpsinterface/customizedrequest"" xmlns:api=""http://cps.huawei.com/synccpsinterface/api_requestmgr"" xmlns:req=""http://cps.huawei.com/synccpsinterface/request"">
			   <soapenv:Header/>
			   <soapenv:Body>
				  <api:Request>
					 <req:Header>
						<req:Version>1.0</req:Version>
						<req:CommandID>QueryOrganizationBalance</req:CommandID>
						<req:OriginatorConversationID>cbd84e0cbc794767be632f251395b681</req:OriginatorConversationID>
						<req:Caller>
						   <req:CallerType>2</req:CallerType>
						 <req:ThirdPartyID>ECWC</req:ThirdPartyID>
						 <req:Password>Ko+d/8EL6d25gzNtmnb2qf9yUEHgUbdO5qdt2T/cFdk=</req:Password>
						</req:Caller>
						<req:KeyOwner>1</req:KeyOwner>
						<req:Timestamp>"+DateTime.Now.ToLongDateString()+@"</req:Timestamp>
					 </req:Header>
					 <req:Body>
						<req:Identity>
							<req:Initiator>
								<req:IdentifierType>12</req:IdentifierType>
								<req:Identifier>334401</req:Identifier>
								<req:SecurityCredential>gnasRuxpwj1P542A84K41wtVN059dmqhQUH4Cl49in8=</req:SecurityCredential>
								<req:ShortCode>3344</req:ShortCode>
							</req:Initiator>
						   <req:ReceiverParty>
							  <req:IdentifierType>4</req:IdentifierType>
							  <req:Identifier>3344</req:Identifier>
						   </req:ReceiverParty>
						</req:Identity>
						<req:QueryOrganizationBalanceRequest>
						   <req:AccountType>Organization E-Money Account</req:AccountType>
						</req:QueryOrganizationBalanceRequest>
					 </req:Body>
				  </api:Request>
			   </soapenv:Body>
			</soapenv:Envelope>";


			using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
			{
				streamWriter.Write(data);
			}

			var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				var result = streamReader.ReadToEnd();
				Console.WriteLine(result);

				XmlDocument balanceXDoc = new XmlDocument();
				balanceXDoc.LoadXml(result);

				XmlNodeList resultType = balanceXDoc.GetElementsByTagName("res:ResultType");
				Console.WriteLine(resultType[0].InnerText);

				XmlNodeList resultCode = balanceXDoc.GetElementsByTagName("res:ResultCode");
				Console.WriteLine(resultCode[0].InnerText);

				XmlNodeList resultDesc = balanceXDoc.GetElementsByTagName("res:ResultDesc");
				Console.WriteLine(resultDesc[0].InnerText);

				XmlNodeList bNodeList = balanceXDoc.GetElementsByTagName("res:AccountBalanceData");
				foreach (XmlNode xmlNode in bNodeList)
				{
					Console.WriteLine(xmlNode.FirstChild["res:AccountNo"].InnerText);
					Console.WriteLine(xmlNode.FirstChild["res:AvailableBalance"].InnerText);
				}
			}

			Console.WriteLine(httpResponse.StatusCode);

			var formattedData = new
			{
				Balance =bal
			};
			return Json(formattedData, JsonRequestBehavior.AllowGet);


		}

		protected override void Dispose(bool disposing)
		{
			_db.Dispose();
			base.Dispose(disposing);
		}
	}
}
