using Pidgin;
using static Pidgin.Parser;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class OriginFormParser
    {
        private static readonly Parser<char, string> Segment = UriComponents.PChar.ManyString();

        public static readonly Parser<char, string> AbsolutePath =
            SimpleParsers.Slash.Then(Segment).AtLeastOnce().Select(res => PrependChar(res, '/'));

        private static string PrependChar(IEnumerable<string> segments, char delimiter)
        {
            var stringBuilder = new StringBuilder();
            foreach (var segment in segments)
            {
                stringBuilder.Append(delimiter);
                stringBuilder.Append(segment);
            }
            return stringBuilder.ToString();
        }
    }
}
