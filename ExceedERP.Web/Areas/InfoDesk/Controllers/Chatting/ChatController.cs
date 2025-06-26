using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Web.Controllers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using System;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.Chatting
{
    [Authorize]
   
    public class ChatController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
        public ActionResult Index()
        {
            return View();
        }

        private string GetUserName(string userId)
        {
            var user = udb.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return "User(Unknown):"; // Return meaningful default value
            }

            var employee = db.Person_Employee.FirstOrDefault(e => e.EmployeeId == user.EmployeeId);
            if (employee != null)
            {
                return $"User({user.UserName} - {employee.FullNameaEnglish}):";
            }

            return $"User({user.UserName}):";
        }
        public ActionResult Chat_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ChatSession> chats = db.ChatSessions;
            DataSourceResult result = chats.ToDataSourceResult(request, chat => new {
                ChatSessionId = chat.ChatSessionId,
                User1Id = chat.User1Id,
                User2Id = chat.User2Id,
            });

            return Json(result);
        }

        [HttpGet]
        public JsonResult GetChatHistory(string user1Id,string user2Id)
        {
            var user = udb.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user.Id != user1Id)
            {
                user1Id = user.Id;
            }
            var messages = db.Messages.AsNoTracking().ToList()
                     .Where(m => (m.SenderId == user1Id && m.ReceiverId == user2Id) ||
                        (m.SenderId == user2Id && m.ReceiverId == user1Id))
                    .OrderBy(m => m.Timestamp)
                    .Select(m => new
                    {
                        Sender = GetUserName( m.SenderId),
                        Receiver = GetUserName(m.ReceiverId),
                        SenderId = m.SenderId,
                        ReceiverId = m.ReceiverId,
                        Content = m.Content,
                        Timestamp = m.Timestamp.ToString("HH:mm")
                    })
                    .ToList();

                return Json(messages, JsonRequestBehavior.AllowGet);
            
        }
        // Insert a new message
        [HttpPost]
        public JsonResult InsertMessage(string senderId, string receiverId, string content)
        {
            var user = udb.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if(user.Id != senderId)
            {
                senderId = user.Id;
            }
            var session = db.ChatSessions.FirstOrDefault(s =>
                (s.User1Id == senderId && s.User2Id == receiverId) ||
                (s.User1Id == receiverId && s.User2Id == senderId));

            if (session == null)
            {
                session = new ChatSession { User1Id = senderId, User2Id = receiverId };
                db.ChatSessions.Add(session);
                db.SaveChanges();
            }

            var message = new Message
            {
                ChatSessionId = session.ChatSessionId,
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                Timestamp = DateTime.UtcNow
            };

            db.Messages.Add(message);
            db.SaveChanges();
            NotifModels notif = new NotifModels
            {
                //UserId = UserInformationHelper.GetEmployeeUserId(assignTicket.AssinedTo),
                UserId = receiverId,
                Title = $"  : {User.Identity.Name} Sent Message ",
                Message = $"Message: {content}",
                URL = "/InfoDesk/Chat"
            };
            Notify.NotifyUser(notif);
            return Json(new { success = true, message = "Message sent!", messageId = message.MessageId }, JsonRequestBehavior.AllowGet);
            
        }

        // Update an existing message
        [HttpPost]
        public JsonResult UpdateMessage(int messageId, string newContent)
        {
          
            var message = db.Messages.FirstOrDefault(m => m.MessageId == messageId);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found!" }, JsonRequestBehavior.AllowGet);
            }

            message.Content = newContent;
            db.Entry(message).State = EntityState.Modified;
            db.SaveChanges();
            NotifModels notif = new NotifModels
            {
                //UserId = UserInformationHelper.GetEmployeeUserId(assignTicket.AssinedTo),
                UserId = message.ReceiverId,
                Title = $"  : {User.Identity.Name} sent Message ",
                Message = $"Message: {newContent}",
                URL = "/InfoDesk/Chat"
            };
            Notify.NotifyUser(notif);

            return Json(new { success = true, message = "Message updated!" }, JsonRequestBehavior.AllowGet);
           
        }

        // Delete a message
        [HttpPost]
        public JsonResult DeleteMessage(int messageId)
        {
            
            var message = db.Messages.FirstOrDefault(m => m.MessageId == messageId);
            if (message == null)
            {
                return Json(new { success = false, message = "Message not found!" }, JsonRequestBehavior.AllowGet);
            }

            db.Messages.Remove(message);
            db.SaveChanges();
            NotifModels notif = new NotifModels
            {
                //UserId = UserInformationHelper.GetEmployeeUserId(assignTicket.AssinedTo),
                UserId = message.ReceiverId,
                Title = $" User : {User.Identity.Name} has  deleted Message {message.Content} ",
                Message = $"Message: {message.Content}",
                URL = "/InfoDesk/Chat"
            };
            Notify.NotifyUser(notif);
            return Json(new { success = true, message = "Message deleted!" }, JsonRequestBehavior.AllowGet);
            
        }
        public JsonResult GetUsers()
        {
            var users = udb.Users
                          // .Where(x => x.IsEnabled) // Optional: Only get active users
                           .Select(x => new
                           {
                               UserId = x.Id,
                               Name = x.UserName
                           })
                           .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMyUserId()
        {
             
            var users = udb.Users
                           .Where(x => x.UserName==User.Identity.Name) // Optional: Only get active users
                           .Select(x => new
                           {
                               UserId = x.Id,
                               Name = x.UserName
                           })
                           .ToList();

            return Json(users, JsonRequestBehavior.AllowGet);
        }

    }

}
