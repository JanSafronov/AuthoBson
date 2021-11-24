using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Messaging.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Messaging.Services
{
    public class ThreadService : SharedService<Thread>
    {
        private IMongoCollection<Thread> Threads { get; set; }

        private ThreadTemplate Template { get; set; }

        public ThreadService(IStoreDatabaseSettings settings, ThreadTemplate template) :
            base(settings, template)
        { Template = template; Threads = Items; }

        public ThreadService(IRoutedDatabaseSettings settings, ThreadTemplate template) :
            base(settings, template)
        { Template = template; Threads = Items; }

        /// <summary>
        /// Returns by conditional Ids the list of all users
        /// </summary>
        /// <param name="senderId">Id of the sender</param>
        /// <param name="receiverId">Id of the receiver</param>
        /// <returns>List of all users by optional Ids</returns>
        public List<Thread> GetAll(Expression<Func<Thread, bool>> filter = null) =>
            GetAll<Thread>(filter, null);

        /// <summary>
        /// Find's the message by it's Id
        /// </summary>
        /// <param name="id">Id of the message to find</param>
        /// <returns>Found message or null</returns>
        public Thread GetThread(string id) =>
            Get<Thread>(id);

        /// <summary>
        /// Creates a new message in the database's collection
        /// </summary>
        /// <param name="User">The message to insert in the database's collection</param>
        /// <returns>The inserted message</returns>
        public Thread CreateThread(Thread thread)
        {
            if (Template.IsSchematic(thread))
            {
                Threads.InsertOne(thread);
                return thread;
            }
            return null;
        }
    }
}
