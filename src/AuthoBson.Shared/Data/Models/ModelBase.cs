using System;

namespace AuthoBson.Shared.Data.Models
{
    public class ModelBase : IModelBase
    {
        /*public ModelBase()
        {
            CreatedAt = DateTime.UtcNow;
        }*/
        
        public string Id { get; }
        /*public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }*/
    }
}