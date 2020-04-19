using System;
using Pidgin;
using static Pidgin.Parser;
using System.Collections.Generic;
using HTTP_Parser.HTTP.RequestTargets;

namespace HTTP_Parser.Parsers.RequestTargetParsers
{
    public static class AuthorityFormParser
    {
        public static readonly Parser<char, string> UserInfo =
            OneOf(new List<Parser<char, char>> {
                SimpleParsers.Unreserved,
                SimpleParsers.PercentEncoding,
                SimpleParsers.UriSubDelims,
                SimpleParsers.Colon })
            .ManyString()
            .Labelled("User info");

        private static readonly Parser<char, string> RegisteredName =
            OneOf(new List<Parser<char, char>> {
                    SimpleParsers.Unreserved,
                    SimpleParsers.PercentEncoding,
                    SimpleParsers.UriSubDelims })
            .ManyString()
            .Labelled("RegisteredName");

        private static readonly Parser<char, string> IpLiteral =
            IPvFutureParser.IpVFuture.Or(IPv6Parser.Address)
            .Between(SimpleParsers.LeftBracket, SimpleParsers.RightBracket)
            .Labelled("IpLiteral");

        public static readonly Parser<char, string> Host =
            IpLiteral
            .Or(IPv4Parser.Address)
            .Or(RegisteredName);

        public static RequestTarget CreateAuthorityForm(Maybe<string> userInfo, string host, Maybe<int> port)
        {
            if (userInfo.HasValue)
            {
                if (port.HasValue)
                {
                    return new AuthorityForm(userInfo.Value, host, port.Value);
                }
                return new AuthorityForm(userInfo.Value, host);
            }
            return new AuthorityForm("", host);
        }
    }
}
