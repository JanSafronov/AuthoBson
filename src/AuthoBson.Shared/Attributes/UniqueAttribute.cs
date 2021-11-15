using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace AuthoBson.Shared
{
    [AttributeUsage(AttributeTargets.Field ^ AttributeTargets.Property ^ AttributeTargets.Parameter, Inherited = false, AllowMultiple = true)]
    public sealed class UniqueAttribute : Attribute
    {
        readonly string propName;

        public UniqueAttribute(string propName)
        {
            this.propName = propName;
        }

        public UniqueAttribute() { }

        public string PropName
        {
            get { return PropName; }
        }

        public int NamedInt { get; set; }
    }
}
