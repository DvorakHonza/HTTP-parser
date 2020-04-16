using Pidgin;
using static Pidgin.Parser;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTTP_Parser.Parsers
{
    public static class IPvFutureParser
    {
        public static readonly Parser<char, string> IpVFuture =
            Map((version, end) => "v" + version.ToString() + "." + SimpleParsers.ConvertIEnumerableToString(end),
                Char('v').Then(HexNum),
                SimpleParsers.Dot.Then(OneOf(new List<Parser<char, char>>() {
                    SimpleParsers.Unreserved,
                    SimpleParsers.UriSubDelims,
                    SimpleParsers.Colon }).AtLeastOnce()
                ));
    }
}
