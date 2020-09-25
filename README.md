

# SignalR-Demo 
for (.NetFramework)
### Description
SignalR uses webSocket technology, it is the only way to send messages from server to client and to notify client when something happen on server like message received or voucher saved or task assinged to him/her.
### Basic Idea
- you create a Hub class in C# inherited from (SignalR.Hub), for example name it **(MyHub)**
- you can create custom functions in this class for example function **(A)**
- inside custom function **(A)** retrieve the current internal  hub from context 
```csharp
var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
```
- use the intarnal hub **(Clients.All)** property to send to All or  **(Clients.Client)** property to send to specific client
- (All) and (Client) are of type (dynamic), that means you can add to them any function you want dynamically for examle , add function **(B)** to **(All)**
```csharp
hub.Clients.All.B("title" , "message text") 
```
althogh **(B)** was not orginally defined in **(All)** property, but you can just add it becuse it is of type dynamic.
- the complete code will be 
```csharp
        public static void A(string Title, string Message)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext("MyHub");
            hub.Clients.All.B(Title, Message);
        }
```
- Now you have function called **(B)** in your internal hub, the above code not just defining **(B)** but also calling it.  when calling it will call the client javascript funciton immediatly.
- Now you can call **(MyHub.A)** function from any where in your C# code in your project,   which will call the internal hub **(B)** function which will call the client javascript **(B)** funciton. for example when voucher is saved you can call **(MyHub.A)** from the backend and it will notify the client immeditaly.

- in javascript,  you have to provide the hub  with implmentation for function **(B)**, it will be called when ever the C#  hub function called. below code in javascript
```javascript
var logger = $.connection.myHub;
logger.client.B = ReceiveMessge;

function ReceiveMessge ( title, msg ) { 
	alert( title + " - " + msg )
};
```
### To Create SIngalR Project 
(with ability to send to speific user or all users )
 - Install Nuget Package (**Microsoft.AspNet.SignalR**)
 - Add class named (**Startup**) to the code, and copy the code from **(startup.cs)** file in this github repo as it is.
 - copy the class (**ConnectionMapping**) from this github repo as it is , it exist in **(SignalR)** folder .
 - create a class inherited from **(Microsoft.SingalR.Hub)** , copy the code from (**MyHub**) class in this repo. you can name the class any thing but you have to change any existance to the word  **(MyHub)** in javascript and in MyHub class when retrieving the internal hub from context to your class name.
 - check razor view (**SignalRDemo.cshtml**) and do the following
 - add following JS Files to the HTML
```javascript
"~/Scripts/jquery.signalR-2.4.1.js"
 "~/signalr/hubs"
```
- setup SignalR engine by first give implementation to your hub funciton **(B)** in this example which will be called when c# function **(A)** is called. then call hub **(start)** function

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
**(myHub)** in above code is the name of my hub class in c#.
-  you can use ajax for example to call C# function that will call Hub funciton to send messages for others , for example in chat apps we need to do this. this is optional and not part of SignalR. in our example code i created function called **(SendMessage)** in **(Home)** and i called it when the user click (Send) button. sure you can use any other method like normal postback button to do that or any way to call backend function.

### To Send message to spcific user
SignalR does not have functionality to send message to specific user directly, although it has functionality to send to specific connection,  we have to do extra work to send to specify specific user to send the message to.

as i said above, SignalR provides us with functionality to send a message to specific connection Id.  and it also provides us with a function that will be called when a connection established. we have to add code when connection established to take current (user name or id) and store it with (connection Id) in a list of (connected users and connections IDs) 

then, when ever we need to send message to that specific user.  we search for user name or id in the list.  then we get the connection ID related to that user. then we can send message to that connection ID.

in this gethub repo, the class  **(ConnectionMapping)**  is to maintain list of connected users and their connections IDs. in **MyHub**  example class in this repo, there is a function **(SendToUser)**  that will get (connection ID) and send to it. and also a code to add user to the list and remove user from list when connect or disconnect to the application.

**Note** : the repo code depend on **ASP Identity** to retrieve current user name to add it to list. but the repo code not actually implement **ASP IDentity** code, it is out of scope of this example. to make this code actually work, either add code to implement **ASP IDentity** or change the code that retreive user name to your own code, this code exists in **MyHub** class when establishing new connection.

### Notification Example
by the way, the project also has an example code to show to you how your site can use windows or OS notification system to notifiy the user with any new messages if user allowes it. it first asks the user to allow site to notify him/her. or it will use normal in page notification by using (toaster) library to notify user. if OS notification is not allowed.

### Advance Subjects
- SignalR is actually a big library, above code is very basic simple usage of it, it has so many more functionalities
- one of the most important functionlity, it **fallback** if the **browser does not support (WebSocket)** new technology,it fallback  to several other functionalitis like (server calls) and (long calls) which is an adhoc to send a request with unlimited time and keep connection open with the server and return response only when the server wants to notify the user, when the user gets the response he immediatly sends an other long call
so , signalR tries to find the most suitable technology to communicate with the client automatically and you don't have to write any thing for that.
- an other issue, if your website is deployed on **multiple servers** (web farm, web garden, .. etc) with a **load balancer** for example. you have to maintain SignalR data in a central location, SignalR supports that, you can use the **(backplane)** option to stores the data in **(SQL or Radis Databases)** for example or any other choices.


