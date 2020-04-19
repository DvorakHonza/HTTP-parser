using Pidgin;
using static Pidgin.Parser;
using System.Collections.Generic;

namespace HTTP_Parser.Parsers
{
    public static class IPvFutureParser
    {
        public static readonly Parser<char, string> IpVFuture =
            Map((version, end) => "v" + version + "." + string.Concat(end),
                Char('v').Then(HexNum),
                SimpleParsers.Dot.Then(OneOf(new List<Parser<char, char>>() {
                    SimpleParsers.Unreserved,
                    SimpleParsers.UriSubDelimiters,
                    SimpleParsers.Colon }).AtLeastOnce()
                ));
    }
}
