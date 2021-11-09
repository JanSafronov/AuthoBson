using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace AuthoBson.Models
{
    public interface IBsonUserDocument
    {
        BsonDocument User { get; set; }

        /// <summary>
        /// Functor mapping between fields and preserving the initial type
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Pattern of mapping</param>
        /// <returns>User object with a field mapped by the functor</returns>
        UserDocument Functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue;
    }

    /// <summary>
    /// Instantiated class of IUserDocument
    /// </summary>
    /// <remarks>
    /// The document doesn't preserves it's initial fields type/value structure
    /// </remarks>
    public class UserDocument : BsonDocument, IBsonUserDocument
    {
        public BsonDocument User { get; set; }

        public UserDocument(IUser User)
        {
            this.User = User.ToBsonDocument();
        }

        /// <summary>
        /// Returns a functor of a document mapping between a bson field and another bson field
        /// </summary>
        /// <param name="key">Identity of the field</param>
        /// <param name="functor">Function to morph the bson value</param>
        /// <returns>User object with a field mapped by the functor</returns>
        public UserDocument Functor<B>(string key, Func<BsonValue, B> functor) where B : BsonValue
        {
            int i = User.IndexOfName(key);
            B b = functor(User.GetValue(i));

            User.Set(i, b);

            return this;
        }
    }
}
