using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace HTTP_Parser.HTTP.RequestTargets
{
    public class AbsoluteForm : RequestTarget
    {
        public string Scheme { get; }
        public string HierarchyPart { get; }
        public ImmutableDictionary<string, string> Queries { get; }
        public string Fragment { get; }

        public AbsoluteForm(string Scheme, string HierarchyPart)
        {
            this.Scheme = Scheme;
            this.HierarchyPart = HierarchyPart;
            this.Queries = ImmutableDictionary.Create<string, string>();
            this.Fragment = "";
        }
        public AbsoluteForm(string Scheme, string HierarchyPart, ImmutableDictionary<string, string> Queries)
        {
            this.Scheme = Scheme;
            this.HierarchyPart = HierarchyPart;
            this.Queries = Queries;
            this.Fragment = "";
        }
        public AbsoluteForm(string Scheme, string HierarchyPart, ImmutableDictionary<string, string> Queries, string Fragment)
        {
            this.Scheme = Scheme;
            this.HierarchyPart = HierarchyPart;
            this.Queries = Queries;
            this.Fragment = Fragment;
        }

        public override string ToString()
        {
            return $"{Scheme}://{HierarchyPart}?{string.Join("&", Queries.Select(kvp => $"{kvp.Key}={kvp.Value}"))}#{Fragment}";
        }

    }
}
