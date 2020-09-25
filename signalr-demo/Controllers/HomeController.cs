using signalr_demo.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace signalr_demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult SignalRDemo()
        {
            return View();
        }

        public ActionResult NotificationDemo()
        {
            return View();
        }


        [HttpGet]
        public ActionResult SendMessage(string MessageText)
        {
            MyHub.SendMessageToAll("Testing Messaging", MessageText, "");
            return Json(new { SucessMessage = "Successfully" }, JsonRequestBehavior.AllowGet);
        }

    }
}