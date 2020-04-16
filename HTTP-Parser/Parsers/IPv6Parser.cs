using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;
using static HTTP_Parser.Parsers.IPv4Parser;


namespace HTTP_Parser.Parsers
{
    public static class IPv6Parser
    {
        private static readonly Parser<char, int> h16 =
            HexNum.Where(res => res >= 0 && res <= 0xFFFF).Labelled("16 bits of IPv6 Address");

        private static readonly Parser<char, string> ls32 =
            h16.Separated(SimpleParsers.Colon).Select(res => res.ToString()).
            Or(Octets.Select(res => IPv4ToHex(res))).Labelled("32 least significant bits of IPv6 address");

        private static readonly Parser<char, string> IPv6WithoutShortening =
            Map((begin, rest) => begin + rest, h16.Select(res => res.ToString("X") + ":").Repeat(6), ls32);

        private static readonly Parser<char, string> IPv6First16BitsShortened =
            Map((begin, rest) => "::" + begin + rest, SimpleParsers.DoubleColon.Then(h16.Select(res => res.ToString("X") + ":").Repeat(5)), ls32);

        public static readonly Parser<char, string> Address =
            OneOf(new List<Parser<char, string>>() {
                IPv6WithoutShortening,
                IPv6First16BitsShortened
            }).Labelled("IPv6Address"); //TODO IPv6 address parsing

    }
}
