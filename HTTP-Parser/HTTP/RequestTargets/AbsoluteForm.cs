using System.Collections.Immutable;
using System.Linq;

namespace HTTP_Parser.HTTP.RequestTargets
{
    public class AbsoluteForm : RequestTarget
    {
        public string Scheme { get; }
        public string HierarchyPart { get; }
        public ImmutableDictionary<string, string> Queries { get; }
        public string Fragment { get; }

        public AbsoluteForm(string scheme, string hierarchyPart)
        {
            Scheme = scheme;
            HierarchyPart = hierarchyPart;
            Queries = ImmutableDictionary.Create<string, string>();
            Fragment = "";
        }
        public AbsoluteForm(string scheme, string hierarchyPart, ImmutableDictionary<string, string> queries)
        {
            Scheme = scheme;
            HierarchyPart = hierarchyPart;
            Queries = queries;
            Fragment = "";
        }
        public AbsoluteForm(string scheme, string hierarchyPart, ImmutableDictionary<string, string> queries, string fragment)
        {
            Scheme = scheme;
            HierarchyPart = hierarchyPart;
            Queries = queries;
            Fragment = fragment;
        }

        public override string ToString()
        {
            return $"{Scheme}://{HierarchyPart}?{string.Join("&", Queries.Select(kvp => $"{kvp.Key}={kvp.Value}"))}#{Fragment}";
        }

    }
}
