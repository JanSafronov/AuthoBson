using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace AuthoBson.Models
{
    [AttributeUsage(AttributeTargets.Field ^ AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    sealed class UniqueAttribute : Attribute
    {
        readonly string propName;

        public UniqueAttribute(string propName)
        {
            this.propName = propName;
        }

        public string PropName
        {
            get { return PropName; }
        }

        public int NamedInt { get; set; }
    }
}
