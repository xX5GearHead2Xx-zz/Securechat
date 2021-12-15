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

            string GroupToken = ArgonHash(ClientConnectionID);

            Groups.Add(ClientConnectionID, GroupToken);

            return Clients.Caller.groupCreated(GroupToken);
        }

        public Task JoinExistingGroup(string GroupToken, string Username)
        {
            Groups.Add(Context.ConnectionId, GroupToken);

            Clients.Group(GroupToken).userJoinedNotification(Username);

            return Clients.Caller.connectedToGroup(GroupToken);
        }

        public Task BroadCastToGroup(string Message, string SenderName, string GroupToken)
        {
            return Clients.Group(GroupToken).receiveGroupMessage(Message, SenderName);
        }

        private string ArgonHash(string Input)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(Input));
            argon2.DegreeOfParallelism = 4;
            argon2.Iterations = 1;
            argon2.MemorySize = 1024 * 1024;

            byte[] ArgonBytes = argon2.GetBytes(16);
            string Final = Convert.ToBase64String(ArgonBytes);
            return Final;
        }
    }
}