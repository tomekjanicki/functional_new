using System.Collections.Generic;

namespace ApiClient.Infrastructure
{
    public sealed class Header
    {
        public Header(string name, IEnumerable<string> values)
        {
            Name = name;
            Values = values;
        }

        public string Name { get; }

        public IEnumerable<string> Values { get; }
    }
}