

# SignalR-Demo 
for (.NetFramework)
### Description
SignalR uses webSocket technology, it is the only way to send messages from server to client and to notify client when something happen on server like message received or voucher saved or task assinged to him/her.
### Basic Idea
- you create a Hub class in C# inherted from (SignalR.Hub), for example call it **(MyHub)**
- you can create custom functions in this class for example function **(A)**
- inside custom function **(A)** retrieve the current internal  hub from context 
```csharp
var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
```
- use the intarnal hub **(Clients.All)** property to send to All or  **(Clients.Client)** property to send to specific client
- (All) and (Client) are from type dynamic, than means you can add to them any function you want dynamically for examle , add function **(B)** to it
```csharp
hub.Clients.All.B("title" , "message text") 
```
althogh **(B)** was not orginally defined in (All) property, but you can just add it becuse it is of type dynamic.
- the complete code will be 
```csharp
        public static void A(string Title, string Message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
            hub.Clients.All.B(Title, Message);
        }
```
- Now you have function already called** (B)** in your internal hub, the above code not just defining **(B)** but also calling it.  when calling it will call the client javascript funciton immediatly.
- Now you can call** (MyHub.A)** function from any where in your C# code in your project,   which will call the internal hub **(B)** function which will call the client javascript **(B)** funciton. for example when voucher is saved you can call **(MyHub.A)** from the backend and it will notify the client immeditaly.

- in javascript,  you have to provide the hub  with implmentation for function **(B)**, it will be called when ever the C#  hub function called. below code in javascript
```javascript
var logger = $.connection.myHub;
logger.client.B = ReceiveMessge;
function ReceiveMessge(title, msg) { 
	alert(title + " - " + msg)
};
```
### To Create SIngalR Project 
(with ability to send to speific user or all users )
 - Install Nuget Package (**Microsoft.AspNet.SignalR**)
 - Add class named (**Startup**) to the code, and copy the code from **startup.cs** file in thia github project as it is.
 - copy the class (**ConnectionMapping**) from this github project as it is , it exist in folder **(SignalR)**.
 - create a class inherited from **SingalR.Hub** , copy the code from (**MyHub**) class in this project. you can call the class any thing but you have to change MyHub in js to your class name. and when retrieving the internal hub from context.
 - check view (**SignalRDemo.cshtml**) and do the following
 - add following JS Files to the HTML
```javascript
"~/Scripts/jquery.signalR-2.4.1.js"
 "~/signalr/hubs"
```
- setup SignalR engine by first give implementation to your hub funciton which will be called when c# function is called. then call hub start function

```javascript
        function SetupSignalR() {
            var logger = $.connection.myHub;
            logger.client.B = ReceiveMessge;
            $.connection.hub.start();
        }

         function ReceiveMessge(title, msg) {
                alert( title + " - " + msg )
          }
```
-  you can use ajax for example to call C# function that will call Hub funciton to send messages for others , for example in Chat apps. this is optional and not part of SignalR

### To Send message to spcific user
SignalR does not normally do that, we have to do extra work to do that.

SignalR provide us with functionality to send message to specific connection ID.  and a function that will be called when connection established. we have to add code when connection established to take current user name or id and store it with connection ID in a list of connected users and IDs 

then, when ever we need to send message to that specific user.  we search for user name or id in the list.  then we get the connection ID related to that user. then we can send message to that connection ID.

in this gethub project, the class  (**ConnectionMapping**)  is to maintain list of connected users and their connections IDs. in **MyHub**  example class in this project, there is a function (SendToUser)  that will get connection ID and send to it. and also a code to add user to the list and remove user from list when connect or disconnect to the application.

**Note** : the code depend on ASP Identity to retrieve current user name to add it to list. but it not actually implement ASP IDentity code. either you add code to use ASP IDentity or change the code that retreive user name to your own code.

### Notification Example
by the way, the project also has an example code to show to you how your site can use windows or OS notification system to notifiy the user with any new messages if user allowes it. it first asks the user to allow site to notify him/her. or it will use normal in page notification by using (toaster) library to notify user. if OS notification is not allowed.


