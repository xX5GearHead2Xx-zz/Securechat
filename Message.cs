using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Konscious.Security.Cryptography;
using Microsoft.AspNet.SignalR;

namespace SecureChat
{
    public class Message : Hub
    {
        public Task CreateChatGroup()
        {
            string ClientConnectionID = Context.ConnectionId;

            string GroupToken = ClientConnectionID;

            Groups.Add(ClientConnectionID, GroupToken);

            return Clients.Caller.groupCreated(GroupToken);
        }

        public Task JoinExistingGroup(string GroupToken, string Username)
        {
            //If the entered group token is not a valid guid
            //Generate a new GUID and then connect the user
            if (!Guid.TryParse(GroupToken, out Guid Token))
            {
                GroupToken = Guid.NewGuid().ToString();
            }

            Groups.Add(Context.ConnectionId, GroupToken.ToString());
            Clients.Group(GroupToken.ToString()).userJoinedNotification(Username);
            return Clients.Caller.connectedToGroup(GroupToken.ToString());
        }

        public Task BroadCastToGroup(string Message, string SenderName, string GroupToken)
        {
            return Clients.Group(GroupToken).receiveGroupMessage(Message, SenderName);
        }
    }
}