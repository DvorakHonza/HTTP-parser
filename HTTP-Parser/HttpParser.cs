using HTTP_Parser.HTTP;
using Pidgin;
using System.Collections.Generic;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;
using System.Linq;
using System;
using System.Text;
using System.Collections.Immutable;
using HTTP_Parser.HTTP.RequestTargets;

namespace HTTP_Parser
{
    public static class HttpParser
    {
        private static readonly char[] Delimiters = { '"', '(', ')', ',', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '{', '}' };
        private static readonly char[] TCharSpecialSymbols = { '!', '#', '$', '%', '&', '\'', '*', '+', '-', '.', '^', '_', '`', '|', '~' };
        private static readonly char[] UriSubcomponentDelimiters = { '!', '$', '&', '\'', '(', ')', '*', '+', ',', ';', '=' };
        private static readonly char[] VCharComplement = { '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0a', '\x0b', '\x0c',
                                                           '\x0d', '\x0e', '\x0f', '\x10', '\x11' ,'\x12', '\x13', '\x14', '\x15' ,'\x16', '\x17', '\x18', '\x19',
                                                           '\x1a', '\x1b', '\x1c', '\x1d', '\x1e', '\x1f', '\x20', '\x7f'};

        private static readonly Parser<char, char> Slash = Char('/');
        private static readonly Parser<char, char> Dot = Char('.');
        private static readonly Parser<char, char> Space = Char(' ');
        private static readonly Parser<char, char> HTab = Char('\t');
        private static readonly Parser<char, char> QuestionMark = Char('?');
        private static readonly Parser<char, char> EqualsSign = Char('=');
        private static readonly Parser<char, string> Http = String("HTTP");
        private static readonly Parser<char, string> Asterisk = String("*");
        private static readonly Parser<char, string> CRLF = String("\r\n");
        private static readonly Parser<char, char> AtSign = Char('@');
        private static readonly Parser<char, char> NumberSign = Char('#');
        private static readonly Parser<char, char> Delimiter = OneOf(Delimiters).Labelled("Delimiter");
        private static readonly Parser<char, char> TChar = OneOf(new List<Parser<char, char>>() { OneOf(TCharSpecialSymbols), LetterOrDigit }).Labelled("tchar");
        private static readonly Parser<char, char> UriSubDelims = OneOf(UriSubcomponentDelimiters).Labelled("UriSubDelims//");
        private static readonly Parser<char, char> Dash = Char('-');
        private static readonly Parser<char, char> Underscore = Char('_');
        private static readonly Parser<char, char> Tilde = Char('~');
        private static readonly Parser<char, char> Colon = Char(':');
        private static readonly Parser<char, char> ColonWhitespace = Colon.Between(SkipWhitespaces);
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
            from version in HexNum.Labelled("IpVFuture version")
            from dot in Dot.Labelled("IpVFuture dot")
            from end in OneOf(new List<Parser<char, char>>() { Unreserved, UriSubDelims, Colon }).AtLeastOnce().Labelled("IpVFuture end")
            select beginning.ToString() + version.ToString() + dot.ToString() + ConvertIEnumerableToString(end);

        private static readonly Parser<char, int> DecOctet = DecimalNum.Where(res => res >= 0 && res <= 255).Labelled("Decimal Octet");
        private static readonly Parser<char, List<int>> IPv4AddressOctets =
            from firstOctet in DecOctet.Before(Dot)
            from secondOctet in DecOctet.Before(Dot)
            from thirdOctet in DecOctet.Before(Dot)
            from fourthOctet in DecOctet.Before(Whitespace) //TODO What if ip address is not followed by whitespace
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

        private static readonly Parser<char, RequestTarget> Authority =
            from userInfo in UserInfo.Labelled("Authority UserInfo")
            from atSign in AtSign.Labelled("Authority at sign")
            from host in Host.Labelled("Authority host")
            from colon in Colon.Labelled("Authority Colon")
            from port in DecimalNum.Labelled("Authority port")
            select new AuthorityForm(userInfo + atSign.ToString() + host, port) as RequestTarget;

        private static readonly Parser<char, char> PChar =
            OneOf(new List<Parser<char, char>>() { Unreserved, PercentEncoding, UriSubDelims, Colon, AtSign }).Labelled("pchar");
        private static readonly Parser<char, string> PathAbempty = Slash.Then(PChar.ManyString()).ManyString().Select(res => "/" + res.ToString()).Labelled("Path AbEmpty");
        private static readonly Parser<char, string> SegmentNz = PChar.AtLeastOnce().Select(res => ConvertIEnumerableToString(res)).Labelled("SegmentNz");

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

        private static readonly Parser<char, string> PathEmpty = PChar.RepeatString(0).Labelled("Path empty");

        private static readonly Parser<char, string> Path = PathAbempty.Or(PathAbsolute).Or(PathRootless).Or(PathEmpty).Labelled("Path");

        private static readonly Parser<char, string> HierPart =
            from authority in DoubleSlash.Then(Authority).Labelled("HierPart Authority")
            from path in Path.Labelled("HierPart Path")
            select authority + path;

        private static readonly Parser<char, string> Fragment = NumberSign.Then(PChar.Or(Slash).Or(QuestionMark).ManyString()).Select(res => "#" + res).Labelled("Fragment");

        private static readonly Parser<char, double> Version = Http.Then(Slash).Then(Real).Labelled("Version");
        //TODO add obs-text
        private static readonly Parser<char, string> ReasonPhrase = LetterOrDigit.Or(Space).Or(HTab).ManyString().Labelled("ReasonPhrase");
        private static readonly Parser<char, StartLine> StatusLineParser =
           from version in Version.Labelled("Status line version")
           from firstSpace in Space.Labelled("Status line first space")
           from statusCode in DecimalNum.Labelled("Status line code")
           from secondSpace in Space.Labelled("Status line second space")
           from reasonPhrase in ReasonPhrase.Labelled("Status line reason phrase")
           from crlf in CRLF.Labelled("Status line crlf")
           select new StatusLine(version, statusCode, reasonPhrase) as StartLine;

        private static readonly Parser<char, string> Method = Letter.AtLeastOnce().Select(res => ConvertIEnumerableToString(res)).Labelled("Method");

        private static readonly Parser<char, string> AbsolutePath =
            from leadingSlash in Slash.Labelled("AbsolutePath slash")
            from absolutePath in LetterOrDigit.ManyString().Labelled("AbsolutePath")
            select leadingSlash.ToString() + absolutePath;

        private static readonly Parser<char, KeyValuePair<string, string>> Query =
            from name in LetterOrDigit.AtLeastOnce().Before(EqualsSign).Select(res => ConvertIEnumerableToString(res)).Labelled("Query name")
            from value in LetterOrDigit.AtLeastOnce().Select(res => ConvertIEnumerableToString(res)).Labelled("Query value")
            select new KeyValuePair<string, string>(name, value);

        private static readonly Parser<char, ImmutableDictionary<string, string>> Queries = 
            Query.Separated(Char('&')).Select(kvps => kvps.ToImmutableDictionary()).Labelled("Queries");

        private static readonly Parser<char, RequestTarget> OriginForm =
            from absolutePath in AbsolutePath.Labelled("OriginForm absolute path")
            from queries in Queries.Optional().Labelled("Optional query")
            select queries.HasValue ? 
                new OriginForm(absolutePath, queries.Value) as RequestTarget : 
                new OriginForm(absolutePath) as RequestTarget; 

        private static readonly Parser<char, RequestTarget> AbsoluteFormParser =
           from scheme in Scheme.Before(Colon).Labelled("AbsoluteUri scheme")
           from hierPart in HierPart.Labelled("AbsoluteUri hierPart")
           from queries in QuestionMark.Then(Queries).Optional().Labelled("AbsoluteUri query")
           from fragment in Fragment.Optional().Labelled("AbsoluteUri fragment")
           select FormatAbsoluteUriOutput(scheme, hierPart, queries, fragment) as RequestTarget;

        private static readonly Parser<char, RequestTarget> AsteriskFormParser = 
            Asterisk.Between(Space).Select(res => new AsteriskForm() as RequestTarget).Labelled("Asterisk form");
        private static readonly Parser<char, RequestTarget> RequestTargetParser = 
            OriginForm.Or(AsteriskFormParser).Or(AbsoluteFormParser).Or(Authority).Labelled("Request target");

        private static readonly Parser<char, StartLine> RequestLineParser =
            from method in Method.Labelled("RequestLine method")
            from requestTarget in RequestTargetParser.Between(SkipWhitespaces).Labelled("RequestLine request target")
            from version in Version.Labelled("RequestLine version")
            from crlf in CRLF.Labelled("RequestLine crlf")
            select new RequestLine(method, requestTarget, version) as StartLine;

        private static readonly Parser<char, StartLine> StartLineParser = StatusLineParser.Or(RequestLineParser).Labelled("StartLine");

        //Header fields
        private static readonly Parser<char, string> FieldName = TChar.AtLeastOnce().Select(res => ConvertIEnumerableToString(res)).Labelled("FieldName");

        //TODO add obs-text
        private static readonly Parser<char, char> FieldVChar = AnyCharExcept(VCharComplement).Labelled("FieldVChar");
        private static readonly Parser<char, string> FieldContentOptional =
            from space in Space.Or(HTab).Labelled("FieldContentOptional space")
            from trailingSpaces in Space.Or(HTab).ManyString().Labelled("FieldContentOptional trailingSpaces")
            from vchar in FieldVChar.Labelled("FieldContentOptional VChar")
            select space + trailingSpaces + vchar;
        private static readonly Parser<char, string> FieldContent =
            from begin in FieldVChar.Labelled("FieldContent begin")
            from rest in FieldContentOptional.Optional().Labelled("FieldContent rest")
            select rest.HasValue ? begin + rest.Value : begin.ToString();
        private static readonly Parser<char, string> ObsFold =
            from crlf in CRLF.Labelled("ObsFold crlf")
            from space in Space.Or(HTab).Labelled("ObsFold space")
            from trailingSpace in Space.Or(HTab).Labelled("ObsFold trailingSpace")
            select crlf + space + trailingSpace;
        private static readonly Parser<char, string> FieldValue = FieldContent.ManyString();//.Or(ObsFold).ManyString();
        private static readonly Parser<char, string> WhitespacesExceptCrLf = Char('\x09').Or(Char('\x0b')).Or(Char('\x0c')).ManyString();

        private static readonly Parser<char, KeyValuePair<string, string>> HeaderFieldParser =
            FieldName.Before(ColonWhitespace)
            .Then(FieldValue, (key, value) => new KeyValuePair<string, string>(key, value)).Before(CRLF);
        private static readonly Parser<char, ImmutableDictionary<string, string>> HeaderFields =
            HeaderFieldParser.Many().Select(kvps => kvps.ToImmutableDictionary());

        
        private static readonly Parser<char, HttpHeader> HttpHeaderParser =
            from startLine in StartLineParser
            from headerFields in HeaderFields
            from crlf in CRLF
            select new HttpHeader(startLine, headerFields);


        public static Result<char, HttpHeader> Parse(string input) => HttpHeaderParser.Parse(input);

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

        private static AbsoluteForm FormatAbsoluteUriOutput(string scheme, string hierPart, Maybe<ImmutableDictionary<string, string>> queries, Maybe<string> fragment)
        {
            if (queries.HasValue)
            {
                if (fragment.HasValue)
                {
                    return new AbsoluteForm(scheme, hierPart, queries.Value, fragment.Value);
                } else
                {
                    return new AbsoluteForm(scheme, hierPart, queries.Value);
                }
            } else
            {
                return new AbsoluteForm(scheme, hierPart);
            }
        }

        private static string ConvertIEnumerableToString<T>(IEnumerable<T> items)
        {
            var stringBuilder = new StringBuilder();
            foreach (var c in items)
            {
                stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }
    }
}
