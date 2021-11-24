using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;

namespace AuthoBson.Shared.Data.Models
{
    // Routed database/collection pairs with a defaul collection pair
    public class RoutedDatabaseSettings : StoreDatabaseSettings, IRoutedDatabaseSettings, IStoreDatabaseSettings, IStoreDatabase
    {
        public IDictionary<string, string[]> Routes { get; set; }

        public IStoreDatabaseSettings DeriveStoreDatabaseSettings() =>
            new StoreDatabaseSettings
            {
                ConnectionString = this.ConnectionString,
                CollectionName = this.CollectionName,
                DatabaseName = this.DatabaseName
            };
    }

    // database/collection pair for default collection
    public class StoreDatabaseSettings : StoreDatabase, IStoreDatabaseSettings, IStoreDatabase
    {
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
    }

    // Connection with no database/collection pairs for defaul collection
    public class StoreDatabase : IStoreDatabase
    {
        public string ConnectionString { get; set; }
    }

    // Routed database/collection pairs with the first being the defaul collection
    public interface IRoutedDatabaseSettings : IStoreDatabaseSettings, IStoreDatabase
    {
        public IDictionary<string, string[]> Routes { get; set; }

        IStoreDatabaseSettings DeriveStoreDatabaseSettings();
    }

    // database/collection pair for default collection
    public interface IStoreDatabaseSettings : IStoreDatabase 
    {
        string CollectionName { get; set; }
        string DatabaseName { get; set; }
    }

    // Connection with no database/collection pairs for defaul collection
    public interface IStoreDatabase
    {
        string ConnectionString { get; set; }
    }
}
