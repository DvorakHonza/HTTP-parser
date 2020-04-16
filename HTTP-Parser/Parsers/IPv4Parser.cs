using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using static Pidgin.Parser;

namespace HTTP_Parser.Parsers
{
    public static class IPv4Parser
    {
        private static readonly Parser<char, int> DecOctet = DecimalNum.Where(res => res >= 0 && res <= 255).Labelled("Decimal Octet");

        public static readonly Parser<char, List<int>> Octets =
            Map((first, second, third, fourth) => new List<int>() { first, second, third, fourth },
                DecOctet.Before(SimpleParsers.Dot),
                DecOctet.Before(SimpleParsers.Dot),
                DecOctet.Before(SimpleParsers.Dot),
                DecOctet.Before(SkipWhitespaces)
                ).Labelled("IPv4Address octets");

        public static readonly Parser<char, string> Address =
            Octets.Select(res => IPv4ToDotted(res)).Labelled("IPv4Address");

        private static string IPv4ToDotted(List<int> octets)
        {
            return $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}";
        }

        public static string IPv4ToHex(List<int> octets)
        {
            int top16 = octets[0] * 16 ^ 2 + octets[1];
            int low16 = octets[2] * 16 ^ 2 + octets[3];
            return top16.ToString("X") + ":" + low16.ToString("X");
        }
    }
}
