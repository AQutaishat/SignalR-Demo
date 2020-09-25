using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace signalr_demo.SignalR
{
    public class MyHub : Hub
    {

        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();


        //public static readonly System.Timers.Timer _Timer = new System.Timers.Timer();
        //static MyHub()
        //{
        //    _Timer.Interval = 2000;
        //    _Timer.Elapsed += TimerElapsed;
        //    _Timer.Start();
        //}
        //static void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
        //    hub.Clients.All.logMessage(string.Format("{0} - Still running", DateTime.UtcNow));
        //}

        public static void SendMessageToAll(string title, string Message, string Link)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
            hub.Clients.All.sendMessage(title, Message, Link);
        }

        public static void SendMessageToUser(string UserName, string title, string Message, string Link)
        {
            //string name = Context.User.Identity.Name;
            var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
            foreach (var connectionId in _connections.GetConnections(Message))
            {
                hub.Clients.Client(connectionId).sendMessage(title, Message, Link, UserName);
            }
        }
        public static void SendMessageToAllExceptUser(string UserName, string title, string Message, string Link)
        {
            //string name = Context.User.Identity.Name;
            var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");

            var UserConnections = _connections.GetConnections(Message);
            if (UserConnections.Count() > 0)
            {
                hub.Clients.AllExcept(_connections.GetConnections(Message).First()).sendMessage(title, Message, Link, UserName);
            }
            else
            {
                SendMessageToAll(title, Message, Link);
            }
        }

        public void SendMessge(string Message)
        {
            Clients.All.sendMessage(Message);
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;

            _connections.Add(name, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;

            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }

            return base.OnReconnected();
        }
    }
}