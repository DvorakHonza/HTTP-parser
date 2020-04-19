using Pidgin;
using static Pidgin.Parser;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class UriComponents
    {
        public static readonly Parser<char, string> Scheme = String("http");

        public static readonly Parser<char, char> PChar =
           OneOf(new List<Parser<char, char>>() {
                SimpleParsers.Unreserved,
                SimpleParsers.PercentEncoding,
                SimpleParsers.UriSubDelims,
                SimpleParsers.Colon,
                SimpleParsers.AtSign }).Labelled("pchar");

        private static readonly Parser<char, KeyValuePair<string, string>> Query =
             PChar
            .Or(SimpleParsers.Slash)
            .Or(SimpleParsers.QuestionMark)
            .ManyString()
            .Select(HttpQueryToKvPair);

        public static readonly Parser<char, string> Fragment =
            SimpleParsers.NumberSign
            .Then(PChar
                .Or(SimpleParsers.Slash)
                .Or(SimpleParsers.QuestionMark)
                .ManyString())
            .Select(res => "#" + res).Labelled("Fragment");

        public static readonly Parser<char, ImmutableDictionary<string, string>> Queries =
            Query
            .Separated(Char('&'))
            .Select(kvps => kvps.ToImmutableDictionary())
            .Labelled("Queries");


        private static KeyValuePair<string, string> HttpQueryToKvPair(string query)
        {
            var parts = query.Split(new[] { '=' }, 2);
            if (parts.Length == 2)
            {
                return new KeyValuePair<string, string>(parts[0], parts[1]);
            }
            return new KeyValuePair<string, string>(parts[0], "");

        }
    }
}
