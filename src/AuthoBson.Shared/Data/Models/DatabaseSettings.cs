using System.Security;
using System.Security.Cryptography;

namespace AuthoBson.Shared.Data.Models
{
    public class StoreDatabaseSettings : StoreDatabase, IStoreDatabaseSettings
    {
        public string ConnectionString { get; set; }
    }

    public class StoreDatabase : IStoreDatabase
    {
        public string CollectionName { get; set; }
        public string DatabaseName { get; set; }
        public HashAlgorithmName HashAlgorithm { get; set; }
    }

    public interface IStoreDatabaseSettings : IStoreDatabase
    {
        string ConnectionString { get; set; }
    }

    public interface IStoreDatabase
    {
        string CollectionName { get; set; }
        string DatabaseName { get; set; }
        HashAlgorithmName HashAlgorithm { get; set; }
    }
}
