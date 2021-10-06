using System;
using System.Collections.Generic;
using System.Linq;

namespace AuthoBson.Messaging.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random Random = new Random();
        
        public static T RandomElement<T>(this List<T> list)
        {
            return list.ElementAt(
                Random.Next(0, list.Count - 1)
            );
        }
    }
}