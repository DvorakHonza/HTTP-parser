using Pidgin;
using static Pidgin.Parser;


namespace HTTP_Parser.Parsers
{
    public static class IPv6Parser
    {
        public static readonly Parser<char, string> Address =
            SimpleParsers.HexDigit
            .Or(Char(':'))
            .ManyString()
            .Where(res => System.Net.IPAddress.TryParse(res, out System.Net.IPAddress ip));

        
    }
}
