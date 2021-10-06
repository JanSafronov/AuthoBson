using System;

namespace AuthoBson.Shared.Data.Models
{
    public class ModelBase : IModelBase
    {
        /*public ModelBase()
        {
            CreatedAt = DateTime.UtcNow;
        }*/
        
        public long Id { get; set; }
        /*public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }*/
    }
}