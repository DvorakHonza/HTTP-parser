﻿using System.Collections.Immutable;
using System.Linq;

namespace HTTP_Parser.HTTP.RequestTargets
{
    public class OriginForm : RequestTarget
    {
        public string AbsolutePath { get; }
        public ImmutableDictionary<string, string> Queries { get; }

        public OriginForm(string absolutePath, ImmutableDictionary<string, string> queries)
        {
            AbsolutePath = absolutePath;
            Queries = queries;
        }
        public OriginForm(string absolutePath)
        {
            AbsolutePath = absolutePath;
            Queries = ImmutableDictionary.Create<string, string>();
        }
        public override string ToString()
        {
            if (Queries.IsEmpty)
            {
                return AbsolutePath;
            }
            return $"{AbsolutePath}?{string.Join("&", Queries.Select(kvp => $"{kvp.Key}={kvp.Value}"))}";
        }

    }
}
