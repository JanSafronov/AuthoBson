using System.Threading.Tasks;

namespace AuthoBson.Messaging.Data
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
    }
}