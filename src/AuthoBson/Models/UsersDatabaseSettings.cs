using System;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AuthoBson.Models
{
    public class UserstoreDatabaseSettings : UserstoreDatabase
    {
        public string ConnectionString { get; set; }
    }

    public class UserstoreDatabase : IUserstoreDatabase {
        public string UsersCollectionName { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IUserstoreDatabaseSettings : IUserstoreDatabase
    {
        string ConnectionString { get; set; }
    }

    public interface IUserstoreDatabase {
        string UsersCollectionName { get; set; }
        string DatabaseName { get; set; }
    }
}