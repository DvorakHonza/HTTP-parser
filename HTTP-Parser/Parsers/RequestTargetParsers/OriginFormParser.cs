using Pidgin;
using HTTP_Parser.HTTP.Utilities;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class OriginFormParser
    {
        public static readonly Parser<char, string> AbsolutePath =
            SimpleParsers.Slash
                .Then(UriComponents.Segment)
                .AtLeastOnce()
                .Select(res => StringFormatting.PrependChar(res, '/'));
    }
}
