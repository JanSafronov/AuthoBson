using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom;
using AuthoBson.Messaging.Data.Models;
using AuthoBson.Shared.Data;
using AuthoBson.Shared.Data.Models;
using AuthoBson.Shared.Services;
using AuthoBson.Shared;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Messaging.Services
{
    public class MessageService : SharedRoutedService<Message, ModelBase>
    {
        private IMongoCollection<Message> Messages { get; set; }

        private IMongoCollection<ModelBase>[] Routes { get; set; }

        private MessageTemplate Template { get; set; }

        private SecurityMechanism<Message, SHA256> Mechanism { get => new(); set => Mechanism = value; }

        public MessageService(IStoreDatabaseSettings settings, MessageTemplate template) :
            base(settings, template)
        { }

        public MessageService(IRoutedDatabaseSettings settings, MessageTemplate template) :
            base(settings, template)
        { }

        /// <summary>
        /// Returns by conditional ids the list of all messages
        /// </summary>
        /// <param name="senderId">Id of the sender</param>
        /// <param name="receiverId">Id of the receiver</param>
        /// <returns>List of all messagess by optional Ids</returns>
        public List<Message> GetAll(string senderId = null, string receiverId = null) => Messages.Find(Message =>
            senderId != null ? Message.Receiver.Id == senderId :
            receiverId == null || Message.Sender.Id == receiverId).ToList();

        /// <summary>
        /// Find's the message by it's id
        /// </summary>
        /// <param name="id">Id of the message to find</param>
        /// <returns>Found message or null</returns>
        public Message GetMessage(string id) =>
            Get<Message>(id);

        /// <summary>
        /// Asynchronously find's the message by it's id
        /// </summary>
        /// <param name="id">Id of the message to find</param>
        /// <returns>Found message or null</returns>
        public async Task<Message> GetMessageAsync(string id) =>
            await GetAsync<Message>(id);

        /// <summary>
        /// Creates a new message in the database's collection
        /// </summary>
        /// <param name="message">The message to insert in the database's collection</param>
        /// <returns>The inserted message</returns>
        public Message CreateMessage(Message message) =>
            Create(message);

        /// <summary>
        /// Asynchronously creates a new message in the database's collection
        /// </summary>
        /// <param name="message">The message to insert in the database's collection</param>
        /// <returns>The inserted message</returns>
        public async Task<Message> CreateMessageAsync(Message message) =>
            await CreateAsync(message);

        /// <summary>
        /// Verifies that all references exist in the typed collection
        /// </summary>
        /// <param name="references">References to models that need to be verified</param>
        /// <returns>Whether all references are verified</returns>
        public bool VerifyReferences(params ModelReference[] references)
        {
            foreach (ModelReference reference in references)
            {
                if (reference == null)
                    return false;

                int index = Array.FindIndex(Routes, route => route.CollectionNamespace.CollectionName == reference.Route.Value && route.Database.DatabaseNamespace.DatabaseName == reference.Route.Key);

                if (!ExistsInRoute<ModelBase>(index, reference.Id, null))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Asynchronously verifies that all references exist in the typed collection
        /// </summary>
        /// <param name="references">References to models that need to be verified</param>
        /// <returns>Whether all references are verified</returns>
        public async Task<bool> VerifyReferencesAsync(params ModelReference[] references) =>
            await Task.FromResult(references.All(reference =>
            {
                if (reference == null)
                    return false;

                int index = Array.FindIndex(Routes, route => route.CollectionNamespace.CollectionName == reference.Route.Value && route.Database.DatabaseNamespace.DatabaseName == reference.Route.Key);

                if (!ExistsInRoute<ModelBase>(index, reference.Id, null))
                    return false;

                return true;
            }));
    }
}
