using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Events;
using MongoDB.Driver.Core.Events.Diagnostics;
using MongoDB.Driver.Core.Operations;
using MongoDB.Driver.Encryption;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Driver.Linq;
using Models;

namespace Services {
    public static class UserService {

        private static IEnumerable<User> _Users { get; }

        /// <summary>
        /// Returns the Users enumerable collection
        /// </summary>
        /// <returns>Users enumerable collection</returns>
        public static IEnumerable<User> GetAll() => _Users;

        /// <summary>
        /// Finds a User from an enumerable collection by username
        /// </summary>
        /// <param name="username">Username of the User to be found</param>
        /// <returns>User object or null</returns>
        public static User GetUser(Field<string> identificator) {
            
            User current = _Users.GetEnumerator().Current;
            
            if (current.Identificator.email == identificator) return current;
            
            return _Users.GetEnumerator().MoveNext() ? GetUser(username) : new GenericUser("", "", "", true);
        }
    }
}