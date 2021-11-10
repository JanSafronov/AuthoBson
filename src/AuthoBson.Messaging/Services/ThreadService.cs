using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Messaging.Data.Models.Templates;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Messaging.Services
{
    /*public class ThreadService : SharedService<Message>
    {
        private IMongoCollection<Message> Messages { get; set; }

        private ThreadTemplate Template { get; set; }

        /*public ThreadService(IStoreDatabaseSettings settings, ThreadTemplate template) :
            base(settings, template)
        { }

        public ThreadService(IStoreDatabase settings, ThreadTemplate template) :
            base(settings, template)
        { }

        /// <summary>
        /// Returns by conditional Ids the list of all users
        /// </summary>
        /// <param name="senderId">Id of the sender</param>
        /// <param name="receiverId">Id of the receiver</param>
        /// <returns>List of all users by optional Ids</returns>
        public List<Thread> GetAll(string senderId = null, string receiverId = null) => Messages.Find(Message =>
            senderId != null ? Message. == senderId :
            receiverId == null || Thread == receiverId).ToList();

        /// <summary>
        /// Find's the message by it's Id
        /// </summary>
        /// <param name="id">Id of the message to find</param>
        /// <returns>Found message or null</returns>
        public Message GetMessage(string Id) => Messages.Find(Message => Message.Id == Id).As<Message>().FirstOrDefault();

        /// <summary>
        /// Creates a new message in the database's collection
        /// </summary>
        /// <param name="User">The message to insert in the database's collection</param>
        /// <returns>The inserted message</returns>
        public Message CreateMessage(Message Message)
        {
            if (Template.IsSchematic(Message))
            {
                Messages.InsertOne(Message);
                return Message;
            }
            return null;
        }
    }*/
}
