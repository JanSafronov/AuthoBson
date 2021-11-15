using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using AuthoBson.Shared.Data.Models;

namespace AuthoBson.Shared
{
    public static class DefinitionBuildersExtensions
    {
        public static UpdateDefinition<M> PushMultiple<M>(this UpdateDefinitionBuilder<M> builder, IDictionary<string, object> pairs) where M : ModelBase
        {
            UpdateDefinition<M> update = default(UpdateDefinition<M>);

            foreach (KeyValuePair<string, object> pair in pairs)
            {
                update = update.Push(pair.Key, pair.Value);
            }
            return update;
        }
    }
}
