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

namespace AuthoBson.Shared.Extensions
{
    public static class DefinitionBuildersExtensions
    {
        public static UpdateDefinition<M> SetMultiple<M>(this UpdateDefinitionBuilder<M> builder, IDictionary<string, object> pairs) where M : ModelBase
        {
            UpdateDefinition<M> update = builder.Set(pairs.FirstOrDefault().Key, pairs.FirstOrDefault().Value);

            foreach (var pair in pairs)
            {
                update = update.Set(pair.Key, pair.Value);
            }
            return update;
        }
    }
}
