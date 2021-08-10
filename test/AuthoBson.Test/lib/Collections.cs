using System;
using System.Collections;
using System.Collections.Generic;
using AuthoBson.Models;

namespace AuthoBson.Test.Collections.Generators {
    public class TestValidationGenerator : IEnumerable<object>
    {
        public Role role { get; }

        public TestValidationGenerator(Role role) {
            this.role = role;
        }

        public List<object[]> GetList() => new List<object[]> 
            { 
                new object[] { role, new Suspension("", DateTime.MaxValue) },
                new object[] { role, new Suspension("", DateTime.MinValue) } 
            };

        public IEnumerator<object> GetEnumerator() => GetList().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class TestListValidationGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new List<object[]>();

        public IEnumerator<object[]> GetEnumerator() {
            _data.AddRange(new TestValidationGenerator(Role.Generic).GetList());
            _data.AddRange(new TestValidationGenerator(Role.Senior).GetList());
            _data.AddRange(new TestValidationGenerator(Role.Moderator).GetList());
            _data.AddRange(new TestValidationGenerator(Role.Administrator).GetList());

            return _data.GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}