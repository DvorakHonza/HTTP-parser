using HTTP_Parser.HTTP;
using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;
using System;

namespace HTTP_Parser
{
    public static class HttpParser
    {
        private static readonly char[] Delimiters = { '"', '(', ')', ',', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '{', '}' };
        private static readonly char[] tCharSpecialSymbols = { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };
        private static readonly char[] UriSubcomponentDelimiters = { '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };

        private static readonly Parser<char, char> Slash = Char('/');
        private static readonly Parser<char, char> Dot = Char('.');
        private static readonly Parser<char, char> Space = Char(' ');
        private static readonly Parser<char, char> HTab = Char('\t');
        private static readonly Parser<char, char> QMark = Char('?');
        private static readonly Parser<char, char> EqualsSign = Char('=');
        private static readonly Parser<char, string> Http = String("HTTP");
        private static readonly Parser<char, string> Asterisk = String("*");
        private static readonly Parser<char, string> CRLF = String("\r\n");
        private static readonly Parser<char, char> AtSign = Char('@');
        private static readonly Parser<char, char> NumberSign = Char('#');
        private static readonly Parser<char, char> Delimiter = OneOf(Delimiters).Labelled("Delimiter");
        private static readonly Parser<char, char> TChar = OneOf(new List<Parser<char, char>>() { OneOf(tCharSpecialSymbols), LetterOrDigit }).Labelled("tchar");
        private static readonly Parser<char, char> UriSubDelims = OneOf(UriSubcomponentDelimiters).Labelled("UriSubDelims//");
        private static readonly Parser<char, char> Dash = Char('-');
        private static readonly Parser<char, char> Underscore = Char('_');
        private static readonly Parser<char, char> Tilde = Char('~');
        private static readonly Parser<char, char> Colon = Char(':');
        private static readonly Parser<char, string> DoubleColon = String("::");
        private static readonly Parser<char, string> DoubleSlash = String("//");
        private static readonly Parser<char, char> LeftBracket = Char('[');
        private static readonly Parser<char, char> RightBracket = Char(']');
        private static readonly Parser<char, char> Unreserved = OneOf(new List<Parser<char, char>>() { LetterOrDigit, Dash, Dot, Underscore, Tilde }).Labelled("Unreserved");
        private static readonly Parser<char, char> Percent = Char('%');
        private static readonly Parser<char, char> PercentEncoding =
            from percent in Percent
            from number in HexNum.Where(res => res >= 0 && res <= 255).Labelled("PercentEncoding hex value")
            select Convert.ToChar(Convert.ToUInt32(number));

        private static readonly Parser<char, string> Scheme = String("http");
        private static readonly Parser<char, string> UserInfo =
            OneOf(new List<Parser<char, char>>() { Unreserved, PercentEncoding, UriSubDelims, Colon }).ManyString().Labelled("User info");
        private static readonly Parser<char, string> IpVFuture =
            from beginning in Char('v').Labelled("IpVFuture v")
            from version in HexNum.AtLeastOnce().Labelled("IpVFuture version")
            from dot in Dot.Labelled("IpVFuture dot")
            from end in OneOf(new List<Parser<char, char>>() { Unreserved, UriSubDelims, Colon }).AtLeastOnce().Labelled("IpVFuture end")
            select beginning.ToString() + version.ToString() + dot.ToString() + end;

        private static readonly Parser<char, int> DecOctet = DecimalNum.Where(res => res >= 0 && res <= 255).Labelled("Decimal Octet");
        private static readonly Parser<char, List<int>> IPv4AddressOctets =
            from firstOctet in DecOctet.Before(Dot)
            from secondOctet in DecOctet.Before(Dot)
            from thirdOctet in DecOctet.Before(Dot)
            from fourthOctet in DecOctet.Before(Whitespace) //What if ip address is not followed by whitespace
            select new List<int>() { firstOctet, secondOctet, thirdOctet, fourthOctet };
        private static readonly Parser<char, string> IPv4Address = IPv4AddressOctets.Select(res => IPv4ToDotted(res)).Labelled("IPv4Address");

        private static readonly Parser<char, int> h16 = HexNum.Where(res => res >= 0 && res <= 0xFFFF).Labelled("16 bits of IPv6 Address");
        private static readonly Parser<char, string> ls32 = 
            h16.Separated(Colon).Select(res => res.ToString()).Or(IPv4AddressOctets.Select(res => IPv4ToHex(res))).Labelled("32 least significant bits of IPv6 address");

        private static readonly Parser<char, string> IPv6WithoutShortening =
            from begin in h16.Select(res => res.ToString("X") + ":").Repeat(6)
            from rest in ls32
            select begin + rest;

        private static readonly Parser<char, string> IPv6First16BitsShortened =
            from doubleColon in DoubleColon
            from begin in h16.Select(res => res.ToString("X") + ":").Repeat(5)
            from rest in ls32
            select doubleColon + begin + rest;

        private static readonly Parser<char, string> IPv6Address =
            OneOf(new List<Parser<char, string>>() {
                IPv6WithoutShortening,
                IPv6First16BitsShortened
            }).Labelled("IPv6Address"); //TODO IPv6 address parsing

        private static readonly Parser<char, string> RegistredName = Unreserved.Or(PercentEncoding).Or(UriSubDelims).ManyString().Labelled("RegistredName");

        private static readonly Parser<char, string> IpLiteral = IpVFuture.Or(IPv6Address).Between(LeftBracket, RightBracket).Labelled("IpLiteral");
        private static readonly Parser<char, string> Host = IpLiteral.Or(IPv4Address).Or(RegistredName).Labelled("Host");

        private static readonly Parser<char, string> Authority =
            from userInfo in UserInfo.Labelled("Authority UserInfo")
            from atSign in AtSign.Labelled("Authority at sign")
            from host in Host.Labelled("Authority host")
            from colon in Colon.Labelled("Authority Colon")
            from port in DecimalNum.Labelled("Authority port")
            select userInfo + atSign.ToString() + host + colon + port.ToString();

        private static readonly Parser<char, char> pChar = 
            OneOf(new List<Parser<char, char>>() { Unreserved, PercentEncoding, UriSubDelims, Colon, AtSign}).Labelled("pchar");
        private static readonly Parser<char, string> PathAbempty = Slash.Then(pChar.ManyString()).ManyString().Select(res => "/" + res.ToString()).Labelled("Path AbEmpty");
        private static readonly Parser<char, string> SegmentNz = pChar.AtLeastOnce().Select(res => res.ToString()).Labelled("SegmentNz");

        private static readonly Parser<char, string> PathAbsoluteOptional =
            from segment in SegmentNz
            from pathAbempty in PathAbempty
            select segment + pathAbempty;

        private static readonly Parser<char, string> PathAbsolute =
            from slash in Slash
            from optional in PathAbsoluteOptional.Optional()
            select optional.HasValue ? optional.Value : "/";

        private static readonly Parser<char, string> PathRootless =
            from segmentNz in SegmentNz
            from pathAbempty in PathAbempty
            select segmentNz + PathAbempty;

        private static readonly Parser<char, string> PathEmpty = pChar.RepeatString(0).Labelled("Path empty");

        private static readonly Parser<char, string> Path = PathAbempty.Or(PathAbsolute).Or(PathRootless).Or(PathEmpty).Labelled("Path");

        private static readonly Parser<char, string> HierPart =
            from doubleSlash in DoubleSlash.Labelled("HierPart DoubleSlash")
            from authority in Authority.Labelled("HierPart Authority")
            from path in Path.Labelled("HierPart Path")
            select doubleSlash + authority + path;

        private static readonly Parser<char, string> Fragment = NumberSign.Then(pChar.Or(Slash).Or(QMark).ManyString()).Select(res => "#" + res).Labelled("Fragment");

        private static readonly Parser<char, double> Version = Http.Then(Slash).Then(Real).Labelled("Version");
        private static readonly Parser<char, string> ReasonPhrase = LetterOrDigit.Or(Space).Or(HTab).ManyString().Labelled("ReasonPhrase");
        private static readonly Parser<char, IStartLine> StatusLineParser =
           from version in Version.Labelled("Status line version")
           from firstSpace in Space.Labelled("Status line first space")
           from statusCode in DecimalNum.Labelled("Status line code")
           from secondSpace in Space.Labelled("Status line second space")
           from reasonPhrase in ReasonPhrase.Labelled("Status line reason phrase")
           from crlf in CRLF.Labelled("Status line crlf")
           select new StatusLine(version, statusCode, reasonPhrase) as IStartLine;

        private static readonly Parser<char, string> Method = Letter.AtLeastOnce().Select(res => res.ToString()).Labelled("Method");

        private static readonly Parser<char, string> AbsolutePath =
            from leadingSlash in Slash.Labelled("AbsolutePath slash")
            from absolutePath in LetterOrDigit.ManyString().Labelled("AbsolutePath")
            select leadingSlash.ToString() + absolutePath;

        //TODO add multiple queries chained with &
        private static readonly Parser<char, string> Query =
            from qMark in QMark.Labelled("Query QuestionMark")
            from name in LetterOrDigit.AtLeastOnce().Labelled("Query name")
            from equalsSign in EqualsSign.Labelled("Query equals")
            from value in LetterOrDigit.AtLeastOnce().Labelled("Query value")
            select qMark.ToString() + name + equalsSign + value;

        private static readonly Parser<char, string> OriginForm =
            from absolutePath in AbsolutePath.Labelled("OriginForm absolute path")
            from query in Query.Optional().Labelled("Optional query")
            select query.HasValue ? absolutePath + query.Value : absolutePath;

        private static readonly Parser<char, string> AbsoluteURI =
           from scheme in Scheme.Before(Colon).Labelled("AbsoluteUri scheme")
           from hierPart in HierPart.Labelled("AbsoluteUri hierPart")
           from query in Query.Optional().Labelled("AbsoluteUri query")
           from fragment in Fragment.Optional().Labelled("AbsoluteUri fragment")
           select FormatAbsoluteUriOutput(scheme, hierPart, query, fragment);

        private static readonly Parser<char, string> AsteriskForm = Asterisk.Between(Space).Labelled("Asterisk form");
        private static readonly Parser<char, string> RequestTarget = OriginForm.Or(AsteriskForm).Or(AbsoluteURI).Or(Authority).Labelled("Request target");

        private static readonly Parser<char, IStartLine> RequestLineParser =
            from method in Method.Labelled("RequestLine method")
            from requestTarget in RequestTarget.Between(SkipWhitespaces).Labelled("RequestLine request target")
            from version in Version.Labelled("RequestLine version")
            from crlf in CRLF.Labelled("RequestLine crlf")
            select new RequestLine(method, requestTarget, version) as IStartLine;

        private static readonly Parser<char, IStartLine> StartLineParser = StatusLineParser.Or(RequestLineParser).Labelled("StartLine");

        public static Result<char, IStartLine> Parse(string input) => StartLineParser.Parse(input);

        private static string IPv4ToHex(List<int> octets)
        {
            int top16 = octets[0] * 16 ^ 2 + octets[1];
            int low16 = octets[2] * 16 ^ 2 + octets[3];
            return top16.ToString("X") + ":" + low16.ToString("X");
        }

        private static string IPv4ToDotted(List<int> octets)
        {
            return $"{octets[0]}.{octets[1]}.{octets[2]}.{octets[3]}";
        }

        private static string FormatAbsoluteUriOutput(string scheme, string hierPart, Maybe<string> query, Maybe<string> fragment)
        {
            var baseUri = scheme + ":" + hierPart;
            if (query.HasValue)
            {
                if (fragment.HasValue)
                {
                    return baseUri + query.Value + fragment.Value;
                } else
                {
                    return baseUri + query.Value;
                }
            } else
            {
                return baseUri;
            }
        }
    }
}
