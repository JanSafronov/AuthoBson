using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared.Data.Models
{
    public record ModelReference : IModelBase
    {
        public string Id { get; }

        public Type Type { get; set; }

        public ModelReference(string Id, string type)
        {
            this.Id = Id;
            this.Type = Type.GetType(type);
        }

        public ModelReference(string Id, IModelBase model)
        {
            this.Id = Id;
            Type = model.GetType();
        }
    }
}
